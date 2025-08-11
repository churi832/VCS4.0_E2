using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Forms;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_EventLog
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryEventLogEvent(object sender, object eventData);

        public event QueryEventLogEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_EventLog(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM EventLog";

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
        #endregion

        #region Methods - Select Query
        public List<DataItem_EventLog> SelectAllOrNull()
        {
            try
            {
                string queryString = string.Format("SELECT * FROM EventLog");

                DataTable table = new DataTable();
                List<DataItem_EventLog> tableData = new List<DataItem_EventLog>();

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

        private List<DataItem_EventLog> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_EventLog> itemList = new List<DataItem_EventLog>();

                foreach (DataRow dr in table.Rows)
                {
                    DateTime time = new DateTime();

                    if (dr["Time"] != null)
                    {
                        DateTime.TryParse(dr["Time"].ToString(), out time);
                    }

                    string module1 = (dr["Module1"] == null) ? string.Empty : dr["Module1"].ToString().Trim();
                    string module2 = (dr["Module2"] == null) ? string.Empty : dr["Module2"].ToString().Trim();
                    string message = (dr["Message"] == null) ? string.Empty : dr["Message"].ToString().Trim();

                    DataItem_EventLog data = new DataItem_EventLog(time, module1, module2, message);
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
        public int Insert(DateTime time, string module1, string module2, string message)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO EventLog ");
                queryString.Append("(Time, Module1, Module2, Message) VALUES (");
                queryString.AppendFormat("'{0}', ", time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("'{0}', ", module1);
                queryString.AppendFormat("'{0}', ", module2);
                queryString.AppendFormat("'{0}')", message);

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
        public int Insert(DataItem_EventLog item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO EventLog ");
                queryString.Append("(Time, Module1, Module2, Message) VALUES (");
                queryString.AppendFormat("'{0}', ", item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                queryString.AppendFormat("'{0}', ", item.Module1);
                queryString.AppendFormat("'{0}', ", item.Module2);
                queryString.AppendFormat("'{0}')", item.Message);

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
        public int Insert(List<DataItem_EventLog> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO EventLog ");

                queryString.Append("(Time, Module1, Module2, Message) VALUES (");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    queryString.AppendFormat("'{0}', ", itemList[index].Module1);
                    queryString.AppendFormat("'{0}', ", itemList[index].Module2);
                    queryString.AppendFormat("'{0}')", itemList[index].Message);

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
        #endregion

        #region Methods - Delete Query
        public int Delete(DataItem_EventLog item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM EventLog ");
                queryString.AppendFormat("WHERE [Time] = '{0}'", item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));

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
