using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF
    {
        #region Fields
        protected int _VehicleNumber = 0;
        protected byte STX = 0x02;
        protected byte ETX = 0x03;
        protected IFMessage _MessageCode = IFMessage.CommandSend;
        protected int _MessageLength = 0;
        private int _SystemByte = 0;
        #endregion

        #region Properties
        public int VehicleNumber
        {
            get { return _VehicleNumber; }
            set { _VehicleNumber = value; }
        }
        public IFMessage MessageCode
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
        public VehicleIF()
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
                //byte[] header = new byte[4];
                //byte[] length = BitConverter.GetBytes(_MessageLength);
                //Array.Copy(length, 0, header, 0, 2);
                //header[2] = Convert.ToByte(MessageCode);
                //header[3] = Convert.ToByte(SystemByte);

                List<byte> byteStream = new List<byte>();
                byteStream.Add(0);
                byteStream.Add(0);
                byteStream.Add(0);
                byteStream.Add(0);
                byteStream.Add(Convert.ToByte(MessageCode));
                //byteStream.Add(Convert.ToByte(SystemByte));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(SystemByte)));

                return byteStream.ToArray();
            }
            catch
            {
                return new byte[7];
            }
        }
        protected void SetLengthBytes(ref List<byte> byteStream)
        {
            try
            {
                byte[] lengthBytes = BitConverter.GetBytes(Convert.ToInt32(byteStream.Count));
                byteStream[1] = lengthBytes[0];
                byteStream[2] = lengthBytes[1];
                byteStream[3] = lengthBytes[2];
                byteStream[4] = lengthBytes[3];

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
                if (header.Length < 7) return false;

                _MessageLength = BitConverter.ToInt32(header, 1);
                _MessageCode = (IFMessage)(Convert.ToInt32(header[5]));
                //_SystemByte = Convert.ToInt32(header[4]);
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
