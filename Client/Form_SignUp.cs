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
    public partial class Form_SignUp : Form
    {
        public Form_SignUp()
        {
            InitializeComponent();
            SafeSetIcon();
        }

        public Form_SignUp(Point location, Size size)
        {
            InitializeComponent();
            SafeSetIcon();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
            this.Size = size;
        }

        private void Label_SignUp_Click(object sender, EventArgs e)
        {
            string fullName = TextBox_FullName.Text.Trim();
            string username = TextBox_Username.Text.Trim();
            string phone = TextBox_PhoneNumber.Text.Trim();
            string email = TextBox_Email.Text.Trim();
            string password = TextBox_Password.Text.Trim();
            string confirmPass = TextBox_ConfirmPass.Text.Trim();

            Form_Captcha captchaForm = new Form_Captcha();
            string token = "";

            if (captchaForm.ShowDialog() == DialogResult.OK)
            {
                token = captchaForm.CaptchaToken;
            }
            else
            {
                MessageBox.Show("Vui lòng xác thực Captcha!");
                return;
            }

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }

            if (password != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khợp!");
                TextBox_ConfirmPass.Clear();
                TextBox_ConfirmPass.Focus();
                return;
            }

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(ConfigHelper.GetServerIP(), ConfigHelper.GetServerPort());

                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        Packet signupRequest = new Packet
                        {
                            Code = 7,
                            SignUpUsername = username,
                            SignUpPassword = password,
                            SignUpEmail = email,
                            SignUpFullName = fullName,
                            SignUpPhoneNumber = phone,
                            CaptchaToken = token
                        };

                        string requestJson = JsonConvert.SerializeObject(signupRequest);
                        writer.WriteLine(requestJson);
                        writer.Flush();

                        string responseJson = reader.ReadLine();
                        Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);

                        if (response.Success)
                        {
                            MessageBox.Show("Ðang ký thành công!");

                            Form_Login loginForm = new Form_Login(this.Location, this.Size);
                            loginForm.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(response.ErrorMessage ?? "Ðăng ký thất bại!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối server: " + ex.Message);
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

        private void Label_Captcha_Click(object sender, EventArgs e)
        {

        }
    }
}
