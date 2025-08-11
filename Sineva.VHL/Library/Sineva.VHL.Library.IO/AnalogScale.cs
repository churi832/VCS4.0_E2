using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.IO
{
    public enum ScalePreset
    {
        Common,
        DP102A,
    }

    [Serializable]
    public class AnalogScale
    {
        #region Field
        private string m_Name = string.Empty;
        private bool m_UseScale = false;
        private UnitType m_Unit = UnitType.None;
        private int m_AdcMin = 0;
        private int m_AdcMax = 0;
        private double m_RealMin = 0;
        private double m_RealMax = 0;
        private int m_DecimalPoint = 5;

        private double m_Offset = 0;    // Zero Offset
        #endregion

        #region Property
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public bool UseScale
        {
            get { return m_UseScale; }
            set { m_UseScale = value; }
        }
        public UnitType Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        public int AdcMin
        {
            get { return m_AdcMin; }
            set { m_AdcMin = value; }
        }
        public int AdcMax
        {
            get { return m_AdcMax; }
            set { m_AdcMax = value; }
        }
        public double RealMin
        {
            get { return m_RealMin; }
            set { m_RealMin = value; }
        }
        public double RealMax
        {
            get { return m_RealMax; }
            set { m_RealMax = value; }
        }
        public int DecimalPoint
        {
            get { return m_DecimalPoint; }
            set { m_DecimalPoint = value; }
        }
        public double Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        [XmlIgnore()]
        public double CurAdc { get; set; }
        [XmlIgnore()]
        public double CalValue
        {
            get
            {
                if(m_UseScale == false) return CurAdc;
                double diffReal = m_RealMax - m_RealMin;
                double diffAdc = m_AdcMax - m_AdcMin;
                if(diffAdc == 0) return CurAdc;
                else return Math.Round((diffReal / diffAdc) * (CurAdc - m_AdcMin) + m_RealMin - m_Offset, m_DecimalPoint);
            }
        }
        #endregion

        #region Constructor
        public AnalogScale()
        {
        }
        public AnalogScale(string name)
        {
            this.Name = name;
        }
        public AnalogScale(ScalePreset preset)
        {
        }
        #endregion

        public double GetCalValue()
        {
            if(UseScale == false) return CurAdc;

            double diffReal = m_RealMax - m_RealMin;
            double diffAdc = m_AdcMax - m_AdcMin;

            if(diffAdc == 0) 
                return CurAdc;
            else
                return Math.Round((diffReal / diffAdc) * (CurAdc - m_AdcMin) + m_RealMin - m_Offset, m_DecimalPoint);
        }
        public AnalogScale GetPreset(ScalePreset preset)
        {
            AnalogScale rv = new AnalogScale();
            switch(preset)
            {
            case ScalePreset.DP102A:
                {
                    rv.AdcMax = 32760;
                    rv.AdcMin = 404;
                    rv.RealMax = 0.78;
                    rv.RealMin = 0;
                    rv.Unit = UnitType.MPa;
                }
                break;
            default:
                {
                    rv.AdcMax = 4095;
                    rv.AdcMin = 0;
                    rv.RealMax = 100;
                    rv.RealMin = 0;
                    rv.Unit = UnitType.None;
                }
                break;
            }
            return rv;
        }
        public void SetZero()
        {
            if(m_UseScale)
            {
                m_Offset += GetCalValue();
                AnalogScaleManager.Instance.UpdateScale(this);
            }
        }
    }

    public class AnalogScaleManager
    {
        #region Field
        private static bool m_Initiated = false;
        private SortedList<string, AnalogScale> m_Scales = new SortedList<string, AnalogScale>();

        private string m_FileName = "";
        private string m_FilePath = "";

        private Queue<bool> m_QueueUpdateEvent = new Queue<bool>();
        #endregion

        #region Property
        [System.Xml.Serialization.XmlIgnore()]
        public SortedList<string, AnalogScale> Scales
        {
            get { return m_Scales; }
            set { m_Scales = value; }
        }
        [System.Xml.Serialization.XmlArray("Scales"), System.ComponentModel.Browsable(false)]
        public SerializeArrayObject<AnalogScale>[] _ScalesArray
        {
            get
            {
                List<SerializeArrayObject<AnalogScale>> values = new List<SerializeArrayObject<AnalogScale>>();
                for(int i = 0; i < m_Scales.Values.Count; i++)
                {
                    SerializeArrayObject<AnalogScale> item = new SerializeArrayObject<AnalogScale>();
                    item.Id = i;
                    item.Object = m_Scales.Values[i];
                    values.Add(item);
                }
                return values.ToArray();
            }
            set
            {
                m_Scales.Clear();
                for(int i = 0; i < value.Length; i++)
                {
                    string name = value[i].Object.Name;
                    if(m_Scales.ContainsKey(name) == false)
                        m_Scales.Add(name, value[i].Object);
                }
            }
        }
        internal Queue<bool> QueueUpdateEvent
        {
            get { return m_QueueUpdateEvent; }
            set { m_QueueUpdateEvent = value; }
        }
        #endregion

        #region Singleton
        public static AnalogScaleManager Instance { get { return Nested.instance; } }
        class Nested
        {
            static Nested()
            {
            }
            internal static readonly AnalogScaleManager instance = new AnalogScaleManager();
        }
        public AnalogScaleManager()
        {
            if(!m_Initiated)
            {
                m_Initiated = true;
                ReadXml();

                TaskHandler.Instance.RegTask(new TaskFileManager(), 1000);
            }
        }
        #endregion

        #region Method
        internal void Save()
        {
            WriteXml();
        }
        public void UpdateScale(AnalogScale scale)
        {
            if(string.IsNullOrEmpty(scale.Name) == false)
            {
                if(m_Scales.ContainsKey(scale.Name) == false)
                    m_Scales.Add(scale.Name, scale);
                else
                    m_Scales[scale.Name] = scale;

                QueueUpdateEvent.Enqueue(true);
            }
        }
        public AnalogScale GetChannelScale(IoChannel ch)
        {
            string name = string.Format("{0} : {1}", ch.WiringNo, ch.Name);
            if(m_Scales.ContainsKey(name) == false)
                m_Scales.Add(name, new AnalogScale(name));

            return m_Scales[name];            
        }
        #endregion

        #region Method - File Read / Write
        private bool ReadXml()
        {
            if(CheckPath())
            {
                return ReadXml(m_FileName);
            }
            else return false;
        }
        private bool ReadXml(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if(fileInfo.Exists)
                {
                    m_FileName = fileName;
                }
                else
                {
                    WriteXml();
                }

                StreamReader sr = new StreamReader(fileName);
                XmlSerializer xmlSer = new XmlSerializer(typeof(AnalogScaleManager));
                AnalogScaleManager factory = new AnalogScaleManager();
                factory = xmlSer.Deserialize(sr) as AnalogScaleManager;
                sr.Close();
                m_Scales = factory.Scales;
            }
            catch(Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
                MessageBox.Show(err.ToString());
            }

            return true;
        }
        private void WriteXml()
        {
            if(string.IsNullOrEmpty(m_FileName))
            {
                string dirName = AppConfig.DefaultConfigFilePath;
                Directory.CreateDirectory(dirName);

                m_FilePath = string.Format("{0}\\{1}.xml", dirName, this.GetType().Name);
            }

            WriteXml(m_FileName);
        }
        private void WriteXml(string fileName)
        {
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
            catch(Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                string msg = err.ToString();
                //System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(method, err.ToString());
                if(sw != null) sw.Close();

                return;
            }

            try
            {
                FileInfo file = new FileInfo(fileName);
                if(file.Exists)
                {
                    file.CopyTo(fileName + ".old", true);
                }

                sw = new StreamWriter(fileName);
                xmlSer.Serialize(sw, this);
                sw.Close();

                m_FilePath = fileName;
            }
            catch(Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                //System.Windows.Forms.MessageBox.Show(err.ToString());
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
            }
        }
        private bool CheckPath()
        {
            bool ok = false;

            try
            {
                string filePath = m_FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppConfig.Instance.XmlIoDefinePath;
                }
                if (Path.HasExtension(filePath)) filePath = Path.GetDirectoryName(filePath);

                if (XFunc.CheckPathAndCreate(filePath))
                {
                    m_FilePath = filePath;
                    m_FileName = string.Format("{0}\\{1}.xml", filePath, this.GetType().Name);
                    ok = true;
                }
                else
                {
                    MessageBox.Show("Io Config File Not Found");
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.Description = "Io Config File Folder";
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
                        m_FileName = string.Format("{0}\\{1}.xml", filePath, this.GetType().Name);
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
            }
            catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
            {
                //System.Windows.Forms.MessageBox.Show(err.ToString());
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
            }

            return ok;
        }
        #endregion

        private class TaskFileManager : XSequence
        {
            public TaskFileManager()
            {
                RegSeq(new SeqFileHandler());
            }

            private class SeqFileHandler : XSeqFunc
            {
                public SeqFileHandler()
                {
                    StartTicks = GetElapsedTicks();
                }

                public override int Do()
                {
                    int seqNo = SeqNo;
                    switch(seqNo)
                    {
                    case 0:
                        if(GetElapsedTicks() > 1000)
                        {
                            seqNo = 10;
                        }
                        break;
                    case 10:
                        if(AnalogScaleManager.Instance.QueueUpdateEvent.Count > 0)
                        {
                            AnalogScaleManager.Instance.QueueUpdateEvent.Clear();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;
                    case 20:
                        if(GetElapsedTicks() > 100)
                        {
                            AnalogScaleManager.Instance.Save();
                            seqNo = 10;
                        }
                        break;
                    }

                    SeqNo = seqNo;
                    return -1;
                }
            }
        }
    }
}
