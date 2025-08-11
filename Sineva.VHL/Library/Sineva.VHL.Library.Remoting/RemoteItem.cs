using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Remoting
{
    [Serializable]
    public class RemoteItem
    {
        public enum DeviceType
        {
            None = 0,
            SlaveTransfer = 1,
            MasterTransfer = 2,
            Slide = 3,
            Rotate = 4,
            Hoist = 5,
            FrontAntiDrop = 6,
            RearAntiDrop = 7,
            FrontSteer = 8,
            RearSteer = 9,

        };
        public enum ActionType
        {
            None = 0,
            JogPlus = 1,
            JogMinus = 2,
            JogStop = 3,
            ServoOn = 4,
            ServoOff = 5,
            ErrorReset = 6,
            SteerLeft = 7, 
            SteerRight = 8,
        }
        public enum VelocitySelect
        {
            Slow,
            Mid,
            High,
        };
        private DeviceType m_DeviceType = DeviceType.None;
        private ActionType m_ActionType = ActionType.None;
        private VelocitySelect m_VelocitySelect = VelocitySelect.Slow;

        public DeviceType RemoteDevice { get => m_DeviceType; set => m_DeviceType = value; }
        public ActionType RemoteAction { get => m_ActionType; set => m_ActionType = value; }
        public VelocitySelect RemoteVelocity { get => m_VelocitySelect; set => m_VelocitySelect = value; }

        public RemoteItem(DeviceType deviceType, ActionType actionType, VelocitySelect velocitySelect)
        {
            RemoteDevice = deviceType;
            RemoteAction = actionType;
            RemoteVelocity = velocitySelect;
        }
        public RemoteItem(DeviceType deviceType, ActionType actionType)
        {
            RemoteDevice = deviceType;
            RemoteAction = actionType;
        }
    }

}
