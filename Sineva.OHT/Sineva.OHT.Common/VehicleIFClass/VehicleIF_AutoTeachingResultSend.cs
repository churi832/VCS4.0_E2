using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_AutoTeachingResultSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public int PortNumber = 0;
        public VehicleOperationResult Result = VehicleOperationResult.Success;
        public int BarcodeValue1 = 0;
        public int BarcodeValue2 = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_AutoTeachingResultSend()
        {
            try
            {
                _MessageCode = IFMessage.AutoTeachingResultSend;
            }
            catch
            {
            }
        }
        public VehicleIF_AutoTeachingResultSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.AutoTeachingResultSend;
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
                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                PortNumber = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                Result = (VehicleOperationResult)stream[offset++];
                BarcodeValue1 = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                BarcodeValue2 = (int)BitConverter.ToUInt32(stream, offset); offset += 4;

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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(PortNumber)));
                byteStream.Add((byte)Result);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(BarcodeValue1)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(BarcodeValue2)));
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
                result.Add(string.Format("\t\tPort Number : {0}", PortNumber));
                result.Add(string.Format("\t\tResult : {0}", Result));

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
