using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_IDReadResultSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public IDReadResult Result = IDReadResult.Success;
        public string CommandID = string.Empty;
        public string CarrierID = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_IDReadResultSend()
        {
            try
            {
                _MessageCode = IFMessage.IDReadResultSend;
            }
            catch
            {
            }
        }
        public VehicleIF_IDReadResultSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.IDReadResultSend;
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

                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                Result = (IDReadResult)stream[offset++];
                CommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim(); offset += 64;
                CarrierID = Encoding.UTF8.GetString(stream, offset, 64).Trim(); offset += 64;

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
                byteStream.Add((byte)Result);

                if (CommandID.Length > 64)
                {
                    CommandID = CommandID.Substring(0, 64);
                }
                CommandID = CommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(CommandID));

                if (CarrierID.Length > 64)
                {
                    CarrierID = CommandID.Substring(0, 64);
                }
                CarrierID = CarrierID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(CarrierID));

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
                result.Add(string.Format("\t\tResult : {0}", Result));
                result.Add(string.Format("\t\tCommand ID : {0}", CommandID));
                result.Add(string.Format("\t\tCarrier ID : {0}", CarrierID));

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
