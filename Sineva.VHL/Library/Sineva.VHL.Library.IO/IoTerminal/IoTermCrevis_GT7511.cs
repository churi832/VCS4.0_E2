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
    public class IoTermCrevis_GT7511 : _IoTermCrevis
    {
		public IoTermCrevis_GT7511()
		{
			m_ChannelCount = 0;
			m_Name = "GT-7511";
			m_ProductNo = 0x7511;
			m_ChannelBitSize = 0x10;
			m_TermIoType = IoType.AI;
			m_Description = "Power Expansion, In 24Vdc, Out 1A/5Vdc";

			m_IoTypes.Add(IoType.AI);
		}

		public override void CreateChannels()
		{
		}
    }
}
