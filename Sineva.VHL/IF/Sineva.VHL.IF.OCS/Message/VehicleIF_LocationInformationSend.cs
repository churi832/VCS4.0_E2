using System;
using System.Collections.Generic;
using System.Text;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.OCS
{
    public class VehicleIF_LocationInformationSend : VehicleIF, IVehicleIF
    {
        #region Fields
        public LocationInformationChangeCode ChangeCode = LocationInformationChangeCode.Add;
        public string Version = string.Empty;
        public int LocationNo = 0;
        public double HoistPosition = 0;
        public double SlidePosition = 0;
        public double RotatePosition = 0;
        public double UnloadHoistPosition = 0;
        public double UnloadSlidePoistion = 0;
        public double UnloadRotatePosition = 0;
        public PortType TypeOfPort = PortType.LeftBuffer;
        public int LinkID = 0;
        public int NodeID = 0;
        public double BarcodeLeft = 0;
        public double BarcodeRight = 0;
        public int PIOID = 0;
        public int PIOCH = 0;
        public bool PIOUsed = false;
        public bool PortProhibition = false;
        public bool OffsetUsed = false;
        public bool PBSUsed = false;
        public int PBSSelectNumber = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_LocationInformationSend()
        {
            try
            {
                _MessageCode = IFMessage.LocationInformationSend;
            }
            catch
            {
            }
        }
        public VehicleIF_LocationInformationSend(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.LocationInformationSend;
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
                ChangeCode = (LocationInformationChangeCode)stream[offset++];
                Version = Encoding.UTF8.GetString(stream, offset, 32).Trim(); offset += 32;
                LocationNo = (int)BitConverter.ToInt32(stream, offset); offset += 4;
                HoistPosition = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                SlidePosition = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                RotatePosition = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                UnloadHoistPosition = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                UnloadSlidePoistion = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                UnloadRotatePosition = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                TypeOfPort = (PortType)stream[offset++];
                LinkID = (int)BitConverter.ToInt32(stream, offset); offset += 4;
                NodeID = (int)BitConverter.ToInt32(stream, offset); offset += 4;
                BarcodeLeft = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                BarcodeRight = (double)BitConverter.ToDouble(stream, offset); offset += 8;
                PIOID = (int)BitConverter.ToInt32(stream, offset); offset += 4;
                PIOCH = (int)BitConverter.ToInt32(stream, offset); offset += 4;
                PIOUsed = Convert.ToBoolean(stream[offset++]);
                PortProhibition = Convert.ToBoolean(stream[offset++]);
                OffsetUsed = Convert.ToBoolean(stream[offset++]);
                PBSUsed = Convert.ToBoolean(stream[offset++]);
                PBSSelectNumber = (int)BitConverter.ToInt32(stream, offset); offset += 4;

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
                byteStream.Add((byte)ChangeCode);

                if (Version.Length > 64)
                {
                    Version = Version.Substring(0, 64);
                }
                Version = Version.Trim().PadRight(64, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(Version));

                byteStream.AddRange(BitConverter.GetBytes(LocationNo));
                byteStream.AddRange(BitConverter.GetBytes(HoistPosition));
                byteStream.AddRange(BitConverter.GetBytes(SlidePosition));
                byteStream.AddRange(BitConverter.GetBytes(RotatePosition));
                byteStream.AddRange(BitConverter.GetBytes(UnloadHoistPosition));
                byteStream.AddRange(BitConverter.GetBytes(UnloadSlidePoistion));
                byteStream.AddRange(BitConverter.GetBytes(UnloadRotatePosition));
                byteStream.Add((byte)TypeOfPort);
                byteStream.AddRange(BitConverter.GetBytes(LinkID));
                byteStream.AddRange(BitConverter.GetBytes(NodeID));
                byteStream.AddRange(BitConverter.GetBytes(BarcodeLeft));
                byteStream.AddRange(BitConverter.GetBytes(BarcodeRight));
                byteStream.AddRange(BitConverter.GetBytes(PIOID));
                byteStream.AddRange(BitConverter.GetBytes(PIOCH));
                byteStream.Add(Convert.ToByte(PIOUsed));
                byteStream.Add(Convert.ToByte(PortProhibition));
                byteStream.Add(Convert.ToByte(OffsetUsed));
                byteStream.Add(Convert.ToByte(PBSUsed));
                byteStream.AddRange(BitConverter.GetBytes(PBSSelectNumber));

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
                result.Add(string.Format("\t\tChange Code : {0}", ChangeCode));
                result.Add(string.Format("\t\tVersion : {0}", Version.Trim('\0')));
                result.Add(string.Format("\t\tLocation Number : {0}", LocationNo));
                result.Add(string.Format("\t\tHoist Position : {0}", HoistPosition));
                result.Add(string.Format("\t\tSlide Position : {0}", SlidePosition));
                result.Add(string.Format("\t\tRotate Position : {0}", RotatePosition));
                result.Add(string.Format("\t\tUnload Hoist Position : {0}", UnloadHoistPosition));
                result.Add(string.Format("\t\tUnload Slide Position : {0}", UnloadSlidePoistion));
                result.Add(string.Format("\t\tUnload Rotate Position : {0}", UnloadRotatePosition));
                result.Add(string.Format("\t\tPort Type : {0}", TypeOfPort));
                result.Add(string.Format("\t\tLink ID : {0}", LinkID));
                result.Add(string.Format("\t\tNode ID : {0}", NodeID));
                result.Add(string.Format("\t\tBarcode Left : {0}", BarcodeLeft));
                result.Add(string.Format("\t\tBarcode Right : {0}", BarcodeRight));
                result.Add(string.Format("\t\tPIO ID : {0}", PIOID));
                result.Add(string.Format("\t\tPIO CH : {0}", PIOCH));
                result.Add(string.Format("\t\tPIO Used : {0}", PIOUsed));
                result.Add(string.Format("\t\tPort Prohibition : {0}", PortProhibition));
                result.Add(string.Format("\t\tOffset Used : {0}", OffsetUsed));
                result.Add(string.Format("\t\tPBS Used : {0}", PBSUsed));
                result.Add(string.Format("\t\tPBS Select Number : {0}", PBSSelectNumber));

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
