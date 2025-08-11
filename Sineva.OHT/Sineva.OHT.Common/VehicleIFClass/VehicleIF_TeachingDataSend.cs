using System;
using System.Collections.Generic;
using System.IO;

namespace Sineva.OHT.Common
{
    public class VehicleIF_TeachingDataSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public byte[] Data;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_TeachingDataSend()
        {
            try
            {
                _MessageCode = IFMessage.TeachingDataSend;
            }
            catch
            {
            }
        }
        public VehicleIF_TeachingDataSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.TeachingDataSend;
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

                //BODY
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                Data = new byte[stream.Length - 8];
                Array.Copy(stream, offset, Data, 0, Data.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SetBinaryDataFromFile(string fileName)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists == false) return;

                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    Data = new byte[fileStream.Length];
                    fileStream.Read(Data, 0, Data.Length);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void SetBinaryDataFromByteArray(byte[] memory)
        {
            try
            {
                Data = new byte[memory.Length];
                Array.Copy(memory, Data, memory.Length);
            }
            catch
            {
            }
        }
        public void SetFileFromBinaryData(string fileName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    fileStream.Write(Data, 0, Data.Length);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public byte[] GetStreamFromDataOrNull()
        {
            try
            {
                //_MessageLength = 8 + Data.Length;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(Data, 0, stream, offset, Data.Length);

                //stream[MessageLength - 1] = ETX;

                //return stream;



                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.AddRange(Data);
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
