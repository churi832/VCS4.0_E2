using Sineva.VHL.IF.WebApi.Models.Dtos;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    /// <summary>
    /// 手动操作接口
    /// </summary>
    public class ManualController : ApiController
    {
        /// <summary>
        /// 模式切换
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode ChangeMode(ManualDto manualDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Operate Mode Change Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    switch (manualDto.OperateMode)
                    {
                        case 0:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Auto);
                            break;
                        case 2:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Manual);
                            break;
                        case 3:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Ready);
                            break;
                        default:
                            return new ReturnCode() { succeed = false, msg = "Operate Mode Code Error！", status = 210, };
                            break;
                    }
                    retcode = new ReturnCode() { succeed = true, msg = "Operate Mode Change Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Operate Mode Change Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Operate Mode Change Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }
        /// <summary>
        /// 运行模式切换
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode ChangeRunMode(RunModeDto runModeDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Run Mode Change Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    switch (runModeDto.RunMode)
                    {
                        case 0:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Start);
                            break;
                        case 1:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Stop);
                            break;
                        case 2:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Pause);
                            break;
                        case 3:
                            RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction(WebDeviceType.None, WebActionType.Abort);
                            break;
                        default:
                            return new ReturnCode() { succeed = false, msg = "Run Mode Code Error！", status = 210, };
                            break;
                    }
                    retcode = new ReturnCode() { succeed = true, msg = "Run Mode Change Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Run Mode Change Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Run Mode Change Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }

        /// <summary>
        /// 手动指令
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode ManualAction(ManualActionDto manualActionDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Manual Action Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction((WebDeviceType)manualActionDto.WebDevice, (WebActionType)manualActionDto.WebAction, new WebVelocitySelect
                    {
                        Velocity = manualActionDto.WebVelocity,
                        Distance = manualActionDto.WebDistance,
                    });
                    retcode = new ReturnCode() { succeed = true, msg = "Manual Action Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Manual Action Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Manual Action Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }

        /// <summary>
        /// 持续指令
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode ContinueCheck()
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction.UpdateTime = DateTime.Now;
                    retcode = new ReturnCode() { succeed = true, msg = "Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }

        /// <summary>
        /// Offset更新
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode UpdateOffset(OffsetUpdateDto offsetUpdateDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Update Offset Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = new WebAction((WebDeviceType)offsetUpdateDto.WebDevice, (WebActionType)offsetUpdateDto.WebAction, new WebOffsetUpdate
                    {
                        PortID = offsetUpdateDto.PortID,
                        DriveLeftOffset = offsetUpdateDto.DriveLeftOffset,
                        DriveRightOffset = offsetUpdateDto.DriveRightOffset,
                        HoistOffset = offsetUpdateDto.HoistOffset,
                        SlideOffset = offsetUpdateDto.SlideOffset,
                        RotateOffset = offsetUpdateDto.RotateOffset,
                    });
                    retcode = new ReturnCode() { succeed = true, msg = "Update Offset Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Update Offset Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Update Offset Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode AddCommand(CommandDto commandDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    var action= new WebAction(WebDeviceType.None, WebActionType.CommandAdd);
                    action.WebCommand = new WebCommand
                    {
                        CommandID = commandDto.CommandID,
                        CassetteID = commandDto.CassetteID,
                        IsValid = commandDto.IsValid,
                        SourceID = commandDto.SourceID,
                        DestinationID = commandDto.DestinationID,
                        TypeOfDestination = commandDto.TypeOfDestination,
                        TargetNodeToDistance = commandDto.TargetNodeToDistance,
                        CommandType = commandDto.CommandType,
                        TotalCount = commandDto.TotalCount,
                        WaitTime = commandDto.WaitTime,
                    };
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = action;
                    retcode = new ReturnCode() { succeed = true, msg = "Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }

        /// <summary>
        /// 删除指令
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode DeteleCommand(CommandDto commandDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    var action = new WebAction(WebDeviceType.None, WebActionType.CommandDelete);
                    action.WebCommand = new WebCommand
                    {
                        CommandID = commandDto.CommandID,
                        CassetteID = commandDto.CassetteID,
                        IsValid = commandDto.IsValid,
                        SourceID = commandDto.SourceID,
                        DestinationID = commandDto.DestinationID,
                        TypeOfDestination = commandDto.TypeOfDestination,
                        TargetNodeToDistance = commandDto.TargetNodeToDistance,
                        CommandType = commandDto.CommandType,
                    };
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = action;
                    retcode = new ReturnCode() { succeed = true, msg = "Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }


        /// <summary>
        /// CarrierState切换
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ReturnCode ChangeCarrierState(CarrierStateDto carrierStateDto)
        {
            var retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
            try
            {
                if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                {
                    var actionType = carrierStateDto.Install ? WebActionType.CarrierInstall : WebActionType.CarrierNone;
                    var action = new WebAction(WebDeviceType.None, actionType);
                    RemoteManager.TouchInstance.Remoting.TouchGUI.WebAction = action;
                    retcode = new ReturnCode() { succeed = true, msg = "Ok！", status = 200, };
                }
                else
                {
                    retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, };
                }
            }
            catch (Exception ex)
            {
                retcode = new ReturnCode() { succeed = false, msg = "Fail！", status = 210, data = ex.Message };
            }
            return retcode;
        }
    }
}