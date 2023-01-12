using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using RailChangerProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class Track : AUNIT
    {
        public const int MIN_ALLOW_BLOCK_RELEASE_INTERVAL_ms = 10_000;
        public event EventHandler<alarmCodeChangeArgs> alarmCodeChange;
        public event EventHandler<TrackStatus> trackStatusChange;
        public event EventHandler<bool> AliveStatusChange;
        public class alarmCodeChangeArgs : EventArgs
        {
            public string railChanger_No;
            private List<TrackAlarm> alarmList_old = new List<TrackAlarm>();
            private List<TrackAlarm> alarmList_new = new List<TrackAlarm>();
            private List<TrackAlarm> removeAlarmList = new List<TrackAlarm>();
            private List<TrackAlarm> addAlarmList = new List<TrackAlarm>();

            public List<TrackAlarm> AlarmList_old { get { return alarmList_old; } }
            public List<TrackAlarm> AlarmList_new { get { return alarmList_new; } }
            public List<TrackAlarm> AddAlarmList { get { return addAlarmList; } }
            public List<TrackAlarm> RemoveAlarmList { get { return removeAlarmList; } }

            public alarmCodeChangeArgs(int alarmCode_old, int alarmCode_new)
            {

                //先把兩個alarmcode 給轉成陣列
                char[] alarmString_old = Convert.ToString(alarmCode_old, 2).PadLeft(16, '0').ToCharArray();
                char[] alarmString_new = Convert.ToString(alarmCode_new, 2).PadLeft(16, '0').ToCharArray();

                for (int i = 0; i < 16; i++)
                {
                    if (alarmString_old[i] == '0' && alarmString_new[i] == '0')
                    {
                        //以前沒發生現在也沒發生，就沒事
                    }
                    else if (alarmString_old[i] == '0' && alarmString_new[i] == '1')
                    {
                        //以前沒發生現在發生，代表新增
                        alarmList_new.Add((TrackAlarm)(16 - i));
                        addAlarmList.Add((TrackAlarm)(16 - i));

                    }
                    else if (alarmString_old[i] == '1' && alarmString_new[i] == '0')
                    {
                        //以前有發生，但現在沒有，代表這個alarm已經被解除
                        alarmList_old.Add((TrackAlarm)(16 - i));
                        removeAlarmList.Add((TrackAlarm)(16 - i));
                    }
                    else if (alarmString_old[i] == '1' && alarmString_new[i] == '1')
                    {
                        //以前有發生，現在仍有，代表alarm持續
                        alarmList_old.Add((TrackAlarm)(16 - i));
                        alarmList_new.Add((TrackAlarm)(16 - i));
                    }
                }

            }
            private List<TrackAlarm> decodeAlarmList(int alarmCode)
            {
                List<TrackAlarm> result = new List<TrackAlarm>();
                return result;
            }
        }
        public enum TrackAlarm
        {
            TrackAlarm_Manual_Timeout = 1,
            TrackAlarm_Track_Position_Error = 2,
            TrackAlarm_Track_NoRelease_Error = 3,
            TrackAlarm_No_Define_4 = 4,
            TrackAlarm_No_Define_5 = 5,
            TrackAlarm_No_Define_6 = 6,
            TrackAlarm_No_Define_7 = 7,
            TrackAlarm_No_Define_8 = 8,
            TrackAlarm_No_Define_9 = 9,
            TrackAlarm_No_Define_10 = 10,
            TrackAlarm_No_Define_11 = 11,
            TrackAlarm_No_Define_12 = 12,
            TrackAlarm_No_Define_13 = 13,
            TrackAlarm_No_Define_14 = 14,
            TrackAlarm_No_Define_15 = 15,
            TrackAlarm_No_Define_16 = 16

        }

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public RailChangerProtocol.TrackBlock TrackBlock { get; private set; }
        public bool IsBlocking { get { return TrackBlock == RailChangerProtocol.TrackBlock.Block; } }
        public bool IsAlive { get; private set; }
        //public TrackDir TrackDir { get; private set; }
        public RailChangerProtocol.TrackStatus TrackStatus { get; private set; }
        public int AlarmCode { get; private set; }
        public UInt32 ResetCount { get; private set; }

        public List<string> RelatedSection { get; private set; } = new List<string>();
        public string sRelatedSection { get; private set; } = "";
        public Stopwatch LastUpdataStopwatch { get; private set; } = new Stopwatch();
        public Stopwatch LastBlockReleaseStopwatch { get; private set; } = new Stopwatch();
        public string LastUpdateTime
        {
            get
            {
                return LastUpdataStopwatch.ElapsedMilliseconds.ToString();
            }
        }
        public bool canExcuteBlockRelease
        {
            get
            {
                return !LastBlockReleaseStopwatch.IsRunning ||
                        LastBlockReleaseStopwatch.ElapsedMilliseconds > MIN_ALLOW_BLOCK_RELEASE_INTERVAL_ms;
            }
        }

        public void setTrackInfo(RailChangerProtocol.TrackInfo trackInfo)
        {
            try
            {

                if (hasDifferent(trackInfo))
                {
                    logger.Debug(trackInfo.ToString());
                }

                //alarmCode有變就進行事件處理
                if (AlarmCode != trackInfo.AlarmCode)
                {
                    this.onAlarmCodeChange(AlarmCode, trackInfo.AlarmCode, UNIT_ID);
                    AlarmCode = trackInfo.AlarmCode;
                }
                if (TrackStatus != trackInfo.Status)
                {
                    onTrackStatusChange(trackInfo.Status);
                    TrackStatus = trackInfo.Status;
                }
                if (IsAlive != trackInfo.IsAlive)
                {
                    if (UNIT_NUM == 1)//因為所有換軌器共用一個Alive，只有第一組才進行上報
                        onAliveStatusChange(trackInfo.IsAlive);
                    IsAlive = trackInfo.IsAlive;
                }

                LastUpdataStopwatch.Restart();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private bool hasDifferent(RailChangerProtocol.TrackInfo trackInfo)
        {

            if (TrackStatus != trackInfo.Status) return true;
            else if (AlarmCode != trackInfo.AlarmCode) return true;

            else return false;
        }


        public void ResetBlock(WebAPI.TrackInfoClient trackInfoClient)
        {
            if (trackInfoClient == null) return;
            bool is_success = trackInfoClient.ResetBlockAsync(UNIT_ID).Result;
            if (is_success)
            {
                ResetCount++;
                LastBlockReleaseStopwatch.Restart();
            }
        }

        public Track()
        {

        }

        public void onAlarmCodeChange(int alarmCode_old, int alarmCode_new, string no)
        {
            try
            {
                alarmCodeChangeArgs args = new alarmCodeChangeArgs(alarmCode_old, alarmCode_new);
                args.railChanger_No = no;

                alarmCodeChange?.Invoke(this, args);
            }
            catch (Exception e)
            {
                logger.Error(e, "Exception");
            }
        }
        public void onTrackStatusChange(TrackStatus trackStatus)
        {
            try
            {
                trackStatusChange?.Invoke(this, trackStatus);
            }
            catch (Exception e)
            {
                logger.Error(e, "Exception");
            }
        }
        public void onAliveStatusChange(bool isAlive)
        {
            try
            {
                AliveStatusChange?.Invoke(this, isAlive);
            }
            catch (Exception e)
            {
                logger.Error(e, "Exception");
            }
        }

    }
}
