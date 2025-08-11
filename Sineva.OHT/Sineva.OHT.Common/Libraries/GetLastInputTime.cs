using System;
using System.Runtime.InteropServices;

namespace Sineva.OHT.Common
{
    public class GetLastInputTime
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.I4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.I4)]
            public int dwTime;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        public static int GetIdleSeconds()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            if (!GetLastInputInfo(ref lastInputInfo))
            {
                return 0;
            }
            else
            {
                int count = (Environment.TickCount & Int32.MaxValue) - (lastInputInfo.dwTime & Int32.MaxValue);
                if (count < 0) count += Int32.MaxValue;
                int seconds = count / 1000;
                return seconds;
            }
        }
        public static int GetIdleMinutes()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            if (!GetLastInputInfo(ref lastInputInfo))
            {
                return 0;
            }
            else
            {
                int count = (Environment.TickCount & Int32.MaxValue) - (lastInputInfo.dwTime & Int32.MaxValue);
                if (count < 0) count += Int32.MaxValue;
                int minutes = count / 60000;
                return minutes;
            }
        }
    }
}
