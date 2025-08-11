/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    [Flags]
    public enum enMotorType : ushort
    {
        Normal = 0,
        Torque = 1,
        Mxp = 2,
        Acs = 3,
    }
    [Flags]
    public enum enAxisInFlag : ushort
    {
        None = 0,
        Alarm = 1 << 0,
        InPos = 1 << 1,
        Busy = 1 << 2,
        HEnd = 1 << 3,
        SvOn = 1 << 4,
        Org = 1 << 5,
        Limit_N = 1 << 6,
        Limit_P = 1 << 7,
        Cmd_Confirm = 1 << 8,
        OverrideAbnormalStop = 1 << 9,
        InRange_Error = 1 << 10,
        InRange_Checking = 1 << 11,
    }
    [Flags]
    public enum enAxisOutFlag : uint
    {
        CommandNone = 0x0,
        HomeStart = 1 << 1,
        AlarmClear = 1 << 2,
        ServoOn = 1 << 3,
        ServoOff = 1 << 4,
        JogPlus = 1 << 5,
        JogMinus = 1 << 6,
        SequenceMotionStart = 1 << 7,
        RelativeMoveStart = 1 << 8,
        MotionStart = 1 << 9,
        MotionStop = 1 << 10,
        MotionCancel = 1 << 11,
        MotionPause = 1 << 12,
        MotionRelease = 1 << 13,
        Reset = 1 << 14,
        ZeroSet = 1 << 15,
    }

    public enum enAxisResult : uint
    {
        None = 0,
        Success = 1,
        Alarm = 2,
        Timeover = 3,
        CmdError = 4,
        NotReadyError = 5,
        VelSettingError = 6,
        PosSettingError = 7,
        IntrError = 8,
        StopTimeout = 9,
        AlarmClearTimeout = 10,
        ServoOnTimeout = 11,
        ServoOffTimeout = 12,
        HomeTimeout = 13,
        MoveTimeout = 14,
        SoftwareLimitPos = 15,
        SoftwareLimitNeg = 16,
        SoftwareLimitSpeed = 17,
        SoftwareLimitAcc = 18,
        SoftwareLimitDec = 19,
        SoftwareLimitJerk = 20,
        ZeroSetTimeout = 21,
        SequenceMoveTimeout = 22,
        OverrideAbnormalStop = 23,
        InrangeError = 24,
    }

    [Serializable()]
    public enum enMp2100CpuNo : ushort
    {
        No1 = 1,
        No2 = 2,
        No3 = 3,
        No4 = 4,
    }
    [Flags]
    public enum enMxpBcrControl : ushort
    {
        MXP = 0,
        PLC = 1,
    }
    public static class EnumUtil<T>
    {
        public static T Parse(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }
    }
}
