using System;
using System.Collections.Generic;
using System.Text;
using ActProgTypeLib;
using System.Threading;
using System.IO;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.ComponentModel;

namespace trackService_RGV.Library
{
    public class trackService
    {
        #region enum定義
        public enum TrackDir
        {
            TrackDir_None = 0,
            TrackDir_Straight = 1,
            TrackDir_Curve = 2
        }
        public enum TrackStatus
        {
            TrackStatus_NotDefine = 0,
            TrackStatus_Manaul = 1,
            TrackStatus_Auto = 2,
            TrackStatus_Alarm = 3
        }
        public enum TrackBlock
        {
            TrackBlock_None = 0,
            TrackBlock_Block = 1,
            TrackBlock_NonBlock = 2
        }
        public enum TrackAlarm
        {
            TrackAlarm_EMO_Error = 0,
            TrackAlarm_Servo_No_On = 1,
            TrackAlarm_Servo_NotGoHome = 2,
            TrackAlarm_CarOut_Timeout = 3,
            TrackAlarm_ServoOn_Timeout = 4,
            TrackAlarm_ServoOff_Timeout = 5,
            TrackAlarm_GoHome_TimeOut = 6,
            TrackAlarm_Pos1Move_TimeOut = 7,
            TrackAlarm_Pos2Move_TimeOut = 8,
            TrackAlarm_PosLimit_Error = 9,
            TrackAlarm_NegLimit_Error = 10,
            TrackAlarm_Drive_Error = 11,
            TrackAlarm_PosSensorAllOn = 12,
            TrackAlarm_CarInTrackCantAuto = 13,
            TrackAlarm_TrackIsManual = 14,
            TrackAlarm_BlockClosInManual = 15
            //TrackAlarm_IPCAlive_Error = 12,

        }
        #endregion

        #region alarm code 定義
        public enum trackServiceResponse
        {
            normalResponse = 0,
            canNotFindTheTrack = 1,
            nothingWasHappend = 2,
            modeChangeOnlyCanChangeAutoOrManual = 3,
            directionChangeOnlyCurveOrStraight = 4,
            autoChangeDirecionAlreadyStart = 5,
            autoChangeDirecionAlreadyStop = 6,
            directionChangeCanNotHappendWhenAutoChangeDirectionStart = 7,
            trackServiceAlreadyRunWithOffLineMode = 8,
        }
        #endregion

        private class masterPLC
        {
            #region lock object
            object writeLock = new object();
            object readLock = new object();
            #endregion

            #region parm
            private string number; //plc number
            public string Number { get { return number; } }
            private ActProgType plc;
            public ActProgType Plc { get { return plc; } }
            private string ip; //plc ip
            private string port; //plc protocol port
            private string unitType; //plc unit type
            private string protocolType; //pl communcation type
            private string readStartAddress_hex;
            private bool offLineMode;
            private int connectResult = -1;
            private int syncStatusTimeInterVal;
            private int lastSyncDateTimeDays = 0;

            public int ConnectResult { get { return connectResult; } }
            public string ReadStartAddress { get { return readStartAddress_hex; } }
            private string writeStartAddress_hex;
            public string WriteStartAddress { get { return writeStartAddress_hex; } }
            #endregion

            #region background worker and event handler
            private BackgroundWorker bgWorker_getStatus, bgWorker_alive;
            public EventHandler<syncStatusArgs> syncStatus;
            public class syncStatusArgs : EventArgs
            {
                public int[] syncData;
                public syncStatusArgs(int[] data)
                {
                    syncData = data;
                }
            }
            private int bgWorker_getStatus_failCounter = 0;

            private Timer send2plcIsAliveTimer;
            #endregion

