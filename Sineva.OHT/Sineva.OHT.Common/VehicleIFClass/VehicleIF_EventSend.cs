using System;
using System.Collections.Generic;
using System.Text;

namespace Sineva.OHT.Common
{
    public class VehicleIF_EventSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public VehicleEvent Event = VehicleEvent.ErrorReset;
        public int NodeNumber = 0;
        public string TransferCommandID = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_EventSend()
        {
            try
            {
                _MessageCode = IFMessage.EventSend;
            }
            catch
            {
            }
        }
        public VehicleIF_EventSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.EventSend;
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

                Event = (VehicleEvent)(stream[offset++]);
                VehicleNumber = (int)(BitConverter.ToInt16(stream, offset)); offset += 2;
                NodeNumber = BitConverter.ToInt32(stream, offset); offset += 4;
                TransferCommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim(); offset += 64;

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
                byteStream.Add((byte)Event);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(NodeNumber)));

                if (TransferCommandID.Length > 64)
                {
                    TransferCommandID = TransferCommandID.Substring(0, 64);
                }
                TransferCommandID = TransferCommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(TransferCommandID));

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
                result.Add(string.Format("\t\tEvent : {0}", Event));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tNode Number : {0}", NodeNumber));
                result.Add(string.Format("\t\tTransfer Command ID : {0}", TransferCommandID));

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
