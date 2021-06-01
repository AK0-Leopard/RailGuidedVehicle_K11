using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.Extension
{
    public static class ExtensionS2F49
    {
        public static ATRANSFER ToATRANSFER(this AK0.RGV.HostMessage.H2E.S2F49_EnhancedRemoteCommand obj, BLL.PortStationBLL portStationBLL)
        {
            string cmdID = obj.CommandId;
            string priority = obj.Priority;
            string replace = obj.Replace;
            string cstID = obj.CarrierId;
            string source = obj.SourcePort;
            string dest = obj.DestPort;
            //string lot_id = obj.lo
            if (!int.TryParse(priority, out int ipriority))
            {
                NLog.LogManager.GetCurrentClassLogger().Warn("command id :{0} of priority parse fail. priority valus:{1}"
                            , cmdID
                            , priority);
            }
            if (!int.TryParse(replace, out int ireplace))
            {
                NLog.LogManager.GetCurrentClassLogger().Warn("command id :{0} of priority parse fail. priority valus:{1}"
                            , cmdID
                            , replace);
            }
            int port_priority = 0;
            if (!Common.SCUtility.isEmpty(source))
            {
                APORTSTATION source_portStation = portStationBLL.OperateCatch.getPortStation(source);
                if (source_portStation == null)
                {
                    NLog.LogManager.GetCurrentClassLogger().Warn($"MCS cmd of hostsource port[{source} not exist.]");
                }
                else
                {
                    port_priority = source_portStation.PRIORITY;
                }
            }

            return new ATRANSFER
            {
                CARRIER_ID = cstID,
                ID = cmdID,
                COMMANDSTATE = 0,
                HOSTSOURCE = source,
                HOSTDESTINATION = dest,
                PRIORITY = ipriority,
                PAUSEFLAG = "0",
                CMD_INSER_TIME = DateTime.Now,
                TIME_PRIORITY = 0,
                PORT_PRIORITY = port_priority,
                PRIORITY_SUM = ipriority + port_priority,
                REPLACE = ireplace,
                LOT_ID = ""
            };
        }
    }
}
