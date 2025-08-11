using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_PathSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public PathRequestType RequestType = PathRequestType.Go;
        public int StartNodeNo = 0;
        public int EndNodeNo = 0;
        public List<int> PathNodes = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_PathSend()
        {
            try
            {
                _MessageCode = IFMessage.PathSend;
            }
            catch
            {
            }
        }
        public VehicleIF_PathSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.PathSend;
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
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                RequestType = (PathRequestType)stream[offset++];
                StartNodeNo = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                EndNodeNo = (int)BitConverter.ToUInt32(stream, offset); offset += 4;

                int countOfNodes = BitConverter.ToUInt16(stream, offset); offset += 2;

                PathNodes.Clear();
                for (int index = 0; index < countOfNodes; index++)
                {
                    PathNodes.Add((int)BitConverter.ToUInt32(stream, offset + index * 4));
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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add((byte)RequestType);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(StartNodeNo)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(EndNodeNo)));
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
                result.Add(string.Format("\t\tVehicle Number : {0}", VehicleNumber));
                result.Add(string.Format("\t\tRequest Type : {0}", RequestType));
                result.Add(string.Format("\t\tStart Node No : {0}", StartNodeNo));
                result.Add(string.Format("\t\tEnd Node No : {0}", EndNodeNo));
                result.Add(string.Format("\t\tCount of Nodes : {0}", PathNodes.Count));
                result.Add(string.Format("\t\tPath Node : {0}", string.Join("-", PathNodes)));

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
