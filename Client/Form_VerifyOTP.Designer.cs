using System.Drawing;

namespace Client
{
    partial class Form_VerifyOTP
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
            Panel_LOGIN = new System.Windows.Forms.Panel();
            Button_Undo = new System.Windows.Forms.Button();
            Button_Send = new System.Windows.Forms.Button();
            TextBox_OTP = new System.Windows.Forms.TextBox();
            Label_Verification = new System.Windows.Forms.Label();
            Label_OPT = new System.Windows.Forms.Label();
            Panel_LOGIN.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_LOGIN
            // 
            Panel_LOGIN.Anchor = System.Windows.Forms.AnchorStyles.None;
            Panel_LOGIN.BackColor = Color.White;
            Panel_LOGIN.Controls.Add(Button_Undo);
            Panel_LOGIN.Controls.Add(Button_Send);
            Panel_LOGIN.Controls.Add(TextBox_OTP);
            Panel_LOGIN.Controls.Add(Label_Verification);
            Panel_LOGIN.Controls.Add(Label_OPT);
            Panel_LOGIN.Location = new Point(227, 139);
            Panel_LOGIN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Panel_LOGIN.Name = "Panel_LOGIN";
            Panel_LOGIN.Size = new Size(354, 267);
            Panel_LOGIN.TabIndex = 7;
            // 
            // Button_Undo
            // 
            Button_Undo.Anchor = System.Windows.Forms.AnchorStyles.None;
            Button_Undo.BackColor = Color.White;
            Button_Undo.BackgroundImage = Properties.Resources.Back;
            Button_Undo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Button_Undo.Cursor = System.Windows.Forms.Cursors.Hand;
            Button_Undo.FlatAppearance.BorderSize = 0;
            Button_Undo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Button_Undo.ForeColor = Color.White;
            Button_Undo.Location = new Point(7, 8);
            Button_Undo.Margin = new System.Windows.Forms.Padding(15, 0, 10, 0);
            Button_Undo.Name = "Button_Undo";
            Button_Undo.Size = new Size(35, 35);
            Button_Undo.TabIndex = 9;
            Button_Undo.Tag = "";
            Button_Undo.UseVisualStyleBackColor = false;
            Button_Undo.Click += Button_Back_Click;
            // 
            // Button_Send
            // 
            Button_Send.Anchor = System.Windows.Forms.AnchorStyles.None;
            Button_Send.BackColor = Color.FromArgb(37, 37, 38);
            Button_Send.Cursor = System.Windows.Forms.Cursors.Hand;
            Button_Send.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Button_Send.FlatAppearance.BorderSize = 0;
            Button_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Button_Send.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_Send.ForeColor = Color.White;
            Button_Send.Location = new Point(120, 189);
            Button_Send.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Button_Send.Name = "Button_Send";
            Button_Send.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Button_Send.Size = new Size(118, 45);
            Button_Send.TabIndex = 25;
            Button_Send.Text = "SEND";
            Button_Send.UseCompatibleTextRendering = true;
            Button_Send.UseVisualStyleBackColor = false;
            Button_Send.Click += Button_Send_Click;
            // 
            // TextBox_OTP
            // 
            TextBox_OTP.Anchor = System.Windows.Forms.AnchorStyles.None;
            TextBox_OTP.Location = new Point(54, 142);
            TextBox_OTP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            TextBox_OTP.Name = "TextBox_OTP";
            TextBox_OTP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            TextBox_OTP.Size = new Size(252, 27);
            TextBox_OTP.TabIndex = 27;
            // 
            // Label_Verification
            // 
            Label_Verification.AutoSize = true;
            Label_Verification.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            Label_Verification.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Verification.Location = new Point(42, 46);
            Label_Verification.Name = "Label_Verification";
            Label_Verification.Size = new Size(205, 46);
            Label_Verification.TabIndex = 30;
            Label_Verification.Text = "Verification";
            // 
            // Label_OPT
            // 
            Label_OPT.Anchor = System.Windows.Forms.AnchorStyles.None;
            Label_OPT.AutoSize = true;
            Label_OPT.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_OPT.ForeColor = Color.FromArgb(37, 37, 38);
            Label_OPT.Location = new Point(50, 106);
            Label_OPT.Name = "Label_OPT";
            Label_OPT.Size = new Size(42, 23);
            Label_OPT.TabIndex = 26;
            Label_OPT.Text = "OTP";
            // 
            // Form_VerifyOTP
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new Size(800, 589);
            Controls.Add(Panel_LOGIN);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form_VerifyOTP";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "KayArt";
            Panel_LOGIN.ResumeLayout(false);
            Panel_LOGIN.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_LOGIN;
        private System.Windows.Forms.Button Button_Send;
        private System.Windows.Forms.TextBox TextBox_OTP;
        private System.Windows.Forms.Label Label_Verification;
        private System.Windows.Forms.Label Label_OPT;
        private System.Windows.Forms.Button Button_Undo;
    }
}