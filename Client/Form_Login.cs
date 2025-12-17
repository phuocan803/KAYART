using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Form_Login : Form
    {
        public Form_Login()
        {
            InitializeComponent();
            SafeSetIcon();
            
            TextBox_Acc.KeyDown += TextBox_KeyDown;
            TextBox_Pass.KeyDown += TextBox_KeyDown;
        }
        public Form_Login(Point location)
        {
            InitializeComponent();
            SafeSetIcon();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
            
            TextBox_Acc.KeyDown += TextBox_KeyDown;
            TextBox_Pass.KeyDown += TextBox_KeyDown;
        }
        public Form_Login(Point location, Size size)
        {
            InitializeComponent();
            SafeSetIcon();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = location;
            this.Size = size;
            
            TextBox_Acc.KeyDown += TextBox_KeyDown;
            TextBox_Pass.KeyDown += TextBox_KeyDown;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 
                Button_LOGIN_Click(sender, e); 
            }
        }

        private void Button_CreateAcc_Click(object sender, EventArgs e)
        {
            Form_SignUp signupForm = new Form_SignUp(this.Location, this.Size);
            signupForm.Show();
            this.Hide();
        }

        private void Link_ForgotPass_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form_ForgotPassword forgotForm = new Form_ForgotPassword(this.Location, this.Size);
            forgotForm.Show();
            this.Hide();
        }

        private void Button_LOGIN_Click(object sender, EventArgs e)
        {
            string acc = TextBox_Acc.Text.Trim();
            string pass = TextBox_Pass.Text.Trim();

            if (string.IsNullOrEmpty(acc) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter username and password!");
                return;
            }

            Form_Captcha captcha = new Form_Captcha();
            string token = "";
            if (captcha.ShowDialog() == DialogResult.OK)
            {
                token = captcha.CaptchaToken;
            }
            else
            {
                MessageBox.Show("Please verify Captcha!");
                return;
            }


            TcpClient client = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            try
            {
                client = new TcpClient();

                var result = client.BeginConnect(ConfigHelper.GetServerIP(), ConfigHelper.GetServerPort(), null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    throw new SocketException();
                }

                client.EndConnect(result);

                NetworkStream stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                Packet loginRequest = new Packet
                {
                    Code = 6,
                    LoginUsername = acc,
                    LoginPassword = pass,
                    CaptchaToken = token
                };

                string requestJson = JsonConvert.SerializeObject(loginRequest);
                writer.WriteLine(requestJson);
                writer.Flush();

                stream.ReadTimeout = 5000;
                string responseJson = reader.ReadLine();

                if (string.IsNullOrEmpty(responseJson))
                {
                    MessageBox.Show("Server not responding!\n\nCheck:\n1. Server is running\n2. Case 6 handler is implemented",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);

                if (response.Success)
                {
                    AuthManager.SaveToken(
                        response.JwtToken,
                        response.UserId,
                        response.LoginUsername,
                        response.Role,
                        response.TokenExpiry
                    );

                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form_Home homeForm = new Form_Home(acc);
                    homeForm.WindowState = FormWindowState.Maximized;
                    homeForm.Show();

                    this.Hide();

                    homeForm.FormClosed += (s, args) =>
                    {
                        this.Close();
                    };
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage ?? "Login failed!",
                        "Login Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    TextBox_Pass.Clear();
                    TextBox_Pass.Focus();
                }
            }
            catch (SocketException)
            {
                MessageBox.Show(
                    "Note: Server is required to use the application!",
                    "Server Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Data read/write error!\n\n" +
                    "Server may have closed the connection.\n" +
                    "Please check and try again.",
                    "I/O Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unidentified error!\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                try { reader?.Close(); } catch { }
                try { writer?.Close(); } catch { }
                try { client?.Close(); } catch { }
            }
        }
        
        private void Button_GUEST_Click(object sender, EventArgs e)
        {
            Form_Home homeForm = new Form_Home("Guest");
            homeForm.WindowState = FormWindowState.Maximized;
            homeForm.Show();
            this.Hide();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
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
