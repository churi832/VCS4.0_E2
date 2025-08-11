using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_UserLoginRequest : VehicleIF, IVehicleIF
    {
        #region Fields
        public string UserID = string.Empty;
        public string Password = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_UserLoginRequest()
        {
            try
            {
                _MessageCode = IFMessage.UserLoginRequest;
            }
            catch
            {
            }
        }
        public VehicleIF_UserLoginRequest(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.UserLoginRequest;
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
                UserID = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;
                Password = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;

                UserID = UserID.Replace('\0', ' ').Trim();
                Password = Password.Replace('\0', ' ').Trim();

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

                //if (UserID.Length > 20)
                //{
                //    UserID = UserID.Substring(0, 20);
                //}
                //UserID = UserID.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(UserID), 0, stream, offset, 20); offset += 20;

                //if (Password.Length > 20)
                //{
                //    Password = Password.Substring(0, 20);
                //}
                //Password = Password.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(Password), 0, stream, offset, 20); offset += 20;

                //stream[MessageLength - 1] = ETX;

                //return stream;




                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));

                if (UserID.Length > 20)
                {
                    UserID = UserID.Substring(0, 20);
                }
                UserID = UserID.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(UserID));

                if (Password.Length > 20)
                {
                    Password = Password.Substring(0, 20);
                }
                Password = Password.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(Password));
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
                result.Add(string.Format("\t\tUser ID : {0}", UserID.Trim('\0')));
                result.Add(string.Format("\t\tPassword : {0}", Password.Trim('\0')));

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
