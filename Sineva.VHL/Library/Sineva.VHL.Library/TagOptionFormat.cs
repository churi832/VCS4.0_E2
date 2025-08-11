/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Drawing;

namespace Sineva.VHL.Library
{
    public enum OptionType
    {
        None,
        Alternative
    }
    // JobMode와 같은 형태 추가시 수정해 주어야 하는 부분
    // 1. enum OptionFormat : item 추가 선언
    // 2. OptionFormatHelper : string description 추가
    // 3. OptionFormatHelper : GetOptionListByOptionFormat(OptionFormat format)
    // 4. OptionFormatHelper : GetStyleByOptionFormat(string format, string value, ref Color backColor, ref Color foreColor)
    // 5. OptionFormatHelper : GetValue<T>(OptionFormat format, string val)
    public enum OptionFormat
    {
        None,
        Boolean,
        Digit,
        Float,
        String,
        Hex,
        Binary,
        IpAddress,
        JobMode,// Process Mode / Pass Mode
        OnOff,
        Use,    // Use / No Use
        Check,  // Check / No Check
        Cw,     // Cw / Ccw
        Fw,     // Fw / Bw
        HsmsConnectionMode, //Active, Passive
        HpmjMode,   // Pressure Mode / Frequency Mode
        FilePath,   //2009.07.01 Youngsik
        MixTankMode,// DI / DET
        MixTankModeHF,// DI / DET//2009.10.12 KIMGUN
        SNTMode,     // Single / Multi
        DetSupplyMode,  //Ccss / Lcss
        Enable, // Enable / Disable
        RobotSpeed,
        ServoAxisType,
    }
    public enum ChemicalType
    {//2009.10.12 kimgun
        DET, HF
    }
    public enum JobMode
    {
        Process, Pass
    }
    public enum OnOff
    {
        On, Off
    }
    public enum HpmjMode
    {
        Pressure, Frequency
    }
    public enum Use
    {
        Use, NoUse
    }
    public enum Check
    {
        Check, NoCheck
    }
    public enum Cw
    {
        Cw, Ccw
    }
    public enum Fw
    {
        Fw, Bw
    }
    public enum HsmsConnectionMode
    {
        Active, Passive
    }
    public enum MixTankMode
    {
        DI, DET
    }
    public enum MixTankModeHF
    {//2009.10.12 kimgun
        DI, HF
    }
    public enum SNTMode
    {
        Single, Multi
    }
    public enum DetSupplyMode
    {
        Ccss, Lcss
    }
    public struct ChemicalInfoMix
    {//2009.10.12 kimguin
        public ChemicalType Type;
    }
    public enum Enable
    {
        Enable, Disable
    }
    public enum RobotSpeed
    {
        RATIO_10, RATIO_25, RATIO_50, RATIO_60,
        RATIO_70, RATIO_80, RATIO_90, RATIO_100,
    }
    public enum ServoControlFamily
    {
        YMp2100,        // Yaskawa MP-2100
        ACS230,         // ACS Motion Controller Ver2.3 C#...Code
        AXT,            // Ajin Controller
        MXP,            // LS Mechapion
    }
    public class OptionFormatHelper
    {
        public static readonly string _Process = JobMode.Process.ToString();
        public static readonly string _On = OnOff.On.ToString();
        public static readonly string _Use = Use.Use.ToString();
        public static readonly string _Check = Check.Check.ToString();
        public static readonly string _Cw = Cw.Cw.ToString();
        public static readonly string _Fw = Fw.Fw.ToString();
        public static readonly string _True = bool.TrueString;
        public static readonly string _False = bool.FalseString;
        public static readonly string _Active = HsmsConnectionMode.Active.ToString();
        public static readonly string _Passive = HsmsConnectionMode.Passive.ToString();
        public static readonly string _Pressure = HpmjMode.Pressure.ToString();
        public static readonly string _Frequency = HpmjMode.Frequency.ToString();
        public static readonly string _DI = MixTankMode.DI.ToString();
        public static readonly string _DET = MixTankMode.DET.ToString();
        public static readonly string _HF = MixTankModeHF.HF.ToString();
        public static readonly string _SINGLE = SNTMode.Single.ToString();
        public static readonly string _Ccss = DetSupplyMode.Ccss.ToString();
        public static readonly string _Lcss = DetSupplyMode.Lcss.ToString();
        public static readonly string _Enable = Enable.Enable.ToString();
        public static readonly string[] _RobotSpeed =
            new string[] { RobotSpeed.RATIO_10.ToString(), RobotSpeed.RATIO_25.ToString(), RobotSpeed.RATIO_50.ToString(), RobotSpeed.RATIO_60.ToString(),
                           RobotSpeed.RATIO_70.ToString(), RobotSpeed.RATIO_80.ToString(), RobotSpeed.RATIO_90.ToString(), RobotSpeed.RATIO_100.ToString()};
        public static readonly string[] _ServoAxisType =
            new string[] { ServoControlFamily.YMp2100.ToString(), ServoControlFamily.ACS230.ToString(), ServoControlFamily.AXT.ToString(), ServoControlFamily.MXP.ToString(), };

