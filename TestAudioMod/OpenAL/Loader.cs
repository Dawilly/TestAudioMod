using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.OpenAL {
    internal class Loader {
        /*
         * Private Classes
         */
        private class Windows {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryW(string lpszLib);
        }

        private class Linux {
            [DllImport("libdl.so.2")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl.so.2")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private class OSX {
            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlopen(string path, int flags);

            [DllImport("/usr/lib/libSystem.dylib")]
            public static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        /*
         * Constants
         */

        private const int RTLD_LAZY = 0x0001;

        /*
         * Properties
         */

        public IntPtr NativeLibrary => this.GetNativeLibrary();

        /*
         * Constructor
         */
        public Loader() { }

        /*
         * Public Functions
         */
        public IntPtr LoadLibrary(string libname) {
            if (CurrentPlatform.OS == OS.Windows)
                return Windows.LoadLibraryW(libname);

            if (CurrentPlatform.OS == OS.MacOSX)
                return OSX.dlopen(libname, RTLD_LAZY);

            return Linux.dlopen(libname, RTLD_LAZY);
        }

        public T LoadFunction<T>(IntPtr libPointer, string function, bool throwIfNotFound = false) {
            IntPtr call = IntPtr.Zero;

            if (CurrentPlatform.OS == OS.Windows) {

            } else if (CurrentPlatform.OS == OS.MacOSX) {

            } else if (CurrentPlatform.OS == OS.Linux) {

            }

            if (call == IntPtr.Zero) {
                if (throwIfNotFound) throw new EntryPointNotFoundException(function);
                return default(T);
            }

            return Marshal.GetDelegateForFunctionPointer<T>(call);
        }

        /*
         * Private Functions
         */
        private IntPtr GetNativeLibrary() {
            IntPtr pointer = IntPtr.Zero;

            string assemblyLocation = Path.GetDirectoryName(typeof(AL).Assembly.Location);
            string libraryName = "";

            if (CurrentPlatform.OS == OS.MacOSX) {
                libraryName = "libopenal.1.dylib";
            } else if (Environment.Is64BitProcess) {
                libraryName = "x64/";
            } else {
                libraryName = "x86/";
            }

            if (CurrentPlatform.OS == OS.Windows) {
                libraryName += "soft_oal.dll";
            } else if (CurrentPlatform.OS == OS.Linux) {
                libraryName += "libopenal.so.1";
            }

            pointer = this.LoadLibrary(Path.Combine(assemblyLocation, libraryName));

            if (pointer == IntPtr.Zero) pointer = this.PointerZeroCase();

            return pointer;
        }

        private IntPtr PointerZeroCase() {
            if (CurrentPlatform.OS == OS.Windows)
                return this.LoadLibrary("soft_oal.dll");
            else if (CurrentPlatform.OS == OS.Linux)
                return this.LoadLibrary("libopenal.so.1");
            else
                return this.LoadLibrary("libopenal.1.dylib");
        }
    }
}
