using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_PathRequest : VehicleIF, IVehicleIF
    {
        #region Fields
        public PathRequestType RequestType = PathRequestType.Go;
        public int StartNodeNo = 0;
        public int EndNodeNo = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_PathRequest()
        {
            try
            {
                _MessageCode = IFMessage.PathRequest;
            }
            catch
            {
            }
        }
        public VehicleIF_PathRequest(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.PathRequest;
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
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                RequestType = (PathRequestType)stream[offset++];
                StartNodeNo = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                EndNodeNo = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
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
                byteStream.Add((byte)RequestType);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(StartNodeNo)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(EndNodeNo)));
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
                result.Add(string.Format("\t\tRequest Type : {0}", RequestType));
                result.Add(string.Format("\t\tStart Node No : {0}", StartNodeNo));
                result.Add(string.Format("\t\tEnd Node No : {0}", EndNodeNo));

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
