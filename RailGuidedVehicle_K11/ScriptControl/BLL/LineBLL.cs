﻿//*********************************************************************************
//      LineBLL.cs
//*********************************************************************************
// File Name: LineBLL.cs
// Description: 業務邏輯：Line、Zone、Node、Equipment、Unit機台基本修改
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.Utility.ul.Data.VO;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class LineBLL
    {
        private SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private LineDao lineDao = null;
        private ZoneDao zoneDao = null;
        private NodeDao nodeDao = null;
        private EqptDao eqptDao = null;
        private UnitDao unitDao = null;
        private PortDao portDao = null;
        private BufferPortDao bufferPortDao = null;
        private ECDataMapDao ecDataMapDao = null;

        public LineBLL()
        {

        }

        /// <summary>
        /// Starts the specified sc application.
        /// </summary>
        /// <param name="scApp">The sc application.</param>
        public void start(SCApplication scApp)
        {
            this.scApp = scApp;
            lineDao = scApp.LineDao;
            zoneDao = scApp.ZoneDao;
            nodeDao = scApp.NodeDao;
            eqptDao = scApp.EqptDao;
            unitDao = scApp.UnitDao;
            portDao = scApp.PortDao;
            bufferPortDao = scApp.BufferPortDao;
            ecDataMapDao = scApp.ECDataMapDao;
            ecDataMapDao = scApp.ECDataMapDao;
        }

        /// <summary>
        /// Gets the first line.
        /// </summary>
        /// <returns>Line.</returns>
        public ALINE getFirstLine()
        {
            ALINE rtnLine = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnLine = lineDao.getFirstLine(conn, false);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Not Found Any Record from ALINE");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnLine;
        }

        /// <summary>
        /// 為確保系統僅保存一個Line，所以此Method還會刪除非此Line_id的資料
        /// </summary>
        /// <param name="line_id">The line_id.</param>
        /// <returns>Line.</returns>
        public ALINE getLineByIDAndDeleteOtherLine(string line_id)
        {
            ALINE rtnLine = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                List<ALINE> allLines = lineDao.loadAllLineInDB(conn);
                foreach (ALINE line in allLines)
                {
                    if (BCFUtility.isMatche(line_id, line.LINE_ID))
                    {
                        continue;
                    }
                    lineDao.deleteLineByLineID(conn, line.LINE_ID);
                }
                rtnLine = lineDao.getLineByID(conn, false, line_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Not Found Record from ALINE [line_id:{0}]", line_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnLine;
        }

        /// <summary>
        /// Loads all line in database.
        /// </summary>
        /// <returns>List&lt;Line&gt;.</returns>
        public List<ALINE> loadAllLineInDB()
        {
            List<ALINE> rtnLine = new List<ALINE>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnLine = lineDao.loadAllLineInDB(conn);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Not Found Record from ALINE");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnLine;
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>Boolean.</returns>
        public Boolean createLine(ALINE line)
        {
            return createLine(line.LINE_ID, line.HOST_MODE, line.LINE_STAT);
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="line_id">The line_id.</param>
        /// <param name="host_mode">The host_mode.</param>
        /// <param name="line_stat">The line_stat.</param>
        /// <returns>Boolean.</returns>
        public Boolean createLine(string line_id, int host_mode, int line_stat)
        {
            Boolean isSuccess = false;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                lineDao.deleteAllLine(conn);
                ALINE line = new ALINE()
                {
                    LINE_ID = line_id,
                    HOST_MODE = host_mode,
                    LINE_STAT = line_stat
                };
                lineDao.insertLine(conn, line);
                conn.Commit();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to ALINE [line_id:{0}]", line_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return isSuccess;
        }


        public bool updateHostControlState(SCAppConstants.LineHostControlState.HostControlState hostControlState)
        {
            try
            {
                ALINE line = scApp.getEQObjCacheManager().getLine();
                line.Host_Control_State = hostControlState;
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to ALINE.", ex);
                return false;
            }
            return true;

        }

        public static byte[] Convert2GPB_TcpMsgInfo(dynamic logEntry)
        {
            EQLOG_INFO eq_gpp = new EQLOG_INFO();
            byte[] arrayByte = null;

            try
            {
                eq_gpp.TIME = logEntry.RPT_TIME;
                if (SCUtility.isMatche(logEntry.MSG_FROM, SCUtility.MSG_ROLE_MCS))
                {
                    eq_gpp.SENDRECEIVE = logEntry.MSG_FROM + " => " + logEntry.MSG_TO;
                }
                else if (SCUtility.isMatche(logEntry.MSG_FROM, SCUtility.MSG_ROLE_CONTROL))
                {
                    if (SCUtility.isMatche(logEntry.MSG_TO, SCUtility.MSG_ROLE_MCS))
                    {
                        eq_gpp.SENDRECEIVE = logEntry.MSG_TO + " <= " + logEntry.MSG_FROM;
                    }
                    else
                    {
                        eq_gpp.SENDRECEIVE = logEntry.MSG_FROM + " => " + logEntry.MSG_TO;
                    }
                }
                else
                {
                    eq_gpp.SENDRECEIVE = logEntry.MSG_TO + " <= " + logEntry.MSG_FROM;
                }

                eq_gpp.FUNNAME = logEntry.FUN_NAME;
                eq_gpp.SEQNO = logEntry.SEQ_NUM;
                eq_gpp.VHID = logEntry.VH_ID;
                eq_gpp.OHTCCMDID = logEntry.OHTC_CMD_ID;
                eq_gpp.ACTTYPE = logEntry.ACT_TYPE;
                eq_gpp.MCSCMDID = logEntry.MCS_CMD_ID;
                eq_gpp.EVENTTYPE = logEntry.EVENT_TYPE;
                eq_gpp.VHSTATUS = logEntry.VH_STATUS;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Function Name: " + logEntry.FUN_NAME);
                sb.AppendLine("Sequence No: " + logEntry.SEQ_NUM);
                sb.AppendLine("VH ID: " + logEntry.VH_ID);
                sb.AppendLine("OHTC CMD ID: " + logEntry.OHTC_CMD_ID);
                sb.AppendLine("ACT Type: " + logEntry.ACT_TYPE);
                sb.AppendLine("MCS CMD ID: " + logEntry.MCS_CMD_ID);
                sb.AppendLine("Travel Distance: " + logEntry.TRAVEL_DIS);
                sb.AppendLine("ADR ID: " + logEntry.ADR_ID);
                sb.AppendLine("SEC ID: " + logEntry.SECID);
                sb.AppendLine("Event Type: " + logEntry.EVENT_TYPE);
                sb.AppendLine("SEC Distance: " + logEntry.SEC_DIS);
                sb.AppendLine("Block SEC Distance: " + logEntry.BLOCK_SEC_ID);
                sb.AppendLine("Is Block Pass: " + logEntry.IS_BLOCK_PASS);
                sb.AppendLine("Is HID Pass: " + logEntry.IS_HID_PASS);
                sb.AppendLine("VH Status: " + logEntry.VH_STATUS);
                sb.AppendLine("Result: " + logEntry.RESULT);
                sb.AppendLine("Message:");

                string sMsg = logEntry.MSG_BODY;
                List<string> sMsgList = sMsg.Split(',').ToList();
                foreach (string msg in sMsgList)
                {
                    string tmpMsg = msg.Replace("\"", " ");
                    sb.AppendLine(tmpMsg);
                }

                eq_gpp.MESSAGE = sb.ToString();

                arrayByte = new byte[eq_gpp.CalculateSize()];
                eq_gpp.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }

            return arrayByte;
        }

        public static byte[] Convert2GPB_SECSMsgInfo(LogTitle_SECS logSECS)
        {
            HOSTLOG_INFO host_gpp = new HOSTLOG_INFO();
            byte[] arrayByte = null;

            try
            {
                host_gpp.TIME = logSECS.Time;
                host_gpp.EQID = logSECS.EQ_ID;
                host_gpp.SENDRECEIVE = logSECS.SendRecive;
                host_gpp.SX = logSECS.Sx;
                host_gpp.FY = logSECS.Fy;
                host_gpp.DEVICE = logSECS.DeviceID;
                host_gpp.FUNNAME = logSECS.FunName;
                host_gpp.MESSAGE = logSECS.Message;

                arrayByte = new byte[host_gpp.CalculateSize()];
                host_gpp.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }

            return arrayByte;
        }

        public static byte[] Convert2GPB_SystemMsgInfo(SYSTEMPROCESS_INFO system_gpp)
        {
            byte[] arrayByte = null;

            try
            {
                arrayByte = new byte[system_gpp.CalculateSize()];
                system_gpp.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }

            return arrayByte;
        }


        public static byte[] Convert2GPB_LineInfo(ALINE line)
        {
            LINE_INFO line_gpb = new LINE_INFO()
            {
                Host = line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK
             ? LinkStatus.LinkOk : LinkStatus.LinkFail,
                HostMode = line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote
             ? HostMode.OnlineRemote : HostMode.Offline,
                TSCState = line.SCStats == ALINE.TSCState.AUTO ? TSCState.Auto : line.SCStats == ALINE.TSCState.PAUSING ? TSCState.Pausing :
                line.SCStats == ALINE.TSCState.PAUSED ? TSCState.Paused : line.SCStats == ALINE.TSCState.TSC_INIT ? TSCState.Tscint : TSCState.Paused,
                PLC = LinkStatus.LinkFail,
                IMS = line.DetectionSystemExist == SCAppConstants.ExistStatus.Exist ?
             LinkStatus.LinkOk : LinkStatus.LinkFail,
                CurrntVehicleModeAutoRemoteCount = line.CurrntVehicleModeAutoRemoteCount,
                CurrntCSTStatueTransferCount = line.CurrntCSTStatueTransferCount,
                CurrntVehicleModeAutoLoaclCount = line.CurrntVehicleModeAutoLoaclCount,
                CurrntVehicleStatusIdelCount = line.CurrntVehicleStatusIdelCount,
                CurrntVehicleStatusErrorCount = line.CurrntVehicleStatusErrorCount,
                CurrntCSTStatueWaitingCount = line.CurrntCSTStatueWaitingCount,
                CurrntHostCommandTransferStatueAssignedCount = line.CurrntHostCommandTransferStatueAssignedCount,
                CurrntHostCommandTransferStatueWaitingCounr = line.CurrntHostCommandTransferStatueWaitingCounr,
                LineID = line.LINE_ID,
                AlarmHappen = line.IsAlarmHappened
            };

            byte[] arrayByte = new byte[line_gpb.CalculateSize()];
            line_gpb.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static byte[] Convert2GPB_ConnectionInfo(ALINE line)
        {
            LINE_INFO line_gpb = new LINE_INFO()
            {
                Host = line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK
             ? LinkStatus.LinkOk : LinkStatus.LinkFail,
                HostMode = line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote
             ? HostMode.OnlineRemote : line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local ? HostMode.OnlineLocal : HostMode.Offline,
                TSCState = line.SCStats == ALINE.TSCState.AUTO ? TSCState.Auto : line.SCStats == ALINE.TSCState.PAUSING ? TSCState.Pausing :
                line.SCStats == ALINE.TSCState.PAUSED ? TSCState.Paused : line.SCStats == ALINE.TSCState.TSC_INIT ? TSCState.Tscint : line.SCStats == ALINE.TSCState.NONE ? TSCState.Tscnone : TSCState.Paused
            };

            byte[] arrayByte = new byte[line_gpb.CalculateSize()];
            line_gpb.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static byte[] Convert2GPB_TransferInfo(ALINE line)
        {
            TRANSFER_INFO line_gpb = new TRANSFER_INFO()
            {
                MCSCommandAutoAssign = line.MCSCommandAutoAssign
            };

            byte[] arrayByte = new byte[line_gpb.CalculateSize()];
            line_gpb.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static byte[] Convert2GPB_OnlineCheckInfo(ALINE line)
        {
            ONLINE_CHECK_INFO online_gpb = new ONLINE_CHECK_INFO()
            {
                CurrentPortStateChecked = line.CurrentPortStateChecked,
                CurrentStateChecked = line.CurrentStateChecked,
                EnhancedVehiclesChecked = line.EnhancedVehiclesChecked,
                TSCStateChecked = line.TSCStateChecked,
                //UnitAlarmStateListChecked = line.UnitAlarmStateListChecked,
                EnhancedTransfersChecked = line.EnhancedTransfersChecked,
                EnhancedCarriersChecked = line.EnhancedCarriersChecked
                //LaneCutListChecked = line.LaneCutListChecked
            };

            byte[] arrayByte = new byte[online_gpb.CalculateSize()];
            online_gpb.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static byte[] Convert2GPB_PingCheckInfo(ALINE line)
        {
            PING_CHECK_INFO ping_gpb = new PING_CHECK_INFO()
            {
                MCSConnectionSuccess = line.MCSConnectionSuccess,
                RouterConnectionSuccess = line.RouterConnectionSuccess,
                AGV1ConnectionSuccess = line.AGV1ConnectionSuccess,
                AGV2ConnectionSuccess = line.AGV2ConnectionSuccess,
                AGV3ConnectionSuccess = line.AGV3ConnectionSuccess,
                AGV4ConnectionSuccess = line.AGV4ConnectionSuccess,
                AGV5ConnectionSuccess = line.AGV5ConnectionSuccess,
                AGV6ConnectionSuccess = line.AGV6ConnectionSuccess,
                AGV7ConnectionSuccess = line.AGV7ConnectionSuccess,
                AGV8ConnectionSuccess = line.AGV8ConnectionSuccess,
                AGV9ConnectionSuccess = line.AGV9ConnectionSuccess,
                AGV10ConnectionSuccess = line.AGV10ConnectionSuccess,
                AGV11ConnectionSuccess = line.AGV11ConnectionSuccess,
                AGV12ConnectionSuccess = line.AGV12ConnectionSuccess,
                AGV13ConnectionSuccess = line.AGV13ConnectionSuccess,
                AGV14ConnectionSuccess = line.AGV14ConnectionSuccess,
                ChargePLCConnectionSuccess = line.ChargePLCConnectionSuccess,
                ADAM1ConnectionSuccess = line.ADAM1ConnectionSuccess,
                ADAM2ConnectionSuccess = line.ADAM2ConnectionSuccess,
                ADAM3ConnectionSuccess = line.ADAM3ConnectionSuccess,
                ADAM4ConnectionSuccess = line.ADAM4ConnectionSuccess,
                ADAM5ConnectionSuccess = line.ADAM5ConnectionSuccess,
                AP1ConnectionSuccess = line.AP1ConnectionSuccess,
                AP2ConnectionSuccess = line.AP2ConnectionSuccess,
                AP3ConnectionSuccess = line.AP3ConnectionSuccess,
                AP4ConnectionSuccess = line.AP4ConnectionSuccess,
                AP5ConnectionSuccess = line.AP5ConnectionSuccess,
                AP6ConnectionSuccess = line.AP6ConnectionSuccess,
                AP7ConnectionSuccess = line.AP7ConnectionSuccess,
                AP8ConnectionSuccess = line.AP8ConnectionSuccess,
                AP9ConnectionSuccess = line.AP9ConnectionSuccess,
                AP10ConnectionSuccess = line.AP10ConnectionSuccess
            };
            byte[] arrayByte = new byte[ping_gpb.CalculateSize()];
            ping_gpb.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        public static byte[] Convert2GPB_TipMsgIngo(List<MPCTipMessage> msgInfos)
        {
            TIP_MESSAGE_COLLECTION tip_msg_collection = new TIP_MESSAGE_COLLECTION();

            foreach (MPCTipMessage msg in msgInfos)
            {
                TIP_MESSAGE_INFO msg_gpb = new TIP_MESSAGE_INFO()
                {
                    Time = msg.Time,
                    Message = msg.Msg,
                    MsgLevel = msg.MsgLevel,
                    XID = msg.XID == null ? "" : msg.XID
                };
                tip_msg_collection.TIPMESSAGEINFOS.Add(msg_gpb);
            }

            byte[] arrayByte = new byte[tip_msg_collection.CalculateSize()];
            tip_msg_collection.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
            return arrayByte;
        }

        //public static byte[] Convert2GPB_SystemLog(LogObj obj)
        //{
        //    SYSTEM_LOG system_log = new SYSTEM_LOG()
        //    {
        //        DateTime = obj.dateTime.ToString("yyyy.MM.dd HH:mm:ss.fffff"),
        //        LogLevel = obj.LogLevel,
        //        Process = obj.Process,
        //        Class = obj.Class,
        //        Method = obj.Method,
        //        Device = obj.Device,
        //        LogID = obj.LogID,
        //        ThreadID = obj.ThreadID,
        //        Data = obj.Data,
        //        VHID = obj.VH_ID,
        //        CarrierID = obj.CarrierID,
        //        Lot = obj.Lot,
        //        Level = obj.Level,
        //        ServiceID = obj.ServiceID,
        //        XID = obj.XID,
        //        Sequence = obj.Sequence.ToString(),
        //        TransactionID = obj.TransactionID,
        //        Details = obj.Details,
        //        Index = obj.Index,
        //    };

        //    byte[] arrayByte = new byte[system_log.CalculateSize()];
        //    system_log.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
        //    return arrayByte;
        //}

        public static LINE_INFO Convert2Object_LineInfo(byte[] rawData)
        {
            return SCUtility.ToObject<LINE_INFO>(rawData);
        }

        public static ONLINE_CHECK_INFO Convert2Object_OnlineCheckInfo(byte[] rawData)
        {
            return SCUtility.ToObject<ONLINE_CHECK_INFO>(rawData);
        }

        public static PING_CHECK_INFO Convert2Object_PingCheckInfo(byte[] rawData)
        {
            return SCUtility.ToObject<PING_CHECK_INFO>(rawData);
        }

        public static TRANSFER_INFO Convert2Object_TransferInfo(byte[] rawData)
        {
            return SCUtility.ToObject<TRANSFER_INFO>(rawData);
        }

        public static TIP_MESSAGE_COLLECTION Convert2Object_TipMsgInfoCollection(byte[] rawData)
        {
            return SCUtility.ToObject<TIP_MESSAGE_COLLECTION>(rawData);
        }


        public static SYSTEMPROCESS_INFO Convert2Object_SystemInfo(byte[] rawData)
        {
            return SCUtility.ToObject<SYSTEMPROCESS_INFO>(rawData);
        }

        public static HOSTLOG_INFO Convert2Object_SECSInfo(byte[] rawData)
        {
            return SCUtility.ToObject<HOSTLOG_INFO>(rawData);
        }

        public static EQLOG_INFO Convert2Object_TcpInfo(byte[] rawData)
        {
            return SCUtility.ToObject<EQLOG_INFO>(rawData);
        }
        /// <summary>
        /// Gets the zone by zone identifier.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>Zone.</returns>
        public AZONE getZoneByZoneID(string zone_id)
        {
            AZONE rtnZone = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnZone = zoneDao.getZoneByZoneID(conn, false, zone_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Not Found Record from AZONE [zone_id:{0}]", zone_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnZone;
        }

        /// <summary>
        /// Creates the zone.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>Boolean.</returns>
        public Boolean createZone(AZONE zone)
        {
            return createZone(zone.ZONE_ID, zone.LINE_ID, zone.LOT_ID);
        }

        /// <summary>
        /// Creates the zone.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        /// <param name="line_id">The line_id.</param>
        /// <param name="lot_id">The lot_id.</param>
        /// <returns>Boolean.</returns>
        public Boolean createZone(string zone_id, string line_id, string lot_id)
        {
            Boolean isSuccess = false;

            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();

                AZONE zone = new AZONE
                {
                    ZONE_ID = zone_id,
                    LINE_ID = line_id,
                    LOT_ID = lot_id
                };

                zoneDao.insertZone(conn, zone);

                conn.Commit();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to AZONE [zone_id:{0}]", zone_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return isSuccess;
        }

        /// <summary>
        /// Creates the equipment.
        /// </summary>
        /// <param name="eqpt">The eqpt.</param>
        /// <returns>Boolean.</returns>
        public Boolean createEquipment(AEQPT eqpt)
        {

            try
            {
                using (DBConnection_EF con = new DBConnection_EF())
                {
                    eqptDao.insertEqpt(con, eqpt);
                }


            }
            catch (Exception ex)
            {

                logger.Warn("Insert Failed to AEQPT [eqpt_id:{0}]", eqpt.EQPT_ID, ex);
                return false;
            }
            finally
            {

            }
            return true;
        }




        /// <summary>
        /// Updates the eqpt.
        /// </summary>
        /// <param name="eqpt">The eqpt.</param>
        /// <returns>Boolean.</returns>
        public Boolean updateEQPT(AEQPT eqpt)
        {
            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    eqptDao.updateEqpt(conn, eqpt);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Update Failed to AEQPT [eqpt_id:{0}]", eqpt.EQPT_ID, ex);
                return false;
            }
            finally
            {

            }
            return true;
        }

        /// <summary>
        /// Updates the eqpt mode.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <param name="cim_mode">The cim_mode.</param>
        /// <param name="oper_mode">The oper_mode.</param>
        /// <param name="inline_mode">The inline_mode.</param>
        /// <returns>Boolean.</returns>







        /// <summary>
        /// Updates the buffer.
        /// </summary>
        /// <param name="buff">The buff.</param>
        /// <returns>Boolean.</returns>
        public Boolean updateBuffer(ABUFFER buff)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                bufferPortDao.updateBuffer(conn, buff);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Update Failed to ABUFFER [buffer_id:{0}]", buff.BUFF_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }



        /// <summary>
        /// Loads the zone list by line.
        /// </summary>
        /// <param name="line_id">The line_id.</param>
        /// <returns>List&lt;Zone&gt;.</returns>
        public List<AZONE> loadZoneListByLine(string line_id)
        {
            List<AZONE> rtnList = new List<AZONE>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnList = zoneDao.loadZoneListByLineID(conn, line_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Query Failed from AZONE [line_id:{0}]", line_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnList;
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Boolean.</returns>
        public Boolean createNode(ANODE node)
        {
            return createNode(node.NODE_ID, node.ZONE_ID);
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>Boolean.</returns>
        public Boolean createNode(string node_id, string zone_id)
        {
            Boolean isSuccess = false;

            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();

                ANODE node = new ANODE()
                {
                    NODE_ID = node_id,
                    ZONE_ID = zone_id
                };

                nodeDao.insertNode(conn, node);

                conn.Commit();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to ANODE [node_id:{0}]", node_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return isSuccess;
        }

        /// <summary>
        /// Gets the node by node identifier.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <returns>Node.</returns>
        public ANODE getNodeByNodeID(string node_id)
        {
            ANODE rtnNode = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnNode = nodeDao.getNodeByNodeID(conn, false, node_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from ANode [node_id:{0}]", node_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnNode;
        }

        /// <summary>
        /// Loads the node list by zone.
        /// </summary>
        /// <param name="zone_id">The zone_id.</param>
        /// <returns>List&lt;Node&gt;.</returns>
        public List<ANODE> loadNodeListByZone(string zone_id)
        {
            List<ANODE> rtnList = new List<ANODE>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnList = nodeDao.loadNodeListByZone(conn, zone_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Query Failed from ANode [zone_id:{0}]", zone_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnList;
        }

        /// <summary>
        /// Gets the eqpt by eqpt identifier.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>Equipment.</returns>
        public AEQPT getEqptByEqptID(string eqpt_id)
        {
            AEQPT rtnEqpt = null;

            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    rtnEqpt = eqptDao.getEqptByEqptID(conn, false, eqpt_id);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from AEQPT [eqpt_id:{0}]", eqpt_id, ex);
            }
            finally
            {

            }
            return rtnEqpt;
        }

        /// <summary>
        /// Loads the eqpt list by node.
        /// </summary>
        /// <param name="node_id">The node_id.</param>
        /// <returns>List&lt;Equipment&gt;.</returns>
        public List<AEQPT> loadEqptListByNode(string node_id)
        {
            List<AEQPT> rtnList = new List<AEQPT>();

            try
            {
                using (DBConnection_EF conn = new DBConnection_EF())
                {
                    rtnList = eqptDao.loadEqptListByNode(conn, node_id);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from AEQPT [node_id:{0}]", node_id, ex);
            }
            finally
            {

            }
            return rtnList;
        }

        /// <summary>
        /// Creates the unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>Boolean.</returns>
        public Boolean createUnit(AUNIT unit)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                unitDao.insertUnit(conn, unit);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to AUNIT [unit_id:{0}]", unit.UNIT_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Gets the unit by unit identifier.
        /// </summary>
        /// <param name="unit_id">The unit_id.</param>
        /// <returns>Unit.</returns>
        public AUNIT getUnitByUnitID(string unit_id)
        {
            AUNIT rtnUnit = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnUnit = unitDao.getUnitByUnitID(conn, false, unit_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from AUNIT [unit_id:{0}]", unit_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnUnit;
        }

        /// <summary>
        /// Loads the unit list by eqpt.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Unit&gt;.</returns>
        public List<AUNIT> loadUnitListByEqpt(string eqpt_id)
        {
            List<AUNIT> unitList = new List<AUNIT>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                unitList = unitDao.loadUnitListByEqpt(conn, eqpt_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from AUNIT [eqpt_id:{0}]", eqpt_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return unitList;
        }

        /// <summary>
        /// Updates the unit status.
        /// </summary>
        /// <param name="unit_id">The unit_id.</param>
        /// <param name="unit_stat">The unit_stat.</param>
        /// <returns>Boolean.</returns>
        public Boolean updateUnitStatus(string unit_id, int unit_stat)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                AUNIT unit = unitDao.getUnitByUnitID(conn, true, unit_id);
                unit.UNIT_STAT = unit_stat;
                unitDao.updateUnit(conn, unit);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Update status Failed from AUNIT [unit_id:{0}]", unit_id, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Creates the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>Boolean.</returns>
        public Boolean createPort(APORT port)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                portDao.insertPort(conn, port);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to APORT [port_id:{0}]", port.PORT_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Updates the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>Boolean.</returns>
        public Boolean updatePort(APORT port)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                portDao.updatePort(conn, port);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Update Failed to APORT [port_id:{0}]", port.PORT_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Updates the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>Boolean.</returns>
        public Boolean updatePORT(APORT port)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                APORT sv_port = portDao.getPortByPortID(conn, true, port.PORT_ID);
                sv_port.PORT_STAT = port.PORT_STAT;
                sv_port.PORT_ENABLE = port.PORT_ENABLE;
                sv_port.PORT_TYPE = port.PORT_TYPE;
                sv_port.PORT_REAL_TYPE = port.PORT_REAL_TYPE;
                sv_port.PORT_NUM = port.PORT_NUM;
                sv_port.TRS_MODE = port.TRS_MODE;
                sv_port.PORT_USE_TYPE = port.PORT_USE_TYPE;
                portDao.updatePort(conn, sv_port);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Update Failed to AEQPT [port_id:{0}]", port.PORT_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Gets the port by port identifier.
        /// </summary>
        /// <param name="port_id">The port_id.</param>
        /// <returns>Port.</returns>
        public APORT getPortByPortID(string port_id)
        {
            APORT rtnPort = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnPort = portDao.getPortByPortID(conn, false, port_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from APORT [port_id:{0}]", port_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnPort;
        }

        /// <summary>
        /// Loads the port list by eqpt.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;Port&gt;.</returns>
        public List<APORT> loadPortListByEqpt(string eqpt_id)
        {
            List<APORT> portList = new List<APORT>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                portList = portDao.loadPortListByEqpt(conn, eqpt_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from APORT [eqpt_id:{0}]", eqpt_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return portList;
        }

        /// <summary>
        /// Creates the buffer port.
        /// </summary>
        /// <param name="buff">The buff.</param>
        /// <returns>Boolean.</returns>
        public Boolean createBufferPort(ABUFFER buff)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                bufferPortDao.insertBuffer(conn, buff);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn("Insert Failed to ABUFFER [buff_id:{0}]", buff.BUFF_ID, ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Gets the buffer port by port identifier.
        /// </summary>
        /// <param name="buff_id">The buff_id.</param>
        /// <returns>BufferPort.</returns>
        public ABUFFER getBufferPortByPortID(string buff_id)
        {
            ABUFFER rtnBuff = null;
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnBuff = bufferPortDao.getBufferByBufferID(conn, false, buff_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from ABUFF [buff_id:{0}]", buff_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnBuff;
        }

        /// <summary>
        /// Loads the buff list by eqpt.
        /// </summary>
        /// <param name="eqpt_id">The eqpt_id.</param>
        /// <returns>List&lt;BufferPort&gt;.</returns>
        public List<ABUFFER> loadBuffListByEqpt(string eqpt_id)
        {
            List<ABUFFER> buffList = new List<ABUFFER>();
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                buffList = bufferPortDao.loadBufferListByEqpt(conn, eqpt_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                logger.Warn("Query Failed from ABUFF [eqpt_id:{0}]", eqpt_id, ex);
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return buffList;
        }


        #region ECID
        /// <summary>
        /// Loads the default ec data list.
        /// </summary>
        /// <param name="ecidList">The ecid list.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<ECDataMap> loadDefaultECDataList(List<string> ecidList)
        {
            List<ECDataMap> rtnList = new List<ECDataMap>();
            try
            {
                if (ecidList == null || ecidList.Count == 0)
                {
                    rtnList = ecDataMapDao.loadAllDefaultECData();
                }
                else
                {
                    foreach (string ecid in ecidList)
                    {
                        ECDataMap item = ecDataMapDao.getDefaultByECID(ecid.Trim());
                        if (item == null) { continue; }
                        rtnList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return rtnList;
        }

        /// <summary>
        /// A0.08
        /// </summary>
        /// <param name="ecDataMapList">The ec data map list.</param>
        /// <param name="rtnMsg">The RTN MSG.</param>
        /// <param name="IsInitial">The is initial.</param>
        /// <returns>Boolean.</returns>
        public Boolean updateECData(List<AECDATAMAP> ecDataMapList, out string rtnMsg, Boolean IsInitial)
        {
            return updateECData(ecDataMapList, out rtnMsg);
        }

        /// <summary>
        /// Updates the ec data.
        /// </summary>
        /// <param name="ecDataMapList">The ec data map list.</param>
        /// <param name="rtnMsg">The RTN MSG.</param>
        /// <returns>Boolean.</returns>
        public Boolean updateECData(List<AECDATAMAP> ecDataMapList, out string rtnMsg)
        {
            DBConnection_EF conn = null;
            rtnMsg = string.Empty;
            try
            {
                if (ecDataMapList != null && ecDataMapList.Count > 0
                    && scApp.getEQObjCacheManager().getLine().Host_Control_State != SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
                {
                    foreach (AECDATAMAP item in ecDataMapList)
                    {
                        if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_DEVICE_ID))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_DEVICE_ID);
                            return false;
                        }
                        else if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_T3))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_T3);
                            return false;
                        }
                        else if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_T5))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_T5);
                            return false;
                        }
                        else if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_T6))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_T6);
                            return false;
                        }
                        else if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_T7))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_T7);
                            return false;
                        }
                        else if (BCFUtility.isMatche(item.ECID, SCAppConstants.ECID_T8))
                        {
                            rtnMsg = string.Format("{0}.Can't Modify EC[{1}] in Online Mode.", SECSConst.EAC_Denied_Busy, SCAppConstants.ECID_T8);
                            return false;
                        }
                    }
                }
                //A0.05 End

                //A0.06 Begin
                if (ecDataMapList != null && ecDataMapList.Count > 0)
                {
                    int evc = 0;
                    int ecmin = 0;
                    int ecmax = 0;
                    foreach (AECDATAMAP item in ecDataMapList)
                    {
                        if (int.TryParse(item.ECV, out evc))
                        {
                            ecmin = Convert.ToInt16(item.ECMIN);
                            ecmax = Convert.ToInt16(item.ECMAX);
                            if (evc < ecmin || evc > ecmax)
                            {
                                rtnMsg = string.Format("{0}.Can't Modify EC[{1}] ,Over range.", SECSConst.EAC_Denied_At_least_one_constant_out_of_range, item.ECNAME);
                                return false;
                            }
                        }
                    }
                }
                //A0.06 End

                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                if (ecDataMapList != null && ecDataMapList.Count > 0)
                {
                    foreach (AECDATAMAP item in ecDataMapList)
                    {
                        ecDataMapDao.updateECData(conn, item);
                        scApp.BCSystemBLL.updateSystemParameter(item.ECID, item.ECV, true);
                    }
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
                rtnMsg = string.Format("updateECData Exception ![{0}]", ex);
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Inserts the new ec data.
        /// </summary>
        /// <param name="ecDataMapList">The ec data map list.</param>
        /// <returns>Boolean.</returns>
        public Boolean insertNewECData(List<AECDATAMAP> ecDataMapList)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                if (ecDataMapList != null && ecDataMapList.Count > 0)
                {
                    foreach (AECDATAMAP item in ecDataMapList)
                    {
                        AECDATAMAP sv_item = ecDataMapDao.getByECID(conn, false, item.ECID);
                        if (sv_item == null)
                        {
                            ecDataMapDao.insertECData(conn, item);
                            scApp.BCSystemBLL.updateSystemParameter(item.ECID, item.ECV, false);
                        }
                    }
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Deletes the ec data list.
        /// </summary>
        /// <param name="ecidList">The ecid list.</param>
        /// <returns>Boolean.</returns>
        public Boolean deleteECDataList(List<string> ecidList)
        {
            DBConnection_EF conn = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                ecDataMapDao.deleteECData(conn, ecidList);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
                return false;
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return true;
        }

        /// <summary>
        /// Loads the ec data list.
        /// </summary>
        /// <param name="ecidList">The ecid list.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<AECDATAMAP> loadECDataList(List<string> ecidList)
        {
            DBConnection_EF conn = null;
            List<AECDATAMAP> rtnList = new List<AECDATAMAP>();
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                if (ecidList == null || ecidList.Count == 0)
                {
                    rtnList = ecDataMapDao.loadAllECData(conn);
                }
                else
                {
                    foreach (string ecid in ecidList)
                    {
                        AECDATAMAP item = ecDataMapDao.getByECID(conn, false, ecid.Trim());
                        if (item == null) { continue; }
                        rtnList.Add(item);
                    }
                }
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnList;
        }

        //A0.01
        /// <summary>
        /// Loads the ec data list.
        /// </summary>
        /// <param name="eqpt_real_id">The eqpt_real_id.</param>
        /// <returns>List&lt;ECDataMap&gt;.</returns>
        public List<AECDATAMAP> loadECDataList(string eqpt_real_id)
        {
            DBConnection_EF conn = null;
            List<AECDATAMAP> rtnList = new List<AECDATAMAP>();
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                rtnList = ecDataMapDao.loadECDataMapsByEQRealID(conn, eqpt_real_id);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return rtnList;
        }

        /// <summary>
        /// Gets the ec data.
        /// </summary>
        /// <param name="ecid">The ecid.</param>
        /// <returns>ECDataMap.</returns>
        public AECDATAMAP getECData(string ecid)
        {
            DBConnection_EF conn = null;
            AECDATAMAP ecData = null;
            try
            {
                conn = DBConnection_EF.GetContext();
                conn.BeginTransaction();
                ecData = ecDataMapDao.getByECID(conn, false, ecid);
                conn.Commit();
            }
            catch (Exception ex)
            {
                if (conn != null) { try { conn.Rollback(); } catch (Exception ex_rollback) { logger.Error(ex_rollback, "Exception"); } }
                logger.Warn(ex, "Exception:");
            }
            finally
            {
                if (conn != null) { try { conn.Close(); } catch (Exception ex_close) { logger.Error(ex_close, "Exception:"); } }
            }
            return ecData;
        }

        #endregion ECID


        #region Communcation Info
        public List<APSetting> loadAPSettiong()
        {
            List<APSetting> lstAPSettiong = null;
            try
            {
                lstAPSettiong = scApp.APSettingDao.loadAPSettingByAPName(scApp);
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Exception:");
            }
            finally
            {
            }
            return lstAPSettiong;
        }
        #endregion Communcation Info
        const string SEGMENT_PRE_DISABLE_EXCUTEING = "SEGMENT_PRE_DISABLE_EXCUTEING";

        public bool isSegmentPreDisableExcuting()
        {
            return scApp.getRedisCacheManager().KeyExists(SEGMENT_PRE_DISABLE_EXCUTEING);
        }

        public void BegingOrEndSegmentPreDisableExcute(bool is_excute)
        {
            if (is_excute)
            {
                scApp.getRedisCacheManager().stringSetAsync(SEGMENT_PRE_DISABLE_EXCUTEING, SCApplication.ServerName);
            }
            else
            {
                scApp.getRedisCacheManager().KeyDelete(SEGMENT_PRE_DISABLE_EXCUTEING);
            }
            scApp.getEQObjCacheManager().getLine().SegmentPreDisableExcuting = is_excute;
        }

    }
}