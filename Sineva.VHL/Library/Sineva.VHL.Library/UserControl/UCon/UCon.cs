using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	public class UCon : UserControl
	{
		#region Fields
		private OperateMode m_EnalbeOperateModeLevel = OperateMode.Manual;
		#endregion

		#region Properties
		#endregion

		#region Constructor
		public UCon()
		{
		}

		public UCon(OperateMode enableLevel)
		{
			m_EnalbeOperateModeLevel = enableLevel;
			EventHandlerManager.Instance.OperateModeChanged += Instance_OperateModeChanged;
            EventHandlerManager.Instance.RunModeChanged += Instance_RunModeChanged;
		}
		#endregion

		#region Methods
		public virtual void Instance_OperateModeChanged(OperateMode opMode)
		{
			if (this.InvokeRequired)
			{
				DelVoid_OperateMode d = new DelVoid_OperateMode(Instance_OperateModeChanged);
				this.Invoke(d, opMode);
			}
			else
			{
				if (m_EnalbeOperateModeLevel == OperateMode.Manual)
				{
					if (opMode == OperateMode.Manual)
					{
						this.Enabled = true;
					}
					else // SemiAuto, Auto
					{
						this.Enabled = false;
					}
				}
				if (m_EnalbeOperateModeLevel == OperateMode.SemiAuto)
				{
					if (opMode == OperateMode.SemiAuto)
					{
						this.Enabled = true;
					}
					else // Auto Mode
					{ 
						this.Enabled = false;
					}
				}
				if (m_EnalbeOperateModeLevel == OperateMode.Auto)
				{
					if (opMode == OperateMode.SemiAuto || opMode == OperateMode.Manual)
					{
						this.Enabled = true;
					}
					else // Auto Mode
					{
						if (EqpStateManager.Instance.RunMode == EqpRunMode.Start)
							this.Enabled = false;
						else this.Enabled = true;
					}
				}
				CancelOperation();
            }
		}

		public virtual void Instance_RunModeChanged(EqpRunMode runMode)
		{
			if (this.InvokeRequired)
			{
				DelVoid_EqpRunMode d = new DelVoid_EqpRunMode(Instance_RunModeChanged);
				this.Invoke(d, runMode);
			}
			else
			{
				try
				{
					if (m_EnalbeOperateModeLevel == OperateMode.Auto)
					{
						if (runMode == EqpRunMode.Start)
							this.Enabled = false;
						else this.Enabled = true;
					}
					if (runMode == EqpRunMode.Abort)
					{
						CancelOperation();
					}
				}
				catch (Exception ex)
				{
					ExceptionLog.WriteLog(ex.ToString());
				}
			}
		}
		public virtual void CancelOperation()
		{
		}
		#endregion
	}
}
