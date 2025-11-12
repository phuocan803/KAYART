using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    partial class Form_SignUp
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
            Panel_SIGNUP = new Panel();
            Button_Undo = new Button();
            Label_ConfirmPass = new Label();
            TextBox_ConfirmPass = new TextBox();
            TextBox_Password = new TextBox();
            Label_PassW = new Label();
            Label_Email = new Label();
            TextBox_Email = new TextBox();
            TextBox_PhoneNumber = new TextBox();
            Label_PhongNumber = new Label();
            Label_SignIn = new Label();
            Label_Username = new Label();
            TextBox_Username = new TextBox();
            TextBox_FullName = new TextBox();
            Label_FullName = new Label();
            Button_SignIn = new Button();
            Panel_SIGNUP.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_SIGNUP
            // 
            Panel_SIGNUP.Anchor = AnchorStyles.None;
            Panel_SIGNUP.BackColor = Color.WhiteSmoke;
            Panel_SIGNUP.Controls.Add(Button_Undo);
            Panel_SIGNUP.Controls.Add(Label_ConfirmPass);
            Panel_SIGNUP.Controls.Add(TextBox_ConfirmPass);
            Panel_SIGNUP.Controls.Add(TextBox_Password);
            Panel_SIGNUP.Controls.Add(Label_PassW);
            Panel_SIGNUP.Controls.Add(Label_Email);
            Panel_SIGNUP.Controls.Add(TextBox_Email);
            Panel_SIGNUP.Controls.Add(TextBox_PhoneNumber);
            Panel_SIGNUP.Controls.Add(Label_PhongNumber);
            Panel_SIGNUP.Controls.Add(Label_SignIn);
            Panel_SIGNUP.Controls.Add(Label_Username);
            Panel_SIGNUP.Controls.Add(TextBox_Username);
            Panel_SIGNUP.Controls.Add(TextBox_FullName);
            Panel_SIGNUP.Controls.Add(Label_FullName);
            Panel_SIGNUP.Controls.Add(Button_SignIn);
            Panel_SIGNUP.Location = new Point(346, 31);
            Panel_SIGNUP.Margin = new Padding(3, 2, 3, 2);
            Panel_SIGNUP.Name = "Panel_SIGNUP";
            Panel_SIGNUP.Size = new Size(398, 630);
            Panel_SIGNUP.TabIndex = 6;
            // 
            // Button_Undo
            // 
            Button_Undo.Anchor = AnchorStyles.None;
            Button_Undo.BackColor = Color.White;
            Button_Undo.BackgroundImage = Properties.Resources.Back;
            Button_Undo.BackgroundImageLayout = ImageLayout.Stretch;
            Button_Undo.Cursor = Cursors.Hand;
            Button_Undo.FlatAppearance.BorderSize = 0;
            Button_Undo.FlatStyle = FlatStyle.Flat;
            Button_Undo.ForeColor = Color.White;
            Button_Undo.Location = new Point(8, 7);
            Button_Undo.Margin = new Padding(15, 0, 10, 0);
            Button_Undo.Name = "Button_Undo";
            Button_Undo.Size = new Size(35, 35);
            Button_Undo.TabIndex = 8;
            Button_Undo.Tag = "";
            Button_Undo.UseVisualStyleBackColor = false;
            Button_Undo.Click += Button_Back_Click;
            // 
            // Label_ConfirmPass
            // 
            Label_ConfirmPass.AutoSize = true;
            Label_ConfirmPass.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_ConfirmPass.ForeColor = Color.FromArgb(37, 37, 38);
            Label_ConfirmPass.Location = new Point(76, 451);
            Label_ConfirmPass.Name = "Label_ConfirmPass";
            Label_ConfirmPass.Size = new Size(157, 23);
            Label_ConfirmPass.TabIndex = 23;
            Label_ConfirmPass.Text = "Confirm password";
            // 
            // TextBox_ConfirmPass
            // 
            TextBox_ConfirmPass.Location = new Point(76, 484);
            TextBox_ConfirmPass.Margin = new Padding(3, 2, 3, 2);
            TextBox_ConfirmPass.Name = "TextBox_ConfirmPass";
            TextBox_ConfirmPass.PasswordChar = '*';
            TextBox_ConfirmPass.Size = new Size(252, 27);
            TextBox_ConfirmPass.TabIndex = 22;
            // 
            // TextBox_Password
            // 
            TextBox_Password.Location = new Point(76, 420);
            TextBox_Password.Margin = new Padding(3, 2, 3, 2);
            TextBox_Password.Name = "TextBox_Password";
            TextBox_Password.PasswordChar = '*';
            TextBox_Password.Size = new Size(252, 27);
            TextBox_Password.TabIndex = 21;
            // 
            // Label_PassW
            // 
            Label_PassW.AutoSize = true;
            Label_PassW.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_PassW.ForeColor = Color.FromArgb(37, 37, 38);
            Label_PassW.Location = new Point(76, 388);
            Label_PassW.Name = "Label_PassW";
            Label_PassW.Size = new Size(85, 23);
            Label_PassW.TabIndex = 20;
            Label_PassW.Text = "Password";
            // 
            // Label_Email
            // 
            Label_Email.AutoSize = true;
            Label_Email.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Email.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Email.Location = new Point(76, 324);
            Label_Email.Name = "Label_Email";
            Label_Email.Size = new Size(120, 23);
            Label_Email.TabIndex = 19;
            Label_Email.Text = "Email address";
            // 
            // TextBox_Email
            // 
            TextBox_Email.Location = new Point(76, 356);
            TextBox_Email.Margin = new Padding(3, 2, 3, 2);
            TextBox_Email.Name = "TextBox_Email";
            TextBox_Email.Size = new Size(252, 27);
            TextBox_Email.TabIndex = 18;
            // 
            // TextBox_PhoneNumber
            // 
            TextBox_PhoneNumber.Location = new Point(76, 292);
            TextBox_PhoneNumber.Margin = new Padding(3, 2, 3, 2);
            TextBox_PhoneNumber.Name = "TextBox_PhoneNumber";
            TextBox_PhoneNumber.Size = new Size(252, 27);
            TextBox_PhoneNumber.TabIndex = 17;
            // 
            // Label_PhongNumber
            // 
            Label_PhongNumber.AutoSize = true;
            Label_PhongNumber.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_PhongNumber.ForeColor = Color.FromArgb(37, 37, 38);
            Label_PhongNumber.Location = new Point(76, 260);
            Label_PhongNumber.Name = "Label_PhongNumber";
            Label_PhongNumber.Size = new Size(127, 23);
            Label_PhongNumber.TabIndex = 16;
            Label_PhongNumber.Text = "Phone number";
            // 
            // Label_SignIn
            // 
            Label_SignIn.AutoSize = true;
            Label_SignIn.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            Label_SignIn.ForeColor = Color.FromArgb(37, 37, 38);
            Label_SignIn.Location = new Point(72, 40);
            Label_SignIn.Name = "Label_SignIn";
            Label_SignIn.Size = new Size(142, 46);
            Label_SignIn.TabIndex = 15;
            Label_SignIn.Text = "Sign up";
            // 
            // Label_Username
            // 
            Label_Username.AutoSize = true;
            Label_Username.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_Username.ForeColor = Color.FromArgb(37, 37, 38);
            Label_Username.Location = new Point(76, 196);
            Label_Username.Name = "Label_Username";
            Label_Username.Size = new Size(89, 23);
            Label_Username.TabIndex = 14;
            Label_Username.Text = "Username";
            // 
            // TextBox_Username
            // 
            TextBox_Username.Location = new Point(76, 229);
            TextBox_Username.Margin = new Padding(3, 2, 3, 2);
            TextBox_Username.Name = "TextBox_Username";
            TextBox_Username.Size = new Size(252, 27);
            TextBox_Username.TabIndex = 13;
            // 
            // TextBox_FullName
            // 
            TextBox_FullName.Location = new Point(76, 165);
            TextBox_FullName.Margin = new Padding(3, 2, 3, 2);
            TextBox_FullName.Name = "TextBox_FullName";
            TextBox_FullName.Size = new Size(252, 27);
            TextBox_FullName.TabIndex = 12;
            // 
            // Label_FullName
            // 
            Label_FullName.AutoSize = true;
            Label_FullName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Label_FullName.ForeColor = Color.FromArgb(37, 37, 38);
            Label_FullName.Location = new Point(76, 132);
            Label_FullName.Name = "Label_FullName";
            Label_FullName.Size = new Size(88, 23);
            Label_FullName.TabIndex = 11;
            Label_FullName.Text = "Full name";
            // 
            // Button_SignIn
            // 
            Button_SignIn.BackColor = Color.FromArgb(37, 37, 38);
            Button_SignIn.Cursor = Cursors.Hand;
            Button_SignIn.DialogResult = DialogResult.Yes;
            Button_SignIn.FlatAppearance.BorderSize = 0;
            Button_SignIn.FlatStyle = FlatStyle.Flat;
            Button_SignIn.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            Button_SignIn.ForeColor = Color.White;
            Button_SignIn.Location = new Point(135, 539);
            Button_SignIn.Margin = new Padding(3, 2, 3, 2);
            Button_SignIn.Name = "Button_SignIn";
            Button_SignIn.RightToLeft = RightToLeft.Yes;
            Button_SignIn.Size = new Size(118, 45);
            Button_SignIn.TabIndex = 10;
            Button_SignIn.Text = "SIGN UP";
            Button_SignIn.UseCompatibleTextRendering = true;
            Button_SignIn.UseVisualStyleBackColor = false;
            Button_SignIn.Click += Label_SignUp_Click;
            // 
            // Form_SignUp
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.CornflowerBlue;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1077, 735);
            Controls.Add(Panel_SIGNUP);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Form_SignUp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KayArt";
            Panel_SIGNUP.ResumeLayout(false);
            Panel_SIGNUP.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private Panel Panel_SIGNUP;
        private Label Label_ConfirmPass;
        private TextBox TextBox_ConfirmPass;
        private TextBox TextBox_Password;
        private Label Label_PassW;
        private Label Label_Email;
        private TextBox TextBox_Email;
        private TextBox TextBox_PhoneNumber;
        private Label Label_PhongNumber;
        private Label Label_SignIn;
        private Label Label_Username;
        private TextBox TextBox_Username;
        private TextBox TextBox_FullName;
        private Label Label_FullName;
        private Button Button_SignIn;
        private Button Button_Undo;
    }
}