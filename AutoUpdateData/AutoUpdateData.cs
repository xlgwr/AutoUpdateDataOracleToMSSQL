using AutoUpdateData.Basic;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;


using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

using System.IO;
using AutoUpdateData.Core.dal;
using Quartz;
using Quartz.Impl;
using AutoUpdateData.Service.Job;

namespace AutoUpdateData
{
    public partial class AutoUpdateData : Form
    {
        private readonly ILog logger;
        public static IScheduler _scheduler;

        public static IJobDetail _upload_job;
        public static ITrigger _upload_trigger;


        public static IList<string> _tableList;
        public static IList<DataSet> _dsList;
        public AutoUpdateData()
        {
            InitializeComponent();

            logger = LogManager.GetLogger(GetType());
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();

            initfrm();
            this.MinimumSizeChanged += AutoUpdateData_MinimumSizeChanged;
            this.FormClosing += AutoUpdateData_FormClosing;
        }

        void AutoUpdateData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生           
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.notifyIcon1.Icon = this.Icon;
                this.Hide();
            }
            //throw new NotImplementedException();
        }

        void AutoUpdateData_MinimumSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (this.WindowState == FormWindowState.Normal && this.Visible == true)
            {
                this.notifyIcon1.Visible = true;//在通知区显示Form的Icon
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
            }
        }
        void initfrm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            _tableList = new List<string>();
            _dsList = new List<DataSet>();

            tInitIni(false);
            lbl0msg.Text = "";
            _tmpFlagMsg = lbl0msg;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _scheduler.Start();
            logger.Info("Quartz服务成功启动");

            tmenu0Start.Enabled = false;
            tmenu1Stop.Enabled = true;
            lbl1Flag.Text = "Runing";

            updateJob(false);

        }
        void initSet()
        {
            _txt0Rtime = Convert.ToInt32(txt0Rtime.Text);
            _txt1batchNum = Convert.ToInt32(txt1batchNum.Text);
            _updatemode = cbox0updateWay.Text;

            logger.Debug("====================以下参数修改后需重启生效===================");
            logger.DebugFormat("====================Synchronization(min):{0}", _txt0Rtime);
            logger.DebugFormat("====================Batch Number：{0}", _txt1batchNum);
            logger.DebugFormat("====================Update mode：{0}", _updatemode);
        }
        void updateJob(bool restart)
        {
            //init 
            initSet();

            if (restart)
            {
                if (!_scheduler.IsShutdown)
                {
                    _scheduler.Shutdown();
                    _scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    _scheduler.Start();
                    lbl0msg.Text = "Save Success,and restart OK.";
                    logger.Debug("====================以下参数修改后需重启生效===================");
                    logger.DebugFormat("====================Synchronization(min):{0}", _txt0Rtime);
                    logger.DebugFormat("====================Batch Number：{0}", _txt1batchNum);
                    logger.DebugFormat("====================Update mode：{0}", _updatemode);
                }

            }
            #region "upload"
            DateTimeOffset runTime = DateBuilder.EvenSecondDate(DateTimeOffset.Now);

            _upload_job = JobBuilder.Create<updateJob>()
               .WithIdentity("UploadJob", "UploadGroup")
               .Build();

            _upload_trigger = TriggerBuilder.Create()
               .WithIdentity("UploadTrigger", "UploadGroup")
               .StartAt(runTime)
               .WithSimpleSchedule(x => x.WithIntervalInMinutes(_txt0Rtime).RepeatForever())
               .Build();

            // Tell quartz to schedule the job using our trigger
            _scheduler.ScheduleJob(_upload_job, _upload_trigger);

            #endregion "upload"
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbox0updateWay.Text))
            {
                cbox0updateWay.Focus();
                lbl0msg.Text = "Please enter the right content。";
                return;
            }
            int irtime = 0;
            if (!int.TryParse(txt0Rtime.Text, out irtime))
            {
                txt0Rtime.Focus();
                lbl0msg.Text = "Please enter the right number >=1。";
                return;
            }
            int ibatch = 0;
            if (!int.TryParse(txt1batchNum.Text, out ibatch))
            {
                txt1batchNum.Focus();
                lbl0msg.Text = "Please enter the right number >=1。";
                return;
            }
            this.btn0Save.Enabled = false;
            var msg = "Synchronization(min): " + txt0Rtime.Text.Trim() + ",\tBatch Number：" + txt1batchNum.Text + "\nUpdate mode：" + cbox0updateWay.Text;
            if (MessageBox.Show(msg, "notice", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                tInitIni(true);
                lbl0msg.Text = "Save OK。" + DateTime.Now;

                updateJob(true);

            }
            else
            {
                lbl0msg.Text = "Cancel OK。" + DateTime.Now;
            }
            this.btn0Save.Enabled = true;
        }
        /// <summary>
        /// ture: write,false:read
        /// </summary>
        /// <param name="isWriteOrRead"></param>
        public void tInitIni(bool isWriteOrRead)
        {
            try
            {
                var tmpfile = AppDomain.CurrentDomain.BaseDirectory + "\\Set.ini";
                INIFile ini;
                if (!File.Exists(tmpfile))
                {
                    File.WriteAllText(tmpfile, "[Set]", System.Text.Encoding.UTF8);
                    ini = new INIFile(tmpfile);
                    ini.IniWriteValue("Tables", "table", "SHOP_ORDER_OPERATION_TAB|,SHOP_ORD_TAB|,N_TRANSPORT_ORDER_TAB|,N_AIS_SHOP_LIST_PICKED_ACT_TAB|,INVENTORY_TRANSACTION_HIST_TAB|EXPIRATION_DATE,INVENTORY_PART_TAB|LAST_ACTIVITY_DATE");
                    ini.IniWriteValue("Common", "retime", "5");
                    ini.IniWriteValue("Common", "batchNum", "100");
                    ini.IniWriteValue("Common", "updateWay", "1-Direct Update");
                }
                else
                {
                    ini = new INIFile(tmpfile);
                }

                if (isWriteOrRead)
                {
                    ini.IniWriteValue("Common", "retime", txt0Rtime.Text);
                    ini.IniWriteValue("Common", "batchNum", txt1batchNum.Text);
                    ini.IniWriteValue("Common", "updateWay", cbox0updateWay.Text);
                }

                else
                {
                    txt0Rtime.Text = ini.IniReadValue("Common", "retime");
                    txt1batchNum.Text = ini.IniReadValue("Common", "batchNum");
                    cbox0updateWay.Text = ini.IniReadValue("Common", "updateWay");

                    getTables(ini);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
                btn0Save.Enabled = true;
            }

        }
        public void getTables(INIFile ini)
        {
            //like tab1,tab2,tab3
            var tmptable = "";
            tmptable = ini.IniReadValue("Tables", "table");
            if (!string.IsNullOrEmpty(tmptable))
            {
                var tt = tmptable.Split(',');
                _tableList.Clear();
                foreach (var item in tt)
                {
                    _tableList.Add(item.Trim());
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _scheduler.Shutdown();

                logger.Info("Quartz服务成功终止");
            }
            finally { }
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }

            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void tmenu3Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit！", "notice", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _scheduler.Shutdown();
                    logger.Info("Quartz服务成功终止");
                }
                finally { }
                Application.Exit();
            }
        }

        private void tmenu2Set_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.ShowDialog();
        }

        private void btn0Re_Click(object sender, EventArgs e)
        {
            tInitIni(false);
            lbl0msg.Text = "Get Seting Success。" + DateTime.Now;
        }

        private void btn2GetOracle_Click(object sender, EventArgs e)
        {

        }

        private void btn3SaveToSQL_Click(object sender, EventArgs e)
        {

        }

        public int _txt0Rtime { get; set; }

        private void tmenu1Stop_Click(object sender, EventArgs e)
        {
            //initSet();

            _scheduler.PauseAll();
            tmenu1Stop.Enabled = false;
            tmenu0Start.Enabled = true;
            lbl1Flag.Text = "Stop";
            lbl0msg.Text = "Stop Success." + DateTime.Now.ToString();
            logger.Info("Quartz服务成功停止");
        }

        private void tmenu0Start_Click(object sender, EventArgs e)
        {
            //initSet();

            _scheduler.ResumeAll();
            tmenu1Stop.Enabled = true;
            tmenu0Start.Enabled = false;
            lbl1Flag.Text = "Running";
            lbl0msg.Text = "Run Success." + DateTime.Now.ToString();
            logger.Info("Quartz服务成功重新开始");
        }
        public static Label _tmpFlagMsg;
        public static void jobflag(string msg)
        {
            _tmpFlagMsg.Invoke(new MethodInvoker(delegate()
            {
                _tmpFlagMsg.Text = msg;
            }));
        }

        public static int _txt1batchNum { get; set; }

        public static string _updatemode { get; set; }

        private void txt0Rtime_TextChanged(object sender, EventArgs e)
        {
            var tmpdd = 0;
            if (!int.TryParse(txt0Rtime.Text, out tmpdd))
            {
                lbl0msg.Text = "Please enter a right number. >=1 (min).";
                txt0Rtime.Focus();
            }
            else
            {
                lbl0msg.Text = "";
            }
        }

        private void txt1batchNum_TextChanged(object sender, EventArgs e)
        {
            var tmpdd = 0;
            if (!int.TryParse(txt1batchNum.Text, out tmpdd))
            {
                lbl0msg.Text = "Please enter a right number.>=1.";
                txt1batchNum.Focus();
            }
            else
            {
                lbl0msg.Text = "";
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.ShowDialog();
        }
    }
}
