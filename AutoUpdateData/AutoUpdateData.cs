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

using System.IO;

namespace AutoUpdateData
{
    public partial class AutoUpdateData : Form
    {
        public AutoUpdateData()
        {
            InitializeComponent();

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
            tInitIni(false);
            lbl0msg.Text = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbox0updateWay.Text))
            {
                cbox0updateWay.Focus();
                lbl0msg.Text = "更新方式内容为空。请选择正确的数据。";
                return;
            }
            int irtime=0;
            if (!int.TryParse(txt0Rtime.Text,out irtime))
            {
                txt0Rtime.Focus();
                lbl0msg.Text = "同步时间输入不正确。请输入正确的数字。";
                return;
            }
            int ibatch = 0;
            if (!int.TryParse(txt1batchNum.Text, out ibatch))
            {
                txt1batchNum.Focus();
                lbl0msg.Text = "分批提交输入不正确。请输入正确的数字。";
                return;
            }
            this.btn0Save.Enabled = false;
            var msg = "确定同步时间：" + txt0Rtime.Text.Trim() + ",\t分批提交数：" + txt1batchNum.Text + ",\t更新方式：" + cbox0updateWay.Text;
            if (MessageBox.Show(msg, "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                tInitIni(true);
                lbl0msg.Text = "保存设置成功。" + DateTime.Now; 

            }
            else
            {
                lbl0msg.Text = "取消保存设置。" + DateTime.Now; 
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
                if (!File.Exists(tmpfile))
                {
                    File.WriteAllText(tmpfile, "[Set]", System.Text.Encoding.UTF8);

                }
                INIFile ini = new INIFile(tmpfile);
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
                }
            }
            catch (Exception ex)
            {
                log<AutoUpdateData>(ex);
                MessageBox.Show(ex.Message);
                btn0Save.Enabled = true;
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            if (MessageBox.Show("是否确认退出！", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
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
            lbl0msg.Text = "刷新设置成功。" + DateTime.Now;
        }
    }
}
