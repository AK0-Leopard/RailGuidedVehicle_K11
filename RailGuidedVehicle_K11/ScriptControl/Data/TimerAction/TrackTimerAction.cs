using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Linq;
namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    public class TrackTimerAction : ITimerAction
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SCApplication scApp = null;
        protected MPLCSMControl smControl;

        public TrackTimerAction(string name, long intervalMilliSec) : base(name, intervalMilliSec)
        {

        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
        private long syncPoint = 0;
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {
                    scApp.TrackService.RefreshTrackStatus();

                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

    }
}