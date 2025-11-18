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
    public partial class Form_ResetPassword : Form
    {
        private string resetEmail;

        public Form_ResetPassword()
        {
            InitializeComponent();
            SafeSetIcon();
        }

        public Form_ResetPassword(Point location, Size size)
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

        private void Button_ResetPass_Click(object sender, EventArgs e)
        {
            string newPassword = TextBox_Password.Text.Trim();
            string confirmPassword = TextBox_ConfirmPass.Text.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                TextBox_ConfirmPass.Clear();
                TextBox_ConfirmPass.Focus();
                return;
            }

            if (string.IsNullOrEmpty(resetEmail))
            {
                MessageBox.Show("Không tìm thấy email để reset password!");
                return;
            }

            try
            {
                TcpClient client = new TcpClient();
                client.Connect(ConfigHelper.GetServerIP(), ConfigHelper.GetServerPort());

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                Packet resetRequest = new Packet
                {
                    Code = 10,
                    ResetPasswordEmail = resetEmail,
                    ResetPasswordNewPassword = newPassword
                };

                string requestJson = JsonConvert.SerializeObject(resetRequest);
                writer.WriteLine(requestJson);
                writer.Flush();

                string responseJson = reader.ReadLine();
                Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);

                if (response.Success)
                {
                    MessageBox.Show("Ðặt lại mật khẩu thành công!");
                    client.Close();

                    Form_Login loginForm = new Form_Login(this.Location, this.Size);
                    loginForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage ?? "Không thể đặt lại mật khẩu!");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối server: " + ex.Message);
            }
        }
        private void Button_Back_Click(object sender, EventArgs e)
        {
            Form_VerifyOTP verifyOTPForm = new Form_VerifyOTP(this.Location, this.Size);
            verifyOTPForm.Show();
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
