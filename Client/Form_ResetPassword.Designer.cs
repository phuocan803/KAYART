using System.Drawing;

namespace Client
{
    partial class Form_ResetPassword
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
            Panel_ResetPass = new System.Windows.Forms.Panel();
            Button_Undo = new System.Windows.Forms.Button();
            Button_ResetPass = new System.Windows.Forms.Button();
            TextBox_ConfirmPass = new System.Windows.Forms.TextBox();
            TextBox_Password = new System.Windows.Forms.TextBox();
            Label_ResetPass = new System.Windows.Forms.Label();
            Label_Pasword = new System.Windows.Forms.Label();
            Label_ConfirmPass = new System.Windows.Forms.Label();
            Panel_ResetPass.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_ResetPass
            // 
            Panel_ResetPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            Panel_ResetPass.BackColor = Color.White;
            Panel_ResetPass.Controls.Add(Button_Undo);
            Panel_ResetPass.Controls.Add(Button_ResetPass);
            Panel_ResetPass.Controls.Add(TextBox_ConfirmPass);
            Panel_ResetPass.Controls.Add(TextBox_Password);
            Panel_ResetPass.Controls.Add(Label_ResetPass);
            Panel_ResetPass.Controls.Add(Label_Pasword);
            Panel_ResetPass.Controls.Add(Label_ConfirmPass);
            Panel_ResetPass.Location = new Point(304, 172);
            Panel_ResetPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Panel_ResetPass.Name = "Panel_ResetPass";
            Panel_ResetPass.Size = new Size(381, 396);
            Panel_ResetPass.TabIndex = 7;
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
            Button_Undo.Location = new Point(8, 7);
            Button_Undo.Margin = new System.Windows.Forms.Padding(15, 0, 10, 0);
            Button_Undo.Name = "Button_Undo";
            Button_Undo.Size = new Size(35, 35);
            Button_Undo.TabIndex = 9;
            Button_Undo.Tag = "";
            Button_Undo.UseVisualStyleBackColor = false;
            // 
            // Button_ResetPass
            // 
            Button_ResetPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            Button_ResetPass.BackColor = Color.FromArgb(37, 37, 38);
            Button_ResetPass.Cursor = System.Windows.Forms.Cursors.Hand;
            Button_ResetPass.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Button_ResetPass.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_ResetPass.ForeColor = Color.White;
            Button_ResetPass.Location = new Point(100, 310);
            Button_ResetPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Button_ResetPass.Name = "Button_ResetPass";
            Button_ResetPass.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Button_ResetPass.Size = new Size(174, 45);
            Button_ResetPass.TabIndex = 25;
            Button_ResetPass.Text = "Reset Password";
            Button_ResetPass.UseCompatibleTextRendering = true;
            Button_ResetPass.UseVisualStyleBackColor = false;
            Button_ResetPass.Click += Button_ResetPass_Click;
            // 
            // TextBox_ConfirmPass
            // 
            TextBox_ConfirmPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            TextBox_ConfirmPass.Location = new Point(63, 250);
            TextBox_ConfirmPass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            TextBox_ConfirmPass.Name = "TextBox_ConfirmPass";
            TextBox_ConfirmPass.PasswordChar = '*';
            TextBox_ConfirmPass.Size = new Size(252, 27);
            TextBox_ConfirmPass.TabIndex = 28;
            // 
            // TextBox_Password
            // 
            TextBox_Password.Anchor = System.Windows.Forms.AnchorStyles.None;
            TextBox_Password.Location = new Point(64, 174);
            TextBox_Password.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            TextBox_Password.Name = "TextBox_Password";
            TextBox_Password.PasswordChar = '*';
            TextBox_Password.RightToLeft = System.Windows.Forms.RightToLeft.No;
            TextBox_Password.Size = new Size(252, 27);
            TextBox_Password.TabIndex = 27;
            // 
            // Label_ResetPass
            // 
            Label_ResetPass.AutoSize = true;
            Label_ResetPass.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            Label_ResetPass.ForeColor = Color.FromArgb(37, 37, 38);
            Label_ResetPass.Location = new Point(55, 39);
            Label_ResetPass.Name = "Label_ResetPass";
            Label_ResetPass.Size = new Size(265, 46);
            Label_ResetPass.TabIndex = 30;
            Label_ResetPass.Text = "Reset Password";
            // 
            // Label_Pasword
            // 
            Label_Pasword.Anchor = System.Windows.Forms.AnchorStyles.None;
            Label_Pasword.AutoSize = true;
            Label_Pasword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Pasword.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Pasword.Location = new Point(60, 139);
            Label_Pasword.Name = "Label_Pasword";
            Label_Pasword.Size = new Size(78, 23);
            Label_Pasword.TabIndex = 26;
            Label_Pasword.Text = "Pasword";
            // 
            // Label_ConfirmPass
            // 
            Label_ConfirmPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            Label_ConfirmPass.AutoSize = true;
            Label_ConfirmPass.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_ConfirmPass.ForeColor = Color.FromArgb(37, 37, 38);
            Label_ConfirmPass.Location = new Point(60, 215);
            Label_ConfirmPass.Name = "Label_ConfirmPass";
            Label_ConfirmPass.Size = new Size(149, 23);
            Label_ConfirmPass.TabIndex = 29;
            Label_ConfirmPass.Text = "Confirm Pasword";
            // 
            // Form_ResetPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new Size(985, 739);
            Controls.Add(Panel_ResetPass);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form_ResetPassword";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "KayArt";
            Panel_ResetPass.ResumeLayout(false);
            Panel_ResetPass.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_ResetPass;
        private System.Windows.Forms.Button Button_ResetPass;
        private System.Windows.Forms.TextBox TextBox_ConfirmPass;
        private System.Windows.Forms.TextBox TextBox_Password;
        private System.Windows.Forms.Label Label_ResetPass;
        private System.Windows.Forms.Label Label_Pasword;
        private System.Windows.Forms.Label Label_ConfirmPass;
        private System.Windows.Forms.Button Button_Undo;
    }
}