using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_OperationConfigDataSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public int StraightSpeed = 0;
        public int BranchStraightSpeed = 0;
        public int JunctionStraightSpeed = 0;
        public int CurveSpeed = 0;
        public int BranchSpeed = 0;
        public int SBranchSpeed = 0;
        public int JunctionSpeed = 0;
        public int SJunctionSpeed = 0;
        public int AutoDoor1Mode = 1; // 1 : PIO Use, 2: ByPass
        public int AutoDoor2Mode = 1;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_OperationConfigDataSend()
        {
            try
            {
                _MessageCode = IFMessage.OperationConfigDataSend;
                //_MessageLength = 22;
            }
            catch
            {
            }
        }
        public VehicleIF_OperationConfigDataSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.OperationConfigDataSend;
                SetSystemByte(systemByte);
                //_MessageLength = 22;
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
                StraightSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                BranchStraightSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                JunctionStraightSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                CurveSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                BranchSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                SBranchSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                JunctionSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                SJunctionSpeed = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                AutoDoor1Mode = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                AutoDoor2Mode = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
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
                //_MessageLength = 37;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(StraightSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(BranchStraightSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CurveSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(BranchSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(SBranchSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(JunctionSpeed)), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(SJunctionSpeed)), 0, stream, offset, 2); offset += 2;

                //stream[MessageLength - 1] = ETX;

                //return stream;


                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(StraightSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(BranchStraightSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(JunctionStraightSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(CurveSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(BranchSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(SBranchSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(JunctionSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(SJunctionSpeed)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(AutoDoor1Mode)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(AutoDoor2Mode)));
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
                result.Add(string.Format("\t\tStraight Speed : {0}", StraightSpeed));
                result.Add(string.Format("\t\tBranch Straight Speed : {0}", BranchStraightSpeed));
                result.Add(string.Format("\t\tJunction Straight Speed : {0}", JunctionStraightSpeed));
                result.Add(string.Format("\t\tCurve Speed : {0}", CurveSpeed));
                result.Add(string.Format("\t\tBranch Speed : {0}", BranchSpeed));
                result.Add(string.Format("\t\tS-Branch Speed : {0}", SBranchSpeed));
                result.Add(string.Format("\t\tJunction Speed : {0}", JunctionSpeed));
                result.Add(string.Format("\t\tS-Junction Speed : {0}", SJunctionSpeed));
                result.Add(string.Format("\t\tAutoDoor1 PIO Use : {0}", AutoDoor1Mode));
                result.Add(string.Format("\t\tAutoDoor2 PIO Use : {0}", AutoDoor2Mode));

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
