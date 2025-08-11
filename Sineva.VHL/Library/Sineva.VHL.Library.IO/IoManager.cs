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

namespace Sineva.VHL.Library.IO
{
	#region XmlInclude
	[XmlInclude(typeof(_IoNode))]
	[XmlInclude(typeof(IoNodeMp2100))]
    [XmlInclude(typeof(IoNodeBnrX20CP0482))]
	[XmlInclude(typeof(IoNodeGN9289))]
	[XmlInclude(typeof(IoNodeGL9089))]
    [XmlInclude(typeof(IoNodeMxpEC))]
    [XmlInclude(typeof(IoTermMpIO2310_30))]
    [XmlInclude(typeof(IoTermMpAN2900))]
    [XmlInclude(typeof(IoTermMpAN2910))]
    [XmlInclude(typeof(IoTermBnrX20DI9372))]
	[XmlInclude(typeof(IoTermBnrX20DO9321))]
	[XmlInclude(typeof(IoTermBnrX20AI2632))]
	[XmlInclude(typeof(IoTermBnrX20AI4632))]
	[XmlInclude(typeof(IoTermBnrX20AIA744))]
	[XmlInclude(typeof(IoTermBnrX20AIB744))]
	[XmlInclude(typeof(IoTermBnrX20AO2632))]
	[XmlInclude(typeof(IoTermBnrX20AO4632))]
	[XmlInclude(typeof(IoTermBnrX20MM2436))]
	[XmlInclude(typeof(BnrAxisChannel))]
	[XmlInclude(typeof(IoTermCrevis_GT1238))]
	[XmlInclude(typeof(IoTermCrevis_GT123F))]
	[XmlInclude(typeof(IoTermCrevis_GT12DF))]
	[XmlInclude(typeof(IoTermCrevis_GT12FA))]
	[XmlInclude(typeof(IoTermCrevis_GT225F))]
	[XmlInclude(typeof(IoTermCrevis_GT226F))]
	[XmlInclude(typeof(IoTermCrevis_GT2318))]
	[XmlInclude(typeof(IoTermCrevis_GT2328))]
	[XmlInclude(typeof(IoTermCrevis_GT2618))]
	[XmlInclude(typeof(IoTermCrevis_GT2628))]
	[XmlInclude(typeof(IoTermCrevis_GT3002))]
	[XmlInclude(typeof(IoTermCrevis_GT3114))]
	[XmlInclude(typeof(IoTermCrevis_GT3118))]
	[XmlInclude(typeof(IoTermCrevis_GT3154))]
	[XmlInclude(typeof(IoTermCrevis_GT3158))]
	[XmlInclude(typeof(IoTermCrevis_GT317F))]
	[XmlInclude(typeof(IoTermCrevis_GT319F))]
	[XmlInclude(typeof(IoTermCrevis_GT3424))]
	[XmlInclude(typeof(IoTermCrevis_GT3428))]
	[XmlInclude(typeof(IoTermCrevis_GT3464))]
	[XmlInclude(typeof(IoTermCrevis_GT3468))]
	[XmlInclude(typeof(IoTermCrevis_GT347F))]
	[XmlInclude(typeof(IoTermCrevis_GT349F))]
	[XmlInclude(typeof(IoTermCrevis_GT4114))]
	[XmlInclude(typeof(IoTermCrevis_GT4118))]
	[XmlInclude(typeof(IoTermCrevis_GT4154))]
	[XmlInclude(typeof(IoTermCrevis_GT4158))]
	[XmlInclude(typeof(IoTermCrevis_GT4424))]
	[XmlInclude(typeof(IoTermCrevis_GT4428))]
	[XmlInclude(typeof(IoTermCrevis_GT4464))]
	[XmlInclude(typeof(IoTermCrevis_GT4468))]
	[XmlInclude(typeof(IoTermCrevis_GT7511))]
    [XmlInclude(typeof(IoTermMxpD232))]
    [XmlInclude(typeof(IoTermMxpDT32K))]
	[XmlInclude(typeof(IoTermMxpECK64))]
	[XmlInclude(typeof(IoTermMxpTR32K))]
    #endregion

    [Serializable()]
	public class IoManager
	{
		public readonly static IoManager Instance = new IoManager();

