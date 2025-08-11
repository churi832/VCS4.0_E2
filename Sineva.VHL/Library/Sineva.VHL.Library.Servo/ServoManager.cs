/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using Sineva.VHL.Library;

namespace Sineva.VHL.Library.Servo
{
	#region XmlInclude
	[XmlInclude(typeof(_Axis))]
	[XmlInclude(typeof(MpAxis))]
	[XmlInclude(typeof(_AxisBlock))]
    [XmlInclude(typeof(AxisBlockMp2100))]
	[XmlInclude(typeof(AxisBlockACS))]
    [XmlInclude(typeof(AxisBlockAXT))]
    [XmlInclude(typeof(AxisBlockMXP))]
	[XmlInclude(typeof(ServoUnit))]
	[XmlInclude(typeof(TeachingData))]
	[XmlInclude(typeof(VelocityData))]
	[XmlInclude(typeof(VelSet))]
	[XmlInclude(typeof(ServoManager))]
	#endregion

	[Serializable()]
	public class ServoManager
	{
		public static readonly ServoManager Instance = new ServoManager();
		public static readonly object LockKey = new object();
        
		#region Fields
		private string m_FileName;
		private string m_FilePath;
		private bool m_Initialized = false;
        private bool m_HasServoMotor = false;
		private List<_AxisBlock> m_AxisBlocks = new List<_AxisBlock>();
		private List<ServoUnit> m_ServoUnits = new List<ServoUnit>();
		private static List<_Axis> m_AxisSource = new List<_Axis>();

        private Thread m_ThreadLogger = null;
        private bool m_UpdateRun = false;
        #endregion

        #region Properties
        public bool HasServoMotor
        {
            get { return m_HasServoMotor; }
            set { m_HasServoMotor = value; }
        }
        [Browsable(false)]
		public List<_AxisBlock> AxisBlocks
		{
			get { return m_AxisBlocks; }
			set { m_AxisBlocks = value; }
		}
		public List<ServoUnit> ServoUnits
		{
			get { return m_ServoUnits; }
			set { m_ServoUnits = value; }
		}

		[XmlIgnore(), Browsable(false)]
		public ServoUnit this[int index]
		{
			get
			{
				if (m_ServoUnits.Count > index)
					return m_ServoUnits[index];
				else 
					return null;
			}
		}

		[XmlIgnore(), Browsable(false)]
		public ServoUnit this[string name]
		{
			get
			{
				for (int i = 0; i < m_ServoUnits.Count; i++)
				{
					if (m_ServoUnits[i].ServoName == name)
						return m_ServoUnits[i];
				}
				return null;
			}
		}

		[XmlIgnore(), Browsable(false)]
		public List<_Axis> AxisSource
		{
			get { return m_AxisSource; }
			set { m_AxisSource = value; }
		}
		[XmlIgnore(), Browsable(false)]
		public bool Initialized
		{
			get { return m_Initialized; }
		}
        [Browsable(false), XmlIgnore()]
        public bool UpdateRun
        {
            get { return m_UpdateRun; }
            set { m_UpdateRun = value; }
        }
        #endregion

        #region Constructor
        private ServoManager()
		{
		}
		#endregion

		#region Methods
		public bool Initialize(bool thread_create = true)
		{
            if (m_Initialized) return true;

			bool rv = true;
			rv &= this.ReadXml();

			foreach (ServoUnit unit in this.ServoUnits)
			{
				rv &= unit.Initialize();
			}

			if (m_Initialized == false)
			{
				m_AxisSource.Clear();
				foreach (_AxisBlock block in this.m_AxisBlocks)
				{
					foreach (_Axis axis in block.Axes)
					{
						if (!axis.IsValid) continue;
                        if (CheckAxisNameDuplicated(axis))
                        {
                            if (axis.CheckedItem != null) axis.CheckedItem.Init(axis);
                            m_AxisSource.Add(axis);
                        }
					}
				}

				foreach (ServoUnit unit in this.ServoUnits)
				{
					for (int i = 0; i < unit.Axes.Count; i++)
					{
                        if (unit.Axes[i] != null)
                        {
                            int targetId = unit.Axes[i].AxisId;
                            unit.Axes.RemoveAt(i);
							_Axis axis = GetAxisById(targetId);
							if (axis != null) unit.Axes.Insert(i, axis); // target aixs가 
							else i--; // axis가 삭제 되었으니 i도 빼야겠지
                        }
					}
				}
			}

			// Teaching Data Reading 시점 변경 hjyou
			foreach (ServoUnit unit in this.ServoUnits)
			{
				unit.ReadTeachinDataFromFile();
				unit.ReadVelDataFromFile();
				unit.ReadSequenceDataToFile();
                m_HasServoMotor = true;
			}

            if(m_ThreadLogger == null && thread_create)
            {
                m_ThreadLogger = new Thread(new ThreadStart(ThreadCallbackLogging));
                m_ThreadLogger.IsBackground = true;
                m_ThreadLogger.Start();
            }

            m_Initialized = rv;
			return rv;
		}

