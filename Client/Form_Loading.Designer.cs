namespace Client
{
    partial class Form_Loading
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelMain = new System.Windows.Forms.Panel();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            lblVersion = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblProgress = new System.Windows.Forms.Label();
            lblLoading = new System.Windows.Forms.Label();
            lblTagline = new System.Windows.Forms.Label();
            lblAppName = new System.Windows.Forms.Label();
            timerLoading = new System.Windows.Forms.Timer(components);
            panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            panelMain.Controls.Add(pictureBox1);
            panelMain.Controls.Add(lblVersion);
            panelMain.Controls.Add(progressBar);
            panelMain.Controls.Add(lblProgress);
            panelMain.Controls.Add(lblLoading);
            panelMain.Controls.Add(lblTagline);
            panelMain.Controls.Add(lblAppName);
            panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            panelMain.Location = new System.Drawing.Point(0, 0);
            panelMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panelMain.Name = "panelMain";
            panelMain.Size = new System.Drawing.Size(571, 467);
            panelMain.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            pictureBox1.BackgroundImage = Properties.Resources.Logo;
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox1.Location = new System.Drawing.Point(236, 81);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(100, 120);
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblVersion.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            lblVersion.Location = new System.Drawing.Point(11, 433);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(46, 19);
            lblVersion.TabIndex = 6;
            lblVersion.Text = "v3.0.0";
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(114, 380);
            progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(343, 7);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar.TabIndex = 5;
            // 
            // lblProgress
            // 
            lblProgress.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblProgress.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            lblProgress.Location = new System.Drawing.Point(114, 393);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new System.Drawing.Size(343, 17);
            lblProgress.TabIndex = 3;
            lblProgress.Text = "Loading... 0%";
            lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoading
            // 
            lblLoading.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblLoading.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            lblLoading.Location = new System.Drawing.Point(114, 333);
            lblLoading.Name = "lblLoading";
            lblLoading.Size = new System.Drawing.Size(343, 31);
            lblLoading.TabIndex = 4;
            lblLoading.Text = "Loading";
            lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTagline
            // 
            lblTagline.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblTagline.ForeColor = System.Drawing.Color.FromArgb(156, 163, 175);
            lblTagline.Location = new System.Drawing.Point(114, 280);
            lblTagline.Name = "lblTagline";
            lblTagline.Size = new System.Drawing.Size(343, 31);
            lblTagline.TabIndex = 2;
            lblTagline.Text = "Collaborative Drawing Platform";
            lblTagline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAppName
            // 
            lblAppName.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            lblAppName.ForeColor = System.Drawing.Color.White;
            lblAppName.Location = new System.Drawing.Point(114, 227);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new System.Drawing.Size(343, 60);
            lblAppName.TabIndex = 1;
            lblAppName.Text = "KayArt";
            lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerLoading
            // 
            timerLoading.Interval = 500;
            timerLoading.Tick += timerLoading_Tick;
            // 
            // Form_Loading
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(571, 467);
            Controls.Add(panelMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Form_Loading";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "KayArt - Loading";
            Load += Form_Loading_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblTagline;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Timer timerLoading;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
