using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HwidGetCurrentEx
{
    public class SMBIOS
    {
        public enum SMBIOSTableType : sbyte
        {
            BaseBoardInformation = 2,
            BIOSInformation = 0,
            BIOSLanguageInformation = 13,
            CacheInformation = 7,
            const_11 = 11,
            EnclosureInformation = 3,
            EndofTable = 0x7f,
            GroupAssociations = 14,
            MemoryArrayMappedAddress = 0x13,
            MemoryControllerInformation = 5,
            MemoryDevice = 0x11,
            MemoryDeviceMappedAddress = 20,
            MemoryErrorInformation = 0x12,
            MemoryModuleInformation = 6,
            OnBoardDevicesInformation = 10,
            PhysicalMemoryArray = 0x10,
            PortConnectorInformation = 8,
            ProcessorInformation = 4,
            SystemConfigurationOptions = 12,
            SystemEventLog = 15,
            SystemInformation = 1,
            SystemSlotsInformation = 9
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableHeader
        {
            public SMBIOSTableType type;
            public byte length;
            public ushort Handle;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableSystemInfo
        {
            public SMBIOSTableHeader header;
            public byte manufacturer;
            public byte productName;
            public byte version;
            public byte serialNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] UUID;
            //public byte[] WakeUpType;
            //public byte[] SKUNumber;
            //public byte[] Family;

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableBaseBoardInfo
        {
            public SMBIOSTableHeader header;
            //public byte[] Manufacturer;
            //public byte[] Product;
            //public byte[] Version;
            //public byte[] SN;
            //public byte[] AssetTag;
            //public byte[] FeatureFlags;
            //public byte[] LocationInChassis;
            //public ushort ChassisHandle;
            //public byte[] Type;
            //public byte[] NumObjHandle;
            //public ushort pObjHandle;

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableEnclosureInfo
        {
            public SMBIOSTableHeader header;
            //public byte[] Manufacturer;
            //public byte[] Type;
            //public byte[] Version;
            //public byte[] SN;
            //public byte[] AssetTag;
            //public byte[] BootupState;
            //public byte[] PowerSupplyState;
            //public byte[] ThermalState;
            //public byte[] SecurityStatus;
            //public uint OEMDefine;
            //public byte[] Height;
            //public byte[] NumPowerCord;
            //public byte[] ElementCount;
            //public byte[] ElementRecordLength;
            //public byte[] pElements;

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableProcessorInfo
        {
            public SMBIOSTableHeader header;
            //public byte[] socketDesignation;
            //public byte[] processorType;
            //public byte[] processorFamily;
            //public byte[] processorManufacturer;
            //public ulong processorID;
            //public byte[] processorVersion;
            //public byte[] processorVoltage;
            //public ushort externalClock;
            //public ushort maxSpeed;
            //public ushort currentSpeed;
            //public byte[] status;
            //public byte[] processorUpgrade;
            //public ushort L1CacheHandler;
            //public ushort L2CacheHandler;
            //public ushort L3CacheHandler;
            //public byte[] serialNumber;
            //public byte[] assetTag;
            //public byte[] partNumber;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SMBIOSTableCacheInfo
        {
            public SMBIOSTableHeader header;
            //public byte[] socketDesignation;
            //public long cacheConfiguration;
            //public ushort maximumCacheSize;
            //public ushort installedSize;
            //public ushort supportedSRAMType;
            //public ushort currentSRAMType;
            //public byte[] cacheSpeed;
            //public byte[] errorCorrectionType;
            //public byte[] systemCacheType;
            //public byte[] associativity;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UnkownInfo
        {
            public SMBIOSTableHeader header;
            // Todo, Here

        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SHAInfo
        {
            public SMBIOSTableHeader header;
            public int size;
            public byte cache;            
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PhysicalMemoryArray
        {
            public SMBIOSTableHeader header;
            //public byte SwapableNumber;
            public uint EmptyPageNumber;
            public uint TotalPageNumber;            
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BIOSInformation
        {
            public SMBIOSTableHeader header;
            public byte vendor;
            public byte version;
            public ushort startingSegment;
            public byte releaseDate;
            public byte biosRomSize;
            public ulong characteristics;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] extensionBytes;
            //public IntPtr MajorRelease;
            //public byte[] MajorRelease;
            //public byte[] MinorRelease;
            //public byte[] ECFirmwareMajor;
            //public byte[] ECFirmwareMinor;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemCtrlInfo
        {
            public SMBIOSTableHeader header;
            // Todo, Here

        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemModuleInfo
        {
            public SMBIOSTableHeader header;
            public byte SocketDesignation;
            public byte BankConnections;
            public byte CurrentSpeed;
            // Todo, Here
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OemString
        {
            public SMBIOSTableHeader header;
            //public byte[] Count;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemoryArrayMappedAddress
        {
            public SMBIOSTableHeader header;
            public uint Starting;
            public uint Ending;
            public ushort Handle;
            public byte PartitionWidth;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BuiltinPointDevice
        {
            public SMBIOSTableHeader header;
            //public byte[] Type;
            //public byte[] Interface;
            //public byte[] NumOfButton;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PortableBattery
        {
            public SMBIOSTableHeader header;
            //public byte[] Location;
            //public byte[] Manufacturer;
            //public byte[] Date;
            //public byte[] SN;
            //public byte[] DeviceName;
            //public byte[] Chemistry;
            //public ushort DesignCapacity;
            //public ushort DesignVoltage;
            //public byte[] SBDSVersionNumber;
            //public byte[] MaximumErrorInBatteryData;
            //public ushort SBDSSerialNumber;
            //public ushort SBDSManufactureDate;
            //public byte[] SBDSDeviceChemistry;
            //public byte[] DesignCapacityMultiplie;
            //public uint OEM;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemoryDevice
        {
            public SMBIOSTableHeader header;
            public ushort PhysicalArrayHandle;
            public ushort ErrorInformationHandle;
            public ushort TotalWidth;
            public ushort DataWidth;
            public ushort Size;
            //public byte[] FormFactor;
            //public byte[] DeviceSet;
            //public byte[] DeviceLocator;
            //public byte[] BankLocator;
            //public byte[] MemoryType;
            //public ushort TypeDetail;
            //public ushort Speed;
            //public byte[] Manufacturer;
            //public byte[] SN;
            //public byte[] AssetTag;
            //public byte[] PN;
            //public byte[] Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RawSMBIOSData
        {
            public byte Used20CallingMethod;
            public byte SMBIOSMajorVersion;
            public byte SMBIOSMinorVersion;
            public byte DmiRevision;
            public uint Length;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            //public byte[] SMBIOSTableData;
            //public GStruct0 BiosInfo;
            //public SMBIOSTableSystemInfo SystemInfo;
            //public SMBIOSTableBaseBoardInfo BaseBoardInfo;
            //public SMBIOSTableEnclosureInfo EnclosureInfo;
            //public SMBIOSTableProcessorInfo ProcessorInfo;
            //public SMBIOSTableCacheInfo CacheInfo;
        }
    }
}
