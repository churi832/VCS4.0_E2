using System;
using System.Diagnostics;

namespace Sineva.OHT.Common
{
    public class TimeCheck
    {
        #region Fields
        private Stopwatch _Timer;
        private double _ProgressedTime;
        #endregion
        #region Constructors
        public TimeCheck()
        {
            this._Timer = new Stopwatch();
        }
        #endregion
        #region Destructor
        #endregion
        #region Properties
        public bool Started
        {
            get
            {
                return this._Timer.IsRunning;
            }
            set
            {
                if (value == false) this._Timer.Stop();
                else StartTimer();
            }
        }
        #endregion
        #region Methods
        public void StartTimer()
        {
            try
            {
                this._ProgressedTime = 0;
                if (this._Timer.IsRunning) this._Timer.Reset();
                this._Timer.Restart();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsElapsed(int milliseconds)
        {
            try
            {
                if (!this._Timer.IsRunning) return false;
                return (this._Timer.Elapsed.TotalMilliseconds >= milliseconds);
            }
            catch
            {

                return false;
            }
        }
        public double GetProgressedTime()
        {
            try
            {
                this._ProgressedTime = this._Timer.Elapsed.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return this._ProgressedTime;
        }
        #endregion
    }
}
