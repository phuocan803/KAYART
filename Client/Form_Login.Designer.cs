using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    partial class Form_Login
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
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolTip1 = new ToolTip(components);
            Panel_LOGIN = new Panel();
            button1 = new Button();
            Button_CreateAcc = new Button();
            Button_LOGIN = new Button();
            TextBox_Pass = new TextBox();
            TextBox_Acc = new TextBox();
            Label_OR = new Label();
            LinkLb_ForgotPass = new LinkLabel();
            Label_Login = new Label();
            Label_Acc = new Label();
            Label_Pass = new Label();
            Panel_LOGIN.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // Panel_LOGIN
            // 
            Panel_LOGIN.Anchor = AnchorStyles.None;
            Panel_LOGIN.BackColor = Color.White;
            Panel_LOGIN.Controls.Add(button1);
            Panel_LOGIN.Controls.Add(Button_CreateAcc);
            Panel_LOGIN.Controls.Add(Button_LOGIN);
            Panel_LOGIN.Controls.Add(TextBox_Pass);
            Panel_LOGIN.Controls.Add(TextBox_Acc);
            Panel_LOGIN.Controls.Add(Label_OR);
            Panel_LOGIN.Controls.Add(LinkLb_ForgotPass);
            Panel_LOGIN.Controls.Add(Label_Login);
            Panel_LOGIN.Controls.Add(Label_Acc);
            Panel_LOGIN.Controls.Add(Label_Pass);
            Panel_LOGIN.Location = new Point(402, 118);
            Panel_LOGIN.Margin = new Padding(3, 2, 3, 2);
            Panel_LOGIN.Name = "Panel_LOGIN";
            Panel_LOGIN.Size = new Size(381, 543);
            Panel_LOGIN.TabIndex = 6;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.None;
            button1.BackColor = Color.FromArgb(37, 37, 38);
            button1.Cursor = Cursors.Hand;
            button1.DialogResult = DialogResult.Yes;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            button1.ForeColor = Color.White;
            button1.Location = new Point(137, 463);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.RightToLeft = RightToLeft.Yes;
            button1.Size = new Size(115, 40);
            button1.TabIndex = 35;
            button1.Text = "GUEST";
            button1.UseCompatibleTextRendering = true;
            button1.UseVisualStyleBackColor = false;
            button1.Click += Button_GUEST_Click;
            // 
            // Button_CreateAcc
            // 
            Button_CreateAcc.Anchor = AnchorStyles.None;
            Button_CreateAcc.BackColor = Color.FromArgb(37, 37, 38);
            Button_CreateAcc.Cursor = Cursors.Hand;
            Button_CreateAcc.DialogResult = DialogResult.Yes;
            Button_CreateAcc.FlatStyle = FlatStyle.Popup;
            Button_CreateAcc.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_CreateAcc.ForeColor = Color.White;
            Button_CreateAcc.Location = new Point(119, 380);
            Button_CreateAcc.Margin = new Padding(3, 2, 3, 2);
            Button_CreateAcc.Name = "Button_CreateAcc";
            Button_CreateAcc.RightToLeft = RightToLeft.Yes;
            Button_CreateAcc.Size = new Size(150, 35);
            Button_CreateAcc.TabIndex = 34;
            Button_CreateAcc.Text = "Create Account";
            Button_CreateAcc.UseCompatibleTextRendering = true;
            Button_CreateAcc.UseVisualStyleBackColor = false;
            Button_CreateAcc.Click += Button_CreateAcc_Click;
            // 
            // Button_LOGIN
            // 
            Button_LOGIN.Anchor = AnchorStyles.None;
            Button_LOGIN.BackColor = Color.FromArgb(37, 37, 38);
            Button_LOGIN.Cursor = Cursors.Hand;
            Button_LOGIN.DialogResult = DialogResult.Yes;
            Button_LOGIN.FlatStyle = FlatStyle.Popup;
            Button_LOGIN.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_LOGIN.ForeColor = Color.White;
            Button_LOGIN.Location = new Point(119, 318);
            Button_LOGIN.Margin = new Padding(3, 2, 3, 2);
            Button_LOGIN.Name = "Button_LOGIN";
            Button_LOGIN.RightToLeft = RightToLeft.Yes;
            Button_LOGIN.Size = new Size(150, 35);
            Button_LOGIN.TabIndex = 25;
            Button_LOGIN.Text = "LOGIN";
            Button_LOGIN.UseCompatibleTextRendering = true;
            Button_LOGIN.UseVisualStyleBackColor = false;
            Button_LOGIN.Click += Button_LOGIN_Click;
            // 
            // TextBox_Pass
            // 
            TextBox_Pass.Anchor = AnchorStyles.None;
            TextBox_Pass.Location = new Point(67, 214);
            TextBox_Pass.Margin = new Padding(3, 2, 3, 2);
            TextBox_Pass.Name = "TextBox_Pass";
            TextBox_Pass.PasswordChar = '*';
            TextBox_Pass.Size = new Size(252, 27);
            TextBox_Pass.TabIndex = 28;
            // 
            // TextBox_Acc
            // 
            TextBox_Acc.Anchor = AnchorStyles.None;
            TextBox_Acc.Location = new Point(68, 146);
            TextBox_Acc.Margin = new Padding(3, 2, 3, 2);
            TextBox_Acc.Name = "TextBox_Acc";
            TextBox_Acc.RightToLeft = RightToLeft.No;
            TextBox_Acc.Size = new Size(252, 27);
            TextBox_Acc.TabIndex = 27;
            // 
            // Label_OR
            // 
            Label_OR.Anchor = AnchorStyles.None;
            Label_OR.ForeColor = SystemColors.ControlDarkDark;
            Label_OR.Location = new Point(62, 428);
            Label_OR.Name = "Label_OR";
            Label_OR.Size = new Size(265, 20);
            Label_OR.TabIndex = 33;
            Label_OR.Text = "_______________OR_______________";
            Label_OR.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LinkLb_ForgotPass
            // 
            LinkLb_ForgotPass.Anchor = AnchorStyles.None;
            LinkLb_ForgotPass.AutoSize = true;
            LinkLb_ForgotPass.Cursor = Cursors.Hand;
            LinkLb_ForgotPass.LinkColor = SystemColors.ControlDarkDark;
            LinkLb_ForgotPass.Location = new Point(65, 261);
            LinkLb_ForgotPass.Name = "LinkLb_ForgotPass";
            LinkLb_ForgotPass.Size = new Size(129, 20);
            LinkLb_ForgotPass.TabIndex = 32;
            LinkLb_ForgotPass.TabStop = true;
            LinkLb_ForgotPass.Text = "Forgot Password ?";
            LinkLb_ForgotPass.LinkClicked += Link_ForgotPass_Click;
            // 
            // Label_Login
            // 
            Label_Login.AutoSize = true;
            Label_Login.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            Label_Login.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Login.Location = new Point(55, 39);
            Label_Login.Name = "Label_Login";
            Label_Login.Size = new Size(110, 46);
            Label_Login.TabIndex = 30;
            Label_Login.Text = "Login";
            // 
            // Label_Acc
            // 
            Label_Acc.Anchor = AnchorStyles.None;
            Label_Acc.AutoSize = true;
            Label_Acc.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Acc.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Acc.Location = new Point(64, 110);
            Label_Acc.Name = "Label_Acc";
            Label_Acc.Size = new Size(75, 23);
            Label_Acc.TabIndex = 26;
            Label_Acc.Text = "Account";
            // 
            // Label_Pass
            // 
            Label_Pass.Anchor = AnchorStyles.None;
            Label_Pass.AutoSize = true;
            Label_Pass.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Pass.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Pass.Location = new Point(64, 179);
            Label_Pass.Name = "Label_Pass";
            Label_Pass.Size = new Size(78, 23);
            Label_Pass.TabIndex = 29;
            Label_Pass.Text = "Pasword";
            // 
            // Form_Login
            // 
            AcceptButton = Button_LOGIN;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.CornflowerBlue;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1182, 803);
            Controls.Add(Panel_LOGIN);
            ForeColor = SystemColors.Desktop;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Form_Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KayArt";
            Panel_LOGIN.ResumeLayout(false);
            Panel_LOGIN.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip contextMenuStrip1;
        private ToolTip toolTip1;
        private Panel Panel_LOGIN;
        private Button Button_CreateAcc;
        private Button Button_LOGIN;
        private TextBox TextBox_Pass;
        private TextBox TextBox_Acc;
        private Label Label_OR;
        private LinkLabel LinkLb_ForgotPass;
        private Label Label_Login;
        private Label Label_Acc;
        private Label Label_Pass;
        private Button button1;
    }
}
