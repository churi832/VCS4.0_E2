using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Sineva.VHL.Library.DBProvider
{
    public class JobSession
    {
        #region Fields
        private class JobObject
        {
            public string QueryString;
            public SqlCommand Command;
            public DBJobMode Mode;
            public NonQueryCallback CallBack_NonQuery;
            public ReaderCallback CallBack_Reader;
            public ScalarCallback CallBack_Scalar;

            public DataTable Table;
            public object ResultObject;
            public ManualResetEvent EventEnd;
        }

        private ManualResetEvent _EventWorker = null;

        //private DBSession _Session = null;
        private DBSession _Session;

        private Queue<JobObject> _JobQueue = null;

        private Thread _ThreadWorker = null;
        private bool _RunFlag = false;

        private static object _SyncRoot = new object();
        private static JobSession _Instance = null;
        private static JobSession _Instance_FDC = null;

        private static string _ConnectionString = string.Empty;
        private static string _ConnectionString_FDC = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Events
        public delegate void JobSessionEvent(object sender, object eventData);
        public static JobSessionEvent ExceptionHappened;
        #endregion

        #region Constructors
        public JobSession()
        {
            try
            {
                _RunFlag = true;
                _EventWorker = new ManualResetEvent(false);
                _JobQueue = new Queue<JobObject>();

                _ThreadWorker = new Thread(WorkerThread);
                _ThreadWorker.IsBackground = true;
                _ThreadWorker.Start();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        ~JobSession()
        {
            _RunFlag = false;
            if (_ThreadWorker != null)
            {
                if (_ThreadWorker.IsAlive == true)
                {
                    if (_ThreadWorker.Join(1000) == false)
                    {
                        _ThreadWorker.Abort();
                    }
                    _ThreadWorker = null;
                }
            }

            if (_JobQueue != null)
            {
                _JobQueue.Clear();
                _JobQueue = null;
            }

            if (_Session != null)
            {
                _Session.Close();
                _Session = null;
            }
        }
        #endregion

        #region Methods
        public static JobSession GetInstanceOrNull()
        {
            try
            {
                if (string.IsNullOrEmpty(_ConnectionString) == true) return null;

                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new JobSession();
                            _Instance._Session = new DBSession();
                            if (_Instance._Session.Open(_ConnectionString) == false)
                            {
                                _Instance._Session = null;
                                return null;
                            }
                        }
                    }
                }

                return _Instance;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public static JobSession GetInstanceOrNull(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString) == true) return null;

                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _ConnectionString = connectionString;
                            _Instance = new JobSession();
                            _Instance._Session = new DBSession();
                            if (_Instance._Session.Open(connectionString) == false)
                            {
                                _Instance._Session = null;
                                return null;
                            }
                        }
                    }
                }

                return _Instance;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public static JobSession NewInstanceOrNull(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString) == true) return null;

                if (_Instance == null && _ConnectionString == string.Empty)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _ConnectionString = connectionString;
                            _Instance = new JobSession();
                            _Instance._Session = new DBSession();
                            if (_Instance._Session.Open(connectionString) == false)
                            {
                                _Instance._Session = null;
                                return null;
                            }
                        }
                    }
                }
                else if (_Instance_FDC == null && _ConnectionString_FDC == string.Empty)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance_FDC == null)
                        {
                            _ConnectionString_FDC = connectionString;
                            _Instance_FDC = new JobSession();
                            _Instance_FDC._Session = new DBSession();
                            if (_Instance_FDC._Session.Open(connectionString) == false)
                            {
                                _Instance_FDC._Session = null;
                                return null;
                            }
                        }
                    }
                }

                if(string.Equals(connectionString, _ConnectionString,StringComparison.OrdinalIgnoreCase)) 
                    return _Instance;
                else if (string.Equals(connectionString, _ConnectionString_FDC, StringComparison.OrdinalIgnoreCase)) 
                    return _Instance_FDC;
                else 
                    return null;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return null;
            }
        }
        public int ExecuteNonQuery(string queryString, NonQueryCallback callBack)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.NONQUERY;
                job.CallBack_NonQuery = callBack;

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }

                _EventWorker.Set();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return 1;
        }
        public int ExecuteNonQuery(string queryString)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.NONQUERY;
                job.CallBack_NonQuery = null;
                job.EventEnd = new ManualResetEvent(false);

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }

                _EventWorker.Set();

                // Thread Delay 원인 된다.
                if (job.EventEnd.WaitOne(45000) == false)
                {
                    return -1;
                }

                return (int)job.ResultObject;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public void ExecuteReader(string queryString, ReaderCallback callBack)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.READER;
                job.CallBack_Reader = callBack;

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }
                _EventWorker.Set();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return;
            }
        }
        public int ExecuteReader(string queryString, ref DataTable table)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.READER;
                job.CallBack_Reader = null;
                job.Table = table;
                job.EventEnd = new ManualResetEvent(false);

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }
                _EventWorker.Set();

                if (job.EventEnd.WaitOne(45000) == false)
                {
                    return -1;
                }

                return (int)job.ResultObject;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                table = null;
                return -1;
            }
        }
        public void ExecuteScalar(string queryString, ScalarCallback callBack)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.SCALAR;
                job.CallBack_Scalar = callBack;

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }

                _EventWorker.Set();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        public int ExecuteScalar(string queryString)
        {
            try
            {
                JobObject job = new JobObject();
                job.QueryString = queryString;
                job.Mode = DBJobMode.SCALAR;
                job.CallBack_Scalar = null;
                job.EventEnd = new ManualResetEvent(false);

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }
                _EventWorker.Set();

                if (job.EventEnd.WaitOne(45000) == false)
                {
                    return -1;
                }

                return (int)job.ResultObject;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        public int ExecuteProcedure(SqlCommand command, ref DataTable table)
        {
            try
            {
                JobObject job = new JobObject();
                job.Command = command;
                job.Mode = DBJobMode.PROCEDURE;
                job.Table = table;
                job.EventEnd = new ManualResetEvent(false);

                lock (_JobQueue)
                {
                    _JobQueue.Enqueue(job);
                }
                _EventWorker.Set();

                if (job.EventEnd.WaitOne(45000) == false)
                {
                    return -1;
                }

                return (int)job.ResultObject;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                return -1;
            }
        }
        private void WorkerThread()
        {
            JobObject job = new JobObject();
            DataTable dtTmp = new DataTable();

            while (_RunFlag)
            {
                try
                {
                    if (_EventWorker.WaitOne(10) == false) continue;

                    if (_JobQueue.Count < 1)
                    {
                        _EventWorker.Reset();
                        continue;
                    }

                    lock (_JobQueue)
                    {
                        job = _JobQueue.Dequeue();
                    }

                    if (job.Mode == DBJobMode.NONQUERY)
                    {
                        int result = _Session.ExecuteNonQuery(job.QueryString);
                        if (job.CallBack_NonQuery != null) job.CallBack_NonQuery(result);
                        else if (job.EventEnd != null)
                        {
                            job.ResultObject = result;
                            job.EventEnd.Set();
                        }
                    }
                    else if (job.Mode == DBJobMode.READER)
                    {
                        if (job.CallBack_Reader != null)
                        {
                            DataTable dt = new DataTable();
                            int result = _Session.ExecuteReader(job.QueryString, ref dt);
                            job.CallBack_Reader(result, dt);
                        }
                        else if (job.EventEnd != null)
                        {
                            job.ResultObject = _Session.ExecuteReader(job.QueryString, ref job.Table);
                            job.EventEnd.Set();
                        }
                    }
                    else if (job.Mode == DBJobMode.SCALAR)
                    {
                        object result = _Session.ExecuteScalar(job.QueryString);
                        if (job.CallBack_Scalar != null) job.CallBack_Scalar(result);
                        else if (job.EventEnd != null)
                        {
                            job.ResultObject = result;
                            job.EventEnd.Set();
                        }
                    }
                    else if (job.Mode == DBJobMode.PROCEDURE)
                    {
                        int result = _Session.ExecuteProcedure(job.Command, ref job.Table);
                        if (job.EventEnd != null)
                        {
                            job.ResultObject = result;
                            job.EventEnd.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
                finally
                {
                    Thread.Sleep(10);
                }
            }
        }

        //2022.10.10 Program End ..by HS
        public void Exit()
        {
            _RunFlag = false;

            if (_ThreadWorker.IsAlive == true)
            {
                if (_ThreadWorker.Join(1000) == false)
                {
                    _ThreadWorker.Abort();
                }
                _ThreadWorker = null;
            }

            _JobQueue.Clear();
            _JobQueue = null;

            _Session.Close();
            _Session = null;
        }
        #endregion
    }
}
