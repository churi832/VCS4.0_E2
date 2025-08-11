using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_ErrorHistory
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryErrorHistoryEvent(object sender, object eventData);

        public event QueryErrorHistoryEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_ErrorHistory(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM ErrorHistory";

                object result = m_JobSession.ExecuteScalar(queryString);

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
        public int SelectCount(DateTime startTime, DateTime endTime, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory ");
                queryString.AppendFormat("WHERE OccurredTime BETWEEN '{0}' AND '{1}'", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public int SelectCount(DateTime startTime, DateTime endTime, int id, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory ");
                queryString.AppendFormat("WHERE (OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(ID = {0})", id);

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public int SelectCount(DateTime startTime, DateTime endTime, string locationID, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory ");
                queryString.AppendFormat("WHERE (OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(LocationID Like '%{0}%')", locationID);

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public int SelectCountJoin(DateTime startTime, DateTime endTime, AlarmType level, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory AS A INNDER JOIN VehicleErrorList AS B ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(ErrorLevel = {0})", Convert.ToInt32(level));

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public int SelectCountJoin(DateTime startTime, DateTime endTime, int code, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory AS A INNDER JOIN VehicleErrorList AS B ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(Code = {0})", Convert.ToInt32(code));

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public int SelectCountJoin(DateTime startTime, DateTime endTime, string location, ref long count)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT COUNT(*) FROM ErrorHistory AS A INNDER JOIN VehicleErrorList AS B ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(LocationID Like '%{0}%')", location);

                object result = m_JobSession.ExecuteScalar(queryString.ToString());

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
        public List<DataItem_ErrorHistory> SelectAllOrNull()
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN ErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNull(DateTime startTime, DateTime endTime)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN VehicleErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE A.OccurredTime BETWEEN '{0}' AND '{1}' ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNull(DateTime startTime, DateTime endTime, int errorID)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN VehicleErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (A.OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(A.ID = {0}) ", errorID);
                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNullByJoin(DateTime startTime, DateTime endTime, int errorCode)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN VehicleErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (A.OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(B.Code = {0}) ", errorCode);
                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNullByJoin(DateTime startTime, DateTime endTime, AlarmType errorLevel)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN VehicleErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (A.OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(B.ErrorLevel = {0}) ", Convert.ToInt32(errorLevel));
                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNullByJoin(DateTime startTime, DateTime endTime, string location)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN VehicleErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (A.OccurredTime BETWEEN '{0}' AND '{1}') AND ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("(A.LocationID Like '%{0}%') ", Convert.ToInt32(location));
                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_ErrorHistory> SelectListOrNullByJoin(DateTime startTime, DateTime endTime, int errorLevel, int errorCleared, int errorCode, string errorUnit, string errorDescription)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("SELECT A.OccurredTime, A.ClearedTime, A.ID, A.LocationID, ");
                queryString.Append("B.Code, B.ErrorLevel, B.Unit, B.Description, B.Comment ");
                queryString.Append("FROM ErrorHistory AS A ");
                queryString.Append("INNER JOIN ErrorList AS B ");
                queryString.Append("ON A.ID = B.ID ");
                queryString.AppendFormat("WHERE (A.OccurredTime BETWEEN '{0}' AND '{1}') ", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (errorLevel > 0)
                {
                    queryString.AppendFormat("AND (B.ErrorLevel = {0}) ", errorLevel);
                }
                switch (errorCleared)
                {
                    case 1:
                        queryString.AppendFormat("AND (A.ClearedTime is not null) ");
                        break;
                    case 2:
                        queryString.AppendFormat("AND (A.ClearedTime is null) ");
                        break;
                    default:
                        break;
                }
                if (errorCode != 0)
                {
                    queryString.AppendFormat("AND (B.Code = {0}) ", errorCode);
                }
                if (!string.IsNullOrWhiteSpace(errorUnit))
                {
                    queryString.AppendFormat("AND (B.Unit like '%{0}%') ", errorUnit);
                }
                if (!string.IsNullOrWhiteSpace(errorDescription))
                {
                    queryString.AppendFormat("AND (B.Description like '%{0}%') ", errorDescription);
                }

                queryString.Append("ORDER BY A.OccurredTime ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorHistory> tableData = new List<DataItem_ErrorHistory>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

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
        private List<DataItem_ErrorHistory> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_ErrorHistory> itemList = new List<DataItem_ErrorHistory>();

                foreach (DataRow dr in table.Rows)
                {
                    DateTime occurredTime = new DateTime();
                    DateTime? clearedTime = null;

                    if (dr["OccurredTime"] != null)
                    {
                        DateTime.TryParse(dr["OccurredTime"].ToString(), out occurredTime);
                    }
                    if (dr["ClearedTime"] != DBNull.Value)
                    {
                        clearedTime = DateTime.Parse(dr["ClearedTime"].ToString());
                    }

                    int id = Convert.ToInt32(dr["ID"]);
                    string location = (dr["LocationID"] == null) ? string.Empty : dr["LocationID"].ToString();
                    int code = Convert.ToInt32(dr["Code"]);
                    int level = Convert.ToInt32(dr["ErrorLevel"]);
                    string unit = (dr["Unit"] == null) ? string.Empty : dr["Unit"].ToString().Trim();
                    string description = (dr["Description"] == null) ? string.Empty : dr["Description"].ToString().Trim();
                    string comment = (dr["Comment"] == null) ? string.Empty : dr["Comment"].ToString().Trim();

                    DataItem_ErrorHistory data = new DataItem_ErrorHistory(occurredTime, id, location, code, (AlarmType)level, unit, description, comment);
                    data.ClearedTime = clearedTime;

                    itemList.Add(data);
                }

                return itemList;
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
        public int Insert(DateTime occurredTime, DateTime clearedTime, int id, string location)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ErrorHistory ");
                queryString.Append("(OccurredTime, ClearedTime, ID, LocationID) VALUES (");
                queryString.AppendFormat("'{0}', ", occurredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("'{0}', ", clearedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("'{0}')", location);
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        public int Insert(DataItem_ErrorHistory item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ErrorHistory ");
                queryString.Append("(OccurredTime, ClearedTime, ID, LocationID) VALUES (");
                queryString.AppendFormat("'{0}', ", item.OccurredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("'{0}', ", item.ClearedTime?.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("{0}, ", item.ID);
                queryString.AppendFormat("'{0}')", item.LocationID);
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        public int Insert(List<DataItem_ErrorHistory> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ErrorHistory ");

                queryString.Append("(OccurredTime, ClearedTime, ID, LocationID) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].OccurredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    queryString.AppendFormat("'{0}', ", itemList[index].ClearedTime?.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    queryString.AppendFormat("{0}, ", itemList[index].ID);
                    queryString.AppendFormat("'{0}')", itemList[index].LocationID);

                    if (index < itemList.Count - 1)
                    {
                        queryString.Append(", ");
                    }
                }
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
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
        public int Update(DataItem_ErrorHistory item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE ErrorHistory SET ");

                queryString.AppendFormat("[ClearedTime] = '{0}' ", item.ClearedTime?.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                queryString.AppendFormat("WHERE [ID] = {0} AND ", item.ID);
                queryString.AppendFormat("[OccurredTime] = '{0}'", item.OccurredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
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
        public int Delete(DateTime startTime, DateTime endTime)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM ErrorHistory ");
                queryString.AppendFormat("WHERE [OccurredTime] BETWEEN '{0}' AND '{1}'", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        public int Delete(DateTime endTime)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM ErrorHistory ");
                queryString.AppendFormat("WHERE [OccurredTime] <= '{0}'", endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
            return rv;
        }
        public int Delete(DataItem_ErrorHistory item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM ErrorHistory ");
                queryString.AppendFormat("WHERE [OccurredTime] = '{0}'", item.OccurredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (CallBack_NonQuery == null)
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                else
                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        #endregion
    }
}