		public _Axis GetAxisById(int id)
		{
			if (id >= 0 && id < m_AxisSource.Count)
				return m_AxisSource[id];
			else 
				return null;
		}

        public _Axis GetAxisByName(string name)
        {
            foreach (_Axis axis in m_AxisSource)
            {
                if (name == axis.AxisName) return axis;
            }
            return null;
        }
        public bool CheckAxisNameDuplicated(_Axis axis)
        {
            if (axis.AxisName.Contains("HomeOffset")) return true;
            if (axis.AxisName.Contains("Spare")) return true;
            int count = 0;
            for (int i = 0; i < m_AxisSource.Count; i++)
            {
                if (m_AxisSource[i].AxisName.Contains("HomeOffset")) continue; //이런건 중복 check 하지 말자
                if (m_AxisSource[i].AxisName.Contains("Spare")) continue;
                if (axis.AxisName == m_AxisSource[i].AxisName) count++;
            }
            if (count > 1) EventHandlerManager.Instance.InvokeConfigErrorHappened(string.Format("There is Duplicate Axis 'Axis Name = {0}, count={1}'",axis.AxisName, count));
            return count < 2;
        }

        public ServoUnit GetServoByName(string name)
        {
            ServoUnit servo = null;
            foreach (ServoUnit unit in this.ServoUnits)
            {
                if (name == unit.ServoName)
                {
                    servo = unit;
                    break;
                }
            }
            return servo;
        }

