using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form_Captcha : Form
    {
        public string CaptchaToken { get; private set; } = null;
        private string recaptchaSiteKey = string.Empty;
        private bool shouldInitWebView = true;

        public Form_Captcha()
        {
            InitializeComponent();
            LoadConfiguration();
            
            if (shouldInitWebView && !string.IsNullOrEmpty(recaptchaSiteKey))
            {
                InitWebView();
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                recaptchaSiteKey = ConfigHelper.GetReCaptchaSiteKey();
                
                if (string.IsNullOrEmpty(recaptchaSiteKey))
                {
                    
                    shouldInitWebView = false;
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
                
            }
            catch (Exception ex)
            {
                shouldInitWebView = false;
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private async void InitWebView()
        {
            try
            {
                await webView21.EnsureCoreWebView2Async(null);

                string appPath = Application.StartupPath;

                string htmlPath = Path.Combine(appPath, "captcha.html");
                
                if (!File.Exists(htmlPath))
                {
                    MessageBox.Show(
                        $"captcha.html not found!\n\nExpected path: {htmlPath}\n\n" +
                        "Make sure captcha.html is set to 'Copy to Output Directory' in project properties.",
                        "Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error
                    );
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                string htmlContent = File.ReadAllText(htmlPath);
                
                
                htmlContent = htmlContent.Replace("{{RECAPTCHA_SITE_KEY}}", recaptchaSiteKey);
                
                
                webView21.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "localhost",
                    appPath,
                    CoreWebView2HostResourceAccessKind.Allow
                );

                string tempHtmlPath = Path.Combine(appPath, "captcha_temp.html");
                File.WriteAllText(tempHtmlPath, htmlContent);
                
                webView21.Source = new Uri("http://localhost/captcha_temp.html");

                webView21.WebMessageReceived += WebView21_WebMessageReceived;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing reCAPTCHA:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void WebView21_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string token = e.TryGetWebMessageAsString();

                if (!string.IsNullOrEmpty(token))
                {
                    this.CaptchaToken = token;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // log check
                }
            }
            catch (Exception ex)
            {
                // log check
            }
        }

        private void webView21_Click(object sender, EventArgs e)
        {
            // log check
        }
    }
}
