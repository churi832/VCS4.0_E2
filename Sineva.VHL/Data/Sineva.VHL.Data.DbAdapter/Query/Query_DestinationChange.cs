using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_DestinationChange
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryDestinationChangeEvent(object sender, object eventData);

        public event QueryDestinationChangeEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_DestinationChange(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM DestinationChange";

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
        public List<DataItem_DestinationChange> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM DestinationChange ORDER BY ID ASC";

                DataTable table = new DataTable();
                List<DataItem_DestinationChange> tableData = new List<DataItem_DestinationChange>();

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

        private List<DataItem_DestinationChange> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_DestinationChange> tableData = new List<DataItem_DestinationChange>();

                foreach (DataRow dr in table.Rows)
                {
                    int id = Convert.ToInt32(dr["ID"]);
                    int start = Convert.ToInt32(dr["StartNode"]);
                    int end = Convert.ToInt32(dr["EndNode"]);
                    DataItem_DestinationChange data = new DataItem_DestinationChange(id, start, end);

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
        public int Insert(DataItem_DestinationChange item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO DestinationChange ");
                queryString.Append("(ID, StartNode, EndNode) VALUES (");
                queryString.AppendFormat("{0}, ", item.ID);
                queryString.AppendFormat("{0}, ", item.StartNode);
                queryString.AppendFormat("{0}, ", item.EndNode);
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
        public int Update(DataItem_DestinationChange item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE DestinationChange SET ");

                queryString.AppendFormat("StartNode = {0}, ", item.StartNode);
                queryString.AppendFormat("EndNode = {0} ", item.EndNode);

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
        public int Update(List<DataItem_DestinationChange> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();

                    foreach (DataItem_DestinationChange item in itemList)
                    {
                        queryString.AppendFormat("UPDATE DestinationChange SET ");
                        queryString.AppendFormat("StartNode = {0}, ", item.StartNode);
                        queryString.AppendFormat("EndNode = {0} ", item.EndNode);
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
                StringBuilder queryString = new StringBuilder("DELETE FROM DestinationChange ");
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
                return -1;
            }
            return rv;
        }
        #endregion

    }
}
