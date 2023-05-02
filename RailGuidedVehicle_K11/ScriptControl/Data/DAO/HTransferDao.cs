using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static com.mirle.ibg3k0.sc.BLL.TransferBLL.DB;


namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class HTransferDao
    {
        public void add(DBConnection_EF con, HTRANSFER hTran)
        {
            con.HTRANSFER.Add(hTran);
            con.SaveChanges();
        }

        public void AddByBatch(DBConnection_EF con, List<HTRANSFER> cmd_ohtcs)
        {
            con.HTRANSFER.AddRange(cmd_ohtcs);
            con.SaveChanges();
        }

        public List<ObjectRelay.HCMD_MCSObjToShow> loadLast24Hours(DBConnection_EF con)
        {
            DateTime query_time = DateTime.Now.AddHours(-24);
            var query = from cmd in con.HTRANSFER
                        where cmd.CMD_INSER_TIME > query_time
            orderby cmd.CMD_INSER_TIME descending
                        select new ObjectRelay.HCMD_MCSObjToShow() { cmd_mcs = cmd };
            return query.ToList();
        }
        public List<HCMD_MCSObjToShow> loadByInsertTimeEndTime(DBConnection_EF con, DateTime insertTime, DateTime finishTime)
        {
            var query = from transfer in con.HTRANSFER
                        join cmd in con.HCMD on transfer.EXCUTE_CMD_ID equals cmd.ID into cmdGroup
                        from cmd in cmdGroup.DefaultIfEmpty()
                        where transfer.CMD_INSER_TIME >= insertTime && transfer.CMD_INSER_TIME <= finishTime
                        orderby transfer.CMD_INSER_TIME descending
                        select new HCMD_MCSObjToShow() { cmd_mcs = transfer, hcmd = cmd};
            return query.ToList();
        }
        public List<HCMD_MCSObjToShow> loadByInsertTimeEndTimeAndVhID(DBConnection_EF con, DateTime insertTime, DateTime finishTime, string vhID)
        {
            var query = from transfer in con.HTRANSFER
                        join cmd in con.HCMD on transfer.EXCUTE_CMD_ID equals cmd.ID
                        where transfer.CMD_INSER_TIME >= insertTime && transfer.CMD_INSER_TIME <= finishTime && cmd.VH_ID == vhID
                        orderby transfer.CMD_INSER_TIME descending
                        select new HCMD_MCSObjToShow() { cmd_mcs = transfer, hcmd = cmd };
            return query.ToList();
        }



    }

}
