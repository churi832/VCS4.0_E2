/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 24.08.20 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MxpPlcCornerControl
    {
        //5400
        public byte Control;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MxpPlcCornerState
    {
        //5401~5403
        public byte byState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] ErrorID;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MxpPlcCornerFunction
    {
        // 5404 ~
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CornerType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] ReadyPosBCRDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CornerStartPos1BCRDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CornerEndPos1BCRDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CornerStartPos2BCRDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CornerEndPos2BCRDirection;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy8;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy9;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy10;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy11;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy12;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy13;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy14;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy15;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy16;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy17;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy18;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy19;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy20;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy21;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy22;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy23;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy24;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] MasterAxisNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy26;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] LeftBCRSlaveNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy28;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] RightBCRSlaveNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy30;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy31;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy32;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy33;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy34;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy35;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy36;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dummy37;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ReadyPosBCRValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerStartPos1BCRValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerEndPos1BCRValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerStartPos2BCRValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerEndPos2BCRValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy43;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy44;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy45;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] InCurveVelocityRatio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] InCurveVelocityTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] OHT_EMG_Pause_Time;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ReadyPosMaxVel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] StartPosMinVel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Ready_OVR_Time;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ReadyToStartDistance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerDistance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] BetweenCornerDistance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] UserOVR;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] UserOVT;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CornerDistance_Margin;
    }
    [Flags]
    public enum enMxpCornerPlcControl : byte
    {
        Activation = 0X01 << 0,
        Abort = 0X01 << 1,
        Pause = 0X01 << 2,
    }
    [Flags]
    public enum enMxpCornerPlcState : byte
    {
        Complete = 0X01 << 0,
        InCorner = 0X01 << 1,
        Error = 0X01 << 2,
    }

    public enum enDriveParameter : int
    {
        //Axis BCR Info
        MST_AXIS_NO = 5450,
        SLV_AXIS_NO = 5474,
        MST_SLAVE_NO = 5482,
        SLV_SLAVE_NO = 5490,
        BCR_LEFT_SLAVE_NO = 5458,
        BCR_RIGHT_SLAVE_NO = 5466,

        BCR_READ_DIRECTION = 5406,
        //Command
        ACTIVATION = 5400,
    }
    public enum enActivationMode : int
    {
        Activation = 0,
        Abort = 1,
        Pause = 2,
    }
    public enum enActivationOnOff : int
    {
        OFF = 0,
        ON = 1,
    }
    public enum enActivationStatus : int
    {
        NONE = -2,
        EXECUTING = -1,
        COMPLETE = 0,
        CANCEL = 1,
        ABORT = 2,
        STOP = 3,
    }
    public enum enExactPosition : int
    {
        //정위치
        EXACT_POSITION_STATE = 5710,
        EXACT_POSITION_TARGETBCR = 5712,
        EXACT_POSITION_VELOCITY = 5716,
        EXACT_POSITION_ACC = 5720,
        EXACT_POSITION_DEC = 5724,
        EXACT_POSITION_JERK = 5728,
        EXACT_CYCLE_COUNT = 5732,
        COMPLETE_POSITION_RATIO = 5736,
        COMPLETE_VELOCITY_RATIO = 5740,
        SECOND_VELOCITY = 5744,
        SECOND_ACC = 5748,
        SECOND_JERK = 5758,
    }
    public enum enExactPositionStatus : int
    {
        Start = 0,
        End = 1,
    }
}
