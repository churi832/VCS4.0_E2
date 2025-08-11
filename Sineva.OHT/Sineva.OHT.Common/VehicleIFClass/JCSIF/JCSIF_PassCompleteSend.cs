using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class JCSIF_PassCompleteSend : JCSIF, IJCSIF
    {
        #region Fields
        public int JuctionPointNumber = 0;
        public JCSType JuctionType = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public JCSIF_PassCompleteSend()
        {
            try
            {
                _MessageCode = JCSIFMessage.JunctionPassCompleteSend;
            }
            catch
            {
            }
        }
        public JCSIF_PassCompleteSend(int systemByte)
        {
            try
            {
                _MessageCode = JCSIFMessage.JunctionPassCompleteSend;
                SetSystemByte((byte)systemByte);
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

                int offset = 6;

                //BODY
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt32(stream, offset)); offset += 4;
                JuctionPointNumber = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                JuctionType = (JCSType)stream[offset++];
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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(VehicleNumber)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(JuctionPointNumber)));
                byteStream.Add(Convert.ToByte(JuctionType));
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
                result.Add(string.Format("\t\tJuction Point Number : {0}", JuctionPointNumber));
                result.Add(string.Format("\t\tJuction Type : {0}", JuctionType));

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
