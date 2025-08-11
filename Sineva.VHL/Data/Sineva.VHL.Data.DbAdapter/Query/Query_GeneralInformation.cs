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
    public class Query_GeneralInformation
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryGeneralInformationEvent(object sender, object eventData);

        public event QueryGeneralInformationEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_GeneralInformation(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM GeneralInformation";

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
        public int SelectCount(GeneralInformationItemName itemName)
        {
            try
            {
                int itemCount = 0;
                string queryString = "SELECT COUNT(*) FROM GeneralInformation WHERE ItenName='" + itemName.ToString() + "'";

                object result = m_JobSession.ExecuteScalar(queryString);

                if (result == null)
                {
                    return 0;
                }

                itemCount = Convert.ToInt32(result);
                return itemCount;
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
        public List<DataItem_GeneralInformation> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM GeneralInformation ORDER BY ItemName ASC";

                DataTable table = new DataTable();
                List<DataItem_GeneralInformation> tableData = new List<DataItem_GeneralInformation>();

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
        public DataItem_GeneralInformation SelectInformationOrNull(GeneralInformationItemName itemName)
        {
            try
            {
                DataItem_GeneralInformation generalInfo = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM GeneralInformation Where ItemName = '{0}'", itemName.ToString()));

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        generalInfo = new DataItem_GeneralInformation();

                        GeneralInformationItemName name = GeneralInformationItemName.LinkDataVersion;
                        if (Enum.TryParse<GeneralInformationItemName>(dr["ItemName"].ToString(), out name) == true)
                        {
                            generalInfo.ItemName = name;
                        }
                        generalInfo.ItemValue = dr["ItemValue"].ToString();

                        break;
                    }
                }

                return generalInfo;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        private List<DataItem_GeneralInformation> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_GeneralInformation> tableData = new List<DataItem_GeneralInformation>();

                foreach (DataRow dr in table.Rows)
                {
                    GeneralInformationItemName itemName = GeneralInformationItemName.LinkDataVersion;
                    if (Enum.TryParse<GeneralInformationItemName>(dr["ItemName"].ToString(), out itemName) == true)
                    {
                    }
                    string itemValue = dr["ItemValue"].ToString();

                    DataItem_GeneralInformation data = new DataItem_GeneralInformation(itemName, itemValue);

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
        public int Insert(GeneralInformationItemName itemName, string itemValue)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO GeneralInformation ");
                queryString.Append("(ItemName, ItemValue) VALUES (");
                queryString.AppendFormat("'{0}', ", itemName.ToString());
                queryString.AppendFormat("'{0}') ", itemValue);

                return m_JobSession.ExecuteNonQuery(queryString.ToString());
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int Insert(DataItem_GeneralInformation item)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO GeneralInformation ");
                queryString.Append("(ItemName, ItemValue) VALUES (");
                queryString.AppendFormat("'{0}', ", item.ItemName.ToString());
                queryString.AppendFormat("'{0}') ", item.ItemValue);

                return m_JobSession.ExecuteNonQuery(queryString.ToString());
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int Insert(List<DataItem_GeneralInformation> itemList)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO GeneralInformation ");

                queryString.Append("(ItemName, ItemValue) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].ItemName.ToString());
                    queryString.AppendFormat("'{0}')", itemList[index].ItemValue);

                    if (index < itemList.Count - 1)
                    {
                        queryString.Append(", ");
                    }
                }

                return m_JobSession.ExecuteNonQuery(queryString.ToString());
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        #endregion

        #region Methods - Update Query
        public int Update(DataItem_GeneralInformation item)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE GeneralInformation SET ");

                queryString.AppendFormat("ItemValue = '{0}' ", item.ItemValue);
                queryString.AppendFormat("WHERE ItemName = '{0}'", Convert.ToInt32(item.ItemName));

                return m_JobSession.ExecuteNonQuery(queryString.ToString());
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        #endregion

        #region Methods - Delete Query
        public int Delete(GeneralInformationItemName itemName)
        {
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM GeneralInformation ");
                queryString.AppendFormat("WHERE ItemName = '{0}'", itemName.ToString());

                return m_JobSession.ExecuteNonQuery(queryString.ToString());
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        #endregion
    }
}
