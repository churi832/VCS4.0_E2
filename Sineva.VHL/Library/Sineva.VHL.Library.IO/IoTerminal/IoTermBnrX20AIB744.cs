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
    /// X20 analog input module, 4 full-bridge strain inputs, 24-bit converter resolution, 2.5 kHz input filter 
    /// </summary>
    [Serializable]
    public class IoTermBnrX20AIB744 : _IoTermBnr
    {
		public IoTermBnrX20AIB744()
		{
			m_ChannelCount = 4;
			m_Name = "AIB744";
			m_IoTypes.Add(IoType.AI);
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_InStartAddress;
			for (i = 0; i < m_ChannelCount; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.AI;
				ch.Name = "ai__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
            //int inAdd = this.InStartAddress * 4;
            int inAdd = this.InStartAddress;

            int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "0.0";
				if (ch.IoType == IoType.AI)
				{
					ch.WiringNo = string.Format("{0}AI{1:X4}H", GetBusNo(), inAdd++);
					//ch.WiringNo = string.Format("{0}AI{1}", GetBusNo(), Lib.UnitConversion.ConverterDuodenary(inAdd++));
				}
			}
		}
    }
}
