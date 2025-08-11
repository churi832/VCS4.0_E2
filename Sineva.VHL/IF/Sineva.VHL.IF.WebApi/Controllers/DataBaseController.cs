using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.IF.WebApi.Models.Dtos;
using Sineva.VHL.IF.WebApi.Models.ReturnModels;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    /// <summary>
    /// Database接口
    /// </summary>
    public class DataBaseController : ApiController
    {
        private Query_Link m_QueryLink = null;
        private Query_Node m_QueryNode = null;
        private Query_Port m_QueryPort = null;
        private Query_FrontDetectFilter m_QueryFrontDetectFilter = null;
        private Query_VelocityLimit m_QueryVelocityLimit = null;
        /// <summary>
        /// 获取Link列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetLinkList(LinkListDto linkListDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (linkListDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {linkListDto.LinkID}";
                }
                if (linkListDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {linkListDto.UseFlag}";
                }
                if (linkListDto.FromNode > -1)
                {
                    searchCondition += $" and FromNodeID = {linkListDto.FromNode}";
                }
                if (linkListDto.ToNode > -1)
                {
                    searchCondition += $" and ToNodeID = {linkListDto.ToNode}";
                }
                if (linkListDto.LinkType > -1)
                {
                    searchCondition += $" and LinkType = {linkListDto.LinkType}";
                }
                string countSql = "select count(*) from link where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / linkListDto.PageSize);
                pagetotal = pagetotal * linkListDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (linkListDto.PageNo < 1)
                { linkListDto.PageNo = 1; }
                if (linkListDto.PageSize * (linkListDto.PageNo - 1) > count)
                {
                    linkListDto.PageNo = pagetotal;
                }
                string querySql = $"select * from link where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by linkID offset {(linkListDto.PageNo - 1) * linkListDto.PageSize} rows fetch next {linkListDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<LinkListData> tableData = new List<LinkListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        int linkID = Convert.ToInt32(dr["LinkID"].ToString());
                        bool useFlag = bool.Parse(dr["UseFlag"].ToString());
                        int fromNode = Convert.ToInt32(dr["FromNodeID"].ToString());
                        int toNode = Convert.ToInt32(dr["ToNodeID"].ToString());
                        int linkType = Convert.ToInt32(dr["LinkType"].ToString());
                        string BCRMatchType = dr["BCRMatchType"].ToString();
                        int steerDirection = Convert.ToInt32(dr["SteerDirection"].ToString());
                        int steerChangeLeftBCR = Convert.ToInt32(dr["SteerChangeLeftBCR"].ToString());
                        int steerChangeRightBCR = Convert.ToInt32(dr["SteerChangeRightBCR"].ToString());
                        double time = Convert.ToDouble(dr["Time"].ToString());
                        int weight = Convert.ToInt32(dr["Weight"].ToString());
                        double distance = Convert.ToDouble(dr["Distance"].ToString());
                        double velocity = Convert.ToDouble(dr["Velocity"].ToString());
                        double acceleration = Convert.ToDouble(dr["Acceleration"].ToString());
                        double deceleration = Convert.ToDouble(dr["Deceleration"].ToString());
                        double jerk = Convert.ToDouble(dr["Jerk"].ToString());
                        int level0 = Convert.ToInt32(dr["UBSLevel0"].ToString());
                        int level1 = Convert.ToInt32(dr["UBSLevel1"].ToString());
                        bool routeChange = bool.Parse(dr["RouteChangeCheck"].ToString());
                        double steerGuideFrom = Convert.ToDouble(dr["SteerGuideLengthFromNode"].ToString());
                        double steerGuideTo = Convert.ToDouble(dr["SteerGuideLengthToNode"].ToString());
                        bool JCSAreaFlag = bool.Parse(dr["JCSAreaFlag"].ToString());
                        int FromExtensionDistance = Convert.ToInt32(dr["FromExtensionDistance"].ToString());
                        int ToExtensionDistance = Convert.ToInt32(dr["ToExtensionDistance"].ToString());

                        LinkListData data = new LinkListData()
                        {
                            LinkID = linkID,
                            UseFlag = useFlag,
                            FromNodeID = fromNode,
                            ToNodeID = toNode,
                            LinkType = linkType,
                            SteerDirectionValue = steerDirection,
                            BCRMatchType = BCRMatchType,
                            SteerChangeLeftBCR = steerChangeLeftBCR,
                            SteerChangeRightBCR = steerChangeRightBCR,
                            Time = time,
                            Weight = weight,
                            Distance = distance,
                            Velocity = velocity,
                            Acceleration = acceleration,
                            Deceleration = deceleration,
                            Jerk = jerk,
                            UBSLevel0 = level0,
                            UBSLevel1 = level1,
                            RouteChangeCheck = routeChange,
                            SteerGuideLengthFromNode = steerGuideFrom,
                            SteerGuideLengthToNode = steerGuideTo,
                            JCSAreaFlag = JCSAreaFlag,
                            FromExtensionDistance = FromExtensionDistance,
                            ToExtensionDistance = ToExtensionDistance,
                        };
                        tableData.Add(data);
                    }
                    LinkListRtn retdata = new LinkListRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.LinkListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "Link Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Link Data acquiring failed！";
                    returnCode.status = 210;
                    returnCode.data = tableData;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 更新Link
        /// </summary>
        /// <param name="linkDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdateLink(LinkDto linkDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryLink = new Query_Link(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Link dataItem_Link = new DataItem_Link()
                {
                    LinkID = linkDto.LinkID,
                    UseFlag = linkDto.UseFlag,
                    FromNodeID = linkDto.FromNodeID,
                    ToNodeID = linkDto.ToNodeID,
                    Type = (LinkType)linkDto.Type,
                    SteerDirectionValue = (enSteerDirection)linkDto.SteerDirectionValue,
                    BCRMatchType = linkDto.BCRMatchType,
                    SteerChangeLeftBCR = linkDto.SteerChangeLeftBCR,
                    SteerChangeRightBCR = linkDto.SteerChangeRightBCR,
                    Time = linkDto.Time,
                    Weight = linkDto.Weight,
                    Distance = linkDto.Distance,
                    Velocity = linkDto.Velocity,
                    Acceleration = linkDto.Acceleration,
                    Deceleration = linkDto.Deceleration,
                    Jerk = linkDto.Jerk,
                    UBSLevel0 = linkDto.UBSLevel0,
                    UBSLevel1 = linkDto.UBSLevel1,
                    RouteChangeCheck = linkDto.RouteChangeCheck,
                    SteerGuideLengthFromNode = linkDto.SteerGuideLengthFromNode,
                    SteerGuideLengthToNode = linkDto.SteerGuideLengthToNode,
                    JCSAreaFlag = linkDto.JCSAreaFlag,
                    FromExtensionDistance = linkDto.FromExtensionDistance,
                    ToExtensionDistance = linkDto.ToExtensionDistance,
                };
                int result = m_QueryLink.Update(dataItem_Link);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Link Data acquired successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Link Data acquired failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 添加Link
        /// </summary>
        /// <param name="linkDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode AddLink(LinkDto linkDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryLink = new Query_Link(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Link dataItem_Link = new DataItem_Link()
                {
                    LinkID = linkDto.LinkID,
                    UseFlag = linkDto.UseFlag,
                    FromNodeID = linkDto.FromNodeID,
                    ToNodeID = linkDto.ToNodeID,
                    Type = (LinkType)linkDto.Type,
                    SteerDirectionValue = (enSteerDirection)linkDto.SteerDirectionValue,
                    BCRMatchType = linkDto.BCRMatchType,
                    SteerChangeLeftBCR = linkDto.SteerChangeLeftBCR,
                    SteerChangeRightBCR = linkDto.SteerChangeRightBCR,
                    Time = linkDto.Time,
                    Weight = linkDto.Weight,
                    Distance = linkDto.Distance,
                    Velocity = linkDto.Velocity,
                    Acceleration = linkDto.Acceleration,
                    Deceleration = linkDto.Deceleration,
                    Jerk = linkDto.Jerk,
                    UBSLevel0 = linkDto.UBSLevel0,
                    UBSLevel1 = linkDto.UBSLevel1,
                    RouteChangeCheck = linkDto.RouteChangeCheck,
                    SteerGuideLengthFromNode = linkDto.SteerGuideLengthFromNode,
                    SteerGuideLengthToNode = linkDto.SteerGuideLengthToNode,
                    JCSAreaFlag = linkDto.JCSAreaFlag,
                    FromExtensionDistance = linkDto.FromExtensionDistance,
                    ToExtensionDistance = linkDto.ToExtensionDistance,
                };
                int result = m_QueryLink.Insert(dataItem_Link);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Link Data add successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Link Data add failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 删除Link
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode DeleteLink(int linkID)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryLink = new Query_Link(DatabaseAdapter.Instance.GetJobSession());
                int result = m_QueryLink.Delete(linkID);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Link Data delete successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Link Data delete failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }

        /// <summary>
        /// 导出Link列表
        /// </summary>
        /// <param name="linkListDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportLinkList(LinkListDto linkListDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "LinkList_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> lindData = new List<string>();
                string searchCondition = string.Empty;
                if (linkListDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {linkListDto.LinkID}";
                }
                if (linkListDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {linkListDto.UseFlag}";
                }
                if (linkListDto.FromNode > -1)
                {
                    searchCondition += $" and FromNodeID = {linkListDto.FromNode}";
                }
                if (linkListDto.ToNode > -1)
                {
                    searchCondition += $" and ToNodeID = {linkListDto.ToNode}";
                }
                if (linkListDto.LinkType > -1)
                {
                    searchCondition += $" and LinkType = {linkListDto.LinkType}";
                }
                string querySql = $"select * from link where 1=1 ";
                querySql += searchCondition;

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        dataline += Convert.ToInt32(dr["LinkID"].ToString()) + ",";
                        dataline += bool.Parse(dr["UseFlag"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["FromNodeID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["ToNodeID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LinkType"].ToString()) + ",";
                        dataline += dr["BCRMatchType"].ToString() + ",";
                        dataline += Convert.ToInt32(dr["SteerDirection"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["SteerChangeLeftBCR"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["SteerChangeRightBCR"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Time"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["Weight"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Distance"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Velocity"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Acceleration"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Deceleration"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["Jerk"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["UBSLevel0"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["UBSLevel1"].ToString()) + ",";
                        dataline += bool.Parse(dr["RouteChangeCheck"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["SteerGuideLengthFromNode"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["SteerGuideLengthToNode"].ToString()) + ",";
                        dataline += bool.Parse(dr["JCSAreaFlag"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["FromExtensionDistance"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["ToExtensionDistance"].ToString());
                        lindData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, lindData);
                    byte[] bytes = Encoding.UTF8.GetBytes(alarmString);
                    MemoryStream ms = new MemoryStream(bytes);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    httpResponseMessage.Content = new StreamContent(ms);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
                    {
                        FileName =
                    browser.Contains("FIREFOX")
                        ? fileName
                        : HttpUtility.UrlEncode(fileName)
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    httpResponseMessage.ReasonPhrase = "Link List Data acquiring failed！";
                    return ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                httpResponseMessage.ReasonPhrase = ex.Message;
            }
            return ResponseMessage(httpResponseMessage);
        }
        /// <summary>
        /// 获取Node列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetNodeList(NodeListDto nodeListDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (nodeListDto.NodeID > -1)
                {
                    searchCondition += $" and NodeID = {nodeListDto.NodeID}";
                }
                if (nodeListDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {nodeListDto.UseFlag}";
                }
                if (nodeListDto.NodeType > -1)
                {
                    searchCondition += $" and NodeType = {nodeListDto.NodeType}";
                }

                string countSql = "select count(*) from node where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / nodeListDto.PageSize);
                pagetotal = pagetotal * nodeListDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (nodeListDto.PageNo < 1)
                { nodeListDto.PageNo = 1; }
                if (nodeListDto.PageSize * (nodeListDto.PageNo - 1) > count)
                {
                    nodeListDto.PageNo = pagetotal;
                }
                string querySql = $"select * from node where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by nodeID offset {(nodeListDto.PageNo - 1) * nodeListDto.PageSize} rows fetch next {nodeListDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<NodeListData> tableData = new List<NodeListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        int nodeID = Convert.ToInt32(dr["NodeID"].ToString());
                        bool useFlag = bool.Parse(dr["UseFlag"].ToString());
                        int leftBCR = Convert.ToInt32(dr["LocationValue1"].ToString());
                        int rightBCR = Convert.ToInt32(dr["LocationValue2"].ToString());
                        int nodeType = Convert.ToInt32(dr["NodeType"].ToString());
                        int UBSLevel = Convert.ToInt32(dr["UBSLevel"].ToString());
                        int UBSCheckSensor = Convert.ToInt32(dr["UBSCheckSensor"].ToString());
                        int JCSCheck = Convert.ToInt32(dr["JCSCheck"].ToString());

                        NodeListData data = new NodeListData()
                        {
                            NodeID = nodeID,
                            UseFlag = useFlag,
                            LocationValue1 = leftBCR,
                            LocationValue2 = rightBCR,
                            NodeType = nodeType,
                            UBSLevel = UBSLevel,
                            UBSCheckSensor = UBSCheckSensor,
                            JCSCheck = JCSCheck,
                        };
                        tableData.Add(data);
                    }
                    NodeListRtn retdata = new NodeListRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.NodeListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "Node Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Node Data acquiring failed！";
                    returnCode.status = 210;
                    returnCode.data = tableData;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 更新Node
        /// </summary>
        /// <param name="nodeDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdateNode(NodeDto nodeDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryNode = new Query_Node(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Node dataItem_Node = new DataItem_Node()
                {
                    NodeID = nodeDto.NodeID,
                    UseFlag = nodeDto.UseFlag,
                    LocationValue1 = nodeDto.LocationValue1,
                    LocationValue2 = nodeDto.LocationValue2,
                    Type = (NodeType)nodeDto.Type,
                    CPSZoneID = nodeDto.CPSZoneID,
                    UBSLevel = nodeDto.UBSLevel,
                    UBSCheckSensor = nodeDto.UBSCheckSensor,
                    JCSCheck = nodeDto.JCSCheck,
                };
                int result = m_QueryNode.Update(dataItem_Node);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Node Data acquired successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Node Data acquired failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 添加Node
        /// </summary>
        /// <param name="nodeDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode AddNode(NodeDto nodeDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryNode = new Query_Node(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Node dataItem_Node = new DataItem_Node()
                {
                    NodeID = nodeDto.NodeID,
                    UseFlag = nodeDto.UseFlag,
                    LocationValue1 = nodeDto.LocationValue1,
                    LocationValue2 = nodeDto.LocationValue2,
                    Type = (NodeType)nodeDto.Type,
                    CPSZoneID = nodeDto.CPSZoneID,
                    UBSLevel = nodeDto.UBSLevel,
                    UBSCheckSensor = nodeDto.UBSCheckSensor,
                    JCSCheck = nodeDto.JCSCheck,
                };
                int result = m_QueryNode.Insert(dataItem_Node);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Node Data add successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Node Data add failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 删除Node
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode DeleteNode(int nodeID)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryNode = new Query_Node(DatabaseAdapter.Instance.GetJobSession());
                int result = m_QueryNode.Delete(nodeID);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Node Data delete successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Node Data delete failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }

        /// <summary>
        /// 导出Node列表
        /// </summary>
        /// <param name="nodeListDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportNodeList(NodeListDto nodeListDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "NodeList_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> nodeData = new List<string>();
                string searchCondition = string.Empty;
                if (nodeListDto.NodeID > -1)
                {
                    searchCondition += $" and NodeID = {nodeListDto.NodeID}";
                }
                if (nodeListDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {nodeListDto.UseFlag}";
                }
                if (nodeListDto.NodeType > -1)
                {
                    searchCondition += $" and NodeType = {nodeListDto.NodeType}";
                }
                string querySql = $"select * from node where 1=1 ";
                querySql += searchCondition;

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        dataline += Convert.ToInt32(dr["NodeID"].ToString()) + ",";
                        dataline += bool.Parse(dr["UseFlag"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LocationValue1"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LocationValue2"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["NodeType"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["UBSLevel"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["UBSCheckSensor"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["JCSCheck"].ToString());
                        nodeData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, nodeData);
                    byte[] bytes = Encoding.UTF8.GetBytes(alarmString);
                    MemoryStream ms = new MemoryStream(bytes);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    httpResponseMessage.Content = new StreamContent(ms);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
                    {
                        FileName =
                    browser.Contains("FIREFOX")
                        ? fileName
                        : HttpUtility.UrlEncode(fileName)
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    httpResponseMessage.ReasonPhrase = "Node List Data acquiring failed！";
                    return ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                httpResponseMessage.ReasonPhrase = ex.Message;
            }
            return ResponseMessage(httpResponseMessage);
        }
        /// <summary>
        /// 获取Port列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetPortList(PortListDto portListDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (portListDto.PortID > -1)
                {
                    searchCondition += $" and PortID = {portListDto.PortID}";
                }
                if (portListDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {portListDto.LinkID}";
                }
                if (portListDto.NodeID > -1)
                {
                    searchCondition += $" and NodeID = {portListDto.NodeID}";
                }
                if (portListDto.PortType > -1)
                {
                    searchCondition += $" and PortType = {portListDto.PortType}";
                }
                if (portListDto.PIOUse > -1)
                {
                    searchCondition += $" and PIOUse = {portListDto.PIOUse}";
                }
                if (portListDto.PBSUse > -1)
                {
                    searchCondition += $" and PBSUse = {portListDto.PBSUse}";
                }
                string countSql = "select count(*) from port where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / portListDto.PageSize);
                pagetotal = pagetotal * portListDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (portListDto.PageNo < 1)
                { portListDto.PageNo = 1; }
                if (portListDto.PageSize * (portListDto.PageNo - 1) > count)
                {
                    portListDto.PageNo = pagetotal;
                }
                string querySql = $"select * from port where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by portID offset {(portListDto.PageNo - 1) * portListDto.PageSize} rows fetch next {portListDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<PortListData> tableData = new List<PortListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        PortListData data = new PortListData()
                        {
                            PortID = Convert.ToInt32(dr["PortID"].ToString()),
                            LinkID = Convert.ToInt32(dr["LinkID"].ToString()),
                            NodeID = Convert.ToInt32(dr["NodeID"].ToString()),
                            PortType = Convert.ToInt32(dr["PortType"].ToString()),
                            BeforeHoistPosition = Convert.ToDouble(dr["BeforeHoistPosition"].ToString()),
                            HoistPosition = Convert.ToDouble(dr["HoistPosition"].ToString()),
                            SlidePosition = Convert.ToDouble(dr["SlidePosition"].ToString()),
                            RotatePosition = Convert.ToDouble(dr["RotatePosition"].ToString()),
                            BeforeUnloadHoistPosition = Convert.ToDouble(dr["BeforeUnloadHoistPosition"].ToString()),
                            UnloadHoistPosition = Convert.ToDouble(dr["UnloadHoistPosition"].ToString()),
                            UnloadSlidePosition = Convert.ToDouble(dr["UnloadSlidePosition"].ToString()),
                            UnloadRotatePosition = Convert.ToDouble(dr["UnloadRotatePosition"].ToString()),
                            BarcodeLeft = Convert.ToDouble(dr["BarcodeLeft"].ToString()),
                            BarcodeRight = Convert.ToDouble(dr["BarcodeRight"].ToString()),
                            PIOID = Convert.ToInt32(dr["PIOID"].ToString()),
                            PIOCH = Convert.ToInt32(dr["PIOCH"].ToString()),
                            PIOCS = Convert.ToInt32(dr["PIOCS"].ToString()),
                            PIOUsed = bool.Parse(dr["PIOUsed"].ToString()),
                            PortProhibition = bool.Parse(dr["PortProhibition"].ToString()),
                            OffsetUsed = bool.Parse(dr["OffsetUsed"].ToString()),
                            PBSUsed = bool.Parse(dr["PBSUsed"].ToString()),
                            PBSSelectNo = Convert.ToInt32(dr["PBSSelectNo"].ToString()),
                            ProfileExistPosition = Convert.ToInt32(dr["ProfileExistPosition"].ToString()),
                        };
                        tableData.Add(data);
                    }
                    PortListRtn retdata = new PortListRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.PortListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "Port Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Port Data acquiring failed！";
                    returnCode.status = 210;
                    returnCode.data = tableData;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 更新Port
        /// </summary>
        /// <param name="portDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdatePort(PortDto portDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryPort = new Query_Port(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Port dataItem_Port = new DataItem_Port()
                {
                    PIOUsed = portDto.PIOUsed,
                    State = portDto.State,
                    PortProhibition = portDto.PortProhibition,
                    OffsetUsed = portDto.OffsetUsed,
                    PortType = (PortType)portDto.PortType,
                    PortID = portDto.PortID,
                    LinkID = portDto.LinkID,
                    NodeID = portDto.NodeID,
                    PIOID = portDto.PIOID,
                    PIOCH = portDto.PIOCH,
                    PIOCS = portDto.PIOCS,
                    SlidePosition = portDto.SlidePosition,
                    RotatePosition = portDto.RotatePosition,
                    BeforeHoistPosition = portDto.BeforeHoistPosition,
                    HoistPosition = portDto.HoistPosition,
                    UnloadSlidePosition = portDto.UnloadSlidePosition,
                    UnloadRotatePosition = portDto.UnloadRotatePosition,
                    BeforeUnloadHoistPosition = portDto.BeforeUnloadHoistPosition,
                    UnloadHoistPosition = portDto.UnloadHoistPosition,
                    BarcodeLeft = portDto.BarcodeLeft,
                    BarcodeRight = portDto.BarcodeRight,
                    PBSSelectNo = portDto.PBSSelectNo,
                    PBSUsed = portDto.PBSUsed,
                    DriveLeftOffset = portDto.DriveLeftOffset,
                    DriveRightOffset = portDto.DriveRightOffset,
                    SlideOffset = portDto.SlideOffset,
                    HoistOffset = portDto.HoistOffset,
                    RotateOffset = portDto.RotateOffset,
                    ProfileExistPosition = (enProfileExistPosition)portDto.ProfileExistPosition,
                };
                int result = m_QueryPort.Update(dataItem_Port);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Port Data acquired successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Port Data acquired failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 更新Teaching Data
        /// </summary>
        /// <param name="teachingDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdateTeaching(TeachingDto teachingDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryPort = new Query_Port(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Port dataItem_Port = m_QueryPort.SelectSingleOrNullByPortId(teachingDto.PortID);
                if (teachingDto.IsLoadPosition)
                {
                    dataItem_Port.BarcodeLeft = teachingDto.BarcodeLeft;
                    dataItem_Port.BarcodeRight = teachingDto.BarcodeRight;
                    dataItem_Port.HoistPosition = teachingDto.HoistPosition;
                    dataItem_Port.SlidePosition = teachingDto.SlidePosition;
                    dataItem_Port.RotatePosition = teachingDto.RotatePosition;
                }
                else
                {
                    dataItem_Port.BarcodeLeft = teachingDto.BarcodeLeft;
                    dataItem_Port.BarcodeRight = teachingDto.BarcodeRight;
                    dataItem_Port.UnloadHoistPosition = teachingDto.UnloadHoistPosition;
                    dataItem_Port.UnloadSlidePosition = teachingDto.UnloadSlidePosition;
                    dataItem_Port.UnloadRotatePosition = teachingDto.UnloadRotatePosition;
                }

                int result = m_QueryPort.Update(dataItem_Port);
                if (result > 0)
                {
                    if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                    {
                        RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.DataRefresh, new WebOffsetUpdate
                        {
                            PortID = teachingDto.PortID,
                        });
                    }
                    returnCode.succeed = true;
                    returnCode.msg = "Teaching Data update successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Teaching Data update failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 添加Port
        /// </summary>
        /// <param name="portDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode AddPort(PortDto portDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryPort = new Query_Port(DatabaseAdapter.Instance.GetJobSession());
                DataItem_Port dataItem_Port = new DataItem_Port()
                {
                    PIOUsed = portDto.PIOUsed,
                    State = portDto.State,
                    PortProhibition = portDto.PortProhibition,
                    OffsetUsed = portDto.OffsetUsed,
                    PortType = (PortType)portDto.PortType,
                    PortID = portDto.PortID,
                    LinkID = portDto.LinkID,
                    NodeID = portDto.NodeID,
                    PIOID = portDto.PIOID,
                    PIOCH = portDto.PIOCH,
                    PIOCS = portDto.PIOCS,
                    SlidePosition = portDto.SlidePosition,
                    RotatePosition = portDto.RotatePosition,
                    BeforeHoistPosition = portDto.BeforeHoistPosition,
                    HoistPosition = portDto.HoistPosition,
                    UnloadSlidePosition = portDto.UnloadSlidePosition,
                    UnloadRotatePosition = portDto.UnloadRotatePosition,
                    BeforeUnloadHoistPosition = portDto.BeforeUnloadHoistPosition,
                    UnloadHoistPosition = portDto.UnloadHoistPosition,
                    BarcodeLeft = portDto.BarcodeLeft,
                    BarcodeRight = portDto.BarcodeRight,
                    PBSSelectNo = portDto.PBSSelectNo,
                    PBSUsed = portDto.PBSUsed,
                    DriveLeftOffset = portDto.DriveLeftOffset,
                    DriveRightOffset = portDto.DriveRightOffset,
                    SlideOffset = portDto.SlideOffset,
                    HoistOffset = portDto.HoistOffset,
                    RotateOffset = portDto.RotateOffset,
                    ProfileExistPosition = (enProfileExistPosition)portDto.ProfileExistPosition,
                };
                int result = m_QueryPort.Insert(dataItem_Port);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Port Data add successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Port Data add failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 删除Port
        /// </summary>
        /// <param name="portID"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode DeletePort(int portID)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryPort = new Query_Port(DatabaseAdapter.Instance.GetJobSession());
                int result = m_QueryPort.Delete(portID);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "Port Data delete successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Port Data delete failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }

        /// <summary>
        /// 导出Port列表
        /// </summary>
        /// <param name="portListDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportPortList(PortListDto portListDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "PortList_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> portData = new List<string>();
                string searchCondition = string.Empty;
                if (portListDto.PortID > -1)
                {
                    searchCondition += $" and PortID = {portListDto.PortID}";
                }
                if (portListDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {portListDto.LinkID}";
                }
                if (portListDto.NodeID > -1)
                {
                    searchCondition += $" and NodeID = {portListDto.NodeID}";
                }
                if (portListDto.PortType > -1)
                {
                    searchCondition += $" and PortType = {portListDto.PortType}";
                }
                if (portListDto.PIOUse > -1)
                {
                    searchCondition += $" and PIOUse = {portListDto.PIOUse}";
                }
                if (portListDto.PBSUse > -1)
                {
                    searchCondition += $" and PBSUse = {portListDto.PBSUse}";
                }
                string querySql = $"select * from port where 1=1 ";
                querySql += searchCondition;

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        dataline += Convert.ToInt32(dr["PortID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LinkID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["NodeID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["PortType"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["BeforeHoistPosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["HoistPosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["SlidePosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["RotatePosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["BeforeUnloadHoistPosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["UnloadHoistPosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["UnloadSlidePosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["UnloadRotatePosition"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["BarcodeLeft"].ToString()) + ",";
                        dataline += Convert.ToDouble(dr["BarcodeRight"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["PIOID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["PIOCH"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["PIOCS"].ToString()) + ",";
                        dataline += bool.Parse(dr["PIOUsed"].ToString()) + ",";
                        dataline += bool.Parse(dr["PortProhibition"].ToString()) + ",";
                        dataline += bool.Parse(dr["OffsetUsed"].ToString()) + ",";
                        dataline += bool.Parse(dr["PBSUsed"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["PBSSelectNo"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["ProfileExistPosition"].ToString()) + ",";
                        portData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, portData);
                    byte[] bytes = Encoding.UTF8.GetBytes(alarmString);
                    MemoryStream ms = new MemoryStream(bytes);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    httpResponseMessage.Content = new StreamContent(ms);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
                    {
                        FileName =
                    browser.Contains("FIREFOX")
                        ? fileName
                        : HttpUtility.UrlEncode(fileName)
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    httpResponseMessage.ReasonPhrase = "Port List Data acquiring failed！";
                    return ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                httpResponseMessage.ReasonPhrase = ex.Message;
            }
            return ResponseMessage(httpResponseMessage);
        }

        /// <summary>
        /// 获取FrontDetectFilter列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetFrontDetectFilter(FrontDetectFilterDto frontDetectFilterDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (frontDetectFilterDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {frontDetectFilterDto.LinkID}";
                }
                if (frontDetectFilterDto.SensorLevel > 0)
                {
                    searchCondition += $" and SensorLevel = {frontDetectFilterDto.SensorLevel}";
                }
                if (frontDetectFilterDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {frontDetectFilterDto.UseFlag}";
                }
                string countSql = "select count(*) from FrontDetectFilter where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / frontDetectFilterDto.PageSize);
                pagetotal = pagetotal * frontDetectFilterDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (frontDetectFilterDto.PageNo < 1)
                { frontDetectFilterDto.PageNo = 1; }
                if (frontDetectFilterDto.PageSize * (frontDetectFilterDto.PageNo - 1) > count)
                {
                    frontDetectFilterDto.PageNo = pagetotal;
                }
                string querySql = $"select * from FrontDetectFilter where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by ID offset {(frontDetectFilterDto.PageNo - 1) * frontDetectFilterDto.PageSize} rows fetch next {frontDetectFilterDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<FrontDetectListData> tableData = new List<FrontDetectListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        FrontDetectListData data = new FrontDetectListData()
                        {
                            ID = Convert.ToInt32(dr["ID"].ToString()),
                            Area = Convert.ToInt32(dr["Area"].ToString()),
                            SensorLevel = Convert.ToInt32(dr["SensorLevel"].ToString()),
                            LinkId = Convert.ToInt32(dr["LinkId"].ToString()),
                            LeftBcrStart = Convert.ToInt32(dr["LBcrStart"].ToString()),
                            LeftBcrEnd= Convert.ToInt32(dr["LBcrEnd"].ToString()),
                            RightBcrStart = Convert.ToInt32(dr["RBcrStart"].ToString()),
                            RightBcrEnd= Convert.ToInt32(dr["RBcrEnd"].ToString()),
                            UseFlag = bool.Parse(dr["UseFlag"].ToString()),
                        };
                        tableData.Add(data);
                    }
                    FrontDetectFilterRtn retdata = new FrontDetectFilterRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.FrontDetectListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "FrontDetectFilter Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "FrontDetectFilter Data acquiring failed！";
                    returnCode.status = 210;
                    returnCode.data = tableData;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 更新FrontDetectFilter
        /// </summary>
        /// <param name="frontDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdateFrontDetectFilter(FrontDto frontDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryFrontDetectFilter = new Query_FrontDetectFilter(DatabaseAdapter.Instance.GetJobSession());
                DataItem_FrontDetectFilter dataItem_FrontDetectFilter = new DataItem_FrontDetectFilter()
                {
                    ID = frontDto.ID,
                    Area = frontDto.Area,
                    SensorLevel = frontDto.SensorLevel,
                    LinkId = frontDto.LinkId,
                    LeftBcrStart = frontDto.LeftBcrStart,
                    LeftBcrEnd = frontDto.LeftBcrEnd,
                    RightBcrStart = frontDto.RightBcrStart,
                    RightBcrEnd = frontDto.RightBcrEnd,
                    UseFlag = frontDto.UseFlag,
                };
                int result = m_QueryFrontDetectFilter.Update(dataItem_FrontDetectFilter);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "FrontDetectFilter Data acquired successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "FrontDetectFilter Data acquired failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 添加FrontDetectFilter
        /// </summary>
        /// <param name="frontDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode AddFrontDetectFilter(FrontDto frontDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryFrontDetectFilter = new Query_FrontDetectFilter(DatabaseAdapter.Instance.GetJobSession());
                DataItem_FrontDetectFilter dataItem_FrontDetectFilter = new DataItem_FrontDetectFilter()
                {
                    ID = frontDto.ID,
                    Area = frontDto.Area,
                    SensorLevel = frontDto.SensorLevel,
                    LinkId = frontDto.LinkId,
                    LeftBcrStart = frontDto.LeftBcrStart,
                    LeftBcrEnd = frontDto.LeftBcrEnd,
                    RightBcrStart = frontDto.RightBcrStart,
                    RightBcrEnd = frontDto.RightBcrEnd,
                    UseFlag = frontDto.UseFlag,
                };
                int result = m_QueryFrontDetectFilter.Insert(dataItem_FrontDetectFilter);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "FrontDetectFilter Data add successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "FrontDetectFilter Data add failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 删除FrontDetectFilter
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode DeleteFrontDetectFilter(int ID)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryFrontDetectFilter = new Query_FrontDetectFilter(DatabaseAdapter.Instance.GetJobSession());
                int result = m_QueryFrontDetectFilter.Delete(ID);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "FrontDetectFilter Data delete successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "FrontDetectFilter Data delete failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }

        /// <summary>
        /// 导出FrontDetectFilter列表
        /// </summary>
        /// <param name="frontDetectFilterDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportFrontDetectFilter(FrontDetectFilterDto frontDetectFilterDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "FrontDetectFilter_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> frontDetectData = new List<string>();
                string searchCondition = string.Empty;
                if (frontDetectFilterDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {frontDetectFilterDto.LinkID}";
                }
                if (frontDetectFilterDto.SensorLevel > 0)
                {
                    searchCondition += $" and SensorLevel = {frontDetectFilterDto.SensorLevel}";
                }
                if (frontDetectFilterDto.UseFlag > -1)
                {
                    searchCondition += $" and UseFlag = {frontDetectFilterDto.UseFlag}";
                }
                string querySql = $"select * from port where 1=1 ";
                querySql += searchCondition;

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        dataline += Convert.ToInt32(dr["ID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["Area"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["SensorLevel"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LinkId"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LBcrStart"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LBcrEnd"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["RBcrStart"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["RBcrEnd"].ToString()) + ",";
                        dataline += bool.Parse(dr["UseFlag"].ToString()) + ",";
                        frontDetectData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, frontDetectData);
                    byte[] bytes = Encoding.UTF8.GetBytes(alarmString);
                    MemoryStream ms = new MemoryStream(bytes);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    httpResponseMessage.Content = new StreamContent(ms);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
                    {
                        FileName =
                    browser.Contains("FIREFOX")
                        ? fileName
                        : HttpUtility.UrlEncode(fileName)
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    httpResponseMessage.ReasonPhrase = "FrontDetectFilter Data acquiring failed！";
                    return ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                httpResponseMessage.ReasonPhrase = ex.Message;
            }
            return ResponseMessage(httpResponseMessage);
        }

        /// <summary>
        /// 获取VelocityLimit列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetVelocityLimit(VelocityLimitDto velocityLimitDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (velocityLimitDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {velocityLimitDto.LinkID}";
                }
                if (velocityLimitDto.ToLinkID > -1)
                {
                    searchCondition += $" and ToLinkID = {velocityLimitDto.ToLinkID}";
                }
                if (velocityLimitDto.MaxVelocity > 0)
                {
                    searchCondition += $" and MaxVelocity = {velocityLimitDto.MaxVelocity}";
                }
                string countSql = "select count(*) from VelocityLimit where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / velocityLimitDto.PageSize);
                pagetotal = pagetotal * velocityLimitDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (velocityLimitDto.PageNo < 1)
                { velocityLimitDto.PageNo = 1; }
                if (velocityLimitDto.PageSize * (velocityLimitDto.PageNo - 1) > count)
                {
                    velocityLimitDto.PageNo = pagetotal;
                }
                string querySql = $"select * from VelocityLimit where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by ID offset {(velocityLimitDto.PageNo - 1) * velocityLimitDto.PageSize} rows fetch next {velocityLimitDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<VelocityLimitData> tableData = new List<VelocityLimitData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        VelocityLimitData data = new VelocityLimitData()
                        {
                            ID = Convert.ToInt32(dr["ID"].ToString()),
                            LinkId = Convert.ToInt32(dr["LinkID"].ToString()),
                            ToLinkId = Convert.ToInt32(dr["ToLinkID"].ToString()),
                            LeftBcrStart = Convert.ToDouble(dr["LeftBcrStart"].ToString()),
                            LeftBcrEnd = Convert.ToDouble(dr["LeftBcrEnd"].ToString()),
                            RightBcrStart = Convert.ToDouble(dr["RightBcrStart"].ToString()),
                            RightBcrEnd = Convert.ToDouble(dr["RightBcrEnd"].ToString()),
                            MaxVelocity = Convert.ToInt32(dr["MaxVelocity"].ToString()),
                        };
                        tableData.Add(data);
                    }
                    VelocityLimitRtn retdata = new VelocityLimitRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.VelocityLimitDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "VelocityLimit Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "VelocityLimit Data acquiring failed！";
                    returnCode.status = 210;
                    returnCode.data = tableData;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 更新VelocityLimit
        /// </summary>
        /// <param name="velocityDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode UpdateVelocityLimit(VelocityDto velocityDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryVelocityLimit = new Query_VelocityLimit(DatabaseAdapter.Instance.GetJobSession());
                DataItem_VelocityLimit dataItem_VelocityLimit = new DataItem_VelocityLimit()
                {
                    ID = velocityDto.ID,
                    LinkId = velocityDto.LinkId,
                    ToLinkId = velocityDto.ToLinkId,
                    MaxVelocity = velocityDto.MaxVelocity,
                    LeftBcrStart = velocityDto.LeftBcrStart,
                    LeftBcrEnd = velocityDto.LeftBcrEnd,
                    RightBcrStart = velocityDto.RightBcrStart,
                    RightBcrEnd = velocityDto.RightBcrEnd,
                };
                int result = m_QueryVelocityLimit.Update(dataItem_VelocityLimit);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "VelocityLimit Data acquired successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "VelocityLimit Data acquired failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }
        /// <summary>
        /// 添加VelocityLimit
        /// </summary>
        /// <param name="velocityDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode AddVelocityLimit(VelocityDto velocityDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryVelocityLimit = new Query_VelocityLimit(DatabaseAdapter.Instance.GetJobSession());
                DataItem_VelocityLimit dataItem_VelocityLimit = new DataItem_VelocityLimit()
                {
                    ID = velocityDto.ID,
                    LinkId = velocityDto.LinkId,
                    ToLinkId = velocityDto.ToLinkId,
                    LeftBcrStart = velocityDto.LeftBcrStart,
                    LeftBcrEnd = velocityDto.LeftBcrEnd,
                    RightBcrStart = velocityDto.RightBcrStart,
                    RightBcrEnd = velocityDto.RightBcrEnd,
                    MaxVelocity = velocityDto.MaxVelocity,
                };
                int result = m_QueryVelocityLimit.Insert(dataItem_VelocityLimit);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "VelocityLimit Data add successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "VelocityLimit Data add failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;

        }
        /// <summary>
        /// 删除VelocityLimit
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode DeleteVelocityLimit(int ID)
        {
            var returnCode = new ReturnCode();
            try
            {
                m_QueryVelocityLimit = new Query_VelocityLimit(DatabaseAdapter.Instance.GetJobSession());
                int result = m_QueryVelocityLimit.Delete(ID);
                if (result > 0)
                {
                    returnCode.succeed = true;
                    returnCode.msg = "VelocityLimit Data delete successful！";
                    returnCode.status = 200;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "VelocityLimit Data delete failed！";
                    returnCode.status = 210;
                }
            }
            catch (Exception ex)
            {
                returnCode.succeed = false;
                returnCode.status = 210;
                returnCode.msg = ex.Message;
            }
            return returnCode;
        }

        /// <summary>
        /// 导出VelocityLimit列表
        /// </summary>
        /// <param name="velocityLimitDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportVelocityLimit(VelocityLimitDto velocityLimitDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "VelocityLimit_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> velocityLimitData = new List<string>();
                string searchCondition = string.Empty;
                if (velocityLimitDto.LinkID > -1)
                {
                    searchCondition += $" and LinkID = {velocityLimitDto.LinkID}";
                }
                if (velocityLimitDto.ToLinkID > -1)
                {
                    searchCondition += $" and ToLinkID = {velocityLimitDto.ToLinkID}";
                }
                if (velocityLimitDto.MaxVelocity > 0)
                {
                    searchCondition += $" and MaxVelocity = {velocityLimitDto.MaxVelocity}";
                }
                string querySql = $"select * from VelocityLimit where 1=1 ";
                querySql += searchCondition;

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        dataline += Convert.ToInt32(dr["ID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LinkId"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["ToLinkId"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["MaxVelocity"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LeftBcrStart"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["LeftBcrEnd"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["RightBcrStart"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["RightBcrEnd"].ToString()) + ",";
                        velocityLimitData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, velocityLimitData);
                    byte[] bytes = Encoding.UTF8.GetBytes(alarmString);
                    MemoryStream ms = new MemoryStream(bytes);
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    httpResponseMessage.Content = new StreamContent(ms);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
                    {
                        FileName =
                    browser.Contains("FIREFOX")
                        ? fileName
                        : HttpUtility.UrlEncode(fileName)
                    };
                    return ResponseMessage(httpResponseMessage);
                }
                else
                {
                    httpResponseMessage.ReasonPhrase = "VelocityLimit Data acquiring failed！";
                    return ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                httpResponseMessage.ReasonPhrase = ex.Message;
            }
            return ResponseMessage(httpResponseMessage);
        }
    }
}
