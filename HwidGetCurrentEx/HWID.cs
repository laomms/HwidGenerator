using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// The 14 hardware classes, in the exact order the DLL walks its table at 0x1800B19C0.
    /// The numeric values are the HWIDCLASS ids, not array positions.
    /// </summary>
    public enum HwidClass
    {
        Cdrom           = 0,
        MobileBroadband = 14,
        Hdc             = 1,
        Hdd             = 2,
        Display         = 3,
        ScsiAdapter     = 4,
        Bluetooth       = 15,
        Pcmcia          = 5,
        Audio           = 6,
        Dock            = 7,
        Network         = 8,
        Cpu             = 9,
        Memory          = 10,
        Bios            = 12,
    }

    /// <summary>
    /// Builds the HWID block exactly the way LicensingWinRT!HwidGetCurrentEx does.
    ///
    /// Layout of the produced block (a WORD[] once the counts start):
    ///   [0]        total size in bytes            (2 * valueCount + 28)
    ///   [2..]      unused
    ///   [22]       dock/PCMCIA flag byte
    ///   [24]       memory hash   (one WORD, filled only when that collector yields exactly 1 instance)
    ///   [26]       BIOS hash     (same rule)
    ///   [28..]     the packed instance hashes, in class order
    ///
    /// The instance counters live at even byte offsets 4,6,8,10,12,14,18,20 - note that
    /// offset 16 is deliberately skipped, and that CDROM/MobileBroadband share offset 4
    /// while ScsiAdapter/Bluetooth share offset 12.
    /// </summary>
    public static class HWID
    {
        private const int BufferSize      = 0x118;   // LocalAlloc(0, 0x118) in the DLL
        private const int ValueAreaOffset = 28;      // 0x1C
        private const int MaxPerSlot      = 14;      // "14 - already" clamp
        private const int FlagOffset      = 22;      // hMem byte 22
        private const int MemoryHashOffset = 24;     // hMem[12]
        private const int BiosHashOffset   = 26;     // hMem[13]

        /// <summary>How one class contributes to the block.</summary>
        private sealed class ClassSpec
        {
            public readonly HwidClass Class;
            public readonly int CountOffset;      // byte offset of the instance counter, -1 if none
            public readonly int FixedHashOffset;  // memory / BIOS: one WORD stored outside the values

            private ClassSpec(HwidClass cls, int countOffset, int fixedHashOffset)
            {
                Class = cls;
                CountOffset = countOffset;
                FixedHashOffset = fixedHashOffset;
            }

            /// <summary>Counter plus a run of hashes in the value area.</summary>
            public static ClassSpec Packed(HwidClass cls, int countOffset)
                => new ClassSpec(cls, countOffset, -1);

            /// <summary>A lone WORD, never entering the value area and never counted.</summary>
            public static ClassSpec Fixed(HwidClass cls, int hashOffset)
                => new ClassSpec(cls, -1, hashOffset);

            /// <summary>Contributes nothing but the shared flag byte.</summary>
            public static ClassSpec Flagged(HwidClass cls)
                => new ClassSpec(cls, -1, -1);
        }

        /// <summary>
        /// The class table, in the exact order the DLL walks 0x1800B19C0.
        ///
        /// Note the deliberate gaps and sharings in the counter offsets: byte 16 is never used,
        /// CDROM and MobileBroadband share byte 4, and ScsiAdapter and Bluetooth share byte 12 -
        /// which is why both pairs are clamped against a single running total.
        /// </summary>
        private static readonly ClassSpec[] Classes =
        {
            ClassSpec.Packed (HwidClass.Cdrom,           4),
            ClassSpec.Packed (HwidClass.MobileBroadband, 4),
            ClassSpec.Packed (HwidClass.Hdc,             6),
            ClassSpec.Packed (HwidClass.Hdd,             8),
            ClassSpec.Packed (HwidClass.Display,        10),
            ClassSpec.Packed (HwidClass.ScsiAdapter,    12),
            ClassSpec.Packed (HwidClass.Bluetooth,      12),
            ClassSpec.Flagged(HwidClass.Pcmcia),
            ClassSpec.Packed (HwidClass.Audio,          14),
            ClassSpec.Flagged(HwidClass.Dock),
            ClassSpec.Packed (HwidClass.Network,        18),
            ClassSpec.Packed (HwidClass.Cpu,            20),
            ClassSpec.Fixed  (HwidClass.Memory,         MemoryHashOffset),
            ClassSpec.Fixed  (HwidClass.Bios,           BiosHashOffset),
        };

        private static ushort ReadWord(byte[] b, int offset) => (ushort)(b[offset] | (b[offset + 1] << 8));

        private static void WriteWord(byte[] b, int offset, int value)
        {
            b[offset] = (byte)(value & 0xFF);
            b[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        /// <summary>
        /// Access to the per-class diagnostics that used to be produced with BinaryFormatter.
        /// Purely informational - it is not part of the HWID and never feeds the hash.
        /// </summary>
        public static readonly Dictionary<HwidClass, List<ushort>> LastCollected =
            new Dictionary<HwidClass, List<ushort>>();

        /// <summary>
        /// Produces the raw HWID block. <paramref name="data"/> receives a readable dump of the
        /// per-class instance hashes (kept for callers that want to log what was seen).
        /// </summary>
        public static byte[] HwidGetCurrentEx(ref byte[] data)
        {
            var hwid = new byte[BufferSize];
            var report = new StringBuilder();
            LastCollected.Clear();

            int valueCount = 0;

            foreach (ClassSpec spec in Classes)
            {
                List<ushort> hashes = HwidCollectors.Collect(spec.Class);

                LastCollected[spec.Class] = hashes;
                report.Append(spec.Class).Append(": ")
                      .Append(string.Join(",", hashes.Select(h => h.ToString("X4"))))
                      .AppendLine();

                if (spec.FixedHashOffset >= 0)
                {
                    // Memory and BIOS live outside the value area and the DLL only fills their
                    // word when the collector produced exactly one instance.
                    if (hashes.Count == 1)
                        WriteWord(hwid, spec.FixedHashOffset, hashes[0]);
                }
                else if (spec.CountOffset >= 0)
                {
                    // The budget is shared per counter slot, and only the clamped number of
                    // hashes is written - the rest are dropped, not overflowed.
                    int already = ReadWord(hwid, spec.CountOffset);
                    int take = Math.Max(0, Math.Min(hashes.Count, MaxPerSlot - already));

                    WriteWord(hwid, spec.CountOffset, already + take);
                    for (int i = 0; i < take; i++)
                        WriteWord(hwid, ValueAreaOffset + 2 * valueCount++, hashes[i]);
                }
                else if (hashes.Count != 0)
                {
                    hwid[FlagOffset] = 1;
                }
            }

            int size = 2 * valueCount + ValueAreaOffset;
            if (size >= BufferSize)
                throw new InvalidOperationException(
                    "HWID block overflowed its 0x118 byte buffer (" + size + " bytes).");

            WriteWord(hwid, 0, size);

            data = Encoding.UTF8.GetBytes(report.ToString());

            var result = new byte[size];
            Buffer.BlockCopy(hwid, 0, result, 0, size);
            return result;
        }

        private static readonly byte[] BlockPrefix = new byte[]
        {
            0x0, 0x2, 0x0, 0x1, 0x1, 0x0, 0x2, 0x5, 0x0, 0x3, 0x1, 0x0,
            0x4, 0x2, 0x0, 0x6, 0x1, 0x0, 0x8, 0x7, 0x0, 0x9, 0x3, 0x0,
            0xA, 0x1, 0x0, 0xC, 0x7, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
        };

        public static string HwidCreateBlock(byte[] arrayHWID, int cbsize)
        {
            int bufferSize = cbsize + 6 + 0x25;
            var buffer = new byte[bufferSize];

            buffer[0] = (byte)bufferSize;
            buffer[4] = 0x13;
            Buffer.BlockCopy(arrayHWID, 0, buffer, 6, cbsize);
            buffer[cbsize + 6] = 0xC;
            Buffer.BlockCopy(BlockPrefix, 0, buffer, cbsize + 7, BlockPrefix.Length);

            return Convert.ToBase64String(buffer);
        }

        #region Short HWID

        /// <summary>
        /// Port of _HWID::ConvertToShort (0x18002e4c8). Folds the packed instance hashes into a
        /// single 64 bit id.
        ///
        /// The block holds nine count words (at word indices 2..10, i.e. byte offsets 4..20) each
        /// followed by that many 16 bit instance hashes, packed back to back starting at byte 28.
        /// Word 8 is never produced by a collector and acts purely as a gap that has to be
        /// stepped over. Words 12 and 13 are not counts: they seed the accumulator.
        ///
        /// Reads past the end of the block yield 0, which is what the DLL sees when it reads
        /// beyond the produced data inside its zeroed 0x118 byte buffer.
        /// </summary>
        public static long ConvertToShort(byte[] hwid)
        {
            if (hwid == null)
                throw new ArgumentNullException(nameof(hwid));

            uint lo = 0;
            uint hi = 0;
            ushort flag = 0;

            // Header seeds. Word 13 feeds the low half, word 12 the high half; a zero (or a
            // zero payload) falls back to the all-ones value.
            ushort word13 = Word(hwid, 13);
            if (word13 != 0)
            {
                flag = (ushort)(word13 & 1);
                ushort seed = (ushort)((word13 >> 1) & 0x3F);
                lo = seed != 0 ? seed : (uint)63;
            }

            ushort word12 = Word(hwid, 12);
            if (word12 != 0)
            {
                flag = (ushort)(word12 & 1);
                ushort seed = (ushort)((word12 >> 1) & 0x7);
                hi = (uint)(seed != 0 ? seed : 7) << 29;
            }

            int position = ValueAreaOffset;

            Pack(hwid, ref position, Word(hwid, 2),  7, 21, 0xFE00000u,  false, 6,  ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 3),  4, 28, 0xF0000000u, false, -1, ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 4),  7, 9,  0xFE00u,     true,  7,  ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 5),  5, 21, 0x3E00000u,  true,  -1, ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 6),  5, 16, 0x1F0000u,   true,  8,  ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 7),  6, 3,  0x1F8u,      true,  9,  ref lo, ref hi, ref flag);

            // Word 8 has no collector behind it: step over whatever it declares.
            position += 2 * Word(hwid, 8);

            Pack(hwid, ref position, Word(hwid, 9),  10, 11, 0x1FF800u,   false, 10, ref lo, ref hi, ref flag);
            Pack(hwid, ref position, Word(hwid, 10), 3,  26, 0x1C000000u, true,  -1, ref lo, ref hi, ref flag);

            return ((long)hi << 32) | lo;
        }

        /// <summary>
        /// Convenience wrapper: produce the block and immediately fold it.
        /// </summary>
        public static long HwidGetCurrentExShort(out byte[] hwidBlock)
        {
            byte[] diagnostic = null;
            hwidBlock = HwidGetCurrentEx(ref diagnostic);
            return ConvertToShort(hwidBlock);
        }

        /// <summary>Reads word <paramref name="index"/> (little endian); out of range reads 0.</summary>
        private static ushort Word(byte[] hwid, int index)
        {
            int offset = index * 2;
            if (offset < 0 || offset + 1 >= hwid.Length)
                return 0;
            return (ushort)(hwid[offset] | (hwid[offset + 1] << 8));
        }

        /// <summary>
        /// Port of FindHashAndTruncate (0x18002e458): pick the first of the next
        /// <paramref name="count"/> instance hashes whose low bit is clear, then take its top
        /// <paramref name="bits"/> bits - falling back to the all-ones value when they are all
        /// zero. The low bit itself is handed back as the flag the callers fold in.
        /// </summary>
        private static ushort FindHashAndTruncate(byte[] hwid, int offset, ushort count, int bits, out ushort flag)
        {
            ushort value = Word(hwid, offset / 2);   // a1[0], kept when no candidate is found

            for (int i = 0; i < count; i++)
            {
                ushort candidate = Word(hwid, (offset + 2 * i) / 2);
                if ((candidate & 1) == 0)
                {
                    value = candidate;
                    break;
                }
            }

            flag = (ushort)(value & 1);

            ushort mask = (ushort)((1 << bits) - 1);
            ushort truncated = (ushort)(mask & (value >> 1));

            return truncated != 0 ? truncated : mask;
        }

        /// <summary>
        /// One instance-hash slot: XOR the truncated hash into the accumulator at
        /// <paramref name="shift"/>, then - when <paramref name="flagShift"/> is present - splice
        /// the running flag into the low 0x7C0 bits.
        /// </summary>
        private static void Pack(byte[] hwid, ref int position, ushort count, int bits, int shift,
                                 uint mask, bool intoHigh, int flagShift,
                                 ref uint lo, ref uint hi, ref ushort flag)
        {
            if (count == 0)
                return;

            ushort hash = FindHashAndTruncate(hwid, position, count, bits, out flag);

            uint acc = intoHigh ? hi : lo;
            acc ^= (acc ^ ((uint)hash << shift)) & mask;

            if (intoHigh)
                hi = acc;
            else
                lo = acc;

            // The flag splice always lands in the LOW half, even for slots whose hash went
            // into the high half - that asymmetry is in the original code.
            if (flagShift >= 0)
            {
                uint injected = (uint)flag << flagShift;
                lo ^= (lo ^ (lo | injected)) & 0x7C0u;
            }

            position += 2 * count;
        }

        #endregion

        /// <summary>
        /// SHA-256 of buffer[offset .. offset+count), folded to the 16 bit instance hash:
        ///   value = (digest[1] &lt;&lt; 8 | digest[0]) &amp; 0xFFFE
        ///   value |= 1 when the caller says the instance is not readable
        /// A null buffer is legal - the DLL then hashes nothing and the digest stays all zero,
        /// which yields 1 for a non-readable instance and 0 for a readable one.
        /// </summary>
        internal static ushort AddInstanceHash(byte[] buffer, int offset, int count, bool readable)
        {
            byte[] digest = new byte[32];

            if (buffer != null)
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                    digest = sha.ComputeHash(buffer, offset, count);
            }

            ushort value = (ushort)(((digest[1] << 8) | digest[0]) & 0xFFFE);
            if (!readable)
                value |= 1;

            return value;
        }
    }
}
