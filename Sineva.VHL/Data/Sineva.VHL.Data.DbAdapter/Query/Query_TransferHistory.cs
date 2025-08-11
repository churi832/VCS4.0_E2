using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_TransferHistory
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion
        #region Events
        public delegate void QueryErrorListEvent(object sender, object eventData);

        public event QueryErrorListEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion
        #region Constructors
        public Query_TransferHistory(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM TransferHistory";

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString);

                if (result == null)
                {
                    return -1;
                }

                count = Convert.ToInt64(result);
                return 1;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int CountAllByInstallTime(DateTime startTime, DateTime endTime, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM TransferHistory WHERE InstallTime BETWEEN ");
                queryString.AppendFormat("'{0}' AND '{1}'", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString.ToString());

                if (result == null)
                {
                    return -1;
                }

                count = Convert.ToInt64(result);
                return 1;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int CountAllByCompleteTime(DateTime startTime, DateTime endTime, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM TransferHistory WHERE TransferCompletedTime BETWEEN ");
                queryString.AppendFormat("'{0}' AND '{1}'", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString.ToString());

                if (result == null)
                {
                    return -1;
                }

                count = Convert.ToInt64(result);
                return 1;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        #endregion

        #region Methods - Select Query
        public DataItem_TransferInfo SelectSingleOrNull(int transferID)
        {
            try
            {
                DataItem_TransferInfo tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM TransferHistory Where CommandID = '{0}'", transferID));

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        string id = dr["CommandID"].ToString();
                        int commandType = Convert.ToInt32(dr["CommandType"]);
                        string carrierID = dr["CarrierID"].ToString();
                        string source = dr["Source"].ToString();
                        string destination = dr["Destination"].ToString();
                        int transferState = Convert.ToInt32(dr["TransferState"]);
                        int priority = Convert.ToInt32(dr["Priority"]);
                        string carrierLocation = dr["CarrierLocation"].ToString();

                        DateTime installTime = DateTime.Now;
                        string assignedTime = string.Empty;
                        string priorityUpdateTime = string.Empty;

                        if (dr["InstallTime"] != null)
                        {
                            installTime = Convert.ToDateTime(dr["InstallTime"]);//.ToString();
                        }
                        if (dr["AssignedTime"] != null)
                        {
                            assignedTime = dr["AssignedTime"].ToString();
                            //DateTime.TryParse(dr["AssignedTime"].ToString(), out assignedTime);
                        }

                        string delayReason = (dr["DealyReason"] == null) ? string.Empty : dr["DealyReason"].ToString();
                        int priorityUpdateCount = (dr["PriorityUpdateCount"] == null) ? 0 : Convert.ToInt32(dr["PriorityUpdateCount"]);

                        if (dr["PriorityUpdateTime"] != null)
                        {
                            priorityUpdateTime = dr["PriorityUpdateTime"].ToString();
                            //DateTime.TryParse(dr["PriorityUpdateTime"].ToString(), out priorityUpdateTime);
                        }

                        string vehicleFromArrivedTime = string.Empty;
                        string vehicleAcquireStartTime = string.Empty;
                        string vehicleAcquireEndTime = string.Empty;
                        string vehicleDepartedTime = string.Empty;
                        string vehicleToArrivedTime = string.Empty;
                        string vehicleDepositStartTime = string.Empty;
                        string vehicleDepositEndTime = string.Empty;
                        string transferCompleteTime = string.Empty;
                        List<int> pathNodes = new List<int>();

                        if (dr["VehicleFromArrivedTime"] != null)
                        {
                            vehicleFromArrivedTime = dr["VehicleFromArrivedTime"].ToString();
                            //DateTime.TryParse(dr["VehicleFromArrivedTime"].ToString(), out vehicleFromArrivedTime);
                        }
                        if (dr["VehicleAcquireStartTime"] != null)
                        {
                            vehicleAcquireStartTime = dr["VehicleAcquireStartTime"].ToString();
                            //DateTime.TryParse(dr["VehicleAcquireStartTime"].ToString(), out vehicleAcquireStartTime);
                        }
                        if (dr["VehicleAcquireEndTime"] != null)
                        {
                            vehicleAcquireEndTime = dr["VehicleAcquireEndTime"].ToString();
                            //DateTime.TryParse(dr["VehicleAcquireEndTime"].ToString(), out vehicleAcquireEndTime);
                        }
                        if (dr["VehicleDepartedTime"] != null)
                        {
                            vehicleDepartedTime = dr["VehicleDepartedTime"].ToString();
                            //DateTime.TryParse(dr["VehicleDepartedTime"].ToString(), out vehicleDepartedTime);
                        }
                        if (dr["VehicleToArrivedTime"] != null)
                        {
                            vehicleToArrivedTime = dr["VehicleToArrivedTime"].ToString();
                            //DateTime.TryParse(dr["VehicleToArrivedTime"].ToString(), out vehicleToArrivedTime);
                        }
                        if (dr["VehicleDepositStartTime"] != null)
                        {
                            vehicleDepositStartTime = dr["VehicleDepositStartTime"].ToString();
                            //DateTime.TryParse(dr["VehicleDepositStartTime"].ToString(), out vehicleDepositStartTime);
                        }
                        if (dr["VehicleDepositEndTime"] != null)
                        {
                            vehicleDepositEndTime = dr["VehicleDepositEndTime"].ToString();
                            //DateTime.TryParse(dr["VehicleDepositEndTime"].ToString(), out vehicleDepositEndTime);
                        }
                        if (dr["TransferCompletedTime"] != null)
                        {
                            transferCompleteTime = dr["TransferCompletedTime"].ToString();
                            //DateTime.TryParse(dr["TransferCompletedTime"].ToString(), out transferCompleteTime);
                        }
                        if (dr["PathNodes"] != null)
                        {
                            string[] nodes = dr["PathNodes"].ToString().Split(',');
                            int[] mynodes = Array.ConvertAll(nodes, x => Convert.ToInt32(x));
                            pathNodes = mynodes.OfType<int>().ToList();
                        }

                        DataItem_TransferInfo data = new DataItem_TransferInfo();
                        tableData.CommandID = id;
                        tableData.Type = (TransferType)commandType;
                        tableData.CarrierID = carrierID;
                        tableData.Source = source;
                        tableData.Destination = destination;
                        tableData.Status = (ProcessStatus)transferState;
                        tableData.Priority = priority;
                        tableData.CarrierLocation = carrierLocation;
                        tableData.InstallTime = installTime;
                        tableData.AssignTime = assignedTime;
                        tableData.VehicleFromArrivedTime = vehicleFromArrivedTime;
                        tableData.VehicleAcquireStartTime = vehicleAcquireStartTime;
                        tableData.VehicleAcquireEndTime = vehicleAcquireEndTime;
                        tableData.VehicleDepartedTime = vehicleDepartedTime;
                        tableData.VehicleToArrivedTime = vehicleToArrivedTime;
                        tableData.VehicleDepositStartTime = vehicleDepositStartTime;
                        tableData.VehicleDepositEndTime = vehicleDepositStartTime;
                        tableData.TransferCompletedTime = transferCompleteTime;
                        tableData.DelayReason = delayReason;
                        tableData.PriorityUpdateCount = priorityUpdateCount;
                        tableData.PriorityUpdateTime = priorityUpdateTime;
                        tableData.PathNodes = pathNodes;
                        break;
                    }
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListOrNull(DateTime startTime, DateTime endTime, TransferType type)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(CommandType = {0}) ", Convert.ToInt32(type));
                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListOrNull(DateTime startTime, DateTime endTime, ProcessStatus status)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (status != ProcessStatus.None) queryString.AppendFormat("AND (TransferStatus = {0}) ", Convert.ToInt32(status));
                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListOrNull(DateTime startTime, DateTime endTime, string source, string destination)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(Source = '{0}') AND ", Convert.ToInt32(source));
                queryString.AppendFormat("(Destination = '{0}') ", Convert.ToInt32(destination));
                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListOrNull(DateTime startTime, DateTime endTime, int type, int status, string source, string destination)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (type != 0)
                {
                    queryString.AppendFormat("AND (CommandType = {0}) ", type);
                }
                if (status != 0)
                {
                    queryString.AppendFormat("AND (TransferStatus = {0}) ", status);
                }
                if (source.Trim() != string.Empty)
                {
                    queryString.AppendFormat("AND (Source like '%{0}%') ", source);
                }
                if (destination.Trim() != string.Empty)
                {
                    queryString.AppendFormat("AND (Destination like '%{0}%') ", destination);
                }

                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListBySourceOrNull(DateTime startTime, DateTime endTime, string source)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(Source = '{0}') ", Convert.ToInt32(source));
                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> SelectListByDestinationOrNull(DateTime startTime, DateTime endTime, string destination)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT * FROM TransferHistory WHERE ");
                queryString.AppendFormat("(InstallTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(Destination = '{0}') ", Convert.ToInt32(destination));
                queryString.AppendFormat("ORDER BY InstallTime ASC");

                DataTable table = new DataTable();
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    tableData = GetListFromTableOrNull(table);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public List<DataItem_TransferInfo> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_TransferInfo> tableData = new List<DataItem_TransferInfo>();

                foreach (DataRow dr in table.Rows)
                {
                    string transferID = dr["CommandID"].ToString();
                    int commandType = Convert.ToInt32(dr["CommandType"]);
                    string carrierID = dr["CarrierID"].ToString();
                    string source = dr["Source"].ToString();
                    string destination = dr["Destination"].ToString();
                    int transferState = Convert.ToInt32(dr["TransferStatus"]);
                    int priority = Convert.ToInt32(dr["Priority"]);
                    string carrierLocation = dr["CarrierLocation"].ToString();

                    DateTime installTime = DateTime.Now;
                    string assignedTime = string.Empty;
                    string priorityUpdateTime = string.Empty;

                    if (dr["InstallTime"] != null)
                    {
                        installTime = Convert.ToDateTime(dr["InstallTime"]); //.ToString();
                    }
                    if (dr["AssignedTime"] != null)
                    {
                        assignedTime = dr["AssignedTime"].ToString();
                        //DateTime.TryParse(dr["AssignedTime"].ToString(), out assignedTime);
                    }

                    string delayReason = (dr["DelayReason"] == null) ? string.Empty : dr["DelayReason"].ToString();
                    int priorityUpdateCount = (dr["PriorityUpdateCount"] == null) ? 0 : Convert.ToInt32(dr["PriorityUpdateCount"]);

                    if (dr["PriorityUpdateTime"] != null)
                    {
                        priorityUpdateTime = dr["PriorityUpdateTime"].ToString();
                        //DateTime.TryParse(dr["PriorityUpdateTime"].ToString(), out priorityUpdateTime);
                    }

                    string vehicleFromArrivedTime = string.Empty;
                    string vehicleAcquireStartTime = string.Empty;
                    string vehicleAcquireEndTime = string.Empty;
                    string vehicleDepartedTime = string.Empty;
                    string vehicleToArrivedTime = string.Empty;
                    string vehicleDepositStartTime = string.Empty;
                    string vehicleDepositEndTime = string.Empty;
                    string transferCompleteTime = string.Empty;
                    List<int> pathNodes = new List<int>();

                    if (dr["VehicleFromArrivedTime"] != null)
                    {
                        vehicleFromArrivedTime = dr["VehicleFromArrivedTime"].ToString();
                        //DateTime.TryParse(dr["VehicleFromArrivedTime"].ToString(), out vehicleFromArrivedTime);
                    }
                    if (dr["VehicleAcquireStartTime"] != null)
                    {
                        vehicleAcquireStartTime = dr["VehicleAcquireStartTime"].ToString();
                        //DateTime.TryParse(dr["VehicleAcquireStartTime"].ToString(), out vehicleAcquireStartTime);
                    }
                    if (dr["VehicleAcquireEndTime"] != null)
                    {
                        vehicleAcquireEndTime = dr["VehicleAcquireEndTime"].ToString();
                        //DateTime.TryParse(dr["VehicleAcquireEndTime"].ToString(), out vehicleAcquireEndTime);
                    }
                    if (dr["VehicleDepartedTime"] != null)
                    {
                        vehicleDepartedTime = dr["VehicleDepartedTime"].ToString();
                        //DateTime.TryParse(dr["VehicleDepartedTime"].ToString(), out vehicleDepartedTime);
                    }
                    if (dr["VehicleToArrivedTime"] != null)
                    {
                        vehicleToArrivedTime = dr["VehicleToArrivedTime"].ToString();
                        //DateTime.TryParse(dr["VehicleToArrivedTime"].ToString(), out vehicleToArrivedTime);
                    }
                    if (dr["VehicleDepositStartTime"] != null)
                    {
                        vehicleDepositStartTime = dr["VehicleDepositStartTime"].ToString();
                        //DateTime.TryParse(dr["VehicleDepositStartTime"].ToString(), out vehicleDepositStartTime);
                    }
                    if (dr["VehicleDepositEndTime"] != null)
                    {
                        vehicleDepositEndTime = dr["VehicleDepositEndTime"].ToString();
                        //DateTime.TryParse(dr["VehicleDepositEndTime"].ToString(), out vehicleDepositEndTime);
                    }
                    if (dr["TransferCompletedTime"] != null)
                    {
                        transferCompleteTime = dr["TransferCompletedTime"].ToString();
                        //DateTime.TryParse(dr["TransferCompletedTime"].ToString(), out transferCompleteTime);
                    }
                    if (dr["PathNodes"] != null)
                    {
                        string[] nodes = dr["PathNodes"].ToString().Split(',');
                        if (nodes.Length > 0 && string.IsNullOrEmpty(nodes[0]) == false)
                        {
                            int[] mynodes = Array.ConvertAll(nodes, x => Convert.ToInt32(x));
                            pathNodes = mynodes.OfType<int>().ToList();
                        }
                    }
                    DataItem_TransferInfo data = new DataItem_TransferInfo();
                    data.CommandID = transferID;
                    data.Type = (TransferType)commandType;
                    data.CarrierID = carrierID;
                    data.Source = source;
                    data.Destination = destination;
                    data.Status = (ProcessStatus)transferState;
                    data.Priority = priority;
                    data.CarrierLocation = carrierLocation;
                    data.InstallTime = installTime;
                    data.AssignTime = assignedTime;
                    data.VehicleFromArrivedTime = vehicleFromArrivedTime;
                    data.VehicleAcquireStartTime = vehicleAcquireStartTime;
                    data.VehicleAcquireEndTime = vehicleAcquireEndTime;
                    data.VehicleDepartedTime = vehicleDepartedTime;
                    data.VehicleToArrivedTime = vehicleToArrivedTime;
                    data.VehicleDepositStartTime = vehicleDepositStartTime;
                    data.VehicleDepositEndTime = vehicleDepositStartTime;
                    data.TransferCompletedTime = transferCompleteTime;
                    data.DelayReason = delayReason;
                    data.PriorityUpdateCount = priorityUpdateCount;
                    data.PriorityUpdateTime = priorityUpdateTime;
                    data.PathNodes = pathNodes;
                    tableData.Add(data);
                }

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        #endregion

        #region Methods - Insert Query
        public int Insert(DataItem_TransferInfo item)
        {
            int rv = -1;
            try
            {
                string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
                StringBuilder queryString = new StringBuilder("INSERT INTO TransferHistory ");
                queryString.Append("(CommandID, CommandType, CarrierID, Source, Destination, TransferStatus, Priority, CarrierLocation, InstallTime, AssignedTime, ");
                queryString.Append("VehicleFromArrivedTime, VehicleAcquireStartTime, VehicleAcquireEndTime, VehicleDepartedTime, VehicleToArrivedTime, VehicleDepositStartTime, ");
                queryString.Append("VehicleDepositEndTime, TransferCompletedTime, DelayReason, PriorityUpdateCount, PriorityUpdateTime, PathNodes) VALUES (");
                queryString.AppendFormat("'{0}', ", item.CommandID.Replace("\0", ""));
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("'{0}', ", item.CarrierID.Replace("\0", ""));
                queryString.AppendFormat("'{0}', ", item.Source.Replace("\0", ""));
                queryString.AppendFormat("'{0}', ", item.Destination.Replace("\0", ""));
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Status));
                queryString.AppendFormat("{0}, ", item.Priority);
                queryString.AppendFormat("'{0}', ", item.CarrierLocation.Replace("\0", ""));
                queryString.AppendFormat("'{0}', ", item.InstallTime.ToString(timeFormat));
                queryString.AppendFormat("'{0}', ", item.AssignTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleFromArrivedTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleAcquireStartTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleAcquireEndTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleDepartedTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleToArrivedTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleDepositStartTime.ToString());
                queryString.AppendFormat("'{0}', ", item.VehicleDepositEndTime.ToString());
                queryString.AppendFormat("'{0}', ", item.TransferCompletedTime.ToString());
                queryString.AppendFormat("'{0}', ", item.DelayReason);
                queryString.AppendFormat("{0}, ", item.PriorityUpdateCount);
                queryString.AppendFormat("'{0}', ", item.PriorityUpdateTime.ToString());
                queryString.AppendFormat("'{0}') ", string.Join(",", item.PathNodes.ToArray()));

                // Worker Process 생성
                BackgroundWorker addWorker = new BackgroundWorker();
                addWorker.DoWork += new DoWorkEventHandler(DoWork);
                addWorker.RunWorkerAsync(new string[] { queryString.ToString() });

                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        public int Insert(List<DataItem_TransferInfo> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO TransferHistory ");
                queryString.Append("(CommandID, CommandType, CarrierID, Source, Destination, TransferStatus, Priority, CarrierLocation, InstallTime, AssignedTime, ");
                queryString.Append("VehicleFromArrivedTime, VehicleAcquireStartTime, VehicleAcquiereEndTime, VehicleDepartedTime, VehicleToArrivedTime, VehicleDepositStartTime, ");
                queryString.Append("VehicleDepositEndTime, TransferCompletedTime, DelayReason, PriorityUpdateCount, PriorityUpdateTime, PathNodes) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].CommandID);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Type));
                    queryString.AppendFormat("'{0}', ", itemList[index].CarrierID);
                    queryString.AppendFormat("'{0}', ", itemList[index].Source);
                    queryString.AppendFormat("'{0}', ", itemList[index].Destination);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Status));
                    queryString.AppendFormat("{0}, ", itemList[index].Priority);
                    queryString.AppendFormat("'{0}', ", itemList[index].CarrierLocation);
                    queryString.AppendFormat("'{0}', ", itemList[index].InstallTime);
                    queryString.AppendFormat("'{0}', ", itemList[index].AssignTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleFromArrivedTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleAcquireStartTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleAcquireEndTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleDepartedTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleToArrivedTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleDepositStartTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].VehicleDepositEndTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].TransferCompletedTime.ToString());
                    queryString.AppendFormat("'{0}', ", itemList[index].DelayReason);
                    queryString.AppendFormat("{0}, ", itemList[index].PriorityUpdateCount);
                    queryString.AppendFormat("'{0}', ", itemList[index].PriorityUpdateTime.ToString());
                    queryString.AppendFormat("'{0}') ", string.Join(",", itemList[index].PathNodes.ToArray()));

                    if (index < itemList.Count - 1)
                    {
                        queryString.Append(", ");
                    }
                }
                // Worker Process 생성
                BackgroundWorker addWorker = new BackgroundWorker();
                addWorker.DoWork += new DoWorkEventHandler(DoWork);
                addWorker.RunWorkerAsync(new string[] { queryString.ToString() });

                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        #endregion

        #region Methods - Update Query
        public int Update(DataItem_TransferInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE TransferHistory SET ");

                queryString.AppendFormat("CommandType = {0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("CarrierID = '{0}', ", item.CarrierID);
                queryString.AppendFormat("Source = '{0}', ", item.Source);
                queryString.AppendFormat("Destination = '{0}', ", item.Destination);
                queryString.AppendFormat("TransferStatus = {0}, ", Convert.ToInt32(item.Status));
                queryString.AppendFormat("Priority = {0}, ", item.Priority);
                queryString.AppendFormat("CarrierLocation = '{0}', ", item.CarrierLocation);
                queryString.AppendFormat("InstallTime = '{0}', ", item.InstallTime);
                queryString.AppendFormat("AssignedTime = '{0}', ", item.AssignTime.ToString());
                queryString.AppendFormat("VehicleFromArrivedTime = '{0}', ", item.VehicleFromArrivedTime.ToString());
                queryString.AppendFormat("VehicleAcquireStartTime = '{0}', ", item.VehicleAcquireStartTime.ToString());
                queryString.AppendFormat("VehicleAcquireEndTime = '{0}', ", item.VehicleAcquireEndTime.ToString());
                queryString.AppendFormat("VehicleDepartedTime = '{0}', ", item.VehicleDepartedTime.ToString());
                queryString.AppendFormat("VehicleToArrivedTime = '{0}', ", item.VehicleToArrivedTime.ToString());
                queryString.AppendFormat("VehicleDepositStartTime = '{0}', ", item.VehicleDepositStartTime.ToString());
                queryString.AppendFormat("VehicleDepositEndTime = '{0}', ", item.VehicleDepositEndTime.ToString());
                queryString.AppendFormat("TransferCompletedTime = '{0}', ", item.TransferCompletedTime.ToString());
                queryString.AppendFormat("DelayReason = '{0}', ", item.DelayReason);
                queryString.AppendFormat("PriorityUpdateCount = {0}, ", item.PriorityUpdateCount);
                queryString.AppendFormat("PriorityUpdateTime = '{0}', ", item.PriorityUpdateTime.ToString());
                queryString.AppendFormat("PathNodes = '{0}' ", string.Join(",", item.PathNodes.ToArray()));

                queryString.AppendFormat("WHERE CommandID = '{0}'", item.CommandID);

                // Worker Process 생성
                BackgroundWorker updateWorker = new BackgroundWorker();
                updateWorker.DoWork += new DoWorkEventHandler(DoWork);
                updateWorker.RunWorkerAsync(new string[] { queryString.ToString() });

                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }

        public int UpdateCarrierLocation(DataItem_TransferInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE TransferHistory SET ");

                queryString.AppendFormat("CarrierID = '{0}', ", item.CarrierID);
                queryString.AppendFormat("CarrierLocation = '{0}', ", item.CarrierLocation);
                queryString.AppendFormat("WHERE CommandID = '{0}'", item.CommandID);

                // Worker Process 생성
                BackgroundWorker updateWorker = new BackgroundWorker();
                updateWorker.DoWork += new DoWorkEventHandler(DoWork);
                updateWorker.RunWorkerAsync(new string[] { queryString.ToString() });
                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }

        public int UpdateTime(DataItem_TransferInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE TransferHistory SET ");

                queryString.AppendFormat("VehicleFromArrivedTime = '{0}', ", item.VehicleFromArrivedTime.ToString());
                queryString.AppendFormat("VehicleAcquireStartTime = '{0}', ", item.VehicleAcquireStartTime.ToString());
                queryString.AppendFormat("VehicleAcquireEndTime = '{0}', ", item.VehicleAcquireEndTime.ToString());
                queryString.AppendFormat("VehicleDepartedTime = '{0}', ", item.VehicleDepartedTime.ToString());
                queryString.AppendFormat("VehicleToArrivedTime = '{0}', ", item.VehicleToArrivedTime.ToString());
                queryString.AppendFormat("VehicleDepositStartTime = '{0}', ", item.VehicleDepositStartTime.ToString());
                queryString.AppendFormat("VehicleDepositEndTime = '{0}', ", item.VehicleDepositEndTime.ToString());
                queryString.AppendFormat("TransferCompletedTime = '{0}', ", item.TransferCompletedTime.ToString());
                queryString.AppendFormat("PriorityUpdateTime = '{0}' ", item.PriorityUpdateTime.ToString());

                queryString.AppendFormat("WHERE CommandID = '{0}'", item.CommandID);

                // Worker Process 생성
                BackgroundWorker updateTimeWorker = new BackgroundWorker();
                updateTimeWorker.DoWork += new DoWorkEventHandler(DoWork);
                updateTimeWorker.RunWorkerAsync(new string[] { queryString.ToString() });

                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }

        public int UpdatePriority(DataItem_TransferInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE TransferHistory SET ");

                queryString.AppendFormat("PriorityUpdateCount = {0}, ", item.PriorityUpdateCount);
                queryString.AppendFormat("PriorityUpdateTime = '{0}' ", item.PriorityUpdateTime.ToString());

                queryString.AppendFormat("WHERE CommandID = '{0}'", item.CommandID);

                // Worker Process 생성
                BackgroundWorker updateTimeWorker = new BackgroundWorker();
                updateTimeWorker.DoWork += new DoWorkEventHandler(DoWork);
                updateTimeWorker.RunWorkerAsync(new string[] { queryString.ToString() });

                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        #endregion

        #region Methods - Delete Query
        public int Delete(string transferID)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM TransferHistory ");
                queryString.AppendFormat("WHERE CommandID = '{0}'", transferID);

                // Worker Process 생성
                BackgroundWorker updateTimeWorker = new BackgroundWorker();
                updateTimeWorker.DoWork += new DoWorkEventHandler(DoWork);
                updateTimeWorker.RunWorkerAsync(new string[] { queryString.ToString() });
                rv = 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        #endregion

        #region Background Work
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            string[] info = (string[])e.Argument;

            try
            {
                if (info.Length > 0)
                {
                    if (CallBack_NonQuery == null)
                        m_JobSession.ExecuteNonQuery(info[0].ToString());
                    else
                        m_JobSession.ExecuteNonQuery(info[0].ToString(), CallBack_NonQuery);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        #endregion

    }
}
