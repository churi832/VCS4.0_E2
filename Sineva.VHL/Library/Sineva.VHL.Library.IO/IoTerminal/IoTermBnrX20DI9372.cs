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
    public class IoTermBnrX20DI9372 : _IoTermBnr
    {
		public IoTermBnrX20DI9372()
		{
			m_ChannelCount = 12;
			m_Name = "DI9372";
			m_IoTypes.Add(IoType.DI);
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_InStartAddress;
			for (i = 0; i < m_ChannelCount; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.DI;
				ch.Name = "di__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int inAdd = this.InStartAddress * 12;

			int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "OFF";
				if (ch.IoType == IoType.DI)
				{
					ch.WiringNo = string.Format("{0}X{1}", GetBusNo(), UnitConversion.ConverterDuodenary(inAdd++));
				}
			}
		}
    }
}
