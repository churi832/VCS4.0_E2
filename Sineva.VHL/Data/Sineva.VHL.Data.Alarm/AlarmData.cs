using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.Alarm
{
    public enum AlarmCode
    {
        NotUsed = 0,
        PersonalSafety = 1,
        EquipmentSafety = 2,
        ParameterControlWarning = 3,
        ParameterControlError = 4,
        IrrecoverableError = 5,
        EquipmentStatusWarning = 6,
        AttentionFlags = 7,
        DataIntegrity = 8,
    }
    public class AlarmData
    {
        #region Fields
        private int m_ID = 0;
        private AlarmLevel m_Level = AlarmLevel.L;
        private AlarmCode m_Code;
        private string m_Unit = string.Empty;
        private string m_Name = string.Empty;
        private string m_Comment = string.Empty;
        #endregion

        #region Properties
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        public AlarmLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        public AlarmCode Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }
        public string Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
        #endregion

        #region Constructor
        public AlarmData()
        {
        }
        public AlarmData(int id, AlarmCode code, AlarmLevel level, string unit = "", string name = "", string comment = "")
        {
            this.m_ID = id;
            this.m_Code = code;
            this.m_Level = level;
            this.m_Unit = unit;
            this.m_Name = name;
            this.m_Comment = comment;
        }
        #endregion

        #region Methods
        public void Clone(AlarmData alarm)
        {
            this.m_ID = alarm.ID;
            this.m_Code = alarm.Code;
            this.m_Level = alarm.Level;
            this.m_Unit = alarm.Unit;
            this.m_Name = alarm.Name;
            this.m_Comment = alarm.Comment;
        }

        public void Clone(DataRow row)
        {
            this.m_ID = (int)(row[0]);
            this.m_Code = (AlarmCode)(int)(row[3]);
            this.m_Level = (AlarmLevel)((int)(row[2]));
            this.m_Unit = (string)(row[4]);
            this.m_Name = (string)(row[1]);
            this.m_Comment = (string)(row[6]);
        }
        public AlarmData GetCopyOrNull()
        {
            try
            {
                return (AlarmData)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(AlarmData target)
        {
            bool result = true;

            result &= (this.m_ID == target.ID);
            result &= (this.m_Code == target.Code);
            result &= (this.m_Level == target.Level);
            result &= (this.m_Unit == target.Unit);
            result &= (this.m_Name == target.Name);
            result &= (this.m_Comment == target.Comment);
            return result;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} / {1} / {2} / {3}", m_Name, m_Unit, m_Level, m_Code);
        }

    }
}
