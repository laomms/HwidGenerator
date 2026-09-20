using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// The whole Win32 surface this assembly needs. Declared by hand on purpose: the tool ships
    /// as a single Costura-embedded exe, and a P/Invoke wrapper library would drag a dozen extra
    /// assemblies (System.Memory, Vanara.Core, ...) plus binding redirects along with it.
    ///
    /// Everything here mirrors what LicensingWinRT.dll itself calls, including the awkward
    /// details: SetupDiEnumDeviceInfo needs a zeroed SP_DEVINFO_DATA with cbSize preset,
    /// SP_DEVICE_INTERFACE_DETAIL_DATA has a 4/8 byte cbSize that depends on the bitness, and
    /// the CM_* calls from cfgmgr32 return CONFIGRET, not a Win32 error.
    /// </summary>
    internal static class Native
    {
        // ---------------------------------------------------------------- IOCTLs
        public const uint IOCTL_STORAGE_QUERY_PROPERTY  = 0x002D1400;
        public const uint IOCTL_NDIS_QUERY_GLOBAL_STATS = 0x00170002;
        public const uint IOCTL_BTH_GET_LOCAL_INFO      = 0x00410000;

        /// <summary>OID_802_3_PERMANENT_ADDRESS - the only NDIS OID the collector asks for.</summary>
        public const uint OID_802_3_PERMANENT_ADDRESS   = 0x01010101;

        /// <summary>'RSMB' firmware table provider signature.</summary>
        public const uint FIRMWARE_TABLE_RSMB           = 0x52534D42;

        // ------------------------------------------------------------- constants
        public const int  ERROR_NOT_FOUND            = 2;
        public const int  ERROR_INVALID_DATA         = 13;
        public const int  ERROR_NO_MORE_ITEMS        = 259;

        public const uint GENERIC_READ   = 0x80000000;
        public const uint OPEN_EXISTING  = 3;

        /// <summary>SetupDiGetClassDevs flags.</summary>
        public const int DIGCF_PRESENT         = 0x02;
        public const int DIGCF_ALLCLASSES      = 0x04;
        public const int DIGCF_DEVICEINTERFACE = 0x10;

        /// <summary>SPDRP_HARDWAREID - a REG_MULTI_SZ; the property the PnP collectors hash.</summary>
        public const uint SPDRP_HARDWAREID = 0x01;

        /// <summary>Raw property 0x1F, the one HwidGetPnPRemovalPolicy reads.</summary>
        public const uint SPDRP_REMOVAL_POLICY = 0x1F;

        /// <summary>CM_DRP_ENUMERATOR_NAME - how IsSoftwareDevice spots "SWD" devices.</summary>
        public const uint CM_DRP_ENUMERATOR_NAME = 0x17;

        /// <summary>DN_ROOT_ENUMERATED - bit 0 of the CM_Get_DevNode_Status status word.</summary>
        public const uint DN_ROOT_ENUMERATED = 0x1;

        public const uint CONFIGRET_SUCCESS = 0;

        public const int STORAGE_DEVICE_DESCRIPTOR_SIZE = 36;

        // ------------------------------------------------------------ structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_DEVICE_DESCRIPTOR
        {
            public uint Version;
            public uint Size;
            public byte DeviceType;
            public byte DeviceTypeModifier;
            public byte RemovableMedia;
            public byte CommandQueueing;
            public uint VendorIdOffset;
            public uint ProductIdOffset;
            public uint ProductRevisionOffset;
            public uint SerialNumberOffset;
            public byte BusType;
            public uint RawPropertiesLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        // ------------------------------------------------------------- setupapi
        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevsW(IntPtr classGuid, IntPtr enumerator,
                                                         IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevsW(ref Guid classGuid, IntPtr enumerator,
                                                         IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, uint memberIndex,
                                                        ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData,
                                                              ref Guid interfaceClassGuid, uint memberIndex,
                                                              ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr deviceInfoSet,
                                                                   ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                                   IntPtr deviceInterfaceDetailData,
                                                                   uint deviceInterfaceDetailDataSize,
                                                                   out uint requiredSize,
                                                                   IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr deviceInfoSet,
                                                                    ref SP_DEVINFO_DATA deviceInfoData,
                                                                    uint property, out uint propertyRegDataType,
                                                                    IntPtr propertyBuffer, uint propertyBufferSize,
                                                                    out uint requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        // ------------------------------------------------------------- cfgmgr32
        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint CM_Get_DevNode_Registry_PropertyW(uint devInst, uint property,
                                                                    out uint regDataType, IntPtr buffer,
                                                                    ref uint length, uint flags);

        [DllImport("cfgmgr32.dll")]
        public static extern uint CM_Get_DevNode_Status(out uint status, out uint problemNumber,
                                                        uint devInst, uint flags);

        [DllImport("cfgmgr32.dll")]
        public static extern uint CM_Get_Parent(out uint parentDevInst, uint devInst, uint flags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint CM_Get_Device_IDW(uint devInst, IntPtr buffer, uint bufferLen, uint flags);

        // --------------------------------------------------------------- kernel
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateFileW(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
                                                IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                                uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer,
                                                  int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize,
                                                  out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", EntryPoint = "GetSystemFirmwareTable", SetLastError = true)]
        public static extern uint GetSystemFirmwareTable(uint FirmwareTableProviderSignature,
                                                         uint FirmwareTableID, IntPtr pFirmwareTableBuffer,
                                                         uint BufferSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        // --------------------------------------------------------------- helpers
        public static bool IsValidHandle(IntPtr h)
        {
            return h != IntPtr.Zero && h != new IntPtr(-1);
        }

        public static SP_DEVINFO_DATA NewDevInfoData()
        {
            return new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA)) };
        }

        public static SP_DEVICE_INTERFACE_DATA NewInterfaceData()
        {
            return new SP_DEVICE_INTERFACE_DATA { cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA)) };
        }

        /// <summary>
        /// The two call SetupDiGetDeviceInterfaceDetail dance, done the way the DLL does it:
        /// ask for the required size, then hand back a buffer whose first DWORD is the
        /// bitness dependent cbSize and whose path starts four bytes in.
        /// </summary>
        public static bool GetDeviceInterfaceDetail(IntPtr deviceInfoSet,
                                                    ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                    out string devicePath,
                                                    out SP_DEVINFO_DATA deviceInfoData)
        {
            devicePath = null;
            deviceInfoData = NewDevInfoData();

            uint requiredSize;
            SetupDiGetDeviceInterfaceDetailW(deviceInfoSet, ref deviceInterfaceData,
                                             IntPtr.Zero, 0, out requiredSize, IntPtr.Zero);

            // The DLL rejects anything that cannot even hold cbSize + a NUL.
            if (requiredSize < 8)
                return false;

            IntPtr detail = Marshal.AllocHGlobal((int)requiredSize);
            int devInfoBytes = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            IntPtr info = Marshal.AllocHGlobal(devInfoBytes);
            try
            {
                Marshal.WriteInt32(detail, IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize);
                Marshal.Copy(new byte[devInfoBytes], 0, info, devInfoBytes);
                Marshal.WriteInt32(info, 0, devInfoBytes);

                if (!SetupDiGetDeviceInterfaceDetailW(deviceInfoSet, ref deviceInterfaceData,
                                                      detail, requiredSize, out requiredSize, info))
                    return false;

                devicePath = Marshal.PtrToStringUni(new IntPtr(detail.ToInt64() + 4));
                deviceInfoData = (SP_DEVINFO_DATA)Marshal.PtrToStructure(info, typeof(SP_DEVINFO_DATA));
                return devicePath != null;
            }
            finally
            {
                Marshal.FreeHGlobal(detail);
                Marshal.FreeHGlobal(info);
            }
        }

        /// <summary>Wraps CM_Get_Device_IDW with a correctly sized buffer.</summary>
        public static uint GetDeviceId(uint devInst, int maxChars, out string deviceId)
        {
            deviceId = null;

            int bytes = maxChars * 2;
            IntPtr buffer = Marshal.AllocHGlobal(bytes);
            try
            {
                uint cr = CM_Get_Device_IDW(devInst, buffer, (uint)maxChars, 0);
                if (cr == CONFIGRET_SUCCESS)
                    deviceId = Marshal.PtrToStringUni(buffer);
                return cr;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Reimplementation of the DLL's AddPropertyString(): appends one NUL-terminated field
        /// taken at <paramref name="offset"/> inside <paramref name="descriptor"/>. An offset of
        /// 0 / 0xFFFFFFFE / 0xFFFFFFFF appends a bare NUL (an empty field).
        /// </summary>
        public static void AppendDescriptorString(byte[] descriptor, uint offset, List<byte> destination)
        {
            if (offset == 0 || offset >= 0xFFFFFFFE)
            {
                destination.Add(0);
                return;
            }

            if (offset >= descriptor.Length)
                throw new InvalidOperationException("STORAGE_DEVICE_DESCRIPTOR field offset is out of range.");

            for (uint i = offset; i < descriptor.Length; i++)
            {
                byte b = descriptor[i];
                destination.Add(b);
                if (b == 0)
                    return;
            }

            throw new InvalidOperationException("STORAGE_DEVICE_DESCRIPTOR field is not NUL terminated.");
        }
    }

    /// <summary>
    /// A zeroed unmanaged scratch buffer. The collectors need these constantly, and handing
    /// them out through IDisposable keeps the FreeHGlobal out of the middle of the logic.
    /// </summary>
    internal sealed class UnmanagedBuffer : IDisposable
    {
        public IntPtr Pointer { get; }
        public int Size { get; }

        public UnmanagedBuffer(int size)
        {
            Size = size;
            Pointer = Marshal.AllocHGlobal(size);
            Zero();
        }

        /// <summary>Clears the buffer, as the DLL does before every DeviceIoControl.</summary>
        public UnmanagedBuffer Zero()
        {
            for (int i = 0; i < Size; i++)
                Marshal.WriteByte(Pointer, i, 0);
            return this;
        }

        /// <summary>Copies up to <paramref name="count"/> bytes out.</summary>
        public byte[] ToArray(int count)
        {
            int length = Math.Min(count, Size);
            var bytes = new byte[length];
            Marshal.Copy(Pointer, bytes, 0, length);
            return bytes;
        }

        public byte[] ToArray() => ToArray(Size);

        public int ReadInt32(int offset) => Marshal.ReadInt32(Pointer, offset);

        /// <summary>Reads a NUL-terminated wide string that starts at <paramref name="offset"/>.</summary>
        public string ReadUnicodeZ(int offset) => Marshal.PtrToStringUni(new IntPtr(Pointer.ToInt64() + offset));

        public T ToStructure<T>() where T : struct => (T)Marshal.PtrToStructure(Pointer, typeof(T));

        public void Dispose() => Marshal.FreeHGlobal(Pointer);
    }
}
