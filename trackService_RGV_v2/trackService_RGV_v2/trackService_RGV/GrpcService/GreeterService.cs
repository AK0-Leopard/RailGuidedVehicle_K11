using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trackService_RGV.Library;
using System.Xml;
using System.IO;
using trackService.GrpcService;
using trackService_RGV;
namespace GrpcServiceForm.GrpcService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private trackService_RGV.Library.trackService trackService;
        private string logPath = @"Log\GrpcService\";
        private StreamWriter sw;
        private int currentDay=0;
        private mainForm parent;
        public EventHandler<serviceBeCallArgs> serviceBeCall;
        private static object logLock = new object();
        //private delegate void ServiceBeCallEventHandler(object sender, serviceBeCallArgs args);
        public class serviceBeCallArgs : EventArgs
        {
            public serviceBeCallArgs(string CallMethod, string TrackNo, string CmdInfo, string Result)
            {
                callMethod = CallMethod;
                trackNo = TrackNo;
                cmdInfo = CmdInfo;
                result = Result;
            }
            public string callMethod {  get; }
            public string trackNo { get; }
            public string cmdInfo { get; }
            public string result {  get; }
        }

        public GreeterService(mainForm mF, trackService_RGV.Library.trackService tS)
        {
            // masterPLC = RC;
            parent = mF;
            trackService = tS;
            logPath = tS.LogPath;
            //serviceBeCall += parent.gRPC_BeCall;
        }

        ~GreeterService()
        {
            //serviceBeCall -= parent.gRPC_BeCall;
        }

        //Write
        private void saveLog(string eventType, string railChangeNumber, string cmdInfo, string result)
        {
            DateTime dt = DateTime.Now;
            string savePath = logPath + "\\" + dt.ToString("yyyy-MM-dd") + "\\gRPC_Service.log";
            Task.Run(() =>
            {
                lock (logLock)
                {
                    using (StreamWriter sw = new StreamWriter(savePath, true))
                    {
                        sw.Write("{\"@t\":\"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffff") + "\",");
                        sw.Write("\"eventType\":\"" + eventType + "\",");
                        sw.Write("\"trackNumber\":\"" + railChangeNumber + "\",");
                        sw.Write("\"cmdInfo\":\"" + cmdInfo + "\",");
                        sw.WriteLine("\"result\":\"" + result + "\"}");
                        sw.Close();
                    }
                }
            });
        }

        public override Task<ReplyTracksInfo> RequestTracksInfo(Empty request, ServerCallContext context)
        {
            
            Google.Protobuf.Collections.RepeatedField<TrackInfo> data = new Google.Protobuf.Collections.RepeatedField<TrackInfo>();
            
            List<string> trackNumberList = trackService.getAllTrackNumber();
            foreach(trackService_RGV.Library.trackService.track t in trackService.getAllTrackList)
            {
                //trackService.track t = trackService.getTrack(trackNumber);
                TrackInfo temp = new TrackInfo();

                temp.Index = t.AliveValue;
                switch (t.TrackStatus)
                {
                    case trackService_RGV.Library.trackService.TrackStatus.TrackStatus_Auto:
                        temp.Status = TrackStatus.Auto;
                        break;
                    case trackService_RGV.Library.trackService.TrackStatus.TrackStatus_Manaul:
                        temp.Status = TrackStatus.Manaul;
                        break;
                    case trackService_RGV.Library.trackService.TrackStatus.TrackStatus_Alarm:
                        temp.Status = TrackStatus.Alarm;
                        break;
                    default:
                        temp.Status = TrackStatus.NotDefine;
                        break;
                }
                switch (t.TrackDir)
                {
                    case trackService_RGV.Library.trackService.TrackDir.TrackDir_Straight:
                        temp.Dir = TrackDir.Straight;
                        break;
                    case trackService_RGV.Library.trackService.TrackDir.TrackDir_Curve:
                        temp.Dir = TrackDir.Curve;
                        break;
                    default:
                        break;

                }
                switch (t.TrackBlock)
                {
                    case trackService_RGV.Library.trackService.TrackBlock.TrackBlock_Block:
                        temp.Block = TrackBlock.Block;
                        break;
                    case trackService_RGV.Library.trackService.TrackBlock.TrackBlock_NonBlock:
                        temp.Block = TrackBlock.NonBlock;
                        break;
                    default:
                        temp.Block = TrackBlock.None;
                        break;
                }
                temp.RgvUser = t.RGV_User;
                temp.TrackUser = t.Track_User;
                temp.TrackChangeCount = Convert.ToInt32(t.TrackChangeCounter);
                temp.AlarmCode = Convert.ToInt32(t.AlarmCode);
                temp.Version = t.Version;
                temp.AlarmCode = Convert.ToInt32(t.AlarmCode);
                
                data.Add(temp);
            }
            ReplyTracksInfo info = new ReplyTracksInfo();
            info.TracksInfo.AddRange(data);
            this.serviceBeCall?.Invoke(this, new serviceBeCallArgs("syncTracksInfo", "ALL", "", ""));
            saveLog("Sync", "All", "getAllStatus", trackNumberList.Count.ToString());
            return Task.FromResult(info);
        }
    }
}