        public static string[] GetOptionListByOptionFormat(OptionFormat format)
        {
            string[] values = null;
            switch (format)
            {
                case OptionFormat.JobMode:
                    values = Enum.GetNames(typeof(DBJobMode));
                    break;
                case OptionFormat.OnOff:
                    values = Enum.GetNames(typeof(OnOff));
                    break;
                case OptionFormat.Use:
                    values = Enum.GetNames(typeof(Use));
                    break;
                case OptionFormat.Check:
                    values = Enum.GetNames(typeof(Check));
                    break;
                case OptionFormat.Cw:
                    values = Enum.GetNames(typeof(Cw));
                    break;
                case OptionFormat.Fw:
                    values = Enum.GetNames(typeof(Fw));
                    break;
                case OptionFormat.Boolean:
                    {
                        values = new string[2];
                        values[0] = Boolean.TrueString;
                        values[1] = Boolean.FalseString;
                    }
                    break;
                case OptionFormat.HsmsConnectionMode:
                    values = Enum.GetNames(typeof(HsmsConnectionMode));
                    break;
                case OptionFormat.HpmjMode:
                    values = Enum.GetNames(typeof(HpmjMode));
                    break;
                case OptionFormat.MixTankMode:
                    values = Enum.GetNames(typeof(MixTankMode));
                    break;
                case OptionFormat.MixTankModeHF:
                    values = Enum.GetNames(typeof(MixTankModeHF));
                    break;
                case OptionFormat.SNTMode:
                    values = Enum.GetNames(typeof(SNTMode));
                    break;
                case OptionFormat.DetSupplyMode:
                    values = Enum.GetNames(typeof(DetSupplyMode));
                    break;
                case OptionFormat.Enable:
                    values = Enum.GetNames(typeof(Enable));
                    break;
                case OptionFormat.RobotSpeed:
                    values = Enum.GetNames(typeof(RobotSpeed));
                    break;
                case OptionFormat.ServoAxisType:
                    values = Enum.GetNames(typeof(ServoControlFamily));
                    break;
            }
            return values;
        }


