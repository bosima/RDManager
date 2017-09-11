namespace RDManager
{
    partial class SSHControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.sshTab = new System.Windows.Forms.TabControl();
            this.shellTab = new System.Windows.Forms.TabPage();
            this.sFtpTab = new System.Windows.Forms.TabPage();
            this.FSSplitContainer = new System.Windows.Forms.SplitContainer();
            this.LRSplitContainer = new System.Windows.Forms.SplitContainer();
            this.localListView = new System.Windows.Forms.ListView();
            this.remoteListView = new System.Windows.Forms.ListView();
            this.statusListView = new System.Windows.Forms.ListView();
            this.sshTab.SuspendLayout();
            this.sFtpTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FSSplitContainer)).BeginInit();
            this.FSSplitContainer.Panel1.SuspendLayout();
            this.FSSplitContainer.Panel2.SuspendLayout();
            this.FSSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LRSplitContainer)).BeginInit();
            this.LRSplitContainer.Panel1.SuspendLayout();
            this.LRSplitContainer.Panel2.SuspendLayout();
            this.LRSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // sshTab
            // 
            this.sshTab.Controls.Add(this.shellTab);
            this.sshTab.Controls.Add(this.sFtpTab);
            this.sshTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sshTab.Location = new System.Drawing.Point(0, 0);
            this.sshTab.Name = "sshTab";
            this.sshTab.SelectedIndex = 0;
            this.sshTab.Size = new System.Drawing.Size(1179, 845);
            this.sshTab.TabIndex = 0;
            // 
            // shellTab
            // 
            this.shellTab.Location = new System.Drawing.Point(8, 39);
            this.shellTab.Name = "shellTab";
            this.shellTab.Padding = new System.Windows.Forms.Padding(3);
            this.shellTab.Size = new System.Drawing.Size(1163, 798);
            this.shellTab.TabIndex = 0;
            this.shellTab.Text = "Shell";
            this.shellTab.UseVisualStyleBackColor = true;
            // 
            // sFtpTab
            // 
            this.sFtpTab.Controls.Add(this.FSSplitContainer);
            this.sFtpTab.Location = new System.Drawing.Point(8, 39);
            this.sFtpTab.Name = "sFtpTab";
            this.sFtpTab.Padding = new System.Windows.Forms.Padding(3);
            this.sFtpTab.Size = new System.Drawing.Size(1163, 798);
            this.sFtpTab.TabIndex = 1;
            this.sFtpTab.Text = "SFTP";
            this.sFtpTab.UseVisualStyleBackColor = true;
            // 
            // FSSplitContainer
            // 
            this.FSSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FSSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.FSSplitContainer.Name = "FSSplitContainer";
            this.FSSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // FSSplitContainer.Panel1
            // 
            this.FSSplitContainer.Panel1.Controls.Add(this.LRSplitContainer);
            // 
            // FSSplitContainer.Panel2
            // 
            this.FSSplitContainer.Panel2.Controls.Add(this.statusListView);
            this.FSSplitContainer.Size = new System.Drawing.Size(1157, 792);
            this.FSSplitContainer.SplitterDistance = 603;
            this.FSSplitContainer.TabIndex = 0;
            // 
            // LRSplitContainer
            // 
            this.LRSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LRSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.LRSplitContainer.Name = "LRSplitContainer";
            // 
            // LRSplitContainer.Panel1
            // 
            this.LRSplitContainer.Panel1.Controls.Add(this.localListView);
            // 
            // LRSplitContainer.Panel2
            // 
            this.LRSplitContainer.Panel2.Controls.Add(this.remoteListView);
            this.LRSplitContainer.Size = new System.Drawing.Size(1157, 603);
            this.LRSplitContainer.SplitterDistance = 557;
            this.LRSplitContainer.TabIndex = 0;
            // 
            // localListView
            // 
            this.localListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localListView.FullRowSelect = true;
            this.localListView.Location = new System.Drawing.Point(0, 0);
            this.localListView.Name = "localListView";
            this.localListView.Size = new System.Drawing.Size(557, 603);
            this.localListView.TabIndex = 0;
            this.localListView.UseCompatibleStateImageBehavior = false;
            this.localListView.View = System.Windows.Forms.View.Details;
            this.localListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.localListView_ColumnClick);
            this.localListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.localListView_MouseClick);
            this.localListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.localListView_MouseDoubleClick);
            // 
            // remoteListView
            // 
            this.remoteListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remoteListView.FullRowSelect = true;
            this.remoteListView.Location = new System.Drawing.Point(0, 0);
            this.remoteListView.Name = "remoteListView";
            this.remoteListView.Size = new System.Drawing.Size(596, 603);
            this.remoteListView.TabIndex = 0;
            this.remoteListView.UseCompatibleStateImageBehavior = false;
            this.remoteListView.View = System.Windows.Forms.View.Details;
            this.remoteListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.remoteListView_ColumnClick);
            this.remoteListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.remoteListView_MouseDoubleClick);
            // 
            // statusListView
            // 
            this.statusListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusListView.FullRowSelect = true;
            this.statusListView.Location = new System.Drawing.Point(0, 0);
            this.statusListView.Name = "statusListView";
            this.statusListView.Size = new System.Drawing.Size(1157, 185);
            this.statusListView.TabIndex = 0;
            this.statusListView.UseCompatibleStateImageBehavior = false;
            this.statusListView.View = System.Windows.Forms.View.Details;
            // 
            // SSHControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sshTab);
            this.Name = "SSHControl";
            this.Size = new System.Drawing.Size(1179, 845);
            this.sshTab.ResumeLayout(false);
            this.sFtpTab.ResumeLayout(false);
            this.FSSplitContainer.Panel1.ResumeLayout(false);
            this.FSSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FSSplitContainer)).EndInit();
            this.FSSplitContainer.ResumeLayout(false);
            this.LRSplitContainer.Panel1.ResumeLayout(false);
            this.LRSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LRSplitContainer)).EndInit();
            this.LRSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl sshTab;
        private System.Windows.Forms.TabPage shellTab;
        private System.Windows.Forms.TabPage sFtpTab;
        private System.Windows.Forms.SplitContainer FSSplitContainer;
        private System.Windows.Forms.SplitContainer LRSplitContainer;
        private System.Windows.Forms.ListView localListView;
        private System.Windows.Forms.ListView remoteListView;
        private System.Windows.Forms.ListView statusListView;
    }
}
