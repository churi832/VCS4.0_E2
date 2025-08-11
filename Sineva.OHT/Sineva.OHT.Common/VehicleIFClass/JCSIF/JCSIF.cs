using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class JCSIF
    {
        #region Fields
        protected int _VehicleNumber = 0;
        protected byte STX = 0x02;
        protected byte ETX = 0x03;
        protected JCSIFMessage _MessageCode = JCSIFMessage.InitialStateRequest;
        protected int _MessageLength = 0;
        private int _SystemByte = 0;
        #endregion

        #region Properties
        public int VehicleNumber
        {
            get { return _VehicleNumber; }
            set { _VehicleNumber = value; }
        }
        public JCSIFMessage MessageCode
        {
            get { return _MessageCode; }
        }
        public int MessageLength
        {
            get { return _MessageLength; }
        }
        public int SystemByte
        {
            get { return _SystemByte; }
        }
        #endregion

        #region Constructors
        public JCSIF()
        {

        }
        #endregion

        #region Methods
        public void SetSystemByte(int systemByte)
        {
            _SystemByte = systemByte;
        }
        protected byte[] GetHeader()
        {
            try
            {
                List<byte> byteStream = new List<byte>();
                byteStream.Add(0);
                byteStream.Add(0);
                byteStream.Add(Convert.ToByte(MessageCode));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(SystemByte)));

                return byteStream.ToArray();
            }
            catch
            {
                return new byte[4];
            }
        }
        protected void SetLengthBytes(ref List<byte> byteStream)
        {
            try
            {
                byte[] lengthBytes = BitConverter.GetBytes(Convert.ToUInt16(byteStream.Count));
                byteStream[1] = lengthBytes[0];
                byteStream[2] = lengthBytes[1];

                _MessageLength = byteStream.Count;
            }
            catch
            {
            }
        }
        protected bool SetHeader(byte[] header)
        {
            try
            {
                if (header.Length < 5) return false;

                _MessageLength = BitConverter.ToInt16(header, 1);
                _MessageCode = (JCSIFMessage)(Convert.ToInt32(header[3]));
                _SystemByte = BitConverter.ToUInt16(header,4);

                return true;
            }
            catch
            {
                return false;
            }
        }
        protected List<string> GetHeaderLog()
        {
            try
            {
                List<string> result = new List<string>();

                result.Add(string.Format("\t<HEADER>"));
                result.Add(string.Format("\t\tMessage Length : {0}", _MessageLength));
                result.Add(string.Format("\t\tMessage Code : {0}", _MessageCode));
                result.Add(string.Format("\t\tSystem Byte : {0}", _SystemByte));

                return result;
            }
            catch
            {
                return new List<string>();
            }
        }
        #endregion
    }
}
