using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.ComponentModel;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.BnrModbus.Data;
using Sineva.VHL.Library.BnrModbus.Device;
using Sineva.VHL.Library.BnrModbus.Utility;

namespace Sineva.VHL.Library.Bnr
{
    public class BnrModbusCtrl
    {
        #region CONSTANT
        private const int ADDRESS_AXIS_COMMAND = 0x4000;
        private const int ADDRESS_AXIS_PARAMETER = 0x6000;
        private const int ADDRESS_AXIS_STATUS = 0x2000;

        private const int SIZE_OF_AXIS_COMMAND = 0x0005;
        private const int SIZE_OF_AXIS_PARAMETER = 0x0012;
        private const int SIZE_OF_AXIS_STATUS = 0x000C;
        private const int MAX_SIZE_OF_AXIS_COMMAND = 0x0008;
        private const int MAX_SIZE_OF_AXIS_PARAMETER = 0x0100;
        private const int MAX_SIZE_OF_AXIS_STATUS = 0x0100;

        private const ushort ADDRESS_INPUT = 0x3000; // ~ 0X3FFF
        private const ushort ADDRESS_OUTPUT = 0x7000; // ~ 0X7FFF
        private const ushort ADDRESS_LOADCELL_INPUT = 0x4000; // ~ 0X4FFF
		private const ushort ADDRESS_ANALOG_INPUT = 0x3000; // ~ 0X3FFF
		private const ushort ADDRESS_ANALOG_OUTPUT = 0x7000; // ~ 0X7FFF

        private const ushort SIZE_OF_MAX_TERM = 10;
        private const ushort SIZE_OF_MAX_CHANNEL = 12;
        #endregion

        #region Fields
        private bool m_FristOutputUpdate = false;
        private IoNodeBnrX20CP0482 m_Node = null;
        private List<_IoTerminal> m_InTerminals = new List<_IoTerminal>();
        private List<_IoTerminal> m_OutTerminals = new List<_IoTerminal>();
        private List<_IoTerminal> m_AiTerminals = new List<_IoTerminal>();
        private List<_IoTerminal> m_AoTerminals = new List<_IoTerminal>();
        private List<_IoTerminal> m_LcTerminals = new List<_IoTerminal>();
        private List<_IoTerminal> m_AxisTerminals = new List<_IoTerminal>();
        private TcpClient m_Client = null;
        private ModbusIpMaster m_Master = null;
        #endregion

        #region Constructor
        public BnrModbusCtrl()
        {

        }

