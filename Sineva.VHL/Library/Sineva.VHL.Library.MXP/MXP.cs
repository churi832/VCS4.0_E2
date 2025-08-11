using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
// 
namespace Sineva.VHL.Library.MXP
{
    public class MXP
    {
        #region MXP ENUM DEFINE
        public enum MXP_SLAVE_DESYNC_MODE
        {
            MXP_SLAVEPOSITION_UNUSING = 0,
            MXP_SLAVEPOSITION_USING = 1,  // SYNC 해제후 SLAVE 위치 옵셋 적용
            MXP_SLAVEPOSITION_UNUSING_POSITIONSET
        }

        public enum IDENT_IN_GROUP_REF
        {
            MCS_X = 0,
            MCS_Y = 1,
            MCS_Z = 2,
            MCS_U = 3,
            MCS_V = 4,
            MCS_W = 5,
            MCS_A = 6,
            MCS_B = 7,
            MCS_C = 8,
            MCS_INTERPOLATION_PATH = 9
        }

        public enum MXP_CIRCLEMODE_ENUM
        {
            MXP_BORDER = 1,
            MXP_CENTER = 2,
            MXP_RADIUS = 3,
            MXP_ANGLE = 4
        }

        public enum MXP_PATHCHOICE_ENUM
        {
            MXP_CLOCKWISE = 0,
            MXP_COUNTERCLOCKWISE = 1
        }

        public enum MXP_STARTMODE_ENUM
        {
            MXP_ABSOLUTE = 0,
            MXP_RELATIVE = 1,
            MXP_RAMPIN = 2
        }

        public enum MXP_SOURCE_ENUM
        {
            MXP_COMMANDEDVALUE = 1,
            MXP_SETVALUE = 2,
            MXP_ACTUALVALUE = 3
        }

        public enum MXP_BUFFERMODE_ENUM
        {
            MXP_ABORTING = 0,
            MXP_BUFFERED = 1,
            MXP_BLENDING_LOW = 2,
            MXP_BLENDING_PREVIOUS = 3,
            MXP_BLENDING_NEXT = 4,
            MXP_BLENDING_HIGH = 5,
            MXP_SINGLE_BLOCK = 6,
            MXP_ABORT_OVERRIDERESET = 7

        }

        public enum MXP_DIRECTION_ENUM
        {
            MXP_NONE_DIRECTION = 0,
            MXP_POSITIVE_DIRECTION = 1,
            MXP_SHORTEST_WAY = 2,
            MXP_NEGATIVE_DIRECTION = 3,
            MXP_CURRENT_DIRECTION = 4
        }

        public enum EC_NETWORK_CMD
        {
            NET_STATE_INIT_CMD = 0x01,
            NET_STATE_PREOP_CMD = 0x02,
            NET_STATE_BOOT_CMD = 0x03,
            NET_STATE_SAFEOP_CMD = 0x04,
            NET_STATE_OP_CMD = 0x08
        }

        public enum MXP_CAM_MASTER_TYPE
        {
            MXP_CAM_MASTER_TYPE_MOTOR = 0,
            MXP_CAM_MASTER_TYPE_EXTENAL_ENC = 1
        }

        public enum MXP_INTERPOLATION_ENUM
        {
            CAM_PROFILE_MONOTONE_CUBIC = 0,
            CAM_PROFILE_LINEAR,
            CAM_PROFILE_POLYNOM3,
            CAM_PROFILE_POLYNOM5,
            CAM_PROFILE_MODIFIEDSINE,
            CAM_PROFILE_CYCLOID,
            CAM_PROFILE_SINUSLINE,
            CAM_PROFILE_POLYNOM7
        }

        public enum MXP_FUNCTION_STATUS_RESULT
        {
            RT_NO_ERROR = 0,
            RT_ERROR_FUNCTION = -1,
            RT_ERROR_FULL = -2,
            RT_ERROR_WRONG_INDEX = -3,
            RT_ERROR_WRONG_AXISNO = -4, //RT_ERROR_MOTION_INVALID_AXIS_NO
            RT_ERROR_MOTIONBUSY = -5,
            RT_ERROR_WRONG_SLAVENO = -6,
            RT_ERROR_WRONG_CAMTABLENO = -7,
            RT_ERROR_WRONG_ECMASTERNO = -8,
            RT_ERROR_WRONG_ECSLAVENO = -9,
            RT_ERROR_NOT_OPMODE = -10,//RT_NETWORK_ERROR
            RT_ERROR_NOTRUNNING = -11,
            RT_ERROR_WRONG_PARAMETER_NO = -12,
            RT_ERROR_WRONG_MXP_TYPE = -13,
            RT_ERROR_ALREADYOPEN = -14,
            RT_ERROR_NOT_SCANMODE = -15,
            RT_ERROR_WRONG_ONLINESTATE = -16,
            RT_ERROR_NOT_SIMULATIONMODE = -17,
            RT_ERROR_WRONG_REG_RANGE = -18,
            RT_ERROR_NOT_COMM_STATE = -19,
            RT_ERROR_WRONG_GROUPNO = -20,
            RT_ERROR_WRONG_SIZE = -21,
            RT_ERROR_NOT_FOEMODE = -22,
            RT_ERROR_NOT_INVALID_LIC_FEATURE = -23,
            RT_ERROR_INVALID_LASTSTEPVEL = -24,
            RT_ERROR_INVALID_LASTTEPPOS = -25,
            RT_ERROR_INVALID_FIRSTSTEPVEL = -26,
            RT_ERROR_INVALID_IO_SIZE = -27,
            RT_ERROR_FAIL_INDEX_CHECK = -28,
            RT_ERROR_EXCEPTIONERROR = -29,
            RT_ERROR_STATECHECK_FAIL = -30,
            RT_ERROR_INVALID_PLCADDRESS = -31,
            RT_ERROR_SEQUENCEMOVE_READ_FAIL = -32,
            RT_ERROR_SEQUENCEMOVE_INVALID_STATE = -33,
            RT_ERROR_SEQUENCEMOVE_PROCESS_FAIL = -34,
            RT_ERROR_MOVE_ABSOLUTE_VELOCITYOVERRIDE_INVALID_PARAMETER = -35,
            RT_ERROR_INVALID_SPINTABLE = -36,
            RT_ERROR_SPINMOVE_COMMAND_INVALID_STATE = -37,
            RT_ERROR_PLCFB_MOVE_FAIL = -38,
            RT_ERROR_FEEDBACKSENSOR_INVALID_PARAM = -39,
            RT_ERROR_FEEDBACKSENSOR_INVALID_TABLE = -40,
            RT_ERROR_FEEDBACKSENSOR_COMMAND_INVALID_STATE = -41,
            RT_ERROR_FEEDBACKSENSOR_MOVE_FAIL = -42,
            RT_ERROR_FEEDBACKSENSOR_MOVE_LASTSTEP_INVALID_DIST = -43,
            RT_ERROR_INVALID_PARAMETER = -44,
            RT_ERROR_MOTION_SETTING = -45,
            RT_ERROR_MOTION_HOME_SEARCHING = -46,  // Home Searching
            RT_ERROR_INVALID_SWLimit = -47,
            RT_ERROR_CONTI_INVALID_MAP_NUMBER = -48,
            RT_ERROR_AXIS_INVALID_STATE = -49,
            RT_ERROR_GROUP_INVALID_STATE = -50,
            RT_ERROR_PDO_MAPPINGCHECK_FAIL = -51,
            RT_ERROR_NOTCONNECT_FIRSTSLAVE = -52,
            RT_ERROR_INVALID_ADDGROUPAXISNO = -53,  // GROUP 에 축이 할당된 경우 
            RT_ERROR_INVALIDSTATE_EDITGROUPCONFIG = -54,  // GROUP 이 동작중인경우 변경 불가 
            RT_ERROR_RTOS_NETWORKDRIVER_SETTING_INCORRECT = -55,
            RT_ERROR_FUNCTION_PARAMETER_INVAILD = -56,  // 함수 동작시 관련 파라메터 설정이 잘못된경우 -> 이름 변경 -> 44 번으로 변경 
            RT_ERROR_TRIGGERMODE_RUNNING = -57,  // Trigger 모드 동작중 Enalbe 시 
            RT_ERROR_EXAPI_MEMORY_OPENFAIL = -58,
            RT_ERROR_DUPLICATE_SETTING_ERROR = -59,
            RT_ERROR_ECAT_REGREAD_INVALID_STATE = -60,
            RT_ERROR_ECAT_REGREAD_TIMEOUT = -61,
            RT_ERROR_ECOMP_FILE_DOES_NOT_EXIST = -62,
            RT_ERROR_ECOMP_MAP_SIZE_DATA_INVALID = -63,
            RT_ERROR_ECOMP_MAP_CELLPOINT_DATA_INVALID = -64,
            RT_ERROR_ECOMP_MAP_ORIGIN_DATA_INVALID = -65,
            RT_ERROR_ECOMP_ERROR_MAP_DATA_INVALID = -66,
            RT_ERROR_ECOMP_DIFFERENT_MAP_ORIGIN_STARTING_POSITION = -67,
            RT_ERROR_ECOMP_USING_AXIS = -68,
            RT_ERROR_ECOMP_USING_ERRORTABLE = -69,
            RT_ERROR_ECOMP_CANNOT_RELEASE_POSITION = -70,
            RT_ERROR_ECOMP_MODULO_AXIS_SET = -71,
            RT_ERROR_ECOMP_NOTREADY = -72,
            RT_ERROR_GANTRY_PARAMETER_INVALID = -73,
            RT_ERROR_EXTERNALENC_SETTING_INVALID = -74,
            RT_ERROR_MOTION_NO_RESPONSE = -75,
            RT_ERROR_MOVER_INDEX_ALLOCATION_FAIL = -76,
            RT_ERROR_MOVER_INVALID_STATE = -77,
            RT_ERROR_TRACK_INDEX_ALLOCATION_FAIL = -78,
            RT_ERROR_MMT_INVALID_STATE = -79,
            RT_ERROR_MMT_PATH_OCCUPIED = -80,                   //MMT 경로 코일에 mover점유
            RT_ERROR_MMT_NOT_MATCHED_PLANE = -81,                   //MMT 트랙과 무버의 Plane이 다르다.
            RT_ERROR_MMT_MOVER_OCCUPIED_TRACK = -82,                    //MMT 트렉의 Plane을 변경하는 중에 코일 사이에 무버가 있을 시.
            RT_ERROR_MMT_PARAMETER_INVALID = -83,
            RT_ERROR_MMT_FILELOAD_FAIL = -84,
            RT_ERROR_MMT_TRACK_INVALID_STATE = -85,
            RT_ERROR_MMT_STATE_RUNNING = -86,
            RT_ERROR_MMT_INVALID_MOVERLOCATION = -87,
            RT_ERROR_MMT_CONFIG_INVALID = -88,

            RT_ERROR_MMT_MODULE_LENGTH_NOT_SUPPOERT = -89,

            RT_ERROR_BOARD_CHECKFAIL = -90,
            RT_ERROR_BOARDSHARDADDRESS_READFAIL = -91,
            RT_ERROR_SYSTEMMOMORY_READFAIL = -92,
            RT_ERROR_BOARD_INVAILDSTATE = -93,
            RT_ERROR_BOARD_FILEDOWNLOADFAIL = -94,
            RT_ERROR_GROUPENABLE_FAIL = -95,
            RT_ERROR_GROUPDISABLE_FAIL = -96,
            RT_ERROR_BOARD_INVALIDID = -97,
            RT_ERROR_PDOWRITE_INVALID_STATE = -98,
            RT_ERROR_MOVEDISTANCE_INVALID = -99,
            RT_ERROR_FILE_FLASH_BUSY_P = -100,
            RT_ERROR_FILE_READING_PROGRESS_NEGATIVE_W = -101,
            RT_ERROR_FILE_MALLOC_FAIL_P = -102,
            RT_ERROR_FILE_MALLOC_FAILED_W = -103,
            RT_ERROR_FILE_PKERNEL_NULL_W = -104,
            RT_ERROR_FILE_HANDLE_RT_ERROR_FILE_W = -105,
            RT_ERROR_FILE_INIT_FUNCTION_FAIL_W = -106,
            RT_ERROR_FILE_INIT_FUNCTION_FAIL_P = -107,
            RT_ERROR_PROGRESS_IS_NEVER_CHANGED_W = -108,
            RT_ERROR_SECTION_VALUE_INVAID_W = -109,
            RT_ERROR_FILE_RECEIVING_SEND_TIMEOUT_P = -110,
            RT_ERROR_FILE_RECEIVING_SEND_TIMEOUT_W = -111,
            RT_ERROR_FILE_RECEIVING_RECEIVE_TIMEOUT_P = -112,
            RT_ERROR_FILE_RECEIVING_RECEIVE_TIMEOUT_W = -113,
            RT_ERROR_FILE_RECEIVING_ACK_TIMEOUT_W = -114,
            RT_ERROR_FILE_WAITING_PROGRESS_TIMEOUT_W = -115,
            RT_ERROR_FILE_READING_TIMEOUT_W = -116,
            RT_ERROR_FILE_CRC_CHECK_FAIL_P = -117,
            RT_ERROR_FILE_CRC_CHECK_FAIL_W = -118,
            RT_ERROR_FILE_FLASH_ERASE_FAIL_P = -119,
            RT_ERROR_FILE_MEMORY_RANGE_OVER_W = -120,
            RT_ERROR_FILE_MEMORY_RANGE_OVER_P = -121,
            RT_ERROR_FILE_READING_RAM_LENGTH_FAIL_P = -122,
            RT_ERROR_FILE_READING_FLASH_LENGTH_FAIL_P = -123,
            RT_ERROR_FILE_READING_FILE_LENGTH_FAIL_W = -124,
            RT_ERROR_FILE_READING_RAM_DATA_FAIL_P = -125,
            RT_ERROR_FILE_READING_FLASH_DATA_FAIL_P = -126,
            RT_ERROR_FILE_READING_IFILE_FAIL_W = -127,
            RT_ERROR_FILE_READING_OFILE_FAIL_W = -128,
            RT_ERROR_FILE_FLASH_WRITE_FAIL_P = -129,
            RT_ERROR_FILE_MEM_WRITE_FAIL_P = -130,
            RT_ERROR_FILE_WRITING_OFILE_FAIL_W = -131,
            RT_ERROR_FILE_VER_PARSER_WRONG_W = -132,
            RT_ERROR_FILE_API_COMMAND_WRONG_W = -133,
            RT_ERROR_FILE_API_COMMAND_WRONG_P = -134,
            RT_ERROR_FILE_RESERVED_135 = -135,
            RT_ERROR_FILE_RESERVED_136 = -136,
            RT_ERROR_FILE_RESERVED_137 = -137,
            RT_ERROR_FILE_RESERVED_138 = -138,
            RT_ERROR_FILE_RESERVED_139 = -139,
            RT_COMM_ERROR_SECTION = -149,
            RT_COMM_APP_IS_NOT_LOAD = -150,
            RT_COMM_PARA_POINTER_IS_NULL = -151,
            RT_COMM_PARA_IS_INVALID_DATA = -152,
            RT_COMM_APP_STATE_IS_ERR = -153,
            RT_COMM_API_TIMEOUT = -154,
            RT_COMM_UNKNOWN_ERR = -155,
            RT_HW_VER_CIRCUIT_DATE_UPDATE_FAIL = -156,
            RT_HW_TEMP_DATA_INVALID = -157,
            RT_GPIO_OUTRANGE = -158,

            RT_ERROR_AXISMAPPING_INVALID = -170,

