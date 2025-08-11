using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_ComponentLife
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryComponentLifeEvent(object sender, object eventData);

        public event QueryComponentLifeEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_ComponentLife(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM ComponentLife";

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
        public int SelectCount(string componentName, ComponentLocation location)
        {
            try
            {
                int count = 0;
                string queryString = "SELECT COUNT(*) FROM ComponentLife WHERE ComponentName='" + componentName + "' and ComponentLocation=" + location;

                object result = m_JobSession.ExecuteScalar(queryString);

                if (result == null)
                {
                    return 0;
                }

                count = Convert.ToInt32(result);
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
        public List<DataItem_ComponentLife> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM ComponentLife";

                DataTable table = new DataTable();
                List<DataItem_ComponentLife> tableData = new List<DataItem_ComponentLife>();

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
        public List<DataItem_ComponentLife> SelectOrNull(int id)
        {
            try
            {
                //int no = 1;
                DataTable table = new DataTable();
                List<DataItem_ComponentLife> tableData = new List<DataItem_ComponentLife>();


                return tableData;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public DataItem_ComponentLife SelectComponentOrNull(string componentName, ComponentLocation location)
        {
            try
            {
                DataItem_ComponentLife componentLife = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM ComponentLife Where ComponentName = '{0}' and ComponentLocation = {1}", componentName, location));

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        componentLife = new DataItem_ComponentLife()
                        {
                            ComponentName = dr["ComponentName"].ToString(),
                            ComponentType = dr["ComponentType"].ToString(),
                            ComponentMaker = dr["ComponentMaker"].ToString(),
                            Unit = dr["Unit"].ToString(),
                            LifeTime = Convert.ToDouble(dr["LifeTime"].ToString()),
                            UsedTime = Convert.ToDouble(dr["UsedTime"].ToString()),
                            IsAlwaysUse = bool.Parse(dr["IsAlwaysUse"].ToString()),
                            ComponentLocation = (ComponentLocation)Convert.ToInt32(dr["ComponentLocation"]),
                            DataType = (LifeDataType)Convert.ToInt32(dr["LifeDataType"]),
                            UseStartTime = DateTime.Parse(dr["UseStartTime"].ToString()),
                        };
                        break;
                    }
                }

                return componentLife;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        private List<DataItem_ComponentLife> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_ComponentLife> tableData = new List<DataItem_ComponentLife>();

                foreach (DataRow dr in table.Rows)
                {
                    DataItem_ComponentLife data = new DataItem_ComponentLife()
                    {
                        ComponentName = dr["ComponentName"].ToString(),
                        ComponentType = dr["ComponentType"].ToString(),
                        ComponentMaker = dr["ComponentMaker"].ToString(),
                        Unit = dr["Unit"].ToString(),
                        LifeTime = Convert.ToDouble(dr["LifeTime"].ToString()),
                        UsedTime = Convert.ToDouble(dr["UsedTime"].ToString()),
                        IsAlwaysUse = bool.Parse(dr["IsAlwaysUse"].ToString()),
                        ComponentLocation = (ComponentLocation)Convert.ToInt32(dr["ComponentLocation"]),
                        DataType = (LifeDataType)Convert.ToInt32(dr["LifeDataType"]),
                        UseStartTime = DateTime.Parse(dr["UseStartTime"].ToString()),
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

        public double GetUsedTimeByName(string componentName, ComponentLocation location)
        {
            try
            {
                string queryString = $"SELECT * FROM ComponentLife where ComponentName = '{componentName}' and ComponentLocation=" + Convert.ToInt32(location);

                DataTable table = new DataTable();
                int returnValue = m_JobSession.ExecuteReader(queryString, ref table);
                if (returnValue > 0)
                {
                    return Convert.ToDouble(table.Rows[0]["UsedTime"].ToString());
                }

                return 0;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionHappened?.Invoke(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return 0;
            }
        }
        #endregion

        #region Methods - Insert Query
        public int Insert(string componentName, string componentType, string componentMaker, double lifeTime, double usedTime, bool isAlwaysUse, DateTime useStartTime, ComponentLocation componentLocation, LifeDataType dataType, string unit)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ComponentLife ");
                queryString.Append("(ComponentName, ComponentType, ComponentMaker, LifeTime, UsedTime, IsAlwaysUse, UseStartTime, ComponentLocation,LifeDataType,Unit) VALUES (");
                queryString.AppendFormat("'{0}', ", componentName);
                queryString.AppendFormat("'{0}', ", componentType);
                queryString.AppendFormat("'{0}', ", componentMaker);
                queryString.AppendFormat("{0}, ", lifeTime);
                queryString.AppendFormat("{0}, ", usedTime);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(isAlwaysUse));
                queryString.AppendFormat("'{0}',", useStartTime.ToString());
                queryString.AppendFormat("{0}, ", Convert.ToInt32(componentLocation));
                queryString.AppendFormat("{0}, ", Convert.ToInt32(dataType));
                queryString.AppendFormat("'{0}') ", unit);
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
        public int Insert(DataItem_ComponentLife item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ComponentLife ");
                queryString.Append("(ComponentName, ComponentType, ComponentMaker, LifeTime, UsedTime, IsAlwaysUse, UseStartTime, ComponentLocation,LifeDataType,Unit) VALUES (");
                queryString.AppendFormat("'{0}', ", item.ComponentName);
                queryString.AppendFormat("'{0}', ", item.ComponentType);
                queryString.AppendFormat("'{0}', ", item.ComponentMaker);
                queryString.AppendFormat("{0}, ", item.LifeTime);
                queryString.AppendFormat("{0}, ", item.UsedTime);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.IsAlwaysUse));
                queryString.AppendFormat("'{0}',", item.UseStartTime.ToString());
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.ComponentLocation));
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.DataType));
                queryString.AppendFormat("'{0}') ", item.Unit);
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
        public int Insert(List<DataItem_ComponentLife> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO ComponentLife ");

                queryString.Append("(ComponentName, ComponentType, ComponentMaker, LifeTime, UsedTime, IsAlwaysUse, UseStartTime, ComponentLocation,LifeDataType,Unit) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].ComponentName);
                    queryString.AppendFormat("'{0}', ", itemList[index].ComponentType);
                    queryString.AppendFormat("'{0}', ", itemList[index].ComponentMaker);
                    queryString.AppendFormat("{0}, ", itemList[index].LifeTime);
                    queryString.AppendFormat("{0}, ", itemList[index].UsedTime);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].IsAlwaysUse));
                    queryString.AppendFormat("'{0}',", itemList[index].UseStartTime.ToString());
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].ComponentLocation));
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].DataType));
                    queryString.AppendFormat("'{0}') ", itemList[index].Unit);

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
        public int Update(DataItem_ComponentLife item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE ComponentLife SET ");

                queryString.AppendFormat("ComponentType = '{0}' ,", item.ComponentType);
                queryString.AppendFormat("ComponentMaker = '{0}' ,", item.ComponentMaker);
                queryString.AppendFormat("LifeTime = {0:F1} ,", item.LifeTime);
                queryString.AppendFormat("UsedTime = {0:F1} ,", item.UsedTime);
                queryString.AppendFormat("IsAlwaysUse = {0} ", Convert.ToInt32(item.IsAlwaysUse));
                queryString.AppendFormat("UseStartTime = '{0}' ,", item.UseStartTime.ToString());
                queryString.AppendFormat("ComponentLocation = {0}, ", Convert.ToInt32(item.ComponentLocation));
                queryString.AppendFormat("LifeDataType = {0}, ", Convert.ToInt32(item.DataType));
                queryString.AppendFormat("Unit = '{0}' ", item.Unit);
                queryString.AppendFormat("WHERE ComponentName = '{0}'", item.ComponentName);
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

        public int UpdateUsedTime(double usedTime, string componentName)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder($"UPDATE ComponentLife SET UsedTime = {usedTime:F1} where ComponentName = '{componentName}'");
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
        public int Delete(string componentName)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM ComponentLife ");
                queryString.AppendFormat("WHERE ComponentName = '{0}'", componentName);
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
