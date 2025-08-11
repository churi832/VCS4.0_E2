using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_EventReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        public VehicleEvent Event = VehicleEvent.ErrorReset;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_EventReply()
        {
            try
            {
                _MessageCode = IFMessage.EventReply;
            }
            catch
            {
            }
        }
        public VehicleIF_EventReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.EventReply;
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

                Acknowledge = (IFAcknowledge)(stream[offset++]);
                Event = (VehicleEvent)(stream[offset++]);
                VehicleNumber = (int)(BitConverter.ToInt16(stream, offset)); offset += 2;

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

                //stream[offset++] = Convert.ToByte(Acknowledge);
                //stream[offset++] = Convert.ToByte(Event);
                //Array.Copy(BitConverter.GetBytes(Convert.ToInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;

                //stream[MessageLength - 1] = ETX;

                //return stream;

                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.Add((byte)Acknowledge);
                byteStream.Add((byte)Event);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
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
                result.Add(string.Format("\t\tEvent : {0}", Event));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));

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
