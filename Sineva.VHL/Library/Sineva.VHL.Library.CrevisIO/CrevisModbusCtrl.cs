using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.BnrModbus;

namespace Sineva.VHL.Library.CrevisIO
{
    public class CrevisModbusCtrl
    {
        #region Field - Constant of Crevis Modbus Protocol Defines
        private const ushort _AddrInWordStart = 0x1102;
        private const ushort _AddrOutWordStart = 0x1103;
        private const ushort _AddrInWordSize = 0x1104;
        private const ushort _AddrOutWordSize = 0x1105;
        private const ushort _AddrExpSlotList = 0x110E;
        private const ushort _AddrExpSlotNo = 0x1110;

        private const ushort _SizeInWordStart = 0x1;
        private const ushort _SizeOutWordStart = 0x1;
        private const ushort _SizeInWordSize = 0x1;
        private const ushort _SizeOutWordSize = 0x1;
        private const ushort _SizeSlotNo = 0x1;

        private const ushort _AddrExpSlotInfoDetail = 0x2000;
        private const ushort _OffsetSlotInfoDetail = 0x20;

        private const ushort _SlotOffsetInWordStart = 0x02;
        private const ushort _SlotOffsetInWordBitOffset = 0x03;
        private const ushort _SlotOffsetInWordBitSize = 0x08;
        private const ushort _SlotOffsetOutWordStart = 0x04;
        private const ushort _SlotOffsetOutWordBitOffset = 0x05;
        private const ushort _SlotOffsetOutWordBitSize = 0x09;
        private const ushort _SizeSlotOffset = 0x01;
        #endregion

        #region Field - Link of I/O Manager
        private IoNodeCrevisModbus m_CrevisIoNode = null;
        private List<_IoTermCrevis> m_IoTerminals = new List<_IoTermCrevis>();
        private bool m_Initialized = false;
        private bool m_InitModuleComp = false;
        private bool m_FirstUpdateOk = false;
        #endregion

        #region Field - Modbus Field Register
        private byte m_UnitId = 0;
        private ushort[] m_InRegBuffer = new ushort[0];
        private ushort[] m_OutRegBuffer = new ushort[0];
        private ushort? m_InRegStartAddr;
        private ushort? m_OutRegStartAddr;
        #endregion

        private BnrModbus.Device.ModbusMaster m_ModbusAdapter = null;
        private TcpClient m_ClientSocket = null;

        private bool m_BusyConnect = false;
        private bool m_BusyMapping = false;

        #region Property
        public bool AdapterRunNormal
        {
            get
            {
                bool rv = true;
                rv &= m_ClientSocket != null && m_ClientSocket.Client != null && m_ClientSocket.Connected;
                rv &= m_ModbusAdapter != null;
                rv &= m_InitModuleComp;
                return rv;
            }
        }
        public bool BusyConnect
        {
            get { return m_BusyConnect; }
        }
        public bool BusyMapping
        {
            get { return m_BusyMapping; }
        }
        #endregion

        #region Constructor
        public CrevisModbusCtrl(_IoNode node)
        {
            try
            {
                m_CrevisIoNode = node as IoNodeCrevisModbus;

                //Thread threadUpdateIo = new Thread(new ThreadStart(ThreadCallbackUpdateIo));
                //threadUpdateIo.IsBackground = true;
                //threadUpdateIo.Start();

                //Thread threadAppLoaderCheck = new Thread(new ThreadStart(ThreadCallbackAppLoaderCheck));
                //threadAppLoaderCheck.IsBackground = true;
                //threadAppLoaderCheck.Start();
            }
            catch
            {

            }
        }
        #endregion

        #region Method
        /// <summary>
        /// IoManager Instance Link Initialize
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            if(m_CrevisIoNode == null) return false;
            if(!m_Initialized)
            {
                InitInstance();
                foreach(_IoTerminal term in m_CrevisIoNode.Terminals)
                {
                    _IoTermCrevis terminal = (term as _IoTermCrevis);
                    if(terminal != null)
                    {
                        m_IoTerminals.Add(terminal);
                    }
                }
                m_Initialized = true;
            }

            return m_Initialized;
        }
        private void InitInstance()
        {
            m_InRegBuffer = new ushort[0];
            m_OutRegBuffer = new ushort[0];
            m_InRegStartAddr = null;
            m_OutRegStartAddr = null;
            m_IoTerminals.Clear();

            m_InitModuleComp = false;
            m_FirstUpdateOk = false;
        }

