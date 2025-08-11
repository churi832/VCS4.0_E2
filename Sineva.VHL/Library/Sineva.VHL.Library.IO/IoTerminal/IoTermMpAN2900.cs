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
    /// 4ch 16bit word Input
    /// </summary>
    [Serializable]
    public class IoTermMpAN2900 : _IoTermMp
	{
		public IoTermMpAN2900()
		{
			m_ChannelCount = 4;
			m_InChannelCnt = 4;
			m_InRegisterCount = 4;
			//InOutType = enMpInOutType.In;
			ChType = enMpChannelType.wordCh;
			m_InRegPrefix = enMpDeviceName.IA;
			m_OutRegPrefix = enMpDeviceName.OA;
			m_Name = "AN2900";
			m_IoTypes.Add(IoType.AI);
		}

		public override void CreateChannels()
		{
			int i = 0;
			for (i = 0; i < m_InChannelCnt; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.AI;
				ch.Name = "ai__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int inAdd = this.InStartAddress;
			int outAdd = this.OutStartAddress;

			int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "0";
				if (ch.IoType == IoType.AI)
				{
					ch.WiringNo = string.Format("IA{0:X4}H", inAdd++);
				}
				else if (ch.IoType == IoType.AO)
				{
					ch.WiringNo = string.Format("OA{0:X4}H", outAdd++);
				}
			}
		}
	}
}
