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
    public class Query_ErrorList
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
        public Query_ErrorList(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ErrorListType type, ref long count)
        {
            try
            {
                string queryString = string.Format("SELECT COUNT(*) FROM ErrorList");

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
        public int GetMinValidId(int startId = 1)
        {
            try
            {
                string queryString = string.Format("SELECT COUNT(*) FROM ErrorList where ID ={0}", startId);
                object startIdExist = m_JobSession.ExecuteScalar(queryString);
                if (Convert.ToInt32(startIdExist) == 0)
                {
                    return startId;
                }
                queryString = string.Format("SELECT min(ID) + 1 FROM ErrorList where ID + 1 not in (SELECT ID FROM ErrorList)");
                object result = m_JobSession.ExecuteScalar(queryString);

                if (result == null)
                {
                    return -1;
                }

                int res = Convert.ToInt32(result);
                return res;
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
        public List<DataItem_ErrorList> SelectAllOrNull()
        {
            try
            {
                string queryString = string.Format("SELECT * FROM ErrorList ORDER BY ID ASC");

                DataTable table = new DataTable();
                List<DataItem_ErrorList> tableData = new List<DataItem_ErrorList>();

                int returnValue = m_JobSession.ExecuteReader(queryString, ref table);

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
        public DataItem_ErrorList SelectSingleOrNull(int alarmID)
        {
            try
            {
                DataItem_ErrorList tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM ErrorList Where ID = {0}", alarmID));

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int id = Convert.ToInt32(dr["ID"]);
                        int code = Convert.ToInt32(dr["Code"]);
                        int level = Convert.ToInt32(dr["ErrorLevel"]);
                        string unit = dr["Unit"].ToString().Trim();
                        string description = dr["Description"].ToString().Trim();
                        string comment = dr["Comment"].ToString().Trim();

                        tableData = new DataItem_ErrorList(id, code, (AlarmType)level, unit, description, comment);

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
        public List<DataItem_ErrorList> SelectListOrNull(int alarmCode)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM ErrorList Where Code = {0} ORDER BY ID ASC", alarmCode));

                DataTable table = new DataTable();
                List<DataItem_ErrorList> tableData = new List<DataItem_ErrorList>();

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
        public List<DataItem_ErrorList> SelectListOrNull(string unit)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM ErrorList Where Unit = '{0}' ORDER BY ID ASC", unit));

                DataTable table = new DataTable();
                List<DataItem_ErrorList> tableData = new List<DataItem_ErrorList>();

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
        public List<DataItem_ErrorList> SelectListOrNull(AlarmType level)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM ErrorList Where ErrorLevel = {0} ORDER BY ID ASC", Convert.ToInt32(level)));

                DataTable table = new DataTable();
                List<DataItem_ErrorList> tableData = new List<DataItem_ErrorList>();

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        tableData = GetListFromTableOrNull(table);
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
        private List<DataItem_ErrorList> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_ErrorList> tableData = new List<DataItem_ErrorList>();

                foreach (DataRow dr in table.Rows)
                {
                    int id = Convert.ToInt32(dr["ID"]);
                    int code = Convert.ToInt32(dr["Code"]);
                    int alarmLevel = Convert.ToInt32(dr["ErrorLevel"]);
                    string alarmUnit = dr["Unit"].ToString().Trim();
                    string description = dr["Description"].ToString().Trim();
                    string comment = (dr["Comment"] == null) ? string.Empty : dr["Comment"].ToString().Trim();

                    DataItem_ErrorList data = new DataItem_ErrorList(id, code, (AlarmType)alarmLevel, alarmUnit, description, comment);

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
        public int Insert(int id, int code, AlarmType alarmType, string unit, string description, string comment)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("INSERT INTO ErrorList "));
                queryString.Append("(ID, Code, ErrorLevel, Unit, Description, Comment) VALUES (");
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("{0}, ", code);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(alarmType));
                queryString.AppendFormat("'{0}', ", unit);
                queryString.AppendFormat("'{0}', ", description);
                queryString.AppendFormat("'{0}') ", comment);
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
        public int Insert(DataItem_ErrorList item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("INSERT INTO ErrorList "));
                queryString.Append("(ID, Code, ErrorLevel, Unit, Description, Comment) VALUES (");
                queryString.AppendFormat("{0}, ", item.ID);
                queryString.AppendFormat("{0}, ", item.Code);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Level));
                queryString.AppendFormat("'{0}', ", item.Unit);
                queryString.AppendFormat("'{0}', ", item.Description);
                queryString.AppendFormat("'{0}') ", item.Comment);
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
        public int Insert(List<DataItem_ErrorList> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("INSERT INTO ErrorList "));

                queryString.Append("(ID, Code, ErrorLevel, Unit, Description, Comment) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].ID);
                    queryString.AppendFormat("{0}, ", itemList[index].Code);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Level));
                    queryString.AppendFormat("'{0}', ", itemList[index].Unit);
                    queryString.AppendFormat("'{0}', ", itemList[index].Description);
                    queryString.AppendFormat("'{0}') ", itemList[index].Comment);

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
        public int Update(DataItem_ErrorList item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("UPDATE ErrorList SET "));

                queryString.AppendFormat("Code = {0}, ", item.Code);
                queryString.AppendFormat("ErrorLevel = {0}, ", Convert.ToInt32(item.Level));
                queryString.AppendFormat("Unit = '{0}', ", item.Unit);
                queryString.AppendFormat("Description = '{0}', ", item.Description);
                queryString.AppendFormat("Comment = '{0}' ", item.Comment);

                queryString.AppendFormat("WHERE ID = {0}", item.ID);

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
        public int Update(List<DataItem_ErrorList> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();

                    foreach (DataItem_ErrorList item in itemList)
                    {
                        queryString.AppendFormat("Code = {0}, ", item.Code);
                        queryString.AppendFormat("ErrorLevel = {0}, ", Convert.ToInt32(item.Level));
                        queryString.AppendFormat("Unit = '{0}', ", item.Unit);
                        queryString.AppendFormat("Description = '{0}', ", item.Description);
                        queryString.AppendFormat("Comment = '{0}' ", item.Comment);

                        queryString.AppendFormat("WHERE ID = {0}" + Environment.NewLine, item.ID);
                    }
                    if (CallBack_NonQuery == null)
                        rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                    else
                        rv = m_JobSession.ExecuteNonQuery(queryString.ToString(), CallBack_NonQuery);
                }
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
        public int Delete(int id)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("DELETE FROM ErrorList "));
                queryString.AppendFormat("WHERE ID = {0}", id);
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
