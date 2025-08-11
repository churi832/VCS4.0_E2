using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.DBProvider;
using Sineva.VHL.Library.Common;
using System.Reflection;

namespace Sineva.VHL.Data.DbAdapter
{
    public class Query_Port
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryPortEvent(object sender, object eventData);
        public event QueryPortEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_Port(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion


        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM Port";

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString);

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
                string queryString = "SELECT MAX(PortID) FROM Port";

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString);

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
        public int SelectCount(int portID)
        {
            try
            {
                int portCount = 0;
                string queryString = "SELECT COUNT(*) FROM Port WHERE PortID =" + portID + "";

                object result = JobSession.GetInstanceOrNull().ExecuteScalar(queryString);

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
        public List<DataItem_Port> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM Port ORDER BY PortID ASC";

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString, ref table);

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
        public List<DataItem_Port> SelectListOrNullByPortType(int portType)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * "));
                queryString.AppendFormat("FROM Port ");
                queryString.AppendFormat("WHERE PortType = {0} ", portType);
                queryString.AppendFormat("ORDER BY PortID ASC");

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_Port> SelectListOrNullByPortID(int portId)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * "));
                queryString.AppendFormat("FROM Port ");
                queryString.AppendFormat("WHERE PortID like '%{0}%' ", portId);
                queryString.AppendFormat("ORDER BY PortID ASC");

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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

        public List<DataItem_Port> SelectListOrNullByNodeID(int nodeId)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * "));
                queryString.AppendFormat("FROM Port ");
                queryString.AppendFormat("WHERE NodeID like '%{0}%' ", nodeId);
                queryString.AppendFormat("ORDER BY NodeID ASC");

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_Port> SelectListOrNullByLinkID(int linkId)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * "));
                queryString.AppendFormat("FROM Port ");
                queryString.AppendFormat("WHERE LinkID like '%{0}%' ", linkId);
                queryString.AppendFormat("ORDER BY LinkID ASC");

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        public DataItem_Port SelectSingleOrNull(int linkId)
        {
            try
            {
                DataItem_Port tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Port WHERE LinkID = {0}", linkId));

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int _PortID = Convert.ToInt32(dr["PortID"]);
                        int _LinkID = Convert.ToInt32(dr["LinkID"]);
                        int _NodeID = Convert.ToInt32(dr["NodeID"]);
                        bool _PIOUsed = Convert.ToBoolean(dr["PIOUsed"]);
                        bool _Prohibition = Convert.ToBoolean(dr["PortProhibition"]);
                        bool _OffsetUsed = Convert.ToBoolean(dr["OffsetUsed"]);
                        int _PortType = Convert.ToInt32(dr["PortType"]);
                        double _RotatePosition = Convert.ToDouble(dr["RotatePosition"]);
                        double _SlidePosition = Convert.ToDouble(dr["SlidePosition"]);
                        double _HoistPosition = Convert.ToDouble(dr["HoistPosition"]);
                        double _BeforeHoistPosition = Convert.ToDouble(dr["BeforeHoistPosition"]);
                        int _PIOID = Convert.ToInt32(dr["PIOID"]);
                        int _PIOCH = Convert.ToInt32(dr["PIOCH"]);
                        int _PIOCS = Convert.ToInt32(dr["PIOCS"]);
                        //2023.2.21 L/UL 구분.. by HS
                        double _UnloadRotatePosition = Convert.ToDouble(dr["UnloadRotatePosition"]);
                        double _UnloadSlidePosition = Convert.ToDouble(dr["UnloadSlidePosition"]);
                        double _UnloadHoistPosition = Convert.ToDouble(dr["UnloadHoistPosition"]);
                        double _BeforeUnloadHoistPosition = Convert.ToDouble(dr["BeforeUnloadHoistPosition"]);

                        double _BarcodeLeft = Convert.ToDouble(dr["BarcodeLeft"]);
                        double _BarcodeRight = Convert.ToDouble(dr["BarcodeRight"]);
                        bool _PBSUsed = Convert.ToBoolean(dr["PBSUsed"]);
                        int _PBSSelectNo = Convert.ToInt32(dr["PBSSelectNo"]);
                        int _ProfileExistPosition = Convert.ToInt32(dr["ProfileExistPosition"]);

                        tableData = new DataItem_Port()
                        {
                            PortID = _PortID,
                            LinkID = _LinkID,
                            NodeID = _NodeID,
                            PIOUsed = _PIOUsed,
                            PortProhibition = _Prohibition,
                            OffsetUsed = _OffsetUsed,
                            PortType = (PortType)_PortType,
                            RotatePosition = _RotatePosition,
                            SlidePosition = _SlidePosition,
                            HoistPosition = _HoistPosition,
                            BeforeHoistPosition = _BeforeHoistPosition,
                            PIOID = _PIOID,
                            PIOCH = _PIOCH,
                            PIOCS = _PIOCS,

                            //2023.2.21 L/UL 구분.. by HS
                            UnloadRotatePosition = _UnloadRotatePosition,
                            UnloadSlidePosition = _UnloadSlidePosition,
                            UnloadHoistPosition = _UnloadHoistPosition,
                            BeforeUnloadHoistPosition = _BeforeUnloadHoistPosition,

                            BarcodeLeft = _BarcodeLeft,
                            BarcodeRight = _BarcodeRight,
                            PBSUsed = _PBSUsed,
                            PBSSelectNo = _PBSSelectNo,
                            ProfileExistPosition = (enProfileExistPosition)_ProfileExistPosition,

                            DriveLeftOffset = 0.0f,
                            DriveRightOffset = 0.0f,
                            HoistOffset = 0.0f,
                            SlideOffset = 0.0f, 
                            RotateOffset = 0.0f,
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

        public DataItem_Port SelectSingleOrNullByPortId(int portId)
        {
            try
            {
                DataItem_Port tableData = null;
                DataTable dataTable = new DataTable();

                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Port WHERE PortID = {0}", portId));

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref dataTable);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        int _PortID = Convert.ToInt32(dr["PortID"]);
                        int _LinkID = Convert.ToInt32(dr["LinkID"]);
                        int _NodeID = Convert.ToInt32(dr["NodeID"]);
                        bool _PIOUsed = Convert.ToBoolean(dr["PIOUsed"]);
                        bool _Prohibition = Convert.ToBoolean(dr["PortProhibition"]);
                        bool _OffsetUsed = Convert.ToBoolean(dr["OffsetUsed"]);
                        int _PortType = Convert.ToInt32(dr["PortType"]);
                        double _RotatePosition = Convert.ToDouble(dr["RotatePosition"]);
                        double _SlidePosition = Convert.ToDouble(dr["SlidePosition"]);
                        double _HoistPosition = Convert.ToDouble(dr["HoistPosition"]);
                        double _BeforeHoistPosition = Convert.ToDouble(dr["BeforeHoistPosition"]);
                        int _PIOID = Convert.ToInt32(dr["PIOID"]);
                        int _PIOCH = Convert.ToInt32(dr["PIOCH"]);
                        int _PIOCS = Convert.ToInt32(dr["PIOCS"]);
                        //2023.2.21 L/UL 구분.. by HS
                        double _UnloadRotatePosition = Convert.ToDouble(dr["UnloadRotatePosition"]);
                        double _UnloadSlidePosition = Convert.ToDouble(dr["UnloadSlidePosition"]);
                        double _UnloadHoistPosition = Convert.ToDouble(dr["UnloadHoistPosition"]);
                        double _BeforeUnloadHoistPosition = Convert.ToDouble(dr["BeforeUnloadHoistPosition"]);

                        double _BarcodeLeft = Convert.ToDouble(dr["BarcodeLeft"]);
                        double _BarcodeRight = Convert.ToDouble(dr["BarcodeRight"]);
                        bool _PBSUsed = Convert.ToBoolean(dr["PBSUsed"]);
                        int _PBSSelectNo = Convert.ToInt32(dr["PBSSelectNo"]);
                        int _ProfileExistPosition = Convert.ToInt32(dr["ProfileExistPosition"]);

                        tableData = new DataItem_Port()
                        {
                            PortID = _PortID,
                            LinkID = _LinkID,
                            NodeID = _NodeID,
                            PIOUsed = _PIOUsed,
                            PortProhibition = _Prohibition,
                            OffsetUsed = _OffsetUsed,
                            PortType = (PortType)_PortType,
                            RotatePosition = _RotatePosition,
                            SlidePosition = _SlidePosition,
                            HoistPosition = _HoistPosition,
                            BeforeHoistPosition = _BeforeHoistPosition,
                            PIOID = _PIOID,
                            PIOCH = _PIOCH,
                            PIOCS = _PIOCS,

                            //2023.2.21 L/UL 구분.. by HS
                            UnloadRotatePosition = _UnloadRotatePosition,
                            UnloadSlidePosition = _UnloadSlidePosition,
                            UnloadHoistPosition = _UnloadHoistPosition,
                            BeforeUnloadHoistPosition = _BeforeUnloadHoistPosition,

                            BarcodeLeft = _BarcodeLeft,
                            BarcodeRight = _BarcodeRight,
                            PBSUsed = _PBSUsed,
                            PBSSelectNo = _PBSSelectNo,
                            ProfileExistPosition = (enProfileExistPosition)_ProfileExistPosition,

                            DriveLeftOffset = 0.0f,
                            DriveRightOffset = 0.0f,
                            HoistOffset = 0.0f,
                            SlideOffset = 0.0f,
                            RotateOffset = 0.0f,
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
        public List<DataItem_Port> SelectListOrNull(PortType type)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Port Where PortType = {0} ORDER BY PortID ASC", Convert.ToInt32(type)));

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_Port> SelectListOrNull(bool useFlag)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Port Where UseFlag = '{0}' ORDER BY PortID ASC", useFlag.ToString()));

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        public List<DataItem_Port> SelectListOrNull(int portID)
        {
            try
            {
                StringBuilder queryString = new StringBuilder(string.Format("SELECT * FROM Port Where PortID = {0} ORDER BY PortID ASC", portID));

                DataTable table = new DataTable();
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                int returnValue = JobSession.GetInstanceOrNull().ExecuteReader(queryString.ToString(), ref table);

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
        private List<DataItem_Port> GetListFromTableOrNull(DataTable table)
        {
            try
            {
                List<DataItem_Port> tableData = new List<DataItem_Port>();

                foreach (DataRow dr in table.Rows)
                {
                    int _PortID = Convert.ToInt32(dr["PortID"]);
                    int _LinkID = Convert.ToInt32(dr["LinkID"]);
                    int _NodeID = Convert.ToInt32(dr["NodeID"]);
                    bool _PIOUsed = Convert.ToBoolean(dr["PIOUsed"]);
                    bool _Prohibition = Convert.ToBoolean(dr["PortProhibition"]);
                    bool _OffsetUsed = Convert.ToBoolean(dr["OffsetUsed"]);
                    int _PortType = Convert.ToInt32(dr["PortType"]);
                    double _RotatePosition = Convert.ToDouble(dr["RotatePosition"]);
                    double _SlidePosition = Convert.ToDouble(dr["SlidePosition"]);
                    double _HoistPosition = Convert.ToDouble(dr["HoistPosition"]);
                    double _BeforeHoistPosition = Convert.ToDouble(dr["BeforeHoistPosition"]);
                    int _PIOID = Convert.ToInt32(dr["PIOID"]);
                    int _PIOCH = Convert.ToInt32(dr["PIOCH"]);
                    int _PIOCS = Convert.ToInt32(dr["PIOCS"]);

                    //2023.2.21 L/UL 구분.. by HS
                    double _UnloadRotatePosition = Convert.ToDouble(dr["UnloadRotatePosition"]);
                    double _UnloadSlidePosition = Convert.ToDouble(dr["UnloadSlidePosition"]);
                    double _UnloadHoistPosition = Convert.ToDouble(dr["UnloadHoistPosition"]);
                    double _BeforeUnloadHoistPosition = Convert.ToDouble(dr["BeforeUnloadHoistPosition"]);

                    double _BarcodeLeft = Convert.ToDouble(dr["BarcodeLeft"]);
                    double _BarcodeRight = Convert.ToDouble(dr["BarcodeRight"]);
                    bool _PBSUsed = Convert.ToBoolean(dr["PBSUsed"]);
                    int _PBSSelectNo = Convert.ToInt32(dr["PBSSelectNo"]);
                    int _ProfileExistPosition = Convert.ToInt32(dr["ProfileExistPosition"]);

                    DataItem_Port data = new DataItem_Port()
                    {
                        PortID = _PortID,
                        LinkID = _LinkID,
                        NodeID = _NodeID,
                        PIOUsed = _PIOUsed,
                        PortProhibition = _Prohibition,
                        OffsetUsed = _OffsetUsed,
                        PortType = (PortType)_PortType,
                        RotatePosition = _RotatePosition,
                        SlidePosition = _SlidePosition,
                        HoistPosition = _HoistPosition,
                        BeforeHoistPosition = _BeforeHoistPosition,
                        PIOID = _PIOID,
                        PIOCH = _PIOCH,
                        PIOCS = _PIOCS,
                        //2023.2.21 L/UL 구분.. by HS
                        UnloadRotatePosition = _UnloadRotatePosition,
                        UnloadSlidePosition = _UnloadSlidePosition,
                        UnloadHoistPosition = _UnloadHoistPosition,
                        BeforeUnloadHoistPosition = _BeforeUnloadHoistPosition,

                        BarcodeLeft = _BarcodeLeft,
                        BarcodeRight = _BarcodeRight,
                        PBSUsed = _PBSUsed,
                        PBSSelectNo = _PBSSelectNo,
                        ProfileExistPosition = (enProfileExistPosition)_ProfileExistPosition,

                        DriveLeftOffset = 0.0f,
                        DriveRightOffset = 0.0f,
                        HoistOffset = 0.0f,
                        SlideOffset = 0.0f,
                        RotateOffset = 0.0f,
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
        public int Insert(DataItem_Port item)
        {
            int rv = -1;
            try
            {
                int result = -1;

                StringBuilder queryString = new StringBuilder("INSERT INTO Port ");
                queryString.Append("(PortID, LinkID, NodeID, PIOUsed, PortProhibition, OffsetUsed, PortType, RotatePosition, SlidePosition, HoistPosition, BeforeHoistPosition,PIOCH, PIOID, PIOCS, UnloadRotatePosition, UnloadSlidePosition, UnloadHoistPosition, BeforeUnloadHoistPosition, BarcodeLeft, BarcodeRight, PBSSelectNo, PBSUsed, ProfileExistPosition) VALUES (");
                queryString.AppendFormat("{0}, ", item.PortID);
                queryString.AppendFormat("{0}, ", item.LinkID);
                queryString.AppendFormat("{0}, ", item.NodeID);
                queryString.AppendFormat("{0}, ", item.PIOUsed ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.PortProhibition ? 1 : 0);
                queryString.AppendFormat("{0}, ", item.OffsetUsed ? 1 : 0);
                queryString.AppendFormat("{0}, ", Convert.ToInt32(item.PortType));
                queryString.AppendFormat("{0:F1}, ", item.RotatePosition);
                queryString.AppendFormat("{0:F1}, ", item.SlidePosition);
                queryString.AppendFormat("{0:F1}, ", item.HoistPosition);
                queryString.AppendFormat("{0:F1}, ", item.BeforeHoistPosition);

                queryString.AppendFormat("{0}, ", item.PIOCH);
                queryString.AppendFormat("{0}, ", item.PIOID);
                queryString.AppendFormat("{0}, ", item.PIOCS);
                //2023.2.21 L/UL 구분.. by HS
                queryString.AppendFormat("{0:F1}, ", item.UnloadRotatePosition);
                queryString.AppendFormat("{0:F1}, ", item.UnloadSlidePosition);
                queryString.AppendFormat("{0:F1}, ", item.UnloadHoistPosition);
                queryString.AppendFormat("{0:F1}, ", item.BeforeUnloadHoistPosition);

                queryString.AppendFormat("{0:F1}, ", item.BarcodeLeft);
                queryString.AppendFormat("{0:F1}, ", item.BarcodeRight);
                queryString.AppendFormat("{0}, ", item.PBSSelectNo);
                queryString.AppendFormat("{0}, ", item.PBSUsed ? 1 : 0);
                queryString.AppendFormat("{0}) ", Convert.ToInt32(item.ProfileExistPosition));

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
        public int Insert(List<DataItem_Port> itemList)
        {
            int rv = -1;
            try
            {
                if (itemList.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder("INSERT INTO Port ");
                    queryString.Append("(PortID, LinkID, NodeID, PIOUsed, PortProhibition, OffsetUsed, PortType, RotatePosition, SlidePosition, HoistPosition, BeforeHoistPosition, PIOCH, PIOID, PIOCS, UnloadRotatePosition, UnloadSlidePosition, UnloadHoistPosition, BeforeUnloadHoistPosition, BarcodeLeft, BarcodeRight, PBSSelectNo, PBSUsed, ProfileExistPosition) VALUES ");

                    for (int index = 0; index < itemList.Count; index++)
                    {
                        queryString.Append("(");
                        queryString.AppendFormat("{0}, ", itemList[index].PortID);
                        queryString.AppendFormat("{0}, ", itemList[index].LinkID);
                        queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOUsed ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].PortProhibition ? 1 : 0);
                        queryString.AppendFormat("{0}, ", itemList[index].OffsetUsed ? 1 : 0);
                        queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].PortType));
                        queryString.AppendFormat("{0:F1}, ", itemList[index].RotatePosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].SlidePosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].HoistPosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].BeforeHoistPosition);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOCH);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOID);
                        queryString.AppendFormat("{0}, ", itemList[index].PIOCS);
                        //2023.2.21 L/UL 구분.. by HS
                        queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadRotatePosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadSlidePosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadHoistPosition);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].BeforeUnloadHoistPosition);

                        queryString.AppendFormat("{0:F1}, ", itemList[index].BarcodeLeft);
                        queryString.AppendFormat("{0:F1}, ", itemList[index].BarcodeRight);
                        queryString.AppendFormat("{0}, ", itemList[index].PBSSelectNo);
                        queryString.AppendFormat("{0}, ", itemList[index].PBSUsed ? 1 : 0);
                        queryString.AppendFormat("{0}) ", Convert.ToInt32(itemList[index].ProfileExistPosition));

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
        private string InsertQuerySql(List<DataItem_Port> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder("INSERT INTO Port ");
                //2023.2.21 L/UL 구분.. by HS
                queryString.Append("(PortID, LinkID, NodeID, PIOUsed, PortProhibition, OffsetUsed, PortType, RotatePosition, SlidePosition, HoistPosition, BeforeHoistPosition, PIOCH, PIOID, PIOCS, UnloadRotatePosition, UnloadSlidePosition, UnloadHoistPosition, BeforeUnloadHoistPosition, BarcodeLeft, BarcodeRight, PBSSelectNo, PBSUsed, ProfileExistPosition) VALUES ");
                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("{0}, ", itemList[index].PortID);
                    queryString.AppendFormat("{0}, ", itemList[index].LinkID);
                    queryString.AppendFormat("{0}, ", itemList[index].NodeID);
                    queryString.AppendFormat("{0}, ", itemList[index].PIOUsed ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].PortProhibition ? 1 : 0);
                    queryString.AppendFormat("{0}, ", itemList[index].OffsetUsed ? 1 : 0);
                    queryString.AppendFormat("{0}, ", Convert.ToInt32(itemList[index].PortType));
                    queryString.AppendFormat("{0:F1}, ", itemList[index].RotatePosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].SlidePosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].HoistPosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].BeforeHoistPosition);
                    //2023.2.21 L/UL 구분.. by HS
                    queryString.AppendFormat("{0}, ", itemList[index].PIOCH);
                    queryString.AppendFormat("{0}, ", itemList[index].PIOID);
                    queryString.AppendFormat("{0}, ", itemList[index].PIOCS);

                    queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadRotatePosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadSlidePosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].UnloadHoistPosition);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].BeforeUnloadHoistPosition);

                    queryString.AppendFormat("{0:F1}, ", itemList[index].BarcodeLeft);
                    queryString.AppendFormat("{0:F1}, ", itemList[index].BarcodeRight);
                    queryString.AppendFormat("{0}, ", itemList[index].PBSSelectNo);
                    queryString.AppendFormat("{0}, ", itemList[index].PBSUsed ? 1 : 0);
                    queryString.AppendFormat("{0}) ", Convert.ToInt32(itemList[index].ProfileExistPosition));

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
        public int Update(DataItem_Port item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE Port SET ");
                queryString.AppendFormat("LinkID = {0}, ", item.LinkID);
                queryString.AppendFormat("NodeID = {0}, ", item.NodeID);
                queryString.AppendFormat("PIOUsed = {0}, ", item.PIOUsed ? 1 : 0);
                queryString.AppendFormat("PortProhibition = {0}, ", item.PortProhibition ? 1 : 0);
                queryString.AppendFormat("OffsetUsed = {0}, ", item.OffsetUsed ? 1 : 0);
                queryString.AppendFormat("PortType = {0}, ", Convert.ToInt32(item.PortType));
                queryString.AppendFormat("RotatePosition = {0:F1}, ", item.RotatePosition);
                queryString.AppendFormat("SlidePosition = {0:F1}, ", item.SlidePosition);
                queryString.AppendFormat("HoistPosition = {0:F1}, ", item.HoistPosition);
                queryString.AppendFormat("BeforeHoistPosition = {0:F1}, ", item.BeforeHoistPosition);
                queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                queryString.AppendFormat("PIOCS = {0}, ", item.PIOCS);
                //2023.2.21 L/UL 구분.. by HS
                queryString.AppendFormat("UnloadRotatePosition = {0:F1}, ", item.UnloadRotatePosition);
                queryString.AppendFormat("UnloadSlidePosition = {0:F1}, ", item.UnloadSlidePosition);
                queryString.AppendFormat("UnloadHoistPosition = {0:F1}, ", item.UnloadHoistPosition);
                queryString.AppendFormat("BeforeUnloadHoistPosition = {0:F1}, ", item.BeforeUnloadHoistPosition);

                queryString.AppendFormat("BarcodeLeft = {0:F1}, ", item.BarcodeLeft);
                queryString.AppendFormat("BarcodeRight = {0:F1}, ", item.BarcodeRight);
                queryString.AppendFormat("PBSSelectNo = {0}, ", item.PBSSelectNo);
                queryString.AppendFormat("PBSUsed = {0}, ", item.PBSUsed ? 1 : 0);
                queryString.AppendFormat("ProfileExistPosition = {0} ", Convert.ToInt32(item.ProfileExistPosition));

                queryString.AppendFormat("WHERE PortID = {0}", item.PortID);

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
        public int Update(List<DataItem_Port> items)
        {
            int rv = -1;
            try
            {
                if (items.Count > 0)
                {
                    StringBuilder queryString = new StringBuilder();
                    foreach (DataItem_Port item in items)
                    {
                        queryString.AppendFormat("UPDATE Port SET ");

                        queryString.AppendFormat("LinkID = {0}, ", item.LinkID);
                        queryString.AppendFormat("NodeID = {0}, ", item.NodeID);
                        queryString.AppendFormat("PIOUsed = {0}, ", item.PIOUsed ? 1 : 0);
                        queryString.AppendFormat("PortProhibition = {0}, ", item.PortProhibition ? 1 : 0);
                        queryString.AppendFormat("OffsetUsed = {0}, ", item.OffsetUsed ? 1 : 0);
                        queryString.AppendFormat("PortType = {0}, ", Convert.ToInt32(item.PortType));
                        queryString.AppendFormat("RotatePosition = {0:F1},", item.RotatePosition);
                        queryString.AppendFormat("SlidePosition = {0:F1}, ", item.SlidePosition);
                        queryString.AppendFormat("HoistPosition = {0:F1}, ", item.HoistPosition);
                        queryString.AppendFormat("BeforeHoistPosition = {0:F1}, ", item.BeforeHoistPosition);
                        queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                        queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                        queryString.AppendFormat("PIOCS = {0}, ", item.PIOCS);
                        //2023.2.21 L/UL 구분.. by HS
                        queryString.AppendFormat("UnloadRotatePosition = {0:F1}, ", item.UnloadRotatePosition);
                        queryString.AppendFormat("UnloadSlidePosition = {0:F1}, ", item.UnloadSlidePosition);
                        queryString.AppendFormat("UnloadHoistPosition = {0:F1}, ", item.UnloadHoistPosition);
                        queryString.AppendFormat("BeforeUnloadHoistPosition = {0:F1}, ", item.BeforeUnloadHoistPosition);

                        queryString.AppendFormat("BarcodeLeft = {0:F1}, ", item.BarcodeLeft);
                        queryString.AppendFormat("BarcodeRight = {0:F1}, ", item.BarcodeRight);
                        queryString.AppendFormat("PBSSelectNo = {0}, ", item.PBSSelectNo);
                        queryString.AppendFormat("PBSUsed = {0}, ", item.PBSUsed ? 1 : 0);
                        queryString.AppendFormat("ProfileExistPosition = {0} ", Convert.ToInt32(item.ProfileExistPosition));

                        queryString.AppendFormat("WHERE PortID = {0}", item.PortID);
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
        private string UpdateQuerySql(List<DataItem_Port> itemList)
        {
            try
            {
                if (itemList.Count == 0) return string.Empty;
                StringBuilder queryString = new StringBuilder();
                for (int i = 0; i < itemList.Count; i++)
                {
                    DataItem_Port item = itemList[i];
                    queryString.AppendFormat("UPDATE Port SET ");

                    queryString.AppendFormat("LinkID = {0}, ", item.LinkID);
                    queryString.AppendFormat("NodeID = {0}, ", item.NodeID);
                    queryString.AppendFormat("PIOUsed = {0}, ", item.PIOUsed ? 1 : 0);
                    queryString.AppendFormat("OffsetUsed = {0}, ", item.OffsetUsed ? 1 : 0);
                    queryString.AppendFormat("PortType = {0}, ", Convert.ToInt32(item.PortType));
                    queryString.AppendFormat("RotatePosition = {0:F1}, ", item.RotatePosition);
                    queryString.AppendFormat("SlidePosition = {0:F1}, ", item.SlidePosition);
                    queryString.AppendFormat("HoistPosition = {0:F1}, ", item.HoistPosition);
                    queryString.AppendFormat("BeforeHoistPosition = {0:F1}, ", item.BeforeHoistPosition);
                    queryString.AppendFormat("PIOCH = {0}, ", item.PIOCH);
                    queryString.AppendFormat("PIOID = {0}, ", item.PIOID);
                    queryString.AppendFormat("PIOCS = {0}, ", item.PIOCS);
                    //2023.2.21 L/UL 구분.. by HS
                    queryString.AppendFormat("UnloadRotatePosition = {0:F1}, ", item.UnloadRotatePosition);
                    queryString.AppendFormat("UnloadSlidePosition = {0:F1}, ", item.UnloadSlidePosition);
                    queryString.AppendFormat("UnloadHoistPosition = {0:F1}, ", item.UnloadHoistPosition);
                    queryString.AppendFormat("BeforeUnloadHoistPosition = {0:F1}, ", item.BeforeUnloadHoistPosition);

                    queryString.AppendFormat("BarcodeLeft = {0:F1}, ", item.BarcodeLeft);
                    queryString.AppendFormat("BarcodeRight = {0:F1}, ", item.BarcodeRight);
                    queryString.AppendFormat("PBSSelectNo = {0}, ", item.PBSSelectNo);
                    queryString.AppendFormat("PBSUsed = {0}, ", item.PBSUsed ? 1 : 0);
                    queryString.AppendFormat("ProfileExistPosition = {0} ", Convert.ToInt32(item.ProfileExistPosition));

                    queryString.AppendFormat("WHERE PortID = {0}" + Environment.NewLine, item.PortID);
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
        public int UpdateInsert(List<DataItem_Port> updateItemList, List<DataItem_Port> insertItemList)
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Port ");
                queryString.AppendFormat("WHERE PortID = {0}", id);

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
                    StringBuilder queryString = new StringBuilder("DELETE FROM Port where PortID = ");
                    queryString.Append(string.Join(" OR PortID = ", ids));
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
                StringBuilder queryString = new StringBuilder("DELETE FROM Port");
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
