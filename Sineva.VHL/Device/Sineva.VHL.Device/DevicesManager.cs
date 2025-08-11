using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sineva.VHL.Data;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Library;

namespace Sineva.VHL.Device
{
    public class DevicesManager
    {
        #region singleton
        public static readonly DevicesManager Instance = new DevicesManager();
        #endregion

        #region Fields
        private string m_FileName;
        private string m_FilePath;
        private bool m_Initialized = false;
        #endregion

        #region Fields - Devices
        private DevAntiDrop m_DevFrontAntiDrop = new DevAntiDrop();
        private DevAntiDrop m_DevRearAntiDrop = new DevAntiDrop();
        private DevEqPIO m_DevEqpPIO = new DevEqPIO();
        private DevGripperPIO m_DevGripperPIO = new DevGripperPIO();
        private DevFoupGripper m_DevFoupGripper = new DevFoupGripper();
        private DevInterlockBeltCut m_DevInterlockBeltCut = new DevInterlockBeltCut();
        private DevInterlockSwingSensor m_DevInterlockSwingSensor = new DevInterlockSwingSensor();
        private DevInterlockCollision m_DevInterlockCollision = new DevInterlockCollision();
        private DevInterlockEmo m_DevInterlockEmo = new DevInterlockEmo();
        private DevOpPanel m_DevOpPanel = new DevOpPanel();
        private DevSteer m_DevSteer = new DevSteer();
        private DevOutRigger m_DevFrontOutRigger = new DevOutRigger();
        private DevOutRigger m_DevRearOutRigger = new DevOutRigger();
        private DevRFID m_DevRFID = new DevRFID();

        private DevSOS m_DevSOSUpper = new DevSOS();
        private DevOBS m_DevOBSUpper = new DevOBS();
        private DevOBS m_DevOBSLower = new DevOBS();
        private DevOBS m_DevOBSLookDown = new DevOBS();

        private DevTransfer m_DevTransfer = new DevTransfer();
        private DevAutoTeaching m_DevAutoTeaching = new DevAutoTeaching();
        private DevCPS m_DevCPS = new DevCPS();

        private DevCleaner m_DevCleaner = new DevCleaner();
        #endregion

        #region Fields - General Item List
        [NonSerialized]
        private List<GeneralObject> m_GenDataItems = new List<GeneralObject>();
        #endregion

        #region Properties
        [Browsable(false), XmlIgnore()]
        public bool Initialized
        {
            get { return m_Initialized; }
        }
        [DeviceSetting(true)]
        public DevAntiDrop DevFrontAntiDrop
        {
            get { return m_DevFrontAntiDrop; }
            set { m_DevFrontAntiDrop = value; }
        }
        [DeviceSetting(true)]
        public DevAntiDrop DevRearAntiDrop
        {
            get { return m_DevRearAntiDrop; }
            set { m_DevRearAntiDrop = value; }
        }
        [DeviceSetting(true)]
        public DevEqPIO DevEqpPIO
        {
            get { return m_DevEqpPIO; }
            set { m_DevEqpPIO = value; }
        }
        [DeviceSetting(true)]
        public DevGripperPIO DevGripperPIO
        {
            get { return m_DevGripperPIO; }
            set { m_DevGripperPIO = value; }
        }
        [DeviceSetting(true)]
        public DevFoupGripper DevFoupGripper
        {
            get { return m_DevFoupGripper; }
            set { m_DevFoupGripper = value; }
        }

        [DeviceSetting(true)]
        public DevInterlockBeltCut DevInterlockBeltCut
        {
            get { return m_DevInterlockBeltCut; }
            set { m_DevInterlockBeltCut = value; }
        }
        [DeviceSetting(true)]
        public DevInterlockSwingSensor DevInterlockSwingSensor
        {
            get { return m_DevInterlockSwingSensor; }
            set { m_DevInterlockSwingSensor = value; }
        }        
        [DeviceSetting(true)]
        public DevInterlockCollision DevInterlockCollision
        {
            get { return m_DevInterlockCollision; }
            set { m_DevInterlockCollision = value; }
        }
        [DeviceSetting(true)]
        public DevInterlockEmo DevInterlockEmo
        {
            get { return m_DevInterlockEmo; }
            set { m_DevInterlockEmo = value; }
        }
        [DeviceSetting(true)]
        public DevOpPanel DevOpPanel
        {
            get { return m_DevOpPanel; }
            set { m_DevOpPanel = value; }
        }
        [DeviceSetting(true)]
        public DevSteer DevSteer
        {
            get { return m_DevSteer; }
            set { m_DevSteer = value; }
        }
        
