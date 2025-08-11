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
    public class Query_Link
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryLinkEvent(object sender, object eventData);

        public event QueryLinkEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_Link(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM Link";

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
                string queryString = "SELECT MAX(LinkID) FROM Link";

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
        public int SelectCount(int linkID)
        {
            try
            {
                int linkCount = 0;
                string queryString = "SELECT COUNT(*) FROM Link WHERE LinkID =" + linkID + "";

                object result = m_JobSession.ExecuteScalar(queryString);

                linkCount = Convert.ToInt32(result);
                return linkCount;
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
        public List<DataItem_Link> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM Link ORDER BY LinkID ASC";

                DataTable table = new DataTable();
                List<DataItem_Link> tableData = new List<DataItem_Link>();

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
        public DataItem_Link SelectSingleOrNull(int id)
        {
            try
            {
                DataItem_Link tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Link WHERE LinkID = {0}", id));

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int linkID = Convert.ToInt32(dr["LinkID"]);
                        bool useFlag = Convert.ToBoolean(dr["UseFlag"]);
                        int fromNodeID = Convert.ToInt32(dr["FromNodeID"]);
                        int toNodeID = Convert.ToInt32(dr["ToNodeID"]);
                        int linkType = Convert.ToInt32(dr["LinkType"]);
                        string bcrMatch = Convert.ToString(dr["BCRMatchType"]);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                        int steerDirection = Convert.ToInt32(dr["SteerDirection"]);
                        int steerChangeLeftBCR = Convert.ToInt32(dr["SteerChangeLeftBCR"]);
                        int steerChangeRightBCR = Convert.ToInt32(dr["SteerChangeRightBCR"]);
                        double time = Convert.ToDouble(dr["Time"]);
                        int weight = Convert.ToInt32(dr["Weight"]);

                        double distance = Convert.ToDouble(dr["Distance"]);
                        double velocity = Convert.ToDouble(dr["Velocity"]);
                        double acceleration = Convert.ToDouble(dr["Acceleration"]);
                        double deceleration = Convert.ToDouble(dr["Deceleration"]);
                        double jerk = Convert.ToDouble(dr["Jerk"]);

                        int ubs0 = Convert.ToInt32(dr["UBSLevel0"]);
                        int ubs1 = Convert.ToInt32(dr["UBSLevel1"]);
                        bool routeChange = Convert.ToBoolean(dr["RouteChangeCheck"]);
                        double steerGuideLengthFromNode = Convert.ToDouble(dr["SteerGuideLengthFromNode"]);
                        double steerGuideLengthToNode = Convert.ToDouble(dr["SteerGuideLengthToNode"]);
                        bool jcsAreaFlag = Convert.ToBoolean(dr["JCSAreaFlag"]);
                        int fromDistance = Convert.ToInt32(dr["FromExtensionDistance"]);
                        int toDistance = Convert.ToInt32(dr["ToExtensionDistance"]);

                        tableData = new DataItem_Link(linkID, useFlag, fromNodeID, toNodeID, (LinkType)linkType, bcrMatch, (enSteerDirection)steerDirection, steerChangeLeftBCR, steerChangeRightBCR, time, weight, 
                            distance, velocity, acceleration, deceleration, jerk, ubs0, ubs1, routeChange, steerGuideLengthFromNode, steerGuideLengthToNode, jcsAreaFlag, fromDistance, toDistance);

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
        public List<DataItem_Link> SelectListOrNull(LinkType type)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Link Where LinkType = {0} ORDER BY LinkID ASC", Convert.ToInt32(type)));

                DataTable table = new DataTable();
                List<DataItem_Link> tableData = new List<DataItem_Link>();

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
        public List<DataItem_Link> SelectListOrNull(bool useFlag)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Link Where UseFlag = '{0}' ORDER BY LinkID ASC", useFlag.ToString()));

                DataTable table = new DataTable();
                List<DataItem_Link> tableData = new List<DataItem_Link>();

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
        public List<DataItem_Link> SelectListOrNull(int toNodeID)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Link Where ToNodeID = {0} ORDER BY LinkID ASC", toNodeID));

                DataTable table = new DataTable();
                List<DataItem_Link> tableData = new List<DataItem_Link>();

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
        private List<DataItem_Link> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_Link> tableData = new List<DataItem_Link>();

                foreach (DataRow dr in table.Rows)
                {
                    int linkID = Convert.ToInt32(dr["LinkID"]);
                    bool useFlag = Convert.ToBoolean(dr["UseFlag"]);
                    int fromNodeID = Convert.ToInt32(dr["FromNodeID"]);
                    int toNodeID = Convert.ToInt32(dr["ToNodeID"]);
                    int linkType = Convert.ToInt32(dr["LinkType"]);
                    string bcrMatch = Convert.ToString(dr["BCRMatchType"]); // 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                    int steerDirection = Convert.ToInt32(dr["SteerDirection"]);
                    int steerChangeLeftBCR = Convert.ToInt32(dr["SteerChangeLeftBCR"]);
                    int steerChangeRightBCR = Convert.ToInt32(dr["SteerChangeRightBCR"]);
                    double time = Convert.ToDouble(dr["Time"].ToString());
                    int weight = Convert.ToInt32(dr["Weight"]);

                    double distance = Convert.ToDouble(dr["Distance"].ToString());
                    double velocity = Convert.ToDouble(dr["Velocity"]);
                    double acceleration = Convert.ToDouble(dr["Acceleration"]);
                    double deceleration = Convert.ToDouble(dr["Deceleration"]);
                    double jerk = Convert.ToDouble(dr["Jerk"]);

                    int ubs0 = Convert.ToInt32(dr["UBSLevel0"]);
                    int ubs1 = Convert.ToInt32(dr["UBSLevel1"]);

                    bool routeChange = Convert.ToBoolean(dr["RouteChangeCheck"]);
                    double steerGuideLengthFromNode = Convert.ToDouble(dr["SteerGuideLengthFromNode"]);
                    double steerGuideLengthToNode = Convert.ToDouble(dr["SteerGuideLengthToNode"]);
                    bool jcsAreaFlag = Convert.ToBoolean(dr["JCSAreaFlag"]);
                    int fromDistance = Convert.ToInt32(dr["FromExtensionDistance"]);
                    int toDistance = Convert.ToInt32(dr["ToExtensionDistance"]);

                    DataItem_Link data = new DataItem_Link(linkID, useFlag, fromNodeID, toNodeID, (LinkType)linkType, bcrMatch, (enSteerDirection)steerDirection, steerChangeLeftBCR, steerChangeRightBCR, time, weight, 
                        distance, velocity, acceleration, deceleration, jerk, ubs0, ubs1, routeChange, steerGuideLengthFromNode, steerGuideLengthToNode, jcsAreaFlag, fromDistance, toDistance);

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
        public int Insert(int id, bool useFlag, int fromNodeID, int toNodeID, LinkType type, string bcrMatch, enSteerDirection steerDirection, int steerChangeLeftBCR, int steerChangeRightBCR, double time, int weight,
            double distance, double velocity, double acceleration, double deceleration, double jerk, int ubs0 = 0, int ubs1 = 0, bool routeChangeCheck = false, double steerGuideLengthFromNode = 0, double steerGuideLengthToNode = 0, bool jcsAreaFlag = false, int fromDistance = 0, int toDistance = 0)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO Link ");
                queryString.Append("(LinkID, UseFlag, FromNodeID, ToNodeID, LinkType, BCRMatchType, SteerDirection, SteerChangeLeftBCR, SteerChangeRightBCR, Time, Weight, Distance, Velocity, Acceleration, Deceleration, Jerk," +
                    "UBSLevel0, UBSLevel1, RouteChangeCheck, SteerGuideLengthFromNode, SteerGuideLengthToNode, JCSAreaFlag, FromExtensionDistance, ToExtensionDistance) VALUES (");
                queryString.AppendFormat("{0}, ", id);
                queryString.AppendFormat("{0}, ", useFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", fromNodeID);
                queryString.AppendFormat("{0}, ", toNodeID);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(type));
                queryString.AppendFormat("{0}, ", bcrMatch);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                queryString.AppendFormat("{0}, ", Convert.ToInt32(steerDirection));
                queryString.AppendFormat("{0}, ", steerChangeLeftBCR);
                queryString.AppendFormat("{0}, ", steerChangeRightBCR);
                queryString.AppendFormat("{0}, ", time);
                queryString.AppendFormat("{0}, ", weight);
                queryString.AppendFormat("{0}, ", distance);
                queryString.AppendFormat("{0}, ", velocity);
                queryString.AppendFormat("{0}, ", acceleration);
                queryString.AppendFormat("{0}, ", deceleration);
                queryString.AppendFormat("{0}, ", jerk);
                queryString.AppendFormat("{0}, ", ubs0);
                queryString.AppendFormat("{0}, ", ubs1);
                queryString.AppendFormat("{0}, ", routeChangeCheck ? 1 : 0);
                queryString.AppendFormat("{0}, ", steerGuideLengthFromNode);
                queryString.AppendFormat("{0}, ", steerGuideLengthToNode);
                queryString.AppendFormat("{0}, ", jcsAreaFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", fromDistance);
                queryString.AppendFormat("{0}) ", toDistance);
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
        public int Insert(DataItem_Link item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO Link ");
                queryString.Append("(LinkID, UseFlag, FromNodeID, ToNodeID, LinkType, BCRMatchType, SteerDirection, SteerChangeLeftBCR, SteerChangeRightBCR, Time, Weight, Distance, Velocity, Acceleration, Deceleration, Jerk," +
                    "UBSLevel0, UBSLevel1, RouteChangeCheck, SteerGuideLengthFromNode, SteerGuideLengthToNode, JCSAreaFlag, FromExtensionDistance, ToExtensionDistance) VALUES (");
                queryString.AppendFormat("{0}, ", item.LinkID);
                queryString.AppendFormat("{0}, ", item.UseFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.FromNodeID);
                queryString.AppendFormat("{0}, ", item.ToNodeID);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("{0}, ", item.BCRMatchType);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.SteerDirectionValue));
                queryString.AppendFormat("{0}, ", item.SteerChangeLeftBCR);
                queryString.AppendFormat("{0}, ", item.SteerChangeRightBCR);
                queryString.AppendFormat("{0}, ", item.Time);
                queryString.AppendFormat("{0}, ", item.Weight);
                queryString.AppendFormat("{0}, ", item.Distance);
                queryString.AppendFormat("{0}, ", item.Velocity);
                queryString.AppendFormat("{0}, ", item.Acceleration);
                queryString.AppendFormat("{0}, ", item.Deceleration);
                queryString.AppendFormat("{0}, ", item.Jerk);
                queryString.AppendFormat("{0}, ", item.UBSLevel0);
                queryString.AppendFormat("{0}, ", item.UBSLevel1);
                queryString.AppendFormat("{0}, ", item.RouteChangeCheck ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.SteerGuideLengthFromNode);
                queryString.AppendFormat("{0}, ", item.SteerGuideLengthToNode);
                queryString.AppendFormat("{0}, ", item.JCSAreaFlag ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.FromExtensionDistance);
                queryString.AppendFormat("{0}) ", item.ToExtensionDistance);
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
        public int Insert(List<DataItem_Link> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO Link ");
                    queryString.Append("(LinkID, UseFlag, FromNodeID, ToNodeID, LinkType, BCRMatchType, SteerDirection, SteerChangeLeftBCR, SteerChangeRightBCR, Time, Weight, Distance, Velocity, Acceleration, Deceleration, Jerk," +
                                        "UBSLevel0, UBSLevel1, RouteChangeCheck, SteerGuideLengthFromNode, SteerGuideLengthToNode, JCSAreaFlag, FromExtensionDistance, ToExtensionDistance) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");
                        queryString.AppendFormat("{0}, ", itemList[index].LinkID);
                        queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].FromNodeID);
                        queryString.AppendFormat("{0}, ", itemList[index].ToNodeID);
                        queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Type));
                        queryString.AppendFormat("{0}, ", itemList[index].BCRMatchType);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                        queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].SteerDirectionValue));
                        queryString.AppendFormat("{0}, ", itemList[index].SteerChangeLeftBCR);
                        queryString.AppendFormat("{0}, ", itemList[index].SteerChangeRightBCR);
                        queryString.AppendFormat("{0}, ", itemList[index].Time);
                        queryString.AppendFormat("{0}, ", itemList[index].Weight);
                        queryString.AppendFormat("{0}, ", itemList[index].Distance);
                        queryString.AppendFormat("{0}, ", itemList[index].Velocity);
                        queryString.AppendFormat("{0}, ", itemList[index].Acceleration);
                        queryString.AppendFormat("{0}, ", itemList[index].Deceleration);
                        queryString.AppendFormat("{0}, ", itemList[index].Jerk);

                        queryString.AppendFormat("{0}, ", itemList[index].UBSLevel0);
                        queryString.AppendFormat("{0}, ", itemList[index].UBSLevel1);
                        queryString.AppendFormat("{0}, ", itemList[index].RouteChangeCheck ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].SteerGuideLengthFromNode);
                        queryString.AppendFormat("{0}, ", itemList[index].SteerGuideLengthToNode);
                        queryString.AppendFormat("{0}, ", itemList[index].JCSAreaFlag ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].FromExtensionDistance);
                        queryString.AppendFormat("{0}) ", itemList[index].ToExtensionDistance);
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
        private string InsertQuerySql(List<DataItem_Link> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO Link ");
                queryString.Append("(LinkID, UseFlag, FromNodeID, ToNodeID, LinkType, BCRMatchType, SteerDirection, SteerChangeLeftBCR, SteerChangeRightBCR, Time, Weight, Distance, Velocity, Acceleration, Deceleration, Jerk," +
                    "UBSLevel0, UBSLevel1, RouteChangeCheck, SteerGuideLengthFromNode, SteerGuideLengthToNode, JCSAreaFlag, FromExtensionDistance, ToExtensionDistance) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].LinkID);
                    queryString.AppendFormat("{0}, ", itemList[index].UseFlag ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].FromNodeID);
                    queryString.AppendFormat("{0}, ", itemList[index].ToNodeID);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].Type));
                    queryString.AppendFormat("{0}, ", itemList[index].BCRMatchType);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].SteerDirectionValue));
                    queryString.AppendFormat("{0}, ", itemList[index].SteerChangeLeftBCR);
                    queryString.AppendFormat("{0}, ", itemList[index].SteerChangeRightBCR);
                    queryString.AppendFormat("{0}, ", itemList[index].Time);
                    queryString.AppendFormat("{0}, ", itemList[index].Weight);

                    queryString.AppendFormat("{0}, ", itemList[index].Distance);
                    queryString.AppendFormat("{0}, ", itemList[index].Velocity);
                    queryString.AppendFormat("{0}, ", itemList[index].Acceleration);
                    queryString.AppendFormat("{0}, ", itemList[index].Deceleration);
                    queryString.AppendFormat("{0}, ", itemList[index].Jerk);

                    queryString.AppendFormat("{0}, ", itemList[index].UBSLevel0);
                    queryString.AppendFormat("{0}, ", itemList[index].UBSLevel1);
                    queryString.AppendFormat("{0}, ", itemList[index].RouteChangeCheck ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].SteerGuideLengthFromNode);
                    queryString.AppendFormat("{0}, ", itemList[index].SteerGuideLengthToNode);
                    queryString.AppendFormat("{0}, ", itemList[index].JCSAreaFlag ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].FromExtensionDistance);
                    queryString.AppendFormat("{0}) ", itemList[index].ToExtensionDistance);

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
        public int Update(DataItem_Link item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE Link SET ");

                queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                queryString.AppendFormat("FromNodeID = {0}, ", item.FromNodeID);
                queryString.AppendFormat("ToNodeID = {0}, ", item.ToNodeID);
                queryString.AppendFormat("LinkType = {0}, ", Convert.ToInt32(item.Type));
                queryString.AppendFormat("BCRMatchType = '{0}', ", item.BCRMatchType);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                queryString.AppendFormat("SteerDirection = {0}, ", Convert.ToInt32(item.SteerDirectionValue));
                queryString.AppendFormat("SteerChangeLeftBCR = {0}, ", item.SteerChangeLeftBCR);
                queryString.AppendFormat("SteerChangeRightBCR = {0}, ", item.SteerChangeRightBCR);
                queryString.AppendFormat("Time = {0}, ", item.Time);
                queryString.AppendFormat("Weight = {0}, ", item.Weight);

                queryString.AppendFormat("Distance = {0}, ", item.Distance);
                queryString.AppendFormat("Velocity = {0}, ", item.Velocity);
                queryString.AppendFormat("Acceleration = {0}, ", item.Acceleration);
                queryString.AppendFormat("Deceleration = {0}, ", item.Deceleration);
                queryString.AppendFormat("Jerk = {0}, ", item.Jerk);

                queryString.AppendFormat("UBSLevel0 = {0}, ", item.UBSLevel0);
                queryString.AppendFormat("UBSLevel1 = {0}, ", item.UBSLevel1);
                queryString.AppendFormat("RouteChangeCheck = {0}, ", item.RouteChangeCheck ? 1 : 0);
                queryString.AppendFormat("SteerGuideLengthFromNode = {0}, ", item.SteerGuideLengthFromNode);
                queryString.AppendFormat("SteerGuideLengthToNode = {0}, ", item.SteerGuideLengthToNode);
                queryString.AppendFormat("JCSAreaFlag = {0}, ", item.JCSAreaFlag ? 1 : 0);
                queryString.AppendFormat("FromExtensionDistance = {0}, ", item.FromExtensionDistance);
                queryString.AppendFormat("ToExtensionDistance = {0} ", item.ToExtensionDistance);

                queryString.AppendFormat("WHERE LinkID = {0}", item.LinkID);
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
        public int Update(List<DataItem_Link> items)
        {
            int rv = -1;
            try
            {
                if (items.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();
                    foreach (DataItem_Link item in items)
                    {
                        queryString.AppendFormat("UPDATE Link SET ");

                        queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                        queryString.AppendFormat("FromNodeID = {0}, ", item.FromNodeID);
                        queryString.AppendFormat("ToNodeID = {0}, ", item.ToNodeID);
                        queryString.AppendFormat("LinkType = {0}, ", Convert.ToInt32(item.Type));
                        queryString.AppendFormat("BCRMatchType = '{0}', ", item.BCRMatchType);// 2023/03/20 HJYOU BCRMatchType = B:Both, L:Left, R:Right
                        queryString.AppendFormat("SteerDirection = {0}, ", Convert.ToInt32(item.SteerDirectionValue));
                        queryString.AppendFormat("SteerChangeLeftBCR = {0}, ", item.SteerChangeLeftBCR);
                        queryString.AppendFormat("SteerChangeRightBCR = {0}, ", item.SteerChangeRightBCR);
                        queryString.AppendFormat("Time = {0}, ", item.Time);
                        queryString.AppendFormat("Weight = {0}, ", item.Weight);

                        queryString.AppendFormat("Distance = {0}, ", item.Distance);
                        queryString.AppendFormat("Velocity = {0}, ", item.Velocity);
                        queryString.AppendFormat("Acceleration = {0}, ", item.Acceleration);
                        queryString.AppendFormat("Deceleration = {0}, ", item.Deceleration);
                        queryString.AppendFormat("Jerk = {0}, ", item.Jerk);

                        queryString.AppendFormat("UBSLevel0 = {0}, ", item.UBSLevel0);
                        queryString.AppendFormat("UBSLevel1 = {0}, ", item.UBSLevel1);
                        queryString.AppendFormat("RouteChangeCheck = {0}, ", item.RouteChangeCheck ? 1 : 0);
                        queryString.AppendFormat("SteerGuideLengthFromNode = {0}, ", item.SteerGuideLengthFromNode);
                        queryString.AppendFormat("SteerGuideLengthToNode = {0}, ", item.SteerGuideLengthToNode);
                        queryString.AppendFormat("JCSAreaFlag = {0}, ", item.JCSAreaFlag ? 1 : 0);
                        queryString.AppendFormat("FromExtensionDistance = {0}, ", item.FromExtensionDistance);
                        queryString.AppendFormat("ToExtensionDistance = {0} ", item.ToExtensionDistance);

                        queryString.AppendFormat("WHERE LinkID = {0}" + Environment.NewLine, item.LinkID);
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
        private string UpdateQuerySql(List<DataItem_Link> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_Link item = itemList[i];
                    queryString.AppendFormat("UPDATE Link SET ");

                    queryString.AppendFormat("UseFlag = {0}, ", item.UseFlag ? 1 : 0);
                    queryString.AppendFormat("FromNodeID = {0}, ", item.FromNodeID);
                    queryString.AppendFormat("ToNodeID = {0}, ", item.ToNodeID);
                    queryString.AppendFormat("LinkType = {0}, ", Convert.ToInt32(item.Type));
                    queryString.AppendFormat("BCRMatchType = {0} ", item.BCRMatchType);
                    queryString.AppendFormat("SteerDirection = {0}, ", Convert.ToInt32(item.SteerDirectionValue));
                    queryString.AppendFormat("SteerChangeLeftBCR = {0}, ", item.SteerChangeLeftBCR);
                    queryString.AppendFormat("SteerChangeRightBCR = {0}, ", item.SteerChangeRightBCR);
                    queryString.AppendFormat("Time = {0}, ", item.Time);
                    queryString.AppendFormat("Weight = {0}, ", item.Weight);

                    queryString.AppendFormat("Distance = {0}, ", item.Distance);
                    queryString.AppendFormat("Velocity = {0}, ", item.Velocity);
                    queryString.AppendFormat("Acceleration = {0}, ", item.Acceleration);
                    queryString.AppendFormat("Deceleration = {0}, ", item.Deceleration);
                    queryString.AppendFormat("Jerk = {0}, ", item.Jerk);

                    queryString.AppendFormat("UBSLevel0 = {0}, ", item.UBSLevel0);
                    queryString.AppendFormat("UBSLevel1 = {0}, ", item.UBSLevel1);
                    queryString.AppendFormat("RouteChangeCheck = {0}, ", item.RouteChangeCheck ? 1 : 0);
                    queryString.AppendFormat("SteerGuideLengthFromNode = {0}, ", item.SteerGuideLengthFromNode);
                    queryString.AppendFormat("SteerGuideLengthToNode = {0}, ", item.SteerGuideLengthToNode);
                    queryString.AppendFormat("JCSAreaFlag = {0}, ", item.JCSAreaFlag ? 1 : 0);
                    queryString.AppendFormat("FromExtensionDistance = {0}, ", item.FromExtensionDistance);
                    queryString.AppendFormat("ToExtensionDistance = {0} ", item.ToExtensionDistance);

                    queryString.AppendFormat("WHERE LinkID = {0}" + Environment.NewLine, item.LinkID);
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
        public int UpdateInsert(List<DataItem_Link> updateItemList, List<DataItem_Link> insertItemList)
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Link ");
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
                if (ids.Count < 1) return 0;
                StringBuilder queryString = new StringBuilder("DELETE FROM Link where LinkID = ");
                queryString.Append(string.Join(" OR LinkID = ", ids));
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Link");
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
