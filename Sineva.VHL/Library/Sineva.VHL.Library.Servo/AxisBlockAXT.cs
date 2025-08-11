using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    public class AxisBlockAXT : _AxisBlock
    {
        #region Field
        private bool m_Connected = false;
        private int m_BoardNo = 0;
        private FileSelect m_MotFilePath = new FileSelect();//AppConfig.DefaultConfigFilePath + "\\AXT_PARAM.mot";
        #endregion

        #region Property
        public int BoardNo
        {
            get { return m_BoardNo; }
            set { m_BoardNo = value; }
        }
        [Browsable(false), XmlIgnore()]
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }

        public FileSelect MotPath
        {
            get { return m_MotFilePath; }
            set { m_MotFilePath = value; }
        }
        #endregion

        #region Constructor
        public AxisBlockAXT()
        {
            this.ControlFamily = ServoControlFamily.AXT;
        }
        #endregion

        #region Override
        #endregion

        #region Method
        public void Initialize()
        {
            this.ControlFamily = ServoControlFamily.AXT;
        }
        public int GetAxisCount()
        {
            return m_Axes.Count;
        }

        public bool TryGetAxis(int nodeId, out _Axis axis)
        {
            axis = null;
            for (int i = 0; i < m_Axes.Count; i++)
            {
                if (m_Axes[i].NodeId == nodeId) axis = m_Axes[i];
            }

            return axis != null;
        }
        public enAxisInFlag GetAxisStatus(int nodeId)
        {
            for (int i = 0; i < m_Axes.Count; i++)
            {
                if (m_Axes[i].NodeId == nodeId)
                    return (m_Axes[i] as MpAxis).AxisStatus;
            }
            return enAxisInFlag.None;
        }
        public enHomeMethod GetAxisHomeMethod(int nodeId)
        {
            for (int i = 0; i < m_Axes.Count; i++)
            {
                if (m_Axes[i].NodeId == nodeId)
                    return m_Axes[i].HomeMethod;
            }

            return enHomeMethod.NONE;
        }

        public ushort GetMotionCommand(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).Command;
        }

        public double GetTargetPosition(int axisId)
        {
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetPos;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
        }

        public double GetTargetSpeed(int axisId)
        {
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetSpeed;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
        }

        public double GetJogSpeed(int axisId)
        {
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpJogSpeed;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
        }

        public double GetTargetAcc(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetAcc;
        }

        public void SetMotionState(int axisId, enAxisInFlag state)
        {
            (m_Axes[axisId - StartAxisId] as MpAxis).AxisStatus = state;
        }

        public void SetCurPosition(int axisId, double pos)
        {
            double curPos = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(pos);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurPos = curPos;
        }

        public void SetCurTorque(int axisId, double torque)
        {
            int curTorque = (int)(m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(torque);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurTorque = curTorque;
        }

        public void SetCurSpeed(int axisId, double speed)
        {
            int curSpeed = (int)(m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(speed);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurSpeed = curSpeed;
        }
        #endregion
    }
}
