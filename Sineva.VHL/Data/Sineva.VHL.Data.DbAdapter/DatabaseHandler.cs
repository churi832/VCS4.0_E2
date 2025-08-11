using Sineva.VHL.Library;
using Sineva.VHL.Library.DBProvider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library.PathFindAlgorithm;
using System.ComponentModel;

namespace Sineva.VHL.Data.DbAdapter
{
    public class DatabaseHandler
    {
        public readonly static DatabaseHandler Instance = new DatabaseHandler();

        #region Fields
        private bool m_Initialized = false;
        private Query_Node m_QueryNode = null;
        private Query_Link m_QueryLink = null;
        private Query_CommentTag m_QueryCommentTag = null;
        private Query_ComponentLife m_QueryComponentLife = null;
        private Query_DestinationChange m_QueryDestinationChange = null;
        private Query_ErrorHistory m_QueryErrorHistory = null;
        private Query_ErrorList m_QueryErrorList = null;
        private Query_FrontDetectFilter m_QueryFrontDetectFilter = null;
        private Query_GeneralInformation m_QueryGeneralInformation = null;
        private Query_LinkMergeArea m_QueryLinkMergeArea = null;
        private Query_PIODevice m_QueryPIODevice = null;
        private Query_Port m_QueryPort = null;
        private Query_TransferHistory m_QueryTransferHistory = null;
        private Query_UserList m_QueryUserList = null;
        private Query_EventLog m_QueryEventLog = null;
        private Query_VelocityLimit m_QueryVelocityLimit = null;
        private Query_FDC m_QueryFDC = null;
        private Query_FDCServer m_QueryFDCServer = null;

        private Dictionary<int, DataItem_Node> m_DictionaryNodeDataList = new Dictionary<int, DataItem_Node>();
        private Dictionary<int, DataItem_Link> m_DictionaryLinkDataList = new Dictionary<int, DataItem_Link>();
        private Dictionary<string, DataItem_CommentTag> m_DictionaryComment = new Dictionary<string, DataItem_CommentTag>();
        private Dictionary<ComponentLocation, List<DataItem_ComponentLife>> m_DictionaryComponentList = new Dictionary<ComponentLocation, List<DataItem_ComponentLife>>();
        private Dictionary<int, DataItem_DestinationChange> m_DictionaryDestinationChangeAreaList = new Dictionary<int, DataItem_DestinationChange>();
        private Dictionary<DateTime, DataItem_ErrorHistory> m_DictionaryErrorHistory = new Dictionary<DateTime, DataItem_ErrorHistory>();
        private Dictionary<int, DataItem_ErrorList> m_DictionaryErrorList = new Dictionary<int, DataItem_ErrorList>();
        private Dictionary<int, DataItem_FrontDetectFilter> m_DictionaryFrontDetectFilter = new Dictionary<int, DataItem_FrontDetectFilter>();
        private Dictionary<GeneralInformationItemName, DataItem_GeneralInformation> m_DictionaryGeneralInformation = new Dictionary<GeneralInformationItemName, DataItem_GeneralInformation>();
        private Dictionary<int, DataItem_LinkMergeArea> m_DictionaryLinkMergeArea = new Dictionary<int, DataItem_LinkMergeArea>();
        private Dictionary<int, DataItem_PIODevice> m_DictionaryPIODevice = new Dictionary<int, DataItem_PIODevice>();
        private Dictionary<int, DataItem_Port> m_DictionaryPortDataList = new Dictionary<int, DataItem_Port>();
        private Dictionary<string, DataItem_TransferInfo> m_DictionaryTransferHistory = new Dictionary<string, DataItem_TransferInfo>();
        private Dictionary<string, DataItem_UserInfo> m_DictionaryUserList = new Dictionary<string, DataItem_UserInfo>();
        private Dictionary<DateTime, DataItem_EventLog> m_DictionaryEventLog = new Dictionary<DateTime, DataItem_EventLog>();
        private Dictionary<int, DataItem_VelocityLimit> m_DictionaryVelocityLimit = new Dictionary<int, DataItem_VelocityLimit>();
        private Dictionary<int, DataItem_FDC> m_DictionaryFDC = new Dictionary<int, DataItem_FDC>();
        #endregion
        #region Events
        public event NonQueryCallback CallBack_NonQuery;
        #endregion
        #region Properties - Query Instance
        [DatabaseSettingAttribute(false)]
        public Query_Node QueryNode { get => m_QueryNode; }
        [DatabaseSettingAttribute(false)]
        public Query_Link QueryLink { get => m_QueryLink; }
        [DatabaseSettingAttribute(false)]
        public Query_CommentTag QueryCommentTag { get => m_QueryCommentTag; }
        [DatabaseSettingAttribute(false)]
        public Query_ComponentLife QueryComponentLife { get => m_QueryComponentLife;  }
        [DatabaseSettingAttribute(false)]
        public Query_DestinationChange QueryDestinationChange { get => m_QueryDestinationChange; }
        [DatabaseSettingAttribute(false)]
        public Query_ErrorHistory QueryErrorHistory { get => m_QueryErrorHistory; }
        [DatabaseSettingAttribute(false)]
        public Query_ErrorList QueryErrorList { get => m_QueryErrorList; }
        [DatabaseSettingAttribute(false)]
        public Query_FrontDetectFilter QueryFrontDetectFilter { get => m_QueryFrontDetectFilter; }
        [DatabaseSettingAttribute(false)]
        public Query_GeneralInformation QueryGeneralInformation { get => m_QueryGeneralInformation; }
        [DatabaseSettingAttribute(false)]
        public Query_LinkMergeArea QueryLinkMergeArea { get => m_QueryLinkMergeArea; }
        [DatabaseSettingAttribute(false)]
        public Query_PIODevice QueryPIODevice { get => m_QueryPIODevice; }
        [DatabaseSettingAttribute(false)]
        public Query_Port QueryPort { get => m_QueryPort; }
        [DatabaseSettingAttribute(false)]
        public Query_TransferHistory QueryTransferHistory { get => m_QueryTransferHistory; }
        [DatabaseSettingAttribute(false)]
        public Query_UserList QueryUserList { get => m_QueryUserList; }
        [DatabaseSettingAttribute(false)]
        public Query_EventLog QueryEventLog { get => m_QueryEventLog; }
        [DatabaseSettingAttribute(false)]
        public Query_VelocityLimit QueryVelocityLimit { get => m_QueryVelocityLimit; set => m_QueryVelocityLimit = value; }
        public Query_FDC QueryFDC { get => m_QueryFDC; set => m_QueryFDC = value; }
        public Query_FDCServer QueryFDCServer { get => m_QueryFDCServer; set => m_QueryFDCServer = value; }
        #endregion

