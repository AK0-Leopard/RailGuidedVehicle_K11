using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Data.SECSDriver
{
    public abstract class IBSEMDriver : SEMDriver
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        #region Receive
        protected abstract void S2F49_EnhancedRemoteCommandExtension(object sender, SECSEventArgs e);
        protected abstract void S2F41_HostCommand(object sender, SECSEventArgs e);
        #endregion Receive

        #region Send
        #region Transfer Event

        public abstract bool S6F11_TransferAbortCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferAbortFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferAbortInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferCancelCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferCancelFailed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferCancelInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferPaused(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferResumed(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11_TransferInitial(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_Transferring(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleArrived(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleAcquireStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleAcquireCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleAssigned(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleDeparted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleDepositStarted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleDepositCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_CarrierInstalled(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_CarrierInstalled(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_CarrierRemoved(string vhID, string carrierID, string location, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_CarrierRemoved(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleUnassinged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferCompleted(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_TransferCompleted(VTRANSFER vtransfer, CompleteStatus completeStatus, List<AMCSREPORTQUEUE> reportQueues = null);

        public abstract bool S6F11_RunTimeStatus(string vhID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleInstalled(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_VehicleRemoved(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        #endregion Transfer Event
        #region Port Event
        public abstract bool S6F11_PortEventStateChanged(string cmdID, List<AMCSREPORTQUEUE> reportQueues = null);
        #endregion Port Event

        public abstract bool S6F11_UnitAlarmCleared(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null);
        public abstract bool S6F11_UnitAlarmSet(string vhID, string transferID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null);


        #region TSC State Transition Event
        public abstract bool S6F11_TSCAutoCompleted();
        public abstract bool S6F11_TSCAutoInitiated();
        public abstract bool S6F11_TSCPauseCompleted();
        public abstract bool S6F11_TSCPaused();
        public abstract bool S6F11_TSCPauseInitiated();
        #endregion TSC State Transition Event


        #endregion Send

    }
}