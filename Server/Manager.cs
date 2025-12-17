using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;

namespace Server
{
    internal class Manager
    {

        ListView Log;
        TextBox RoomCnt;
        TextBox UserCnt;



        public Manager(ListView log, TextBox room_count, TextBox user_count)
        {
            this.Log = log;
            this.RoomCnt = room_count;
            this.UserCnt = user_count;
        }



        public void WriteToLog(string line)
        {
            if (Log.InvokeRequired)
            {
                Log.Invoke(new Action(() =>
                {
                    Log.Items.Add(string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm"), line));
                }));
            }
            else
            {
                Log.Items.Add(string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm"), line));
            }
        }

        public void UpdateRoomCount(int num)
        {
            if (RoomCnt.InvokeRequired)
            {
                RoomCnt.Invoke(new Action(() =>
                {
                    RoomCnt.Text = num.ToString();
                }));
            }
            else
            {
                RoomCnt.Text = num.ToString();
            }
        }

        public void UpdateUserCount(int num)
        {
            if (UserCnt.InvokeRequired)
            {
                UserCnt.Invoke(new Action(() =>
                {
                    UserCnt.Text = num.ToString();
                }));
            }
            else
            {
                UserCnt.Text = num.ToString();
            }
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        public string GetLocalIPv4(NetworkInterfaceType type)
        {
            string localIPv4 = string.Empty;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localIPv4 = ip.Address.ToString();
                        }
                    }
                }
            }
            return localIPv4;
        }



        public async Task<string> GenerateMagicLink(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            var actionCodeSettings = new ActionCodeSettings
            {
                HandleCodeInApp = false,
                Url = "https://kayart-auth-nt106.web.app",
            };

            try
            {
                string magicLink = await FirebaseAuth.DefaultInstance.GenerateSignInWithEmailLinkAsync(
                    email,
                    actionCodeSettings
                );

                WriteToLog($"Created Magic Link for email: {email}");
                return magicLink;
            }
            catch (FirebaseAuthException ex)
            {
                WriteToLog($"Firebase Auth error: {ex.Message}");
                throw new Exception("Firebase authentication error: " + ex.ErrorCode);
            }
            catch (Exception ex)
            {
                WriteToLog($"General error creating Magic Link: {ex.Message}");
                throw;
            }
        }



        public async Task SendHybridEmail(string toEmail, string otpCode, string magicLinkUrl)
        {
            try
            {
                string SENDER_EMAIL = ConfigLoader.GetSenderEmail();
                string SENDER_PASSWORD = ConfigLoader.GetSenderPassword();
                string SMTP_HOST = ConfigLoader.GetSmtpHost();
                int SMTP_PORT = ConfigLoader.GetSmtpPort();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SENDER_EMAIL, "KayArt Support");
                    mail.To.Add(toEmail);
                    mail.Subject = "Verification Code & Password Recovery Link";
                    mail.Body = $@"
                    <div style='font-family: Helvetica, Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #4CAF50; text-align: center;'>Password Recovery Request</h2>
                        <p>Hello,</p>
                        <p>You have requested to reset your password for your KayArt account. Below is your OTP code:</p>
                        
                        <div style='text-align: center; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #333; background: #f4f4f4; padding: 10px 20px; border-radius: 5px;'>
                                {otpCode}
                            </span>
                        </div>

                        <p>You can click the secure link below:</p>
                        
                        <div style='text-align: center; margin: 20px 0;'>
                            <a href='{magicLinkUrl}' style='background-color: #008CBA; color: white; padding: 12px 25px; text-decoration: none; font-size: 16px; border-radius: 5px;'>
                                ➤ Magic Link
                            </a>
                        </div>

                        <hr style='border: 0; border-top: 1px solid #eee;' />
                        <p style='font-size: 12px; color: #888;'>This code will expire in 5 minutes. If you did not request this, please ignore this email.</p>
                    </div>";
                    mail.IsBodyHtml = true; 

                    using (SmtpClient smtp = new SmtpClient(SMTP_HOST, SMTP_PORT))
                    {
                        smtp.Credentials = new NetworkCredential(SENDER_EMAIL, SENDER_PASSWORD);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }

                WriteToLog($"Email sent to: {toEmail}");
            }
            catch (Exception ex)
            {
                WriteToLog($"Email error: {ex.Message}");
                throw new Exception("Email sending error: " + ex.Message);
            }
        }



        public async Task<bool> VerifyCaptcha(string captchaToken)
        {
            if (string.IsNullOrEmpty(captchaToken))
                return false;

            try
            {
                string secretKey = ConfigLoader.GetRecaptchaSecretKey();

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.PostAsync(
                        $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaToken}",

                        null
                    );

                    string jsonResult = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResult);

                    bool success = result.success == true;
                    WriteToLog($"reCAPTCHA verification: {(success ? "SUCCESS" : "FAILED")}");
                    return success;
                }
            }
            catch (Exception ex)
            {
                WriteToLog($"reCAPTCHA error: {ex.Message}");
                return false;
            }
        }

    }
}
