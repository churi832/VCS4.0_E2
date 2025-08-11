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
using MotionAPI;
using System.Windows.Forms;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.IO;

namespace Sineva.VHL.Library.Mp2100
{
	/// <summary>
	/// dd
	/// </summary>
	[Serializable]
	public class Mp2100Manager
	{
		public static readonly Mp2100Manager Instance = new Mp2100Manager();

		#region Fields
		private List<Mp2100IoCtrl> m_IoCtrls = new List<Mp2100IoCtrl>();
		private List<Mp2100AxisCtrl> m_AxisCtrls = new List<Mp2100AxisCtrl>();
		#endregion

		#region Properties
		#endregion

		#region Constructor
		private Mp2100Manager()
		{

		}
		#endregion

		#region [Methods]
		public bool Initialize()
		{
			bool rv = true;
			rv &= InitializeIo();
			rv &= InitializeAxis();
			return rv;
		}

		public bool InitializeIo()
		{
			foreach (_IoNode ioNode in IoManager.Instance.Nodes)
			{
				if (ioNode.GetType().ToString() == typeof(IoNodeMp2100).ToString())
				{
					m_IoCtrls.Add(new Mp2100IoCtrl((IoNodeMp2100)ioNode));
				}
			}

			bool rv = true;
			foreach (Mp2100IoCtrl ioCtrl in m_IoCtrls)
			{
				rv &= ioCtrl.InitializeIo();
			}

			return rv;
		}
		public bool InitializeAxis()
		{
			// Mp2100(C) Axis Block Initialize
			foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
			{
				if (block.ControlFamily == ServoControlFamily.YMp2100)
				{
					(block as AxisBlockMp2100).Initialize();
					m_AxisCtrls.Add(new Mp2100AxisCtrl(block as AxisBlockMp2100));
				}
			}

			bool rv = true;
			foreach (Mp2100AxisCtrl ctrl in m_AxisCtrls)
			{
				rv &= ctrl.Initialize();
			}

			return true;
		}
		#endregion
	}
}
