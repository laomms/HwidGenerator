using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SignPKCS.Managed
{
  
    public interface IVbnRsaVault : IDisposable
    {
        /// <summary>模数字节数 cbParams（RSA-2048 = 256）。</summary>
        int ModulusSize { get; }

        /// <summary>公开指数（原 GetPubParams 的输出，实测恒为 0x10001）。</summary>
        uint PublicExponent { get; }

        /// <summary>模数，大端（常规）顺序。用于验签/展示。</summary>
        byte[] Modulus { get; }

        /// <summary>
        /// VbnRsaVault_ModExpPriv_clear 的等价物：原地把 <paramref name="leBlock"/>
        /// 当作小端整数做 x^d mod n。入参出参都是 ModulusSize 字节。
        /// </summary>
        void ModExpPriv(byte[] leBlock);
    }

    /// <summary>
    /// 后端 A：P/Invoke 到你自己的 SignPKCS.dll。不需要私钥，今天就能跑。
    ///
    /// 实测签名（x86，均为小端）：
    ///   unsigned char* __stdcall VbnRsaVault_GetPubParams(ULONG* pdwPubExp, ULONG* pcbModulus);
    ///   int            __stdcall VbnRsaVault_ModExpPriv_clear(void* pOut, const void* pIn);
    /// </summary>
    public sealed class NativeVbnRsaVault : IVbnRsaVault
    {
        private const string Dll = "SignPKCS.dll";

        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr VbnRsaVault_GetPubParams(out uint pdwPubExp, out uint pcbModulus);

        [DllImport(Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern int VbnRsaVault_ModExpPriv_clear(byte[] pOut, byte[] pIn);

        public int ModulusSize { get; private set; }
        public uint PublicExponent { get; private set; }
        public byte[] Modulus { get; private set; }

        public NativeVbnRsaVault()
        {
            uint e, cb;
            IntPtr p = VbnRsaVault_GetPubParams(out e, out cb);
            if (p == IntPtr.Zero || cb == 0 || cb > 4096)
                throw new CryptographicException("VbnRsaVault_GetPubParams 返回了无效的模数。");

            PublicExponent = e;
            ModulusSize = (int)cb;

            byte[] le = new byte[cb];                  // DLL 里模数是小端存的
            Marshal.Copy(p, le, 0, (int)cb);
            Modulus = Pkcs1SigningFormat.Reverse(le);  // 翻成大端
        }

        public void ModExpPriv(byte[] leBlock)
        {
            if (leBlock == null) throw new ArgumentNullException(nameof(leBlock));
            if (leBlock.Length != ModulusSize)
                throw new ArgumentException("块长度必须等于 ModulusSize。", nameof(leBlock));

            if (VbnRsaVault_ModExpPriv_clear(leBlock, leBlock) == 0)
                throw new CryptographicException("VbnRsaVault_ModExpPriv_clear 失败。");
        }

        public void Dispose() { }
    }

    /// <summary>
    /// 后端 B：纯托管。需要你自己提供 RSA 私钥 (n, e, d)。
    ///
    /// 刻意用 BigInteger.ModPow 而不是 RSA.SignHash，是为了像原生 vault 一样
    /// 在**小端数据块**上做原始私钥运算，可以逐字节对照。
    /// </summary>
    public sealed class ManagedRsaVault : IVbnRsaVault
    {
        private readonly BigInteger _n;
        private readonly BigInteger _d;

        public int ModulusSize { get; private set; }
        public uint PublicExponent { get; private set; }
        public byte[] Modulus { get; private set; }

        public ManagedRsaVault(RSAParameters key)
        {
            if (key.Modulus == null || key.D == null)
                throw new ArgumentException("需要包含私钥指数 D 的 RSAParameters。", nameof(key));

            // RSAParameters 一律是大端
            _n = BigIntegerFromBigEndian(key.Modulus);
            _d = BigIntegerFromBigEndian(key.D);

            Modulus = (byte[])key.Modulus.Clone();
            ModulusSize = key.Modulus.Length;
            PublicExponent = key.Exponent != null && key.Exponent.Length > 0
                ? (uint)BigIntegerFromBigEndian(key.Exponent)
                : 0x10001u;
        }

        public void ModExpPriv(byte[] leBlock)
        {
            if (leBlock == null) throw new ArgumentNullException(nameof(leBlock));
            if (leBlock.Length != ModulusSize)
                throw new ArgumentException("块长度必须等于 ModulusSize。", nameof(leBlock));

            BigInteger m = BigIntegerFromLittleEndian(leBlock);   // 块本身就是小端
            BigInteger c = BigInteger.ModPow(m, _d, _n);

            byte[] le = c.ToByteArray();                                   // 小端输出
            Array.Clear(leBlock, 0, leBlock.Length);
            Buffer.BlockCopy(le, 0, leBlock, 0, Math.Min(le.Length, leBlock.Length));
        }

        public void Dispose() { }

        /// <summary>从 Windows CSP 密钥容器建一个托管 vault（会导出私钥到 RSAParameters）。</summary>
        public static ManagedRsaVault FromKeyContainer(string keyName,
                                                       int providerType = 24, /* PROV_RSA_AES */
                                                       CspProviderFlags flags = CspProviderFlags.UseExistingKey)
        {
            CspParameters cp = new CspParameters(providerType)
            {
                KeyContainerName = keyName,
                Flags = flags
            };
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp))
            {
                RSAParameters p = rsa.ExportParameters(true);
                return new ManagedRsaVault(p);
            }
        }

        /// <summary>
        /// BigInteger(byte[]) 把输入当**有符号**小端补码，所以末尾补 0 强制为正。
        /// 这两个 helper 是"大端/小端 -> 正整数"的唯一入口，别再混用。
        /// </summary>
        private static BigInteger BigIntegerFromLittleEndian(byte[] le)
        {
            byte[] padded = new byte[le.Length + 1];
            Buffer.BlockCopy(le, 0, padded, 0, le.Length);
            return new BigInteger(padded);
        }

        private static BigInteger BigIntegerFromBigEndian(byte[] be)
        {
            byte[] le = (byte[])be.Clone();
            Array.Reverse(le);
            return BigIntegerFromLittleEndian(le);
        }
    }
}
