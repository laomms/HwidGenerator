using System;
using System.Numerics;
using System.Security.Cryptography;

namespace HwidGenerator
{
    /// <summary>
    /// 纯托管 PKCS#1 v1.5 + SHA-256 签名器。
    /// 只需要 n, e, d，不依赖 CRT 参数、不依赖 CSP、不依赖 SignPKCS.dll。
    /// 兼容 .NET Framework 4.x（不使用 isUnsigned/isBigEndian 重载）。
    /// </summary>
    internal sealed class RsaPkcs1Signer : IDisposable
    {
        private static readonly byte[] Sha256DigestInfoPrefix = new byte[]
        {
            0x30, 0x31, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01,
            0x65, 0x03, 0x04, 0x02, 0x01, 0x05, 0x00, 0x04, 0x20
        };

        private readonly BigInteger _n;
        private readonly BigInteger _d;
        private readonly uint _e;
        private readonly byte[] _modulusBe;
        private readonly byte[] _exponentBe;

        public RsaPkcs1Signer(RSAParameters priv)
        {
            if (priv.Modulus == null || priv.Modulus.Length == 0)
                throw new ArgumentException("Modulus 为空。", nameof(priv));
            if (priv.D == null || priv.D.Length == 0)
                throw new ArgumentException("D 为空（签名必须有私钥指数 d）。", nameof(priv));
            if (priv.Exponent == null || priv.Exponent.Length == 0)
                throw new ArgumentException("Exponent 为空。", nameof(priv));

            _modulusBe = priv.Modulus;
            _exponentBe = priv.Exponent;

            _n = BytesToBigIntegerBe(priv.Modulus);
            _d = BytesToBigIntegerBe(priv.D);

            uint e = 0;
            foreach (byte b in priv.Exponent) e = (e << 8) | b;
            _e = e;
        }

        public int ModulusSize => _modulusBe.Length;
        public uint PublicExponent => _e;
        public byte[] Modulus => _modulusBe;
        public byte[] Exponent => _exponentBe;

        public byte[] SignHash(byte[] sha256Hash)
        {
            if (sha256Hash == null || sha256Hash.Length != 32)
                throw new ArgumentException("需要 32 字节 SHA-256 摘要。", nameof(sha256Hash));

            int k = _modulusBe.Length;
            int tLen = Sha256DigestInfoPrefix.Length + sha256Hash.Length;
            if (k < tLen + 11)
                throw new CryptographicException("模数太短，装不下 PKCS#1 v1.5 签名块。");

            int psLen = k - tLen - 3;
            byte[] em = new byte[k];
            em[0] = 0x00;
            em[1] = 0x01;
            for (int i = 0; i < psLen; i++) em[2 + i] = 0xFF;
            em[2 + psLen] = 0x00;
            Buffer.BlockCopy(Sha256DigestInfoPrefix, 0, em, 3 + psLen, Sha256DigestInfoPrefix.Length);
            Buffer.BlockCopy(sha256Hash, 0, em, 3 + psLen + Sha256DigestInfoPrefix.Length, sha256Hash.Length);

            BigInteger m = BytesToBigIntegerBe(em);
            BigInteger s = BigInteger.ModPow(m, _d, _n);

            return BigIntegerToBytesBeFixed(s, k);
        }

        public byte[] SignData(byte[] data)
        {
            using (var sha = SHA256.Create())
                return SignHash(sha.ComputeHash(data));
        }

        public static bool VerifyHash(byte[] modulusBe, byte[] exponentBe, byte[] sha256Hash, byte[] signatureBe)
        {
            if (modulusBe == null || modulusBe.Length == 0) return false;
            if (exponentBe == null || exponentBe.Length == 0) return false;
            if (sha256Hash == null || sha256Hash.Length != 32) return false;
            if (signatureBe == null || signatureBe.Length == 0) return false;

            try
            {
                int k = modulusBe.Length;
                BigInteger n = BytesToBigIntegerBe(modulusBe);
                BigInteger e = BytesToBigIntegerBe(exponentBe);
                BigInteger s = BytesToBigIntegerBe(signatureBe);

                BigInteger m = BigInteger.ModPow(s, e, n);
                byte[] em = BigIntegerToBytesBeFixed(m, k);

                // 校验 EM = 0x00 0x01 FF..FF 0x00 DigestInfo
                if (em[0] != 0x00 || em[1] != 0x01) return false;
                int i = 2;
                while (i < k && em[i] == 0xFF) i++;
                if (i < 10) return false;                 // PS 至少 8 字节
                if (i >= k || em[i] != 0x00) return false;
                i++;

                int tLen = Sha256DigestInfoPrefix.Length + sha256Hash.Length;
                if (k - i != tLen) return false;

                for (int j = 0; j < Sha256DigestInfoPrefix.Length; j++)
                    if (em[i + j] != Sha256DigestInfoPrefix[j]) return false;
                for (int j = 0; j < sha256Hash.Length; j++)
                    if (em[i + Sha256DigestInfoPrefix.Length + j] != sha256Hash[j]) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ---- 大端 byte[] <-> BigInteger（net48 兼容） ----

        /// <summary>大端无符号字节数组 → BigInteger。</summary>
        private static BigInteger BytesToBigIntegerBe(byte[] be)
        {
            if (be == null || be.Length == 0) return BigInteger.Zero;

            // 去掉前导 0，避免被判为负数
            int start = 0;
            while (start < be.Length - 1 && be[start] == 0) start++;

            // 反转成小端
            int len = be.Length - start;
            byte[] le = new byte[len];
            for (int i = 0; i < len; i++) le[i] = be[be.Length - 1 - i];

            // 如果最高位是 1，补一个 0 字节防负数
            if ((le[len - 1] & 0x80) != 0)
            {
                byte[] le2 = new byte[len + 1];
                Buffer.BlockCopy(le, 0, le2, 0, len);
                le = le2;
            }
            return new BigInteger(le);
        }

        /// <summary>BigInteger → 固定长度 k 字节的大端无符号数组（左补 0）。</summary>
        private static byte[] BigIntegerToBytesBeFixed(BigInteger v, int k)
        {
            if (v.Sign < 0) throw new CryptographicException("BigInteger 为负，非法。");

            byte[] le = v.ToByteArray();   // 小端补码
            // 去掉末尾的 0 符号字节
            int leLen = le.Length;
            while (leLen > 1 && le[leLen - 1] == 0) leLen--;

            if (leLen > k) throw new CryptographicException("BigInteger 超出模数长度。");

            byte[] be = new byte[k];
            for (int i = 0; i < leLen; i++)
                be[k - 1 - i] = le[i];
            return be;
        }

        public void Dispose() { }
    }
}