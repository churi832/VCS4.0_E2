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
using System.Drawing.Design;
using System.ComponentModel;

namespace Sineva.VHL.Library.IO
{
	public interface IChannelCommand
	{
		void SetDo(bool val);
		bool GetDi();
		void SetAo(double val);
		double GetAi();
	}
}
