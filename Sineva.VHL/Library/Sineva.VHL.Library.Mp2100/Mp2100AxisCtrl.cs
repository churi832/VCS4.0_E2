using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using MotionAPI;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.Mp2100
{
    public class Mp2100AxisCtrl
    {
        #region Field
		private AxisBlockMp2100 m_AxisBlock = null;

		private static object m_Lock = new object();

        private UInt32 m_CtrlHandle = 0;
        private CMotionAPI.COM_DEVICE m_ComDevice;
        private bool m_OpenComp = false;
        private bool m_Use = false;
        private bool m_OpenFail = false;

        private bool m_OneTimeSimulatorCheck = true;
        #endregion

        #region Property
        #endregion

        #region Constructor
        public Mp2100AxisCtrl(AxisBlockMp2100 block)
        {
            #if WIN64PCIe
			m_ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCIe_MODE;
            #else
			m_ComDevice.ComDeviceType = (UInt16)CMotionAPI.ApiDefs.COMDEVICETYPE_PCI_MODE;
            #endif
			m_ComDevice.PortNumber = 1;
            m_ComDevice.CpuNumber = (ushort)block.CpuNo;
            m_ComDevice.NetworkNumber = 0;
            m_ComDevice.StationNumber = 0;
            m_ComDevice.UnitNumber = 0;
            m_ComDevice.IPAddress = "";
            m_ComDevice.Timeout = block.DeviceTimeout;

            m_AxisBlock = block;
        }
		public bool Initialize()
		{
			TaskHandler.Instance.RegTask(new TaskMp2100Interface(this), 10, System.Threading.ThreadPriority.Normal);
			return true;
		}
        #endregion

        #region [PCI Methods]
        private uint OpenAxisController()
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            rv = CMotionAPI.ymcOpenController(ref m_ComDevice, ref m_CtrlHandle);
            if(rv == CMotionAPI.MP_SUCCESS)
            {
                rv = CMotionAPI.ymcSetAPITimeoutValue(3000);
                if(rv == CMotionAPI.MP_SUCCESS)
                {
                    m_OpenComp = true;
                    return CMotionAPI.MP_SUCCESS;
                }
                else
                {
                    MessageBox.Show(String.Format("Mp2100 SetApiTimeout Error(Code = 0x{0:X})", rv));
                    return rv;
                }
            }
            else
            {
                MessageBox.Show(String.Format("Mp2100 Open Error(Code = 0x{0:X})", rv));
                return rv;
            }
            return rv;           
        }

        public uint CloseController()
        {
            UInt32 rv = CMotionAPI.MP_SUCCESS;
            try
            {
                rv = CMotionAPI.ymcCloseController(m_CtrlHandle);
                m_CtrlHandle = 0;
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.Message);
                rv = CMotionAPI.MP_FAIL;
            }
            return rv;
        }

		private uint GetRegisterDataHandle(string registerName, ref uint hRegisterData)
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            try
            {
                rv = CMotionAPI.ymcGetRegisterDataHandle(registerName, ref hRegisterData);
            }
            catch (Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
                rv = CMotionAPI.MP_FAIL;
            }
            return rv;
        }

		private uint GetRegisterData(UInt32 registerDataHandle, UInt32 registerDataNo, UInt16[] registerShortData, ref UInt32 readDataNo)
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            try
            {
                rv = CMotionAPI.ymcGetRegisterData(registerDataHandle, registerDataNo, registerShortData, ref readDataNo);
            }
            catch(Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
                rv = CMotionAPI.MP_FAIL;
            }
            return rv;
        }

		private uint SetRegisterData(UInt32 registerDataHandle, UInt32 registerDataNo, UInt16[] registerShortData)
        {
            uint rv = CMotionAPI.MP_SUCCESS;
            try
            {
                rv = CMotionAPI.ymcSetRegisterData(registerDataHandle, registerDataNo, registerShortData);
            }
            catch(Exception err)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, err.ToString());
                rv = CMotionAPI.MP_FAIL;
            }
            return rv;
        }

		private void WriteRegViaBoard()
		{
			string regName = string.Empty;
			uint regDataHandle = 0x00000000;

			// Write Register To MP2100
			regDataHandle = 0x00000000;
			regName = string.Empty;// io manager 만들고 고쳐야함. IO._IoTermMp.enMpDeviceName.MW.ToString() + string.Format("{0:00000}", m_AxisBlock.WriteStartAddress);
            uint rv = GetRegisterDataHandle(regName, ref regDataHandle);
			if (rv == CMotionAPI.MP_SUCCESS && regDataHandle != (uint)0)
			{
				uint regDataNo = (uint)(3 + 7 * m_AxisBlock.AxisCount);  // HeartBit (1Word), AN2900 (1Word), Tens Intr (1Word), Axis Data (7Word)
				uint buildDataSize = 0;
				UInt16[] regShortData = m_AxisBlock.BuildRegWriteData(ref buildDataSize);
				if (buildDataSize != regDataNo)
				{
					// Data Size가 일치하지 않을 경우... 일단은 쓰지말까?
				}
				else
				{
					if ((rv = SetRegisterData(regDataHandle, regDataNo, regShortData)) != CMotionAPI.MP_SUCCESS)
					{
						m_OpenComp = false;
						m_OpenFail = true;
						m_AxisBlock.Connected = false;
                        System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                        ExceptionLog.WriteLog(method, String.Format("Mp2100 {0} SetRegisterData Error(Code = 0x{1:X})", regName, rv));
					}

					regShortData = null;
				}
			}
			else if (AppConfig.Instance.Simulation.IO == false)
			{
				m_OpenComp = false;
				m_OpenFail = true;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format("Mp2100 {0} GetRegisterData Error(Code = 0x{1:X})", regName, rv));
			}
		}

		private void ReadRegViaBoard()
		{
			string regName = string.Empty;
			uint regDataHandle = 0x00000000;

            // Read Register From MP2100
            regName = string.Empty;// io manager 만들고 고쳐야함. IO._IoTermMp.enMpDeviceName.MW.ToString() + string.Format("{0:00000}", m_AxisBlock.ReadStartAddress);
			uint rv = GetRegisterDataHandle(regName, ref regDataHandle);
			if (rv == CMotionAPI.MP_SUCCESS)
			{
				uint regDataNo = (uint)(3 + 7 * m_AxisBlock.AxisCount);  // HeartBit (1Word), AN2900 (1Word), Tens Intr (1Word), Axis Data (7Word)
				UInt16[] regShortData = new UInt16[regDataNo];
				uint readDataNo = 0;

				if ((rv = GetRegisterData(regDataHandle, regDataNo, regShortData, ref readDataNo)) == CMotionAPI.MP_SUCCESS)
				{
					m_AxisBlock.ParseRegReadResponse(regShortData);
					m_AxisBlock.Connected = true;
				}
				else if (AppConfig.Instance.Simulation.IO == false)
				{
					m_OpenComp = false;
					m_OpenFail = true;
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, String.Format("Mp2100 {0} GetRegisterData Error(Code = 0x{1:X})", regName, rv));
				}
			}
			else
			{
				m_OpenComp = false;
				m_OpenFail = true;
				m_AxisBlock.Connected = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, String.Format("Mp2100 {0} GetRegisterDataHandel Error(Code = 0x{1:X})", regName, rv));
			}
		}

		private void UpdateAxis()
		{
			if (!m_OpenComp)
			{
				if (CMotionAPI.MP_SUCCESS != OpenAxisController())
				{
					m_OpenFail = true;
					m_AxisBlock.Connected = false;
					return;
				}
			}
			ReadRegViaBoard();
			System.Threading.Thread.Sleep(1);
			if (m_AxisBlock.MpFromTenInterlock == 0) // 임시로....Heart Bit OFF 일때만 Write.....hjyou
			{
				WriteRegViaBoard();
			}
		}

        private void UpdateAxisSimulator()
        {
            int axis_count = m_AxisBlock.AxisCount - m_AxisBlock.AxisExcluedCount;
            for (int i = 0; i < axis_count; i++)
            {
                _Axis axis = m_AxisBlock.Axes[i];

                enAxisOutFlag command = (enAxisOutFlag)(axis as MpAxis).Command;
                if (Convert.ToBoolean(command & enAxisOutFlag.ServoOn))
                {
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.SvOn) != enAxisInFlag.SvOn) (axis as MpAxis).AxisStatus |= enAxisInFlag.SvOn;
                }
                else if (Convert.ToBoolean(command & enAxisOutFlag.ServoOff))
                {
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn) (axis as MpAxis).AxisStatus ^= enAxisInFlag.SvOn;
                }
                else if (Convert.ToBoolean(command & enAxisOutFlag.JogMinus)) (axis as MpAxis).CurPos--;
                else if (Convert.ToBoolean(command & enAxisOutFlag.JogPlus)) (axis as MpAxis).CurPos++;
                else if (Convert.ToBoolean(command & enAxisOutFlag.MotionStart)) (axis as MpAxis).CurPos = (axis as MpAxis).TargetPos;
                else if (Convert.ToBoolean(command & enAxisOutFlag.HomeStart))
                {
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.HEnd) != enAxisInFlag.HEnd) (axis as MpAxis).AxisStatus |= enAxisInFlag.HEnd;
                }
                else if (Convert.ToBoolean(command & enAxisOutFlag.ZeroSet)) (axis as MpAxis).HomeOffset = (axis as MpAxis).HomeOffset;

                if (command == enAxisOutFlag.CommandNone ||
                    command == enAxisOutFlag.MotionStop)
                {
                    bool inPos = true;
                    inPos &= (((axis as MpAxis).AxisStatus & enAxisInFlag.SvOn) == enAxisInFlag.SvOn) ? true : false;
                    inPos &= (((axis as MpAxis).AxisStatus & enAxisInFlag.InPos) != enAxisInFlag.InPos) ? true : false;
                    if (inPos) (axis as MpAxis).AxisStatus |= enAxisInFlag.InPos;
                }
                else
                {
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.InPos) == enAxisInFlag.InPos) (axis as MpAxis).AxisStatus ^= enAxisInFlag.InPos;
                }

                if (m_OneTimeSimulatorCheck)
                {
                    // 무조건 servo On 상태로 만들자....
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.SvOn) != enAxisInFlag.SvOn) (axis as MpAxis).AxisStatus |= enAxisInFlag.SvOn;
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.InPos) != enAxisInFlag.InPos) (axis as MpAxis).AxisStatus |= enAxisInFlag.InPos;
                    if (((axis as MpAxis).AxisStatus & enAxisInFlag.HEnd) != enAxisInFlag.HEnd) (axis as MpAxis).AxisStatus |= enAxisInFlag.HEnd;
                }
            }
            m_OneTimeSimulatorCheck = false;
        }
        #endregion

        #region Thread
		public class TaskMp2100Interface : XSequence
        {
            SeqUpdate m_SeqUpdate = null;
            Mp2100AxisCtrl m_Control = null;
            public TaskMp2100Interface(Mp2100AxisCtrl control)
            {
                m_Control = control;
                m_SeqUpdate = new SeqUpdate(control);
                RegSeq(m_SeqUpdate);
            }

            protected override void ExitRoutine()
            {
                m_Control.CloseController();
            }
        }
        class SeqUpdate: XSeqFunc
        {
            Mp2100AxisCtrl m_Control = null;

            public SeqUpdate(Mp2100AxisCtrl ctrl)
            {
                m_Control = ctrl;
            }

            public override int Do()
            {
                if (AppConfig.Instance.Simulation.MP)
                {
                    m_Control.UpdateAxisSimulator();
                    return -1;
                }

                m_Control.UpdateAxis();
                return -1;
            }
        }
        #endregion  
    }
}
