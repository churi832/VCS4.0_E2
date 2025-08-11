using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Sineva.VHL.Library.Common
{
    public class CpuUsage
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetSystemTimes(
                    out ComTypes.FILETIME lpIdleTime,
                    out ComTypes.FILETIME lpKernelTime,
                    out ComTypes.FILETIME lpUserTime
                    );

        #region Fields
        ComTypes.FILETIME _PrevSysKernel;
        ComTypes.FILETIME _PrevSysUser;

        TimeSpan _PreviousProcessTotal;

        float _CpuUsage;
        DateTime _LastRunTime;
        long _RunCount;
        #endregion

        public CpuUsage()
        {
            _CpuUsage = -1;
            _LastRunTime = DateTime.MinValue;
            _PrevSysUser.dwHighDateTime = _PrevSysUser.dwLowDateTime = 0;
            _PrevSysKernel.dwHighDateTime = _PrevSysKernel.dwLowDateTime = 0;
            _PreviousProcessTotal = TimeSpan.MinValue;
            _RunCount = 0;
        }

        public float GetUsage()
        {
            float cpuCopy = _CpuUsage;
            if (Interlocked.Increment(ref _RunCount) == 1)
            {
                if (!EnoughTimePassed)
                {
                    Interlocked.Decrement(ref _RunCount);
                    return cpuCopy;
                }

                ComTypes.FILETIME sysIdle, sysKernel, sysUser;
                TimeSpan procTime;

                Process process = Process.GetCurrentProcess();
                procTime = process.TotalProcessorTime;

                if (!GetSystemTimes(out sysIdle, out sysKernel, out sysUser))
                {
                    Interlocked.Decrement(ref _RunCount);
                    return cpuCopy;
                }

                if (!IsFirstRun)
                {
                    UInt64 sysKernelDiff = SubtractTimes(sysKernel, _PrevSysKernel);
                    UInt64 sysUserDiff = SubtractTimes(sysUser, _PrevSysUser);
                    UInt64 sysTotal = sysKernelDiff + sysUserDiff;
                    Int64 procTotal = procTime.Ticks - _PreviousProcessTotal.Ticks;

                    if (sysTotal > 0)
                    {
                        _CpuUsage = (float)((100.0 * (double)procTotal) / (double)sysTotal);
                    }
                }

                _PreviousProcessTotal = procTime;
                _PrevSysKernel = sysKernel;
                _PrevSysUser = sysUser;

                _LastRunTime = DateTime.Now;

                cpuCopy = _CpuUsage;
            }
            Interlocked.Decrement(ref _RunCount);

            return cpuCopy;
        }

        private UInt64 SubtractTimes(ComTypes.FILETIME a, ComTypes.FILETIME b)
        {
            UInt64 aInt = ((UInt64)(a.dwHighDateTime << 32)) | (UInt64)a.dwLowDateTime;
            UInt64 bInt = ((UInt64)(b.dwHighDateTime << 32)) | (UInt64)b.dwLowDateTime;

            return aInt - bInt;
        }

        private bool EnoughTimePassed
        {
            get
            {
                const int minimumElapsedMS = 250;
                TimeSpan tsSinceLast = DateTime.Now - _LastRunTime;
                return tsSinceLast.TotalMilliseconds > minimumElapsedMS;
            }
        }

        private bool IsFirstRun
        {
            get
            {
                return (_LastRunTime == DateTime.MinValue);
            }
        }
    }
}
