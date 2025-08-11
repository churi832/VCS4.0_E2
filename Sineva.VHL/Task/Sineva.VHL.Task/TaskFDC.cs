using Sineva.VHL.Data;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sineva.VHL.Task.TaskMonitor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Sineva.VHL.Task
{
    public class TaskFDC : XSequence
    {
        #region Field
        public static readonly TaskFDC Instance = new TaskFDC();
        public SeqFDCCollection FDCCollection { get; set; }
        private class DataClassification
        {
            public _DevAxis Axis { get; set; }
            public _Device Device { get; set; }
        }
        #endregion

        #region Constructor
        public TaskFDC()
        {
            FDCCollection = SeqFDCCollection.Instance;
            this.RegSeq(FDCCollection);
            this.RegSeq(new SeqFDCReport());
        }
        #endregion
        
        #region Methods
        public class SeqFDCCollection : XSeqFunc
        {
            private const string FuncName = "[SeqFDCCollection]";
            public static readonly SeqFDCCollection Instance = new SeqFDCCollection();
            #region Fields
            private ProcessDataHandler m_ProcessDataHandler;
            private DatabaseHandler m_DatabaseHandler;
            private long _lastTick;

            private Dictionary<FDC_DataType, Func<_DevAxis, double>> m_dataExtractorsAxis;
            private Dictionary<FDC_DataType, Func<_Device, double>> m_dataExtractorsOther;
            private Dictionary<FDC_Unit, DataClassification> m_unitSourceMap;
            private ConcurrentQueue<KeyValuePair<string, string>> m_QueueReport;
            #endregion

            #region Property
            public ConcurrentQueue<KeyValuePair<string, string>> QueueFDCReport
            {
                get { return m_QueueReport; }
            }
            #endregion


            #region Contructor
            public SeqFDCCollection()
            {
                this.SeqName = $"SeqFDCCollection";
                m_ProcessDataHandler = ProcessDataHandler.Instance;
                m_DatabaseHandler = DatabaseHandler.Instance;
                m_QueueReport = new ConcurrentQueue<KeyValuePair<string, string>>();
                m_dataExtractorsAxis = new Dictionary<FDC_DataType, Func<_DevAxis, double>>
                {
                    { FDC_DataType.Torque,axis => axis.GetCurTorque() },
                    { FDC_DataType.Velocity,axis => axis.GetCurVelocity() },
                    { FDC_DataType.Acc_Dec,axis => axis.GetAxis().TargetAcc },
                    { FDC_DataType.Load_Factor,axis => 0 },
                    { FDC_DataType.BCR,axis => axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetAxis().AxisName ? (m_ProcessDataHandler.CurVehicleStatus.CurrentPath.BcrDirection == enBcrCheckDirection.Right
                                                           ? m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr : m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr) : 0 },
                    { FDC_DataType.Shock_Data,axis => 0 },
                    { FDC_DataType.Motor_RPM,axis => 0 },
                    { FDC_DataType.Accumulate_Distance, axis => axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetAxis().AxisName ? DevicesManager.Instance.DevTransfer.WheelMasterWorkingDistance
                                                       : axis.GetAxis().AxisName == DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().GetAxis().AxisName ? DevicesManager.Instance.DevTransfer.WheelSlaveWorkingDistance
                                                       : axis.GetAxis().AxisName == DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetAxis().AxisName ? DevicesManager.Instance.DevFoupGripper.HoistWorkingDistance
                                                       : axis.GetAxis().AxisName == DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().GetAxis().AxisName ? DevicesManager.Instance.DevFoupGripper.SlideWorkingDistance
                                                       : axis.GetAxis().AxisName == DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().GetAxis().AxisName ? DevicesManager.Instance.DevFoupGripper.RotateWorkingDistance
                                                       : 0 },
                    { FDC_DataType.CPS_Voltage,axis => 0 },
                    { FDC_DataType.CPS_Current,axis => 0 },
                    { FDC_DataType.CPS_Temperature,axis => 0 }
                };

                m_dataExtractorsOther = new Dictionary<FDC_DataType, Func<_Device, double>>
                {
                    { FDC_DataType.Torque,device => 0 },
                    { FDC_DataType.Velocity,device => 0 },
                    { FDC_DataType.Acc_Dec,device => 0 },
                    { FDC_DataType.Load_Factor,device => 0 },
                    { FDC_DataType.BCR,device => 0 },
                    { FDC_DataType.Shock_Data,device => 0 },
                    { FDC_DataType.Motor_RPM,device => 0 },
                    { FDC_DataType.Accumulate_Distance, device => device.MyName == "DevGripperPio"? DevicesManager.Instance.DevGripperPIO.OpenCount: 0 },
                    { FDC_DataType.CPS_Voltage,device => device.MyName == "DevCPS" ? DevicesManager.Instance.DevCPS.BoostVoltage: 0 },
                    { FDC_DataType.CPS_Current,device => device.MyName == "DevCPS" ? DevicesManager.Instance.DevCPS.BoostCurrent : 0 },
                    { FDC_DataType.CPS_Temperature,device => device.MyName == "DevCPS"  ? DevicesManager.Instance.DevCPS.HeatSinkTemp : 0 }
                };

                m_unitSourceMap = new Dictionary<FDC_Unit, DataClassification>
                {
                    { FDC_Unit.OHT,new DataClassification { Axis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis(), Device = null } },
                    { FDC_Unit.Drive_Master,new DataClassification { Axis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis(), Device = null } },
                    { FDC_Unit.Drive_Slave,new DataClassification { Axis = DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis(), Device = null } },
                    { FDC_Unit.Hoist,new DataClassification { Axis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis(), Device = null } },
                    { FDC_Unit.Slide, new DataClassification { Axis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis(), Device = null } },
                    { FDC_Unit.Rotate,new DataClassification { Axis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis(), Device = null } },
                    { FDC_Unit.Gripper,new DataClassification { Axis = null, Device = DevicesManager.Instance.DevGripperPIO } },
                    { FDC_Unit.CPS, new DataClassification { Axis = null, Device = DevicesManager.Instance.DevCPS } }
                };
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seq = this.SeqNo;
                switch (seq)
                {
                    case 0:
                        if (SetupManager.Instance.SetupOCS.UseFDCReport == Use.NoUse)
                        {
                            seq = 0;
                        }
                        else
                        {
                            _lastTick = XFunc.GetTickCount();
                            seq = 10;
                        }
                        break;

                    case 10:
                        if (SetupManager.Instance.SetupOCS.UseFDCReport == Use.NoUse)
                        { 
                            seq = 0;
                        }

                        if (XFunc.GetTickCount() - _lastTick >= 100)
                        {
                            var values = new List<string>();
                            foreach (var item in m_DatabaseHandler.DictionaryFDC.Values)
                            {
                                string text = ParsinData(item);
                                values.Add(text);
                            }
                           string key = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.f00");
                            string dataLine = string.Join("|", values);

                            m_QueueReport.Enqueue(new KeyValuePair<string, string>(key, dataLine));
                            _lastTick = XFunc.GetTickCount();
                        }
                        break;
                }
                this.SeqNo = seq;
                return -1;
            }
            #endregion

            #region Methods
            private string ParsinData(DataItem_FDC data)
            {
                try
                {
                    if (m_unitSourceMap.TryGetValue(data.Unit, out var source))
                    {
                        if (source.Axis != null && m_dataExtractorsAxis.TryGetValue(data.DataType, out var axisExt))
                        {
                            double raw = axisExt(source.Axis);
                            return raw.ToString($"F{data.DecimalPoint}");
                        }
                        if (source.Device != null && m_dataExtractorsOther.TryGetValue(data.DataType, out var devExt))
                        {
                            double raw = devExt(source.Device);
                            return raw.ToString($"F{data.DecimalPoint}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return string.Empty;
            }
            #endregion
        }

        public class SeqFDCReport : XSeqFunc
        {
            private const string FuncName = "[SeqFDCReport]";

            #region Fields
            private DatabaseHandler m_DatabaseHandler;
            private DataItem_FDCItem m_FDCItem;
            private long _lastTick;
            private string m_EQPID;
            #endregion

            #region Property
            #endregion


            #region Contructor
            public SeqFDCReport()
            {
                this.SeqName = $"SeqFDCReport";
                m_DatabaseHandler = DatabaseHandler.Instance;
                m_FDCItem = new DataItem_FDCItem();
                m_EQPID = AppConfig.Instance.VehicleNumber.ToString();
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seq = this.SeqNo;
                switch (seq)
                {
                    case 0:
                        if (SetupManager.Instance.SetupOCS.UseFDCReport == Use.NoUse)
                        {
                            seq = 0;
                        }
                        else
                        {
                            _lastTick = XFunc.GetTickCount();
                            seq = 10;
                        }
                        break;

                    case 10:
                        if (SetupManager.Instance.SetupOCS.UseFDCReport == Use.NoUse)
                        {
                            seq = 0;
                        }
                        if (XFunc.GetTickCount() - _lastTick < 100) break;

                        if (TaskFDC.Instance.FDCCollection.QueueFDCReport.TryDequeue(out KeyValuePair<string, string> data))
                        {
                            m_FDCItem = new DataItem_FDCItem()
                            { 
                                EQPID = m_EQPID,
                                Time = data.Key,
                                ValueString = data.Value,
                            };
                            m_DatabaseHandler.QueryFDCServer.Update(m_FDCItem);
                        }
                        break;
                }
                this.SeqNo = seq;
                return -1;
            }
            #endregion

            #region Methods
            #endregion
        }
        #endregion
    }
}
