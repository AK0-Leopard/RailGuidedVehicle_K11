//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/07/16    Mark Chou      N/A            M0.01   修正回覆S1F4 SVID305會發生Exception的問題
// 2019/08/26    Kevin Wei      N/A            M0.02   修正原本在只要有From、To命令還是在Wating的狀態時，
//                                                     此時MCS若下達一筆命令則會拒絕，改成只要是From相同，就會拒絕。
//**********************************************************************************

using com.mirle.AK0.RGV.HostMessage.H2E;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.Extension;
using Grpc.Core;
using NLog;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction.ASE.K11
{
    public class MCSDefaultMapActionReceive : RGV_K11_H2E.RGV_K11_H2EBase
    {
        private readonly ILogger _logger;

        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string DEVICE_NAME_MCS = "MCS";

        public override Task<S1F2_OnLineData> SendS1F1_AreYouThereReq(S1F1_AreYouThereRequest request, ServerCallContext context)
        {
            return Task.FromResult(new S1F2_OnLineData
            {
                Softrev = SCAppConstants.getMainFormVersion("")
            });
        }
        #region S1F4
        public override Task<S1F4_SelectedEquipmentStatusData> SendS1F3_SelectedEquipmentStatusReq(S1F3_SelectedEquipmentStatusRequest request, ServerCallContext context)
        {
            SCApplication scApp = SCApplication.getInstance();
            S1F4_SelectedEquipmentStatusData s1f4 = new S1F4_SelectedEquipmentStatusData();
            buildAlarmSet(scApp, ref s1f4);
            buildControlState(scApp, ref s1f4);
            buildTSCState(scApp, ref s1f4);
            buildEnhancedCarriers(scApp, ref s1f4);
            buildEnhancedTransfers(scApp, ref s1f4);
            return Task.FromResult(s1f4);
        }

        private void buildEnhancedTransfers(SCApplication scApp, ref S1F4_SelectedEquipmentStatusData s1f4)
        {
            var transfers = scApp.TransferBLL.db.transfer.loadUnfinishedTransfer();
            var tran_infos = from tran in transfers
                             select new VID_EnhancedTransferCommand()
                             {
                                 CommandId = SCUtility.Trim(tran.ID),
                                 CarrierId = SCUtility.Trim(tran.CARRIER_ID),
                                 SourcePort = SCUtility.Trim(tran.HOSTSOURCE),
                                 DestPort = SCUtility.Trim(tran.HOSTDESTINATION),
                                 Priority = tran.PRIORITY.ToString(),
                                 Replace = tran.REPLACE.ToString(),
                                 TransferState = converToTranState(tran.TRANSFERSTATE),
                             };
            s1f4.TransfersInfo = new VID_63_EnhancedTransfers();
            s1f4.TransfersInfo.TransferCommands.AddRange(tran_infos);
        }
        private TRAN_STATE converToTranState(E_TRAN_STATUS status)
        {
            switch (status)
            {
                case E_TRAN_STATUS.Queue:
                case E_TRAN_STATUS.PreInitial:
                    return TRAN_STATE.Queue;
                case E_TRAN_STATUS.Initial:
                    return TRAN_STATE.Waiting;
                case E_TRAN_STATUS.Canceling:
                    return TRAN_STATE.Canceling;
                case E_TRAN_STATUS.Aborting:
                    return TRAN_STATE.Abortling;
                default:
                    throw new ArgumentException($"status:{status} not define to mcs");
            }
        }
        private void buildEnhancedCarriers(SCApplication scApp, ref S1F4_SelectedEquipmentStatusData s1f4)
        {
            var in_line_carriers = scApp.CarrierBLL.db.loadCurrentInLineCarrier();
            in_line_carriers = in_line_carriers.
                Where(cst => cst.STATE == ProtocolFormat.OHTMessage.E_CARRIER_STATE.Installed).ToList();

            var carrier_infos = from carrier in in_line_carriers
                                select new VID_EnhancedCarrierInfo()
                                {
                                    CarrierId = SCUtility.Trim(carrier.ID),
                                    CarrierLoc = SCUtility.Trim(carrier.LOCATION),
                                    InstallTime = ((DateTimeOffset)(carrier.INSTALLED_TIME.HasValue ?
                                                                    carrier.INSTALLED_TIME.Value : DateTime.MinValue)).
                                                                    ToUnixTimeSeconds(),
                                    VehicleId = SCUtility.Trim(carrier.LOCATION)
                                };

            s1f4.CarriersInfo = new VID_62_EnhancedCarriers();
            s1f4.CarriersInfo.CarrierInfos.AddRange(carrier_infos);
        }

        private void buildTSCState(SCApplication scApp, ref S1F4_SelectedEquipmentStatusData s1f4)
        {
            ALINE line = scApp.getEQObjCacheManager().getLine();
            TSCState state = TSCState.Pause;
            switch (line.TSC_state_machine.State)
            {
                case ALINE.TSCState.NONE:
                case ALINE.TSCState.PAUSING:
                case ALINE.TSCState.TSC_INIT:
                case ALINE.TSCState.PAUSED:
                    state = TSCState.Pause;
                    break;
                case ALINE.TSCState.AUTO:
                    state = TSCState.Pause;
                    break;
            }

            s1f4.TscState = new VID_73_TSCState();
            s1f4.TscState.TscState = state;
        }

        private void buildControlState(SCApplication scApp, ref S1F4_SelectedEquipmentStatusData s1f4)
        {
            ALINE line = scApp.getEQObjCacheManager().getLine();
            ControlState state = ControlState.Offline;
            switch (line.Host_Control_State)
            {
                case SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line:
                case SCAppConstants.LineHostControlState.HostControlState.Going_Online:
                case SCAppConstants.LineHostControlState.HostControlState.Host_Offline:
                    state = ControlState.Offline;
                    break;
                case SCAppConstants.LineHostControlState.HostControlState.On_Line_Local:
                    state = ControlState.OnlineLocal;
                    break;
                case SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote:
                    state = ControlState.OnlineRemote;
                    break;
            }

            s1f4.ControlState = new VID_6_ControlState();
            s1f4.ControlState.ControlState = state;
        }

        private void buildAlarmSet(SCApplication scApp, ref S1F4_SelectedEquipmentStatusData s1f4)
        {
            var alarms = scApp.AlarmBLL.getCurrentErrorAlarms();
            VID_4_AlarmSet alarm_set = new VID_4_AlarmSet();
            var report_11 = from alarm in alarms
                            select new Report_ID_11()
                            {
                                UnitId = SCUtility.Trim(alarm.EQPT_ID),
                                AlarmId = SCUtility.Trim(alarm.ALAM_CODE),
                                ErrorCode = 0,
                                AlaemText = SCUtility.Trim(alarm.ALAM_DESC)
                            };
            alarm_set.CurrentSetAlarms.AddRange(report_11);
            s1f4.SetAlarms = alarm_set;
        }
        #endregion S1F4
        public override Task<S1F18_OnLineAck> SendS1F17_ReqOnLine(S1F17_RequestOnLine request, ServerCallContext context)
        {
            string msg = "";
            SCApplication scApp = SCApplication.getInstance();
            ALINE line = scApp.getEQObjCacheManager().getLine();
            S1F18_OnLineAck s1f18 = new S1F18_OnLineAck();
            if (DebugParameter.RejectEAPOnline)
            {
                s1f18.Onlack = ONLACK.OnLineNotAllowed;
            }
            else if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote)
            {
                //s1f18.ONLACK = SECSConst.ONLACK_Equipment_Already_On_Line;
                s1f18.Onlack = ONLACK.EquipmentAlreadyOnLine;
                msg = "OHS is online remote ready!!"; //A0.05
            }
            else
            {
                s1f18.Onlack = ONLACK.OnLineAccepted;
            }

            if (s1f18.Onlack == ONLACK.OnLineAccepted)
            {
                scApp.LineService.OnlineWithHostByHost();
            }

            return Task.FromResult(s1f18);
        }
        public override Task<S2F32_DateAndTimeSetAck> SendS2F31_DateAndTimeSetReq(S2F31_DateAndTimeSetRequest request, ServerCallContext context)
        {
            S2F32_DateAndTimeSetAck s2f32 = new S2F32_DateAndTimeSetAck();
            DateTime mesDateTime = DateTime.Now;
            try
            {
                mesDateTime = DateTime.ParseExact(SCUtility.Trim(request.Time), SCAppConstants.TimestampFormat_16, CultureInfo.CurrentCulture);
                s2f32.Tiack = TIACK.Ok;
            }
            catch (Exception dtEx)
            {
                s2f32.Tiack = TIACK.Error;
            }
            if (s2f32.Tiack == TIACK.Ok)
            {
                SCUtility.updateSystemTime(mesDateTime);
            }
            return Task.FromResult(s2f32);
        }
        #region S2F42
        public override Task<S2F42_HostCommandAck> SendS2F41_HostCmdSend(S2F41_HostCommandSend request, ServerCallContext context)
        {
            SCApplication scApp = SCApplication.getInstance();
            S2F42_HostCommandAck s2f42 = new S2F42_HostCommandAck();
            switch (request.RemoteCommand)
            {
                case RCMD.Cancel:
                    procsscHostCanaelCommand(scApp, request, ref s2f42);
                    break;
                case RCMD.Abort:
                    procsscHostAbortCommand(scApp, request, ref s2f42);
                    break;
                case RCMD.Pause:
                    procsscHostPauseCommand(scApp, ref s2f42);
                    break;
                case RCMD.Resume:
                    procsscHostResumeCommand(scApp, ref s2f42);
                    break;
            }
            return Task.FromResult(s2f42);
        }

        private void procsscHostResumeCommand(SCApplication scApp, ref S2F42_HostCommandAck s2f42)
        {
            ALINE line = scApp.getEQObjCacheManager().getLine();
            if (line.TSC_state_machine.State == ALINE.TSCState.PAUSED ||
                line.TSC_state_machine.State == ALINE.TSCState.PAUSING)
            {
                s2f42.Hcack = HCACK.Ack;
            }
            else
            {
                s2f42.Hcack = HCACK.CannotPerformNow;
            }
            if (s2f42.Hcack == HCACK.Ack)
            {
                line.ResumeToAuto(scApp.ReportBLL);
            }
        }

        private void procsscHostPauseCommand(SCApplication scApp, ref S2F42_HostCommandAck s2f42)
        {
            ALINE line = scApp.getEQObjCacheManager().getLine();
            if (line.TSC_state_machine.State == ALINE.TSCState.AUTO)
            {
                s2f42.Hcack = HCACK.Ack;
            }
            else
            {
                s2f42.Hcack = HCACK.CannotPerformNow;
            }
            if (s2f42.Hcack == HCACK.Ack)
            {
                scApp.LineService.TSCStateToPause();
            }
        }

        private void procsscHostCanaelCommand(SCApplication scApp, S2F41_HostCommandSend request, ref S2F42_HostCommandAck s2f42)
        {
            s2f42.Hcack = HCACK.Ack;
            string command_id = request.CommandId;
            ATRANSFER cmd_mcs = scApp.CMDBLL.GetTransferByID(command_id);
            if (cmd_mcs == null)
            {
                s2f42.Hcack = HCACK.NoSuchObjExist;
            }
            if (s2f42.Hcack == HCACK.Ack)
                scApp.TransferService.AbortOrCancel(command_id, ProtocolFormat.OHTMessage.CancelActionType.CmdCancel);
        }
        private void procsscHostAbortCommand(SCApplication scApp, S2F41_HostCommandSend request, ref S2F42_HostCommandAck s2f42)
        {
            s2f42.Hcack = HCACK.Ack;
            string command_id = request.CommandId;
            ATRANSFER cmd_mcs = scApp.CMDBLL.GetTransferByID(command_id);
            if (cmd_mcs == null)
            {
                s2f42.Hcack = HCACK.NoSuchObjExist;
            }
            if (s2f42.Hcack == HCACK.Ack)
                scApp.TransferService.AbortOrCancel(command_id, ProtocolFormat.OHTMessage.CancelActionType.CmdAbort);
        }
        #endregion S2F42

        public override Task<S2F18_DateAndTimeData> SendS2F17_DateAndTimeReq(S2F17_DateAndTimeRequest request, ServerCallContext context)
        {
            return base.SendS2F17_DateAndTimeReq(request, context);
        }

        public override Task<S2F50_EnhancedRemoteCommandAck> SendS2F49_TranCommand(S2F49_EnhancedRemoteCommand request, ServerCallContext context)
        {
            S2F50_EnhancedRemoteCommandAck s2f50 = new S2F50_EnhancedRemoteCommandAck();
            SCApplication scApp = SCApplication.getInstance();
            ALINE line = scApp.getEQObjCacheManager().getLine();
            try
            {
                LogHelper.RecordReportInfo(request);
                if (line.ServerPreStop)
                {
                    return Task.FromResult(new S2F50_EnhancedRemoteCommandAck
                    {
                        Hcack = HCACK.CannotPerformNow
                    });
                }
                string errorMsg = string.Empty;

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MCSDefaultMapActionReceive), Device: DEVICE_NAME_MCS,
                   Data: request.ToString());

                //s2f50.DataID = s2f49_transfer.SystemByte;需額外使用data id?
                s2f50.Hcack = HCACK.Rejected;

                string rtnStr = "";
                var check_result = doCheckMCSCommand(scApp, request, ref s2f50, out rtnStr);
                s2f50.Hcack = check_result.result;
                ATRANSFER transfer = request.ToATRANSFER(scApp.PortStationBLL);
                transfer.SetCheckResult(check_result.isSuccess, ((int)s2f50.Hcack).ToString());
                bool is_process_success = scApp.TransferService.Creat(transfer);
                if (!is_process_success)
                {
                    s2f50.Hcack = HCACK.Rejected;
                    rtnStr = $"creat mcs command fail, command info:{ transfer.ToString()}";
                }

                if (is_process_success && check_result.isSuccess)
                {
                    scApp.TransferBLL.web.receiveMCSCommandNotify();
                }
                else
                {
                    string xid = DateTime.Now.ToString(SCAppConstants.TimestampFormat_19);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(MCSDefaultMapActionReceive), Device: string.Empty,
                                  Data: rtnStr,
                                  XID: xid);
                    BCFApplication.onWarningMsg(this, new bcf.Common.LogEventArgs(rtnStr, xid));
                }
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(MCSDefaultMapActionReceive), Device: DEVICE_NAME_MCS,
                   Data: s2f50.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("ASEMCSDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F49_Receive_Remote_Command", ex);
            }
            LogHelper.RecordReportInfo(s2f50);
            return Task.FromResult(s2f50);
        }
        private (bool isSuccess, HCACK result) doCheckMCSCommand(SCApplication scApp, S2F49_EnhancedRemoteCommand request, ref S2F50_EnhancedRemoteCommandAck s2f50, out string check_result)
        {
            check_result = string.Empty;
            bool isSuccess = true;

            string command_id = request.CommandId;
            string priority = request.Priority;
            string replace = request.Replace;

            string carrier_id = request.CarrierId;
            string source_port_or_vh_location_id = request.SourcePort;
            string dest_port = request.DestPort;

            //確認命令是否已經執行中
            if (isSuccess)
            {
                var cmd_obj = scApp.CMDBLL.GetTransferByID(command_id);
                if (cmd_obj != null)
                {
                    check_result = $"MCS command id:{command_id} already exist.";
                    return (false, HCACK.Rejected);
                }
            }
            //確認參數是否正確 Todo
            //isSuccess &= checkCommandID(comminfo_check_result, s2F49_TRANSFER.REPITEMS.COMMINFO.COMMAINFOVALUE.COMMANDID.CPNAME, command_id);
            //isSuccess &= checkPriorityID(comminfo_check_result, s2F49_TRANSFER.REPITEMS.COMMINFO.COMMAINFOVALUE.PRIORITY.CPNAME, priority);
            //isSuccess &= checkReplace(comminfo_check_result, s2F49_TRANSFER.REPITEMS.COMMINFO.COMMAINFOVALUE.REPLACE.CPNAME, replace);

            //isSuccess &= checkCarierID(traninfo_check_result, s2F49_TRANSFER.REPITEMS.TRANINFO.TRANSFERINFOVALUE.CARRIERIDINFO.CPNAME, carrier_id);
            //isSuccess &= checkPortID(traninfo_check_result, s2F49_TRANSFER.REPITEMS.TRANINFO.TRANSFERINFOVALUE.SOUINFO.CPNAME, source_port_or_vh_location_id);
            //isSuccess &= checkPortID(traninfo_check_result, s2F49_TRANSFER.REPITEMS.TRANINFO.TRANSFERINFOVALUE.DESTINFO.CPNAME, dest_port);

            if (!isSuccess)
            {
                check_result = $"MCS command id:{command_id} has parameter invalid";
                return (false, HCACK.AtLeastOneParameterIsInvalid);
            }

            //確認是否有同一顆正在搬送的CST ID
            if (isSuccess)
            {
                var cmd_obj = scApp.CMDBLL.getExcuteCMD_MCSByCarrierID(carrier_id);
                if (cmd_obj != null)
                {
                    check_result = $"MCS command id:{command_id} of carrier id:{carrier_id} already excute by command id:{cmd_obj.ID.Trim()}";
                    return (false, HCACK.Rejected);
                }
            }

            //確認是否有在相同Load Port的Transfer Command且該命令狀態還沒有變成Transferring(代表還在Port上還沒搬走)
            if (isSuccess)
            {
                //M0.02 var cmd_obj = scApp.CMDBLL.getWatingCMDByFromTo(source_port_or_vh_id, dest_port);
                var cmd_obj = scApp.CMDBLL.getWatingCMDByFrom(source_port_or_vh_location_id);//M0.02 
                if (cmd_obj != null)
                {
                    check_result = $"MCS command id:{command_id} is same as orther mcs command id {cmd_obj.ID.Trim()} of load port.";//M0.02 
                                                                                                                                     //M0.02 check_result = $"MCS command id:{command_id} of transfer load port is same command id:{cmd_obj.CMD_ID.Trim()}";
                    return (false, HCACK.Rejected);
                }
            }

            //確認 Port是否存在
            bool source_is_a_port = scApp.PortStationBLL.OperateCatch.IsExist(source_port_or_vh_location_id);
            if (source_is_a_port)
            {
                isSuccess = true;
            }
            //如果不是PortID的話，則可能是VehicleID
            else
            {
                //isSuccess = scApp.VehicleBLL.cache.IsVehicleExistByRealID(source_port_or_vh_id);
                isSuccess = scApp.VehicleBLL.cache.IsVehicleLocationExistByLocationRealID(source_port_or_vh_location_id);
            }
            if (!isSuccess)
            {
                check_result = $"MCS command id:{command_id} - source Port:{source_port_or_vh_location_id} not exist.{Environment.NewLine}please confirm the port name";
                return (false, HCACK.NoSuchObjExist);
            }

            isSuccess = scApp.PortStationBLL.OperateCatch.IsExist(dest_port);
            if (!isSuccess)
            {
                check_result = $"MCS command id:{command_id} - destination Port:{dest_port} not exist.{Environment.NewLine}please confirm the port name";
                return (false, HCACK.NoSuchObjExist);
            }

            //如果Source是個Port才需要檢查
            if (source_is_a_port)
            {
                ////確認是否有車子來可以搬送
                //AVEHICLE vh = scApp.VehicleBLL.findBestSuitableVhStepByStepFromAdr(source_port_or_vh_id, E_VH_TYPE.None, isCheckHasVhCarry: true);
                //isSuccess = vh != null;
                //if (!isSuccess)
                //{
                //    check_result = $"No vehicle can reach mcs command id:{command_id} - source port:{source_port_or_vh_id}.{Environment.NewLine}please check the road traffic status.";
                //    return SECSConst.HCACK_Cannot_Perform_Now;
                //}
                ////確認路徑是否可以行走
                APORTSTATION source_port_station = scApp.PortStationBLL.OperateCatch.getPortStation(source_port_or_vh_location_id);
                APORTSTATION dest_port_station = scApp.PortStationBLL.OperateCatch.getPortStation(dest_port);
                isSuccess = scApp.GuideBLL.IsRoadWalkable(source_port_station.ADR_ID, dest_port_station.ADR_ID);
                if (!isSuccess)
                {
                    check_result = $"MCS command id:{command_id} ,source port:{source_port_or_vh_location_id} to destination port:{dest_port} no path to go{Environment.NewLine}," +
                        $"please check the road traffic status.";
                    return (false, HCACK.CannotPerformNow);
                }
            }
            //如果不是Port(則為指定車號)，要檢查是否從該車位置可以到達放貨地點
            else
            {
                //AVEHICLE carry_vh = scApp.VehicleBLL.cache.getVehicleByRealID(source_port_or_vh_id);
                AVEHICLE carry_vh = scApp.VehicleBLL.cache.getVehicleByLocationRealID(source_port_or_vh_location_id);
                APORTSTATION dest_port_station = scApp.PortStationBLL.OperateCatch.getPortStation(dest_port);
                isSuccess = scApp.GuideBLL.IsRoadWalkable(carry_vh.CUR_ADR_ID, dest_port_station.ADR_ID);
                if (!isSuccess)
                {
                    check_result = $"MCS command id:{command_id} ,vh:{source_port_or_vh_location_id} current address:{carry_vh.CUR_ADR_ID} to destination port:{dest_port}:{dest_port_station.ADR_ID} no path to go{Environment.NewLine}," +
                        $"please check the road traffic status.";
                    return (false, HCACK.CannotPerformNow);
                }
            }

            return (true, HCACK.Ack);
        }

    }
}
