﻿using AutoUpdateData.Basic;
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

namespace AutoUpdateData
{
    public partial class AutoUpdateData : Form
    {
        private readonly ILog logger;
        public static IScheduler scheduler;


        public static IList<string> _tableList;
        public static IList<DataSet> _dsList;
        public AutoUpdateData()
        {
            InitializeComponent();

            logger = LogManager.GetLogger(GetType());
            scheduler = StdSchedulerFactory.GetDefaultScheduler();

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
        #region ilog

        public static void log<T>(string msg)
            where T : class
        {
            //第一种记录用法
            //（1）FormMain是类名称
            //（2）第二个参数是字符串信息 "测试Log4Net日志是否写入"
            LogHelper.WriteLog(typeof(T), msg);
        }
        public static void log<T>(Exception ex)
            where T : class
        {
            LogHelper.WriteLog(typeof(T), ex);

            //第二种记录用法
            //（1）FormMain是类名称
            //（2）第二个参数是需要捕捉的异常块
            //try { 

            //}catch(Exception ex){

            //    LogHelper.WriteLog(typeof(FormMain), ex);

            //}
        }
        #endregion
        void initfrm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            _tableList = new List<string>();
            _dsList = new List<DataSet>();

            tInitIni(false);
            lbl0msg.Text = "";

        }
        private void Form1_Load(object sender, EventArgs e)
        {

            logger.Debug("====================以下参数修改后需重启服务生效===================");

            scheduler.Start();
            logger.Info("Quartz服务成功启动");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbox0updateWay.Text))
            {
                cbox0updateWay.Focus();
                lbl0msg.Text = "Please enter the right content 3。";
                return;
            }
            int irtime = 0;
            if (!int.TryParse(txt0Rtime.Text, out irtime))
            {
                txt0Rtime.Focus();
                lbl0msg.Text = "Please enter the right number 1。";
                return;
            }
            int ibatch = 0;
            if (!int.TryParse(txt1batchNum.Text, out ibatch))
            {
                txt1batchNum.Focus();
                lbl0msg.Text = "Please enter the right number 2。";
                return;
            }
            this.btn0Save.Enabled = false;
            var msg = "Synchronization(min): " + txt0Rtime.Text.Trim() + ",\tBatch Number：" + txt1batchNum.Text + "\nUpdate mode：" + cbox0updateWay.Text;
            if (MessageBox.Show(msg, "notice", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                tInitIni(true);
                lbl0msg.Text = "Save OK。" + DateTime.Now;

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
                    ini.IniWriteValue("Common", "updateWay", "Direct Update");
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
                log<AutoUpdateData>(ex);
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
                scheduler.Shutdown();
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
                    scheduler.Shutdown();
                    logger.Info("Quartz服务成功终止");
                }
                finally { }
                Application.Exit();
            }
        }

        private void tmenu2Set_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void btn0Re_Click(object sender, EventArgs e)
        {
            tInitIni(false);
            lbl0msg.Text = "Get Seting Success。" + DateTime.Now;
        }

        private void btn2GetOracle_Click(object sender, EventArgs e)
        {
            if (_tableList.Count > 0)
            {
                _dsList.Clear();
                foreach (var item in _tableList)
                {
                    if (item.Contains('|'))
                    {
                        var td = item.Split('|');
                        var tmpwhere = td[1] + ">=to_date(:gxsj,'yyyy-MM-dd HH24:mi:ss')";
                        OracleParameter[] parameters = { new OracleParameter(":gxsj", OracleDbType.Varchar2, 10) };
                        parameters[0].Value = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH") + ":00:00";

                        var tmpds = OracleDal.GetData(td[0].Trim(), tmpwhere, 2, parameters);
                        tmpds.DataSetName = td[0].Trim();
                        _dsList.Add(tmpds);
                        dgv01GetData.DataSource = tmpds.Tables[0].DefaultView;
                        dgv01GetData.Refresh();
                    }
                    else
                    {
                        var tmpds = OracleDal.GetData(item.Trim(), "", 2);
                        tmpds.DataSetName = item.Trim();
                        _dsList.Add(tmpds);
                    }
                }
            }
            else
            {
                MessageBox.Show("no Table,Please check Set.ini,and add Table.");
            }
        }

        private void btn3SaveToSQL_Click(object sender, EventArgs e)
        {
            var strSQLinsert = new List<String>();
            if (_dsList.Count > 0)
            {
                foreach (var item in _dsList)
                {
                    var tmpcolDS = OracleDal.GetTableColumns(item.DataSetName);

                    foreach (DataRow row in item.Tables[0].Rows)
                    {
                        var tmpinstall = OracleDal.getSQLColumnsForInsert(tmpcolDS, item.DataSetName, row);
                        if (!string.IsNullOrEmpty(tmpinstall))
                        {
                            strSQLinsert.Add(tmpinstall);
                        }

                    }
                }

            }
            var dd = DbHelperSQL.ExecuteSqlTran(strSQLinsert);
            if (dd > 0)
            {
                MessageBox.Show("Update Count:" + dd.ToString());
            }
        }
    }
}
