using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HwidGetCurrentEx
{


    public static partial class Native
    {

        public const int ERROR_INVALID_HANDLE_VALUE = -1;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;

        public const uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x00560000;
        public const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x2D1400;
        public const uint IOCTL_BTH_GET_LOCAL_INFO = 0x410000;
        public const uint IOCTL_NDIS_QUERY_GLOBAL_STATS = 0x170002;
        public const uint PERMANENT_ADDRESS= 0x1010101;
        public const uint RSMB = 1381190978;

        public enum DI_FUNCTION 
        {
            DIF_SELECTDEVICE = 0x00000001,
            DIF_INSTALLDEVICE = 0x00000002,
            DIF_ASSIGNRESOURCES = 0x00000003,
            DIF_PROPERTIES = 0x00000004,
            DIF_REMOVE = 0x00000005,
            DIF_FIRSTTIMESETUP = 0x00000006,
            DIF_FOUNDDEVICE = 0x00000007,
            DIF_SELECTCLASSDRIVERS = 0x00000008,
            DIF_VALIDATECLASSDRIVERS = 0x00000009,
            DIF_INSTALLCLASSDRIVERS = 0x0000000A,
            DIF_CALCDISKSPACE = 0x0000000B,
            DIF_DESTROYPRIVATEDATA = 0x0000000C,
            DIF_VALIDATEDRIVER = 0x0000000D,
            DIF_MOVEDEVICE = 0x0000000E,
            DIF_DETECT = 0x0000000F,
            DIF_INSTALLWIZARD = 0x00000010,
            DIF_DESTROYWIZARDDATA = 0x00000011,
            DIF_PROPERTYCHANGE = 0x00000012,
            DIF_ENABLECLASS = 0x00000013,
            DIF_DETECTVERIFY = 0x00000014,
            DIF_INSTALLDEVICEFILES = 0x00000015,
            DIF_UNREMOVE = 0x00000016,
            DIF_SELECTBESTCOMPATDRV = 0x00000017,
            DIF_ALLOW_INSTALL = 0x00000018,
            DIF_REGISTERDEVICE = 0x00000019,
            DIF_NEWDEVICEWIZARD_PRESELECT = 0x0000001A,
            DIF_NEWDEVICEWIZARD_SELECT = 0x0000001B,
            DIF_NEWDEVICEWIZARD_PREANALYZE = 0x0000001C,
            DIF_NEWDEVICEWIZARD_POSTANALYZE = 0x0000001D,
            DIF_NEWDEVICEWIZARD_FINISHINSTALL = 0x0000001E,
            DIF_UNUSED1 = 0x0000001F,
            DIF_INSTALLINTERFACES = 0x00000020,
            DIF_DETECTCANCEL = 0x00000021,
            DIF_REGISTER_COINSTALLERS = 0x00000022,
            DIF_ADDPROPERTYPAGE_ADVANCED = 0x00000023,
            DIF_ADDPROPERTYPAGE_BASIC = 0x00000024,
            DIF_RESERVED1 = 0x00000025,
            DIF_TROUBLESHOOTER = 0x00000026,
            DIF_POWERMESSAGEWAKE = 0x00000027,
            DIF_ADDREMOTEPROPERTYPAGE_ADVANCED = 0x00000028,
            DIF_UPDATEDRIVER_UI = 0x00000029,
            DIF_RESERVED2 = 0x00000030,
        };


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

        [StructLayout(LayoutKind.Sequential)]
        public class HWProfile
        {
            public Int32 dwDockInfo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 39)]
            public string szHwProfileGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szHwProfileName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DISK_EXTENT
        {
            public uint DiskNumber;
            public long StartingOffset;
            public long ExtentLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class VOLUME_DISK_EXTENTS
        {
            public uint NumberOfDiskExtents;
            public DISK_EXTENT Extents;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE_SEEK_PENALTY_DESCRIPTOR
        {
            public readonly uint Version;
            public readonly uint Size;
            [MarshalAs(UnmanagedType.U1)]
            public readonly bool IncursSeekPenalty;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_DESCRIPTOR_HEADER
        {
            public uint Version;
            public uint Size;
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] RawDeviceProperties;
        }


        public enum WWAN_INTERFACE_STATE
        {
             WwanInterfaceStateNotReady = 0,
            WwanInterfaceStateDeviceLocked = 1,
            WwanInterfaceStateUserAccountNotActivated = 2,
            WwanInterfaceStateRegistered = 3,
            WwanInterfaceStateRegistering = 4,
            WwanInterfaceStateDeregistered = 5,
            WwanInterfaceStateAttached = 6,
            WwanInterfaceStateAttaching = 7,
            WwanInterfaceStateDetaching = 8,
            WwanInterfaceStateActivated = 9,
            WwanInterfaceStateActivating = 10,
            WwanInterfaceStateDeactivating = 11
        }

        public  struct WWAN_INTERFACE_STATUS
        {
            bool fInitialized;
            WWAN_INTERFACE_STATE InterfaceState;
        }
        public struct WWAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] strInterfaceDescription;
            public WWAN_INTERFACE_STATUS InterfaceStatus;
            public int dwReserved1;
            public Guid guidReserved;
            public Guid ParentInterfaceGuid;
            public int dwReserved2;
            public int dwIndex;
            public int dwReserved3;
            public int dwReserved4;
        }
        public struct WWAN_INTERFACE_INFO_LIST
        {
            public int dwNumberOfItems;
            public WWAN_INTERFACE_INFO[] InterfaceInfo;
        }





#if !WIN64
        [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Unicode)]
