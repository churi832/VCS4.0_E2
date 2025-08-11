using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_CommandSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public OCSCommand ProcessCommand = OCSCommand.None;
        public int DataSendInterval = 0;
        public string TransferCommandID = string.Empty;
        public string CassetteID = string.Empty;
        public string DateTimeSet = string.Empty; //"20231201142401"
        public int SourceID = 0;
        public int DestinationID = 0; // Go Command 일때 TypeOfDestination==1 일때 NodeID로 생각해야 함.
        public int TypeOfDestination = 0; //Go Command 구분( 1:By Location, 2:By Distance )
        public double TargetNodeToDistance = 0.0f;
        public List<int> PathNodes = new List<int>();
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
                ProcessCommand = (OCSCommand)stream[offset++];
                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                DataSendInterval = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                TransferCommandID = Encoding.UTF8.GetString(stream, offset, 64).Trim('\0'); offset += 64;
                CassetteID = Encoding.UTF8.GetString(stream, offset, 64).Trim('\0'); offset += 64;
                DateTimeSet = Encoding.UTF8.GetString(stream, offset, 14).Trim('\0'); offset += 14;
                TypeOfDestination = (int)stream[offset]; offset += 1;

                SourceID = (int)(BitConverter.ToUInt32(stream, offset)); offset += 4;
                DestinationID = (int)(BitConverter.ToUInt32(stream, offset)); offset += 4;
                TargetNodeToDistance = (double)(BitConverter.ToDouble(stream, offset)); offset += 8;

                int countOfNodes = (int)(BitConverter.ToUInt16(stream, offset)); offset += 2;
                PathNodes.Clear();
                for (int index = 0; index < countOfNodes; index++)
                {
                    PathNodes.Add((int)(BitConverter.ToUInt32(stream, offset + index * 4)));
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
                byteStream.Add((byte)ProcessCommand);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(DataSendInterval)));

                if (TransferCommandID.Length > 64)
                {
                    TransferCommandID = TransferCommandID.Substring(0, 64);
                }
                TransferCommandID = TransferCommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(TransferCommandID));

                if (CassetteID.Length > 64)
                {
                    CassetteID = CassetteID.Substring(0, 64);
                }
                CassetteID = CassetteID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(CassetteID));

                if (DateTimeSet.Length > 14)
                {
                    DateTimeSet = DateTimeSet.Substring(0, 14);
                }
                DateTimeSet = DateTimeSet.Trim().PadRight(14, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(DateTimeSet));

                byteStream.Add((byte)TypeOfDestination);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(SourceID)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(DestinationID)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToDouble(TargetNodeToDistance)));
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
                result.Add(string.Format("\t\tCommand : {0}", ProcessCommand));
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tData Send Interval : {0}", DataSendInterval));
                result.Add(string.Format("\t\tTransfer Command ID : {0}", TransferCommandID.Trim('\0')));
                result.Add(string.Format("\t\tCassette ID : {0}", CassetteID.Trim('\0')));
                result.Add(string.Format("\t\tDateTimeSet : {0}", DateTimeSet.Trim('\0')));
                result.Add(string.Format("\t\tDestination Type : {0}", TypeOfDestination));
                result.Add(string.Format("\t\tSource ID : {0}", SourceID));
                result.Add(string.Format("\t\tDestination ID : {0}", DestinationID));
                result.Add(string.Format("\t\tDistance from Last Node : {0}", TargetNodeToDistance));
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
