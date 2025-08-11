using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Device.ServoControl
{
    [Serializable()]
    public class ServoControlManager
    {
        #region Singleton
        public static readonly ServoControlManager Instance = new ServoControlManager();
        #endregion

        #region Fields
        private string m_FileName;
        private string m_FilePath;
        private bool m_Initialized = false;

        private List<GeneralObject> m_GenDataMotorTorque = new List<GeneralObject>();
        #endregion

        #region Fields(Devices)
        private List<_DevServoUnit> m_DevServoUnits = null;
        private List<ServoUnitTag> m_ServoUnitTags = null;
        #endregion

        #region Properties
        [DeviceSetting(true)]
        public List<_DevServoUnit> DevServoUnits
        {
            get { return m_DevServoUnits; }
            set { m_DevServoUnits = value; }
        }
        [DeviceSetting(true)]
        public List<ServoUnitTag> ServoUnitTags
        {
            get { return m_ServoUnitTags; }
            set { m_ServoUnitTags = value; }
        }
        #endregion

        #region Variables
        [XmlIgnore(), Browsable(false)]
        public List<GeneralObject> GenDataMotorTorque
        {
            get { return m_GenDataMotorTorque; }
            set { m_GenDataMotorTorque = value; }
        }
        #endregion

        #region Constructor
        private ServoControlManager()
        {
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;

            try
            {
                ServoManager.Instance.Initialize();
                TaskServoControl.Instance.Initialize();

                bool rv = true;

                // Device Create
                m_DevServoUnits = new List<_DevServoUnit>();
                m_ServoUnitTags = new List<ServoUnitTag>();
                int index = 0;
                foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
                {
                    ServoUnitTag tag = new ServoUnitTag(servo.ServoName);
                    m_ServoUnitTags.Add(tag);

                    _DevServoUnit dev = new _DevServoUnit(tag);
                    dev.IsValid = true;
                    rv &= dev.Initialize("ServoController", false); // Servo Unit Device는 자동으로 만들자....!, "ServoController" 명칭은 Fix 해야함. Alarm 생성에 사용됨.
                    m_DevServoUnits.Add(dev);

                    // Torque Monitoring
                    foreach (_DevAxis axis in dev.DevAxes)
                    {
                        bool append = true;
                        append &= axis.GetAxis().IsValid;
                        if (append)
                        {
                            string svName = string.Format("TORQUE_{0:00}", index + 1);
                            string description = string.Format("{0}{1} Motor Torque (%)", servo.ToString(), axis.GetAxis().AxisName);
                            m_GenDataMotorTorque.Add(new GeneralObject(svName, description, axis, "GetCurTorque", 1000));
                            index++;
                        }
                    }
                }

                // ServoControl에 등록 되어 있는 각 축에 대해 I/F Sequence를 등록해 두자....!
                InterlockManager.Instance.Initialize();
                m_Initialized = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return m_Initialized;
        }

        public _DevAxis GetDevAxisByName(string axis_name)
        {
            _DevAxis findDev = null;
            try
            {
                for (int i = 0; i < m_DevServoUnits.Count; i++)
                {
                    for (int j = 0; j < m_DevServoUnits[i].DevAxes.Count; j++)
                    {
                        if (m_DevServoUnits[i].DevAxes[j].AxisTag.AxisName == axis_name)
                        {
                            findDev = m_DevServoUnits[i].DevAxes[j];
                            break;
                        }
                    }
                    if (findDev != null) break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return findDev;
        }
        public _DevServoUnit GetDevServoUnitByName(string servo_name)
        {
            _DevServoUnit findDev = null;
            try
            {
                for (int i = 0; i < m_DevServoUnits.Count; i++)
                {
                    if (m_DevServoUnits[i].GetName() == servo_name)
                    {
                        findDev = m_DevServoUnits[i];
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return findDev;
        }
        public List<_DevAxis> GetDevAxes()
        {
            List<_DevAxis> dev_axes = new List<_DevAxis>();
            try
            {
                for (int i = 0; i < m_DevServoUnits.Count; i++)
                {
                    for (int j = 0; j < m_DevServoUnits[i].DevAxes.Count; j++)
                    {
                        dev_axes.Add(m_DevServoUnits[i].DevAxes[j]);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return dev_axes;
        }
        public void Dispose()
        {
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

                var helperXml = new XmlHelper<ServoControlManager>();
                ServoControlManager mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.DevServoUnits = mng.DevServoUnits;
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
            InterlockManager.Instance.WriteXml();

            if (CheckPath())
            {
                if (string.IsNullOrEmpty(m_FileName))
                {
                    string dirName = AppConfig.DefaultConfigFilePath;
                    Directory.CreateDirectory(dirName);

                    m_FilePath = string.Format("{0}\\{1}.xml", dirName, GetDefaultFileName());
                }

                WriteXml(m_FileName);
            }
        }

        public void WriteXml(string fileName)
        {
            try
            {
                ServoManager.Instance.WriteXml();
                var helperXml = new XmlHelper<ServoControlManager>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
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
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
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
