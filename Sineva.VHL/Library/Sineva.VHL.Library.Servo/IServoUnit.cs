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

namespace Sineva.VHL.Library.Servo
{
	public interface IServoUnit
	{
        List<_Axis> GetAxes();
		string GetName();
        VelSet GetVel(_Axis axis, int prop);
		double GetCurAxisPoint(_Axis axis, int point);
		enAxisResult MoveStart(int point, int prop);
		enAxisResult MoveRelativeStart(List<double> posOffset, int prop);
		enAxisResult MoveRelativeStart(int prop, params double[] posOffset);
		enAxisResult MoveRelativeStart(_Axis axis, double Offset, VelSet set);
		enAxisResult MoveRelativeStart(_Axis axis, double Offset, int prop);
        enAxisResult MoveAxisStart(_Axis axis, int point, int prop);
		enAxisResult MoveAxisStart(_Axis axis, double pos, VelSet set);
        enAxisResult ContinuousMoveAxisStart(_Axis axis, double pos, VelSet set);
        enAxisResult SetTargetPosition(_Axis axis, int point, int prop);
        enAxisResult SetTargetPosition(_Axis axis, double pos, VelSet set);
        enAxisResult MotionDone();
		enAxisResult MotionDoneAxis(_Axis axis);
		int GetStartAlarmId();
		int GetStartAlarmId(_Axis axis);
        enAxisResult JogMove(enAxisOutFlag nCmd, VelSet set);
        enAxisResult JogMove(_Axis axis, enAxisOutFlag nCmd, VelSet set);
        enAxisResult JogStop(_Axis axis);
		void ResetCommand();
        bool ResetJogSpeed();
		bool IsServoReady();
        bool IsServoOn();
		int GetCurPoint();
		int GetCurPoint(_Axis axis);
		enAxisResult Home();
		enAxisResult Home(_Axis axis);
		enAxisResult ServoOn();
		enAxisResult ServoOn(_Axis axis);
		enAxisResult ServoOff();
		enAxisResult ServoOff(_Axis axis);
		enAxisResult Stop();
		enAxisResult Stop(_Axis axis);
		enAxisResult AlarmClear();
		enAxisResult AlarmClear(_Axis axis);
	}
}
