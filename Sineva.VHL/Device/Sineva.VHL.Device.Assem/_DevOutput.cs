using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Device.Assem
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class _DevOutput : _Device
    {
        private const string DevName = "OUTPUT";

        #region Field
        private bool m_IsVirtualDevice = false;
        private bool m_VirtualStatus = false;
        private IoTag m_Do = new IoTag();
        private uint m_DoCheckTime = 30;
        protected AlarmData m_ALM_OnTimeout = null;
        protected AlarmData m_ALM_OffTimeout = null;
        #endregion

        #region Property - Alarm
        [Browsable(false), XmlIgnore()]
        public AlarmData ALM_OnTimeout { get { return m_ALM_OnTimeout; } }
        [Browsable(false), XmlIgnore()]
        public AlarmData ALM_OffTimeout { get { return m_ALM_OffTimeout; } }
        #endregion

        #region Property - I/O
        [Category("!Setting Device (Sensor Check Timeout(sec))"), Description("Timeout(sec)"), DeviceSetting(false, true)]
        public uint DoCheckTime
        {
            get { return m_DoCheckTime; }
            set { m_DoCheckTime = value; }
        }
        [Category("!Setting Device (Digital Output)"), Description(""), DeviceSetting(true)]
        public IoTag Do { get { return m_Do; } set { m_Do = value; } }
        [Category("!Setting Device (Digital Output)"), DisplayName("Virtual I/O"), DeviceSetting(true, false)]
        public bool IsVirtualDevice { get { return m_IsVirtualDevice; } set { m_IsVirtualDevice = value; } }
        #endregion

        #region Property
        [Browsable(false), XmlIgnore()]
        public bool IsDetected
        {
            get
            {
                if (Initialized == false) return false;
                if (m_IsVirtualDevice) return m_VirtualStatus;

                return Do.GetChannel().GetDi();
            }
        }
        #endregion

        #region Constructor
        public _DevOutput()
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
            // 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 2. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            if (m_IsVirtualDevice == false)
            {
                ok &= Do.GetChannel() != null ? true : false;
            }
            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_OnTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "Output ON Timeout");
            m_ALM_OffTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentSafety, heavy_alarm, ParentName, MyName, "Output OFF Timeout");
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            // 4. Device Variable 초기화
            //m_Variable = false;
            if (DoCheckTime == 0) DoCheckTime = 10;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
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

        #region Method
        public bool SetDo(bool flg)
        {
            if (m_IsVirtualDevice)
            {
                m_VirtualStatus = flg;
                return true;
            }
            else
            {
                if (Do.GetChannel() == null) return false;
                Do.GetChannel().SetDo(flg);
                return true;
            }
        }
        #endregion

        #region Sequence
        //private class SeqAction : XSeqFunc
        //{
        //    #region Field
        //    private _DevOutput m_Device = null;
        //    #endregion
        //    public SeqAction(_DevOutput device)
        //    {
        //        this.SeqName = $"SeqAction{device.MyName}";
        //        m_Device = device;
        //        TaskDeviceControl.Instance.RegSeq(this);
        //    }

        //    public override int Do()
        //    {
        //        int seqNo = SeqNo;
        //        int rv = -1;

        //        switch (seqNo)
        //        {
        //            case 0:
        //                {
        //                }
        //                break;
        //            case 100:
        //                {
        //                }
        //                break;
        //        }

        //        SeqNo = seqNo;
        //        return rv;
        //    }
        //}
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

                var helperXml = new XmlHelper<_DevOutput>();
                _DevOutput dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.Do = dev.Do;
                    this.DoCheckTime = dev.DoCheckTime;
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
                var helperXml = new XmlHelper<_DevOutput>();
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
