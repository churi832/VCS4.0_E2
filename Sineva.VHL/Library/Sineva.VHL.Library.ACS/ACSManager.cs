using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.ACS
{
	[Serializable]
	public class ACSManager
	{
		public static readonly ACSManager Instance = new ACSManager();

		#region Field
        private List<ACSAxisCtrl230> m_AxisCtrls230 = new List<ACSAxisCtrl230>();
        #endregion

		#region Properties
		#endregion

		#region Constructor
		private ACSManager()
		{

		}

		public bool Initialize()
		{
			bool rv = true;
			foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
			{
				if (block.ControlFamily == ServoControlFamily.ACS230)
				{
					(block as AxisBlockACS).Initialize(ServoControlFamily.ACS230);
					m_AxisCtrls230.Add(new ACSAxisCtrl230(block as AxisBlockACS));
				}
			}

			foreach (ACSAxisCtrl230 ctrl in m_AxisCtrls230)
			{
				rv &= ctrl.Initialize();
			}
			return rv;
		}
		#endregion
	}
}
