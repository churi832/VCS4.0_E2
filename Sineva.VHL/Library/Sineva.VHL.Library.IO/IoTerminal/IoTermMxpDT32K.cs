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
    /// In 16, Out 16
    /// </summary>
    [Serializable]
    public class IoTermMxpDT32K : _IoTermMxp
	{
		public IoTermMxpDT32K()
		{
			m_ChannelCount = 32;
			m_InChannelCnt = 16;
			m_OutChannelCnt = 16;
			m_Name = "DT32K";
			m_IoTypes.Add(IoType.DI);
			m_IoTypes.Add(IoType.DO);
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
					ch.WiringNo = string.Format("X{0:X5}H", inAdd++);
				}
				else if (ch.IoType == IoType.DO)
				{
					ch.WiringNo = string.Format("Y{0:X5}H", outAdd++);
				}
			}
		}
	}
}
