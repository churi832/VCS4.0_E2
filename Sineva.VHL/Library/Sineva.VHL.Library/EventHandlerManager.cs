/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System.Collections.Generic;

namespace Sineva.VHL.Library
{
    public class EventHandlerManager
    {
        public readonly static EventHandlerManager Instance = new EventHandlerManager();

        #region Event
        public event OccurEventMessageHandler OccurEventMessage;

        public event DelVoid_Bool PmInterlock = null;
        public event DelVoid_Bool RecipeChangePermission = null;
        public event DelVoid_Bool AutoTeachModeChanged = null;
        public event DelVoid_Bool AutoScreenLockActivate = null;
        public event DelVoid_Int AlarmHappened = null;
        public event DelVoid_String ConfigErrorNotifyMessage = null;
        public event DelSensorPv_IntDouble UpdateSensorPv_IntDouble = null;
        public event DelSensorPv_IntList UpdateSensorPv_IntList = null;

        public event DelVoid_OperateMode OperateModeChanged;
        public event DelVoid_EqpRunMode RunModeChanged;
        public event DelVoid_OperatorCall OperatorCalled;
        public event DelVoid_OperatorCallCommand OperatorCallCommand;
        public event DelVoid_Bool EmoSaftyWindowActivate;
        public event DelVoid_String NotifyConfigErrorMessage = null;

        public event DelVoid_PermissionRequest LoginPermissionRequest = null;

        public event DelVoid_Object LinkChanged = null;
        public event DelVoid_DataVersionChanged DataVersionChanged = null;
        public event DelVoid_Object UpdateCommandChanged = null;
        public event DelVoid_Object UpdatePathMapChanged = null;
        public event DelVoid_Object UpdateMotionProcess = null;

        public event DelVoid_Bool RemoteControlConnected = null;
        public event DelVoid_Int UpdateTargetPosition = null;
        public event DelVoid_AutoTeachingVisionResult AutoTeachingVisionResult = null;
        public event DelVoid_Bool AutoTeachingMonitorShow = null;

        public event DelVoid_Int ServoAxisOriginStart = null;
        public event DelVoid_String CarrierIDReadingConfirm = null;
        public event DelVoid_Void OverrideResetting = null;

        public event DelVoid_UpdatePathData UpdatePathData = null;
        #endregion

        public EventHandlerManager() 
        { 
        }

        #region Methods
        public void InvokePmInterlockChanged(bool show)
        {
            if (PmInterlock != null) PmInterlock(show);
        }
        public void InvokeRecipeChangePermission(bool possible)
        {
            if (RecipeChangePermission != null) RecipeChangePermission(possible);
        }
        public void InvokeAutoTeachModeChanged(bool auto)
        {
            AutoTeachModeChanged?.Invoke(auto);
        }
        public void InvokeAutoScreenLockActivate(bool active)
        {
            AutoScreenLockActivate?.Invoke(active);
        }
        public void InvokeAlarmHappened(int alarm_id)
        {
            AlarmHappened?.Invoke(alarm_id);
        }
        public void InvokeConfigErrorNotifyMessage(string message)
        {
            ConfigErrorNotifyMessage?.Invoke(message);
        }
        public void InvokeEventMessage(string msg)
        {
            OccurEventMessage?.Invoke(msg);
        }
        public void InvokeSensorPv(int Id, double value)
        {
            UpdateSensorPv_IntDouble?.Invoke(Id, value);
        }
        public void InvokeSensorPv(int Id, List<double> values)
        {
            UpdateSensorPv_IntList?.Invoke(Id, values);
        }

        public void InvokeOperateModeChanged(OperateMode opMode)
        {
            OperateModeChanged?.Invoke(opMode);
        }
        public void InvokeRunModeChanged(EqpRunMode runMode)
        {
            RunModeChanged?.Invoke(runMode);
        }
        public void InvokeOperatorCalled(OperatorCallKind kind, string message)
        {
            OperatorCalled?.Invoke(kind, message);
        }
        public void InvokeOperatorCallCommand(string message, string[] commands, bool show)
        {
            OperatorCallCommand?.Invoke(message, commands, show);
        }
        public void InvokeEmoSaftyWindowActivate(bool show)
        {
            EmoSaftyWindowActivate?.Invoke(show);
        }
        public void InvokeConfigErrorHappened(string message)
        {
            NotifyConfigErrorMessage?.Invoke(message);
        }
        public void InvokeLinkChanged(object link_data)
        {
            LinkChanged?.Invoke(link_data);
        }
        public void InvokeDataVersionChanged(GeneralInformationItemName name, string version)
        {
            DataVersionChanged?.Invoke(name, version);
        }
        
        public void InvokeUpdateCommandChanged(object command)
        {
            UpdateCommandChanged?.Invoke(command);
        }
        public void InvokeUpdatePathMapChanged(object command)
        {
            UpdatePathMapChanged?.Invoke(command);
        }
        public void InvokeUpdateMotionProcess(object path)
        {
            UpdateMotionProcess?.Invoke(path);
        }
        public void InvokeRemoteControlConnected(bool connected)
        {
            RemoteControlConnected?.Invoke(connected);
        }
        public void InvokeUpdateTargetPosition(int axisId)
        {
            UpdateTargetPosition?.Invoke(axisId);
        }
        public void InvokeAutoTeachingVisionResult(double dx, double dy, double dt)
        {
            AutoTeachingVisionResult?.Invoke(dx, dy, dt);
        }
        public void InvokeAutoTeachingMonitorShow(bool show)
        {
            AutoTeachingMonitorShow?.Invoke(show);
        }
        public void InvokeServoAxisOriginStart(int axisId)
        {
            ServoAxisOriginStart?.Invoke(axisId);
        }
        public void InvokeCarrierIDReadingConfirm(string carrierID)
        {
            CarrierIDReadingConfirm?.Invoke(carrierID);
        }
        public void InvokeOverrideResetting()
        {
            OverrideResetting?.Invoke();
        }
        public void InvokeUpdatePathData(object a, bool b)
        {
            UpdatePathData?.Invoke(a, b);
        }
        #endregion

        #region - for OCS
        public void FireLoginPermissionRequest(int key, string[] values)
        {
            LoginPermissionRequest?.Invoke(key, values);
        }
        #endregion
    }
}
