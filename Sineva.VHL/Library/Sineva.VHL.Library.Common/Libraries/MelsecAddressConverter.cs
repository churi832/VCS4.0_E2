using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Library.Common
{
    public class MelsecInformationConverter : Singleton<MelsecInformationConverter>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        private MelsecInformationConverter()
        {
        }
        #endregion

        #region Methods
        public byte GetDeviceCode(MelsecDeviceType type)
        {
            if (type == MelsecDeviceType.X) return 0x9C;
            else if (type == MelsecDeviceType.Y) return 0x9D;
            else if (type == MelsecDeviceType.M) return 0x90;
            else if (type == MelsecDeviceType.L) return 0x92;
            else if (type == MelsecDeviceType.B) return 0xA0;
            else if (type == MelsecDeviceType.F) return 0x93;
            else if (type == MelsecDeviceType.D) return 0xA8;
            else if (type == MelsecDeviceType.W) return 0xB4;
            else if (type == MelsecDeviceType.R) return 0xAF;
            else if (type == MelsecDeviceType.ZR) return 0xB0;
            else return 0x00;
        }
        public MelsecDeviceType GetDeviceType(string address)
        {
            try
            {
                MelsecDeviceType result = MelsecDeviceType.None;
                if (address.ToUpper().StartsWith("ZR") == true)
                {
                    result = MelsecDeviceType.ZR;
                }
                else
                {
                    string device = address.Substring(0, 1);
                    Enum.TryParse<MelsecDeviceType>(device, out result);
                }
                return result;
            }
            catch
            {
                return MelsecDeviceType.None;
            }
        }
        public int GetDeviceOffset(string address)
        {
            try
            {
                int result = -1;
                string offset = string.Empty;
                if (address.ToUpper().StartsWith("ZR") == true)
                {
                    offset = address.Substring(2, address.Length - 2);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[0]);
                    }
                    else
                    {
                        result = Convert.ToInt32(offset);
                    }
                }
                else if ((address.ToUpper().StartsWith("X") == true) || (address.ToUpper().StartsWith("Y") == true) ||
                    (address.ToUpper().StartsWith("B") == true) || (address.ToUpper().StartsWith("W") == true))
                {
                    offset = address.Substring(1, address.Length - 1);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[0], 16);
                    }
                    else
                    {
                        result = Convert.ToInt32(offset, 16);
                    }
                }
                else
                {
                    offset = address.Substring(1, address.Length - 1);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[0]);
                    }
                    else
                    {
                        result = Convert.ToInt32(offset);
                    }
                }
                return result;
            }
            catch
            {
                return -1;
            }
        }
        public int GetDeviceBitOffset(string address)
        {
            try
            {
                int result = -1;
                string offset = string.Empty;
                if (address.ToUpper().StartsWith("ZR") == true)
                {
                    offset = address.Substring(2, address.Length - 2);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[1], 16);
                    }
                }
                else if ((address.ToUpper().StartsWith("X") == true) || (address.ToUpper().StartsWith("Y") == true) ||
                    (address.ToUpper().StartsWith("B") == true) || (address.ToUpper().StartsWith("W") == true))
                {
                    offset = address.Substring(1, address.Length - 1);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[1], 16);
                    }
                }
                else
                {
                    offset = address.Substring(1, address.Length - 1);

                    if (offset.IndexOf('.') >= 0)
                    {
                        string[] splitOffset = offset.Split('.');
                        result = Convert.ToInt32(splitOffset[1], 16);
                    }
                }

                return result;
            }
            catch
            {
                return -1;
            }
        }
        public string GetAddressString(MelsecDeviceType type, int offset)
        {
            try
            {
                string result = string.Empty;

                if ((type == MelsecDeviceType.B) || (type == MelsecDeviceType.W) ||
                    (type == MelsecDeviceType.X) || (type == MelsecDeviceType.Y))
                {
                    //Address type is Hexa-Decimal
                    result = string.Format("{0}{1:X}", type.ToString(), offset);
                }
                else
                {
                    //Address type is Decimal
                    result = string.Format("{0}{1}", type.ToString(), offset);
                }

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        public string GetAddressString(MelsecDeviceType type, int offset, int bitOffset)
        {
            try
            {
                string result = GetAddressString(type, offset);
                if (bitOffset >= 0)
                {
                    result += string.Format(".{0:X}", bitOffset);
                }

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        public string GetTableName(MelsecTableType type)
        {
            try
            {
                string result = string.Empty;
                switch (type)
                {
                    case MelsecTableType.JCU:
                        {
                            result = "JCUIOList";
                        }
                        break;
                    case MelsecTableType.MTL:
                        {
                            result = "MTLIOList";
                        }
                        break;
                    case MelsecTableType.VHL:
                        {
                            result = "VehicleIOList";
                        }
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