        public bool IsConnected()
        {
            if(m_ClientSocket == null || m_ClientSocket.Client == null) return false;
            return m_ClientSocket.Client.Connected;
        }
        public bool Open()
        {
            if(m_ClientSocket != null && m_ClientSocket.Client != null && m_ClientSocket.Connected) return false;

            m_BusyConnect = true;
            if(m_CrevisIoNode == null)
            {
                m_BusyConnect = false;
                return false;
            }

            Thread thread = new Thread(new ThreadStart(ThreadCallbackConnect));
            thread.IsBackground = true;
            thread.Start();

            return true;
        }
        public void Close()
        {
            try
            {
                m_ClientSocket.Client.Disconnect(false);
                m_ClientSocket = null;
                m_ModbusAdapter = null;
                m_InitModuleComp = false;
                m_FirstUpdateOk = false;
            }
            catch { }
        }
        private void ThreadCallbackConnect()
        {
            bool rv = false;
            try
            {
                IPAddress ip;
                if(IPAddress.TryParse(m_CrevisIoNode.IpAddress, out ip))
                {
                    IPEndPoint ipep = new IPEndPoint(ip, m_CrevisIoNode.PortNo);
                    m_ClientSocket = new TcpClient();
                    m_ClientSocket.Connect(ipep);
                    m_ModbusAdapter = BnrModbus.Device.ModbusIpMaster.CreateIp(m_ClientSocket);

                    bool connected = false;
                    if(m_ClientSocket != null && m_ClientSocket.Client != null && m_ClientSocket.Connected)
                    {
                        connected = true;
                        Thread thread = new Thread(new ThreadStart(ThreadCallbackReadModuleInfo));
                        thread.IsBackground = true;
                        thread.Start();
                    }

                    rv = connected;
                }
            }
            catch
            {
                m_BusyConnect = false;
                m_ClientSocket = null;
                rv = false;
            }
        }
        private void ThreadCallbackReadModuleInfo()
        {
            int retryCount = 0;
            while(!m_InitModuleComp && retryCount++ < 10)
            {
                Thread.Sleep(1000);

                if(m_ClientSocket == null || m_ClientSocket.Client == null || !m_ClientSocket.Client.Connected)
                    continue;

                ushort[] readRegisters;

                ushort? inWordStart = null, outWordStart = null;
                ushort? inWordSize = null, outWordSize = null;
                ushort? slotCount = null;

                try
                {
                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(0, _AddrInWordStart, _SizeInWordStart)).Length > 0) inWordStart = readRegisters.First();
                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(0, _AddrOutWordStart, _SizeOutWordStart)).Length > 0) outWordStart = readRegisters.First();
                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(0, _AddrInWordSize, _SizeInWordSize)).Length > 0) inWordSize = readRegisters.First();
                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(0, _AddrOutWordSize, _SizeOutWordSize)).Length > 0) outWordSize = readRegisters.First();
                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(0, _AddrExpSlotNo, _SizeSlotNo)).Length > 0) slotCount = readRegisters.First();  // Net Adapter를 제외한 Slot Module Count

                    if(inWordSize.HasValue) m_InRegBuffer = new ushort[(int)inWordSize];
                    if(outWordSize.HasValue) m_OutRegBuffer = new ushort[(int)outWordSize];
                    if(inWordStart.HasValue) m_InRegStartAddr = inWordStart;
                    if(outWordStart.HasValue) m_OutRegStartAddr = outWordStart;

                    bool slotMatchingOk = slotCount.HasValue && slotCount == m_IoTerminals.Count;
                    if(slotCount.HasValue)
                    {
                        // Network Bus 상에 있는 Slot List를 가져온다.
                        ushort[] slotModuleIds;
                        if((slotModuleIds = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, _AddrExpSlotList, m_CrevisIoNode.ExpansionSlotWordSize)).Length > 0)
                        {
                            ushort addrSlotInfo = _AddrExpSlotInfoDetail;
                            for(int i = 0; i < slotCount; i++)
                            {
                                ushort productNumber = slotModuleIds[i + 1];    // 0번 Index는 Net Adapter
                                slotMatchingOk &= m_IoTerminals.Count > i && m_IoTerminals[i].ProductNo == productNumber;
                                if(slotMatchingOk == false) break;


                                if(m_IoTerminals[i].TermIoType == IoType.DI || m_IoTerminals[i].TermIoType == IoType.AI)
                                {
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetInWordStart), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordStart = readRegisters.First();
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetInWordBitOffset), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordBitOffset = readRegisters.First();
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetInWordBitSize), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordBitSize = readRegisters.First();
                                }
                                else if(m_IoTerminals[i].TermIoType == IoType.DO || m_IoTerminals[i].TermIoType == IoType.AO)
                                {
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetOutWordStart), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordStart = readRegisters.First();
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetOutWordBitOffset), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordBitOffset = readRegisters.First();
                                    if((readRegisters = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)(addrSlotInfo + _SlotOffsetOutWordBitSize), _SizeSlotOffset)).Length > 0) m_IoTerminals[i].ImageWordBitSize = readRegisters.First();
                                }

                                slotMatchingOk &= m_IoTerminals[i].ImageWordStart.HasValue;
                                slotMatchingOk &= m_IoTerminals[i].ImageWordBitOffset.HasValue;
                                slotMatchingOk &= m_IoTerminals[i].ImageWordBitSize.HasValue;

                                addrSlotInfo += _OffsetSlotInfoDetail;
                            }
                        }
                    }

                    if(slotMatchingOk)
                        m_InitModuleComp = true;
                    //else
                    //{
                    //    m_ClientSocket.Client.Disconnect(false);
                    //    m_ClientSocket = null;
                    //}
                }
                catch
                {
                }
            }

            if(m_InitModuleComp) FirstUpdateIo();

            m_BusyConnect = false;
        }
        public bool FirstUpdateIo()
        {
            if(m_FirstUpdateOk) return true;

            bool rv = true;
            try
            {
                ushort[] buf;
                if((buf = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)m_OutRegStartAddr, (ushort)m_OutRegBuffer.Length)).Length == m_OutRegBuffer.Length)
                {
                    foreach(_IoTermCrevis terminal in m_IoTerminals)
                    {
                        ushort imageStartWord = (ushort)(terminal.ImageWordStart - m_OutRegStartAddr);
                        ushort imageBitOffset = (ushort)terminal.ImageWordBitOffset;
                        ushort imageBitSize = (ushort)terminal.ImageWordBitSize;
                        if(terminal.TermIoType == IoType.DO)
                        {
                            for(int i = 0; i < imageBitSize; i++)
                            {
                                terminal.Channels[i].State = ((buf[imageStartWord + ((imageBitOffset + i) / 0x10)] >> (imageBitOffset + i)) & 0x1) == 0x1 ? "ON" : "OFF";
                            }
                        }
                        else if(terminal.TermIoType == IoType.AO)
                        {
                        }
                    }
                }
            }
            catch { rv = false; }

            m_FirstUpdateOk = rv;
            return rv;
        }
        public bool UpdateIo()
        {
            bool updateOk = true;
            try
            {
                ushort[] buf;
                // Read Input Registers from Modbus Bus
                if((buf = m_ModbusAdapter.ReadInputRegisters(m_UnitId, (ushort)m_InRegStartAddr, (ushort)m_InRegBuffer.Length)).Length == m_InRegBuffer.Length)
                {
                    foreach(_IoTermCrevis terminal in m_IoTerminals)
                    {
                        ushort imageStartWord = (ushort)(terminal.ImageWordStart - m_InRegStartAddr);
                        ushort imageBitOffset = (ushort)terminal.ImageWordBitOffset;
                        ushort imageBitSize = (ushort)terminal.ImageWordBitSize;
                        if(terminal.TermIoType == IoType.DI)
                        {
                            for(int i = 0; i < imageBitSize; i++)
                            {
                                terminal.Channels[i].State = ((buf[imageStartWord + ((imageBitOffset + i) / 0x10)] >> (imageBitOffset + i)) & 0x1) == 0x1 ? "ON" : "OFF";
                            }
                        }
                        else if(terminal.TermIoType == IoType.AI)
                        {
                            if(terminal.Channels.Count == 0) continue;

                            ushort[] imageWords = new ushort[imageBitSize / 0x10];
                            for(int i = 0; i < imageWords.Length; i++)
                            {
                                imageWords[i] = 0;
                                for(int j = 0; j < 0x10; j++)
                                {
                                    imageWords[i] |= (ushort)((buf[(imageStartWord + i) + ((imageBitOffset + j) / 0x10)] >> ((imageBitOffset + j) % 0x10) & 0x1) == 0x1 ? (0x1 << j) : 0);
                                }
                            }

                            ushort chWordSize = (ushort)((imageBitSize / terminal.Channels.Count) / 0x10);
                            if(chWordSize == 1)
                            {
                                // 1-Channel Data Size : 1 WORD
                                for(int i = 0; i < terminal.Channels.Count; i++)
                                {
                                    //if(terminal.Channels[i].Scale.UseScale)
                                    //{
                                    //    terminal.Channels[i].Scale.CurAdc = imageWords[i];
                                    //    terminal.Channels[i].State = terminal.Channels[i].Scale.GetCalValue().ToString();
                                    //}
                                    //else
                                    //{
                                    //    terminal.Channels[i].State = imageWords[i].ToString();
                                    //}
                                    terminal.Channels[i].Scale.CurAdc = imageWords[i];
                                    terminal.Channels[i].State = imageWords[i].ToString();
                                }
                            }
                            else if(chWordSize == 2)
                            {
                                // 1-Channel Data Size : 2 WORD
                                for(int i = 0; i < terminal.Channels.Count; i++)
                                {
                                    List<byte> bytes = new List<byte>();
                                    bytes.AddRange(BitConverter.GetBytes(imageWords[i * 2 + 1]));
                                    bytes.AddRange(BitConverter.GetBytes(imageWords[i * 2 + 0]));
                                    //if(terminal.Channels[i].Scale.UseScale)
                                    //{
                                    //    terminal.Channels[i].Scale.CurAdc = BitConverter.ToUInt32(bytes.ToArray(), 0);
                                    //    terminal.Channels[i].State = terminal.Channels[i].Scale.GetCalValue().ToString();
                                    //}
                                    //else
                                    //{
                                    //    terminal.Channels[i].State = BitConverter.ToUInt32(bytes.ToArray(), 0).ToString();
                                    //}
                                    terminal.Channels[i].Scale.CurAdc = BitConverter.ToUInt32(bytes.ToArray(), 0);
                                    terminal.Channels[i].State = BitConverter.ToUInt32(bytes.ToArray(), 0).ToString();
                                }
                            }
                        }
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Write Output Registers to Modbus Bus
                buf = new ushort[m_OutRegBuffer.Length];
                for(int i = 0; i < buf.Length; i++) buf[i] = 0;
                foreach(_IoTermCrevis terminal in m_IoTerminals)
                {
                    ushort imageStartWord = (ushort)(terminal.ImageWordStart - m_OutRegStartAddr);
                    ushort imageBitOffset = (ushort)terminal.ImageWordBitOffset;
                    ushort imageBitSize = (ushort)terminal.ImageWordBitSize;

                    if(terminal.TermIoType == IoType.DO)
                    {
                        for(int i = 0; i < terminal.Channels.Count; i++)
                        {
                            buf[imageStartWord + ((imageBitOffset + i) / 0x10)] |= (ushort)(terminal.Channels[i].State == "ON" ? 0x1 << (i + imageBitOffset) : 0);
                        }
                    }
                    else if(terminal.TermIoType == IoType.AO)
                    {
                        ushort chWordSize = (ushort)(terminal.ChannelBitSize / 0x10);
                        for(int i = 0; i < terminal.Channels.Count; i++)
                        {
                            if(chWordSize == 1)
                            {
                                // 1-Channel Data Size : 1 WORD
                                ushort value;
                                if(UInt16.TryParse(terminal.Channels[i].State, out value) == false) continue;

                                for(int j = 0; j < 0x10; j++)
                                {
                                    buf[imageStartWord + ((imageBitOffset + j) / 0x10)] |= (ushort)(((value >> j) & 0x1) == 0x1 ? (0x1 << ((imageBitOffset + j) % 0x10)) : 0);
                                }
                            }
                            else if(chWordSize == 2)
                            {
                                // 1-Channel Data Size : 2 WORD
                                uint value = Convert.ToUInt32(terminal.Channels[i].State);
                                if(UInt32.TryParse(terminal.Channels[i].State, out value) == false) continue;

                                byte[] bytes = BitConverter.GetBytes(value);
                                ushort wordHi = BitConverter.ToUInt16(bytes.Skip(0).Take(2).ToArray(), 0);
                                ushort wordLo = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);

                                for(int j = 0; j < 0x10; j++)
                                {
                                    buf[imageStartWord + ((imageBitOffset + j) / 0x10) + 0] |= (ushort)(((wordHi >> j) & 0x1) == 0x1 ? (0x1 << ((imageBitOffset + j) % 0x10)) : 0);
                                    buf[imageStartWord + ((imageBitOffset + j) / 0x10) + 1] |= (ushort)(((wordLo >> j) & 0x1) == 0x1 ? (0x1 << ((imageBitOffset + j) % 0x10)) : 0);
                                }
                            }
                        }
                    }
                }
                if(buf.Length > 0)
                {
                    ushort[] bufDevice;
                    if((bufDevice = m_ModbusAdapter.ReadHoldingRegisters(m_UnitId, (ushort)m_OutRegStartAddr, (ushort)m_OutRegBuffer.Length)).Length == m_OutRegBuffer.Length)
                    {
                        bool write = false;
                        for(int i = 0; i < buf.Length; i++)
                        {
                            if(buf.Length > i) write |= bufDevice[i] != buf[i];
                        }
                        if(write)
                        {
                            m_ModbusAdapter.WriteMultipleRegisters(m_UnitId, (ushort)m_OutRegStartAddr, buf);
                            for(int i = 0; i < m_OutRegBuffer.Length; i++) m_OutRegBuffer[i] = buf[i];
                        }
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            catch { updateOk = false; }

            return updateOk;
        }
        private void CreateModbusRegisterMap()
        {
        }
        private void FirstOutuptUpdate()
        {
        }
        public void TestFunc_SetDo_FromCtrlToIoManager(int addr, bool on)
        {
            m_IoTerminals[2].Channels[addr].SetDo(on);
            //m_IoTermListDO[0].Channels[addr].SetDo(on);
        }
        #endregion
    }
}
