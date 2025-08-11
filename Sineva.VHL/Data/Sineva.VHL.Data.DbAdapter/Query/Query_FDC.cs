using Sineva.VHL.Library;
using Sineva.VHL.Library.DBProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_FDC
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryFDCEvent(object sender, object eventData);

        public event QueryFDCEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_FDC(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int Count(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM FDCItem";

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
        public List<DataItem_FDC> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM FDCItem";

                DataTable table = new DataTable();
                List<DataItem_FDC> tableData = new List<DataItem_FDC>();

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
        private List<DataItem_FDC> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_FDC> tableData = new List<DataItem_FDC>();

                foreach (DataRow dr in table.Rows)
                {
                    DataItem_FDC data = new DataItem_FDC()
                    {
                        ID = Convert.ToInt32(dr["ID"].ToString()),
                        Unit = (FDC_Unit)Convert.ToInt32(dr["Unit"].ToString()),
                        DataType = (FDC_DataType)Convert.ToInt32(dr["DataType"].ToString()),
                        Name = dr["Name"].ToString(),
                        Discription = dr["Discription"].ToString(),
                        DecimalPoint = Convert.ToInt32(dr["DecimalPoint"].ToString()),
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
        public int Insert(int ID, FDC_Unit Unit, FDC_DataType DataType, string Name, string Discription, int DecimalPoint, double ReportingCycle)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO FDCItem ");
                queryString.Append("(ID, Unit, DataType, Name, Discription, DecimalPoint, ReportingCycle) VALUES (");
                queryString.AppendFormat("'{0}', ", ID);
                queryString.AppendFormat("'{0}', ", Convert.ToInt32(Unit));
                queryString.AppendFormat("'{0}', ", Convert.ToInt32(DataType));
                queryString.AppendFormat("{0}, ", Name);
                queryString.AppendFormat("{0}, ", Discription);
                queryString.AppendFormat("{0}, ", DecimalPoint);
                queryString.AppendFormat("{0}') ", ReportingCycle);
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
        public int Insert(DataItem_FDC item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO FDCItem ");
                queryString.Append("(ID, Unit, DataType, Name, Discription, DecimalPoint, ReportingCycle) VALUES (");
                queryString.AppendFormat("'{0}', ", item.ID);
                queryString.AppendFormat("'{0}', ", Convert.ToInt32(item.Unit));
                queryString.AppendFormat("'{0}', ", Convert.ToInt32(item.DataType));
                queryString.AppendFormat("{0}, ", item.Name);
                queryString.AppendFormat("{0}, ", item.Discription);
                queryString.AppendFormat("{0}, ", item.DecimalPoint);
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
        public int Insert(List<DataItem_FDC> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO FDCItem ");

                queryString.Append("(ID, Unit, DataType, Name, Discription, DecimalPoint, ReportingCycle) VALUES");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(ID, Unit, DataType, Name, Discription, DecimalPoint, ReportingCycle) VALUES (");
                    queryString.AppendFormat("'{0}', ", itemList[index].ID);
                    queryString.AppendFormat("'{0}', ", Convert.ToInt32(itemList[index].Unit));
                    queryString.AppendFormat("'{0}', ", Convert.ToInt32(itemList[index].DataType));
                    queryString.AppendFormat("{0}, ", itemList[index].Name);
                    queryString.AppendFormat("{0}, ", itemList[index].Discription);
                    queryString.AppendFormat("{0}')'", itemList[index].DecimalPoint);

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
        public int Update(DataItem_FDC item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE FDCItem SET ");

                queryString.Append("(ID, Unit, DataType, Name, Discription, DecimalPoint, ReportingCycle) VALUES (");

                queryString.AppendFormat("Unit = '{0}' ,", Convert.ToInt32(item.Unit));
                queryString.AppendFormat("DataType = {0:F1} ,", Convert.ToInt32(item.DataType));
                queryString.AppendFormat("Name = {0:F1} ,", item.Name);
                queryString.AppendFormat("Discription = {0},", item.Discription);
                queryString.AppendFormat("DecimalPoint = '{0}' ", item.DecimalPoint);
                queryString.AppendFormat("WHERE ID = '{0}'", item.ID);
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
        public int Delete(int ID)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM FDCItem ");
                queryString.AppendFormat("WHERE ID = '{0}'", ID);
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
