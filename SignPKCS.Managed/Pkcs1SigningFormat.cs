using System;

namespace SignPKCS.Managed
{
   
    public static class Pkcs1SigningFormat
    {
        /// <summary>EMSA-PKCS1-v1_5 中 SHA-256 的 DigestInfo 前缀（大端/DER 顺序）。</summary>
        public static readonly byte[] Sha256DigestInfoPrefix =
        {
            0x30, 0x31, 0x30, 0x0D, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01,
            0x65, 0x03, 0x04, 0x02, 0x01, 0x05, 0x00, 0x04, 0x20
        };

        /// <summary>原 DLL 里存的就是上面那串的反序（见 0x1003C00D，长度字节 0x13 在 0x1003C00C）。</summary>
        public static readonly byte[] ReversedSha256DigestInfoPrefix = Reverse(Sha256DigestInfoPrefix);

        /// <summary>NTE_BAD_LEN，原函数在长度不够时返回它。</summary>
        public const int NteBadLen = unchecked((int)0x80090004);

        /// <summary>
        /// 按原函数语义生成 **小端** 块。和 DLL 逐字节一致，可直接喂给
        /// VbnRsaVault_ModExpPriv_clear。
        /// </summary>
        /// <param name="cbBuf">块长度 = 模数字节数（RSA-2048 时是 256）。</param>
        /// <param name="hash">消息摘要，长度必须是 32（SHA-256）。</param>
        /// <returns>写入好的小端块；长度不足时抛 <see cref="ArgumentException"/>。</returns>
        public static byte[] EncodeLittleEndian(int cbBuf, byte[] hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (cbBuf < hash.Length + Sha256DigestInfoPrefix.Length + 3)
                throw new ArgumentException("NTE_BAD_LEN", nameof(cbBuf));

            byte[] block = new byte[cbBuf];
            for (int i = 0; i < cbBuf - 2; i++) block[i] = 0xFF;   // memset(pBuf, 0xFF, cbBuf-2)
            block[cbBuf - 2] = 0x01;                                // 块类型
            block[cbBuf - 1] = 0x00;                                // 原调用方先清零

            // hash 反序拷到最前面
            for (int i = 0; i < hash.Length; i++) block[i] = hash[hash.Length - 1 - i];

            int p = hash.Length;
            foreach (byte b in ReversedSha256DigestInfoPrefix) block[p++] = b;
            block[p] = 0x00;
            return block;
        }

        /// <summary>
        /// 生成标准（大端）EMSA-PKCS1-v1_5 编码块，等价于
        /// <c>0x00 || 0x01 || 0xFF...FF || 0x00 || DigestInfo || hash</c>。
        /// 这正是 vMemrev 之后、也是 .NET <c>RSA.SignHash(..., Pkcs1)</c> 内部使用的形式。
        /// </summary>
        public static byte[] EncodeBigEndian(int cbBuf, byte[] hash)
            => Reverse(EncodeLittleEndian(cbBuf, hash));

        /// <summary>vMemrev 的托管等价物（原地字节反转）。</summary>
        public static void Memrev(byte[] buffer) => Array.Reverse(buffer);

        /// <summary>返回反序副本（不改动入参）。</summary>
        public static byte[] Reverse(byte[] src)
        {
            byte[] dst = (byte[])src.Clone();
            Array.Reverse(dst);
            return dst;
        }
    }
}