		public static List<ServoUnit> GetUnitTypes()
		{
			List<ServoUnit> servoTypes = new List<ServoUnit>();
			Assembly asm = Assembly.Load((typeof(ServoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (XFunc.CheckTypeCompatibility(typeof(ServoUnit), type, Compatibility.Compatible))
				{
					if (type.IsAbstract == false)
					{
						ServoUnit unit = Activator.CreateInstance(type) as ServoUnit;
						servoTypes.Add(unit);
					}
				}
			}
			return servoTypes;
		}

		public static List<_AxisBlock> GetBlockTypes()
		{
			List<_AxisBlock> blockTypes = new List<_AxisBlock>();
			Assembly asm = Assembly.Load((typeof(ServoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if(XFunc.CheckTypeCompatibility(typeof(_AxisBlock), type, Compatibility.Compatible))
				{
					if (type.IsAbstract == false)
					{
						_AxisBlock node = Activator.CreateInstance(type) as _AxisBlock;
						blockTypes.Add(node);
					}
				}
			}

			return blockTypes;
		}
		#endregion

		#region [Xml R/W]
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
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					m_FileName = fileName;
				}
				else
				{
					// to make Sample templete
					WriteXml();
				}

				using (StreamReader sr = new StreamReader(fileName))
				{
					XmlSerializer xmlSer = new XmlSerializer(typeof(ServoManager));
					ServoManager mng = new ServoManager();
					mng = xmlSer.Deserialize(sr) as ServoManager;
					sr.Close();
					m_ServoUnits = mng.ServoUnits;
					m_AxisBlocks = mng.AxisBlocks;
				}

			}
			catch (Exception err)
			{
                MessageBox.Show(err.ToString() + this.ToString());
				ExceptionLog.WriteLog(method, err.ToString());
			}

			return true;
		}

		public void WriteXml()
		{
			if (string.IsNullOrEmpty(m_FileName))
			{
				string dirName = AppConfig.DefaultConfigFilePath;
				Directory.CreateDirectory(dirName);

				m_FilePath = string.Format("{0}\\{1}.xml", dirName, GetDefaultFileName());
			}

			WriteXml(m_FileName);
		}

		public void WriteXml(string fileName)
		{
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            StreamWriter sw = null;
			XmlSerializer xmlSer = new XmlSerializer(this.GetType());

			try
			{
				sw = new StreamWriter(fileName + ".try");
				xmlSer.Serialize(sw, this);
				sw.Close();
				FileInfo file = new FileInfo(fileName + ".try");
				file.Delete();
			}
			catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
			{
				System.Windows.Forms.MessageBox.Show(err.ToString());
				ExceptionLog.WriteLog(method, err.ToString());
				if (sw != null) sw.Close();

				return;
			}

			try
			{
				FileInfo file = new FileInfo(fileName);
				if (file.Exists)
				{
					file.CopyTo(fileName + ".old", true);
				}

                if (m_ServoUnits != null) RedefineServoId();
                if (m_AxisBlocks != null) RedefineAxisId();

				sw = new StreamWriter(fileName);
				xmlSer.Serialize(sw, this);
				sw.Close();

				m_FilePath = fileName;
			}
			catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
			{
				System.Windows.Forms.MessageBox.Show(err.ToString());
				ExceptionLog.WriteLog(method, err.ToString());
			}
		}

        public void RedefineServoId()
        {
            int index0 = 0;
            foreach (ServoUnit servo in this.ServoUnits)
            {
                servo.ServoId = index0++;
            }
        }

        public void RedefineAxisId()
        {
			try
			{
                int index0 = 0;
                int index1 = 0;
                foreach (_AxisBlock block in this.m_AxisBlocks)
                {
                    block.BlockId = index0++;
                    block.StartAxisId = index1;
                    int node_id = 0;
                    foreach (_Axis axis in block.Axes)
                    {
                        axis.NodeId = node_id++;
                        axis.AxisId = index1++;
						if (axis.MXConfigratorSlaveNo == -1) axis.MXConfigratorSlaveNo = axis.NodeId;
                        if (axis.SlaveNodeId == -1) axis.SlaveNodeId = axis.NodeId;
                        if (axis.MasterNodeId == -1) axis.MasterNodeId = axis.NodeId;
                        //axis.AlarmStartId = 1000 + 20 * axis.AxisId;
                        int findtext = axis.AxisName.IndexOf("Spare");
                        if (findtext >= 0)
                        {
                            axis.IsValid = false;
                            axis.AxisName = string.Format("Mp{0}Spare{1}", block.BlockId, axis.AxisId);
                        }
                    }
                }
            }
            catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(method, err.ToString());
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
                    filePath = AppConfig.Instance.XmlAxisBlockPath;
                }

                if (Directory.Exists(filePath) == false)
                {
                    MessageBox.Show("Servo Setting File Not Found");
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.Description = "ServoManager.xml file find";
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
            catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(method, err.ToString());
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

        private void ThreadCallbackLogging()
        {
			while (true)
			{
                if (AppConfig.AppMainDisposed) break;
                try
                {
					if (AppConfig.AppMainInitiated)
					{
                        string log = string.Empty;
                        for (int i = 0; i < m_AxisBlocks.Count; i++)
                        {
                            log += string.Format("[Controller:{0}],", m_AxisBlocks[i].BlockStateMsg);
                        }
                        for (int i = 0; i < m_ServoUnits.Count; i++)
                        {
                            if (m_ServoUnits[i].ServoLoggingEnable && m_ServoUnits[i].IsServoReadyLog() == false) // Servo가 준비되지 않은 경우에만 Log를 기록하자 ~~~
                            {
                                foreach (_Axis axis in m_ServoUnits[i].Axes)
                                {
                                    IAxisCommand axisCmd = (axis as IAxisCommand);
                                    log += string.Format("{0}:{1}\t{2:F1}\t{3:F1}\t{4:F1}\t{5}\t\t",
                                        string.Format("{0}_{1}", m_ServoUnits[i].ServoName, axis.AxisName),
                                        axisCmd.GetAxisCurStatus(), axisCmd.GetAxisCurSpeed(), axisCmd.GetAxisCurPos(), axisCmd.GetAxisCurTorque(), axis.AxisStateMsg);
                                }
                            }
                        }
                        ServoAxisLog.WriteLog(log);
                    }
                }
				catch (Exception ex)
				{
					ExceptionLog.WriteLog(ex.ToString());
				}
				finally
				{
                    Thread.Sleep(200);
                }
            }
            if (m_ThreadLogger != null) m_ThreadLogger.Abort();
        }
	}
}
