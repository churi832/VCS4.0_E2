using Sineva.VHL.Library.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Sineva.VHL.Library.Melsec
{
    public class MelsecMCProtocol : IMelsecConnection
    {
        #region Fields
        private static object _LockKey = new object();
        private IPAddress _RemoteIP;
        private IPEndPoint _IPEndPoint;
        private EndPoint _RemotePoint;
        private IPEndPoint _IPLocalPoint;
        private EndPoint _LocalPoint;
        private Socket _Socket;
        private string _IpAddress;
        private ushort _PortNo;
        private ProtocolType _ProtocolType = ProtocolType.Tcp;

        private bool _IsOpened = false;
        private byte[] _Buffer = new byte[2048];
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public MelsecMCProtocol(string ipAddress, ushort portNo, ProtocolType protocol)
        {
            try
            {
                _IpAddress = ipAddress;
                _PortNo = portNo;
                _ProtocolType = protocol;
            }
            catch
            {
            }
        }
        ~MelsecMCProtocol()
        {
            Close();
        }
        #endregion

        #region Methods
        public bool Open()
        {
            //일단 연결 요청을 한다.
            Connect();

            //연결여부를 return
            //연결여부와 상관없이 일단 ok 해야 연결이 안되어있더라도 HMI를 실행할 수 있다..
            //단, 동작에 대한 interlock은 연결상태를 봐야 한다.
            //return m_IsOpened;
            return true;
        }

        public bool Close()
        {
            try
            {
                if (_Socket != null)
                {
                    _Socket.Close();
                    _Socket = null;
                }
                _IPEndPoint = null;
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
        public void Connect()
        {
            lock (_LockKey)
            {
                try
                {
                    if (_ProtocolType == ProtocolType.Tcp)
                    {
                        _RemoteIP = IPAddress.Parse(_IpAddress);
                        _IPEndPoint = new IPEndPoint(_RemoteIP, (int)_PortNo);

                        _Socket = new Socket(_RemoteIP.AddressFamily, SocketType.Stream, _ProtocolType);
                        _Socket.Connect(_IPEndPoint);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)1000);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)1000);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, 1);
                    }
                    else if (_ProtocolType == ProtocolType.Udp)
                    {
                        _RemoteIP = IPAddress.Parse(_IpAddress);
                        _IPEndPoint = new IPEndPoint(_RemoteIP, (int)_PortNo);
                        _RemotePoint = (EndPoint)_IPEndPoint;

                        _IPLocalPoint = new IPEndPoint(IPAddress.Any, (int)_PortNo);
                        _LocalPoint = (EndPoint)_IPLocalPoint;

                        _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, _ProtocolType);
                        _Socket.Connect(_RemotePoint);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)1000);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)1000);
                        _Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, 1);
                    }

                    _IsOpened = true;
                }
                catch (Exception ex)
                {
                    _Socket = null;
                    _IsOpened = false;
                }
            }
        }

        /// <summary>
        /// Create EtherNet TCP Message Header for Block Access
        /// </summary>
        /// <param name="cmdType">Read or Write</param>
        /// <param name="devType">Melsec device type to access</param>
        /// <param name="index">Device index</param>
        /// <param name="byteSize">Byte size of read or write device</param>
        /// <param name="message">Made byte array of command message header
        /// message[0]~message[1] = 0x50, 0x00 : Command (Response의 경우 0xD0, 0x00)
        /// message[2] = 0x00 : 네트워크 번호 (Q시리즈 E71 장착국(자국)의 경우 0x00)
        /// message[3] = 0xFF : PLC 번호 (Q시리즈 E71 장착국(자국)의 경우 0xFF : 0xFF는 네트워크 번호가 0x00일때만 유효)
        /// message[4]~message[5] = 0xFF, 0x03 : 요구상대 모듈 I/O 번호 (멀티 CPU 시스템의 PLC CPU가 아닌 경우 고정)
        /// message[6] = 0x00 : 요구상대 모듈 국번호 (멀티 CPU 시스템의 PLC CPU가 아닌 경우 고정)
        /// message[7] = variable : Message 길이의 Lower byte (header[9] 부터 Message 끝까지의 byte 수)
        /// message[8] = variable : Message 길이의 Upper byte (header[9] 부터 Message 끝까지의 byte 수)
        /// message[9] = variable : CPU 감시 타이머의 Lower byte
        ///                        (Q시리즈E71이 PLC CPU로 읽기/쓰기요구를 출력 후 응답이 올때 까지의 대기 시간, 0이면 무한대기)
        /// message[10] = variable : CPU 감시 타이머의 Upper byte
        ///                        (Q시리즈E71이 PLC CPU로 읽기/쓰기요구를 출력 후 응답이 올때 까지의 대기 시간, 0이면 무한대기)
        /// message[11]~message[12] = variable : Command (0x01, 0x04 : Block Read), (0x01, 0x14 : Block Write)
        /// message[13]~message[14] = variable : Sub-Command (0x00, 0x00 : Word 단위), (0x01, 0x00 : Bit 단위)
        /// message[15]~message[17] = variable : Access할 Device의 시작 주소
        /// message[18] = variable : Access할 Device의 Type ==> GetDeviceCode() Method 참조
        /// message[19]~message[20] = variable : Access할 Device 수의 Word Size
        /// </param>
        /// <returns></returns>
        private int BuildMessageHeader_BlockAccess(MelsecCommandType cmdType, MelsecDeviceType devType, int index, short byteSize, ref byte[] message)
        {
            int wordSize = (int)(byteSize / 2) + (int)((byteSize % 2) > 0 ? 1 : 0);

            message[0] = 0x50;
            message[1] = 0x00;
            message[2] = 0x00;
            message[3] = 0xFF;
            message[4] = 0xFF;
            message[5] = 0x03;
            message[6] = 0x00;

            if (cmdType == MelsecCommandType.Read)
            {
                message[7] = 0x0C;
                message[8] = 0x00;
                message[11] = 0x01;
                message[12] = 0x04;
            }
            else
            {
                int length = byteSize + 12;
                message[7] = (byte)(length % 0x100);
                message[8] = (byte)(length / 0x100);
                message[11] = 0x01;
                message[12] = 0x14;
            }

            message[9] = 0x10;
            message[10] = 0x00;
            message[13] = (byte)((ushort)MelsecAccessType.WORD % 0x100);
            message[14] = (byte)((ushort)MelsecAccessType.WORD / 0x100); ;

            message[15] = (byte)(index & 0xFF);
            message[16] = (byte)((index >> 8) & 0xFF);
            message[17] = (byte)((index >> 16) & 0xFF);

            message[18] = MelsecInformationConverter.Instance.GetDeviceCode(devType);
            message[19] = (byte)(wordSize % 0x100);
            message[20] = (byte)(wordSize / 0x100);

            return 0;
        }

        /// <summary>
        /// Create EtherNet TCP Message Header for Random Access
        /// </summary>
        /// <param name="cmdType">Read or Write</param>
        /// <param name="devType">Melsec device type to access</param>
        /// <param name="index">Device index</param>
        /// <param name="byteSize">Byte size of read or write device</param>
        /// <param name="message">Made byte array of command message header
        /// message[0]~message[1] = 0x50, 0x00 : Command (Response의 경우 0xD0, 0x00)
        /// message[2] = 0x00 : 네트워크 번호 (Q시리즈 E71 장착국(자국)의 경우 0x00)
        /// message[3] = 0xFF : PLC 번호 (Q시리즈 E71 장착국(자국)의 경우 0xFF : 0xFF는 네트워크 번호가 0x00일때만 유효)
        /// message[4]~message[5] = 0xFF, 0x03 : 요구상대 모듈 I/O 번호 (멀티 CPU 시스템의 PLC CPU가 아닌 경우 고정)
        /// message[6] = 0x00 : 요구상대 모듈 국번호 (멀티 CPU 시스템의 PLC CPU가 아닌 경우 고정)
        /// message[7] = variable : Message 길이의 Lower byte (header[9] 부터 Message 끝까지의 byte 수)
        /// message[8] = variable : Message 길이의 Upper byte (header[9] 부터 Message 끝까지의 byte 수)
        /// message[9] = variable : CPU 감시 타이머의 Lower byte
        ///                        (Q시리즈E71이 PLC CPU로 읽기/쓰기요구를 출력 후 응답이 올때 까지의 대기 시간, 0이면 무한대기)
        /// message[10] = variable : CPU 감시 타이머의 Upper byte
        ///                        (Q시리즈E71이 PLC CPU로 읽기/쓰기요구를 출력 후 응답이 올때 까지의 대기 시간, 0이면 무한대기)
        /// message[11]~message[12] = variable : Command (0x03, 0x04 : Random Read), (0x02, 0x14 : Random Write)
        /// message[13]~message[14] = variable : Sub-Command (0x00, 0x00 : Word 단위), (0x01, 0x00 : Bit 단위)
        /// 
        /// 1. case of Random Bit Write
        ///   message[15] = variable : Access Bit Count
        ///
        /// 2. case of Random Word Write
        ///   message[15] = variable : Access Word Count
        ///   message[16] = variable : Access Double-Word Count
        /// </param>
        /// <returns></returns>
        private int BuildMessageHeader_RandomWrite(MelsecAccessType accessType, byte deviceCount, ref byte[] message)
        {
            ushort msgLength = 0;

            if (accessType == MelsecAccessType.BIT)
            {
                msgLength = (ushort)(deviceCount * 5 + 7);
            }
            else
            {
                msgLength = (ushort)(deviceCount * 6 + 8);
            }

            message[0] = 0x50;
            message[1] = 0x00;
            message[2] = 0x00;
            message[3] = 0xFF;
            message[4] = 0xFF;
            message[5] = 0x03;
            message[6] = 0x00;
            message[7] = (byte)(msgLength % 0x100);
            message[8] = (byte)(msgLength / 0x100);
            message[9] = 0x10;
            message[10] = 0x00;
            message[11] = 0x02;
            message[12] = 0x14;
            message[13] = (byte)((ushort)accessType % 0x100);
            message[14] = (byte)((ushort)accessType / 0x100);

            if (accessType == MelsecAccessType.BIT)
            {
                message[15] = deviceCount;
            }
            else
            {
                message[15] = deviceCount;
                message[16] = 0x00;  //일단 double-word는 write하지 않는 것으로 사용...
            }

            return 0;
        }

        private int BuildMessageHeader_RandomRead(byte deviceCount, ref byte[] message)
        {
            ushort msgLength = 0;
            msgLength = (ushort)(deviceCount * 4 + 8);

            message[0] = 0x50;
            message[1] = 0x00;
            message[2] = 0x00;
            message[3] = 0xFF;
            message[4] = 0xFF;
            message[5] = 0x03;
            message[6] = 0x00;
            message[7] = (byte)(msgLength % 0x100);
            message[8] = (byte)(msgLength / 0x100);
            message[9] = 0x10;
            message[10] = 0x00;
            message[11] = 0x03;
            message[12] = 0x04;
            message[13] = (byte)((ushort)MelsecAccessType.WORD % 0x100);  //RandomRead는 Word만 가능함
            message[14] = (byte)((ushort)MelsecAccessType.WORD / 0x100);
            message[15] = deviceCount;
            message[16] = 0x00;  //일단 double-word는 write하지 않는 것으로 사용...

            return 0;
        }

        /// <summary>
        /// Block Read Device. (Max 960 words)
        /// </summary>
        /// <param name="type">Type of device to access</param>
        /// <param name="index">Device index</param>
        /// <param name="byteSize">Byte size to read</param>
        /// <param name="readData">Read data</param>
        /// <returns></returns>
        public int BlockReadDevice(MelsecDeviceType type, int index, ref short byteSize, ref short[] readData)
        {
            try
            {
                short size = (short)(byteSize + byteSize % 2);
                byte[] message = new byte[21];
                int rv = BuildMessageHeader_BlockAccess(MelsecCommandType.Read, type, index, size, ref message);

                if (_ProtocolType == ProtocolType.Tcp)
                {
                    _Socket.Send(message, 0, message.Length, SocketFlags.None);
                    rv = _Socket.Receive(_Buffer, 0, _Buffer.Length, SocketFlags.None);
                }
                else if (_ProtocolType == ProtocolType.Udp)
                {
                    _Socket.SendTo(message, message.Length, SocketFlags.None, _RemotePoint);
                    rv = _Socket.ReceiveFrom(_Buffer, _Buffer.Length, SocketFlags.None, ref _LocalPoint);
                }

                if (rv != 0)
                {
                    short error = (short)(_Buffer[9] + _Buffer[10] * 0x100);

                    if ((error == 0) && ((rv - 11) == size))
                    {
                        int count = (size / 2);
                        for (int i = 0; i < count; i++)
                        {
                            readData[i] = (short)(_Buffer[i * 2 + 11] + (_Buffer[i * 2 + 12] * 0x100));
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }

                return 0;
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
        /// <param name="byteSize">Byte size to write</param>
        /// <param name="writeData">Write data</param>
        /// <returns></returns>
        public int BlockWriteDevice(MelsecDeviceType type, int index, ref short byteSize, ref short[] writeData)
        {
            try
            {
                short size = (short)(byteSize + (byteSize % 2));
                byte[] message = new byte[21 + size];
                int rv = BuildMessageHeader_BlockAccess(MelsecCommandType.Write, type, index, size, ref message);

                int count = (size / 2);
                for (int i = 0; i < count; i++)
                {
                    message[i * 2 + 21] = (byte)((ushort)writeData[i] % 0x100);
                    message[i * 2 + 22] = (byte)((ushort)writeData[i] / 0x100);
                }

                if (_ProtocolType == ProtocolType.Tcp)
                {
                    _Socket.Send(message, 0, message.Length, SocketFlags.None);
                    rv = _Socket.Receive(_Buffer, 0, _Buffer.Length, SocketFlags.None);
                }
                else if (_ProtocolType == ProtocolType.Udp)
                {
                    _Socket.SendTo(message, message.Length, SocketFlags.None, _RemotePoint);
                    rv = _Socket.ReceiveFrom(_Buffer, _Buffer.Length, SocketFlags.None, ref _LocalPoint);
                }

                if (rv != 0)
                {
                    short error = (short)(_Buffer[9] + _Buffer[10] * 0xFF);
                    return error;
                }

                return 0;
            }
            catch
            {
                Close();
                return 1;
            }
        }

        /// <summary>
        /// Random write bit device. (Max 188 bits)
        /// </summary>
        /// <param name="type">Device types to access</param>
        /// <param name="indexes">Device indexes</param>
        /// <param name="size">Byte size of each device index</param>
        /// <param name="writeData">Write data</param>
        /// <returns></returns>
        public int RandomWriteBitDevice(MelsecDeviceType[] type, int[] indexes, short[] size, short[] writeData)
        {
            try
            {
                if ((type.Length != indexes.Length) || (type.Length != writeData.Length) ||
                    (indexes.Length != writeData.Length))
                {
                    return 100; //array의 길이가 다를 때...
                }

                int msgLength = 16 + (int)(type.Length * 5);
                byte[] message = new byte[msgLength];

                int rv = BuildMessageHeader_RandomWrite(MelsecAccessType.BIT, (byte)type.Length, ref message);

                int count = type.Length;

                for (int i = 0; i < count; i++)
                {
                    int startIndex = (i * 5) + 16;

                    message[startIndex + 0] = (byte)((ushort)(indexes[i] % 0x100));
                    message[startIndex + 1] = (byte)((ushort)(indexes[i] / 0x100));
                    message[startIndex + 2] = 0x00;
                    message[startIndex + 3] = MelsecInformationConverter.Instance.GetDeviceCode(type[i]);
                    message[startIndex + 4] = (byte)((ushort)(writeData[i] % 0x100));
                }

                if (_ProtocolType == ProtocolType.Tcp)
                {
                    _Socket.Send(message, 0, message.Length, SocketFlags.None);
                    rv = _Socket.Receive(_Buffer, 0, _Buffer.Length, SocketFlags.None);
                }
                else if (_ProtocolType == ProtocolType.Udp)
                {
                    _Socket.SendTo(message, message.Length, SocketFlags.None, _RemotePoint);
                    rv = _Socket.ReceiveFrom(_Buffer, _Buffer.Length, SocketFlags.None, ref _LocalPoint);
                }

                if (rv != 0)
                {
                    ushort error = (ushort)(_Buffer[9] + _Buffer[10] * 0xFF);
                    return (short)error;
                }

                return 0;
            }
            catch//(Exception err)
            {
                Close();
                return 1;
            }
        }

        /// <summary>
        /// Random write word device. (Max 120 words)
        /// </summary>
        /// <param name="type">Device types to access</param>
        /// <param name="indexes">Device indexes</param>
        /// <param name="size">Byte size of each device index</param>
        /// <param name="writeData">Write data</param>
        /// <returns></returns>
        public int RandomWriteWordDevice(MelsecDeviceType[] type, int[] indexes, short[] size, short[] writeData)
        {
            try
            {
                if ((type.Length != indexes.Length) || (type.Length != writeData.Length) ||
                    (indexes.Length != writeData.Length))
                {
                    return 100; //array의 길이가 다를 때...
                }

                int msgLength = 17 + (int)(type.Length * 6);
                byte[] message = new byte[msgLength];

                int rv = BuildMessageHeader_RandomWrite(MelsecAccessType.WORD, (byte)type.Length, ref message);

                int count = type.Length;

                for (int i = 0; i < count; i++)
                {
                    int startIndex = 17 + (i * 6);

                    message[startIndex + 0] = (byte)((ushort)(indexes[i] % 0x100));
                    message[startIndex + 1] = (byte)((ushort)(indexes[i] / 0x100));
                    message[startIndex + 2] = 0x00;
                    message[startIndex + 3] = MelsecInformationConverter.Instance.GetDeviceCode(type[i]);
                    message[startIndex + 4] = (byte)((ushort)(writeData[i] % 0x100));
                    message[startIndex + 5] = (byte)((ushort)(writeData[i] / 0x100));
                }

                if (_ProtocolType == ProtocolType.Tcp)
                {
                    _Socket.Send(message, 0, message.Length, SocketFlags.None);
                    rv = _Socket.Receive(_Buffer, 0, _Buffer.Length, SocketFlags.None);
                }
                else if (_ProtocolType == ProtocolType.Udp)
                {
                    _Socket.SendTo(message, message.Length, SocketFlags.None, _RemotePoint);
                    rv = _Socket.ReceiveFrom(_Buffer, _Buffer.Length, SocketFlags.None, ref _LocalPoint);
                }

                if (rv != 0)
                {
                    short error = (short)(_Buffer[9] + _Buffer[10] * 0xFF);
                    return error;
                }

                return 0;
            }
            catch
            {
                Close();
                return 1;
            }
        }
        public int RandomWriteDevice(string[] devices, ref short[] writeData)
        {
            return 0;
        }
        public int SetDevice(string address, short writeData)
        {
            return 0;
        }

        /// <summary>
        /// Random read device. (Max 192 words)
        /// </summary>
        /// <param name="type">Device type to access</param>
        /// <param name="indexes">Device indexes</param>
        /// <param name="readData">Read data</param>
        /// <returns></returns>
        public int RandomReadDevice(MelsecDeviceType[] type, int[] indexes, ref short[] readData)
        {
            try
            {
                if ((type.Length != indexes.Length) || (type.Length != readData.Length) ||
                    (indexes.Length != readData.Length))
                {
                    return 100; //array의 길이가 다를 때...
                }

                int size = type.Length * 2;
                int msgLength = 17 + (int)(type.Length * 4);
                byte[] message = new byte[msgLength];

                int rv = BuildMessageHeader_RandomRead((byte)type.Length, ref message);

                int count = type.Length;

                for (int i = 0; i < count; i++)
                {
                    int startIndex = 17 + (i * 4);

                    message[startIndex + 0] = (byte)((ushort)(indexes[i] % 0x100));
                    message[startIndex + 1] = (byte)((ushort)(indexes[i] / 0x100));
                    message[startIndex + 2] = 0x00;
                    message[startIndex + 3] = MelsecInformationConverter.Instance.GetDeviceCode(type[i]);
                }

                if (_ProtocolType == ProtocolType.Tcp)
                {
                    _Socket.Send(message, 0, message.Length, SocketFlags.None);
                    rv = _Socket.Receive(_Buffer, 0, _Buffer.Length, SocketFlags.None);
                }
                else if (_ProtocolType == ProtocolType.Udp)
                {
                    _Socket.SendTo(message, message.Length, SocketFlags.None, _RemotePoint);
                    rv = _Socket.ReceiveFrom(_Buffer, _Buffer.Length, SocketFlags.None, ref _LocalPoint);
                }

                if (rv != 0)
                {
                    short error = (short)(_Buffer[9] + _Buffer[10] * 0x100);

                    if ((error == 0) && ((rv - 11) == size))
                    {
                        int index = (size / 2);
                        for (int i = 0; i < index; i++)
                        {
                            readData[i] = (short)(_Buffer[i * 2 + 11] + (_Buffer[i * 2 + 12] * 0x100));
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }

                return 0;
            }
            catch
            {
                Close();
                return 1;
            }
        }
        #endregion
    }
}