        [DeviceSetting(true)]
        public DevSOS DevSOSUpper
        {
            get { return m_DevSOSUpper; }
            set { m_DevSOSUpper = value; }
        }
        [DeviceSetting(true)]
        public DevOBS DevOBSUpper
        {
            get { return m_DevOBSUpper; }
            set { m_DevOBSUpper = value; }
        }
        [DeviceSetting(true)]
        public DevOBS DevOBSLower
        {
            get { return m_DevOBSLower; }
            set { m_DevOBSLower = value; }
        }
        [DeviceSetting(true)]
        public DevOBS DevOBSLookDown
        {
            get { return m_DevOBSLookDown; }
            set { m_DevOBSLookDown = value; }
        }

        [DeviceSetting(true)]
        public DevTransfer DevTransfer
        {
            get { return m_DevTransfer; }
            set { m_DevTransfer = value; }
        }
        [Browsable(false), XmlIgnore()]
        public List<GeneralObject> GenDataItems
        {
            get { return m_GenDataItems; }
        }
        [DeviceSetting(true)]
        public DevOutRigger DevFrontOutRigger
        {
            get { return m_DevFrontOutRigger; }
            set { m_DevFrontOutRigger = value; }
        }
        [DeviceSetting(true)]
        public DevOutRigger DevRearOutRigger
        {
            get { return m_DevRearOutRigger; }
            set { m_DevRearOutRigger = value; }
        }

        [DeviceSetting(true)]
        public DevRFID DevRFID
        {
            get { return m_DevRFID; }
            set { m_DevRFID = value; }
        }
        [DeviceSetting(true)]
        public DevAutoTeaching DevAutoTeaching
        {
            get { return m_DevAutoTeaching; }
            set { m_DevAutoTeaching = value; }
        }
        [DeviceSetting(true)]
        public DevCPS DevCPS
        {
            get { return m_DevCPS; }
            set { m_DevCPS = value; }
        }
        [DeviceSetting(true)]
        public DevCleaner DevCleaner
        {
            get { return m_DevCleaner; }
            set { m_DevCleaner = value; }
        }        
        #endregion

        #region Constructor
        private DevicesManager()
        {
        }
        #endregion

