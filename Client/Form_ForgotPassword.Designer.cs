using System.Drawing;

namespace Client
{
    partial class Form_ForgotPassword
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
            Panel_ForgotPass = new System.Windows.Forms.Panel();
            Button_Undo = new System.Windows.Forms.Button();
            Label_ForgotPass = new System.Windows.Forms.Label();
            TextBox_Email = new System.Windows.Forms.TextBox();
            Label_Email = new System.Windows.Forms.Label();
            Button_SendOTP = new System.Windows.Forms.Button();
            Panel_ForgotPass.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_ForgotPass
            // 
            Panel_ForgotPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            Panel_ForgotPass.BackColor = Color.WhiteSmoke;
            Panel_ForgotPass.Controls.Add(Button_Undo);
            Panel_ForgotPass.Controls.Add(Label_ForgotPass);
            Panel_ForgotPass.Controls.Add(TextBox_Email);
            Panel_ForgotPass.Controls.Add(Label_Email);
            Panel_ForgotPass.Controls.Add(Button_SendOTP);
            Panel_ForgotPass.Location = new Point(286, 170);
            Panel_ForgotPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Panel_ForgotPass.Name = "Panel_ForgotPass";
            Panel_ForgotPass.Size = new Size(419, 310);
            Panel_ForgotPass.TabIndex = 7;
            // 
            // Button_Undo
            // 
            Button_Undo.BackColor = Color.White;
            Button_Undo.BackgroundImage = Properties.Resources.Back;
            Button_Undo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Button_Undo.Cursor = System.Windows.Forms.Cursors.Hand;
            Button_Undo.FlatAppearance.BorderSize = 0;
            Button_Undo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Button_Undo.ForeColor = Color.White;
            Button_Undo.Location = new Point(6, 7);
            Button_Undo.Margin = new System.Windows.Forms.Padding(15, 0, 10, 0);
            Button_Undo.Name = "Button_Undo";
            Button_Undo.Size = new Size(35, 35);
            Button_Undo.TabIndex = 9;
            Button_Undo.Tag = "";
            Button_Undo.UseVisualStyleBackColor = false;
            Button_Undo.Click += Button_Back_Click;
            // 
            // Label_ForgotPass
            // 
            Label_ForgotPass.AutoSize = true;
            Label_ForgotPass.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            Label_ForgotPass.ForeColor = Color.FromArgb(37, 37, 38);
            Label_ForgotPass.Location = new Point(68, 48);
            Label_ForgotPass.Name = "Label_ForgotPass";
            Label_ForgotPass.Size = new Size(288, 46);
            Label_ForgotPass.TabIndex = 15;
            Label_ForgotPass.Text = "Forgot Password";
            // 
            // TextBox_Email
            // 
            TextBox_Email.Location = new Point(83, 170);
            TextBox_Email.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            TextBox_Email.Name = "TextBox_Email";
            TextBox_Email.Size = new Size(252, 27);
            TextBox_Email.TabIndex = 12;
            // 
            // Label_Email
            // 
            Label_Email.AutoSize = true;
            Label_Email.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Email.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Email.Location = new Point(83, 138);
            Label_Email.Name = "Label_Email";
            Label_Email.Size = new Size(54, 23);
            Label_Email.TabIndex = 11;
            Label_Email.Text = "Email";
            // 
            // Button_SendOTP
            // 
            Button_SendOTP.BackColor = Color.FromArgb(37, 37, 38);
            Button_SendOTP.Cursor = System.Windows.Forms.Cursors.Hand;
            Button_SendOTP.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Button_SendOTP.FlatAppearance.BorderSize = 0;
            Button_SendOTP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Button_SendOTP.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_SendOTP.ForeColor = Color.White;
            Button_SendOTP.Location = new Point(138, 229);
            Button_SendOTP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Button_SendOTP.Name = "Button_SendOTP";
            Button_SendOTP.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Button_SendOTP.Size = new Size(135, 45);
            Button_SendOTP.TabIndex = 10;
            Button_SendOTP.Text = "SEND OTP";
            Button_SendOTP.UseCompatibleTextRendering = true;
            Button_SendOTP.UseVisualStyleBackColor = false;
            Button_SendOTP.Click += Button_SendOTP_Click;
            // 
            // Form_ForgotPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new Size(994, 778);
            Controls.Add(Panel_ForgotPass);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form_ForgotPassword";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "KayArt";
            Panel_ForgotPass.ResumeLayout(false);
            Panel_ForgotPass.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_ForgotPass;
        private System.Windows.Forms.Label Label_ForgotPass;
        private System.Windows.Forms.TextBox TextBox_Email;
        private System.Windows.Forms.Label Label_Email;
        private System.Windows.Forms.Button Button_SendOTP;
        private System.Windows.Forms.Button Button_Undo;
    }
}