        // jemoon : C#에는 아래처럼 Template를 사용한다. -> Generics
        // 참고 : http://msdn.microsoft.com/ko-kr/library/ms379564(VS.80).aspx
        public static T GetValue<T>(OptionFormat format, string val)
        {
            string str = val;

            switch (format)
            {
                case OptionFormat.Digit:
                case OptionFormat.Float:
                case OptionFormat.Boolean:
                    break;
                case OptionFormat.JobMode:
                    str = (val == OptionFormatHelper._Process ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.OnOff:
                    str = (val == OptionFormatHelper._On ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.Use:
                    str = (val == OptionFormatHelper._Use ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.Check:
                    str = (val == OptionFormatHelper._Check ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.Cw:
                    str = (val == OptionFormatHelper._Cw ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.Fw:
                    str = (val == OptionFormatHelper._Fw ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.HsmsConnectionMode:
                    str = (val == OptionFormatHelper._Active ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.IpAddress:
                    System.Net.IPAddress ip = System.Net.IPAddress.Parse(str);
                    return (T)Convert.ChangeType(ip, typeof(T));
                case OptionFormat.HpmjMode:
                    str = (val == OptionFormatHelper._Pressure ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.MixTankMode:
                    str = (val == OptionFormatHelper._DET ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.MixTankModeHF:
                    str = (val == OptionFormatHelper._HF ? bool.TrueString : bool.FalseString);//2009.10.12 kimgun
                    break;
                case OptionFormat.SNTMode:
                    str = (val == OptionFormatHelper._SINGLE ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.DetSupplyMode:
                    str = (val == OptionFormatHelper._Ccss ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.Enable:
                    str = (val == OptionFormatHelper._Enable ? bool.TrueString : bool.FalseString);
                    break;
                case OptionFormat.RobotSpeed:
                    {
                        for (int i = 0; i < _RobotSpeed.Length; i++)
                        {
                            if (val == OptionFormatHelper._RobotSpeed[i]) str = OptionFormatHelper._RobotSpeed[i].ToString();
                            else break;
                        }
                    }
                    break;
                case OptionFormat.ServoAxisType:
                    {
                        for (int i = 0; i < _ServoAxisType.Length; i++)
                        {
                            if (val == OptionFormatHelper._ServoAxisType[i]) str = OptionFormatHelper._ServoAxisType[i].ToString();
                        }
                    }
                    break;
            }

            return (T)Convert.ChangeType(str, typeof(T));
        }

        static public void GetStyleByOptionFormat(string format, string value, ref Color backColor, ref Color foreColor)
        {
            OptionFormat optionFormat = (OptionFormat)(Enum.Parse(typeof(OptionFormat), format));
            switch (optionFormat)
            {
                case OptionFormat.JobMode:
                case OptionFormat.OnOff:
                case OptionFormat.Use:
                case OptionFormat.Check:
                case OptionFormat.Cw:
                case OptionFormat.Fw:
                case OptionFormat.Boolean:
                case OptionFormat.HsmsConnectionMode:
                case OptionFormat.HpmjMode:
                case OptionFormat.MixTankMode:
                case OptionFormat.MixTankModeHF:
                case OptionFormat.SNTMode:
                case OptionFormat.DetSupplyMode:
                case OptionFormat.Enable:
                    {
                        if (value == OptionFormatHelper._Process ||
                            value == OptionFormatHelper._Use ||
                            value == OptionFormatHelper._Check ||
                            value == OptionFormatHelper._Cw ||
                            value == OptionFormatHelper._Fw ||
                            value == OptionFormatHelper._True ||
                            value == OptionFormatHelper._Active ||
                            value == OptionFormatHelper._On ||
                            value == OptionFormatHelper._Pressure ||
                            value == OptionFormatHelper._DET ||
                            value == OptionFormatHelper._HF ||//2009.10.12 kimgun 
                            value == OptionFormatHelper._SINGLE ||
                            value == OptionFormatHelper._Ccss ||
                            value == OptionFormatHelper._Enable)
                        {
                            backColor = Color.GreenYellow;
                            foreColor = Color.Black;
                        }
                        else
                        {
                            backColor = SystemColors.Info;
                            foreColor = Color.Gray;
                        }
                    }
                    break;
                default:
                    {
                        backColor = Color.White;
                        foreColor = Color.Black;
                    }
                    break;
            }
        }


        static public void GetStyleByUnitType(string type, ref Color backColor, ref Color foreColor)
        {
            UnitType unitType = (UnitType)(Enum.Parse(typeof(UnitType), type));
            switch (unitType)
            {
                case UnitType.None:
                    {
                        backColor = Color.White;
                        foreColor = Color.LightGray;
                    }
                    break;
                default:
                    {
                        backColor = Color.White;
                        foreColor = Color.Black;
                    }
                    break;
            }
        }
    }
}
