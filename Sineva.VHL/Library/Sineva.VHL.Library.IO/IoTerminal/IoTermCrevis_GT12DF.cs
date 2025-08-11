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
    public class IoTermCrevis_GT12DF : _IoTermCrevis
    {
		public IoTermCrevis_GT12DF()
		{
			m_ChannelCount = 16;
			m_Name = "GT-12DF";
			m_ProductNo = 0x12DF;
			m_ChannelBitSize = 0x01;
			m_TermIoType = IoType.DI;
			m_Description = "Digital Input, 16 Points, Universal, 24Vdc, 18RTB";


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
    }
}
