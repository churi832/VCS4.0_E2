using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sineva.VHL.Data.Process
{
    [Serializable]
    public class VehicleStatus
    {
        #region Fields
        public bool _SaveCurState = false;

        private VehicleInRailStatus m_InRail = VehicleInRailStatus.InRail;
        private CarrierState m_CarrierStatus = CarrierState.None;
        private DataItem_Node m_CurrentNode = new DataItem_Node();
        private DataItem_Node m_NextNode = new DataItem_Node();
        private DataItem_Port m_CurrentPort = new DataItem_Port();
        private VehicleState m_CurrentVehicleState = VehicleState.NotAssigned;
        private Path m_CurrentPath = new Path();
        private ObsStatus m_ObsStatus = new ObsStatus();
        private BcrStatus m_CurrentBcrStatus = new BcrStatus();

        private bool m_IsNearPosition = false; // Target Link에 도착 했을때 사용...
        private bool m_IsInPosition = false; // Target 도착 했을때 사용...inrange 범위...
        private bool m_DistanceErrorBCREncorder = false;
        private double m_MasterWheelVelocity = 0.0f;
        private double m_MasterWheelCMDVelocity = 0.0f;
        #endregion

        #region Properties
        public VehicleInRailStatus InRail 
        {
            get { return m_InRail; }
            set { m_InRail = value; }
        }
        public CarrierState CarrierStatus 
        {
            get { return m_CarrierStatus; }
            set 
            {
                _SaveCurState = true;
                m_CarrierStatus = value; 
            }
        }
        public DataItem_Node CurrentNode 
        {
            get { return m_CurrentNode; }
            set { m_CurrentNode = value; }
        }
        public DataItem_Node NextNode 
        {
            get { return m_NextNode; }
            set { m_NextNode = value; }
        }
        public DataItem_Port CurrentPort 
        {
            get { return m_CurrentPort; }
            set 
            {
                _SaveCurState = true;
                m_CurrentPort = value; 
            }
        }
        public VehicleState CurrentVehicleState
        {
            get { return m_CurrentVehicleState; }
            set 
            {
                _SaveCurState = true;
                m_CurrentVehicleState = value; 
            }
        }
        public Path CurrentPath 
        {
            get { return m_CurrentPath; }
            set 
            {
                _SaveCurState = true;
                m_CurrentPath = value; 
            }
        }
        public ObsStatus ObsStatus
        {
            get { return m_ObsStatus; }
            set { m_ObsStatus = value; }
        }
        public BcrStatus CurrentBcrStatus
        {
            get { return m_CurrentBcrStatus; }
            set { m_CurrentBcrStatus = value; }
        }

        public bool IsNearPosition
        {
            get { return m_IsNearPosition; }
            set { m_IsNearPosition = value; }
        }
        public bool IsInPosition 
        {
            get { return m_IsInPosition; }
            set { m_IsInPosition = value; }
        }
        public bool DistanceErrorBCREncorder
        {
            get { return m_DistanceErrorBCREncorder; }
            set { m_DistanceErrorBCREncorder = value; }
        }
        public double MasterWheelVelocity
        {
            get { return m_MasterWheelVelocity; }
            set { m_MasterWheelVelocity = value; }
        }
        public double MasterWheelCMDVelocity
        {
            get { return m_MasterWheelCMDVelocity; }
            set { m_MasterWheelCMDVelocity = value; }
        }
        #endregion

        #region Constructor
        public VehicleStatus() 
        {
        }
        #endregion

        #region Methods
        public void SetVehicleStatus(VehicleState state)
        {
            try
            {
                if (m_CurrentVehicleState != state)
                {
                    SequenceLog.WriteLog("[VehicleStatus]", string.Format("Transfer Status Change : {0} -> {1}", m_CurrentVehicleState, state));
                    m_CurrentVehicleState = state;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }

        public VehicleState GetVehicleStatus()
        {
            VehicleState state = VehicleState.NotAssigned;
            try
            {
                state = m_CurrentVehicleState;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return state;
        }

        public void SetCarrierStatus(CarrierState state)
        {
            try
            {
                if (CarrierStatus != state)
                {
                    SequenceLog.WriteLog("[VehicleStatus]", string.Format("Carrier Status Change : {0} -> {1}", CarrierStatus, state));
                    CarrierStatus = state;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        #endregion
    }
}