        #region Properties
        [DatabaseSettingAttribute(false), DisplayName("CommentTag")]
        public Dictionary<string, DataItem_CommentTag> DictionaryComment { get => m_DictionaryComment; set => m_DictionaryComment = value; }
        [DatabaseSettingAttribute(true), DisplayName("ComponentLife")]
        public Dictionary<ComponentLocation, List<DataItem_ComponentLife>> DictionaryComponentList { get => m_DictionaryComponentList; set => m_DictionaryComponentList= value; }
        [DatabaseSettingAttribute(true), DisplayName("DestinationChange")]
        public Dictionary<int, DataItem_DestinationChange> DictionaryDestinationChangeAreaList { get => m_DictionaryDestinationChangeAreaList; set => m_DictionaryDestinationChangeAreaList = value; }
        [DatabaseSettingAttribute(true), DisplayName("Link")]
        public Dictionary<int, DataItem_Link> DictionaryLinkDataList { get => m_DictionaryLinkDataList; set => m_DictionaryLinkDataList = value; }
        [DatabaseSettingAttribute(true), DisplayName("Node")]
        public Dictionary<int, DataItem_Node> DictionaryNodeDataList { get => m_DictionaryNodeDataList; set => m_DictionaryNodeDataList = value; }
        [DatabaseSettingAttribute(false), DisplayName("ErrorHistory")]
        public Dictionary<DateTime, DataItem_ErrorHistory> DictionaryErrorHistory { get => m_DictionaryErrorHistory; set => m_DictionaryErrorHistory = value; }
        [DatabaseSettingAttribute(false), DisplayName("ErrorList")]
        public Dictionary<int, DataItem_ErrorList> DictionaryErrorList { get => m_DictionaryErrorList; set => m_DictionaryErrorList = value; }
        [DatabaseSettingAttribute(true), DisplayName("FrontDetectFilter")]
        public Dictionary<int, DataItem_FrontDetectFilter> DictionaryFrontDetectFilter { get => m_DictionaryFrontDetectFilter; set => m_DictionaryFrontDetectFilter = value; }
        [DatabaseSettingAttribute(false), DisplayName("GeneralInformation")]
        public Dictionary<GeneralInformationItemName, DataItem_GeneralInformation> DictionaryGeneralInformation { get => m_DictionaryGeneralInformation; set => m_DictionaryGeneralInformation = value; }
        [DatabaseSettingAttribute(true), DisplayName("LinkMergeArea")]
        public Dictionary<int, DataItem_LinkMergeArea> DictionaryLinkMergeArea { get => m_DictionaryLinkMergeArea; set => m_DictionaryLinkMergeArea = value; }
        [DatabaseSettingAttribute(true), DisplayName("PIODevice")]
        public Dictionary<int, DataItem_PIODevice> DictionaryPIODevice { get => m_DictionaryPIODevice; set => m_DictionaryPIODevice = value; }
        [DatabaseSettingAttribute(true), DisplayName("Port")]
        public Dictionary<int, DataItem_Port> DictionaryPortDataList { get => m_DictionaryPortDataList; set => m_DictionaryPortDataList = value; }
        [DatabaseSettingAttribute(true), DisplayName("TransferList")]
        public Dictionary<string, DataItem_TransferInfo> DictionaryTransferHistory { get => m_DictionaryTransferHistory; set => m_DictionaryTransferHistory = value; }
        [DatabaseSettingAttribute(true), DisplayName("UserList")]
        public Dictionary<string, DataItem_UserInfo> DictionaryUserList { get => m_DictionaryUserList; set => m_DictionaryUserList = value; }
        [DatabaseSettingAttribute(true), DisplayName("EventLog")]
        public Dictionary<DateTime, DataItem_EventLog> DictionaryEventLog { get => m_DictionaryEventLog; set => m_DictionaryEventLog = value; }
        [DatabaseSettingAttribute(true), DisplayName("VelocityLimit")]
        public Dictionary<int, DataItem_VelocityLimit> DictionaryVelocityLimit { get => m_DictionaryVelocityLimit; set => m_DictionaryVelocityLimit = value; }
        public Dictionary<int, DataItem_FDC> DictionaryFDC { get => m_DictionaryFDC; set => m_DictionaryFDC = value; }
        #endregion

