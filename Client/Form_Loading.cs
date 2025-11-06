using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form_Loading : Form
    {
        private int progress = 0;

        public Form_Loading()
        {
            InitializeComponent();
        }

        private void Form_Loading_Load(object sender, EventArgs e)
        {
            timerLoading.Start();

            Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i <= 100; i += 20)
                    {
                        progress = i;
                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar.Value = progress;
                            lblProgress.Text = $"Loading... {progress}%";
                        });
                        await Task.Delay(300);
                    }

                    await Task.Delay(500);

                    this.Invoke((MethodInvoker)delegate
                    {
                        bool serverAvailable = false;
                        try
                        {
                            using (var client = new System.Net.Sockets.TcpClient())
                            {
                                var result = client.BeginConnect(ConfigHelper.GetServerIP(), ConfigHelper.GetServerPort(), null, null);
                                serverAvailable = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                                if (serverAvailable)
                                {
                                    try { client.EndConnect(result); } catch { }
                                }
                            }
                        }
                        catch { serverAvailable = false; }

                        if (serverAvailable)
                        {
                            Form_Login loginForm = new Form_Login();
                            loginForm.Show();
                        }
                        else
                        {
                            Form_Home homeForm = new Form_Home("Guest");
                            homeForm.Show();
                        }
                        this.Hide();
                    });
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Form_Home homeForm = new Form_Home("Guest");
                            homeForm.Show();
                            this.Hide();
                        });
                    }
                    catch { }
                }
            });
        }

        private void timerLoading_Tick(object sender, EventArgs e)
        {
            // Animate dots
            if (lblLoading.Text.EndsWith("..."))
                lblLoading.Text = "Loading";
            else
                lblLoading.Text += ".";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
