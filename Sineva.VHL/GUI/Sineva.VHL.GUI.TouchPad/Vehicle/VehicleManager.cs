using Sineva.VHL.Library.Remoting;

namespace Sineva.VHL.GUI.TouchPad
{
    public class VehicleManager
    {
        public static VehicleManager Instance = new VehicleManager();

        private RemoteItem.VelocitySelect m_volecity = RemoteItem.VelocitySelect.Slow;
        private SteerStatus m_steer = SteerStatus.None;
        private GripperStatus m_gripper = GripperStatus.None;
        private AntiDropStatus m_antiDrop = AntiDropStatus.None;

        private string m_vehicleNum = string.Empty;
        private string m_ip = string.Empty;

        public string VehicleNum { get => m_vehicleNum; set => m_vehicleNum = value; }
        public string Ip { get => m_ip; set => m_ip = value; }


        public RemoteItem.VelocitySelect Volecity
        {
            get { return m_volecity; }
            set { m_volecity = value; }
        }

        public SteerStatus Steer
        {
            get { return m_steer; }
            set { m_steer = value; }
        }

        public GripperStatus Gripper
        {
            get { return m_gripper; }
            set { m_gripper = value; }
        }

        public AntiDropStatus AntiDrop
        {
            get { return m_antiDrop; }
            set { m_antiDrop = value; }
        }



        public enum VolecitySelect
        {
            Slow,
            Mid,
            High
        }

        public enum SteerStatus
        {
            None,
            Left,
            Right
        }

        public enum GripperStatus
        {
            None,
            Open,
            Close
        }

        public enum AntiDropStatus
        {
            None,
            Open,
            Close
        }
    }
}