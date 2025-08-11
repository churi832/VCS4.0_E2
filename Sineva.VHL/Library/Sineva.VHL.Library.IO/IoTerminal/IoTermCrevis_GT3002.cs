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
    /// 
    /// </summary>
    [Serializable]
    public class IoTermCrevis_GT3002 : _IoTermCrevis
    {
		public IoTermCrevis_GT3002()
		{
			m_ChannelCount = 2;
			m_Name = "GT-3002";
			m_ProductNo = 0x3002;
			m_ChannelBitSize = 0x20;
			m_TermIoType = IoType.AI;
			m_Description = "Analog Input, 2 Channels, Loadcell Input unit, strain gauge";

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
    }
}
