using System;
using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public class JCSIF_PR : JCSIF, IJCSIF
    {
        #region Fields
        public int CurrentNode = 0;
        public int CurrentLink = 0;
        public int TargetDistance = 0;
        public int PathNodeCount = 0;
        public List<int> PathNodes = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public JCSIF_PR()
        {
            try
            {
                _Code = MessageCode.PR;
            }
            catch
            {
            }
        }
        public JCSIF_PR(int systemByte)
        {
            try
            {
                _Code = MessageCode.PR;
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
                CurrentNode = BitConverter.ToInt32(stream, offset); offset += 4;
                CurrentLink = BitConverter.ToInt32(stream, offset); offset += 4;
                TargetDistance = BitConverter.ToInt32(stream, offset); offset += 4;
                PathNodeCount = BitConverter.ToInt32(stream, offset); offset += 4;

                for (int index = 0; index < PathNodeCount; index++)
                {
                    PathNodes.Add(BitConverter.ToInt32(stream, offset));
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
                byteStream.AddRange(BitConverter.GetBytes(CurrentNode));
                byteStream.AddRange(BitConverter.GetBytes(CurrentLink));
                byteStream.AddRange(BitConverter.GetBytes(TargetDistance));
                byteStream.AddRange(BitConverter.GetBytes(PathNodeCount));
                for (int index = 0; index < PathNodeCount; index++)
                {
                    byteStream.AddRange(BitConverter.GetBytes(PathNodes[index]));
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
                result.Add(string.Format("\t\tCurrentNode : {0}", CurrentNode));
                result.Add(string.Format("\t\tCurrentLink : {0}", CurrentLink));
                result.Add(string.Format("\t\tTargetDistance : {0}", TargetDistance));
                result.Add(string.Format("\t\tPathNodeCount : {0}", PathNodeCount));

                string extData = string.Empty;
                for (int index = 0; index < PathNodeCount; index++)
                {
                    extData += string.Format("{0} - ", PathNodes[index]);
                }
                result.Add(string.Format("\t\tPathNodes : {0}", extData.Substring(0, extData.Length - 3)));

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
