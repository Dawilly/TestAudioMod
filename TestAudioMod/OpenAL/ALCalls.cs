using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.OpenAL {
    internal class ALCalls {
        /**************
        ***Alc Calls***
        **************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void native_alcOpenDevice(string device);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void native_alcCloseDevice(IntPtr device);

        private native_alcOpenDevice alcOpenDevice;
        private native_alcCloseDevice alcCloseDevice;

        /*****************
        ***Buffer Calls***
        *****************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void native_alGenBuffers(int size, int* buffers);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alBufferData(uint buffer, int format, IntPtr data, int size, int frequency);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void native_alDeleteBuffers(int size, int* buffers);

        internal native_alGenBuffers alGenBuffers;
        internal native_alBufferData alBufferData;
        private native_alDeleteBuffers alDeleteBuffers;

        /*****************
        ***Source Calls***
        *****************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void native_alGenSources(int size, uint* sources);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alSourcef(uint source, int param, float value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alSourcei(uint source, int param, int value);

        internal native_alGenSources alGenSources;
        internal native_alSourcef alSourcef;
        internal native_alSourcei alSourcei;

        /*****************
        ***Filter Calls***
        *****************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void native_alGenFilters(int size, [Out]uint* filters);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void native_alFilterf(uint filter, FilterF param, float value);

        private native_alGenFilters alGenFilters;
        private native_alFilterf alFilterf;

        /****************
        ***Usage Calls***
        ****************/
        /// <summary>Delegate defintion for alSourcePlay - Plays a source.</summary>
        /// <param name="source">Address Pointer to the source.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alSourcePlay(uint source);
        /// <summary>Delegate definition for alSourcePause - Pauses a source.</summary>
        /// <param name="source"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alSourcePause(uint source);
        /// <summary>Delegate definition for alSourceStop - Stops a source.</summary>
        /// <param name="source"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void native_alSourceStop(uint source);

        /// <summary>alSourcePlay Function - Plays a source.</summary>
        internal native_alSourcePlay alSourcePlay;
        /// <summary>alSourcePause Function - Pauses a source.</summary>
        internal native_alSourcePause alSourcePause;
        /// <summary>alSourceStop Function - Stops a source.</summary>
        internal native_alSourceStop alSourceStop;

        public ALCalls(Loader loader) {
            this.alcOpenDevice = loader.LoadFunction<native_alcOpenDevice>(loader.NativeLibrary, "alcOpenDevice");
            this.alcCloseDevice = loader.LoadFunction<native_alcCloseDevice>(loader.NativeLibrary, "alcCloseDevice");

            this.alBufferData = loader.LoadFunction<native_alBufferData>(loader.NativeLibrary, "alBufferData");
            this.alGenBuffers = loader.LoadFunction<native_alGenBuffers>(loader.NativeLibrary, "alGenBuffers");
            this.alDeleteBuffers = loader.LoadFunction<native_alDeleteBuffers>(loader.NativeLibrary, "alDeleteBuffers");

            this.alGenSources = loader.LoadFunction<native_alGenSources>(loader.NativeLibrary, "alGenSources");
            this.alSourcef = loader.LoadFunction<native_alSourcef>(loader.NativeLibrary, "alSourcef");
            this.alSourcei = loader.LoadFunction<native_alSourcei>(loader.NativeLibrary, "alSourcei");

            this.alGenFilters = loader.LoadFunction<native_alGenFilters>(loader.NativeLibrary, "alGenFilters");
            this.alFilterf = loader.LoadFunction<native_alFilterf>(loader.NativeLibrary, "alFilterf");

            this.alSourcePlay = loader.LoadFunction<native_alSourcePlay>(loader.NativeLibrary, "alSourcePlay");
            this.alSourcePause = loader.LoadFunction<native_alSourcePause>(loader.NativeLibrary, "alSourcePause");
            this.alSourceStop = loader.LoadFunction<native_alSourceStop>(loader.NativeLibrary, "alSourceStop");
        }
    }
}
