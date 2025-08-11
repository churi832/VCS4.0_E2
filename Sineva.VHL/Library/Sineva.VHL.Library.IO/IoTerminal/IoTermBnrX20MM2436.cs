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
    /// In 12
    /// </summary>
    [Serializable]
    public class IoTermBnrX20MM2436 : _IoTermBnr
    {
		public IoTermBnrX20MM2436()
		{
			m_ChannelCount = 2;
			m_Name = "MM2436";
			m_IoTypes.Add(IoType.AX);
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_InStartAddress;
			for (i = 0; i < m_ChannelCount; i++)
			{
				BnrAxisChannel ch = new BnrAxisChannel();
				ch.IoType = IoType.AX;
				ch.Name = "axis__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int inAdd = this.InStartAddress * 2;

			int i = 0;
			foreach (BnrAxisChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "OFF";
				if (ch.IoType == IoType.AX)
				{
					ch.WiringNo = string.Format("{0}AX{1}", GetBusNo(), UnitConversion.ConverterDuodenary(inAdd++));
				}
			}
		}
    }
}
