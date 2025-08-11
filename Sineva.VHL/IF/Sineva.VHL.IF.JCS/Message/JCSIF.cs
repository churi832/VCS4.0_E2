using System;
using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public class JCSIF
    {
        #region Fields
        private int vehicleNumber = 0;
        private int junctionNumber = 0;
        protected byte STX = 0x02;
        protected byte ETX = 0x03;
        protected MessageCode _Code = MessageCode.None;
        protected UInt32 _MessageLength = 0;
        private UInt16 _SystemByte = 0;
        #endregion

        #region Properties
        public MessageCode Code
        {
            get { return _Code; }
        }
        public UInt32 MessageLength
        {
            get { return _MessageLength; }
        }
        public UInt16 SystemByte
        {
            get { return _SystemByte; }
        }

        public int VehicleNumber { get => vehicleNumber; set => vehicleNumber = value; }
        public int JunctionNumber { get => junctionNumber; set => junctionNumber = value; }
        #endregion

        #region Constructors
        public JCSIF()
        {

        }
        #endregion

        #region Methods
        public void SetSystemByte(UInt16 systemByte)
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
                byteStream.Add(0);
                byteStream.Add(0);
                byteStream.Add(Convert.ToByte(Code));
                byteStream.AddRange(BitConverter.GetBytes(SystemByte));

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
                byte[] lengthBytes = BitConverter.GetBytes(Convert.ToUInt32(byteStream.Count));
                byteStream[1] = lengthBytes[0];
                byteStream[2] = lengthBytes[1];
                byteStream[3] = lengthBytes[2];
                byteStream[4] = lengthBytes[3];

                _MessageLength = Convert.ToUInt16(byteStream.Count);
            }
            catch
            {
            }
        }
        protected bool SetHeader(byte[] header)
        {
            try
            {
                if (header.Length < 6) return false;

                _MessageLength = BitConverter.ToUInt32(header, 1);
                _Code = (MessageCode)(Convert.ToInt32(header[5]));
                _SystemByte = BitConverter.ToUInt16(header, 6);

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
                result.Add(string.Format("\t\tMessage Code : {0}", _Code));
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
