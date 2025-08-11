using System.Runtime.InteropServices;

namespace Sineva.VHL.Library.Common
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public static class SystemTimeControl
    {
        #region Import System DLL Function
        /// <summary>시스템 날짜/시간을 설정한다.</summary>
        /// <param name="dtNew">설정한 Date/Time</param>
        /// <returns>오류가 없는 경우 true를 응답하며,
        /// 그렇지 않은 경우 false를 응답한다.</returns>
        [DllImport("kernel32")]
        public static extern bool SetLocalTime(ref SystemTime time);

        [DllImport("kernel32")]
        public static extern void GetLocalTime(ref SystemTime lpSystemTime);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemTime(out SystemTime lpSystemTime);
        #endregion
    }
}
