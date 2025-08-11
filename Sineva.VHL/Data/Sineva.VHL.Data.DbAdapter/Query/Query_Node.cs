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
    public class Query_Node
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryNodeEvent(object sender, object eventData);
        public event QueryNodeEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_Node(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM Node";

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
        public int SelectMax()
        {
            try
            {
                string queryString = "SELECT MAX(NodeID) FROM Node";

                object result = m_JobSession.ExecuteScalar(queryString);

                if (result == null)
                {
                    return -1;
                }
                int count = Convert.ToInt32(result);
                return count;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int SelectCount(int nodeID)
        {
            try
            {
                int nodeCount = 0;
                string queryString = "SELECT COUNT(*) FROM Node WHERE NodeID =" + nodeID + "";

                object result = m_JobSession.ExecuteScalar(queryString);

                nodeCount = Convert.ToInt32(result);
                return nodeCount;
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
        public List<DataItem_Node> SelectAllOrNull()
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck "));
                queryString.AppendFormat("FROM Node ");
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_Node> tableData = new List<DataItem_Node>();

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
        public DataItem_Node SelectSingleOrNull(int id)
        {
            try
            {
                DataItem_Node tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck "));
                queryString.AppendFormat("FROM Node ");
                queryString.AppendFormat("WHERE NodeID = {0}", id);

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int nodeID = Convert.ToInt32(dr["NodeID"]);
                        bool useFlag = Convert.ToBoolean(dr["UseFlag"]);
                        int locationValue1 = Convert.ToInt32(dr["LocationValue1"]);
                        int locationValue2 = Convert.ToInt32(dr["LocationValue2"]);
                        int nodeType = Convert.ToInt32(dr["NodeType"]);
                        int ubsLevel = Convert.ToInt32(dr["UBSLevel"]);
                        int ubsCheckSensor = Convert.ToInt32(dr["UBSCheckSensor"]);
                        int jcsCheck = Convert.ToInt32(dr["JCSCheck"]);

                        tableData = new DataItem_Node(nodeID, useFlag, locationValue1, locationValue2, (NodeType)nodeType, ubsLevel, ubsCheckSensor, jcsCheck);

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
        public List<DataItem_Node> SelectListOrNull(int id)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck "));
                queryString.AppendFormat("FROM Node ");
                queryString.AppendFormat("WHERE NodeID like '%{0}%' ", id);
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_Node> tableData = new List<DataItem_Node>();

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
        public List<DataItem_Node> SelectListOrNull(NodeType type)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck "));
                queryString.AppendFormat("FROM Node ");
                queryString.AppendFormat("WHERE NodeType = {0} ", Convert.ToInt32(type));
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_Node> tableData = new List<DataItem_Node>();

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
        public List<DataItem_Node> SelectListOrNull(bool useFlag)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck "));
                queryString.AppendFormat("FROM Node ");
                queryString.AppendFormat("WHERE UseFlag = {0} ", useFlag.ToString());
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_Node> tableData = new List<DataItem_Node>();

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
        private List<DataItem_Node> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_Node> tableData = new List<DataItem_Node>();

                foreach (DataRow dr in table.Rows)
                {
                    int nodeID = Convert.ToInt32(dr["NodeID"]);
                    bool useFlag = Convert.ToBoolean(dr["UseFlag"]);
                    int locationValue1 = dr["LocationValue1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationValue1"]);
                    int locationValue2 = dr["LocationValue2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationValue2"]);
                    int nodeType = Convert.ToInt32(dr["NodeType"]);
                    int ubsLevel = Convert.ToInt32(dr["UBSLevel"]);
                    int ubsCheckSensor = Convert.ToInt32(dr["UBSCheckSensor"]);
                    int jcsCheck = Convert.ToInt32(dr["JCSCheck"]);

                    DataItem_Node data = new DataItem_Node(nodeID, useFlag, locationValue1, locationValue2, (NodeType)nodeType, ubsLevel, ubsCheckSensor, jcsCheck);

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
        public int Insert(int id, bool useFlag, int locationValue1, int locationValue2, NodeType type, int ubsLevel, int ubsCheckSensor, int jcs_check)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO Node ");
                queryString.Append("(NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCehckSensor, JCSCheck) VALUES (");
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("{0}, ", useFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", locationValue1);
                queryString.AppendFormat("{0}, ", locationValue2);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(type));
                queryString.AppendFormat("{0}, ", ubsLevel);
                queryString.AppendFormat("{0}, ", ubsCheckSensor);
                queryString.AppendFormat("{0})", jcs_check);
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
        public int Insert(DataItem_Node item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO Node ");
                queryString.Append("(NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck) VALUES (");
                queryString.AppendFormat("{0}, ", item.NodeID);
                queryString.AppendFormat("{0}, ", item.UseFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.LocationValue1);
                queryString.AppendFormat("{0}, ", item.LocationValue2);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("{0}, ", item.UBSLevel);
                queryString.AppendFormat("{0}, ", item.UBSCheckSensor);
                queryString.AppendFormat("{0}) ", item.JCSCheck);
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
        public int Insert(List<DataItem_Node> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO Node ");
                    queryString.Append("(NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");
                        queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                        queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].LocationValue1);
                        queryString.AppendFormat("{0}, ", itemList[index].LocationValue2);
                        queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Type));
                        queryString.AppendFormat("{0}, ", itemList[index].UBSLevel);
                        queryString.AppendFormat("{0}, ", itemList[index].UBSCheckSensor);
                        queryString.AppendFormat("{0}) ", itemList[index].JCSCheck);

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
                return -1;
            }
            return rv;
        }
        #endregion

        #region Methods - Update Query
        public int Update(DataItem_Node item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE Node SET ");

                queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                queryString.AppendFormat("LocationValue1 = {0}, ", item.LocationValue1);
                queryString.AppendFormat("LocationValue2 = {0}, ", item.LocationValue2);
                queryString.AppendFormat("NodeType = {0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("UBSLevel = {0}, ", item.UBSLevel);
                queryString.AppendFormat("UBSCheckSensor = {0}, ", item.UBSCheckSensor);
                queryString.AppendFormat("JCSCheck = {0} ", item.JCSCheck);

                queryString.AppendFormat("WHERE NodeID = {0}", item.NodeID);
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
        public int Update(List<DataItem_Node> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();

                    foreach (DataItem_Node item in itemList)
                    {
                        queryString.AppendFormat("UPDATE Node SET ");

                        queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                        queryString.AppendFormat("LocationValue1 = {0}, ", item.LocationValue1);
                        queryString.AppendFormat("LocationValue2 = {0}, ", item.LocationValue2);
                        queryString.AppendFormat("NodeType = {0}, ", Convert.ToInt32(item.Type));
                        queryString.AppendFormat("UBSLevel = {0}, ", item.UBSLevel);
                        queryString.AppendFormat("UBSCheckSensor = {0}, ", item.UBSCheckSensor);
                        queryString.AppendFormat("JCSCheck = {0} ", item.JCSCheck);

                        queryString.AppendFormat("WHERE NodeID = {0}" + Environment.NewLine, item.NodeID);
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
        #region Methods - Update And Insert Query
        private string UpdateQuerySql(List<DataItem_Node> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_Node item = itemList[i];
                    queryString.AppendFormat("UPDATE Node SET ");

                    queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                    queryString.AppendFormat("LocationValue1 = {0}, ", item.LocationValue1);
                    queryString.AppendFormat("LocationValue2 = {0}, ", item.LocationValue2);
                    queryString.AppendFormat("NodeType = {0}, ", Convert.ToInt32(item.Type));
                    queryString.AppendFormat("UBSLevel = {0}, ", item.UBSLevel);
                    queryString.AppendFormat("UBSCheckSensor = {0}, ", item.UBSCheckSensor);
                    queryString.AppendFormat("JCSCheck = {0} ", item.JCSCheck);

                    queryString.AppendFormat("WHERE NodeID = {0}" + Environment.NewLine, item.NodeID);
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

        private string InsertQuerySql(List<DataItem_Node> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO Node ");
                queryString.Append("(NodeID, UseFlag, LocationValue1, LocationValue2, NodeType, UBSLevel, UBSCheckSensor, JCSCheck) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                    queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].LocationValue1);
                    queryString.AppendFormat("{0}, ", itemList[index].LocationValue2);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Type));
                    queryString.AppendFormat("{0}, ", itemList[index].UBSLevel);
                    queryString.AppendFormat("{0}, ", itemList[index].UBSCheckSensor);
                    queryString.AppendFormat("{0}) ", itemList[index].JCSCheck);

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

        public int UpdateInsert(List<DataItem_Node> updateItemList, List<DataItem_Node> insertItemList)
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Node ");
                queryString.AppendFormat("WHERE NodeID = {0}", id);

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
                    StringBuilder queryString = new StringBuilder("DELETE FROM Node where NodeID = ");
                    queryString.Append(string.Join(" OR NodeID = ", ids));
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Node");
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
