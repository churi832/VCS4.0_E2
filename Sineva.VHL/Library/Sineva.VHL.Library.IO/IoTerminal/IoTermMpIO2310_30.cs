/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sineva.VHL.Library.IO
{
    /// <summary>
    /// In 64, Out 64
    /// </summary>
    [Serializable]
    public class IoTermMpIO2310_30 : _IoTermMp
	{
		public IoTermMpIO2310_30()
		{
			m_ChannelCount = 128;
			m_InChannelCnt = 64;
			m_OutChannelCnt = 64;
			m_InRegisterCount = 4; // 4 word, 8 byte, 64 bit
			m_OutRegisterCount = 4; // 4 word, 8 byte, 64 bit
			m_InRegPrefix = enMpDeviceName.IL;
			m_OutRegPrefix = enMpDeviceName.OL;
			m_Name = "IO2310_30";
			m_IoTypes.Add(IoType.DI);
			m_IoTypes.Add(IoType.DO);

			ChType = enMpChannelType.bitCh;
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_InStartAddress;
			for (i = 0; i < m_InChannelCnt; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.DI;
				ch.Name = "di__";
				this.Channels.Add(ch);
			}
			add = this.m_OutStartAddress;
			for (i = 0; i < m_OutChannelCnt; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.DO;
				ch.Name = "do__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int inAdd = this.InStartAddress * 16;
			int outAdd = this.OutStartAddress * 16;

			int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "OFF";
				if (ch.IoType == IoType.DI)
				{
					ch.WiringNo = string.Format("IB{0:X5}H", inAdd++);
				}
				else if (ch.IoType == IoType.DO)
				{
					ch.WiringNo = string.Format("OB{0:X5}H", outAdd++);
				}
			}
		}
	}
}
