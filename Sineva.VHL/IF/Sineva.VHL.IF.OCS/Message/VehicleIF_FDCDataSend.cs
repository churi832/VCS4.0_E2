using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_FDCDataSend : VehicleIF, IVehicleIF
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_FDCDataSend()
        {
            try
            {
                _MessageCode = IFMessage.FDCDataSend;
                _MessageLength = 8;
            }
            catch
            {
            }
        }
        public VehicleIF_FDCDataSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.FDCDataSend;
                SetSystemByte(systemByte);
                _MessageLength = 8;
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

                //stream[MessageLength - 1] = ETX;

                //return stream;

                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
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