            RT_ERROR_MIF_RANGE_OVER = -171,
            RT_ERROR_MIF_DATA_TYPE = -172,
            RT_ERROR_MIF_WRONG_DATA = -173,
            RT_ERROR_MIF_AXIS_STATE = -174,
            RT_ERROR_MIF_UNUSED_GROUP = -175,
            RT_ERROR_MIF_TRJ_SYNCAXIS_COMMAND = -176,
            RT_ERROR_CAM_EMPTYCAMTABLEID = -177,
            RT_ERROR_CAM_SLAVESTATENOTSTANDSTILL = -178,
            RT_ERROR_MODEOFOP_PDOCHECK_FAIL = -179,
            RT_ERROR_TARGETVELOCITY_PDOCHECK_FAIL = -180,
            RT_ERROR_TARGETTORUE_PDOCHECK_FAIL = -181,
            RT_ERROR_MULTICROSSCOUPLESET_FAIL = -182,
            RT_ERROR_MULTICROSSCOUPLEAXISCOUNT = -183,
            RT_ERROR_AXISPARAMETER_INVAILD = -184,
            RT_ERROR_GROUPMOVE_INVALID_STATE = -185,
            RT_ERROR_COMMAND_ARGUMENT_INVALID = -186,
            RT_ERROR_PDOCHECK_FAIL = -187,
            RT_ERROR_HOME_PARAMETER_INVALID = -188,
            RT_ERROR_MASTERAXIS_STATE_INVALID = -189,
            RT_ERROR_CONTROLMODE_MISMATCH = -190,
            RT_ERROR_MIF_TRAJ_ZEROVELOCITY = -191,
            RT_NETWORK_ERROR = -1000,
            RT_ERROR_MOTION_NOT_INITIAL_AXIS_NO = -1001,
            RT_ERROR_MOTION_INVALID_AXIS_NO = -1002,
            RT_ERROR_MOTION_INVALID_METHOD = -1003,
            RT_ERROR_MOTION_INVALID_VELOCITY = -1004,
            RT_ERROR_MOTION_IN_NONMOTION = -1005,
            RT_ERROR_IN_MOTION = -1006,
            RT_ERROR_MOTION_GANTRY_ENABLE = -1007,
            RT_ERROR_MOTION_MASTER_SERVOON = -1008,
            RT_ERROR_MOTION_SLAVE_SERVOON = -1009,
            RT_ERROR_MOTION_INVALID_POSITION = -1010,
            RT_ERROR_CONTI_INVALID_MAP_NO = -1011,
            RT_ERROR_MOTION_HOME_GANTRY = -1012,
            RT_ERROR_INVALID_TABLE_ENABLED = -1013,
            RT_ERROR_INVALID_TABLE_NOT_ENABLED = -1014,
            RT_ERROR_INVALID_GET_TABLE = -1015,
            RT_ERRRT_ERROR_AXIS_INVALID_STATEOR_IN_MOTION = -1016,



        }

        public enum KERNEL_STATUS
        {
            SYSTEM_UNLICENSED = -2,
            SYSTEM_IDLE = 1,
            SYSTEM_KILLING = 2,
            SYSTEM_KILLED = 3,
            SYSTEM_CREATING = 4,
            SYSTEM_CREATED = 5,
            SYSTEM_INITING = 6,
            SYSTEM_INITED = 7,
            SYSTEM_READY = 8,
            SYSTEM_RUN = 9
        }

        public enum MXP_ONLINE_STATE
        {
            ONLINE_STATE_NONE = 0x00,
            ONLINE_STATE_INIT = 0x01,
            ONLINE_STATE_PREOP = 0x02,
            ONLINE_STATE_BOOT = 0x03,
            ONLINE_STATE_SAFEOP = 0x04,
            ONLINE_STATE_OP = 0x08,
            ONLINE_STATE_ERROR_NONE = 0x10,
            ONLINE_STATE_ERROR_INIT = 0x11,
            ONLINE_STATE_ERROR_PREOP = 0x12,
            ONLINE_STATE_ERROR_BOOT = 0x13,
            ONLINE_STATE_ERROR_SAFEOP = 0x14,
            ONLINE_STATE_ERROR_OP = 0x18
        }

        public enum MXP_NODE_TYPE
        {
            TYPE_NUll = 0,
            TYPE_DRIVE,
            TYPE_IO
        }

        public enum MXP_PORT_STATE
        {
            DL_STATUS_LOOP_OPEN_NO_LINK = 0,
            DL_STATUS_LOOP_CLOSE_NO_LINK = 1,
            DL_STATUS_LOOP_OPEN_WITH_LINK = 2,
            DL_STATUS_LOOP_CLOSE_WITH_LINK = 3
        }

        public enum MXP_IO_TYPE
        {
            IO_OUT = 0,
            IO_IN
        }

        public enum MXP_ACTIVATIONMODE
        {
            IMMEDIATELY = 0,
            ACTIVATIONPOSITION,
            NEXTPERIOD
        }

        public enum MXP_TOUCHPROB_NUMBER
        {
            TOUCHPROB1 = 0,
            TOUCHPROB2
        }

        public enum MXP_TRIGGER_MODE
        {
            TRIGGER_MODE_SINGLE = 0,
            TRIGGER_MODE_CONTINUOUS
        }

        public enum MXP_TRIGGER_TYPE
        {
            TRIGGER_TYPE_TOUCHPROBE = 0,
            TRIGGER_TYPE_INDEX
        }

        public enum MXP_TRIGGER_EDGE
        {
            TRIGGER_EDGE_RISING = 0,
            TRIGGER_EDGE_FALLING,
            TRIGGER_EDGE_BOTH
        }

        public enum MXP_PDO_DIRECTION
        {
            SERVO_WRITE = 0,
            MXP_WRITE
        }

        public enum MXP_MOVESTATE
        {
            MOVESTATE_NULL = 0,
            MOVESTATE_MOVING,
            MOVESTATE_COMPLETE,
            MOVESTATE_FAIL,
            MOVESTATE_WAIT_INPOS,
            MOVESTATE_ABORT // 외부 Stop 에 의한 이송 정지 
        }

        public enum MXP_PLANE
        {
            XPLANE = 0,
            YPLANE,
            ZPLANE,
            UPLANE,
            VPLANE,
            WPLANE,
            APLANE,
            BPLANE,
            CPLANE
        }

        public enum MXP_USERLIB
        {
            MXP_R2R = 0,
            MXP_ROBOTICS,
            MXP_EDG
        }

        public enum MXP_CAM_STATUS
        {
            CAM_NO_ERROR = 0,
            CAM_ERROR_TABLE_ID,
            CAM_ERROR_INPUT_PARAM,
            CAM_ERROR_DATASIZE,
            CAM_ERROR_INTERPOLATION,
            CAM_ERROR_MASTERPOS,
            CAM_ERROR_EXECUTIONMODE
        }

        public enum SEQUENCE_CMD_TYPE
        {
            CMD_TYPE_ABSMOVE = 0,
            CMD_TYPE_RELATIVEMOVE,
            CMD_TYPE_DWELL,
            CMD_TYPE_IO,
            CMD_TYPE_CIRCULAR_ABS,
            CMD_TYPE_CIRCULAR_RELATIVE
        }

        public enum SEQUENCE_BUFFER_STATE
        {
            BUFFER_STATE_NULL = 0,
            BUFFER_STATE_RUNNING,
            BUFFER_STATE_COMPLETE,
            BUFFER_STATE_FAIL
        }

        public enum SEQUENCE_MOVE_MODE
        {
            MOVE_MODE_ABSOLUTE = 0,
            MOVE_MODE_RELATIVE
        }

        public enum FEEDBACKSENSOR_ADDRESS
        {
            FEEDBACKSENSOR_FB_START = 5000,
            FEEDBACKSENSOR_FB_AXISNO = 5000,
            FEEDBACKSENSOR_FB_COMMAND,
            FEEDBACKSENSOR_FB_SENSOR_SLAVENO,
            FEEDBACKSENSOR_FB_SENSOR_POS,
            FEEDBACKSENSOR_FB_SENSOR_SIZE,
            FEEDBACKSENSOR_FB_SENSOR_TARGET = 5008,
            FEEDBACKSENSOR_FB_SENSOR_SETRANGE = 5012,
            FEEDBACKSENSOR_FB_SENSOR_UNIT = 5016,
            FEEDBACKSENSOR_FB_SENSOR_OVERRIDE_DIST = 5020,

            FEEDBACKSENSOR_FB_SENSOR_TARGETVEL = 5024,
            FEEDBACKSENSOR_FB_SENSOR_VALIDFLAG = 5028,
            FEEDBACKSENSOR_FB_SENSOR_TARGET_CALC_DIST = 5032,

            FEEDBACKSENSOR_FB_RUN_STATE = 5040,
            FEEDBACKSENSOR_FB_ERROR_CODE = 5042,
            FEEDBACKSENSOR_FB_SENSOR_DIST1 = 5080,
            FEEDBACKSENSOR_FB_SENSOR_DIST2 = 5084,
            FEEDBACKSENSOR_FB_SENSOR_VEL1 = 5088,
            FEEDBACKSENSOR_FB_SENSOR_VEL2 = 5092,

            FEEDBACKSENSOR_FB_END = 5399,
            FEEDBACKSENSOR_FB_MAX_PLCBLOCK = 5500
        }

        public enum FB_RUN_STATE
        {
            FB_STATE_READY = 0,
            FB_STATE_RUN,
            FB_STATE_COMPLETE,
            FB_STATE_FAIL
        }

        public enum FB_RUN_MODE
        {
            FB_MODE_NULL = 0,
            FB_MODE_RUN,
            FB_MODE_STOP,
            FB_MODE_RESET
        }

        public enum SPIN_ADDRESS
        {
            SPIN_ADDRESS_MAX_PLCBLOCK = 5000,
            SPIN_ADDRESS_RECIPE_TABLE = 5008,
            SPIN_ADDRESS_ORIGIN_WAIT_TIME = 5328,
            SPIN_ADDRESS_ORIGIN_VEL = 5332,
            SPIN_ADDRESS_FB_START = 5340,
            SPIN_ADDRESS_FB_AXISNO = 5340,
            SPIN_ADDRESS_FB_COMMAND,
            SPIN_ADDRESS_FB_RECIPE_COUNT,
            SPIN_ADDRESS_FB_RUNSTATE = 5346,
            SPIN_ADDRESS_FB_RUNSTEP = 5347,
            SPIN_ADDRESS_FB_ERRORCODE = 5348,
            SPIN_ADDRESS_FB_END = 5509
        }

        public enum MXP_DATA_TYPE
        {
            DATA_TYPE_BIT = 0x10,
            DATA_TYPE_WORD = 0x20,
            DATA_TYPE_L = 0x30,
            DATA_TYPE_F = 0x40,
            DATA_TYPE_A = 0x50,
            DATA_TYPE_BYTE = 0x60,
            DATA_TYPE_D = 0x70
        }

        public enum PLC_DEFINE
        {
            SPIN_FB_SIZE = 10,
            FEEDBACKSENSOR_FB_SIZE = 40,
            PLC_START_ADDRESS = 5000,
            PLC_END_ADDRESS = 6000
        }

        public enum LINEMOVE_FLAG
        {
            LINEMOVE_NULL = 0,
            LINEMOVE_CMD_IN = 1,
            LINEMOVE_COMPLETE = 2
        }

        public enum TRIGGER_DIRECTION_ENUM
        {
            POSITIVE_DIRECTION = 0,
            NEGATIVE_DIRECTION = 1,
            BOTH_DIRECTION = 2
        }

        public enum TRIGGER_MODE_ENUM
        {
            TRIGGER_MODE_DISTANCE = 0,
            TRIGGER_MODE_TIME = 1,
            TRIGGER_MODE_POSITION = 2
        }

        public enum TRIGGERCMD_MODE_ENUM
        {
            TRIGGERCMD_MODE_AXIS = 0,
            TRIGGERCMD_MODE_GROUP = 1
        }

        public enum TRIGGERLEVEL_ENUM
        {
            TRIGGER_ON = 0,
            TRIGGER_OFF = 1,
            TRIGGER_TOGGLE = 2
        }

        public enum EMOIO_MODE_ENUM
        {
            EMOIO_UNUSED = 0,
            EMOIO_ESTOP = 1,
            EMOIO_STOP = 2
        }
        public enum ET_REGISTER_ENUM
        {
            ET_REG_VENDERID = 0,
            ET_REG_PRODUCTCODE = 1,
            ET_READ_REVISIONNO = 2
        }

        public enum ExternalEncoder_Mode
        {
            ExternalEncoder_Mode_NONE = 0,  // 외부 엔코더 미사용
            ExternalEncoder_Mode_MotorEncoder = 1,
            ExternalEncoder_Mode_Both = 2,
            ExternalEncoder_Mode_ExternalEncoder = 3,
            ExternalEncoder_Mode_ReleaseWait = 4  // 외부 엔코더 사용중 해제시 ActualPulse Sync 상태 대기 
        }

        public enum INSTALL_MODE_ENUM
        {
            INSTALLMODE_NULL = -1,
            INSTALLMODE_INTIME = 0,
            INSTALLMODE_TRIAL = 1,
            INSTALLMODE_DEV = 2,
            INSTALLMODE_PCIe = 3
        }


        public enum ESYNC_CMD
        {
            ESYNC_CMD_Null = 0,
            ESYNC_CMD_GEARIN,
            ESYNC_CMD_GEARINPOS,
            ESYNC_CMD_CAM
        }

        public enum MXP_MOTOR_TYPE
        {
            MXP_P_AXIS_MOTOR_TYPE_ROTARY = 0,
            MXP_P_AXIS_MOTOR_TYPE_LINEAR,
            MXP_P_AXIS_MOTOR_TYPE_COIL,
            MXP_P_AXIS_MOTOR_TYPE_MOVER
        }


        public enum MXP_MMTSYSTEM_STATE
        {
            MMT_STATUS_NULL = 0,
            MMT_STATUS_ACTIVE,
            MMT_STATUS_INIT,
            MMT_STATUS_INIT_WAIT,
            MMT_STATUS_RECOVERY,
            MMT_STATUS_RECOVERY_WAIT,
            MMT_STATUS_FAIL,
            MMT_STATUS_READY,
            MMT_STATUS_RUN,
            MMT_STATUS_ALARM
        }


        public enum MXP_MMTSYSTEM_PLANE
        {
            MMT_PLANE_X = 0,
            MMT_PLANE_Y,
            MMT_PLANE_Z,
            MMT_PLANE_THETA
        }


        public enum MXP_MMT_TRACK_TYPE
        {
            MMT_TRACK_X = 0,
            MMT_TRACK_XY,
            MMT_TRACK_UPDOWN,
            MMT_TRACK_TURN
        }


        public enum MXP_MMT_INITMODE
        {
            MMT_LEFT_MODULE_INIT = 0,
            MMT_RIGHT_MODULE_INIT,
            MMT_CURRENTPOS_INIT,
        }
        public enum USAGE_ENUM
        {
            USAGE_UNUSED = 0,
            USAGE_USED = 1
        };

        public enum SYSTEM_POS_ENUM
        {
            SYSTEM_POS_MM = 0,
            SYSTEM_POS_INCH = 1,
            SYSTEM_POS_DEGREE = 2,
            SYSTEM_POS_RADIAN = 3,
            SYSTEM_POS_PULSE = 4,
            SYSTEM_POS_UM = 5
        };

        public enum SYSTEM_VEL_ENUM
        {
            SYSTEM_VEL_SEC = 0,
            SYSTEM_VEL_MIN = 1,
            SYSTEM_VEL_RPM = 2
        };

        public enum PRECISION_ENUM
        {
            PRECISION_10 = 0,
            PRECISION_1 = 1,
            PRECISION_0_1 = 2,
            PRECISION_0_01 = 3,
            PRECISION_0_001 = 4,
            PRECISION_0_0001 = 5
        };

        public enum CONTROL_MODE_ENUM
        {
            CONTROL_MODE_PP = 0,
            CONTROL_MODE_CSP = 1,
            CONTROL_MODE_CSV = 2,
            CONTROL_MODE_CST = 3
        };

        public enum HOME_CONTROL_ENUM
        {
            HOME_CONTROL_MXP = 0,
            HOME_CONTROL_SERVO = 1
        };

        public enum HOME_MODE_ENUM
        {
            HOME_MODE_SIMPLE = 0,
            HOME_MODE_1_STEP = 1,
            HOME_MODE_2_STEP = 2
        };

        public enum GROUP_AXIS_ENUM
        {
            GROUP_AXIS_X = 0,
            GROUP_AXIS_Y = 1,
            GROUP_AXIS_Z = 2,
            GROUP_AXIS_U = 3,
            GROUP_AXIS_V = 4,
            GROUP_AXIS_W = 5,
            GROUP_AXIS_A = 6,
            GROUP_AXIS_B = 7,
            GROUP_AXIS_C = 8
        };

