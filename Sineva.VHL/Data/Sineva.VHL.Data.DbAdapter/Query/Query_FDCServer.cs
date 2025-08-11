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
    public class Query_FDCServer
    {
        #region Fields
        private JobSession m_JobSession = null;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void QueryFDCServerEvent(object sender, object eventData);

        public event QueryFDCServerEvent ExceptionHappened;
        public event NonQueryCallback CallBack_NonQuery;
        #endregion

        #region Constructors
        public Query_FDCServer(JobSession instance)
        {
            m_JobSession = instance;
        }
        #endregion

        #region Methods - Update Query
        public int Update(DataItem_FDCItem item)
        {
            try
            {
                int rv = -1;

                if (item == null)
                {
                    rv = 0;
                }
                else
                {
                    StringBuilder queryString = new StringBuilder();

                    queryString.AppendFormat("IF EXISTS (SELECT 1 FROM FDCItem WHERE EQPID = '{0}')", item.EQPID);
                    queryString.AppendFormat(" BEGIN UPDATE FDCItem SET Time = '{0}', ValueString = '{1}' WHERE EQPID = '{2}'; END ", item.Time, item.ValueString, item.EQPID);
                    queryString.AppendFormat("ELSE BEGIN INSERT INTO FDCItem (EQPID, Time, ValueString) VALUES ('{0}','{1}','{2}'); END", item.EQPID, item.Time, item.ValueString);

                    rv = m_JobSession.ExecuteNonQuery(queryString.ToString());
                }

                return rv;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
                return -1;
            }
        }
        #endregion
    }
}
