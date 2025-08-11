using System;
using System.Collections.Generic;
using System.Text;

namespace Sineva.OHT.Common
{
    public class VehicleIF_TeachingDataReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        public string DataVersion = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_TeachingDataReply()
        {
            try
            {
                _MessageCode = IFMessage.TeachingDataReply;
            }
            catch
            {
            }
        }
        public VehicleIF_TeachingDataReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.TeachingDataReply;
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

                int offset = 5;

                //BODY
                Acknowledge = (IFAcknowledge)stream[offset++];
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                DataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;

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
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //stream[offset++] = (byte)Acknowledge;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;

                //if (DataVersion.Length > 20)
                //{
                //    DataVersion = DataVersion.Substring(0, 20);
                //}
                //DataVersion = DataVersion.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(DataVersion), 0, stream, offset, 20); offset += 20;

                //stream[MessageLength - 1] = ETX;

                //return stream;


                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.Add((byte)Acknowledge);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));

                if (DataVersion.Length > 20)
                {
                    DataVersion = DataVersion.Substring(0, 20);
                }
                DataVersion = DataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(DataVersion));
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
                result.Add(string.Format("\t\tAcknowledge : {0}", Acknowledge));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tData Version : {0}", DataVersion));

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