		#region Fields
		private static List<IoChannel> m_DiChannels = new List<IoChannel>();
		private static List<IoChannel> m_DoChannels = new List<IoChannel>();
		private static List<IoChannel> m_AiChannels = new List<IoChannel>();
		private static List<IoChannel> m_AoChannels = new List<IoChannel>();
        private static List<IoChannel> m_AxChannels = new List<IoChannel>();
//		private List<IoTerminal> m_Terminals = new List<IoTerminal>();
		private List<_IoNode> m_Nodes = new List<_IoNode>();
		private string m_FileName = "";
		private string m_FilePath = "";
		private bool m_Initialized = false;
        private bool m_UpdateRun = false;
        #endregion

        #region Properties
        [Browsable(false), XmlIgnore()]
        public bool Initialized
        {
            get { return m_Initialized; }
            set { m_Initialized = value; }
        }
        [Browsable(false), XmlIgnore()]
        public bool UpdateRun
        {
            get { return m_UpdateRun; }
            set { m_UpdateRun = value; }
        }

        public static List<IoChannel> DiChannels
		{
			get { return m_DiChannels; }
			set { m_DiChannels = value; }
		}

		public static List<IoChannel> DoChannels
		{
			get { return m_DoChannels; }
			set { m_DoChannels = value; }
		}

		public static List<IoChannel> AiChannels
		{
			get { return m_AiChannels; }
			set { m_AiChannels = value; }
		}

		public static List<IoChannel> AoChannels
		{
			get { return m_AoChannels; }
			set { m_AoChannels = value; }
		}
        public static List<IoChannel> AxChannels
        {
            get { return m_AxChannels; }
            set { m_AxChannels = value; }
        }

        public List<_IoNode> Nodes
		{
			get { return m_Nodes; }
			set { m_Nodes = value; }
		}

		#endregion

		#region Constructor
		private IoManager()
		{
		}
		#endregion

		#region Methods
		public bool Initialize()
		{
			if(m_Initialized == false)
				this.ReadXml();
			m_Initialized = true;
			return m_Initialized;
		}

