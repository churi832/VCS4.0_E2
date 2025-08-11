using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.GL310
{
    class GL5LibraryCORE
    {
        //const string DllName = "../VHL/Sineva.VHL/Sineva.VHL.GL310/lib/x64/libsoslab_core-x64-Release.dll";
        const string DllName = "libsoslab_core-x86-Debug.dll";
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GL3_CORE_createInstance();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GL3_CORE_releaseInstance(IntPtr core);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool GL3_CORE_connectUDP(IntPtr core, string udp_sensor_ip, int udp_sensor_port, string udp_pc_ip, int udp_pc_port);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool GL3_CORE_connectSerial(IntPtr core, string port_name, int baudrate);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GL3_CORE_disconnect(IntPtr core);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GL3_CORE_isConnected(IntPtr core);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct framedata_t
    {
        public ulong data_size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public double[] distance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public double[] pulse_width;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public double[] angle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public double[] x;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public double[] y;
        public byte input_area;
        public byte output_level;
        public byte error_bit;
        public ushort dist_offset;
        public ushort backreflector_pulse_width;
        public ushort pd_hv;
        public ushort ld_hv;
        public ushort pd_temp;
        public ushort ld_temp;
    }

    class GL5LibraryUSER
    {
        //const string DllName = "../VHL/Sineva.VHL/Sineva.VHL.GL310/lib/x64/libsoslab_user-x64-Release.dll";
        const string DllName = "libsoslab_user-x86-Debug.dll";
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GL3_USER_createInstance();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GL3_USER_releaseInstance(IntPtr user);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GL3_USER_getSerialNum(IntPtr core, IntPtr user, IntPtr serialnum, ref UIntPtr serialnum_size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GL3_USER_setStreamEnable(IntPtr core, IntPtr user, bool enable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GL3_USER_getLidarData(IntPtr core, IntPtr user, ref UIntPtr data_size, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2048)] double[] distance, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2048)] double[] pulsewidth, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2048)] double[] angle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2048)] double[] x, [MarshalAs(UnmanagedType.LPArray, SizeConst = 2048)] double[] y, ref UIntPtr input_area, ref UIntPtr output_level, ref UIntPtr error_bit, ref UIntPtr dist_offset, ref UIntPtr backreflector_pulse_width, ref UIntPtr pd_hv, ref UIntPtr ld_hv, ref UIntPtr pd_temp, ref UIntPtr ld_temp, bool filter_on);
    }

}