        public BnrModbusCtrl(IoNodeBnrX20CP0482 node)
        {
            m_Node = node;
        }
        public bool Initialize()
        {
            foreach (_IoTerminal terminal in m_Node.Terminals)
            {
                if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20DI9372))) m_InTerminals.Add(terminal);
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20DO9321))) m_OutTerminals.Add(terminal);
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AI2632))) m_AiTerminals.Add(terminal); 
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AI4632))) m_AiTerminals.Add(terminal);
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AIA744))) m_LcTerminals.Add(terminal); // loadcell ai
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AIB744))) m_LcTerminals.Add(terminal); // loadcell ai
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AO2632))) m_AoTerminals.Add(terminal);
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20AO4632))) m_AoTerminals.Add(terminal);
                else if (Type.Equals(terminal.GetType(), typeof(IoTermBnrX20MM2436))) m_AxisTerminals.Add(terminal);
            }

            foreach (IoTermBnrX20MM2436 term in m_AxisTerminals)
            {
                foreach (BnrAxisChannel axis in term.Channels)
                {
                    axis.CommandChangedEvent += Ch_CommandChangedEvent;
                    axis.ParameterChangedEvent += Ch_ParameterChangedEvent;
                }
            }

            return true;
        }

        private void Ch_ParameterChangedEvent(object sender, EventArgs e)
        {
            WriteAxisParameter(sender as BnrAxisChannel);
        }

        private void Ch_CommandChangedEvent(object sender, EventArgs e)
        {
            WriteAxisCommand(sender as BnrAxisChannel);
        }
        #endregion

        #region Methods
        public bool IsBnrAxisExist()
        {
            return m_AxisTerminals.Count > 0;
        }

        public bool IsConnected()
        {
            if (AppConfig.Instance.Simulation.BNR) return true;

            if (m_Client == null) return false;
            return m_Client.Connected;
        }
        /// <summary>
        /// IP : 192.168.0.100
        /// PORT : 502
        /// </summary>
        public void ClientConnection()
        {
            if (AppConfig.Instance.Simulation.BNR) return;

            try
            {
                if (m_Client == null)
                {
                    m_Client = new TcpClient(m_Node.IpAddress, m_Node.PortNo);
                    m_Master = ModbusIpMaster.CreateIp(m_Client);
                }
                else if (m_Client.Connected == false)
                {
                    m_Client = null;
                    m_Master = null;
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
                return;
            }
        }
        public bool MasterWriteMultipleCoils(ushort startAddress, bool[] data)
        {
            try
            {
                lock (m_Master)
                {
                    m_Master.WriteMultipleCoils(startAddress, data);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool[] MasterReadMultipleCoils(ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                bool[] rv = null;
                lock (m_Master)
                {
                    rv = m_Master.ReadCoils(startAddress, numberOfPoints);
                }
                return rv;
            }
            catch
            {
                return null;
            }
        }
        public bool[] MasterReadInputs(ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                bool[] rv = null;
                lock (m_Master)
                {
                    rv = m_Master.ReadInputs(startAddress, numberOfPoints);
                }
                return rv;
            }
            catch
            {
                return null;
            }
        }
        public ushort[] MasterReadInputRegisters(ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                ushort[] rv = null;
                lock (m_Master)
                {
                    rv = m_Master.ReadInputRegisters(startAddress, numberOfPoints);
                }
                return rv;
            }
            catch
            {
                return null;
            }
        }
        public ushort[] MasterReadHoldingRegisters(ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                ushort[] rv = null;
                lock (m_Master)
                {
                    rv = m_Master.ReadHoldingRegisters(startAddress, numberOfPoints);
                }
                return rv;
            }
            catch
            {
                return null;
            }
        }
        public bool MasterWriteMultipleRegisters(ushort startAddress, ushort[] data)
        {
            try
            {
                lock (m_Master)
                {
                    m_Master.WriteMultipleRegisters(startAddress, data);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Update()
        {
            FirstOutputUpdate();
            if (m_FristOutputUpdate)
            {
                UpdateIo();
                UpdateAxis();
            }
        }
        // Data 읽고 쓰기
        // Node에 있는 I/O Mapping
        public void UpdateIo()
        {
            if(m_Client == null) return;
            if(m_Master == null) return;

            try
            {
                // input update 하자
                int input_reading_term = m_InTerminals.Count;
                int output_reading_term = m_OutTerminals.Count;

                int max_reading_term_no = input_reading_term / SIZE_OF_MAX_TERM + 1;
                int max_reading_term_mod = input_reading_term % SIZE_OF_MAX_TERM;
                int max_writing_term_no = output_reading_term / SIZE_OF_MAX_TERM + 1;
                int max_writing_term_mod = output_reading_term % SIZE_OF_MAX_TERM;

                // Input <====
                for (int i = 0; i < max_reading_term_no; i++)
                {
                    ushort start_address = (ushort)(ADDRESS_INPUT + i * (SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL));
                    ushort reading_size = (ushort)(SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL); // SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL = 120 읽어오자
                    if (i >= max_reading_term_no - 1) // mod * SIZE_OF_MAX_CHANNEL = ? 나머지 개수를 읽어오자
                        reading_size = (ushort)(max_reading_term_mod * SIZE_OF_MAX_CHANNEL);
                    if(reading_size == 0) continue;

                    bool[] Data = MasterReadInputs(start_address, reading_size);
                    if (Data == null) break;

                    for (int j = 0; j < Data.Length; j++)
                    {
                        int term_no = i * SIZE_OF_MAX_TERM + j / SIZE_OF_MAX_CHANNEL;
                        int ch_no = j % SIZE_OF_MAX_CHANNEL;
                        if (m_InTerminals[term_no].Channels[ch_no].IsBContact == false)
                            m_InTerminals[term_no].Channels[ch_no].State = Data[j] == true ? "ON" : "OFF";
                        else
                            m_InTerminals[term_no].Channels[ch_no].State = Data[j] == true ? "OFF" : "ON";
                    }
                }

                // Output ====>
                for (int i = 0; i < max_writing_term_no; i++)
                {
                    ushort start_address = (ushort)(ADDRESS_OUTPUT + i * (SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL));
                    ushort writing_size = (ushort)(SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL); // SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL = 120 읽어오자
                    if (i >= max_writing_term_no - 1) // mod * SIZE_OF_MAX_CHANNEL = ? 나머지 개수를 읽어오자
                        writing_size = (ushort)(max_writing_term_mod * SIZE_OF_MAX_CHANNEL);
                    if(writing_size == 0) continue;

                    bool[] Data = new bool[writing_size];
                    for (int j = 0; j < Data.Length; j++)
                    {
                        int term_no = i * SIZE_OF_MAX_TERM + j / SIZE_OF_MAX_CHANNEL;
                        int ch_no = j % SIZE_OF_MAX_CHANNEL;
                        Data[j] = m_OutTerminals[term_no].Channels[ch_no].State == "ON" ? true : false;
                    }
                    bool rv = MasterWriteMultipleCoils(start_address, Data);
                    if (rv == false) break;
                }

                // Analog Input State Reading ... double or float 표현 (32bit)(2 ushort)(4byte)
                if (m_AiTerminals.Count > 0)
                {
                    int channel_count = 0;
                    foreach (_IoTerminal term in m_AiTerminals) channel_count += term.Channels.Count;
                    ushort[] stsBuffer = new ushort[2 * channel_count];
                    byte[] arrByte = new byte[4 * channel_count];
                    stsBuffer = MasterReadInputRegisters((ushort)(ADDRESS_ANALOG_INPUT), (ushort)stsBuffer.Length);
                    Buffer.BlockCopy(stsBuffer, 0, arrByte, 0, arrByte.Length);

                    if (stsBuffer != null)
                    {
                        int startindex = 0;
                        foreach (_IoTerminal term in m_AiTerminals)
                        {
                            foreach (IoChannel ch in term.Channels)
                            {
                                float ai = BitConverter.ToSingle(arrByte, startindex);
                                ch.State = Math.Round(ai, 5).ToString();
                                startindex += 4;
                            }
                        }
                    }
                }
                // Loadcell State Reading ... double or float 표현 (32bit)(2 ushort)(4byte)
                if (m_LcTerminals.Count > 0)
                {
                    int channel_count = 0;
                    foreach (_IoTerminal term in m_LcTerminals) channel_count += term.Channels.Count;
                    ushort[] stsBuffer = new ushort[2 * channel_count];
                    byte[] arrByte = new byte[4 * channel_count];
                    stsBuffer = MasterReadInputRegisters((ushort)(ADDRESS_LOADCELL_INPUT), (ushort)stsBuffer.Length);
                    Buffer.BlockCopy(stsBuffer, 0, arrByte, 0, arrByte.Length);

                    if (stsBuffer != null)
                    {
                        int startindex = 0;
                        foreach (_IoTerminal term in m_LcTerminals)
                        {
                            foreach (IoChannel ch in term.Channels)
                            {
                                float ai = BitConverter.ToSingle(arrByte, startindex);
                                ch.State = Math.Round(ai, 5).ToString();
                                startindex += 4;
                            }
                        }
                    }
                }

                // Analog Output State Writing ... double or float 표현 (32bit)(2 ushort)(4byte)
                if (m_AoTerminals.Count > 0)
                {
                    int channel_count = 0;
                    foreach (_IoTerminal term in m_AoTerminals) channel_count += term.Channels.Count;
                    ushort[] writeData = new ushort[2 * channel_count];
                    List<byte> arrByte = new List<byte>();

                    foreach (_IoTerminal term in m_AoTerminals)
                    {
                        foreach (IoChannel ch in term.Channels)
                        {
                            float val = 0.0f;
                            if (float.TryParse(ch.State, out val)) arrByte.AddRange(BitConverter.GetBytes(val));
                        }
                    }
                    Buffer.BlockCopy(arrByte.ToArray(), 0, writeData, 0, arrByte.Count);

                    lock (m_Master)
                    {
                        bool rv = MasterWriteMultipleRegisters((ushort)(ADDRESS_ANALOG_OUTPUT), writeData);
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.Message);
            }
        }

        public void UpdateAxis()
        {
            if (m_Client == null) return;
            if (m_Master == null) return;

            try
            {
                ushort[] stsBuffer = new ushort[SIZE_OF_AXIS_STATUS];
                byte[] arrByte = new byte[28];

                foreach (IoTermBnrX20MM2436 term in m_AxisTerminals)
                    foreach (BnrAxisChannel axis in term.Channels)
                    {
                        ushort addr = (ushort)(ADDRESS_AXIS_STATUS + axis.Id * MAX_SIZE_OF_AXIS_STATUS);
                        stsBuffer = MasterReadInputRegisters(addr, SIZE_OF_AXIS_STATUS);
                        if (stsBuffer == null) break;

                        Buffer.BlockCopy(stsBuffer, 0, arrByte, 0, 24);

                        axis.Status = (enBnrState)BitConverter.ToInt16(arrByte, 0);
                        axis.LastAlarmCode = (ushort)BitConverter.ToInt16(arrByte, 2);
                        axis.CurOpenPosition = BitConverter.ToSingle(arrByte, 4);
                        axis.CurDistance = BitConverter.ToSingle(arrByte, 8);
                        axis.CurHoldPosition = BitConverter.ToSingle(arrByte, 12);
                        axis.CurPosition = BitConverter.ToSingle(arrByte, 16);
                        axis.CurCurrent = BitConverter.ToSingle(arrByte, 20);
                    }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.Message);
            }
        }

        public bool WriteAxisParameter(BnrAxisChannel axis)
        {
            bool rv = false;
            if (m_Client == null) return rv;
            if (m_Master == null) return rv;

            ushort[] writeData = new ushort[SIZE_OF_AXIS_PARAMETER];
            byte[] buffer = new byte[SIZE_OF_AXIS_STATUS * 2];
            ushort addr = (ushort)(ADDRESS_AXIS_PARAMETER + axis.Id * MAX_SIZE_OF_AXIS_PARAMETER);

            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(axis.ParaMaxCurrent)); // max torque ?
            buf.AddRange(BitConverter.GetBytes(axis.ParaHoldTorque));
            buf.AddRange(BitConverter.GetBytes(axis.ParaPeakTorque));
            buf.AddRange(BitConverter.GetBytes(axis.ParaMoveTorque));
            buf.AddRange(BitConverter.GetBytes(axis.ParaMoveTime));
            buf.AddRange(BitConverter.GetBytes(axis.ParaTolerance));
            buf.AddRange(BitConverter.GetBytes(axis.ParaResolution)); //resolution ?
            buf.AddRange(BitConverter.GetBytes(axis.ParaOpenDirection));
            buf.AddRange(BitConverter.GetBytes(axis.ParaForceSet));
            Buffer.BlockCopy(buf.ToArray(), 0, writeData, 0, SIZE_OF_AXIS_PARAMETER * 2);

            lock (m_Master)
            {
                rv = MasterWriteMultipleRegisters(addr, writeData);
            }
            return rv;
        }

        public bool WriteAxisCommand(BnrAxisChannel axis)
        {
            bool rv = false;
            if (m_Client == null) return rv;
            if (m_Master == null) return rv;

            ushort start_address = (ushort)(ADDRESS_AXIS_COMMAND + axis.Id * MAX_SIZE_OF_AXIS_COMMAND);
            bool[] Data = new bool[MAX_SIZE_OF_AXIS_COMMAND];
            Data[0] = axis.Command.HasFlag(enBnrCmdFlag.cmdInit) ? true : false;
            Data[1] = axis.Command.HasFlag(enBnrCmdFlag.cmdOpen) ? true : false;
            Data[2] = axis.Command.HasFlag(enBnrCmdFlag.cmdClose) ? true : false;
            Data[3] = axis.Command.HasFlag(enBnrCmdFlag.cmdStop) ? true : false;
            Data[4] = axis.Command.HasFlag(enBnrCmdFlag.cmdReset) ? true : false;
            Data[5] = axis.Command.HasFlag(enBnrCmdFlag.cmdForce) ? true : false;

            lock (m_Master)
            {
                rv = MasterWriteMultipleCoils(start_address, Data);
            }
            return rv;
        }

        public void FirstOutputUpdate()
        {
            if (m_FristOutputUpdate) return;
            if(m_Client == null) return;
            if(m_Master == null) return;

            try
            {
                // Output State update 하자
                int output_reading_term = m_OutTerminals.Count;
                int max_writing_term_no = output_reading_term / SIZE_OF_MAX_TERM + 1;
                int max_writing_term_mod = output_reading_term % SIZE_OF_MAX_TERM;

                // Output State Reading
                for(int i = 0; i < max_writing_term_no; i++)
                {
                    ushort start_address = (ushort)(ADDRESS_OUTPUT + i * (SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL));
                    ushort reading_size = (ushort)(SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL); // SIZE_OF_MAX_TERM * SIZE_OF_MAX_CHANNEL = 120 읽어오자
                    if(i >= max_writing_term_no - 1) // mod * SIZE_OF_MAX_CHANNEL = ? 나머지 개수를 읽어오자
                        reading_size = (ushort)(max_writing_term_mod * SIZE_OF_MAX_CHANNEL);
                    if(reading_size == 0) continue;

                    bool[] Data = MasterReadMultipleCoils(start_address, reading_size);
                    if (Data != null)
                    {
                        for (int j = 0; j < Data.Length; j++)
                        {
                            int term_no = i * SIZE_OF_MAX_TERM + j / SIZE_OF_MAX_CHANNEL;
                            int ch_no = j % SIZE_OF_MAX_CHANNEL;
                            m_OutTerminals[term_no].Channels[ch_no].State = Data[j] == true ? "ON" : "OFF";
                        }
                    }
                }

                // Analog Output State Reading ... double or float 표현 (32bit)(2 ushort)(4byte)
                if (m_AoTerminals.Count > 0)
                {
                    int channel_count = 0;
                    foreach (_IoTerminal term in m_AoTerminals) channel_count += term.Channels.Count;
                    ushort[] stsBuffer = new ushort[2 * channel_count];
                    byte[] arrByte = new byte[4 * channel_count];
                    stsBuffer = MasterReadHoldingRegisters((ushort)(ADDRESS_ANALOG_OUTPUT), (ushort)stsBuffer.Length);

                    if (stsBuffer != null)
                    {
                        Buffer.BlockCopy(stsBuffer, 0, arrByte, 0, arrByte.Length);

                        int startindex = 0;
                        foreach (_IoTerminal term in m_AoTerminals)
                        {
                            foreach (IoChannel ch in term.Channels)
                            {
                                ch.State = BitConverter.ToSingle(arrByte, startindex).ToString();
                                startindex += 4;
                            }
                        }
                    }
                }

                // Axis Parameter Write .... 운영에서 parameter를 관리하는데 굳이 upload 할 필요 없을 것 같음.
                foreach (IoTermBnrX20MM2436 term in m_AxisTerminals)
                    foreach (BnrAxisChannel axis in term.Channels) WriteAxisParameter(axis);

                m_FristOutputUpdate = true;
            }
            catch(Exception err)
            {
                ExceptionLog.WriteLog(err.Message);
            }
        }
        #endregion
    }
}
