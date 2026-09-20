using System;
using System.Security.Cryptography;

namespace SignPKCS.Managed
{
    
    public static class VRsaVaultSignPKCS
    {
        /// <summary>
        /// 对 SHA-256 摘要签名，返回大端签名（长度 = ModulusSize）。
        /// 与原生 VRSAVaultSignPKCS 逐字节一致。
        /// </summary>
        public static byte[] SignHash(IVbnRsaVault vault, byte[] sha256Hash)
        {
            if (vault == null) throw new ArgumentNullException(nameof(vault));
            if (sha256Hash == null) throw new ArgumentNullException(nameof(sha256Hash));

            int cb = vault.ModulusSize;
            if (sha256Hash.Length + Pkcs1SigningFormat.Sha256DigestInfoPrefix.Length + 3 > cb)
                throw new CryptographicException("NTE_BAD_LEN (0x80090004)：模数对这段摘要来说太短。");

            // 1) ApplyPKCS1SigningFormat：小端 EM 块
            byte[] block = Pkcs1SigningFormat.EncodeLittleEndian(cb, sha256Hash);

            // 2) VbnRsaVault_ModExpPriv_clear：块 = 块^d mod n，仍为小端
            vault.ModExpPriv(block);

            // 3) vMemrev：翻成大端，得到最终签名
            Pkcs1SigningFormat.Memrev(block);
            return block;
        }

        /// <summary>对消息本身（而不是摘要）做 SHA-256 后签名。</summary>
        public static byte[] SignData(IVbnRsaVault vault, byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
                return SignHash(vault, sha.ComputeHash(data));
        }

        /// <summary>
        /// 用不透明密钥句柄（RSA 实例）验证签名 —— 对应原生链路的
        /// 输出校验，也用来证明托管实现是对的。
        /// </summary>
        public static bool VerifyHash(byte[] modulusBe, byte[] exponentBe, byte[] sha256Hash, byte[] signatureBe)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(new RSAParameters { Modulus = modulusBe, Exponent = exponentBe });
                return rsa.VerifyHash(sha256Hash, signatureBe, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        /// 当你直接持有 RSA 私钥时，"这个算法" 在 .NET 里就是这一行 ——
        /// 内部做的事与上面 SignHash 完全相同（EMSA-PKCS1-v1_5 + 私钥模幂 + 大端输出）。
        /// </summary>
        public static byte[] SignHashWithKey(RSA rsa, byte[] sha256Hash)
            => rsa.SignHash(sha256Hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
