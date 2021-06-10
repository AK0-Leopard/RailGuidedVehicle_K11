using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.stc.Data.SecsData;
using Google.Protobuf;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Common
{
    public static class LogHelper
    {
        public const string CALL_CONTEXT_KEY_WORD_SERVICE_ID = "SERVICE_ID";
        static ObjectPool<LogObj> LogObjPool = new ObjectPool<LogObj>(() => new LogObj());
        static Logger logger = LogManager.GetLogger("RecordReportInfo");


        public static void setCallContextKey_ServiceID(string service_id)
        {
            string xid = System.Runtime.Remoting.Messaging.CallContext.GetData(CALL_CONTEXT_KEY_WORD_SERVICE_ID) as string;
            if (SCUtility.isEmpty(xid))
            {
                System.Runtime.Remoting.Messaging.CallContext.SetData(LogHelper.CALL_CONTEXT_KEY_WORD_SERVICE_ID, service_id);
            }
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, SXFY Data,
            string VehicleID = null, string CST_ID_L = null, string CST_ID_R = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Transaction = null,
            [CallerMemberName] string Method = "")
        {
            return;
            //如果被F'Y'，Y可以被2整除的話代表是收到的
            bool isReceive = Data.getF() % 2 == 0;
            LogConstants.Type type = isReceive ? LogConstants.Type.Receive : LogConstants.Type.Send;
            Log(logger, LogLevel, Class, Device,
                Data: $"[{Data.SystemByte}]{Data.StreamFunction}-{Data.StreamFunctionName}",
                VehicleID: VehicleID,
                CST_ID_L: CST_ID_L,
                CST_ID_R: CST_ID_R,
                Type: type,
                LogID: LogID,
                Level: Level,
                ThreadID: ThreadID,
                Lot: Lot,
                XID: XID,
                Details: Data.toSECSString(),
                Method: Method
                );
        }


        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, int seq_num, IMessage Data,
            string VehicleID = null, string CST_ID_L = null, string CST_ID_R = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Transaction = null,
            [CallerMemberName] string Method = "")
        {
            return;
            string function_name = $"[{seq_num}]{Data.Descriptor.Name}";

            LogConstants.Type? type = null;
            if (function_name.Contains("_"))
            {
                int packet_id = 0;
                string[] function_name_splil = function_name.Split('_');
                if (int.TryParse(function_name_splil[1], out packet_id))
                {
                    type = packet_id > 100 ? LogConstants.Type.Receive : LogConstants.Type.Send;
                }
            }
            Log(logger, LogLevel, Class, Device,
            Data: function_name,
            Method: Method,
            VehicleID: VehicleID,
            CST_ID_L: CST_ID_L,
            CST_ID_R: CST_ID_R,
            Type: type,
            LogID: LogID,
            Level: Level,
            ThreadID: ThreadID,
            Lot: Lot,
            XID: XID,
            Details: Data.ToString()
            );
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, Exception Data,
            string VehicleID = null, string CST_ID_L = null, string CST_ID_R = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Details = null,
            [CallerMemberName] string Method = "")
        {
            Log(logger, LogLevel, Class, Device,
                Data: Data.ToString(),
                VehicleID: VehicleID,
                CST_ID_L: CST_ID_L,
                CST_ID_R: CST_ID_R,
                LogID: LogID,
                Level: Level,
                ThreadID: ThreadID,
                Lot: Lot,
                XID: XID,
                Details: Details,
                Method: Method
                );
        }

        public static void Log(Logger logger, NLog.LogLevel LogLevel,
            string Class, string Device, string Data = "",
            string VehicleID = null, string CST_ID_L = null, string CST_ID_R = null, LogConstants.Type? Type = null, string LogID = null, string Level = null, string ThreadID = null, string Lot = null, string XID = null, string Details = null,
            [CallerMemberName] string Method = "")
        {
            LogObj logObj = LogObjPool.GetObject();
            try
            {
                logObj.dateTime = DateTime.Now;
                logObj.Sequence = getSequence();
                logObj.LogLevel = LogLevel.Name;
                logObj.Class = Class;
                logObj.Method = Method;
                logObj.Device = Device;
                logObj.Data = Data;
                logObj.VH_ID = VehicleID;
                logObj.CST_ID_L = CST_ID_L;
                logObj.CST_ID_R = CST_ID_R;

                logObj.Type = Type;
                logObj.LogID = LogID;
                logObj.ThreadID = ThreadID != null ?
                    ThreadID : Thread.CurrentThread.ManagedThreadId.ToString();
                logObj.Lot = Lot;
                logObj.Level = Level;
                string service_id = System.Runtime.Remoting.Messaging.CallContext.GetData(CALL_CONTEXT_KEY_WORD_SERVICE_ID) as string;
                logObj.ServiceID = service_id;

                logObj.XID = XID;

                Transaction Transaction = getCurrentTransaction();
                logObj.TransactionID = Transaction == null ?
                    string.Empty : Transaction.TransactionInformation.LocalIdentifier.ToString();
                logObj.Details = Details;
                logObj.Index = "SystemProcessLog";

                //LogHelper.logger.Log(LogLevel, logObj.ToString());
                //Task.Run(() => SCApplication.getInstance().LineService.PublishSystemLog(logObj));
                SYSTEMPROCESS_INFO systemProc = new SYSTEMPROCESS_INFO();
                systemProc.TIME = DateTime.Now.ToString(SCAppConstants.DateTimeFormat_23);
                systemProc.SEQ = logObj.Sequence;
                systemProc.LOGLEVEL = LogLevel.Name == null ? string.Empty : LogLevel.Name;
                systemProc.CLASS = Class == null ? string.Empty : Class;
                systemProc.METHOD = Method == null ? string.Empty : Method;
                systemProc.DEVICE = Device == null ? string.Empty : Device;
                systemProc.DATA = Data == null ? string.Empty : Data;
                systemProc.VHID = VehicleID == null ? string.Empty : VehicleID;
                systemProc.CRRID = CST_ID_L == null ? string.Empty : CST_ID_L;
                systemProc.TYPE = Type.ToString();
                systemProc.LOGID = LogID == null ? string.Empty : LogID;
                systemProc.THREADID = logObj.ThreadID;
                systemProc.LOT = Lot == null ? string.Empty : Lot;
                systemProc.LEVEL = Level == null ? string.Empty : Level;
                systemProc.XID = XID == null ? string.Empty : XID;
                systemProc.TRXID = logObj.TransactionID;
                systemProc.DETAILS = Details == null ? string.Empty : Details;
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(SCApplication.getInstance().LineService.PublishSystemMsgInfo), systemProc);
            }
            catch (Exception e)
            {
                LogHelper.logger.Error($"{e}, Exception");
            }
            finally
            {
                LogObjPool.PutObject(logObj);
            }
        }
        private static Transaction getCurrentTransaction()
        {
            try
            {
                Transaction Transaction = Transaction.Current;
                return Transaction;
            }
            catch { return null; }
        }

        static object sequence_lock = new object();
        static UInt64 NextSequence = 1;
        static private UInt64 getSequence()
        {
            lock (sequence_lock)
            {
                UInt64 currentSeq = NextSequence;
                NextSequence++;
                return currentSeq;
            }

        }
        public static string PrintMessage(IMessage message)
        {
            var descriptor = message.Descriptor;
            var sb = new StringBuilder();
            sb.AppendLine($"Name:{descriptor.Name}");
            foreach (var field in descriptor.Fields.InDeclarationOrder())
            {
                sb.AppendLine($"{field.Name} : {field.Accessor.GetValue(message)}");
            }
            return sb.ToString();
        }
        public static string getIMessageName(IMessage message)
        {
            var descriptor = message.Descriptor;
            string name = descriptor.Name;
            if (name.Contains('_'))
            {
                string[] name_temp = name.Split('_');
                if (name_temp.Length >= 2)
                {
                    name = $"{name_temp[0]}_{name_temp[1]}";
                }
            }
            return name;
        }
        public static void RecordReportInfo(BLL.CMDBLL cmdBLL, AVEHICLE vh, IMessage message, int seqNum, [CallerMemberName] string Method = "")
        {
            string vhID = vh.VEHICLE_ID;
            string detail = PrintMessage(message);
            string function = getIMessageName(message);
            if (message is ID_31_TRANS_REQUEST)
            {
                var id_31 = message as ID_31_TRANS_REQUEST;
                var cmd_id = id_31.CmdID;
                string cst_id = id_31.CSTID;
                string lot_id = id_31.LOTID;
                var command_action = id_31.CommandAction;
                var load_adr = id_31.LoadAdr;
                var dest_adr = id_31.DestinationAdr;
                var load_port = id_31.LoadPortID;
                var unload_port = id_31.UnloadPortID;
                string display_load = sc.Common.SCUtility.isEmpty(load_port) ? load_adr : $"{load_port}({load_adr})";
                string display_dest = sc.Common.SCUtility.isEmpty(unload_port) ? dest_adr : $"{unload_port}({dest_adr})";

                var cmd = cmdBLL.cache.getExcuteCmd(cmd_id);
                string tran_id = "";
                if (cmd != null)
                {
                    tran_id = SCUtility.Trim(cmd.TRANSFER_ID);
                }
                var attenion_vaule_1 = new { cmdID = cmd_id, tranID = tran_id, cstID = cst_id, lotID = lot_id, commandAction = command_action, load = display_load, dest = display_dest };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);
            }
            else if (message is ID_131_TRANS_RESPONSE)
            {
                var id_131 = message as ID_131_TRANS_RESPONSE;
                var cmd_id = id_131.CmdID;
                var command_action = id_131.CommandAction;
                var reply_code = id_131.ReplyCode;
                var ng_reason = id_131.NgReason;
                var vehicle_c_ngreason = id_131.Vehice_C_Ng_Reason;
                var cmd = cmdBLL.cache.getExcuteCmd(cmd_id);
                string tran_id = "";
                if (cmd != null)
                {
                    tran_id = SCUtility.Trim(cmd.TRANSFER_ID);
                }

                var attenion_vaule_1 = new { cmdID = cmd_id, tranID = tran_id, commandAction = command_action, replyCode = reply_code, NgReason = ng_reason, vehicleControlNgReason = vehicle_c_ngreason };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);

            }
            else if (message is ID_132_TRANS_COMPLETE_REPORT)
            {
                var id_132 = message as ID_132_TRANS_COMPLETE_REPORT;
                var cmd_id = id_132.CmdID;
                string cst_id = id_132.CSTID;
                var complete_status = id_132.CmpStatus;
                var cmd = cmdBLL.cache.getExcuteCmd(cmd_id);
                string tran_id = "";
                if (cmd != null)
                {
                    tran_id = SCUtility.Trim(cmd.TRANSFER_ID);
                }
                var attenion_vaule_1 = new { cmdID = cmd_id, tranID = tran_id, cstID = cst_id, completeStatus = complete_status };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);

            }
            else if (message is ID_32_TRANS_COMPLETE_RESPONSE)
            {
                var id_32 = message as ID_32_TRANS_COMPLETE_RESPONSE;
                var reply_code = id_32.ReplyCode;
                var wait_time = id_32.WaitTime;

                var attenion_vaule_1 = new { replyCode = reply_code, waitTime = wait_time };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);

            }
            else if (message is ID_134_TRANS_EVENT_REP)
            {
                var id_134 = message as ID_134_TRANS_EVENT_REP;
                var attenion_vaule = new { secID = id_134.CurrentSecID, adrID = id_134.CurrentSecID, distance = id_134.SecDistance };
                LogManager.GetLogger("RecordReportInfo").WithProperty("msgDetail", detail).Info(vh, "{vhID}|{method}|{seqNum} {attenion_vaule}"
                                                                , vhID
                                                                , function
                                                                , seqNum
                                                                , attenion_vaule);
                //LogManager.GetLogger("RecordReportInfo").WithProperty
            }
            else if (message is ID_136_TRANS_EVENT_REP)
            {
                var id_136 = message as ID_136_TRANS_EVENT_REP;
                var event_tpye = id_136.EventType;
                string cmd_id = id_136.CmdID;
                string trna_id = "";
                var cmd = cmdBLL.cache.getExcuteCmd(cmd_id);
                if (cmd != null)
                {
                    trna_id = SCUtility.Trim(cmd.TRANSFER_ID);
                }
                switch (event_tpye)
                {
                    case EventType.LoadArrivals:
                    case EventType.Vhloading:
                    case EventType.LoadComplete:
                    case EventType.UnloadArrivals:
                    case EventType.Vhunloading:
                    case EventType.UnloadComplete:
                        var attenion_vaule_1 = new { cmdID = cmd_id, tranID = trna_id, cstID = id_136.CSTID, location = id_136.Location };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, event_tpye, vhID, seqNum, attenion_vaule_1);
                        break;
                    case EventType.ReserveReq:
                        var attenion_vaule_2 = new { eventType = event_tpye, reserveSecInfos = id_136.ReserveInfos };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, event_tpye, vhID, seqNum, attenion_vaule_2);
                        break;
                    case EventType.Bcrread:
                        var attenion_vaule_3 = new { eventType = event_tpye, cmdID = id_136.CmdID, BcrReadResult = id_136.BCRReadResult, cstID = id_136.CSTID };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, event_tpye, vhID, seqNum, attenion_vaule_3);
                        break;
                    default:
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum}"
                                       , function, event_tpye, vhID, seqNum);
                        break;
                }
            }
            else if (message is ID_36_TRANS_EVENT_RESPONSE)
            {
                var id_36 = message as ID_36_TRANS_EVENT_RESPONSE;
                var eventType = id_36.EventType;
                string cmd_id = id_36.CmdID;
                string trna_id = "";
                var cmd = cmdBLL.cache.getExcuteCmd(cmd_id);
                if (cmd != null)
                {
                    trna_id = SCUtility.Trim(cmd.TRANSFER_ID);
                }

                switch (eventType)
                {
                    case EventType.LoadArrivals:
                    case EventType.Vhloading:
                    case EventType.LoadComplete:
                    case EventType.UnloadArrivals:
                    case EventType.Vhunloading:
                    case EventType.UnloadComplete:
                        var attenionVaule_1 = new { cmdID = cmd_id, tranID = trna_id, replyAction = id_36.ReplyAction };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, eventType, vhID, seqNum, attenionVaule_1);
                        break;
                    case EventType.ReserveReq:
                        var attenionVaule_2 = new { cmdID = id_36.CmdID, isReserveOK = id_36.IsReserveSuccess, reserveSecInfos = id_36.ReserveInfos };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, eventType, vhID, seqNum, attenionVaule_2);
                        break;
                    case EventType.Bcrread:
                        var attenionVaule_3 = new { cmdID = id_36.CmdID, replyAction = id_36.ReplyAction, renameID = id_36.RenameCarrierID };
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum} {@attenionVaule}"
                                        , function, eventType, vhID, seqNum, attenionVaule_3);
                        break;
                    default:
                        logger.WithProperty("msgDetail", detail).
                               Info(vh, "{method} | {eventType} | {vhID} | {seqNum}"
                                        , function, eventType, vhID, seqNum);
                        break;
                }
            }
            else if (message is ID_138_GUIDE_INFO_REQUEST)
            {
                var id_138 = message as ID_138_GUIDE_INFO_REQUEST;
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} "
                                , function, vhID, seqNum);
            }
            else if (message is ID_38_GUIDE_INFO_RESPONSE)
            {
                var id_38 = message as ID_38_GUIDE_INFO_RESPONSE;
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} "
                                , function, vhID, seqNum);
            }
            else if (message is ID_37_TRANS_CANCEL_REQUEST)
            {
                var id_37 = message as ID_37_TRANS_CANCEL_REQUEST;
                var cmd_id = id_37.CmdID;
                var cancel_action = id_37.CancelAction;
                var attenion_vaule_1 = new { cmdID = cmd_id, cancelAction = cancel_action };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);
            }
            else if (message is ID_137_TRANS_CANCEL_RESPONSE)
            {
                var id_137 = message as ID_137_TRANS_CANCEL_RESPONSE;
                var cmd_id = id_137.CmdID;
                var cancel_action = id_137.CancelAction;
                var reply_code = id_137.ReplyCode;
                var attenion_vaule_1 = new { cmdID = cmd_id, cancelAction = cancel_action, replyCode = reply_code };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);
            }
            else if (message is ID_39_PAUSE_REQUEST)
            {
                var id_39 = message as ID_39_PAUSE_REQUEST;
                var event_tpye = id_39.EventType;
                var pause_tpye = id_39.PauseType;
                var attenion_vaule_1 = new { pauseEventType = event_tpye, pauseType = pause_tpye };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);
            }
            else if (message is ID_139_PAUSE_RESPONSE)
            {
                var id_139 = message as ID_139_PAUSE_RESPONSE;
                var event_tpye = id_139.EventType;
                var attenion_vaule_1 = new { pauseEventType = event_tpye, repleCode = id_139.ReplyCode };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenion_vaule_1);
            }
            else if (message is ID_43_STATUS_REQUEST)
            {
                var id_43 = message as ID_43_STATUS_REQUEST;
                var attenionVaule_1 = new
                {
                    time = id_43.SystemTime
                };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenionVaule_1);
            }
            else if (message is ID_143_STATUS_RESPONSE)
            {
                var id_143 = message as ID_143_STATUS_RESPONSE;
                string cmd_id1 = id_143.CmdId1;
                string trna_id1 = "";
                var cmd1 = cmdBLL.cache.getExcuteCmd(cmd_id1);
                if (cmd1 != null)
                {
                    trna_id1 = SCUtility.Trim(cmd1.TRANSFER_ID);
                }
                string cmd_id2 = id_143.CmdId2;
                string trna_id2 = "";
                var cmd2 = cmdBLL.cache.getExcuteCmd(cmd_id2);
                if (cmd2 != null)
                {
                    trna_id2 = SCUtility.Trim(cmd2.TRANSFER_ID);
                }
                string cmd_id3 = id_143.CmdId3;
                string trna_id3 = "";
                var cmd3 = cmdBLL.cache.getExcuteCmd(cmd_id3);
                if (cmd1 != null)
                {
                    trna_id3 = SCUtility.Trim(cmd3.TRANSFER_ID);
                }
                string cmd_id4 = id_143.CmdId4;
                string trna_id4 = "";
                var cmd4 = cmdBLL.cache.getExcuteCmd(cmd_id4);
                if (cmd4 != null)
                {
                    trna_id4 = SCUtility.Trim(cmd4.TRANSFER_ID);
                }

                var attenionVaule_1 = new
                {
                    cmdID1 = cmd_id1,
                    tranID1 = trna_id1,
                    cmdID2 = cmd_id2,
                    tranID2 = trna_id2,
                    cmdID3 = cmd_id3,
                    tranID3 = trna_id3,
                    cmdID4 = cmd_id4,
                    tranID4 = trna_id4,
                    HasCst_L = id_143.HasCstL,
                    HasCst_R = id_143.HasCstR,
                    CstID_L = id_143.CstIdL,
                    CstID_R = id_143.CstIdR
                };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenionVaule_1);
            }
            else if (message is ID_144_STATUS_CHANGE_REP)
            {
                var id_144 = message as ID_144_STATUS_CHANGE_REP;
                string cmd_id1 = id_144.CmdId1;
                string trna_id1 = "";
                var cmd1 = cmdBLL.cache.getExcuteCmd(cmd_id1);
                if (cmd1 != null)
                {
                    trna_id1 = SCUtility.Trim(cmd1.TRANSFER_ID);
                }
                string cmd_id2 = id_144.CmdId2;
                string trna_id2 = "";
                var cmd2 = cmdBLL.cache.getExcuteCmd(cmd_id2);
                if (cmd2 != null)
                {
                    trna_id2 = SCUtility.Trim(cmd2.TRANSFER_ID);
                }
                string cmd_id3 = id_144.CmdId3;
                string trna_id3 = "";
                var cmd3 = cmdBLL.cache.getExcuteCmd(cmd_id3);
                if (cmd1 != null)
                {
                    trna_id3 = SCUtility.Trim(cmd3.TRANSFER_ID);
                }
                string cmd_id4 = id_144.CmdId4;
                string trna_id4 = "";
                var cmd4 = cmdBLL.cache.getExcuteCmd(cmd_id4);
                if (cmd4 != null)
                {
                    trna_id4 = SCUtility.Trim(cmd4.TRANSFER_ID);
                }

                var attenionVaule_1 = new
                {
                    cmdID1 = cmd_id1,
                    tranID1 = trna_id1,
                    cmdID2 = cmd_id2,
                    tranID2 = trna_id2,
                    cmdID3 = cmd_id3,
                    tranID3 = trna_id3,
                    cmdID4 = cmd_id4,
                    tranID4 = trna_id4,
                    HasCst_L = id_144.HasCstL,
                    HasCst_R = id_144.HasCstR,
                    CstID_L = id_144.CstIdL,
                    CstID_R = id_144.CstIdR
                };
                logger.WithProperty("msgDetail", detail).
                       Info(vh, "{method} | {vhID} | {seqNum} {@attenionVaule}"
                                , function, vhID, seqNum, attenionVaule_1);
            }

        }
        public static void RecordReportInfo(IMessage message, [CallerMemberName] string method = "", int seqNum = 0)
        {
        }
        public static void RecordHostReportInfoAsk(IMessage message, VTRANSFER vTrn = null, [CallerMemberName] string method = "", int seqNum = 0)
        {
            RecordHostReportInfo(message, vTrn, $"{method}Ask", seqNum);
        }
        public static void RecordHostReportInfo(IMessage message, VTRANSFER vTrn = null, [CallerMemberName] string method = "", int seqNum = 0)
        {
            string tran_id = "";
            string ExcuteVhCmdID = "";
            string ExcuteVhID = "";
            string vCstID = "";
            string eventType = "";
            if (method.Contains('_'))
            {
                var s_temp = method.Split('_');
                method = s_temp[0];
                eventType = s_temp[1];
            }

            if (vTrn != null)
            {
                tran_id = SCUtility.Trim(vTrn.ID);
                ExcuteVhCmdID = SCUtility.Trim(vTrn.EXCUTE_CMD_ID);
                ExcuteVhID = SCUtility.Trim(vTrn.VH_ID);
                vCstID = SCUtility.Trim(vTrn.CARRIER_ID);
            }
            string detail = PrintMessage(message);
            var attenionVaule_1 = new { cmdID = ExcuteVhCmdID, tranID = tran_id, cstID = vCstID, vhID = ExcuteVhID };
            logger.WithProperty("msgDetail", detail).
                   Info(vTrn, "{method} | {eventType} | {seqNum} {@attenionVaule}"
                              , method, eventType, seqNum, attenionVaule_1);

        }

    }

    public static class LogConstants
    {
        public enum Type
        {
            Send,
            Receive
        }
    }
}
