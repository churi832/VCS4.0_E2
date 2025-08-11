using System;
using System.Collections.Generic;

namespace Sineva.VHL.IF.JCS
{
    public class JCSIF_SR : JCSIF, IJCSIF
    {
        #region Fields
        public int CurrentNode = 0;
        public int CurrentLink = 0;
        public double PositionInLink = 0.0;
        public bool InRailState = false;
        public bool WorkingState = false;
        public bool PauseState = false;
        public bool DownState = false;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public JCSIF_SR()
        {
            try
            {
                _Code = MessageCode.SR;
            }
            catch
            {
            }
        }
        public JCSIF_SR(int systemByte)
        {
            try
            {
                _Code = MessageCode.SR;
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
                PositionInLink = BitConverter.ToDouble(stream, offset); offset += 8;
                InRailState = BitConverter.ToBoolean(stream, offset); offset += 1;
                WorkingState = BitConverter.ToBoolean(stream, offset); offset += 1;
                PauseState = BitConverter.ToBoolean(stream, offset); offset += 1;
                DownState = BitConverter.ToBoolean(stream, offset); offset += 1;

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
                byteStream.AddRange(BitConverter.GetBytes(PositionInLink));
                byteStream.AddRange(BitConverter.GetBytes(InRailState));
                byteStream.AddRange(BitConverter.GetBytes(WorkingState));
                byteStream.AddRange(BitConverter.GetBytes(PauseState));
                byteStream.AddRange(BitConverter.GetBytes(DownState));
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
                result.Add(string.Format("\t\tCurrent Node : {0}", CurrentNode));
                result.Add(string.Format("\t\tCurrent Link : {0}", CurrentLink));
                result.Add(string.Format("\t\tPosition In Link : {0}", PositionInLink));
                result.Add(string.Format("\t\tIn-Rail State : {0}", InRailState));
                result.Add(string.Format("\t\tWorking State : {0}", WorkingState));
                result.Add(string.Format("\t\tPause State : {0}", PauseState));
                result.Add(string.Format("\t\tDown State : {0}", DownState));

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
