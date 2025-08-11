using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_AlarmEventReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_AlarmEventReply()
        {
            try
            {
                _MessageCode = IFMessage.AlarmEventReply;
            }
            catch
            {
            }
        }
        public VehicleIF_AlarmEventReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.AlarmEventReply;
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

                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                Acknowledge = (IFAcknowledge)(stream[offset]);

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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add(Convert.ToByte(Acknowledge));
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
                result.Add(string.Format("\t\tVehicle Number : {0}", _VehicleNumber));
                result.Add(string.Format("\t\tAcknowledge : {0}", Acknowledge));

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
