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
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.IF.WebApi.Models.Dtos;
using Sineva.VHL.IF.WebApi.Models.ReturnModels;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    /// <summary>
    /// 报警信息接口
    /// </summary>
    public class AlarmController : ApiController
    {
        private Query_ErrorHistory m_QueryHistory = null;
        private Query_ErrorList m_QueryList = null;
        /// <summary>
        /// 获取alarm历史
        /// </summary>
        /// <param name="alarmHistoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetAlarmHistory([FromBody] AlarmHistoryDto alarmHistoryDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (alarmHistoryDto.OccurredTimeStart != null)
                {
                    searchCondition += "and A.OccurredTime>'" + alarmHistoryDto.OccurredTimeStart?.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                if (alarmHistoryDto.OccurredTimeEnd != null)
                {
                    searchCondition += "and A.OccurredTime<'" + alarmHistoryDto.OccurredTimeEnd?.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                if (alarmHistoryDto.ErrorID > -1)
                {
                    searchCondition += $" and A.ID = {alarmHistoryDto.ErrorID}";
                }
                if (alarmHistoryDto.ErrorLevel > -1)
                {
                    searchCondition += $" and B.ErrorLevel = {alarmHistoryDto.ErrorLevel}";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Unit))
                {
                    searchCondition += $" and B.Unit like '%{alarmHistoryDto.Unit}%'";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Description))
                {
                    searchCondition += $" and B.Description like '%{alarmHistoryDto.Description}%'";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Comment))
                {
                    searchCondition += $" and B.Comment like '%{alarmHistoryDto.Comment}%'";
                }
                string countSql = "select count(*) from ErrorHistory AS A INNER JOIN ErrorList AS B ON A.ID = B.ID where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / alarmHistoryDto.PageSize);
                pagetotal = pagetotal * alarmHistoryDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (alarmHistoryDto.PageNo < 1)
                { alarmHistoryDto.PageNo = 1; }
                if (alarmHistoryDto.PageSize * (alarmHistoryDto.PageNo - 1) > count)
                {
                    alarmHistoryDto.PageNo = pagetotal;
                }
                string querySql = $"select * from ErrorHistory AS A INNER JOIN ErrorList AS B ON A.ID = B.ID where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by A.OccurredTime desc offset {(alarmHistoryDto.PageNo - 1) * alarmHistoryDto.PageSize} rows fetch next {alarmHistoryDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<AlarmHistoryData> tableData = new List<AlarmHistoryData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {

                        int id = Convert.ToInt32(dr["ID"].ToString());
                        string locationID = dr["LocationID"].ToString();
                        int code = Convert.ToInt32(dr["Code"].ToString());
                        int level = Convert.ToInt32(dr["ErrorLevel"].ToString());
                        string unit = dr["Unit"].ToString();
                        string description = dr["Description"].ToString();
                        string comment = dr["Comment"].ToString();


                        AlarmHistoryData data = new AlarmHistoryData()
                        {
                            ID = id,
                            LocationID = locationID,
                            Code = code,
                            Level = level,
                            Unit = unit,
                            Description = description,
                            Comment = comment,
                        };
                        DateTime dateTime = DateTime.MinValue;
                        if ((dr["OccurredTime"] != null) && DateTime.TryParse(dr["OccurredTime"].ToString(), out dateTime))
                        {
                            data.OccurredTime = dateTime;
                        }
                        if ((dr["ClearedTime"] != null) && DateTime.TryParse(dr["ClearedTime"].ToString(), out dateTime))
                        {
                            data.ClearedTime = dateTime;
                        }
                        tableData.Add(data);
                    }
                    AlarmHistoryRtn retdata = new AlarmHistoryRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.AlarmHistoryDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "Alarm History Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Alarm History Data acquiring failed！";
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
        /// 导出alarm历史
        /// </summary>
        /// <param name="alarmHistoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportAlarmHistory([FromBody] AlarmHistoryDto alarmHistoryDto)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                string fileName = "AlarmHistory" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
                List<string> alarmData = new List<string>();
                string searchCondition = string.Empty;
                if (alarmHistoryDto.OccurredTimeStart != null)
                {
                    searchCondition += "and A.OccurredTime>'" + alarmHistoryDto.OccurredTimeStart?.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                if (alarmHistoryDto.OccurredTimeEnd != null)
                {
                    searchCondition += "and A.OccurredTime<'" + alarmHistoryDto.OccurredTimeEnd?.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                }
                if (alarmHistoryDto.ErrorID > -1)
                {
                    searchCondition += $" and A.ID = {alarmHistoryDto.ErrorID}";
                }
                if (alarmHistoryDto.ErrorLevel > -1)
                {
                    searchCondition += $" and B.ErrorLevel = {alarmHistoryDto.ErrorLevel}";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Unit))
                {
                    searchCondition += $" and B.Unit like '%{alarmHistoryDto.Unit}%'";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Description))
                {
                    searchCondition += $" and B.Description like '%{alarmHistoryDto.Description}%'";
                }
                if (!string.IsNullOrEmpty(alarmHistoryDto.Comment))
                {
                    searchCondition += $" and B.Comment like '%{alarmHistoryDto.Comment}%'";
                }
                string querySql = $"select * from ErrorHistory AS A INNER JOIN ErrorList AS B ON A.ID = B.ID where 1=1 ";
                querySql += searchCondition;

                alarmData.Add("OccurredTime,ClearedTime,AlarmID,AlarmCode,ErrorLevel,Unit,Description,Comment");

                DataTable table = new DataTable();
                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string dataline = "";
                        DateTime dateTime = DateTime.MinValue;
                        if ((dr["OccurredTime"] != null) && DateTime.TryParse(dr["OccurredTime"].ToString(), out dateTime))
                        {
                            dataline += dateTime.ToString("yyyy-MM-dd HH:mm:ss") + ",";
                        }
                        else
                        {
                            dataline += ",";
                        }
                        if ((dr["ClearedTime"] != null) && DateTime.TryParse(dr["ClearedTime"].ToString(), out dateTime))
                        {
                            dataline += dateTime.ToString("yyyy-MM-dd HH:mm:ss") + ",";
                        }
                        else
                        {
                            dataline += ",";
                        }
                        dataline += Convert.ToInt32(dr["ID"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["Code"].ToString()) + ",";
                        dataline += Convert.ToInt32(dr["ErrorLevel"].ToString()) + ",";
                        dataline += dr["Unit"].ToString() + ",";
                        dataline += dr["Description"].ToString() + ",";
                        dataline += dr["Comment"].ToString();
                        alarmData.Add(dataline);
                    }
                    string alarmString = string.Join(Environment.NewLine, alarmData);
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
                    httpResponseMessage.ReasonPhrase = "Alarm History Data acquiring failed！";
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
        /// 获取AlarmList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetAlarmList([FromBody] AlarmListDto alarmListDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (alarmListDto.ErrorID > -1)
                {
                    searchCondition += $" and ID = {alarmListDto.ErrorID}";
                }
                if (alarmListDto.ErrorLevel > -1)
                {
                    searchCondition += $" and ErrorLevel = {alarmListDto.ErrorLevel}";
                }
                if (!string.IsNullOrEmpty(alarmListDto.Unit))
                {
                    searchCondition += $" and Unit like '%{alarmListDto.Unit}%'";
                }
                if (!string.IsNullOrEmpty(alarmListDto.Description))
                {
                    searchCondition += $" and Description like '%{alarmListDto.Description}%'";
                }
                if (!string.IsNullOrEmpty(alarmListDto.Comment))
                {
                    searchCondition += $" and Comment like '%{alarmListDto.Comment}%'";
                }
                string countSql = "select count(*) from ErrorList where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / alarmListDto.PageSize);
                pagetotal = pagetotal * alarmListDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (alarmListDto.PageNo < 1)
                { alarmListDto.PageNo = 1; }
                if (alarmListDto.PageSize * (alarmListDto.PageNo - 1) > count)
                {
                    alarmListDto.PageNo = pagetotal;
                }
                string querySql = $"select * from ErrorList where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by ID offset {(alarmListDto.PageNo - 1) * alarmListDto.PageSize} rows fetch next {alarmListDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<AlarmListData> tableData = new List<AlarmListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        int id = Convert.ToInt32(dr["ID"].ToString());
                        int code = Convert.ToInt32(dr["Code"].ToString());
                        int level = Convert.ToInt32(dr["ErrorLevel"].ToString());
                        string unit = dr["Unit"].ToString();
                        string description = dr["Description"].ToString();
                        string comment = dr["Comment"].ToString();

                        AlarmListData data = new AlarmListData()
                        {
                            ID = id,
                            Code = code,
                            Level = level,
                            Unit = unit,
                            Description = description,
                            Comment = comment,
                        };
                        tableData.Add(data);
                    }
                    AlarmListRtn retdata = new AlarmListRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.AlarmListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "Alarm List Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Alarm List Data acquiring failed！";
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
        /// 获取当前Alarm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ReturnCode GetCurrentAlarm()
        {
            var returnCode = new ReturnCode { succeed = true, msg = "成功", status = 200 };
            return returnCode;
        }
        /// <summary>
        /// 获取所有AlarmList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReturnCode GetAllAlarmList()
        {
            var returnCode = new ReturnCode();
            try
            {
                string querySql = $"select * from ErrorList order by id";
                DataTable table = new DataTable();
                List<AlarmListData> tableData = new List<AlarmListData>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        int id = Convert.ToInt32(dr["ID"].ToString());
                        int code = Convert.ToInt32(dr["Code"].ToString());
                        int level = Convert.ToInt32(dr["ErrorLevel"].ToString());
                        string unit = dr["Unit"].ToString();
                        string description = dr["Description"].ToString();
                        string comment = dr["Comment"].ToString();

                        AlarmListData data = new AlarmListData()
                        {
                            ID = id,
                            Code = code,
                            Level = level,
                            Unit = unit,
                            Description = description,
                            Comment = comment,
                        };
                        tableData.Add(data);
                    }
                    returnCode.succeed = true;
                    returnCode.msg = "Alarm List Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = tableData;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "Alarm List Data acquiring failed！";
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
    }
}
