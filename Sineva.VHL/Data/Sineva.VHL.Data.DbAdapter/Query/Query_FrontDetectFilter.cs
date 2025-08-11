using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;
using static System.Windows.Forms.LinkLabel;
using System.Reflection;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_FrontDetectFilter
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryFrontDetectFilterEvent(object sender, object eventData);

        public event QueryFrontDetectFilterEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_FrontDetectFilter(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM FrontDetectFilter";

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
        public List<DataItem_FrontDetectFilter> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM FrontDetectFilter ORDER BY ID ASC";

                DataTable table = new DataTable();
                List<DataItem_FrontDetectFilter> tableData = new List<DataItem_FrontDetectFilter>();

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
        private List<DataItem_FrontDetectFilter> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_FrontDetectFilter> tableData = new List<DataItem_FrontDetectFilter>();

                foreach (DataRow dr in table.Rows)
                {
                    int id = Convert.ToInt32(dr["ID"]);
                    int area = Convert.ToInt32(dr["Area"]);
                    int sensor_level = Convert.ToInt32(dr["SensorLevel"]);
                    int link_id = Convert.ToInt32(dr["LinkId"]);
                    bool useflag = Convert.ToBoolean(dr["UseFlag"]);
                    double left_bcr0 = dr["LBcrStart"] == DBNull.Value ? 0 : Convert.ToDouble(dr["LBcrStart"]);
                    double left_bcr1 = dr["LBcrEnd"] == DBNull.Value ? 0 : Convert.ToDouble(dr["LBcrEnd"]);
                    double right_bcr0 = dr["RBcrStart"] == DBNull.Value ? 0 : Convert.ToDouble(dr["RBcrStart"]);
                    double right_bcr1 = dr["RBcrEnd"] == DBNull.Value ? 0 : Convert.ToDouble(dr["RBcrEnd"]);
                    DataItem_FrontDetectFilter data = new DataItem_FrontDetectFilter(id, area, sensor_level, link_id, useflag, left_bcr0, left_bcr1, right_bcr0, right_bcr1);

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
        public int Insert(int id, int area, int sensor_level, int linkId, bool useflag, double left_bcr0, double left_bcr1, double right_bcr0, double right_bcr1)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO FrontDetectFilter ");
                queryString.Append("(ID, Area, SensorLevel, LinkId, UseFlag, LBcrStart, LBcrEnd, RBcrStart, RBcrEnd) VALUES (");
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("{0}, ", area);
                queryString.AppendFormat("{0}, ", sensor_level);
                queryString.AppendFormat("{0}, ", linkId);
                queryString.AppendFormat("{0}, ", useflag ? 1 : 0);
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
            }
            return rv;
        }
        public int Insert(DataItem_FrontDetectFilter item)
        {
            int rv = -1;
            try
            {
                int result = -1;

                StringBuilder queryString = new StringBuilder("INSERT INTO FrontDetectFilter ");
                queryString.Append("(ID, Area, SensorLevel, LinkId, UseFlag, LBcrStart, LBcrEnd, RBcrStart, RBcrEnd) VALUES (");
                queryString.AppendFormat("{0}, ", item.ID);
                queryString.AppendFormat("{0}, ", item.Area);
                queryString.AppendFormat("{0}, ", item.SensorLevel);
                queryString.AppendFormat("{0}, ", item.LinkId);
                queryString.AppendFormat("{0}, ", item.UseFlag ? 1 : 0);
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
        public int Insert(List<DataItem_FrontDetectFilter> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO FrontDetectFilter ");
                    queryString.Append("(ID, Area, SensorLevel, LinkId, UseFlag, LBcrStart, LBcrEnd, RBcrStart, RBcrEnd) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");

                        queryString.AppendFormat("{0}, ", itemList[index].ID);
                        queryString.AppendFormat("{0}, ", itemList[index].Area);
                        queryString.AppendFormat("{0}, ", itemList[index].SensorLevel);
                        queryString.AppendFormat("{0}, ", itemList[index].LinkId);
                        queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].LeftBcrStart);
                        queryString.AppendFormat("{0}, ", itemList[index].LeftBcrEnd);
                        queryString.AppendFormat("{0}, ", itemList[index].RightBcrStart);
                        queryString.AppendFormat("{0})", itemList[index].RightBcrEnd);

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
        private string InsertQuerySql(List<DataItem_FrontDetectFilter> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO FrontDetectFilter ");
                queryString.Append("(ID, Area, SensorLevel, LinkId, UseFlag, LBcrStart, LBcrEnd, RBcrStart, RBcrEnd) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");

                    queryString.AppendFormat("{0}, ", itemList[index].ID);
                    queryString.AppendFormat("{0}, ", itemList[index].Area);
                    queryString.AppendFormat("{0}, ", itemList[index].SensorLevel);
                    queryString.AppendFormat("{0}, ", itemList[index].LinkId);
                    queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].LeftBcrStart);
                    queryString.AppendFormat("{0}, ", itemList[index].LeftBcrEnd);
                    queryString.AppendFormat("{0}, ", itemList[index].RightBcrStart);
                    queryString.AppendFormat("{0})", itemList[index].RightBcrEnd);

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
        public int Update(DataItem_FrontDetectFilter item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE FrontDetectFilter SET ");
                queryString.AppendFormat("Area = {0}, ", item.Area);
                queryString.AppendFormat("SensorLevel = {0}, ", item.SensorLevel);
                queryString.AppendFormat("LinkId = {0}, ", item.LinkId);
                queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                queryString.AppendFormat("LBcrStart = {0}, ", item.LeftBcrStart);
                queryString.AppendFormat("LBcrEnd = {0}, ", item.LeftBcrEnd);
                queryString.AppendFormat("RBcrStart = {0}, ", item.RightBcrStart);
                queryString.AppendFormat("RBcrEnd = {0} ", item.RightBcrEnd);

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
        public int Update(List<DataItem_FrontDetectFilter> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();

                    foreach (DataItem_FrontDetectFilter item in itemList)
                    {
                        queryString.AppendFormat("UPDATE FrontDetectFilter SET ");

                        queryString.AppendFormat("Area = {0}, ", item.Area);
                        queryString.AppendFormat("SensorLevel = {0}, ", item.SensorLevel);
                        queryString.AppendFormat("LinkId = {0}, ", item.LinkId);
                        queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                        queryString.AppendFormat("LBcrStart = {0}, ", item.LeftBcrStart);
                        queryString.AppendFormat("LBcrEnd = {0}, ", item.LeftBcrEnd);
                        queryString.AppendFormat("RBcrStart = {0}, ", item.RightBcrStart);
                        queryString.AppendFormat("RBcrEnd = {0} ", item.RightBcrEnd);

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
        private string UpdateQuerySql(List<DataItem_FrontDetectFilter> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_FrontDetectFilter item = itemList[i];
                    queryString.AppendFormat("UPDATE FrontDetectFilter SET ");

                    queryString.AppendFormat("Area = {0}, ", item.Area);
                    queryString.AppendFormat("SensorLevel = {0}, ", item.SensorLevel);
                    queryString.AppendFormat("LinkId = {0}, ", item.LinkId);
                    queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                    queryString.AppendFormat("LBcrStart = {0}, ", item.LeftBcrStart);
                    queryString.AppendFormat("LBcrEnd = {0}, ", item.LeftBcrEnd);
                    queryString.AppendFormat("RBcrStart = {0}, ", item.RightBcrStart);
                    queryString.AppendFormat("RBcrEnd = {0} ", item.RightBcrEnd);

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
        public int UpdateInsert(List<DataItem_FrontDetectFilter> updateItemList, List<DataItem_FrontDetectFilter> insertItemList)
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
                StringBuilder queryString = new StringBuilder("DELETE FROM FrontDetectFilter ");
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
        public int Delete(List<int> ids)
        {
            int rv = -1;
            try
            {
                if (ids.Count < 1) return 0;
                StringBuilder queryString = new StringBuilder("DELETE FROM FrontDetectFilter where ID = ");
                queryString.Append(string.Join(" OR ID = ", ids));
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
        public int Delete()
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM FrontDetectFilter");
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
