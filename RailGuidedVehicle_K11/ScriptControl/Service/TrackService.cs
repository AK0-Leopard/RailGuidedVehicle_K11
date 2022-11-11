using com.mirle.ibg3k0.sc.App;
using System.Linq;
using NLog;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.Service
{
    public class TrackService
    {
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
