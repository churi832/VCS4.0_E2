using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_MapDataSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public MapDataType DataType = MapDataType.NodeData;
        public string DataVersion = string.Empty;
        public byte[] Data;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_MapDataSend()
        {
            try
            {
                _MessageCode = IFMessage.MapDataSend;
            }
            catch
            {
            }
        }
        public VehicleIF_MapDataSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.MapDataSend;
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
                DataType = (MapDataType)stream[offset++];
                DataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;
                //Data = Array.Cop
                Data = new byte[stream.Length - 32];
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
                //_MessageLength = 29 + Data.Length;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;
                //stream[offset++] = (byte)DataType;

                //if (DataVersion.Length > 20)
                //{
                //    DataVersion = DataVersion.Substring(0, 20);
                //}
                //DataVersion = DataVersion.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(DataVersion), 0, stream, offset, 20);offset += 20;
                //Array.Copy(Data, 0, stream, offset, Data.Length);

                //stream[MessageLength - 1] = ETX;

                //return stream;



                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add((byte)DataType);

                if (DataVersion.Length > 20)
                {
                    DataVersion = DataVersion.Substring(0, 20);
                }
                DataVersion = DataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(DataVersion));
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
                result.Add(string.Format("\t\tData Type : {0}", DataType));
                result.Add(string.Format("\t\tData Version : {0}", DataVersion.Trim('\0')));
                result.Add(string.Format("\t\tData : "));

                string temp = string.Empty;



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
