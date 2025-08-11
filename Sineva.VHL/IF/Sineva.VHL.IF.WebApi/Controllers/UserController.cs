using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.IF.WebApi.Models;
using Sineva.VHL.IF.WebApi.Models.Dtos;
using Sineva.VHL.IF.WebApi.Models.ReturnModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    /// <summary>
    /// 用户信息接口
    /// </summary>
    public class UserController : ApiController
    {
        private Query_UserList m_QueryUser = null;
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode Login([FromBody] UserDto userDto)
        {
            var retcode = new ReturnCode();
            try
            {
                m_QueryUser = new Query_UserList(DatabaseAdapter.Instance.GetJobSession());
                List<DataItem_UserInfo> users = m_QueryUser.SelectAllOrNull();
                if (users.Any(x => !string.IsNullOrEmpty(x.ClientIp)))
                {
                    retcode.succeed = false;
                    retcode.msg = $"已有其他用户:{users.FirstOrDefault(x => !string.IsNullOrEmpty(x.ClientIp))?.ClientIp}登录，超出最大登录个数！";
                    retcode.status = 210;
                    return retcode;
                }
                var user = users.FirstOrDefault(x => x.UserName.ToLower() == userDto.UserName.ToLower() && x.Password == userDto.Password);
                if (user != null)
                {
                    var userInfo = new User();
                    userInfo.Token = Guid.NewGuid().ToString();
                    userInfo.UserName = user.UserName;
                    //userInfo.Password = user.Password;
                    userInfo.Level = Convert.ToInt32(user.Level);
                    userInfo.UserName = user.UserName;
                    userInfo.Department = user.Department;
                    userInfo.Description = user.Description;
                    m_QueryUser.UpdateClientIp(userDto.UserName, userDto.ClientIp);
                    retcode.succeed = true;
                    retcode.msg = "登录成功！";
                    retcode.status = 200;
                    retcode.data = userInfo;
                }
                else
                {
                    retcode.succeed = false;
                    retcode.msg = "登录失败！";
                    retcode.status = 210;
                }
            }
            catch (Exception ex)
            {
                retcode.succeed = false;
                retcode.status = 210;
                retcode.msg = ex.Message;
            }
            return retcode;
        }
        /// <summary>
        /// 登录状态确认
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode LoginCheck([FromBody] UserDto userDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "失败！", status = 210 };
            try
            {
                m_QueryUser = new Query_UserList(DatabaseAdapter.Instance.GetJobSession());
                m_QueryUser.UpdateClientIp(userDto.UserName, userDto.ClientIp);
                retcode.succeed = true;
                retcode.msg = "成功！";
                retcode.status = 200;
            }
            catch (Exception ex)
            {
                retcode.succeed = false;
                retcode.msg = ex.Message;
                retcode.status = 210;
            }
            return retcode;
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode Logout([FromBody] UserDto userDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "失败！", status = 210 };
            try
            {
                m_QueryUser = new Query_UserList(DatabaseAdapter.Instance.GetJobSession());
                m_QueryUser.UpdateClientIp(userDto.UserName, "");
                retcode.succeed = true;
                retcode.msg = "退出登录成功！";
                retcode.status = 200;
            }
            catch (Exception ex)
            {
                retcode.succeed = false;
                retcode.msg = ex.Message;
                retcode.status = 210;
            }
            return retcode;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode GetUserList(UserDto userDto)
        {
            var returnCode = new ReturnCode();
            try
            {
                string searchCondition = string.Empty;
                if (!string.IsNullOrEmpty(userDto.UserName))
                {
                    searchCondition += $" and UserName = '{userDto.UserName}'";
                }
                if (userDto.Level > -1)
                {
                    searchCondition += $" and AuthorizationLevel = {userDto.Level}";
                }

                string countSql = "select count(*) from UserList where 1=1 ";
                object result = DatabaseAdapter.Instance.GetJobSession().ExecuteScalar(countSql + searchCondition);

                if (result == null)
                {
                    throw new Exception("No data");
                }

                int count = Convert.ToInt32(result);
                int pagetotal = (int)(count / userDto.PageSize);
                pagetotal = pagetotal * userDto.PageSize < count ? pagetotal + 1 : pagetotal;
                if (userDto.PageNo < 1)
                { userDto.PageNo = 1; }
                if (userDto.PageSize * (userDto.PageNo - 1) > count)
                {
                    userDto.PageNo = pagetotal;
                }
                string querySql = $"select * from UserList where 1=1 ";
                querySql += searchCondition;
                querySql += $" order by UserName offset {(userDto.PageNo - 1) * userDto.PageSize} rows fetch next {userDto.PageSize} rows only";
                DataTable table = new DataTable();
                List<User> tableData = new List<User>();

                int returnValue = DatabaseAdapter.Instance.GetJobSession().ExecuteReader(querySql.ToString(), ref table);
                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        User data = new User()
                        {
                            UserName = dr["UserName"].ToString(),
                            //Password = dr["Password"].ToString(),
                            Level = Convert.ToInt32(dr["AuthorizationLevel"].ToString()),
                            Department = dr["Department"].ToString(),
                            Description = dr["Description"].ToString(),
                        };
                        tableData.Add(data);
                    }
                    UserRtn retdata = new UserRtn();
                    retdata.ItemTotal = count;
                    retdata.PageTotal = pagetotal;
                    retdata.UserListDatas = tableData;
                    returnCode.succeed = true;
                    returnCode.msg = "User Data acquired successful！";
                    returnCode.status = 200;
                    returnCode.data = retdata;
                }
                else
                {
                    returnCode.succeed = false;
                    returnCode.msg = "User Data acquiring failed！";
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
        /// 添加用户
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode AddUser([FromBody] UserDto loginDto)
        {
            var userInfo = new User();
            userInfo.Token = Guid.NewGuid().ToString();
            var retcode = new ReturnCode() { succeed = true, msg = "退出登录成功！", status = 200, data = userInfo };
            return retcode;
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode UpdateUser([FromBody] UserDto loginDto)
        {
            var userInfo = new User();
            userInfo.Token = Guid.NewGuid().ToString();
            var retcode = new ReturnCode() { succeed = true, msg = "退出登录成功！", status = 200, data = userInfo };
            return retcode;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode DeleteUser([FromBody] UserDto loginDto)
        {
            var userInfo = new User();
            userInfo.Token = Guid.NewGuid().ToString();
            var retcode = new ReturnCode() { succeed = true, msg = "退出登录成功！", status = 200, data = userInfo };
            return retcode;
        }
    }
}