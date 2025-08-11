/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Servo
{
    public interface IAxisCommand
    {
        void SetPosAsync(double pos);
        void SetSpeedAsync(VelSet set);
        void SetJogSpeedAsync(VelSet set);
        void SetTargetDistance(double distance);
        void SetSpeedOverrideRate(double rate);
        void SetCommandAsync(enAxisOutFlag command);
        void SetHolding(bool hold);
        void SetPause(bool pause);
        void SetSequenceMoveCommand(bool set);
        enAxisInFlag GetAxisCurStatus();
        double GetAxisCurPos();
        double GetAxisCurSpeed();
        double GetAxisCurTorque();
        double GetSpeedOverrideRate();
        double GetAxisCurLeftBarcode();
        double GetAxisCurRightBarcode();
    }
}
