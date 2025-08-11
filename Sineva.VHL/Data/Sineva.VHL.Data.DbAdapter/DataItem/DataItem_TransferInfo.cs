using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_TransferInfo : DataItem //class name을 다음 Version에서 TransferCommand로 변경하자~~~
    {
        #region Fields - TransferList & TransferHistory
        private string m_CommandID = string.Empty;
        private TransferType m_Type = TransferType.Transfer;
        private string m_CarrierID = string.Empty;
        private string m_Source = string.Empty;
        private string m_Destination = string.Empty;
        private ProcessStatus m_Status = ProcessStatus.Queued;
        private int m_Priority = 0;
        private string m_CarrierLocation = string.Empty;

        #region Fields - TransferHistory Only
        private DateTime m_InstallTime = DateTime.Now;
        private string m_AssignTime = string.Empty;
        private string m_VehicleFromArrivedTime = string.Empty;
        private string m_VehicleAcquireStartTime = string.Empty;
        private string m_VehicleAcquireEndTime = string.Empty;
        private string m_VehicleDepartedTime = string.Empty;
        private string m_VehicleToArrivedTime = string.Empty;
        private string m_VehicleDepositStartTime = string.Empty;
        private string m_VehicleDepositEndTime = string.Empty;
        private string m_TransferCompletedTime = string.Empty;
        #endregion
        private string m_DelayReason = string.Empty;
        private int m_PriorityUpdateCount = 0;
        private string m_PriorityUpdateTime = string.Empty;
        private List<int> m_PathNodes = new List<int>();
        #endregion


        #region Properties
        [DatabaseSettingAttribute(true)]
        public string CommandID
        {
            get { return m_CommandID; }
            set { m_CommandID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public TransferType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string CarrierID
        {
            get { return m_CarrierID; }
            set { m_CarrierID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string Destination
        {
            get { return m_Destination; }
            set { m_Destination = value; }
        }
        [DatabaseSettingAttribute(true)]
        public ProcessStatus Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string CarrierLocation
        {
            get { return m_CarrierLocation; }
            set { m_CarrierLocation = value; }
        }
        [DatabaseSettingAttribute(true)]
        public DateTime InstallTime
        {
            get { return m_InstallTime; }
            set { m_InstallTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string AssignTime
        {
            get { return m_AssignTime; }
            set { m_AssignTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string DelayReason
        {
            get { return m_DelayReason; }
            set { m_DelayReason = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int PriorityUpdateCount
        {
            get { return m_PriorityUpdateCount; }
            set { m_PriorityUpdateCount = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string PriorityUpdateTime
        {
            get { return m_PriorityUpdateTime; }
            set { m_PriorityUpdateTime = value; }
        }
        [DatabaseSettingAttribute(true)]
        public List<int> PathNodes
        {
            get { return m_PathNodes; }
            set { m_PathNodes = value; }
        }
        #endregion

        #region Properties - TransferHistory Only
        public string VehicleFromArrivedTime
        {
            get { return m_VehicleFromArrivedTime; }
            set { m_VehicleFromArrivedTime = value; }
        }
        public string VehicleAcquireStartTime
        {
            get { return m_VehicleAcquireStartTime; }
            set { m_VehicleAcquireStartTime = value; }
        }
        public string VehicleAcquireEndTime
        {
            get { return m_VehicleAcquireEndTime; }
            set { m_VehicleAcquireEndTime = value; }
        }
        public string VehicleDepartedTime
        {
            get { return m_VehicleDepartedTime; }
            set { m_VehicleDepartedTime = value; }
        }
        public string VehicleToArrivedTime
        {
            get { return m_VehicleToArrivedTime; }
            set { m_VehicleToArrivedTime = value; }
        }
        public string VehicleDepositStartTime
        {
            get { return m_VehicleDepositStartTime; }
            set { m_VehicleDepositStartTime = value; }
        }
        public string VehicleDepositEndTime
        {
            get { return m_VehicleDepositEndTime; }
            set { m_VehicleDepositEndTime = value; }
        }
        public string TransferCompletedTime
        {
            get { return m_TransferCompletedTime; }
            set { m_TransferCompletedTime = value; }
        }
        #endregion

        #region Constructors
        public DataItem_TransferInfo()
        {
        }
        public DataItem_TransferInfo(string commandID, TransferType type, string carrierID, string source, string destination, ProcessStatus status, int priority)
        {
            try
            {
                this.m_CommandID = commandID;
                this.m_Type = type;
                this.m_CarrierID = carrierID;
                this.m_Source = source;
                this.m_Destination = destination;
                this.m_Status = status;
                this.m_Priority = priority;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_TransferInfo source)
        {
            try
            {
                this.m_CommandID = source.CommandID;
                this.m_Type = source.Type;
                this.m_CarrierID = source.CarrierID;
                this.m_Source = source.Source;
                this.m_Destination = source.Destination;
                this.m_Status = source.Status;
                this.m_Priority = source.Priority;
                this.m_CarrierLocation = source.CarrierLocation;
                this.m_InstallTime = source.InstallTime;
                this.m_AssignTime = source.AssignTime;
                this.m_DelayReason = source.DelayReason;
                this.m_PriorityUpdateCount = source.PriorityUpdateCount;
                this.m_PriorityUpdateTime = source.PriorityUpdateTime;

                this.m_VehicleFromArrivedTime = source.VehicleFromArrivedTime;
                this.m_VehicleAcquireStartTime = source.VehicleAcquireStartTime;
                this.m_VehicleAcquireEndTime = source.VehicleAcquireEndTime;
                this.m_VehicleDepartedTime = source.VehicleDepartedTime;
                this.m_VehicleToArrivedTime = source.VehicleToArrivedTime;
                this.m_VehicleDepositStartTime = source.VehicleDepositStartTime;
                this.m_VehicleDepositEndTime = source.VehicleDepositEndTime;
                this.m_PathNodes = source.PathNodes;
            }
            catch
            {
            }
        }
        public DataItem_TransferInfo GetCopyOrNull()
        {
            try
            {
                return (DataItem_TransferInfo)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_TransferInfo target)
        {
            try
            {
                bool result = true;

                result &= (this.m_CommandID == target.CommandID);
                result &= (this.m_Type == target.Type);
                result &= (this.m_CarrierID == target.CarrierID);
                result &= (this.m_Source == target.Source);
                result &= (this.m_Destination == target.Destination);
                result &= (this.m_Status == target.Status);
                result &= (this.m_Priority == target.Priority);
                result &= (this.m_CarrierLocation == target.CarrierLocation);
                result &= (this.m_InstallTime == target.InstallTime);
                result &= (this.m_AssignTime == target.AssignTime);
                result &= (this.m_DelayReason == target.DelayReason);
                result &= (this.m_PriorityUpdateCount == target.PriorityUpdateCount);
                result &= (this.m_PriorityUpdateTime == target.PriorityUpdateTime);

                result &= (this.m_VehicleFromArrivedTime == target.VehicleFromArrivedTime);
                result &= (this.m_VehicleAcquireStartTime == target.VehicleAcquireStartTime);
                result &= (this.m_VehicleAcquireEndTime == target.VehicleAcquireEndTime);
                result &= (this.m_VehicleDepartedTime == target.VehicleDepartedTime);
                result &= (this.m_VehicleToArrivedTime == target.VehicleToArrivedTime);
                result &= (this.m_VehicleDepositStartTime == target.VehicleDepositStartTime);
                result &= (this.m_VehicleDepositEndTime == target.VehicleDepositEndTime);
                result &= (this.m_PathNodes == target.PathNodes);

                return result;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return string.Format("{0},{1}_{2},{3}", m_CommandID, m_Source, m_Destination, m_PathNodes);
        }
        #endregion
    }
}
