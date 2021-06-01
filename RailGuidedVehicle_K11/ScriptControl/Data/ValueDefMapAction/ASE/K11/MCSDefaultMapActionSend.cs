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
            channel = new Channel("127.0.0.1", 7001, ChannelCredentials.Insecure);
            client = new AGVC_K11_E2H.AGVC_K11_E2HClient(channel);
            scApp = sc.App.SCApplication.getInstance();
        }

        public override AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object Vids)
        {
            return new AMCSREPORTQUEUE();
        }

        public override bool S6F11PortEventStateChanged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendAlarmCleared()
        {
            return true;
        }

        public override bool S6F11SendAlarmSet()
        {
            return true;
        }

        public override bool S6F11SendCarrierInstalled(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_8 report_obj = BulidReport8(vhID, carrierID, location);
                LogHelper.RecordReportInfo(report_obj);
                 client.SendS6F11_151_CarrierInstalled(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendCarrierInstalled(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport8(cmdID).reportObj;
                client.SendS6F11_151_CarrierInstalled(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendCarrierRemoved(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport9(vhID, carrierID, location);
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_152_CarrierRemoved(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendCarrierRemoved(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport9(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_152_CarrierRemoved(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendControlStateLocal()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_002_OnlineLocal(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendControlStateRemote()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_003_OnlineRemote(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendEquiptmentOffLine()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_001_Offline(report_obj);
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

        public override bool S6F11SendRunTimeStatus(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            return true;
        }

        public override bool S6F11SendTransferAbortCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_2 report_obj = BulidReport2(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_101_TransferAbortCompleted(report_obj);
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

        public override bool S6F11SendTransferAbortFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_102_TransferAbortFailed(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferAbortInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_103_TransferAbortInitiated(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferCancelCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_104_TransferCancelCompleted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferCancelFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_105_TransferCancelFailed(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferCancelInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_106_TransferCancelInitiated(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;

        }

        public override bool S6F11SendTransferCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_3 report_obj = BulidReport3(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_107_TransferCompleted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferCompleted(VTRANSFER vtransfer, CompleteStatus completeStatus, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_3 report_obj = BulidReport3(vtransfer, completeStatus).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_107_TransferCompleted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_108_TransferInitiated(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferPaused(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_109_TransferPaused(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferResumed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_110_TransferResumed(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTransferring(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_1 report_obj = BulidReport1(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_111_Transferring(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTSCAutoCompleted()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_053_TscAutoComplete(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTSCAutoInitiated()
        {
            return true;
        }

        public override bool S6F11SendTSCPauseCompleted()
        {
            bool is_success = true;
            try
            {
                Report_ID_0 report_obj = new Report_ID_0()
                {
                    EqName = scApp.BC_ID
                };
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_055_TscPauseComplete(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendTSCPaused()
        {
            return true;
        }

        public override bool S6F11SendTSCPauseInitiated()
        {
            return true;
        }

        public override bool S6F11SendUnitAlarmCleared(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_11 report_obj = BulidReport11(vhID, alarmID, alarmTest);
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_901_UnitErrorCleared(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendUnitAlarmSet(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                Report_ID_11 report_obj = BulidReport11(vhID, alarmID, alarmTest);
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_902_UnitErrorSet(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleAcquireCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport7(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_203_VehicleAcquireCompleted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleAcquireStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport7(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_202_VehicleAcquireStarted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleArrived(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport6(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_201_VehicleArrived(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleAssigned(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport5(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_204_VehicleAssigned(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleDeparted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport6(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_205_VehicleDeparted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleDepositCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport7(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_207_VehicleDepositCompleted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleDepositStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport7(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_206_VehicleDepositStarted(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleInstalled(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport4(vhID);
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_208_VehicleInstalled(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleRemoved(string vhID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport4(vhID);
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_208_VehicleInstalled(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        public override bool S6F11SendVehicleUnassinged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            bool is_success = true;
            try
            {
                var report_obj = BulidReport5(cmdID).reportObj;
                LogHelper.RecordReportInfo(report_obj);
                client.SendS6F11_210_VehicleUnassinged(report_obj);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                is_success = false;
            }
            return is_success;
        }

        protected override void S2F41ReceiveHostCommand(object sender, SECSEventArgs e)
        {
            return;
        }

        protected override void S2F49ReceiveEnhancedRemoteCommandExtension(object sender, SECSEventArgs e)
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
                VehicleId = vtransfer.VH_ID,
            };
            return (report_obj, vtransfer);
        }
        private (Report_ID_6 reportObj, VTRANSFER vtransfer) BulidReport6(string cmdID)
        {
            VTRANSFER vtransfer = scApp.TransferBLL.db.vTransfer.GetVTransferByTransferID(cmdID);
            sc.Common.SCUtility.TrimAllParameter(vtransfer);
            var report_obj = new Report_ID_6()
            {
                VehicleId = vtransfer.VH_ID,
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
                VehicleId = vtransfer.VH_ID,
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
                VehicleId = vtransfer.VH_ID,
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
                VehicleId = vtransfer.VH_ID,
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
