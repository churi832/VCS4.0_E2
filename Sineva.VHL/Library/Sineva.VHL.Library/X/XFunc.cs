/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team HJYOU
 * Issue Date	: 23.01.17
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Management;

namespace Sineva.VHL.Library
{
    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

    public enum Compatibility
    {
        Match,
        Compatible
    }

    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMillisecond;

        public static DateTime Now = DateTime.Now;

        public override string ToString()
        {
            //다음형식으로 -> 2009-12-04 14:39:48:406
            string now = string.Format("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", wYear, wMonth, wDay, wHour, wMinute, wSecond);
            return now;
        }

        public static bool CheckValidation(SystemTime time)
        {
            DateTime dateTime;
            bool valid = DateTime.TryParse(time.ToString(), out dateTime);
            return valid;
        }
    };

    public class XFunc
    {
        private static XEncryption m_Encryption = new XEncryption();
        #region Import user32.dll
        public const int SC_CLOSE = 0xF060;
        public const int MF_ENABLED = 0x0;
        public const int MF_GRAYED = 0x1;
        public const int MF_DISABLED = 0x2;
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);
        #endregion

        #region Import kernel32.dll
        [DllImport("kernel32.dll")]
        public static extern UInt32 GetTickCount();

        [DllImport("kernel32.dll", EntryPoint = "GetSystemTime", SetLastError = true)]
        public extern static void Win32GetSystemTime(ref SystemTime sysTime);

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        public extern static bool Win32SetSystemTime(ref SystemTime sysTime);

        [DllImport("kernel32.dll", EntryPoint = "GetLocalTime", SetLastError = true)]
        public extern static void Win32GetLocalTime(ref SystemTime sysTime);

        [DllImport("kernel32.dll", EntryPoint = "SetLocalTime", SetLastError = true)]
        public extern static bool Win32SetLocalTime(ref SystemTime sysTime);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, int min, int max);
        #endregion

        #region Import psapi.dll
        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

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

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            return -1;
        }

        public static Int64 GetTotalMemoryInMiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            return -1;
        }

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
        #endregion

        public static void CurrentProcessGCMemoryClear()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Application.DoEvents(); //이게 동시에 실행되어야 한다고 함.

            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
        public static string ConvertToString(short word, ByteOrder byteorder)
        {
            try
            {
                char ch1, ch2;
                string sTemp = "";

                ch1 = Convert.ToChar((word & 0x00FF));
                if (ch1 == 0) ch1 = ' ';
                ch2 = Convert.ToChar(((word >> 8) & 0x00FF));
                if (ch2 == 0) ch2 = ' ';

                if (byteorder == ByteOrder.BigEndian)
                {
                    sTemp += ch1.ToString();
                    sTemp += ch2.ToString();
                }
                else
                {
                    sTemp += ch2.ToString();
                    sTemp += ch1.ToString();
                }
                return sTemp;
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToString(byte word, ByteOrder byteorder)
        {
            try
            {
                char ch1, ch2;
                string sTemp = "";

                ch1 = Convert.ToChar((word & 0x00FF));
                if (ch1 == 0) ch1 = ' ';
                ch2 = Convert.ToChar(((word >> 8) & 0x00FF));
                if (ch2 == 0) ch2 = ' ';

                if (byteorder == ByteOrder.BigEndian)
                {
                    sTemp += ch1.ToString();
                    sTemp += ch2.ToString();
                }
                else
                {
                    sTemp += ch2.ToString();
                    sTemp += ch1.ToString();
                }
                return sTemp;
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToString(int[] word, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    short nData = (short)word[startIndex + i];
                    stemp += ConvertToString(nData, byteorder);
                }

                return stemp;
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToString(short[] word, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    short nData = word[startIndex + i];
                    stemp += ConvertToString(nData, byteorder);
                }

                return stemp;
            }
            catch
            {
                return "";
            }
        }

        public static string ConvertToString(byte[] word, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    byte nData = word[startIndex + i];
                    stemp += ConvertToString(nData, byteorder);
                }

                return stemp;
            }
            catch
            {
                return "";
            }
        }

        public static short ConvertToWord(string sdata, ByteOrder byteorder)
        {
            try
            {
                char ch1, ch2;

                if (sdata.Length <= 0) ch1 = (char)0x20;
                else ch1 = Convert.ToChar(sdata.Substring(0, 1));

                if (sdata.Length <= 1) ch2 = (char)0x20;
                else ch2 = Convert.ToChar(sdata.Substring(1, 1));

                if (ch1 == ' ') ch1 = (char)0x20;
                if (ch2 == ' ') ch2 = (char)0x20;
                if (ch1 == '\r') ch1 = (char)0x00;
                if (ch2 == '\r') ch2 = (char)0x00;


                ushort ntemp;
                if (byteorder == ByteOrder.BigEndian)
                {
                    ntemp = (ushort)((ch2 << 8) | ch1);
                }
                else
                {
                    ntemp = (ushort)((ch1 << 8) | ch2);
                }

                return (short)ntemp;
            }
            catch
            {
                return 0;
            }
        }

        public static void ConvertToWord(string sdata, ref ushort[] stream, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    if (sdata == null)
                    {
                        stemp = "";
                    }
                    else if (sdata.Length >= 2)
                    {
                        stemp = sdata.Substring(0, 2);
                        sdata = sdata.Remove(0, 2);
                    }
                    else if (sdata.Length >= 1)
                    {
                        stemp = sdata.Substring(0, 1);
                        sdata = sdata.Remove(0, 1);
                    }
                    else
                    {
                        stemp = "";
                    }

                    stream[startIndex + i] = (ushort)ConvertToWord(stemp, byteorder);
                }
            }
            catch
            {

            }
        }

        public static void ConvertToWord(string sdata, ref short[] stream, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    if (sdata == null)
                    {
                        stemp = "";
                    }
                    else if (sdata.Length >= 2)
                    {
                        stemp = sdata.Substring(0, 2);
                        sdata = sdata.Remove(0, 2);
                    }
                    else if (sdata.Length >= 1)
                    {
                        stemp = sdata.Substring(0, 1);
                        sdata = sdata.Remove(0, 1);
                    }
                    else
                    {
                        stemp = "";
                    }

                    stream[startIndex + i] = ConvertToWord(stemp, byteorder);
                }
            }
            catch
            {

            }
        }

        public static void ConvertToWord(string sdata, ref byte[] stream, int startIndex, int count, ByteOrder byteorder)
        {
            try
            {
                string stemp = "";
                for (int i = 0; i < count; i++)
                {
                    if (sdata == null)
                    {
                        stemp = "";
                    }
                    else if (sdata.Length >= 2)
                    {
                        stemp = sdata.Substring(0, 2);
                        sdata = sdata.Remove(0, 2);
                    }
                    else if (sdata.Length >= 1)
                    {
                        stemp = sdata.Substring(0, 1);
                        sdata = sdata.Remove(0, 1);
                    }
                    else
                    {
                        stemp = "";
                    }

                    stream[startIndex + i] = (byte)ConvertToWord(stemp, byteorder);
                }
            }
            catch
            {

            }
        }

        public static int ConvertBcdToInt(short value)
        {
            //string sBin = ConvertToBin(value);
            //string temp = "";
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < 4; i++)
            //{
            //    temp = sBin.Substring(i * 4, 4);

            //    sb.Append(Convert.ToInt32(temp, 2));
            //}

            //return sb.ToString();

            int nReturnVal = 0;

            nReturnVal = value & 0X0F;
            nReturnVal += (value >> 4 & 0X0F) * 10;
            nReturnVal += (value >> 8 & 0X0F) * 100;
            nReturnVal += (value >> 12 & 0X0F) * 1000;

            return nReturnVal;
        }

        public static short ConvertToAscii(string value)
        {
            int nReturnVal = 0;

            char[] temp;

            temp = value.ToCharArray();

            if (value.Length > 1)
            {
                nReturnVal = temp[1] << 8 | temp[0];
            }
            else nReturnVal = temp[0];

            return (short)nReturnVal;
        }

        public static short ConvertBcdToShort(string value)
        {
            string temp = "";
            string sBin = "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                temp = value.Substring(i * 1, 1);

                sBin = Convert.ToString(Convert.ToInt16(temp), 2).PadLeft(4, '0');

                sb.Append(sBin);
            }

            return Convert.ToInt16(sb.ToString(), 2);
        }

        public static short ConvertIntToBcd(int value)
        {
            int nReturnVal = 0;

            int[] nTmp = new int[4];
            nTmp[0] = value / 1000;
            nTmp[1] = (value - nTmp[0] * 1000) / 100;
            nTmp[2] = (value - nTmp[0] * 1000 - nTmp[1] * 100) / 10;
            nTmp[3] = (value - nTmp[0] * 1000 - nTmp[1] * 100 - nTmp[2] * 10);

            nReturnVal = nTmp[0] << 12 | nTmp[1] << 8 | nTmp[2] << 4 | nTmp[3];

            return (short)nReturnVal;
        }

        public static string ConvertShortToBcd(short value)
        {
            string sBin = ConvertToBin(value);
            string temp = "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                temp = sBin.Substring(i * 4, 4);

                sb.Append(Convert.ToInt32(temp, 2));
            }

            return sb.ToString();
        }

        public static int ConvertBinToDec(string sValue)
        {
            try
            {
                int nDec = 0;
                nDec = Convert.ToInt32(sValue, 2);

                return nDec;
            }
            catch
            {
                return -1;
            }
        }

        public static string ConvertToBin(short value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(16, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        public static string ConvertToBin8(short value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(8, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        public static string ConvertToBin16(UInt16 value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(16, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        public static string ConvertToBin32(UInt32 value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(32, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        public static string ConvertToBin64(long value)
        {
            string sBin = "";

            try
            {
                sBin = Convert.ToString(value, 2).PadLeft(64, '0');
            }
            catch (Exception err)    //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                sBin = err.Message.ToString();
            }

            return sBin.ToUpper();
        }

        /// <summary>
        /// double word로 읽어들인 PLC Single precision 값을 float으로 변환하여 줌
        /// </summary>
        /// <param name="dword">binary formatted single</param>
        /// <returns>float value</returns>
        public static Single ConvertBinedSingleToSingle(UInt32 dword)
        {
            var binString = XFunc.ConvertToBin32(dword);
            UInt32 v = 0;
            for (int i = binString.Length - 1; i >= 0; i--)
            {
                v = v + ((binString[i] == '0' ? (UInt32)0 : (UInt32)1) << (binString.Length - 1 - i));
            }

            var f = BitConverter.ToSingle(BitConverter.GetBytes(v), 0);
            return f;
        }

        public static double ConvertBinedDoubleToDouble(long val)
        {
            var binString = XFunc.ConvertToBin64(val);
            long v = 0;
            for (int i = binString.Length - 1; i >= 0; i--)
            {
                v = v + ((binString[i] == '0' ? (long)0 : (long)1) << (binString.Length - 1 - i));
            }

            var f = BitConverter.ToDouble(BitConverter.GetBytes(v), 0);
            return f;
        }



        public static int ConvertHexaStringToInteger(string hex)
        {
            int rv = 0, v;
            int jari = hex.Length - 1;
            int count = hex.Length;

            for (int i = 0; i < count; i++)
            {
                v = hex[i];

                if (v >= 'A' && v <= 'F')
                {
                    rv += (int)Math.Pow(0x10, jari) * (v - 'A' + 0xA);
                }
                else if (v >= 'a' && v <= 'f')
                {
                    rv += (int)Math.Pow(0x10, jari) * (v - 'a' + 0xA);
                }
                else if (v >= '0' && v <= '9')
                {
                    rv += (int)((Math.Pow(0x10, jari)) * (v - '0'));
                }
                jari--;
            }
            return rv;
        }

        /// <summary>
        /// 두개의 16bit word를 32bit double word로
        /// </summary>
        /// <param name="small">작은놈</param>
        /// <param name="big">큰놈</param>
        /// <returns>결과값</returns>
        public static UInt32 ConvertToDoubleWord(short small, short big)
        {
            UInt32 dval = ((UInt32)big) & 0xFFFF;
            dval = dval << 16;
            //			dval = dval | (Convert.ToUInt32(small) & 0xFFFF);
            dval = dval | ((UInt32)small & 0xFFFF);
            return dval;
        }

        // Class Type의 호환성 검사
        //
        public static bool CheckTypeCompatibility(Type type, Type target, Compatibility compatibility)
        {
            if (target == null)
            {
                return false;
            }
            else if (type == target)
            {
                return true;
            }
            else if (type.IsInterface)
            {
                Type[] types = target.GetInterfaces();
                foreach (Type typeofInterface in types)
                {
                    if (type == typeofInterface)
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (compatibility == Compatibility.Match)
            {
                return false;
            }
            else
            {
                return CheckTypeCompatibility(type, target.BaseType, compatibility);
            }
        }

        // Class의 모든 Property정보를 반환
        //
        public static PropertyInfo[] GetProperties(object obj)
        {
            if (obj == null) return null;

            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
            return propertyInfos;
        }

        // Class의 Property정보중에서 Type호환성이 있는 정보만 반환
        //
        public static PropertyInfo[] GetProperties(object obj, Type type, Compatibility compatibility)
        {
            if (obj == null) return null;

            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            PropertyInfo[] propertyInfosAll = GetProperties(obj);
            // 해당 object의 모든 property에서 type이 호환되는것만 가져온다.
            foreach (PropertyInfo info in propertyInfosAll)
            {
                if (true == CheckTypeCompatibility(type, info.PropertyType, compatibility))
                {
                    propertyInfos.Add(info);
                }
            }

            return propertyInfos.ToArray();
        }
        public static CustomProperty[] GetCustomProperties(object instance)
        {
            List<CustomProperty> infos = new List<CustomProperty>();
            PropertyInfo[] propertyInfos = instance.GetType().GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                try
                {
                    object[] attributes = info.GetCustomAttributes(typeof(CustomAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        CustomProperty Info = new CustomProperty();
                        Info.Name = (attributes[0] as CustomAttribute).ItemName;
                        Info.ValueType = (attributes[0] as CustomAttribute).ItemType;
                        string valueText = string.Empty;
                        var value = info.GetValue(instance, null);
                        if (value != null)
                        {
                            Info.Value = value.ToString();
                        }
                        infos.Add(Info);
                    }
                }
                catch
                {
                }
            }

            return infos.ToArray();
        }

        // device name에 공백, 특수문자 등을 '_'로 치환
        public static string FilterigName(string name)
        {
            char[] chars = name.ToCharArray();
            int count = chars.Length;
            for (int i = 0; i < count; i++)
            {
                if (!char.IsLetterOrDigit(chars[i]))
                {
                    chars[i] = '_';
                }
            }

            string result = new string(chars);
            return result;
        }

        //        public static List<Exception> ExceptionHandler = new List<Exception>();
        //public static XExceptionHandler ExceptionHandler = XExceptionHandler.Instance;

        // 구조체를 byte 배열로 변환해주는 함수
        public static byte[] StructureToByte(object obj)
        {
            byte[] data = null;

            try
            {
                int datasize = Marshal.SizeOf(obj);         // 구조체에 할당된 메모리의 크기를 구한다.
                data = new byte[datasize];                  // 구조체가 복사될 배열
                IntPtr buff = Marshal.AllocHGlobal(datasize);// 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
                Marshal.StructureToPtr(obj, buff, false);   // 할당된 구조체 객체의 주소를 구한다.
                Marshal.Copy(buff, data, 0, datasize);      // 구조체 객체를 배열에 복사
                Marshal.FreeHGlobal(buff);                  // 비관리 메모리 영역에 할당했던 메모리를 해제함
                                                            //return data; // 배열을 리턴
            }
            catch (Exception err)
            {
                err.Message.ToString();
                data = null;
            }

            return data;
        }

        //byte 배열을 구조체로 변환해주는 함수
        public static object ByteToStructure(byte[] data, Type type)
        {
            object obj = null;

            try
            {
                IntPtr buff = Marshal.AllocHGlobal(data.Length);// 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
                Marshal.Copy(data, 0, buff, data.Length);       // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
                obj = Marshal.PtrToStructure(buff, type);       // 복사된 데이터를 구조체 객체로 변환한다.
                Marshal.FreeHGlobal(buff);                      // 비관리 메모리 영역에 할당했던 메모리를 해제함
                if (Marshal.SizeOf(obj) != data.Length)         // 구조체와 원래의 데이터의 크기 비교
                    obj = null;                                 // 크기가 다르면 null 리턴
                                                                //return obj; // 구조체 리턴
            }
            catch (Exception err)
            {
                err.Message.ToString();
                obj = null;
            }

            return obj;
        }

        public static object ByteToStructure(byte[] data, Type type, int size)
        {
            object obj = null;

            try
            {
                IntPtr buff = Marshal.AllocHGlobal(data.Length);// 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
                Marshal.Copy(data, 0, buff, data.Length);       // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
                obj = Marshal.PtrToStructure(buff, type);       // 복사된 데이터를 구조체 객체로 변환한다.
                Marshal.FreeHGlobal(buff);                      // 비관리 메모리 영역에 할당했던 메모리를 해제함
                                                                //if (size != data.Length)         // 구조체와 원래의 데이터의 크기 비교
                                                                //    obj = null;                                 // 크기가 다르면 null 리턴
                                                                //return obj; // 구조체 리턴
            }
            catch (Exception err)
            {
                err.Message.ToString();
                obj = null;
            }

            return obj;
        }

        public static byte[] ShortToByteArray(ushort data)
        {
            byte[] rv = new byte[2];
            rv[0] = (byte)(data >> 8 & 0xFF);
            rv[1] = (byte)(data & 0xFF);
            return rv;
        }

        public static ushort ToShort(byte[] data)
        {
            int MS = (data[0] << 8);
            int LS = data[1];
            ushort rv = (ushort)(MS + LS);
            return rv;

        }

        public static ushort ToShort(byte hi, byte lo)
        {
            int MS = (hi << 8);
            int LS = lo;
            ushort rv = (ushort)(MS + LS);
            return rv;
        }

        public static void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        public static byte[] GetCRC_A(params byte[] message)
        {
            byte[] crc = new byte[2];

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            crc[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            crc[0] = CRCLow = (byte)(CRCFull & 0xFF);
            return crc;
        }

        public static byte[] GetCRC_B(params byte[] message)
        {
            byte[] crc = new byte[2];

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length - 2); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            crc[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            crc[0] = CRCLow = (byte)(CRCFull & 0xFF);
            return crc;
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //Procedures and sample program for increasing the minimum working set area of the PC
        //The following provides measures for increasing the minimum working set area of the PC when an
        //error of error code 77 occurs due to MD function execution, and its sample program.
        //The PC board driver runs using the minimum working set area in the memory area reserved in the
        //application program. Some application program may use a large area of the minimum working set
        //area. In such a case, when the minimum working set area for the PC board driver cannot be
        //reserved, an error code 77 is returned.
        //If this situation occurs, increase the minimum working set area in the application program before
        //executing the MD function. (See the following sample program.)
        //The minimum working set area of 200KB is reserved at startup of the personal computer.
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetProcessWorkingSet(int minMB, int maxMB)
        {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr iptrMinWorkingSet = minMB > 0 ? new IntPtr(minMB * 1024 * 1024) : new IntPtr(ProcessWorkingSetDefaultMin);
            IntPtr iptrMaxWorkingSet = new IntPtr(maxMB * 1024 * 1024);
            currentProcess.MinWorkingSet = iptrMinWorkingSet;
            currentProcess.MaxWorkingSet = iptrMaxWorkingSet;
        }

        private static uint ProcessWorkingSetDefaultMin = 204800; //byte
        private static uint ProcessWorkingSetDefaultMax = 1413120; //byte
        public static void SetProcessWorkingSetDefault()
        {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr iptrMinWorkingSet = new IntPtr(ProcessWorkingSetDefaultMin); //default 
            IntPtr iptrMaxWorkingSet = new IntPtr(ProcessWorkingSetDefaultMax); //default
            currentProcess.MinWorkingSet = iptrMinWorkingSet;
            currentProcess.MaxWorkingSet = iptrMaxWorkingSet;
        }

        //문자열이 Digit인지 검사
        public static bool IsDigit(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;

            char[] chars = data.ToCharArray();
            if (chars == null)
            {
                return false;
            }
            else
            {
                bool isDigit = true;
                int count = chars.Length;
                for (int i = 0; i < count; i++)
                {
                    if (!char.IsDigit(chars[i]))
                    {
                        isDigit = false;
                        break;
                    }
                }

                return isDigit;
            }
        }

        public static bool IsFloat(string data)
        {
            if (string.IsNullOrEmpty(data)) return false;

            char[] chars = data.ToCharArray();
            if (chars == null)
            {
                return false;
            }
            else
            {
                bool isDigit = true;
                int count = chars.Length;
                for (int i = 0; i < count; i++)
                {
                    if (!char.IsDigit(chars[i]))
                    {
                        if (chars[i] != '.')
                        {
                            isDigit = false;
                            break;
                        }
                    }
                }

                return isDigit;
            }
        }

        public static long GetDiskFreeSpace(string driveName)
        {
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                    return drive.TotalFreeSpace;
            }
            return -1;
        }

        public static long GetDiskTotalSpace(string driveName)
        {
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                    return drive.TotalSize;
            }
            return -1;
        }

        public static int GetDiskFreeRatio(string driveName)
        {
            int ratio = 0;
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    ratio = (int)(((double)drive.TotalFreeSpace / (double)drive.TotalSize) * 100);
                    return ratio;
                }
            }
            ratio = -1;
            return ratio;
        }

        public static decimal GetCpuTemperature()
        {
            System.Management.ManagementClass mos = new System.Management.ManagementClass(@"root\WMI:MSAcpi_ThermalZoneTemperature");
            foreach (System.Management.ManagementObject mo in mos.GetInstances())
            {
                return CovertToCelsius(mo.GetPropertyValue("CurrentTemperature").ToString());
            }
            return 0;
        }

        static decimal CovertToCelsius(string reading)
        {
            return (decimal.Parse(reading) / 10 - 273.15m);
        }

        public static bool IsDesignTime()
        {
            return (LicenseManager.UsageMode == LicenseUsageMode.Designtime ? true : false);
        }
        public static bool IsRunTime()
        {
            return (LicenseManager.UsageMode == LicenseUsageMode.Runtime ? true : false);
        }
        public static string PasswordEncrypt(string userId, string password)
        {
            string encPwd = m_Encryption.GetEncryptedPassword(userId, password);
            string md5Hash = m_Encryption.GetMd5Hash(Encoding.UTF8.GetBytes(encPwd));
            return md5Hash;
        }
        public static bool FindProcess(string name)
        {
            bool rv = false;
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                Process[] curProcessList = Process.GetProcesses();
                foreach (Process p in curProcessList)
                {
                    if (p.ProcessName == name)
                    {
                        rv = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.WriteLog(method, String.Format(e.ToString()));
            }
            return rv;
        }

        public static bool FindProcessFileName(string name)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            bool rv = false;
            try
            {
                Process[] curProcessList = Process.GetProcesses();
                foreach (Process p in curProcessList)
                {
                    if (p.MainModule == null) continue;
                    if (p.MainModule.FileName == name)
                    {
                        rv = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.WriteLog(method, String.Format(e.ToString()));
            }
            return rv;
        }

        public static bool CheckPathAndCreate(string path)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                bool valid = string.IsNullOrEmpty(path) == false;
                char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
                for (int i = 0; i < invalidPathChars.Length; i++)
                {
                    valid &= !path.Contains(invalidPathChars[i]);
                }
                if (valid) valid &= System.IO.Path.IsPathRooted(path);

                if (System.IO.Directory.Exists(path) == true)
                {
                    return true;
                }
                else if (valid)
                {
                    System.IO.DirectoryInfo dirInfo = System.IO.Directory.CreateDirectory(path);
                    return dirInfo.Exists;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ExceptionLog.WriteLog(method, String.Format(e.ToString()));
                return false;
            }
        }

        public static object ParseString(Type type, string value)
        {
            if (!type.IsValueType) return null;

            if (type == typeof(Boolean)) return Convert.ToBoolean(value);
            if (type == typeof(Char)) return Convert.ToChar(value);
            if (type == typeof(Byte)) return Convert.ToByte(value);
            if (type == typeof(Int16)) return Convert.ToInt16(value);
            if (type == typeof(Int32)) return Convert.ToInt32(value);
            if (type == typeof(Int64)) return Convert.ToInt64(value);
            if (type == typeof(UInt16)) return Convert.ToUInt16(value);
            if (type == typeof(UInt32)) return Convert.ToUInt32(value);
            if (type == typeof(UInt64)) return Convert.ToUInt64(value);
            if (type == typeof(Single)) return Convert.ToSingle(value);
            if (type == typeof(Double)) return Convert.ToDouble(value);
            if (type == typeof(Decimal)) return Convert.ToDecimal(value);
            if (type == typeof(DateTime)) return Convert.ToDateTime(value);

            return null;
        }
    }
}
