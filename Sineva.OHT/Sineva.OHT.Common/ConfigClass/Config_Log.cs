using System;
using System.ComponentModel;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public class Config_Log : Config, IConfig
    {
        #region Fields
        private FolderSelect _LogPath = new FolderSelect();
        private LogFileSplitType _LogFileSplit = LogFileSplitType.Day;
        private int _OriginalLogKeepingDays = 30;
        private int _CompressedLogKeepingDays = 180;
        private int _MaxRecordLines = 10000;
        #endregion

        #region Properties
        [Category("Log Configuration")]
        [Description("Log file path.")]
        [DisplayName("Log File Save Path")]
        [ConfigAttribute(SaveName = "LogPath")]
        [Editor(typeof(ConfigFolderSelector_WPF), typeof(ConfigFolderSelector_WPF))]
        public FolderSelect LogPath
        {
            get { return _LogPath; }
            set { _LogPath = value; }
        }
        [Category("Log Configuration")]
        [Description("Log file split type.")]
        [ConfigAttribute(SaveName = "LogSplitType")]
        public LogFileSplitType LogFileSplit
        {
            get { return _LogFileSplit; }
            set { _LogFileSplit = value; }
        }
        [Category("Log Configuration")]
        [Description("Original log files keeping days. (30 ~ CompressedLogKeepingDays days)")]
        [ConfigAttribute(SaveName = "LogKeepingDays")]
        public int OriginalLogKeepingDays
        {
            get { return _OriginalLogKeepingDays; }
            set
            {
                if ((value < 30) || (value > _CompressedLogKeepingDays))
                {
                    return;
                }
                _OriginalLogKeepingDays = value;
            }
        }
        [Category("Log Configuration")]
        [Description("Compressed log files keeping days. (OriginalLogKeepingDays ~ 360 days)")]
        [DisplayName("Compressed Log File Keeping Days")]
        [ConfigAttribute(SaveName = "CompressedLogKeepingDays")]
        public int CompressedLogKeepingDays
        {
            get { return _CompressedLogKeepingDays; }
            set
            {
                if ((value < _OriginalLogKeepingDays) || (value > 360))
                {
                    return;
                }
                _CompressedLogKeepingDays = value;
            }
        }
        [Category("Log Configuration")]
        [Description("Log file max lines. (100,000 ~ 2,000,000 lines)\r\nOnly in LogFileSplitType is DayAndLines.")]
        [DisplayName("Max Record Lines")]
        [ConfigAttribute(SaveName = "MaxRecordLines")]
        public int MaxRecordLines
        {
            get { return _MaxRecordLines; }
            set
            {
                if ((value < 100000) || (value > 2000000))
                {
                    return;
                }
                _MaxRecordLines = value;
            }
        }
        #endregion

        #region Constructors
        public Config_Log()
        {
            _Category = "Log";
        }
        #endregion

        #region Methods
        public Config_Log GetCopyOrNull()
        {
            try
            {
                Config_Log clone = new Config_Log();

                clone.LogPath = this._LogPath;
                clone.LogFileSplit = this._LogFileSplit;
                clone.OriginalLogKeepingDays = this._OriginalLogKeepingDays;
                clone.CompressedLogKeepingDays = this._CompressedLogKeepingDays;
                clone.MaxRecordLines = this._MaxRecordLines;
                return clone;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
