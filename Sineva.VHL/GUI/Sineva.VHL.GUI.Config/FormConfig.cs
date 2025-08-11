using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.MXP;
using Sineva.VHL.Device;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Device.ServoControl;

namespace Sineva.VHL.GUI.Config
{
    public partial class FormConfig : Form, IFormUpdate
    {
        #region Fields
        private AppConfig m_AppConfig;
        private ServoManager m_ServoFactory;
        private IoManager m_IoFactory;
        private FormNotifyConfigError m_DlgNotifyError = null;

        private bool m_UpdateNeed = false;
        private bool m_IoUpdate = false;
        private bool m_ServoUpdate = false;
        #endregion

        #region Properties
        public bool UpdateNeed
        {
            get { return m_UpdateNeed; }
            set { m_UpdateNeed = value; }
        }
        #endregion

        #region Constructor
        public FormConfig()
        {
            InitializeComponent();

            EventHandlerManager.Instance.NotifyConfigErrorMessage += Instance_NotifyErrorMessage;
            // AppConfig File Name Setting ///////////////////////////////////////
            m_AppConfig = AppConfig.Instance;
            this.propertyGridAppConfig.SelectedObject = m_AppConfig;

            m_ServoFactory = ServoManager.Instance;
            m_IoFactory = IoManager.Instance;

            m_ServoFactory.Initialize();
            m_IoFactory.Initialize();

            DatabaseHandler.Instance.Initialize();
            AlarmListProvider.Instance.LoadFromDb();

            this.ucConfigServo1.Initialize(m_ServoFactory);
            this.ucConfigIo1.Initialize(m_IoFactory);
            this.ucConfigDevices1.Initialize();

            TaskHandler.Instance.InitTaskHandler();
            AppConfig.AppMainInitiated = true;
        }
        #endregion

        #region delegate
        private void Instance_NotifyErrorMessage(string val)
        {
            if (this.InvokeRequired)
            {
                DelVoid_String d = new DelVoid_String(Instance_NotifyErrorMessage);
                this.Invoke(d, val);
            }
            else
            {
                if (m_DlgNotifyError == null || m_DlgNotifyError.IsDisposed)
                    m_DlgNotifyError = new FormNotifyConfigError();

                m_DlgNotifyError.ShowError(val);
            }
        }
        #endregion

        #region IFormUpdate
        public void KillTimer()
        {
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_IoUpdate)
                viewIoEdit1.UpdateState();
            if (m_ServoUpdate)
                servoControlView1.UpdateState();
        }

        #region Methods
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                m_AppConfig.WriteXml();
                ucConfigServo1.Save();
                ucConfigIo1.Save();
                this.ucConfigDevices1.Save();

                MakeDefineAlarmFile();
                MakeDefineIoFile();
                MakeDefineServoFile();

