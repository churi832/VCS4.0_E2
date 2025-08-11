using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.Ajin
{
    public class AxtAxis
    {
        #region Field
        
        private AxisBlockAXT m_AxisBlock = null;
        private _Axis m_Axis = null;
        
        protected AXT_MODULE m_ModuleType = AXT_MODULE.AXT_UNKNOWN;
        private int m_AxisID = 0;
        private uint m_ModuleVersion = 0;

        private double m_AxisAcc = 100;
        private double m_AxisDec = 100;

        private uint m_AxisStatus = 0;
        private double m_AxisCmdVel = 0;
        private double m_AxisCmdPos = 0;
        private double m_AxisActVel = 0;
        private double m_AxisActPos = 0;
        private double m_AxisTorque = 0;

        private bool[] m_DIUniversal = new bool[ Enum.GetValues( typeof( AXT_MOTION_UNIV_INPUT ) ).Length ];
        private bool[] m_DOUniversal = new bool[ Enum.GetValues( typeof( AXT_MOTION_UNIV_OUTPUT ) ).Length ];

        private bool m_CmdTrigger = false;
        private bool m_InMotion = false;

        private enAxisOutFlag m_AxisCmdFlag = enAxisOutFlag.CommandNone;

        private bool m_HomeEnd = false;
        private bool m_Busy = false;

        private bool m_MoveHome = false;
        private bool m_MotionStop = false;
        private double m_HomeOffset = 1000;

        private uint m_StartTicks = 0;
        private int m_SeqHomeNo = 0;
        private int m_SeqJogNo = 0;
        private int m_SeqMoveNo = 0;
        #endregion

        #region Property
        public AxisBlockAXT AxisBlock
        {
            get { return m_AxisBlock; }
            set { m_AxisBlock = value; }
        }
        public _Axis Axis
        {
            get { return m_Axis; }
            set { m_Axis = value; }
        }
        
        public AXT_MODULE ModuleType
        {
            get { return m_ModuleType; }
            set { m_ModuleType = value; }
        }
        public int AxisId
        {
            get { return m_AxisID; }
            set { m_AxisID = value; }
        }
        public uint ModuleVersion
        {
            get { return m_ModuleVersion; }
            set { m_ModuleVersion = value; }
        }

        [DisplayName ("AXT Axis Module 가속도 설정")]
        public double AxisAcc
        {
            get { return m_AxisAcc; }
            set
            {
                double temp = value;
                m_AxisAcc = Math.Abs( temp );
            }
        }
        [DisplayName ("AXT Axis Module 감속도 설정")]
        public double AxisDec
        {
            get { return m_AxisDec; }
            set
            {
                double temp = value;
                m_AxisDec = Math.Abs( temp );
            }
        }
        [XmlIgnore()]
        public uint AxisStatus { get { return m_AxisStatus; } }
        [XmlIgnore()]
        public double AxisCmdVel { get { return m_AxisCmdVel; } set { m_AxisCmdVel = value; } }
        [XmlIgnore()]
        public double AxisCmdPos { get { return m_AxisCmdPos; } set { m_AxisCmdPos = value; } }
        [XmlIgnore()]
        public double AxisActVel 
        { 
            get { return m_AxisActVel; }
            set { m_AxisActVel = value; }
        }
        [XmlIgnore()]
        public double AxisActPos 
        {
            set { m_AxisActPos = value; }
            get { return m_AxisActPos; }
        }
        [XmlIgnore()]
        public double AxisTorque 
        { 
            get { return m_AxisTorque; }
            set { m_AxisTorque = value; }

        }
        [XmlIgnore()]
        public bool InMotion { get { return m_InMotion; } }
        public bool CmdTrigger
        {
            get { return m_CmdTrigger; }
            set { m_CmdTrigger = value; }
        }
        public enAxisOutFlag AxisCmdFlag
        {
            get { return m_AxisCmdFlag; }
            set { m_AxisCmdFlag = value; }
        }
        public double HomeOffset
        {
            get { return m_HomeOffset; }
            set { m_HomeOffset = value; }
        }
        public bool MotionsStop
        {
            get { return m_MotionStop; }
            set { m_MotionStop = value; }
        }
        public bool MoveHome
        {
            get { return m_MoveHome; }
            set { m_MoveHome = value; }
        }
        public bool Busy
        {
            get { return m_Busy; }
            set { m_Busy = value; }
        }
        public bool HomeEnd
        {
            get { return m_HomeEnd; }
            set { m_HomeEnd = value; }
        }
        #endregion

        #region Property (Module Status Signal)
        public bool SIG_LIMIT_P { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PELM_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PELM_LEVEL; } }
        public bool SIG_LIMIT_N { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NELM_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NELM_LEVEL; } }
        public bool SIG_SLIMIT_P { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PSLM_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PSLM_LEVEL; } }
        public bool SIG_SLIMIT_N { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NSLM_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NSLM_LEVEL; } }
        public bool SIG_ALARM { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ALARM_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ALARM_LEVEL; } }
        public bool SIG_IN_POSITION { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_INP_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_INP_LEVEL; } }
        public bool SIG_ESTOP { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ESTOP_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ESTOP_LEVEL; } }
        public bool SIG_ORIGIN { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ORG_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ORG_LEVEL; } }
        public bool SIG_ZPHASE { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ZPHASE_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ZPHASE_LEVEL; } }
        public bool SIG_TERM_EC_UP { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ECUP_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ECUP_LEVEL; } }
        public bool SIG_TERM_EC_DN { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ECDN_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ECDN_LEVEL; } }
        public bool SIG_TERM_EX_PP { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_EXPP_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_EXPP_LEVEL; } }
        public bool SIG_TERM_EX_MP { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_EXMP_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_EXMP_LEVEL; } }
        public bool SIG_TERM_SQSTR1 { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTR1_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTR1_LEVEL; } }
        public bool SIG_TERM_SQSTR2 { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTR2_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTR2_LEVEL; } }
        public bool SIG_TERM_SQSTP1 { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTP1_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTP1_LEVEL; } }
        public bool SIG_TERM_SQSTP2 { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTP2_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_SQSTP2_LEVEL; } }
        public bool SIG_TERM_MODE { get { return ( m_AxisStatus & ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_MODE_LEVEL ) == ( uint )AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_MODE_LEVEL; } }
        #endregion

        #region Property (Universal Signal Status)
        // INPUT
        public bool SW_Origin { get { return m_DIUniversal[ ( int )AXT_MOTION_UNIV_INPUT.UIO_INP0 ]; } }
        public bool SW_ZPhase { get { return m_DIUniversal[ ( int )AXT_MOTION_UNIV_INPUT.UIO_INP1 ]; } }
        // OUTPUT
        public bool IsServoOn { get { return m_DOUniversal[ ( int )AXT_MOTION_UNIV_OUTPUT.UIO_OUT0 ]; } }
        public bool IsAlarmClear { get { return m_DOUniversal[ ( int )AXT_MOTION_UNIV_OUTPUT.UIO_OUT1 ]; } }
        #endregion

        #region Constructor
        public AxtAxis()
        {

        }
        public AxtAxis(AxisBlockAXT _block, _Axis _axis)
        {
            m_AxisBlock = _block;
            m_Axis = _axis;

        }
        #endregion

        #region Method - Sequence
        private void Init()
        {
            m_SeqHomeNo = 0;
            m_SeqJogNo = 0;
            m_SeqMoveNo = 0;
        }
        private void SeqJog(enAxisOutFlag command)
        {
            int seqNo = m_SeqJogNo;
            switch(seqNo)
            {
                case 0:
                    {
                        uint rv = 0;
                        if(Convert.ToBoolean(command & enAxisOutFlag.JogPlus))
                        {
                            rv = JogMovePos();
                        }
                        else if (Convert.ToBoolean(command & enAxisOutFlag.JogMinus))
                        {
                            rv = JogMoveNeg();
                        }

                        if( rv == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS ) seqNo = 10;
                        else seqNo = 0;
                    }
                    break;
                case 10:
                    {
                        bool rv = false;
                        rv |= Convert.ToBoolean( command & enAxisOutFlag.JogPlus );
                        rv |= Convert.ToBoolean( command & enAxisOutFlag.JogMinus );
                        if(false == rv)
                        {
                            JogStop();
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqJogNo = seqNo;
        }
        private void SeqMove(enAxisOutFlag command)
        {
            int seqNo = m_SeqMoveNo;

            switch(seqNo)
            { 
                case 0:
                    {
                        uint rv = 0;
                        if(Convert.ToBoolean(command & enAxisOutFlag.MotionStart))
                        {
                            rv = Move();
                            if(rv == (uint) AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            {
                                seqNo = 10;
                            }
                            else
                            {
                                m_Busy = true;
                                m_StartTicks = XFunc.GetTickCount();
                                seqNo = 30;
                            }
                        }
                    }
                    break;
                case 10:
                    {
                        double dTarget = Math.Round( m_AxisBlock.GetTargetPosition( m_Axis.AxisId ), 6);
                        bool bMovigComp = Math.Abs( dTarget - AxisCmdPos ) < 0.0001 ? true : false;
                        if(IsMoving() || bMovigComp)
                        {
                            Busy = true;
                            m_StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;
                case 20:
                    {
                        if( IsInPos() && ( XFunc.GetTickCount() - m_StartTicks > 100 ) )
                        {
                            Busy = false;
                            seqNo = 0;
                        }
                    }
                    break;
                case 30:
                    {
                        if(XFunc.GetTickCount() - m_StartTicks > 500)
                        {
                            Busy = false;
                            seqNo = 0;
                        }
                    }
                    break;
            }
            m_SeqMoveNo = seqNo;
        }
        private int SeqHome(enAxisOutFlag command)
        {
            int seqNo = m_SeqHomeNo;
            int rv = -1;
            uint rv1 = 0;

            if(m_MotionStop)
            {
                if(MoveStop()==0)
                {
                    m_MotionStop = false;
                    rv = 1;
                    seqNo = 0;
                }
                m_SeqHomeNo = seqNo;
                return rv;
            }

            switch(m_SeqHomeNo)
            {
                case 0:
                    {
                        if( Convert.ToBoolean( command & enAxisOutFlag.HomeStart ) )
                        {
                            rv1 = CAXM.AxmSignalSetLimit( m_AxisID, 0, 2, 1 );   // 데모키트 C상 위치가 + Limit 바깥에 있음... 임시로 해제
                            m_Busy = true;
                            m_HomeEnd = false;
                            if( ( rv1 = CAXM.AxmMoveSignalSearch( m_AxisID, -1000, 1000, 1, 1, 0 ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                            {
                                seqNo = 10;
                            }
                            else if( rv1 > 0 )
                            {
                                rv = ( int )rv1;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        if(SIG_LIMIT_N)
                        {
                            seqNo = 11;
                        }
                        else if(SIG_ALARM || IsServoOn == false)
                        {
                            m_MotionStop = true;
                            rv = 1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 11:
                    {
                        bool abnormal = false;
                        abnormal |= IsServoOn == false;
                        abnormal |= SIG_ALARM;

                        uint status = 0;
                        if(CAXM.AxmStatusReadInMotion(m_AxisID, ref status) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS && status== (uint)AXT_BOOLEAN.FALSE)
                        {
                            if(abnormal)
                            {
                                m_MotionStop = true;
                                rv = 1;
                                seqNo = 0;
                            }
                            else if(rv1 > 0)
                            {
                                rv=(int)rv1;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 20:
                    {
                        bool abnormal = false;
                        abnormal |= IsServoOn == false;
                        abnormal |= SIG_ALARM;

                        uint status = 0;
                        if( CAXM.AxmStatusReadInMotion( m_AxisID, ref status ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS && status == ( uint )AXT_BOOLEAN.FALSE )
                        {
                            if( abnormal )
                            {
                                m_MotionStop = true;
                                rv = 1;
                                seqNo = 0;
                            }
                            else
                            {
                                if( ( rv1 = CAXM.AxmMoveSignalSearch( m_AxisID, -100, 100, 5, 1, 0 ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                                {
                                    m_StartTicks = XFunc.GetTickCount();
                                    seqNo = 30;
                                }
                                else if( rv1 > 0 )
                                {
                                    rv = ( int )rv1;
                                    seqNo = 0;
                                }
                            }
                        }
                        else if( abnormal )
                        {
                            m_MotionStop = true;
                            rv = 1;
                            seqNo = 0;
                        }
                    }
                    break;

                case 30:
                    {
                        bool abnormal = false;
                        abnormal |= IsServoOn == false;
                        abnormal |= SIG_ALARM;
                        
                        if(abnormal)
                        {
                            m_MotionStop = true;
                            rv = 1;
                            seqNo = 0;
                        }
                        else
                        {
                            uint status = 0;
                            if(CAXM.AxmStatusReadInMotion(m_AxisID, ref status)==(uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS && status ==(uint)AXT_BOOLEAN.FALSE)
                            {
                                double curPos = 0;

                                if(CAXM.AxmMotSetAbsRelMode(m_AxisID, 1) == (uint) AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                                {
                                    if(CAXM.AxmMoveStartPos(m_AxisID, m_HomeOffset,1000,1000,1000) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                                    {
                                        m_StartTicks = XFunc.GetTickCount();
                                        seqNo = 40;
                                    }
                                }
                                else
                                {
                                    rv = 2;
                                    seqNo = 0;
                                }
                            }
                            else if(XFunc.GetTickCount()-m_StartTicks > 10 * 1000)
                            {
                                m_MotionStop = true;
                                rv = 1;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 40:
                    {
                        bool abnormal = false;
                        //abnormal |= SIG_LIMIT_P;  // 데모키트 C상 위치가 + Limit 바깥에 있음... 임시로 해제
                        abnormal |= IsServoOn == false;
                        abnormal |= SIG_ALARM;

                        if(abnormal)
                        {
                            m_MotionStop = true;
                            rv = 1;
                            seqNo = 0;
                        }
                        else
                        {
                            uint status = 0;
                            if(CAXM.AxmStatusReadInMotion(m_AxisID, ref status) ==(uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS && status ==(uint)AXT_BOOLEAN.FALSE)
                            {
                                m_StartTicks = XFunc.GetTickCount();
                                seqNo = 50;
                            }
                            else if(abnormal || XFunc.GetTickCount()- m_StartTicks >60*1000)
                            {
                                m_MotionStop = true;
                                rv = 1;
                                seqNo = 0;
                            }
                        }
                    }
                    break;

                case 50:
                    {
                        if(XFunc.GetTickCount()-m_StartTicks > 1000)
                        {
                            if(CAXM.AxmStatusSetActPos(m_AxisID,0)== (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            {
                                m_Busy = false;
                                m_HomeEnd = true;
                                rv = 0;
                                seqNo = 0;
                            }
                            else
                            {
                                rv = 4;
                                seqNo = 0;
                            }
                        }
                    }
                    break;
            }
            m_SeqHomeNo = seqNo;
            return rv;
        }
        private uint MoveStop()
        {
            return CAXM.AxmMoveSStop( m_AxisID );
        }
        #endregion
      
        #region Method 

        public string GetModuleInfo()
        {
            string info = string.Empty;
            switch( m_ModuleType )
            {
                //case AXT_MODULE.AXT_UNKNOWN: info = "Unknown Baseboard"; break;
                case AXT_MODULE.AXT_SMC_2V01: info = "CAMC-5M, 2 Axis"; break;
                case AXT_MODULE.AXT_SMC_2V02: info = "CAMC-FS, 2 Axis"; break;
                case AXT_MODULE.AXT_SMC_1V01: info = "CAMC-5M, 1 Axis"; break;
                case AXT_MODULE.AXT_SMC_1V02: info = "CAMC-FS, 1 Axis"; break;
                case AXT_MODULE.AXT_SMC_2V03: info = "CAMC-IP, 2 Axis"; break;
                case AXT_MODULE.AXT_SMC_4V04: info = "CAMC-QI, 4 Axis"; break;
                case AXT_MODULE.AXT_SMC_R1V04A4: info = "CAMC-QI, 1 Axis, For RTEX A4 slave only"; break;
                case AXT_MODULE.AXT_SMC_1V03: info = "CAMC-IP, 1 Axis"; break;
                case AXT_MODULE.AXT_SMC_R1V04: info = "CAMC-QI, 1 Axis, For RTEX SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIISV: info = "CAMC-QI, 1 Axis, For Sigma-X series."; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIPM: info = "2 Axis, For Pulse output series(JEPMC-PL2910)."; break;
                case AXT_MODULE.AXT_SMC_2V04: info = "CAMC-QI, 2 Axis"; break;
                case AXT_MODULE.AXT_SMC_R1V04A5: info = "CAMC-QI, 1 Axis, For RTEX A5N slave only"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIICL: info = "CAMC-QI, 1 Axis, For MLII Convex Linear only"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIICR: info = "CAMC-QI, 1 Axis, For MLII Convex Rotary only"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM2Q: info = "CAMC-QI, 2 Axis, For RTEX SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM2QE: info = "CAMC-QI, 4 Axis, For RTEX SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIORI: info = "CAMC-QI, 1 Axis, For MLII Oriental Step Driver only"; break;
                case AXT_MODULE.AXT_SMC_R1V04A6: info = "CAMC-QI, 1 Axis, For RTEX A5N slave only"; break;
                case AXT_MODULE.AXT_SMC_R1V04SIIIHMIV: info = "CAMC-QI, 1 Axis, For SSCNETIII/H MRJ4"; break;
                case AXT_MODULE.AXT_SMC_R1V04SIIIHMIV_R: info = "CAMC-QI, 1 Axis, For SSCNETIII/H MRJ4"; break;
                case AXT_MODULE.AXT_SMC_R1V04SIIIHME: info = "CAMC-QI, 1 Axis, For SSCNETIII/H MRJE"; break;
                case AXT_MODULE.AXT_SMC_R1V04SIIIHME_R: info = "CAMC-QI, 1 Axis, For SSCNETIII/H MRJE"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIS7: info = "CAMC-QI, 1 Axis, For ML-II/YASKWA Sigma7(SGD7S)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIISV: info = "DSP, 1 Axis, For ML-3 SigmaV/YASKAWA only"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIPM: info = "DSP, 1 Axis, For ML-3 SLAVE/AJINEXTEK only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIISV_MD: info = "DSP, 1 Axis, For ML-3 SigmaV-MD/YASKAWA only (Multi-Axis Control amp)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIS7S: info = "DSP, 1 Axis, For ML-3 Sigma7S/YASKAWA only"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIS7W: info = "DSP, 2 Axis, For ML-3 Sigma7W/YASKAWA only(Dual-Axis control amp)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIRS2: info = "DSP, 1 Axis, For ML-3 RS2A/SANYO DENKY"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIS7_MD: info = "DSP, 1 Axis, For ML-3 Sigma7-MD/YASKAWA only (Multi-Axis Control amp)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIAZ: info = "DSP, 4 Axis, For ML-3 AZD/ORIENTAL only (Multi-Axis Control amp)"; break;
                case AXT_MODULE.AXT_SMC_R1V04MLIIIPCON: info = "DSP, 1 Axis, For ML-3 PCON/IAI only"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM2QSIIIH: info = "CAMC-QI, 2Axis For SSCNETIII/H SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM4QSIIIH: info = "CAMC-QI, 4Axis For SSCNETIII/H SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SIO_RCNT2SIIIH: info = "Counter slave module, Reversible counter, 2 channels, For SSCNETIII/H Only"; break;
                case AXT_MODULE.AXT_SIO_RDI32SIIIH: info = "DI slave module, SSCNETIII AXT, IN 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDO32SIIIH: info = "DO slave module, SSCNETIII AXT, OUT 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDB32SIIIH: info = "DB slave module, SSCNETIII AXT, IN 16-Channel, OUT 16-Channel"; break;
                case AXT_MODULE.AXT_SIO_RAI16SIIIH: info = "AI slave module, SSCNETIII AXT, Analog IN 16ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAO08SIIIH: info = "A0 slave module, SSCNETIII AXT, Analog OUT 8ch, 16 bit"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM2QSIIIH_R: info = "CAMC-QI, 2Axis For SSCNETIII/H SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SMC_R1V04PM4QSIIIH_R: info = "CAMC-QI, 4Axis For SSCNETIII/H SLAVE only(Pulse Output Module)"; break;
                case AXT_MODULE.AXT_SIO_RCNT2SIIIH_R: info = "Counter slave module, Reversible counter, 2 channels, For SSCNETIII/H Only"; break;
                case AXT_MODULE.AXT_SIO_RDI32SIIIH_R: info = "DI slave module, SSCNETIII AXT, IN 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDO32SIIIH_R: info = "DO slave module, SSCNETIII AXT, OUT 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDB32SIIIH_R: info = "DB slave module, SSCNETIII AXT, IN 16-Channel, OUT 16-Channel"; break;
                case AXT_MODULE.AXT_SIO_RAI16SIIIH_R: info = "AI slave module, SSCNETIII AXT, Analog IN 16ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAO08SIIIH_R: info = "A0 slave module, SSCNETIII AXT, Analog OUT 8ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RDI32MLIII: info = "DI slave module, MechatroLink III AXT, IN 32-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDO32MLIII: info = "DO slave module, MechatroLink III AXT, OUT 32-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDB32MLIII: info = "DB slave module, MechatroLink III AXT, IN 16-Channel, OUT 16-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDI32MSMLIII: info = "DI slave module, MechatroLink III M-SYSTEM, IN 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDO32AMSMLIII: info = "DO slave module, MechatroLink III M-SYSTEM, OUT 32-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDI32PMLIII: info = "DI slave module, MechatroLink III AXT, IN 32-Channel PNP"; break;
                case AXT_MODULE.AXT_SIO_RDO32PMLIII: info = "DO slave module, MechatroLink III AXT, OUT 32-Channel PNP"; break;
                case AXT_MODULE.AXT_SIO_RDB32PMLIII: info = "DB slave module, MechatroLink III AXT, IN 16-Channel, OUT 16-Channel PNP"; break;
                case AXT_MODULE.AXT_SIO_RDI16MLIII: info = "DI slave module, MechatroLink III M-SYSTEM, IN 16-Channel"; break;
                case AXT_MODULE.AXT_SIO_UNDEFINEMLIII: info = "IO slave module, MechatroLink III Any, Max. IN 480-Channel, Max. OUT 480-Channel"; break;
                case AXT_MODULE.AXT_SIO_RDI32MLIIISFA: info = "DI slave module, MechatroLink III AXT(SFA), IN 32-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDO32MLIIISFA: info = "DO slave module, MechatroLink III AXT(SFA), OUT 32-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDB32MLIIISFA: info = "DB slave module, MechatroLink III AXT(SFA), IN 16-Channel, OUT 16-Channel NPN"; break;
                case AXT_MODULE.AXT_SIO_RDB128MLIIIAI: info = "Intelligent I/O (Product by IAI), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII: info = "Digital IN/Out xx점, Simple I/O series, MLII 전용."; break;
                case AXT_MODULE.AXT_SIO_RDO16AMLIII: info = "DO slave module, MechatroLink III M-SYSTEM, OUT 16-Channel, NPN"; break;
                case AXT_MODULE.AXT_SIO_RDI16MLII: info = "DISCRETE INPUT MODULE, 16 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RDO16AMLII: info = "NPN TRANSISTOR OUTPUT MODULE, 16 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RDO16BMLII: info = "PNP TRANSISTOR OUTPUT MODULE, 16 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RDB96MLII: info = "Digital IN/OUT(Selectable), MAX 96 points, For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RDO32RTEX: info = "Digital OUT 32점"; break;
                case AXT_MODULE.AXT_SIO_RDI32RTEX: info = "Digital IN 32점"; break;
                case AXT_MODULE.AXT_SIO_RDB32RTEX: info = "Digital IN/OUT 32점"; break;
                case AXT_MODULE.AXT_SIO_RDO32RTEXNT1_D1: info = "Digital OUT 32점 IntekPlus 전용"; break;
                case AXT_MODULE.AXT_SIO_RDI32RTEXNT1_D1: info = "Digital IN 32점 IntekPlus 전용"; break;
                case AXT_MODULE.AXT_SIO_RDB32RTEXNT1_D1: info = "Digital IN/OUT 32점 IntekPlus 전용"; break;
                case AXT_MODULE.AXT_SIO_RDO16BMLIII: info = "DO slave module, MechatroLink III M-SYSTEM, OUT 16-Channel, PNP"; break;
                case AXT_MODULE.AXT_SIO_RDB32ML2NT1: info = "DB slave module, MechatroLink2 AJINEXTEK NT1 Series"; break;
                case AXT_MODULE.AXT_SIO_RDB128YSMLIII: info = "IO slave module, MechatroLink III Any, Max. IN 480-Channel, Max. OUT 480-Channel"; break;
                case AXT_MODULE.AXT_SIO_DI32_P: info = "Digital IN 32점, PNP type(source type)"; break;
                case AXT_MODULE.AXT_SIO_DO32T_P: info = "Digital OUT 32점, Power TR, PNT type(source type)"; break;
                case AXT_MODULE.AXT_SIO_RDB128MLII: info = "Digital IN 64점 / OUT 64점, MLII 전용(JEPMC-IO2310)"; break;
                case AXT_MODULE.AXT_SIO_RDI32: info = "Digital IN 32점, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_RDO32: info = "Digital OUT 32점, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_DI32: info = "Digital IN 32점"; break;
                case AXT_MODULE.AXT_SIO_DO32P: info = "Digital OUT 32점"; break;
                case AXT_MODULE.AXT_SIO_DB32P: info = "Digital IN 16점 / OUT 16점"; break;
                case AXT_MODULE.AXT_SIO_RDB32T: info = "Digital IN 16점 / OUT 16점, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_DO32T: info = "Digital OUT 16점, Power TR OUTPUT"; break;
                case AXT_MODULE.AXT_SIO_DB32T: info = "Digital IN 16점 / OUT 16점, Power TR OUTPUT"; break;
                case AXT_MODULE.AXT_SIO_RAI16RB: info = "A0h(160) : AI 16Ch, 16 bit, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_AI4RB: info = "A1h(161) : AI 4Ch, 12 bit"; break;
                case AXT_MODULE.AXT_SIO_AO4RB: info = "A2h(162) : AO 4Ch, 12 bit"; break;
                case AXT_MODULE.AXT_SIO_AI16H: info = "A3h(163) : AI 4Ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_AO8H: info = "A4h(164) : AO 4Ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_AI16HB: info = "A5h(165) : AI 16Ch, 16 bit (SIO-AI16HR(input module))"; break;
                case AXT_MODULE.AXT_SIO_AO2HB: info = "A6h(166) : AO 2Ch, 16 bit (SIO-AI16HR(output module))"; break;
                case AXT_MODULE.AXT_SIO_RAI8RB: info = "A7h(167) : AI 8Ch, 16 bit, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_RAO4RB: info = "A8h(168) : AO 4Ch, 16 bit, For RTEX only"; break;
                case AXT_MODULE.AXT_SIO_RAI4MLII: info = "A9h(169) : AI 4Ch, 16 bit, For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RAO2MLII: info = "AAh(170) : AO 2Ch, 16 bit, For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RAVCI4MLII: info = "DC VOLTAGE/CURRENT INPUT MODULE, 4 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RAVO2MLII: info = "DC VOLTAGE OUTPUT MODULE, 2 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RACO2MLII: info = "DC CURRENT OUTPUT MODULE, 2 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RATI4MLII: info = "THERMOCOUPLE INPUT MODULE, 4 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RARTDI4MLII: info = "RTD INPUT MODULE, 4 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RCNT2MLII: info = "Counter slave module, Reversible counter, 2 channels (Product by YASKWA), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_CN2CH: info = "Counter Module, 2 channels, Remapped ID, Actual ID is (0xA8)"; break;
                case AXT_MODULE.AXT_SIO_RCNT2RTEX: info = "Counter slave module, Reversible counter, 2 channels, For RTEX Only"; break;
                case AXT_MODULE.AXT_SIO_RCNT2MLIII: info = "Counter slave module, MechatroLink III AXT, 2 ch, Trigger per channel"; break;
                case AXT_MODULE.AXT_SIO_RHPC4MLIII: info = "Counter slave module, MechatroLink III AXT, 4 ch"; break;
                case AXT_MODULE.AXT_SIO_RAI16RTEX: info = "ANALOG VOLTAGE INPUT(+- 10V) 16 Channel RTEX"; break;
                case AXT_MODULE.AXT_SIO_RAO08RTEX: info = "ANALOG VOLTAGE OUTPUT(+- 10V) 08 Channel RTEX"; break;
                case AXT_MODULE.AXT_SIO_RAI8MLIII: info = "AI slave module, MechatroLink III AXT, Analog IN 8ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAI16MLIII: info = "AI slave module, MechatroLink III AXT, Analog IN 16ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAO4MLIII: info = "A0 slave module, MechatroLink III AXT, Analog OUT 4ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAO8MLIII: info = "A0 slave module, MechatroLink III AXT, Analog OUT 8ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RAVO4MLII: info = "DC VOLTAGE OUTPUT MODULE, 4 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RAV04MLIII: info = "AO Slave module, MechatroLink III M-SYSTEM Voltage output module"; break;
                case AXT_MODULE.AXT_SIO_RAVI4MLIII: info = "AI Slave module, MechatroLink III M-SYSTEM Voltage/Current input module"; break;
                case AXT_MODULE.AXT_SIO_RAI16MLIIISFA: info = "AI slave module, MechatroLink III AXT(SFA), Analog IN 16ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_RDB32MSMLIII: info = "DIO slave module, MechatroLink III M-SYSTEM, IN 16-Channel, OUT 16-Channel"; break;
                case AXT_MODULE.AXT_SIO_RAVI4MLII: info = "DC VOLTAGE/CURRENT INPUT MODULE, 4 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_SIO_RMEMORY_MLIII: info = "Memory Access type module, MechatroLink III"; break;
                case AXT_MODULE.AXT_SIO_RAVCI8MLII: info = "DC VOLTAGE/CURRENT INPUT MODULE, 8 points (Product by M-SYSTEM), For MLII only"; break;
                case AXT_MODULE.AXT_COM_234R: info = "COM-234R"; break;
                case AXT_MODULE.AXT_COM_484R: info = "COM-484R"; break;
                case AXT_MODULE.AXT_COM_234IDS: info = "COM-234IDS"; break;
                case AXT_MODULE.AXT_COM_484IDS: info = "COM-484IDS"; break;
                case AXT_MODULE.AXT_SIO_AO4F: info = "AO 4Ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_AI8F: info = "AI 8Ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_AI8AO4F: info = "AI 8Ch, AO 4Ch, 16 bit"; break;
                case AXT_MODULE.AXT_SIO_HPC4: info = "External Encoder module for 4Channel with Trigger function."; break;
                case AXT_MODULE.AXT_ECAT_MOTION: info = "EtherCAT Motion Module"; break;
                case AXT_MODULE.AXT_ECAT_DIO: info = "EtherCAT DIO Module"; break;
                case AXT_MODULE.AXT_ECAT_AIO: info = "EtherCAT AIO Module"; break;
                case AXT_MODULE.AXT_ECAT_COM: info = "EtherCAT Serial COM(RS232C) Module"; break;
                case AXT_MODULE.AXT_ECAT_COUPLER: info = "EtherCAT Coupler Module"; break;
                case AXT_MODULE.AXT_ECAT_CNT: info = "EtherCAT Count Module"; break;
                case AXT_MODULE.AXT_ECAT_UNKNOWN: info = "EtherCAT Unknown Module"; break;
                case AXT_MODULE.AXT_SMC_4V04_A: info = "Nx04_A"; break;
                default: info = "Unknown Module"; break;
            }
            return info;
        }
        public uint RefreshAxisStatus()
        {
            //m_AxisStatus = CAXM.AxmInfoGetAxisStatus(m_AxisId);
            uint rv = 0;
            uint rv1;
            try
            {
                int count = 0;
                uint uintVar = 0;
                int intVar = 0;
                double doubleVar = 0;

                //++ 지정 축의 Mechanical Signal Data(현재 기계적인 신호상태)를 확인합니다.
                if( ( rv1 = CAXM.AxmStatusReadMechanical( m_AxisID, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                {
                    m_AxisStatus = uintVar;
                }
                else if( rv == 0 )
                    rv = rv1;

                //++ 범용입력 신호의(Bit00-Bit04) 상태를 확인합니다.
                count = Enum.GetValues( typeof( AXT_MOTION_UNIV_INPUT ) ).Length;
                for( int i = 0; i < count; i++ )
                    if( ( rv1 = CAXM.AxmSignalReadInputBit( m_AxisID, i, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                        m_DIUniversal[ i ] = Convert.ToBoolean( uintVar );
                    else if( rv == 0 )
                        rv = rv1;

                //++ 범용출력 신호의(Bit00-Bit04) 상태를 확인합니다.
                count = Enum.GetValues( typeof( AXT_MOTION_UNIV_OUTPUT ) ).Length;
                for( int i = 0; i < count; i++ )
                    if( ( rv1 = CAXM.AxmSignalReadOutputBit( m_AxisID, i, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                        m_DOUniversal[ i ] = Convert.ToBoolean( uintVar );
                    else if( rv == 0 )
                        rv = rv1;

                // Actual Axis Position / Velocity
                if( ( rv1 = CAXM.AxmStatusReadVel( m_AxisID, ref doubleVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                {
                    m_AxisActVel = Math.Round( doubleVar, 5 );
                    m_AxisBlock.SetCurSpeed( m_Axis.AxisId, m_AxisActVel );

                }
                else if( rv == 0 )
                    rv = rv1;
                if( ( rv1 = CAXM.AxmStatusGetActPos( m_AxisID, ref doubleVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                //if( ( rv1 = CAXM.AxmStatusGetCmdPos( m_AxisID, ref doubleVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                {
                    m_AxisActPos = Math.Round( doubleVar, 5 );
                    m_AxisBlock.SetCurPosition( m_Axis.AxisId, m_AxisActPos );

                }
                else if( rv == 0 )
                    rv = rv1;

                //if( ( rv1 = CAXM.AxmStatusSetReadServoLoadRatio( m_AxisID, 2 ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                //{
                //}
                // Axis Torque
                if( ( rv1 = CAXM.AxmStatusReadServoLoadRatio( m_AxisID, ref doubleVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                {
                    m_AxisTorque = Math.Round( doubleVar, 5 );
                    m_AxisBlock.SetCurTorque( m_Axis.AxisId, m_AxisTorque );                    
                }
                else if( rv == 0 )
                    rv = rv1;

                if( ( rv = CAXM.AxmStatusReadInMotion( m_AxisID, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) m_InMotion = uintVar == 1;


                double dpHomeDogLength = 0;
                uint lpLevelScanTime = 0;
                uint upFineSearchUse = 0;
                uint upHomeClrUse = 0;
                rv1 = CAXM.AxmHomeGetFineAdjust( m_AxisID, ref dpHomeDogLength, ref lpLevelScanTime, ref upFineSearchUse, ref upHomeClrUse );
                //rv1 = CAXM.AxmHomeSetFineAdjust( m_AxisID, dpHomeDogLength, lpLevelScanTime, ( uint )( m_AxisID % 2 ), 1 );

                uint upValue = 0;
                enAxisInFlag state = enAxisInFlag.None;

                //Is Servo On?
                if( true == IsServoEnable() ) state |= enAxisInFlag.SvOn;
                //Is In position?
                if( true == IsInPos() ) state |= enAxisInFlag.InPos;
                // Is Motor moving?
                if( true == IsMoving() ) state |= enAxisInFlag.Busy;

                //Limit Sensor
                uint uIntPos = 0, uIntNeg =0;
                bool bNegLmt = false;
                bool bPosLmt = false;
                
                if((rv = CAXM.AxmSignalReadLimit(m_AxisID, ref uIntPos, ref uIntNeg)) ==(uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    bNegLmt = uIntNeg == 1 ? true : false;
                    bPosLmt = uIntPos == 1 ? true : false;
                    if( AppConfig.Instance.Simulation.AXT ) { bNegLmt = bPosLmt = false; }
                    if( bNegLmt ) state |= enAxisInFlag.Limit_N;
                    if( bPosLmt ) state |= enAxisInFlag.Limit_P;
                }

                //Alarm
                bool bAlarm = true;
                if((rv = CAXM.AxmSignalReadServoAlarm(m_AxisID, ref upValue)) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    bAlarm = upValue == 1 ? true : false;
                    if( bAlarm )
                    {
                        ushort usAlarm = 0;
                        state |= enAxisInFlag.Alarm;
                        if( ( rv = CAXM.AxmSignalReadServoAlarmCode( m_AxisID, ref usAlarm ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                        {
                            ExceptionLog.WriteLog( usAlarm.ToString() );
                        }
                    }
                }

                //Home
                if( HomeEnd ) state |= enAxisInFlag.HEnd;

                m_AxisBlock.SetMotionState( m_Axis.AxisId, state );
                    
            }
            catch(Exception ex)                
            {
                ExceptionLog.WriteLog( ex.Message );
            }
            return rv;
        }

        public void DoAxis()
        {
            enAxisOutFlag command = ( enAxisOutFlag )m_AxisBlock.GetMotionCommand( m_Axis.AxisId );
            if(command != enAxisOutFlag.CommandNone)
            {
                if( Convert.ToBoolean( command & enAxisOutFlag.ServoOn ) ) ServoOn();
                else if( Convert.ToBoolean( command & enAxisOutFlag.ServoOff ) ) ServoOff();
                else if( Convert.ToBoolean( command & enAxisOutFlag.AlarmClear ) ) AlarmReset();
                else if( Convert.ToBoolean( command & enAxisOutFlag.MotionStop ) ) Stop();
            }

            SeqMove( command );
            SeqJog( command );
            SeqHome( command );
        }
        public uint ServoOn()
        {
            uint upValue = 0;
            uint rv = CAXM.AxmSignalReadOutput( m_AxisID, ref upValue );
            return CAXM.AxmSignalWriteOutputBit( m_AxisID, ( int )AXT_MOTION_UNIV_OUTPUT.UIO_OUT0, ( uint )AXT_DIO_STATE.ON_STATE );
        }
        public uint ServoOff()
        {
            return CAXM.AxmSignalWriteOutputBit( m_AxisID, ( int )AXT_MOTION_UNIV_OUTPUT.UIO_OUT0, ( uint )AXT_DIO_STATE.OFF_STATE );
        }
        public uint AlarmReset()
        {
            return CAXM.AxmSignalWriteOutputBit( m_AxisID, ( int )AXT_MOTION_UNIV_OUTPUT.UIO_OUT1, ( uint )AXT_DIO_STATE.ON_STATE );
        }
        public uint JogMovePos( double moveVel )
        {
            uint rv;
            if( ( rv = CAXM.AxmMotSetProfileMode( m_AxisID, ( uint )AXT_MOTION_PROFILE_MODE.SYM_TRAPEZOIDE_MODE ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                return rv;

            if( ( rv = CAXM.AxmMoveVel( m_AxisID, Math.Abs( moveVel ), m_AxisAcc, m_AxisDec ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                return rv;
            return 0;
        }
        public uint JogMovePos()
        {
            uint rv = 0;
            double dSpeed = m_AxisBlock.GetJogSpeed( m_Axis.AxisId );
            rv = JogMovePos( dSpeed );
            return rv;
        }
        public uint JogMoveNeg( double moveVel )
        {
            uint rv;
            if( ( rv = CAXM.AxmMotSetProfileMode( m_AxisID, ( uint )AXT_MOTION_PROFILE_MODE.SYM_TRAPEZOIDE_MODE ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                return rv;

            if( ( rv = CAXM.AxmMoveVel( m_AxisID, Math.Abs( moveVel ) * ( -1 ), m_AxisAcc, m_AxisDec ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                return rv;
            return 0;
        }
        public uint JogMoveNeg()
        {
            double dSpeed = m_AxisBlock.GetJogSpeed( m_Axis.AxisId );
            uint rv = JogMoveNeg( dSpeed );
            return rv;
        }
        public uint JogStop()
        {
            uint rv;
            uint status = 0;
            if( ( rv = CAXM.AxmStatusReadInMotion( m_AxisID, ref status ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;
            if( status == ( uint )AXT_BOOLEAN.TRUE )    // TRUE : 모션 구동 중
            {
                if( ( rv = CAXM.AxmMoveSStop( m_AxisID ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;
            }

            return 0;
        }

        public uint TorqueMoveCw( double torque, double vel )
        {
            uint rv;
            if( ( rv = CAXM.AxmMoveStartTorque( m_AxisID, torque, Math.Abs( vel ) * ( 1 ), 0, 0, 0 ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;

            return 0;
        }
        public uint TorqueMoveCcw( double torque, double vel )
        {
            uint rv;
            if( ( rv = CAXM.AxmMoveStartTorque( m_AxisID, Math.Abs( torque ) * ( -1 ), Math.Abs( vel ) * ( -1 ), 0, 0, 0 ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;

            return 0;
        }
        public uint TorqueMoveStop()
        {
            uint rv;
            if( ( rv = CAXM.AxmMoveTorqueStop( m_AxisID, 0 ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;

            return 0;
        }
        public uint Move( double movePos, double moveVel )
        {
            uint rv;
            uint status = 0;
            if( ( rv = CAXM.AxmStatusReadInMotion( m_AxisID, ref status ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;
            if( status == ( uint )AXT_BOOLEAN.TRUE )
            {
                return 1;   // Error
            }
            else
            {
                AxisCmdPos = movePos;
                if( ( rv = CAXM.AxmMoveStartPos( m_AxisID, movePos, Math.Abs( moveVel ), m_AxisAcc, m_AxisDec ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                    return rv;
            }
            return 0;
        }
        public uint Move()
        {
            double dTargetVel = m_AxisBlock.GetTargetSpeed( m_Axis.AxisId );
            double dTargetPos = m_AxisBlock.GetTargetPosition( m_Axis.AxisId );

            uint rv = Move( dTargetPos, dTargetVel );
            return rv;
        }
        public uint Stop()
        {
            m_MotionStop = true;
            //return 0;

            uint rv = 0;
            uint status = 0;
            if( ( rv = CAXM.AxmStatusReadInMotion( m_AxisID, ref status ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;
            if( status == ( uint )AXT_BOOLEAN.TRUE )    // TRUE : 모션 구동 중
            {
                if( ( rv = CAXM.AxmMoveSStop( m_AxisID ) ) != ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) return rv;

                Init();
            }

            return rv;
        }

        public bool IsMoving()
        {
            bool bMove = true;
            uint rv = 0, upValue = 0;
            if( ( rv = CAXM.AxmSignalReadStop( m_AxisID, ref upValue ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
            {
                bMove = upValue == 0 ? true : false;
            }
            return bMove;
        }

        public bool IsInPos()
        {
            bool bInPos = true;
            uint rv = 0, upValue = 0;
            if( ( rv = CAXM.AxmSignalReadInpos( m_AxisID, ref upValue ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
            {
                bInPos = upValue == 1 ? true : false;
            }
            return bInPos;
        }
        public bool IsServoEnable()
        {
            bool bEnable = true;
            uint rv = 0, upValue = 0;
            if( ( rv = CAXM.AxmSignalIsServoOn( m_AxisID, ref upValue ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
            {
                bEnable = upValue == 1 ? true : false;
            }
            return bEnable;
        }

        public int GetAxisId()
        {
            return m_Axis.AxisId;
        }
        #endregion
    }

}