        public enum ACTIVATION_ENUM
        {
            ACTIVATION_UNUSED = 0,
            ACTIVATION_USED = 1,
            ACTIVATION_VIRTUAL = 2
        };

        public enum CSV_PID_I_MODE_ENUM
        {
            CSV_PID_I_MODE_ALWAYS = 0,
            CSV_PID_I_MODE_VEL_DIFF_0 = 1
        };

        public enum MULTI_CONTROL_MODE_ENUM
        {
            MULTI_CONTROL_MODE_MASTER = 0,
            MULTI_CONTROL_MODE_YAW_CONTROL = 1,
            MULTI_CONTROL_MODE_CROSS_COUPLED = 2,
            MULTI_CONTROL_MODE_TORQUE = 3,
            MULTI_CONTROL_MODE_VELOCITY = 4,
            MULTI_CONTROL_MODE_TORQUE_BOOST = 5
        };

        public enum DIRECTION_ENUM
        {
            DIRECTION_CCW = 0,
            DIRECTION_CW = 1
        };

        public enum MOTOR_TYPE_ENUM
        {
            MOTOR_TYPE_ROTARY = 0,
            MOTOR_TYPE_LINEAR = 1,
            MOTOR_TYPE_COIL = 2,
            MOTOR_TYPE_MOVER = 3
        };

        public enum SERVO_INDEX_PULSE_TYPE_ENUM
        {
            SERVO_INDEX_PULSE_TYPE_SINGLE_TURN = 0,
            SERVO_INDEX_PULSE_TYPE_TOUCH_PROB1 = 1,
            SERVO_INDEX_PULSE_TYPE_TOUCH_PROB1_INDEX = 2,
            SERVO_INDEX_PULSE_TYPE_TOUCH_PROB2 = 3,
            SERVO_INDEX_PULSE_TYPE_TOUCH_PROB2_INDEX = 4
        };

        public enum FOLLOWINGMODE_MASTER_SOURCE_ENUM
        {
            FOLLOWINGMODE_MASTER_SOURCE_ACTUAL = 0,
            FOLLOWINGMODE_MASTER_SOURCE_TARGET = 1
        };

        public enum FOLLOWINGMODE_MASTER_OFFSET_MODE_ENUM
        {
            FOLLOWINGMODE_MASTER_OFFSET_MODE_UNUSED = 0,
            FOLLOWINGMODE_MASTER_OFFSET_MODE_VERTICAL = 1,
            FOLLOWINGMODE_MASTER_OFFSET_MODE_HORIZONTAL = 2,
            FOLLOWINGMODE_MASTER_OFFSET_MODE_LOWPASS = 3
        };

        public enum INPOS_MODE_ENUM
        {
            INPOS_MODE_UNUSED = 0,
            INPOS_MODE_MASTER = 1,
            INPOS_MODE_DRIVE = 2
        };

        public enum EMO_IO_MODE_ENUM
        {
            EMO_IO_MODE_UNUSED = 0,
            EMO_IO_MODE_ESTOP = 1,
            EMO_IO_MODE_NORMAL = 2
        };

        public enum EMO_IO_EDGE_ENUM
        {
            EMO_IO_EDGE_HIGH = 0,
            EMO_IO_EDGE_LOW = 1
        };

        public enum GENTRY_HOME_MODE_ENUM
        {
            GENTRY_HOME_MASTER = 0,
            GENTRY_HOME_MASTER_SLAVE_SINGLE_TURN = 1,
            GENTRY_HOME_MASTER_HORIZONTAL_COMP = 2,
            GENTRY_HOME_MASTER_SLAVE_SENSOR = 3,
            GENTRY_HOME_MASTER_SLAVE_TOUCH_PROB = 4
        };

        public enum TORQUE_VELOCITY_FOLLOWING_COMPENSATION_LOOP_MODE_ENUM
        {
            TORQUE_VELOCITY_FOLLOWING_COMPENSATION_LOOP_MODE_UNUSED = 0,
            TORQUE_VELOCITY_FOLLOWING_COMPENSATION_LOOP_MODE_ALWAYS = 1,
            TORQUE_VELOCITY_FOLLOWING_COMPENSATION_LOOP_MODE_MOVE_FLAG_ON = 2
        };

        public enum PID_MODE_ENUM
        {
            PID_NORMAL = 0,
            PID_TORQUE_BOOST = 1,
            PID_CUSTOM_OPTION1 = 2
        };

        public enum MASTER_POSITION_SYNC_MODE_ENUM
        {
            MASTER_POSITION_SYNC_MODE_UNUSED = 0,
            MASTER_POSITION_SYNC_MODE_USED = 1
        };

        public enum FOLLOWING_LIMIT_MODE_ENUM
        {
            FOLLOWING_LIMIT_MODE_UNUSED = 0,
            FOLLOWING_LIMIT_MODE_SEQUENCE_MOVE = 1,
            FOLLOWING_LIMIT_MODE_USER_SETTING = 2
        };
        #endregion

        #region MXP Structure Define
        public struct MXP_File_TABLE_IN
        {
            public Byte tableIndexNo;
            public Single position;
            public Single velocity;
            public Single time;
            public Single diffPosition;
            public Single diffVelocity;
            public Single acceleration;
        }

        public struct MXP_PROFILE_TABLE_IN
        {
            public Byte tableIndexNo;
            public Byte motionMode;
            public Single position;
            public Single velocity;
            public Single acceleration;
            public Single deceleration;
            public Single jerk;
            public Int32 direction;
            public Int32 bufferMode;
        }

        public struct MXP_IO_TABLE_IN
        {
            public Byte tableIndexNo;
            public UInt16 slaveNo;
            public UInt16 bitPos;
            public Byte bitValue;
        }

        public struct MXP_MULTIAXISCOUPLE_SINGLEAXISGAIN
        {
            public UInt16 feedForward_V_Gain;
            public UInt16 feedForward_A_Gain;
            public UInt16 pGain;
            public UInt16 iGain;
            public UInt16 dGain;
        }

        public struct MXP_MULTIAXISCOUPLE_CCCGAIN
        {
            public UInt16 coupledPGain;
            public UInt16 coupledIGain;
        }

        public struct MXP_FILE_TABLE_ARRAY_IN
        {
            public UInt32 dataSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public MXP_File_TABLE_IN[] fileTable;
        }

        public struct MXP_PROFILE_TABLE_ARRAY_IN
        {
            public UInt32 dataSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public MXP_PROFILE_TABLE_IN[] profileTable;
        }

        public struct MXP_IO_TABLE_ARRAY_IN
        {
            public UInt32 dataSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public MXP_IO_TABLE_IN[] ioTable;
        }

        public struct MXP_CAMTABLE_REF
        {
            public Single masterPos;
            public Single slavePos;
            public Single slaveVel;
            public Single slaveAcc;
            public Single slaveJerk;
            public UInt32 pointType;
            public Int32 interpolationType;
        }
        public struct MXP_MOVER_INFO
        {
            public Int16 MoverIndex;
            public Int16 CurTrackID;
            public UInt16 DriveID;
            public Int16 LeftCoilNo;
            public Int16 MiddleCoilNo;
            public Int16 RightCoilNo;
            public Int16 VirtualAxisNo;
            public Int16 AlarmCode;
            public UInt16 MoverState;
            public UInt16 MovingPlane;


            public Single Position_X;
            public Single Position_Y;
            public Single Position_Z;
            public Single Position_Theta;


        }

        public struct MXP_MMT_MOVERDATA
        {
            public Int16 LeftCoilNo;
            public Int16 MiddleCoilNo;
            public UInt16 TrackID;
            public UInt16 MoverID;
            public Single PositionX;
            public Single PositionY;
            public Single PositionZ;
            public Single PositionTheta;
            public Int32 nErrorCode;
        }




        public struct MXP_MMT_MOVERINFO_SEARCHED
        {
            public UInt16 MoverCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
            public MXP_MMT_MOVERDATA[] MoverData;

        }

        public struct PORT_STATE
        {
            public MXP_PORT_STATE port1State;
            public MXP_PORT_STATE port2State;
            public MXP_PORT_STATE port3State;
            public MXP_PORT_STATE port4State;
        }

        public struct FBPROCESS_CHECK
        {
            public FB_RUN_STATE runState;
            public UInt16 errorID;
            public Byte runStep;
        }

        public struct PROCESS_CHECK
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public UInt16 errorID;
        }

