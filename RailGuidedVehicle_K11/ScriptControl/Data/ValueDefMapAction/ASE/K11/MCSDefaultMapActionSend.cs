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

using com.mirle.AK0.RGV.HostMessage.E2H;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECSDriver;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.stc.Common;
using Grpc.Core;
using NLog;
using System;
using System.Collections.Generic;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction.ASE.K11
{
    public class MCSDefaultMapActionSend : IBSEMDriver
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string DEVICE_NAME_MCS = "MCS";
        Channel channel = null;
        AGVC_K11_E2H.AGVC_K11_E2HClient client = null;
        sc.App.SCApplication scApp = null;
        public MCSDefaultMapActionSend()
        {
            scApp = sc.App.SCApplication.getInstance();
            string s_grpc_client_ip = scApp.getString("gRPCClientIP", "127.0.0.1");
            string s_grpc_client_port = scApp.getString("gRPCClientPort", "7001");
            int.TryParse(s_grpc_client_port, out int i_grpc_client_port);
            line = scApp.getEQObjCacheManager().getLine();
            channel = new Channel(s_grpc_client_ip, i_grpc_client_port, ChannelCredentials.Insecure);
            client = new AGVC_K11_E2H.AGVC_K11_E2HClient(channel);

            line.addEventHandler(nameof(MCSDefaultMapActionSend)
            , BCFUtility.getPropertyName(() => line.Secs_Link_Stat)
                , (s1, e1) =>
                {
                    if (line.Secs_Link_Stat == App.SCAppConstants.LinkStatus.LinkFail)
                        scApp.LineService.OfflineWithHost();
                }
                );
        }

        public override AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object Vids)
        {
            return new AMCSREPORTQUEUE();
        }

        public override bool S6F11_PortEventStateChanged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11_SendAlarmCleared()
        {
            return true;
        }

        public override bool S6F11_SendAlarmSet()
        {
            return true;
        }

        public override bool S6F11_CarrierInstalled(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_8 report_obj = BulidReport8(vhID, carrierID, location);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_151_CarrierInstalled(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_CarrierInstalled(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport8(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_151_CarrierInstalled(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_CarrierRemoved(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport9(vhID, carrierID, location);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_152_CarrierRemoved(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_CarrierRemoved(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport9(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_152_CarrierRemoved(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_ControlStateLocal()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_002_OnlineLocal(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_ControlStateRemote()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_003_OnlineRemote(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_EquiptmentOffLine()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_001_Offline(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendMessage(AMCSREPORTQUEUE queue)
        {
            return true;
        }

        public override bool S6F11_RunTimeStatus(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11_TransferAbortCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport2(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_101_TransferAbortCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(report.reportObj, report.vtransfer);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }


        public RESULT_CODE convert2MCS(CompleteStatus? tran_cmp_status)
        {
            switch (tran_cmp_status)
            {
                case ProtocolFormat.OHTMessage.CompleteStatus.Move:
                case ProtocolFormat.OHTMessage.CompleteStatus.Load:
                case ProtocolFormat.OHTMessage.CompleteStatus.Unload:
                case ProtocolFormat.OHTMessage.CompleteStatus.Loadunload:
                case ProtocolFormat.OHTMessage.CompleteStatus.MoveToCharger:
                case ProtocolFormat.OHTMessage.CompleteStatus.ForceNormalFinishByOp:
                    return RESULT_CODE.Successful;
                case ProtocolFormat.OHTMessage.CompleteStatus.Cancel:
                case ProtocolFormat.OHTMessage.CompleteStatus.Abort:
                case ProtocolFormat.OHTMessage.CompleteStatus.VehicleAbort:
                case ProtocolFormat.OHTMessage.CompleteStatus.LongTimeInaction:
                //case ProtocolFormat.OHTMessage.CompleteStatus.ForceFinishByOp:
                case ProtocolFormat.OHTMessage.CompleteStatus.CommandInitailFail:
                case ProtocolFormat.OHTMessage.CompleteStatus.ForceAbnormalFinishByOp:
                case ProtocolFormat.OHTMessage.CompleteStatus.InterlockError:
                    return RESULT_CODE.Unsuccessful;
                case ProtocolFormat.OHTMessage.CompleteStatus.IdmisMatch:
                    return RESULT_CODE.CarrierIdUnmatch;
                case ProtocolFormat.OHTMessage.CompleteStatus.IdreadFailed:
                    return RESULT_CODE.BcrReadError;
                default:
                    throw new Exception($"參數錯誤:{tran_cmp_status}");
            }
        }

        public override bool S6F11_TransferAbortFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_102_TransferAbortFailed(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferAbortInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_103_TransferAbortInitiated(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferCancelCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_104_TransferCancelCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferCancelFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_105_TransferCancelFailed(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferCancelInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_106_TransferCancelInitiated(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;

        }

        public override bool S6F11_TransferCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport3(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_107_TransferCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferCompleted(VTRANSFER vtransfer, CompleteStatus completeStatus, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport3(vtransfer, completeStatus);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_107_TransferCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {

                var report = BulidReport1(cmdID);
                Report_ID_1 report_obj = report.reportObj;
                LogHelper.RecordHostReportInfo(report_obj, report.vtransfer);
                var ask = client.SendS6F11_108_TransferInitiated(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferPaused(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                client.SendS6F11_109_TransferPaused(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(report.reportObj, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TransferResumed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                client.SendS6F11_110_TransferResumed(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(report.reportObj, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_Transferring(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport1(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_111_Transferring(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TSCAutoCompleted()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_053_TscAutoComplete(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TSCAutoInitiated()
        {
            return true;
        }

        public override bool S6F11_TSCPauseCompleted()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_055_TscPauseComplete(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TSCPaused()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_056_TscPaused(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_TSCPauseInitiated()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_057_TscPauseInitiated(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_UnitAlarmCleared(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_11 report_obj = BulidReport11(vhID, alarmID, alarmTest);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_901_UnitErrorCleared(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_UnitAlarmSet(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_11 report_obj = BulidReport11(vhID, alarmID, alarmTest);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_902_UnitErrorSet(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleAcquireCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport7(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_203_VehicleAcquireCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleAcquireStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport7(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_202_VehicleAcquireStarted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleArrived(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport6(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_201_VehicleArrived(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleAssigned(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport5(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_204_VehicleAssigned(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleDeparted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport6(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_205_VehicleDeparted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleDepositCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport7(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_207_VehicleDepositCompleted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleDepositStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport7(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_206_VehicleDepositStarted(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport4(vhID);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_208_VehicleInstalled(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport4(vhID);
                LogHelper.RecordHostReportInfo(report_obj);
                var ask = client.SendS6F11_208_VehicleInstalled(report_obj);
                LogHelper.RecordHostReportInfoAsk(ask);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11_VehicleUnassinged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report = BulidReport5(cmdID);
                LogHelper.RecordHostReportInfo(report.reportObj, report.vtransfer);
                var ask = client.SendS6F11_210_VehicleUnassinged(report.reportObj);
                LogHelper.RecordHostReportInfoAsk(ask, report.vtransfer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        protected override void S2F41_HostCommand(object sender, SECSEventArgs e)
        {
            return;
        }

        protected override void S2F49_EnhancedRemoteCommandExtension(object sender, SECSEventArgs e)
        {
            return;
        }

        private (Report_ID_1 reportObj, VTRANSFER vtransfer) BulidReport1(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_1()
            {
                CommandId = vtransfer.ID,
                CarrierId = vtransfer.CARRIER_ID,
                SourcePort = vtransfer.HOSTSOURCE,
                DestPort = vtransfer.HOSTDESTINATION,
                Priority = vtransfer.PRIORITY,
                Replace = 0,
            };
            return (report_obj, vtransfer);
        }


        private (Report_ID_2 reportObj, VTRANSFER vtransfer) BulidReport2(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_2()
            {
                CommandId = vtransfer.ID,
                CarrierId = vtransfer.CARRIER_ID,
                CarrierLoc = vtransfer.CARRIER_ID,
                SourcePort = vtransfer.HOSTSOURCE,
                DestPort = vtransfer.HOSTDESTINATION,
                Priority = vtransfer.PRIORITY,
                Replace = 0,
                NextStockerPort = "",
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_3 reportObj, VTRANSFER vtransfer) BulidReport3(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_3()
            {
                CommandId = vtransfer.ID,
                CarrierId = vtransfer.CARRIER_ID,
                SourcePort = vtransfer.HOSTSOURCE,
                DestPort = vtransfer.HOSTDESTINATION,
                Priority = vtransfer.PRIORITY,
                Replace = 0,
                CarrierLoc = vtransfer.CARRIER_LOCATION,
                NextStockerPort = "",
                ResultCode = convert2MCS(vtransfer.COMPLETE_STATUS)
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_3 reportObj, VTRANSFER vtransfer) BulidReport3(VTRANSFER vtransfer, CompleteStatus completeStatus)
        {
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_3()
            {
                CommandId = vtransfer.ID,
                CarrierId = vtransfer.CARRIER_ID,
                SourcePort = vtransfer.HOSTSOURCE,
                DestPort = vtransfer.HOSTDESTINATION,
                Priority = vtransfer.PRIORITY,
                Replace = 0,
                CarrierLoc = vtransfer.CARRIER_LOCATION,
                NextStockerPort = "",
                ResultCode = convert2MCS(completeStatus)
            };
            return (report_obj, vtransfer);
        }
        private Report_ID_4 BulidReport4(string vhID)
        {
            var report_obj = new Report_ID_4()
            {
                VehicleId = vhID
            };
            return report_obj;
        }
        private (Report_ID_5 reportObj, VTRANSFER vtransfer) BulidReport5(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_5()
            {
                CommandId = vtransfer.ID,
                //VehicleId = vtransfer.VH_ID,
                VehicleId = vtransfer.getRealVhID(scApp.VehicleBLL),
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_6 reportObj, VTRANSFER vtransfer) BulidReport6(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_6()
            {
                VehicleId = vtransfer.getRealVhID(scApp.VehicleBLL),
                TransferPort = getTransferPort(vtransfer.COMMANDSTATE, vtransfer.HOSTSOURCE, vtransfer.HOSTDESTINATION),
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_7 reportObj, VTRANSFER vtransfer) BulidReport7(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_7()
            {
                VehicleId = vtransfer.getRealVhID(scApp.VehicleBLL),
                CarrierId = vtransfer.CARRIER_ID,
                TransferPort = getTransferPort(vtransfer.COMMANDSTATE, vtransfer.HOSTSOURCE, vtransfer.HOSTDESTINATION),
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_8 reportObj, VTRANSFER vtransfer) BulidReport8(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_8()
            {
                VehicleId = vtransfer.getRealVhID(scApp.VehicleBLL),
                CarrierId = vtransfer.CARRIER_ID,
                CarrierLoc = vtransfer.CARRIER_LOCATION,
                NextStockerPort = ""
            };
            return (report_obj, vtransfer);
        }
        private Report_ID_8 BulidReport8(string vhID, string carrierID, string carrierLoc)
        {
            var report_obj = new Report_ID_8()
            {
                VehicleId = vhID,
                CarrierId = carrierID,
                CarrierLoc = carrierLoc,
                NextStockerPort = ""
            };
            return (report_obj);
        }

        private (Report_ID_9 reportObj, VTRANSFER vtransfer) BulidReport9(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_9()
            {
                VehicleId = vtransfer.getRealVhID(scApp.VehicleBLL),
                CarrierId = vtransfer.CARRIER_ID,
                CarrierLoc = vtransfer.CARRIER_LOCATION,

            };
            return (report_obj, vtransfer);
        }
        private Report_ID_9 BulidReport9(string vhID, string carrierID, string carrierLoc)
        {
            var report_obj = new Report_ID_9()
            {
                VehicleId = vhID,
                CarrierId = carrierID,
                CarrierLoc = carrierLoc,
            };
            return (report_obj);
        }
        private string getTransferPort(int commandState, string sourcePort, string descPort)
        {
            if (commandState > ATRANSFER.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE)
            {
                return descPort;
            }
            else
            {
                return sourcePort;
            }
        }

        private Report_ID_11 BulidReport11(string unitID, string alarmID, string alarmText)
        {
            var report_obj = new Report_ID_11()
            {
                UnitId = unitID,
                AlarmId = alarmID,
                ErrorCode = 0,
                AlaemText = alarmText
            };
            return (report_obj);
        }

    }
}
