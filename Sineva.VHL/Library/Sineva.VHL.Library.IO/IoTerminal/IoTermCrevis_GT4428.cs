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
    public class IoTermCrevis_GT4428 : _IoTermCrevis
    {
		public IoTermCrevis_GT4428()
		{
			m_ChannelCount = 8;
			m_Name = "GT-4428";
			m_ProductNo = 0x4428;
			m_ChannelBitSize = 0x10;
			m_TermIoType = IoType.AO;
			m_Description = "Analog Output, 8 Channels, 0~10Vdc, 12Bits, 10RTB";

			m_IoTypes.Add(IoType.AI);
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
    }
}
