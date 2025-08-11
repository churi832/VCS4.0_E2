using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_AlarmEventSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public int AlarmID = 0;
        public AlarmSetCode AlarmStatus = AlarmSetCode.Set;
        public AlarmType AlarmType = AlarmType.Warning;
        public int AlarmCode = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_AlarmEventSend()
        {
            try
            {
                _MessageCode = IFMessage.AlarmEventSend;
            }
            catch
            {
            }
        }
        public VehicleIF_AlarmEventSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.AlarmEventSend;
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
                AlarmID = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                AlarmStatus = (AlarmSetCode)stream[offset++];
                AlarmType = (AlarmType)stream[offset++];
                AlarmCode = (int)stream[offset++];

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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(AlarmID)));
                byteStream.Add((byte)AlarmStatus);
                byteStream.Add((byte)AlarmType);
                byteStream.Add((byte)AlarmCode);
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
                result.Add(string.Format("\t\tAlarm ID : {0}", AlarmID));
                result.Add(string.Format("\t\tAlarm Status : {0}", AlarmStatus));
                result.Add(string.Format("\t\tAlarm Type : {0}", AlarmType));
                result.Add(string.Format("\t\tAlarm Code : {0}", AlarmCode));

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
