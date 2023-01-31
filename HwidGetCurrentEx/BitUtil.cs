using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HwidGetCurrentEx
{
    internal static class BitUtil
    {

        public static string ReadNullTerminatedAnsiString(byte[] buffer, int offset)
        {
            StringBuilder builder = new StringBuilder();
            char c = (char)buffer[offset];
            while (c != '\0')
            {
                builder.Append(c);
                offset++;
                c = (char)buffer[offset];
            }
            return builder.ToString();
        }
        public static byte[] StrToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }

        public static ulong array2ulong(byte[] bytes, int start, int length)
        {
            bytes = bytes.Skip(start).Take(length).ToArray();

            ulong acc = 0;
            foreach (var b in bytes) acc = (acc * 0x100) + b;

            return acc;
        }
        public static T[] Concats<T>(this T[] array1, params T[] array2) => ConcatArray(array1, array2);
        public static T[] ConcatArray<T>(params T[][] arrays)
        {
            int l, i;

            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++) ;

            var a = new T[l];

            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++)
                arrays[i].CopyTo(a, l);

            return a;
        }
        public static byte LOBYTE(int a)
        {
            return (byte)((short)a & 0xff);
        }
        public static short MAKEWORD(byte a, byte b)
        {
            return ((short)(((byte)(a & 0xff)) | ((short)((byte)(b & 0xff))) << 8));
        }

        public static byte LOBYTE(short a)
        {
            return ((byte)(a & 0xff));
        }

        public static byte HIBYTE(short a)
        {
            return ((byte)(a >> 8));
        }

        public static int MAKELONG(short a, short b)
        {
            return (((int)(a & 0xffff)) | (((int)(b & 0xffff)) << 16));
        }

        public static short HIWORD(int a)
        {
            return ((short)(a >> 16));
        }

        public static short LOWORD(int a)
        {
            return ((short)(a & 0xffff));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset)
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (value << offset) | (value >> (32 - offset));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft64(ulong value, int offset)
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (value << offset) | (value >> (64 - offset));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, int offset)
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (value >> offset) | (value << (32 - offset));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight64(ulong value, int offset)
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (value >> offset) | (value << (64 - offset));
#endif
        }

        public static int HIDWORD(long intValue)
        {
            return Convert.ToInt32(intValue >> 32);
        }

        public static int LODWORD(long intValue)
        {
            long tmp = intValue << 32;
            return Convert.ToInt32(tmp >> 32);
        }

        public static short PAIR(sbyte high, sbyte low)
        {
            return (short)((((short)high) << sizeof(sbyte) * 8) | (byte)low);
        }
        public static int PAIR(short high, int low)
        {
            return (((int)high) << sizeof(short) * 8) | (ushort)low;
        }
        public static long PAIR(int high, long low)
        {
            return (((long)high) << sizeof(int) * 8) | (uint)low;
        }

        public static ushort PAIR(byte high, ushort low)
        {
            return (ushort)((((ushort)high) << sizeof(byte) * 8) | (byte)low);
        }

        public static uint PAIR(ushort high, uint low)
        {
            return (uint)((((uint)high) << sizeof(ushort) * 8) | (ushort)low);
        }

        public static ulong PAIR(uint high, ulong low)
        {
            return (ulong)((((ulong)high) << sizeof(uint) * 8) | (uint)low);
        }

        public static int adc(uint first, uint second, ref uint carry)
        {
            uint res;
            uint carry_out = 0;

            if (carry == 0)
            {
                res = first + second;
                carry = (uint)((res < first) && (res < second) ? 1 : 0);

                return (int)res;
            }

            res = (uint)adc(first, second, ref carry_out);
            if (carry != 0)
            {
                res++;
                carry_out |= (uint)(res == 0 ? 1 : 0);
            }

            carry = carry_out;

            return (int)res;
        }

    }

    public static class MinWinDef
    {
        internal static Func<object, object, object> MAKEWORD = (a, b) => ((ushort)(((byte)(((ulong)(a)) & 0xff)) | ((ushort)((byte)(((ulong)(b)) & 0xff))) << 8));
        internal static Func<object, object, object> MAKELONG = (a, b) => ((ulong)(((ushort)(((ulong)(a)) & 0xff)) | ((ushort)((byte)(((ulong)(b)) & 0xff))) << 8));
        internal static Func<object, object> LOWORD = l => ((ushort)(((ulong)(l)) & 0xffff));
        internal static Func<object, object> HIWORD = (l) => ((ushort)((((ulong)(l)) >> 16) & 0xffff));
        internal static Func<object, object> LOBYTE = (w) => ((byte)(((ulong)(w)) & 0xff));
        internal static Func<object, object> HIBYTE = (w) => ((byte)((((ulong)(w)) >> 8) & 0xff));

        internal static Func<object, object> GET_WHEEL_DELTA_WPARAM = (wParam) => ((short)HIWORD(wParam));
        internal static Func<object, object> GET_KEYSTATE_WPARAM = (wParam) => (LOWORD(wParam));
        internal static Func<object, object> GET_NCHITTEST_WPARAM = (wParam) => ((short)LOWORD(wParam));
        internal static Func<object, object> GET_XBUTTON_WPARAM = (wParam) => (HIWORD(wParam));

    }

}