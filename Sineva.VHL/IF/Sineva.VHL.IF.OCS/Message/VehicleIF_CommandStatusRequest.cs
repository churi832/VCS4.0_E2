using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_CommandStatusRequest : VehicleIF, IVehicleIF
    {
        #region Fields
        public string TransferCommandID = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_CommandStatusRequest()
        {
            try
            {
                _MessageCode = IFMessage.CommandStatusRequest;  //0x1D
            }
            catch
            {
            }
        }
        public VehicleIF_CommandStatusRequest(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.CommandStatusRequest;  //0x1D
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
                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                TransferCommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim('\0'); offset += 64;

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
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tTransfer Command ID : {0}", TransferCommandID.Trim('\0')));

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
