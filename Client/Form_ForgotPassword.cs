using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace Client
{
    public partial class Form_ForgotPassword : Form
    {
        public Form_ForgotPassword()
        {
            InitializeComponent();
            SafeSetIcon();
        }

        public Form_ForgotPassword(Point location, Size size)
        {
            InitializeComponent();
            SafeSetIcon();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
            this.Size = size;
        }

        private void Button_SendOTP_Click(object sender, EventArgs e)
        {
            string email = TextBox_Email.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter email!");
                return;
            }

            Form_Captcha captchaForm = new Form_Captcha();
            string token = "";

            if (captchaForm.ShowDialog() == DialogResult.OK)
            {
                token = captchaForm.CaptchaToken;
            }
            else
            {
                MessageBox.Show("Please verify Captcha!");
                return;
            }
            try
            {
                // Connect to server via TCP
                TcpClient client = new TcpClient();
                client.Connect(ConfigHelper.GetServerIP(), ConfigHelper.GetServerPort());

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                // Create forgot password packet
                Packet forgotRequest = new Packet
                {
                    Code = 8,
                    ForgotPasswordEmail = email,
                    CaptchaToken = token
                };

                // Send to server
                string requestJson = JsonConvert.SerializeObject(forgotRequest);
                writer.WriteLine(requestJson);
                writer.Flush();

                // Receive response
                string responseJson = reader.ReadLine();
                Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);

                client.Close();

                if (response.Success)
                {
                    MessageBox.Show(
                        "A verification code (OTP) and login link (Magic Link) have been sent to your email.\n" +
                        "Please check your email inbox",
                        "Email Sent",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.Close();

                    if (response.Success)
                    {
                        MessageBox.Show("OTP code and verification link have been sent to your email.", "Notification");

                        // open verify otp form
                        Form_VerifyOTP verifyForm = new Form_VerifyOTP(this.Location, this.Size);
                        verifyForm.SetResetEmail(email); 
                        verifyForm.Show();

                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show(response.ErrorMessage);
                    }

                }
                else
                {
                    MessageBox.Show(response.ErrorMessage ?? "Email does not exist!");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server connection error: " + ex.Message);
            }
        }

        private void Button_Back_Click(object sender, EventArgs e)
        {
            Form_Login loginForm = new Form_Login(this.Location, this.Size);
            loginForm.Show();
            this.Hide();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
        private void SafeSetIcon()
        {
            try
            {
                var logo = Properties.Resources.Logo;
                if (logo is Bitmap bmp && bmp != null)
                {
                    IntPtr hIcon = IntPtr.Zero;
                    Icon tmpIcon = null;
                    try
                    {
                        hIcon = bmp.GetHicon();
                        tmpIcon = Icon.FromHandle(hIcon);
                        this.Icon = (Icon)tmpIcon.Clone();
                    }
                    finally
                    {
                        if (tmpIcon != null)
                        {
                            tmpIcon.Dispose();
                        }
                        if (hIcon != IntPtr.Zero)
                        {
                            DestroyIcon(hIcon);
                        }
                    }
                }
            }
            catch
            {
                // log check
            }

        }
    }
}
