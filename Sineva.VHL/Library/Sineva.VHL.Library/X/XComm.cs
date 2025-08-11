/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team HJYOU
 * Issue Date	: 23.01.17
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    [Editor(typeof(UIEditorPropertyEdit), typeof(System.Drawing.Design.UITypeEditor))]
    [Serializable]
    public class XComm
    {
        #region Fields
        private bool m_Initialized;

        private System.IO.Ports.SerialPort m_SerialPort;
        private PortNo m_CommPortNo = PortNo._Emulate;
        private int m_Baudrate = 9600;
        private Parity m_Parity = Parity.Even;
        private int m_DataBits = 8;
        private StopBits m_StopBits = StopBits.One;
        private int m_ReadTimeout = 1000;
        private int m_WriteTimeout = 1000;
        private static object m_LockKey = new object();

        private bool m_ForceOpen = false;
        #endregion

        #region Event
        public delegate void ReceivedDataEventHandler(object sender);
        public event ReceivedDataEventHandler ReceivedData;
        #endregion

        #region Properties
        [XmlIgnore(), Browsable(false), ReadOnly(true)]
        public System.IO.Ports.SerialPort SerialPort
        {
            get { return m_SerialPort; }
        }
        public PortNo _CommPortNo
        {
            get { return m_CommPortNo; }
            set { m_CommPortNo = value; }
        }
        public int _Baudrate
        {
            get { return m_Baudrate; }
            set { m_Baudrate = value; }
        }
        public Parity _Parity
        {
            get { return m_Parity; }
            set { m_Parity = value; }
        }
        public int _DataBits
        {
            get { return m_DataBits; }
            set { m_DataBits = value; }
        }
        public StopBits _StopBits
        {
            get { return m_StopBits; }
            set { m_StopBits = value; }
        }
        public int _ReadTimeout
        {
            get { return m_ReadTimeout; }
            set { m_ReadTimeout = value; }
        }
        public int _WriteTimeout
        {
            get { return m_WriteTimeout; }
            set { m_WriteTimeout = value; }
        }
        #endregion

        #region Constructor
        public XComm()
        {

        }

        public XComm(PortNo port, int baudrate, Parity parity, int databit, StopBits stopbit)
        {
            m_CommPortNo = port;
            m_Baudrate = baudrate;
            m_Parity = parity;
            m_DataBits = databit;
            m_StopBits = stopbit;
        }
        #endregion

        #region Methods - Initialize
        public void Initialize(bool force_open = false)
        {
            m_ForceOpen = force_open; //Simulation.SERIAL 를 무시 할수 있는 방법이 필요....
            m_SerialPort = new SerialPort();

            m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            m_SerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialErrorReceived);

            m_Initialized = true;
        }

        public void Open(PortNo port_no, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (!m_ForceOpen && port_no == PortNo._Emulate)
            {
                MessageBox.Show(string.Format("System run in Serial Port[{0}] Simulation mode!!", port_no.ToString()));
                return;
            }

            if (m_SerialPort != null && IsOpen() == false)
            {
                _CommPortNo = port_no;
                _Baudrate = baudRate;
                _Parity = parity;
                _DataBits = dataBits;
                _StopBits = stopBits;
                _ReadTimeout = 1000;
                _WriteTimeout = 1000;

                Open();
            }
        }

        public void Open()
        {
            if (!m_ForceOpen && _CommPortNo == PortNo._Emulate)
            {
                MessageBox.Show(string.Format("System run in Serial Port[{0}] Simulation mode!!", _CommPortNo.ToString()));
                return;
            }

            if (m_SerialPort != null && IsOpen() == false)
            {
                //lock(m_LockKey)
                {
                    m_SerialPort.PortName = _CommPortNo.ToString();
                    m_SerialPort.BaudRate = _Baudrate;
                    m_SerialPort.Parity = _Parity;
                    m_SerialPort.DataBits = _DataBits;
                    m_SerialPort.StopBits = _StopBits;
                    m_SerialPort.ReadTimeout = _ReadTimeout;
                    m_SerialPort.WriteTimeout = _WriteTimeout;
                    try
                    {
                        m_SerialPort.Open();
                        SerialCommLog.WriteLog(string.Format("XComm PortNo{0} Open", m_SerialPort.PortName));
                    }
                    catch (Exception err)
                    {
                        System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                        ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                    }
                }
            }
        }

        public void Close()
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return;

            try
            {
                m_SerialPort.Close();
                SerialCommLog.WriteLog(string.Format("XComm PortNo{0} Close", m_SerialPort.PortName));
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }
        }

        public override string ToString()
        {
            string value = string.Format("{0},{1},{2},{3}", _CommPortNo.ToString(), _Baudrate.ToString(), _Parity.ToString(), _DataBits.ToString(), _StopBits.ToString());
            return value;
        }
        #endregion

        #region - Methods
        public bool IsOpen()
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate)
                return false;

            if (m_SerialPort != null)
                return m_SerialPort.IsOpen;
            return false;
        }

        public void FlushInBuffer()
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return;

            if (IsOpen() == true)
            {
                try
                {
                    SerialPort.DiscardInBuffer();
                }
                catch (Exception err)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                }
            }
        }

        public void FlushOutBuffer()
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return;

            if (IsOpen() == true)
            {
                try
                {
                    SerialPort.DiscardOutBuffer();
                }
                catch (Exception err)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                }
            }
        }

        public bool IsDataInBuffer()
        {
            if (m_SerialPort == null) return false;
            if (m_SerialPort.BytesToRead > 0) return true;
            else return false;
        }

        public void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                FireReceivedDataEvent(this);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }
        }

        public void SerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            SerialCommLog.WriteLog(string.Format("{0} : {1} - [{2}]", (sender as SerialPort).PortName, e.ToString(), e.EventType.ToString()));
        }

        /// <summary>
        /// 수신버퍼에서 1byte읽기
        /// </summary>
        /// <returns>int32로 casting된 byte값, 읽은값이 없으면 -1</returns>
        public int ReadByte()
        {
            if (m_SerialPort == null) return -1;
            return m_SerialPort.ReadByte();
        }

        public int Read(ref char[] data, int length)
        {
            try
            {
                length = m_SerialPort.Read(data, 0, length);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return length;
        }

        public string ReadLine()
        {
            string data = null;

            try
            {
                data = m_SerialPort.ReadLine();
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return data;
        }

        public string ReadTo(string value)
        {
            string data = null;

            try
            {
                data = m_SerialPort.ReadTo(value);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return data;
        }



        public string ReadExisting()
        {
            string data = null;

            try
            {
                data = m_SerialPort.ReadExisting(); // 09.07.29 minhan
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return data;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int data = 0;

            try
            {
                data = m_SerialPort.Read(buffer, offset, count);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
            }

            return data;
        }

        public bool Write(string msg)
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return true;

            try
            {
                m_SerialPort.Write(msg);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                return false;
            }
            return true;
        }

        public bool Write(char[] buffer, int offset, int count)
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return true;

            try
            {
                m_SerialPort.Write(buffer, offset, count);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                return false;
            }
            return true;
        }

        public bool Write(byte[] buffer, int offset, int count)
        {
            if (!m_ForceOpen && m_CommPortNo == PortNo._Emulate) return true;

            try
            {
                m_SerialPort.Write(buffer, offset, count);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format(err.ToString()));
                return false;
            }
            return true;
        }

        public void FireReceivedDataEvent(object sender)
        {
            if (ReceivedData != null)
            {
                ReceivedData(sender);
            }
        }
        #endregion
    }
}
