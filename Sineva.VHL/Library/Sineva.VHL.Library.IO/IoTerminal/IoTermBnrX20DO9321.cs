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
    /// Out 12
    /// </summary>
    [Serializable]
    public class IoTermBnrX20DO9321 : _IoTermBnr
    {
		public IoTermBnrX20DO9321()
		{
			m_ChannelCount = 12;
			m_Name = "DO9321";
			m_IoTypes.Add(IoType.DO);
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_OutStartAddress;
			for (i = 0; i < m_ChannelCount; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.DO;
				ch.Name = "do__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int outAdd = this.OutStartAddress * 12;

			int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "OFF";
				if (ch.IoType == IoType.DO)
				{
                    ch.WiringNo = string.Format("{0}Y{1}", GetBusNo(), UnitConversion.ConverterDuodenary(outAdd++));
				}
			}
		}
	}
}
