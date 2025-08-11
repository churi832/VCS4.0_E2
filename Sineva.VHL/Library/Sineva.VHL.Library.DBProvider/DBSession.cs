using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace Sineva.VHL.Library.DBProvider
{
    internal class DBSession
    {
        #region Enums
        private enum SQLServiceFlag
        {
            None,
            Stop,
            Start,
        }
        #endregion

        #region Fields
        private bool _TransactionFlag = false;
        private SqlConnection _Connection = null;
        private SqlTransaction _Transaction = null;

        private const int MAX_TIMEOUT = 2147438;
        private string _ConnectionString = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void DBSessionEvent(object sender, object eventData);

        public DBSessionEvent ExceptionHappened = null;
        #endregion

        #region Constructors
        public DBSession()
        {
            try
            {
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public bool Open()
        {
            try
            {
                //CheckSQLService();

                _Connection = new SqlConnection(_ConnectionString);
                _Connection.Open();

                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Close();
                    _Connection = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }

                if (_Connection != null)
                {
                    _Connection.Close();
                }
                _Connection = null;
                return false;
            }
        }
        public bool Open(string connectionString)
        {
            try
            {
                _ConnectionString = connectionString;

                return Open();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }

                if (_Connection != null)
                {
                    _Connection.Close();
                }
                _Connection = null;
                return false;
            }
        }
        public void Close()
        {
            try
            {
                if (_Connection == null)
                {
                    return;
                }

                _Connection.Close();
                _Connection = null;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
        }
        /// <summary>
        /// Need to modify... Kim Youngsik.
        /// </summary>
        private void CheckSQLService()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "MSSQLSERVER")
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
        }
        private void CheckConnectionState()
        {
            if (_Connection == null || _Connection.State != ConnectionState.Open)
            {
                Open();
            }
        }
        public bool GetSchema(ref DataTable table)
        {
            try
            {
                CheckConnectionState();

                table = _Connection.GetSchema();

                return true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
                return false;
            }
        }
        public void BeginTransaction()
        {
            try
            {
                CheckConnectionState();

                if (_TransactionFlag == true)
                {
                    return;
                }

                _TransactionFlag = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
        }
        public void Commit()
        {
            try
            {
                _TransactionFlag = false;

                CheckConnectionState();

                if (_Transaction == null)
                {
                    return;
                }

                _Transaction.Commit();
                _Transaction.Dispose();
                _Transaction = null;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
        }
        public void RollBack()
        {
            try
            {
                _TransactionFlag = false;

                CheckConnectionState();

                if (_Transaction == null)
                {
                    return;
                }

                _Transaction.Rollback();
                _Transaction.Dispose();
                _Transaction = null;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
        }
        public int ExecuteNonQuery(string queryString, int timeout = MAX_TIMEOUT)
        {
            try
            {
                CheckConnectionState();

                SqlCommand command = null;
                int result = -1;

                if (_TransactionFlag == true)
                {
                    if (_Transaction == null)
                    {
                        _Transaction = _Connection.BeginTransaction();
                    }

                    command = new SqlCommand(queryString, _Connection, _Transaction);
                }
                else
                {
                    command = new SqlCommand(queryString, _Connection);
                }

                command.CommandTimeout = timeout;
                result = command.ExecuteNonQuery();
                command.Dispose();
                command = null;

                return result;
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
        public int ExecuteReader(string queryString, ref DataTable table, int timeout = MAX_TIMEOUT)
        {
            try
            {
                CheckConnectionState();

                SqlCommand command = null;
                SqlDataAdapter adapter = null;

                if (_TransactionFlag == true)
                {
                    if (_Transaction == null)
                    {
                        _Transaction = _Connection.BeginTransaction();
                    }

                    command = new SqlCommand(queryString, _Connection, _Transaction);
                }
                else
                {
                    command = new SqlCommand(queryString, _Connection);
                }

                command.CommandTimeout = timeout;
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(table);

                adapter.Dispose();
                command.Dispose();
                adapter = null;
                command = null;

                return table.Rows.Count;
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
        public int ExecuteProcedure(SqlCommand command, ref DataTable table, int timeout = MAX_TIMEOUT)
        {
            try
            {
                CheckConnectionState();

                command.Connection = _Connection;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(table);

                adapter.Dispose();
                command.Dispose();
                adapter = null;
                command = null;

                return table.Rows.Count;
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
        public object ExecuteScalar(string queryString, int timeout = MAX_TIMEOUT)
        {
            try
            {
                CheckConnectionState();

                SqlCommand command = null;
                object result = null;

                if (_TransactionFlag == true)
                {
                    if (_Transaction == null)
                    {
                        _Transaction = _Connection.BeginTransaction();
                    }

                    command = new SqlCommand(queryString, _Connection, _Transaction);
                }
                else
                {
                    command = new SqlCommand(queryString, _Connection);
                }

                command.CommandTimeout = timeout;
                result = command.ExecuteScalar();
                command.Dispose();
                command = null;

                return result;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                if (ExceptionHappened != null)
                {
                    ExceptionHappened(this, string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
                return null;
            }
        }
        #endregion
    }
}
