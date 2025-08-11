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
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device.Assem
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevOBS : _Device
    {
        private const string DevName = "OBS";

        #region Fields
        private readonly int MAX_TYPE = 32;
        private _DevInput m_OBS1 = new _DevInput();
        private _DevInput m_OBS2 = new _DevInput();
        private _DevInput m_OBS3 = new _DevInput();
        private _DevInput m_OBSError = new _DevInput();
        private _DevOutput m_TYPE1 = new _DevOutput();
        private _DevOutput m_TYPE2 = new _DevOutput();
        private _DevOutput m_TYPE3 = new _DevOutput();
        private _DevOutput m_TYPE4 = new _DevOutput();
        private _DevOutput m_TYPE5 = new _DevOutput();

        private Queue<int> m_QueueOBS1 = new Queue<int>();
        private Queue<int> m_QueueOBS2 = new Queue<int>();
        private Queue<int> m_QueueOBS3 = new Queue<int>();
        #endregion

        #region Property - I/O, Device Relation
        [Category("I/O Setting (Digital Input)"), Description("Stop Sensor Detect"), DeviceSetting(true)]
        public _DevInput OBS1 { get { return m_OBS1; } set { m_OBS1 = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Second Deccelation"), DeviceSetting(true)]
        public _DevInput OBS2 { get { return m_OBS2; } set { m_OBS2 = value; } }
        [Category("I/O Setting (Digital Input)"), Description("First Deccelation"), DeviceSetting(true)]
        public _DevInput OBS3 { get { return m_OBS3; } set { m_OBS3 = value; } }

        [Category("I/O Setting (Digital Input)"), Description("Sensor Alarm"), DeviceSetting(true)]
        public _DevInput OBSError { get { return m_OBSError; } set { m_OBSError = value; } }
        

        [Category("I/O Setting (Digital Output)"), Description("Level Set Bit1"), DeviceSetting(true)]
        public _DevOutput TYPE1 { get { return m_TYPE1; } set { m_TYPE1 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Level Set Bit1"), DeviceSetting(true)]
        public _DevOutput TYPE2 { get { return m_TYPE2; } set { m_TYPE2 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Level Set Bit1"), DeviceSetting(true)]
        public _DevOutput TYPE3 { get { return m_TYPE3; } set { m_TYPE3 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Level Set Bit1"), DeviceSetting(true)]
        public _DevOutput TYPE4 { get { return m_TYPE4; } set { m_TYPE4 = value; } }
        [Category("I/O Setting (Digital Output)"), Description("Level Set Bit1"), DeviceSetting(true)]
        public _DevOutput TYPE5 { get { return m_TYPE5; } set { m_TYPE5 = value; } }
        #endregion

        #region Constructor
        public _DevOBS()
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
            if (name != "") this.ParentName = name;
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
            if (OBS1.IsValid) ok &= OBS1.Initialize(this.ToString(), false);
            if (OBS2.IsValid) ok &= OBS2.Initialize(this.ToString(), false);
            if (OBS3.IsValid) ok &= OBS3.Initialize(this.ToString(), false);
            if (m_OBSError.IsValid) ok &= m_OBSError.Initialize(this.ToString(), false);
            if (TYPE1.IsValid) ok &= TYPE1.Initialize(this.ToString(), false);
            if (TYPE2.IsValid) ok &= TYPE2.Initialize(this.ToString(), false);
            if (TYPE3.IsValid) ok &= TYPE3.Initialize(this.ToString(), false);
            if (TYPE4.IsValid) ok &= TYPE4.Initialize(this.ToString(), false);
            if (TYPE5.IsValid) ok &= TYPE5.Initialize(this.ToString(), false);

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
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
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

        #region Methods
        public void SetAllOff()
        {
			if (!Initialized) return;
            if (m_TYPE1.IsValid) m_TYPE1.SetDo(false);
            if (m_TYPE2.IsValid) m_TYPE2.SetDo(false);
            if (m_TYPE3.IsValid) m_TYPE3.SetDo(false);
            if (m_TYPE4.IsValid) m_TYPE4.SetDo(false);
            if (m_TYPE5.IsValid) m_TYPE5.SetDo(false);
        }
        public void SetOBSType(uint type)
        {
			if (!Initialized) return;
            try
            {
                if (type >= 0 && type < MAX_TYPE)
                {
                    int area = (int)(MAX_TYPE - type - 2);
                    if (m_TYPE1.IsValid) m_TYPE1.SetDo(((area >> 0) & 0x01) == 0x01);
                    if (m_TYPE2.IsValid) m_TYPE2.SetDo(((area >> 1) & 0x01) == 0x01);
                    if (m_TYPE3.IsValid) m_TYPE3.SetDo(((area >> 2) & 0x01) == 0x01);
                    if (m_TYPE4.IsValid) m_TYPE4.SetDo(((area >> 3) & 0x01) == 0x01);
                    if (m_TYPE5.IsValid) m_TYPE5.SetDo(((area >> 4) & 0x01) == 0x01);
                }
            }
            catch (Exception Ex)
            {
                ExceptionLog.WriteLog(Ex.ToString());
            }
        }
        public uint GetOBSType()
        {
			if (!Initialized) return 0;
            uint rv = 0;
            try
            {
                if (m_TYPE1.IsValid) rv |= m_TYPE1.IsDetected ? (uint)(0x01 << 0) : 0x00;
                if (m_TYPE2.IsValid) rv |= m_TYPE2.IsDetected ? (uint)(0x01 << 1) : 0x00;
                if (m_TYPE3.IsValid) rv |= m_TYPE3.IsDetected ? (uint)(0x01 << 2) : 0x00;
                if (m_TYPE4.IsValid) rv |= m_TYPE4.IsDetected ? (uint)(0x01 << 3) : 0x00;
                if (m_TYPE5.IsValid) rv |= m_TYPE5.IsDetected ? (uint)(0x01 << 4) : 0x00;
            }
            catch (Exception Ex)
            {
                ExceptionLog.WriteLog(Ex.ToString());
            }
            uint area = (uint)(MAX_TYPE - rv - 2);
            return area;
        }
        public bool GetUSB1()
        {
			if (!Initialized) return false;
            bool rv = false;
            try
            {
                if (m_OBS1.IsValid)
                {
                    m_QueueOBS1.Enqueue(m_OBS1.IsDetected ? 1 : 0);
                    if (m_QueueOBS1.Count > 10) m_QueueOBS1.Dequeue();
                    rv = m_QueueOBS1.Average() == 1 ? true : false;
                }
            }
            catch (Exception Ex)
            {
                ExceptionLog.WriteLog(Ex.ToString());
            }
            return rv;
        }
        public bool GetUSB2()
        {
			if (!Initialized) return false;

            bool rv = false;
            try
            {
                if (m_OBS2.IsValid)
                {
                    m_QueueOBS2.Enqueue(m_OBS2.IsDetected ? 1 : 0);
                    if (m_QueueOBS2.Count > 10) m_QueueOBS2.Dequeue();
                    rv = m_QueueOBS2.Average() == 1 ? true : false;
                }
            }
            catch (Exception Ex)
            {
                ExceptionLog.WriteLog(Ex.ToString());
            }
            return rv;
        }
        public bool GetUSB3()
        {
			if (!Initialized) return false;
            bool rv = false;
            try
            {
                if (m_OBS3.IsValid)
                {
                    m_QueueOBS3.Enqueue(m_OBS3.IsDetected ? 1 : 0);
                    if (m_QueueOBS3.Count > 10) m_QueueOBS3.Dequeue();
                    rv = m_QueueOBS3.Average() == 1 ? true : false;
                }
            }
            catch (Exception Ex)
            {
                ExceptionLog.WriteLog(Ex.ToString());
            }
            return rv;
        }
        public bool GetError()
        {
			if (!Initialized) return false;
            bool rv = false;
            if (m_OBSError.IsValid) 
                rv = m_OBSError.IsDetected;
            return rv;
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

                var helperXml = new XmlHelper<_DevOBS>();
                _DevOBS dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.OBS1 = dev.OBS1;
                    this.OBS2 = dev.OBS2;
                    this.OBS3 = dev.OBS3;
                    this.OBSError = dev.OBSError;
                    this.TYPE1 = dev.TYPE1;
                    this.TYPE2 = dev.TYPE2;
                    this.TYPE3 = dev.TYPE3;
                    this.TYPE4 = dev.TYPE4;
                    this.TYPE5 = dev.TYPE5;
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
                var helperXml = new XmlHelper<_DevOBS>();
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
