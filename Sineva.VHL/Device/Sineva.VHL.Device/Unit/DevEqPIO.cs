/////////////////////////////////////////////////////////////////////////////////
///SEMI E84-1107 PIO Interface Review
/////////////////////////////////////////////////////////////////////////////////

using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device.Assem
{
    public class IfFlag // PIO <-> 장비 간 소통
    {
        public int CS; // <- Target Stage, 1 (CS0:ON, CS1:OFF), 2 (CS0:OFF, CS1:ON), 3 (CS0:ON, CS1:ON)
        public bool StartReq; //<- 시작해
        public bool Busy;     //-> 이동해(down)
        public bool OnIng;    //-> Material Exist, 2차 이동조건(gripper Action + up)
        public bool Complete; //<- 완료
        public bool ES; //-> Emergency Stop
        public bool Cancel; // -> Signal Abnormal (Interface 시작전, OnIng 전), Acquire/Deposit Fail 처리하자
        public bool Abort; // -> Signal Abnormal (Interface 시작후, OnIng 후), Foup 유무에 따라 Acquire/Deposit Fail 처리하자
        public bool OperateCancel; //<- Alarm 발생 강제 종료
        public bool PioOff; //-> PIO Disconnect 완료 했음.
        public bool PioOffNg; // -> Disconnect Alarm
        public bool HO; // -> EQ Alive

        public IfFlag() { }
        public void Reset()
        {
            //CS = 0; 이걸 지우면 않된다.
            StartReq = false;
            Busy = false;
            OnIng = false;
            Complete = false;
            ES = false;
            Cancel = false;
            Abort = false;
            OperateCancel = false;
            PioOff = false;
            PioOffNg = false;
            HO = false;
        }
        public void Start()
        {
            StartReq = true;
            Busy = false;
            OnIng = false;
            Complete = false;
            ES = false;
            Cancel = false;
            Abort = false;
            OperateCancel = false;
            PioOff = false;
            PioOffNg = false;
            HO = false;
        }
        public string SignalOnMsg()
        {
            string msg = string.Empty;
            msg += StartReq ? "StartReq," : "";
            msg += Busy ? "Busy," : "";
            msg += OnIng ? "OnIng," : "";
            msg += Complete ? "Complete," : "";
            msg += ES ? "ES," : "";
            msg += Cancel ? "Cancel," : "";
            msg += Abort ? "Abort," : "";
            msg += OperateCancel ? "OperateCancel," : "";
            msg += PioOff ? "PioOff," : "";
            msg += PioOffNg ? "PioOffNg," : "";
            msg += HO ? "HO," : "";
            return msg;
        }
    }

    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevEqPIO : _Device
    {
        private const string DevName = "DevEqPio";

        #region Fields - I/O
        private _DevPioComm m_PioComm = new _DevPioComm();

        private _DevOutput m_DoVALID = new _DevOutput();
        private _DevOutput m_DoCS1 = new _DevOutput();
        private _DevOutput m_DoCS2 = new _DevOutput();
        private _DevOutput m_DoAVBL = new _DevOutput(); // NC
        private _DevOutput m_DoTRREQ = new _DevOutput();
        private _DevOutput m_DoBUSY = new _DevOutput();
        private _DevOutput m_DoCOMPT = new _DevOutput();
        private _DevOutput m_DoCONT = new _DevOutput();

        private _DevInput m_DiLDREQ = new _DevInput();
        private _DevInput m_DiULREQ = new _DevInput();
        private _DevInput m_DiVA = new _DevInput(); // NC
        private _DevInput m_DiREADY = new _DevInput();
        private _DevInput m_DiVS0 = new _DevInput(); // NC
        private _DevInput m_DiVS1 = new _DevInput(); // NC
        private _DevInput m_DiHOAVBL = new _DevInput();
        private _DevInput m_DiES = new _DevInput();

        // PIO 관련 Alarm은 Warning 처리해야 OCS에서 Abrot/Cancel이 내려오지 않는다.
        private AlarmData m_ALM_Load_SignalAlreadyOn = null; // Valid 전에 Signal ON 상태
        private AlarmData m_ALM_Load_TA1_LReqNotGoOn_Timeout = null; // Valid -> L_REQ ON
        private AlarmData m_ALM_Load_LReqNotGoOff = null; // L_REQ not off, allotted amount of time during a load transaction
        private AlarmData m_ALM_Load_TransferFail = null; // transfer operation failure
        private AlarmData m_ALM_Load_MaterialStatusNotMatch = null; //Loading 해야 하는데 Foup 감지
        private AlarmData m_ALM_Load_TA2_ReadyNotGoOn_Timeout = null; // Ready not on, after tr_req
        private AlarmData m_ALM_Load_ReadyAlreadyGoOff = null; // ready signal off during load operation 
        private AlarmData m_ALM_Load_LReqRemainOn = null; // L_REQ not Off after loading complete
        private AlarmData m_ALM_Load_TA3_ReadyNotGoOff_Timeout = null;
        private AlarmData m_ALM_Load_Interrupt = null;
        private AlarmData m_ALM_Load_HO_Signal_Off = null; //Valid 전에 HO Signal Off 상태

        private AlarmData m_ALM_Load_LReq_SignalAlreadyOn = null; // Valid 전에 L_Req Signal ON 상태
        private AlarmData m_ALM_Load_Ready_SignalAlreadyOn = null; // Valid 전에 Ready Signal ON 상태
        private AlarmData m_ALM_Load_TA1_Ready_Signal_AlreadyOn = null; // Ready 신호가 L_Req전에 On됨. 
        private AlarmData m_ALM_Load_TA2_LReq_Signal_AbnormalOff = null; // L_Req 신호가 Ready전에 Off됨. 
        private AlarmData m_ALM_Load_ULReq_Signal_AbnormalOn = null; // UL_Req 신호가 비정상 On됨. 
        private AlarmData m_ALM_Load_Ready_off_during_Busy_TA3 = null; // Busy중 Ready 신호 비정상 Off됨. 

        // PIO 관련 Alarm은 Warning 처리해야 OCS에서 Abrot/Cancel이 내려오지 않는다.
        private AlarmData m_ALM_Unload_SignalAlreadyOn = null; // Valid 전에 Signal ON 상태
        private AlarmData m_ALM_Unload_TA1_UReqNotGoOn_Timeout = null; // Valid -> U_REQ ON
        private AlarmData m_ALM_Unload_TransferFail= null;
        private AlarmData m_ALM_Unload_MaterialStatusNotMatch = null; // Unloading 해야 하는데 Foup 없음.
        private AlarmData m_ALM_Unload_TA2_ReadyNotGoOn_Timeout = null;
        private AlarmData m_ALM_Unload_ReadyAlreadyGoOff = null;
        private AlarmData m_ALM_Unload_UReqRemainOn = null;
        private AlarmData m_ALM_Unload_TA3_ReadyNotGoOff_Timeout = null;
        private AlarmData m_ALM_Unload_Interrupt = null;
        private AlarmData m_ALM_Unload_HO_Signal_Off = null; // Valid 전에 HO Signal Off 상태

        private AlarmData m_ALM_Unload_ULReq_SignalAlreadyOn = null; // Valid 전에 UL_Req Signal ON 상태
        private AlarmData m_ALM_Unload_Ready_SignalAlreadyOn = null; // Valid 전에 Ready Signal ON 상태
        private AlarmData m_ALM_Unload_TA1_Ready_Signal_AlreadyOn = null; // Ready 신호가 UL_Req전에 On됨. 
        private AlarmData m_ALM_Unload_TA2_ULReq_Signal_AbnormalOff = null; // UL_Req 신호가 Ready전에 Off됨. 
        private AlarmData m_ALM_Unload_LReq_Signal_AbnormalOn = null; // L_Req 신호가 비정상 On됨. 
        private AlarmData m_ALM_Unload_Ready_off_during_Busy_TA3 = null; // Busy중 Ready 신호 비정상 Off됨. 

        private float m_TimerTa1 = 2.0f; // Valid -> L_REQ ON : 2sec . Active Eqp
        private float m_TimerTa2 = 2.0f; // TR_REQ -> READY ON  : 2sec . Active Eqp
        private float m_TimerTa3 = 2.0f; // COMPT -> READY OFF : 2sec . Active Eqp
        private float m_TimerTp1 = 2.0f; // L_REQ -> TR_REQ ON: 2sec . Passive Eqp
        private float m_TimerTp2 = 2.0f; // READY -> BUSY ON : 2sec. Passive Eqp
        private float m_TimerTp3 = 60.0f; // BUSY -> L_REQ OFF : 60sec. Passive Eqp
        private float m_TimerTp4 = 60.0f; // L_REQ OFF -> BUSY OFF : 60sec. Passive Eqp
        private float m_TimerTp5 = 2.0f;  // READY OFF -> Valid OFF : 2sec. Passive Eqp
        private float m_TimerTp6 = 1.0f; // Valid Off -> Valid On : 1sec. Passive Eqp

        private float m_TimerTD0ValidOnDelay = 0.1f; // CS On -> VALID On
        private float m_TimerTD1ValidOnDelay = 1.0f; // Continue handshake

        //SPL Alarm/////////////////////////////
        private AlarmData m_ALM_Spl_StartOn_Timeout = null;
        private AlarmData m_ALM_Spl_InsertRequestOff_Timeout = null;
        private AlarmData m_ALM_Spl_InsertReady_Timeout = null;
        private AlarmData m_ALM_Spl_ExportRequestOff_Timeout = null;
        private AlarmData m_ALM_Spl_ExportReady_Timeout = null;
        private AlarmData m_ALM_Spl_BusyOn_Timeout = null;
        private AlarmData m_ALM_Spl_PioSignalOff_Timeout = null;
        private AlarmData m_ALM_Spl_ESSignalOff_Alarm = null;
        //MTL Alarm/////////////////////////////
        private AlarmData m_ALM_Mtl_StartOn_Timeout = null;
        private AlarmData m_ALM_Mtl_InsertRequestOff_Timeout = null;
        private AlarmData m_ALM_Mtl_InsertReady_Timeout = null;
        private AlarmData m_ALM_Mtl_ExportRequestOff_Timeout = null;
        private AlarmData m_ALM_Mtl_ExportReady_Timeout = null;
        private AlarmData m_ALM_Mtl_BusyOn_Timeout = null;
        private AlarmData m_ALM_Mtl_PioSignalOff_Timeout = null;
        private AlarmData m_ALM_Mtl_ESSignalOff_Alarm = null;

        private SeqReadPIOLog m_SeqReadPIOLog = null;

        #endregion
        #region Fields -variable
        [XmlIgnore(), Browsable(false)]
        public IfFlag IfFlagRecv = new IfFlag();
        [XmlIgnore(), Browsable(false)]
        public IfFlag IfFlagSend = new IfFlag();
        [XmlIgnore(), Browsable(false)]
        public IfFlag IfFlagSpl = new IfFlag();
        [XmlIgnore(), Browsable(false)]
        public IfFlag IfFlagMtl = new IfFlag();

        private int m_PID = 0;

        #endregion

        #region Property - I/O, Device Relation       
        [Category("I/O Setting (Serial Comm)"), Description("Serial Comm"), DeviceSetting(true, true)]
        public _DevPioComm PioComm { get { return m_PioComm; } set { m_PioComm = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[1]VHL->EQP : Valid"), DeviceSetting(true)]
        public _DevOutput DoVALID { get { return m_DoVALID; } set { m_DoVALID = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[2]VHL->EQP : CS1"), DeviceSetting(true)]
        public _DevOutput DoCS1 { get { return m_DoCS1; } set { m_DoCS1 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[3]VHL->EQP : CS2"), DeviceSetting(true)]
        public _DevOutput DoCS2 { get { return m_DoCS2; } set { m_DoCS2 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[4]VHL->EQP : Available (NC)"), DeviceSetting(true)]
        public _DevOutput DoAVBL { get { return m_DoAVBL; } set { m_DoAVBL = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[]VHL->EQP : TR REQ"), DeviceSetting(true)]
        public _DevOutput DoTRREQ { get { return m_DoTRREQ; } set { m_DoTRREQ = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[6]VHL->EQP : BUSY"), DeviceSetting(true)]
        public _DevOutput DoBUSY { get { return m_DoBUSY; } set { m_DoBUSY = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[7]VHL->EQP : Complete"), DeviceSetting(true)]
        public _DevOutput DoCOMPT { get { return m_DoCOMPT; } set { m_DoCOMPT = value; } }
        [Category("I/O Setting (Digital Output)"), Description("[8]VHL->EQP : Continue"), DeviceSetting(true)]
        public _DevOutput DoCONT { get { return m_DoCONT; } set { m_DoCONT = value; } }


        [Category("I/O Setting (Digital Input)"), Description("[1]VHL<-EQP : LD REQ"), DeviceSetting(true)]
        public _DevInput DiLDREQ { get { return m_DiLDREQ; } set { m_DiLDREQ = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[2]VHL<-EQP : UL REQ"), DeviceSetting(true)]
        public _DevInput DiULREQ { get { return m_DiULREQ; } set { m_DiULREQ = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[3]VHL<-EQP : VA (NC)"), DeviceSetting(true)]
        public _DevInput DiVA { get { return m_DiVA; } set { m_DiVA = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[4]VHL<-EQP : READY"), DeviceSetting(true)]
        public _DevInput DiREADY { get { return m_DiREADY; } set { m_DiREADY = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[5]VHL<-EQP : VS0 (NC)"), DeviceSetting(true)]
        public _DevInput DiVS0 { get { return m_DiVS0; } set { m_DiVS0 = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[6]VHL<-EQP : VS1 (NC)"), DeviceSetting(true)]
        public _DevInput DiVS1 { get { return m_DiVS1; } set { m_DiVS1 = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[7]VHL<-EQP : Hand Off Available"), DeviceSetting(true)]
        public _DevInput DiHOAVBL { get { return m_DiHOAVBL; } set { m_DiHOAVBL = value; } }
        [Category("I/O Setting (Digital Input)"), Description("[8]VHL<-EQP : ES"), DeviceSetting(true)]
        public _DevInput DiES { get { return m_DiES; } set { m_DiES = value; } }

        public int PID { get { return m_PID; } set { m_PID = value; } }

        #endregion

        #region Property - Timer Setting
        [Category("!Setting Device (Active Timeout)"), DisplayName("Valid -> L_REQ ON Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTa1 { get { return m_TimerTa1; } set { m_TimerTa1 = value; } }
        [Category("!Setting Device (Active Timeout)"), DisplayName("TR_REQ -> READY ON Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTa2 { get { return m_TimerTa2; } set { m_TimerTa2 = value; } }
        [Category("!Setting Device (Active Timeout)"), DisplayName("COMPT -> READY OFF Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTa3 { get { return m_TimerTa3; } set { m_TimerTa3 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("L_REQ -> TR_REQ ON Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp1 { get { return m_TimerTp1; } set { m_TimerTp1 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("READY -> BUSY ON Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp2 { get { return m_TimerTp2; } set { m_TimerTp2 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("BUSY -> L_REQ OFF Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp3 { get { return m_TimerTp3; } set { m_TimerTp3 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("L_REQ OFF -> BUSY OFF Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp4 { get { return m_TimerTp4; } set { m_TimerTp4 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("READY OFF -> Valid OFF Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp5 { get { return m_TimerTp5; } set { m_TimerTp5 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("Valid Off -> Valid On Timeout(sec)"), Description("Interface Timeout (sec)"), DeviceSetting(false, true)]
        public float TimerTp6 { get { return m_TimerTp6; } set { m_TimerTp6 = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("CS On -> VALID On Time Delay(sec)"), Description("Interface Delay (sec)"), DeviceSetting(false, true)]
        public float TimerTD0ValidOnDelay { get { return m_TimerTD0ValidOnDelay; } set { m_TimerTD0ValidOnDelay = value; } }
        [Category("!Setting Device (Passive Timeout)"), DisplayName("Valid On -> Next Valid On Time Delay(sec)"), Description("Interface Delay (sec)"), DeviceSetting(false, true)]
        public float TimerTD1ValidOnDelay { get { return m_TimerTD1ValidOnDelay; } set { m_TimerTD1ValidOnDelay = value; } }
        #endregion

        #region AlarmData 
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_SignalAlreadyOn
        {
            get { return m_ALM_Load_SignalAlreadyOn; }
            set { m_ALM_Load_SignalAlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TA1_LReqNotGoOn_Timeout
        {
            get { return m_ALM_Load_TA1_LReqNotGoOn_Timeout; }
            set { m_ALM_Load_TA1_LReqNotGoOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_LReqNotGoOff
        {
            get { return m_ALM_Load_LReqNotGoOff; }
            set { m_ALM_Load_LReqNotGoOff = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TransferFail
        {
            get { return m_ALM_Load_TransferFail; }
            set { m_ALM_Load_TransferFail = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_MaterialStatusNotMatch
        {
            get { return m_ALM_Load_MaterialStatusNotMatch; }
            set { m_ALM_Load_MaterialStatusNotMatch = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TA2_ReadyNotGoOn_Timeout
        {
            get { return m_ALM_Load_TA2_ReadyNotGoOn_Timeout; }
            set { m_ALM_Load_TA2_ReadyNotGoOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_ReadyAlreadyGoOff
        {
            get { return m_ALM_Load_ReadyAlreadyGoOff; }
            set { m_ALM_Load_ReadyAlreadyGoOff = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_LReqRemainOn
        {
            get { return m_ALM_Load_LReqRemainOn; }
            set { m_ALM_Load_LReqRemainOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TA3_ReadyNotGoOff_Timeout
        {
            get { return m_ALM_Load_TA3_ReadyNotGoOff_Timeout; }
            set { m_ALM_Load_TA3_ReadyNotGoOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_Interrupt
        {
            get { return m_ALM_Load_Interrupt; }
            set { m_ALM_Load_Interrupt = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_SignalAlreadyOn
        {
            get { return m_ALM_Unload_SignalAlreadyOn; }
            set { m_ALM_Unload_SignalAlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_TA1_UReqNotGoOn_Timeout
        {
            get { return m_ALM_Unload_TA1_UReqNotGoOn_Timeout; }
            set { m_ALM_Unload_TA1_UReqNotGoOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_TransferFail
        {
            get { return m_ALM_Unload_TransferFail; }
            set { m_ALM_Unload_TransferFail = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_MaterialStatusNotMatch
        {
            get { return m_ALM_Unload_MaterialStatusNotMatch; }
            set { m_ALM_Unload_MaterialStatusNotMatch = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_ReadyNotGoOn_TA2Timeout
        {
            get { return m_ALM_Unload_TA2_ReadyNotGoOn_Timeout; }
            set { m_ALM_Unload_TA2_ReadyNotGoOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_ReadyAlreadyGoOff
        {
            get { return m_ALM_Unload_ReadyAlreadyGoOff; }
            set { m_ALM_Unload_ReadyAlreadyGoOff = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_UReqRemainOn
        {
            get { return m_ALM_Unload_UReqRemainOn; }
            set { m_ALM_Unload_UReqRemainOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_TA3_ReadyNotGoOff_Timeout
        {
            get { return m_ALM_Unload_TA3_ReadyNotGoOff_Timeout; }
            set { m_ALM_Unload_TA3_ReadyNotGoOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_Interrupt
        {
            get { return m_ALM_Unload_Interrupt; }
            set { m_ALM_Unload_Interrupt = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_StartOn_Timeout
        {
            get { return m_ALM_Spl_StartOn_Timeout; }
            set { m_ALM_Spl_StartOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_InsertRequestOff_Timeout
        {
            get { return m_ALM_Spl_InsertRequestOff_Timeout; }
            set { m_ALM_Spl_InsertRequestOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_InsertReady_Timeout
        {
            get { return m_ALM_Spl_InsertReady_Timeout; }
            set { m_ALM_Spl_InsertReady_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_ExportRequestOff_Timeout
        {
            get { return m_ALM_Spl_ExportRequestOff_Timeout; }
            set { m_ALM_Spl_ExportRequestOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_ExportReady_Timeout
        {
            get { return m_ALM_Spl_ExportReady_Timeout; }
            set { m_ALM_Spl_ExportReady_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_BusyOn_Timeout
        {
            get { return m_ALM_Spl_BusyOn_Timeout; }
            set { m_ALM_Spl_BusyOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_PioSignalOff_Timeout
        {
            get { return m_ALM_Spl_PioSignalOff_Timeout; }
            set { m_ALM_Spl_PioSignalOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Spl_ESSignalOff_Alarm
        {
            get { return m_ALM_Spl_ESSignalOff_Alarm; }
            set { m_ALM_Spl_ESSignalOff_Alarm = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_StartOn_Timeout
        {
            get { return m_ALM_Mtl_StartOn_Timeout; }
            set { m_ALM_Mtl_StartOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_InsertRequestOff_Timeout
        {
            get { return m_ALM_Mtl_InsertRequestOff_Timeout; }
            set { m_ALM_Mtl_InsertRequestOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_InsertReady_Timeout
        {
            get { return m_ALM_Mtl_InsertReady_Timeout; }
            set { m_ALM_Mtl_InsertReady_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_ExportRequestOff_Timeout
        {
            get { return m_ALM_Mtl_ExportRequestOff_Timeout; }
            set { m_ALM_Mtl_ExportRequestOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_ExportReady_Timeout
        {
            get { return m_ALM_Mtl_ExportReady_Timeout; }
            set { m_ALM_Mtl_ExportReady_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_BusyOn_Timeout
        {
            get { return m_ALM_Mtl_BusyOn_Timeout; }
            set { m_ALM_Mtl_BusyOn_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_PioSignalOff_Timeout
        {
            get { return m_ALM_Mtl_PioSignalOff_Timeout; }
            set { m_ALM_Mtl_PioSignalOff_Timeout = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Mtl_ESSignalOff_Alarm
        {
            get { return m_ALM_Mtl_ESSignalOff_Alarm; }
            set { m_ALM_Mtl_ESSignalOff_Alarm = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_HO_Signal_Off
        {
            get { return m_ALM_Load_HO_Signal_Off; }
            set { m_ALM_Load_HO_Signal_Off = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_HO_Signal_Off
        {
            get { return m_ALM_Unload_HO_Signal_Off; }
            set { m_ALM_Unload_HO_Signal_Off = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_Ready_SignalAlreadyOn
        {
            get { return m_ALM_Load_Ready_SignalAlreadyOn; }
            set { m_ALM_Load_Ready_SignalAlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TA1_Ready_Signal_AlreadyOn
        {
            get { return m_ALM_Load_TA1_Ready_Signal_AlreadyOn; }
            set { m_ALM_Load_TA1_Ready_Signal_AlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_TA2_LReq_Signal_AbnormalOff
        {
            get { return m_ALM_Load_TA2_LReq_Signal_AbnormalOff; }
            set { m_ALM_Load_TA2_LReq_Signal_AbnormalOff = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_ULReq_Signal_AbnormalOn
        {
            get { return m_ALM_Load_ULReq_Signal_AbnormalOn; }
            set { m_ALM_Load_ULReq_Signal_AbnormalOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_Ready_off_during_Busy_TA3
        {
            get { return m_ALM_Load_Ready_off_during_Busy_TA3; }
            set { m_ALM_Load_Ready_off_during_Busy_TA3 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_Ready_SignalAlreadyOn
        {
            get { return m_ALM_Unload_Ready_SignalAlreadyOn; }
            set { m_ALM_Unload_Ready_SignalAlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_TA1_Ready_Signal_AlreadyOn
        {
            get { return m_ALM_Unload_TA1_Ready_Signal_AlreadyOn; }
            set { m_ALM_Unload_TA1_Ready_Signal_AlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_TA2_ULReq_Signal_AbnormalOff
        {
            get { return m_ALM_Unload_TA2_ULReq_Signal_AbnormalOff; }
            set { m_ALM_Unload_TA2_ULReq_Signal_AbnormalOff = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_LReq_Signal_AbnormalOn
        {
            get { return m_ALM_Unload_LReq_Signal_AbnormalOn; }
            set { m_ALM_Unload_LReq_Signal_AbnormalOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_Ready_off_during_Busy_TA3
        {
            get { return m_ALM_Unload_Ready_off_during_Busy_TA3; }
            set { m_ALM_Unload_Ready_off_during_Busy_TA3 = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Load_LReq_SignalAlreadyOn
        {
            get { return m_ALM_Load_LReq_SignalAlreadyOn; }
            set { m_ALM_Load_LReq_SignalAlreadyOn = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Unload_ULReq_SignalAlreadyOn
        {
            get { return m_ALM_Unload_ULReq_SignalAlreadyOn; }
            set { m_ALM_Unload_ULReq_SignalAlreadyOn = value; }
        }
        #endregion


        #region Constructor
        public DevEqPIO()
        {
            if (!Initialized)
            {
                this.MyName = DevName;
            }
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            this.ParentName = name;
            if (read_xml) ReadXml();
            if (this.IsValid == false) return true;

            //////////////////////////////////////////////////////////////////////////////
            #region 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 2. 필수 I/O 할당 여부 확인

            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            if (m_PioComm.IsValid)
            {
                m_PioComm.SetAutoConnection(false);
                ok &= m_PioComm.Initialize(this.ToString(), false, false);
            }

            if (DoVALID.IsValid) ok &= DoVALID.Initialize(this.ToString(), false, false);
            if (DoCS1.IsValid) ok &= DoCS1.Initialize(this.ToString(), false, false);
            if (DoCS2.IsValid) ok &= DoCS2.Initialize(this.ToString(), false, false);
            if (DoTRREQ.IsValid) ok &= DoTRREQ.Initialize(this.ToString(), false, false);
            if (DoBUSY.IsValid) ok &= DoBUSY.Initialize(this.ToString(), false, false);
            if (DoCOMPT.IsValid) ok &= DoCOMPT.Initialize(this.ToString(), false, false);
            if (DoCONT.IsValid) ok &= DoCONT.Initialize(this.ToString(), false, false);
            if (DoAVBL.IsValid) ok &= DoAVBL.Initialize(this.ToString(), false, false);

            if (DiLDREQ.IsValid) ok &= DiLDREQ.Initialize(this.ToString(), false, false);
            if (DiULREQ.IsValid) ok &= DiULREQ.Initialize(this.ToString(), false, false);
            if (DiREADY.IsValid) ok &= DiREADY.Initialize(this.ToString(), false, false);
            if (DiHOAVBL.IsValid) ok &= DiHOAVBL.Initialize(this.ToString(), false, false);
            if (DiES.IsValid) ok &= DiES.Initialize(this.ToString(), false, false);
            if (DiVA.IsValid) ok &= DiVA.Initialize(this.ToString(), false, false);
            if (DiVS0.IsValid) ok &= DiVS0.Initialize(this.ToString(), false, false);
            if (DiVS1.IsValid) ok &= DiVS1.Initialize(this.ToString(), false, false);

            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_Load_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Signal Already ON before Valid ON Alarm");
            m_ALM_Load_TA1_LReqNotGoOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request On Timeout TA1");
            m_ALM_Load_LReqNotGoOff = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request Off Timeout");
            m_ALM_Load_TransferFail = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Transfer Fail Alarm");
            m_ALM_Load_MaterialStatusNotMatch = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Material Exist Not Match");
            m_ALM_Load_TA2_ReadyNotGoOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready On Timeout TA2");
            m_ALM_Load_ReadyAlreadyGoOff = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready Signal Off During Transfer");
            m_ALM_Load_LReqRemainOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request Not Off After Complete");
            m_ALM_Load_TA3_ReadyNotGoOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready Off Timeout TA3");
            m_ALM_Load_Interrupt = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Interrupt Alarm");
            m_ALM_Load_HO_Signal_Off = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load HO Signal Off before Valid ON Warning");

            m_ALM_Load_LReq_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request Already On before Valid ON Alarm");
            m_ALM_Load_Ready_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready Signal Already On before Valid ON Alarm");
            m_ALM_Load_TA1_Ready_Signal_AlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready Signal Already On before Load Request ON TA1 Warning");
            m_ALM_Load_TA2_LReq_Signal_AbnormalOff = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request is off before Ready TA2 Warning");
            m_ALM_Load_ULReq_Signal_AbnormalOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Request is Abnormally ON Warning");
            m_ALM_Load_Ready_off_during_Busy_TA3 = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Ready signal off during Busy TA3");

            m_ALM_Unload_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Signal Already ON before Valid ON Alarm");
            m_ALM_Unload_TA1_UReqNotGoOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Request On Timeout TA1");
            m_ALM_Unload_TransferFail = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Transfer Fail Alarm");
            m_ALM_Unload_MaterialStatusNotMatch = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Material Exist Not Match");
            m_ALM_Unload_TA2_ReadyNotGoOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready On Timeout TA2");
            m_ALM_Unload_ReadyAlreadyGoOff = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready Signal Off During Transfer");
            m_ALM_Unload_UReqRemainOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Request Not Off After Complete");
            m_ALM_Unload_TA3_ReadyNotGoOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready Off Timeout TA3");
            m_ALM_Unload_Interrupt = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Interrupt Alarm");
            m_ALM_Unload_HO_Signal_Off = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload HO Signal Off before Valid ON Warning");

            m_ALM_Unload_ULReq_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Request Already On before Valid ON Alarm");
            m_ALM_Unload_Ready_SignalAlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready Signal Already On before Valid ON Alarm");
            m_ALM_Unload_TA1_Ready_Signal_AlreadyOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready Signal Already On before Unload Request ON TA1 Warning");
            m_ALM_Unload_TA2_ULReq_Signal_AbnormalOff = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Request is off before Ready TA2 Warning");
            m_ALM_Unload_LReq_Signal_AbnormalOn = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Load Request is Abnormally ON Warning");
            m_ALM_Unload_Ready_off_during_Busy_TA3 = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, false, MyName, "PIO", "Unload Ready signal off during Busy TA3");

            ////SPL Alarm/////////////////////////////
            m_ALM_Spl_StartOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Start On Timeout Alarm");
            m_ALM_Spl_InsertRequestOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Insert Request Off Timeout Alarm");
            m_ALM_Spl_InsertReady_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Insert Ready On Timeout Alarm");
            m_ALM_Spl_ExportRequestOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Export Request Off Timeout Alarm");
            m_ALM_Spl_ExportReady_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Export Ready On Timeout Alarm");
            m_ALM_Spl_BusyOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Busy On Timeout Alarm");
            m_ALM_Spl_PioSignalOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "Signal Off Timeout Alarm");
            m_ALM_Spl_ESSignalOff_Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "SPL PIO", "ES Signal Off Alarm");
            ////MTL Alarm/////////////////////////////
            m_ALM_Mtl_StartOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Start On Timeout Alarm");
            m_ALM_Mtl_InsertRequestOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Insert Request Off Timeout Alarm");
            m_ALM_Mtl_InsertReady_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Insert Ready On Timeout Alarm");
            m_ALM_Mtl_ExportRequestOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Export Request Off Timeout Alarm");
            m_ALM_Mtl_ExportReady_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Export Ready On Timeout Alarm");
            m_ALM_Mtl_BusyOn_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Busy On Timeout Alarm");
            m_ALM_Mtl_PioSignalOff_Timeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "Signal Off Timeout Alarm");
            m_ALM_Mtl_ESSignalOff_Alarm = AlarmListProvider.Instance.NewAlarm(AlarmCode.IrrecoverableError, true, MyName, "MTL PIO", "ES Signal Off Alarm");
            ////////////////////////////////////////////////////////////////////////////////
            #endregion

            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            ResetIfRecv();
            ResetIfSend();

            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqReadPIOLog = new SeqReadPIOLog(this);
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            Initialized = true;
            Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return Initialized;
        }
        #endregion

        #region Methods - EQP
        public override void SeqAbort()
        {
            if (!Initialized) return;

            ResetPIO();
            ResetIfSend();
            ResetIfRecv();
            IfFlagSpl.Reset();
            IfFlagMtl.Reset();
            m_PioComm.SeqAbort();
        }
        public int SetChannelId(int id, int channel, bool spl = false)
        {
            if (!Initialized) return 0;

            int rv = -1;
            rv = m_PioComm.SelectChannel(id, channel, spl);
            m_PID = id;
            return rv;
        }
        public void ResetPIO()
        {
            if (!Initialized) return;

            SetValid(false);
            SetCs1(false);
            SetCs2(false);
            SetTrReq(false);
            SetBusy(false);
            SetComplete(false);
            SetContinue(false);
            SetAvailable(false);
        }
        public bool IsPioRun()
        {
            bool rv = false;
            rv = m_DoVALID.IsDetected;
            return rv;
        }
        private int readEQPPIO()
        {
            if (!Initialized) return 0;

            #region "Code"
            int rv = 0;
            try
            {
                if (m_DoVALID.IsValid) rv |= m_DoVALID.IsDetected ? 0x01 << 0 : 0x00;
                if (m_DoCS1.IsValid) rv |= m_DoCS1.IsDetected ? 0x01 << 1 : 0x00;
                if (m_DoCS2.IsValid) rv |= m_DoCS2.IsDetected ? 0x01 << 2 : 0x00;
                if (m_DoAVBL.IsValid) rv |= m_DoAVBL.IsDetected ? 0x01 << 3 : 0x00;
                if (m_DoTRREQ.IsValid) rv |= m_DoTRREQ.IsDetected ? 0x01 << 4 : 0x00;
                if (m_DoBUSY.IsValid) rv |= m_DoBUSY.IsDetected ? 0x01 << 5 : 0x00;
                if (m_DoCOMPT.IsValid) rv |= m_DoCOMPT.IsDetected ? 0x01 << 6 : 0x00;
                if (m_DoCONT.IsValid) rv |= m_DoCONT.IsDetected ? 0x01 << 7 : 0x00;

                rv |= GetLdReq() ? 0x01 << 8 : 0x00;
                rv |= GetUlReq() ? 0x01 << 9 : 0x00;
                rv |= GetVA() ? 0x01 << 10 : 0x00;
                rv |= GetReady() ? 0x01 << 11 : 0x00;
                rv |= GetVs0() ? 0x01 << 12 : 0x00;
                rv |= GetVs1() ? 0x01 << 13 : 0x00;
                rv |= GetHandOffAvailable() ? 0x01 << 14 : 0x00;
                rv |= GetEs() ? 0x01 << 15 : 0x00;
            }
            catch (Exception ex)
            {
                rv = 0;
            }
            return rv;
            #endregion
        }
        #endregion
        #region Port Interface
        public void SetValid(bool flg)
        {
            if (!Initialized) return;
            if (m_DoVALID.IsValid) m_DoVALID.SetDo(flg);
        }
        public void SetCs1(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS1.IsValid) m_DoCS1.SetDo(flg);
        }
        public void SetCs2(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS2.IsValid) m_DoCS2.SetDo(flg);
        }
        public void SetAvailable(bool flg)
        {
            if (!Initialized) return;
            if (m_DoAVBL.IsValid) m_DoAVBL.SetDo(flg);
        }
        public void SetTrReq(bool flg)
        {
            if (!Initialized) return;
            if (m_DoTRREQ.IsValid) m_DoTRREQ.SetDo(flg);
        }
        public void SetBusy(bool flg)
        {
            if (!Initialized) return;
            if (m_DoBUSY.IsValid) m_DoBUSY.SetDo(flg);
        }
        public void SetComplete(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCOMPT.IsValid) m_DoCOMPT.SetDo(flg);
        }
        public void SetContinue(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCONT.IsValid) m_DoCONT.SetDo(flg);
        }
        public bool GetLdReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiLDREQ.IsValid ? m_DiLDREQ.IsDetected : false;
            return rv;
        }
        public bool GetUlReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiULREQ.IsValid ? m_DiULREQ.IsDetected : false;
            return rv;
        }
        public bool GetVA()
        {
            if (!Initialized) return false;
            bool rv = m_DiVA.IsValid ? m_DiVA.IsDetected : false;
            return rv;
        }
        public bool GetReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiREADY.IsValid ? m_DiREADY.IsDetected : false;
            return rv;
        }
        public bool GetVs0()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS0.IsValid ? m_DiVS0.IsDetected : false;
            return rv;
        }
        public bool GetVs1()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS1.IsValid ? m_DiVS1.IsDetected : false;
            return rv;
        }
        public bool GetHandOffAvailable()
        {
            if (!Initialized) return false;
            bool rv = m_DiHOAVBL.IsValid ? m_DiHOAVBL.IsDetected : false;
            return rv;
        }
        public bool GetEs()
        {
            if (!Initialized) return false;
            bool rv = m_DiES.IsValid ? m_DiES.IsDetected : false;
            //if (AppConfig.Instance.Simulation.MY_DEBUG) rv = true;
            return rv;
        }
        #endregion

        #region SPL Interface
        public void SetSplValid(bool flg)
        {
            if (!Initialized) return;
            if (m_DoVALID.IsValid) m_DoVALID.SetDo(flg);
        }
        public void SetSplCs1(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS1.IsValid) m_DoCS1.SetDo(flg);
        }
        public void SetSplCs2(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS2.IsValid) m_DoCS2.SetDo(flg);
        }
        public void __SetSplAvailable(bool flg)
        {
            if (!Initialized) return;
            if (m_DoAVBL.IsValid) m_DoAVBL.SetDo(flg);
        }
        public void SetSplMoveRequest(bool flg)
        {
            if (!Initialized) return;
            if (m_DoTRREQ.IsValid) m_DoTRREQ.SetDo(flg);
        }
        public void SetSplMoveStart(bool flg)
        {
            if (!Initialized) return;
            if (m_DoBUSY.IsValid) m_DoBUSY.SetDo(flg);
        }
        public void SetSplMoveComplete(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCOMPT.IsValid) m_DoCOMPT.SetDo(flg);
        }
        public void __SetSplContinue(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCONT.IsValid) m_DoCONT.SetDo(flg);
        }

        public bool GetSplInsertReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiLDREQ.IsValid ? m_DiLDREQ.IsDetected : false;
            return rv;
        }
        public bool GetSplExportReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiULREQ.IsValid ? m_DiULREQ.IsDetected : false;
            return rv;
        }
        public bool GetSplBusy()
        {
            if (!Initialized) return false;
            bool rv = m_DiVA.IsValid ? m_DiVA.IsDetected : false;
            return rv;
        }
        public bool GetSplInsertReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiREADY.IsValid ? m_DiREADY.IsDetected : false;
            return rv;
        }
        public bool GetSplExportReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS0.IsValid ? m_DiVS0.IsDetected : false;
            return rv;
        }
        public bool __GetSplVS1()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS1.IsValid ? m_DiVS1.IsDetected : false;
            return rv;
        }
        public bool GetSplPioStart()
        {
            if (!Initialized) return false;
            bool rv = m_DiHOAVBL.IsValid ? m_DiHOAVBL.IsDetected : false;
            return rv;
        }
        public bool GetSplES()
        {
            if (!Initialized) return false;
            bool rv = m_DiES.IsValid ? m_DiES.IsDetected : false;
            return rv;
        }
        #endregion

        #region MTL Interface
        public void SetMtlValid(bool flg)
        {
            if (!Initialized) return;
            if (m_DoVALID.IsValid) m_DoVALID.SetDo(flg);
        }
        public void SetMtlCs1(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS1.IsValid) m_DoCS1.SetDo(flg);
        }
        public void SetMtlCs2(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS2.IsValid) m_DoCS2.SetDo(flg);
        }
        public void SetMtlAvailable(bool flg)
        {
            if (!Initialized) return;
            if (m_DoAVBL.IsValid) m_DoAVBL.SetDo(flg);
        }
        public void SetMtlMoveRequest(bool flg)
        {
            if (!Initialized) return;
            if (m_DoTRREQ.IsValid) m_DoTRREQ.SetDo(flg);
        }
        public void SetMtlMoveStart(bool flg)
        {
            if (!Initialized) return;
            if (m_DoBUSY.IsValid) m_DoBUSY.SetDo(flg);
        }
        public void SetMtlMoveComplete(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCOMPT.IsValid) m_DoCOMPT.SetDo(flg);
        }
        public void __SetMtlContinue(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCONT.IsValid) m_DoCONT.SetDo(flg);
        }

        public bool GetMtlInsertReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiLDREQ.IsValid ? m_DiLDREQ.IsDetected : false;
            return rv;
        }
        public bool GetMtlExportReq()
        {
            if (!Initialized) return false;
            bool rv = m_DiULREQ.IsValid ? m_DiULREQ.IsDetected : false;
            return rv;
        }
        public bool GetMtlBusy()
        {
            if (!Initialized) return false;
            bool rv = m_DiVA.IsValid ? m_DiVA.IsDetected : false;
            return rv;
        }
        public bool GetMtlInsertReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiREADY.IsValid ? m_DiREADY.IsDetected : false;
            return rv;
        }
        public bool GetMtlExportReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS0.IsValid ? m_DiVS0.IsDetected : false;
            return rv;
        }
        public bool GetMtlPioStart()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS1.IsValid ? m_DiVS1.IsDetected : false;
            return rv;
        }
        public bool __GetMtlES()
        {
            if (!Initialized) return false;
            bool rv = m_DiHOAVBL.IsValid ? m_DiHOAVBL.IsDetected : false;
            return rv;
        }
        public bool GetMtlES()
        {
            if (!Initialized) return false;
            bool rv = m_DiES.IsValid ? m_DiES.IsDetected : false;
            return rv;
        }
        #endregion

        #region AutoDoor Interface
        public void SetAutoDoorStart(bool flg)
        {
            if (!Initialized) return;
            if (m_DoVALID.IsValid) m_DoVALID.SetDo(flg);
        }
        public void __SetAutoDoorCs1(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS1.IsValid) m_DoCS1.SetDo(flg);
        }
        public void __SetAutoDoorCs2(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCS2.IsValid) m_DoCS2.SetDo(flg);
        }
        public void __SetAutoDoorAvailable(bool flg)
        {
            if (!Initialized) return;
            if (m_DoAVBL.IsValid) m_DoAVBL.SetDo(flg);
        }
        public void __SetAutoDoorTrReq(bool flg)
        {
            if (!Initialized) return;
            if (m_DoTRREQ.IsValid) m_DoTRREQ.SetDo(flg);
        }
        public void __SetAutoDoorBusy(bool flg)
        {
            if (!Initialized) return;
            if (m_DoBUSY.IsValid) m_DoBUSY.SetDo(flg);
        }
        public void __SetAutoDoorComplete(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCOMPT.IsValid) m_DoCOMPT.SetDo(flg);
        }
        public void __SetAutoDoorContinue(bool flg)
        {
            if (!Initialized) return;
            if (m_DoCONT.IsValid) m_DoCONT.SetDo(flg);
        }
        public bool GetAutoDoorStart()
        {
            if (!Initialized) return false;
            bool rv = m_DiLDREQ.IsValid ? m_DiLDREQ.IsDetected : false;
            return rv;
        }
        public bool GetAutoDoorOpen()
        {
            if (!Initialized) return false;
            bool rv = m_DiULREQ.IsValid ? m_DiULREQ.IsDetected : false;
            return rv;
        }
        public bool GetAutoDoorClose()
        {
            if (!Initialized) return false;
            bool rv = m_DiVA.IsValid ? m_DiVA.IsDetected : false;
            return rv;
        }
        public bool __GetAutoDoorReady()
        {
            if (!Initialized) return false;
            bool rv = m_DiREADY.IsValid ? m_DiREADY.IsDetected : false;
            return rv;
        }
        public bool __GetAutoDoorVs0()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS0.IsValid ? m_DiVS0.IsDetected : false;
            return rv;
        }
        public bool __GetAutoDoorVs1()
        {
            if (!Initialized) return false;
            bool rv = m_DiVS1.IsValid ? m_DiVS1.IsDetected : false;
            return rv;
        }
        public bool __GetAutoDoorHandOffAvailable()
        {
            if (!Initialized) return false;
            bool rv = m_DiHOAVBL.IsValid ? m_DiHOAVBL.IsDetected : false;
            return rv;
        }
        public bool GetAutoDoorPassPossible()
        {
            if (!Initialized) return false;
            bool rv = m_DiES.IsValid ? m_DiES.IsDetected : false;
            return rv;
        }
        #endregion

        #region AutoTeaching
        public bool GetReflectiveSensor()
        {
            if (!Initialized) return false;
            bool rv = m_DiULREQ.IsValid ? m_DiULREQ.IsDetected : false;
            return rv;
        }
        #endregion

        #region Methods - varibale reset
        public void ResetIfRecv()
        {
            string log = string.Empty;
            log += IfFlagRecv.SignalOnMsg();
            if (log.Length > 0) SequenceDeviceLog.WriteLog(DevName, string.Format("Recv I/F Flag Reset : {0}", log));
            IfFlagRecv.Reset();
        }
        public void ResetIfSend()
        {
            string log = string.Empty;
            log += IfFlagSend.SignalOnMsg();
            if (log.Length > 0) SequenceDeviceLog.WriteLog(DevName, string.Format("Send I/F Flag Reset : {0}", log));
            IfFlagSend.Reset();
        }
        #endregion

        #region Sequence
        private class SeqReadPIOLog : XSeqFunc
        {
            #region Enum
            [Serializable()]
            public enum enPIOName : int
            {
                PID = 0,

                VALID = 1,
                CS1,
                CS2,
                AVBL,
                TR_REQ,
                BUSY,
                COMPT,
                CONT,

                LD_REQ,
                UD_REQ,
                VA,
                READY,
                VS0,
                VS1,
                HO,
                ES,
            }
            #endregion

            #region Field
            private DevEqPIO m_Device = null;

            private PIOSignalLog m_PioLogger = null;
            private Dictionary<enPIOName, string> m_Pio = new Dictionary<enPIOName, string>();

            private int m_OldPIO = 0;


            #endregion

            public SeqReadPIOLog(DevEqPIO device)
            {
                this.SeqName = $"SeqReadPIOLog{device.MyName}";
                m_Device = device;

                #region LOG Title
                foreach (enPIOName pio in Enum.GetValues(typeof(enPIOName)))
                {
                    m_Pio.Add(pio, "");
                }
                #endregion
                string[] pio_names = Enum.GetNames(typeof(enPIOName));
                m_PioLogger = new PIOSignalLog(pio_names);

                TaskDeviceControl.Instance.RegSeq(this);
            }

            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            try
                            {
                                int curPIO = m_Device.readEQPPIO();
                                if (curPIO != m_OldPIO)
                                {
									//고객요청으로 인해 수정
                                    m_Pio[enPIOName.PID] = m_Device.PID.ToString();
                                    m_Pio[enPIOName.VALID] = m_Device.m_DoVALID.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.CS1] = m_Device.m_DoCS1.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.CS2] = m_Device.m_DoCS2.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.AVBL] = m_Device.DoAVBL.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.TR_REQ] = m_Device.m_DoTRREQ.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.BUSY] = m_Device.m_DoBUSY.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.COMPT] = m_Device.m_DoCOMPT.IsDetected ? "1" : "0";
                                    m_Pio[enPIOName.CONT] = m_Device.m_DoCONT.IsDetected ? "1" : "0";

                                    m_Pio[enPIOName.LD_REQ] = m_Device.GetLdReq() ? "1" : "0";
                                    m_Pio[enPIOName.UD_REQ] = m_Device.GetUlReq() ? "1" : "0";
                                    m_Pio[enPIOName.VA] = m_Device.GetVA() ? "1" : "0";
                                    m_Pio[enPIOName.READY] = m_Device.GetReady() ? "1" : "0";
                                    m_Pio[enPIOName.VS0] = m_Device.GetVs0() ? "1" : "0";
                                    m_Pio[enPIOName.VS1] = m_Device.GetVs1() ? "1" : "0";
                                    m_Pio[enPIOName.HO] = m_Device.GetHandOffAvailable() ? "1" : "0";
                                    m_Pio[enPIOName.ES] = m_Device.GetEs() ? "1" : "0";

                                    m_PioLogger.WriteLog(m_Pio.Values.ToArray());
                                    m_OldPIO = curPIO;
                                }
                                seqNo = 0;
                            }
                            catch (Exception ex)
                            {
                                ExceptionLog.WriteLog(ex.ToString());
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
        }
        #endregion

        #region [Xml Read/Write]
        public override bool ReadXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return false;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists == false)
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<DevEqPIO>();
                DevEqPIO dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.PioComm = dev.PioComm;

                    this.DoVALID = dev.DoVALID;
                    this.DoCS1 = dev.DoCS1;
                    this.DoCS2 = dev.DoCS2;
                    this.DoTRREQ = dev.DoTRREQ;
                    this.DoBUSY = dev.DoBUSY;
                    this.DoCOMPT = dev.DoCOMPT;
                    this.DoCONT = dev.DoCONT;
                    this.DoAVBL = dev.DoAVBL;

                    this.DiLDREQ = dev.DiLDREQ;
                    this.DiULREQ = dev.DiULREQ;
                    this.DiREADY = dev.DiREADY;
                    this.DiHOAVBL = dev.DiHOAVBL;
                    this.DiES = dev.DiES;
                    this.DiVA = dev.DiVA;
                    this.DiVS0 = dev.DiVS0;
                    this.DiVS1 = dev.DiVS1;

                    this.TimerTa1 = dev.TimerTa1;
                    this.TimerTa2 = dev.TimerTa2;
                    this.TimerTa3 = dev.TimerTa3;
                    this.TimerTp1 = dev.TimerTp1;
                    this.TimerTp2 = dev.TimerTp2;
                    this.TimerTp3 = dev.TimerTp3;
                    this.TimerTp4 = dev.TimerTp4;
                    this.TimerTp5 = dev.TimerTp5;
                    this.TimerTp6 = dev.TimerTp6;
                    this.TimerTD0ValidOnDelay = dev.TimerTD0ValidOnDelay;
                    this.TimerTD1ValidOnDelay = dev.TimerTD1ValidOnDelay;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public override void WriteXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return;

            try
            {
                var helperXml = new XmlHelper<DevEqPIO>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public bool CheckPath(ref string fileName)
        {
            bool ok = false;
            string filePath = AppConfig.Instance.XmlDevicesPath;

            if (Directory.Exists(filePath) == false)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "Configuration folder select";
                dlg.SelectedPath = AppConfig.GetSolutionPath();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filePath = dlg.SelectedPath;
                    if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                        AppConfig.Instance.WriteXml();
                    }
                    fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                ok = true;
            }
            return ok;
        }

        public string GetDefaultFileName()
        {
            if (this.MyName == "") this.MyName = DevName;
            return this.ToString();
        }
        #endregion
    }
}
