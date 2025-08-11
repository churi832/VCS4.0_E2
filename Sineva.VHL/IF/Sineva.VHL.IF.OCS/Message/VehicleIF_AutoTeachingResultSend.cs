using System;
using System.Collections.Generic;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_AutoTeachingResultSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public int PortNumber = 0;
        public VehicleOperationResult Result = VehicleOperationResult.Success;
        public int BarcodeValue1 = 0;
        public int BarcodeValue2 = 0;
        public double SlideValue = 0.0f;
        public double RotateValue = 0.0f;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_AutoTeachingResultSend()
        {
            try
            {
                _MessageCode = IFMessage.AutoTeachingResultSend;
            }
            catch
            {
            }
        }
        public VehicleIF_AutoTeachingResultSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.AutoTeachingResultSend;
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
                PortNumber = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                Result = (VehicleOperationResult)stream[offset++];
                BarcodeValue1 = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                BarcodeValue2 = (int)BitConverter.ToUInt32(stream, offset); offset += 4;
                SlideValue = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                RotateValue = (double)BitConverter.ToDouble(stream, offset); offset += 8;

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
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(PortNumber)));
                byteStream.Add((byte)Result);
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(BarcodeValue1)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt32(BarcodeValue2)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToDouble(SlideValue)));
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToDouble(RotateValue)));
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
                result.Add(string.Format("\t\tPort Number : {0}", PortNumber));
                result.Add(string.Format("\t\tResult : {0}", Result));
                result.Add(string.Format("\t\tBarcode Value #1 : {0}", BarcodeValue1));
                result.Add(string.Format("\t\tBarcode Value #2 : {0}", BarcodeValue2));
                result.Add(string.Format("\t\tSlide Value : {0}", SlideValue));
                result.Add(string.Format("\t\tRotate Value : {0}", RotateValue));

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
