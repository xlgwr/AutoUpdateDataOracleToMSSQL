namespace AutoUpdateData
{
    partial class AutoUpdateData
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoUpdateData));
            this.btn0Save = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmenu0Start = new System.Windows.Forms.ToolStripMenuItem();
            this.tmenu1Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.tmenu2Set = new System.Windows.Forms.ToolStripMenuItem();
            this.tmenu3Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbox0updateWay = new System.Windows.Forms.ComboBox();
            this.lbl1Flag = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt1batchNum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt0Rtime = new System.Windows.Forms.TextBox();
            this.lbl0msg = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn0Save
            // 
            this.btn0Save.Location = new System.Drawing.Point(159, 158);
            this.btn0Save.Name = "btn0Save";
            this.btn0Save.Size = new System.Drawing.Size(97, 45);
            this.btn0Save.TabIndex = 0;
            this.btn0Save.Text = "Save(&S)";
            this.btn0Save.UseVisualStyleBackColor = true;
            this.btn0Save.Click += new System.EventHandler(this.button1_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "AutoUpdateData";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmenu0Start,
            this.tmenu1Stop,
            this.tmenu2Set,
            this.tmenu3Exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(117, 92);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // tmenu0Start
            // 
            this.tmenu0Start.Name = "tmenu0Start";
            this.tmenu0Start.Size = new System.Drawing.Size(116, 22);
            this.tmenu0Start.Text = "开始(&S)";
            this.tmenu0Start.Click += new System.EventHandler(this.tmenu0Start_Click);
            // 
            // tmenu1Stop
            // 
            this.tmenu1Stop.Name = "tmenu1Stop";
            this.tmenu1Stop.Size = new System.Drawing.Size(116, 22);
            this.tmenu1Stop.Text = "停止(&T)";
            this.tmenu1Stop.Click += new System.EventHandler(this.tmenu1Stop_Click);
            // 
            // tmenu2Set
            // 
            this.tmenu2Set.Name = "tmenu2Set";
            this.tmenu2Set.Size = new System.Drawing.Size(116, 22);
            this.tmenu2Set.Text = "设置(&C)";
            this.tmenu2Set.Click += new System.EventHandler(this.tmenu2Set_Click);
            // 
            // tmenu3Exit
            // 
            this.tmenu3Exit.Name = "tmenu3Exit";
            this.tmenu3Exit.Size = new System.Drawing.Size(116, 22);
            this.tmenu3Exit.Text = "退出(&E)";
            this.tmenu3Exit.Click += new System.EventHandler(this.tmenu3Exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbox0updateWay);
            this.groupBox1.Controls.Add(this.lbl1Flag);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt1batchNum);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txt0Rtime);
            this.groupBox1.Location = new System.Drawing.Point(12, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 148);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seting";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Update Mode：";
            // 
            // cbox0updateWay
            // 
            this.cbox0updateWay.FormattingEnabled = true;
            this.cbox0updateWay.Items.AddRange(new object[] {
            "1-Deleted First,Then Adding",
            "2-Direct Update"});
            this.cbox0updateWay.Location = new System.Drawing.Point(147, 101);
            this.cbox0updateWay.Name = "cbox0updateWay";
            this.cbox0updateWay.Size = new System.Drawing.Size(182, 20);
            this.cbox0updateWay.TabIndex = 6;
            // 
            // lbl1Flag
            // 
            this.lbl1Flag.AutoSize = true;
            this.lbl1Flag.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl1Flag.ForeColor = System.Drawing.Color.Red;
            this.lbl1Flag.Location = new System.Drawing.Point(249, 0);
            this.lbl1Flag.Name = "lbl1Flag";
            this.lbl1Flag.Size = new System.Drawing.Size(62, 16);
            this.lbl1Flag.TabIndex = 5;
            this.lbl1Flag.Text = "notice";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Batch Number：";
            // 
            // txt1batchNum
            // 
            this.txt1batchNum.Location = new System.Drawing.Point(147, 62);
            this.txt1batchNum.Name = "txt1batchNum";
            this.txt1batchNum.Size = new System.Drawing.Size(112, 21);
            this.txt1batchNum.TabIndex = 4;
            this.txt1batchNum.Text = "100";
            this.txt1batchNum.TextChanged += new System.EventHandler(this.txt1batchNum_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Synchronization(min):";
            // 
            // txt0Rtime
            // 
            this.txt0Rtime.Location = new System.Drawing.Point(147, 24);
            this.txt0Rtime.Name = "txt0Rtime";
            this.txt0Rtime.Size = new System.Drawing.Size(112, 21);
            this.txt0Rtime.TabIndex = 0;
            this.txt0Rtime.TextChanged += new System.EventHandler(this.txt0Rtime_TextChanged);
            // 
            // lbl0msg
            // 
            this.lbl0msg.AutoSize = true;
            this.lbl0msg.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl0msg.ForeColor = System.Drawing.Color.Red;
            this.lbl0msg.Location = new System.Drawing.Point(35, 216);
            this.lbl0msg.Name = "lbl0msg";
            this.lbl0msg.Size = new System.Drawing.Size(55, 14);
            this.lbl0msg.TabIndex = 3;
            this.lbl0msg.Text = "notice";
            // 
            // AutoUpdateData
            // 
            this.AcceptButton = this.btn0Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(426, 249);
            this.Controls.Add(this.lbl0msg);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn0Save);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AutoUpdateData";
            this.Text = "AutoUpdateData";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn0Save;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tmenu0Start;
        private System.Windows.Forms.ToolStripMenuItem tmenu1Stop;
        private System.Windows.Forms.ToolStripMenuItem tmenu2Set;
        private System.Windows.Forms.ToolStripMenuItem tmenu3Exit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt0Rtime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt1batchNum;
        private System.Windows.Forms.Label lbl1Flag;
        public System.Windows.Forms.Label lbl0msg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbox0updateWay;
    }
}

