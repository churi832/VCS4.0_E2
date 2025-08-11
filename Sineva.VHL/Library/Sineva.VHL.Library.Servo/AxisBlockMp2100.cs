using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    public class AxisBlockMp2100 : _AxisBlock
    {
        #region Field
        private static object m_Lock = new object();
        private const int m_RcvCmdSize = 20;
        private TimeSpan m_TimeToWait = TimeSpan.FromMilliseconds(50);

        private enMp2100CpuNo m_CpuNo = enMp2100CpuNo.No1;
        private uint m_DeviceTimeout = 10000;

        private uint m_WriteStartAddress;
        private uint m_ReadStartAddress;
        private uint m_WriteRegByteSize;
        private uint m_ReadRegByteSize;
        private uint m_WriteRegCount;
        private uint m_ReadRegCount;
        private int m_SerialNo = 0;

        private int m_AxisCount = 0;
        private int m_AxisExcluedCount = 10; // Home Offset으로 사용할 10개 축은 빼자

        private ushort m_MpFromHeartBit;
        private ushort m_MpFromCardFault;
        private ushort m_MpFromTenInterlock;

        private ushort m_MpToHeartBit;
        private ushort m_MpToAutoRun;
        private ushort m_MpToInterlock;

        private bool m_Connected = false;
        private bool m_OneTimeUpdatePos = false;
        #endregion

        #region Property
        [Category("MP2100 Setting")]
        public enMp2100CpuNo CpuNo
        {
            get { return m_CpuNo; }
            set { m_CpuNo = value; }
        }
        [Category("MP2100 Setting")]
        public uint DeviceTimeout
        {
            get { return m_DeviceTimeout; }
            set { m_DeviceTimeout = value; }
        }
        public uint ReadStartAddress
        {
            get { return m_ReadStartAddress; }
            set { m_ReadStartAddress = value; }
        }

        public uint WriteStartAddress
        {
            get { return m_WriteStartAddress; }
            set { m_WriteStartAddress = value; }
        }
        public int AxisCount
        {
            get
            {
                m_AxisCount = m_Axes.Count;
                return m_AxisCount;
            }
            //set { m_AxisCount = value; }
        }
        public int AxisExcluedCount
        {
            get { return m_AxisExcluedCount; }
            set { m_AxisExcluedCount = value; }
        }
        
        [Browsable(false)]
        public ushort MpFromTenInterlock
        {
            get { return m_MpFromTenInterlock; }
        }
        [Browsable(false)]
        public ushort MpFromCardFault
        {
            get { return m_MpFromCardFault; }
        }
        [Browsable(false)]
        public ushort MpFromHeartBit
        {
            get { return m_MpFromHeartBit; }
        }
        [Browsable(false), XmlIgnore()]
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }
        [Browsable(false)]
        public ushort MpToHeartBit
        {
            set { m_MpToHeartBit = value; }
        }
        [Browsable(false)]

        public ushort MpToAutoRun
        {
            set { m_MpToAutoRun = value; }
        }
        [Browsable(false)]

        public ushort MpToInterlock
        {
            set { m_MpToInterlock = value; }
        }

        #endregion

        #region Constructor
        public AxisBlockMp2100()
        {
            this.ControlFamily = ServoControlFamily.YMp2100;
        }
        #endregion

        #region Method
        public void Initialize()
        {
            m_ControlFamily = ServoControlFamily.YMp2100;

            m_WriteRegByteSize = (uint)WriteRegDataByteSize();
            m_ReadRegByteSize = (uint)ReadRegDataByteSize();

            m_WriteRegCount = (ushort)((int)m_WriteRegByteSize / 2);
            m_ReadRegCount = (ushort)((int)m_ReadRegByteSize / 2);
        }

		public int GetAxisCount()
        {
            return m_Axes.Count;
        }

		private int WriteRegDataByteSize()
        {
            //return GetAxisCount() * Marshal.SizeOf(typeof(MpWriteReg));
            return (GetAxisCount() * 14) + 6; // + 3word = heart bit, auto run, interlock
        }

        private int ReadRegDataByteSize()
        {
            //return GetAxisCount() * Marshal.SizeOf(typeof(MpReadReg)); // + 3 word 해줘야 함(Heartbit, cardfault, Tens interlock)
            //return GetAxisCount() * Marshal.SizeOf(typeof(MpReadReg)) + 6; // + 3 word 해줘야 함(Heartbit, cardfault, Tens interlock)
            return (GetAxisCount() * 14) + 6;
        }

		public ushort[] BuildRegWriteData(ref uint writeWordSize)
        {
            uint startAdd = WriteStartAddress;
            List<ushort> data = new List<ushort>();

            data.Add(m_MpToHeartBit);           // Heart Bit (1Word)
            data.Add(m_MpToAutoRun);            // Auto Run (1Word)
            data.Add(m_MpToInterlock);          // Interlock (1Word)

            for(int i = 0; i < m_AxisCount - m_AxisExcluedCount; i++)
            {
                data.Add((m_Axes[i] as MpAxis).Command);                                    // Command (1Word)

                //data.Add((ushort)((m_Axes[i] as MpAxis).AbsTargetPos & 0xFFFF));          // ABS POS (2Word)
                //data.Add((ushort)(((m_Axes[i] as MpAxis).AbsTargetPos >> 16) & 0xFFFF));
                Int32 target_pos = (Int32)(m_Axes[i] as MpAxis).MpTargetPos;
                data.Add((ushort)(target_pos & 0xFFFF));                                    // ABS POS (2Word)
                data.Add((ushort)((target_pos >> 16) & 0xFFFF));

                data.Add((ushort)((Int32)(m_Axes[i] as MpAxis).MpTargetSpeed & 0xFFFF));          // ABS SPEED (2Word)
                data.Add((ushort)(((Int32)(m_Axes[i] as MpAxis).MpTargetSpeed >> 16) & 0xFFFF));

                data.Add((ushort)((Int32)(m_Axes[i] as MpAxis).MpJogSpeed & 0xFFFF));          // JOG SPEED (2Word)
                data.Add((ushort)(((Int32)(m_Axes[i] as MpAxis).MpJogSpeed >> 16) & 0xFFFF));
            }

            // Home Offset .. 32축의 HomeOffset 값만 가져와서 순서대로.... Axis 10개는 필요 : 2word * 32 = 64word  <  7word * 10 = 70word
            for (int i = 0; i < 32; i++)
            {
                if (i < m_Axes.Count)
                {
                    data.Add((ushort)((Int32)(m_Axes[i] as MpAxis).HomeOffset & 0xFFFF));          // JOG SPEED (2Word)
                    data.Add((ushort)(((Int32)(m_Axes[i] as MpAxis).HomeOffset >> 16) & 0xFFFF));
                }
            }

            // dspcrassus - 191125 : Word Size 맞춰줘야 함... 6word 빵꾸가 났음
            for (int i = 0; i < 6; i++)
            {
                data.Add(0);
            }

            writeWordSize = (uint)data.Count;

            return data.ToArray();
        }

		public int ParseRegReadResponse(ushort[] array)
        {
            if(array.Length != (int)m_ReadRegCount) return -1;

            int index = 0;
            m_MpFromHeartBit = array[index++];
            m_MpFromCardFault = array[index++];
            m_MpFromTenInterlock = array[index++];

            for(int i = 0; i < m_AxisCount - m_AxisExcluedCount; i++)
            {
                (m_Axes[i] as MpAxis).AxisStatus = (enAxisInFlag)array[index++];
                (m_Axes[i] as MpAxis).CurPos = (Int32)(array[index++] | (array[index++] << 16));
                (m_Axes[i] as MpAxis).CurTorque = (Int32)(array[index++] | (array[index++] << 16));
                (m_Axes[i] as MpAxis).CurSpeed = (Int32)(array[index++] | (array[index++] << 16));
            }

            // Home Offset .. 32축의 HomeOffset 값만 가져와서 순서대로 
            for (int i = 0; i < 32; i++)
            {
                if (i < m_Axes.Count)
                {
                    (m_Axes[i] as MpAxis).HomeOffset = (Int32)(array[index++] | (array[index++] << 16));
                }
            }

            if (!m_OneTimeUpdatePos)
            {
                m_OneTimeUpdatePos = true;
                for (int i = 0; i < m_AxisCount - m_AxisExcluedCount; i++)
                {
                    double cur_pos = (m_Axes[i] as MpAxis).CurPos;
                    (m_Axes[i] as MpAxis).TargetPos = (m_Axes[i] as MpAxis).Pulse2Len(cur_pos);
                }
            }

            return 0;
        }
        #endregion
    }
}
