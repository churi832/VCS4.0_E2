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
    public class Query_VelocityLimit
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryVelocityEvent(object sender, object eventData);
        public event QueryVelocityEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_VelocityLimit(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM VelocityLimit";

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
        public List<DataItem_VelocityLimit> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM VelocityLimit ORDER BY ID ASC";

                DataTable table = new DataTable();
                List<DataItem_VelocityLimit> tableData = new List<DataItem_VelocityLimit>();

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
        private List<DataItem_VelocityLimit> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_VelocityLimit> tableData = new List<DataItem_VelocityLimit>();

                foreach (DataRow dr in table.Rows)
                {
                    int id = Convert.ToInt32(dr["ID"]);
                    int link_id = Convert.ToInt32(dr["LinkID"]);
                    int to_link_id = Convert.ToInt32(dr["ToLinkID"]);
                    int maxVelocity = Convert.ToInt32(dr["MaxVelocity"]);
                    double left_bcr0 = dr["LeftBcrStart"] == DBNull.Value ? 0 : Convert.ToDouble(dr["LeftBcrStart"]);
                    double left_bcr1 = dr["LeftBcrEnd"] == DBNull.Value ? 0 : Convert.ToDouble(dr["LeftBcrEnd"]);
                    double right_bcr0 = dr["RightBcrStart"] == DBNull.Value ? 0 : Convert.ToDouble(dr["RightBcrStart"]);
                    double right_bcr1 = dr["RightBcrEnd"] == DBNull.Value ? 0 : Convert.ToDouble(dr["RightBcrEnd"]);
                    DataItem_VelocityLimit data = new DataItem_VelocityLimit(id, link_id, to_link_id, maxVelocity, left_bcr0, left_bcr1, right_bcr0, right_bcr1);

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
        public int Insert(int id, int link_id, int to_link_id, int maxVelocity, double left_bcr0, double left_bcr1, double right_bcr0, double right_bcr1)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO VelocityLimit ");
                queryString.Append("(ID, LinkID, ToLinkID, MaxVelocity, LeftBcrStart, LeftBcrEnd, RightBcrStart, RightBcrEnd) VALUES (");
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("{0}, ", link_id);
                queryString.AppendFormat("{0}, ", to_link_id);
                queryString.AppendFormat("{0}, ", maxVelocity);
                queryString.AppendFormat("{0}, ", left_bcr0);
                queryString.AppendFormat("{0}, ", left_bcr1);
                queryString.AppendFormat("{0}, ", right_bcr0);
                queryString.AppendFormat("{0})", right_bcr1);

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
        public int Insert(DataItem_VelocityLimit item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO VelocityLimit ");
                queryString.Append("(ID , LinkID, ToLinkID, MaxVelocity, LeftBcrStart, LeftBcrEnd, RightBcrStart, RightBcrEnd) VALUES (");
                queryString.AppendFormat("{0}, ", item.ID);
                queryString.AppendFormat("{0}, ", item.LinkId);
                queryString.AppendFormat("{0}, ", item.ToLinkId);
                queryString.AppendFormat("{0}, ", item.MaxVelocity);
                queryString.AppendFormat("{0}, ", item.LeftBcrStart);
                queryString.AppendFormat("{0}, ", item.LeftBcrEnd);
                queryString.AppendFormat("{0}, ", item.RightBcrStart);
                queryString.AppendFormat("{0})", item.RightBcrEnd);

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
        public int Insert(List<DataItem_VelocityLimit> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO VelocityLimit ");
                    queryString.Append("(ID, LinkID, ToLinkID, MaxVelocity, LeftBcrStart, LeftBcrEnd, RightBcrStart, RightBcrEnd) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");
                        queryString.AppendFormat("{0}, ", itemList[index].ID);
                        queryString.AppendFormat("{0}, ", itemList[index].LinkId);
                        queryString.AppendFormat("{0}, ", itemList[index].MaxVelocity);
                        queryString.AppendFormat("{0}, ", itemList[index].LeftBcrStart);
                        queryString.AppendFormat("{0}, ", itemList[index].LeftBcrEnd);
                        queryString.AppendFormat("{0}, ", itemList[index].RightBcrStart);
                        queryString.AppendFormat("{0}, ", itemList[index].RightBcrEnd);

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
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return rv;
        }
        private string InsertQuerySql(List<DataItem_VelocityLimit> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO VelocityLimit ");
                queryString.Append("(ID, LinkID, ToLinkID, MaxVelocity, LeftBcrStart, LeftBcrEnd, RightBcrStart, RightBcrEnd) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].ID);
                    queryString.AppendFormat("{0}, ", itemList[index].LinkId);
                    queryString.AppendFormat("{0}, ", itemList[index].ToLinkId);
                    queryString.AppendFormat("{0}, ", itemList[index].MaxVelocity);
                    queryString.AppendFormat("{0}, ", itemList[index].LeftBcrStart);
                    queryString.AppendFormat("{0}, ", itemList[index].LeftBcrEnd);
                    queryString.AppendFormat("{0}, ", itemList[index].RightBcrStart);
                    queryString.AppendFormat("{0}, ", itemList[index].RightBcrEnd);

                    if (index < itemList.Count - 1)
                    {
                        queryString.Append(", ");
                    }
                }

                return queryString.ToString();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return string.Empty;
            }
        }
        #endregion

        #region Methods - Update Query
        public int Update(DataItem_VelocityLimit item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE VelocityLimit SET ");

                queryString.AppendFormat("LinkID = {0}, ", item.LinkId);
                queryString.AppendFormat("ToLinkID = {0}, ", item.ToLinkId);
                queryString.AppendFormat("MaxVelocity = {0}, ", item.MaxVelocity);
                queryString.AppendFormat("LeftBcrStart = {0}, ", item.LeftBcrStart);
                queryString.AppendFormat("LeftBcrEnd = {0}, ", item.LeftBcrEnd);
                queryString.AppendFormat("RightBcrStart = {0}, ", item.RightBcrStart);
                queryString.AppendFormat("RightBcrEnd = {0} ", item.RightBcrEnd);

                queryString.AppendFormat("WHERE ID = {0} ", item.ID);

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
        public int Update(List<DataItem_VelocityLimit> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();

                    foreach (DataItem_VelocityLimit item in itemList)
                    {
                        queryString.AppendFormat("UPDATE VelocityLimit SET ");

                        queryString.AppendFormat("LinkID = {0}, ", item.LinkId);
                        queryString.AppendFormat("ToLinkID = {0}, ", item.ToLinkId);
                        queryString.AppendFormat("MaxVelocity = {0}, ", item.MaxVelocity);
                        queryString.AppendFormat("LeftBcrStart = {0}, ", item.LeftBcrStart);
                        queryString.AppendFormat("LeftBcrEnd = {0}, ", item.LeftBcrEnd);
                        queryString.AppendFormat("RightBcrStart = {0}, ", item.RightBcrStart);
                        queryString.AppendFormat("RightBcrEnd = {0} ", item.RightBcrEnd);

                        queryString.AppendFormat("WHERE ID = {0} " + Environment.NewLine, item.ID);
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
        private string UpdateQuerySql(List<DataItem_VelocityLimit> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_VelocityLimit item = itemList[i];
                    queryString.AppendFormat("UPDATE VelocityLimit SET ");

                    queryString.AppendFormat("LinkID = {0}, ", item.LinkId);
                    queryString.AppendFormat("ToLinkID = {0}, ", item.ToLinkId);
                    queryString.AppendFormat("MaxVelocity = {0}, ", item.MaxVelocity);
                    queryString.AppendFormat("LeftBcrStart = {0}, ", item.LeftBcrStart);
                    queryString.AppendFormat("LeftBcrEnd = {0}, ", item.LeftBcrEnd);
                    queryString.AppendFormat("RightBcrStart = {0}, ", item.RightBcrStart);
                    queryString.AppendFormat("RightBcrEnd = {0} ", item.RightBcrEnd);

                    queryString.AppendFormat("WHERE ID = {0} " + Environment.NewLine, item.ID);
                }

                return queryString.ToString();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return string.Empty;
            }
        }
        #endregion
        #region Methods - Update And Insert Query
        public int UpdateInsert(List<DataItem_VelocityLimit> updateItemList, List<DataItem_VelocityLimit> insertItemList)
        {
            int rv = -1;
            try
            {
                if (updateItemList.Count > 0 || insertItemList.Count > 0)
                {
                    string updateSql = UpdateQuerySql(updateItemList);
                    string insertSql = InsertQuerySql(insertItemList);

                    if (CallBack_NonQuery == null)
                        rv = m_JobSession.ExecuteNonQuery(updateSql + insertSql);
                    else
                        rv = m_JobSession.ExecuteNonQuery(updateSql + insertSql, CallBack_NonQuery);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
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
                StringBuilder queryString = new StringBuilder("DELETE FROM VelocityLimit ");
                queryString.AppendFormat("WHERE LinkID = {0}", id);

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
        public int Delete(List<int> ids)
        {
            int rv = -1;
            try
            {
                if (ids.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("DELETE FROM VelocityLimit where ID = ");
                    queryString.Append(string.Join(" OR ID = ", ids));
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
        public int Delete()
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM VelocityLimit");
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
