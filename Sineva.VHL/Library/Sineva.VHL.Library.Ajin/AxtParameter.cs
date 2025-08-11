using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Ajin
{
    public enum AxtParamItem
    {
        AXIS_NO = 0,
        PULSE_OUT_METHOD = 1,
        ENC_INPUT_METHOD = 2,
        INPOSITION = 3,    // Signal (Use / NoUse) : InPosition
        ALARM = 4,    // Signal (Use / NoUse) : Alarm
        NEG_END_LIMIT = 5,    // Signal (Use / NoUse) : - Limit
        POS_END_LIMIT = 6,    // Signal (Use / NoUse) : + Limit
        MIN_VELOCITY = 7,    // Motion : Max Velocity
        MAX_VELOCITY = 8,    // Motion : Min Velocity
        HOME_SIGNAL = 9,    // 
        HOME_LEVEL = 10,
        HOME_DIR = 11,
        ZPHASE_LEVEL = 12,
        ZPHASE_USE = 13,
        STOP_SIGNAL_MODE = 14,
        STOP_SIGNAL_LEVEL = 15,
        HOME_FIRST_VELOCITY = 16,
        HOME_SECOND_VELOCITY = 17,
        HOME_THIRD_VELOCITY = 18,
        HOME_LAST_VELOCITY = 19,
        HOME_FIRST_ACCEL = 20,
        HOME_SECOND_ACCEL = 21,
        HOME_END_CLEAR_TIME = 22,
        HOME_END_OFFSET = 23,
        NEG_SOFT_LIMIT = 24,
        POS_SOFT_LIMIT = 25,
        MOVE_PULSE = 26,
        MOVE_UNIT = 27,
        INIT_POSITION = 28,
        INIT_VELOCITY = 29,
        INIT_ACCEL = 30,
        INIT_DECEL = 31,
        INIT_ABSRELMODE = 32,
        INIT_PROFILEMODE = 33,
        SVON_LEVEL = 34,
        ALARM_RESET_LEVEL = 35,
        ENCODER_TYPE = 36,
        SOFT_LIMIT_SEL = 37,
        SOFT_LIMIT_STOP_MODE = 38,
        SOFT_LIMIT_ENABLE = 39,
        MOVE_ACC_UNIT = 40,
    }


    [Flags]
    public enum AxtLibraryErrorFlag : uint
    {
        None = 0x0,
        StatusReadMechanical,           // 지정 축의 Mechanical Signal Data(현재 기계적인 신호상태)를 확인합니다.
        StatusReadHomeEnd,              // 원점검색 결과를 반환한다.
        SignalReadInput,                // 범용 입력값을 Hex값으로 반환한다.
        SignalReadOutput,               // 범용 출력값을 반환한다.
        StatusReadActVel,               // 지정한 축의 실제 구동 속도를 읽어온다.
        StatusGetActPos,                // 지정 축의 Actual 위치를 반환한다.
        StatusReadServoLoadRatio,       // 지정 축의 부하율을 반환한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
        StatusReadInMotion,             // (구동상태)모션 구동 중인가를 확인
        SignalWriteOutputBit1,          // 범용 출력값을 비트별로 설정한다. (Servo On)
        SignalWriteOutputBit2,          // 범용 출력값을 비트별로 설정한다. (Alarm Clear)
        AbsEncoderInit,                 // 서보 타입 슬레이브 기기의 센서전원 ON를 요구한다.
        MotionMoveJog,                  // 설정한 속도로 구동한다.
        MotionMoveAbs,                  // 설정한 거리만큼 또는 위치까지 이동한다.
        MotionMoveTorque,               // 설정한 토크 및 속도 값으로 모터를 구동한다.(PCIR-1604-MLII/SIIIH, PCIe-Rxx04-SIIIH  전용 함수)
        MotionMoveSearchSignal,         // // 특정 Input 신호의 Edge를 검출하여 즉정지 또는 감속정지하는 함수.
        MotionStop,                     // 지정 축을 감속 정지한다.
        MotionEStop,                    // 지정 축을 급 정지 한다.
        MotionStopTorque,               // 지정 축의 토크 구동을 정지 한다.
    }

    public class AxtParameter
    {
    }
}