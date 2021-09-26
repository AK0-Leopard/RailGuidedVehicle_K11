using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ProtocolFormat.Converter.ASE_K11
{
    public static class ExtensionRgvmessage
    {
        #region Receive
        #region ID_131
        public static OHTMessage.ID_131_TRANS_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_131_TRANS_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_131_TRANS_RESPONSE()
            {
                CmdID = msg.CmdID,
                CommandAction = convertToVhRGVContent(msg.ActType),
                NgReason = msg.NgReason,
                ReplyCode = msg.ReplyCode
            };

            return local_msg;
        }
        private static OHTMessage.CommandActionType convertToVhRGVContent(AKA.ProtocolFormat.RGVMessage.ActiveType content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Home:
                    return OHTMessage.CommandActionType.Home;
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Load:
                    return OHTMessage.CommandActionType.Load;
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Loadunload:
                    return OHTMessage.CommandActionType.Loadunload;
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Move:
                    return OHTMessage.CommandActionType.Move;
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Unload:
                    return OHTMessage.CommandActionType.Unload;
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Techingmove:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Systemout:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Systemin:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Scan:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Round:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Override:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Mtlhome:
                case AKA.ProtocolFormat.RGVMessage.ActiveType.Movetomtl:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_131
        #region ID_132
        public static OHTMessage.ID_132_TRANS_COMPLETE_REPORT ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_132_TRANS_COMPLETE_REPORT msg)
        {
            var local_msg = new OHTMessage.ID_132_TRANS_COMPLETE_REPORT()
            {
                CurrentAdrID = msg.CurrentAdrID,
                CmdDistance = msg.CmdDistance,
                DirectionAngle = 0,
                VehicleAngle = 0,
                XAxis = 0,
                YAxis = 0,
                CmdID = msg.CmdID,
                CmdPowerConsume = 0,
                CmpStatus = convertToVhMsgContent(msg.CmpStatus),
                CSTID = msg.BOXID,
                CurrentSecID = msg.CurrentSecID,
                SecDistance = msg.SecDistance
            };

            return local_msg;
        }
        private static OHTMessage.CompleteStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.CompleteStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusAbort:
                    return OHTMessage.CompleteStatus.Abort;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusCancel:
                    return OHTMessage.CompleteStatus.Cancel;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusForceFinishByOp:
                    return OHTMessage.CompleteStatus.ForceAbnormalFinishByOp;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIddoubleStorage:
                    return OHTMessage.CompleteStatus.IddoubleStorage;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIdemptyRetrival:
                    return OHTMessage.CompleteStatus.IdemptyRetrival;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIdmisMatch:
                    return OHTMessage.CompleteStatus.IdmisMatch;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIdreadDuplicate:
                    return OHTMessage.CompleteStatus.IdreadDuplicate;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIdreadFailed:
                    return OHTMessage.CompleteStatus.IdreadFailed;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusInterlockError:
                    return OHTMessage.CompleteStatus.InterlockError;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusLoad:
                    return OHTMessage.CompleteStatus.Load;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusLoadunload:
                    return OHTMessage.CompleteStatus.Loadunload;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusLongTimeInaction:
                    return OHTMessage.CompleteStatus.LongTimeInaction;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusMove:
                    return OHTMessage.CompleteStatus.Move;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusUnload:
                    return OHTMessage.CompleteStatus.Unload;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusVehicleAbort:
                    return OHTMessage.CompleteStatus.VehicleAbort;
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusSystemOut:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusTechingMove:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusSystemIn:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusScan:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusOverride:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusMtlhome:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusMoveToMtl:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusIdcsttypeMismatch:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusHome:
                case AKA.ProtocolFormat.RGVMessage.CompleteStatus.CmpStatusCstIdrenmae:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_132
        #region ID_134
        public static OHTMessage.ID_134_TRANS_EVENT_REP ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_134_TRANS_EVENT_REP msg)
        {
            var local_msg = new OHTMessage.ID_134_TRANS_EVENT_REP()
            {
                CurrentAdrID = msg.CurrentAdrID,
                CurrentSecID = msg.CurrentSecID,
                DirectionAngle = 0,
                VehicleAngle = msg.Angle,
                XAxis = msg.XAxis,
                YAxis = msg.YAxis,
                DrivingDirection = OHTMessage.DriveDirction.DriveDirNone,
                EventType = convertToVhMsgContent(msg.EventType),
                SecDistance = msg.SecDistance,
                Speed = msg.Speed
            };
            return local_msg;
        }
        #endregion ID_134
        #region ID_136
        public static OHTMessage.ID_136_TRANS_EVENT_REP ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_136_TRANS_EVENT_REP msg)
        {
            var local_msg = new OHTMessage.ID_136_TRANS_EVENT_REP()
            {

                CurrentAdrID = msg.CurrentAdrID,
                ReleaseBlockAdrID = msg.ReleaseBlockAdrID,
                BCRReadResult = convertToVhMsgContent(msg.BCRReadResult),
                CmdID = msg.CmdID,
                CSTID = msg.BOXID,
                CurrentPortID = getCurrentPort(msg),
                CurrentSecID = msg.CurrentSecID,
                EventType = convertToVhMsgContent(msg.EventType),
                Location = convertToVhMsgContent(msg.Location),
                RequestBlockID = msg.RequestBlockID,
                //ReserveInfos = convertToVhMsgContent(msg.ReserveInfos),
                SecDistance = msg.SecDistance,
                IsNeedAvoid = msg.IsNeedAvoid
            };
            local_msg.ReserveInfos.AddRange(convertToVhMsgContent(msg.ReserveInfos));

            return local_msg;
        }
        private static List<OHTMessage.ReserveInfo> convertToVhMsgContent(RepeatedField<AKA.ProtocolFormat.RGVMessage.ReserveInfo> content)
        {
            var list =
                content.Select(c => new OHTMessage.ReserveInfo()
                {
                    DriveDirction = convertToVhMsgContent(c.DriveDirction),
                    ReserveSectionID = c.ReserveSectionID
                }).ToList();
            return list;
        }
        private static OHTMessage.DriveDirction convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.DriveDirction content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirForward:
                    return OHTMessage.DriveDirction.DriveDirForward;
                case AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirNone:
                    return OHTMessage.DriveDirction.DriveDirNone;
                case AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirReverse:
                    return OHTMessage.DriveDirction.DriveDirReverse;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }

        private static OHTMessage.AGVLocation convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.AGVLocation content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.AGVLocation.Left:
                    return OHTMessage.AGVLocation.Left;
                case AKA.ProtocolFormat.RGVMessage.AGVLocation.None:
                    return OHTMessage.AGVLocation.None;
                case AKA.ProtocolFormat.RGVMessage.AGVLocation.Right:
                    return OHTMessage.AGVLocation.Right;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.BCRReadResult convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.BCRReadResult content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.BCRReadResult.BcrMisMatch:
                    return OHTMessage.BCRReadResult.BcrMisMatch;
                case AKA.ProtocolFormat.RGVMessage.BCRReadResult.BcrNormal:
                    return OHTMessage.BCRReadResult.BcrNormal;
                case AKA.ProtocolFormat.RGVMessage.BCRReadResult.BcrReadFail:
                    return OHTMessage.BCRReadResult.BcrReadFail;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.EventType convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.EventType eventType)
        {
            switch (eventType)
            {
                case AKA.ProtocolFormat.RGVMessage.EventType.AdrOrMoveArrivals:
                    return OHTMessage.EventType.AdrOrMoveArrivals;
                case AKA.ProtocolFormat.RGVMessage.EventType.AdrPass:
                    return OHTMessage.EventType.AdrPass;
                case AKA.ProtocolFormat.RGVMessage.EventType.Bcrread:
                    return OHTMessage.EventType.Bcrread;
                case AKA.ProtocolFormat.RGVMessage.EventType.BlockRelease:
                    return OHTMessage.EventType.BlockRelease;
                case AKA.ProtocolFormat.RGVMessage.EventType.BlockReq:
                    return OHTMessage.EventType.BlockReq;
                case AKA.ProtocolFormat.RGVMessage.EventType.CsttypeMismatch:
                    return OHTMessage.EventType.CsttypeMismatch;
                case AKA.ProtocolFormat.RGVMessage.EventType.DoubleStorage:
                    return OHTMessage.EventType.DoubleStorage;
                case AKA.ProtocolFormat.RGVMessage.EventType.EmptyRetrieval:
                    return OHTMessage.EventType.EmptyRetrieval;
                case AKA.ProtocolFormat.RGVMessage.EventType.Initial:
                    return OHTMessage.EventType.Initial;
                case AKA.ProtocolFormat.RGVMessage.EventType.LoadArrivals:
                    return OHTMessage.EventType.LoadArrivals;
                case AKA.ProtocolFormat.RGVMessage.EventType.LoadComplete:
                    return OHTMessage.EventType.LoadComplete;
                case AKA.ProtocolFormat.RGVMessage.EventType.ReserveReq:
                    return OHTMessage.EventType.ReserveReq;
                case AKA.ProtocolFormat.RGVMessage.EventType.UnloadArrivals:
                    return OHTMessage.EventType.UnloadArrivals;
                case AKA.ProtocolFormat.RGVMessage.EventType.UnloadComplete:
                    return OHTMessage.EventType.UnloadComplete;
                case AKA.ProtocolFormat.RGVMessage.EventType.Vhloading:
                    return OHTMessage.EventType.Vhloading;
                case AKA.ProtocolFormat.RGVMessage.EventType.Vhunloading:
                    return OHTMessage.EventType.Vhunloading;
                case AKA.ProtocolFormat.RGVMessage.EventType.AvoideReq:
                    return OHTMessage.EventType.AvoidReq;
                case AKA.ProtocolFormat.RGVMessage.EventType.Scan:
                case AKA.ProtocolFormat.RGVMessage.EventType.MoveRestart:
                case AKA.ProtocolFormat.RGVMessage.EventType.MovePause:
                case AKA.ProtocolFormat.RGVMessage.EventType.Hidreq:
                case AKA.ProtocolFormat.RGVMessage.EventType.Hidrelease:
                case AKA.ProtocolFormat.RGVMessage.EventType.BlockHidrelease:
                case AKA.ProtocolFormat.RGVMessage.EventType.BlockHidreq:
                default:
                    throw new ArgumentException($"{eventType}not using");
            }
        }

        private static string getCurrentPort(AKA.ProtocolFormat.RGVMessage.ID_136_TRANS_EVENT_REP msg)
        {
            switch (msg.EventType)
            {
                case AKA.ProtocolFormat.RGVMessage.EventType.LoadArrivals:
                case AKA.ProtocolFormat.RGVMessage.EventType.Vhloading:
                case AKA.ProtocolFormat.RGVMessage.EventType.LoadComplete:
                case AKA.ProtocolFormat.RGVMessage.EventType.Bcrread:
                    return msg.LoadPortID;
                case AKA.ProtocolFormat.RGVMessage.EventType.UnloadArrivals:
                case AKA.ProtocolFormat.RGVMessage.EventType.Vhunloading:
                case AKA.ProtocolFormat.RGVMessage.EventType.UnloadComplete:
                    return msg.UnloadPortID;
                default:
                    return "";
            }
        }
        #endregion ID_136
        #region ID_137
        public static OHTMessage.ID_137_TRANS_CANCEL_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_137_TRANS_CANCEL_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_137_TRANS_CANCEL_RESPONSE()
            {
                CmdID = msg.CmdID,
                CancelAction = convertToVhRGVContent(msg.ActType),
                ReplyCode = msg.ReplyCode
            };
            return local_msg;
        }
        private static OHTMessage.CancelActionType convertToVhRGVContent(AKA.ProtocolFormat.RGVMessage.CMDCancelType content)
        {
            switch (content)
            {

                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdNone:
                    return OHTMessage.CancelActionType.CmdNone;
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdAbort:
                    return OHTMessage.CancelActionType.CmdAbort;
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancel:
                    return OHTMessage.CancelActionType.CmdCancel;
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdMismatch:
                    return OHTMessage.CancelActionType.CmdCancelIdMismatch;
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdReadFailed:
                    return OHTMessage.CancelActionType.CmdCancelIdReadFailed;
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdRetry:
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdReadDuplicate:
                case AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdReadForceFinish:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }

        #endregion ID_137
        #region ID_138
        public static OHTMessage.ID_138_GUIDE_INFO_REQUEST ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_138_GUIDE_INFO_REQUEST msg)
        {
            var local_msg = new OHTMessage.ID_138_GUIDE_INFO_REQUEST();
            local_msg.FromToAdrList.AddRange(convertToVhMsgContent(msg.FromToAdrList));
            return local_msg;
        }
        private static List<OHTMessage.FromToAdr> convertToVhMsgContent(RepeatedField<AKA.ProtocolFormat.RGVMessage.FromToAdr> content)
        {
            var list =
                content.Select(c => new OHTMessage.FromToAdr()
                {
                    From = c.From,
                    To = c.To
                }).ToList();
            return list;
        }

        #endregion ID_138
        #region ID_139
        public static OHTMessage.ID_139_PAUSE_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_139_PAUSE_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_139_PAUSE_RESPONSE()
            {
                EventType = convertToVhRGVContent(msg.EventType),
                ReplyCode = msg.ReplyCode,

            };
            return local_msg;
        }
        private static OHTMessage.PauseEvent convertToVhRGVContent(AKA.ProtocolFormat.RGVMessage.PauseEvent content)
        {
            switch (content)
            {

                case AKA.ProtocolFormat.RGVMessage.PauseEvent.Continue:
                    return OHTMessage.PauseEvent.Continue;
                case AKA.ProtocolFormat.RGVMessage.PauseEvent.Pause:
                    return OHTMessage.PauseEvent.Pause;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }

        #endregion ID_139
        #region ID_141
        public static OHTMessage.ID_141_MODE_CHANGE_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_141_MODE_CHANGE_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_141_MODE_CHANGE_RESPONSE()
            {
                ReplyCode = msg.ReplyCode
            };
            return local_msg;
        }
        #endregion ID_141
        #region ID_143
        public static OHTMessage.ID_143_STATUS_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_143_STATUS_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_143_STATUS_RESPONSE()
            {
                ActionStatus = convertToVhMsgContent(msg.ActionStatus),
                BatteryCapacity = 0,
                BatteryTemperature = 0,
                BlockingStatus = convertToVhMsgContent(msg.BlockingStatus),
                ChargeStatus = OHTMessage.VhChargeStatus.ChargeStatusNone,
                CmdId1 = msg.CmdId1,
                CmdId2 = msg.CmdId2,
                CmdId3 = msg.CmdId3,
                CmdId4 = msg.CmdId4,
                CmsState1 = convertToVhMsgContent(msg.CmdState1),
                CmsState2 = convertToVhMsgContent(msg.CmdState2),
                CmsState3 = convertToVhMsgContent(msg.CmdState3),
                CmsState4 = convertToVhMsgContent(msg.CmdState4),
                CstIdL = msg.BoxIdL,
                CstIdR = msg.BoxIdR,
                CurrentAdrID = msg.CurrentAdrID,
                CurrentExcuteCmdId = msg.CurrentExcuteCmdId,
                CurrentSecID = msg.CurrentSecID,
                DirectionAngle = 0,
                DrivingDirection = OHTMessage.DriveDirction.DriveDirNone,
                EarthquakePauseTatus = convertToVhMsgContent(msg.EarthquakePauseTatus),
                ErrorStatus = convertToVhMsgContent(msg.ErrorStatus),
                HasCstL = convertToVhMsgContent(msg.HasBoxL),
                HasCstR = convertToVhMsgContent(msg.HasBoxR),
                ModeStatus = convertToVhMsgContent(msg.ModeStatus),
                ObstacleStatus = convertToVhMsgContent(msg.ObstacleStatus),
                ObstDistance = msg.ObstDistance,
                ObstVehicleID = msg.ObstVehicleID,
                OpPauseStatus = OHTMessage.VhStopSingle.Off,
                PauseStatus = convertToVhMsgContent(msg.PauseStatus),
                PowerStatus = convertToVhMsgContent(msg.PowerStatus),
                //ReserveInfos
                ReserveStatus = convertToVhMsgContent(msg.ReserveStatus),
                SafetyPauseStatus = convertToVhMsgContent(msg.SafetyPauseStatus),
                SecDistance = msg.SecDistance,
                ShelfStatusL = convertToVhMsgContent(msg.ShelfStatusL),
                ShelfStatusR = convertToVhMsgContent(msg.ShelfStatusR),
                Speed = msg.Speed,
                SteeringWheel = 0,
                StoppedBlockID = "",
                SystemTime = "",
                VehicleAngle = msg.Angle,
                //WillPassGuideSection 
                XAxis = msg.XAxis,
                YAxis = msg.YAxis,
            };
            return local_msg;
        }
        #endregion ID_143
        #region ID_144
        public static OHTMessage.ID_144_STATUS_CHANGE_REP ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_144_STATUS_CHANGE_REP msg)
        {
            var local_msg = new OHTMessage.ID_144_STATUS_CHANGE_REP()
            {
                ActionStatus = convertToVhMsgContent(msg.ActionStatus),
                BatteryCapacity = 0,
                BatteryTemperature = 0,
                BlockingStatus = convertToVhMsgContent(msg.BlockingStatus),
                ChargeStatus = OHTMessage.VhChargeStatus.ChargeStatusNone,
                CmdId1 = msg.CmdId1,
                CmdId2 = msg.CmdId2,
                CmdId3 = msg.CmdId3,
                CmdId4 = msg.CmdId4,
                CmsState1 = convertToVhMsgContent(msg.CmdState1),
                CmsState2 = convertToVhMsgContent(msg.CmdState2),
                CmsState3 = convertToVhMsgContent(msg.CmdState3),
                CmsState4 = convertToVhMsgContent(msg.CmdState4),
                CstIdL = msg.BoxIdL,
                CstIdR = msg.BoxIdR,
                CurrentAdrID = msg.CurrentAdrID,
                CurrentExcuteCmdId = msg.CurrentExcuteCmdId,
                CurrentSecID = msg.CurrentSecID,
                DirectionAngle = 0,
                DrivingDirection = OHTMessage.DriveDirction.DriveDirNone,
                EarthquakePauseTatus = convertToVhMsgContent(msg.EarthquakePauseTatus),
                ErrorStatus = convertToVhMsgContent(msg.ErrorStatus),
                HasCstL = convertToVhMsgContent(msg.HasBoxL),
                HasCstR = convertToVhMsgContent(msg.HasBoxR),
                ModeStatus = convertToVhMsgContent(msg.ModeStatus),
                ObstacleStatus = convertToVhMsgContent(msg.ObstacleStatus),
                ObstDistance = msg.ObstDistance,
                ObstVehicleID = msg.ObstVehicleID,
                OpPauseStatus = OHTMessage.VhStopSingle.Off,
                PauseStatus = convertToVhMsgContent(msg.PauseStatus),
                PowerStatus = convertToVhMsgContent(msg.PowerStatus),
                //ReserveInfos
                ReserveStatus = convertToVhMsgContent(msg.ReserveStatus),
                SafetyPauseStatus = convertToVhMsgContent(msg.SafetyPauseStatus),
                SecDistance = msg.SecDistance,
                ShelfStatusL = convertToVhMsgContent(msg.ShelfStatusL),
                ShelfStatusR = convertToVhMsgContent(msg.ShelfStatusR),
                Speed = msg.Speed,
                SteeringWheel = 0,
                StoppedBlockID = "",
                SystemTime = "",
                VehicleAngle = msg.Angle,
                //WillPassGuideSection 
                XAxis = msg.XAxis,
                YAxis = msg.YAxis,
            };
            local_msg.ReserveInfos.AddRange(convertToVhMsgContent(msg.ReserveInfos));

            return local_msg;
        }
        private static OHTMessage.ShelfStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.ShelfStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.ShelfStatus.Disable:
                    return OHTMessage.ShelfStatus.Disable;
                case AKA.ProtocolFormat.RGVMessage.ShelfStatus.Enable:
                    return OHTMessage.ShelfStatus.Enable;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.VhPowerStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.VhPowerStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.VhPowerStatus.PowerOff:
                    return OHTMessage.VhPowerStatus.PowerOff;
                case AKA.ProtocolFormat.RGVMessage.VhPowerStatus.PowerOn:
                    return OHTMessage.VhPowerStatus.PowerOn;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.VHModeStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.VHModeStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.None:
                    return OHTMessage.VHModeStatus.None;
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.AutoLocal:
                    return OHTMessage.VHModeStatus.AutoLocal;
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.AutoRemote:
                    return OHTMessage.VHModeStatus.AutoRemote;
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.Manual:
                    return OHTMessage.VHModeStatus.Manual;
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.AutoMtl:
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.AutoMts:
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.InitialPowerOff:
                case AKA.ProtocolFormat.RGVMessage.VHModeStatus.InitialPowerOn:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.VhLoadCSTStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.VhLoadCSTStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.VhLoadCSTStatus.Exist:
                    return OHTMessage.VhLoadCSTStatus.Exist;
                case AKA.ProtocolFormat.RGVMessage.VhLoadCSTStatus.NotExist:
                    return OHTMessage.VhLoadCSTStatus.NotExist;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.CommandState convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.CommandState content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.CommandState.LoadEnroute:
                    return OHTMessage.CommandState.LoadEnroute;
                case AKA.ProtocolFormat.RGVMessage.CommandState.None:
                    return OHTMessage.CommandState.None;
                case AKA.ProtocolFormat.RGVMessage.CommandState.UnloadEnroute:
                    return OHTMessage.CommandState.UnloadEnroute;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.VhStopSingle convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.VhStopSingle content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.VhStopSingle.StopSingleOff:
                    return OHTMessage.VhStopSingle.Off;
                case AKA.ProtocolFormat.RGVMessage.VhStopSingle.StopSingleOn:
                    return OHTMessage.VhStopSingle.On;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static OHTMessage.VHActionStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.VHActionStatus content)
        {
            switch (content)
            {
                case AKA.ProtocolFormat.RGVMessage.VHActionStatus.NoCommand:
                    return OHTMessage.VHActionStatus.NoCommand;
                case AKA.ProtocolFormat.RGVMessage.VHActionStatus.Commanding:
                    return OHTMessage.VHActionStatus.Commanding;
                case AKA.ProtocolFormat.RGVMessage.VHActionStatus.Teaching:
                    return OHTMessage.VHActionStatus.Teaching;
                case AKA.ProtocolFormat.RGVMessage.VHActionStatus.GripperTeaching:
                    return OHTMessage.VHActionStatus.GripperTeaching;
                case AKA.ProtocolFormat.RGVMessage.VHActionStatus.CycleRun:
                    return OHTMessage.VHActionStatus.CycleRun;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }

        #endregion ID_144
        #region ID_151
        public static OHTMessage.ID_151_AVOID_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_151_AVOID_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_151_AVOID_RESPONSE()
            {
                NgReason = msg.NgReason,
                ReplyCode = msg.ReplyCode
            };

            return local_msg;
        }
        #endregion ID_151
        #region ID_152
        public static OHTMessage.ID_152_AVOID_COMPLETE_REPORT ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_152_AVOID_COMPLETE_REPORT msg)
        {
            var local_msg = new OHTMessage.ID_152_AVOID_COMPLETE_REPORT()
            {
                CmpStatus = msg.CmpStatus
            };

            return local_msg;
        }
        #endregion ID_152
        #region ID_191
        public static OHTMessage.ID_191_ALARM_RESET_RESPONSE ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_191_ALARM_RESET_RESPONSE msg)
        {
            var local_msg = new OHTMessage.ID_191_ALARM_RESET_RESPONSE()
            {
                ReplyCode = msg.ReplyCode
            };
            return local_msg;
        }
        private static OHTMessage.ErrorStatus convertToVhMsgContent(AKA.ProtocolFormat.RGVMessage.ErrorStatus eventType)
        {
            switch (eventType)
            {
                case AKA.ProtocolFormat.RGVMessage.ErrorStatus.ErrReset:
                    return OHTMessage.ErrorStatus.ErrReset;
                case AKA.ProtocolFormat.RGVMessage.ErrorStatus.ErrSet:
                    return OHTMessage.ErrorStatus.ErrSet;
                default:
                    throw new ArgumentException($"{eventType}not using");
            }
        }
        #endregion ID_191
        #region ID_194
        public static OHTMessage.ID_194_ALARM_REPORT ConvertToVhMsg(this AKA.ProtocolFormat.RGVMessage.ID_194_ALARM_REPORT msg)
        {
            var local_msg = new OHTMessage.ID_194_ALARM_REPORT()
            {
                ErrCode = msg.ErrCode,
                ErrDescription = msg.ErrDescription,
                ErrStatus = convertToVhMsgContent(msg.ErrStatus)
            };
            return local_msg;
        }
        #endregion ID_194
        #endregion Receive

        #region Send
        #region ID_31
        public static AKA.ProtocolFormat.RGVMessage.ID_31_TRANS_REQUEST ConvertToRGVMsg(this OHTMessage.ID_31_TRANS_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_31_TRANS_REQUEST()
            {
                ActType = convertToVhRGVContent(msg.CommandAction),
                BOXID = msg.CSTID,
                LoadAdr = msg.LoadAdr,
                ToAdr = msg.DestinationAdr,
                CmdID = msg.CmdID,
                LoadPortID = msg.LoadPortID,
                UnloadPortID = msg.UnloadPortID,
                LOTID = msg.LOTID,
            };
            return remote_msg;
        }
        private static AKA.ProtocolFormat.RGVMessage.ActiveType convertToVhRGVContent(OHTMessage.CommandActionType content)
        {
            switch (content)
            {
                case OHTMessage.CommandActionType.Home:
                    return AKA.ProtocolFormat.RGVMessage.ActiveType.Home;
                case OHTMessage.CommandActionType.Load:
                    return AKA.ProtocolFormat.RGVMessage.ActiveType.Load;
                case OHTMessage.CommandActionType.Loadunload:
                    return AKA.ProtocolFormat.RGVMessage.ActiveType.Loadunload;
                case OHTMessage.CommandActionType.Move:
                    return AKA.ProtocolFormat.RGVMessage.ActiveType.Move;
                case OHTMessage.CommandActionType.Unload:
                    return AKA.ProtocolFormat.RGVMessage.ActiveType.Unload;
                case OHTMessage.CommandActionType.Override:
                case OHTMessage.CommandActionType.Movetocharger:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }


        #endregion ID_31
        #region ID_32
        public static AKA.ProtocolFormat.RGVMessage.ID_32_TRANS_COMPLETE_RESPONSE ConvertToRGVMsg(this OHTMessage.ID_32_TRANS_COMPLETE_RESPONSE msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_32_TRANS_COMPLETE_RESPONSE()
            {
                ReplyCode = msg.ReplyCode
            };
            return remote_msg;
        }
        #endregion ID_32
        #region ID_36
        public static AKA.ProtocolFormat.RGVMessage.ID_36_TRANS_EVENT_RESPONSE ConvertToRGVMsg(this OHTMessage.ID_36_TRANS_EVENT_RESPONSE msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_36_TRANS_EVENT_RESPONSE()
            {
                ReplyActiveType = convertToVhRGVContent(msg.ReplyAction),
                IsBlockPass = convertToVhRGVContent(msg.IsBlockPass),
                //IsHIDPass
                IsReserveSuccess = convertToVhRGVContent(msg.IsReserveSuccess),
                RenameBOXID = msg.RenameCarrierID,
                //RenameLOTID
                ReplyCode = msg.ReplyCode
            };
            remote_msg.ReserveInfos.AddRange(convertToVhRGVContent(msg.ReserveInfos));
            return remote_msg;
        }

        private static AKA.ProtocolFormat.RGVMessage.ReserveResult convertToVhRGVContent(OHTMessage.ReserveResult content)
        {
            switch (content)
            {
                case OHTMessage.ReserveResult.Success:
                    return AKA.ProtocolFormat.RGVMessage.ReserveResult.Success;
                case OHTMessage.ReserveResult.Unsuccess:
                    return AKA.ProtocolFormat.RGVMessage.ReserveResult.Unsuccess;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static AKA.ProtocolFormat.RGVMessage.PassType convertToVhRGVContent(OHTMessage.PassType content)
        {
            switch (content)
            {
                case OHTMessage.PassType.Block:
                    return AKA.ProtocolFormat.RGVMessage.PassType.Block;
                case OHTMessage.PassType.Pass:
                    return AKA.ProtocolFormat.RGVMessage.PassType.Pass;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static AKA.ProtocolFormat.RGVMessage.CMDCancelType convertToVhRGVContent(OHTMessage.ReplyActionType content)
        {
            switch (content)
            {
                case OHTMessage.ReplyActionType.Abort:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdAbort;
                case OHTMessage.ReplyActionType.Cancel:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancel;
                case OHTMessage.ReplyActionType.CancelIdMisnatch:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdMismatch;
                case OHTMessage.ReplyActionType.CancelIdReadFailed:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdReadFailed;
                case OHTMessage.ReplyActionType.Continue:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdNone;
                case OHTMessage.ReplyActionType.Retry:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdRetry;
                case OHTMessage.ReplyActionType.Wait:
                case OHTMessage.ReplyActionType.CancelPidFailed:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static List<AKA.ProtocolFormat.RGVMessage.ReserveInfo> convertToVhRGVContent(RepeatedField<OHTMessage.ReserveInfo> content)
        {
            var list =
                content.Select(c => new AKA.ProtocolFormat.RGVMessage.ReserveInfo()
                {
                    DriveDirction = convertToVhRGVContent(c.DriveDirction),
                    ReserveSectionID = c.ReserveSectionID
                }).ToList();
            return list;
        }
        private static AKA.ProtocolFormat.RGVMessage.DriveDirction convertToVhRGVContent(OHTMessage.DriveDirction content)
        {
            switch (content)
            {
                case OHTMessage.DriveDirction.DriveDirForward:
                    return AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirForward;
                case OHTMessage.DriveDirction.DriveDirNone:
                    return AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirNone;
                case OHTMessage.DriveDirction.DriveDirReverse:
                    return AKA.ProtocolFormat.RGVMessage.DriveDirction.DriveDirReverse;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_36
        #region ID_37
        public static AKA.ProtocolFormat.RGVMessage.ID_37_TRANS_CANCEL_REQUEST ConvertToRGVMsg(this OHTMessage.ID_37_TRANS_CANCEL_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_37_TRANS_CANCEL_REQUEST()
            {
                ActType = convertToVhRGVContent(msg.CancelAction),
                CmdID = msg.CmdID
            };
            return remote_msg;
        }
        private static AKA.ProtocolFormat.RGVMessage.CMDCancelType convertToVhRGVContent(OHTMessage.CancelActionType content)
        {
            switch (content)
            {
                case OHTMessage.CancelActionType.CmdAbort:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdAbort;
                case OHTMessage.CancelActionType.CmdCancel:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancel;
                case OHTMessage.CancelActionType.CmdCancelIdMismatch:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdMismatch;
                case OHTMessage.CancelActionType.CmdCancelIdReadFailed:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdCancelIdReadFailed;
                case OHTMessage.CancelActionType.CmdNone:
                    return AKA.ProtocolFormat.RGVMessage.CMDCancelType.CmdNone;
                case OHTMessage.CancelActionType.CmdEms:
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_37
        #region ID_38
        public static AKA.ProtocolFormat.RGVMessage.ID_38_GUIDE_INFO_RESPONSE ConvertToRGVMsg(this OHTMessage.ID_38_GUIDE_INFO_RESPONSE msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_38_GUIDE_INFO_RESPONSE();
            remote_msg.GuideInfoList.Add(convertToVhRGVContent(msg.GuideInfoList));
            return remote_msg;
        }
        private static List<AKA.ProtocolFormat.RGVMessage.GuideInfo> convertToVhRGVContent(RepeatedField<OHTMessage.GuideInfo> content)
        {
            var list = new List<AKA.ProtocolFormat.RGVMessage.GuideInfo>();
            foreach (var c in content)
            {
                var info = new AKA.ProtocolFormat.RGVMessage.GuideInfo();
                info.FromTo = convertToRGVMsgContent(c.FromTo);
                info.GuideAddresses.AddRange(c.GuideAddresses);
                info.GuideSections.AddRange(c.GuideSections);
                list.Add(info);
            }
            return list;
        }
        private static AKA.ProtocolFormat.RGVMessage.FromToAdr convertToRGVMsgContent(OHTMessage.FromToAdr content)
        {
            var from_to_adr =
            new AKA.ProtocolFormat.RGVMessage.FromToAdr()
            {
                From = content.From,
                To = content.To
            };
            return from_to_adr;
        }

        #endregion ID_38
        #region ID_39
        public static AKA.ProtocolFormat.RGVMessage.ID_39_PAUSE_REQUEST ConvertToRGVMsg(this OHTMessage.ID_39_PAUSE_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_39_PAUSE_REQUEST()
            {
                EventType = convertToVhRGVContent(msg.EventType),
                PauseType = convertToVhRGVContent(msg.PauseType),
            };
            return remote_msg;
        }
        private static AKA.ProtocolFormat.RGVMessage.PauseEvent convertToVhRGVContent(OHTMessage.PauseEvent content)
        {
            switch (content)
            {
                case OHTMessage.PauseEvent.Continue:
                    return AKA.ProtocolFormat.RGVMessage.PauseEvent.Continue;
                case OHTMessage.PauseEvent.Pause:
                    return AKA.ProtocolFormat.RGVMessage.PauseEvent.Pause;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        private static AKA.ProtocolFormat.RGVMessage.PauseType convertToVhRGVContent(OHTMessage.PauseType content)
        {
            switch (content)
            {
                case OHTMessage.PauseType.None:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.None;
                case OHTMessage.PauseType.Normal:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.OhxC;
                case OHTMessage.PauseType.Block:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.Block;
                case OHTMessage.PauseType.EarthQuake:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.EarthQuake;
                case OHTMessage.PauseType.Safety:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.Safety;
                case OHTMessage.PauseType.All:
                    return AKA.ProtocolFormat.RGVMessage.PauseType.All;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_39
        #region ID_41
        public static AKA.ProtocolFormat.RGVMessage.ID_41_MODE_CHANGE_REQ ConvertToRGVMsg(this OHTMessage.ID_41_MODE_CHANGE_REQ msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_41_MODE_CHANGE_REQ()
            {
                OperatingVHMode = convertToVhRGVContent(msg.OperatingVHMode)
            };
            return remote_msg;
        }
        private static AKA.ProtocolFormat.RGVMessage.OperatingVHMode convertToVhRGVContent(OHTMessage.OperatingVHMode content)
        {
            switch (content)
            {
                case OHTMessage.OperatingVHMode.OperatingAuto:
                    return AKA.ProtocolFormat.RGVMessage.OperatingVHMode.OperatingAuto;
                case OHTMessage.OperatingVHMode.OperatingManual:
                    return AKA.ProtocolFormat.RGVMessage.OperatingVHMode.OperatingManual;
                default:
                    throw new ArgumentException($"{content}not using");
            }
        }
        #endregion ID_41
        #region ID_43
        public static AKA.ProtocolFormat.RGVMessage.ID_43_STATUS_REQUEST ConvertToRGVMsg(this OHTMessage.ID_43_STATUS_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_43_STATUS_REQUEST()
            {
                SystemTime = msg.SystemTime
            };
            return remote_msg;
        }
        #endregion ID_43
        #region ID_51
        public static AKA.ProtocolFormat.RGVMessage.ID_51_AVOID_REQUEST ConvertToRGVMsg(this OHTMessage.ID_51_AVOID_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_51_AVOID_REQUEST()
            {
                DestinationAdr = msg.DestinationAdr,
            };
            remote_msg.GuideAddresses.AddRange(msg.GuideAddresses);
            remote_msg.GuideSections.AddRange(msg.GuideSections);
            remote_msg.CmdID = msg.CmdID;
            return remote_msg;
        }
        #endregion ID_51
        #region ID_52
        public static AKA.ProtocolFormat.RGVMessage.ID_52_AVOID_COMPLETE_RESPONSE ConvertToRGVMsg(this OHTMessage.ID_52_AVOID_COMPLETE_RESPONSE msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_52_AVOID_COMPLETE_RESPONSE()
            {
                ReplyCode = msg.ReplyCode
            };
            return remote_msg;
        }
        #endregion ID_52
        #region ID_91
        public static AKA.ProtocolFormat.RGVMessage.ID_91_ALARM_RESET_REQUEST ConvertToRGVMsg(this OHTMessage.ID_91_ALARM_RESET_REQUEST msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_91_ALARM_RESET_REQUEST()
            {

            };
            return remote_msg;
        }
        #endregion ID_91
        #region ID_94
        public static AKA.ProtocolFormat.RGVMessage.ID_94_ALARM_RESPONSE ConvertToRGVMsg(this OHTMessage.ID_94_ALARM_RESPONSE msg)
        {
            var remote_msg = new AKA.ProtocolFormat.RGVMessage.ID_94_ALARM_RESPONSE()
            {
                ReplyCode = msg.ReplyCode
            };
            return remote_msg;
        }
        #endregion ID_94
        #endregion Send
    }
}
