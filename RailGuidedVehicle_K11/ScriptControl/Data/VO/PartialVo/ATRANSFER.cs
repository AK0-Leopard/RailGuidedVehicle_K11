﻿using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ATRANSFER
    {
        /// <summary>
        /// 1 2 4 8 16 32 64 128
        /// 1 1 1 1 1  1  1  1
        /// 1 0 0 0 ...
        /// 1 1 0 0 ....
        /// 1 1 1 0 ....
        /// </summary>
        public const int COMMAND_STATUS_BIT_INDEX_ENROUTE = 1;
        public const int COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE = 2;
        public const int COMMAND_STATUS_BIT_INDEX_LOADING = 4;
        public const int COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE = 8;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE = 16;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOADING = 32;
        public const int COMMAND_STATUS_BIT_INDEX_UNLOAD_COMPLETE = 64;
        public const int COMMAND_STATUS_BIT_INDEX_COMMNAD_FINISH = 128;



        public void SetCheckResult(bool isSuccess, string checkCode)
        {
            this.TRANSFERSTATE = isSuccess ? E_TRAN_STATUS.Queue : E_TRAN_STATUS.Reject;
            this.CHECKCODE = checkCode;
        }
        //public ACARRIER GetCarrierInfo()
        public ACARRIER GetCarrierInfo(BLL.VehicleBLL vehicleBLL)
        {
            bool is_vh_location = vehicleBLL.cache.IsVehicleLocationExistByLocationRealID(this.HOSTSOURCE);
            return new ACARRIER()
            {
                ID = this.CARRIER_ID,
                LOT_ID = this.LOT_ID,
                INSER_TIME = this.CMD_INSER_TIME,
                INSTALLED_TIME = this.CMD_INSER_TIME,
                LOCATION = this.HOSTSOURCE,
                //STATE = ProtocolFormat.OHTMessage.E_CARRIER_STATE.WaitIn,
                STATE = is_vh_location ? ProtocolFormat.OHTMessage.E_CARRIER_STATE.Installed :
                                         ProtocolFormat.OHTMessage.E_CARRIER_STATE.WaitIn,
                HOSTSOURCE = this.HOSTSOURCE,
                HOSTDESTINATION = this.HOSTDESTINATION,
                READ_STATUS = ProtocolFormat.OHTMessage.E_ID_READ_STSTUS.Successful
            };
        }
        public ASYSEXCUTEQUALITY GetSysExcuteQuality(BLL.VehicleBLL vehicleBLL)
        {
            int total_act_vh = vehicleBLL.cache.getActVhCount();
            int total_idle_vh = vehicleBLL.cache.getIdleVhCount();
            return new ASYSEXCUTEQUALITY()
            {
                CMD_ID_MCS = this.ID,
                CST_ID = this.CARRIER_ID,
                CMD_INSERT_TIME = this.CMD_INSER_TIME,
                SOURCE_ADR = this.HOSTSOURCE,
                DESTINATION_ADR = this.HOSTDESTINATION,
                TOTAL_ACT_VH_COUNT = total_act_vh,
                TOTAL_IDLE_VH_COUNT = total_idle_vh,
                PARKING_VH_COUNT = 0,
                CYCLERUN_VH_COUNT = 0
            };
        }

        public ACMD ConvertToCmd(BLL.PortStationBLL portStationBLL, BLL.SequenceBLL sequenceBLL, AVEHICLE assignVehicle)
        {
            var source_port_station = portStationBLL.OperateCatch.getPortStation(this.HOSTSOURCE);
            var desc_port_station = portStationBLL.OperateCatch.getPortStation(this.HOSTDESTINATION);
            //如果HostSource port是與車子一樣的話，代表是要去進行Unload的動作
            bool source_is_a_port = portStationBLL.OperateCatch.IsExist(this.HOSTSOURCE);

            string host_source = source_is_a_port ? this.HOSTSOURCE : "";
            E_CMD_TYPE cmd_type = source_is_a_port ? E_CMD_TYPE.LoadUnload : E_CMD_TYPE.Unload; //如果Source不是Port的話，則代表是在車上

            string from_adr = source_port_station == null ? string.Empty : source_port_station.ADR_ID;
            string to_adr = desc_port_station == null ? string.Empty : desc_port_station.ADR_ID;
            return new ACMD()
            {
                ID = sequenceBLL.getCommandID(SCAppConstants.GenOHxCCommandType.Auto),
                TRANSFER_ID = this.ID,
                VH_ID = assignVehicle.VEHICLE_ID,
                CARRIER_ID = this.CARRIER_ID,
                CMD_TYPE = cmd_type,
                SOURCE = from_adr,
                DESTINATION = to_adr,
                PRIORITY = this.PRIORITY_SUM,
                CMD_INSER_TIME = DateTime.Now,
                CMD_STATUS = E_CMD_STATUS.Queue,
                SOURCE_PORT = host_source,
                DESTINATION_PORT = this.HOSTDESTINATION,
                COMPLETE_STATUS = ProtocolFormat.OHTMessage.CompleteStatus.Move
            };
        }

        public HTRANSFER ToHCMD_MCS()
        {
            return new HTRANSFER()
            {
                ID = this.ID,
                LOT_ID = this.LOT_ID,
                CARRIER_ID = this.CARRIER_ID,
                TRANSFERSTATE = this.TRANSFERSTATE,
                COMMANDSTATE = this.COMMANDSTATE,
                HOSTSOURCE = this.HOSTSOURCE,
                HOSTDESTINATION = this.HOSTDESTINATION,
                PRIORITY = this.PRIORITY,
                CHECKCODE = this.CHECKCODE,
                PAUSEFLAG = this.PAUSEFLAG,
                CMD_INSER_TIME = this.CMD_INSER_TIME,
                CMD_START_TIME = this.CMD_START_TIME,
                CMD_FINISH_TIME = this.CMD_FINISH_TIME,
                TIME_PRIORITY = this.TIME_PRIORITY,
                PORT_PRIORITY = this.PORT_PRIORITY,
                PRIORITY_SUM = this.PRIORITY_SUM,
                REPLACE = this.REPLACE
            };
        }
        public AEQPT getSourcePortEQ(BLL.EqptBLL eqptBLL)
        {
            var eq = eqptBLL.OperateCatch.GetEqpt(this.HOSTSOURCE);
            if (eq == null) return null;
            return eq;
        }

        public string getSourcePortGroupID(BLL.PortStationBLL portStationBLL)
        {
            var port_station = portStationBLL.OperateCatch.getPortStation(ID);
            if (port_station == null) return "";
            return SCUtility.Trim(port_station.GROUP_ID, true);
        }
        public bool IsSourcePortAGVStation(BLL.PortStationBLL portStationBLL, BLL.EqptBLL eqptBLL)
        {
            var port_station = portStationBLL.OperateCatch.getPortStation(this.HOSTSOURCE);
            if (port_station == null) return false;
            return port_station.GetEqpt(eqptBLL) is IAGVStationType;
        }

        public override string ToString()
        {
            return $"Command:{this.ID},source:{this.HOSTSOURCE},desc:{this.HOSTDESTINATION},inser time:{CMD_INSER_TIME.ToString()}";
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

}
