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
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace Sineva.VHL.Library.Servo
{
	public enum enServoMoveType
	{
		S1,
		S2
	}

	public enum enServoJogDir
	{
		Positive,
		Negative,
	}

	public enum enServoJogMoveType
	{
		Vel,
		Relative
	}

	public struct VelSet
	{
		public enAxisCoord AxisCoord;
        public double Vel;
		public double Acc;
		public double Dec;
        public double Jerk;
    }

    [Serializable]
    [Editor(typeof(UIEditorTeachingPosition), typeof(UITypeEditor))]
    /// <summary>
	/// 1개 Position에 대한 data
	/// </summary>
	public class TeachingData
	{
		#region Fields
        private string m_ServoName = "!";
        private string m_PosName = "!";
        private ushort m_PosId = 0;
		private List<double> m_AxesPosValueArray = new List<double>();
		#endregion

		#region Properties
        public string ServoName
        {
            get { return m_ServoName; }
            set { m_ServoName = value; }
        }
        public string PosName
        {
            get { return m_PosName; }
            set { m_PosName = value; }
        }
        public ushort PosId
		{
			get { return m_PosId; }
			set { m_PosId = value; }
		}
        [Browsable(false), XmlIgnore()]
        public List<double> AxesPosValueArray
		{
			get { return m_AxesPosValueArray; }
			set { m_AxesPosValueArray = value; }
		}
		[Browsable(false), XmlIgnore()]
		public double this[int axisNo]
		{
			get { return m_AxesPosValueArray[axisNo]; }
			set { m_AxesPosValueArray[axisNo] = value; }
		}
		#endregion

		#region Constructor
		public TeachingData()
		{
		}
        public TeachingData(string servoName, string posName, ushort posId, params double[] pos)
		{
            m_ServoName = servoName;
            m_PosName = posName;
            m_PosId = posId;

			AxesPosValueArray.Clear();
			for (int i = 0; i < pos.Length; i++)
			{
				AxesPosValueArray.Add(pos[i]);
			}
		}
		#endregion

        #region Methods
        public override string ToString()
        {
            return string.Format("{0} | {1} | {2}", m_ServoName, m_PosId, m_PosName);
        }
        #endregion
    }

    [Serializable]
    [Editor(typeof(UIEditorTeachingVelocity), typeof(UITypeEditor))]
    public class VelocityData
    {
        #region Fields
        private string m_ServoName = "!";
        private string m_PropName = "!";
        private ushort m_PropId = 0;
        private List<VelSet> m_VelSetList = new List<VelSet>();
        #endregion

        #region Properties
        public string ServoName
        {
            get { return m_ServoName; }
            set { m_ServoName = value; }
        }
        public string PropName
        {
            get { return m_PropName; }
            set { m_PropName = value; }
        }
        public ushort PropId
        {
            get { return m_PropId; }
            set { m_PropId = value; }
        }
        [Browsable(false), XmlIgnore()]
        public List<VelSet> VelSetList
        {
            get { return m_VelSetList; }
            set { m_VelSetList = value; }
        }
        #endregion

        #region Constructor
        public VelocityData(string servoName, string propName, ushort propId, VelSet[] velInfos)
		{
            m_ServoName = servoName;
            m_PropName = propName;
            m_PropId = propId;

			VelSetList.Clear();
			for (int i = 0; i < velInfos.Length; i++)
			{
				VelSetList.Add(velInfos[i]);
			}
		}

		public VelocityData()
		{
		}
        #endregion

        #region Methods
        public void SetTargetVelToAxes(List<_Axis> Axes)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                Axes[i].TargetSpeed = VelSetList[i].Vel;
            }
        }
        public override string ToString()
        {
            return string.Format("{0} | {1} | {2}", m_ServoName, m_PropId, m_PropName);
        }
        #endregion
	}

	[Serializable]
    public class ServoUnit : IServoUnitFactory, IServoUnit
	{
		#region Fields
        protected int m_ServoId;
		protected string m_ServoName = "";
		private string m_ServoDescription = "";
		protected int m_TeachingPointCount;
		protected int m_MovingPropCount;
		protected string[] m_TeachingPointNames = { "NONE" };
		protected string[] m_MovingPropNames = { "NONE" };
		protected List<TeachingData> m_TeachingTable = null;
		protected List<VelocityData> m_MovingPropTable = null;
		protected List<_Axis> m_Axes = new List<_Axis>();
		protected float m_JogSpeed = 0.0F;
		protected float m_JogDistance = 0.0F;
		protected enServoJogMoveType m_JogMoveType = enServoJogMoveType.Vel;
		protected ServoCmdProxy m_CmdPxy = new ServoCmdProxy();
		private readonly static object m_LockKey = new object();

        private bool m_ServoLoggingEnable = false;
        private uint m_ServoLogInterval = 6000;
        #endregion

        #region Properties Servo Command
        [Browsable(false), XmlIgnore()]
		public ServoCmdProxy CmdPxy
		{
			get { return m_CmdPxy; }
			set { m_CmdPxy = value; }
		}
		#endregion

		#region Properties
        [Category("!Servo Info"), ReadOnly(true)]
        public int ServoId
        {
            get { return m_ServoId; }
            set { m_ServoId = value; }
        }
		[Category("!Servo Info")]
		public string ServoName
		{
			get { return m_ServoName; }
			set { m_ServoName = value; }
		}

		[Category("Teaching Info")]
		public string ServoDescription
		{
			get { return m_ServoDescription; }
			set { m_ServoDescription = value; }
		}

		[Category("Teaching Info")]
		public int TeachingPointCount
		{
			get
			{
				m_TeachingPointCount = m_TeachingPointNames.Length;
				return m_TeachingPointCount;
			}
		}

		[Category("Teaching Info")]
		public string[] TeachingPointNames
		{
			get { return m_TeachingPointNames; }
			set { m_TeachingPointNames = value; }
		}

		[Category("Teaching Info")]
		public int MovingPropCount
		{
			get
			{
				m_MovingPropCount = m_MovingPropNames.Length;
				return m_MovingPropCount;
			}
		}

		[Category("Teaching Info")]
		public string[] MovingPropNames
		{
			get { return m_MovingPropNames; }
			set { m_MovingPropNames = value; }
		}

		[Browsable(false)]
		public List<TeachingData> TeachingTable
		{
			get { return m_TeachingTable; }
			set { m_TeachingTable = value; }
		}

		[Browsable(false)]
		public List<VelocityData> MovingPropTable
		{
			get { return m_MovingPropTable; }
			set { m_MovingPropTable = value; }
		}

		[Category("Operating Info")]
		public float JogSpeed
		{
			get { return m_JogSpeed; }
			set { m_JogSpeed = value; }
		}

		[Category("Operating Info")]
		public float JogDistance
		{
			get { return m_JogDistance; }
			set { m_JogDistance = value; }
		}

		[Category("Operating Info")]
		public enServoJogMoveType JogMoveType
		{
			get { return m_JogMoveType; }
			set { m_JogMoveType = value; }
		}

		[Category("!Servo Info")]
		public List<_Axis> Axes
		{
			get { return m_Axes; }
			set { m_Axes = value; }
		}

		[Category("!Servo Info")]
		public int AxisCount 
		{
			get { return this.Axes.Count; }
		}

		[Category("!Servo Status"), Browsable(false), XmlIgnore()]
		public bool HomeComp { get; set; }

		[Category("!Servo Status"), Browsable(false), XmlIgnore()]
		public bool Ready { get; set; }

        [Category("!Servo Status"), Browsable(false), XmlIgnore()]
        public bool Repeat { get; set; }
        [Category("!Logging Config")]
        public bool ServoLoggingEnable
        {
            get { return m_ServoLoggingEnable; }
            set { m_ServoLoggingEnable = value; }
        }
        [Category("!Logging Config"), DisplayName("Logging Interval (ms)")]
        public uint ServoLogInterval
        {
            get { return m_ServoLogInterval; }
            set { m_ServoLogInterval = value; }
        }
        #endregion

		#region Constructor
		//public _ServoUnit()
		//{
		//    //m_TeachingTable = new List<TeachingData>();
		//    //m_MovingPropTable = new List<VelociyData>();
		//}
		#endregion

		#region Methods
		public virtual bool Initialize()
		{
			bool rv = true;
			//foreach (string teachData in m_TeachingPointNames)
			//{
			//    double[] pos = new double[m_Axes.Count];
			//    TeachingTable.Add(new TeachingData(pos));
			//}
			//foreach (string velData in m_MovingPropNames)
			//{
			//    VelSet[] sets = new VelSet[m_Axes.Count];
			//    MovingPropTable.Add(new VelociyData(sets));
			//}
			return rv;
		}
		#endregion

		#region Methods Teaching Data File Read Write
		public virtual bool ReadTeachinDataFromFile()
		{
			// Clear Teaching table
			TeachingTable.Clear();

			string dirName = AppConfig.Instance.XmlServoParameterPath;
			string fileName = string.Format("{0}\\{1}.pos", dirName, this.ServoName);
			if (File.Exists(fileName) == false)
			{
				for (int count = 0; count < this.TeachingPointCount; count++)
				{
					double[] vals = new double[this.AxisCount];
					TeachingData data = new TeachingData(ServoName, TeachingPointNames[count], (ushort)count, vals);
					this.TeachingTable.Add(data);
				}
                SaveTeachingDataToFile();
				return false;
			}
			else
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
                    try
                    {
                        string streamText = sr.ReadToEnd();
                        string[] textLines = streamText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if(textLines.Length != this.TeachingPointCount)
                        {
                            string message = "Servo Unit Teaching Table Error\n";
                            message += string.Format("Allocated Teaching Point Count : {0}\nTeaching Table Count : {1}\n\n", this.TeachingPointCount, textLines.Length);
                            message += string.Format("\nDevice : {0}\nFilePath : {1}", this.ServoName, fileName);
                            MessageBox.Show(message);
                            //return false;
                        }

                        for(int count = 0; count < this.TeachingPointCount; count++)
                        {
                            double[] vals = new double[this.AxisCount];
                            string posName = m_TeachingPointNames[count];
                            vals = FindeTeachingData(textLines, posName);

                            TeachingData data = new TeachingData(ServoName, posName, (ushort)count, vals);
                            this.TeachingTable.Add(data);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Servo Teaching Table File is invalid.");
                        return false;
                    }
				}
			}
			return true;
		}

        private double[] FindeTeachingData(string[] teaching_lists, string find_name)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();

            double[] teaching_data = new double[this.AxisCount];

            try
            {
                for (int i = 0; i < teaching_lists.Length; i++)
                {
                    string[] splitText = teaching_lists[i].Split(new char[] { ',' });
                    if (splitText.Length < 2) continue;
                    if (splitText[1] == find_name)
                    {
                        double temp = 0.0f;
                        int prefixCount = 2;
                        for (int j = prefixCount; j < prefixCount + this.AxisCount; j++)
                        {
                            if (j >= splitText.Length) break;
                            double pos = 0.0f;
                            if (!string.IsNullOrEmpty(splitText[j])) pos = double.TryParse(splitText[j], out temp) ? temp : 0.0f;
                            teaching_data[j - prefixCount] = pos;
                        }
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(method, err.ToString() + this.ToString());
            }
            return teaching_data;
        }

		public virtual bool SaveTeachingDataToFile()
		{
			string dirName = AppConfig.Instance.XmlServoParameterPath;
			string fileName = string.Format("{0}\\{1}.pos", dirName, this.ServoName);

			using (StreamWriter sw = File.CreateText(fileName))
			{
				string writeText = "";
				int id = 0;
				for (int count = 0; count < this.TeachingPointCount; count++)
				{
					writeText += string.Format("{0:D3},", count);
					writeText += this.TeachingPointNames[count];
					writeText += ",";
                    if (count < TeachingTable.Count)
                    {
                        foreach (double val in this.TeachingTable[count].AxesPosValueArray)
                        {
                            writeText += string.Format("{0:F4},", val);
                        }
                    }
					sw.WriteLine(writeText);
					writeText = "";
				}
				sw.Close();
			}
			return true;
		}

		/// <summary>
		/// Format : 
		///	   |		 |Axis1  |Axis2  |Axis3
		/// No |Name	 |V   A D|V   A D|V   A D
		/// 001,InitSpeed,100,1,1,200,1,1,200,1,1
		/// </summary>
		/// <returns></returns>
		public virtual bool ReadVelDataFromFile()
		{
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();

            // Clear Teaching table
            MovingPropTable.Clear();

			string dirName = AppConfig.Instance.XmlServoParameterPath;
			string fileName = string.Format("{0}\\{1}.vel", dirName, this.ServoName);
			if (File.Exists(fileName) == false)
			{
				for (int count = 0; count < this.MovingPropCount; count++)
				{
					VelSet[] velSets = new VelSet[m_Axes.Count];
                    VelocityData data = new VelocityData(ServoName, MovingPropNames[count], (ushort)count, velSets);
					MovingPropTable.Add(data);
				}
				return false;
			}
			else
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					for (int count = 0; count < this.MovingPropCount; count++) // Fast, Slow, Measure, Recovery, ...
					{
						string readText = sr.ReadLine();
						string[] splitText = null;

						string propName = "";
						if (readText != null)
						{
							splitText = readText.Split(new char[] { ',' });
						}
                        if(splitText == null) continue;

                        double temp = 0.0f;
						propName = this.MovingPropNames[count];
						int prefixCount = 2;

                        List<VelSet> VelSetList = new List<VelSet>();
                        for (int i = 0; i < m_Axes.Count; i++) // Axis1, Axis2, ...
						{
                            VelSet set = new VelSet();
							set.AxisCoord = m_Axes[i].AxisCoord;
								 
							for (int j = 0; j < 4; j++) // Vel, Acc, Dec, Jerk
							{
                                if(prefixCount + i * 4 + j >= splitText.Length) break;

								double val = 0.0;

								try
								{
									if (splitText[prefixCount + i * 4 + j] != null)
									{
                                        val = double.TryParse(splitText[prefixCount + i * 4 + j], out temp) ? temp : 0.0f;
									}
								}
								catch (Exception err)
								{
                                    MessageBox.Show(err.ToString() + this.ToString());
                                    ExceptionLog.WriteLog(method, err.ToString() + this.ToString());
								}

								switch (j)
								{
									case 0:
										set.Vel = val;
										break;
									case 1:
										set.Acc = val;
										break;
									case 2:
										set.Dec = val;
										break;
									case 3:
										set.Jerk = val;
										break;
								}
							}
							VelSetList.Add(set);
						}

                        VelocityData data = new VelocityData(ServoName, propName, (ushort)count, VelSetList.ToArray());
						MovingPropTable.Add(data);
					}
				}
			}
			return true;
		}

		public virtual bool SaveVelDataToFile()
		{
			string dirName = AppConfig.Instance.XmlServoParameterPath;
			string fileName = string.Format("{0}\\{1}.vel", dirName, this.ServoName);

			using (StreamWriter sw = File.CreateText(fileName))
			{
				string writeText = "";
				for (int count = 0 ; count < this.MovingPropCount ; count ++)
				{
					writeText += string.Format("{0:D3},", count);
					writeText += this.MovingPropNames[count];
					writeText += ",";
                    if (count < MovingPropTable.Count)
                    {
                        foreach (VelSet val in this.MovingPropTable[count].VelSetList)
                        {
                            writeText += string.Format("{0:F3},{1:F3},{2:F3}, {3:F3}, ", val.Vel, val.Acc, val.Dec, val.Jerk);
                        }
                    }
					sw.WriteLine(writeText);
					writeText = "";
				}
				sw.Close();
			}
			return true;
		}

        public virtual bool ReadSequenceDataToFile()
        {
			try
			{
                string dirName = AppConfig.Instance.XmlServoParameterPath;
                foreach (_Axis axis in m_Axes)
                {
                    string fileName = string.Format("{0}\\{1}_{2}.profile", dirName, this.ServoName, axis.AxisName);
                    if (File.Exists(fileName))
                    {
                        using (StreamReader sr = new StreamReader(fileName))
                        {
                            string streamText = sr.ReadToEnd();
                            string[] textLines = streamText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            int index = 0;
                            if (textLines.Length > 0)
                            {
                                uint temp_uint = 0;
                                ushort temp_ushort = 0;
                                float temp_float = 0.0f;
                                // sensor info
                                string[] splitText = null;
                                splitText = textLines[index++].Split(new char[] { ',' });
                                if (splitText != null && splitText.Length >= 9)
                                {
                                    int index1 = 0;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorUse = uint.TryParse(splitText[index1++], out temp_uint) ? temp_uint : 0;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SlaveNo = uint.TryParse(splitText[index1++], out temp_uint) ? temp_uint : 9;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.Offset = ushort.TryParse(splitText[index1++], out temp_ushort) ? temp_ushort : (ushort)0;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.Size = ushort.TryParse(splitText[index1++], out temp_ushort) ? temp_ushort : (ushort)4;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorTargetValue = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 0.0f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorPositionSetRange = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 0.5f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorPulseToUnit = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 0.1f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorScanDistance = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 0.0f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorScanAcceleration = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 2800.0f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorScanDeceleration = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 2800.0f;
                                    if (splitText.Length >= index1) axis.MotionSensorPara.SensorScanJerk = float.TryParse(splitText[index1++], out temp_float) ? temp_float : 2200.0f;
                                    axis.SequenceCommand.PositionSensorInfo = ObjectCopier.Clone(axis.MotionSensorPara);
                                }
                                // Profile Info
                                axis.SequenceCommand.MotionProfiles.Clear();
                                double temp_double = 0.0f;
                                byte temp_byte = 0;
                                for (int i = 0; i < textLines.Length - 1; i++)
                                {
                                    splitText = textLines[index++].Split(new char[] { ',' });
                                    if (splitText != null && splitText.Length >= 7)
                                    {
                                        MotionProfile prof = new MotionProfile();
										if (splitText.Length >= 0) { int no = byte.TryParse(splitText[0], out temp_byte) ? temp_byte : 0; }
                                        if (splitText.Length >= 1) prof.Distance = double.TryParse(splitText[1], out temp_double) ? temp_double : 0;
                                        if (splitText.Length >= 2) prof.Velocity = double.TryParse(splitText[2], out temp_double) ? temp_double : 2000.0f;
                                        if (splitText.Length >= 3) prof.Acceleration = double.TryParse(splitText[3], out temp_double) ? temp_double : 2800.0f;
                                        if (splitText.Length >= 4) prof.Deceleration = double.TryParse(splitText[4], out temp_double) ? temp_double : 2800.0f;
                                        if (splitText.Length >= 5) prof.Jerk = double.TryParse(splitText[5], out temp_double) ? temp_double : 2200.0f;
                                        if (splitText.Length >= 6) prof.VelocityLimitFlag = byte.TryParse(splitText[6], out temp_byte) ? temp_byte : (byte)0;
                                        axis.SequenceCommand.MotionProfiles.Add(prof);
                                    }
                                }
                            }
                            sr.Close();
                        }
                    }
                }
            }
			catch (Exception ex)
			{
                MessageBox.Show("Servo Read Sequence Data To File is invalid.");
                return false;
            }
            return true;
        }

        public virtual bool SaveSequenceDataToFile()
        {
			try
			{
                string dirName = AppConfig.Instance.XmlServoParameterPath;
                foreach (_Axis axis in m_Axes)
                {
                    string fileName = string.Format("{0}\\{1}_{2}.profile", dirName, this.ServoName, axis.AxisName);
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        string writeText = "";
                        writeText += string.Format("{0:D1},", axis.MotionSensorPara.SensorUse);
                        writeText += string.Format("{0:D2},", axis.MotionSensorPara.SlaveNo);
                        writeText += string.Format("{0:D4},", axis.MotionSensorPara.Offset);
                        writeText += string.Format("{0:D2},", axis.MotionSensorPara.Size);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorTargetValue);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorPositionSetRange);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorPulseToUnit);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorScanDistance);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorScanAcceleration);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorScanDeceleration);
                        writeText += string.Format("{0:F4},", axis.MotionSensorPara.SensorScanJerk);
                        sw.WriteLine(writeText);
                        writeText = "";
                        for (int i = 0; i < axis.SequenceCommand.MotionProfiles.Count; i++)
                        {
                            writeText += string.Format("{0:D4},", i);
                            writeText += string.Format("{0:F4},", axis.SequenceCommand.MotionProfiles[i].Distance);
                            writeText += string.Format("{0:F4},", axis.SequenceCommand.MotionProfiles[i].Velocity);
                            writeText += string.Format("{0:F4},", axis.SequenceCommand.MotionProfiles[i].Acceleration);
                            writeText += string.Format("{0:F4},", axis.SequenceCommand.MotionProfiles[i].Deceleration);
                            writeText += string.Format("{0:F4},", axis.SequenceCommand.MotionProfiles[i].Jerk);
                            writeText += string.Format("{0},", axis.SequenceCommand.MotionProfiles[i].VelocityLimitFlag);
                            sw.WriteLine(writeText);
                            writeText = "";
                        }

                        sw.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Servo Teaching Table File is invalid.");
                return false;
            }
            return true;
        }
        #endregion

        #region Methods Servo Command - 1
        public bool IsTriggerExist()
		{
			lock (m_LockKey)
			{
				var rv = false;
				foreach (_Axis axis in this.Axes)
				{
					rv |= axis.CmdPxy.CmdTrigger;
				}
				return rv;
			}
		}

		public bool IsTriggerExist(_Axis axis)
		{
			lock (m_LockKey)
			{
				var rv = false;
				rv |= axis.CmdPxy.CmdTrigger;
				return rv;
			}
		}

		public void ResetTrigger()
		{
			lock (m_LockKey)
			{
				foreach (_Axis axis in this.Axes)
				{
					axis.CmdPxy.CmdTrigger = false;
				}
			}
		}

		public void ResetTrigger(_Axis axis)
		{
			lock (m_LockKey)
			{
				foreach (_Axis ax in this.Axes)
				{
					if (System.Object.ReferenceEquals(ax, axis))
						axis.CmdPxy.CmdTrigger = false;
				}
			}
		}

		public bool IsCommandExist()
		{
			lock (m_LockKey)
			{
				var rv = false;
				foreach (_Axis axis in this.Axes)
				{
					rv |= axis.CmdPxy.Cmdtype != enAxisOutFlag.CommandNone ? true : false;
				}
				return rv;
			}
		}

		public bool SetCommand(enAxisOutFlag cmd)
		{
			if (IsCommandExist()) 
				return false;

			foreach (_Axis axis in this.Axes)
			{
				axis.CmdPxy.SetCmd(cmd);
			}
			return true;
		}

		public bool SetCommand(enAxisOutFlag cmd, string axisName)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisName == axisName)
				{
					bool move = Convert.ToBoolean(cmd & enAxisOutFlag.MotionStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.RelativeMoveStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.SequenceMotionStart);
					bool home = Convert.ToBoolean(cmd & enAxisOutFlag.HomeStart);

					if (move)
					{
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand({1}) Move Fail, {0} Status Not Ready", axis.AxisName, cmd));
							return false;
						}
						if (!GetHomeEnd(axisName))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand Move Fail, {0} Status Not Home End", axis.AxisName));
							return false;
						}
					}
					if (home)
					{
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand Home Start Fail, {0} Status Not Ready", axis.AxisName));
							return false;
						}
					}

					return axis.CmdPxy.SetCmd(cmd);
				}
			}
			return false;
		}

		public bool SetCommand(enAxisOutFlag cmd, int axisId)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisId == axisId)
				{
                    bool move = Convert.ToBoolean(cmd & enAxisOutFlag.MotionStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.RelativeMoveStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.SequenceMotionStart);
                    bool home = Convert.ToBoolean(cmd & enAxisOutFlag.HomeStart);

                    if (move)
                    {
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand({1}) Move Fail, {0} Status Not Ready", axis.AxisName, cmd));
							return false;
						}
						if (!GetHomeEnd(axis.AxisName))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand Move Fail, {0} Status Not Home End", axis.AxisName));
							return false;
						}
                    }
                    if (home)
					{
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand Home Start Fail, {0} Status Not Ready", axis.AxisName));
							return false;
						}
					}
					return axis.CmdPxy.SetCmd(cmd);
				}
			}
			return false;
		}

		public bool SetCommand(enAxisOutFlag cmd, _Axis axis)
		{
            //Trace.WriteLine(string.Format("Axis - {0}, CMD - {1}", axis.AxisName, cmd.ToString()));
            foreach (_Axis ax in this.Axes)
			{
				if (System.Object.ReferenceEquals(ax, axis))
				{
                    bool move = Convert.ToBoolean(cmd & enAxisOutFlag.MotionStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.RelativeMoveStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.SequenceMotionStart);
                    bool home = Convert.ToBoolean(cmd & enAxisOutFlag.HomeStart);

                    if (move)
					{
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand({1}) Move Fail, {0} Status Not Ready", axis.AxisName, cmd));
							return false;
						}
						if (!GetHomeEnd(axis.AxisName)) return false;
                    }
					if (home)
					{
						if (!GetReady(axis))
						{
							ServoLog.WriteLog(string.Format("{0} SetCommand Home Start Fail, {0} Status Not Ready", axis.AxisName));
							return false;
						}
					}
                    return ax.CmdPxy.SetCmd(cmd);
				}
			}

			return false;
		}
		public bool SetContinuousCommand(enAxisOutFlag cmd, _Axis axis)
		{
            //Trace.WriteLine(string.Format("Axis - {0}, CMD - {1}", axis.AxisName, cmd.ToString()));
            foreach (_Axis ax in this.Axes)
			{
				if (System.Object.ReferenceEquals(ax, axis))
				{
                    bool move = Convert.ToBoolean(cmd & enAxisOutFlag.MotionStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.RelativeMoveStart);
                    move |= Convert.ToBoolean(cmd & enAxisOutFlag.SequenceMotionStart);
                    bool home = Convert.ToBoolean(cmd & enAxisOutFlag.HomeStart);

                    if (move)
					{
						if (!GetContinuousReady(axis)) return false;
                        if (!GetHomeEnd(axis.AxisName)) return false;
                    }
                    if (home)
                    {
                        if (!GetContinuousReady(axis)) return false;
                    }
                    return ax.CmdPxy.SetCmd(cmd);
				}
			}

			return false;
		}
		public enAxisOutFlag GetCommand(_Axis axis)
		{
			enAxisOutFlag cmd = enAxisOutFlag.CommandNone;
            foreach (_Axis ax in this.Axes)
            {
                if (System.Object.ReferenceEquals(ax, axis))
                {
					cmd = ax.CmdPxy.GetCurCmd();
                }
            }
            return cmd;
		}
		public void ResetCommand()
		{
			ResetTrigger();
			foreach (_Axis ax in this.Axes)
			{
				ax.CmdPxy.Cmdtype = enAxisOutFlag.CommandNone;
                (ax as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
			}
		}

		public void ResetCommand(_Axis ax)
		{
			ResetTrigger(ax);
            ax.CmdPxy.Cmdtype = enAxisOutFlag.CommandNone;
			(ax as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
		}

		public bool GetAlarm()
		{
			bool rv = false;
			foreach (IAxisCommand axis in this.Axes)
			{
				rv |= (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? true : false;
			}
			return rv;
		}

		public bool GetAlarm(string axisName)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisName == axisName)
				{
					return ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? true : false;
				}
			}
			throw new Exception(axisName + " was not found");
		}

		public bool GetServoOn()
		{
			bool rv = true;
			foreach (IAxisCommand axis in this.Axes)
			{
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
			}
			return rv;
		}

		public bool GetServoOn(string axisName)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisName == axisName)
				{
					return ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
				}
			}
			throw new Exception(axisName + " was not found");
		}

		public bool GetHomeEnd()
		{
			bool rv = true;
			foreach (IAxisCommand axis in this.Axes)
			{
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
			}
			return rv;
		}

		public bool GetHomeEnd(string axisName)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisName == axisName)
				{
					return ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
				}
			}
			throw new Exception(axisName + " was not found");
		}

		public bool GetReady()
		{
			bool rv = true;

			foreach (IAxisCommand axis in this.Axes)
			{
                if ((axis as _Axis).CommandSkip) continue;
                if ((axis as _Axis).InPosCheckSkip == false)
                    rv &= (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? false : true;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? false : true;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? false : true;
				if (!rv)
				{
					if ((axis as _Axis).InPosCheckSkip == false && ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos)
						ServoLog.WriteLog(string.Format("{0} Status is Not InPos", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
						ServoLog.WriteLog(string.Format("{0} Status is Not Servo On", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
						ServoLog.WriteLog(string.Format("{0} Status is Alarm", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
						ServoLog.WriteLog(string.Format("{0} Status is Limit_N", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
						ServoLog.WriteLog(string.Format("{0} Status is Limit_P", axis));
				}
			}
            return rv;
		}

		public static bool GetReady(List<_Axis> axes)
		{
			bool rv = true;

			foreach (IAxisCommand axis in axes)
			{
                if ((axis as _Axis).CommandSkip) continue;
                if ((axis as _Axis).InPosCheckSkip == false)
                    rv &= (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? false : true;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? false : true;
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? false : true;
				if (!rv)
				{
					if ((axis as _Axis).InPosCheckSkip == false && ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos)
						ServoLog.WriteLog(string.Format("{0} Status is Not InPos", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
						ServoLog.WriteLog(string.Format("{0} Status is Not Servo On", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
						ServoLog.WriteLog(string.Format("{0} Status is Alarm", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
						ServoLog.WriteLog(string.Format("{0} Status is Limit_N", axis));
					if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
						ServoLog.WriteLog(string.Format("{0} Status is Limit_P", axis));
				}
			}
			return rv;
		}

		public bool GetReady(string axisName)
		{
			bool rv = true;

			foreach (_Axis axis in this.Axes)
			{
                if (axis.AxisName == axisName)
				{
                    if ((axis as _Axis).CommandSkip) return rv;
                    if ((axis as _Axis).InPosCheckSkip == false)
                        rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
                    rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                    rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? false : true;
                    rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? false : true;
                    rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? false : true;
					if (!rv)
					{
						if ((axis as _Axis).InPosCheckSkip == false && ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos)
							ServoLog.WriteLog(string.Format("{0} Status is Not InPos", axisName));
						if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
							ServoLog.WriteLog(string.Format("{0} Status is Not Servo On", axisName));
						if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
							ServoLog.WriteLog(string.Format("{0} Status is Alarm", axisName));
						if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
							ServoLog.WriteLog(string.Format("{0} Status is Limit_N", axisName));
						if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
							ServoLog.WriteLog(string.Format("{0} Status is Limit_P", axisName));
					}
					return rv;
				}
			}
			throw new Exception(axisName + " was not found");
		}

		public bool GetReady(_Axis axis)
		{
			bool rv = true;
            if ((axis as _Axis).CommandSkip) return rv;
            if ((axis as _Axis).InPosCheckSkip == false)
                rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? false : true;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? false : true;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? false : true;

			if (!rv)
			{
				if ((axis as _Axis).InPosCheckSkip == false && ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) != enAxisInFlag.InPos)
					ServoLog.WriteLog(string.Format("{0} Status is Not InPos", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
					ServoLog.WriteLog(string.Format("{0} Status is Not Servo On", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
					ServoLog.WriteLog(string.Format("{0} Status is Alarm", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
					ServoLog.WriteLog(string.Format("{0} Status is Limit_N", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
					ServoLog.WriteLog(string.Format("{0} Status is Limit_P", axis.AxisName));
			}

			return rv;
		}
		public bool GetContinuousReady(_Axis axis)
		{
			bool rv = true;
            if ((axis as _Axis).CommandSkip) return rv;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm ? false : true;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? false : true;
            rv &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? false : true;
            
			if (!rv)
			{
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) != enAxisInFlag.SvOn)
					ServoLog.WriteLog(string.Format("{0} Status is Not Servo On", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Alarm) == enAxisInFlag.Alarm)
					ServoLog.WriteLog(string.Format("{0} Status is Alarm", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N)
					ServoLog.WriteLog(string.Format("{0} Status is Limit_N", axis.AxisName));
				if (((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P)
					ServoLog.WriteLog(string.Format("{0} Status is Limit_P", axis.AxisName));
			}
			
			return rv;
		}
		public bool GetInPos()
		{
			bool rv = true;
			foreach (IAxisCommand axis in this.Axes)
			{
                if ((axis as _Axis).InPosCheckSkip == false)
                    rv &= (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
			}
			return rv;
		}

		public bool GetInPos(string axisName)
		{
			foreach (_Axis axis in this.Axes)
			{
				if (axis.AxisName == axisName)
				{
                    if ((axis as _Axis).InPosCheckSkip == false)
                        return ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
				}
			}
			throw new Exception(axisName + " was not found");
		}

		#endregion

        #region Methods interface IServoUnit Command
        public List<_Axis> GetAxes()
        {
            return Axes;
        }

		public string GetName()
		{
			return ServoName;
		}

		public VelSet GetVel(_Axis axis, int prop)
		{
            VelSet set = new VelSet();
            int index = Axes.FindIndex(x=>x.AxisName == axis.AxisName);
			if (index >= 0)
			{
				set = MovingPropTable[prop].VelSetList[index];
            }
			return set;
		}

		public double GetCurAxisPoint(_Axis axis, int point)
		{
			double rv = 0.0;
			int index = Axes.FindIndex(x=>x.AxisName==axis.AxisName);
			if (index >= 0)
			{
                rv = TeachingTable[point].AxesPosValueArray[index];
            }
			return rv;
		}

        public enAxisResult MoveStart(int point, int prop)
        {
			if (!GetReady())
			{
				return enAxisResult.NotReadyError;
			}
			
			enAxisResult rv = enAxisResult.None;
            bool VelCheckOk = true;
			for (int i = 0; i < Axes.Count; i++)
			{
                if (Axes[i].CommandSkip) continue;
				VelCheckOk &= (MovingPropTable[prop].VelSetList[i].Vel > 0.0) ? true : false;
			}
			if (!VelCheckOk)
			{
				return enAxisResult.VelSettingError;
			}

			for (int i = 0; i < Axes.Count; i++)
            {
				rv |= MoveAxisStart(Axes[i], point, prop);
            }

			return rv;
        }
		/// <summary>
		/// posOffset는 상대 위치 이동량
		/// </summary>
		/// <param name="posOffset">Servo Unit 의 모든 축의 offset</param>
		/// <param name="prop"></param>
		/// <returns></returns>
		public enAxisResult MoveRelativeStart(List<double> posOffset, int prop)
		{
			if (!GetReady())
			{
				return enAxisResult.NotReadyError;
			}

			enAxisResult rv = enAxisResult.None;

			bool VelCheckOk = true;
			for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].CommandSkip) continue;
                VelCheckOk &= (MovingPropTable[prop].VelSetList[i].Vel > 0.0) ? true : false;
            }
			if (!VelCheckOk)
			{
                return enAxisResult.VelSettingError;
			}
			if (posOffset.Count < Axes.Count)
			{
				return enAxisResult.PosSettingError;
			}

			for (int i = 0; i < Axes.Count; i++)
			{
				rv |= MoveRelativeStart(Axes[i], posOffset[i], MovingPropTable[prop].VelSetList[i]);
			}
			
			return rv;
		}

		public enAxisResult MoveRelativeStart(int prop, params double[] posOffset)
		{
			if (!GetReady())
			{
				return enAxisResult.NotReadyError;
			}

			enAxisResult rv = enAxisResult.None;

			bool VelCheckOk = true;
			for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].CommandSkip) continue;
                VelCheckOk &= (MovingPropTable[prop].VelSetList[i].Vel > 0.0) ? true : false;
            }
			if (!VelCheckOk)
				return enAxisResult.VelSettingError;
			if (posOffset.Length < Axes.Count)
				return enAxisResult.PosSettingError;

            //int AlmId = 0;
			for (int i = 0; i < Axes.Count; i++)
			{
				rv |= MoveRelativeStart(Axes[i], posOffset[i], MovingPropTable[prop].VelSetList[i]);
			}
			
			return rv;
		}


        // posOffset는 상대 위치 이동량
        public enAxisResult MoveRelativeStart(_Axis axis, double Offset, VelSet set)
        {
            double target = 0.0f;
            target = (axis as IAxisCommand).GetAxisCurPos() + Offset;

			if (!GetReady(axis))
			{
				ServoLog.WriteLog(string.Format("{0} Move Relative Start Fail, {0} Status Not Ready", axis.AxisName));
				return enAxisResult.NotReadyError;
			}
			if (set.Vel <= 0.0 && !axis.CommandSkip) return enAxisResult.VelSettingError;
            if (target < axis.NegLimitPos && axis.NegLimitUse) return enAxisResult.SoftwareLimitNeg;
            if (target > axis.PosLimitPos && axis.PosLimitUse) return enAxisResult.SoftwareLimitPos;
            if (set.Vel > axis.SpeedLimit) return enAxisResult.SoftwareLimitSpeed;

            enAxisResult rv = enAxisResult.None;
            axis.TargetDistance = Math.Round(Offset, 6);
            axis.TargetSpeed = Math.Round(set.Vel, 6);
			axis.TargetAcc = Math.Round(set.Acc, 6);
            axis.TargetDec = Math.Round(set.Dec, 6);
            axis.TargetJerk = Math.Round(set.Jerk, 6);
            if (SetCommand(enAxisOutFlag.RelativeMoveStart, axis))
                rv = enAxisResult.Success;
            else
                rv = enAxisResult.NotReadyError;

            return rv;
        }

        // posOffset는 상대 위치 이동량
        public enAxisResult MoveRelativeStart(_Axis axis, double Offset, int prop)
        {
			enAxisResult rv = enAxisResult.None;
            for (int i = 0; i < Axes.Count; i++ )
                if( Axes[i].AxisName == axis.AxisName )
					rv |= MoveRelativeStart(axis, Offset, MovingPropTable[prop].VelSetList[i]);
               
            return rv;
        }

        public enAxisResult MoveAxisStart(_Axis axis, int point, int prop)
        {
			enAxisResult rv = enAxisResult.None;
			for( int i=0; i<Axes.Count; i++ )
				if (Axes[i].AxisName == axis.AxisName)
					rv |= MoveAxisStart(axis, TeachingTable[point].AxesPosValueArray[i], MovingPropTable[prop].VelSetList[i]);

            return rv;
        }

        public enAxisResult MoveAxisStart(_Axis axis, double pos, VelSet set)
        {
            axis.CmdResult = enAxisResult.None;

			if (!GetReady(axis))
			{
				ServoLog.WriteLog(string.Format("{0} Move Axis Start Fail, {0} Status Not Ready", axis.AxisName));
				return enAxisResult.NotReadyError;
			}
            if (set.Vel <= 0.0 && !axis.CommandSkip) return enAxisResult.VelSettingError;
            if (pos < axis.NegLimitPos && axis.NegLimitUse) return enAxisResult.SoftwareLimitNeg;
            if (pos > axis.PosLimitPos && axis.PosLimitUse) return enAxisResult.SoftwareLimitPos;
            if (set.Vel > axis.SpeedLimit) return enAxisResult.SoftwareLimitSpeed;
			enAxisResult rv = enAxisResult.None;

            axis.TargetPos = Math.Round(pos, 6);
            axis.TargetSpeed = Math.Round(set.Vel, 6);
            axis.TargetAcc = Math.Round(set.Acc, 6);
            axis.TargetDec = Math.Round(set.Dec, 6);
            axis.TargetJerk = Math.Round(set.Jerk, 6);
            if (SetCommand(enAxisOutFlag.MotionStart, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }
        public enAxisResult ContinuousMoveAxisStart(_Axis axis, double pos, VelSet set)
        {
            axis.CmdResult = enAxisResult.None;

            if (!GetContinuousReady(axis))
			{
				ServoLog.WriteLog(string.Format("{0} Continuous Move Axis Start Fail, {0} Status Not Ready", axis.AxisName));
				return enAxisResult.NotReadyError;
			}
            if (set.Vel <= 0.0 && !axis.CommandSkip) return enAxisResult.VelSettingError;
            if (pos < axis.NegLimitPos && axis.NegLimitUse) return enAxisResult.SoftwareLimitNeg;
            if (pos > axis.PosLimitPos && axis.PosLimitUse) return enAxisResult.SoftwareLimitPos;
            if (set.Vel > axis.SpeedLimit) return enAxisResult.SoftwareLimitSpeed;
			enAxisResult rv = enAxisResult.None;

            axis.TargetPos = Math.Round(pos, 6);
            axis.TargetSpeed = Math.Round(set.Vel, 6);
            axis.TargetAcc = Math.Round(set.Acc, 6);
            axis.TargetDec = Math.Round(set.Dec, 6);
            axis.TargetJerk = Math.Round(set.Jerk, 6);
            if (SetContinuousCommand(enAxisOutFlag.MotionStart, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }
        public enAxisResult SequenceMoveAxisStart(_Axis axis, SequenceCommand command)
        {
            axis.CmdResult = enAxisResult.None;

			if (!GetReady(axis))
			{
				ServoLog.WriteLog(string.Format("{0} Sequence Move Axis Start Fail, {0} Status Not Ready", axis.AxisName));
				return enAxisResult.NotReadyError;
			}
            enAxisResult rv = enAxisResult.None;

            axis.SequenceCommand = command;
            if (SetCommand(enAxisOutFlag.SequenceMotionStart, axis))
                rv = enAxisResult.Success;
            else
                rv = enAxisResult.NotReadyError;

            return rv;
        }

        public enAxisResult SetTargetPosition(_Axis axis, int point, int prop)
        {
            enAxisResult rv = enAxisResult.None;
            for(int i = 0; i < Axes.Count; i++)
                if(Axes[i].AxisName == axis.AxisName)
                    rv |= SetTargetPosition(axis, TeachingTable[point].AxesPosValueArray[i], MovingPropTable[prop].VelSetList[i]);
            return rv;
        }

        public enAxisResult SetTargetPosition(_Axis axis, double pos, VelSet set)
        {
            if (pos > axis.PosLimitPos) return enAxisResult.SoftwareLimitPos;
			if (pos < axis.NegLimitPos) return enAxisResult.SoftwareLimitPos;
            if (set.Vel > axis.SpeedLimit) return enAxisResult.SoftwareLimitSpeed;

            enAxisResult rv = enAxisResult.None;
            axis.TargetPos = Math.Round(pos, 6);
            axis.TargetSpeed = Math.Round(set.Vel, 6);
            axis.TargetAcc = Math.Round(set.Acc, 6);
            axis.TargetDec = Math.Round(set.Dec, 6);
            axis.TargetJerk = Math.Round(set.Jerk, 6);
            rv = enAxisResult.Success;
            return rv;
        }
        public enAxisResult MotionDone()
        {		
			enAxisResult rv = enAxisResult.None;

            bool MoveEnd = true;

            foreach (_Axis axis in Axes) 
            {
                if (axis.CommandSkip) continue;
                MoveEnd &= (axis.CmdResult == enAxisResult.Success) ? true : false;
				if (axis.CmdResult > enAxisResult.Success)
				{
					rv = axis.CmdResult;
				}
            }

            if (MoveEnd) rv = enAxisResult.Success;

			return rv;
		}

		public enAxisResult MotionDoneAxis(_Axis axis)
        {
			enAxisResult rv = axis.CmdResult;

			if (rv > enAxisResult.Success)
			{
				rv = axis.CmdResult;
			}
			return rv;
        }
		public int GetStartAlarmId()
		{
			return Axes[0].AlarmStartId;
		}
		public int GetStartAlarmId(_Axis axis)
		{
			return axis.AlarmStartId;
		}

        public enAxisResult JogMove(enAxisOutFlag nCmd, VelSet set)
        {
            enAxisResult rv = enAxisResult.None;
            //if (!GetReady())
            //    return enMpAxisResult.NotReadyError;
            if (set.Vel <= 0.0f) 
				return enAxisResult.VelSettingError;

            foreach (_Axis axis in Axes)
            {
                rv |= JogMove(axis, nCmd, set);
            }
            return rv;
        }

        public enAxisResult JogMove(_Axis axis, enAxisOutFlag nCmd, VelSet set)
        {
            enAxisResult rv = enAxisResult.None;

            bool needMoveN = true;
            needMoveN &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_P) == enAxisInFlag.Limit_P ? true : false;
            needMoveN &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
            needMoveN &= Convert.ToBoolean(nCmd & enAxisOutFlag.JogMinus) ? true : false;

            bool needMoveP = true;
            needMoveP &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.Limit_N) == enAxisInFlag.Limit_N ? true : false;
            needMoveP &= ((axis as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
            needMoveP &= Convert.ToBoolean(nCmd & enAxisOutFlag.JogPlus) ? true : false;

            if (!GetReady(axis.AxisName) && !needMoveN && !needMoveP) 
				return enAxisResult.NotReadyError;
            if (set.Vel <= 0.0f) 
				return enAxisResult.VelSettingError;

            if (axis.CommandSkip) return enAxisResult.Success;

            (axis as IAxisCommand).SetJogSpeedAsync(set);
            (axis as IAxisCommand).SetCommandAsync(nCmd);

            rv = enAxisResult.Success;
            return rv;
        }
        public enAxisResult JogStop(_Axis axis)
        {
            axis.CmdPxy.Cmdtype = enAxisOutFlag.CommandNone;
            (axis as IAxisCommand).SetCommandAsync(enAxisOutFlag.CommandNone);
			Repeat = false;

            return enAxisResult.Success;
        }

		public bool ResetJogSpeed()
		{
			foreach (_Axis axis in Axes)
			{
				ResetJogSpeed(axis);
			}
			return true;
		}
        public bool ResetJogSpeed(_Axis axis)
        {
            VelSet set = new VelSet
            {
                Vel = 0.0f,
                Acc = 0.0f,
                Dec = 0.0f,
                Jerk = 0.0f,
            };
            (axis as IAxisCommand).SetJogSpeedAsync(set);
            return true;
        }

        public bool IsServoReady()
		{
			var rv = true;
			foreach (IAxisCommand axis in Axes)
			{
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) != enAxisInFlag.Alarm ? true : false;
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
				if ((axis as _Axis).InPosCheckSkip == false)
                    rv &= (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
			}
			return rv;
		}
		public bool IsServoReadyLog()
        {
			var rv = true;
			foreach (IAxisCommand axis in Axes)
			{
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.Alarm) != enAxisInFlag.Alarm ? true : false;
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
				rv &= (axis.GetAxisCurStatus() & enAxisInFlag.InPos) == enAxisInFlag.InPos ? true : false;
			}
			return rv ;
		}

        public bool IsServoOn()
        {
			var rv = true;
            foreach (IAxisCommand axis in Axes)
            {
                rv &= (axis.GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
            }
            return rv;
        }

		public int GetCurPoint()
		{
			double dev = 0.0;
			var rv = -1;
			bool checkFlag = false;
			foreach (TeachingData td in TeachingTable)
			{
				checkFlag = true;
				for (int i = 0; i < Axes.Count; i++)
				{
					dev = Math.Abs((Axes[i] as IAxisCommand).GetAxisCurPos() - td.AxesPosValueArray[i]);
					if (dev > 0.001)
						checkFlag &= false;
				}
				if (checkFlag == true)
				{
					return td.PosId;
				}
			}
			return rv;
		}

		public int GetCurPoint(_Axis axis)
		{
			double dev = 0.0;
			var rv = -1;
			bool checkFlag = false;
			foreach (TeachingData td in TeachingTable)
			{
				checkFlag = true;
				for (int i = 0; i < Axes.Count; i++)
				{
					if (Axes[i] == axis)
					{
						dev = Math.Abs((Axes[i] as IAxisCommand).GetAxisCurPos() - td.AxesPosValueArray[i]);
						if (dev > 0.1)
							checkFlag &= false;
					}
				}
				if (checkFlag == true)
				{
					return td.PosId;
				}
			}
			return rv;
		}

        public enAxisResult Home()
        {
            enAxisResult rv = enAxisResult.None;
            if (!GetReady())
			{
				return enAxisResult.NotReadyError;
			}

			for (int i = 0; i < Axes.Count; i++ )
			{
				rv |= Home(Axes[i]);
			}
            return rv;
        }

		public enAxisResult Home(_Axis axis)
        {
            if((axis as _Axis).CommandSkip) return enAxisResult.Success;

            axis.CmdResult = enAxisResult.None;
            if (!GetReady(axis.AxisName))
			{
				return enAxisResult.NotReadyError;
			}

            enAxisResult rv = enAxisResult.None;
            if(SetCommand(enAxisOutFlag.HomeStart, axis))
				rv = enAxisResult.Success;
			else
			{
				rv = enAxisResult.NotReadyError;
			}

            return rv;
        }

        public enAxisResult ServoOn()
        {
            enAxisResult rv = enAxisResult.None;

			for (int i = 0; i < Axes.Count; i++)
            {
				rv |= ServoOn(Axes[i]);
            }
            return rv;
        }

		public enAxisResult ServoOn(_Axis axis)
        {
            if((axis as _Axis).CommandSkip) return enAxisResult.Success;

            enAxisResult rv = enAxisResult.None;

            axis.CmdResult = enAxisResult.None;
			if (SetCommand(enAxisOutFlag.ServoOn, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }

		public enAxisResult ServoOff()
        {
            enAxisResult rv = enAxisResult.None;

			for (int i = 0; i < Axes.Count; i++)
			{
				rv |= ServoOff(Axes[i]);
			}
            return rv;
        }

		public enAxisResult ServoOff(_Axis axis)
        {
			enAxisResult rv = enAxisResult.None;

			axis.CmdResult = enAxisResult.None;
			if (SetCommand(enAxisOutFlag.ServoOff, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }

		public enAxisResult Stop()
        {
            //Trace.WriteLine(string.Format("Axis - {0}, Stop()", this.ServoName));

            enAxisResult rv = enAxisResult.None;
			Repeat = false;
			for (int i = 0; i < Axes.Count; i++)
			{
				rv |= Stop(Axes[i]);
			}

            return rv;
        }

		public enAxisResult Stop(_Axis axis)
        {
            //Trace.WriteLine(string.Format("Axis - {0}, Stop(_Axis axis)", axis.AxisName));

			enAxisResult rv = enAxisResult.None;
            Repeat = false;

            axis.CmdResult = enAxisResult.None;
			if (SetCommand(enAxisOutFlag.MotionStop, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }

		public enAxisResult AlarmClear()
        {
            enAxisResult rv = enAxisResult.None;

			for (int i = 0; i < Axes.Count; i++)
			{
				rv |= AlarmClear(Axes[i]);
			}
            return rv;
        }

		public enAxisResult AlarmClear(_Axis axis)
        {
			enAxisResult rv = enAxisResult.None;

			axis.CmdResult = enAxisResult.None;
			if (SetCommand(enAxisOutFlag.AlarmClear, axis))
				rv = enAxisResult.Success;
			else
				rv = enAxisResult.NotReadyError;

            return rv;
        }

        public enAxisResult ZeroSet()
        {
            enAxisResult rv = enAxisResult.None;

            for (int i = 0; i < Axes.Count; i++)
            {
                rv |= ZeroSet(Axes[i]);
            }
            return rv;
        }

        public enAxisResult ZeroSet(_Axis axis)
        {
            enAxisResult rv = enAxisResult.None;

            axis.CmdResult = enAxisResult.None;
            if (SetCommand(enAxisOutFlag.ZeroSet, axis))
                rv = enAxisResult.Success;
            else
                rv = enAxisResult.NotReadyError;

            return rv;
        }
        #endregion

        #region Methods
        public static List<_Axis> GetAxisTypes()
		{
			List<_Axis> axisTypes = new List<_Axis>();
			System.Reflection.Assembly asm = System.Reflection.Assembly.Load((typeof(ServoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (XFunc.CheckTypeCompatibility(typeof(_Axis), type, Compatibility.Compatible))
				{
					if (type.IsAbstract == false)
					{
						_Axis axis = Activator.CreateInstance(type) as _Axis;
						axisTypes.Add(axis);
					}
				}
			}
			return axisTypes;
		}

		public ServoUnit CreateObject()
		{
			return Activator.CreateInstance(this.GetType()) as ServoUnit;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(ServoName))
			{
				return base.ToString();
			}
			else
			{
				return string.Format("{0}-{1}", m_ServoName, m_ServoDescription);
			}
		}
		#endregion

	}
}
