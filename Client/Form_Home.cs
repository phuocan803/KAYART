using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Database;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace Client
{
    public partial class Form_Home : Form
    {
        private const int REFRESH_INTERVAL_MS = 30000;
        private const int ROOM_CODE_LENGTH = 4;

        private bool isOffline;
        private string loggedInUsername;
        private bool isGuestMode;

        private Manager dataManager;
        private int currentUserId = -1;

        private static int cachedUserId = -1;
        private static string cachedUsername = null;

        private readonly List<Image> loadedImages = new List<Image>();

        private volatile bool isDisposing = false;

        private readonly object stateLock = new object();

        private ChatPanel chatPanel;

        public Form_Home() : this("Guest") { }

        public Form_Home(string username)
        {
            InitializeComponent();
            SafeSetIcon();

            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;

            if (string.IsNullOrEmpty(username))
                username = "Guest";

            this.loggedInUsername = username;
            this.isGuestMode = (username == "Guest");

            Text = isGuestMode ? "KayArt - Guest Mode (Offline Only)" : $"KayArt - Welcome {username}!";

            dataManager = new Manager(null, null);

            UpdateUILabels();

            TryAssignAvatar();
            ApplyInitialUIState();


            if (!isGuestMode)
            {
                this.Load += Form_Home_Load;
            }
        }

        private void InitializeChatPanel()
        {
            try
            {
                if (chatPanelContainer == null)
                {
                    return;
                }

                chatPanel = new ChatPanel
                {
                    Dock = DockStyle.Fill
                };

                chatPanelContainer.Controls.Add(chatPanel);
            }
            catch (Exception ex)
            {
            }
        }

        [Obsolete("Use Form_Home(string username) instead")]
        public Form_Home(Point location, Size size) : this("Guest") { }

        [Obsolete("Use Form_Home(string username) instead")]
        public Form_Home(string username, Point location, Size size) : this(username) { }

        [Obsolete("Use Form_Home(string username) instead")]
        public Form_Home(string username, Point location) : this(username) { }

        private async Task InitializeUserSessionAsync()
        {
            try
            {
                if (!isGuestMode && !string.IsNullOrEmpty(loggedInUsername))
                {
                    if (cachedUserId > 0 && cachedUsername == loggedInUsername)
                    {
                        currentUserId = cachedUserId;

                        try
                        {
                            await dataManager.UpdateUserOnlineStatusAsync(currentUserId, true);
                        }
                        catch { }
                        return;
                    }

                    currentUserId = await dataManager.GetUserIdByUsernameAsync(loggedInUsername);

                    if (currentUserId > 0)
                    {
                        cachedUserId = currentUserId;
                        cachedUsername = loggedInUsername;

                        await dataManager.UpdateUserOnlineStatusAsync(currentUserId, true);
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {

                if (cachedUserId > 0 && cachedUsername == loggedInUsername)
                {
                    currentUserId = cachedUserId;
                    return;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        MessageBox.Show("Could not connect to server. Some features may not work.",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                }
            }
        }

        private void UpdateUILabels()
        {
            try
            {
                if (label1 != null)
                    label1.Text = $"Welcome back, {loggedInUsername}!";
                if (lblUsername != null)
                    lblUsername.Text = loggedInUsername;
                if (lblUserEmail != null)
                    lblUserEmail.Text = isGuestMode ? "Guest Mode" : $"{loggedInUsername}@kayart.com";
            }
            catch
            {
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            lock (stateLock)
            {
                isDisposing = true;
            }

            try
            {
                dataManager?.DisconnectDataChannel();

                if (currentUserId > 0)
                {
                    try
                    {
                        _ = dataManager.UpdateUserOnlineStatusAsync(currentUserId, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.ApplicationExitCall)
                {
                    cachedUserId = -1;
                    cachedUsername = null;
                }

                DisposeAllImages();


                if (Application.OpenForms.Count <= 1)
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
            }

            base.OnFormClosing(e);
        }

        private void DisposeAllImages()
        {
            lock (loadedImages)
            {
                foreach (var img in loadedImages)
                {
                    try
                    {
                        img?.Dispose();
                    }
                    catch { }
                }
                loadedImages.Clear();
            }
        }

        private void ApplyInitialUIState()
        {
            try
            {
                if (button_create_room != null)
                    button_create_room.Visible = !isGuestMode;
                if (Button_join_room1 != null)
                    Button_join_room1.Visible = !isGuestMode;

                isOffline = isGuestMode;

                UpdateUILabels();
                TryAssignAvatar();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task TryAssignAvatar()
        {
            try
            {
                if (currentUserId > 0)
                {
                    string avatarBase64 = await dataManager.GetUserAvatarAsync(currentUserId);

                    if (!string.IsNullOrEmpty(avatarBase64))
                    {
                        Bitmap avatarBmp = DatabaseHelper.Base64ToBitmap(avatarBase64);
                        if (avatarBmp != null)
                        {
                            if (Pic_Avt?.Image != null)
                            {
                                var oldImage = Pic_Avt.Image;
                                Pic_Avt.Image = null;
                                oldImage.Dispose();
                            }

                            Pic_Avt.SizeMode = PictureBoxSizeMode.Zoom;
                            Pic_Avt.Image = avatarBmp;

                            lock (loadedImages)
                            {
                                loadedImages.Add(avatarBmp);
                            }
                            return;
                        }
                    }
                }

                if (Properties.Resources.Logo is Bitmap bmp)
                {
                    Pic_Avt.SizeMode = PictureBoxSizeMode.Zoom;
                    Pic_Avt.Image = bmp;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region Project Methods

        private async Task LoadMyProjectsAsync()
        {
            if (isDisposing || currentUserId <= 0 || flowLayoutPanel_Projects == null)
                return;

            try
            {
                var projects = await dataManager.GetUserProjectsAsync(currentUserId);

                if (isDisposing) return;

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateProjectsUI(projects)));
                }
                else
                {
                    UpdateProjectsUI(projects);
                }
            }
            catch (Exception ex)
            {

                if (InvokeRequired)
                {
                    Invoke(new Action(() => ShowProjectsErrorState(ex.Message)));
                }
                else
                {
                    ShowProjectsErrorState(ex.Message);
                }
            }
        }

        private void ShowProjectsErrorState(string errorMessage)
        {
            if (flowLayoutPanel_Projects == null) return;

            try
            {
                flowLayoutPanel_Projects.SuspendLayout();
                flowLayoutPanel_Projects.Controls.Clear();
                flowLayoutPanel_Projects.Controls.Add(new Label()
                {
                    Text = $"Error loading projects:\n{errorMessage}",
                    AutoSize = true,
                    ForeColor = Color.OrangeRed,
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(20)
                });
                flowLayoutPanel_Projects.ResumeLayout(true);
            }
            catch { }
        }

        private void UpdateProjectsUI(List<Project> projects)
        {
            if (flowLayoutPanel_Projects == null || isDisposing) return;

            try
            {
                flowLayoutPanel_Projects.SuspendLayout();

                int scrollPosition = flowLayoutPanel_Projects.AutoScrollPosition.Y;

                flowLayoutPanel_Projects.Controls.Clear();

                foreach (var project in projects)
                {
                    var card = CreateProjectCard(project);
                    flowLayoutPanel_Projects.Controls.Add(card);
                }

                if (projects.Count == 0)
                {
                    Label emptyLabel = new Label()
                    {
                        Text = "No projects yet. Create your first canvas!",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 11),
                        Padding = new Padding(20)
                    };
                    flowLayoutPanel_Projects.Controls.Add(emptyLabel);
                }

                flowLayoutPanel_Projects.AutoScrollPosition = new Point(0, Math.Abs(scrollPosition));

                flowLayoutPanel_Projects.ResumeLayout(true);
            }
            catch (Exception ex)
            {
            }
        }

        private Panel CreateProjectCard(Project project)
        {
            Panel card = new Panel()
            {
                Size = new Size(200, 280),
                BackColor = Color.FromArgb(40, 50, 100),
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            PictureBox thumbnail = new PictureBox()
            {
                Size = new Size(180, 140),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            if (!string.IsNullOrEmpty(project.ThumbnailData))
            {
                try
                {
                    var thumbBmp = DatabaseHelper.Base64ToBitmap(project.ThumbnailData);

                    if (thumbBmp != null)
                    {
                        const int maxWidth = 180;
                        const int maxHeight = 140;

                        if (thumbBmp.Width > maxWidth * 2 || thumbBmp.Height > maxHeight * 2)
                        {

                            float ratioX = (float)maxWidth / thumbBmp.Width;
                            float ratioY = (float)maxHeight / thumbBmp.Height;
                            float ratio = Math.Min(ratioX, ratioY);

                            int newWidth = (int)(thumbBmp.Width * ratio);
                            int newHeight = (int)(thumbBmp.Height * ratio);

                            var resized = new Bitmap(newWidth, newHeight);
                            using (var g = Graphics.FromImage(resized))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(thumbBmp, 0, 0, newWidth, newHeight);
                            }

                            thumbBmp.Dispose();
                            thumbBmp = resized;
                        }

                        thumbnail.Image = thumbBmp;
                        lock (loadedImages)
                        {
                            loadedImages.Add(thumbBmp);
                        }
                    }
                }
                catch (Exception ex)
                {

                    thumbnail.BackColor = Color.FromArgb(50, 50, 50);
                    thumbnail.Image = null;

                    Label errorLabel = new Label()
                    {
                        Text = "⚠️",
                        Font = new Font("Segoe UI", 24),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        BackColor = Color.Transparent
                    };
                    thumbnail.Controls.Add(errorLabel);
                }
            }
            else
            {
                thumbnail.BackColor = Color.Gray;
                Label placeholderLabel = new Label()
                {
                    Text = "📄",
                    Font = new Font("Segoe UI", 36),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                thumbnail.Controls.Add(placeholderLabel);
            }

            Label nameLabel = new Label()
            {
                Text = project.ProjectName,
                Location = new Point(10, 160),
                Size = new Size(180, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label dateLabel = new Label()
            {
                Text = project.UpdatedAt.ToString("MMM dd, yyyy"),
                Location = new Point(10, 185),
                Size = new Size(180, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8)
            };

            Button openBtn = new Button()
            {
                Text = "Open",
                Location = new Point(10, 210),
                Size = new Size(85, 28),
                BackColor = Color.CornflowerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            openBtn.FlatAppearance.BorderSize = 0;
            int projectId = project.Id;
            openBtn.Click += (s, e) => SafeOpenProject(projectId);

            Button deleteBtn = new Button()
            {
                Text = "Delete",
                Location = new Point(105, 210),
                Size = new Size(85, 28),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            deleteBtn.FlatAppearance.BorderSize = 0;
            deleteBtn.Click += (s, e) => SafeDeleteProject(projectId, project.ProjectName);

            Button shareBtn = new Button()
            {
                Text = "📤 Share",
                Location = new Point(10, 245),
                Size = new Size(180, 28),
                BackColor = Color.FromArgb(100, 149, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            shareBtn.FlatAppearance.BorderSize = 0;
            shareBtn.Click += (s, e) => ShowShareProjectDialog(projectId, project.ProjectName);

            card.Controls.AddRange(new Control[] { thumbnail, nameLabel, dateLabel, openBtn, deleteBtn, shareBtn });

            EventHandler doubleClickHandler = (s, e) => SafeOpenProject(projectId);
            card.DoubleClick += doubleClickHandler;
            thumbnail.DoubleClick += doubleClickHandler;

            card.Tag = doubleClickHandler;

            return card;
        }

        private async void SafeOpenProject(int projectId)
        {
            try
            {
                await OpenProject(projectId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening project: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OpenProject(int projectId)
        {
            try
            {
                var project = await dataManager.GetProjectByIdAsync(projectId);
                if (project == null || string.IsNullOrEmpty(project.ImageData))
                { MessageBox.Show("Project data not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                var activeUsers = await CheckProjectActiveUsers(projectId);
                bool hasActiveUsers = activeUsers != null && activeUsers.Count > 0;

                if (hasActiveUsers)
                {
                    string userList = string.Join(", ", activeUsers);
                    var result = MessageBox.Show(
                        $"This project has active collaborators:\n{userList}\n\nYes = Collaborate (real-time sync)\nNo = Open Alone (read-only)\nCancel = Abort",
                        "Collaboration Mode", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return;
                    bool collaborationMode = (result == DialogResult.Yes);
                    OpenProjectInMode(project, projectId, collaborationMode);
                }
                else { OpenProjectInMode(project, projectId, false); }
            }
            catch (Exception ex) { MessageBox.Show($"Error loading project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async Task<List<string>> CheckProjectActiveUsers(int projectId)
        {
            try
            {
                string serverIP = ConfigHelper.GetServerIP();
                using (TcpClient client = new TcpClient(serverIP, 9999))
                using (StreamReader reader = new StreamReader(client.GetStream()))
                using (StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true })
                {
                    Packet request = new Packet { Code = 41, ProjectId = projectId };
                    writer.WriteLine(JsonConvert.SerializeObject(request));
                    string responseJson = await Task.Run(() => reader.ReadLine());
                    Packet response = JsonConvert.DeserializeObject<Packet>(responseJson);
                    if (response.Success && !string.IsNullOrEmpty(response.ResponseData))
                        return JsonConvert.DeserializeObject<List<string>>(response.ResponseData);
                }
            }
            catch { }
            return new List<string>();
        }

        private void OpenProjectInMode(Project project, int projectId, bool collaborationMode)
        {
            Rectangle screenBounds = Screen.FromControl(this).Bounds;
            string serverIP = ConfigHelper.GetServerIP();
            Form_Client canvas = new Form_Client(
                !collaborationMode, serverIP, collaborationMode ? projectId : 0,
                loggedInUsername, collaborationMode ? projectId.ToString() : "",
                screenBounds.Location, screenBounds.Size);
            canvas.SetUsername(loggedInUsername);
            canvas.SetUserId(currentUserId);
            Bitmap bmp = DatabaseHelper.Base64ToBitmap(project.ImageData);
            if (bmp != null)
            {
                canvas.LoadBitmap(bmp, projectId, project.ProjectName);
                canvas.WindowState = FormWindowState.Maximized;
                canvas.Show();
            }
            else { MessageBox.Show("Failed to load project image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); canvas.Dispose(); }
        }

        private void SafeDeleteProject(int projectId, string projectName)
        {
            try
            {
                DeleteProject(projectId, projectName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting project: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteProject(int projectId, string projectName)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{projectName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                bool success = await dataManager.DeleteProjectAsync(projectId);

                if (success)
                {
                    _ = LoadMyProjectsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete project.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Project Sharing Methods

        private async void ShowShareProjectDialog(int projectId, string projectName)
        {
            if (isGuestMode)
            {
                MessageBox.Show("Cannot share projects in Guest mode!", "Guest Mode",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Form shareDialog = new Form()
                {
                    Text = $"Share '{projectName}'",
                    Size = new Size(500, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(45, 45, 48)
                };

                Label titleLabel = new Label()
                {
                    Text = "Select friends to share with:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold)
                };

                FlowLayoutPanel friendListPanel = new FlowLayoutPanel()
                {
                    Location = new Point(20, 60),
                    Size = new Size(440, 400),
                    AutoScroll = true,
                    BackColor = Color.FromArgb(30, 30, 30),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var friends = await dataManager.GetFriendsAsync(currentUserId);
                var alreadyShared = await dataManager.GetProjectSharesAsync(projectId);

                foreach (var friend in friends)
                {
                    bool isAlreadyShared = alreadyShared.Any(s => s.UserId == friend.UserId);

                    Panel friendCard = new Panel()
                    {
                        Size = new Size(420, 60),
                        BackColor = Color.FromArgb(40, 40, 45),
                        Margin = new Padding(5)
                    };

                    PictureBox avatar = new PictureBox()
                    {
                        Size = new Size(40, 40),
                        Location = new Point(10, 10),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BackColor = Color.Gray
                    };

                    if (!string.IsNullOrEmpty(friend.AvatarBase64))
                    {
                        try
                        {
                            var avatarBmp = DatabaseHelper.Base64ToBitmap(friend.AvatarBase64);
                            if (avatarBmp != null)
                            {
                                avatar.Image = avatarBmp;
                                lock (loadedImages)
                                {
                                    loadedImages.Add(avatarBmp);
                                }
                            }
                        }
                        catch { }
                    }

                    Label nameLabel = new Label()
                    {
                        Text = friend.Username,
                        Location = new Point(60, 10),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    };

                    Label statusLabel = new Label()
                    {
                        Text = isAlreadyShared ? "✓ Already shared" : "",
                        Location = new Point(60, 30),
                        AutoSize = true,
                        ForeColor = Color.LightGreen,
                        Font = new Font("Segoe UI", 8)
                    };

                    Button actionBtn = new Button()
                    {
                        Text = isAlreadyShared ? "Unshare" : "Share",
                        Location = new Point(330, 15),
                        Size = new Size(80, 30),
                        BackColor = isAlreadyShared ? Color.FromArgb(180, 50, 50) : Color.Green,
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Tag = new { FriendId = friend.UserId, IsShared = isAlreadyShared }
                    };
                    actionBtn.FlatAppearance.BorderSize = 0;

                    actionBtn.Click += async (s, e) =>
                    {
                        dynamic data = actionBtn.Tag;
                        bool currentlyShared = data.IsShared;
                        int friendId = data.FriendId;

                        if (currentlyShared)
                        {
                            if (await dataManager.UnshareProjectAsync(projectId, friendId))
                            {
                                actionBtn.Text = "Share";
                                actionBtn.BackColor = Color.Green;
                                statusLabel.Text = "";
                                actionBtn.Tag = new { FriendId = friendId, IsShared = false };
                                MessageBox.Show($"Unshared '{projectName}' with {friend.Username}", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            if (await dataManager.ShareProjectAsync(projectId, friendId, "view"))
                            {
                                actionBtn.Text = "Unshare";
                                actionBtn.BackColor = Color.FromArgb(180, 50, 50);
                                statusLabel.Text = "✓ Already shared";
                                actionBtn.Tag = new { FriendId = friendId, IsShared = true };
                                MessageBox.Show($"Shared '{projectName}' with {friend.Username}!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to share project.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    };

                    friendCard.Controls.AddRange(new Control[] { avatar, nameLabel, statusLabel, actionBtn });
                    friendListPanel.Controls.Add(friendCard);
                }

                if (friends.Count == 0)
                {
                    Label emptyLabel = new Label()
                    {
                        Text = "No friends to share with.\nAdd friends first!",
                        Location = new Point(140, 180),
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Segoe UI", 11),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    friendListPanel.Controls.Add(emptyLabel);
                }

                Button closeBtn = new Button()
                {
                    Text = "Close",
                    Location = new Point(190, 480),
                    Size = new Size(100, 35),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                closeBtn.FlatAppearance.BorderSize = 0;

                shareDialog.Controls.AddRange(new Control[] { titleLabel, friendListPanel, closeBtn });
                shareDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing share dialog: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Active Rooms Methods

        private async Task LoadActiveRoomsAsync()
        {
            if (isDisposing || flowLayoutPanel_ActiveRooms == null)
                return;

            try
            {

                if (InvokeRequired)
                    Invoke(new Action(() => flowLayoutPanel_ActiveRooms.Controls.Clear()));
                else
                    flowLayoutPanel_ActiveRooms.Controls.Clear();

                var activeRooms = await dataManager.GetActiveRoomsAsync(20);

                if (isDisposing) return;


                if (InvokeRequired)
                    Invoke(new Action(() => UpdateActiveRoomsUI(activeRooms)));
                else
                    UpdateActiveRoomsUI(activeRooms);
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateActiveRoomsUI(List<ActiveRoomInfo> activeRooms)
        {
            if (flowLayoutPanel_ActiveRooms == null || isDisposing) return;

            try
            {
                flowLayoutPanel_ActiveRooms.SuspendLayout();

                foreach (var room in activeRooms)
                {
                    var card = CreateActiveRoomCard(room);
                    flowLayoutPanel_ActiveRooms.Controls.Add(card);
                }

                if (activeRooms.Count == 0)
                {
                    Label emptyLabel = new Label()
                    {
                        Text = "No active rooms. Create one to collaborate!",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 11),
                        Padding = new Padding(20)
                    };
                    flowLayoutPanel_ActiveRooms.Controls.Add(emptyLabel);
                }

                flowLayoutPanel_ActiveRooms.ResumeLayout();
            }
            catch (Exception ex)
            {
            }
        }

        private Panel CreateActiveRoomCard(ActiveRoomInfo room)
        {
            Panel card = new Panel()
            {
                Size = new Size(280, 120),
                BackColor = Color.FromArgb(60, 70, 80),
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            Label codeLabel = new Label()
            {
                Text = $"Room: {room.RoomCode}",
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            Label ownerLabel = new Label()
            {
                Text = $"Owner: {room.OwnerUsername}",
                Location = new Point(10, 35),
                AutoSize = true,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9)
            };

            Label userCountLabel = new Label()
            {
                Text = $"Users: {room.UserCount}",
                Location = new Point(10, 55),
                AutoSize = true,
                ForeColor = Color.LightBlue,
                Font = new Font("Segoe UI", 9)
            };

            Label activityLabel = new Label()
            {
                Text = $"Active: {room.LastActivity:HH:mm}",
                Location = new Point(10, 75),
                AutoSize = true,
                ForeColor = Color.LightGreen,
                Font = new Font("Segoe UI", 8)
            };

            Button joinBtn = new Button()
            {
                Text = "Join",
                Location = new Point(200, 85),
                Size = new Size(70, 28),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            joinBtn.FlatAppearance.BorderSize = 0;

            int roomCode = room.RoomCode;
            joinBtn.Click += (s, e) => JoinActiveRoom(roomCode);

            card.Controls.AddRange(new Control[] {
                codeLabel, ownerLabel, userCountLabel, activityLabel, joinBtn
            });

            return card;
        }

        private void JoinActiveRoom(int roomCode)
        {
            try
            {
                string username = !string.IsNullOrEmpty(loggedInUsername) ? loggedInUsername : "Guest";
                string serverIP = ConfigHelper.GetServerIP();

                if (isGuestMode)
                {
                    MessageBox.Show("Cannot join rooms in Guest mode!", "Guest Mode",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!IPv4IsValid(serverIP))
                {
                    MessageBox.Show("Invalid server IP! Please check configuration.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                go_to_canvas(serverIP, 1, username, roomCode.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining room: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Shared Projects Display

        private Panel CreateSharedProjectCard(SharedProject shared)
        {
            Panel card = new Panel()
            {
                Size = new Size(200, 270),
                BackColor = Color.FromArgb(50, 60, 90),
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            PictureBox thumbnail = new PictureBox()
            {
                Size = new Size(180, 140),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            if (!string.IsNullOrEmpty(shared.ThumbnailData))
            {
                try
                {
                    var thumbBmp = DatabaseHelper.Base64ToBitmap(shared.ThumbnailData);
                    if (thumbBmp != null)
                    {
                        thumbnail.Image = thumbBmp;
                        lock (loadedImages)
                        {
                            loadedImages.Add(thumbBmp);
                        }
                    }
                }
                catch
                {
                    thumbnail.BackColor = Color.FromArgb(50, 50, 50);
                }
            }

            Label nameLabel = new Label()
            {
                Text = shared.ProjectName,
                Location = new Point(10, 160),
                Size = new Size(180, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label ownerLabel = new Label()
            {
                Text = $"By: {shared.OwnerUsername}",
                Location = new Point(10, 185),
                Size = new Size(180, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8)
            };

            Label dateLabel = new Label()
            {
                Text = shared.SharedAt.ToString("MMM dd, yyyy"),
                Location = new Point(10, 205),
                Size = new Size(180, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8)
            };

            Button openBtn = new Button()
            {
                Text = "Open",
                Location = new Point(10, 230),
                Size = new Size(180, 28),
                BackColor = Color.MediumPurple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            openBtn.FlatAppearance.BorderSize = 0;
            int projectId = shared.ProjectId;
            openBtn.Click += (s, e) => SafeOpenProject(projectId);

            card.Controls.AddRange(new Control[] { thumbnail, nameLabel, ownerLabel, dateLabel, openBtn });
            return card;
        }

        #endregion

        #region User State & Navigation

        public void SetUserState(string username, bool guestMode)
        {
            if (string.IsNullOrWhiteSpace(username))
                username = "Guest";

            lock (stateLock)
            {
                loggedInUsername = username;
                isGuestMode = guestMode;
            }

            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetUserState(username, guestMode)));
                return;
            }

            Text = isGuestMode ? "KayArt - Guest Mode (Offline Only)" : $"KayArt - Welcome {username} !";

            UpdateUILabels();
            ApplyInitialUIState();

            this.WindowState = FormWindowState.Maximized;
        }

        /**
         * ✅ Validate IPv4 address
         */
        public bool IPv4IsValid(string ipv4)
        {
            if (string.IsNullOrWhiteSpace(ipv4)) return false;

            string[] splitValues = ipv4.Split('.');
            if (splitValues.Length != 4) return false;

            return splitValues.All(i => byte.TryParse(i, out _));
        }

        private bool IsValidRoomCode(string roomCode)
        {
            return !string.IsNullOrWhiteSpace(roomCode) &&
                   roomCode.Length == ROOM_CODE_LENGTH &&
                   roomCode.All(char.IsDigit);
        }

        private void go_to_canvas(string serverIP, int code, string username, string roomID = "")
        {
            try
            {
                Rectangle screenBounds = Screen.FromControl(this).Bounds;
                Form_Client canvas = new Form_Client(isOffline, serverIP, code, username, roomID, screenBounds.Location, screenBounds.Size);
                canvas.SetUsername(username);
                canvas.WindowState = FormWindowState.Maximized;
                canvas.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create canvas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handlers

        private void button_join_room_Click(object sender, EventArgs e)
        {
            try
            {
                string username = string.IsNullOrWhiteSpace(lblUsername?.Text) ? loggedInUsername : lblUsername.Text;
                string roomID = ShowRoomCodeDialog();
                string serverIP = ConfigHelper.GetServerIP();

                if (string.IsNullOrEmpty(roomID))
                    return;

                if (!IsValidRoomCode(roomID))
                {
                    MessageBox.Show($"Mã phòng phải là {ROOM_CODE_LENGTH} chữ số!",
                        "Mã phòng không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!isOffline)
                {
                    if (!IPv4IsValid(serverIP))
                    {
                        MessageBox.Show("Địa chỉ IP không hợp lệ!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                go_to_canvas(serverIP, 1, username, roomID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining room: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_create_room_Click(object sender, EventArgs e)
        {
            try
            {
                string username = string.IsNullOrWhiteSpace(lblUsername?.Text) ? loggedInUsername : lblUsername.Text;
                string serverIP = ConfigHelper.GetServerIP();

                if (!isOffline)
                {
                    if (!IPv4IsValid(serverIP))
                    {
                        MessageBox.Show("Địa chỉ IP không hợp lệ!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                go_to_canvas(serverIP, 0, username);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating room: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_go_create_canvas_Click(object sender, EventArgs e)
        {
            try
            {
                string username = !string.IsNullOrEmpty(loggedInUsername) ? loggedInUsername : "Guest";
                bool openOffline = true;

                Rectangle screenBounds = Screen.FromControl(this).Bounds;
                Form_Client canvas = new Form_Client(openOffline, "", 0, username, "", screenBounds.Location, screenBounds.Size);
                canvas.SetUsername(username);
                canvas.WindowState = FormWindowState.Maximized;
                canvas.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating canvas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_Logout_Click(object sender, EventArgs e)
        {
            try
            {
                if (isGuestMode)
                {
                    Form_Login loginForm = new Form_Login();
                    loginForm.Show();
                    this.Close();
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "Bạn có chắc muốn đăng xuất?",
                        "Xác nhận đăng xuất",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        Form_Login loginForm = new Form_Login();
                        loginForm.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void Button_AVT_click(object sender, EventArgs e)
        {
            if (isGuestMode)
            {
                MessageBox.Show("Profile is not available in Guest mode.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                openDialog.Title = "Select Avatar Image";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Bitmap bmp = new Bitmap(openDialog.FileName))
                        {
                            string avatarBase64 = DatabaseHelper.GenerateThumbnailBase64(bmp, 200, 200);

                            bool success = await dataManager.UpdateUserAvatarAsync(currentUserId, avatarBase64);

                            if (success)
                            {
                                await TryAssignAvatar();
                                MessageBox.Show("Avatar updated successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to update avatar.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to update avatar: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void Button_MyProjects_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentUserId <= 0)
                {
                    if (cachedUserId > 0 && cachedUsername == loggedInUsername)
                    {
                        currentUserId = cachedUserId;
                    }
                    else if (!isGuestMode && !string.IsNullOrEmpty(loggedInUsername))
                    {
                        await InitializeUserSessionAsync();
                    }
                }

                if (currentUserId <= 0)
                {
                    MessageBox.Show("Session expired. Please login again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await Task.WhenAll(
                    LoadMyProjectsAsync(),
                    LoadActiveRoomsAsync()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing projects:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Button_SharedWithMe_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentUserId <= 0)
                {
                    if (cachedUserId > 0 && cachedUsername == loggedInUsername)
                    {
                        currentUserId = cachedUserId;
                    }
                    else if (!isGuestMode && !string.IsNullOrEmpty(loggedInUsername))
                    {
                        await InitializeUserSessionAsync();
                    }
                }

                if (currentUserId <= 0)
                {
                    MessageBox.Show("Session expired. Please login again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await Task.WhenAll(
                    LoadSharedProjectsInMainAreaAsync(),
                    LoadActiveRoomsAsync()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing shared projects:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSharedProjectsInMainAreaAsync()
        {
            if (isDisposing || currentUserId <= 0 || flowLayoutPanel_Projects == null)
                return;

            try
            {
                flowLayoutPanel_Projects.SuspendLayout();

                int scrollPosition = flowLayoutPanel_Projects.AutoScrollPosition.Y;

                flowLayoutPanel_Projects.Controls.Clear();

                var sharedProjects = await dataManager.GetSharedProjectsAsync(currentUserId);

                if (isDisposing) return;

                foreach (var shared in sharedProjects)
                {
                    flowLayoutPanel_Projects.Controls.Add(CreateSharedProjectCard(shared));
                }

                if (sharedProjects.Count == 0)
                {
                    flowLayoutPanel_Projects.Controls.Add(new Label()
                    {
                        Text = "No projects shared with you yet.",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 11),
                        Padding = new Padding(20)
                    });
                }

                flowLayoutPanel_Projects.AutoScrollPosition = new Point(0, Math.Abs(scrollPosition));

                flowLayoutPanel_Projects.ResumeLayout(true);
            }
            catch (Exception ex)
            {

                try
                {
                    flowLayoutPanel_Projects.ResumeLayout(true);
                }
                catch { }
            }
        }

        #endregion

        #region Icon Helper

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
                        tmpIcon?.Dispose();
                        if (hIcon != IntPtr.Zero)
                        {
                            DestroyIcon(hIcon);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Form Load

        private async void Form_Home_Load(object sender, EventArgs e)
        {
            try
            {

                if (!isGuestMode)
                {
                    string serverIP = ConfigHelper.GetServerIP();

                    await dataManager.ConnectDataChannelAsync(serverIP, 9999);

                    await InitializeUserSessionAsync();

                    if (currentUserId > 0)
                    {
                        if (chatPanel == null)
                        {
                            InitializeChatPanel();
                        }

                        if (chatPanel != null)
                        {
                            chatPanel.Initialize(dataManager, currentUserId, loggedInUsername);
                        }
                    }

                    if (currentUserId > 0)
                    {
                        await Task.WhenAll(
                            LoadMyProjectsAsync(),
                            LoadActiveRoomsAsync()
                        );

                    }
                    else
                    {

                        if (flowLayoutPanel_Projects != null)
                        {
                            flowLayoutPanel_Projects.Controls.Clear();
                            flowLayoutPanel_Projects.Controls.Add(new Label()
                            {
                                Text = "Có vẻ như bạn chưa đăng nhập.\nVui lòng đăng nhập để truy cập dự án của bạn.",
                                AutoSize = true,
                                ForeColor = Color.Orange,
                                Font = new Font("Segoe UI", 11),
                                Padding = new Padding(20)
                            });
                        }

                        if (flowLayoutPanel_ActiveRooms != null)
                        {
                            flowLayoutPanel_ActiveRooms.Controls.Clear();
                            flowLayoutPanel_ActiveRooms.Controls.Add(new Label()
                            {
                                Text = "Không tìm thấy phòng hoạt động nào.\nTạo một phòng mới để bắt đầu hợp tác!",
                                AutoSize = true,
                                ForeColor = Color.Orange,
                                Font = new Font("Segoe UI", 11),
                                Padding = new Padding(20)
                            });
                        }

                        MessageBox.Show(
                            "Could not connect to server.\nSome features may not work properly.\n\nPlease check:\n- Server is running\n- Server IP in appsettings.json is correct",
                            "Connection Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                else
                {
                    if (flowLayoutPanel_Projects != null)
                    {
                        flowLayoutPanel_Projects.Controls.Clear();
                        flowLayoutPanel_Projects.Controls.Add(new Label()
                        {
                            Text = "Chế độ khách - Không có dự án nào khả dụng.\nVui lòng đăng nhập để truy cập dự án của bạn.",
                            AutoSize = true,
                            ForeColor = Color.Gray,
                            Font = new Font("Segoe UI", 11),
                            Padding = new Padding(20)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing application:\n{ex.Message}\n\nPlease check:\n- Server is running\n- Database connection is valid",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }



        #endregion

        #region Panel Room Code Event Handlers

        private void button_join_click(object sender, EventArgs e)
        {
            try
            {

                string roomCode = ShowRoomCodeDialog();

                if (string.IsNullOrEmpty(roomCode))
                {
                    return;
                }

                if (!IsValidRoomCode(roomCode))
                {
                    MessageBox.Show($"Mã phòng phải là {ROOM_CODE_LENGTH} chữ số!",
                        "Mã phòng không hợp lệ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }


                string username = !string.IsNullOrEmpty(loggedInUsername) ? loggedInUsername : lblUsername?.Text ?? "Guest";
                string serverIP = ConfigHelper.GetServerIP();

                if (isGuestMode)
                {
                    MessageBox.Show("Cannot join rooms in Guest mode!", "Guest Mode",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!IPv4IsValid(serverIP))
                {
                    MessageBox.Show($"Invalid server IP: {serverIP}\nPlease check appsettings.json",
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                go_to_canvas(serverIP, 1, username, roomCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining room:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ShowRoomCodeDialog()
        {
            Form dialog = new Form()
            {
                Text = "Join Room",
                Size = new Size(400, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            Label label = new Label()
            {
                Text = "Enter Room Code (4 digits):",
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };

            TextBox textBox = new TextBox()
            {
                Location = new Point(20, 50),
                Width = 340,
                Font = new Font("Segoe UI", 14),
                MaxLength = 4,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ShortcutsEnabled = true
            };

            textBox.KeyPress += (s, e) =>
            {
                if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == 22)
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == 3)
                {
                    e.Handled = false;
                }
                else if (e.KeyChar == 24)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            };

            textBox.TextChanged += (s, e) =>
            {
                string cleaned = new string(textBox.Text.Where(char.IsDigit).ToArray());

                if (textBox.Text != cleaned)
                {
                    int cursorPos = textBox.SelectionStart;
                    textBox.Text = cleaned;
                    textBox.SelectionStart = Math.Min(cursorPos, textBox.Text.Length);
                }

                if (textBox.Text.Length > 4)
                {
                    textBox.Text = textBox.Text.Substring(0, 4);
                    textBox.SelectionStart = 4;
                }
            };

            Button btnJoin = new Button()
            {
                Text = "Join",
                Location = new Point(150, 95),
                Width = 90,
                Height = 35,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnJoin.FlatAppearance.BorderSize = 0;

            Button btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(250, 95),
                Width = 90,
                Height = 35,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            dialog.Controls.AddRange(new Control[] { label, textBox, btnJoin, btnCancel });
            dialog.AcceptButton = btnJoin;
            dialog.CancelButton = btnCancel;

            dialog.Shown += (s, e) => textBox.Focus();

            DialogResult result = dialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                return textBox.Text.Trim();
            }

            return null;
        }
        #endregion
    }
}