        #region Constructor
        public DatabaseHandler()
        {
        }

        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;
            m_Initialized = DatabaseAdapter.Instance.Initialize();
            m_Initialized &= TeachingOffsetAdapter.Instance.Initialize();
            if (m_Initialized)
            {
                m_Initialized &= UpdateDatabase();
            }
            return m_Initialized;
        }
        public bool UpdateDatabase()
        {
            bool initialized = true;
            initialized &= Initialize_Node();//우선순위 #1
            initialized &= Initialize_Link();//우선순위 #2 
            initialized &= Initialize_ErrorList();
            initialized &= Initialize_ErrorHistory();
            initialized &= Initialize_Comment();
            initialized &= Initialize_ComponentLife();
            initialized &= Initialize_DestinationChange();
            initialized &= Initialize_FrontDetectFilter();
            initialized &= Initialize_GeneralInformation();
            initialized &= Initialize_LinkMergeArea();
            initialized &= Initialize_PIODevice();
            initialized &= Initialize_Port();
            initialized &= Initialize_TransferList();
            initialized &= Initialize_UserList();
            initialized &= Initialize_EventLog();
            initialized &= Initialize_VelocityLimit();
            initialized &= Initialize_FDC();

            return initialized;
        }
        private void queryExceptionHappened(object sender, object eventData)
        {
            ExceptionLog.WriteLog(string.Format("{0}, {1}", sender.ToString(), eventData));
        }
        private void queryCallBack_NonQuery(int nResult)
        {
            CallBack_NonQuery?.Invoke(nResult);
        }
        #endregion

