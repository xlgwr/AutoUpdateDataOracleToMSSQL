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
using System.Resources;


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

        public static string _CONTRACT;
        public static string _PRIME_COMMODITY;
        public static Dictionary<string, int> _tableList;
        public static Dictionary<string, string> _tableKeyList;
        public static IList<DataSet> _dsList;

        public static bool _isUploading;
        public static INIFile _ini;
        public static INIFile _iniToday;

        public static string _ipAddMac;

        public AutoUpdateData()
        {
            InitializeComponent();
            try
            {
                logger = LogManager.GetLogger(GetType());
                _scheduler = StdSchedulerFactory.GetDefaultScheduler();

                initfrm();
                this.MinimumSizeChanged += AutoUpdateData_MinimumSizeChanged;
                this.FormClosing += AutoUpdateData_FormClosing;
            }
            catch (Exception ex)
            {
                AutoUpdateData.jobflag("Error:" + ex.Message);
                MessageBox.Show(ex.Message); ;
            }

        }

        void AutoUpdateData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生           
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                notifyIcon1.Icon = Properties.Resources.run;
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
                this.notifyIcon1.Icon = Properties.Resources.run;
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
            }
        }

        #region init set
        void initfrm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            this.btn0InitFirst.Visible = false;
            this.btn0InitFirst.Enabled = false;
            ////this.TopMost = true;
            this.btn0Save.Enabled = false;
            this.groupBox1.Enabled = false;


            _tableList = new Dictionary<string, int>();
            _tableKeyList = new Dictionary<string, string>();
            _dsList = new List<DataSet>();
            _isUploading = false;
            _CONTRACT = System.Configuration.ConfigurationManager.AppSettings["CONTRACT"].ToString();
            _PRIME_COMMODITY = System.Configuration.ConfigurationManager.AppSettings["PRIME_COMMODITY"].ToString();

            if (string.IsNullOrEmpty(_CONTRACT))
            {
                _CONTRACT = "no CONTRACT,please set,than Run again.";
            }
            tInitIni(false);
            tInitIniToday(DateTime.Now.ToString("yyyyMMdd"));
            lbl0msg.Text = "";
            _tmpFlagMsg = lbl0msg;

            this.Text += ",C:[" + _CONTRACT + "], P_C:" + _PRIME_COMMODITY;

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.Visible = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _scheduler.Start();
                logger.Info("Quartz服务成功启动");
                notifyIcon1.Text = "AutoUpdateDate is Running.";

                tmenu0Start.Enabled = false;
                tmenu1Stop.Enabled = true;
                lbl1Flag.Text = "Runing";

                notifyIcon1.Icon = Properties.Resources.run;
                _tmpNotifyIcon = notifyIcon1;

                updateJob(false);
                this.btn0Save.Enabled = false;
                this.tmenu2Set.Visible = false;
                _ipAddMac = OracleDal.getIp();
            }
            catch (Exception ex)
            {
                MessageBox.Show("首次运行出错：" + ex.Message);
                logger.Error(ex.Message);
                //AutoUpdateData.jobflag("Error:" + ex.Message);
            }

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
                    _scheduler.DeleteJob(_upload_job.Key);
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

        #endregion
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
        public static void tInitIniToday(string tmpnameToday)
        {
            try
            {
                //var tmpnameToday = DateTime.Now.ToString("yyyyMMdd");
                var tmpfilePath = AppDomain.CurrentDomain.BaseDirectory + "\\TableUploadNum";

                var tmpfile = tmpfilePath + "\\TableUploadNum" + tmpnameToday + ".ini";

                if (!Directory.Exists(tmpfilePath))
                {
                    Directory.CreateDirectory(tmpfilePath);
                }

                if (!File.Exists(tmpfile))
                {

                    File.WriteAllText(tmpfile, "********************Set for today Upload Num for each Table******************:" + tmpnameToday, System.Text.Encoding.UTF8);
                    _iniToday = new INIFile(tmpfile);
                    foreach (var item in _tableList.Keys)
                    {
                        var tmpkey = item;
                        if (item.IndexOf('|') > 0)
                        {
                            var dd = item.Split('|');
                            tmpkey = dd[0];
                        }
                        _iniToday.IniWriteValue("TableTakeDataNum", tmpkey, "0");
                    }
                }
                else
                {
                    _iniToday = new INIFile(tmpfile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

                if (!File.Exists(tmpfile))
                {
                    File.WriteAllText(tmpfile, "[Set]", System.Text.Encoding.UTF8);
                    _ini = new INIFile(tmpfile);
                }
                else
                {
                    _ini = new INIFile(tmpfile);
                }

                _ini.IniWriteValue("Tables", "table1", System.Configuration.ConfigurationManager.AppSettings["Tables.table1"]);//delete then add
                _ini.IniWriteValue("Tables", "table2", System.Configuration.ConfigurationManager.AppSettings["Tables.table2"]);//追加累积更新,id
                //CONTRACT,N_SHOP_LIST_ID,PART_NO,LOT_BATCH_NO //CONTRACT,N_TRANSPORT_ORDER_NO
                _ini.IniWriteValue("Tables", "table3", System.Configuration.ConfigurationManager.AppSettings["Tables.table3"]);//追加累积更新,time table|where|order by
                //P|where|order by|C get key
                _ini.IniWriteValue("Tables", "table4", System.Configuration.ConfigurationManager.AppSettings["Tables.table4"]); //父子表更新
                //init key of tables
                _ini.IniWriteValue("TablesKey", "tableKeys1", System.Configuration.ConfigurationManager.AppSettings["TablesKey.tableKeys1"]);//delete then add
                _ini.IniWriteValue("TablesKey", "tableKeys2", System.Configuration.ConfigurationManager.AppSettings["TablesKey.tableKeys2"]);//追加累积更新,id
                _ini.IniWriteValue("TablesKey", "tableKeys3", System.Configuration.ConfigurationManager.AppSettings["TablesKey.tableKeys3"]);//追加累积更新,time table|where|order by
                _ini.IniWriteValue("TablesKey", "tableKeys4", System.Configuration.ConfigurationManager.AppSettings["TablesKey.tableKeys4"]);//父子表更新

                //ini.IniWriteValue("Tables", "CONTRACT", "sh");//sh：上海,tai:泰国,jp:日本

                _ini.IniWriteValue("Common", "retime", System.Configuration.ConfigurationManager.AppSettings["Common.retime"]);
                _ini.IniWriteValue("Common", "batchNum", System.Configuration.ConfigurationManager.AppSettings["Common.batchNum"]);
                //1-删除后再追加 2-直接更新
                _ini.IniWriteValue("Common", "updateWay", System.Configuration.ConfigurationManager.AppSettings["Common.updateWay"]);//2-Direct Update

                //init first
                //_ini.IniWriteValue("InitFirst", "FristDownload", "0");//System.Configuration.ConfigurationManager.AppSettings["InitFirst.FristDownload"]);//0:没有首次导入，1：已首次导入。
                //_ini.IniWriteValue("InitFirst", "FristDownloadtime", System.Configuration.ConfigurationManager.AppSettings["InitFirst.FristDownloadtime"]);


                if (isWriteOrRead)
                {
                    _ini.IniWriteValue("Common", "retime", txt0Rtime.Text);
                    _ini.IniWriteValue("Common", "batchNum", txt1batchNum.Text);
                    _ini.IniWriteValue("Common", "updateWay", cbox0updateWay.Text);
                }

                else
                {
                    txt0Rtime.Text = _ini.IniReadValue("Common", "retime");
                    txt1batchNum.Text = _ini.IniReadValue("Common", "batchNum");
                    cbox0updateWay.Text = _ini.IniReadValue("Common", "updateWay");

                    getTables(_ini);
                }
                //init falg

                //_getInitFlag = _ini.IniReadValue("InitFirst", "FristDownload");
                //_getInitFlagTime = _ini.IniReadValue("InitFirst", "FristDownloadtime");
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

            var tmptable1 = ini.IniReadValue("Tables", "table1");
            var tmptable2 = ini.IniReadValue("Tables", "table2");
            var tmptable3 = ini.IniReadValue("Tables", "table3");
            var tmptable4 = ini.IniReadValue("Tables", "table4");
            //key
            var tableKeys1 = ini.IniReadValue("TablesKey", "tableKeys1");
            var tableKeys2 = ini.IniReadValue("TablesKey", "tableKeys2");
            var tableKeys3 = ini.IniReadValue("TablesKey", "tableKeys3");
            var tableKeys4 = ini.IniReadValue("TablesKey", "tableKeys4");

            _tableList.Clear();

            init_tableList(tmptable1, 1);
            init_tableList(tmptable2, 2);
            init_tableList(tmptable3, 3);
            init_tableList(tmptable4, 4);
            //for key
            _tableKeyList.Clear();
            init_tableKeyList(tableKeys1);
            init_tableKeyList(tableKeys2);
            init_tableKeyList(tableKeys3);
            init_tableKeyList(tableKeys4);

        }
        void init_tableList(string tmptable, int flag)
        {
            if (!string.IsNullOrEmpty(tmptable))
            {
                var tt = tmptable.Split('*');
                foreach (var item in tt)
                {
                    _tableList.Add(item.Trim(), flag);
                }

            }
        }
        void init_tableKeyList(string tmptable)
        {
            if (!string.IsNullOrEmpty(tmptable))
            {
                var tt = tmptable.Split('*');
                foreach (var item in tt)
                {
                    var tmpKeys = item.Split('|');
                    _tableKeyList.Add(tmpKeys[0], tmpKeys[1]);
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
            this.Visible = true;
        }


        private void tmenu1Stop_Click(object sender, EventArgs e)
        {
            //initSet();

            notifyIcon1.Icon = Properties.Resources.stop;
            _scheduler.PauseAll();
            tmenu1Stop.Enabled = false;
            tmenu0Start.Enabled = true;
            lbl1Flag.Text = "Stop";
            lbl0msg.Text = "Stop Success." + DateTime.Now.ToString();
            logger.Info("Quartz服务成功停止");

            notifyIcon1.Text = "AutoUpdateDate is Stop.";
        }

        private void tmenu0Start_Click(object sender, EventArgs e)
        {
            //initSet();
            notifyIcon1.Icon = Properties.Resources.run;
            _scheduler.ResumeAll();
            tmenu1Stop.Enabled = true;
            tmenu0Start.Enabled = false;
            lbl1Flag.Text = "Running";
            lbl0msg.Text = "Run Success." + DateTime.Now.ToString();
            logger.Info("Quartz服务成功重新开始");
            notifyIcon1.Text = "AutoUpdateDate is Running.";
        }
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
        public static Icon _run = Properties.Resources.run;
        public static Icon _stop = Properties.Resources.stop;

        public static void jobflag(string msg)
        {
            _tmpFlagMsg.Invoke(new MethodInvoker(delegate()
            {
                _tmpFlagMsg.Text = msg;
                if (msg.StartsWith("Error:"))
                {
                    _tmpNotifyIcon.Icon = _stop;
                }
                else
                {
                    _tmpNotifyIcon.Icon = _run;
                }
            }));
        }

        public static int _txt1batchNum { get; set; }
        public static string _updatemode { get; set; }//1-删除后再追加 2-直接更新
        public static Label _tmpFlagMsg;
        public static NotifyIcon _tmpNotifyIcon;
        public int _txt0Rtime { get; set; }

        public string tmptable1 { get; set; }

        private void txt0Rtime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9 || e.KeyCode == Keys.F10)
            {
                //0：没有首次导入，1：已首次导入
                _getInitFlag = _ini.IniReadValue("InitFirst", "FristDownload");
                _getInitFlagTime = _ini.IniReadValue("InitFirst", "FristDownloadtime");
                if (_getInitFlag.Equals("0") || string.IsNullOrEmpty(_getInitFlag))
                {
                    this.btn0InitFirst.Visible = true;
                    this.btn0InitFirst.Enabled = true;

                }
                else
                {
                    lbl0msg.Text = _getInitFlagTime + " already Update." + DateTime.Now;
                }
            }
            else
            {
                this.btn0InitFirst.Visible = false;
                this.btn0InitFirst.Enabled = false;
            }
        }

        private void btn0InitFirst_Click(object sender, EventArgs e)
        {
            if (_InitFirst.Equals("1"))
            {
                if (MessageBox.Show("Notice", "Are you Sure to Update First,That will use some time. Please Wait.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //
                    var tmpcom = " CONTRACT='" + _CONTRACT + "'";
                    try
                    {
                        _scheduler.Shutdown();
                        //1table INVENTORY_PART_TAB
                        var tmpsql_INVENTORY_PART_TAB = "select * from INVENTORY_PART_TAB where " + tmpcom;
                        var allCount = OracleDal.GetCount("INVENTORY_PART_TAB", tmpsql_INVENTORY_PART_TAB);
                        logger.DebugFormat("*************initFirst INVENTORY_PART_TAB:总有 {0} 条.", allCount);

                        var tmpds_INVENTORY_PART_TAB = OracleDal.Query(tmpsql_INVENTORY_PART_TAB);
                        tmpds_INVENTORY_PART_TAB.DataSetName = "INVENTORY_PART_TAB";

                        //**************************同步 INVENTORY_PART_TAB
                        OracleDal.StartToMSSQL(true, tmpds_INVENTORY_PART_TAB, "");
                        logger.DebugFormat("*************initFirst INVENTORY_PART_TAB 成功.");
                        //
                    }
                    catch (Exception ex)
                    {
                        logger.Error("**************更新失败:" + ex.Message);
                    }

                    //2
                }
            }
        }

        public bool _InitFirst { get; set; }

        public string _getInitFlag { get; set; }

        public string _getInitFlagTime { get; set; }
    }
}
