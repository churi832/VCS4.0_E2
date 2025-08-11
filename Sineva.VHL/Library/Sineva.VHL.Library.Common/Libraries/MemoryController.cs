using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Sineva.VHL.Library.Common
{
    public static class MemoryController
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        private static bool _IsRunning = false;
        public static void FlushMemory(object obj)
        {
            try
            {
                if (_IsRunning == true) return;
                _IsRunning = true;

                System.Threading.Thread.Sleep(300);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
            catch
            {
            }
            finally
            {
                _IsRunning = false;
            }
        }

        public struct PROCESS_MEMORY_COUNTERS
        {
            public UInt32 cb;
            public UInt32 PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
        }
        public struct PROCESS_MEMORY_COUNTERS_EX
        {
            public UInt32 cb;
            public UInt32 PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
            public UIntPtr PrivateUsage;
        }
        public struct COMPATIBLE_PROCESS_MEMORY_COUNTERS
        {
            public COMPATIBLE_PROCESS_MEMORY_COUNTERS(PROCESS_MEMORY_COUNTERS pmc)
            {
                cb = pmc.cb;
                PageFaultCount = pmc.PageFaultCount;
                PeakWorkingSetSize = pmc.PeakWorkingSetSize;
                WorkingSetSize = pmc.WorkingSetSize;
                QuotaPeakPagedPoolUsage = pmc.QuotaPeakPagedPoolUsage;
                QuotaPagedPoolUsage = pmc.QuotaPagedPoolUsage;
                QuotaPeakNonPagedPoolUsage = pmc.QuotaPeakNonPagedPoolUsage;
                QuotaNonPagedPoolUsage = pmc.QuotaNonPagedPoolUsage;
                PagefileUsage = pmc.PagefileUsage;
                PeakPagefileUsage = pmc.PeakPagefileUsage;
                PrivateUsage = UIntPtr.Zero;
            }
            public COMPATIBLE_PROCESS_MEMORY_COUNTERS(PROCESS_MEMORY_COUNTERS_EX pmcex)
            {
                cb = pmcex.cb;
                PageFaultCount = pmcex.PageFaultCount;
                PeakWorkingSetSize = pmcex.PeakWorkingSetSize;
                WorkingSetSize = pmcex.WorkingSetSize;
                QuotaPeakPagedPoolUsage = pmcex.QuotaPeakPagedPoolUsage;
                QuotaPagedPoolUsage = pmcex.QuotaPagedPoolUsage;
                QuotaPeakNonPagedPoolUsage = pmcex.QuotaPeakNonPagedPoolUsage;
                QuotaNonPagedPoolUsage = pmcex.QuotaNonPagedPoolUsage;
                PagefileUsage = pmcex.PagefileUsage;
                PeakPagefileUsage = pmcex.PeakPagefileUsage;
                PrivateUsage = pmcex.PrivateUsage;
            }
            public UInt32 cb;
            public UInt32 PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
            public UIntPtr PrivateUsage;
        }
        [DllImport("psapi")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetProcessMemoryInfo(IntPtr hProcess, ref COMPATIBLE_PROCESS_MEMORY_COUNTERS cpmc, Int32 cb);
        public static bool GetProcessMemory(ref UInt32 size)
        {
            COMPATIBLE_PROCESS_MEMORY_COUNTERS cpmc = default(COMPATIBLE_PROCESS_MEMORY_COUNTERS);
            Process p = Process.GetCurrentProcess();
            Int32 type = 0; //default:0, 확장정보:1
            if (type == 0) cpmc.cb = (UInt32)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
            else cpmc.cb = (UInt32)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX));

            bool rv = GetProcessMemoryInfo(p.Handle, ref cpmc, (Int32)cpmc.cb);
            if (rv)
            {
                size = cpmc.WorkingSetSize.ToUInt32() / 1048576; //byte => Mbyte
                UInt32 fault_size = cpmc.PageFaultCount;
            }
            return rv;
        }

    }
}
