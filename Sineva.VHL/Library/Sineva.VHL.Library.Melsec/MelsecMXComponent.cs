using ActUtlTypeLib;
using Sineva.VHL.Library.Common;
using System;

namespace Sineva.VHL.Library.Melsec
{
    public class MelsecMXComponent : IMelsecConnection
    {
        #region Fields
        private ActUtlType _Connection;
        private static object _LockKey = new object();

        private bool _IsOpened = false;
        private byte[] _Buffer = new byte[2048];
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public MelsecMXComponent(int logicalStationNumber)
        {
            try
            {
                _Connection = new ActUtlType();
                _Connection.ActLogicalStationNumber = logicalStationNumber;
            }
            catch (Exception ex)
            {
            }
        }
        ~MelsecMXComponent()
        {
            Close();
        }
        #endregion

        #region Methods
        public bool Open()
        {
            //일단 연결 요청을 한다.
            int result = _Connection.Open();

            if (result == 0)
            {
                _IsOpened = true;
            }
            else
            {
                _IsOpened = false;
            }

            return (result == 0);
        }

        public bool Close()
        {
            try
            {
                _Connection.Close();

                _IsOpened = false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool GetOpenStatus()
        {
            return _IsOpened;
        }

        /// <summary>
        /// Block Read Device.
        /// </summary>
        /// <param name="type">Type of device to access</param>
        /// <param name="index">Device index</param>
        /// <param name="wordSize">Byte size to read</param>
        /// <param name="readData">Read data</param>
        /// <returns></returns>
        public int BlockReadDevice(MelsecDeviceType type, int index, ref short wordSize, ref short[] readData)
        {
            try
            {
                string address = MelsecInformationConverter.Instance.GetAddressString(type, index);

                short[] temp = new short[wordSize];
                int result = _Connection.ReadDeviceBlock2(address, wordSize, out temp[0]);

                if (result == 0)
                {
                    Array.Copy(temp, readData, temp.Length);
                }

                return result;
            }
            catch (Exception ex)
            {
                Close();
                return 1;
            }
        }

        /// <summary>
        /// Block Write Device
        /// </summary>
        /// <param name="type">Device type to access</param>
        /// <param name="index">Device index</param>
        /// <param name="wordSize">Byte size to write</param>
        /// <param name="writeData">Write data</param>
        /// <returns></returns>
        public int BlockWriteDevice(MelsecDeviceType type, int index, ref short wordSize, ref short[] writeData)
        {
            try
            {
                string address = MelsecInformationConverter.Instance.GetAddressString(type, index);

                short[] temp = new short[wordSize];
                Array.Copy(writeData, temp, wordSize);
                int result = _Connection.WriteDeviceBlock2(address, wordSize, ref temp[0]);

                if (result == 0)
                {
                    Array.Copy(temp, writeData, temp.Length);
                }

                return result;
            }
            catch (Exception ex)
            {
                Close();
                return 1;
            }
        }

        public int RandomWriteDevice(string[] devices, ref short[] writeData)
        {
            try
            {
                string address = string.Empty;
                for (int i = 0; i < devices.Length; i++)
                {
                    address += devices[i];
                    if (i < (devices.Length - 1))
                    {
                        address += "\n";
                    }
                }

                short[] temp = new short[writeData.Length];
                Array.Copy(writeData, temp, writeData.Length);
                int result = _Connection.WriteDeviceRandom2(address, writeData.Length, ref temp[0]);

                if (result == 0)
                {
                    Array.Copy(temp, writeData, temp.Length);
                }

                return result;
            }
            catch
            {
                Close();
                return -1;
            }
        }
        public int SetDevice(string address, short writeData)
        {
            try
            {
                int result = _Connection.SetDevice2(address, writeData);
                return result;
            }
            catch
            {
                Close();
                return -1;
            }
        }
        #endregion
    }
}
