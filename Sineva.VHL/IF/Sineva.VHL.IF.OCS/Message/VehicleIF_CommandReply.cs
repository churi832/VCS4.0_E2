using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_CommandReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public IFAcknowledge Acknowledge = IFAcknowledge.ACK;
        public OCSCommand Command = OCSCommand.None;
        public string TransferCommandID = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_CommandReply()
        {
            try
            {
                _MessageCode = IFMessage.CommandReply;
            }
            catch
            {
            }
        }
        public VehicleIF_CommandReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.CommandReply;
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

                Acknowledge = (IFAcknowledge)stream[offset++];
                Command = (OCSCommand)stream[offset++];
                TransferCommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim('\0'); offset += 64;
                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;

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
                //stream[offset++] = Convert.ToByte(Command);
                //Array.Copy(BitConverter.GetBytes(Convert.ToInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;

                //stream[MessageLength - 1] = ETX;

                //return stream;

                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.Add((byte)Acknowledge);
                byteStream.Add((byte)Command);

                if (TransferCommandID.Length > 64)
                {
                    TransferCommandID = TransferCommandID.Substring(0, 64);
                }
                TransferCommandID = TransferCommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(TransferCommandID));

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
                result.Add(string.Format("\t\tCommand : {0}", Command));
                result.Add(string.Format("\t\tTransfer Command ID : {0}", TransferCommandID.Trim('\0')));
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
