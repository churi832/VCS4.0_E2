using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public enum FDC_Unit : int
    {
        OHT = 0,
        Drive_Master = 1,
        Drive_Slave = 2,
        Hoist = 3,
        Slide = 4,
        Rotate = 5,
        Gripper = 6,
        CPS = 7,
    }

    [Serializable()]
    public enum FDC_DataType : int
    {
        Torque = 0,
        Velocity,
        Acc_Dec,
        Load_Factor,
        BCR,
        Shock_Data,
        Motor_RPM,
        Accumulate_Distance,
        CPS_Voltage,
        CPS_Current,
        CPS_Temperature,
    }

    [Serializable()]
    public class DataItem_FDC : DataItem
    {
        #region Fields - Database
        private int m_ID = 0;
        private FDC_Unit m_Unit = FDC_Unit.OHT;
        private FDC_DataType m_DataType = FDC_DataType.Torque;
        private string m_Name = "";
        private string m_Discription = "";
        private int m_DecimalPoint = 0;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public FDC_Unit Unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        [DatabaseSettingAttribute(true)]
        public FDC_DataType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Discription
        {
            get { return m_Discription; }
            set { m_Discription = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int DecimalPoint
        {
            get { return m_DecimalPoint; }
            set { m_DecimalPoint = value; }
        }
        #endregion

        #region Constructors
        public DataItem_FDC()
        {
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_FDC source)
        {
            try
            {
                this.m_ID = source.ID;
                this.m_Unit = source.Unit;
                this.m_DataType = source.DataType;
                this.m_Name = source.Name;
                this.m_Discription = source.Discription;
                this.m_DecimalPoint = source.DecimalPoint;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public DataItem_FDC GetCopyOrNull()
        {
            DataItem_FDC component = null;
            try
            {
                component = (DataItem_FDC)base.MemberwiseClone();
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return component;
        }
        public bool CompareWith(DataItem_FDC target)
        {
            bool result = false;
            try
            {
                result = (this.m_ID == target.ID);
                result &= (this.m_Unit == target.Unit);
                result &= (this.m_DataType == target.DataType);
                result &= (this.m_Name == target.Name);
                result &= (this.m_Discription == target.Discription);
                result &= (this.m_DecimalPoint == target.DecimalPoint);
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
