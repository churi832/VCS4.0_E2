using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;
using System.Windows.Forms;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_PIODevice
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryPIODeviceEvent(object sender, object eventData);
        public event QueryPIODeviceEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion


        #region Constructors
        public Query_PIODevice(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion


        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM PIODevice";

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
                string queryString = "SELECT MAX(NodeID) FROM PIODevice";

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
                int portCount = 0;
                string queryString = "SELECT COUNT(*) FROM PIODevice WHERE NodeID =" + nodeID + "";

                object result = m_JobSession.ExecuteScalar(queryString);

                portCount = Convert.ToInt32(result);
                return portCount;
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
        public List<DataItem_PIODevice> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM PIODevice ORDER BY NodeID ASC";

                DataTable table = new DataTable();
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

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
        public List<DataItem_PIODevice> SelectListOrNullByNodeID(int nodeId)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * "));
                queryString.AppendFormat("FROM PIODevice ");
                queryString.AppendFormat("WHERE NodeID like '%{0}%' ", nodeId);
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

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
        public DataItem_PIODevice SelectSingleOrNull(int id)
        {
            try
            {
                DataItem_PIODevice tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM PIODevice WHERE NodeID = {0}", id));

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int _DeviceType = Convert.ToInt32(dr["DeviceType"]);
                        int _NodeID = Convert.ToInt32(dr["NodeID"]);
                        int _PIOID = Convert.ToInt32(dr["PIOID"]);
                        int _PIOCH = Convert.ToInt32(dr["PIOCH"]);

                        tableData = new DataItem_PIODevice()
                        {
                            NodeID = _NodeID,
                            DeviceType = (PIODeviceType)_DeviceType,
                            PIOID = _PIOID,
                            PIOCH = _PIOCH,
                        };

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
        public List<DataItem_PIODevice> SelectListOrNull(PIODeviceType type)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM PIODevice Where DeviceType = {0} ORDER BY NodeID ASC", Convert.ToInt32(type)));

                DataTable table = new DataTable();
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

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
        public List<DataItem_PIODevice> SelectListOrNull(bool useFlag)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM PIODevice Where OffsetUsed = '{0}' ORDER BY NodeID ASC", useFlag.ToString()));

                DataTable table = new DataTable();
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

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
        public List<DataItem_PIODevice> SelectListOrNull(int nodeID)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM PIODevice Where NodeID = {0} ORDER BY NodeID ASC", nodeID));

                DataTable table = new DataTable();
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

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
        private List<DataItem_PIODevice> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_PIODevice> tableData = new List<DataItem_PIODevice>();

                foreach (DataRow dr in table.Rows)
                {
                    int _DeviceType = Convert.ToInt32(dr["DeviceType"]);
                    int _NodeID = Convert.ToInt32(dr["NodeID"]);
                    int _PIOID = Convert.ToInt32(dr["PIOID"]);
                    int _PIOCH = Convert.ToInt32(dr["PIOCH"]);

                    DataItem_PIODevice data = new DataItem_PIODevice()
                    {
                        NodeID = _NodeID,
                        PIOID = _PIOID,
                        PIOCH = _PIOCH,
                        DeviceType = (PIODeviceType)_DeviceType,
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
        public int Insert(int nodeID, int pioID, int pioCH, PIODeviceType type)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO PIODevice ");
                queryString.Append("(NodeID, PIOID, PIOCH, DeviceType) VALUES (");
                queryString.AppendFormat("{0}, ", nodeID);
                queryString.AppendFormat("{0}, ", pioID);
                queryString.AppendFormat("{0}, ", pioCH);
                queryString.AppendFormat("{0}, ", (int)type);
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
        public int Insert(DataItem_PIODevice item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO PIODevice ");
                queryString.Append("(NodeID, PIOID, PIOCH, DeviceType) VALUES (");
                queryString.AppendFormat("{0}, ", item.NodeID);
                queryString.AppendFormat("{0}, ", item.PIOID);
                queryString.AppendFormat("{0}, ", item.PIOCH);
                queryString.AppendFormat("{0}, ", (int)item.DeviceType);
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
        public int Insert(List<DataItem_PIODevice> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO PIODevice ");
                    queryString.Append("(NodeID, PIOID, PIOCH, DeviceType) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");
                        queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOID);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOCH);
                        queryString.AppendFormat("{0}, ", (int)itemList[index].DeviceType);

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
        private string InsertQuerySql(List<DataItem_PIODevice> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO PIODevice ");
                queryString.Append("(NodeID, PIOID, PIOCH, DeviceType) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                    queryString.AppendFormat("{0}, ", itemList[index].PIOID);
                    queryString.AppendFormat("{0}, ", itemList[index].PIOCH);
                    queryString.AppendFormat("{0}, ", (int)itemList[index].DeviceType);

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
        public int Update(DataItem_PIODevice item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE PIODevice SET ");
                queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                queryString.AppendFormat("DeviceType = {0} ", (int)item.DeviceType);
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
        public int Update(List<DataItem_PIODevice> items)
        {
            int rv = -1;
            try
            {
                if (items.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();
                    foreach (DataItem_PIODevice item in items)
                    {
                        queryString.AppendFormat("UPDATE PIODevice SET ");
                        queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                        queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                        queryString.AppendFormat("DeviceType = {0} ", (int)item.DeviceType);
                        queryString.AppendFormat("WHERE NodeID = {0}", item.NodeID);
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
        private string UpdateQuerySql(List<DataItem_PIODevice> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_PIODevice item = itemList[i];
                    queryString.AppendFormat("UPDATE PIODevice SET ");

                    queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                    queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                    queryString.AppendFormat("DeviceType = {0}, ", (int)item.DeviceType);

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
        #endregion

        #region Methods - Update And Insert Query
        public int UpdateInsert(List<DataItem_PIODevice> updateItemList, List<DataItem_PIODevice> insertItemList)
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
                StringBuilder queryString = new StringBuilder("DELETE FROM PIODevice ");
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
                    StringBuilder queryString = new StringBuilder("DELETE FROM PIODevice where NodeID = ");
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
                StringBuilder queryString = new StringBuilder("DELETE FROM PIODevice");
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
