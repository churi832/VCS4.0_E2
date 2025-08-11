using System;
using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public class JCSIF_PS : JCSIF, IJCSIF
    {
        #region Fields        
        public PassSendCode SendCode = PassSendCode.PassPermit;
        public int PassJunctionCount = 0;
        public List<int> PassJunctionNumber = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public JCSIF_PS()
        {
            try
            {
                _Code = MessageCode.PS;
            }
            catch
            {
            }
        }
        public JCSIF_PS(int systemByte)
        {
            try
            {
                _Code = MessageCode.PS;
                SetSystemByte((UInt16)systemByte);
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

                VehicleNumber = BitConverter.ToInt32(stream, offset); offset += 4;
                JunctionNumber = BitConverter.ToInt32(stream, offset); offset += 4;
                SendCode = (PassSendCode)stream[offset]; offset += 1;
                PassJunctionCount = BitConverter.ToInt32(stream, offset); offset += 4;
                for (int index = 0; index < PassJunctionCount; index++)
                {
                    PassJunctionNumber.Add(BitConverter.ToInt32(stream, offset));
                    offset += 4;
                }

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
                byteStream.AddRange(BitConverter.GetBytes(VehicleNumber));
                byteStream.AddRange(BitConverter.GetBytes(JunctionNumber));
                byteStream.Add(Convert.ToByte(SendCode));
                byteStream.AddRange(BitConverter.GetBytes(PassJunctionCount));
                for (int index = 0; index < PassJunctionCount; index++)
                {
                    byteStream.AddRange(BitConverter.GetBytes(PassJunctionNumber[index]));
                }
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
                result.Add(string.Format("\t\tJunction Number : {0}", JunctionNumber));
                result.Add(string.Format("\t\tPass Send Code : {0}", SendCode));
                result.Add(string.Format("\t\tPass Junction Count : {0}", PassJunctionCount));

                string extData = string.Empty;
                for (int index = 0; index < PassJunctionCount; index++)
                {
                    extData += string.Format("{0} - ", PassJunctionNumber[index]);
                }
                result.Add(string.Format("\t\tPass Junction Number : {0}", extData.Substring(0, extData.Length - 3)));

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
