using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable]
    public class TeachingOffsetAdapter
    {
        public readonly static TeachingOffsetAdapter Instance = new TeachingOffsetAdapter();
        #region Fields
        private bool m_Initialized = false;
        private string m_FileName;
        private string m_FilePath;

        private List<TeachingOffset> m_Offsets = new List<TeachingOffset>();
        #endregion

        #region Properties
        public List<TeachingOffset> Offsets
        {
            get { return m_Offsets; }
            set { m_Offsets = value; }
        }
        #endregion

        #region Constructor
        public TeachingOffsetAdapter()
        {
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;

            m_Initialized = ReadXml();
            return m_Initialized;
        }
        // compare TeachingOffset <-> DataItem_Port
        // add, change value
        public void CheckOffset(DataItem_Port item)
        {
            try
            {
                if (item == null) return;
                TeachingOffset offset = m_Offsets.Find(x => x.PortID == item.PortID);
                if (offset != null)
                {
                    if (offset.DriveLeftOffset == 0.0f) offset.DriveLeftOffset = item.DriveLeftOffset;
                    else item.DriveLeftOffset = offset.DriveLeftOffset;
                    if (offset.DriveRightOffset == 0.0f) offset.DriveRightOffset = item.DriveRightOffset;
                    else item.DriveRightOffset = offset.DriveRightOffset;
                    if (offset.SlideOffset == 0.0f) offset.SlideOffset = item.SlideOffset;
                    else item.SlideOffset = offset.SlideOffset;
                    if (offset.HoistOffset == 0.0f) offset.HoistOffset = item.HoistOffset;
                    else item.HoistOffset = offset.HoistOffset;
                    if (offset.RotateOffset == 0.0f) offset.RotateOffset = item.RotateOffset;
                    else item.RotateOffset = offset.RotateOffset;
                }
                else
                {
                    
                    //var sameTypePortIDs = Offsets
                    //    .Select(x => x.PortID)
                    //    .ToList();
 
                    var sameTypeOffsets = m_Offsets.Where(x =>
                    {
                        return GetPortTypeByPortID(x.PortID) == item.PortType;
                    }).ToList();

                    double GetMostCommon(Func<TeachingOffset, double> selector, double defaultValue)
                    {
                        if (sameTypeOffsets.Count == 0) return defaultValue;
                        return sameTypeOffsets
                            .GroupBy(selector)
                            .OrderByDescending(g => g.Count())
                            .First().Key;
                    }

                    double driveLeftOffset = GetMostCommon(x => x.DriveLeftOffset, item.DriveLeftOffset);
                    double driveRightOffset = GetMostCommon(x => x.DriveRightOffset, item.DriveRightOffset);
                    double slideOffset = GetMostCommon(x => x.SlideOffset, item.SlideOffset);
                    double hoistOffset = GetMostCommon(x => x.HoistOffset, item.HoistOffset);
                    double rotateOffset = GetMostCommon(x => x.RotateOffset, item.RotateOffset);
                    m_Offsets.Add(new TeachingOffset(item.PortID, driveLeftOffset, driveRightOffset, slideOffset, hoistOffset, rotateOffset));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public PortType GetPortTypeByPortID(int portID)
        {
           
            if (DatabaseHandler.Instance.DictionaryPortDataList != null && DatabaseHandler.Instance.DictionaryPortDataList.TryGetValue(portID, out var port))
            {
                return port.PortType;
            }
            return PortType.NotDefined;
        }

        public void UpdateSaveOffset(DataItem_Port item)
        {
            try
            {
                if (item == null) return;
                TeachingOffset offset = m_Offsets.Find(x => x.PortID == item.PortID);
                if (offset != null)
                {
                    offset.DriveLeftOffset = item.DriveLeftOffset;
                    offset.DriveRightOffset = item.DriveRightOffset;
                    offset.SlideOffset = item.SlideOffset;
                    offset.HoistOffset = item.HoistOffset;
                    offset.RotateOffset = item.RotateOffset;
                }
                else
                {
                    m_Offsets.Add(new TeachingOffset(item.PortID, item.DriveLeftOffset, item.DriveRightOffset, item.SlideOffset, item.HoistOffset, item.RotateOffset));
                }
                Save();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void UpdateOffset(DataItem_Port item)
        {
            try
            {
                if (item == null) return;
                TeachingOffset offset = m_Offsets.Find(x => x.PortID == item.PortID);
                if (offset != null)
                {
                    offset.DriveLeftOffset = item.DriveLeftOffset;
                    offset.DriveRightOffset = item.DriveRightOffset;
                    offset.SlideOffset = item.SlideOffset;
                    offset.HoistOffset = item.HoistOffset;
                    offset.RotateOffset = item.RotateOffset;
                }
                else
                {
                    m_Offsets.Add(new TeachingOffset(item.PortID, item.DriveLeftOffset, item.DriveRightOffset, item.SlideOffset, item.HoistOffset, item.RotateOffset));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void UpdateOffset(List<DataItem_Port> items)
        {
            try
            {
                if (items == null || items.Count == 0) return;
                foreach (DataItem_Port item in items) {UpdateOffset(item); }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public double GetAutoTrackingValue(bool acquire, int portID)
        {
            double rv = 0.0f;
            try
            {
                TeachingOffset offset = m_Offsets.Find(x => x.PortID == portID);
                if (offset != null) rv = acquire ? offset.HoistAcquireTrackingValue : offset.HoistDepositeTrackingValue;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public void SetAutoTrackingValue(bool acquire, int portID, double error)
        {
            try
            {
                TeachingOffset offset = m_Offsets.Find(x => x.PortID == portID);
                if (offset != null)
                {
                    double k = 0.15f;//0.3f //수치를 작게 조정하여 변화랑이 차근차근 적용되도록...
                    double oldValue = acquire ? offset.HoistAcquireTrackingValue : offset.HoistDepositeTrackingValue;
                    double newValue = (1 - k) * oldValue + k * (oldValue + error);
                    if (acquire) offset.HoistAcquireTrackingValue = Math.Round(newValue, 2);
                    else offset.HoistDepositeTrackingValue = Math.Round(newValue, 2); 
                    Save();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void Save()
        {
            WriteXml();
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

                var helperXml = new XmlHelper<TeachingOffsetAdapter>();
                TeachingOffsetAdapter mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.Offsets = mng.Offsets;
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
                WriteXml(m_FileName);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void WriteXml(string fileName)
        {
            try
            {
                var helperXml = new XmlHelper<TeachingOffsetAdapter>();
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
                    filePath = AppConfig.Instance.XmlDatabasePath;
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

    [Serializable]
    public class TeachingOffset
    {
        #region Fields
        private int m_PortID;
        private double m_DriveLeftOffset = 0.0f;
        private double m_DriveRightOffset = 0.0f;
        private double m_SlideOffset = 0.0f;
        private double m_HoistOffset = 0.0f;
        private double m_RotateOffset = 0.0f;
        private double m_HoistAcquireTrackingValue = 0.0f;
        private double m_HoistDepositeTrackingValue = 0.0f;
        #endregion

        #region Properties
        public int PortID { get => m_PortID; set => m_PortID = value; }
        public double DriveLeftOffset { get => m_DriveLeftOffset; set => m_DriveLeftOffset = value; }
        public double DriveRightOffset { get => m_DriveRightOffset; set => m_DriveRightOffset = value; }
        public double HoistOffset { get => m_HoistOffset; set => m_HoistOffset = value; }
        public double SlideOffset { get => m_SlideOffset; set => m_SlideOffset = value; }
        public double RotateOffset { get => m_RotateOffset; set => m_RotateOffset = value; }
        public double HoistAcquireTrackingValue { get => m_HoistAcquireTrackingValue; set => m_HoistAcquireTrackingValue = value; }
        public double HoistDepositeTrackingValue { get => m_HoistDepositeTrackingValue; set => m_HoistDepositeTrackingValue = value; }
        #endregion

        #region Constructor
        public TeachingOffset()
        {
        }
        public TeachingOffset(int port, double driveL, double driveR, double slide, double hoist, double rotate)
        {
            m_PortID = port;
            m_DriveLeftOffset = driveL;
            m_DriveRightOffset = driveR;
            m_SlideOffset = slide;
            m_HoistOffset = hoist;
            m_RotateOffset = rotate;
        }
        #endregion
    }

}
