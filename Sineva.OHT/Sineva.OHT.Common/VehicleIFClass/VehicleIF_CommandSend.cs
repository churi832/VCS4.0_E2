using System;
using System.Collections.Generic;
using System.Text;

namespace Sineva.OHT.Common
{
    public class VehicleIF_CommandSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public OCSCommand Command = OCSCommand.TransferCommandSend;
        public int DataSendInterval = 0;
        public string TransferCommandID = string.Empty;
        public string CassetteID = string.Empty;
        public int SourceID = 0;
        public int DestinationID = 0;
        public List<int> PathNodes = new List<int>();
        public List<int> SendPath = new List<int>();
        public List<int> FullPath = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_CommandSend()
        {
            try
            {
                _MessageCode = IFMessage.CommandSend;
            }
            catch
            {
            }
        }
        public VehicleIF_CommandSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.CommandSend;
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
                Command = (OCSCommand)stream[offset++];
                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                DataSendInterval = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                TransferCommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim(); offset += 64;
                CassetteID = Encoding.UTF8.GetString(stream, offset, 10).Trim(); offset += 10;

                SourceID = (int)(BitConverter.ToUInt32(stream, offset)); offset += 4;
                DestinationID = (int)(BitConverter.ToUInt32(stream, offset)); offset += 4;
                int countOfNodes = (int)(BitConverter.ToUInt16(stream, offset)); offset += 2;

                PathNodes.Clear();
                for (int index = 0; index < countOfNodes; index++)
                {
                    PathNodes.Add((int)(BitConverter.ToUInt32(stream, offset + index * 4)));
                    //offset += 2;
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
                //_MessageLength = 47 + PathNodes.Count * 2;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //stream[5] = (byte)Command;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, 6, 2);
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(DataSendInterval)), 0, stream, 8, 2);

                //if (TransferCommandID.Length > 20)
                //{
                //    TransferCommandID = TransferCommandID.Substring(0, 20);
                //}
                //TransferCommandID = TransferCommandID.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(TransferCommandID), 0, stream, 10, 20);

                //if (CassetteID.Length > 10)
                //{
                //    CassetteID = CassetteID.Substring(0, 10);
                //}
                //CassetteID = CassetteID.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(CassetteID), 0, stream, 30, 10);


                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(SourceID)), 0, stream, 40, 2);
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(DestinationID)), 0, stream, 42, 2);
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(PathNodes.Count)), 0, stream, 44, 2);

                //for (int index = 0; index < PathNodes.Count; index++)
                //{
                //    Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(PathNodes[index])), 0, stream, 46 + index * 2, 2);
                //}

                //stream[MessageLength - 1] = ETX;

                //return stream;


                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.Add((byte)Command);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(DataSendInterval)));

                if (TransferCommandID.Length > 64)
                {
                    TransferCommandID = TransferCommandID.Substring(0, 64);
                }
                TransferCommandID = TransferCommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(TransferCommandID));

                if (CassetteID.Length > 10)
                {
                    CassetteID = CassetteID.Substring(0, 10);
                }
                CassetteID = CassetteID.Trim().PadRight(10, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(CassetteID));

                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(SourceID)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(DestinationID)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(PathNodes.Count)));

                for (int index = 0; index < PathNodes.Count; index++)
                {
                    byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(PathNodes[index])));
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
                result.Add(string.Format("\t\tCommand : {0}", Command));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tData Send Interval : {0}", DataSendInterval));
                result.Add(string.Format("\t\tTransfer Command ID : {0}", TransferCommandID));
                result.Add(string.Format("\t\tCassette ID : {0}", CassetteID));
                result.Add(string.Format("\t\tSource ID : {0}", SourceID));
                result.Add(string.Format("\t\tDestination ID : {0}", DestinationID));
                result.Add(string.Format("\t\tCount of Nodes : {0}", PathNodes.Count));

                StringBuilder log = new StringBuilder(string.Format("\t\tPath Node : "));
                foreach (int node in PathNodes)
                {
                    log.Append(string.Format("{0} - ", node));
                }
                result.Add(log.ToString());

                return result;
            }
            catch
            {
                return new List<string>();
            }
        }
        public VehicleIF_CommandSend GetCopyOrNull()
        {
            try
            {
                return (VehicleIF_CommandSend)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
