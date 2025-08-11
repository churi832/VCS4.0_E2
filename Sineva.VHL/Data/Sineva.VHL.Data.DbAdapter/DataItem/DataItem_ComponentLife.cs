using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public enum LifeDataType : int
    {
        Time = 0,//时间
        Distance = 1,//距离
        Frequency = 2,//次数

    }
    [Serializable()]
    public enum ComponentLocation : int
    {
        None = 0,
        Driver = 1,
        Hoist = 2,
        Slide = 3,
        Rotate = 4,
        FrontSteer = 5,
        RearSteer = 6,
        FOUPDetect = 7,
        FrontAntiDrop = 8,
        RearAntiDrop = 9,
        AlwaysUse = 10,
    }

    [Serializable()]
    public class DataItem_ComponentLife : DataItem
    {
        #region Fields - Database
        private string m_ComponentName = "";
        private string m_ComponentType = "";
        private string m_ComponentMaker = "";
        private string m_Unit = "";
        private ComponentLocation m_ComponentLocation = ComponentLocation.None;
        private LifeDataType m_DataType = LifeDataType.Time;
        private DateTime m_UsedStartTime = new DateTime();
        private double m_LifeTime = 0;
        private double m_UsedTime = 0;
        private bool m_IsAlwaysUse = false;
        #endregion
        #region Fields - Sequence
        private bool m_IsStart = false;
        private TimeCheck m_RunTime = new TimeCheck();
        private int m_ActionTimes = 0;
        private double m_ActionDistance = 0.0d;
        private double m_StartPosition = 0.0d;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public string ComponentName
        {
            get { return m_ComponentName; }
            set { m_ComponentName = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string ComponentType
        {
            get { return m_ComponentType; }
            set { m_ComponentType = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string ComponentMaker
        {
            get { return m_ComponentMaker; }
            set { m_ComponentMaker = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        [DatabaseSettingAttribute(true)]
        public ComponentLocation ComponentLocation
        {
            get { return m_ComponentLocation; }
            set { m_ComponentLocation = value; }
        }
        [DatabaseSettingAttribute(true)]
        public LifeDataType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double LifeTime
        {
            get { return m_LifeTime; }
            set { m_LifeTime = value; }

        }
        [DatabaseSettingAttribute(true)]
        public double UsedTime
        {
            get { return m_UsedTime; }
            set { m_UsedTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool IsAlwaysUse
        {
            get { return m_IsAlwaysUse; }
            set { m_IsAlwaysUse = value; }
        }
        [DatabaseSettingAttribute(true)]
        public DateTime UseStartTime
        {
            get { return m_UsedStartTime; }
            set { m_UsedStartTime = value; }
        }
        [DatabaseSettingAttribute(false)]
        public bool IsStart
        {
            get { return m_IsStart; }
            set { m_IsStart = value; }
        }
        [DatabaseSettingAttribute(false)]
        public TimeCheck RunTime
        {
            get { return m_RunTime; }
            set { m_RunTime = value; }
        }
        [DatabaseSettingAttribute(false)]
        public int ActionTimes
        {
            get { return m_ActionTimes; }
            set { m_ActionTimes = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double ActionDistance
        {
            get { return m_ActionDistance; }
            set { m_ActionDistance = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double StartPosition
        {
            get { return m_StartPosition; }
            set { m_StartPosition = value; }
        }
        #endregion

        #region Constructors
        public DataItem_ComponentLife()
        {
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_ComponentLife source)
        {
            try
            {
                this.m_ComponentName = source.ComponentName;
                this.m_ComponentType = source.ComponentType;
                this.m_ComponentMaker = source.ComponentMaker;
                this.m_LifeTime = source.LifeTime;
                this.m_UsedTime = source.UsedTime;
                this.m_UsedStartTime = source.UseStartTime;
                this.m_IsAlwaysUse = source.IsAlwaysUse;
                this.m_ComponentLocation = source.ComponentLocation;
                this.m_DataType = source.DataType;
                this.m_Unit = source.Unit;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public DataItem_ComponentLife GetCopyOrNull()
        {
            DataItem_ComponentLife component = null;
            try
            {
                component = (DataItem_ComponentLife)base.MemberwiseClone();
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return component;
        }
        public bool CompareWith(DataItem_ComponentLife target)
        {
            bool result = false;
            try
            {
                result = (this.m_ComponentName == target.ComponentName);
                result &= (this.m_ComponentType == target.ComponentType);
                result &= (this.m_ComponentMaker == target.ComponentMaker);
                result &= (Math.Abs(this.m_LifeTime - target.LifeTime) < 1);
                result &= (Math.Abs(this.m_UsedTime - target.UsedTime) < 1);
                result &= (this.m_UsedStartTime == target.UseStartTime);
                result &= (this.m_IsAlwaysUse == target.IsAlwaysUse);
                result &= (this.m_ComponentLocation == target.ComponentLocation);
                result &= (this.m_DataType == target.DataType);
                result &= (this.m_Unit == target.Unit);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return result;
        }
        #endregion
    }
}