#else  
        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
#endif
        public struct SP_DRVINFO_DATA
        {
            public int cbSize;
            public uint DriverType;
            public UIntPtr Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string MfgName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ProviderName;
            public System.Runtime.InteropServices.ComTypes.FILETIME DriverDate;
            public ulong DriverVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_PROPERTY_QUERY
        {
            public Int32 PropertyId;
            public Int32 QueryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] AdditionalParameters;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct HidD_Attributes
        { public int Size; public ushort VendorID; public ushort ProductID; public ushort VersionNumber; }

#if !WIN64
        [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Unicode)]
#else  
        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
#endif
        public struct SP_DRVINFO_DETAIL_DATA
        {
            public Int32 cbSize;
            public System.Runtime.InteropServices.ComTypes.FILETIME InfDate;
            public Int32 CompatIDsOffset;
            public Int32 CompatIDsLength;
            public IntPtr Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String SectionName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String InfFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String DrvDescription;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public String HardwareID;
        };

        [Flags]
        public enum DiGetClassFlags : uint
        {
            DIGCF_DEFAULT = 0x00000001,
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            SPDRP_UNUSED2 = 0x00000006,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010,
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct NativeDeviceInterfaceDetailData
        {
            public int size;
            public char devicePath;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string DevicePath;
        }

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetupDiGetDriverInfoDetail(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref SP_DRVINFO_DATA DriverInfoData, ref SP_DRVINFO_DETAIL_DATA DriverInfoDetailData, Int32 DriverInfoDetailDataSize, ref Int32 RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetupDiEnumDriverInfo(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int DriverType, int MemberIndex, ref SP_DRVINFO_DATA DriverInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, [MarshalAs(UnmanagedType.LPTStr)] string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, [MarshalAs(UnmanagedType.LPTStr)] string Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevsW([In] ref Guid ClassGuid, [MarshalAs(UnmanagedType.LPWStr)] string Enumerator, IntPtr parent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll")]
        public static extern int CM_Get_Parent(ref IntPtr pdnDevInst, IntPtr dnDevInst, int ulFlags);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_Registry_PropertyW", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CM_Get_DevNode_Registry_PropertyW(IntPtr dnDevInst, int ulProperty, ref int pulRegDataType, IntPtr Buffer, ref int pulLength, int ulFlags);

        [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_Registry_PropertyA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int CM_Get_DevNode_Registry_Property(IntPtr dnDevInst, int ulProperty, ref int pulRegDataType, ref IntPtr Buffer, ref int pulLength, int ulFlags);


        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern int CM_Get_DevNode_Status(ref int status, ref int probNum, IntPtr devInst, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Get_Device_ID(IntPtr pdnDevInst, ref IntPtr buffer, int bufferLen, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Get_Device_IDW(IntPtr pdnDevInst, IntPtr Buffer, uint bufferLen, uint flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, int propertyRegDataType, IntPtr propertyBuffer, uint propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int property,  int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, out int requiredSize);


        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileW(string filename, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);


        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("hid.dll", ExactSpelling = true)]
        public static extern bool HidD_GetAttributes(IntPtr HidDeviceObject, ref HidD_Attributes Attributes);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,ref uint lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetCurrentHwProfile(IntPtr fProfile);

        [DllImport("kernel32")]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX stat);

        [DllImport("kernel32.dll", EntryPoint = "GetSystemFirmwareTable")]
        public static extern uint GetSystemFirmwareTable(uint FirmwareTableProviderSignature, uint FirmwareTableID, IntPtr pFirmwareTableBuffer, uint BufferSize);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WwanOpenHandle(int dwClientVersion, IntPtr pReserved, out int pdwNegotiatedVersion, out IntPtr phClientHandle);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WwanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WwanEnumerateInterfaces(IntPtr hClientHandle, int pdwReserved, out WWAN_INTERFACE_INFO_LIST ppInterfaceList);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WwanFreeMemory(IntPtr pMem);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WwanSetInterface(IntPtr hClientHandle, Guid pInterfaceGuid, int OpCode, int dwDataSize, IntPtr pData, IntPtr pReserved1, IntPtr pReserved2, IntPtr pReserved3);

        [DllImport("wwapi.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern int WlanQueryInterface(IntPtr hClientHandle, Guid pInterfaceGuid, int OpCode, IntPtr pReserved, out int pdwDataSize, out IntPtr ppData, out int pWlanOpcodeValueType);

        [DllImport("rpcrt4.dll", SetLastError = true)]
        public static extern int UuidCreateSequential(out System.Guid guid);

    }
}
