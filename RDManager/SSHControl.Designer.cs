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
            this.fsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.lrSplitContainer = new System.Windows.Forms.SplitContainer();
            this.leftSplitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlLocalPath = new System.Windows.Forms.Panel();
            this.txtCurrentLocalPath = new System.Windows.Forms.TextBox();
            this.lblLocalDirectory = new System.Windows.Forms.Label();
            this.localListView = new System.Windows.Forms.ListView();
            this.rightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.pnRemotePath = new System.Windows.Forms.Panel();
            this.btnReconnect = new System.Windows.Forms.Button();
            this.txtCurrentRemotePath = new System.Windows.Forms.TextBox();
            this.lblRemoteDirectory = new System.Windows.Forms.Label();
            this.remoteListView = new System.Windows.Forms.ListView();
            this.processListView = new System.Windows.Forms.ListView();
            this.sshTab.SuspendLayout();
            this.sFtpTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsSplitContainer)).BeginInit();
            this.fsSplitContainer.Panel1.SuspendLayout();
            this.fsSplitContainer.Panel2.SuspendLayout();
            this.fsSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lrSplitContainer)).BeginInit();
            this.lrSplitContainer.Panel1.SuspendLayout();
            this.lrSplitContainer.Panel2.SuspendLayout();
            this.lrSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftSplitContainer)).BeginInit();
            this.leftSplitContainer.Panel1.SuspendLayout();
            this.leftSplitContainer.Panel2.SuspendLayout();
            this.leftSplitContainer.SuspendLayout();
            this.pnlLocalPath.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplitContainer)).BeginInit();
            this.rightSplitContainer.Panel1.SuspendLayout();
            this.rightSplitContainer.Panel2.SuspendLayout();
            this.rightSplitContainer.SuspendLayout();
            this.pnRemotePath.SuspendLayout();
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
            this.sFtpTab.Controls.Add(this.fsSplitContainer);
            this.sFtpTab.Location = new System.Drawing.Point(8, 39);
            this.sFtpTab.Name = "sFtpTab";
            this.sFtpTab.Padding = new System.Windows.Forms.Padding(3);
            this.sFtpTab.Size = new System.Drawing.Size(1163, 798);
            this.sFtpTab.TabIndex = 1;
            this.sFtpTab.Text = "SFTP";
            this.sFtpTab.UseVisualStyleBackColor = true;
            // 
            // fsSplitContainer
            // 
            this.fsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fsSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.fsSplitContainer.Name = "fsSplitContainer";
            this.fsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // fsSplitContainer.Panel1
            // 
            this.fsSplitContainer.Panel1.Controls.Add(this.lrSplitContainer);
            // 
            // fsSplitContainer.Panel2
            // 
            this.fsSplitContainer.Panel2.Controls.Add(this.processListView);
            this.fsSplitContainer.Size = new System.Drawing.Size(1157, 792);
            this.fsSplitContainer.SplitterDistance = 603;
            this.fsSplitContainer.TabIndex = 0;
            // 
            // lrSplitContainer
            // 
            this.lrSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lrSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.lrSplitContainer.Name = "lrSplitContainer";
            // 
            // lrSplitContainer.Panel1
            // 
            this.lrSplitContainer.Panel1.Controls.Add(this.leftSplitContainer);
            // 
            // lrSplitContainer.Panel2
            // 
            this.lrSplitContainer.Panel2.Controls.Add(this.rightSplitContainer);
            this.lrSplitContainer.Size = new System.Drawing.Size(1157, 603);
            this.lrSplitContainer.SplitterDistance = 557;
            this.lrSplitContainer.TabIndex = 0;
            // 
            // leftSplitContainer
            // 
            this.leftSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSplitContainer.IsSplitterFixed = true;
            this.leftSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.leftSplitContainer.Name = "leftSplitContainer";
            this.leftSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // leftSplitContainer.Panel1
            // 
            this.leftSplitContainer.Panel1.Controls.Add(this.pnlLocalPath);
            // 
            // leftSplitContainer.Panel2
            // 
            this.leftSplitContainer.Panel2.Controls.Add(this.localListView);
            this.leftSplitContainer.Size = new System.Drawing.Size(557, 603);
            this.leftSplitContainer.SplitterDistance = 59;
            this.leftSplitContainer.TabIndex = 0;
            // 
            // pnlLocalPath
            // 
            this.pnlLocalPath.Controls.Add(this.txtCurrentLocalPath);
            this.pnlLocalPath.Controls.Add(this.lblLocalDirectory);
            this.pnlLocalPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLocalPath.Location = new System.Drawing.Point(0, 0);
            this.pnlLocalPath.Name = "pnlLocalPath";
            this.pnlLocalPath.Size = new System.Drawing.Size(557, 59);
            this.pnlLocalPath.TabIndex = 0;
            this.pnlLocalPath.SizeChanged += new System.EventHandler(this.pnlLocalPath_SizeChanged);
            // 
            // txtCurrentLocalPath
            // 
            this.txtCurrentLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentLocalPath.Location = new System.Drawing.Point(174, 13);
            this.txtCurrentLocalPath.Name = "txtCurrentLocalPath";
            this.txtCurrentLocalPath.Size = new System.Drawing.Size(370, 35);
            this.txtCurrentLocalPath.TabIndex = 3;
            this.txtCurrentLocalPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCurrentLocalPath_KeyUp);
            // 
            // lblLocalDirectory
            // 
            this.lblLocalDirectory.AutoSize = true;
            this.lblLocalDirectory.Location = new System.Drawing.Point(13, 21);
            this.lblLocalDirectory.Name = "lblLocalDirectory";
            this.lblLocalDirectory.Size = new System.Drawing.Size(154, 24);
            this.lblLocalDirectory.TabIndex = 2;
            this.lblLocalDirectory.Text = "本地文件夹：";
            // 
            // localListView
            // 
            this.localListView.AllowDrop = true;
            this.localListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localListView.FullRowSelect = true;
            this.localListView.Location = new System.Drawing.Point(0, 0);
            this.localListView.Name = "localListView";
            this.localListView.Size = new System.Drawing.Size(557, 540);
            this.localListView.TabIndex = 2;
            this.localListView.UseCompatibleStateImageBehavior = false;
            this.localListView.View = System.Windows.Forms.View.Details;
            this.localListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.localListView_ColumnClick);
            this.localListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.localListView_ItemDrag);
            this.localListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.localListView_DragDrop);
            this.localListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.localListView_DragEnter);
            this.localListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.localListView_MouseClick);
            this.localListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.localListView_MouseDoubleClick);
            // 
            // rightSplitContainer
            // 
            this.rightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplitContainer.IsSplitterFixed = true;
            this.rightSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.rightSplitContainer.Name = "rightSplitContainer";
            this.rightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightSplitContainer.Panel1
            // 
            this.rightSplitContainer.Panel1.Controls.Add(this.pnRemotePath);
            // 
            // rightSplitContainer.Panel2
            // 
            this.rightSplitContainer.Panel2.Controls.Add(this.remoteListView);
            this.rightSplitContainer.Size = new System.Drawing.Size(596, 603);
            this.rightSplitContainer.SplitterDistance = 59;
            this.rightSplitContainer.TabIndex = 1;
            // 
            // pnRemotePath
            // 
            this.pnRemotePath.Controls.Add(this.btnReconnect);
            this.pnRemotePath.Controls.Add(this.txtCurrentRemotePath);
            this.pnRemotePath.Controls.Add(this.lblRemoteDirectory);
            this.pnRemotePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnRemotePath.Location = new System.Drawing.Point(0, 0);
            this.pnRemotePath.Name = "pnRemotePath";
            this.pnRemotePath.Size = new System.Drawing.Size(596, 59);
            this.pnRemotePath.TabIndex = 0;
            this.pnRemotePath.SizeChanged += new System.EventHandler(this.pnRemotePath_SizeChanged);
            // 
            // btnReconnect
            // 
            this.btnReconnect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReconnect.Location = new System.Drawing.Point(491, 10);
            this.btnReconnect.Name = "btnReconnect";
            this.btnReconnect.Size = new System.Drawing.Size(95, 45);
            this.btnReconnect.TabIndex = 5;
            this.btnReconnect.Text = "重连";
            this.btnReconnect.UseVisualStyleBackColor = true;
            this.btnReconnect.Click += new System.EventHandler(this.btnReconnect_Click);
            // 
            // txtCurrentRemotePath
            // 
            this.txtCurrentRemotePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentRemotePath.Location = new System.Drawing.Point(173, 13);
            this.txtCurrentRemotePath.Name = "txtCurrentRemotePath";
            this.txtCurrentRemotePath.Size = new System.Drawing.Size(308, 35);
            this.txtCurrentRemotePath.TabIndex = 4;
            this.txtCurrentRemotePath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCurrentRemotePath_KeyUp);
            // 
            // lblRemoteDirectory
            // 
            this.lblRemoteDirectory.AutoSize = true;
            this.lblRemoteDirectory.Location = new System.Drawing.Point(13, 21);
            this.lblRemoteDirectory.Name = "lblRemoteDirectory";
            this.lblRemoteDirectory.Size = new System.Drawing.Size(154, 24);
            this.lblRemoteDirectory.TabIndex = 3;
            this.lblRemoteDirectory.Text = "远程文件夹：";
            // 
            // remoteListView
            // 
            this.remoteListView.AllowDrop = true;
            this.remoteListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remoteListView.FullRowSelect = true;
            this.remoteListView.Location = new System.Drawing.Point(0, 0);
            this.remoteListView.Name = "remoteListView";
            this.remoteListView.Size = new System.Drawing.Size(596, 540);
            this.remoteListView.TabIndex = 0;
            this.remoteListView.UseCompatibleStateImageBehavior = false;
            this.remoteListView.View = System.Windows.Forms.View.Details;
            this.remoteListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.remoteListView_ColumnClick);
            this.remoteListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.remoteListView_ItemDrag);
            this.remoteListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.remoteListView_DragDrop);
            this.remoteListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.remoteListView_DragEnter);
            this.remoteListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.remoteListView_MouseClick);
            this.remoteListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.remoteListView_MouseDoubleClick);
            // 
            // processListView
            // 
            this.processListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processListView.FullRowSelect = true;
            this.processListView.Location = new System.Drawing.Point(0, 0);
            this.processListView.Name = "processListView";
            this.processListView.Size = new System.Drawing.Size(1157, 185);
            this.processListView.TabIndex = 0;
            this.processListView.UseCompatibleStateImageBehavior = false;
            this.processListView.View = System.Windows.Forms.View.Details;
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
            this.fsSplitContainer.Panel1.ResumeLayout(false);
            this.fsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fsSplitContainer)).EndInit();
            this.fsSplitContainer.ResumeLayout(false);
            this.lrSplitContainer.Panel1.ResumeLayout(false);
            this.lrSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lrSplitContainer)).EndInit();
            this.lrSplitContainer.ResumeLayout(false);
            this.leftSplitContainer.Panel1.ResumeLayout(false);
            this.leftSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.leftSplitContainer)).EndInit();
            this.leftSplitContainer.ResumeLayout(false);
            this.pnlLocalPath.ResumeLayout(false);
            this.pnlLocalPath.PerformLayout();
            this.rightSplitContainer.Panel1.ResumeLayout(false);
            this.rightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplitContainer)).EndInit();
            this.rightSplitContainer.ResumeLayout(false);
            this.pnRemotePath.ResumeLayout(false);
            this.pnRemotePath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl sshTab;
        private System.Windows.Forms.TabPage shellTab;
        private System.Windows.Forms.TabPage sFtpTab;
        private System.Windows.Forms.SplitContainer fsSplitContainer;
        private System.Windows.Forms.SplitContainer lrSplitContainer;
        private System.Windows.Forms.ListView remoteListView;
        private System.Windows.Forms.ListView processListView;
        private System.Windows.Forms.SplitContainer leftSplitContainer;
        private System.Windows.Forms.ListView localListView;
        private System.Windows.Forms.Panel pnlLocalPath;
        private System.Windows.Forms.TextBox txtCurrentLocalPath;
        private System.Windows.Forms.Label lblLocalDirectory;
        private System.Windows.Forms.SplitContainer rightSplitContainer;
        private System.Windows.Forms.Panel pnRemotePath;
        private System.Windows.Forms.Label lblRemoteDirectory;
        private System.Windows.Forms.TextBox txtCurrentRemotePath;
        private System.Windows.Forms.Button btnReconnect;
    }
}
