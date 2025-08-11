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
    public class Query_UserList
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryUserListEvent(object sender, object eventData);
        public event QueryUserListEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_UserList(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM UserList";

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
        public int SelectCount(string userName)
        {
            try
            {
                int userCount = 0;
                string queryString = "SELECT COUNT(*) FROM UserList WHERE UserName='" + userName + "'";

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString);

                if (result == null)
                {
                    return 0;
                }

                userCount = Convert.ToInt32(result);
                return userCount;
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
        public List<DataItem_UserInfo> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM UserList ORDER BY UserName ASC";

                DataTable table = new DataTable();
                List<DataItem_UserInfo> tableData = new List<DataItem_UserInfo>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString, ref table);

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
        public List<DataItem_UserInfo> SelectOrNull(int id)
        {
            try
            {
                //int no = 1;
                DataTable table = new DataTable();
                List<DataItem_UserInfo> tableData = new List<DataItem_UserInfo>();

                //DateTime startTime = DateTime.Now.AddDays(-30);

                //SqlCommand command = new SqlCommand();
                //command.CommandText = "SELECT_EVENTHISTORY";
                //command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.Add("@EVENT_ID", SqlDbType.Int);
                //command.Parameters.Add("@START_TIME", SqlDbType.DateTime);
                //command.Parameters.Add("@END_TIME", SqlDbType.DateTime);
                //command.Parameters["@EVENT_ID"].Value = id;
                //command.Parameters["@START_TIME"].Value = startTime;
                //command.Parameters["@END_TIME"].Value = DateTime.Now;

                //int returnValue = JobSession.GetInstanceOrNull().ExecuteProcedure(command, ref table);

                //if (returnValue >= 0)
                //{
                //    foreach (DataRow dr in table.Rows)
                //    {
                //        DBData_EventHistory data = new DBData_EventHistory();
                //        DateTime eventTime = DateTime.Now;
                //        DateTime.TryParse(dr["EventTime"].ToString(), out eventTime);
                //        data.No = no;
                //        data.EventTime = eventTime;
                //        data.EventID = Convert.ToInt32(dr["EventNo"]);
                //        data.EventDescription = dr["EventDescription"].ToString();

                //        tableData.Add(data);

                //        no++;
                //    }
                //}

                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public DataItem_UserInfo SelectUserOrNull(string name)
        {
            try
            {
                DataItem_UserInfo userInfo = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM UserList Where UserName = '{0}'", name));

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        userInfo = new DataItem_UserInfo();

                        userInfo.UserName = dr["UserName"].ToString();
                        userInfo.Password = dr["Password"].ToString();
                        userInfo.Department = dr["Department"].ToString();
                        userInfo.Description = dr["Description"].ToString();
                        userInfo.Level = (AuthorizationLevel)Convert.ToInt32(dr["AuthorizationLevel"]);

                        break;
                    }
                }

                return userInfo;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        private List<DataItem_UserInfo> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_UserInfo> tableData = new List<DataItem_UserInfo>();

                foreach (DataRow dr in table.Rows)
                {
                    string userName = dr["UserName"].ToString();
                    string password = dr["Password"].ToString();
                    string department = (dr["Department"] == null) ? string.Empty : dr["Department"].ToString();
                    string description = (dr["Description"] == null) ? string.Empty : dr["Description"].ToString();
                    int level = Convert.ToInt32(dr["AuthorizationLevel"]);
                    string clientIp = (dr["ClientIp"] == null) ? string.Empty : dr["ClientIp"].ToString();
                    DateTime loginTime = (string.IsNullOrEmpty(dr["LoginTime"].ToString())) ? DateTime.Now : Convert.ToDateTime(dr["LoginTime"].ToString());

                    DataItem_UserInfo data = new DataItem_UserInfo()
                    {
                        UserName = userName,
                        Password = password,
                        Department = department,
                        Description = description,
                        Level = (AuthorizationLevel)level,
                        ClientIp = clientIp,
                        LoginTime = loginTime,
                    };

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
        public int Insert(string name, string password, string department, string description, AuthorizationLevel level)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO UserList ");
                queryString.Append("(UserName, Password, Department, AuthorizationLevel, Description) VALUES (");
                queryString.AppendFormat("'{0}', ", name);
                queryString.AppendFormat("'{0}', ", password);
                queryString.AppendFormat("'{0}', ", department);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(level));
                queryString.AppendFormat("'{0}') ", description);

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
        public int Insert(DataItem_UserInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO UserList ");
                queryString.Append("(UserName, Password, Department, AuthorizationLevel, Description) VALUES (");
                queryString.AppendFormat("'{0}', ", item.UserName);
                queryString.AppendFormat("'{0}', ", item.Password);
                queryString.AppendFormat("'{0}', ", item.Department);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Level));
                queryString.AppendFormat("'{0}') ", item.Description);

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
        public int Insert(List<DataItem_UserInfo> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO UserList ");

                queryString.Append("(UserName, Password, Department, AuthorizationLevel, Description) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].UserName);
                    queryString.AppendFormat("'{0}', ", itemList[index].Password);
                    queryString.AppendFormat("'{0}', ", itemList[index].Department);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Level));
                    queryString.AppendFormat("'{0}')", itemList[index].Department);

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
        public int Update(DataItem_UserInfo item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE UserList SET ");

                queryString.AppendFormat("Password = '{0}' ,", item.Password);
                queryString.AppendFormat("Department = '{0}' ,", item.Department);
                queryString.AppendFormat("AuthorizationLevel = {0} ,", Convert.ToInt32(item.Level));
                queryString.AppendFormat("Description = '{0}' ", item.Description);
                queryString.AppendFormat("WHERE UserName = '{0}'", item.UserName);

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
        public int UpdateClientIp(string userName, string clientIp)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE UserList SET ");

                queryString.AppendFormat("ClientIp = '{0}' ,", clientIp);
                queryString.AppendFormat("LoginTime = '{0}' ", DateTime.Now.ToString());
                queryString.AppendFormat("WHERE UserName = '{0}'", userName);

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
        public int Delete(string name)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM UserList ");
                queryString.AppendFormat("WHERE UserName = '{0}'", name);

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
