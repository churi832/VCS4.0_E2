using System;
using System.Collections.Generic;

namespace Sineva.OHT.Common
{
    public class VehicleIF_UserLoginReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public UserLoginResult Result = UserLoginResult.Success;
        public AuthorizationLevel Level = AuthorizationLevel.Operator;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_UserLoginReply()
        {
            try
            {
                _MessageCode = IFMessage.UserLoginReply;
            }
            catch
            {
            }
        }
        public VehicleIF_UserLoginReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.UserLoginReply;
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

                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                Result = (UserLoginResult)stream[offset++];
                Level = (AuthorizationLevel)stream[offset++];

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

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;
                //stream[offset++] = (byte)Result;
                //stream[offset++] = (byte)Level;

                //stream[MessageLength - 1] = ETX;

                //return stream;



                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add((byte)Result);
                byteStream.Add((byte)Level);
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
                result.Add(string.Format("\t\tLevel : {0}", Level));

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
