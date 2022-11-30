using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using trackService.GrpcService;
using Grpc.Core;
using GrpcServiceForm.GrpcService;
using System.Net;

namespace trackService_RGV
{
    public partial class mainForm : Form
    {

        Server gRPC_Service;
        trackService_RGV.Library.trackService trackService;
        Timer timer;
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {

            #region timer
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timerTick;
            #endregion

            #region trackService initial
            trackService = new trackService_RGV.Library.trackService(@"trackServiceConfig.xml", out string result);
            #endregion

            #region gRPC Service
            gRPC_Service = new Server()
            {
                Services = { Greeter.BindService(new GreeterService(this, trackService)) },
                Ports = { new ServerPort(IPAddress.Any.ToString(), 6060, ServerCredentials.Insecure) },
            };
            gRPC_Service.Start();
            #endregion

            #region initial comboBox
            var v = trackService.getAllTrackNumber();
            foreach(string number in v)
                comboBox1.Items.Add(number);
            comboBox1.SelectedIndex = 0;
            #endregion

            timer.Start();

        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            gRPC_Service.KillAsync();
            timer.Stop();
            timer.Dispose();
        }

        private void timerTick(object o, EventArgs args)
        {
            lbl_connectionResultValue.Text = trackService.isConnectionSuccess.ToString();
            dataGridView1.Rows.Clear();

            var track = trackService.getTrack(comboBox1.Text);

            //dataGridView1.Rows.Add(new string[] { "編號", track.TrackNumber });
            //dataGridView1.Rows.Add(new string[] { "index", track.AliveValue.ToString() });
            //dataGridView1.Rows.Add(new string[] { "status", track.TrackStatus.ToString() });
            //dataGridView1.Rows.Add(new string[] { "dir", track.TrackDir.ToString() });
            //dataGridView1.Rows.Add(new string[] { "block", track.TrackBlock.ToString() });
            //dataGridView1.Rows.Add(new string[] { "rgv_user", track.RGV_User.ToString() });
            //dataGridView1.Rows.Add(new string[] { "track_user", track.Track_User.ToString() });
            //dataGridView1.Rows.Add(new string[] { "切換次數", track.TrackChangeCounter.ToString() });
            dataGridView1.Rows.Add(new string[] { "alarmCode", track.AlarmCode });
            //dataGridView1.Rows.Add(new string[] { "version", track.Version });
        }

    }
}
 