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
    public partial class Form_VerifyOTP : Form
    {
        private string resetEmail;

        public Form_VerifyOTP()
        {
            InitializeComponent();
            SafeSetIcon();
        }

        public Form_VerifyOTP(Point location, Size size)
        {
            InitializeComponent();
            SafeSetIcon();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
            this.Size = size;
        }

        public void SetResetEmail(string email)
        {
            resetEmail = email;
        }

        private void Button_Send_Click(object sender, EventArgs e)
        {
            string otpCode = TextBox_OTP.Text.Trim();

            if (string.IsNullOrEmpty(otpCode))
            {
                MessageBox.Show("Please enter OTP code!");
                return;
            }

            if (otpCode.Length != 6)
            {
                MessageBox.Show("OTP code must be 6 digits!");
                return;
            }

            if (string.IsNullOrEmpty(resetEmail))
            {
                MessageBox.Show("Email not found!");
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

                // Create verify OTP packet - Code 9
                Packet verifyRequest = new Packet
                {
                    Code = 9,
                    VerifyOTPEmail = resetEmail,
                    VerifyOTPCode = otpCode
                };

                // Send to server
                string requestJson = JsonConvert.SerializeObject(verifyRequest);
                writer.WriteLine(requestJson);
                writer.Flush();

                // Receive response
                string responseJson = reader.ReadLine();
                Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);

                if (response.Success)
                {
                    MessageBox.Show("OTP verification successful!");
                    client.Close();

                    Form_ResetPassword resetForm = new Form_ResetPassword(this.Location, this.Size);
                    resetForm.SetResetEmail(resetEmail);
                    resetForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage ?? "OTP is incorrect!");
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
            Form_ForgotPassword forgotPasswordForm = new Form_ForgotPassword(this.Location, this.Size);
            forgotPasswordForm.Show();
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
