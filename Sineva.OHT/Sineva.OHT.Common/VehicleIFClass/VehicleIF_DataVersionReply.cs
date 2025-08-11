using System;
using System.Collections.Generic;
using System.Text;

namespace Sineva.OHT.Common
{
    public class VehicleIF_DataVersionReply : VehicleIF, IVehicleIF
    {
        #region Fields
        public string NodeDataVersion = string.Empty;
        public string LinkDataVersion = string.Empty;
        public string PortDataVersion = string.Empty;
        public string PIODeviceDataVersion = string.Empty;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public VehicleIF_DataVersionReply()
        {
            try
            {
                _MessageCode = IFMessage.DataVersionReply;
            }
            catch
            {
            }
        }
        public VehicleIF_DataVersionReply(int systemByte)
        {
            try
            {
                _MessageCode = IFMessage.DataVersionReply;
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
                VehicleNumber = Convert.ToInt32(BitConverter.ToUInt16(stream, offset)); offset += 2;
                NodeDataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;
                LinkDataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;
                PortDataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;
                PIODeviceDataVersion = Encoding.UTF8.GetString(stream, offset, 20).Trim('\0', ' '); offset += 20;

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
                //_MessageLength = 68;
                //byte[] stream = new byte[MessageLength];

                //stream[0] = STX;
                //Array.Copy(GetHeader(), 0, stream, 1, 4);

                //int offset = 5;

                //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)), 0, stream, offset, 2); offset += 2;

                //if (NodeDataVersion.Length > 20)
                //{
                //    NodeDataVersion = NodeDataVersion.Substring(0, 20);
                //}
                //NodeDataVersion = NodeDataVersion.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(NodeDataVersion), 0, stream, offset, 20); offset += 20;

                //if (LinkDataVersion.Length > 20)
                //{
                //    LinkDataVersion = LinkDataVersion.Substring(0, 20);
                //}
                //LinkDataVersion = LinkDataVersion.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(LinkDataVersion), 0, stream, offset, 20); offset += 20;

                //if (PortDataVersion.Length > 20)
                //{
                //    PortDataVersion = PortDataVersion.Substring(0, 20);
                //}
                //PortDataVersion = PortDataVersion.Trim().PadRight(20, ' ');
                //Array.Copy(Encoding.UTF8.GetBytes(PortDataVersion), 0, stream, offset, 20); offset += 20;

                //stream[MessageLength - 1] = ETX;

                //return stream;

                List<byte> byteStream = new List<byte>();

                byteStream.Add(STX);
                byteStream.AddRange(GetHeader());
                byteStream.AddRange(BitConverter.GetBytes(Convert.ToUInt16(VehicleNumber)));

                if (NodeDataVersion.Length > 20)
                {
                    NodeDataVersion = NodeDataVersion.Substring(0, 20);
                }
                NodeDataVersion = NodeDataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(NodeDataVersion));

                if (LinkDataVersion.Length > 20)
                {
                    LinkDataVersion = LinkDataVersion.Substring(0, 20);
                }
                LinkDataVersion = LinkDataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(LinkDataVersion));

                if (PortDataVersion.Length > 20)
                {
                    PortDataVersion = PortDataVersion.Substring(0, 20);
                }
                PortDataVersion = PortDataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(PortDataVersion));

                if (PIODeviceDataVersion.Length > 20)
                {
                    PIODeviceDataVersion = PIODeviceDataVersion.Substring(0, 20);
                }
                PIODeviceDataVersion = PIODeviceDataVersion.Trim().PadRight(20, '\0');
                byteStream.AddRange(Encoding.UTF8.GetBytes(PIODeviceDataVersion));

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
                result.Add(string.Format("\t\tNode Data Version : {0}", NodeDataVersion));
                result.Add(string.Format("\t\tLink Data Version : {0}", LinkDataVersion));
                result.Add(string.Format("\t\tPort Data Version : {0}", PortDataVersion));
                result.Add(string.Format("\t\tPIO Device Data Version : {0}", PIODeviceDataVersion));

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
