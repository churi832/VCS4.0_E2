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
    public class IoTermCrevis_GT2328 : _IoTermCrevis
    {
		public IoTermCrevis_GT2328()
		{
			m_ChannelCount = 8;
			m_Name = "GT-2328";
			m_ProductNo = 0x2328;
			m_ChannelBitSize = 0x01;
			m_TermIoType = IoType.DO;
			m_Description = "Digital Output, 8 Points, Source, 24Vdc/0.5A, 10RTB";

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