            public masterPLC(string Number, string Ip, string Port, string ReadStartAddress,
                string WriteStartAddress, int syncStatusTimeInterVal, string cpuType = "34", string UnitType = "44", string ProtocolType = "5", string ActConnectUnitNumber = "0", string ActNetWork = "0", bool offLineMode = false)
            {
                try
                {
                    number = Number;
                    ip = Ip;
                    port = Port;
                    unitType = UnitType;
                    protocolType = ProtocolType;
                    readStartAddress_hex = ReadStartAddress;
                    writeStartAddress_hex = WriteStartAddress;
                    this.syncStatusTimeInterVal = syncStatusTimeInterVal;
                    plc = new ActProgType();
                    plc.ActHostAddress = ip;
                    plc.ActUnitType = Convert.ToInt32(unitType);
                    plc.ActCpuType = Convert.ToInt32(cpuType);
                    plc.ActProtocolType = Convert.ToInt32(protocolType);
                    plc.ActPortNumber = Convert.ToInt32(port);
                    plc.ActConnectUnitNumber = Convert.ToInt32(ActConnectUnitNumber);
                    plc.ActNetworkNumber = Convert.ToInt32(ActNetWork);
                    if (!offLineMode)
                        connectResult = plc.Open();
                }
                catch (Exception ex)
                {
                    expectionRecorde(ex);
                }

                if (!offLineMode)
                {
                    //未開啟離線模式，所以要打開bgWorker_syncStatus
                    bgWorker_getStatus = new BackgroundWorker();
                    bgWorker_getStatus.DoWork += new DoWorkEventHandler(bgWorker_getStatus_DoWork);
                    bgWorker_getStatus.WorkerSupportsCancellation = true;

                    bgWorker_alive = new BackgroundWorker();
                    bgWorker_alive.DoWork += new DoWorkEventHandler(bgWorker_alive_DoWorkder);
                    bgWorker_alive.WorkerSupportsCancellation = true;

                    bgWorker_alive.RunWorkerAsync();
                    bgWorker_getStatus.RunWorkerAsync();
                }

            }
            ~masterPLC()
            {
                this.bgWorker_alive?.CancelAsync();
                this.bgWorker_getStatus?.CancelAsync();
                this.plc.Close();
            }
            public (bool result, string resultString) writeToPLC(string seq_no, string trackNumber, string address, ushort value, [CallerMemberName] string memberName = "")
            {
                int PLCResult = 0;
                bool resultFlag = true; //預設都是true, 只有在遇到狀況問題轉變為false並結束事件
                string resultString = "";
                //一般寫入PLC的，輸入多少寫入多少
                lock (writeLock) { PLCResult = plc.SetDevice("W" + address, value); }
                resultFlag = PLCResult == 0 ? true : false;
                resultString = PLCResult.ToString();
                saveMasterPLCLog(seq_no, number, "Write", "W" + address, value.ToString(), "0x" + PLCResult.ToString("X8"), trackNumber);
                return (resultFlag, "0x" + PLCResult.ToString("X8"));
            }
            public (bool result, string resultString) readFormPLC(string seq_no, string trackNumber, string address, [CallerMemberName] string memberName = "")
            {
                int PLCResult = 0;
                int readValue = 0;
                bool resultFlag = true; //預設都是true, 只有在遇到狀況問題轉變為false並結束事件
                string resultString = "";
                lock (readLock) { PLCResult = plc.GetDevice("W" + address, out readValue); }
                saveMasterPLCLog(seq_no, number, "Read", "W" + address, readValue.ToString(), "0x" + PLCResult.ToString("X8"), trackNumber);
                resultFlag = PLCResult == 0 ? true : false;
                resultString = PLCResult == 0 ? readValue.ToString() : "0x" + PLCResult.ToString("X8");
                return (resultFlag, resultString);
            }
            public (bool result, string resultString) writeToPLC_byIncrease(string seq_no, string trackNumber, string address, [CallerMemberName] string memberName = "")
            {
                int PLCResult = 0;
                int readValue = 0;
                int increaseValue;
                bool resultFlag = true; //預設都是true, 只有在遇到狀況問題轉變為false並結束事件
                string resultString = "";

                lock (readLock) { PLCResult = plc.GetDevice("W" + address, out readValue); }
                saveMasterPLCLog(seq_no, number, "Incress_Read", "W" + address, readValue.ToString(), "0x" + PLCResult.ToString("X8"), trackNumber);
                if (PLCResult == 0)
                {
                    //讀取成功後嘗試寫入
                    if (readValue > 9999)
                        increaseValue = 0;
                    else
                        increaseValue = readValue + 1;
                    lock (writeLock) { PLCResult = plc.SetDevice("W" + address, increaseValue); }
                    saveMasterPLCLog(seq_no, number, "Incress_Write", "W" + address, increaseValue.ToString(), "0x" + PLCResult.ToString("X8"), trackNumber);
                    if (PLCResult == 0)
                    {
                        //代表寫入也成功了，這樣就都成功了
                        resultFlag = true;
                        resultString = "0x" + PLCResult.ToString("X8");
                    }
                    else
                    {
                        //寫入失敗
                        resultFlag = false;
                        resultString = "0x" + PLCResult.ToString("X8");
                    }
                }
                else
                {
                    //讀取失敗
                    resultFlag = false;
                    resultString = "0x" + PLCResult.ToString("X8");
                }
                return (resultFlag, resultString);
            }
            private void bgWorker_getStatus_DoWork(object sender, EventArgs args)
            {
                while (true)
                {
                    if (bgWorker_getStatus.CancellationPending)
                        break;
                    //取得一大塊PLC資料
                    int[] data = new int[640];
                    int result = plc.ReadDeviceBlock("ZR" + readStartAddress_hex, 81, out data[0]);
                    if (result == 0)
                    {
                        syncStatus?.Invoke(this, new syncStatusArgs(data));
                        if (bgWorker_getStatus_failCounter != 0)
                        {
                            bgWorker_getStatus_failCounter = 0;
                            saveMasterPLCLog("", number, "syncStatus", "", "", "恢復");
                        }
                    }
                    else
                    {
                        bgWorker_getStatus_failCounter++;
                        saveMasterPLCLog("", number, "syncStatus", "", "", "異常次數" + bgWorker_getStatus_failCounter.ToString());
                        if (bgWorker_getStatus_failCounter > 100)
                        {
                            //如果sync 連續100次失敗，則強制關閉bgWorker
                            bgWorker_getStatus.CancelAsync();
                            saveMasterPLCLog("", number, "syncStatus", "", "", "異常次數已達100次，將關閉syncStatus功能");
                        }
                    }
                    Thread.Sleep(syncStatusTimeInterVal);
                }
            }
            private void bgWorker_alive_DoWorkder(object sender, EventArgs args)
            {
                while (true)
                {
                    if (bgWorker_alive.CancellationPending)
                        break;
                    if (lastSyncDateTimeDays == 0 || lastSyncDateTimeDays != DateTime.Now.Day)
                    {
                        DateTime dt = DateTime.Now;
                        ushort yearAndMonth = Convert.ToUInt16((dt.Year % 100) * 256 + (dt.Month));
                        ushort dayAndHH = Convert.ToUInt16((dt.Day) * 256 + (dt.Hour));
                        ushort mmAndSS = Convert.ToUInt16((dt.Minute) * 256 + (dt.Second));
                        plc.SetDevice("W00005", yearAndMonth);
                        plc.SetDevice("W00006", dayAndHH);
                        plc.SetDevice("W00007", mmAndSS);
                        writeToPLC_byIncrease("syncDateTime", "", "00001");
                        lastSyncDateTimeDays = DateTime.Now.Day;
                        cleanLog(); // sync時間的同時順便去洗一下log
                    }
                    writeToPLC_byIncrease("send2plcIsAlive", "", "00000");
                    Thread.Sleep(3000);
                }
            }
        }
        public class track
        {
            #region pulbic parm
            public string TrackNumber { get { return trackNumber; } }
            public TrackBlock TrackBlock { get { return trackBlock; } }
            public TrackStatus TrackStatus { get { return trackStatus; } }
            public TrackDir TrackDir { get { return trackDir; } }
            public string Version { get { return version; } }
            public string AliveInfo { get { return aliveResult ? "Live(" + aliveValue.ToString() + ")" : "Dead(" + aliveValue.ToString() + ")"; } }
            public int AliveValue { get { return aliveValue; } }
            public string TrackChangeCounter { get { return trackChangeCounter.ToString(); } }
            public string AlarmCode { get { return alarmCode.ToString(); } }
            public bool IsAlive { get { return aliveResult; } }
            public string BoxNumber { get { return boxNumber; } }
            public string RGV_User { get { return RGV_user; } }
            public string Track_User { get { return track_user; } }
            #endregion
            string trackNumber;

