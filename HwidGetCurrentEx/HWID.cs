using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static HwidGetCurrentEx.Native;
using static HwidGetCurrentEx.BitUtil;
using static HwidGetCurrentEx.SMBIOS;
using System.Diagnostics;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace HwidGetCurrentEx
{
    [Serializable]
    public class StructHWID
    {
        public ushort[] BiosHwidBlock;
        public ushort[] MemoryHwidBlock;
        public ushort[] CpuHwidBlock;
        public ushort[] NdisHwidBlock;
        public ushort[] HWProfileBlock;
        public ushort[] AudioHwidBlock;
        public ushort[] PcmciaHwidBlock;
        public ushort[] BthPortHwidBlock;
        public ushort[] ScsiAdapterHwidBlock;
        public ushort[] DisplayHwidBlock;
        public ushort[] DiskHwidBlock;
        public ushort[] HdcHwidBlock;
        public ushort[] WwanHwidBlock;
        public ushort[] CdromHwidBlock;
    }
    public class StringHWID
    {
        public List<string> BiosHwidBlock { get; set; } = new List<string>();
        public List<string> MemoryHwidBlock { get; set; } = new List<string>();
        public List<string> CpuHwidBlock { get; set; } = new List<string>();
        public List<string> NdisHwidBlock { get; set; } = new List<string>();
        public List<string> HWProfileBlock { get; set; } = new List<string>();
        public List<string> AudioHwidBlock { get; set; } = new List<string>();
        public List<string> PcmciaHwidBlock { get; set; } = new List<string>();
        public List<string> BthPortHwidBlock { get; set; } = new List<string>();
        public List<string> ScsiAdapterHwidBlock { get; set; } = new List<string>();
        public List<string> DisplayHwidBlock { get; set; } = new List<string>();
        public List<string> DiskHwidBlock { get; set; } = new List<string>();
        public List<string> HdcHwidBlock { get; set; } = new List<string>();
        public List<string> WwanHwidBlock { get; set; } = new List<string>();
        public List<string> CdromHwidBlock { get; set; } = new List<string>();
    }

    public static class HWID
    {
        public static StringHWID stringHwid = new StringHWID();
        public static StructHWID structHwid = new StructHWID();
        static byte[] signByte = new byte[] { 0, 0xE, 1, 2, 3, 4, 0xF, 5, 6, 7, 8, 9, 0xA, 0xC };       
        static uint[] MemoryMagic = new uint[] { 0x00000000, 0x10000000, 0x00000000, 0x20000000, 0x00000000, 0x40000000, 0x00000000, 0x60000000, 0x00000000, 0x80000000, 0x00000000, 0xC0000000, 0x00000000, 0x40000000, 0x00000000 };

        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        public static string HwidCreateBlock(byte[] arrayHWID, int cbsize)
        {
            byte[] bytearray = new byte[] { 0x0, 0x2, 0x0, 0x1, 0x1, 0x0, 0x2, 0x5, 0x0, 0x3, 0x1, 0x0, 0x4, 0x2, 0x0, 0x6, 0x1, 0x0, 0x8, 0x7, 0x0, 0x9, 0x3, 0x0, 0xA, 0x1, 0x0, 0xC, 0x7, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var buffersize = cbsize + 6 + 0x25;
            byte[] buffers = new byte[buffersize];
            buffers[0] = (byte)buffersize;
            buffers[4] = 0x13;
            Buffer.BlockCopy(arrayHWID, 0, buffers, 6, cbsize);
            buffers[cbsize + 6] = 0xC;
            Buffer.BlockCopy(bytearray, 0, buffers, cbsize + 6 + 1, bytearray.Length);
            return Convert.ToBase64String(buffers);
        }
        public static byte[] HwidGetCurrentEx(ref byte[] data)
        {

            int m = 0;
            int n = 0;
            int cbSize = 0;
            byte[] DST = new byte[0x118];
            List<ushort> ValueList = new List<ushort>();
            for (int i = 0; i < signByte.Length; i++)
            {
                var v = signByte[i];
                if (v == 0)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_DEVINTERFACE_CDROM, false, "CDROM");
                    structHwid.CdromHwidBlock = ValueList.ToArray();
                    n = 0;
                }
                else if (v == 0xE)
                {
                    ValueList = CollectWwan();
                    structHwid.WwanHwidBlock = ValueList.ToArray();
                    n = 0;
                }
                else if (v == 1)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_DEVCLASS_HDC, false, "HDC");
                    structHwid.HdcHwidBlock = ValueList.ToArray();
                    n = 1;
                }
                else if (v == 2)
                {
                    ValueList = EnumInterfaces(ref GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK, IOCTL_STORAGE_QUERY_PROPERTY);
                    structHwid.DiskHwidBlock = ValueList.ToArray();
                    n = 2;
                }
                else if (v == 3)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_DEVCLASS_DISPLAY, false, "DISPLAY");
                    structHwid.DisplayHwidBlock = ValueList.ToArray();
                    n = 3;
                }
                else if (v == 4)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_DEVCLASS_SCSIADAPTER, false, "SCSIADAPTER");
                    structHwid.ScsiAdapterHwidBlock = ValueList.ToArray();
                    n = 4;
                }
                else if (v == 0xF)
                {
                    ValueList = EnumInterfaces(ref GUID_DEVINTERFACE.GUID_BTHPORT_DEVICE_INTERFACE, IOCTL_BTH_GET_LOCAL_INFO);
                    structHwid.BthPortHwidBlock = ValueList.ToArray();
                    n = 4;
                }
                else if (v == 5)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_DEVCLASS_PCMCIA, false, "PCMCIA");
                    structHwid.PcmciaHwidBlock = ValueList.ToArray();
                    n = -1;
                }
                else if (v == 6)
                {
                    ValueList = CollectInternal(ref GUID_DEVINTERFACE.GUID_KSCATEGORY_AUDIO, true, "AUDIO");
                    structHwid.AudioHwidBlock = ValueList.ToArray();
                    n = 5;
                }
                else if (v == 7)
                {
                    ValueList = CollectHWProfile();
                    structHwid.HWProfileBlock = ValueList.ToArray();
                    DST[0x16] = 1;
                    continue;
                }
                else if (v == 8)
                {
                    ValueList = EnumInterfaces(ref GUID_DEVINTERFACE.GUID_NDIS_LAN_CLASS, IOCTL_NDIS_QUERY_GLOBAL_STATS);
                    structHwid.NdisHwidBlock = ValueList.ToArray();
                    n = 7;
                }
                else if (v == 9)
                {
                    ValueList = CollectCPU();
                    structHwid.CpuHwidBlock = ValueList.ToArray();
                    n = 8;
                }
                else if (v == 0xA)
                {
                    ValueList = CollectMemory();
                    structHwid.MemoryHwidBlock = ValueList.ToArray();
                    n = -2;
                    cbSize -= ValueList.Count;
                    m = -2;
                }
                else if (v == 0xC)
                {
                    ValueList = CollectBIOS();
                    structHwid.BiosHwidBlock = ValueList.ToArray();
                    n = -1;
                    cbSize -= ValueList.Count;
                    m = -1;
                }
                if (ValueList.Count > 0)
                {
                    if (n >= 0)
                    {
                        int postion1 = n * 2 + 4;
                        DST[postion1] += (byte)(ValueList.Count & 0xFF);
                    }
                    ValueList.Sort();
                    for (int x = 0; x < ValueList.Count; x++)
                    {
                        int pValue = (m + 0xE) * 2;
                        byte[] bytes = BitConverter.GetBytes(ValueList[x]);
                        DST[pValue + 1] = bytes[1];
                        DST[pValue] = bytes[0];
                        cbSize += 1;
                        m += 1;
                    }
                }
                else
                {
                    int postion1 = n * 2 + 4;
                    DST[postion1 + 1] += (byte)(ValueList.Count & 0xFF);
                }
            }
            DST[0] = (byte)(cbSize * 2 + 0x1C);

            data = ObjectToByteArray(structHwid);
            //StructHWID sHwid = (StructHWID)ByteArrayToObject(data);
            Debug.Print(DST[0].ToString() + Environment.NewLine + BitConverter.ToString(DST.Take(DST[0]).ToArray()).Replace("-", " "));
            Debug.Print(Environment.NewLine + BitConverter.ToString(data).Replace("-", " "));
            return DST.Take(DST[0]).ToArray();
        }

        #region CollectWwan
        static List<ushort> CollectWwan()
        {
            List<ushort> result = new List<ushort>();
            IntPtr hClient = IntPtr.Zero;
            int dwMaxClient = 1;
            int dwCurVersion = 0;
            int dwResult = 0;
            int iterationMax = 5;
            int iteration = 0;
            WWAN_INTERFACE_INFO_LIST pIfList = new WWAN_INTERFACE_INFO_LIST();
            try
            {
                dwResult = WwanOpenHandle(dwMaxClient, IntPtr.Zero, out dwCurVersion, out hClient);
                if (dwResult != 0)
                {
                    while (true)
                    {
                        dwResult = WwanEnumerateInterfaces(hClient, 0, out pIfList);
                        if (dwResult != 0)
                        {
                            break;
                        }
                        else
                        {
                            iteration++;
                            if (iteration > iterationMax)
                            {
                                break;
                            }
                            bool HasActivated = false;
                            IntPtr pConnectInfo;
                            int connectInfoSize;
                            int opCode;
                            for (int i = 0; i < (int)pIfList.dwNumberOfItems; i++)
                            {
                                WWAN_INTERFACE_INFO pIfInfo = pIfList.InterfaceInfo[i];
                                dwResult = WlanQueryInterface(hClient, pIfInfo.InterfaceGuid, 7, IntPtr.Zero, out connectInfoSize, out pConnectInfo, out opCode);
                                if (dwResult != 0)
                                {
                                    byte[] bytesIn = new byte[36];
                                    Marshal.Copy(new IntPtr(IntPtr.Size == 4 ? pConnectInfo.ToInt32() + 652 : pConnectInfo.ToInt64() + 652), bytesIn, 0, bytesIn.Length);
                                    stringHwid.WwanHwidBlock.Add(BitConverter.ToString(bytesIn).Replace("-", " "));
                                    ushort value = AddInstanceHash(bytesIn, 36, true);
                                    if (!result.Contains(value)) result.Add(value); ;
                                }
                            }
                            if (HasActivated)
                            {
                                break;
                            }
                        }
                    }
                }
                if (hClient != IntPtr.Zero)
                {
                    WwanCloseHandle(hClient, IntPtr.Zero);
                }
            }
            catch
            {

            }

            return result;
        }
        #endregion

        #region CollectHWProfile
        static List<ushort> CollectHWProfile()
        {
            List<ushort> result = new List<ushort>();
            IntPtr lHWInfoPtr = Marshal.AllocHGlobal(123);
            HWProfile lProfile = new HWProfile();
            Marshal.StructureToPtr(lProfile, lHWInfoPtr, false);

            if (GetCurrentHwProfile(lHWInfoPtr))
            {
                Marshal.PtrToStructure(lHWInfoPtr, lProfile);
                //string lText = lProfile.szHwProfileGuid.ToString();
            }
            else
            {
                lProfile.dwDockInfo = 1;
            }
            if ((lProfile.dwDockInfo & 4) == 0)
            {
                int size = lProfile.dwDockInfo & 3;
                if (size != 3)
                {
                    byte[] bytesIn = BitConverter.GetBytes(lProfile.dwDockInfo);
                    //bytesIn = bytesIn.Concats(Encoding.Unicode.GetBytes(lProfile.szHwProfileGuid.ToString())).ToArray();
                    stringHwid.HWProfileBlock.Add(lProfile.szHwProfileGuid.ToString());
                    ushort value = AddInstanceHash(bytesIn, 4, false);
                    if (!result.Contains(value)) result.Add(value);
                }
            }
            Marshal.FreeHGlobal(lHWInfoPtr);
            return result;
        }
        #endregion

        #region CollectCPU
        static List<ushort> CollectCPU()
        {
            List<ushort> result = new List<ushort>();
            byte[] bytesIn = new byte[0];
            //ManagementObjectSearcher sql = new ManagementObjectSearcher("SELECT * FROM Win32_PRocessor");
            //foreach (ManagementObject cpu in sql.Get())
            //{
            //    string Manufacturer = cpu["Manufacturer"].ToString();
            //    byte[] processorID = StrToByteArray(cpu["ProcessorId"].ToString());
            //    int values1 = (int)(array2ulong(processorID, 0, 8) & 0xFFFFFFF0);
            //    bytesIn = Encoding.UTF8.GetBytes(Manufacturer);
            //}
            byte[] ProcessorId = CPUID.Invoke(0);
            byte[] CpuId = CPUID.Invoke(1);
            int value1 = (int)(BitConverter.ToUInt32(CpuId, 0) & 0xFFFFFFF0);
            int value2 = (int)(BitConverter.ToUInt32(CpuId, 4) & 0xFFFFFF);
            bytesIn = bytesIn.Concat(ProcessorId.Skip(4).Take(4).ToArray()).Concat(ProcessorId.Skip(12).Take(4).ToArray()).Concat(ProcessorId.Skip(8).Take(4).ToArray()).Concat(BitConverter.GetBytes(value1)).Concat(BitConverter.GetBytes(value2)).ToArray();
            stringHwid.CpuHwidBlock.Add(Encoding.UTF8.GetString(ProcessorId.Skip(4).ToArray()) + " " + BitConverter.ToString(CpuId).Replace("-", " "));
            ushort value = AddInstanceHash(bytesIn, 20, true);
            if (!result.Contains(value)) result.Add(value); ;
            return result;
        }
        #endregion

        #region CollectHWProfile
        static List<ushort> CollectMemory()
        {
            List<ushort> result = new List<ushort>();

            MEMORYSTATUSEX memEx = new MEMORYSTATUSEX();
            memEx.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            GlobalMemoryStatusEx(ref memEx);
            byte[] bytes = BitConverter.GetBytes(memEx.ullTotalPhys);
            int lTotalPhys = BitConverter.ToInt32(bytes, 0);
            int hTotalPhys = BitConverter.ToInt32(bytes, 4);

            int value1 = (int)(PAIR(hTotalPhys, lTotalPhys) >> 10);
            int value2 = (int)(hTotalPhys >> 10);

            uint InstalledMemorySize = 0;
            if (!GetInstalledMemorySize(ref InstalledMemorySize))
            {
                InstalledMemorySize = 0;
            }
            if (InstalledMemorySize < value1)
            {
                InstalledMemorySize = (uint)value1;
            }
            uint value3 = (uint)(PAIR((uint)0, InstalledMemorySize) >> 22);
            uint value4 = InstalledMemorySize << 10;
            int m = 0;
            int values = 0;
            do
            {
                if (value3 < MemoryMagic[2 * m] || value3 <= MemoryMagic[2 * m] && value4 <= MemoryMagic[2 * m])
                    break;
                ++m;
            }
            while (m < 8);
            if (m == 8)
                values = 8 * (int)((PAIR(value3, value4) - 0xC0000000) >> 30) + 7;
            else
                values = m;
            byte[] bytesIn = BitConverter.GetBytes(values);
            stringHwid.MemoryHwidBlock.Add(values.ToString());
            ushort value = AddInstanceHash(bytesIn, 4, true);
            if (!result.Contains(value)) result.Add(value); ;
            return result;
        }
        static bool GetInstalledMemorySize(ref uint InstalledMemorySize)
        {
            List<string> productList = new List<string>();
            uint res = GetSystemFirmwareTable(RSMB, 0, IntPtr.Zero, 0);
            IntPtr buffer = Marshal.AllocHGlobal((int)res);
            //using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSSmBios_RawSMBiosTables"))
            //{
            //    foreach (ManagementObject obj in searcher.Get())
            //    {
            //        byte[] raw = (byte[])obj["SMBiosData"];
            //    }
            //}
            res = GetSystemFirmwareTable(RSMB, 0, buffer, res);
            if (res != 0)
            {
                int position = 8;
                byte type = Marshal.ReadByte(buffer, position);
                while (position + 4 < res && type != 127)
                {
                    switch (type)
                    {
                        //case 0x00:  //BIOSInformation
                        //    BIOSInformation BiosInfo = (BIOSInformation)Marshal.PtrToStructure(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position), typeof(BIOSInformation));
                        //    position += BiosInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x01:  //SystemInformation
                        //    SMBIOSTableSystemInfo SystemInfo = (SMBIOSTableSystemInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32() + position: buffer.ToInt64() + position), typeof(SMBIOSTableSystemInfo));
                        //    position += SystemInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x03:  //EnclosureInfo
                        //    SMBIOSTableEnclosureInfo EnclosureInfo = (SMBIOSTableEnclosureInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(SMBIOSTableEnclosureInfo));
                        //    position += EnclosureInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x02:  //BaseBoardInfo
                        //    SMBIOSTableBaseBoardInfo BaseBoardInfo = (SMBIOSTableBaseBoardInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32() : buffer.ToInt64() + position), typeof(SMBIOSTableBaseBoardInfo));
                        //    position += BaseBoardInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x04:
                        //    SMBIOSTableProcessorInfo ProcessorInfo = (SMBIOSTableProcessorInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(SMBIOSTableProcessorInfo));
                        //    position += ProcessorInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x18:
                        //    UnkownInfo unkownInfo = (UnkownInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(UnkownInfo));
                        //    position += unkownInfo.header.length;
                        //    position += 2;
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x80:
                        //    SHAInfo shaInfo = (SHAInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32() + position: buffer.ToInt64() + position), typeof(SHAInfo));
                        //    position += shaInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 0x10:  //PhysicalMemoryArray                           
                        //    PhysicalMemoryArray PhysicalMemory = (PhysicalMemoryArray)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(PhysicalMemoryArray));
                        //    position += PhysicalMemory.header.length;
                        //    position += 2;
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        case 0x11: //MemoryDevice                            
                            MemoryDevice memoryDevice = (MemoryDevice)Marshal.PtrToStructure(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position), typeof(MemoryDevice));
                            if (memoryDevice.Size == 0) return false;
                            InstalledMemorySize += (uint)(memoryDevice.Size << 10);
                            position += memoryDevice.header.length;
                            position = GetProductName(ref productList, ref buffer, ref position);
                            type = Marshal.ReadByte(buffer, position);
                            break;
                        //case 0x13:
                        //    MemoryArrayMappedAddress MemoryArrayMapped = (MemoryArrayMappedAddress)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(MemoryArrayMappedAddress));
                        //    position += MemoryArrayMapped.header.length;
                        //    position += 2;
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        //case 7:
                        //    SMBIOSTableCacheInfo CacheInfo = (SMBIOSTableCacheInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size==4 ? buffer.ToInt32()+ position : buffer.ToInt64() + position), typeof(SMBIOSTableCacheInfo));
                        //    position += CacheInfo.header.length;
                        //    position = GetProductName(ref productList, ref buffer, ref position);
                        //    type = Marshal.ReadByte(buffer, position);
                        //    break;
                        default:
                            int size = Marshal.ReadByte(buffer, position + 1);
                            //int handle = Marshal.ReadByte(buffer, position + 2);
                            position += size;
                            position = GetProductName(ref productList, ref buffer, ref position);
                            type = Marshal.ReadByte(buffer, position);
                            break;
                    }
                }
            }
            return true;
        }
        #endregion       

        #region CollectBIOS
        static List<ushort> CollectBIOS()
        {
            List<ushort> result = new List<ushort>();
            List<string> productList = new List<string>();

            uint res = GetSystemFirmwareTable(RSMB, 0, IntPtr.Zero, 0);
            IntPtr buffer = Marshal.AllocHGlobal((int)res);
            res = GetSystemFirmwareTable(RSMB, 0, buffer, res);

            int position = 8;
            BIOSInformation BiosInfo = (BIOSInformation)Marshal.PtrToStructure(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position), typeof(BIOSInformation));
            position += BiosInfo.header.length;
            string productName = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
            if (productName == "")
                position += 2;
            else
            {
                position += productName.Length + 1;
                string productType = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                position += productType.Length + 1;
                string factoryTime = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                position += factoryTime.Length + 2;
            }

            int index = position + 8;
            SMBIOSTableSystemInfo SystemInfo = (SMBIOSTableSystemInfo)Marshal.PtrToStructure(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position), typeof(SMBIOSTableSystemInfo));
            position += SystemInfo.header.length;
            string BiosVendor = "";
            string BiosVersion = "";
            string BiosSerial = "";
            string manufacturer = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
            if (manufacturer == "")
                position += 2;
            else
            {
                position += manufacturer.Length + 1;
                BiosVendor = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                position += BiosVendor.Length + 1;
                BiosSerial = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                position += BiosSerial.Length + 1;
                BiosVersion = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                position += BiosVersion.Length + 2;
            }

            byte[] buff1 = new byte[16];
            Marshal.Copy(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + index : buffer.ToInt64() + index), buff1, 0, buff1.Length);
            byte[] bytesIn = new byte[0];
            if (BiosVendor == "None" || BiosVendor == "")
            {
                bytesIn = buff1.Concat(Encoding.UTF8.GetBytes(manufacturer)).Concat(Encoding.UTF8.GetBytes(BiosVersion)).Concat(Encoding.UTF8.GetBytes(BiosSerial)).Concat(Encoding.UTF8.GetBytes(productName)).ToArray();
                stringHwid.BiosHwidBlock.Add(manufacturer + " " + BiosVersion + " " + BiosSerial + " " + productName);
            }
            else
            {
                bytesIn = buff1.Concat(Encoding.UTF8.GetBytes(manufacturer)).Concat(Encoding.UTF8.GetBytes(BiosVendor)).Concat(Encoding.UTF8.GetBytes(BiosVersion)).Concat(Encoding.UTF8.GetBytes(productName)).ToArray();
                stringHwid.BiosHwidBlock.Add(manufacturer + " " + BiosVendor + " " + BiosVersion + " " + productName);
            }

            ushort value = AddInstanceHash(bytesIn, (uint)bytesIn.Length, true);
            if (!result.Contains(value)) result.Add(value);
            Marshal.FreeHGlobal(buffer);
            return result;
        }
        static int GetProductName(ref List<string> productList, ref IntPtr buffer, ref int position)
        {
            do
            {
                string productName = Marshal.PtrToStringAnsi(new IntPtr(IntPtr.Size == 4 ? buffer.ToInt32() + position : buffer.ToInt64() + position));
                if (!string.IsNullOrEmpty(productName)) productList.Add(productName);
                position += productName.Length + 1;
            } while (Marshal.ReadByte(buffer, position) != 0);
            position += 1;
            return position;
        }
        #endregion

        #region EnumInterfaces
        static List<ushort> EnumInterfaces(ref Guid ClassGuid, uint dwIoControlCode)
        {
            uint index = 0;
            bool readable = false;
            IntPtr hFile = IntPtr.Zero;
            List<ushort> result = new List<ushort>();
            try
            {
                IntPtr hDevInfo = SetupDiGetClassDevsW(ref ClassGuid, null, IntPtr.Zero, (int)DI_FUNCTION.DIF_PROPERTYCHANGE);
                if (hDevInfo != IntPtr.Zero)
                {
                    while (true)
                    {
                        if (hFile != IntPtr.Zero && hFile.ToInt32() != -1)
                        {
                            try
                            {
                                CloseHandle(hFile);
                                hFile = IntPtr.Zero;
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.ToString());
                                hFile = IntPtr.Zero;
                            }
                        }
                        SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData);
                        if (NextInterfaceDeviceHandle(hDevInfo, ref ClassGuid, ref readable, ref index, ref hFile))
                        {
                            if (hFile != IntPtr.Zero && hFile.ToInt32() != -1)
                            {
                                if (dwIoControlCode == IOCTL_STORAGE_QUERY_PROPERTY)
                                {
                                    if (readable)
                                    {
                                        IntPtr lpOutBuffer = Marshal.AllocHGlobal(0x400);
                                        uint BytesReturned = 0;
                                        uint lpInBuffer = 0;
                                        bool success = DeviceIoControl(hFile, IOCTL_STORAGE_QUERY_PROPERTY, ref lpInBuffer, 0xc, lpOutBuffer, 0x400, out BytesReturned, IntPtr.Zero);
                                        if (!success)
                                        {
                                            int errorCode = Marshal.GetLastWin32Error();
                                        }
                                        STORAGE_DEVICE_DESCRIPTOR deviceDescriptor = (STORAGE_DEVICE_DESCRIPTOR)Marshal.PtrToStructure(lpOutBuffer, typeof(STORAGE_DEVICE_DESCRIPTOR));
                                        int rawDevicePropertiesOffset = Marshal.SizeOf(deviceDescriptor);

                                        int rawDevicePropertiesLength = (int)deviceDescriptor.Size - rawDevicePropertiesOffset;
                                        deviceDescriptor.RawDeviceProperties = new byte[rawDevicePropertiesLength];
                                        Marshal.Copy(new IntPtr(lpOutBuffer.ToInt64() + rawDevicePropertiesOffset), deviceDescriptor.RawDeviceProperties, 0, rawDevicePropertiesLength);

                                        string description = String.Empty;
                                        string serialNumber = String.Empty;
                                        if (deviceDescriptor.VendorIdOffset > 0)
                                        {
                                            int offset = (int)deviceDescriptor.VendorIdOffset - Marshal.SizeOf(deviceDescriptor);
                                            string vendor = ReadNullTerminatedAnsiString(deviceDescriptor.RawDeviceProperties, offset);
                                            while (vendor.EndsWith("  "))
                                            {
                                                vendor = vendor.Remove(vendor.Length - 1);
                                            }
                                            description += vendor;
                                        }
                                        if (deviceDescriptor.ProductIdOffset > 0)
                                        {
                                            int offset = (int)deviceDescriptor.ProductIdOffset - Marshal.SizeOf(deviceDescriptor);
                                            string product = ReadNullTerminatedAnsiString(deviceDescriptor.RawDeviceProperties, offset);
                                            description += product.Trim();
                                        }
                                        if (deviceDescriptor.SerialNumberOffset > 0 && deviceDescriptor.SerialNumberOffset != 0xFFFFFFFF)
                                        {
                                            int offset = (int)deviceDescriptor.SerialNumberOffset - Marshal.SizeOf(deviceDescriptor);
                                            serialNumber = ReadNullTerminatedAnsiString(deviceDescriptor.RawDeviceProperties, offset);

                                        }
                                        if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(serialNumber))
                                        {
                                            stringHwid.DiskHwidBlock.Add(description + " " + serialNumber);
                                            byte[] bytesIn = new byte[] { 0 }.Concats(Encoding.UTF8.GetBytes(description)).Concats(new byte[] { 0 }).Concats(Encoding.UTF8.GetBytes(serialNumber)).Concats(new byte[] { 0 }).ToArray();
                                            ushort value = AddInstanceHash(bytesIn, (uint)bytesIn.Length, true);
                                            if (!readable) value += 1;
                                            if (!result.Contains(value)) result.Add(value); ;
                                        }
                                    }
                                }
                                else if (dwIoControlCode == IOCTL_BTH_GET_LOCAL_INFO)
                                {
                                    ushort value = EnumInterfaceCallback(hFile);
                                    if (value != 0)
                                    {
                                        if (!readable) value += 1;
                                        if (!result.Contains(value)) result.Add(value); ;
                                        return result;
                                    }

                                }
                                else if (dwIoControlCode == IOCTL_NDIS_QUERY_GLOBAL_STATS)
                                {
                                    IntPtr lpOutBuffer = Marshal.AllocHGlobal(6);
                                    uint BytesReturned = 0;
                                    uint lpInBuffer = PERMANENT_ADDRESS;
                                    bool success = DeviceIoControl(hFile, IOCTL_NDIS_QUERY_GLOBAL_STATS, ref lpInBuffer, 4, lpOutBuffer, 6, out BytesReturned, IntPtr.Zero);
                                    if (!success)
                                    {
                                        int errorCode = Marshal.GetLastWin32Error();
                                    }
                                    if (BytesReturned == 6)
                                    {
                                        byte[] bytes = new byte[6];
                                        Marshal.Copy(lpOutBuffer, bytes, 0, bytes.Length);
                                        stringHwid.NdisHwidBlock.Add(BitConverter.ToString(bytes).Replace("-", " "));
                                        ushort value = AddInstanceHash(bytes, 6, true);
                                        if (!readable) value += 1;
                                        if (result.Contains(value) == false)
                                            result.Add((ushort)value);
                                    }
                                    Marshal.FreeHGlobal(lpOutBuffer);
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
            return result;
        }
        static ushort EnumInterfaceCallback(IntPtr hFile)
        {
            ushort result = 0;
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(0x124);
            uint BytesReturned;
            bool success = DeviceIoControl(hFile, IOCTL_BTH_GET_LOCAL_INFO, IntPtr.Zero, 0, lpOutBuffer, 0X124, out BytesReturned, IntPtr.Zero);
            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
            }
            if (BytesReturned == 0x124)
            {
                byte[] bytes = new byte[6];
                Marshal.Copy(new IntPtr(IntPtr.Size == 4 ? lpOutBuffer.ToInt32() + 8 : lpOutBuffer.ToInt64() + 8), bytes, 0, bytes.Length);
                string strHex = BitConverter.ToString(bytes.Reverse().ToArray()).Replace("-", "").ToLower();
                byte[] bytesIn = Encoding.Unicode.GetBytes(strHex).Concat(new byte[] { 0, 0 }).ToArray();
                result = AddInstanceHash(bytesIn, 0x1A, true);
            }
            return result;
        }
        #endregion        

        #region CollectInternal
        static List<ushort> CollectInternal(ref Guid ClassGuid, bool UnknownDevice, string DeviceName)
        {
            List<ushort> result = new List<ushort>();
            Guid NullGuid = Guid.Empty;
            IntPtr deviceInfoSet = SetupDiGetClassDevsW(ref ClassGuid, null, IntPtr.Zero, (int)(UnknownDevice ? DI_FUNCTION.DIF_PROPERTYCHANGE : DI_FUNCTION.DIF_FIRSTTIMESETUP));
            if (deviceInfoSet != IntPtr.Zero)
            {
                SP_DEVINFO_DATA sDevInfoData = new SP_DEVINFO_DATA();
                sDevInfoData.cbSize = (uint)Marshal.SizeOf(new SP_DEVINFO_DATA());
                for (uint i = 0; SetupDiEnumDeviceInfo(deviceInfoSet, i, ref sDevInfoData); i++)
                {
                    if (SetupDiEnumDeviceInfo(deviceInfoSet, (uint)i, ref sDevInfoData))
                    {
                        Guid myGuid = Guid.Empty;
                        if (UnknownDevice)
                        {
                            int cbsize = 0;
                            byte[] buffer = new byte[0];
                            if (HwidGetPnPDeviceRegistryProperty(deviceInfoSet, sDevInfoData, ref cbsize, ref buffer))
                            {
                                if ((cbsize & 0xFFFFFFFE) == 0x4E)
                                {
                                    string guidString = Encoding.Unicode.GetString(buffer).Replace("\0", "");
                                    myGuid = new Guid(guidString);
                                }
                            }
                        }
                        else
                        {
                            myGuid = ClassGuid;
                        }
                        if (sDevInfoData.ClassGuid == myGuid)
                        {
                            if (IsSoftwareDevice(new IntPtr(sDevInfoData.DevInst)) == false)
                            {
                                if (sDevInfoData.Reserved != IntPtr.Zero)
                                {
                                    int RequiredSize = 0;
                                    IntPtr buffer = Marshal.AllocHGlobal(512);
                                    if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref sDevInfoData, 1, 0, buffer, 0x800, ref RequiredSize))
                                    {
                                        string ControllerDeviceDesc = Marshal.PtrToStringUni(buffer);
                                        if (DeviceName == "Display")
                                        {
                                            stringHwid.DisplayHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        else if (DeviceName == "CDROM")
                                        {
                                            stringHwid.CdromHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        else if (DeviceName == "HDC")
                                        {
                                            stringHwid.HdcHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        else if (DeviceName == "SCSIADAPTER")
                                        {
                                            stringHwid.ScsiAdapterHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        else if (DeviceName == "PCMCIA")
                                        {
                                            stringHwid.PcmciaHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        else if (DeviceName == "AUDIO")
                                        {
                                            stringHwid.AudioHwidBlock.Add(ControllerDeviceDesc);
                                        }
                                        int length = ControllerDeviceDesc.Length * 2;
                                        length += 2;
                                        //Debug.Print(ControllerDeviceDesc);
                                        byte[] buff = new byte[RequiredSize];
                                        Marshal.Copy(buffer, buff, 0, RequiredSize);
                                        bool readable = HwidGetPnPRemovalPolicy(deviceInfoSet, sDevInfoData);
                                        ushort value = AddInstanceHash(buff, (uint)length, readable);
                                        if (!result.Contains(value)) result.Add(value);
                                    }
                                    Marshal.FreeHGlobal(buffer);
                                }
                            }
                        }
                    }
                }
            }
            SetupDiDestroyDeviceInfoList(deviceInfoSet);
            return result;
        }
        static bool IsSoftwareDevice(IntPtr DevInst)
        {

            IntPtr Buffer = Marshal.AllocHGlobal(206);
            int dwReqLen = 0x192;
            int regDataType = 0;
            int result = CM_Get_DevNode_Registry_PropertyW(DevInst, 0x17, ref regDataType, Buffer, ref dwReqLen, 0);
            if (result == 0)
            {
                //Debug.Print(Marshal.PtrToStringUni(Buffer));
                if ((regDataType != 1) || (dwReqLen < 2) || (Marshal.PtrToStringUni(Buffer).ToString().Trim() == "SWD"))
                {
                    return true;
                }
                else
                {
                    int status = 0;
                    int problem = 0;
                    if (CM_Get_DevNode_Status(ref status, ref problem, DevInst, 0) == 0)
                    {
                        if ((status & 1) == 0) return false;
                        IntPtr pdnDevInst = new IntPtr();
                        if (CM_Get_Parent(ref pdnDevInst, DevInst, 0) == 0)
                        {
                            if (CM_Get_Device_IDW(pdnDevInst, Buffer, 0xC9, 0) == 0)
                            {
                                if (Marshal.PtrToStringUni(Buffer).Contains("HTREE\\ROOT\\0"))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        static bool HwidGetPnPDeviceRegistryProperty(IntPtr hDevInfo, SP_DEVINFO_DATA devData, ref int cbsize, ref byte[] buffer)
        {
            int RequiredSize = 0;
            if (!SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devData, 8, 0, null, 0, out RequiredSize))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 13)
                {
                    return false;
                }
                else if (errorCode != 0x7A)
                {
                    return false;
                }
                buffer = new byte[RequiredSize];
                if (SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devData, 8, 0, buffer, RequiredSize, out RequiredSize))
                {
                    cbsize = RequiredSize;
                    return true;
                }
            }
            return false;
        }
        static bool HwidGetPnPRemovalPolicy(IntPtr hDevInfo, SP_DEVINFO_DATA devData)
        {
            int requiredSize = 0;
            byte[] propertyBuffer = new byte[4];
            bool success = SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devData, 0x1F, 0, propertyBuffer, 4, out requiredSize);
            if (success)
            {
                if (BitConverter.ToInt32(propertyBuffer, 0) != 1)
                    return (BitConverter.ToInt32(propertyBuffer, 0) - 2) > 3;
            }
            return true;
        }
        static bool NextInterface(IntPtr hDevInfo, ref Guid ClassGuid, ref SP_DEVICE_INTERFACE_DETAIL_DATA devDetail, ref SP_DEVINFO_DATA deviceInfoData, ref uint index)
        {
            uint nBytes = 0;
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData);
            bool hResult = SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref ClassGuid, index, ref deviceInterfaceData);
            if (hResult)
            {
                SetupDiGetDeviceInterfaceDetailW(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, out nBytes, IntPtr.Zero);
                if (nBytes > 6)
                {
                    SP_DEVICE_INTERFACE_DETAIL_DATA didd = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                    if (IntPtr.Size == 8)
                        didd.cbSize = 8;
                    else
                        didd.cbSize = 4 + Marshal.SystemDefaultCharSize;

                    SP_DEVINFO_DATA devData;
                    devData = new SP_DEVINFO_DATA();
                    devData.cbSize = (uint)Marshal.SizeOf(devData);
                    if (SetupDiGetDeviceInterfaceDetailW(hDevInfo, ref deviceInterfaceData, ref didd, nBytes, out nBytes, ref devData))
                    {
                        devDetail = didd;
                        deviceInfoData = devData;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool NextInterfaceDeviceHandle(IntPtr hDevInfo, ref Guid ClassGuid, ref bool readable, ref uint index, ref IntPtr hFile)
        {
            SP_DEVICE_INTERFACE_DETAIL_DATA devDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            SP_DEVINFO_DATA devData = new SP_DEVINFO_DATA();
            do
            {
                devDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                devData = new SP_DEVINFO_DATA();
                if (NextInterface(hDevInfo, ref ClassGuid, ref devDetail, ref devData, ref index))
                {
                    index++;
                    if (devData.cbSize >= 0)
                    {
                        bool isSoftwareDvices = IsSoftwareDevice(new IntPtr(devData.DevInst));
                        if (!isSoftwareDvices)
                        {
                            hFile = CreateFileW(devDetail.DevicePath, 0, 3, 0, OPEN_EXISTING, 0, 0);
                        }
                    }
                }
                else
                {
                    return false;
                }
            } while (hFile == IntPtr.Zero || hFile.ToInt32() == -1);
            if (hFile != IntPtr.Zero && hFile.ToInt32() != -1)
            {
                readable = HwidGetPnPRemovalPolicy(hDevInfo, devData);
                if (!readable)
                {
                    //CloseHandle(hFile);
                }
            }
            else
            {
                hDevInfo = IntPtr.Zero;
            }
            return true;
        }


        #endregion

        #region AddInstanceHash
        static ushort AddInstanceHash(byte[] buffer, uint cbsize, bool readable)
        {
            SHA256Managed hasher = new SHA256Managed();
            byte[] Bytes = hasher.ComputeHash(buffer.Take((int)cbsize).ToArray());
            Debug.Print(BitConverter.ToString(Bytes).Replace("-", " "));
            ushort result = (ushort)(((Bytes[1] << 8) | Bytes[0]) & 0xFFFE);
            if (!readable)
            {
                result |= 1;
            }
            return result;
        }
        #endregion

        

    }
}
