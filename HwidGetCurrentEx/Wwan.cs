using System;
using System.Runtime.InteropServices;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// The five entry points the DLL pulls out of wwapi.dll. They are resolved at run time
    /// rather than declared with DllImport because the DLL does the same and because a missing
    /// export must degrade to "no WWAN hardware" instead of throwing EntryPointNotFoundException.
    /// </summary>
    internal sealed class WwanApi : IDisposable
    {
        /// <summary>sizeof(WWAN_INTERFACE_INFO): GUID(16) + WCHAR[256](512) + status(8) + 4 ULONGs + 2 GUIDs.</summary>
        public const int InterfaceStride = 0x24C;

        // The fields the collector reads out of a WwanQueryInterface result. These sit outside the
        // hashed window and have no documented name in the SDK header - the DLL reads them as
        // plain DWORDs, so they are named for what they do here rather than guessed at.
        public const int SkipFlagOffset   = 0x388;   // non-zero: skip this interface entirely
        public const int ReadableOffset   = 0x228;   // == 1 means the instance is "readable"
        public const int AddressOffset    = 0x28C;   // start of the hashed window
        public const int AddressWindowSize = 36;

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int OpenHandleFn(int clientVersion, IntPtr reserved, out int negotiatedVersion, out IntPtr clientHandle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int CloseHandleFn(IntPtr clientHandle, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int EnumerateInterfacesFn(IntPtr clientHandle, int reserved, out IntPtr interfaceList);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int QueryInterfaceFn(IntPtr clientHandle, ref Guid interfaceGuid, int opcode,
                                              IntPtr reserved, out int dataSize, out IntPtr data,
                                              out int opcodeValueType);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int FreeMemoryFn(IntPtr memory);

        private IntPtr _module;
        private OpenHandleFn _openHandle;
        private CloseHandleFn _closeHandle;
        private EnumerateInterfacesFn _enumerateInterfaces;
        private QueryInterfaceFn _queryInterface;
        private FreeMemoryFn _freeMemory;

        public int Init()
        {
            _module = LoadLibraryW("wwapi.dll");
            if (_module == IntPtr.Zero)
                return Marshal.GetLastWin32Error();

            _openHandle = Resolve<OpenHandleFn>("WwanOpenHandle");
            _closeHandle = Resolve<CloseHandleFn>("WwanCloseHandle");
            _enumerateInterfaces = Resolve<EnumerateInterfacesFn>("WwanEnumerateInterfaces");
            _queryInterface = Resolve<QueryInterfaceFn>("WwanQueryInterface");
            _freeMemory = Resolve<FreeMemoryFn>("WwanFreeMemory");

            if (_openHandle == null || _closeHandle == null || _enumerateInterfaces == null ||
                _queryInterface == null || _freeMemory == null)
            {
                Dispose();
                return 1;
            }

            return 0;
        }

        private T Resolve<T>(string name) where T : class
        {
            IntPtr address = GetProcAddress(_module, name);
            return address == IntPtr.Zero ? null : (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }

        public int OpenHandle(int clientVersion, IntPtr reserved, out int negotiatedVersion, out IntPtr clientHandle)
        {
            return _openHandle(clientVersion, reserved, out negotiatedVersion, out clientHandle);
        }

        public int CloseHandle(IntPtr clientHandle, IntPtr reserved)
        {
            return _closeHandle(clientHandle, reserved);
        }

        public int EnumerateInterfaces(IntPtr clientHandle, int reserved, out IntPtr interfaceList)
        {
            return _enumerateInterfaces(clientHandle, reserved, out interfaceList);
        }

        public int QueryInterface(IntPtr clientHandle, Guid interfaceGuid, int opcode, IntPtr reserved,
                                  out int dataSize, out IntPtr data, out int opcodeValueType)
        {
            return _queryInterface(clientHandle, ref interfaceGuid, opcode, reserved,
                                   out dataSize, out data, out opcodeValueType);
        }

        public void FreeMemory(IntPtr memory)
        {
            if (memory != IntPtr.Zero)
                _freeMemory(memory);
        }

        public void Dispose()
        {
            if (_module != IntPtr.Zero)
            {
                FreeLibrary(_module);
                _module = IntPtr.Zero;
            }
            _openHandle = null;
            _closeHandle = null;
            _enumerateInterfaces = null;
            _queryInterface = null;
            _freeMemory = null;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr LoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
    }
}
