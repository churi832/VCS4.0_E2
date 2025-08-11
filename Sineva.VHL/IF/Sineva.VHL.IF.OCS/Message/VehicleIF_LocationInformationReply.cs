using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_LocationInformationReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public LocationInformationChangeCode ChangeCode = LocationInformationChangeCode.Add;
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_LocationInformationReply()
        {
            try
            {
                _MessageCode = IFMessage.LocationInformationSendReply;
            }
            catch
            {
            }
        }
        public VehicleIF_LocationInformationReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.LocationInformationSendReply;
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
                ChangeCode = (LocationInformationChangeCode)stream[offset++];
                Acknowledge = (IFAcknowledge)stream[offset++];

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
                byteStream.Add((byte)ChangeCode);
                byteStream.Add((byte)Acknowledge);
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
                result.Add(string.Format("\t\tChange Code : {0}", ChangeCode));
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
