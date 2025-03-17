namespace KastViewer.GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openDirectoryToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem = new ToolStripMenuItem();
            exportAllToolStripMenuItem = new ToolStripMenuItem();
            exportSelectedToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            openFolderBrowserDialog = new FolderBrowserDialog();
            splitContainer1 = new SplitContainer();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            pictureBox1 = new PictureBox();
            saveFolderBrowserDialog = new FolderBrowserDialog();
            exportBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, exportToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1264, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openDirectoryToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(39, 21);
            fileToolStripMenuItem.Text = "File";
            // 
            // openDirectoryToolStripMenuItem
            // 
            openDirectoryToolStripMenuItem.Name = "openDirectoryToolStripMenuItem";
            openDirectoryToolStripMenuItem.Size = new Size(165, 22);
            openDirectoryToolStripMenuItem.Text = "Open Directory";
            openDirectoryToolStripMenuItem.Click += openDirectoryToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportAllToolStripMenuItem, exportSelectedToolStripMenuItem });
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(58, 21);
            exportToolStripMenuItem.Text = "Export";
            // 
            // exportAllToolStripMenuItem
            // 
            exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            exportAllToolStripMenuItem.Size = new Size(167, 22);
            exportAllToolStripMenuItem.Text = "Export All";
            exportAllToolStripMenuItem.Click += exportAllToolStripMenuItem_Click;
            // 
            // exportSelectedToolStripMenuItem
            // 
            exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            exportSelectedToolStripMenuItem.Size = new Size(167, 22);
            exportSelectedToolStripMenuItem.Text = "Export Selected";
            exportSelectedToolStripMenuItem.Click += exportSelectedToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 659);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1264, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 17);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 25);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(pictureBox1);
            splitContainer1.Size = new Size(1264, 634);
            splitContainer1.SplitterDistance = 503;
            splitContainer1.TabIndex = 2;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(503, 634);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.KeyDown += listView1_KeyDown;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Size";
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Path";
            columnHeader3.Width = 500;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ControlDark;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(757, 634);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // exportBackgroundWorker
            // 
            exportBackgroundWorker.WorkerReportsProgress = true;
            exportBackgroundWorker.WorkerSupportsCancellation = true;
            exportBackgroundWorker.DoWork += exportBackgroundWorker_DoWork;
            exportBackgroundWorker.ProgressChanged += exportBackgroundWorker_ProgressChanged;
            exportBackgroundWorker.RunWorkerCompleted += exportBackgroundWorker_RunWorkerCompleted;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "KastViewer";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem openDirectoryToolStripMenuItem;
        private FolderBrowserDialog openFolderBrowserDialog;
        private SplitContainer splitContainer1;
        private ListView listView1;
        private PictureBox pictureBox1;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem exportAllToolStripMenuItem;
        private ToolStripMenuItem exportSelectedToolStripMenuItem;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private FolderBrowserDialog saveFolderBrowserDialog;
        private System.ComponentModel.BackgroundWorker exportBackgroundWorker;
    }
}
