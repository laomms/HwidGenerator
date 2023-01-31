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
        static uint[] SHA256Magic = new uint[] {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5, 0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3, 0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC, 0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7, 0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13, 0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3, 0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5, 0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208, 0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2 };

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
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }
        public static String HwidCreateBlock(byte[] arrayHWID, int cbsize)
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
            Debug.Print(Encoding.Unicode.GetString(buffer) + Environment.NewLine);
            Debug.Print(Encoding.UTF8.GetString(buffer) + Environment.NewLine);
            uint[] bSHA256Init = SHA256Init();
            SHA256Update(ref bSHA256Init, buffer, cbsize);
            uint[] DST = SHA256Final(ref bSHA256Init);
            ushort result = (ushort)(DST[0] & 0xFFFE);
            if (!readable)
            {
                result |= 1;
            }
            return result;
        }
        #endregion

        #region SHA256
        static uint[] SHA256Init()
        {
            uint[] arr = new uint[0x1C * 4];
            uint[] SHA256Init = { 0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19, 0x00000000, 0x00000000 };
            Buffer.BlockCopy(SHA256Init, 0, arr, 0, SHA256Init.Length * 4);
            return arr;
        }
        static void SHA256Update(ref uint[] SHA256Init, byte[] buffer, uint cbsize)
        {
            int len = 0;
            byte[] bytes = new byte[0];
            if (buffer.Length % 4 != 0)
            {
                buffer = buffer.Concat(new byte[4 - (buffer.Length % 4)]).ToArray();
            }
            uint[] result = new uint[buffer.Length / 4];
            Buffer.BlockCopy(buffer, 0, result, 0, buffer.Length);  //byte[] to uint[]          
            byte[] bSHA256Init = SHA256Init.SelectMany(BitConverter.GetBytes).ToArray();
            var value1 = cbsize + bSHA256Init[0x24];
            var value2 = bSHA256Init[0x24] & 0x3F;
            int value3 = bSHA256Init[0x24] & 0x3F;
            bSHA256Init[0x24] = (byte)value1;
            SHA256Init[9] = (byte)value1;
            if (value1 < cbsize)
            {
                ++bSHA256Init[0x20];
            }
            var size = value2 + cbsize;
            if ((value2 != 0) && (size == value2 + cbsize) && ((uint)(value2 + cbsize) >= 0x40))
            {
                Buffer.BlockCopy(buffer, 0, SHA256Init, value2 + 0x28, 0x40 - value2);
                cbsize = (uint)(size - 64);
                uint[] buf = new uint[(SHA256Init.Length - 0x28) * 4];
                Buffer.BlockCopy(SHA256Init, 40, buf, 0, (SHA256Init.Length - 0x28) * 4);
                uint[] newbyte = SHA256Transform(ref SHA256Init, buf);
                bytes = new byte[buffer.Length - (0x40 - value3)];
                Buffer.BlockCopy(buffer, 0x40 - value3, bytes, 0, buffer.Length - (0x40 - value3));
                len = bytes.Length;
            }
            if (len >= 3)
            {
                if (cbsize >= 0x40)
                {
                    uint[] newArray = new uint[bSHA256Init.Length - 0x28];
                    Buffer.BlockCopy(SHA256Init, 0x28, newArray, 0, newArray.Length);
                    int i = (int)(cbsize >> 6);
                    do
                    {
                        if (cbsize > 0)
                        {
                            Buffer.BlockCopy(bytes, 0, newArray, 0, 0x40);
                            uint[] newbyte = SHA256Transform(ref SHA256Init, newArray);
                            Buffer.BlockCopy(bSHA256Init, 0, newArray, 0, 0x28);
                            Buffer.BlockCopy(buffer, 0x40, result, 0, buffer.Length - 0x40);
                            buffer = buffer.Skip(0x40).ToArray();
                            cbsize -= 0x40;
                            i--;
                        }
                    } while (i != 0);
                }
            }
            if (cbsize >= 0x40)
            {
                int flag = (int)(cbsize >> 6);
                do
                {
                    uint[] newbyte = SHA256Transform(ref SHA256Init, result);
                    cbsize -= 0x40;
                    if (cbsize > 0)
                    {
                        Buffer.BlockCopy(buffer, 0x40, result, 0, buffer.Length - 0x40);
                        buffer = buffer.Skip(0x40).ToArray();
                        --flag;
                    }
                } while (flag != 0);
            }
            if (cbsize > 0)
            {
                Buffer.BlockCopy(result, 0, SHA256Init, value3 + 40, (int)cbsize);
            }
        }
        static uint[] SHA256Transform(ref uint[] SHA256Init, uint[] buffer)
        {
            uint[] DST = SHA256Init;
            uint[] newbyte1 = new uint[16];
            int n = 0;
            uint value = 0;
            uint val = 0;
            while (n < 16)
            {
                byte[] b = BitConverter.GetBytes(buffer[n]).Reverse().ToArray();
                newbyte1[n] = BitConverter.ToUInt32(b, 0);
                n += 1;
            }
            var sign1 = SHA256Init[1];
            var sign4 = SHA256Init[4];
            var sign3 = SHA256Init[3];
            var sign0 = SHA256Init[0];
            var sign2 = SHA256Init[2];
            var sign5 = SHA256Init[5];
            var sign6 = SHA256Init[6];
            var sign7 = SHA256Init[7];
            uint m = 0;
            var _sign1 = sign1;
            var _sign4 = sign4;
            var _sign0 = sign0;
            while (true)
            {
                var value1 = sign7 + newbyte1[m] + SHA256Magic[m] + (_sign4 & sign5 ^ sign6 & ~sign4) + (RotateRight(sign4, 6) ^ RotateRight(sign4, 11) ^ RotateRight(sign4, 25));
                var value2 = value1 + sign3;
                var value3 = _sign0 ^ sign1;
                var value4 = _sign0 & sign1;
                var value5 = value1 + (RotateRight(_sign0, 2) ^ RotateRight(_sign0, 13) ^ RotateRight(_sign0, 22)) + (value4 ^ sign2 & value3);
                var value6 = sign6 + newbyte1[1 + m] + SHA256Magic[m + 1] + (value2 & _sign4 ^ sign5 & ~value2) + (RotateRight(value2, 6) ^ RotateRight(value2, 11) ^ RotateRight(value2, 25));
                var value7 = value6 + sign2;
                var value8 = value6 + (RotateRight(value5, 2) ^ RotateRight(value5, 13) ^ RotateRight(value5, 22)) + (value4 ^ value5 & value3);
                var value9 = value8;
                var value10 = sign5 + newbyte1[2 + m] + SHA256Magic[m + 2] + (value7 & value2 ^ _sign4 & ~value7) + (RotateRight(value7, 6) ^ RotateRight(value7, 11) ^ RotateRight(value7, 25));
                var value11 = value10 + _sign1;
                var value12 = value10 + (RotateRight(value9, 2) ^ RotateRight(value9, 13) ^ RotateRight(value9, 22)) + (_sign0 & value8 ^ value5 & (_sign0 ^ value8));
                var value13 = _sign4 + newbyte1[3 + m] + SHA256Magic[m + 3] + (value11 & value7 ^ value2 & ~value11) + (RotateRight(value11, 6) ^ RotateRight(value11, 11) ^ RotateRight(value11, 25));
                var value14 = value13 + _sign0;
                var value15 = value13 + (RotateRight(value12, 2) ^ RotateRight(value12, 13) ^ RotateRight(value12, 22)) + (value12 & value8 ^ value5 & (value12 ^ value8));
                var value16 = value2 + newbyte1[4 + m] + SHA256Magic[m + 4] + (value14 & value11 ^ value7 & ~value14) + (RotateRight(value14, 6) ^ RotateRight(value14, 11) ^ RotateRight(value14, 25));
                sign7 = value16 + value5;
                sign3 = value16 + (RotateRight(value15, 2) ^ RotateRight(value15, 13) ^ RotateRight(value15, 22)) + (value15 & value12 ^ value8 & (value15 ^ value12));
                var value21 = sign3;
                var value18 = value7 + newbyte1[5 + m] + SHA256Magic[m + 5] + (value14 & sign7 ^ value11 & ~sign7) + (RotateRight(sign7, 6) ^ RotateRight(sign7, 11) ^ RotateRight(sign7, 25));
                sign6 = value18 + value9;
                var values28 = value18 + (RotateRight(sign3, 2) ^ RotateRight(sign3, 13) ^ RotateRight(sign3, 22)) + (sign3 & value15 ^ value12 & (sign3 ^ value15));
                sign2 = values28;
                var value20 = value11 + newbyte1[6 + m] + SHA256Magic[m + 6] + (sign6 & sign7 ^ value14 & ~sign6) + (RotateRight(sign6, 6) ^ RotateRight(sign6, 11) ^ RotateRight(sign6, 25));
                sign5 = value20 + value12;
                _sign1 = value20 + (RotateRight(values28, 2) ^ RotateRight(values28, 13) ^ RotateRight(values28, 22)) + (values28 & sign3 ^ value15 & (values28 ^ sign3));
                var value22 = newbyte1[7 + m] + SHA256Magic[m + 7] + (sign5 & sign6 ^ sign7 & ~sign5) + (RotateRight(sign5, 6) ^ RotateRight(sign5, 11) ^ RotateRight(sign5, 25));
                m += 8;
                var value23 = value14 + value22;
                _sign4 = value23 + value15;
                var value24 = value23 + (RotateRight(_sign1, 2) ^ RotateRight(_sign1, 13) ^ RotateRight(_sign1, 22));
                var value25 = _sign1 & values28 ^ sign3 & (_sign1 ^ values28);
                var value26 = value24 + value25;
                _sign0 = value24 + value25;
                if (m >= 0x10)
                {
                    break;
                }
                sign4 = _sign4;
                sign1 = _sign1;
            }
            if (m < 0x40)
            {
                char a = (char)(m - 2);
                int b = (int)(m - 7);
                char c = (char)(m + 1);
                int d = (int)(m - 2);
                int e = (int)(m + 1);
                do
                {
                    var values1 = newbyte1[m & 0xF];
                    char t = (char)b;
                    int i = b + 1;
                    values1 += newbyte1[t & 0xF] + ((newbyte1[c & 0xF] >> 3) ^ RotateRight(newbyte1[c & 0xF], 7) ^ RotateRight(newbyte1[c & 0xF], 18)) + ((newbyte1[a & 0xF] >> 10) ^ RotateRight(newbyte1[a & 0xF], 17) ^ RotateRight(newbyte1[a & 0xF], 19));
                    newbyte1[m & 0xF] = values1;

                    var values2 = sign7 + values1 + SHA256Magic[m] + (_sign4 & sign5 ^ sign6 & ~_sign4) + (RotateRight(_sign4, 6) ^ RotateRight(_sign4, 11) ^ RotateRight(_sign4, 25));
                    var values3 = values2 + sign3;
                    int p = e + 1;
                    int q = p;
                    int y = (int)(d + 1);
                    int s = y;
                    var values4 = values2 + (RotateRight(_sign0, 2) ^ RotateRight(_sign0, 13) ^ RotateRight(_sign0, 22)) + (_sign0 & _sign1 ^ sign2 & (_sign0 ^ _sign1));
                    var values5 = newbyte1[(m + 2) & 0xF];
                    var values6 = newbyte1[(m + 1) & 0xF];
                    int z = i++ & 0xF;
                    values6 += newbyte1[z] + ((values5 >> 3) ^ RotateRight(values5, 7) ^ RotateRight(values5, 18)) + ((newbyte1[y & 0xF] >> 10) ^ RotateRight(newbyte1[y & 0xF], 17) ^ RotateRight(newbyte1[y & 0xF], 19));
                    newbyte1[(m + 1) & 0xF] = values6;

                    var values8 = sign6 + values6 + SHA256Magic[m + 1] + (values3 & _sign4 ^ sign5 & ~values3) + (RotateRight(values3, 6) ^ RotateRight(values3, 11) ^ RotateRight(values3, 25));
                    var values9 = values8 + sign2;
                    var values10 = values8 + (RotateRight(values4, 2) ^ RotateRight(values4, 13) ^ RotateRight(values4, 22)) + (_sign0 & _sign1 ^ values4 & (_sign0 ^ _sign1));
                    int r = q + 1;
                    int v = r;
                    var values11 = newbyte1[(m + 3) & 0xF];
                    var values12 = newbyte1[(m + 2) & 0xF];
                    int x = i++ & 0xF;
                    values12 += newbyte1[x] + ((values11 >> 3) ^ RotateRight(values11, 7) ^ RotateRight(values11, 18)) + ((newbyte1[(y + 1) & 0xF] >> 10) ^ RotateRight(newbyte1[(y + 1) & 0xF], 17) ^ RotateRight(newbyte1[(y + 1) & 0xF], 19));
                    newbyte1[(m + 2) & 0xF] = values12;

                    var values13 = sign5 + values12 + SHA256Magic[m + 2] + (values9 & values3 ^ _sign4 & ~values9) + (RotateRight(values9, 6) ^ RotateRight(values9, 11) ^ RotateRight(values9, 25));
                    var values14 = values13 + _sign1;
                    x = LOBYTE(v + 1);
                    y = LOBYTE(y + 2);
                    var values15 = values13 + (RotateRight(values10, 2) ^ RotateRight(values10, 13) ^ RotateRight(values10, 22)) + (_sign0 & values10 ^ values4 & (_sign0 ^ values10));
                    var values16 = newbyte1[(m + 3) & 0xF];
                    values16 += newbyte1[i++ & 0xF] + ((newbyte1[x & 0xF] >> 3) ^ RotateRight(newbyte1[x & 0xF], 7) ^ RotateRight(newbyte1[x & 0xF], 18)) + ((newbyte1[y & 0xF] >> 10) ^ RotateRight(newbyte1[y & 0xF], 17) ^ RotateRight(newbyte1[y & 0xF], 19));
                    newbyte1[(m + 3) & 0xF] = values16;

                    var values17 = _sign4 + values16 + SHA256Magic[m + 3] + (values14 & values9 ^ values3 & ~values14) + (RotateRight(values14, 6) ^ RotateRight(values14, 11) ^ RotateRight(values14, 25));
                    var values18 = values17 + _sign0;
                    x = LOBYTE(v + 2);
                    var values19 = values17 + (RotateRight(values15, 2) ^ RotateRight(values15, 13) ^ RotateRight(values15, 22)) + (values15 & values10 ^ values4 & (values15 ^ values10));
                    var values20 = newbyte1[(m + 4) & 0xF];
                    values20 += newbyte1[i++ & 0xF] + ((newbyte1[x & 0xF] >> 3) ^ RotateRight(newbyte1[x & 0xF], 7) ^ RotateRight(newbyte1[x & 0xF], 18)) + ((newbyte1[(s + 3) & 0xF] >> 10) ^ RotateRight(newbyte1[(s + 3) & 0xF], 17) ^ RotateRight(newbyte1[(s + 3) & 0xF], 19));
                    newbyte1[(m + 4) & 0xF] = values20;

                    var values21 = values3 + values20 + SHA256Magic[m + 4] + (values18 & values14 ^ values9 & ~values18) + (RotateRight(values18, 6) ^ RotateRight(values18, 11) ^ RotateRight(values18, 25));
                    sign7 = values21 + values4;
                    x = LOBYTE(v + 3);
                    var value21 = values21 + (RotateRight(values19, 2) ^ RotateRight(values19, 13) ^ RotateRight(values19, 22)) + (values19 & values15 ^ values10 & (values19 ^ values15));
                    var values22 = newbyte1[(m + 5) & 0xF];
                    values22 += newbyte1[i & 0xF] + ((newbyte1[x & 0xF] >> 3) ^ RotateRight(newbyte1[x & 0xF], 7) ^ RotateRight(newbyte1[x & 0xF], 18)) + ((newbyte1[(s + 4) & 0xF] >> 10) ^ RotateRight(newbyte1[(s + 4) & 0xF], 17) ^ RotateRight(newbyte1[(s + 4) & 0xF], 19));
                    newbyte1[(m + 5) & 0xF] = values22;

                    var values23 = values9 + values22 + SHA256Magic[m + 5] + (values18 & sign7 ^ values14 & ~sign7) + (RotateRight(sign7, 6) ^ RotateRight(sign7, 11) ^ RotateRight(sign7, 25));
                    sign6 = values23 + values10;
                    ++i;
                    x = LOBYTE(v + 4);
                    sign2 = values23 + (RotateRight(value21, 2) ^ RotateRight(value21, 13) ^ RotateRight(value21, 22)) + (value21 & values19 ^ values15 & (value21 ^ values19));
                    var values24 = newbyte1[(m + 6) & 0xF];
                    values24 += newbyte1[i & 0xF] + ((newbyte1[x & 0xF] >> 3) ^ RotateRight(newbyte1[x & 0xF], 7) ^ RotateRight(newbyte1[x & 0xF], 18)) + ((newbyte1[(s + 5) & 0xF] >> 10) ^ RotateRight(newbyte1[(s + 5) & 0xF], 17) ^ RotateRight(newbyte1[(s + 5) & 0xF], 19));
                    newbyte1[(m + 6) & 0xF] = values24;

                    var values25 = values14 + values24 + SHA256Magic[m + 6] + (sign6 & sign7 ^ values18 & ~sign6) + (RotateRight(sign6, 6) ^ RotateRight(sign6, 11) ^ RotateRight(sign6, 25));
                    sign5 = values25 + values15;
                    ++i;
                    int h = v + 5;
                    int f = h;
                    int g = s + 6;
                    int w = g;
                    _sign1 = values25 + (RotateRight(sign2, 2) ^ RotateRight(sign2, 13) ^ RotateRight(sign2, 22)) + (sign2 & value21 ^ values19 & (sign2 ^ value21));
                    var values26 = newbyte1[(m + 7) & 0xF];
                    values26 += newbyte1[(i & 0xF)] + ((newbyte1[h & 0xF] >> 3) ^ RotateRight(newbyte1[h & 0xF], 7) ^ RotateRight(newbyte1[h & 0xF], 18)) + ((newbyte1[g & 0xF] >> 10) ^ RotateRight(newbyte1[g & 0xF], 17) ^ RotateRight(newbyte1[g & 0xF], 19));
                    newbyte1[(m + 7) & 0xF] = values26;

                    var values27 = values26 + SHA256Magic[m + 7] + (sign5 & sign6 ^ sign7 & ~sign5) + (RotateRight(sign5, 6) ^ RotateRight(sign5, 11) ^ RotateRight(sign5, 25));
                    val = sign2;
                    var values29 = values18 + values27;
                    _sign4 = values29 + values19;
                    sign3 = value21;
                    m += 8;
                    value = values29 + (RotateRight(_sign1, 2) ^ RotateRight(_sign1, 13) ^ RotateRight(_sign1, 22)) + (_sign1 & sign2 ^ value21 & (_sign1 ^ sign2));
                    c = (char)(f + 1);
                    b = i + 1;
                    a = (char)(g + 1);
                    _sign0 = value;
                    e = f + 1;
                    d = w + 1;
                } while (m < 0x40);
            }
            var values = value + sign0;
            DST[0] = values;
            DST[3] += sign3;
            DST[2] += val;
            DST[1] += _sign1;
            DST[4] += _sign4;
            DST[5] += sign5;
            DST[6] += sign6;
            DST[7] += sign7;
            Buffer.BlockCopy(DST, 0, SHA256Init, 0, DST.Length * 4);
            return newbyte1;
        }
        static uint[] SHA256Final(ref uint[] bSHA256Init)
        {
            uint cbsize = 0x40 - (bSHA256Init[9] & 0x3F);
            if (cbsize <= 8)
            {
                cbsize += 0x40;
            }
            byte[] Dst = new byte[cbsize - 8];
            Dst[0] = 0x80;

            uint value1 = (bSHA256Init[9] >> 29) | 8 * bSHA256Init[8];
            uint value2 = 8 * bSHA256Init[9];
            Dst = Dst.Concat(BitConverter.GetBytes(value1).Reverse().ToArray()).Concat(BitConverter.GetBytes(value2).Reverse().ToArray()).ToArray();
            SHA256Update(ref bSHA256Init, Dst, cbsize);
            //uint[] buffer = new uint[(bSHA256Init.Length - index) * 4];
            //Buffer.BlockCopy(bSHA256Init, 0, buffer, 0, (bSHA256Init.Length - index) * 4);
            uint[] newArray = new uint[8];
            int i = 0;
            do
            {
                uint val = BitConverter.ToUInt32(BitConverter.GetBytes(bSHA256Init[i]).Reverse().ToArray(), 0);
                newArray[i] = val;
                i++;
            } while (i < 8);
            bSHA256Init[8] = 0;
            bSHA256Init[9] = 0;
            bSHA256Init[0] = 0x6A09E667;
            bSHA256Init[1] = 0xBB67AE85;
            bSHA256Init[2] = 0x3C6EF372;
            bSHA256Init[3] = 0xA54FF53A;
            bSHA256Init[4] = 0x510E527F;
            bSHA256Init[5] = 0x9B05688C;
            bSHA256Init[6] = 0x1F83D9AB;
            bSHA256Init[7] = 0x5BE0CD19;
            int n = 64 / 4;
            i = 0;
            do
            {
                bSHA256Init[10 + i] = 0;
                i++;
                --n;
            } while (n != 0);
            return newArray;
        }

        #endregion

    }
}
