using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using static HwidGetCurrentEx.Native;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// One collector per HWID class. Each returns its instance hashes de-duplicated and sorted
    /// ascending, which is exactly what the DLL's CSortedArray hands back.
    ///
    /// The hashed byte ranges are byte-for-byte what LicensingWinRT hashes; each collector says
    /// which buffer that is and why.
    /// </summary>
    internal static class HwidCollectors
    {
        public static List<ushort> Collect(HwidClass cls)
        {
            switch (cls)
            {
                // PnP classes: the device set is "every present devnode", filtered by class GUID.
                case HwidClass.Cdrom:       return CollectDeviceNodes(null, GUID_DEVINTERFACE.GUID_DEVCLASS_CDROM);
                case HwidClass.Hdc:         return CollectDeviceNodes(null, GUID_DEVINTERFACE.GUID_DEVCLASS_HDC);
                case HwidClass.Display:     return CollectDeviceNodes(null, GUID_DEVINTERFACE.GUID_DEVCLASS_DISPLAY);
                case HwidClass.ScsiAdapter: return CollectDeviceNodes(null, GUID_DEVINTERFACE.GUID_DEVCLASS_SCSIADAPTER);
                case HwidClass.Pcmcia:      return CollectDeviceNodes(null, GUID_DEVINTERFACE.GUID_DEVCLASS_PCMCIA);

                // Audio is the odd one: a KSCATEGORY_AUDIO device set, filtered to the MEDIA class.
                case HwidClass.Audio:
                    return CollectDeviceNodes(GUID_DEVINTERFACE.GUID_KSCATEGORY_AUDIO,
                                              GUID_DEVINTERFACE.GUID_DEVCLASS_MEDIA);

                case HwidClass.Hdd:             return CollectDisks();
                case HwidClass.Network:         return CollectNetwork();
                case HwidClass.Bluetooth:       return CollectBluetooth();
                case HwidClass.Cpu:             return CollectCpu();
                case HwidClass.Memory:          return CollectMemory();
                case HwidClass.Bios:            return CollectBios();
                case HwidClass.MobileBroadband: return CollectWwan();

                // LicensingWinRT's dock collector is a stub: `xor eax,eax; ret`. It contributes
                // nothing but the flag its class raises.
                case HwidClass.Dock:            return new List<ushort>();

                default:
                    throw new ArgumentOutOfRangeException(nameof(cls), cls, "unknown HWID class");
            }
        }

        #region result set

        /// <summary>Deduplicating accumulator - the moral equivalent of CSortedArray.</summary>
        private sealed class InstanceSet
        {
            private readonly List<ushort> _values = new List<ushort>();

            public void Add(ushort value)
            {
                if (!_values.Contains(value))
                    _values.Add(value);
            }

            public List<ushort> ToSortedList()
            {
                _values.Sort();
                return _values;
            }
        }

        #endregion

        #region enumerator plumbing

        /// <summary>
        /// Opens a device information set and fails loudly. Passing <paramref name="classGuid"/>
        /// as <see cref="IntPtr.Zero"/> means DIGCF_ALLCLASSES, where the class cannot be chosen.
        /// </summary>
        private static IntPtr OpenDeviceSet(IntPtr classGuid, int flags)
        {
            IntPtr set = SetupDiGetClassDevsW(classGuid, IntPtr.Zero, IntPtr.Zero, flags);
            if (!IsValidHandle(set))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiGetClassDevs");
            return set;
        }

        private static IntPtr OpenDeviceSet(ref Guid classGuid, int flags)
        {
            IntPtr set = SetupDiGetClassDevsW(ref classGuid, IntPtr.Zero, IntPtr.Zero, flags);
            if (!IsValidHandle(set))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiGetClassDevs");
            return set;
        }

        /// <summary>Walks a device information set; the caller owns the set.</summary>
        private static IEnumerable<SP_DEVINFO_DATA> EnumerateDevNodes(IntPtr deviceInfoSet)
        {
            for (uint index = 0; ; index++)
            {
                SP_DEVINFO_DATA devInfo = NewDevInfoData();

                if (!SetupDiEnumDeviceInfo(deviceInfoSet, index, ref devInfo))
                {
                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                        yield break;
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiEnumDeviceInfo");
                }

                yield return devInfo;
            }
        }

        /// <summary>A device interface that opened successfully, plus its removal policy.</summary>
        private struct OpenedDevice
        {
            public IntPtr Handle;
            public bool Readable;
        }

        /// <summary>
        /// Walks a device interface class and opens every non-software device, in the manner of
        /// CHwidPnPClassEnumeratorT::NextInterface + NextInterfaceDeviceHandle: software devices
        /// are skipped and a failed CreateFile simply moves on. The handle is closed for you when
        /// the loop advances.
        /// </summary>
        private static IEnumerable<OpenedDevice> OpenDeviceInterfaces(Guid interfaceClass,
                                                                     uint desiredAccess,
                                                                     uint shareMode)
        {
            Guid guid = interfaceClass;
            IntPtr set = OpenDeviceSet(ref guid, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            try
            {
                for (uint index = 0; ; index++)
                {
                    SP_DEVICE_INTERFACE_DATA interfaceData = NewInterfaceData();

                    if (!SetupDiEnumDeviceInterfaces(set, IntPtr.Zero, ref guid, index, ref interfaceData))
                    {
                        if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                            yield break;
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiEnumDeviceInterfaces");
                    }

                    string devicePath;
                    SP_DEVINFO_DATA devInfo;
                    if (!GetDeviceInterfaceDetail(set, ref interfaceData, out devicePath, out devInfo))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiGetDeviceInterfaceDetail");

                    if (IsSoftwareDevice(ref devInfo))
                        continue;

                    IntPtr handle = CreateFileW(devicePath, desiredAccess, shareMode,
                                                IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                    if (!IsValidHandle(handle))
                        continue;

                    try
                    {
                        yield return new OpenedDevice
                        {
                            Handle = handle,
                            Readable = GetRemovalPolicy(set, ref devInfo),
                        };
                    }
                    finally
                    {
                        CloseHandle(handle);
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(set);
            }
        }

        #endregion

        #region PnP helpers

        /// <summary>REG_SZ - the only property type IsSoftwareDevice accepts.</summary>
        private const uint REG_SZ = 1;

        /// <summary>wchar_t[208] in the DLL; it claims 402 bytes are available.</summary>
        private const int EnumeratorNameCapacity = 416;

        private const uint EnumeratorNameLength = 402;

        private static void EnsureSuccess(uint configRet, string what)
        {
            if (configRet != CONFIGRET_SUCCESS)
                throw new InvalidOperationException(what + " failed with CONFIGRET " + configRet);
        }

        /// <summary>
        /// Port of CHwidUtil::IsSoftwareDevice. A device counts as software only when
        /// CM_DRP_ENUMERATOR_NAME is a properly terminated REG_SZ equal to "SWD" (case
        /// insensitive), or - when the devnode is root enumerated - when its parent is exactly
        /// "HTREE\ROOT\0". Everything else is real hardware and gets collected.
        /// </summary>
        private static bool IsSoftwareDevice(ref SP_DEVINFO_DATA devInfo)
        {
            using (var buffer = new UnmanagedBuffer(EnumeratorNameCapacity))
            {
                uint length = EnumeratorNameLength;
                uint type;
                EnsureSuccess(CM_Get_DevNode_Registry_PropertyW(
                        devInfo.DevInst, CM_DRP_ENUMERATOR_NAME, out type, buffer.Pointer, ref length, 0),
                    "CM_Get_DevNode_Registry_Property(CM_DRP_ENUMERATOR_NAME)");

                if (type == REG_SZ && length >= 2 && (length & 1) == 0)
                {
                    string enumerator = buffer.ReadUnicodeZ(0);
                    if (enumerator != null &&
                        string.Equals(enumerator, "SWD", StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                uint status;
                uint problem;
                EnsureSuccess(CM_Get_DevNode_Status(out status, out problem, devInfo.DevInst, 0),
                    "CM_Get_DevNode_Status");

                if ((status & DN_ROOT_ENUMERATED) == 0)
                    return false;

                uint parent;
                EnsureSuccess(CM_Get_Parent(out parent, devInfo.DevInst, 0), "CM_Get_Parent");

                string parentId;
                EnsureSuccess(GetDeviceId(parent, 0xC9, out parentId), "CM_Get_Device_ID");

                return string.Equals(parentId, @"HTREE\ROOT\0", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Port of CHwidUtil::HwidGetPnPRemovalPolicy. The comparison against 3 is UNSIGNED in
        /// the DLL, which changes the answer when the property holds 0.
        /// </summary>
        private static bool GetRemovalPolicy(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfo)
        {
            using (var buffer = new UnmanagedBuffer(4))
            {
                uint requiredSize;
                uint type;

                if (SetupDiGetDeviceRegistryPropertyW(deviceInfoSet, ref devInfo,
                        SPDRP_REMOVAL_POLICY, out type, buffer.Pointer, 4, out requiredSize))
                {
                    int value = buffer.ReadInt32(0);
                    if (value != 1)
                        return unchecked((uint)(value - 2)) > 3;
                }

                return true;
            }
        }

        /// <summary>
        /// Port of CHwidUtil::HwidGetPnPHardwareIdProperty. SPDRP_HARDWAREID is a REG_MULTI_SZ;
        /// null means the property is simply absent (the DLL's "rc == 1" case).
        /// </summary>
        private static byte[] GetHardwareIdProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfo)
        {
            const int capacity = 0x800;

            using (var buffer = new UnmanagedBuffer(capacity))
            {
                uint requiredSize;
                uint type;

                if (SetupDiGetDeviceRegistryPropertyW(deviceInfoSet, ref devInfo,
                        SPDRP_HARDWAREID, out type, buffer.Pointer, capacity, out requiredSize))
                    return buffer.ToArray((int)requiredSize);

                int error = Marshal.GetLastWin32Error();
                if (error == ERROR_INVALID_DATA || error == ERROR_NOT_FOUND)
                    return null;

                throw new Win32Exception(error, "SetupDiGetDeviceRegistryProperty(SPDRP_HARDWAREID)");
            }
        }

        /// <summary>WCHARs before the first NUL; throws if the buffer never terminates.</summary>
        private static int WideLength(byte[] property)
        {
            for (int i = 0; i < property.Length / 2; i++)
            {
                if (property[2 * i] == 0 && property[2 * i + 1] == 0)
                    return i;
            }

            throw new InvalidOperationException("SPDRP_HARDWAREID is not NUL terminated.");
        }

        #endregion

        #region PnP collectors

        /// <summary>
        /// CHwidPnPDataCollector&lt;N,0&gt;::CollectInternal (and the audio adaptor variant).
        /// Hashes the first string of the device's SPDRP_HARDWAREID multi-string INCLUDING its
        /// terminating NUL, i.e. 2 * wcslen + 2 bytes.
        ///
        /// <paramref name="deviceSetClass"/> chooses how the device set is built: null means
        /// DIGCF_ALLCLASSES (every present devnode, as the PnP collectors do), a value means a
        /// device interface class (as the audio adaptor does). <paramref name="classFilter"/> is
        /// always a device *class* GUID - for audio those two are different namespaces, and
        /// comparing the interface GUID instead silently widens the result set.
        /// </summary>
        private static List<ushort> CollectDeviceNodes(Guid? deviceSetClass, Guid classFilter)
        {
            var hashes = new InstanceSet();

            IntPtr set;
            if (deviceSetClass.HasValue)
            {
                Guid setClass = deviceSetClass.Value;
                set = OpenDeviceSet(ref setClass, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            }
            else
            {
                set = OpenDeviceSet(IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            }

            try
            {
                foreach (SP_DEVINFO_DATA node in EnumerateDevNodes(set))
                {
                    // A local copy: the ref-taking helpers cannot take a foreach variable.
                    SP_DEVINFO_DATA devInfo = node;

                    if (devInfo.ClassGuid != classFilter || IsSoftwareDevice(ref devInfo))
                        continue;

                    byte[] hardwareId = GetHardwareIdProperty(set, ref devInfo);
                    if (hardwareId == null)
                        continue;

                    int cbSize = 2 * WideLength(hardwareId) + 2;
                    hashes.Add(HWID.AddInstanceHash(hardwareId, 0, cbSize,
                                                    GetRemovalPolicy(set, ref devInfo)));
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(set);
            }

            return hashes.ToSortedList();
        }

        /// <summary>
        /// CHwidHddDataCollector / EnumInterfaces&lt;CHwidHddDataCollector&gt;. The hashed blob is
        /// "vendor\0product\0serial\0" straight out of the STORAGE_DEVICE_DESCRIPTOR, with a bare
        /// NUL standing in for any field whose offset is absent. No trimming, no emptiness check,
        /// and the policy handed to AddInstanceHash is hard coded to 1.
        /// </summary>
        private static List<ushort> CollectDisks()
        {
            const int outCapacity = 0x400;
            var hashes = new InstanceSet();

            foreach (OpenedDevice device in OpenDeviceInterfaces(
                         GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK, 0, 3))
            {
                using (var query = new UnmanagedBuffer(12))          // already zero: PropertyId
                using (var output = new UnmanagedBuffer(outCapacity)) // QueryType, AdditionalParams
                {
                    uint bytesReturned;
                    bool ok = DeviceIoControl(device.Handle, IOCTL_STORAGE_QUERY_PROPERTY,
                                              query.Pointer, query.Size,
                                              output.Pointer, output.Size,
                                              out bytesReturned, IntPtr.Zero);

                    // The DLL parses the descriptor only on success and only when it got back at
                    // least a full STORAGE_DEVICE_DESCRIPTOR.
                    if (!ok || bytesReturned < STORAGE_DEVICE_DESCRIPTOR_SIZE)
                        continue;

                    byte[] raw = output.ToArray((int)bytesReturned);
                    var descriptor = output.ToStructure<STORAGE_DEVICE_DESCRIPTOR>();

                    var blob = new List<byte>();
                    AppendDescriptorString(raw, descriptor.VendorIdOffset, blob);
                    AppendDescriptorString(raw, descriptor.ProductIdOffset, blob);
                    AppendDescriptorString(raw, descriptor.SerialNumberOffset, blob);

                    hashes.Add(HWID.AddInstanceHash(blob.ToArray(), 0, blob.Count, true));
                }
            }

            return hashes.ToSortedList();
        }

        /// <summary>
        /// EnumInterfaces&lt;CHwidNetworkDataCollector&gt;. A MAC whose bytes 2..5 spell " RAS"
        /// (the async RAS adapter) and an all-zero MAC are both dropped before hashing.
        /// </summary>
        private static List<ushort> CollectNetwork()
        {
            const uint rasMarker = 0x53415220;   // bytes 20 52 41 53 == " RAS"
            var hashes = new InstanceSet();

            foreach (OpenedDevice device in OpenDeviceInterfaces(
                         GUID_DEVINTERFACE.GUID_NDIS_LAN_CLASS, GENERIC_READ, 1))
            {
                using (var query = new UnmanagedBuffer(4))
                using (var output = new UnmanagedBuffer(6))
                {
                    Marshal.WriteInt32(query.Pointer, unchecked((int)OID_802_3_PERMANENT_ADDRESS));

                    uint bytesReturned;
                    if (!DeviceIoControl(device.Handle, IOCTL_NDIS_QUERY_GLOBAL_STATS,
                                         query.Pointer, query.Size,
                                         output.Pointer, output.Size,
                                         out bytesReturned, IntPtr.Zero))
                        continue;
                    if (bytesReturned != 6)
                        continue;

                    byte[] mac = output.ToArray();
                    if (BitConverter.ToUInt32(mac, 2) == rasMarker)
                        continue;
                    if (BitConverter.ToUInt32(mac, 0) == 0 && BitConverter.ToUInt16(mac, 4) == 0)
                        continue;

                    hashes.Add(HWID.AddInstanceHash(mac, 0, mac.Length, device.Readable));
                }
            }

            return hashes.ToSortedList();
        }

        /// <summary>
        /// EnumInterfaces&lt;CHwidBluetoothDataCollector&gt;. Every local radio contributes its
        /// address as "%04x%08x" in UTF-16 - twelve characters plus a NUL, so 26 bytes. It does
        /// not stop at the first one.
        /// </summary>
        private static List<ushort> CollectBluetooth()
        {
            const int localInfoSize = 0x124;
            var hashes = new InstanceSet();

            foreach (OpenedDevice device in OpenDeviceInterfaces(
                         GUID_DEVINTERFACE.GUID_BTHPORT_DEVICE_INTERFACE, GENERIC_READ, 1))
            {
                using (var output = new UnmanagedBuffer(localInfoSize))
                {
                    uint bytesReturned;
                    if (!DeviceIoControl(device.Handle, IOCTL_BTH_GET_LOCAL_INFO,
                                         IntPtr.Zero, 0,
                                         output.Pointer, output.Size,
                                         out bytesReturned, IntPtr.Zero))
                        continue;
                    if (bytesReturned != localInfoSize)
                        continue;

                    byte[] info = output.ToArray();
                    string address = BitConverter.ToUInt16(info, 0xC).ToString("x4") +
                                     BitConverter.ToUInt32(info, 0x8).ToString("x8");

                    byte[] hashed = Encoding.Unicode.GetBytes(address + "\0");
                    hashes.Add(HWID.AddInstanceHash(hashed, 0, hashed.Length, device.Readable));
                }
            }

            return hashes.ToSortedList();
        }

        #endregion

        #region CPU

        /// <summary>HwidCPUDataCollector::Collect - 20 bytes built from CPUID leaves 0 and 1.</summary>
        private static List<ushort> CollectCpu()
        {
            var hashes = new InstanceSet();

            byte[] leaf0 = CPUID.Invoke(0);
            byte[] leaf1 = CPUID.Invoke(1);

            // Vendor id as the leaf-0 registers read EBX, EDX, ECX, then leaf 1 EAX/EBX masked.
            var buffer = new byte[20];
            Buffer.BlockCopy(leaf0, 4, buffer, 0, 4);
            Buffer.BlockCopy(leaf0, 12, buffer, 4, 4);
            Buffer.BlockCopy(leaf0, 8, buffer, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(BitConverter.ToUInt32(leaf1, 0) & 0xFFFFFFF0u), 0, buffer, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(BitConverter.ToUInt32(leaf1, 4) & 0x00FFFFFFu), 0, buffer, 16, 4);

            hashes.Add(HWID.AddInstanceHash(buffer, 0, buffer.Length, true));
            return hashes.ToSortedList();
        }

        #endregion

        #region Memory

        /// <summary>
        /// The non-linear bucket thresholds from 0x1800B1B70, in bytes. They are 64 bit values;
        /// reading them as 32 bit pairs sends every machine with more than 3 GB down the
        /// fall-through branch - which happens to agree - but machines below that disagree.
        /// </summary>
        private static readonly ulong[] MemoryStepups =
        {
            0x00000000UL, 0x10000000UL, 0x20000000UL, 0x40000000UL,
            0x60000000UL, 0x80000000UL, 0xC0000000UL, 0x40000000UL,
        };

        private static List<ushort> CollectMemory()
        {
            var hashes = new InstanceSet();

            var status = new MEMORYSTATUSEX();
            status.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            if (!GlobalMemoryStatusEx(ref status))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GlobalMemoryStatusEx");

            ulong physicalKb = status.ullTotalPhys >> 10;

            ulong installedKb;
            if (!SMBIOS.TryGetInstalledMemorySize(out installedKb) || installedKb < physicalKb)
                installedKb = physicalKb;

            if (installedKb == 0)
                throw new InvalidOperationException("Unable to determine the installed memory size.");

            ulong bytes = installedKb << 10;

            int bucket = Array.FindIndex(MemoryStepups, threshold => bytes <= threshold);
            if (bucket < 0)
                bucket = (int)(8 * ((bytes - 0xC0000000UL) >> 30) + 7);

            hashes.Add(HWID.AddInstanceHash(BitConverter.GetBytes(bucket), 0, 4, true));
            return hashes.ToSortedList();
        }

        #endregion

        #region BIOS

        /// <summary>
        /// CHwidBiosDataCollector::Collect. The blob is
        ///   UUID(16, raw) + Manufacturer + ProductName + SerialNumber + BiosVendor
        /// with each field picked by its SMBIOS string INDEX (type 1 offsets 4/5/7, type 0 offset
        /// 4) rather than by its position in the string table. When SMBIOS cannot be read the DLL
        /// hashes a null buffer, which yields the value 0.
        /// </summary>
        private static List<ushort> CollectBios()
        {
            var hashes = new InstanceSet();
            byte[] blob = SMBIOS.GetBiosBlob();

            hashes.Add(blob == null
                ? HWID.AddInstanceHash(null, 0, 0, true)
                : HWID.AddInstanceHash(blob, 0, blob.Length, true));

            return hashes.ToSortedList();
        }

        #endregion

        #region WWAN

        /// <summary>
        /// CHwidMobileBroadbandDataCollector::Collect. wwapi.dll exports WwanQueryInterface
        /// (there is no WlanQueryInterface) and all five entry points must resolve. There is a
        /// single pass over the interface list: each 36 byte window at offset 0x28C of the query
        /// result is hashed unless the DWORD at 0x388 is set, and 0x228 decides readability.
        /// ERROR_SERVICE_NOT_ACTIVE means "no WWAN hardware" and yields no instances, not an error.
        /// </summary>
        private static List<ushort> CollectWwan()
        {
            var hashes = new InstanceSet();

            using (var api = new WwanApi())
            {
                if (api.Init() != 0)
                    return hashes.ToSortedList();

                int negotiatedVersion;
                IntPtr client;
                if (api.OpenHandle(1, IntPtr.Zero, out negotiatedVersion, out client) != 0)
                    return hashes.ToSortedList();

                try
                {
                    IntPtr interfaces;
                    // A non-zero result also covers ERROR_SERVICE_NOT_ACTIVE, which just means
                    // "this machine has no WWAN stack" - not an error worth reporting.
                    if (api.EnumerateInterfaces(client, 0, out interfaces) != 0)
                        return hashes.ToSortedList();

                    try
                    {
                        CollectWwanInterfaces(api, client, interfaces, hashes);
                    }
                    finally
                    {
                        api.FreeMemory(interfaces);
                    }
                }
                finally
                {
                    api.CloseHandle(client, IntPtr.Zero);
                }
            }

            return hashes.ToSortedList();
        }

        /// <summary>
        /// One pass over the interface list. A failing query abandons the rest of the list but
        /// keeps whatever was already collected, which is what the DLL does.
        /// </summary>
        private static void CollectWwanInterfaces(WwanApi api, IntPtr client, IntPtr interfaces, InstanceSet hashes)
        {
            int count = Marshal.ReadInt32(interfaces);

            for (int i = 0; i < count; i++)
            {
                IntPtr entry = new IntPtr(interfaces.ToInt64() + 4 + (long)i * WwanApi.InterfaceStride);
                var interfaceGuid = (Guid)Marshal.PtrToStructure(entry, typeof(Guid));

                int dataSize;
                IntPtr data;
                int opcodeType;
                if (api.QueryInterface(client, interfaceGuid, 7, IntPtr.Zero,
                                       out dataSize, out data, out opcodeType) != 0)
                    return;

                try
                {
                    if (Marshal.ReadInt32(new IntPtr(data.ToInt64() + WwanApi.SkipFlagOffset)) != 0)
                        continue;

                    bool readable = Marshal.ReadInt32(new IntPtr(data.ToInt64() + WwanApi.ReadableOffset)) == 1;

                    var window = new byte[WwanApi.AddressWindowSize];
                    Marshal.Copy(new IntPtr(data.ToInt64() + WwanApi.AddressOffset), window, 0, window.Length);

                    hashes.Add(HWID.AddInstanceHash(window, 0, window.Length, readable));
                }
                finally
                {
                    api.FreeMemory(data);
                }
            }
        }

        #endregion
    }
}
