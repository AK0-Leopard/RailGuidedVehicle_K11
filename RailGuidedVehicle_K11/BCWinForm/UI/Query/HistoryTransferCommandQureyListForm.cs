using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class HistoryTransferCommandQureyListForm : Form
    {
        BCMainForm mainform;
        BindingSource cmsMCS_bindingSource = new BindingSource();
        List<HCMD_MCSObjToShow> cmdMCSList = null;
        public HistoryTransferCommandQureyListForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            dgv_TransferCommand.AutoGenerateColumns = false;
            mainform = _mainForm;
            var vh_ids = mainform.BCApp.SCApplication.VehicleBLL.cache.loadAllVh().Select(v => v.VEHICLE_ID);
            List<string> eq_ids = new List<string>();
            eq_ids.Add("");
            eq_ids.AddRange(vh_ids);
            m_EqptIDCbx.DataSource = eq_ids;
            dgv_TransferCommand.DataSource = cmsMCS_bindingSource;
        }

        private void updateTransferCommand()
        {
            cmdMCSList = mainform.BCApp.SCApplication.CMDBLL.loadHCMD_MCSs();
            cmsMCS_bindingSource.DataSource = cmdMCSList;
            dgv_TransferCommand.Refresh();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            updateTransferCommand();
        }
        private void TransferCommandQureyListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainform.removeForm(this.Name);
        }

        const int SEARCH_MAX_DATS = 90;
        private async void btnlSearch_Click(object sender, EventArgs e)
        {
            try
            {

                btnlSearch.Enabled = false;
                DateTime start_time = m_StartDTCbx.Value;
                DateTime end_time = m_EndDTCbx.Value;
                string vh_id = m_EqptIDCbx.Text;
                if (start_time > end_time)
                {
                    MessageBox.Show("From time cannot be later than end time.", "search fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((end_time - start_time).TotalDays > SEARCH_MAX_DATS)
                {
                    MessageBox.Show($"The search date cannot exceed {SEARCH_MAX_DATS} days.", "search fail.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //List<HCMD_MCSObjToShow> cmd_mcs_list = null;
                await Task.Run(() =>
                {
                    cmdMCSList = mainform.BCApp.SCApplication.CMDBLL.loadByInsertTimeEndTimeAndVhID(start_time, end_time, vh_id);
                    SCUtility.TrimAllParameter(cmdMCSList);
                });
                dgv_TransferCommand.DataSource = cmdMCSList;
                lbl_commandCountValue.Text = cmdMCSList.Count.ToString();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
            }
            finally
            {
                btnlSearch.Enabled = true;
            }
        }

        private async void m_exportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Command files (*.xlsx)|*.xlsx";
                if (string.IsNullOrWhiteSpace(m_EqptIDCbx.Text))
                {
                    dlg.FileName = "TransferCommand" + "_" + m_StartDTCbx.Value.ToString("yyyyMMddHH") + "-" + m_EndDTCbx.Value.ToString("yyyyMMddHH");
                }
                else
                {
                    dlg.FileName = "[" + m_EqptIDCbx.Text + "] " + "TransferCommand" + "_" + m_StartDTCbx.Value.ToString("yyyyMMddHH") + "-" + m_EndDTCbx.Value.ToString("yyyyMMddHH");
                }
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK || bcf.Common.BCFUtility.isEmpty(dlg.FileName))
                {
                    return;
                }
                string filename = dlg.FileName;
                //建立 xlxs 轉換物件
                Common.XSLXHelper helper = new Common.XSLXHelper();
                //取得轉為 xlsx 的物件
                ClosedXML.Excel.XLWorkbook xlsx = null;
                await Task.Run(() => xlsx = helper.Export(cmdMCSList));
                if (xlsx != null)
                    xlsx.SaveAs(filename);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Exception");
            }
        }
    }
}
