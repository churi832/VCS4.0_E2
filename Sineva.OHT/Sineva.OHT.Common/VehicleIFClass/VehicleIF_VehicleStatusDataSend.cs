using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sineva.OHT.Common
{
    public class VehicleIF_VehicleStatusDataSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public bool InRailStatus = false;
        public bool BusyStatus = false;
        public bool PowerOnStatus = false;
        public bool AutoRunStatus = false;
        public bool PauseStatus = false;
        public bool ErrorStatus = false;
        public bool MovingStatus = false;
        //public bool AutoTeachingStatus = false;
        public bool CarrierExistStatus = false;
        public bool[] InputStatus = new bool[64];
        public bool[] OutputStatus = new bool[64];
        public int CurrentSpeed = 0;
        public int BarcodeValue1 = 0;
        public int BarcodeValue2 = 0;
        public int CurrentNodeNo = 0;
        public int NextNodeNo = 0;
        public int TargetNodeNo = 0;
        public string CurrentCommandID = string.Empty;

        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_VehicleStatusDataSend()
        {
            try
            {
                _MessageCode = IFMessage.StatusDataSend;
            }
            catch
            {
            }
        }
        public VehicleIF_VehicleStatusDataSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.StatusDataSend;
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

                VehicleNumber = (int)BitConverter.ToUInt16(stream, offset); offset += 2;
                InRailStatus = Convert.ToBoolean(stream[offset++]);
                BusyStatus = Convert.ToBoolean(stream[offset++]);
                PowerOnStatus = Convert.ToBoolean(stream[offset++]);
                AutoRunStatus = Convert.ToBoolean(stream[offset++]);
                PauseStatus = Convert.ToBoolean(stream[offset++]);
                ErrorStatus = Convert.ToBoolean(stream[offset++]);
                MovingStatus = Convert.ToBoolean(stream[offset++]);
                //AutoTeachingStatus = Convert.ToBoolean(stream[offset++]);
                CarrierExistStatus = Convert.ToBoolean(stream[offset++]);

                InputStatus = new bool[64];
                OutputStatus = new bool[64];

                BitArray bits;

                for (int byteIndex = 0; byteIndex < 8; byteIndex++)
                {
                    bits = new BitArray(new byte[] { stream[offset + byteIndex] });
                    bits.CopyTo(InputStatus, 8 * byteIndex);

                    bits = new BitArray(new byte[] { stream[offset + 8 + byteIndex] });
                    bits.CopyTo(OutputStatus, 8 * byteIndex);
                }

                offset += 16;

                CurrentSpeed = BitConverter.ToInt32(stream, offset); offset += 4;
                BarcodeValue1 = BitConverter.ToInt32(stream, offset); offset += 4;
                BarcodeValue2 = BitConverter.ToInt32(stream, offset); offset += 4;
                CurrentNodeNo = BitConverter.ToInt32(stream, offset); offset += 4;
                NextNodeNo = BitConverter.ToInt32(stream, offset); offset += 4;
                TargetNodeNo = BitConverter.ToInt32(stream, offset); offset += 4;
                CurrentCommandID = Encoding.ASCII.GetString(stream, offset, 64).Trim(); offset += 64;

                CurrentCommandID = CurrentCommandID.Replace('\0', ' ').Trim();

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

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, 5, 2); offset += 2;
                //stream[offset++] = (InRailStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (BusyStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (PowerOnStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (AutoRunStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (PauseStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (ErrorStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (MovingStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (AutoTeachingStatus == true) ? (byte)0x1 : (byte)0x0;
                //stream[offset++] = (CarrierExistStatus == true) ? (byte)0x1 : (byte)0x0;

                //BitArray inputBits = new BitArray(InputStatus);
                //BitArray outputBits = new BitArray(OutputStatus);
                //inputBits.CopyTo(stream, offset); offset += 8;
                //outputBits.CopyTo(stream, offset); offset += 8;

                //Array.Copy(BitConverter.GetBytes(CurrentSpeed), 0, stream, offset, 4); offset += 4;
                //Array.Copy(BitConverter.GetBytes(BarcodeValue1), 0, stream, offset, 4); offset += 4;
                //Array.Copy(BitConverter.GetBytes(BarcodeValue2), 0, stream, offset, 4); offset += 4;
                //Array.Copy(BitConverter.GetBytes(CurrentNodeNo), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(NextNodeNo), 0, stream, offset, 2); offset += 2;
                //Array.Copy(BitConverter.GetBytes(TargetNodeNo), 0, stream, offset, 2); offset += 2;

                //if (CurrentCommandID.Length > 20)
                //{
                //    CurrentCommandID = CurrentCommandID.Substring(0, 20);
                //}
                //else
                //{
                //    CurrentCommandID = CurrentCommandID.PadRight(20, ' ');
                //}

                //Array.Copy(Encoding.ASCII.GetBytes(CurrentCommandID), 0, stream, offset, 20); offset += 20;
                //stream[MessageLength - 1] = ETX;

                //return stream;



                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));
                byteStream.Add((InRailStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((BusyStatus == true) ? (byte)0x1 : (byte)0x0);

                byteStream.Add((PowerOnStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((AutoRunStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((PauseStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((ErrorStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((MovingStatus == true) ? (byte)0x1 : (byte)0x0);
                //byteStream.Add((AutoTeachingStatus == true) ? (byte)0x1 : (byte)0x0);
                byteStream.Add((CarrierExistStatus == true) ? (byte)0x1 : (byte)0x0);

                BitArray inputBits = new BitArray(InputStatus);
                byte[] inputBitArray = new byte[inputBits.Length / 8];
                inputBits.CopyTo(inputBitArray, 0);
                byteStream.AddRange(inputBitArray);

                BitArray outputBits = new BitArray(OutputStatus);
                byte[] outputBitArray = new byte[outputBits.Length / 8];
                outputBits.CopyTo(outputBitArray, 0);
                byteStream.AddRange(outputBitArray);

                byteStream.AddRange(BitConverter.GetBytes(CurrentSpeed));
                byteStream.AddRange(BitConverter.GetBytes(BarcodeValue1));
                byteStream.AddRange(BitConverter.GetBytes(BarcodeValue2));
                byteStream.AddRange(BitConverter.GetBytes(CurrentNodeNo));
                byteStream.AddRange(BitConverter.GetBytes(NextNodeNo));
                byteStream.AddRange(BitConverter.GetBytes(TargetNodeNo));

                if (CurrentCommandID.Length > 64)
                {
                    CurrentCommandID = CurrentCommandID.Substring(0, 64);
                }
                CurrentCommandID = CurrentCommandID.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(CurrentCommandID));
                byteStream.Add(ETX);

                SetLengthBytes(ref byteStream);

                return byteStream.ToArray();
            }
            catch
            {
                return null;
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
                result.Add(string.Format("\t\tIn-Rail Status : {0}", InRailStatus));
                result.Add(string.Format("\t\tBusy Status : {0}", BusyStatus));
                result.Add(string.Format("\t\tPower On Status : {0}", PowerOnStatus));
                result.Add(string.Format("\t\tAuto Run Status : {0}", AutoRunStatus));
                result.Add(string.Format("\t\tPause Status : {0}", PauseStatus));
                result.Add(string.Format("\t\tError Status : {0}", ErrorStatus));
                result.Add(string.Format("\t\tMoving Status : {0}", MovingStatus));
                //result.Add(string.Format("\t\tAuto Teaching Status : {0}", AutoTeachingStatus));
                result.Add(string.Format("\t\tCarrier Exist Status : {0}", CarrierExistStatus));

                StringBuilder inputStatus = new StringBuilder(string.Format("\t\tInput Status : "));
                StringBuilder outputStatus = new StringBuilder(string.Format("\t\tOutput Status : "));
                for (int i = 0; i < InputStatus.Length; i++)
                {
                    inputStatus.Append((InputStatus[i] == true) ? "1" : "0");
                    outputStatus.Append((OutputStatus[i] == true) ? "1" : "0");
                }
                result.Add(inputStatus.ToString());
                result.Add(outputStatus.ToString());
                result.Add(string.Format("\t\tCurrent Speed : {0}", CurrentSpeed));
                result.Add(string.Format("\t\tBarcode #1 Value : {0}", BarcodeValue1));
                result.Add(string.Format("\t\tBarcode #2 Value : {0}", BarcodeValue2));
                result.Add(string.Format("\t\tCurrent Node No.: {0}", CurrentNodeNo));
                result.Add(string.Format("\t\tNext Node No.: {0}", NextNodeNo));
                result.Add(string.Format("\t\tTarget Node No : {0}", TargetNodeNo));
                result.Add(string.Format("\t\tCurrent Command ID: {0}", CurrentCommandID));


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
