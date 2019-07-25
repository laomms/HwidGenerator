using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// Raw SMBIOS access, mirroring CSmBiosInformation. Structures come from
    /// GetSystemFirmwareTable('RSMB') and, when that fails, from the WMI
    /// MSSmBios_RawSMBiosTables class.
    ///
    /// Every failure path returns null / false, which is what the DLL does too: it ignores the
    /// error, hashes a NULL buffer and ends up with the instance hash 0.
    /// </summary>
    internal static class SMBIOS
    {
        private const byte EndOfTable = 0x7F;

        private struct Structure
        {
            public byte Type;
            public int Offset;   // offset of the formatted area inside the table
            public int Length;   // length of the formatted area
            public byte[] Table;
        }

        #region raw table

        /// <summary>
        /// The structure area of the SMBIOS table, or null. GetSystemFirmwareTable prepends an
        /// 8 byte RawSMBIOSData header that is stripped here; the WMI fallback has no header.
        /// </summary>
        private static byte[] GetStructureArea()
        {
            byte[] fromFirmware = TryGetFirmwareTable();
            if (fromFirmware != null)
                return fromFirmware;

            return TryGetWmiTable();
        }

        private static byte[] TryGetFirmwareTable()
        {
            try
            {
                uint size = Native.GetSystemFirmwareTable(Native.FIRMWARE_TABLE_RSMB, 0, IntPtr.Zero, 0);
                if (size < 8)
                    return null;

                IntPtr buffer = Marshal.AllocHGlobal((int)size);
                try
                {
                    uint read = Native.GetSystemFirmwareTable(Native.FIRMWARE_TABLE_RSMB, 0, buffer, size);
                    if (read < 8)
                        return null;

                    var raw = new byte[read];
                    Marshal.Copy(buffer, raw, 0, (int)read);

                    // RawSMBIOSData: Used20CallingMethod, Major, Minor, DmiRevision, DWORD Length
                    uint length = BitConverter.ToUInt32(raw, 4);
                    if (length != read - 8)
                        return null;

                    var area = new byte[length];
                    Buffer.BlockCopy(raw, 8, area, 0, (int)length);
                    return area;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] TryGetWmiTable()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                           "root\\wmi", "SELECT * FROM MSSmBios_RawSMBiosTables"))
                {
                    foreach (ManagementBaseObject obj in searcher.Get())
                    {
                        using (obj)
                        {
                            var data = obj["SMBiosData"] as byte[];
                            if (data != null && data.Length > 0)
                                return data;
                        }
                    }
                }
            }
            catch
            {
                // No WMI / no firmware table - the caller treats this as "no SMBIOS".
            }

            return null;
        }

        #endregion

        #region structure walking

        private static IEnumerable<Structure> EnumerateStructures(byte[] table)
        {
            int position = 0;

            while (position + 4 <= table.Length)
            {
                byte type = table[position];
                int length = table[position + 1];

                if (length < 4 || position + length > table.Length)
                    yield break;

                int end = position + length;

                // String area: NUL terminated strings up to the double NUL.
                int scan = end;
                while (scan + 1 < table.Length && !(table[scan] == 0 && table[scan + 1] == 0))
                    scan++;

                yield return new Structure { Type = type, Offset = position, Length = length, Table = table };

                if (type == EndOfTable)
                    yield break;

                position = scan + 2;
            }
        }

        /// <summary>
        /// SMBIOS strings are 1-based; index 0 means "no string". An index past the end of the
        /// string area is an error in the DLL (0x8007000D), which makes the whole BIOS hash 0.
        /// </summary>
        private static string GetString(Structure s, byte index)
        {
            if (index == 0)
                return string.Empty;

            byte[] table = s.Table;
            int position = s.Offset + s.Length;

            for (int current = 1; ; current++)
            {
                if (position + 1 >= table.Length || (table[position] == 0 && table[position + 1] == 0))
                    throw new InvalidOperationException("SMBIOS string index " + index + " is out of range.");

                int end = position;
                while (end < table.Length && table[end] != 0)
                    end++;

                if (current == index)
                    return System.Text.Encoding.Default.GetString(table, position, end - position);

                position = end + 1;
            }
        }

        #endregion

        #region BIOS

        /// <summary>
        /// The exact blob CHwidBiosDataCollector hashes:
        ///   UUID(16 raw bytes at type 1 offset 8)
        ///   + Manufacturer  (type 1, string index at offset 4)
        ///   + ProductName   (type 1, offset 5)
        ///   + SerialNumber  (type 1, offset 7)
        ///   + BiosVendor    (type 0, offset 4)  -- appended last, after every type 1 field
        /// No separators and no terminators inside the hashed range. Returns null when the DLL
        /// would have ended up with a NULL buffer (nothing collected, or a malformed table).
        /// </summary>
        public static byte[] GetBiosBlob()
        {
            try
            {
                byte[] table = GetStructureArea();
                if (table == null)
                    return null;

                var systemInformation = new List<byte>();
                var biosInformation = new List<byte>();
                int seen = 0;   // bit 0 = type 0 seen, bit 1 = type 1 seen

                foreach (Structure s in EnumerateStructures(table))
                {
                    if (seen == 3)
                        break;

                    if (s.Type == 1)
                    {
                        if (s.Length >= 24)
                        {
                            for (int i = 0; i < 16; i++)
                                systemInformation.Add(s.Table[s.Offset + 8 + i]);
                        }

                        if (s.Length < 8)
                            return null;

                        systemInformation.AddRange(Bytes(GetString(s, s.Table[s.Offset + 4])));
                        systemInformation.AddRange(Bytes(GetString(s, s.Table[s.Offset + 5])));
                        systemInformation.AddRange(Bytes(GetString(s, s.Table[s.Offset + 7])));
                        seen |= 2;
                    }
                    else if (s.Type == 0)
                    {
                        if (s.Length < 5)
                            return null;

                        biosInformation.AddRange(Bytes(GetString(s, s.Table[s.Offset + 4])));
                        seen |= 1;
                    }
                }

                if (systemInformation.Count + biosInformation.Count == 0)
                    return null;

                systemInformation.AddRange(biosInformation);
                return systemInformation.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static byte[] Bytes(string value)
        {
            // SMBIOS strings are single byte; the DLL copies them verbatim.
            return System.Text.Encoding.Default.GetBytes(value);
        }

        #endregion

        #region installed memory

        private struct MemoryArray
        {
            public ushort Handle;
            public byte Use;
        }

        private struct MemoryDevice
        {
            public ushort ArrayHandle;
            public ushort Size;
            public uint ExtendedSize;
        }

        /// <summary>
        /// Port of CSmBiosInformation::GetInstalledMemorySize. Only type 17 devices whose
        /// physical array (type 16) reports Use == 3 ("system memory") count towards the total,
        /// and the 0x7FFF / 0x8000..0xFFFF sentinels and the Extended Size field at offset 28
        /// all have to be honoured. The result is in kilobytes.
        /// </summary>
        public static bool TryGetInstalledMemorySize(out ulong kilobytes)
        {
            kilobytes = 0;

            try
            {
                byte[] table = GetStructureArea();
                if (table == null)
                    return false;

                var arrays = new List<MemoryArray>();
                var devices = new List<MemoryDevice>();

                foreach (Structure s in EnumerateStructures(table))
                {
                    if (s.Type == 0x10)
                    {
                        if (s.Length < 6)
                            return false;

                        arrays.Add(new MemoryArray
                        {
                            Handle = BitConverter.ToUInt16(s.Table, s.Offset + 2),
                            Use = s.Table[s.Offset + 5],
                        });
                    }
                    else if (s.Type == 0x11)
                    {
                        if (s.Length < 14)
                            return false;

                        devices.Add(new MemoryDevice
                        {
                            ArrayHandle = BitConverter.ToUInt16(s.Table, s.Offset + 4),
                            Size = BitConverter.ToUInt16(s.Table, s.Offset + 12),
                            ExtendedSize = s.Length >= 32 ? BitConverter.ToUInt32(s.Table, s.Offset + 28) : 0u,
                        });
                    }
                }

                if (devices.Count == 0)
                    return false;

                ulong total = 0;
                foreach (MemoryDevice device in devices)
                {
                    // The DLL keeps the type 16 array sorted by handle purely so it can binary
                    // search it; the answer is the same either way.
                    int index = arrays.FindIndex(a => a.Handle == device.ArrayHandle);
                    if (index < 0)
                        return false;

                    if (arrays[index].Use != 3)   // 3 == "system memory"
                        continue;

                    ulong size = device.Size;

                    if (size == 0x7FFF)
                    {
                        size = device.ExtendedSize;
                        if (size == 0 || size > 0x7FFFFFFFUL)
                            return false;
                        size <<= 10;
                    }
                    else if (size >= 0x8000)
                    {
                        if (size == 0xFFFF || device.ExtendedSize != 0)
                            return false;
                        // 0x8000..0xFFFE is taken verbatim, with no scaling.
                    }
                    else
                    {
                        if (device.ExtendedSize != 0)
                            return false;
                        size <<= 10;
                    }

                    ulong next = total + size;
                    if (next < total)
                        return false;

                    total = next;
                }

                kilobytes = total;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