		public void InitializeIoCollection()
		{
			try
			{
                ClearIoCollection();
                int diId = 0, doId = 0, aiId = 0, aoId = 0, axId = 0, terminalId = 0;

                int nodeId = 0;
                foreach (_IoNode node in m_Nodes)
                {
                    int nodeDI = 0, nodeDO = 0, nodeAI = 0, nodeAO = 0;
                    nodeId++;

                    foreach (_IoTerminal terminal in node.GetTerminals())
                    {
                        terminal.Id = terminalId++;
                        if (terminal.IoTermBusType == IoBusType.CrevisModbus)
                            terminal.BuildChannels(nodeId, ref nodeDI, ref nodeDO, ref nodeAI, ref nodeAO);
                        else
                            terminal.BuildChannels();
                        foreach (IoChannel ch in terminal.GetChannels())
                        {
                            switch (ch.IoType)
                            {
                                case IoType.DI:
                                    ch.Id = diId++;
                                    m_DiChannels.Add(ch);
                                    break;

                                case IoType.DO:
                                    ch.Id = doId++;
                                    m_DoChannels.Add(ch);
                                    break;

                                case IoType.AI:
                                    ch.Id = aiId++;
                                    ch.Scale = AnalogScaleManager.Instance.GetChannelScale(ch);
                                    m_AiChannels.Add(ch);
                                    break;

                                case IoType.AO:
                                    ch.Id = aoId++;
                                    m_AoChannels.Add(ch);
                                    break;

                                case IoType.AX:
                                    ch.Id = axId++;
                                    m_AxChannels.Add(ch);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
                MessageBox.Show(err.ToString());
            }
		}

		private void ClearIoCollection()
		{
			m_DiChannels.Clear();
			m_DoChannels.Clear();
			m_AiChannels.Clear();
			m_AoChannels.Clear();
            m_AxChannels.Clear();
		}

		public static List<_IoNode> GetNodeTypes()
		{
            List<_IoNode> nodeTypes = new List<_IoNode>();
            try
            {
                Assembly asm = Assembly.Load((typeof(IoManager)).Namespace);
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (XFunc.CheckTypeCompatibility(typeof(_IoNode), type, Compatibility.Compatible))
                    {
                        if (type.IsAbstract == false)
                        {
                            _IoNode node = Activator.CreateInstance(type) as _IoNode;
                            nodeTypes.Add(node);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
                MessageBox.Show(err.ToString());
            }

			return nodeTypes;
		}

		public IoChannel GetChannelByName(IoType type, string name)
		{
			List<IoChannel> channels = new List<IoChannel>();
			switch (type)
			{
				case IoType.DI:
					channels = DiChannels;
					break;

				case IoType.DO:
					channels = DoChannels;
					break;

				case IoType.AI:
					channels = AiChannels;
					break;
	
				case IoType.AO:
					channels = AoChannels;
					break;

                case IoType.AX:
                    channels = AxChannels;
                    break;

			}
			foreach (IoChannel ch in channels)
			{
				if (name == ch.Name)
				{
					return ch;
				}
			}
			return null;
		}
        // 대상 I/O 목록이 연속으로 있을 때에만 쓸 것....
        public IoChannel[] GetContinuousChannels(IChannelCommand start, int count)
        {
            int length = -1;
            List<IoChannel> channelList = new List<IoChannel>();

			try
			{
                switch ((start as IoChannel).IoType)
                {
                    case IoType.DI:
                        {
                            length = m_DiChannels.Count;
                            for (int id = 0; id < length; id++)
                            {
                                if (m_DiChannels[id].Equals(start))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        channelList.Add(m_DiChannels[id + i]);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case IoType.DO:
                        {
                            length = m_DoChannels.Count;
                            for (int id = 0; id < length; id++)
                            {
                                if (m_DoChannels[id].Equals(start))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        channelList.Add(m_DoChannels[id + i]);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case IoType.AI:
                        {
                            length = m_AiChannels.Count;
                            for (int id = 0; id < length; id++)
                            {
                                if (m_AiChannels[id].Equals(start))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        channelList.Add(m_AiChannels[id + i]);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case IoType.AO:
                        {
                            length = m_AoChannels.Count;
                            for (int id = 0; id < length; id++)
                            {
                                if (m_AoChannels[id].Equals(start))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        channelList.Add(m_AoChannels[id + i]);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case IoType.AX:
                        {
                            length = m_AxChannels.Count;
                            for (int id = 0; id < length; id++)
                            {
                                if (m_AxChannels[id].Equals(start))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        channelList.Add(m_AxChannels[id + i]);
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
                MessageBox.Show(err.ToString());
            }

            return channelList.ToArray();
        }

		public List<bool> GetStateInputs()
		{
            List<bool> inputs = m_DiChannels.Select(x => x.State).ToList().ConvertAll(x=>x.Equals("ON"));
            return inputs;
        }
        public List<bool> GetStateOutputs()
        {
            List<bool> outputs = m_DoChannels.Select(x => x.State).ToList().ConvertAll(x => x.Equals("ON"));
            return outputs;
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
					// to make Sample templete
					//IoTerminal terminal = new IoTerminal();
					//m_Terminals.Add(terminal);
					WriteXml();
				}

				StreamReader sr = new StreamReader(fileName);
				XmlSerializer xmlSer = new XmlSerializer(typeof(IoManager));
				IoManager ioFactory = new IoManager();
				ioFactory = xmlSer.Deserialize(sr) as IoManager;
				sr.Close();
				m_Nodes = ioFactory.Nodes;

				InitializeIoCollection();
			}
			catch (Exception err)
			{
				ExceptionLog.WriteLog(err.ToString()); 
				MessageBox.Show(err.ToString());
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
				string msg = err.ToString();
				System.Windows.Forms.MessageBox.Show(err.ToString());
				ExceptionLog.WriteLog(err.ToString()); 
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

				sw = new StreamWriter(fileName);
				xmlSer.Serialize(sw, this);
				sw.Close();

				m_FilePath = fileName;
			}
			catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
			{
				System.Windows.Forms.MessageBox.Show(err.ToString());
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
                    filePath = AppConfig.Instance.XmlIoDefinePath;
                }
                if (Path.HasExtension(filePath)) filePath = Path.GetDirectoryName(filePath);

                if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
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
