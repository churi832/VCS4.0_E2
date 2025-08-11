using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_MapDataRequestAcknowledge : VehicleIF, IVehicleIF
    {
        #region Fields
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        public MapDataType DataType = MapDataType.NodeData;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_MapDataRequestAcknowledge()
        {
            try
            {
                _MessageCode = IFMessage.MapDataRequestAcknowledge;
            }
            catch
            {
            }
        }
        public VehicleIF_MapDataRequestAcknowledge(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.MapDataRequestAcknowledge;
                SetSystemByte(systemByte);
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

                //BODY
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                Acknowledge = (IFAcknowledge)stream[offset++];
                DataType = (MapDataType)stream[offset++];

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
                //_MessageLength = 37;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;
                //stream[offset++] = (byte)Acknowledge;
                //stream[offset++] = (byte)DataType;




                //stream[MessageLength - 1] = ETX;

                //return stream;


                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add((byte)Acknowledge);
                byteStream.Add((byte)DataType);
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
                result.Add(string.Format("\t\tAcknowledge : {0}", Acknowledge));
                result.Add(string.Format("\t\tData Type : {0}", DataType));

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
