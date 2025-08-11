using Sineva.OHT.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sineva.OHT.IF.Socket
{
    [Guid("0C874F18-9CFA-4DF0-9ABA-F1BE2BC420EB")]
    public class ClientObject
    {
        #region Fields
        private int _VehicleNo = -1;
        private byte[] _ReceiveBuffer;
        private bool _VehicleConnected = false;
        private List<byte> _Buffer = new List<byte>();
        private System.Net.Sockets.Socket _ClientSocket;
        private DateTime _LastWatchdogTime;
        private bool _WatchdogErrorState = false;
        private byte _SystemByte = 0x1;
        private Dictionary<byte, TimeCheck> _SentMessageList = new Dictionary<byte, TimeCheck>();
        #endregion

        #region Properties
        public int VehicleNo
        {
            get { return _VehicleNo; }
            set { _VehicleNo = value; }
        }
        public bool VehicleConnected
        {
            get { return _VehicleConnected; }
            set
            {
                if (_VehicleConnected != value)
                {
                    _VehicleConnected = value;
                    if (VehicleConnectionChanged != null)
                    {
                        VehicleConnectionChanged(this, _VehicleConnected);
                    }
                }
            }
        }
        public System.Net.Sockets.Socket ClientSocket
        {
            get { return _ClientSocket; }
        }
        public Dictionary<byte, TimeCheck> SentMessageList
        {
            get { return _SentMessageList; }
        }
        public bool WatchdogErrorState
        {
            get { return _WatchdogErrorState; }
            set { _WatchdogErrorState = value; }
        }
        public byte[] ReceiveBuffer
        {
            get { return _ReceiveBuffer; }
            set { _ReceiveBuffer = value; }
        }
        public List<byte> Buffer
        {
            get { return _Buffer; }
            set { _Buffer = value; }
        }
        #endregion

        #region Events
        public delegate void ClientObjectEvent(object sender, object eventData);

        public event ClientObjectEvent VehicleConnectionChanged;
        #endregion

        #region Constructors
        public ClientObject(System.Net.Sockets.Socket socket)
        {
            _ClientSocket = socket;
        }
        #endregion

        #region Methods
        public void UpdateLastWatchdog()
        {
            try
            {
                _LastWatchdogTime = DateTime.Now;
            }
            catch
            {
            }
        }
        public Int64 GetCurrentWatchdogProgressTime()
        {
            try
            {
                return Convert.ToInt64((DateTime.Now - _LastWatchdogTime).TotalMilliseconds);
            }
            catch
            {
                return 0;
            }
        }
        public byte GetCurrentSystemByte()
        {
            try
            {
                byte returnValue = _SystemByte;

                _SystemByte += 1;
                if (_SystemByte == 0)
                {
                    _SystemByte = 1;
                }

                return returnValue;
            }
            catch
            {
                return 0;
            }
        }
        public void SentMessage(byte systemByte)
        {
            try
            {
                if (_SentMessageList.ContainsKey(systemByte) == false)
                {
                    _SentMessageList.Add(systemByte, new TimeCheck());
                    _SentMessageList[systemByte].StartTimer();
                }
            }
            catch
            {
            }
        }
        public void ReceivedReply(byte systemByte)
        {
            try
            {
                if (_SentMessageList.ContainsKey(systemByte) == true)
                {
                    _SentMessageList.Remove(systemByte);
                }
            }
            catch
            {
            }
        }
        public IEnumerable<byte> SentMessageListKey()
        {
            return _SentMessageList.Keys;
        }
        public UInt32 GetMessageProgressTime(byte systemByte)
        {
            try
            {
                if (_SentMessageList.ContainsKey(systemByte) == true)
                {
                    return Convert.ToUInt32(_SentMessageList[systemByte].GetProgressedTime());
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
    }
}