        #region Methods
        public void SeqAbort()
        {
            foreach (PropertyInfo info in XFunc.GetProperties(this))
            {
                _Device dev = info.GetValue(this, null) as _Device;

                bool set_type = false;
                object obj = info.GetCustomAttribute(typeof(DeviceSettingAttribute));
                if (obj != null) set_type = (obj as DeviceSettingAttribute).IsDeviceSettingType ? true : false;
                if (set_type && dev.IsValid) dev.SeqAbort();
            }
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;

            bool rv = true;
            rv &= ServoControlManager.Instance.Initialize();

            List<string> initFailDevices = new List<string>();
            if (m_DevFrontAntiDrop.Initialize("FrontAniDrop") == false) initFailDevices.Add("DevFrontAniDrop");
            if (m_DevRearAntiDrop.Initialize("RearAniDrop") == false) initFailDevices.Add("DevRearAniDrop");

            if (m_DevEqpPIO.Initialize("EqpPIO") == false) initFailDevices.Add("DevEqpPIO");
            if (m_DevGripperPIO.Initialize("GripperPIO") == false) initFailDevices.Add("DevGripperPIO");

            if (m_DevFoupGripper.Initialize("FoupGripper") == false) initFailDevices.Add("DevFoupGripper");
            if (m_DevInterlockBeltCut.Initialize("HoistBeltCut") == false) initFailDevices.Add("DevInterlockBeltCut");
            if (m_DevInterlockSwingSensor.Initialize("HoistSwingSensor") == false) initFailDevices.Add("DevInterlockSwingSensor");
            if (m_DevInterlockCollision.Initialize("BumperCollision") == false) initFailDevices.Add("DevInterlockCollision");
            if (m_DevInterlockEmo.Initialize("Emo") == false) initFailDevices.Add("DevInterlockEmo");

            if (m_DevOpPanel.Initialize("OpPanel") == false) initFailDevices.Add("DevOpPanel");
            if (m_DevSteer.Initialize("Steer") == false) initFailDevices.Add("DevSteer");

            if (DevSOSUpper.Initialize("UpperSOS") == false) initFailDevices.Add("DevSOSUpper");
            if (DevOBSLower.Initialize("LowerOBS") == false) initFailDevices.Add("DevOBSLower");
            if (DevOBSUpper.Initialize("UpperOBS") == false) initFailDevices.Add("DevOBSUpper");
            if (DevOBSLookDown.Initialize("LookDown") == false) initFailDevices.Add("DevOBSLookDown");

            if (DevTransfer.Initialize("Transfer") == false) initFailDevices.Add("DevTransfer");

            if (m_DevFrontOutRigger.Initialize("FrontOutRigger") == false) initFailDevices.Add("DevFrontOutRigger");
            if (m_DevRearOutRigger.Initialize("RearOutRigger") == false) initFailDevices.Add("DevRearOutRigger");

            if (m_DevRFID.Initialize("RFID") == false) initFailDevices.Add("DevRFID");

            if (m_DevAutoTeaching.Initialize("AutoTeaching") == false) initFailDevices.Add("DevAutoTeaching");
            if (m_DevAutoTeaching.IsValid) m_DevAutoTeaching.SetEqpPio(DevEqpPIO);

            if (m_DevCPS.Initialize("CPS") == false) initFailDevices.Add("DevCPS");
            if (m_DevCleaner.Initialize("CLEANER") == false) initFailDevices.Add("DevCleaner");
            rv &= initFailDevices.Count == 0 ? true : false;
            if (rv)
            {
                m_GenDataItems.AddRange(ServoControlManager.Instance.GenDataMotorTorque.ToArray());
            }
            else
            {
                if (initFailDevices.Count > 0)
                {
                    string message = "Initialize Exception Devices : \n";
                    for (int i = 0; i < initFailDevices.Count; i++)
                    {
                        message += string.Format("{0}\n", initFailDevices[i]);
                    }
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show(string.Format("Program Initialize Fail : Indispensable [I/O,Motor] is not assigned \nCheck Exception Log"));
                }
            }
            m_Initialized = true;
            return rv;
        }
        #endregion

        #region [Xml Read/Write]
        public bool ReadXml()
        {
            if (CheckPath())
            {
                return ReadXml(m_FileName);
            }
            else return false;
        }

        public bool ReadXml(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    m_FileName = fileName;
                }
                else
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<DevicesManager>();
                DevicesManager mng = helperXml.Read(fileName);
                if (mng != null)
                {

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public void WriteXml()
        {
            try
            {
                foreach (PropertyInfo info in XFunc.GetProperties(this))
                {
                    object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                    if (attributes == null || attributes.Length <= 0) continue;

                    _Device dev = info.GetValue(this, null) as _Device;
                    dev.WriteXml();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public void WriteXml(string fileName)
        {
            try
            {
                var helperXml = new XmlHelper<DevicesManager>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public bool IsSave()
        {
            bool rv = false;
            try
            {
                foreach (PropertyInfo info in XFunc.GetProperties(this))
                {
                    object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                    if (attributes == null || attributes.Length <= 0) continue;

                    _Device dev = info.GetValue(this, null) as _Device;
                    rv |= dev.SaveCurState;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public void Save()
        {
            try
            {
                foreach (PropertyInfo info in XFunc.GetProperties(this))
                {
                    object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                    if (attributes == null || attributes.Length <= 0) continue;

                    _Device dev = info.GetValue(this, null) as _Device;
                    if (dev.SaveCurState)
                    {
                        dev.WriteXml();
                        dev.SaveCurState = false;   
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public bool CheckPath()
        {
            bool ok = false;

            try
            {
                string filePath = m_FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppConfig.Instance.XmlDevicesPath;
                }

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
                        m_FilePath = filePath;
                        m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
                else
                {
                    m_FilePath = filePath;
                    m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return ok;
        }

        public string GetDefaultFileName()
        {
            string fileName;
            fileName = this.GetType().Name;
            return fileName;
        }
        #endregion

    }
}