            string boxNumber;
            string readStartAddress;
            string writeStartAddress;
            int memoryLocation;
            masterPLC masterPLC;

            #region track status
            int aliveValue; //alive 值，PLC將每3秒做一次+1
            bool aliveResult; //是否存活 (依據設定若超過時間則此處為false
            int alarmCode = 0;
            TrackStatus trackStatus;
            TrackBlock trackBlock;

            TrackDir trackDir;

            DateTime lastAliveTime = DateTime.MinValue;
            int trackChangeCounter = 0;
            string version;

            //for RGV
            string RGV_user;
            string track_user;
            #endregion

            private bool createResultFlag = false;
            public bool CreateResultFlag { get { return createResultFlag; } }
            private string createResultString = "";
            public string CreateResultString { get { return createResultString; } }
            private bool offLineMode;
            private bool isFirstSyncStatus = false;

            public track(string TrackNumber, string BoxNumber, string plcNumber, string memoryLocation, bool offLineMode)
            {
                try
                {
                    trackNumber = TrackNumber;
                    boxNumber = BoxNumber;
                    this.memoryLocation = Convert.ToInt32(memoryLocation) * 4;
                    if (masterPLCList.ContainsKey(plcNumber))
                        masterPLC = masterPLCList[plcNumber];
                    //readStartAddress = Dec2Hex(
                    //    (Hex2Dec(masterPLC.ReadStartAddress) + (Convert.ToInt32(memoryLocation) * 16)), 5);
                    readStartAddress = (Convert.ToInt32(masterPLC.ReadStartAddress)) + (memoryLocation);
                    writeStartAddress = Dec2Hex(
                        (Hex2Dec(masterPLC.WriteStartAddress) + (Convert.ToInt32(memoryLocation) * 16)), 5);

                    if (masterPLC != null)
                    {
                        createResultFlag = true;
                        createResultString = "readStartAddress:" + readStartAddress +
                            ", writeStartAddress:" + writeStartAddress;
                        masterPLC.syncStatus += syncTrackStatus;
                    }
                    this.offLineMode = offLineMode;
                    #region offLineMode
                    if (offLineMode)
                    {
                        trackStatus = TrackStatus.TrackStatus_Auto;
                        trackDir = TrackDir.TrackDir_Straight;
                        trackBlock = TrackBlock.TrackBlock_NonBlock;
                        aliveResult = true;
                        RGV_user = "1";
                        track_user = "1";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    expectionRecorde(ex);
                }
            }

            private void syncTrackStatus(object sender, masterPLC.syncStatusArgs args)
            {
                bool hasChange = false; //是否有狀態被更動
                bool autoChangeDirectionSWHasChange = false;
                bool trackVersionHasChange = false;

                //#region word[0] alive
                //if (aliveValue == 0 || lastAliveTime == DateTime.MinValue)
                //{
                //    aliveValue = args.syncData[memoryLocation];
                //    aliveResult = true;
                //    lastAliveTime = DateTime.Now;
                //}
                //else if(aliveValue == args.syncData[memoryLocation] && (DateTime.Now-lastAliveTime).Seconds > 5)
                //{
                //    hasChange=true;
                //    aliveResult = false;
                //}
                //else if (aliveValue != args.syncData[memoryLocation])
                //{
                //    aliveValue = args.syncData[memoryLocation];
                //    aliveResult = true;
                //    lastAliveTime = DateTime.Now;
                //}
                //#endregion
                //#region word[1] status
                //TrackStatus newStatus = TrackStatus.TrackStatus_NotDefine;
                //switch(args.syncData[memoryLocation + 1])
                //{
                //    case 1:
                //        newStatus = TrackStatus.TrackStatus_Manaul;
                //        break;
                //    case 2:
                //        newStatus = TrackStatus.TrackStatus_Auto;
                //        break;
                //    case 3:
                //        newStatus = TrackStatus.TrackStatus_Alarm;
                //        break;
                //    default:
                //        newStatus = TrackStatus.TrackStatus_NotDefine;
                //        break;
                //}
                //if(newStatus != trackStatus)
                //{
                //    hasChange = true;
                //    trackStatus = newStatus;
                //}
                //#endregion
                //#region word[2] dir
                //TrackDir newDir = TrackDir.TrackDir_None;
                //switch(args.syncData[memoryLocation + 2])
                //{
                //    case 1:
                //        newDir = TrackDir.TrackDir_Straight;
                //        break;
                //    case 2:
                //        newDir = TrackDir.TrackDir_Curve;
                //        break;
                //    default:
                //        newDir = TrackDir.TrackDir_None;
                //        break;
                //}
                //if(newDir != trackDir)
                //{
                //    hasChange = true;
                //    trackDir = newDir;
                //}
                //#endregion
                //#region word[3] block
                //switch (args.syncData[memoryLocation + 3])
                //{
                //    case 1:
                //        trackBlock = TrackBlock.TrackBlock_Block;
                //        break;
                //    case 2:
                //        trackBlock = TrackBlock.TrackBlock_NonBlock;
                //        break;
                //    default:
                //        trackBlock = TrackBlock.TrackBlock_None;
                //        break;
                //}
                //#endregion
                //#region word[4] RGV佔用人
                //RGV_user = args.syncData[memoryLocation + 4].ToString();
                //#endregion
                //#region word[5] 軌道目前佔用人
                //track_user = args.syncData[memoryLocation + 5].ToString();
                //#endregion
                //#region word[6.7] trackChangeCounter
                //string counterString = Dec2Hex(args.syncData[memoryLocation + 6], 4) + Dec2Hex(args.syncData[memoryLocation + 7], 4);
                //trackChangeCounter = Hex2Dec(counterString);
                //#endregion
                #region word[13] alarmCode
                if (alarmCode != args.syncData[memoryLocation + 0])
                {
                    hasChange = true;
                    alarmCode = args.syncData[memoryLocation + 0];
                }
                #endregion
                //#region word[14.15] version
                //string newVersion = Dec2Hex(args.syncData[memoryLocation + 14], 4) + "." + Dec2Hex(args.syncData[memoryLocation + 15], 4);
                //if(version != newVersion) trackVersionHasChange = true;
                //version = newVersion;
                //#endregion
                if (isFirstSyncStatus == false || autoChangeDirectionSWHasChange || trackVersionHasChange)
                {
                    saveTrackStatusLog(trackNumber, aliveResult, aliveValue, trackStatus, Dec2Hex(alarmCode, 16), trackBlock, trackDir, false, TrackDir.TrackDir_None, version);
                    isFirstSyncStatus = true;
                }
                else if (hasChange)
                {
                    saveTrackStatusLog(trackNumber, aliveResult, aliveValue, trackStatus, Dec2Hex(alarmCode, 16), trackBlock, trackDir);
                }
            }

            #region getTrackStatus
            #endregion

            #region track Controll
            public (bool resultFlag, string resultString) blockRst(string seq_no)
            {
                bool resultFlag = false;
                string resultString = trackServiceResponse.nothingWasHappend.ToString();

                #region offLineMode
                if (offLineMode)
                {
                    if (trackBlock == TrackBlock.TrackBlock_Block)
                        trackBlock = TrackBlock.TrackBlock_NonBlock;
                    else
                        trackBlock = TrackBlock.TrackBlock_Block;
                    return (true, trackServiceResponse.trackServiceAlreadyRunWithOffLineMode.ToString());
                }
                #endregion

                if (this.trackBlock == TrackBlock.TrackBlock_NonBlock)
                    return (resultFlag, resultString);
                //解block是固定在 writeStartAddress+1的地方做變動
                (resultFlag, resultString) = masterPLC.writeToPLC_byIncrease(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 3), 5));
                return (resultFlag, resultString);
            }
            public (bool resultFlag, string resultString) alarmRst(string seq_no)
            {
                bool resultFlag = false;
                string resultString = trackServiceResponse.nothingWasHappend.ToString();
                #region offLineMode
                if (offLineMode)
                {
                    if (trackStatus != TrackStatus.TrackStatus_Alarm)
                    {
                        trackStatus = TrackStatus.TrackStatus_Alarm;
                        alarmCode = 1;
                    }
                    else
                    {
                        trackStatus = TrackStatus.TrackStatus_Auto;
                        alarmCode = 0;
                    }
                    return (true, trackServiceResponse.trackServiceAlreadyRunWithOffLineMode.ToString());
                }
                #endregion
                if (trackStatus != TrackStatus.TrackStatus_Alarm)
                {
                    return (resultFlag, resultString);
                }
                //解block是固定在 writeStartAddress+1的地方做變動
                (resultFlag, resultString) = masterPLC.writeToPLC_byIncrease(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 3), 5));
                return (resultFlag, resultString);
            }
            public (bool resultFlag, string resultString) modeChange(string seq_no, TrackStatus status)
            {
                bool resultFlag = false;
                string resultString = trackServiceResponse.nothingWasHappend.ToString();
                #region offLineMode
                if (offLineMode)
                {
                    if (trackStatus != status)
                        trackStatus = status;
                    return (true, trackServiceResponse.trackServiceAlreadyRunWithOffLineMode.ToString());
                }
                #endregion
                if (status == this.trackStatus)
                    return (resultFlag, resultString);
                if (status != TrackStatus.TrackStatus_Auto && status != TrackStatus.TrackStatus_Manaul)
                {
                    resultString = trackServiceResponse.modeChangeOnlyCanChangeAutoOrManual.ToString();
                    return (resultFlag, resultString);
                }
                //如果欲切狀態與實際狀態不同，才要做磨是切換
                switch (status)
                {
                    case TrackStatus.TrackStatus_Auto:
                        (resultFlag, resultString) = masterPLC.writeToPLC(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 12), 5), 2);
                        break;
                    case TrackStatus.TrackStatus_Manaul:
                        (resultFlag, resultString) = masterPLC.writeToPLC(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 12), 5), 1);
                        break;
                    default:
                        break;
                }

                if (resultFlag == false)
                    return (resultFlag, resultString);

                (resultFlag, resultString) = masterPLC.writeToPLC_byIncrease(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 4), 5));
                return (resultFlag, resultString);
            }
            public (bool resultFlag, string resultString) directionChange(string seq_no, TrackDir dir)
            {
                bool resultFlag = false;
                string resultString = trackServiceResponse.nothingWasHappend.ToString();
                #region offLineMode
                if (offLineMode)
                {
                    if (trackDir != dir)
                        trackDir = dir;
                    return (true, trackServiceResponse.trackServiceAlreadyRunWithOffLineMode.ToString());
                }
                #endregion
                if (dir == trackDir)
                    return (resultFlag, resultString);
                if (dir != TrackDir.TrackDir_Curve && dir != TrackDir.TrackDir_Straight)
                {
                    resultString = trackServiceResponse.directionChangeOnlyCurveOrStraight.ToString();
                    return (resultFlag, resultString);
                }

                switch (dir)
                {
                    case TrackDir.TrackDir_Curve:
                        (resultFlag, resultString) = masterPLC.writeToPLC(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 9), 5), 2);
                        break;
                    case TrackDir.TrackDir_Straight:
                        (resultFlag, resultString) = masterPLC.writeToPLC(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 9), 5), 1);
                        break;
                    default:
                        break;
                }
                if (resultFlag == false)
                    return (resultFlag, resultString);
                (resultFlag, resultString) = masterPLC.writeToPLC_byIncrease(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 1), 5));
                return (resultFlag, resultString);
            }

            public (bool resultFlag, string resultString) EMOStop(string seq_no)
            {
                bool resultFlag = false;
                string resultString = trackServiceResponse.nothingWasHappend.ToString();
                //解block是固定在 writeStartAddress+1的地方做變動
                (resultFlag, resultString) = masterPLC.writeToPLC_byIncrease(seq_no, trackNumber, Dec2Hex((Hex2Dec(writeStartAddress) + 5), 5));
                return (resultFlag, resultString);
            }
            #endregion
        }
        public class sequenceController
        {
            private int seqNumber = 0;
            private static sequenceController instance;
            private static int maxSeqNumber = 100000;
            sequenceController()
            {
            }
            public static sequenceController getInstance()
            {
                if (instance == null)
                    instance = new sequenceController();
                return instance;
            }
            public static int getSeqNumber()
            {
                if (instance == null)
                    instance = new sequenceController();
                if (instance.seqNumber >= maxSeqNumber)
                    instance.seqNumber = 1;
                else
                    instance.seqNumber = instance.seqNumber + 1;
                return instance.seqNumber;
            }
        }

        #region service parm
        private static Dictionary<string, masterPLC> masterPLCList = new Dictionary<string, masterPLC>();
        private static Dictionary<string, track> trackList = new Dictionary<string, track>();
        public List<track> getAllTrackList
        {
            get
            {
                List<track> result = new List<track>();
                foreach (track t in trackList.Values)
                    result.Add(t);
                return result;
            }
        }
        public string LogPath { get { return logPath; } }
        private bool offLineMode = true; //離線模式
        private bool autoRstBlock = false; //自動解閉塞功能，盡可能不要使用
        private int autoRstBlockTime;
        private int aliveTimeout = 5000;
        private int syncStatusTimeInterVal = 100;
        private static string logPath = "";
        private static object logLock = new object();
        private sequenceController SeqController;
        #endregion
        public trackService(string configFilePath, out string result)
        {
            XmlDocument config = new XmlDocument();
            XmlNode common, plcs, tracks;
            string temp;
            string seq_no = sequenceController.getSeqNumber().ToString();
            result = "";
            #region 檢查設定檔
            if (File.Exists(configFilePath))
            {
                config.Load(configFilePath);
            }
            else
            {
                result = ("找不到設定檔，請檢查資料夾中的trackServiceConfig是否存在，初始化停止");
                return;
            }
            #endregion

            #region 設定common物件
            common = config.SelectSingleNode("TrackService").SelectSingleNode("common");
            if (common == null)
            {
                result = ("設定檔中無common參數，該參數為定義trackService的必備參數，初始化停止");
                return;
            }

            //log Path Setting
            if (common.SelectSingleNode("logPath") == null)
            {
                result = ("common設定檔中無logPath參數設定，該參數為設定log存放的地方");
                return;
            }
            else
            {
                logPath = common.SelectSingleNode("logPath").InnerText;
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);
                saveSystemLog(seq_no, "system initial", "initial common", "checking logPath", true, logPath);
            }
            //offLineMode Setting
            if (common.SelectSingleNode("offLineMode") == null)
            {
                result = ("common設定檔中無offLineMode參數設定，該參數為是否啟動離線模式的設定，初始化停止");
                saveSystemLog(seq_no, "system initial", "initial common", "checking offLineMode", false, result);
                return;
            }
            else
            {
                offLineMode = common.SelectSingleNode("offLineMode").InnerText == "Y" ? true : false;
                saveSystemLog(seq_no, "system initial", "initial common", "checking offLineMode", true, offLineMode ? "open" : "close");
            }
            //aliveTimeOut Setting
            if (common.SelectSingleNode("aliveTimeout") == null)
            {
                result = ("common設定檔中無aliveTimeout參數設定，該參數為設定alive停多久後定義該轉轍器已故障的參數");
                saveSystemLog(seq_no, "system initial", "initial common", "checking aliveTimeout", false, result);
                return;
            }
            else
            {
                try
                {
                    aliveTimeout = Convert.ToInt32(common.SelectSingleNode("aliveTimeout").InnerText);
                    saveSystemLog(seq_no, "system initial", "initial common", "checking aliveTimeout", true, result);
                }
                catch (Exception ex)
                {
                    result = ("設定檔的aliveTimeout設定值為非數字，無法進行設定");
                    expectionRecorde(ex);
                    saveSystemLog(seq_no, "system initial", "initial common", "checking aliveTimeout", false, result);
                    return;
                }
            }
            //autoRstBlockTime Setting
            if (common.SelectSingleNode("autoRstBlockTime") == null)
            {
                result = ("common設定檔中無autoRstBlockTime參數設定，該參數為設定若自動解閉塞功能啟動，則在閉塞on起時多久解閉塞");
                saveSystemLog(seq_no, "system initial", "initial common", "checking autoRstBlockTime", false, result);
                return;
            }
            else
            {
                try
                {
                    autoRstBlockTime = Convert.ToInt32(common.SelectSingleNode("autoRstBlockTime").InnerText);
                    saveSystemLog(seq_no, "system initial", "initial common", "checking autoRstBlockTime", true, result);
                }
                catch (Exception ex)
                {
                    result = ("設定檔的autoRstBlockTime設定值為非數字，無法進行設定");
                    saveSystemLog(seq_no, "system initial", "initial common", "checking autoRstBlockTime", false, result);
                    expectionRecorde(ex);
                    return;
                }
            }
            //syncStatusTimeInterVal
            if (common.SelectSingleNode("syncStatusTimeInterVal") == null)
            {
                result = ("common設定檔中無syncStatusTimeInterVal參數設定，該參數為設定多久從PLC撈一次資料");
                saveSystemLog(seq_no, "system initial", "initial common", "checking syncStatusTimeInterVal", false, result);
                return;
            }
            else
            {
                try
                {
                    syncStatusTimeInterVal = Convert.ToInt32(common.SelectSingleNode("syncStatusTimeInterVal").InnerText);
                    saveSystemLog(seq_no, "system initial", "initial common", "checking syncStatusTimeInterVal", true, syncStatusTimeInterVal.ToString());
                }
                catch (Exception ex)
                {
                    result = ("設定檔的syncStatusTimeInterVal設定值為非數字，無法進行設定");
                    saveSystemLog(seq_no, "system initial", "initial common", "checking syncStatusTimeInterVal", false, result);
                    expectionRecorde(ex);
                    return;
                }
            }
            #endregion

            #region master PLC 註冊
            plcs = config.SelectSingleNode("TrackService").SelectSingleNode("masterPLC");
            saveSystemLog(seq_no, "system initial", "initial masterPLC", "", true, result);
            foreach (XmlNode node in plcs.SelectNodes("PLC"))
            {
                masterPLC plc = new masterPLC(
                    Number: node.Attributes.GetNamedItem("no").Value,
                    Ip: node.Attributes.GetNamedItem("ActHostAddress").Value,
                    Port: node.Attributes.GetNamedItem("ActPortNumber").Value,
                    ReadStartAddress: node.Attributes.GetNamedItem("readStartAddress").Value,
                    WriteStartAddress: node.Attributes.GetNamedItem("writeStartAddress").Value,
                    syncStatusTimeInterVal: syncStatusTimeInterVal,
                    cpuType: node.Attributes.GetNamedItem("ActCpuType").Value,
                    UnitType: node.Attributes.GetNamedItem("ActUnitType").Value,
                    ProtocolType: node.Attributes.GetNamedItem("ActProtocolType").Value,
                    ActConnectUnitNumber: node.Attributes.GetNamedItem("ActConnectUnitNumber").Value,
                    ActNetWork: node.Attributes.GetNamedItem("ActNetWork").Value,
                    offLineMode: offLineMode);
                if (plc.ConnectResult == 0)
                    saveSystemLog(seq_no, "system initial", "initial masterPLC", "masterPLC_Number:" + plc.Number, true, plc.ConnectResult.ToString());
                else
                    saveSystemLog(seq_no, "system initial", "initial masterPLC", "masterPLC_Number:" + plc.Number, false, plc.ConnectResult.ToString());
                masterPLCList.Add(plc.Number, plc);
            }
            #endregion

            #region tracks 註冊
            tracks = config.SelectSingleNode("TrackService").SelectSingleNode("trackList");
            foreach (XmlNode node in tracks.SelectNodes("railChanger"))
            {
                int memoryBlockNumber = Convert.ToInt32(node.Attributes.GetNamedItem("memoryLocation").Value);
                track track = new track(
                    TrackNumber: node.Attributes.GetNamedItem("sn").Value,
                    BoxNumber: node.Attributes.GetNamedItem("boxNumber").Value,
                    plcNumber: node.Attributes.GetNamedItem("usePLC").Value,
                    memoryLocation: node.Attributes.GetNamedItem("memoryLocation").Value,
                    offLineMode: this.offLineMode);
                if (track.CreateResultFlag)
                {
                    saveSystemLog(seq_no, "system initial", "initial track", "", true, track.CreateResultString, track.TrackNumber);
                    trackList.Add(track.TrackNumber, track);
                }
                else
                {
                    saveSystemLog(seq_no, "system initial", "initial track", "track_Number:" + track.TrackNumber, false, track.CreateResultString);
                }
            }
            #endregion
            result = "success";
            saveSystemLog(seq_no, "system initial", "initial finish", "" + "", true, result);

            cleanLog();

        }

        #region track status get
        public string getFirstTrackNumberByBoxNumber(string boxNumber)
        {
            string result = "";
            foreach (track t in trackList.Values)
            {
                if (t.BoxNumber == boxNumber)
                {
                    result = t.TrackNumber;
                }
            }
            return result;
        }
        public List<string> getAllTrackNumber()
        {
            List<string> result = new List<string>();
            foreach (track t in trackList.Values)
            {
                result.Add(t.TrackNumber);
            }
            return result;
        }
        public List<string> getAllTrackBoxNumber()
        {
            List<string> result = new List<string>();
            foreach (track t in trackList.Values)
            {
                if (!result.Exists(x => x == t.BoxNumber))
                    result.Add(t.BoxNumber);
            }
            return result;
        }
        public track getTrack(string trackNumber)
        {
            track track;
            trackList.TryGetValue(trackNumber, out track);
            return track;
        }
        #endregion

        #region status set or change
        public (bool resultFlag, string resultString) blockRst(string trackNumber)
        {
            track track;
            bool resultFlag = false;
            string resultString = "";
            string seq_no = sequenceController.getSeqNumber().ToString();
            saveSystemLog(seq_no, "blockRst", "start", ":", resultFlag, resultString, trackNumber);
            if (trackList.TryGetValue(trackNumber, out track))
            {
                (resultFlag, resultString) = track.blockRst(seq_no);
            }
            else
            {
                resultFlag = false;
                resultString = trackServiceResponse.canNotFindTheTrack.ToString();
            }
            saveSystemLog(seq_no, "blockRst", "finish", "", resultFlag, resultString, trackNumber);
            return (resultFlag, resultString);
        }
        public (bool resultFlag, string resultString) alarmRst(string trackNumber)
        {
            track track;
            bool resultFlag = false;
            string resultString = "";
            string seq_no = sequenceController.getSeqNumber().ToString();
            saveSystemLog(seq_no, "alarmRst", "start", ":", resultFlag, resultString, trackNumber);
            if (trackList.TryGetValue(trackNumber, out track))
            {
                (resultFlag, resultString) = track.alarmRst(seq_no);
            }
            else
            {
                resultFlag = false;
                resultString = trackServiceResponse.canNotFindTheTrack.ToString();
            }
            saveSystemLog(seq_no, "alarmRst", "finish", "", resultFlag, resultString, trackNumber);
            return (resultFlag, resultString);
        }
        public (bool resultFlag, string resultString) modeChange(string trackNumber, TrackStatus status)
        {
            track track;
            bool resultFlag = false;
            string resultString = "";
            string seq_no = sequenceController.getSeqNumber().ToString();
            saveSystemLog(seq_no, "modeChange", "start", status.ToString(), resultFlag, resultString, trackNumber);
            if (trackList.TryGetValue(trackNumber, out track))
            {
                (resultFlag, resultString) = track.modeChange(seq_no, status);
            }
            else
            {
                resultFlag = false;
                resultString = trackServiceResponse.canNotFindTheTrack.ToString();
            }
            saveSystemLog(seq_no, "modeChange", "finish", status.ToString(), resultFlag, resultString, trackNumber);
            return (resultFlag, resultString);
        }
        public (bool resultFlag, string resultString) directionChange(string trackNumber, TrackDir dir)
        {
            track track;
            bool resultFlag = false;
            string resultString = "";
            string seq_no = sequenceController.getSeqNumber().ToString();
            saveSystemLog(seq_no, "directionChange", "start", dir.ToString(), resultFlag, resultString, trackNumber);
            if (trackList.TryGetValue(trackNumber, out track))
            {
                (resultFlag, resultString) = track.directionChange(seq_no, dir);
            }
            else
            {
                resultFlag = false;
                resultString = trackServiceResponse.canNotFindTheTrack.ToString();
            }
            saveSystemLog(seq_no, "directionChange", "finish", dir.ToString(), resultFlag, resultString, trackNumber);
            return (resultFlag, resultString);
        }
        public (bool resultFlag, string resultString) trackEMOStop(string trackNumber)
        {
            track track;
            bool resultFlag = false;
            string resultString = "";
            string seq_no = sequenceController.getSeqNumber().ToString();
            saveSystemLog(seq_no, "trackEMOStop", "start", "", resultFlag, resultString, trackNumber);
            if (trackList.TryGetValue(trackNumber, out track))
            {
                (resultFlag, resultString) = track.EMOStop(seq_no);
            }
            else
            {
                resultFlag = false;
                resultString = trackServiceResponse.canNotFindTheTrack.ToString();
            }
            saveSystemLog(seq_no, "trackEMOStop", "finish", "", resultFlag, resultString, trackNumber);
            return (resultFlag, resultString);
        }

        #endregion

        #region saveLog or deleteLog

        private void saveSystemLog(string seq_no, string callByMethodBlock, string methodAction, string methodDetail, bool methodResultFlag, string methodResultString, string trackNumber = "")
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            string log = "{";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\trackService.log";
            //log maker
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        log += "\"@t\":\"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"seq_no\":\"" + seq_no + "\"," +
                            "\"callByMethodBlock\":\"" + callByMethodBlock + "\"," +
                            "\"methodAction\":\"" + methodAction + "\"," +
                            "\"methodDetail\":\"" + methodDetail + "\"," +
                            "\"methodResultFlag\":\"" + (methodResultFlag ? "success" : "faild") + "\"," +
                            "\"methodResultString\":\"" + methodResultString + "\"," +
                            "\"trackNumber\":\"" + trackNumber + "\"," +
                            "\"logType\":\"" + "trackService_ServiceLog" + "\"}";
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });

        }
        private static void saveMasterPLCLog(string seq_no, string masterPLC_Number, string action,
            string memeoryLocation, string dataValue, string reusltString, string trackNumber = "")
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            string log = "{";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\masterPLC_" + masterPLC_Number + ".log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        log += "\"@t\":\"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"seq_no\":\"" + seq_no + "\"," +
                            "\"action\":\"" + action + "\"," +
                            "\"memeoryLocation\":\"" + memeoryLocation + "\"," +
                            "\"dataValue\":\"" + dataValue + "\"," +
                            "\"reusltString\":\"" + reusltString + "\"," +
                            "\"trackNumber\":\"" + trackNumber + "\"," +
                            "\"logType\":\"" + "trackService_masterPLCLog" + "\"}";
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });
        }
        private static void saveTrackStatusLog(string trackNumber, bool isAlive, int aliveValue, TrackStatus status, string alarmCode, TrackBlock block,
            TrackDir direction)
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            string log = "{";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\track_" + trackNumber + ".log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        log += "\"@t\":\"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"trackNumber\":\"" + trackNumber + "\"," +
                            "\"isAlive\":\"" + (isAlive ? "alive" : "dead") + "\"," +
                            "\"aliveValue\":\"" + Dec2Hex(aliveValue, 4) + "\"," +
                            "\"status\":\"" + status.ToString() + "\"," +
                            "\"alarmCode\":\"" + alarmCode??"" + "\"," +
                            "\"block\":\"" + block.ToString() + "\"," +
                            "\"direction\":\"" + direction.ToString() + "\"," +
                            "\"logType\":\"" + "trackService_trackLog" + "\"}";
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });

        }

        private static void saveTrackStatusLog(string trackNumber, bool isAlive, int aliveValue, TrackStatus status, string alarmCode, TrackBlock block,
            TrackDir direction, bool autoChangeDirectionSW, TrackDir autoChangeDirection, string version)
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            string log = "{";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\track_" + trackNumber + ".log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        log += "\"@t\":\"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"trackNumber\":\"" + trackNumber + "\"," +
                            "\"isAlive\":\"" + (isAlive ? "alive" : "dead") + "\"," +
                            "\"aliveValue\":\"" + Dec2Hex(aliveValue, 4) + "\"," +
                            "\"status\":\"" + status.ToString() + "\"," +
                            "\"alarmCode\":\"" + alarmCode + "\"," +
                            "\"block\":\"" + block.ToString() + "\"," +
                            "\"direction\":\"" + direction.ToString() + "\"," +
                            "\"autoChangeDirectionSW\":\"" + (autoChangeDirectionSW ? "On" : "Off") + "\"," +
                            "\"autoChangeDirection\":\"" + autoChangeDirection.ToString() + "\"," +
                            "\"version\":\"" + version ?? "" + "\"," +
                            "\"logType\":\"" + "trackService_trackLog" + "\"}";
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });
        }

        private static void expectionRecorde(Exception ex)
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\expectionRecorde" + ".log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        string log = ex.ToString();
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });
        }
        private static void blockRstEventRecoder(DateTime blockRstCmdTime,
            DateTime blockClearTime, string trackNumber, string result)
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += "\\blockRstEventRecoder" + ".log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        string log = "";
                        log += "{\"CmdTime\":\"" + blockRstCmdTime.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"blockClearTime\":\"" + blockClearTime.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\"," +
                            "\"trackNumber\":\"" + trackNumber + "\"," +
                            "\"result\":\"" + result + "\"}";
                        sw.WriteLine(log);
                        sw.Close();
                    }
                }
            });
        }
        private static void cleanLog()
        {
            //20220817 新增
            foreach (string s in Directory.GetDirectories(logPath))
            {
                DateTime dt_last = Directory.GetLastWriteTime(s);
                if ((DateTime.Now - dt_last).TotalDays > 60)
                    Directory.Delete(s, true);
            }
        }
        #endregion


        #region common
        public static int Hex2Dec(string hex)
        {
            try { return int.Parse(hex, System.Globalization.NumberStyles.HexNumber); }
            catch (Exception ex) { expectionRecorde(ex); }
            return 0;
        }
        public static string Dec2Hex(int dec, int length)
        {
            return dec.ToString("X" + length.ToString());
        }
        #endregion
    }
}
