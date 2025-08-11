using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Data.Process;
using System.Xml.Serialization;
using Sineva.VHL.Data.DbAdapter;

namespace Sineva.VHL.Device.ServoControl
{
    [Serializable]
    public class InterlockManager
    {
        #region Fields
        public readonly static InterlockManager Instance = new InterlockManager();
        private readonly static object m_LockKey = new object();

        private string m_FileName;
        private string m_FilePath;
        #endregion

        #region Fields (Axis)
        private DevAxisTag m_WheelMaster = null;
        private DevAxisTag m_Hoist = null;
        private DevAxisTag m_Slide = null;
        private DevAxisTag m_Rotate = null;
        #endregion

        #region Fields (I/O)
        private IoTag m_diAntiDropLock1 = null;
        private IoTag m_diAntiDropLock2 = null;
        private IoTag m_diAntiDropUnlock1 = null;
        private IoTag m_diAntiDropUnlock2 = null;
        private IoTag m_diWheelBrake = null;
        private IoTag m_diHoistBrake = null;
        private IoTag m_diLeftFoupExist = null;
        private IoTag m_diRightFoupExist = null;
        private IoTag m_diGripperOpen = null;
        private IoTag m_diGripperClose = null;
        #endregion

        #region Fields - Profile Position Interlock
        private Dictionary<int, List<XyztPosition>> m_LeftProfilePositions = new Dictionary<int, List<XyztPosition>>(); // xy : start <-> zt : end ... 범위내일 경우 Interlock
        private Dictionary<int, List<XyztPosition>> m_RightProfilePositions = new Dictionary<int, List<XyztPosition>>(); // xy : start <-> zt : end ... 범위내일 경우 Interlock
        #endregion

