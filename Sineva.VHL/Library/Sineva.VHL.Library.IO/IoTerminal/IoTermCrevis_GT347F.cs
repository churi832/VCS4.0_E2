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
    public class IoTermCrevis_GT347F : _IoTermCrevis
    {
		public IoTermCrevis_GT347F()
		{
			m_ChannelCount = 16;
			m_Name = "GT-347F";
			m_ProductNo = 0x347F;
			m_ChannelBitSize = 0x10;
			m_TermIoType = IoType.AI;
			m_Description = "Analog Input, 16 Channels, (0~10/0~5/1~5)Vdc, 12Bits, 18RTB";

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
