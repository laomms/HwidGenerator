using System;
using System.Runtime.InteropServices;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// Executes the CPUID instruction and returns EAX, EBX, ECX, EDX as four little endian DWORDs.
    /// The stub is compiled once and kept for the process lifetime - the previous version
    /// VirtualAlloc'd, copied and freed a fresh page on every single call.
    /// </summary>
    internal static class CPUID
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CpuIdDelegate(int level, byte[] buffer);

        private static readonly object Sync = new object();
        private static CpuIdDelegate _invoke;

        public static byte[] Invoke(int level)
        {
            var buffer = new byte[16];
            GetDelegate()(level, buffer);
            return buffer;
        }

        private static CpuIdDelegate GetDelegate()
        {
            if (_invoke != null)
                return _invoke;

            lock (Sync)
            {
                if (_invoke != null)
                    return _invoke;

                byte[] code = IntPtr.Size == 4 ? X86CodeBytes : X64CodeBytes;

                IntPtr codePointer = VirtualAlloc(
                    IntPtr.Zero,
                    new UIntPtr((uint)code.Length),
                    AllocationType.COMMIT | AllocationType.RESERVE,
                    MemoryProtection.EXECUTE_READWRITE);

                if (codePointer == IntPtr.Zero)
                    throw new InvalidOperationException("VirtualAlloc for the CPUID stub failed.");

                Marshal.Copy(code, 0, codePointer, code.Length);
                _invoke = (CpuIdDelegate)Marshal.GetDelegateForFunctionPointer(codePointer, typeof(CpuIdDelegate));
                return _invoke;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize,
                                                  AllocationType flAllocationType, MemoryProtection flProtect);

        [Flags]
        private enum AllocationType : uint
        {
            COMMIT = 0x1000,
            RESERVE = 0x2000,
        }

        [Flags]
        private enum MemoryProtection : uint
        {
            EXECUTE_READWRITE = 0x40,
        }

        // void cpuid_stub(int level, byte* buffer) { eax = level; cpuid;
        //     buffer[0]=eax; buffer[4]=ebx; buffer[8]=ecx; buffer[12]=edx; }

        private static readonly byte[] X86CodeBytes =
        {
            0x55,                   // push ebp
            0x8B, 0xEC,             // mov  ebp, esp
            0x53,                   // push ebx
            0x57,                   // push edi
            0x8B, 0x45, 0x08,       // mov  eax, [ebp+8]
            0x0F, 0xA2,             // cpuid
            0x8B, 0x7D, 0x0C,       // mov  edi, [ebp+12]
            0x89, 0x07,             // mov  [edi+0],  eax
            0x89, 0x5F, 0x04,       // mov  [edi+4],  ebx
            0x89, 0x4F, 0x08,       // mov  [edi+8],  ecx
            0x89, 0x57, 0x0C,       // mov  [edi+12], edx
            0x5F,                   // pop  edi
            0x5B,                   // pop  ebx
            0x8B, 0xE5,             // mov  esp, ebp
            0x5D,                   // pop  ebp
            0xC3                    // ret
        };

        private static readonly byte[] X64CodeBytes =
        {
            0x53,                       // push rbx            (cpuid clobbers it)
            0x49, 0x89, 0xD0,           // mov  r8, rdx        (keep the buffer; cpuid clobbers rdx)
            0x89, 0xC8,                 // mov  eax, ecx       (level)
            0x0F, 0xA2,                 // cpuid
            0x41, 0x89, 0x40, 0x00,     // mov  [r8+0],  eax
            0x41, 0x89, 0x58, 0x04,     // mov  [r8+4],  ebx
            0x41, 0x89, 0x48, 0x08,     // mov  [r8+8],  ecx
            0x41, 0x89, 0x50, 0x0C,     // mov  [r8+12], edx
            0x5B,                       // pop  rbx
            0xC3                        // ret
        };
    }
}
