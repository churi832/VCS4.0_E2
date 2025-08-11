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
    public class Query_CommentTag
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryCommentTagEvent(object sender, object eventData);

        public event QueryCommentTagEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_CommentTag(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Select Count
        public int CountAll(ref long count)
        {
            try
            {
                string queryString = "SELECT COUNT(*) FROM CommentTag";

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
        public List<DataItem_CommentTag> SelectAllOrNull()
        {
            try
            {
                string queryString = "SELECT * FROM CommentTag ORDER BY ID ASC";

                DataTable table = new DataTable();
                List<DataItem_CommentTag> tableData = new List<DataItem_CommentTag>();

                int returnValue = m_JobSession.ExecuteReader(queryString, ref table);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string id = dr["ID"].ToString().Trim();
                        string chinese = dr["Chinese"].ToString().Trim();
                        string english = dr["English"].ToString().Trim();
                        string korean = dr["Korean"].ToString().Trim();

                        DataItem_CommentTag data = new DataItem_CommentTag(id, chinese, english, korean);

                        tableData.Add(data);
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
        public DataItem_CommentTag SelectSingleOrNull(string tagID)
        {
            try
            {
                DataTable table = new DataTable();
                DataItem_CommentTag tableData = null;

                StringBuilder queryString = new StringBuilder("SELECT * FROM CommentTag WHERE ");
                queryString.AppendFormat("ID = '{0}'", tagID);

                int returnValue = m_JobSession.ExecuteReader(queryString.ToString(), ref table);

                if (returnValue >= 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        string id = dr["ID"].ToString().Trim();
                        string chinese = dr["Chinese"].ToString().Trim();
                        string english = dr["English"].ToString().Trim();
                        string korean = dr["Korean"].ToString().Trim();

                        tableData = new DataItem_CommentTag(id, chinese, english, korean);
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
        #endregion

        #region Methods - Insert Query
        public int Insert(string id, string chinese, string english, string korean)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO CommentTag ");
                queryString.Append("(ID, Chinese, English, Korean) VALUES (");
                queryString.AppendFormat("'{0}', ", id);
                queryString.AppendFormat("'{0}', ", chinese);
                queryString.AppendFormat("'{0}', ", english);
                queryString.AppendFormat("'{0}') ", korean);
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
        public int Insert(DataItem_CommentTag item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO CommentTag ");
                queryString.Append("(ID, Chinese, English, Korean) VALUES (");
                queryString.AppendFormat("'{0}', ", item.TagID);
                queryString.AppendFormat("'{0}', ", item.Comment[Language.Chinese]);
                queryString.AppendFormat("'{0}', ", item.Comment[Language.English]);
                queryString.AppendFormat("'{0}') ", item.Comment[Language.Korean]);
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
        public int Insert(List<DataItem_CommentTag> itemList)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("INSERT INTO CommentTag ");

                queryString.Append("(ID, Chinese, English, Korean) VALUES ");

                for (int index = 0; index < itemList.Count; index++)
                {
                    queryString.Append("(");
                    queryString.AppendFormat("'{0}', ", itemList[index].TagID);
                    queryString.AppendFormat("'{0}', ", itemList[index].Comment[Language.Chinese]);
                    queryString.AppendFormat("'{0}', ", itemList[index].Comment[Language.English]);
                    queryString.AppendFormat("'{0}')", itemList[index].Comment[Language.Korean]);

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
        public int Update(DataItem_CommentTag item)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("UPDATE CommentTag SET ");

                queryString.AppendFormat("[Chinese] = '{0}' ,", item.Comment[Language.Chinese]);
                queryString.AppendFormat("[English] = '{0}' ,", item.Comment[Language.English]);
                queryString.AppendFormat("[Korean] = '{0}' ", item.Comment[Language.Korean]);
                queryString.AppendFormat("WHERE [ID] = '{0}'", item.TagID);
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
        public int Delete(string id)
        {
            int rv = -1;
            try
            {
                StringBuilder queryString = new StringBuilder("DELETE FROM CommentTag ");
                queryString.AppendFormat("WHERE [ID] = '{0}'", id);
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
