using System;
using System.Threading;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public class ThreadFunction
    {
        #region Fields
        private bool _IsStpped;
        private Thread _Thread;
        public delegate void ThreadFunctionDelegate();
        #endregion
        #region Constructors
        public ThreadFunction()
        {
            this._IsStpped = false;
            this._Thread = null;
        }
        #endregion
        #region Destructor
        ~ThreadFunction()
        {
            this._IsStpped = true;
            if (this._Thread != null)
            {
                if (!this._Thread.Join(100))
                {
                    this._Thread.Abort();
                }
                this._Thread = null;
            }
        }
        #endregion
        #region Properties
        public bool IsStopped
        {
            get
            {
                return this._IsStpped;
            }
        }
        public bool IsRunning
        {
            get
            {
                return (this._Thread != null && this._Thread.IsAlive);
            }
        }
        #endregion
        #region Methods
        public bool Start(ThreadFunctionDelegate function)
        {
            try
            {
                this._IsStpped = false;

                if (this.IsRunning) return true;
                if (this._Thread != null)
                {
                    if (!this._Thread.Join(1000))
                    {
                        this._Thread.Abort();
                    }
                    this._Thread = null;
                }

                this._Thread = new Thread(new ThreadStart(function));
                this._Thread.IsBackground = true;
                this._Thread.Start();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Stop()
        {
            try
            {
                if (!this.IsRunning) return true;
                this._IsStpped = true;
                if (this._Thread != null)
                {
                    if (!this._Thread.Join(500))
                    {
                        this._Thread.Abort();
                    }
                    this._Thread = null;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