        #region Properties
        [Category("! 1.SERVO"), DeviceSetting(true, true)]
        public DevAxisTag WheelMaster { get { return m_WheelMaster; } set { m_WheelMaster = value; } }
        [Category("! 1.SERVO"), DeviceSetting(true, true)]
        public DevAxisTag Hoist { get { return m_Hoist; } set { m_Hoist = value; } }
        [Category("! 1.SERVO"), DeviceSetting(true, true)]
        public DevAxisTag Slide { get { return m_Slide; } set { m_Slide = value; } }
        [Category("! 1.SERVO"), DeviceSetting(true, true)]
        public DevAxisTag Rotate { get { return m_Rotate; } set { m_Rotate = value; } }

        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diAntiDropLock1 { get { return m_diAntiDropLock1; } set { m_diAntiDropLock1 = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diAntiDropLock2 { get { return m_diAntiDropLock2; } set { m_diAntiDropLock2 = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diAntiDropUnlock1 { get { return m_diAntiDropUnlock1; } set { m_diAntiDropUnlock1 = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diAntiDropUnlock2 { get { return m_diAntiDropUnlock2; } set { m_diAntiDropUnlock2 = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diWheelBrake { get { return m_diWheelBrake; } set { m_diWheelBrake = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diHoistBrake { get { return m_diHoistBrake; } set { m_diHoistBrake = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diLeftFoupExist { get { return m_diLeftFoupExist; } set { m_diLeftFoupExist = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diRightFoupExist { get { return m_diRightFoupExist; } set { m_diRightFoupExist = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diGripperOpen { get { return m_diGripperOpen; } set { m_diGripperOpen = value; } }
        [Category("! 2.I/O DI"), DeviceSetting(true, true)]
        public IoTag diGripperClose { get { return m_diGripperClose; } set { m_diGripperClose = value; } }
        [XmlIgnore, ReadOnly(true), Browsable(false)]
        public Dictionary<int, List<XyztPosition>> LeftProfilePositions { get { return m_LeftProfilePositions; } set { m_LeftProfilePositions = value; } }
        [XmlIgnore, ReadOnly(true), Browsable(false)]
        public Dictionary<int, List<XyztPosition>> RightProfilePositions { get { return m_RightProfilePositions; } set { m_RightProfilePositions = value; } }
        
        #endregion

        #region Event
        public static event DelVoid_String InterlockMessage;
        #endregion

        #region Constructor
        private InterlockManager()
        {
        }

        public bool Initialize()
        {
            ReadXml();
            return true;
        }
        #endregion

        #region Methods
        public static void SetProfilePosition(Dictionary<int, List<XyztPosition>> LPos, Dictionary<int, List<XyztPosition>> RPos)
        {
            ///////////////////////////////////////////////////////
            Instance.LeftProfilePositions.Clear();
            foreach (KeyValuePair<int, List<XyztPosition>> item in LPos)
                Instance.LeftProfilePositions.Add(item.Key, item.Value);
            Instance.RightProfilePositions.Clear();
            foreach (KeyValuePair<int, List<XyztPosition>> item in RPos)
                Instance.RightProfilePositions.Add(item.Key, item.Value);
        }
        public static bool IsSafe(_Axis axis, double targetPos, bool moveOrigin = false)
        {
            bool safe = true;
            // General Condition Check
            SetInterlockMessage(GV.PowerOn, "Equipment Power Off Condition", ref safe);
            SetInterlockMessage(!GV.EmoAlarm, "Emergency Stop Condition", ref safe);
            SetInterlockMessage(!GV.SaftyAlarm, "Emergency Stop Condition", ref safe);
            SetInterlockMessage(!GV.CpAlarm, "Emergency Stop Condition", ref safe);
            SetInterlockMessage(!GV.ManualPendantActivated, "Emergency Stop Condition", ref safe);
            SetInterlockMessage(!GV.AutoModeSwitched, "Emergency Stop Condition", ref safe);
            SetInterlockMessage(!GV.CleanerDoorOpenInterlock, "Cleaner Door Open Stop Condition", ref safe);
            
            if (!safe) return safe;

            try
            {
                safe &= IsSafeCheck(axis, targetPos, moveOrigin);
            }
            catch (Exception err)
            {
                SetInterlockMessage(false, err.ToString(), ref safe);
            }

            return safe;
        }
        public static bool IsSafeCheck(_Axis axis, double targetPos, bool moveOrigin = false)
        {
            bool safe = true;
            //////////////////////////
            // Stroke Limit Check, 주행축은 제외
            bool axisWheel = false;
            if (IsValidAxis(Instance.WheelMaster))
            {
                if (axis.AxisId == Instance.WheelMaster.AxisId) axisWheel = true;
            }
            if (axisWheel == false)
            {
                bool strokeCheck = true;
                double strokeLimitPos = axis.PosLimitPos;
                double strokeLimitNeg = axis.NegLimitPos;
                strokeCheck &= targetPos <= strokeLimitPos ? true : false;
                strokeCheck &= targetPos >= strokeLimitNeg ? true : false;
                SetInterlockMessage(strokeCheck, string.Format("Safety Moving Stroke OverRange Condition[{0}], Target pos[{1}], NegLimit[{2}], PosLimit[{3}]", axis.AxisName, targetPos, axis.NegLimitPos, axis.PosLimitPos), ref safe);
            }

            // Cur Axis Ready Check
            enAxisInFlag axisStatus = axis.AxisStatus;
            bool axisInPos = true;
            if (axis.InPosCheckSkip == false && !axisWheel)
                axisInPos &= (axisStatus & enAxisInFlag.InPos) == enAxisInFlag.InPos;
            axisInPos &= (axisStatus & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
            SetInterlockMessage(axisInPos || moveOrigin, string.Format("Safety Moving Not Ready Condition[{0} = {1}]", axis.AxisName, axisStatus), ref safe);

            bool swingSensorAlarm = !GV.SwingSensorInterlock;
            SetInterlockMessage(swingSensorAlarm, string.Format("Safety Swing Sensor Condition[{0}]", axis.AxisName), ref safe);
            bool beltCutAlarm = !GV.BeltCutInterlock;
            SetInterlockMessage(beltCutAlarm, string.Format("Safety Belt Cut Condition[{0}]", axis.AxisName), ref safe);
            bool bumpCollisionAlarm = !GV.BumpCollisionInterlock;
            SetInterlockMessage(bumpCollisionAlarm, string.Format("Safety Bump Collision Condition[{0}]", axis.AxisName), ref safe);

            double LBcr = Instance.WheelMaster.GetDevAxis().GetAxisCurLeftBarcode();
            double RBcr = Instance.WheelMaster.GetDevAxis().GetAxisCurRightBarcode();
            double masterPos = Instance.WheelMaster.GetDevAxis().GetCurPosition();
            double hoistPos = Instance.Hoist.GetDevAxis().GetCurPosition();
            double slidePos = Instance.Slide.GetDevAxis().GetCurPosition();
            if (axis.AxisId == Instance.WheelMaster.AxisId)
            {
                SetInterlockMessage(Math.Abs(hoistPos) < 5, string.Format("Safety WheelMaster Moving Alarm ! Hoist Interlock Condition[{0}]", hoistPos), ref safe);
                //SetInterlockMessage(Math.Abs(slidePos) < 5, string.Format("Safety WheelMaster Moving Alarm ! Slide Interlock Condition[{0}]", slidePos), ref safe);
                int curLinkID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID;
                if (Math.Abs(slidePos) > 5.0f)
                {
                    bool range_intr = true;
                    double moveDistance = targetPos - masterPos;
                    if (moveDistance >= 0 && moveDistance < ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RemainDistanceOfLink)
                    {
                        double target_lbcr = LBcr + moveDistance;
                        double target_rbcr = RBcr + moveDistance;
                        List<XyztPosition> posOnLink = new List<XyztPosition>();
                        if (Instance.LeftProfilePositions != null)
                        {
                            if (Instance.LeftProfilePositions.ContainsKey(curLinkID) && slidePos > 5.0f)
                            {
                                posOnLink.AddRange(Instance.LeftProfilePositions[curLinkID].Where(item => item.X > LBcr && item.Y > RBcr && item.X < target_lbcr && item.Y < target_rbcr).ToList());
                            }
                        }
                        if (Instance.RightProfilePositions != null)
                        {
                            if (Instance.RightProfilePositions.ContainsKey(curLinkID) && slidePos < -5.0f)
                            {
                                posOnLink.AddRange(Instance.RightProfilePositions[curLinkID].Where(item => item.X > LBcr && item.Y > RBcr && item.X < target_lbcr && item.Y < target_rbcr).ToList());
                            }
                        }
                        // Start 위치가 CurBCR보다 크고 Target 위치보다 적으면....이건 Interlock area를 지나간다는 뜻
                        range_intr &= posOnLink.Count == 0;
                        SetInterlockMessage(range_intr, string.Format("Safety Wheel Moving Alarm ! Wheel Interlock Condition. Slide Position Check"), ref safe);
                    }
                    else if (moveDistance < 0 && Math.Abs(moveDistance) < ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink)
                    {
                        double target_lbcr = LBcr + moveDistance;
                        double target_rbcr = RBcr + moveDistance;
                        List<XyztPosition> posOnLink = new List<XyztPosition>();
                        if (Instance.LeftProfilePositions != null)
                        {
                            if (Instance.LeftProfilePositions.ContainsKey(curLinkID) && slidePos > 5.0f)
                            {
                                posOnLink.AddRange(Instance.LeftProfilePositions[curLinkID].Where(item => item.X < LBcr && item.Y < RBcr && item.X > target_lbcr && item.Y > target_rbcr).ToList());
                            }
                        }
                        if (Instance.RightProfilePositions != null)
                        {
                            if (Instance.RightProfilePositions.ContainsKey(curLinkID) && slidePos < -5.0f)
                            {
                                posOnLink.AddRange(Instance.RightProfilePositions[curLinkID].Where(item => item.X < LBcr && item.Y < RBcr && item.X > target_lbcr && item.Y > target_rbcr).ToList());
                            }
                        }
                        // End 위치가 CurBCR보다 적고 Target 위치보다 크면....이건 Interlock area를 지나간다는 뜻
                        range_intr &= posOnLink.Count == 0;
                        SetInterlockMessage(range_intr, string.Format("Safety Wheel Moving Alarm ! Wheel Interlock Condition. Slide Position Check"), ref safe);
                    }
                }
            }
            if (axis.AxisId == Instance.Hoist.AxisId)
            {
                if (AppConfig.Instance.Simulation.IO) Instance.diHoistBrake.SetDo(true);
                bool brake_off = Instance.diHoistBrake != null && IsValidIO(Instance.diHoistBrake) ? Instance.diHoistBrake.GetDi() : true;
                SetInterlockMessage(brake_off, string.Format("Safety Hoist Moving Alarm ! Hoist Interlock Condition Brake ON"), ref safe);
                bool left = Instance.diLeftFoupExist != null && IsValidIO(Instance.diLeftFoupExist) ? Instance.diLeftFoupExist.GetDi() : true;
                bool right = Instance.diRightFoupExist != null && IsValidIO(Instance.diRightFoupExist) ? Instance.diRightFoupExist.GetDi() : true;
                bool foup_exist = left && right;
                bool foup_not_exist = !left && !right;
                SetInterlockMessage(foup_exist || foup_not_exist, string.Format("Safety Hoist Moving Alarm ! Foup Exist Sensor Abnormal, Left={0}, Right={1}", left, right), ref safe);
                bool open = Instance.diGripperOpen != null && IsValidIO(Instance.diGripperOpen) ? Instance.diGripperOpen.GetDi() : true;
                bool close = Instance.diGripperClose != null && IsValidIO(Instance.diGripperClose) ? Instance.diGripperClose.GetDi() : true;
                SetInterlockMessage(open || close, string.Format("Safety Hoist Moving Alarm ! Gripper Open/Close Abnormal, Open={0}, Close={1}", open, close), ref safe);
                bool anti_lock = Instance.diAntiDropLock1 != null && IsValidIO(Instance.diAntiDropLock1) ? Instance.diAntiDropLock1.GetDi() : true;
                anti_lock &= Instance.diAntiDropLock2 != null && IsValidIO(Instance.diAntiDropLock2) ? Instance.diAntiDropLock2.GetDi() : true;
                bool anti_unlock = Instance.diAntiDropUnlock1 != null && IsValidIO(Instance.diAntiDropUnlock1) ? Instance.diAntiDropUnlock1.GetDi() : true;
                anti_unlock &= Instance.diAntiDropUnlock2 != null && IsValidIO(Instance.diAntiDropUnlock2) ? Instance.diAntiDropUnlock2.GetDi() : true;
                SetInterlockMessage(anti_unlock && !anti_lock, string.Format("Safety Hoist Moving Alarm ! AntiDrop Unlock Abnormal, Lock={0}, Unlock={1}", anti_lock, anti_unlock), ref safe);
            }
            if (axis.AxisId == Instance.Slide.AxisId)
            {
                if (Math.Abs(targetPos) > 15.0f)
                {
                    bool range_intr = true;
                    int curLinkID = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID;
                    if (Instance.LeftProfilePositions != null)
                    {
                        if (Instance.LeftProfilePositions.ContainsKey(curLinkID) && slidePos > 15.0f)
                        {
                            List<XyztPosition> posOnLink = Instance.LeftProfilePositions[curLinkID].Where(item => item.X <= LBcr &&
                            item.Y <= RBcr && item.Z >= LBcr && item.T >= RBcr).ToList();
                            range_intr &= posOnLink.Count == 0;
                        }
                    }
                    if (Instance.RightProfilePositions != null)
                    {
                        if (Instance.RightProfilePositions.ContainsKey(curLinkID) && slidePos < -15.0f)
                        {
                            List<XyztPosition> posOnLink = Instance.RightProfilePositions[curLinkID].Where(item => item.X <= LBcr &&
                            item.Y <= RBcr && item.Z >= LBcr && item.T >= RBcr).ToList();
                            range_intr &= posOnLink.Count == 0;
                        }
                    }
                    SetInterlockMessage(range_intr, string.Format("Safety Slide Moving Alarm ! Slide Interlock Condition. Wheel Position Check"), ref safe);
                }
            }

            return safe;
        }

        private static void SetInterlockMessage(bool condition, string message, ref bool safe)
        {
            if (safe && !condition)
            {
                InterlockMessage(message);
                EqpStateManager.Instance.SetOpCallMessage(OperatorCallKind.InterlockMessage, message);
                ServoLog.WriteLog(string.Format("Interlock {0}", message));
            }
            else if (!condition)
            {
                ServoLog.WriteLog(string.Format("Interlock_Safe_Release {0}", message));
            }

            safe &= condition;
        }
        public static int GetAxisId(DevAxisTag axis)
        {
            int axis_id = -1;
            if (axis == null) return -1;
            if (axis.GetDevAxis() == null) return -1;
            if (axis.GetDevAxis().GetAxis() == null) return -1;

            axis_id = axis.GetDevAxis().GetAxis().AxisId;
            return axis_id;
        }

        public static bool IsValidAxis(DevAxisTag axis)
        {
            if (axis == null) return false;
            if (axis.GetDevAxis() == null) return false;
            if (axis.GetDevAxis().GetAxis() == null) return false;
            return true;
        }

        public static bool IsValidIO(IoTag io)
        {
            if (io == null || io.Name == null) return false;
            if (io.GetChannel() == null) return false;
            return true;
        }
        public class IoTagListConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(List<IoTag>))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is List<IoTag>)
                {
                    List<IoTag> m = (List<IoTag>)value;
                    if (m == null) string.Format("(Collection)");
                    else return string.Format("(Collection) : {0}ea", m.Count);
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        public class DevAxisTagListConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(List<DevAxisTag>))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is List<DevAxisTag>)
                {
                    List<DevAxisTag> m = (List<DevAxisTag>)value;
                    if (m == null) string.Format("(Collection)");
                    else return string.Format("(Collection) : {0}ea", m.Count);
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion
        #region [Xml Read/Write]
        public bool ReadXml()
        {
            if (CheckPath())
            {
                return ReadXml(m_FileName);
            }
            else return false;
        }

        public bool ReadXml(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    m_FileName = fileName;
                }
                else
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<InterlockManager>();
                InterlockManager mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.WheelMaster = mng.WheelMaster;
                    this.Hoist = mng.Hoist;
                    this.Slide = mng.Slide;
                    this.Rotate = mng.Rotate;
                    this.diAntiDropLock1 = mng.diAntiDropLock1;
                    this.diAntiDropLock2 = mng.diAntiDropLock2;
                    this.diAntiDropUnlock1 = mng.diAntiDropUnlock1;
                    this.diAntiDropUnlock2 = mng.diAntiDropUnlock2;
                    this.diWheelBrake = mng.diWheelBrake;
                    this.diHoistBrake = mng.diHoistBrake;
                    this.diLeftFoupExist = mng.diLeftFoupExist;
                    this.diRightFoupExist = mng.diRightFoupExist;
                    this.diGripperOpen = mng.diGripperOpen;
                    this.diGripperClose = mng.diGripperClose;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("InterlockManager_ReadXml_" + err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public void WriteXml()
        {
            if (string.IsNullOrEmpty(m_FileName))
            {
                string dirName = AppConfig.DefaultConfigFilePath;
                Directory.CreateDirectory(dirName);

                m_FilePath = string.Format("{0}\\{1}.xml", dirName, GetDefaultFileName());
            }

            WriteXml(m_FileName);
        }

        public void WriteXml(string fileName)
        {
            try
            {
                var helperXml = new XmlHelper<InterlockManager>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public bool CheckPath()
        {
            bool ok = false;
            try
            {
                string filePath = m_FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = AppConfig.Instance.XmlInterlockManagerPath;
                }

                if (Directory.Exists(filePath) == false)
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.Description = "Configuration folder select";
                    dlg.SelectedPath = AppConfig.GetSolutionPath();
                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        filePath = dlg.SelectedPath;
                        if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                            AppConfig.Instance.WriteXml();
                        }
                        m_FilePath = filePath;
                        m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
                else
                {
                    m_FilePath = filePath;
                    m_FileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return ok;
        }

        public string GetDefaultFileName()
        {
            string fileName;
            fileName = this.GetType().Name;
            return fileName;
        }
        #endregion
    }
}
