using com.mirle.ibg3k0.sc.App;
using System.Linq;
using NLog;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.Service
{
    public class TrackService
    {
        const string TRACK_ALARM_CODE_TRACK_STATUS_IS_ALARM = "101";
        const string TRACK_ALARM_CODE_IS_NOT_ALIVE = "102";

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;

        public TrackService()
        {

        }
        public void Start(SCApplication _app)
        {
            scApp = _app;
            foreach (Track t in scApp.UnitBLL.OperateCatch.GetALLTracks())
            {
                t.alarmCodeChange += trackAlarmHappend;
                t.trackStatusChange += T_trackStatusChange;
                t.AliveStatusChange += T_AliveStatusChange;
            }
        }

        private void T_AliveStatusChange(object sender, bool isAlive)
        {
            Track track = sender as Track;
            if (isAlive)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({track.UNIT_ID}) is alive:{isAlive},report alarm Code:{TRACK_ALARM_CODE_IS_NOT_ALIVE} reset ");
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, TRACK_ALARM_CODE_IS_NOT_ALIVE, ProtocolFormat.OHTMessage.ErrorStatus.ErrReset, "TRACK IS NOT ALIVE");
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({track.UNIT_ID}) is alive:{isAlive},report alarm Code:{TRACK_ALARM_CODE_IS_NOT_ALIVE} set ");
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, TRACK_ALARM_CODE_IS_NOT_ALIVE, ProtocolFormat.OHTMessage.ErrorStatus.ErrSet, "TRACK IS NOT ALIVE");
            }
        }

        private void T_trackStatusChange(object sender, RailChangerProtocol.TrackStatus status)
        {
            Track track = sender as Track;
            if (status == RailChangerProtocol.TrackStatus.Alarm)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({track.UNIT_ID}) current status is :{status},report alarm Code:{TRACK_ALARM_CODE_TRACK_STATUS_IS_ALARM} set");
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, TRACK_ALARM_CODE_TRACK_STATUS_IS_ALARM, ProtocolFormat.OHTMessage.ErrorStatus.ErrSet, "TRACK STATUS IS ALARM");
            }
            else
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({track.UNIT_ID}) current status is {status},report alarm Code:{TRACK_ALARM_CODE_TRACK_STATUS_IS_ALARM} reset ");
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, TRACK_ALARM_CODE_TRACK_STATUS_IS_ALARM, ProtocolFormat.OHTMessage.ErrorStatus.ErrReset, "TRACK STATUS IS ALARM");
            }
        }
        #region 轉轍器alarm發生的對應事件處理
        private void trackAlarmHappend(object sender, Track.alarmCodeChangeArgs e)
        {
            Track track = sender as Track;
            //要再上報Alamr Rerport給MCS
            //scApp.TransferService.OHBC_AlarmSet(scApp.getEQObjCacheManager().getLine().LINE_ID, ((int)AlarmLst.OHT_CommandNotFinishedInTime).ToString());
            foreach (Track.TrackAlarm alarm in e.AddAlarmList)
            {
                string alarmCode = ((int)alarm).ToString();
                string alarmDesc = alarm.ToString();
                //逐一上報
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, alarmCode, ProtocolFormat.OHTMessage.ErrorStatus.ErrSet, alarmDesc);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({e.railChanger_No}) alarm is happend, alarm Code:{alarmCode}, alarm Desc: {alarmDesc} ");
                //Data: $"Find vehicle {vehicleCache.VEHICLE_ID}, vehicle address Id = {vehicleCache.CUR_ADR_ID}, = port address ID {portAddressID}");
            }
            //如果有已經移除的alarm要逐一清掉
            foreach (Track.TrackAlarm alarm in e.RemoveAlarmList)
            {
                string alarmCode = ((int)alarm).ToString();
                string alarmDesc = alarm.ToString();
                //逐一清除
                scApp.LineService.ProcessAlarmReport(track.NODE_ID, track.UNIT_ID, alarmCode, ProtocolFormat.OHTMessage.ErrorStatus.ErrReset, alarmDesc);
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(VehicleService), Device: "AGVC",
                    Data: $"Track({e.railChanger_No}) alarm is cleared, alarm Code:{alarmCode}, alarm Desc: {alarmDesc} ");
            }
        }
        #endregion
        public void RefreshTrackStatus()
        {
            var get_track_info_result = scApp.TrackInfoClient.getTrackInfos();
            if (get_track_info_result.isGetSuccess)
            {
                var all_track = scApp.UnitBLL.OperateCatch.GetALLTracks();
                foreach (var track in all_track)
                {
                    var track_info = get_track_info_result.trackInfos.
                        Where(t => Common.SCUtility.isMatche(t.TrackNumber, track.UNIT_NUM.ToString())).
                        FirstOrDefault();
                    if (track_info == null)
                    {
                        Common.LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(TrackService), Device: "OHx",
                           Data: $"Want to update track:{track.UNIT_ID} but get result not exist.");
                        continue;
                    }
                    track.setTrackInfo(track_info);
                }
            }
        }


    }
}
