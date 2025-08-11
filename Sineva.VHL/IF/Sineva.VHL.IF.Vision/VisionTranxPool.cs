using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sineva.VHL.Library;
using Sineva.VHL.Library.EasySocket;

namespace Sineva.VHL.IF.Vision
{
	/// <summary>
	/// T1 : Trigger1
	/// </summary>
	public class V_Trigger1 : VisionCommand
	{
		public V_Trigger1()
			: base(enVisionCommandCode.T1, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}

		/// <summary>
		/// data = 0,1,2,3,4,5,6,7,8
		/// 0 : Command or Error
		/// 1 : 위X (1번 Camera)
		/// 2 : 위Y (1번 Camera)
		/// 3 : 아래X (1번 Camera)
		/// 4 : 아래Y (1번 Camera)
		/// 5 : 위X (2번 Camera)
		/// 6 : 위Y (2번 Camera)
		/// 7 : 아래X (2번 Camera)
		/// 8 : 아래Y (2번 Camera)
		/// 1번 Camera : 좁은 범위
		/// 2번 Camera : 넓은 범위
		/// </summary>
		/// <param name="outP"></param>
		/// <returns></returns>
		public override enVisionResult IsSecondaryRcvd(ref XyPosition Camera1_Left, ref XyPosition Camera1_Right, ref XyPosition Camera2_Left_Down, ref XyPosition Camera2_Left_Up, ref XyPosition Camera2_Right_Down, ref XyPosition Camera2_Right_Up)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					string[] MsgBlocks = null;
					MsgBlocks = data.Split(',');
					if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
					else
					{
						// MsgBlocks[0] 
						Camera1_Left.X = Convert.ToDouble(MsgBlocks[1]);
						Camera1_Left.Y = Convert.ToDouble(MsgBlocks[2]);
						Camera1_Right.X = Convert.ToDouble(MsgBlocks[3]);
						Camera1_Right.Y = Convert.ToDouble(MsgBlocks[4]);
						Camera2_Left_Down.X = Convert.ToDouble(MsgBlocks[5]);
						Camera2_Left_Down.Y = Convert.ToDouble(MsgBlocks[6]);
						Camera2_Left_Up.X = Convert.ToDouble(MsgBlocks[7]);
						Camera2_Left_Up.Y = Convert.ToDouble(MsgBlocks[8]);
						Camera2_Right_Down.X = Convert.ToDouble(MsgBlocks[9]);
						Camera2_Right_Down.Y = Convert.ToDouble(MsgBlocks[10]);
						Camera2_Right_Up.X = Convert.ToDouble(MsgBlocks[11]);
						Camera2_Right_Up.Y = Convert.ToDouble(MsgBlocks[12]);

						rv = enVisionResult.OK;
					}
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// T2 : Trigger2
	/// </summary>
	public class V_Trigger2 : VisionCommand
	{
		public V_Trigger2()
			: base(enVisionCommandCode.T2, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd(ref XyPosition outP)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					string[] MsgBlocks = null;
					MsgBlocks = data.Split(',');
					if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
					else
					{
						outP.X = Convert.ToDouble(MsgBlocks[1]);
						outP.Y = Convert.ToDouble(MsgBlocks[2]);

						rv = enVisionResult.OK;
					}
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// T3 : Trigger3
	/// </summary>
	public class V_Trigger3 : VisionCommand
	{
		public V_Trigger3()
			: base(enVisionCommandCode.T3, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd(ref XyPosition outP)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					string[] MsgBlocks = null;
					MsgBlocks = data.Split(',');
					if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
					else
					{
						outP.X = Convert.ToDouble(MsgBlocks[1]);
						outP.Y = Convert.ToDouble(MsgBlocks[2]);

						rv = enVisionResult.OK;
					}
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// T4 : Trigger4
	/// </summary>
	public class V_Trigger4 : VisionCommand
	{
		public V_Trigger4()
			: base(enVisionCommandCode.T4, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd(ref XyPosition outP)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					string[] MsgBlocks = null;
					MsgBlocks = data.Split(',');
					if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
					else
					{
						outP.X = Convert.ToDouble(MsgBlocks[1]);
						outP.Y = Convert.ToDouble(MsgBlocks[2]);

						rv = enVisionResult.OK;
					}
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// TA : Trigger ALL
	/// </summary>
	public class V_TriggerAll : VisionCommand
	{
		public V_TriggerAll()
			: base(enVisionCommandCode.TA, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
			}

			return rv;
		}
	}

	/// <summary>
	/// R0 : Driving Mode
	/// </summary>
	public class V_DrivingMode : VisionCommand
	{
		public V_DrivingMode()
			: base(enVisionCommandCode.R0, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				string[] MsgBlocks = null;
				MsgBlocks = data.Split(',');
				if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
				else
				{
					rv = enVisionResult.OK;
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// S0 : Setting Mode
	/// </summary>
	public class V_SettingMode : VisionCommand
	{
		public V_SettingMode()
			: base(enVisionCommandCode.S0, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				string[] MsgBlocks = null;
				MsgBlocks = data.Split(',');
				if (MsgBlocks[0] == "ER") rv = enVisionResult.NG;
				else
				{
					rv = enVisionResult.OK;
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// RS : Reset
	/// </summary>
	public class V_Reset : VisionCommand
	{
		public V_Reset()
			: base(enVisionCommandCode.RS, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// RB : ReBooting
	/// </summary>
	public class V_ReBooting : VisionCommand
	{
		public V_ReBooting()
			: base(enVisionCommandCode.RB, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// SS : Setting Save
	/// </summary>
	public class V_SettingSave : VisionCommand
	{
		public V_SettingSave()
			: base(enVisionCommandCode.SS, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// CE : ERROR CLEAR
	/// </summary>
	public class V_ErrorClear : VisionCommand
	{
		public V_ErrorClear()
			: base(enVisionCommandCode.CE, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// VW : 지정한 운전 화면 및 카메라 화면으로 표시를 전환
	/// </summary>
	public class V_SwitchDisplay : VisionCommand
	{
		public V_SwitchDisplay()
			: base(enVisionCommandCode.VW, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string ScreenNo)
		{
			string sCmd = string.Format("{0},{1}", ScreenNo.ToString(), dev.ToString());
			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// RE : Trigger Reset
	/// 여러번 촬상을 유효로 하고 있는 경우 1회의 계측 도중까지 트리거가 입력 된 상태를 해제합니다.
	/// 실행중인 계측의 촬상화상 및 계측 결과를 삭제하고 계측 실행 전의 상태로 돌아갑니다.
	/// </summary>
	public class V_TriggerReset : VisionCommand
	{
		public V_TriggerReset()
			: base(enVisionCommandCode.RE, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd(ref string outData)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
				this.m_SecondaryIn = false;
				if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					outData = data;
					rv = enVisionResult.OK;
				}
			}

			return rv;
		}
	}

	/// <summary>
	/// RM : Read Mode
	/// 컨트롤러의 상태 (운전 모드/ 설정 모드)를 읽습니다.
	/// </summary>
	public class V_ReadMode : VisionCommand
	{
		public V_ReadMode()
			: base(enVisionCommandCode.RM, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// PW : PW 검사 설정 전환
	/// 열려 있는 다이얼로그를 전부 닫아 지정된 SD카드의 nnn번으로 설정을 전환합니다.
	/// - 변경 되어 있는 설정 데이터가 있어도 저장하지 않고 파기 합니다.
	/// - 검사 설정의 전환에 성공하였을 경우에는 전환 실행 후에 환경 설정 파일을 저장합니다.
	/// </summary>
	public class V_InspectionSetting : VisionCommand
	{
		public V_InspectionSetting()
			: base(enVisionCommandCode.PW, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string SDcardNo, string InspectionNo)
		{
			string sCmd = string.Format("{0},{1}", SDcardNo.ToString(), InspectionNo.ToString());
			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
			}

			return rv;
		}
	}

	/// <summary>
	/// PR : PR 검사 설정 읽기
	/// 현재 불러오고 있는 설정의 SD 카드 번호, 검사 설정을 반환합니다.
	/// </summary>
	public class V_InspectionRead : VisionCommand
	{
		public V_InspectionRead()
			: base(enVisionCommandCode.PR, true, false)
		{
		}

		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int SDNo = 0;
							int inspectionNo = 0;
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (int.TryParse(splitText[1], out temp)) { SDNo = temp; DataOk &= true; }
							else { SDNo = 0; DataOk &= false; }
							if (int.TryParse(splitText[2], out temp)) { inspectionNo = temp; DataOk &= true; }
							else { inspectionNo = 0; DataOk &= false; }



							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
                        rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}
	}

	/// <summary>
	/// CSH : 셔터 스피드 설정
	/// 지정한 카메라의 셔터 스피드를 변경합니다.
	/// 0:1/15
	/// 1:1/30
	/// 2:1/60
	/// 3:1/120
	/// 4:1/240
	/// 5:1/500
	/// 6:1/1000
	/// 7:1/2000
	/// 8:1/5000
	/// 9:1/10000
	/// 10:1/20000
	/// 11:1/50000*1
	/// 12:1/100000*1
	/// </summary>
	public class V_ShutterSpeed : VisionCommand
	{
		public V_ShutterSpeed()
			: base(enVisionCommandCode.CSH, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string shutterspeed)
		{
			string sCmd = "";

			sCmd = string.Format("{0},{1}", dev.ToString(), shutterspeed.ToString());

			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
                        rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}
	}

	/// <summary>
	/// CSE : 카메라 감도 설정
	/// 지정한 카메라의 감도를 변경합니다.
	/// </summary>
	public class V_CameraSensitivity : VisionCommand
	{
		public V_CameraSensitivity()
			: base(enVisionCommandCode.CSE, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string sensitivity)
		{
			string sCmd = "";

			sCmd = string.Format("{0},{1}", dev.ToString(), sensitivity.ToString());

			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}
	}

	/// <summary>
	/// CTD : Trigger Delay Setting
	/// Trigger 입력에 대해 실제로 촬상이 개시 될 때까지의 Delay Time을 설정합니다.
	/// </summary>
	public class V_TriggerDelay : VisionCommand
	{
		public V_TriggerDelay()
			: base(enVisionCommandCode.CTD, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string triggerdelay)
		{
			string sCmd = "";

			sCmd = string.Format("{0},{1}", dev.ToString(), triggerdelay.ToString());

			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}

	}

	/// <summary>
	/// BS : BS 기준 화상 등록
	/// 최신의 입력 화상을 번호 nnn의 기준 화상으로서 저장하여, 저장한 기준 화상으로 기준값을 계산합니다.
	/// 인수의 지정이 없는 경우에는 현재의 기준 화상으로 기준값을 재계산 합니다.
	/// </summary>
	public class V_BasicScreen : VisionCommand
	{
		public V_BasicScreen()
			: base(enVisionCommandCode.BS, true, false)
		{
		}

		public override bool SendPrimary(enVisionDevice dev, string triggerdelay)
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}

	}

	/// <summary>
	/// EXW : 실행조건 쓰기
	/// 현재 유효한 실행 조건 번호를 지정한 조건 번호로 변경합니다.
	/// </summary>
	public class V_ExecutionCondition : VisionCommand
	{
		public V_ExecutionCondition()
			: base(enVisionCommandCode.EXW, true, false)
		{
		}

		public override bool SendPrimary(string conditionNo)
		{
			string sCmd = "";

			sCmd = string.Format("{0},{1}", conditionNo.ToString());

			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}

	}

	/// <summary>
	/// EXW : 실행조건 쓰기
	/// 현재 유효한 실행 조건 번호를 지정한 조건 번호로 변경합니다.
	/// </summary>
	public class V_ExecutionConditionRead : VisionCommand
	{
		public V_ExecutionConditionRead()
			: base(enVisionCommandCode.EXR, true, false)
		{
		}

		public override bool SendPrimary(string conditionNo)
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							int commandNo = 0;

							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (int.TryParse(splitText[1], out temp)) { commandNo = temp; DataOk &= true; }
							else { commandNo = 0; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}

	}

	/// <summary>
	/// TE : Trigger 입력 허가/금지
	/// "TE,0"을 실행하면 READY 단자가 상시 OFF 상태로 되며, 트리거 입력은 일절 접수하지 않게 됩니다.'
	/// "TE, 1"을 실행하면 허가상태로 돌아갑니다.
	/// 0: 트리거 입력 금지
	/// 1: 트리거 입력 허가
	/// </summary>
	public class V_ChangeTriggerInterlock : VisionCommand
	{
		public V_ChangeTriggerInterlock()
			: base(enVisionCommandCode.TE, false, false)
		{
		}

		public override bool SendPrimary(string interlock)
		{
			string sCmd = "";

			if(interlock == "lock")
				sCmd = string.Format("0");
			else
				sCmd = string.Format("1");

			return base._SendPrimary(sCmd);
		}
		public override enVisionResult IsSecondaryRcvd()
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK)
			{
                this.m_SecondaryIn = false;
                if (data == "0") rv = enVisionResult.OK;            //성공
				else if (data == "22") rv = enVisionResult.NG;      //필요없는 파라매터가 붙어 있을 때
				else if (data == "03") rv = enVisionResult.NG;      //접수 될수 없는 타이밍 또는 다른 동작 금지 상태일 때
				else
				{
					try
					{
						char[] splitter = { ',' };
						string[] splitText = data.Split(splitter);
						if (splitText.Length >= 2)
						{
							int rv1 = -1;
							int temp = 0;
							bool DataOk = true;

							if (int.TryParse(splitText[0], out temp)) { rv1 = temp; DataOk &= true; }
							else { rv1 = -1; DataOk &= false; }

							if (DataOk == false) rv = enVisionResult.NG;
							if (rv1 != 0) rv = enVisionResult.NG;
						}
						else
						{
							rv = enVisionResult.NG;
						}
					}
					catch (Exception err)
					{
						rv = enVisionResult.NG;
                        ExceptionLog.WriteLog(err.ToString());
                    }
                }
			}

			return rv;
		}

	}

	/// <summary>
	/// VI : Version Information Read
	/// 컨트롤러의 시스템 정보(모델, ROM 버전)를 반환합니다.
	/// Command Test용으로 쓰기에 좋을 듯...
	/// </summary>
	public class V_VersionInformationRead : VisionCommand
	{
		public V_VersionInformationRead()
			: base(enVisionCommandCode.VI, true, false)
		{
		}
		public override bool SendPrimary()
		{
			return base._SendPrimary("");
		}
		public override enVisionResult IsSecondaryRcvd(ref string outData)
		{
			enVisionResult rv = enVisionResult.WAIT;

			string data = "";
			rv = base._IsSecondaryRcvd(ref data);
			if (rv == enVisionResult.OK && data != null)
			{
                this.m_SecondaryIn = false;
                if (data == "NG") rv = enVisionResult.NG;
				else if (data == "OK") rv = enVisionResult.OK;
				else
				{
					string[] MsgBlocks = null;
					MsgBlocks = data.Split(',');

					outData = MsgBlocks[2];
					rv = enVisionResult.OK;
				}
			}

			return rv;
		}

	}
}
