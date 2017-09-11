namespace RDManager
{
    partial class SFTPForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FSSplitContainer = new System.Windows.Forms.SplitContainer();
            this.LRSplitContainer = new System.Windows.Forms.SplitContainer();
            this.localList = new System.Windows.Forms.ListView();
            this.remoteList = new System.Windows.Forms.ListView();
            this.statusList = new System.Windows.Forms.ListView();
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
            // FSSplitContainer
            // 
            this.FSSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FSSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.FSSplitContainer.Name = "FSSplitContainer";
            this.FSSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // FSSplitContainer.Panel1
            // 
            this.FSSplitContainer.Panel1.Controls.Add(this.LRSplitContainer);
            // 
            // FSSplitContainer.Panel2
            // 
            this.FSSplitContainer.Panel2.Controls.Add(this.statusList);
            this.FSSplitContainer.Size = new System.Drawing.Size(998, 697);
            this.FSSplitContainer.SplitterDistance = 510;
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
            this.LRSplitContainer.Panel1.Controls.Add(this.localList);
            this.LRSplitContainer.Panel1MinSize = 100;
            // 
            // LRSplitContainer.Panel2
            // 
            this.LRSplitContainer.Panel2.Controls.Add(this.remoteList);
            this.LRSplitContainer.Panel2MinSize = 100;
            this.LRSplitContainer.Size = new System.Drawing.Size(998, 510);
            this.LRSplitContainer.SplitterDistance = 494;
            this.LRSplitContainer.TabIndex = 0;
            // 
            // localList
            // 
            this.localList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localList.Location = new System.Drawing.Point(0, 0);
            this.localList.Name = "localList";
            this.localList.Size = new System.Drawing.Size(494, 510);
            this.localList.TabIndex = 0;
            this.localList.UseCompatibleStateImageBehavior = false;
            // 
            // remoteList
            // 
            this.remoteList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remoteList.Location = new System.Drawing.Point(0, 0);
            this.remoteList.Name = "remoteList";
            this.remoteList.Size = new System.Drawing.Size(500, 510);
            this.remoteList.TabIndex = 0;
            this.remoteList.UseCompatibleStateImageBehavior = false;
            // 
            // statusList
            // 
            this.statusList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusList.Location = new System.Drawing.Point(0, 0);
            this.statusList.Name = "statusList";
            this.statusList.Size = new System.Drawing.Size(998, 183);
            this.statusList.TabIndex = 0;
            this.statusList.UseCompatibleStateImageBehavior = false;
            // 
            // SFTPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 697);
            this.Controls.Add(this.FSSplitContainer);
            this.Name = "SFTPForm";
            this.Text = "文件传输（SFTP）";
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

        private System.Windows.Forms.SplitContainer FSSplitContainer;
        private System.Windows.Forms.SplitContainer LRSplitContainer;
        private System.Windows.Forms.ListView localList;
        private System.Windows.Forms.ListView remoteList;
        private System.Windows.Forms.ListView statusList;
    }
}