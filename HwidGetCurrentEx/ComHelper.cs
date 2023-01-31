using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HwidGetCurrentEx
{


    public static partial class ComHelper
    {
        public struct MyGuid
        {
            public int Data1;
            public short Data2;
            public short Data3;
            public byte[] Data4;

            public MyGuid(Guid g)
            {
                byte[] gBytes = g.ToByteArray();
                Data1 = BitConverter.ToInt32(gBytes, 0);
                Data2 = BitConverter.ToInt16(gBytes, 4);
                Data3 = BitConverter.ToInt16(gBytes, 6);
                Data4 = new Byte[8];
                Buffer.BlockCopy(gBytes, 8, Data4, 0, 8);
            }

            public Guid ToGuid()
            {
                return new Guid(Data1, Data2, Data3, Data4);
            }
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("eb89a21b-1f9c-4093-9a4d-05d4002543f6")]
        public interface IUnknown
        {
            int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid);
            [PreserveSig]
            int AddRef(IntPtr pUnk);
            [PreserveSig]
            int Release(IntPtr pUnk);
        }

        [ClassInterface(ClassInterfaceType.None), Guid("eb89a21b-1f9c-4093-9a4d-05d4002543f6")]
        public class MyUnknown : IUnknown
        {
            public int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid)
            {
                return Marshal.QueryInterface(pUnk, ref riid, out pVoid);
            }
            public int AddRef(IntPtr pUnk)
            {
                return Marshal.AddRef(pUnk);
            }
            public int Release(IntPtr pUnk)
            {
                return Marshal.Release(pUnk);
            }
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("dca42645-c410-4859-ab3c-9e9c563c57bb")]
        public interface ISppNamedParamsReadWrite
        {
            int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid);
        }

        [ClassInterface(ClassInterfaceType.None), Guid("dca42645-c410-4859-ab3c-9e9c563c57bb")]
        public class MySppNamedParamsReadWrite : ISppNamedParamsReadWrite
        {
            public int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid)
            {
                return Marshal.QueryInterface(pUnk, ref riid, out pVoid);
            }
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("22F58556-C467-43CD-98FF-7DBCADB2F661")]
        public interface ISppNamedParamsReadOnly
        {
            int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid);
        }

        [ClassInterface(ClassInterfaceType.None), Guid("22F58556-C467-43CD-98FF-7DBCADB2F661")]
        public class MySppNamedParamsReadOnly : ISppNamedParamsReadOnly
        {
            public int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid)
            {
                return Marshal.QueryInterface(pUnk, ref riid, out pVoid);
            }
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("96B97320-ED0E-4D9F-B390-6C17EAF67277")]
        public interface ISppParamsReadWrite
        {
            int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid);
        }

        [ClassInterface(ClassInterfaceType.None), Guid("96B97320-ED0E-4D9F-B390-6C17EAF67277")]
        public class MySppParamsReadWrite : ISppParamsReadWrite
        {
            public int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid)
            {
                return Marshal.QueryInterface(pUnk, ref riid, out pVoid);
            }
        }

        [ComVisible(false)]
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("BE73DD34-4DAD-4AC5-BBE0-7930F45CED73")]
        public interface ISppParamsReadOnly
        {
            int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid);
        }

        [ClassInterface(ClassInterfaceType.None), Guid("BE73DD34-4DAD-4AC5-BBE0-7930F45CED73")]
        public class MySppParamsReadOnly : ISppParamsReadOnly
        {
            public int QueryInterface(IntPtr pUnk, ref Guid riid, out IntPtr pVoid)
            {
                return Marshal.QueryInterface(pUnk, ref riid, out pVoid);
            }
        }
    }
}