        #region Node
        public bool Initialize_Node()
        {
            bool rv = false;
            try
            {
                m_QueryNode = new Query_Node(DatabaseAdapter.Instance.GetJobSession());
                m_QueryNode.ExceptionHappened += queryExceptionHappened;
                m_QueryNode.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryNodeDataList.Clear();
                List<DataItem_Node> Items = m_QueryNode.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    m_DictionaryNodeDataList = Items.Select((s, i) => new { s, i }).ToDictionary(x => x.s.NodeID, x => x.s);
                    rv = true;
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }

        public DataItem_Node GetNodeDataOrNull(int nodeID)
        {
            try
            {
                if (m_DictionaryNodeDataList.ContainsKey(nodeID) == true)
                {
                    return m_DictionaryNodeDataList[nodeID];
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Link
        public bool Initialize_Link()
        {
            bool rv = false;
            try
            {
                m_QueryLink = new Query_Link(DatabaseAdapter.Instance.GetJobSession());
                m_QueryLink.ExceptionHappened += queryExceptionHappened;
                m_QueryLink.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryLinkDataList.Clear();
                List<DataItem_Link> Items = m_QueryLink.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    List<DataItem_Link> useItems = Items.Where(x => x.UseFlag == true).ToList();
                    m_DictionaryLinkDataList = useItems.Select((s, i) => new { s, i }).ToDictionary(x => x.s.LinkID, x => x.s);

                    foreach (DataItem_Link link in m_DictionaryLinkDataList.Values)
                    {
                        DataItem_Node fromNode = DictionaryNodeDataList.Values.Where(item => item.NodeID == link.FromNodeID).FirstOrDefault();
                        DataItem_Node toNode = DictionaryNodeDataList.Values.Where(item => item.NodeID == link.ToNodeID).FirstOrDefault();
                        bool sCurve = false;
                        sCurve |= link.Type == LinkType.LeftSBranch;
                        sCurve |= link.Type == LinkType.RightSBranch;
                        sCurve &= link.BCRMatchType != "B";

                        if (fromNode != null)
                        {
                            // Default Value Setting
                            // Start, Target은 Link 내의 Location 위치에서 출발 도착할 경우 필요함.
                            if (sCurve)
                            {
                                if (link.BCRMatchType == "L")
                                {
                                    link.LeftBCRBegin = fromNode.LocationValue1;
                                    link.LeftBCREnd = fromNode.LocationValue1 + link.Distance;
                                    link.RightBCRBegin = toNode.LocationValue2 - link.Distance;
                                    link.RightBCREnd = toNode.LocationValue2;

                                    link.LeftBCRStart = fromNode.LocationValue1;
                                    link.LeftBCRTarget = fromNode.LocationValue1 + link.Distance;
                                    link.RightBCRStart = toNode.LocationValue2 - link.Distance;
                                    link.RightBCRTarget = toNode.LocationValue2;
                                }
                                else if (link.BCRMatchType == "R")
                                {
                                    link.LeftBCRBegin = toNode.LocationValue1 - link.Distance;
                                    link.LeftBCREnd = toNode.LocationValue1;
                                    link.RightBCRBegin = fromNode.LocationValue2;
                                    link.RightBCREnd = fromNode.LocationValue2 + link.Distance;

                                    link.LeftBCRStart = toNode.LocationValue1 - link.Distance;
                                    link.LeftBCRTarget = toNode.LocationValue1;
                                    link.RightBCRStart = fromNode.LocationValue2;
                                    link.RightBCRTarget = fromNode.LocationValue2 + link.Distance;
                                }
                            }
                            else
                            {
                                link.LeftBCRBegin = fromNode.LocationValue1;
                                link.LeftBCREnd = fromNode.LocationValue1 + link.Distance;
                                link.RightBCRBegin = fromNode.LocationValue2;
                                link.RightBCREnd = fromNode.LocationValue2 + link.Distance;
                                link.LeftBCRStart = fromNode.LocationValue1;
                                link.LeftBCRTarget = fromNode.LocationValue1 + link.Distance;
                                link.RightBCRStart = fromNode.LocationValue2;
                                link.RightBCRTarget = fromNode.LocationValue2 + link.Distance;
                            }
                        }
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public DataItem_Link GetLinkDataOrNull(int linkID)
        {
            try
            {
                if (m_DictionaryLinkDataList.ContainsKey(linkID) == true)
                {
                    return m_DictionaryLinkDataList[linkID];
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region CommandTag
        public bool Initialize_Comment()
        {
            bool rv = false;
            try
            {
                m_QueryCommentTag = new Query_CommentTag(DatabaseAdapter.Instance.GetJobSession());
                m_QueryCommentTag.ExceptionHappened += queryExceptionHappened;
                m_QueryCommentTag.CallBack_NonQuery += queryCallBack_NonQuery;

                DictionaryComment.Clear();
                List<DataItem_CommentTag> Items = m_QueryCommentTag.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_CommentTag item in Items)
                    {
                        if (!DictionaryComment.ContainsKey(item.TagID)) 
                            DictionaryComment.Add(item.TagID, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public string GetComment(string ItemTag)
        {
            string comment = string.Empty;
            try
            {
                if (m_DictionaryComment.ContainsKey(ItemTag))
                {
                    comment = m_DictionaryComment[ItemTag].Comment[AppConfig.Instance.AppLanguage];
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
            }
            return comment;
        }
        #endregion

        #region Component Life
        public bool Initialize_ComponentLife()
        {
            bool rv = false;
            try
            {
                m_QueryComponentLife = new Query_ComponentLife(DatabaseAdapter.Instance.GetJobSession());
                m_QueryComponentLife.ExceptionHappened += queryExceptionHappened;
                m_QueryComponentLife.CallBack_NonQuery += queryCallBack_NonQuery;

                DictionaryComponentList.Clear();
                List<DataItem_ComponentLife> Items = m_QueryComponentLife.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    List<DataItem_ComponentLife> componentLifeItems = new List<DataItem_ComponentLife>();
                    foreach (var dataItem in Items)
                    {
                        DataItem_ComponentLife componentLifeItem = new DataItem_ComponentLife()
                        {
                            ComponentName = dataItem.ComponentName,
                            ComponentLocation = dataItem.ComponentLocation,
                            ComponentType = dataItem.ComponentType,
                            ComponentMaker = dataItem.ComponentMaker,
                            LifeTime = dataItem.LifeTime,
                            UseStartTime = dataItem.UseStartTime,
                            UsedTime = dataItem.UsedTime,
                            IsAlwaysUse = dataItem.IsAlwaysUse,
                            DataType = dataItem.DataType,
                        };
                        componentLifeItems.Add(componentLifeItem);
                    }
                    var list = componentLifeItems.GroupBy(x => x.ComponentLocation).ToList();
                    foreach (var item in list)
                    {
                        if (!DictionaryComponentList.ContainsKey(item.Key))
                            DictionaryComponentList.Add(item.Key, list.Where(x => x.Key == item.Key).SelectMany(group => group).ToList());
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion

        #region Destination Change Area List
        public bool Initialize_DestinationChange()
        {
            bool rv = false;
            try
            {
                m_QueryDestinationChange = new Query_DestinationChange(DatabaseAdapter.Instance.GetJobSession());
                m_QueryDestinationChange.ExceptionHappened += queryExceptionHappened;
                m_QueryDestinationChange.CallBack_NonQuery += queryCallBack_NonQuery;

                DictionaryDestinationChangeAreaList.Clear();
                List<DataItem_DestinationChange> Items = m_QueryDestinationChange.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    List<DataItem_DestinationChange> destinationChangeItems = new List<DataItem_DestinationChange>();

                    int maxNodeNumber = DictionaryNodeDataList.Values.Max(item => item.NodeID);
                    double[,] _DistanceMap = new double[maxNodeNumber + 1, maxNodeNumber + 1];

                    for (int from = 0; from < _DistanceMap.GetLength(0); from++)
                    {
                        for (int to = 0; to < _DistanceMap.GetLength(1); to++)
                        {
                            _DistanceMap[from, to] = Constants.INFINITE;
                        }
                    }

                    foreach (DataItem_Link link in DictionaryLinkDataList.Values)
                    {
                        _DistanceMap[link.FromNodeID, link.ToNodeID] = link.Distance;
                    }

                    foreach (DataItem_DestinationChange item in destinationChangeItems)
                    {
                        int start = item.StartNode;
                        int end = item.EndNode;
                        List<int> NodeList = DijkstraAlgorithm.MakePathByDijkstra(start, end, DijkstraAlgorithm.Dijkstra(_DistanceMap, start));
                        item.AreaList = NodeList.ToList();
                        if (!DictionaryDestinationChangeAreaList.ContainsKey(item.ID))
                            DictionaryDestinationChangeAreaList.Add(item.ID, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion

        #region Error History
        public bool Initialize_ErrorHistory()
        {
            bool rv = false;
            try
            {
                m_QueryErrorHistory = new Query_ErrorHistory(DatabaseAdapter.Instance.GetJobSession());
                m_QueryErrorHistory.ExceptionHappened += queryExceptionHappened;
                m_QueryErrorHistory.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryErrorHistory.Clear();
                List<DataItem_ErrorHistory> Items = m_QueryErrorHistory.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_ErrorHistory alarm in Items)
                    {
                        if (m_DictionaryErrorHistory.ContainsKey(alarm.OccurredTime) == false)
                            m_DictionaryErrorHistory.Add(alarm.OccurredTime, alarm);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public bool AddAlarmHistory(DataItem_ErrorHistory item)
        {
            bool add = false;
            try
            {
                m_DictionaryErrorHistory.Add(item.OccurredTime, item);
                m_QueryErrorHistory.Insert(item);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return add;
        }
        public bool RemoveAlarmHistory(DataItem_ErrorHistory item)
        {
            bool add = false;
            try
            {
                m_DictionaryErrorHistory.Remove(item.OccurredTime);
                m_QueryErrorHistory.Delete(item);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return add;
        }
        #endregion

        #region Error List
        public bool Initialize_ErrorList()
        {
            bool rv = false;
            try
            {
                m_QueryErrorList = new Query_ErrorList(DatabaseAdapter.Instance.GetJobSession());
                m_QueryErrorList.ExceptionHappened += queryExceptionHappened;
                m_QueryErrorList.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryErrorList.Clear();
                List<DataItem_ErrorList> Items = m_QueryErrorList.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    m_DictionaryErrorList = Items.Select((s, i) => new { s, i }).ToDictionary(x => x.s.ID, x => x.s);
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public bool AddAlarmList(DataItem_ErrorList item)
        {
            bool add = false;
            try
            {
                if (m_DictionaryErrorList.ContainsKey(item.ID) == false)
                {
                    m_DictionaryErrorList.Add(item.ID, item);
                    m_QueryErrorList.Insert(item);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return add;
        }
        #endregion

        #region Front Detect Filter
        public bool Initialize_FrontDetectFilter()
        {
            bool rv = false;
            try
            {
                m_QueryFrontDetectFilter = new Query_FrontDetectFilter(DatabaseAdapter.Instance.GetJobSession());
                m_QueryFrontDetectFilter.ExceptionHappened += queryExceptionHappened;
                m_QueryFrontDetectFilter.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryFrontDetectFilter.Clear();
                List<DataItem_FrontDetectFilter> Items = m_QueryFrontDetectFilter.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    m_DictionaryFrontDetectFilter = Items.Select((s, i) => new { s, i }).ToDictionary(x => x.s.ID, x => x.s);
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion

        #region General Information
        public bool Initialize_GeneralInformation()
        {
            bool rv = false;
            try
            {
                m_QueryGeneralInformation = new Query_GeneralInformation(DatabaseAdapter.Instance.GetJobSession());
                m_QueryGeneralInformation.ExceptionHappened += queryExceptionHappened;
                m_QueryGeneralInformation.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryGeneralInformation.Clear();
                List<DataItem_GeneralInformation> Items = m_QueryGeneralInformation.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_GeneralInformation item in Items)
                    {
                        if (!DictionaryGeneralInformation.ContainsKey(item.ItemName))
                            DictionaryGeneralInformation.Add(item.ItemName, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion

        #region Link Merge Area
        public bool Initialize_LinkMergeArea()
        {
            bool rv = false;
            try
            {
                m_QueryLinkMergeArea = new Query_LinkMergeArea(DatabaseAdapter.Instance.GetJobSession());
                m_QueryLinkMergeArea.ExceptionHappened += queryExceptionHappened;
                m_QueryLinkMergeArea.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryLinkMergeArea.Clear();
                List<DataItem_LinkMergeArea> Items = m_QueryLinkMergeArea.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_LinkMergeArea item in Items)
                    {
                        if (!m_DictionaryLinkMergeArea.ContainsKey(item.AreaID))
                            m_DictionaryLinkMergeArea.Add(item.AreaID, item);
                    }

                    if (DictionaryLinkDataList.Count > 0)
                    {
                        foreach (DataItem_LinkMergeArea item in m_DictionaryLinkMergeArea.Values)
                        {
                            if (item.NodeIdList.Count > 1)
                            {
                                item.LinkIdList.Clear();
                                for (int i = 0; i < item.NodeIdList.Count - 1; i++)
                                {
                                    int fromNode = item.NodeIdList[i];
                                    int toNode = item.NodeIdList[i + 1];
                                    DataItem_Link link = GetLinkData(fromNode, toNode, DictionaryLinkDataList);
                                    if (link != null) item.LinkIdList.Add(link.LinkID);
                                }
                            }
                        }
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public DataItem_Link GetLinkData(int sourceID, int destinationID, Dictionary<int, DataItem_Link> linkDataLists)
        {
            DataItem_Link link = null;
            try
            {
                List<KeyValuePair<int, DataItem_Link>> lists = linkDataLists.Where(item => item.Value.FromNodeID == sourceID && item.Value.ToNodeID == destinationID).ToList();

                if (lists.Count > 0)
                {
                    link = lists.FirstOrDefault().Value;
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return link;
        }
        #endregion

        #region PIO Device
        public bool Initialize_PIODevice()
        {
            bool rv = false;
            try
            {
                m_QueryPIODevice = new Query_PIODevice(DatabaseAdapter.Instance.GetJobSession());
                m_QueryPIODevice.ExceptionHappened += queryExceptionHappened;
                m_QueryPIODevice.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryPIODevice.Clear();
                List<DataItem_PIODevice> Items = m_QueryPIODevice.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_PIODevice item in Items)
                    {
                        if (!m_DictionaryPIODevice.ContainsKey(item.NodeID))
                            m_DictionaryPIODevice.Add(item.NodeID, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion

        #region Port
        public bool Initialize_Port()
        {
            bool rv = false;
            try
            {
                m_QueryPort = new Query_Port(DatabaseAdapter.Instance.GetJobSession());
                m_QueryPort.ExceptionHappened += queryExceptionHappened;
                m_QueryPort.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryPortDataList.Clear();
                List<DataItem_Port> Items = m_QueryPort.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_Port item in Items)
                    {
                        if (!m_DictionaryPortDataList.ContainsKey(item.PortID))
                            m_DictionaryPortDataList.Add(item.PortID, item);
                        TeachingOffsetAdapter.Instance.CheckOffset(item);
                    }
                }
                m_QueryPort.Update(Items);

                /// Teaching Offset Update
                TeachingOffsetAdapter.Instance.Save();
                ///////////////////////////////////////
               
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public bool UpdatePort(DataItem_Port port)
        {
            bool update = false;
            try
            {
                if (m_DictionaryPortDataList.ContainsKey(port.PortID))
                {
                    m_DictionaryPortDataList[port.PortID] = port;
                }
                m_QueryPort.Update(port);
                TeachingOffsetAdapter.Instance.UpdateSaveOffset(port);
                update = true;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return update;
        }
        #endregion

        #region Transfer Command List
        public bool Initialize_TransferList()
        {
            bool rv = false;
            try
            {
                m_QueryTransferHistory = new Query_TransferHistory(DatabaseAdapter.Instance.GetJobSession());
                m_QueryTransferHistory.ExceptionHappened += queryExceptionHappened;
                m_QueryTransferHistory.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryTransferHistory.Clear();

                DateTime startTime = DateTime.Now.AddDays(-30);
                DateTime endTime = DateTime.Now;
                List<DataItem_TransferInfo> Items = m_QueryTransferHistory.SelectListOrNull(startTime, endTime, ProcessStatus.None);
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_TransferInfo item in Items)
                    {
                        if (!m_DictionaryTransferHistory.ContainsKey(item.CommandID))
                            m_DictionaryTransferHistory.Add(item.CommandID, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }

        public void AddTransferCommand(DataItem_TransferInfo item)
        {
            try
            {
                if (m_DictionaryTransferHistory.ContainsKey(item.CommandID) == false)
                {
                    m_DictionaryTransferHistory.Add(item.CommandID, item);
                    m_QueryTransferHistory.Insert(item);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void UpdateTransferCommand(DataItem_TransferInfo item)
        {
            try
            {
                if (m_DictionaryTransferHistory.ContainsKey(item.CommandID) == true)
                {
                    m_DictionaryTransferHistory[item.CommandID] = ObjectCopier.Clone(item);
                    m_QueryTransferHistory.Update(item);
                }
                else
                {
                    AddTransferCommand(item);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void RemoveTransferCommand(DataItem_TransferInfo item)
        {
            try
            {
                if (m_DictionaryTransferHistory.ContainsKey(item.CommandID))
                {
                    m_DictionaryTransferHistory.Remove(item.CommandID);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public void UpdateTransferTime(string commandID, transferUpdateTime enTime)
        {
            try
            {
                string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
                if (m_DictionaryTransferHistory.ContainsKey(commandID))
                {
                    if (enTime == transferUpdateTime.Install) m_DictionaryTransferHistory[commandID].InstallTime = DateTime.Now;//.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.CommandAssigned) m_DictionaryTransferHistory[commandID].AssignTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.MoveToSource) m_DictionaryTransferHistory[commandID].AssignTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.ArrivedSource) m_DictionaryTransferHistory[commandID].VehicleFromArrivedTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.AquireStart) m_DictionaryTransferHistory[commandID].VehicleAcquireStartTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.AquireEnd) m_DictionaryTransferHistory[commandID].VehicleAcquireEndTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.MoveToDestination) m_DictionaryTransferHistory[commandID].VehicleDepartedTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.ArrivedDestination) m_DictionaryTransferHistory[commandID].VehicleToArrivedTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.DepositStart) m_DictionaryTransferHistory[commandID].VehicleDepositStartTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.DepositEnd) m_DictionaryTransferHistory[commandID].VehicleDepositEndTime = DateTime.Now.ToString(timeFormat);
                    else if (enTime == transferUpdateTime.CommandCompleted) m_DictionaryTransferHistory[commandID].TransferCompletedTime = DateTime.Now.ToString(timeFormat);
                    m_QueryTransferHistory.UpdateTime(m_DictionaryTransferHistory[commandID]);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public DataItem_TransferInfo GetTransferCommand(string command_id)
        {
            DataItem_TransferInfo item = null;
            try
            {
                if (m_DictionaryTransferHistory.ContainsKey(command_id) == true)
                {
                    item = ObjectCopier.Clone(m_DictionaryTransferHistory[command_id]);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return item;
        }
        #endregion

        #region UserList
        public bool Initialize_UserList()
        {
            bool rv = false;
            try
            {
                m_QueryUserList = new Query_UserList(DatabaseAdapter.Instance.GetJobSession());
                m_QueryUserList.ExceptionHappened += queryExceptionHappened;
                m_QueryUserList.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryUserList.Clear();
                List<DataItem_UserInfo> Items = m_QueryUserList.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_UserInfo item in Items)
                    {
                        if (!m_DictionaryUserList.ContainsKey(item.UserName))
                            m_DictionaryUserList.Add(item.UserName, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }

        #endregion

        #region EventLog
        public bool Initialize_EventLog()
        {
            bool rv = false;
            try
            {
                m_QueryEventLog = new Query_EventLog(DatabaseAdapter.Instance.GetJobSession());
                m_QueryEventLog.ExceptionHappened += queryExceptionHappened;
                m_QueryEventLog.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryEventLog.Clear();
                List<DataItem_EventLog> Items = m_QueryEventLog.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    foreach (DataItem_EventLog item in Items)
                    {
                        if (!m_DictionaryEventLog.ContainsKey(item.Time))
                            m_DictionaryEventLog.Add(item.Time, item);
                    }
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public bool AddEventLog(DataItem_EventLog item)
        {
            bool add = false;
            try
            {
                m_DictionaryEventLog.Add(item.Time, item);
                m_QueryEventLog.Insert(item);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return add;
        }
        public bool RemoveEventLog(DataItem_EventLog item)
        {
            bool add = false;
            try
            {
                m_DictionaryEventLog.Remove(item.Time);
                m_QueryEventLog.Delete(item);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return add;
        }
        #endregion

        #region Velocity Limit
        public bool Initialize_VelocityLimit()
        {
            bool rv = false;
            try
            {
                m_QueryVelocityLimit = new Query_VelocityLimit(DatabaseAdapter.Instance.GetJobSession());
                m_QueryVelocityLimit.ExceptionHappened += queryExceptionHappened;
                m_QueryVelocityLimit.CallBack_NonQuery += queryCallBack_NonQuery;

                m_DictionaryVelocityLimit.Clear();
                List<DataItem_VelocityLimit> Items = m_QueryVelocityLimit.SelectAllOrNull();
                if (Items != null && Items.Count > 0)
                {
                    m_DictionaryVelocityLimit = Items.Select((s, i) => new { s, i }).ToDictionary(x => x.s.ID, x => x.s);
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        public bool UpdateVelocityLimit(DataItem_VelocityLimit velLimit)
        {
            bool update = false;
            try
            {
                if (m_DictionaryVelocityLimit.ContainsKey(velLimit.LinkId))
                {
                    m_DictionaryVelocityLimit[velLimit.LinkId] = velLimit;
                }
                m_QueryVelocityLimit.Update(velLimit);
                update = true;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return update;
        }

        #endregion

        #region FDC
        public bool Initialize_FDC()
        {
            bool rv = false;
            try
            {
                m_QueryFDC = new Query_FDC(DatabaseAdapter.Instance.GetJobSession());
                m_QueryFDC.ExceptionHappened += queryExceptionHappened;
                m_QueryFDC.CallBack_NonQuery += queryCallBack_NonQuery;

                m_QueryFDCServer = new Query_FDCServer(DatabaseAdapter.Instance.GetFDCSession());
                m_QueryFDCServer.ExceptionHappened += queryExceptionHappened;
                m_QueryFDCServer.CallBack_NonQuery += queryCallBack_NonQuery;
                

                DictionaryFDC.Clear();
                List<DataItem_FDC> Items = m_QueryFDC.SelectAllOrNull();

                if (Items != null && Items.Count > 0)
                {
                    DictionaryFDC = Items.Select((s, i) => new { s, i }).ToDictionary(x => x.s.ID, x => x.s);
                    rv = true;
                }
                rv = Items != null;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return rv;
        }
        #endregion
    }
}