        public struct READ_CAMTABLE_REPLY
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public UInt16 errorID;
            public UInt16 tableRowCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 400)]
            public MXP_CAMTABLE_REF[] camDataArray;
        }

        public struct READ_CAMSLAVEPOSITION_REPLY
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public UInt16 errorID;
            public Single slavePos;
        }

        public struct AXIS_ERROR
        {
            public UInt16 mxpError;
            public UInt16 driveError;
        }

        public struct READ_ECAT_PARAMETER_REPLY
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public UInt16 errorID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Byte[] readData;
        }

        public struct ACCDEC_TO_ACCTIME_REPLY
        {
            public Single accDecBuildUp;
            public Single limitAccDec;
            public Single accDecRampDown;
        }

        public struct ACCTIME_TO_ACCDEC_REPLY
        {
            public Single accDec;
            public Single jerk;
        }

        public struct TOUCHPROBE_READPOS_REPLY
        {
            public Single edgePositivePosition;
            public Single edgeNegativePosition;
        }

        public struct TOUCHPROBE_STATE
        {
            public Byte touchprobeUsing;
            public Byte touchprobeRisingEdgeSave;
            public Byte touchprobeFallingEdgeSave;
            public Byte touchprobeRisingPositionUpdate;
            public Byte touchprobeFallingPositionUpdate;
        }

        public struct MXP_AXIS_STATEBIT
        {
            public Byte errorStop;
            public Byte disabled;
            public Byte stopping;
            public Byte standstill;
            public Byte discreteMotion;
            public Byte continuousMotion;
            public Byte synchronizedMotion;
            public Byte homing;
            public Byte constantVelocity;
            public Byte accelerating;
            public Byte decelerating;
            public Byte directionPositive;
            public Byte directionNegative;
            public Byte homeAbsSwitch;
            public Byte hardwareLimitPositiveEvent;
            public Byte hardwareLimitNegativeEvent;
            public Byte readyForPowerOn;
            public Byte powerOn;
            public Byte isHomed;
            public Byte inPos;
            public Byte axisWarning;
            public Byte servoFault;
            public Byte servoWarning;
            public Byte servoTargetReached;
        }

        public struct GROUP_POS
        {
            public Single posX;
            public Single posY;
            public Single posZ;
            public Single posU;
            public Single posV;
            public Single posW;
            public Single posA;
            public Single posB;
            public Single posC;
        }

        public struct SEQUENCEMOVE_PROCESS_CHECK
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public Int32 errorID;
        }

        public struct SEQUENCEMOVE_STEP
        {
            public Single pos;
            public Single vel;
            public Single acc;
            public Single dec;
            public Single jerk;
            public Byte velocityLimitUsing;
        }

        public struct SEQUENCEMOVE_CMD
        {
            public SEQUENCE_CMD_TYPE cmdType;
            public Single pos;
            public Single vel;
            public Single acc;
            public Single dec;
            public Single jerk;
            public Single dwellTime;
            public UInt16 ioSlaveNo;
            public UInt16 ioBitNo;
            public Byte ioBitSet;
            public MXP_DIRECTION_ENUM direction;
            public MXP_BUFFERMODE_ENUM bufferMode;
            public Byte velocityLimitUsing;
            public StringBuilder stepName;
        }

        public struct SEQUENCEMOVE_GROUP_CMD
        {
            public SEQUENCE_CMD_TYPE cmdType;
            public Single posX;
            public Single posY;
            public Single posZ;
            public Single posU;
            public Single posV;
            public Single posW;
            public Single posA;
            public Single posB;
            public Single posC;
            public Single vel;
            public Single acc;
            public Single dec;
            public Single jerk;
            public Single dwellTime;
            public UInt16 ioSlaveNo;
            public UInt16 ioBitNo;
            public Byte ioBitSet;
            public MXP_BUFFERMODE_ENUM bufferMode;
            public StringBuilder stepName;
            public MXP_PATHCHOICE_ENUM pathChoice;
            public Single auxPoint1;
            public Single auxPoint2;
            public Single endPoint1;
            public Single endPoint2;
            public MXP_PLANE plane1;
            public MXP_PLANE plane2;
        }

        public struct SPINTABLE
        {
            public Single stepVel;
            public Single stepTime;
        }

        public struct SPINRECIPE_WRITE_TABLE
        {
            public Single vel;
            public Single acc;
            public Single jerk;
            public Int32 time;
        }

        public struct PLC_FB_RUNINFO
        {
            public Byte use;
            public Int32 fbIndex;
            public FB_RUN_MODE runState;
        }

        public struct GROUP_AXIS_CONFIG
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public Int32[] groupAxisID;
        }
        public struct READ_2DError_REPLY
        {
            public Byte busy;
            public Byte done;
            public Byte errorOn;
            public Byte errorID;
            public Single compensationX;
            public Single compensationY;
        }
        #endregion

        #region System
        /// <summary>
        /// internal initialization to use the MXP.
        /// The result of Calling the function is returned. 
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Init();

        /// <summary>
        /// internal initialization to use the MXP.
        /// The result of Calling the function is returned.
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Init_Developer();

        /// <summary>
        /// This function runs the MXP.
        /// The result of calling the function is returned.
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Run();

        /// <summary>
        /// This function stops the MXP.
        /// The result of calling the function is returned.
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Stop();

        /// <summary>
        /// This function is called to terminate the connection with the MXP.<para />
        /// The result of calling the function is returned.<para />
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Destroy();

        /// <summary>
        /// This function is called to terminate the connection with the MXP.<para />
        /// The result of calling the function is returned.<para />
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Destroy_Developer();

        /// <summary>
        /// This function returns the current status of the MXP State.<para />
        /// Before Calling this function. initialize the MXP by calling SYS_Init.<para />
        /// The result of calling the function is returned.<para />
        /// </summary>
        /// <param name="status">return the current status of the MXP.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetStatus(ref KERNEL_STATUS status);

        /// <summary>
        /// This function check the MXP license.
        /// The result of calling the function is returned.<para />
        /// </summary>
        /// <param name="function">Set MXP_USERLib value</param>
        /// <param name="status">Return the licence Status(valid : true, invalid : false)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_CheckLicense(MXP_USERLIB function, ref Byte status);

        /// <summary>
        /// This function returns the network state of the current EtherCAT.
        /// the result of calling the function is returned.
        /// </summary>
        /// <param name="status">return the current EtherCAT master.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetMasterOnlineMode(ref MXP_ONLINE_STATE status);

        /// <summary>
        /// This function returns the MXP Run Mode
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="simulation">return MXP simulation Mode.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetMasterSimulationMode(ref Byte simulation);

        /// <summary>
        /// When MXP state is running, Set the mode of master 
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="mode">Set the mode(0:Init, 1:ProOP, 2:Boot, 4:SafeOP, 8:OP)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_SetMasterOnlineMode(EC_NETWORK_CMD mode);

        /// <summary>
        /// When MXP state is running, Set the mode of slave 
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="slaveNo">Set the slave number</param>
        /// <param name="mode">Set the mode(0:Init, 1:ProOP, 2:Boot, 4:SafeOP, 8:OP)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_SetSlaveOnlineMode(UInt32 slaveNo, EC_NETWORK_CMD mode);

        /// <summary>
        /// This function Restart the MXP 
        /// The result of calling the function is returned.
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_Reset();
        #endregion

        #region Slave
        /// <summary>
        /// This function obtains slave number using the NodeID<para />
        /// The result of Calling the function is returned.
        /// </summary>
        /// <param name="nodeID">NodeID of the EtherCAT module(0~255)</param>
        /// <param name="slaveNo">Return slave number of the EtherCAT module(0~127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveNoFromNodeId(UInt32 nodeID, ref UInt32 slaveNo);

        /// <summary>
        /// This function obtains axis number using the NodeID. <para />
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="nodeID">NodeID of the EtherCAT module(0~255)</param>
        /// <param name="axisNo">Return axis number of the EtherCAT module(0~127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetAxisNoFromNodeId(UInt32 nodeID, ref UInt32 axisNo);

        /// <summary>
        /// Return the network state of an individual slave. <para />
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="slaveNo">Slave number of the EtherCAT module(0~127)</param>
        /// <param name="status">Return the EtherCAT network state(0~0x18). refer MXP_ONLINE_STATE</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveCurrentState(UInt32 slaveNo, ref MXP_ONLINE_STATE status);

        /// <summary>
        /// Return the number of slave devices currently connected.
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="slaveCount">Return the number of slave devices currently connected</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveCount(ref UInt32 slaveCount);

        /// <summary>
        /// Return the number of Axis devices currently connected.
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="axisCount">Return the number of axes currently connected</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetAxisCount(ref UInt32 axisCount);

        /// <summary>
        /// Return the type of a slave node.
        /// The result of calling the function is returned.
        /// </summary>
        /// <param name="slaveNo">Number of the slave module</param>
        /// <param name="type">Return the node type(1 : Servo Drive, 2 :IO)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveType(UInt32 slaveNo, ref MXP_NODE_TYPE type);

        /// <summary>
        /// This function returns the hardware network connect state of the EtherCAT master <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="connectState">Return the PortState</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetMasterEtherCATLineStatus(ref Byte connectState);

        ///<summary> 
        /// This function returns the hardware network connect state of the EtherCAT slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the slave number</param>
        /// <param name="portState">Return the PortState(port1 ~ port4)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveEtherCATLineStatus(UInt32 slaveNo, ref PORT_STATE portState);

        /// <summary>
        /// This function returns the EtherCAT network alarm history<para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="pArrAlarm">Return alarm history</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetEtherCATHistoryAlarm(ref Int32 pArrAlarm);

        /// <summary>
        /// Return the Slave name of entered slave number
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the Slave number</param>
        /// <param name="name">Return the slave name</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetSlaveName(UInt32 slaveNo, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);

        /// <summary>
        /// Check the slave communication ready status
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the Slave number</param>
        /// <param name="ready">Return the value which check the slave communication status</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_SlaveReadyCheck(UInt32 slaveNo, ref Byte ready);

        /// <summary>
        /// Check the All slaves communication ready status
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="ready">Return the value which check the All slaves communication status</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadyCheck(ref Byte ready);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_PDOMappingCheck(UInt32 axisNo, MXP_PDO_DIRECTION direction, UInt16 pdoIndex, ref Byte checkState);
        #endregion

        #region IO
        /// <summary>
        ///  This function writes Byte array value to the selected slave <para />
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of the slave</param>
        /// <param name="offset">Start address of array to write</param>
        /// <param name="size">Size of the write Byte array</param>
        /// <param name="pData">Byte array value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_Write(UInt32 slaveNo, UInt16 offset, UInt16 size, Byte[] pData);

        /// <summary>
        /// This function writes bit value to the selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of the slave</param>
        /// <param name="offset">Start address to write</param>
        /// <param name="bitOffset">Bit number to write(0 ~ 7)</param>
        /// <param name="data">Bit value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_WriteBit(UInt32 slaveNo, UInt16 offset, Byte bitOffset, Byte data);

        /// <summary>
        /// This function writes Byte value to the selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of the slave</param>
        /// <param name="offset">Start address to write</param>
        /// <param name="data">Byte value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_WriteByte(UInt32 slaveNo, UInt16 offset, Byte data);

        /// <summary>
        /// This function writes word value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="offset">Start address to write</param>
        /// <param name="data">word value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_WriteWord(UInt32 slaveNo, UInt16 offset, UInt16 data);

        /// <summary>
        /// This function writes Dword value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="offset">Start address to write</param>
        /// <param name="data">Dword value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_WriteDword(UInt32 slaveNo, UInt16 offset, UInt32 data);

        /// <summary>
        /// This function reads Byte array value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="ioType">MXP_IO_Type(IO_IN or IO_OUT)</param>
        /// <param name="offset">Start address of Byte array to read</param>
        /// <param name="size">Size of the Byte array to read</param>
        /// <param name="pData">Byte array value to read</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_Read(UInt32 slaveNo, MXP_IO_TYPE ioType, UInt16 offset, UInt16 size, ref Byte pData);

        /// <summary>
        /// This function reads bit value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="ioType">MXP_IO_Type(IO_IN or IO_OUT)</param>
        /// <param name="offset">Start address to read</param>
        /// <param name="bitOffset">Bit number of address to read</param>
        /// <param name="data">Return bit value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_ReadBit(UInt32 slaveNo, MXP_IO_TYPE ioType, UInt16 offset, Byte bitOffset, ref Byte data);

        /// <summary>
        /// This function read Byte value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="ioType">MXP_IO_Type(IO_IN or IO_OUT)</param>
        /// <param name="offset">Start address to read</param>
        /// <param name="data">Return Byte value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_ReadByte(UInt32 slaveNo, MXP_IO_TYPE ioType, UInt16 offset, ref Byte data);

        /// <summary>
        /// This function read word value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Number of slave</param>
        /// <param name="ioType">MXP_IO_Type(IO_IN or IO_OUT)</param>
        /// <param name="offset">Start address to read</param>
        /// <param name="data">Return word value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_ReadWord(UInt32 slaveNo, MXP_IO_TYPE ioType, UInt16 offset, ref UInt16 data);

        /// <summary>
        /// This function read Dword value to selected slave <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">number of slave</param>
        /// <param name="ioType">MXP_IO_Type(IO_IN or IO_OUT)</param>
        /// <param name="offset">Start address to read</param>
        /// <param name="data">Return Dword value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT IO_ReadDword(UInt32 slaveNo, MXP_IO_TYPE ioType, UInt16 offset, ref UInt32 data);
        #endregion

        #region Single Motion
        /// <summary>
        /// Control the servo on/off state of the specified axis <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="enable">True : Servo On, False : Servo Off</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Power(UInt32 axisNo, Byte enable);

        /// <summary>
        ///  Reset all axis-related errors and Change the state of the axis from errorStop to standstill <para />
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Reset(UInt32 axisNo);

        /// <summary>
        /// Command the servo axis to perform the homing motion <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Home(UInt32 axisNo);

        /// <summary>
        /// When selected axis is in the standstill state, Reset the actual position to set Position. <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="position">Set position value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetPosition(UInt32 axisNo, Single position);

        /// <summary>
        /// Stop the ongoing motion and Transfer the axis to the standstill state <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="dec">Set Decelation value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Halt(UInt32 axisNo, Single dec, Single jerk);

        /// <summary>
        ///  Stop the ongoing motion and Transfer the axis to the stopping state <para />
        ///  To perform another motion command, the execute parameter must be changed from true to false
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="execute ">Execute the command with rising edge</param>
        /// <param name="dec">Set Decelation value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Stop(UInt32 axisNo, Byte execute, Single dec, Single jerk);

        /// <summary>
        /// Move the axis from the actual position to a specified absolute position <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="position">Set Position value</param>
        /// <param name="vel">Set Velocity value</param>
        /// <param name="acc">Set Acceleration value</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <param name="direction">Set Direction of movement</param>
        /// <param name="bufferMode">Set Buffer mode </param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveAbsolute(UInt32 axisNo, Single position, Single vel, Single acc,
        Single dec, Single jerk, MXP_DIRECTION_ENUM direction = MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        /// <summary>
		/// This is a function that determines the direction of movement of the input command location and then moves targetpos <para />
		/// The result of calling the function is returned
		/// </summary>
		/// <param name="axisNo">Axis number(0 ~ 127)</param>
		/// <param name="position">Set Position value</param>
		/// <param name="vel">Set Velocity value</param>
		/// <param name="acc">Set Acceleration value</param>
		/// <param name="dec">Set Deceleration value</param>
		/// <param name="jerk">Set Jerk value</param>
		/// <param name="direction">Set Direction of movement</param>
		/// <returns></returns>
		[DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveAbsoluteWithDirection(UInt32 axisNo, Single position, Single vel, Single acc,
        Single dec, Single jerk, MXP_DIRECTION_ENUM direction = MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION);


        /// <summary>
        /// Setting the override position for the entire interval performs 
        /// the corresponding velocity override function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="targetPosition">Set Target Position value</param>
        /// <param name="targetVel">Set Target Velocity value</param>
        /// <param name="overrideCount">Set Override Count</param>
        /// <param name="overridePosition">Set Override Position</param>
        /// <param name="overrideVelocity">Set Override Velocity</param>
        /// <param name="acc">Set Acceleration value</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <param name="bufferMode">Set Buffer mode</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveAbsolute_VelocityOverride(UInt32 axisNo, Single targetPosition, Single targetVel, Int16 overrideCount,
        Single[] overridePosition, Single[] overrideVelocity, Single acc, Single dec, Single jerk, MXP_BUFFERMODE_ENUM bufferMode);

        /// <summary>
        /// Move the axis from the actual position by the distance set in the distance input <para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="distance">Set Distance</param>
        /// <param name="vel">Set Velocity value</param>
        /// <param name="acc">Set Acceleration value</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <param name="bufferMode">Set Buffer mode</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveRelative(UInt32 axisNo, Single distance, Single vel, Single acc,
        Single dec, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        /// <summary>
        /// Move the axis from the actual position by the distance set in the distance input<para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="distance">Set Distance</param>
        /// <param name="vel">Set Velocity value</param>
        /// <param name="acc">Set Acceleration value</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveSuperImposed(UInt32 axisNo, Single distance, Single vel, Single acc, Single dec, Single jerk);


        /// <summary>
        /// Stop the ongoing superimposed motion<para />
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_HaltSuperImposed(UInt32 axisNo, Single dec, Single jerk);
        /// Move the axis at the velocity specified in the velocity parameter 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="vel">Set Velocity value</param>
        /// <param name="acc">Set Acceleration value </param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <param name="direction">Set Direction of movement</param>
        /// <param name="bufferMode">Set Buffer mode</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveVelocity(UInt32 axisNo, Single vel, Single acc, Single dec, Single jerk,
        MXP_DIRECTION_ENUM direction, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        /// <summary>
        /// Set the velocity rate for all function
        /// The factor can be set as a real number from 0 to 1(0 ~ 100%) 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="velFactor">Set the velocity scale factor as a multiplier of a real number</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetOverride(UInt32 axisNo, Single velFactor);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetOverride_Ex(UInt32 axisNo, Single velFactor, Single overrideTime);



        #endregion

        #region GearIn
        /// <summary>
        /// Command a controlled motion on an electronic gear 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="ratioNumerator">Set Gear ratio of the slave axis</param>
        /// <param name="ratioDenominator">Set Gear ratio of the master axis</param>
        /// <param name="masterValueSource">Select master data for synchronization 
        /// <param name="acc">Set Acceleration value</param>
        /// <param name="dec">Set Deceleration value</param>
        /// <param name="jerk">Set Jerk value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearIn(UInt32 masterAxis, UInt32 slaveAxis, Int32 ratioNumerator, UInt32 ratioDenominator,
        MXP_SOURCE_ENUM masterValueSource, Single acc, Single dec, Single jerk);

        /// <summary>
        /// Return a Status and ErrorID of the slave which is executed GearIn command 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="status">Return the GearIn status.(type : MXPEasyClass.PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearInMonitor(UInt32 slaveAxis, ref PROCESS_CHECK status);

        /// <summary>
        /// Disengage a slave axis from the master axis 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearOut(UInt32 slaveAxis);
        #endregion

        #region GearInPos
        /// <summary>
        /// Command a controlled motion on an electronic gear 
        /// Unlike AX_GearIn, Set the position in which the slave and master axes are synchronized 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="ratioNumerator">Set Gear ratio of the slave axis</param>
        /// <param name="ratioDenominator">Set Gear ratio of the master axis</param>
        /// <param name="masterValueSource">Select master data for synchronizition</param>
        /// <param name="masterSyncPos">Set the position where synchronization of the master axis is completed</param>
        /// <param name="slaveSyncPos">Set the position where synchronization of the slave axis is completed</param>
        /// <param name="masterStartDistance">Set the position of the master axis where the master axis starts synchronizing with the slave axis</param>
        /// <param name="vel">Set Maximum Velocity of the slave axis until the synchronization is complete</param>
        /// <param name="acc">Set Maximum Acceleration of the slave axis until the synchronization is complete</param>
        /// <param name="dec">Set Maximum Deceleration of the slave axis until the synchronization is complete</param>
        /// <param name="jerk">Set Maximum Jerk of the slave axis until the synchronization is complete</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearInPos(UInt32 masterAxis, UInt32 slaveAxis, Int32 ratioNumerator, UInt32 ratioDenominator, MXP_SOURCE_ENUM masterValueSource,
        Single masterSyncPos, Single slaveSyncPos, Single masterStartDistance, Single vel, Single acc, Single dec, Single jerk);

        /// <summary>
        /// Return a Status and ErrorID of the slave which is executed GearInPos command 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="status">Return the GearInPos status.(type : MXPEasyClass.PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearInPosMonitor(UInt32 slaveAxis, ref PROCESS_CHECK status);

        /// <summary>
        /// Disengage a slave axis from the master axis 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GearInPosOut(UInt32 slaveAxis);
        #endregion

        #region CAM
        /// <summary>
        /// Read the CAM table to use the electronic CAM function 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="camTableID">Set the number of the table to be read in</param>
        /// <param name="periodic">Specify whether to execute the table periodically. (0 : NonPeriodic, 1 : Periodic)
        /// false : NonPeriodic</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamTableSelect(UInt32 masterAxis, UInt32 slaveAxis, UInt16 camTableID, Byte periodic);

        /// <summary>
        /// Write the CAM table data 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">Set the table number to write</param>
        /// <param name="execute ">Execute the command with rising edge</param>
        /// <param name="tableRowCount">Set the number of table rows to write</param>
        /// <param name="pArrCamData">CAM table value to write</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamWriteTableRequest(UInt16 camTableID, Byte execute, UInt16 tableRowCount, MXP_CAMTABLE_REF[] pArrCamData);

        /// <summary>
        /// Return a Status and ErrorID of the CAM table which is executed AX_CamWriteTableRequest command 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">CamTableID(0~63)</param>
        /// <param name="status">Return the CAMWriteTable status.(type : MXPEasyClass.PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamWriteTableReply(UInt16 camTableID, ref PROCESS_CHECK status);

        /// <summary>
        /// Execute the AX_CamWriteTableRequest and AX_CamWriteTableReply Command
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">CamTableID(0~63)</param>
        /// <param name="tableRowCount">Set the number of table rows to write</param>
        /// <param name="pArrCamData">CAM table value to write</param>
        /// <param name="waitTime">Set the wait time(ms) before AX_CamWriteTableReply Command</param>
        /// <param name="status">Return the CAMWriteTable status.(type : MXPEasyClass.PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamWriteTable(UInt16 camTableID, UInt16 tableRowCount, MXP_CAMTABLE_REF[] pArrCamData, Int32 waitTime, ref PROCESS_CHECK status);

        /// <summary>
        /// Read the existing CAM table file 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">Set the table number to read</param>
        /// <param name="execute ">Execute the command with rising edge</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadTableRequest(UInt16 camTableID, Byte execute);

        /// <summary>
        /// Return a Status and ErrorID of the CAM table which is executed AX_CamReadTableRequest command 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">Set the table number to read</param>
        /// <param name="status">Return the CAMReadTable status(Type:READ_CAMTABLE_REPLY)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadTableReply(UInt16 camTableID, ref READ_CAMTABLE_REPLY status);

        /// <summary>
        /// Execute the AX_CamReadTableRequest and AX_CamReadTableReply Command
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">Set the table number to read</param>
        /// <param name="waitTime">Set the wait time(ms) before AX_CamReadTableReply Command</param>
        /// <param name="status">Return the CAMReadTable status(Type:READ_CAMTABLE_REPLY)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadTable(UInt16 camTableID, Int32 waitTime, ref READ_CAMTABLE_REPLY status);

        /// <summary>
        /// Engage the electronic CAM(CAM table must have been set by AX_CamTableSelect) 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="masterOffset">Set the offset for the table master position</param>
        /// <param name="slaveOffset">Set the offset for the table slave position</param>
        /// <param name="masterScaling">Set the scaling factor for the table master position</param>
        /// <param name="slaveScaling">Set the scaling factor for the table slave position </param>
        /// <param name="masterSyncPosition">Set the position where synchronization of the slave axis is completed</param>
        /// <param name="masterStartDistance">Set the master distance for the slave to start synchronizing with the master</param>
        /// <param name="startMode">Set the talbe position date type to start synchronizition</param>
        /// <param name="masterValueSource">Select the source data of the master axis for synchronization</param>
        /// <param name="camTableID">Select the CAM table ID for synchronization</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamIn(UInt32 masterAxis, UInt32 slaveAxis, Single masterOffset, Single slaveOffset, Single masterScaling, Single slaveScaling,
        Single masterSyncPosition, Single masterStartDistance, MXP_STARTMODE_ENUM startMode, MXP_SOURCE_ENUM masterValueSource, UInt16 camTableID);

        /// <summary>
        /// Sets the electronic CAM 
        /// behavior to the master of an external Encoder or IO signal.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="externalENCSlaveNo">Sets the external Encoder input Slave number.</param>
        /// <param name="externalENCSlavePos">Sets the external Encoder input address.</param>
        /// <param name="externalENCSlaveSize">Sets the external Encoder input size.</param>
        /// <param name="externalENCResolution">Sets the external Encoder resolution.</param>
        /// <param name="externalENCPulseToCamMasterUnit">Enter the external Encoder pulse required for user input unit 1.</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="masterOffset">Sets the offset of the master axis.</param>
        /// <param name="slaveOffset">Sets offset for slave axis</param>
        /// <param name="masterScaling">Set the scaling of the master axis.</param>
        /// <param name="slaveScaling">Set the scaling of the slave axis.</param>
        /// <param name="masterSyncPosition">Sets the position of the master axis 
        /// where the slave axis will complete synchronization with the master axis.</param>
        /// <param name="masterStartDistance">Sets the Distance of the master axis 
        /// to start synchronizing with the master axis.</param>
        /// <param name="startMode">Sets the Start Mode</param>
        /// <param name="masterValueSource">Select the data of the master axis to be synchronized.</param>
        /// <param name="camTableID">Select the table to use for Cam operation. (0~63)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamIn_MasterIO(UInt32 externalENCSlaveNo, UInt32 externalENCSlavePos, Byte externalENCSlaveSize, UInt32 externalENCResolution,
        Single externalENCPulseToCamMasterUnit, UInt32 slaveAxis, Single masterOffset, Single slaveOffset, Single masterScaling, Single slaveScaling,
        Single masterSyncPosition, Single masterStartDistance, MXP_STARTMODE_ENUM startMode, MXP_SOURCE_ENUM masterValueSource, UInt16 camTableID);

        /// <summary>
        /// Return a Status and ErrorID of the CAM table which is executed AX_CamIn command 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="status">Return the Status to check Cam In(Type:PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamInMonitor(UInt32 slaveAxis, ref PROCESS_CHECK status);

        /// <summary>
        /// Disengage a slave axis from the master axis 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamOut(UInt32 slaveAxis);

        /// <summary>
        /// A CAM plate coupling can be scaled with AX_CamScaling 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="activationMode">Specify the scaling time and position</param>
        /// <param name="activationPos">Master position at which a cam plate is scaled, dapending on the ActivationMode</param>
        /// <param name="masterScaling">Scaling of the master position of the cam plate</param>
        /// <param name="slaveScaling">Scaling of the slave position of the cam plate</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamScaling(UInt32 masterAxis, UInt32 slaveAxis, MXP_ACTIVATIONMODE activationMode, Single activationPos,
        Single masterScaling, Single slaveScaling);

        /// <summary>
        /// Achieve an offset of the slave axis with respect to the master axis 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="phaseShift">The calculated phase shift is transferred to the slave axis as the master axis position.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Phasing(UInt32 masterAxis, UInt32 slaveAxis, Single phaseShift);

        /// <summary>
        /// Achieve an offset of the slave axis with respect to the master axis 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="masterAxis">Master Axis number(0 ~ 127)</param>
        /// <param name="slaveAxis">Slave Axis number(0 ~ 127)</param>
        /// <param name="phaseShift">The calculated phase shift is transferred to the slave axis as the master axis position</param>
        /// <param name="phaseVel">Shift Velocity</param>
        /// <param name="phaseAcc">Shift accerlation</param>
        /// <param name="phaseJerk">Shift Jerk</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Phasing_Ex(UInt32 masterAxis, UInt32 slaveAxis, Single phaseShift, Single phaseVel, Single phaseAcc, Single phaseJerk = 0);

        /// <summary>
        /// Determine the slave position at a certain point of a cam plate table 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">CAM table ID to request(0~63)</param>
        /// <param name="execute ">Execute the command with rising edge</param>
        /// <param name="masterPos">Master position within the table for which the slave position is to be determined</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadSlavePositionRequest(UInt16 camTableID, Byte execute, Single masterPos);

        /// <summary>
        ///  Return the slave position at a certain point of a cam plate table 
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">CAM table ID to reply(0~63)</param>
        /// <param name="status">Return the Cam slave position(Type:READ_CAMSLAVEPOSITION_REPLY)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadSlavePositionReply(UInt16 camTableID, ref READ_CAMSLAVEPOSITION_REPLY status);

        /// <summary>
        /// Execute the AX_CamReadSlavePositionRequest and AX_CamReadSlavePositionReply Command
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="camTableID">CAM table ID(0~63)</param>
        /// <param name="masterPos">Master position within the table for which the slave position is to be determined</param>
        /// <param name="waitTime">Set the wait time(ms) before AX_CamReadSlavePositionReply Command</param>
        /// <param name="status">Return the Cam slave position(Type:READ_CAMSLAVEPOSITION_REPLY)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamReadSlavePosition(UInt16 camTableID, Single masterPos, Int32 waitTime, ref READ_CAMSLAVEPOSITION_REPLY status);
        #endregion

        #region ProfileMove
        /// <summary>
        /// When driving the Recipe, convert the transfer table(Step,Position,Velocity) according to the Step to the table for the MXP and return it 
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="userTable">Sets the UserTable. It starts from 0</param>
        /// <param name="userTableCount">Set the total driving step of the user table</param>
        /// <param name="motionTable">Return the profile table value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetProfileTable(ref MXP_FILE_TABLE_ARRAY_IN userTable, UInt16 userTableCount, ref MXP.MXP_PROFILE_TABLE_ARRAY_IN motionTable);

        /// <summary>
        ///  It is a function that makes profile movement
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="motionTableCount">Set the profile table count</param>
        /// <param name="ioTableCount">Set the IO table count</param>
        /// <param name="repeatCount">Set the repeat count for how many iterations motion</param>
        /// <param name="startDwell">Set the Waiting time before driving (msec)</param>
        /// <param name="endDwell">When setting the RepeatCount, Set the waiting time before return operation (msec).</param>
        /// <param name="reverseMode">Set the reverse mode(1 : reverse mode)</param>
        /// <param name="pMotionTable">return the profile table value</param>
        /// <param name="pIOTable">return the IO table value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ProfileMove(UInt32 axisNo, UInt32 motionTableCount, UInt32 ioTableCount, UInt16 repeatCount,
        Single startDwell, Single endDwell, UInt16 reverseMode, ref MXP_PROFILE_TABLE_ARRAY_IN pMotionTable, ref MXP_IO_TABLE_ARRAY_IN pIOTable);

        /// <summary>
        ///  It checks the operating status of the profile movement.
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="status">Return the Profile move ref value(Type:PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ProfileMoveCheck(UInt32 axisNo, ref PROCESS_CHECK status);
        #endregion

        #region TouchProbe
        /// <summary>
        /// Configure the touch probe function of the servo drive. 
        /// The touch probe function reads the position value of the servo drive when the specified sensor sends an input signal 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="mode">Select a trigger mode(Use MXP_TRIGEER_MODE)</param>
        /// <param name="triggerType">Select a trigger type(Use MXP_TRIGGER_TYPE)</param>
        /// <param name="triggerEdge">Select a trigger edge(Use MXP_TRIGGER_EDGE)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe1Set(UInt32 axisNo, MXP_TRIGGER_MODE mode, MXP_TRIGGER_TYPE triggerType, MXP_TRIGGER_EDGE triggerEdge);

        /// <summary>
        /// Reset the touchprobe1.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe1SingleModeReSet(UInt32 axisNo);

        /// <summary>
        /// Configure the touch probe function of the servo drive. 
        /// The touch probe function reads the position value of the servo drive when the specified sensor sends an input signal 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="mode">Select a trigger mode(Use MXP_TRIGEER_MODE)</param>
        /// <param name="triggerType">Selcet a trigger type(Use MXP_TRIGGER_TYPE)</param>
        /// <param name="triggerEdge">Select a trigger edge(Use MXP_TRIGGER_EDGE)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe2Set(UInt32 axisNo, MXP_TRIGGER_MODE mode, MXP_TRIGGER_TYPE triggerType, MXP_TRIGGER_EDGE triggerEdge);

        /// <summary>
        /// Reset the touchprobe2
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe2SingleModeReSet(UInt32 axisNo);

        /// <summary>
        /// Read the current status of the touch probe
        /// The result of calling of the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="state">Returns the state of the touch probe(Type : strTouchprobeState)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe1ReadState(UInt32 axisNo, ref TOUCHPROBE_STATE state);

        /// <summary>
        /// Read the current status of the touch probe
        /// The result of calling of the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="state">Returns the state of the touch probe(Type : strTouchprobeState)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe2ReadState(UInt32 axisNo, ref TOUCHPROBE_STATE state);

        /// <summary>
        /// Read the actual position obtained by the touch probe of the servo drive
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="triggerEdge">Select an edge (Use MXP_TRIGGER_EDGE_ENUM)</param>
        /// <param name="position">Return the actual position of the touch probe(Use TouchProbeReadPos_Reply)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe1ReadPosition(UInt32 axisNo, MXP_TRIGGER_EDGE triggerEdge, ref TOUCHPROBE_READPOS_REPLY position);

        /// <summary>
        /// Read the actual position obtained by the touch probe of the servo drive
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the axis on which to use the touch probe(0~127)</param>
        /// <param name="triggerEdge">Select an edge (Use MXP_TRIGGER_EDGE_ENUM)</param>
        /// <param name="position">Return the actual position of the touch probe (Use TouchProbeReadPos_Reply)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TouchProbe2ReadPosition(UInt32 axisNo, MXP_TRIGGER_EDGE triggerEdge, ref TOUCHPROBE_READPOS_REPLY position);
        #endregion

        #region DirectSet
        /// <summary>
        /// Move the axis at the torque specified in the torque parameter
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127).</param>
        /// <param name="torque">Set the torque to be commanded</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveDirectTorque(UInt32 axisNo, Single torque);

        /// <summary>
        /// Move the axis at the velocity specified in the Velocity parameter
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="velocity">Set the movement velocity</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveDirectVelocity(UInt32 axisNo, Single velocity);

        /// <summary>
        /// Move the axis at the position specified in the position parameter
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="position">Set the target position</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveDirectPosition(UInt32 axisNo, Single position);
        #endregion

        #region Read Data
        /// <summary>
        /// Return the actual position of the specified axis
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="position">Return the position value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadActualPosition(UInt32 axisNo, ref Single position);

        /// <summary>
        /// Return the actual position of the specified axis
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="velocity">Returns the velocity (position/time) value
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadActualVelocity(UInt32 axisNo, ref Single velocity);

        /// <summary>
        /// Returns the Following Error values of the axes commanded
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to read the Following Error value(0~127).</param>
        /// <param name="followingErrorValue">Return the Following Error value.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadFollowingError(UInt32 axisNo, ref Single followingErrorValue);

        /// <summary>
        /// Return the actual torque of the specified axis
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127).</param>
        /// <param name="torque">Return the rated torque value of the specified axis as a percentage (%).</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadActualTorque(UInt32 axisNo, ref Single torque);

        /// <summary>
        /// This function block returns the position commanded to the servo drive in real time.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is instructed(0~127).</param>
        /// <param name="position">Return the command position.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadCommandPosition(UInt32 axisNo, ref Single position);

        /// <summary>
        /// This function block returns the velocity commanded to the servo drive in real time.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is instructed(0~127).</param>
        /// <param name="velocity">Return the command velocity.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadCommandVelocity(UInt32 axisNo, ref Single velocity);

        /// <summary>
        /// Return an Error code when the axis is in the errorStop state.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127)</param>
        /// <param name="axisError">Return the Error state(Type:AXIS_ERROR)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadError(UInt32 axisNo, ref AXIS_ERROR axisError);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadErrorHistory(UInt32 axisNo, ref UInt16 axisErrorCount, ref UInt16 driveErrorCount, ref UInt16 pArrAxisError, ref UInt16 pArrDriveError);

        /// <summary>
        /// Return the selected axis status.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127).</param>
        /// <param name="axisStatus">Return the Axis State value(Type:MXP_AXIS_STATEBIT)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadStatus(UInt32 axisNo, ref MXP_AXIS_STATEBIT axisStatus);

        /// <summary>
        /// Return the setting value of an axis parameter.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127).</param>
        /// <param name="parameterNo">Set the number of the parameter</param>
        /// <param name="value">Return the value of the specified parameter.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadParameter(UInt32 axisNo, UInt16 parameterNo, ref Single value);

        /// <summary>
        /// Set the value of the motion parameter for the specified axis.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis to which the motion command is issued(0~127).</param>
        /// <param name="parameterNo">Set the number of the parameter.</param>
        /// <param name="value">Set the new value of the specified parameter.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_WriteParameter(UInt32 axisNo, UInt16 parameterNo, Single value);

        /// <summary>
        /// Store current all parameters.
        /// If you want to use saved parameter, Use the Upload option in mxConfigurator.
        /// The result of calling the function is returned
        /// </summary>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_StoreParameter();

        /// <summary>
        /// In real time, Return the PDO data currently mapped to the specified slave device. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="direction">Set PDO direction(0 : Servo write, 1 : MXP write)</param>
        /// <param name="offset">Set the offset for PDO mapping.</param>
        /// <param name="size">Set the size of the PDO data.</param>
        /// <param name="pValue">Return the value.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadPdoData(UInt32 slaveNo, MXP_PDO_DIRECTION direction, UInt16 offset, UInt16 size, ref Byte pValue);

        /// <summary>
        /// Output the data to the PDO currently mapped to the specified slave device.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="offset">Set the offset for PDO mapping.</param>
        /// <param name="size">Set the size of the PDO data.</param>
        /// <param name="pValue">Set the PDO data value</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_WritePdoData(UInt32 slaveNo, UInt16 offset, UInt16 size, Byte[] pValue);

        /// <summary>
        /// Returns the physical output data of the drive.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="pValue">Returns the set data as an array of bytes.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadAxisIO(UInt32 axisNo, ref Byte pValue);

        /// <summary>
        /// Sets the physical output data for the drive.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="pValue">Sets the byte array data.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_WriteAxisIO(UInt32 axisNo, Byte[] pValue);

        /// <summary>
        /// Read parameters from a slave device through EtherCAT communication.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="index">Set the index of the parameter.</param>
        /// <param name="subIndex">Set the subIndex value.</param>
        /// <param name="size">Set the buffer size.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeReadRequest(UInt32 slaveNo, UInt16 index, UInt16 subIndex, UInt16 size);

        /// <summary>
        /// Return the result about calling the ECAT_CoeReadRequest function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="status">Output the data contained in the specified index of the parameter(Type:READ_ETParameterReply) </param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeReadReply(UInt32 slaveNo, ref READ_ECAT_PARAMETER_REPLY status);

        /// <summary>
        /// Write parameters to a slave device through the EtherCAT communication network.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="index">Set the index of the parameter.</param>
        /// <param name="subIndex">Set the subIndex value.</param>
        /// <param name="size">Set the buffer size.</param>
        /// <param name="writeData">Set the data in the parameter.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeWriteRequest(UInt32 slaveNo, UInt16 index, UInt16 subIndex, UInt16 size, UInt32 writeData);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeWriteRequest_Ex(UInt32 slaveNo, UInt16 index, UInt16 subIndex, UInt16 size, Byte[] pWriteData);


        /// <summary>
        /// Return the result about calling the ECAT_CoeWriteRequest function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="status">Return the status value(Type:PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeWriteReply(UInt16 slaveNo, ref PROCESS_CHECK status);

        /// <summary>
        /// Execute the ECAT_CoeWriteRequest and ECAT_CoeWriteReply Command
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="index">Set the index of the parameter.</param>
        /// <param name="subIndex">Set the subIndex value.</param>
        /// <param name="size">Set the buffer size.</param>
        /// <param name="writeData">Set the data in the parameter.</param>
        /// <param name="waitTime">Set the wait time(ms) before ECAT_CoeWriteReply Command</param>
        /// <param name="status">Return the status value(Type:PROCESS_CHECK)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeWrite(UInt32 slaveNo, UInt16 index, UInt16 subIndex, UInt16 size, UInt32 writeData,
        Int32 waitTime, ref PROCESS_CHECK status);

        /// <summary>
        /// Execute the ECAT_CoeReadRequest and ECAT_CoeReadReply Command
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="slaveNo">Set the number of the slave(0~127)</param>
        /// <param name="index">Set the index of the parameter.</param>
        /// <param name="subIndex">Set the subIndex value.</param>
        /// <param name="size">Set the buffer size.</param>
        /// <param name="waitTime">Set the wait time(ms) before ECAT_CoeReadReply Command</param>
        /// <param name="status">Output the data contained in the specified index of the parameter(Type:READ_ETParameterReply)</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_CoeRead(UInt32 slaveNo, UInt16 index, UInt16 subIndex, UInt16 size, Int32 waitTime, ref READ_ECAT_PARAMETER_REPLY status);

        /// <summary>
        /// Return to Change MXP Unit to Time value
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="targetVel">Set the Velocity value</param>
        /// <param name="accDec">Set the Acc/Dec value</param>
        /// <param name="jerk">Set the Jerk value</param>
        /// <param name="data">Return the Time Value after calcurating</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AccDecToAccTime(UInt32 axisNo, Single targetVel, Single accDec, Single jerk, ref ACCDEC_TO_ACCTIME_REPLY data);

        /// <summary>
        /// Return to Change Time value to MXP Unit
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="targetVel">Set the Velocity value</param>
        /// <param name="accDecBuildUp">Set the AccDecBuildUp value</param>
        /// <param name="limitAccDec">Set the LimitAccDec value</param>
        /// <param name="accDecRampDown">Set the AccDecRampDown value</param>
        /// <param name="data">Return the MXP Unit Value after calcurating</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AccTimeToAccDec(UInt32 axisNo, Single targetVel, Single accDecBuildUp, Single limitAccDec, Single accDecRampDown,
        ref ACCTIME_TO_ACCDEC_REPLY data);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AccTimeToAccDecByScurveRate(UInt32 axisNo, Single targetVel, Single accTime, Single sCurveRate, ref ACCTIME_TO_ACCDEC_REPLY data);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_DistanceToAccDec(UInt32 axisNo, Single targetVel, Single distance, Single sCurveRate, ref ACCTIME_TO_ACCDEC_REPLY data);


        /// <summary>
        /// Check the Axis ready status
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="ready">Return the value which check the axis status</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadyCheck(UInt32 axisNo, ref Byte ready);

        /// <summary>
        /// Check the Axis move status
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Set the number of the axis(0~127)</param>
        /// <param name="targetPos">Set the target position which wrote when you call the Axis move motion function </param>
        /// <param name="inpositionCheckRagne">Set the inposition range</param>
        /// <param name="moveState">Return the move status</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveStateCheck(UInt32 axisNo, Single targetPos, Single inpositionCheckRagne, ref MXP_MOVESTATE moveState);
        #endregion

        #region Group
        /// <summary>
        /// Command a controlled motion stop on the motor axes of the specified axis group.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="execute ">Execute the command with rising edge.</param>
        /// <param name="dec">Set the deceleration.</param>
        /// <param name="jerk">Set the jerk value.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Stop(UInt32 axesGroup, Byte execute, Single dec, Single jerk);

        /// <summary>
        /// Command a linear interpolation motion from the actual position to a specified absolute position. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="position">Set the position value of the path</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path.</param>
        /// <param name="jerk">Set the jerk of the path.</param>
        /// <param name="bufferMode">Set the buffer mode.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveLinearAbsolute(UInt32 axesGroup, GROUP_POS position, Single velocity, Single acceleration,
        Single deceleration, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GroupMoveAbsolute(UInt32 axesGroup, Single[] pPosition, Single velocity, Single acceleration,
        Single deceleration, Single jerk);

        /// <summary>
        /// Command a linear interpolation from the actual position by the specified distance.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="distance">Set the distance value of the path</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path.</param>
        /// <param name="jerk">Set the jerk of the path.</param>
        /// <param name="bufferMode">Set the buffer mode.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveLinearRelative(UInt32 axesGroup, GROUP_POS distance, Single velocity, Single acceleration,
        Single deceleration, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GroupMoveRelative(UInt32 axesGroup, Single[] pDistance, Single velocity, Single acceleration,
        Single deceleration, Single jerk);
        /// <summary>
        /// Command a circular interpolation based on the actual position and the absolute coordinate system.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="pathChoice">Set the direction of the circular path (0: clockwise, 1: counterclockwise).</param>
        /// <param name="auxPoint1">Set the center point of the circle on Plane1.</param>
        /// <param name="auxPoint2">Set the center point of the circle on Plane2.</param>
        /// <param name="endPoint1">Set the end point of the circle on Plane1.</param>
        /// <param name="endPoint2">Set the end point of the circle on Plane2.</param>
        /// <param name="plane1">Specify a plane to be Plane1 (X-C: 0~8)</param>
        /// <param name="plane2">Specify a plane to be Plane2 (X-C: 0~8)</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path.</param>
        /// <param name="jerk">Set the jerk of the path.</param>
        /// <param name="bufferMode">Set the buffer mode.</param>
        /// <param name="circleMode">Set the circle mode.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularAbsolute_Ex(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);

        /// <summary>
        /// Command a circular Int32erpolation based on the actual position and the absolute coordinate system.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31)</param>
        /// <param name="pathChoice">Set the direction of the circular path (0: clockwise, 1: counterclockwise).</param>
        /// <param name="auxPoint1">Set the center poInt32 of the circle on Plane1</param>
        /// <param name="auxPoint2">Set the center poInt32 of the circle on Plane2</param>
        /// <param name="endPoint1">Set the end poInt32 of the circle on Plane1</param>
        /// <param name="endPoint2">Set the end poInt32 of the circle on Plane2</param>
        /// <param name="plane1">Specify a plane to be Plane1 (X-C: 0~8)</param>
        /// <param name="plane2">Specify a plane to be Plane2 (X-C: 0~8)</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path</param>
        /// <param name="jerk">Set the jerk of the path</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularBorderMoveAbsolute(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk);

        /// <summary>
        /// Command a circular Int32erpolation based on the actual position and the absolute coordinate system.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31)</param>
        /// <param name="pathChoice">Set the direction of the circular path (0: clockwise, 1: counterclockwise).</param>
        /// <param name="auxPoint1">Set the center poInt32 of the circle on Plane1</param>
        /// <param name="auxPoint2">Set the center poInt32 of the circle on Plane2</param>
        /// <param name="endPoint1">Set the end poInt32 of the circle on Plane1</param>
        /// <param name="endPoint2">Set the end poInt32 of the circle on Plane2</param>
        /// <param name="plane1">Specify a plane to be Plane1 (X-C: 0~8)</param>
        /// <param name="plane2">Specify a plane to be Plane2 (X-C: 0~8)</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path</param>
        /// <param name="jerk">Set the jerk of the path</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularBorderMoveRelative(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk);

        /// <summary>
        /// Command a circular interpolation based on the actual position and the relative coordinate system.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="pathChoice">Set the direction of the circular path (0: clockwise, 1: counterclockwise).</param>
        /// <param name="auxPoint1">Set the center point of the circle on Plane1 relative to the starting point.</param>
        /// <param name="auxPoint2">Set the center point of the circle on Plane2 relative to the starting point.</param>
        /// <param name="endPoint1">Set the end point of the circle on Plane1 relative to the starting point.</param>
        /// <param name="endPoint2">Set the end point of the circle on Plane2 relative to the starting point.</param>
        /// <param name="plane1">Specify a plane to be Plane1 (X-C: 0~8)</param>
        /// <param name="plane2">Specify a plane to be Plane2 (X-C: 0~8)</param>
        /// <param name="velocity">Set the velocity of the path.</param>
        /// <param name="acceleration">Set the acceleration of the path.</param>
        /// <param name="deceleration">Set the deceleration of the path.</param>
        /// <param name="jerk">Set the jerk of the path.</param>
        /// <param name="bufferMode">Set the buffer mode.</param>
        /// <param name="circleMode">Set the circle mode.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularRelative_Ex(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularAngle(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single centerX, Single centerY, Single angle, MXP_PLANE plane1, MXP_PLANE plane2, Single velocity, Single acceleration, Single deceleration, Single jerk, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveHelicalAbsolute(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF syncAxisGroupAxisID, Single moveDistance,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveHelicalRelative(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF syncAxisGroupAxisID, Single moveDistance,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveHelicalAngle(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice,
        Single centerX, Single centerY, Single angle, MXP_PLANE plane1, MXP_PLANE plane2, Single velocity, Single acceleration, Single deceleration, Single jerk,
        IDENT_IN_GROUP_REF syncAxisGroupAxisID, Single moveDistance, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        /// <summary>
        /// Set the velocity rate for all function in group axis
        /// The factor can be set as a real number from 0 to 1(0 ~ 100%) 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="velFactor">Set the velocity scale factor as a multiplier of a real number</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_SetOverride(UInt32 axesGroup, Single velFactor);

        /// <summary>
        /// Return the actual velocity of the path in real time.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="pathVelocity">Return the actual velocity of the path.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadActualVelocity(UInt32 axesGroup, ref Single pathVelocity);

        /// <summary>
        /// Return the actual position of each coordinate in real time. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="position">Return the position</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadActualPosition(UInt32 axesGroup, ref GROUP_POS position);

        /// <summary>
        ///  Return the command velocity of the path in real time.
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31). </param>
        /// <param name="pathVelocity">Return the command velocity of the path. </param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadCommandVelocity(UInt32 axesGroup, ref Single pathVelocity);

        /// <summary>
        /// Return the actual position of each coordinate in real time. 
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31). </param>
        /// <param name="position">Return the position</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadCommandPosition(UInt32 axesGroup, ref GROUP_POS position);

        /// <summary>
        /// Return the axis number from the entered group number.
        ///  The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the target group(0~31).</param>
        /// <param name="pAxisNo">Return the axis numbers in group</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_GetGroupAxis(UInt32 axesGroup, ref Int32 pAxisNo);

        /// <summary>
        /// Check the Axis ready status
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Set the number of the Group(0~31)</param>
        /// <param name="ready">Return the value which check the axis status</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadyCheck(UInt32 axesGroup, ref Byte ready);

        /// <summary>
        /// Returns the status of the selected group.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Sets the number of the Group(0~31)</param>
        /// <param name="axisStatus">Returns the status of the group.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadStatus(UInt32 axesGroup, ref MXP_AXIS_STATEBIT axisStatus);
        #endregion

        #region PLC INTERFACE
        /// <summary>
        /// Read Bit data of PLC from entered address and bit number. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="bitNo">Set the bit number</param>
        /// <param name="data">Return the bit data of Byte format</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadBit(UInt32 address, Byte bitNo, ref Byte data);

        /// <summary>
        /// Read Byte data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Byte data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadByte(UInt32 address, ref Byte data);

        /// <summary>
        /// Read SByte data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the SByte data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadSByte(UInt32 address, ref SByte data);

        /// <summary>
        /// Read UInt16 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the UInt16 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadUInt16(UInt32 address, ref UInt16 data);

        /// <summary>
        /// Read Int16 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Int16 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadInt16(UInt32 address, ref Int16 data);

        /// <summary>
        /// Read UInt32 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the UInt32 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadUInt32(UInt32 address, ref UInt32 data);

        /// <summary>
        /// Read Int32 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Int32 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadInt32(UInt32 address, ref Int32 data);

        /// <summary>
        /// Read Single data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Single data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadFloat(UInt32 address, ref Single data);

        /// <summary>
        /// Read UInt64 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the UInt64 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadUInt64(UInt32 address, ref UInt64 data);

        /// <summary>
        /// Read Int64 data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Int64 data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadInt64(UInt32 address, ref Int64 data);

        /// <summary>
        /// Read Double data of PLC from entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Return the Double data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadDouble(UInt32 address, ref Double data);

        /// <summary>
        /// Read Byte Buffer data to entered address. 
        /// Address must be over 5000.
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="size">Byte Buffer Size</param>
        /// <param name="pData">Read Byte Buffer Data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_ReadBuffer(UInt32 address, UInt32 size, ref Byte pData);

        /// <summary>
        /// Write Byte Buffer data to entered address. 
        /// Address must be over 5000.
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="size">Byte Buffer Size</param>
        /// <param name="pData">Write ByteBuffer Data</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteBuffer(UInt32 address, UInt32 size, Byte[] pData, Byte logSave = 1);

        /// <summary>
        /// Write Bit data to entered address and bit number. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="bitNo">Set the bit number</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteBit(UInt32 address, Byte bitNo, Byte data, Byte logSave = 1);

        /// <summary>
        /// Write Byte data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteByte(UInt32 address, Byte data, Byte logSave = 1);

        /// <summary>
        /// Write SByte data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteSByte(UInt32 address, SByte data, Byte logSave = 1);

        /// <summary>
        /// Write UInt16 data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteUInt16(UInt32 address, UInt16 data, Byte logSave = 1);

        /// <summary>
        /// Write Int16 data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteInt16(UInt32 address, Int16 data, Byte logSave = 1);

        /// <summary>
        /// Write UInt32 data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteUInt32(UInt32 address, UInt32 data, Byte logSave = 1);

        /// <summary>
        /// Write Int32 data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteInt32(UInt32 address, Int32 data, Byte logSave = 1);

        /// <summary>
        /// Write Single data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteFloat(UInt32 address, Single data, Byte logSave = 1);

        /// <summary>
        /// Write UInt64 data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteUInt64(UInt32 address, UInt64 data, Byte logSave = 1);

        /// <summary>
        /// Write Int64 data to entered address 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteInt64(UInt32 address, Int64 data, Byte logSave = 1);

        /// <summary>
        /// Write Double data to entered address. 
        /// Address must be over 5000.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="address">Set the address number(over 5000)</param>
        /// <param name="data">Set the write data to PLC</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_WriteDouble(UInt32 address, Double data, Byte logSave = 1);
        #endregion

        #region Read PLC System Value
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadBit(UInt32 address, Byte bitNo, ref Byte data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadByte(UInt32 address, ref byte data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadSByte(UInt32 address, ref sbyte data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadUInt16(UInt32 address, ref UInt16 data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadInt16(UInt32 address, ref Int16 data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadUInt32(UInt32 address, ref UInt32 data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadInt32(UInt32 address, ref Int32 data);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PLC_SystemValueReadFloat(UInt32 address, ref Single data);
        #endregion

        #region CCC
        /// <summary>
        /// Sets the entered axis to the CCC function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <param name="mode">0 : CCC always applied 
        /// 1: CCC operation only if the axis command speed is not zero.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCSet(UInt16 axisCount, UInt32[] pAxis, UInt16 mode);

        /// <summary>
        /// Turns off the CCC function on the entered axis.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCReset(UInt16 axisCount, UInt32[] pAxis);

        /// <summary>
        /// Sets the Gain on the CCC axis.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <param name="pSingleAxisGain">Set the PID Control Loop Gain for each axis.</param>
        /// <param name="pCCCGain">Set CCC Gain.</param>
        /// <param name="multiControlKffGain">Sets the reward gain between the last axis and the first and second axes.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCGainSet(UInt16 axisCount, UInt32[] pAxis,
        MXP_MULTIAXISCOUPLE_SINGLEAXISGAIN[] pSingleAxisGain, MXP_MULTIAXISCOUPLE_CCCGAIN[] pCCCGain,
        UInt16 multiControlKffGain);

        /// <summary>
        /// Returns the operational state of the AX_MultiAxisCCCSet function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <param name="status">Returns the AX_MultiAxisCCCSet function behavior state.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCSetCheck(UInt16 axisCount, UInt32[] pAxis, ref PROCESS_CHECK status);

        /// <summary>
        /// Returns the operational state of the AX_MultiAxisCCCReset function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <param name="status">Returns the AX_MultiAxisCCCReset function action state.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCResetCheck(UInt16 axisCount, UInt32[] pAxis, ref PROCESS_CHECK status);

        /// <summary>
        /// Returns the operational state of the AX_MultiAxisCCCGainSet function.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisCount">Sets the entire axis count that is Cross Coupled Control (CCC). (2 ~ 10)</param>
        /// <param name="pAxis">Sets the axis number to be CCC. (0 ~ 127) 
        /// Supports up to 10 axes.</param>
        /// <param name="status">Returns the AX_MultiAxisCCCGainSet function behavior state.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MultiAxisCCCGainSetCheck(UInt16 axisCount, UInt32[] pAxis, ref PROCESS_CHECK status);
        #endregion

        #region SpinFBInterface
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SpinRecipeMoveCheck(UInt16 axisNo, ref PROCESS_CHECK status);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SpinnerRecipeMove(UInt16 axisNo, FB_RUN_MODE runMode);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SpinRecipeWrite(SPINTABLE[] pSpinnerStep, Int32 spinStep, Single acc, Single originWaitTime, Single originVel);
        #endregion

        #region FeedbackSensorFBInterface
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_FeedBackSensorUsingCheck(UInt32 axisNo, ref FBPROCESS_CHECK status);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_FeedBackSensorLog(UInt32 axisNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_FeedBackSensorOverrideInvalidFlag(UInt32 axisNo, Byte invalidFlag);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_FeedBackSensorDataSet(UInt32 axisNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_FeedBackSensorRun(UInt32 axisNo, FB_RUN_MODE runMode);
        #endregion

        #region SequenceMove
        /// <summary>
        /// Load and run the defined file (.txt,.json) with commands 
        /// in IO, Dwell, and Group Actions. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="filePath">Sets the path of the file which is defined the single axis sequence.</param>
        /// <param name="startStep">Sets the Start step</param>
        /// <param name="endStep">Sets the End step</param>
        /// <param name="errorMessage">Returns the relevant information if the file type is invalid.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_SequenceMove_File([MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath, UInt16 startStep, UInt16 endStep, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder errorMessage);

        /// <summary>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_SequenceMove_FileLoad([MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder errorMessage);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_SequenceMove_FileRun(UInt32 axesGroup, UInt16 startStep, UInt16 endStep);
        /// Returns the GRP_SequenceMove_File action state.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axesGroup">Sets the number of the Group(0~31)</param>
        /// <param name="currentStep">Returns the Step number currently in progress.</param>
        /// <param name="currentStepName">Returns the name of the Step currently in progress</param>
        /// <param name="remainCount">Returns the remaining unused Step count.</param>
        /// <param name="status">Returns the status of the Sequence Move</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_SequenceMoveCheck(UInt32 axesGroup, ref UInt16 currentStep, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder currentStepName, ref UInt16 remainCount, ref SEQUENCEMOVE_PROCESS_CHECK status);

        /// <summary>
        /// Load and run files (.txt, .json) that define the sequence of actions, 
        /// such as IO, Dwell, interpolation, etc.
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="filePath">Sets the path of the file which is defined the single axis sequence.</param>
        /// <param name="startStep">Sets the Start step</param>
        /// <param name="endStep">Sets the End step</param>
        /// <param name="fileReadError">Returns the relevant information if the file type is invalid.</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceMove_File([MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath, UInt16 startStep, UInt16 endStep, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder fileReadError);

        /// <summary>
        /// Returns the AX_SequenceMove_File action state. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="nCurStep">Returns the Step number currently in progress.</param>
        /// <param name="strCurStepName">Returns the name of the Step currently in progress</param>
        /// <param name="nRemainCount">Returns the remaining unexecuted Step count.</param>
        /// <param name="status">Returns the status of the Sequence Move</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceMoveCheck(UInt32 axisNo, ref UInt16 nCurStep, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder strCurStepName, ref UInt16 nRemainCount, ref SEQUENCEMOVE_PROCESS_CHECK status);

        /// <summary>
        /// Supports multipoint transfer command function. 
        /// The result of calling the function is returned
        /// </summary>
        /// <param name="axisNo">Axis number(0 ~ 127)</param>
        /// <param name="nMoveMode">Sets Sequence Move Mode.</param>
        /// <param name="nBufferedMode">Sets the Buffer Mode.</param>
        /// <param name="pData">Set Sequence Move Data.</param>
        /// <param name="acc">Sets the acceleration value.</param>
        /// <param name="dec">Sets the deceleration value.</param>
        /// <param name="jerk">Sets Jerk value.</param>
        /// <param name="bPositionSensorUsing">Sets Using PositionSensor or not</param>
        /// <returns></returns>
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceMoveRequest(UInt32 axisNo, SEQUENCE_MOVE_MODE nMoveMode, MXP_BUFFERMODE_ENUM nBufferedMode,
        SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, Single acc, Single dec, Single jerk, Byte bPositionSensorUsing);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceMoveRequest_Ex(UInt32 axisNo, SEQUENCE_MOVE_MODE nMoveMode, MXP_BUFFERMODE_ENUM nBufferedMode,
        SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, Byte bPositionSensorUsing, Single sensorRunDistance);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceMoveRequest_Ex2(UInt32 axisNo, SEQUENCE_MOVE_MODE nMoveMode, MXP_BUFFERMODE_ENUM nBufferedMode,
        SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, ref UInt16 shortblockNo);



        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SequenceCurveMoveRequest(UInt32 axisNo, SEQUENCE_MOVE_MODE nMoveMode, MXP_BUFFERMODE_ENUM nBufferedMode,
        SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, UInt16 curveStep, Single maxJerk, ref Byte shortDistFlag);



        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetUsingPositionSensor(UInt32 axisNo, Byte bUsing, UInt32 slaveNo, UInt16 offset, UInt16 size, Single fSensorTargetValue,
        Single fSensorPositionSetRange, Single fSensorPulseToUnit, Single fTargetPosCalDist);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static void Tick();
        #endregion

        #region Single Extendded Function
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetAbsRelMode(UInt32 axisNo, UInt16 absRelMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetAbsRelMode(UInt32 axisNo, ref UInt16 pAbsRelMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetProfileMode(UInt32 axisNo, UInt16 profileMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetProfileMode(UInt32 axisNo, ref UInt16 pProfileMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalSetInpos(UInt32 axisNo, UInt16 use);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalGetInpos(UInt32 axisNo, ref UInt16 pUse);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalReadInpos(UInt32 axisNo, ref UInt16 pStatus);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalReadServoAlarm(UInt32 axisNo, ref UInt16 pStatus);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalSetServoAlarm(UInt32 axisNo, UInt16 use);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalGetServoAlarm(UInt32 axisNo, ref UInt16 pUse);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalReadStop(UInt32 axisNo, ref UInt16 pStatus);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalSetStop(UInt32 axisNo, UInt16 stopMode, UInt16 level);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalGetStop(UInt32 axisNo, ref UInt16 pStopMode, ref UInt16 pLevel);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveStartPos(UInt32 axisNo, Single pos, Single vel, Single acc, Single dec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveVel(UInt32 axisNo, Single vel, Single acc, Single dec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveStartMultiPos(UInt32 arraySize, ref UInt32 pAxisNo, ref Single pPos, ref Single pVel, ref Single pAcc, ref Single pDec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveStop(UInt32 axisNo, Single dec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveEStop(UInt32 axisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveSStop(UInt32 axisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_InfoGetAxisEx(UInt32 axisNo, ref UInt16 pModuleSubID, ref StringBuilder pModuleName, ref StringBuilder pModuleDescription);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalSetSoftLimit(UInt32 axisNo, UInt16 use, UInt16 stopMode, UInt16 selection, Single positivePos, Single negativePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalGetSoftLimit(UInt32 axisNo, ref UInt16 pUse, ref UInt16 pStopMode, ref UInt16 pSelection, ref Single pPositivePos, ref Single pNegativePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MovePos(UInt32 axisNo, Single pos, Single vel, Single acc, Single dec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_MoveMultiPos(UInt32 nArraySize, ref UInt32 axisNo, ref Single pPos, ref Single pVel, ref Single pAcc, ref Single pDec);
        #endregion

        #region ContiMove
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiWriteClear(UInt32 groupNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiSetAxisMap(UInt32 groupNo, Int32 size, UInt32[] pRealAxisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiGetAxisMap(UInt32 groupNo, ref Int32 pSize, ref UInt32 pRealAxisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiSetAbsRelMode(UInt32 groupNo, UInt16 absRelMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiGetAbsRelMode(UInt32 groupNo, ref UInt16 pAbsRelMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_LineMove(UInt32 groupNo, Single[] pPos, Single vel, Single acc, Single dec);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_CircleCenterMove(UInt32 groupNo, UInt32[] pAxisNo, Single[] pCenterPos, Single[] pEndPos, Single vel, Single acc, Single dec, Int32 cwDir);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_CirclePointMove(UInt32 groupNo, UInt32[] pAxisNo, Single[] pMidPos, Single[] pEndPos, Single vel, Single acc, Single dec, Int32 arcCircle);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiBeginNode(UInt32 groupNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiEndNode(UInt32 groupNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiStart(UInt32 groupNo, Int32 profileSet, Int32 angle);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiGetTotalNodeNum(UInt32 groupNo, ref Int32 pNodeNum);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiReadFree(UInt32 groupNo, ref UInt16 pQueueFree);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiReadIndex(UInt32 groupNo, ref Int32 pQueueIndex);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiGetNodeNum(UInt32 groupNo, ref Int32 pNodeNum);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ContiIsMotion(UInt32 groupNo, ref UInt16 pInMotion);
        #endregion

        #region EGear
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EGearSet(UInt32 masterAxisNo, Int32 size, UInt32[] pSlaveAxisNo, Single[] pGearRatio);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EGearGet(UInt32 masterAxisNo, ref Int32 pSize, ref UInt32 pSlaveAxisNo, ref Single pGearRatio);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EGearReset(UInt32 masterAxisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EGearEnable(UInt32 masterAxisNo, UInt16 enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EGearIsEnable(UInt32 masterAxisNo, ref UInt16 pEnable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_LinkSetMode(UInt32 masterAxisNo, UInt32 slaveAxisNo, Single slaveRatio);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_LinkGetMode(UInt32 masterAxisNo, ref UInt32 pSlaveAxisNo, ref Single pGearRatio);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_LinkResetMode(UInt32 masterAxisNo);
        #endregion

        #region Gantry
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantrySetEnable(UInt32 masterAxisNo, UInt32 slaveAxisNo, UInt16 slaveHomeUse, Single slaveOffset, Single slaveOffsetRange);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantryGetEnable(UInt32 masterAxisNo, ref UInt16 slaveHomeUse, ref Single pSlaveOffset, ref Single pSlaveOffsetRange, ref UInt16 pGantryOn);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantrySetErrorRange(UInt32 masterAxisNo, Single errorRange, UInt16 use);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantryGetErrorRange(UInt32 masterAxisNo, ref Single pErrorRange, ref UInt16 pUse);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantryReadErrorRangeStatus(UInt32 masterAxisNo, ref UInt16 pStatus);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GantryReadErrorRangeComparePos(UInt32 masterAxisNo, ref Single pComparePos);
        #endregion

        #region Override
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverridePos(UInt32 axisNo, Single overridePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideSetMaxVel(UInt32 axisNo, Single overrideMaxVel);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverridePosSetFunction(UInt32 axisNo, UInt16 usage, Int32 decelPosRatio, Single reserved);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverridePosGetFunction(UInt32 axisNo, ref UInt16 pUsage, ref Int32 pDecelPosRatio, ref Single pReserved);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideVel(UInt32 axisNo, Single overrideVelocity);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideVelAtPos(UInt32 axisNo, Single pos, Single vel, Single acc, Single dec, Single overridePos, Single overrideVelocity, Int32 target);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideAccelVelDecel(UInt32 axisNo, Single overrideVelocity, Single maxAccel, Single maxDecel);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideAccelVelDecelAtPos(UInt32 axisNo, Single pos, Single vel, Single acc, Single dec, Single overridePos, Single overrideVelocity, Single overrideAccel, Single overrideDecel, Int32 target);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_OverrideVelAtMultiPos(UInt32 axisNo, Single pos, Single vel, Single acc, Single dec, Int32 arraySize, Single[] pOverridePos, Single[] pOverrideVelocity, Int32 target, UInt16 overrideMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCheck(UInt32 axesGroup, ref Byte moveState);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_OverrideLinePos(UInt32 groupNo, Single[] pOverridePos); // 미지원
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_OverrideLineVel(UInt32 groupNo, Single overrideVel, Single[] pDistance); // 미지원
        #endregion

        #region ECAM
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamSet(UInt32 axisNo, UInt32 masterAxis, Int32 numEntry, Single masterStartPos, Single[] pMasterPos, Single[] pSlavePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamSetWithSource(UInt32 axisNo, UInt32 masterAxis, Int32 numEntry, Single masterStartPos, Single[] pMasterPos, Single[] pSlavePos, UInt16 source); // source : 0:cmd 1: act ref AXT_MOTION_SELECTION_DEF
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamGet(UInt32 axisNo, ref UInt32 pMasterAxis, ref Int32 pNumEntry, ref Single pMasterStartPos, ref Single pMasterPos, ref Single pSlavePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamGetWithSource(UInt32 axisNo, ref UInt32 pMasterAxis, ref Int32 pNumEntry, ref Single pMasterStartPos, ref Single pMasterPos, ref Single pSlavePos, ref UInt16 pSource);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamEnableBySlave(UInt32 axisNo, UInt16 enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamEnableByMaster(UInt32 axisNo, UInt16 enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_EcamIsSlaveEnable(UInt32 axisNo, ref UInt16 pEnable);
        #endregion

        #region TriggerFunction
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerSet_Distance(UInt32 axisNo, Single startPosition, Single endPosition, Single periodicPosition, MXP_SOURCE_ENUM positionSource, TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerSet_Time(UInt32 axisNo, Single periodicTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerSet_Position(UInt32 axisNo, UInt32 comparePositionCount, Single[] pComparePosition, MXP_SOURCE_ENUM positionSource, TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerSet_Distance(UInt32 groupNo, IDENT_IN_GROUP_REF groupAxisID, Single startPosition, Single endPosition, Single periodicDistance, MXP_SOURCE_ENUM positionSource, TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerSet_Time(UInt32 groupNo, Single periodicTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerSet_Position(UInt32 groupNo, IDENT_IN_GROUP_REF groupAxisID, UInt32 comparePositionCount, Single[] pComparePosition, MXP_SOURCE_ENUM positionSource, TRIGGER_DIRECTION_ENUM direction);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerGet_Distance(UInt32 axisNo, ref Single startPosition, ref Single endPosition, ref Single periodicPosition, ref MXP_SOURCE_ENUM positionSource, ref TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerGet_Time(UInt32 axisNo, ref Single periodicTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_TriggerGet_Position(UInt32 axisNo, ref UInt32 comparePositionCount, ref Single pComparePosition, ref MXP_SOURCE_ENUM positionSource, ref TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerGet_Distance(UInt32 groupNo, ref IDENT_IN_GROUP_REF groupAxisID, ref Single startPosition, ref Single endPosition, ref Single periodicPosition, ref MXP_SOURCE_ENUM positionSource, ref TRIGGER_DIRECTION_ENUM direction);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerGet_Time(UInt32 groupNo, ref Single periodicTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_TriggerGet_Position(UInt32 groupNo, ref IDENT_IN_GROUP_REF groupAxisID, ref UInt32 comparePositionCount, ref Single pComparePosition, ref MXP_SOURCE_ENUM positionSource, ref TRIGGER_DIRECTION_ENUM direction);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Trigger_State(UInt32 groupNo, ref TRIGGER_MODE_ENUM triggerMode, ref Byte enableState, ref UInt32 triggerCount);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Trigger_State(UInt32 axisNo, ref TRIGGER_MODE_ENUM triggerMode, ref Byte enableState, ref UInt32 triggerCount);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Trigger_Enable(UInt32 axisNo, TRIGGER_MODE_ENUM triggerMode, UInt32 ioSlaveNo, UInt32 ioOffset, UInt32 ioBitNo, TRIGGERLEVEL_ENUM triggerLevel, Single triggerOnTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Trigger_Enable(UInt32 groupNo, TRIGGER_MODE_ENUM triggerMode, UInt32 ioSlaveNo, UInt32 ioOffset, UInt32 ioBitNo, TRIGGERLEVEL_ENUM triggerLevel, Single triggerOnTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Trigger_Disable(UInt32 axisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Trigger_Disable(UInt32 groupNo);
        #endregion

        #region ExtendFunction
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadServoAlarmRequest(UInt32 axisNo, UInt16 index, UInt16 subindex, UInt16 size);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadServoAlarmReply(UInt32 axisNo, ref READ_ECAT_PARAMETER_REPLY status, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder alarm);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadServoAlarm(UInt32 axisNo, UInt16 index, UInt16 subindex, UInt16 size, Int32 waitTime, ref READ_ECAT_PARAMETER_REPLY status, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder alarm);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AllStop(Single dec, Single jerk);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AllEMGStop();
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AllPowerOn(Byte enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadInMotion(UInt32 axisNo, ref Byte movingStatus); // MovingStatus : 1: Moving 
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetUsableBufferCount(UInt32 axisNo, ref Int32 usableCount);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_GetUsableBufferCount(UInt32 axesGroup, ref Int32 usableCount);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SignalCapture(UInt32 axisNo, MXP_TOUCHPROB_NUMBER probNo, MXP_TRIGGER_TYPE triggerType, MXP_TRIGGER_EDGE triggerEdge);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetSignalCapture(UInt32 axisNo, ref PROCESS_CHECK checkState, ref TOUCHPROBE_READPOS_REPLY capturePos);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_AddAxisToGroup(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, UInt32 axisNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_WriteConfiguration(UInt32 axesGroup, UInt32 axisCount, UInt32[] pAxis);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GroupDestroy(UInt32 axesGroup);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_GroupDestroy(UInt32 axesGroup);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GroupCreate(UInt32 axesGroup, UInt32 axisCount, UInt32[] pAxis);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_GroupCreate(UInt32 axesGroup, UInt32 axisCount, UInt32[] pAxis);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GroupDestroyAll(UInt32 errorCount, UInt32[] pErrInfo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_GroupDestroyAll(UInt32 errorCount, UInt32[] pErrInfo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_AddSyncAxesToGroup(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, UInt32 axisCount, UInt32[] pAxis, MXP_SLAVE_DESYNC_MODE desyncMode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_RemoveAxisFromGroup(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_UngroupAllAxes(UInt32 axesGroup);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadConfiguration(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, ref Int32 axisNo);  // -1 인 경우 미할당
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadAllConfiguration(UInt32 axesGroup, ref UInt32 axisCount, Int32[] pAxis);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadSyncAxesConfig(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, ref UInt32 axisCount, Int32[] pAxis);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Enable(UInt32 axesGroup);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Disable(UInt32 axesGroup);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadEnableState(UInt32 axesGroup, ref Byte enableState);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularKinetic(UInt32 axesGroup, UInt32 axisCount, UInt32 pAxis, MXP_PATHCHOICE_ENUM pathChoice, Single x1, Single y1, Single x2, Single y2, Single velocity, Single acceleration, Single deceleration, Single jerk, Single PosDataArr,
        Int16 Cnt, Single pPos, Single pVel, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID, UInt32 Type = 1, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveLinearAbsolute_VelocityOverride(UInt32 axesGroup, GROUP_POS position, Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID,
        Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveLinearRelative_VelocityOverride(UInt32 axesGroup, GROUP_POS distance, Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID,
        Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity, MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularAbsolute_VelocityOverride(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice, Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID, Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveCircularRelative_VelocityOverride(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice, Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID, Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveHelicalAbsolute_VelocityOverride(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice, Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF syncAxisGroupAxisID, Single moveDistance, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID, Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_MoveHelicalRelative_VelocityOverride(UInt32 axesGroup, MXP_PATHCHOICE_ENUM pathChoice, Single auxPoint1, Single auxPoint2, Single endPoint1, Single endPoint2, MXP_PLANE plane1, MXP_PLANE plane2,
        Single velocity, Single acceleration, Single deceleration, Single jerk, IDENT_IN_GROUP_REF syncAxisGroupAxisID, Single moveDistance, IDENT_IN_GROUP_REF overrideReferenceGroupAxisID, Int16 overrideCount, Single[] pOverridePosition, Single[] pOverrideVelocity,
        MXP_BUFFERMODE_ENUM bufferMode = MXP_BUFFERMODE_ENUM.MXP_ABORTING, MXP_CIRCLEMODE_ENUM circleMode = MXP_CIRCLEMODE_ENUM.MXP_CENTER);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_SetEmergencyIO(Byte enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetEmergencyIO(ref Byte enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetEmergencyIO(UInt32 axisNo, EMOIO_MODE_ENUM mode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetEmergencyIO(UInt32 axisNo, ref EMOIO_MODE_ENUM mode);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadRegisterRequest(UInt32 slaveNo, UInt32 index, UInt32 address, UInt32 command, UInt32 offset, UInt32 size);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadRegisterReply(UInt32 slaveNo, ref READ_ECAT_PARAMETER_REPLY status);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_WriteRegisterRequest(UInt32 slaveNo, UInt32 index, UInt32 command, UInt32 address, UInt32 offset, UInt32 size, Byte[] pValue);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_WriteRegisterReply(UInt32 slaveNo, ref PROCESS_CHECK status);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_2DErrorCompensation(UInt32 axisX, UInt32 axisY, UInt32 tableID, Byte enable);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_2DErrorCompensationState(UInt32 tableID, ref Byte status, ref UInt32 axisX, ref UInt32 axisY);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Read2DErrorCompensationValueRequest(UInt32 tableID, Single refPosition_X, Single refPosition_Y);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Read2DErrorCompensationValueReply(UInt32 tableID, ref READ_2DError_REPLY status);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadRegisterRequestCancel_Ex(UInt32 slaveNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadRegisterRequest_Ex(UInt32 slaveNo, ref ET_REGISTER_ENUM command);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_ReadRegisterReply_Ex(UInt32 slaveNo, ref ET_REGISTER_ENUM command);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_WriteTorqueFollowingRate(UInt32 axisNo, Single rate, Single conversionTime);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_CamTableSelect_Ex(UInt32 masterAxis, UInt32 slaveAxis, UInt16 camTableID, Byte periodic, UInt16 camIndexNo);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadCAMIndex(UInt32 axisNo, ref UInt16 camIndex);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadVelocityProfile(UInt32 axisNo, Single distance, Single startVel, Single targetVel, Single endVel, Single acc, Single dec, Single jerk
        , ref byte shortDistFlag, ref Single stepFeed, ref Single stepPos, ref Single stepTime);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetMoveDistance(UInt32 axisNo, Single startVel, Single targetVel, Single endVel, Single acc, Single dec, Single jerk, ref Single moveDistance);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadSequenceMoveStepAccDecDist(UInt32 axisNo, SEQUENCE_MOVE_MODE nMoveMode, MXP_BUFFERMODE_ENUM nBufferedMode,
            SEQUENCEMOVE_STEP[] pData, UInt16 stepCount, ref Single stepAccDecDist);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_ThreadTickCount(ref Int32 tickCount);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetExternalEncoder(UInt32 axisNo, Byte bUsing, UInt32 slaveNo, UInt16 offset, UInt16 size, Single fSensorPulseToUnit,
        Single startExternalEncPosition, Single startMotorEncPosition, Single dualFeedBackUsingDistance, Single externalEncRollOver = 0, Single externalEncMaxPosition = 0, Single externalEncMinPosition = 0);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetExternalEncoderState(UInt32 axisNo, ref Int32 state);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetExternalEncoder2(UInt32 axisNo, Byte bUsing, UInt32 slaveNo, UInt16 offset, UInt16 size, Single fSensorPulseToUnit, Single targetExternalEncPosition, Single targetMotorEncPosition, Single externalEncCalDistance);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetSequencedMoveFailStopParameter(UInt32 axisNo, Single dec, Single jerk);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_SetVelocityLimit(UInt32 axisNo, Byte setUsing, Single offset);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_GetVelocityLimitUsingState(UInt32 axisNo, ref Byte setUsing, ref Single offset);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Dwell(UInt32 axisNo, Single time);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_Dwell(UInt32 axesGroup, Single time);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_AllReset();

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_Phasing_Ex(UInt32 masterAxis, UInt32 slaveAxis, Single phaseShift, Single phaseVel, Single phaseAcc);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadVelocityOverrideFactor(UInt32 axisNo, ref Single curOverrideValue);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadTrajectoryState(UInt32 axisNo, ref double targetTime, ref double currentTime, ref UInt16 currentStep, ref UInt16 trajectoryState);
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetInstallMode(ref INSTALL_MODE_ENUM installMode);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetDataPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder dataPath);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT PCIe_ReadBoardInfo(UInt16 boardID, ref UInt16 boardState, ref UInt16 boardMaxSlaveCount, ref UInt16 axisStartIndex
                , ref UInt16 slaveStartIndex, ref UInt16 axisCount, ref UInt16 slaveCount);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT SYS_GetRegKeyPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder keyPath);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadBufferInfo(UInt32 axisNo, ref UInt16 nSaveNum, ref UInt16 nReadNum, ref UInt16 nCurBlock);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT GRP_ReadBufferInfo(UInt32 axesGroup, ref UInt16 nSaveNum, ref UInt16 nReadNum, ref UInt16 nCurBlock);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadSyncInfo(UInt32 axisNo, ref ESYNC_CMD syncCmdType, ref UInt32 masterAxisNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT AX_ReadTrajectoryStateEx(UInt32 axisNo, ref double targetTime, ref double currentTime, ref double targetVelocity, ref UInt16 currentStep, ref UInt16 trajectoryState);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT ECAT_GetNodeIDFromAxisNo(UInt32 axisNo, ref UInt32 nodeID);

        #endregion


        #region MMT(Moving Magnet)
        //Tool
        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetSystemConfig(ref Byte moverBCRUsing, ref Byte hardwareCheckMode, ref Single initMoveDist, ref Single collisionDistance);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SetSystemConfig(Byte moverBCRUsing, Byte hardwareCheckMode, Single initMoveDist, Single collisionDistance);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_LoadFile([MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder errorMessage);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SaveFile([MarshalAs(UnmanagedType.LPWStr)] StringBuilder filePath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder errorMessage);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_LoadSystem();

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SetPLCInterfaceConfig(UInt32 reqAddress, UInt32 replyAddress, UInt32 errorAddress);

        //user


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_StopSystem();

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_Load();

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_Store();

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_LoadStoreReply(ref Int32 result, ref Int32 errorID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetAxisInfo(ref UInt16 moverCount, ref UInt16 realAxisCount, ref UInt16 moverStartAxisNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetSystemStatus(ref UInt16 systemState);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetSystemInfo(ref UInt16 systemState, ref UInt16 moverMaxCount, ref UInt16 moverCurrentCount, ref UInt16 trackCount);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_InitSystem(MXP_MMT_INITMODE initMode);



        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetMoverIDToAxisID(UInt32 moverID, ref UInt32 axisID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetAxisIDToMoverID(UInt32 axisID, ref UInt32 moverID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetMoverInfo(UInt32 moverID, ref MXP_MOVER_INFO moverInfo); // Mover 전체 정보 취합

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SetMoverPlane(UInt32 moverID, UInt16 plane, UInt16 slotno, UInt16 angleNo);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SetTrackPlane(UInt32 trackID, UInt16 plane, UInt16 slotno, UInt16 angleNo);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetMoverLimitRange(UInt32 moverID, ref Single negativeSWLimit, ref Single positiveSWLimit);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_AddMover(UInt32 moverID, UInt16 trackID, Int16 leftCoilNo, Int16 middleCoilNo, Int16 rightCoilNo, UInt16 plane, UInt16 slotNo, UInt16 angleNo, UInt16 virtualAxisNo = 0);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_RemoveMover(UInt32 moverID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_InitMover(UInt32 moverID, MXP_MMT_INITMODE initMode);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_AutoSeaching(ref MXP_MMT_MOVERINFO_SEARCHED pMoverInfo, Byte compareSearchingUsing = 0);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetRecoveryInfo(UInt32 moverID, Single searchingDistance, ref UInt16 trackID, ref Int16 leftCoilNo, ref Int16 middleCoilNo, ref Int16 rightCoilNo, ref UInt16 plane, ref UInt16 slotNo, ref UInt16 angleNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_RecoveryRequest(UInt32 moverID, UInt16 trackID, Int16 leftCoilNo, Int16 middleCoilNo, Int16 rightCoilNo, UInt16 plane, UInt16 slotNo, UInt16 angleNo);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_RecoveryReplay(UInt32 moverID, ref Int32 result);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ChangeMoverID(UInt32 moverID, UInt32 newMoverID);


        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_GetModuleState(UInt32 moduleNo, ref Byte sensor_L, ref Byte sensor_R, ref Byte vaild, ref Byte home);


        //단축 이송함수  Wrapping

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_Power(UInt32 moverID, Byte enable);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_Reset(UInt32 moverID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadyCheck(UInt32 moverID, ref Byte ready);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_MoveRelative(UInt32 moverID, UInt16 plane, Single distance, Single vel, Single acc, Single dec, Single jerk
            , MXP_DIRECTION_ENUM direction = MXP.MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP_BUFFERMODE_ENUM bufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_MoveAbsolute(UInt32 moverID, UInt16 plane, Single position, Single vel, Single acc, Single dec, Single jerk,
            MXP_DIRECTION_ENUM direction = MXP.MXP_DIRECTION_ENUM.MXP_NONE_DIRECTION, MXP_BUFFERMODE_ENUM bufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_MoveVelocity(UInt32 moverID, Single vel, Single acc, Single dec, Single jerk);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_Halt(UInt32 moverID, Single dec = 0, Single jerk = 0);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_SetOverrideEx(UInt32 moverID, Single velFactor, Single time);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadStatus(UInt32 moverID, ref MXP_AXIS_STATEBIT moverStatus);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadActualPosition(UInt32 moverID, ref Single position);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadCommandPosition(UInt32 moverID, ref Single position);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadFollowingError(UInt32 moverID, ref Single followingErrorValue);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadCommandVelocity(UInt32 moverID, ref Single velocity);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadActualVelocity(UInt32 moverID, ref Single velocity);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadError(UInt32 moverID, ref AXIS_ERROR axisError, ref Int16 moverAlarmCode);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadInMotion(UInt32 moverID, ref Byte MovingStatus); // MovingStatus : 1: Moving 

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadParameter(UInt32 moverID, UInt16 parameterNo, ref Single value);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_WriteParameter(UInt32 moverID, UInt16 parameterNo, Single value);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_WriteGroupConfiguration(UInt32 axesGroup, UInt32 moverCount, UInt32[] pMoverID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_WriteSyncMoverToGroup(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, UInt32 moverCount, UInt32[] pMoverID, MXP_SLAVE_DESYNC_MODE desyncMode);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadGroupConfiguraion(UInt32 axesGroup, ref UInt32 moverCount, ref Int32[] pMoverID);

        [DllImport("MXP_ExAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static MXP_FUNCTION_STATUS_RESULT MMT_ReadSyncMoverFromGroup(UInt32 axesGroup, IDENT_IN_GROUP_REF groupAxisID, ref UInt32 moverCount, Int32[] pMoverID);



        #endregion
    }
}