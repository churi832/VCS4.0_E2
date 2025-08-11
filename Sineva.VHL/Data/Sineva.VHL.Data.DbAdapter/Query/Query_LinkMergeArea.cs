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
    public class Query_LinkMergeArea
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryLinkMergeAreaEvent(object sender, object eventData);
        public event QueryLinkMergeAreaEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_LinkMergeArea(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM LinkMergeArea";

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
                string queryString = "SELECT MAX(LinkID) FROM LinkMergeArea";

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
        #endregion

        #region Methods - Select Query
        public List<DataItem_LinkMergeArea> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM LinkMergeArea";

                DataTable table = new DataTable();
                List<DataItem_LinkMergeArea> tableData = new List<DataItem_LinkMergeArea>();

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
        private List<DataItem_LinkMergeArea> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_LinkMergeArea> tableData = new List<DataItem_LinkMergeArea>();

                foreach (DataRow dr in table.Rows)
                {
                    int areaId = Convert.ToInt32(dr["ID"]);
                    double velocity = Convert.ToDouble(dr["AreaVelocity"].ToString());
                    string nodes = Convert.ToString(dr["NodeList"]);
                    DataItem_LinkMergeArea data = new DataItem_LinkMergeArea(areaId, velocity, nodes);

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

        #region Methods - Update
        public int Update(DataItem_LinkMergeArea item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE LinkMergeArea SET ");
                queryString.AppendFormat("AreaVelocity = {0}, ", item.AreaVelocity);
                queryString.AppendFormat("NodeList = '{0}' ", string.Join(",", item.NodeIdList));
                queryString.AppendFormat("WHERE ID = {0}", item.AreaID);

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
        public int Update(List<DataItem_LinkMergeArea> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();
                    foreach (DataItem_LinkMergeArea item in itemList)
                    {
                        queryString.AppendFormat("UPDATE LinkMergeArea SET ");
                        queryString.AppendFormat("AreaVelocity = {0}, ", item.AreaVelocity);
                        queryString.AppendFormat("NodeList = '{0}' ", string.Join(",", item.NodeIdList));
                        queryString.AppendFormat("WHERE ID = {0}" + Environment.NewLine, item.AreaID);
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
    }
}
