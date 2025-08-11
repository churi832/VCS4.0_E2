using System;
using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public class JCSIF_PSR : JCSIF, IJCSIF
    {
        #region Fields
        public PassSendCode ReplyCode = PassSendCode.PassPermit;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public JCSIF_PSR()
        {
            try
            {
                _Code = MessageCode.PSR;
            }
            catch
            {
            }
        }
        public JCSIF_PSR(int systemByte)
        {
            try
            {
                _Code = MessageCode.PSR;
                SetSystemByte((UInt16)systemByte);
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public bool SetDataFromStream(byte[] stream)
        {
            try
            {
                if (SetHeader(stream) == false) return false;
                if (stream.Length < _MessageLength) return false;

                int offset = GetHeader().Length + 1;

                VehicleNumber = BitConverter.ToInt32(stream, offset); offset += 4;
                JunctionNumber = BitConverter.ToInt32(stream, offset); offset += 4;
                ReplyCode = (PassSendCode)stream[offset];

                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] GetStreamFromDataOrNull()
        {
            try
            {
                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(VehicleNumber));
                byteStream.AddRange(BitConverter.GetBytes(JunctionNumber));
                byteStream.Add(Convert.ToByte(ReplyCode));
                byteStream.Add(ETX);

                SetLengthBytes(ref byteStream);

                return byteStream.ToArray();
            }
            catch
            {
                return new byte[0];
            }
        }
        public List<String> GetLogData()
        {
            try
            {
                List<string> result = new List<string>();

                result.AddRange(GetHeaderLog());
                result.Add(string.Format("\t<BODY>"));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tJunction Number : {0}", JunctionNumber));
                result.Add(string.Format("\t\tPass Send Reply : {0}", ReplyCode));

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
