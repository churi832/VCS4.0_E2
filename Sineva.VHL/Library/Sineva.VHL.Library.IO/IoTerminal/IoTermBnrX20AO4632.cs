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
    /// X20 analog output module, 4 outputs, ±10 V or 0 to 20 mA, 16-bit converter resolution
    /// </summary>
    [Serializable]
    public class IoTermBnrX20AO4632 : _IoTermBnr
    {
		public IoTermBnrX20AO4632()
		{
			m_ChannelCount = 4;
			m_Name = "AO4632";
			m_IoTypes.Add(IoType.AO);
		}

		public override void CreateChannels()
		{
			int i = 0;
			int add = this.m_InStartAddress;
			for (i = 0; i < m_ChannelCount; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.AO;
				ch.Name = "ao__";
				this.Channels.Add(ch);
			}
		}

		public override void BuildChannels()
		{
			int inAdd = this.InStartAddress * 4;

			int i = 0;
			foreach (IoChannel ch in Channels)
			{
				ch.ChannelNo = i++;
				ch.State = "0.0";
				if (ch.IoType == IoType.AO)
				{
					ch.WiringNo = string.Format("{0}AO{1:X4}H", GetBusNo(), inAdd++);
					//ch.WiringNo = string.Format("{0}AO{1}", GetBusNo(), Lib.UnitConversion.ConverterDuodenary(inAdd++));
				}
			}
		}
    }
}