                MessageBox.Show("Config Save OK ！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config Save Fail\nError : " + ex.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void iOInitializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MxpManager.Instance.Initialize();

            tabControl.SelectTab(tabPageIOTest);
            this.viewIoEdit1.Initialize(IoManager.Instance, (IFormUpdate)this, true);
        }
        private void iORunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabPageIOTest);
            if (!m_IoUpdate)
            {
                iORunToolStripMenuItem.Checked = true;
                iOStopToolStripMenuItem.Checked = false;
                m_UpdateNeed = true;
                m_IoUpdate = true;
            }
        }
        private void iOStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabPageIOTest);
            if (m_IoUpdate)
            {
                iORunToolStripMenuItem.Checked = false;
                iOStopToolStripMenuItem.Checked = true;
                m_UpdateNeed = false;
                m_IoUpdate = false;
            }
        }

        private void servoInitializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MxpManager.Instance.Initialize();

            tabControl.SelectTab(tabPageServoTest);
            servoControlView1.Initialize();
        }

        private void servoRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabPageServoTest);
            if (!m_ServoUpdate)
            {
                servoRunToolStripMenuItem.Checked = true;
                servoStopToolStripMenuItem.Checked = false;
                m_UpdateNeed = true;
                m_ServoUpdate = true;
            }
        }

        private void servoStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabPageServoTest);
            if (m_ServoUpdate)
            {
                servoRunToolStripMenuItem.Checked = false;
                servoStopToolStripMenuItem.Checked = true;
                m_UpdateNeed = false;
                m_ServoUpdate = false;
            }
        }
        #endregion

        #region CodeDom
        private void MakeDefineAlarmFile()
        {
            string ClassName = string.Format("DefineAlarm");
            string FilePathName = Application.StartupPath + string.Format("\\AppDefine\\{0}.cs", ClassName);
            CodeDomDefineAlarm code = new CodeDomDefineAlarm(FilePathName, ClassName);

            AlarmListProvider.Instance.LoadFromDb();
            Dictionary<int, AlarmData> dictionary_alarms = AlarmListProvider.Instance.GetAlarm();
            foreach (AlarmData data in dictionary_alarms.Values)
            {
                code.AddInt32Fields(data.Name, data.ID);
            }

            
            code.Write();
        }
        private void MakeDefineIoFile()
        {
            string ClassName = string.Format("DefineIo");
            string FilePathName = Application.StartupPath + string.Format("\\AppDefine\\{0}.cs", ClassName);
            CodeDomDefineIo code = new CodeDomDefineIo(FilePathName, ClassName);
            code.AddConstructor();
            code.AddSingleTonFields(ClassName);

            foreach (_IoNode node in IoManager.Instance.Nodes)
            {
                if (node.Terminals == null) continue;
                foreach (_IoTerminal term in node.Terminals)
                {
                    foreach (IoChannel ch in term.Channels)
                    {
                        string name = ch.Name;
                        if (name == "di__" || name == "diSPARE" ||
                           name == "do__" || name == "doSPARE" ||
                           name == "ai__" || name == "aiSPARE" ||
                           name == "ao__" || name == "aoSPARE")
                            name += "_" + ch.WiringNo.ToString();
                        code.AddProperties(name, ch.IoType, ch.Id, ch.WiringNo);
                    }
                }
            }
            code.Write();
        }
        private void MakeDefineServoFile()
        {
            string ClassName = string.Format("DefineServo");
            string FilePathName = Application.StartupPath + string.Format("\\AppDefine\\{0}.cs", ClassName);
            CodeDomDefineServoUnit code = new CodeDomDefineServoUnit(FilePathName, ClassName);
            code.AddConstructor();
            code.AddSingleTonFields(ClassName);

            ServoControlManager.Instance.DevServoUnits.Clear();
            foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
            {
                _DevServoUnit dev = new _DevServoUnit(new ServoUnitTag(servo.ServoName));
                dev.Initialize("ServoController", false);
                ServoControlManager.Instance.DevServoUnits.Add(dev);
            }

            int ServoId = 0;
            int No = 0;
            foreach (_DevServoUnit dev in ServoControlManager.Instance.DevServoUnits)
            {
                string servoName = string.Format("{0}", dev.ServoTag.ServoName);
                code.AddServoUnitFields(servoName, ServoId);
                if (dev.DevAxes.Count <= 0) continue;

                foreach (_DevAxis axis in dev.DevAxes)
                {
                    string axisName = string.Format("{0}_{1}", dev.ServoTag.ServoName, axis.AxisTag.AxisName);
                    code.AddAxisFields(axisName, No, ServoId);
                    No++;
                }
                No = 0;

                foreach (string point in dev.GetServoUnit().TeachingPointNames)
                {
                    string pointName = string.Format("{0}_Pos_{1}", dev.GetServoUnit().ServoName, point);
                    code.AddFields(pointName, No);
                    No++;
                }
                No = 0;

                foreach (string prop in dev.GetServoUnit().MovingPropNames)
                {
                    string pointName = string.Format("{0}_Vel_{1}", dev.GetServoUnit().ServoName, prop);
                    code.AddFields(pointName, No);
                    No++;
                }
                No = 0;

                ServoId++;
            }

            code.Write();
        }

        #endregion
    }
}
