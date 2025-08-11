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
    public class IoTermCrevis_GT226F : _IoTermCrevis
    {
		public IoTermCrevis_GT226F()
		{
			m_ChannelCount = 16;
			m_Name = "GT-226F";
			m_ProductNo = 0x226F;
			m_ChannelBitSize = 0x01;
			m_TermIoType = IoType.DO;
			m_Description = "Digital Output, 16 Points, Source, 24Vdc/0.3A, 18RTB";

			m_IoTypes.Add(IoType.DO);
		}

		public override void CreateChannels()
		{
			int i = 0;
			for (i = 0; i < m_ChannelCount; i++)
			{
				IoChannel ch = new IoChannel();
				ch.IoType = IoType.DO;
				ch.Name = "do__";
				this.Channels.Add(ch);
			}
		}
	}
}
