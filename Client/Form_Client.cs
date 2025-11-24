using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Database;

namespace Client
{
    public partial class Form_Client : Form
    {
        private Bitmap bitmap;
        private Graphics graphics;
        private bool isDrawing = false;
        private Point startPoint, endPoint;
        private Pen pen;
        private string currentTool = "Pen";
        private List<Point> polygonPoints = new List<Point>();
        private SolidBrush fillBrush;
        private Bitmap tempBmp;

        private Color stateColor = Color.Black;

        // stack lưu trạng thái bitmap
        private Stack<byte[]> undoStack = new Stack<byte[]>();
        private Stack<byte[]> redoStack = new Stack<byte[]>(
    );
        private const int MaxHistory = 5; // giới hạn số bước undo và redo

        // quản lý ảnh import
        private Image loadedImage;
        private Rectangle imageRect;
        private bool isResizing = false;
        private bool isDragging = false;
        private Point lastMousePos;
        private bool isImageEditMode = false; // true = chỉnh sửa ảnh, false = vẽ
        // enum để biết góc nào đang resize
        private enum ResizeAnchor { None, TopLeft, TopRight, BottomLeft, BottomRight }
        private ResizeAnchor currentAnchor = ResizeAnchor.None;

        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Packet this_client_info;
        private IPEndPoint serverIP;
        private Manager Manager;
        private bool isOffline;
        private bool isNew;
        private Thread receiveThread;
        private volatile bool isClosing = false; // flag dừng 

        private SynchronizationContext context;
        private bool isFullScreen = false;
        private FormBorderStyle previousBorderStyle;
        private FormWindowState previousWindowState;
        private Rectangle previousBounds;
        private int currentProjectId = -1;
        private string currentProjectName = "Untitled";
        private string currentUsername = "Guest";
        private int currentUserId = -1; // trigger từ server để chat, id chat
        private Bitmap canvas => bitmap; // alias for bitmap

        // tạo nền chat panel
        private ChatPanel chatPanel;
        private Panel chatPanelContainer;

        // test cái ai 
        private Button btnAIEnhance;



        public Form_Client()
        {
            InitializeComponent();
            SafeSetIcon();
            this.WindowState = FormWindowState.Maximized;
            SafeInitializeDrawing();
            currentUsername = "Guest";
            
            try
            {
                InitializeChatPanel();
            }
            catch (Exception ex)
            {
            }

            InitializeAIEnhanceButton();
        }
        public Form_Client(bool mode, string _serverIP, int code, string username, string roomID, Point location, Size size)
        {
            InitializeComponent();
            SafeSetIcon();
            this.WindowState = FormWindowState.Maximized;
            SafeInitializeDrawing();
            PushState();

            this.ActiveControl = null;

            this_client_info = new Packet()
            {
                Code = code,
                Username = username,
                RoomID = roomID,
            };

            isNew = true;
            isOffline = mode;

            if (!isOffline && !string.IsNullOrEmpty(_serverIP))
            {
                try
                {
                    serverIP = new IPEndPoint(IPAddress.Parse(_serverIP), 9999);
                }
                catch (Exception ex)
                {
                    serverIP = new IPEndPoint(IPAddress.Loopback, 9999);
                }
            }

            var dummyListView = new ListView();
            var dummyTextBox = new TextBox();
            Manager = new Manager(dummyListView, dummyTextBox);
            currentUsername = username ?? "Guest";

            try
            {
                InitializeChatPanel();
            }
            catch (Exception ex)
            {
            }

            try
            {
                InitializeAIEnhanceButton();
            }
            catch (Exception ex)
            {
            }
        }
        public Form_Client(bool mode, string _serverIP, int code, string username, string roomID)
            : this(mode, _serverIP, code, username, roomID, new Point(100, 100), new Size(1626, 894))
        {
        }

        private bool _initializedOnce = false;
        private void SafeInitializeDrawing()
        {
            if (Canvas.Image == null)
            {
                bitmap = new Bitmap(Canvas.Width, Canvas.Height);
                Canvas.Image = bitmap;
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                pen = new Pen(Color.Black, 2)
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round,
                    LineJoin = LineJoin.Round
                };

                fillBrush = new SolidBrush(Color.Black);

                if (!_initializedOnce) // chỉ clear lần đầu
                {
                    graphics.Clear(Color.White);
                    _initializedOnce = true;
                }
                Canvas.Refresh();
            }
            else
            {
                bitmap = (Bitmap)Canvas.Image;
                graphics?.Dispose();
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // gắn lại sự kiện chuột
            Canvas.MouseDown -= Canvas_MouseDown;
            Canvas.MouseMove -= Canvas_MouseMove;
            Canvas.MouseUp -= Canvas_MouseUp;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
        }
        private async void Form_Client_Load(object sender, EventArgs e)
        {
            context = SynchronizationContext.Current;

            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.KeyDown += Form_Client_KeyDown;

            AdjustCanvasSize();
            this.Resize += Form_Client_Resize;

            int screenHalf = Screen.PrimaryScreen.WorkingArea.Width / 2;
            int minWidth = Math.Max(screenHalf, 800); // min 800
            int minHeight = 600;

            this.MinimumSize = new Size(minWidth, minHeight);

            SafeInitializeDrawing();
            UpdateUndoRedoButtons();

            InitializeColorBoxes();

            UpdateToolbarResponsive();

            if (this.ClientSize.Width >= 1350 && panelColorPalette != null)
            {
                panelColorPalette.Visible = true;
                panelColorPalette.BringToFront();
                panelColors?.Refresh();
            }

            if (!isOffline)
            {
                try
                {
                    client = new TcpClient();
                    client.NoDelay = true;
                    await client.ConnectAsync(serverIP.Address, serverIP.Port);
                }
                catch (Exception ex)
                {
                    Manager.ShowError($"Cannot connect to server at {serverIP.Address}:{serverIP.Port}\n{ex.Message}");
                    this.Close();
                    return;
                }
                NetworkStream stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
                {
                    this_client_info.JwtToken = AuthManager.CurrentToken;
                }

                sendToServer(this_client_info);

                Manager.UpdateRoomID(this_client_info.RoomID);
                Manager.AddToUserListView(this_client_info.Username + " (you)");

                receiveThread = new Thread(Receive);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                try
                {
                    string serverIP = ConfigHelper.GetServerIP();
                    int serverPort = ConfigHelper.GetServerPort();
                    bool connected = await Manager.ConnectDataChannelAsync(serverIP, serverPort);
                    
                    if (connected)
                    {
                        // Get user ID for chat panel
                        int userId = await Manager.GetUserIdByUsernameAsync(currentUsername);
                        if (userId > 0)
                        {
                            InvokeOnUIThread(() =>
                            {
                                chatPanel?.Initialize(Manager, userId, currentUsername, null);
                            });
                        }
                        else
                        {
                            // log check
                        }
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
            else
            {
                InitializeChatPanelOffline();
                
                if (btnMakeOnline != null)
                {
                    btnMakeOnline.Visible = true;
                }
            }
        }

        // tạo chat nếu online
        private void InitializeChatPanelOnline()
        {
            if (chatPanel == null) return;

            try
            {
                chatPanel.Initialize(Manager, -1, currentUsername, null);
            }
            catch (Exception ex)
            {
                // log check
            }
        }

        // tách ra online offline do 2 mode config
        private void InitializeChatPanelOffline()
        {
            if (chatPanel == null) return;

            try
            {
                chatPanel.Initialize(Manager, -1, currentUsername, null);
                
                if (chatPanelContainer != null)
                {
                    chatPanelContainer.Visible = false;
                }
                
            }
            catch (Exception ex)
            {
                // log check
            }
        }

        private void Form_Client_Resize(object? sender, EventArgs e)
        {
            AdjustCanvasSize();
            UpdateToolbarResponsive();
        }

        private async void btnMakeOnline_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "Convert this project to online collaborative mode?\n\n" +
                    "• Project will be saved to server\n" +
                    "• Real-time collaboration enabled\n" +
                    "• Shared users can edit together\n\nContinue?",
                    "Make Project Online", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                if (isOffline || client == null || !client.Connected)
                {
                    string serverIP = ConfigHelper.GetServerIP();
                    int serverPort = ConfigHelper.GetServerPort();
                    
                    client = new TcpClient();
                    await client.ConnectAsync(serverIP, serverPort);
                    
                    reader = new StreamReader(client.GetStream());
                    writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    isOffline = false;
                    
                    if (Manager != null)
                    {
                        bool dataConnected = await Manager.ConnectDataChannelAsync(serverIP, serverPort);

                    }
                }

                if (this_client_info == null)
                {
                    this_client_info = new Packet { Username = currentUsername, UserId = currentUserId };
                }

                // check user id
                if (currentUserId <= 0 && !string.IsNullOrEmpty(currentUsername))
                {
                    Packet getUserIdRequest = new Packet { Code = 32, Username = currentUsername };
                    if (!string.IsNullOrEmpty(AuthManager.CurrentToken)) getUserIdRequest.JwtToken = AuthManager.CurrentToken;
                    writer.WriteLine(JsonConvert.SerializeObject(getUserIdRequest)); writer.Flush();
                    string userIdResponseJson = await Task.Run(() => reader.ReadLine());
                    Packet userIdResponse = JsonConvert.DeserializeObject<Packet>(userIdResponseJson);
                    if (userIdResponse.Success && userIdResponse.RequestUserId > 0)
                    {
                        currentUserId = userIdResponse.RequestUserId;
                        this_client_info.UserId = currentUserId;
                    }
                    else
                    {
                        MessageBox.Show("Failed to get user ID. Please login first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string imageData = BitmapToBase64(canvas);
                string thumbnailData = GenerateThumbnailBase64(canvas);

                Packet createRequest = new Packet { Code = 33, RequestUserId = this_client_info.UserId, 
                    ProjectName = currentProjectName ?? "Untitled Project", ImageData = imageData, 
                    ThumbnailData = thumbnailData, ImageWidth = canvas.Width, ImageHeight = canvas.Height };
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken)) createRequest.JwtToken = AuthManager.CurrentToken;
                writer.WriteLine(JsonConvert.SerializeObject(createRequest)); writer.Flush();
                string responseJson = await Task.Run(() => reader.ReadLine());
                Packet createResponse = JsonConvert.DeserializeObject<Packet>(responseJson);
                if (!createResponse.Success || createResponse.ProjectId <= 0)
                { MessageBox.Show("Failed to upload project to server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                int projectId = createResponse.ProjectId;
                currentProjectId = projectId;

                // mode online
                Packet makeOnlineRequest = new Packet { Code = 40, ProjectId = projectId, RequestUserId = this_client_info.UserId };
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken)) makeOnlineRequest.JwtToken = AuthManager.CurrentToken;
                writer.WriteLine(JsonConvert.SerializeObject(makeOnlineRequest)); writer.Flush();
                responseJson = await Task.Run(() => reader.ReadLine());
                Packet makeOnlineResponse = JsonConvert.DeserializeObject<Packet>(responseJson);

                if (makeOnlineResponse.Success)
                {
                    btnMakeOnline.Visible = false;
                    this_client_info.RoomID = makeOnlineResponse.RoomID;
                    
                    
                    if (receiveThread == null || !receiveThread.IsAlive)
                    {
                        receiveThread = new Thread(Receive) { IsBackground = true };
                        receiveThread.Start();
                    }
                    
                    if (chatPanelContainer != null)
                    {
                        this.Controls.Remove(chatPanelContainer);
                        chatPanelContainer.Dispose();
                        chatPanelContainer = null;
                        chatPanel = null;
                    }
                    
                    {
                        chatPanelContainer = new Panel
                        {
                            Dock = DockStyle.Right,
                            Width = 350,
                            BackColor = Color.FromArgb(30, 30, 35),
                            Name = "chatPanelContainer",
                            Visible = true
                        };
                        
                        this.Controls.Add(chatPanelContainer);
                        chatPanelContainer.BringToFront();
                        
                        try
                        {
                            chatPanel = new ChatPanel();
                            chatPanel.Dock = DockStyle.Fill;
                            chatPanelContainer.Controls.Add(chatPanel);
                            chatPanel.BringToFront();
                            
                            int roomCode = int.Parse(makeOnlineResponse.RoomID);
                            int userId = await Manager.GetUserIdByUsernameAsync(this_client_info.Username);
                            
                            if (userId > 0)
                            {
                                chatPanel.Initialize(Manager, userId, this_client_info.Username, roomCode);
                            }
                            else
                            {
                                chatPanel.SetRoomCode(roomCode);
                            }
                        }
                        catch (Exception chatPanelEx)
                        {
                            MessageBox.Show($"Warning: Chat UI failed to load.\n{chatPanelEx.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        
                        this.PerformLayout();
                        this.Refresh();
                    }
                    
                    MessageBox.Show($"Project is now online!\n\nRoom Code: {makeOnlineResponse.RoomID}\n\nChat panel should be visible on the right.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                else { MessageBox.Show($"Failed to make project online: {makeOnlineResponse.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to make project online: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateToolbarResponsive()
        {
            try
            {
                if (panelMainToolbar == null) return;

                int currentWidth = this.ClientSize.Width;

                bool isVeryCompact = currentWidth < 850;      
                bool isCompact = currentWidth < 1100;         
                bool isMedium = currentWidth < 1350;          

                if (panelColorPalette != null && panelColorPalette.Controls.Count == 0)
                {
                    InitializeColorBoxes();
                }

                ShowControl(panelColorPalette);
                ShowControl(ptbEditColor);
                ShowControl(ptbColor1);
                ShowControl(ptbColor2);
                ShowControl(panelColors);
                ShowControl(labelColors);
                ShowControl(Button_LineSize);
                ShowControl(labelBrushSize);

                if (isVeryCompact)
                {
                    HideControl(Button_LineSize);
                    HideControl(labelBrushSize);

                    SetLabelCompact(labelActions, "Act");
                    SetLabelCompact(labelTools, "Tool");
                    SetLabelCompact(labelShapes, "Shp");
                }
                else if (isCompact)
                {
                    ShowControl(labelActions);
                    ShowControl(labelTools);
                    ShowControl(labelShapes);

                    RestoreLabel(labelActions, "Actions");
                    RestoreLabel(labelTools, "Tools");
                    RestoreLabel(labelShapes, "Shapes");
                }
                else if (isMedium)
                {
                    ShowControl(labelActions);
                    ShowControl(labelTools);
                    ShowControl(labelShapes);

                    RestoreLabel(labelActions, "Actions");
                    RestoreLabel(labelTools, "Tools");
                    RestoreLabel(labelShapes, "Shapes");
                }
                else
                {
                    ShowControl(labelActions);
                    ShowControl(labelTools);
                    ShowControl(labelShapes);

                    RestoreLabel(labelActions, "Actions");
                    RestoreLabel(labelTools, "Tools");
                    RestoreLabel(labelShapes, "Shapes");
                }

                if (panelColorPalette != null)
                {
                    panelColorPalette.Visible = true;
                    panelColorPalette.BringToFront();
                    panelColorPalette.Show();

                    foreach (Control ctrl in panelColorPalette.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            ctrl.Visible = true;
                        }
                    }

                }

                if (panelColors != null)
                {
                    panelColors.SuspendLayout();
                    panelColors.ResumeLayout(true);
                    panelColors.PerformLayout();
                    panelColors.Refresh();
                }

                panelMainToolbar?.PerformLayout();
                panelMainToolbar?.Refresh();

            }
            catch (Exception ex)
            {
                // log check
            }
        }

        private void HideControl(Control ctrl)
        {
            if (ctrl != null && ctrl.Visible)
                ctrl.Visible = false;
        }

        private void ShowControl(Control ctrl)
        {
            if (ctrl != null && !ctrl.Visible)
                ctrl.Visible = true;
        }

        private void SetLabelCompact(Label label, string text)
        {
            if (label != null && label.Text != text)
            {
                label.Text = text;
                label.AutoSize = true;
            }
        }

        private void RestoreLabel(Label label, string text)
        {
            if (label != null && label.Text != text)
            {
                label.Text = text;
                label.AutoSize = true;
            }
        }

        private int CalculateToolbarWidth()
        {
            int totalWidth = 10; // Left margin

            totalWidth += (2 * 46) + 2;
            totalWidth += 6; // Space before separator
            totalWidth += 1 + 6; // Separator + space

            totalWidth += 46;
            totalWidth += 6; // Space before separator
            totalWidth += 1 + 6; // Separator + space

            totalWidth += (4 * 46) + (3 * 2) + 6;
            totalWidth += 6; // Space before separator
            totalWidth += 1 + 6; // Separator + space

            totalWidth += (5 * 46) + (4 * 2) + 6;
            totalWidth += 6; // Space before separator
            totalWidth += 1 + 6; // Separator + space

            totalWidth += 120 + 6;
            totalWidth += 6; // Space before separator
            totalWidth += 1 + 6; // Separator + space

            totalWidth += 330;
            totalWidth += 10; // Right margin

            return totalWidth; // ~1020px
        }

        private void Form_Client_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && isFullScreen)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                previousBorderStyle = this.FormBorderStyle;
                previousWindowState = this.WindowState;
                previousBounds = this.Bounds;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;

                isFullScreen = true;
            }
            else
            {
                this.FormBorderStyle = previousBorderStyle;
                this.WindowState = previousWindowState;
                this.TopMost = false;

                isFullScreen = false;
            }

            AdjustCanvasSize();
        }

        private void AdjustCanvasSize()
        {
            if (Canvas == null) return;

            int canvasWidth = Canvas.ClientSize.Width;
            int canvasHeight = Canvas.ClientSize.Height;
            
            if (canvasWidth < 400) canvasWidth = 400;
            if (canvasHeight < 300) canvasHeight = 300;
            
            if (bitmap != null && (bitmap.Width != canvasWidth || bitmap.Height != canvasHeight))
            {
                
                var oldBitmap = bitmap;
                bitmap = new Bitmap(canvasWidth, canvasHeight);
                
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    if (oldBitmap != null)
                    {
                        g.DrawImage(oldBitmap, 0, 0);
                        oldBitmap.Dispose();
                    }
                }
                
                Canvas.Image = bitmap;
                graphics?.Dispose();
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
            }
            else if (bitmap == null)
            {
                bitmap = new Bitmap(canvasWidth, canvasHeight);
                Canvas.Image = bitmap;
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.White);
            }
        }
        private void Receive()
        {
            try
            {
                string responseInJson;
                while (!isClosing) // flag gắn dừng
                {
                    responseInJson = reader?.ReadLine();
                    if (responseInJson == null) break;

                    Packet response;
                    try
                    {
                        response = JsonConvert.DeserializeObject<Packet>(responseInJson);
                        if (response == null) continue;
                    }
                    catch
                    {
                        continue;
                    }

            switch (response.Code)
            {
                case 0:
                    if (context != null)
                        context.Post(_ => generate_room_status(response), null);
                    else
                        try { this.Invoke((MethodInvoker)(() => generate_room_status(response))); } catch { }
                    break;
                case 1:
                    if (context != null)
                        context.Post(_ => join_room_status(response), null);
                    else
                        try { this.Invoke((MethodInvoker)(() => join_room_status(response))); } catch { }
                    break;
                case 2:
                    if (context != null)
                        context.Post(_ => sync_bitmap_status(response), null);
                    else
                        try { this.Invoke((MethodInvoker)(() => sync_bitmap_status(response))); } catch { }
                    break;
                case 3:
                    if (context != null)
                        context.Post(_ => draw_bitmap_handler(response), null);
                    else
                        try { this.Invoke((MethodInvoker)(() => draw_bitmap_handler(response))); } catch { }
                    break;
                case 4:
                    draw_graphics_handler(response); 
                    break;
                case 5:
                    break;
            }
        }
    }
    catch (Exception ex)
    {
                if (!isClosing)
                {
                    // log check
                }
            }
            finally
            {
                try
                {
                    reader?.Dispose();
                    writer?.Dispose();
                    client?.Close();
                }
                catch { }
            }
        }

        private void InvokeOnUIThread(Action action)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(action);
                }
                catch (ObjectDisposedException)
                {
                    //logcheck
                }
            }
            else
            {
                action();
            }
        }

        private byte[] SnapshotBitmap(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
        private Bitmap RestoreBitmap(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return new Bitmap(ms);
            }
        }
        private void PushState()
        {
            if (bitmap == null || Canvas.Image == null) return;

            try
            {
                byte[] currentState = SnapshotBitmap(bitmap);
                undoStack.Push(currentState);

                if (undoStack.Count > MaxHistory)
                {
                    var temp = new Stack<byte[]>();
                    for (int i = 0; i < MaxHistory; i++)
                    {
                        if (undoStack.Count > 0)
                            temp.Push(undoStack.Pop());
                    }

                    while (undoStack.Count > 0)
                        undoStack.Pop();

                    undoStack = new Stack<byte[]>(temp);
                }

                redoStack.Clear();

                UpdateUndoRedoButtons();
            }
            catch (Exception ex)
            {
            }
        }
        private Bitmap CloneBitmapSafe(Bitmap src)
        {
            if (src == null) return null;

            var pixelFormat = src.PixelFormat;
            if ((pixelFormat & System.Drawing.Imaging.PixelFormat.Indexed) != 0)
            {
                pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            }

            Bitmap clone = new Bitmap(src.Width, src.Height, pixelFormat);
            try
            {
                using (Graphics g = Graphics.FromImage(clone))
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.SmoothingMode = SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(src, 0, 0, src.Width, src.Height);
                }
            }
            catch
            {
                clone.Dispose();
                throw;
            }
            return clone;
        }
        void generate_room_status(Packet response)
        {
            this_client_info.RoomID = response.RoomID;
            Manager.UpdateRoomID(this_client_info.RoomID);

            if (!isOffline && chatPanel != null && int.TryParse(response.RoomID, out int roomCode))
            {
                chatPanel.SetRoomCode(roomCode);
                InitializeChatPanelWithRoom(roomCode);
            }

            isNew = false;
        }

        private void InitializeChatPanelWithRoom(int roomCode)
        {
            if (chatPanel == null || Manager == null) return;

            try
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        int userId = await Manager.GetUserIdByUsernameAsync(this_client_info.Username);
                        
                        if (userId > 0)
                        {
                            InvokeOnUIThread(() =>
                            {
                                chatPanel.Initialize(Manager, userId, this_client_info.Username, roomCode);
                            });
                        }
                        else
                        {
                            // log check
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }
        void join_room_status(Packet response)
        {
            if (isNew)
            {
                sendToServer(new Packet
                {
                    Code = 2,
                    RoomID = response.RoomID,
                });
                isNew = false;
            }

            if (response.Username == "err:thisroomdoesnotexist")
            {
                Manager.ShowError("The room you requested does not exist");
                client.Close();
                this.Close();
                return;
            }

            if (!isOffline && chatPanel != null && int.TryParse(response.RoomID?.ToString(), out int roomCode))
            {
                chatPanel.SetRoomCode(roomCode);
                InitializeChatPanelWithRoom(roomCode);
            }

            if (response.Username.Contains('!'))
            {
                Manager.RemoveFromUserListView(response.Username.Substring(1));
            }
            else
            {
                List<string> list = response.Username.Split(',').ToList();
                
                if (!isNew && bitmap != null)
                {
                    try
                    {
                        Packet syncPacket = new Packet
                        {
                            Code = 3,
                            RoomID = response.RoomID,
                            BitmapString = Manager.BitmapToString(bitmap),
                        };
                        sendToServer(syncPacket);
                    }
                    catch (Exception ex)
                    {
                        // log check
                    }
                }
                
                Manager.ClearUserListView();
                foreach (string u in list)
                {
                    Manager.AddToUserListView(u);
                }

            }
        }
        private void sync_bitmap_status(Packet response)
        {
            try
            {
                if (response == null || string.IsNullOrEmpty(response.BitmapString))
                {
                    return;
                }
                
                Bitmap receivedBitmap = Manager.StringToBitmap(response.BitmapString);
                if (receivedBitmap == null)
                {
                    return;
                }
                
                bitmap = receivedBitmap;
                graphics?.Dispose();
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Canvas.Image = bitmap;
                Canvas.Invalidate();
                
                // log check
            }
            catch (Exception ex)
            {
                // log check
            }
        }
        private void draw_bitmap_handler(Packet response)
{
    try
    {
        if (response == null || string.IsNullOrEmpty(response.BitmapString))
        {
            // log check
            return;
        }
        
        Bitmap _bitmap = Manager.StringToBitmap(response.BitmapString);
        if (_bitmap == null)
        {
            // log check
            return;
        }
        
        bitmap = _bitmap;
        graphics = Graphics.FromImage(bitmap);
        Canvas.Image = bitmap;
        
        if (context != null)
        {
            context.Post(_ => { Canvas.Refresh(); }, null);
        }
        else
        {
            try { Canvas.Invoke((MethodInvoker)(() => Canvas.Refresh())); } catch { }
        }
    }
    catch (Exception ex)
    {
        // log check
    }
}        
        void draw_graphics_handler(Packet response)
        {
            if (response.Username == currentUsername) return;

            if (context != null)
            {
                context.Post(_ =>
                {
                    try
                    {
                        Color pColor = Color.FromName(response.PenColor);
                        float pWidth = response.PenWidth;
                        
                        using (Pen p = new Pen(pColor, pWidth))
                        {
                            p.StartCap = LineCap.Round;
                            p.EndCap = LineCap.Round;
                            p.LineJoin = LineJoin.Round;

                            using (Graphics g = Graphics.FromImage(this.bitmap)) 
                            {
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                if (response.ShapeTag == 10)
                                {
                                    if (response.Points_1 != null && response.Points_2 != null)
                                    {
                                        int count = Math.Min(response.Points_1.Count, response.Points_2.Count);
                                        for (int i = 0; i < count; i++)
                                        {
                                            g.DrawLine(p, response.Points_1[i], response.Points_2[i]);
                                        }
                                    }
                                }
                                else 
                                {
                                    int x = (int)response.Position[0];
                                    int y = (int)response.Position[1];
                                    float w = response.Position[2];
                                    float h = response.Position[3];

                                    if (response.ShapeTag == 11) // Line
                                        g.DrawLine(p, x, y, x + (int)w, y + (int)h);
                                    else if (response.ShapeTag == 12) // Rectangle
                                        g.DrawRectangle(p, x, y, w, h);
                                    else if (response.ShapeTag == 13) // Ellipse
                                        g.DrawEllipse(p, x, y, w, h);
                                    else if (response.ShapeTag == 99) // Clear
                                        g.Clear(Color.White);
                                }
                            }
                        }

                        Canvas.Image = this.bitmap;
                        Canvas.Invalidate(); // Lighter than Refresh()
                    }
                    catch (Exception ex)
                    {
                        // log check
                    }
                }, null);
            }
            else
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            Color pColor = Color.FromName(response.PenColor);
                            float pWidth = response.PenWidth;
                            
                            using (Pen p = new Pen(pColor, pWidth))
                            {
                                p.StartCap = LineCap.Round;
                                p.EndCap = LineCap.Round;
                                p.LineJoin = LineJoin.Round;

                                using (Graphics g = Graphics.FromImage(this.bitmap)) 
                                {
                                    g.SmoothingMode = SmoothingMode.AntiAlias;

                                    if (response.ShapeTag == 10)
                                    {
                                        if (response.Points_1 != null && response.Points_2 != null)
                                        {
                                            int count = Math.Min(response.Points_1.Count, response.Points_2.Count);
                                            for (int i = 0; i < count; i++)
                                            {
                                                g.DrawLine(p, response.Points_1[i], response.Points_2[i]);
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        int x = (int)response.Position[0];
                                        int y = (int)response.Position[1];
                                        float w = response.Position[2];
                                        float h = response.Position[3];

                                        if (response.ShapeTag == 11)
                                            g.DrawLine(p, x, y, x + (int)w, y + (int)h);
                                        else if (response.ShapeTag == 12)
                                            g.DrawRectangle(p, x, y, w, h);
                                        else if (response.ShapeTag == 13)
                                            g.DrawEllipse(p, x, y, w, h);
                                        else if (response.ShapeTag == 99)
                                            g.Clear(Color.White);
                                    }
                                }
                            }

                            Canvas.Image = this.bitmap;
                            Canvas.Invalidate();
                        }
                        catch (Exception ex)
                        {
                            // log check
                        }
                    });
                }
                catch { }
            }
        }
        

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (isImageEditMode)
            {
                Canvas_MouseDown_Resize(sender, e);
                return;
            }

            if (graphics == null) SafeInitializeDrawing();

            if (currentTool != "Polygon")
            {
                PushState();
            }

            isDrawing = true;
            startPoint = e.Location;
            endPoint = startPoint;

            if (currentTool == "Fill")
            {
                Bitmap b = (Bitmap)Canvas.Image;
                Color targetColor = b.GetPixel(e.X, e.Y);
                Color newColor = ptbColor.BackColor;

                FloodFill(b, e.Location, targetColor, newColor);
                Canvas.Refresh();

                isDrawing = false;
                SyncBitmapToServer();
            }
            else if (currentTool == "Polygon")
            {
                HandlePolygonMouseDown(e);
            }
            else if (currentTool == "Line" || currentTool == "Rectangle"
                  || currentTool == "Ellipse" || currentTool == "Bezier")
            {
                tempBmp?.Dispose();
                if (bitmap == null) SafeInitializeDrawing();

                tempBmp = CloneBitmapSafe(bitmap);
                Canvas.Image = tempBmp;
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isImageEditMode)
            {
                Canvas_MouseMove_Resize(sender, e);
                return;
            }

            if (!isDrawing || graphics == null) return;
            endPoint = e.Location;

            if (currentTool == "Pen" || currentTool == "Eraser")
            {
                double distance = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                if (distance < 2.0) return;

                Color drawColor = (currentTool == "Pen") ? pen.Color : Color.White;
                float sentWidth = (currentTool == "Eraser") ? pen.Width + 4f : pen.Width;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLine(startPoint, endPoint);
                    using (Pen smoothPen = new Pen(drawColor, sentWidth)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round,
                        LineJoin = LineJoin.Round
                    })
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.DrawPath(smoothPen, path);
                    }
                }
                try
                {
                    if (!isOffline && writer != null)
                    {
                        var drawPkt = new Packet
                        {
                            Code = 4,
                            RoomID = this_client_info.RoomID,
                            Username = this_client_info.Username,
                            PenColor = drawColor.Name,
                            PenWidth = sentWidth,
                            ShapeTag = 10, // 10 = Pen/Line Segment
                            Points_1 = new List<Point> { startPoint },
                            Points_2 = new List<Point> { endPoint }
                        };
                        sendToServer(drawPkt);
                    }
                    startPoint = endPoint;
                    Canvas.Invalidate();
                }
                catch (Exception ex)
                { // log check }

                    startPoint = endPoint;
                    Canvas.Invalidate();
                }
            }
            else
            {
                tempBmp?.Dispose();

                if (bitmap == null) SafeInitializeDrawing();
                try
                {
                    tempBmp = CloneBitmapSafe(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tạo bản vẽ tạm thời: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (Graphics tempG = Graphics.FromImage(tempBmp))
                {
                    tempG.SmoothingMode = SmoothingMode.AntiAlias;
                    switch (currentTool)
                    {
                        case "Line": tempG.DrawLine(pen, startPoint, endPoint); break;
                        case "Rectangle": tempG.DrawRectangle(pen, GetRectangle(startPoint, endPoint)); break;
                        case "Ellipse": tempG.DrawEllipse(pen, GetRectangle(startPoint, endPoint)); break;
                        case "Bezier":
                            tempG.DrawBezier(pen,
                                startPoint,
                                new Point(startPoint.X + 50, startPoint.Y - 50),
                                new Point(endPoint.X - 50, endPoint.Y + 50),
                                endPoint);
                            break;
                    }
                }
                Canvas.Image = tempBmp;
            }
        }
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (isImageEditMode)
            {
                Canvas_MouseUp_Resize(sender, e);
                return;
            }

            if (!isDrawing || graphics == null) return;
            isDrawing = false;
            endPoint = e.Location;

            switch (currentTool)
            {
                case "Pen":
                case "Eraser":
                    break;

                case "Line":
                    graphics.DrawLine(pen, startPoint, endPoint);
                    TrySendShape(11, new float[] {
                startPoint.X, startPoint.Y,
                endPoint.X - startPoint.X,
                endPoint.Y - startPoint.Y
            });
                    break;

                case "Rectangle":
                    var rect = GetRectangle(startPoint, endPoint);
                    graphics.DrawRectangle(pen, rect);
                    TrySendShape(12, new float[] {
                rect.X, rect.Y, rect.Width, rect.Height
            });
                    break;

                case "Ellipse":
                    var ellipse = GetRectangle(startPoint, endPoint);
                    graphics.DrawEllipse(pen, ellipse);
                    TrySendShape(13, new float[] {
                ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height
            });
                    break;

                case "Bezier":
                    graphics.DrawBezier(pen,
                        startPoint,
                        new Point(startPoint.X + 50, startPoint.Y - 50),
                        new Point(endPoint.X - 50, endPoint.Y + 50),
                        endPoint);
                    SyncBitmapToServer();
                    break;
            }
            Canvas.Image = bitmap;
            Canvas.Refresh();

            tempBmp?.Dispose();
            tempBmp = null;
        }
        private void Canvas_MouseDown_Resize(object sender, MouseEventArgs e)
        {
            if (loadedImage == null) return;
            if (Math.Abs(e.X - imageRect.Left) < 10 && Math.Abs(e.Y - imageRect.Top) < 10)
                currentAnchor = ResizeAnchor.TopLeft;
            else if (Math.Abs(e.X - imageRect.Right) < 10 && Math.Abs(e.Y - imageRect.Top) < 10)
                currentAnchor = ResizeAnchor.TopRight;
            else if (Math.Abs(e.X - imageRect.Left) < 10 && Math.Abs(e.Y - imageRect.Bottom) < 10)
                currentAnchor = ResizeAnchor.BottomLeft;
            else if (Math.Abs(e.X - imageRect.Right) < 10 && Math.Abs(e.Y - imageRect.Bottom) < 10)
                currentAnchor = ResizeAnchor.BottomRight;

            if (currentAnchor != ResizeAnchor.None)
            {
                isResizing = true;
                lastMousePos = e.Location;
            }
            else if (imageRect.Contains(e.Location))
            {
                isDragging = true;
                lastMousePos = e.Location;
            }
        }
        private void Canvas_MouseMove_Resize(object sender, MouseEventArgs e)
        {
            if (loadedImage == null) return;

            if (isResizing)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                switch (currentAnchor)
                {
                    case ResizeAnchor.TopLeft:
                        imageRect.X += dx;
                        imageRect.Y += dy;
                        imageRect.Width -= dx;
                        imageRect.Height -= dy;
                        break;
                    case ResizeAnchor.TopRight:
                        imageRect.Y += dy;
                        imageRect.Width += dx;
                        imageRect.Height -= dy;
                        break;
                    case ResizeAnchor.BottomLeft:
                        imageRect.X += dx;
                        imageRect.Width -= dx;
                        imageRect.Height += dy;
                        break;
                    case ResizeAnchor.BottomRight:
                        imageRect.Width += dx;
                        imageRect.Height += dy;
                        break;
                }

                lastMousePos = e.Location;
                Canvas.Invalidate();
            }
            else if (isDragging)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;
                imageRect.X += dx;
                imageRect.Y += dy;
                lastMousePos = e.Location;
                Canvas.Invalidate();
            }
        }
        private void Canvas_MouseUp_Resize(object sender, MouseEventArgs e)
        {
            isResizing = false;
            isDragging = false;
            currentAnchor = ResizeAnchor.None;
        }
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            if (loadedImage != null)
            {
                e.Graphics.DrawImage(loadedImage, imageRect);
                using (Pen penBorder = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(penBorder, imageRect);
                }
            }
        }

        private void HandlePolygonMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (polygonPoints.Count > 2)
                {
                    PushState();
                    graphics.DrawPolygon(pen, polygonPoints.ToArray());
                    polygonPoints.Clear();
                    Canvas.Refresh();
                    SyncBitmapToServer();
                }
                return;
            }

            // Lưu state chỉ khi bắt đầu polygon mới
            if (polygonPoints.Count == 0)
            {
                PushState();
            }

            polygonPoints.Add(e.Location);
            if (polygonPoints.Count > 1)
            {
                graphics.DrawLine(pen,
                    polygonPoints[polygonPoints.Count - 2],
                    polygonPoints[polygonPoints.Count - 1]);
                Canvas.Refresh();
                TrySendPolygonSegment();
            }
        }

        private void TrySendShape(int shapeTag, float[] pos)
        {
            try
            {
                if (!isOffline && writer != null)
                {
                    var p = new Packet
                    {
                        Code = 4,
                        RoomID = this_client_info.RoomID,
                        Username = this_client_info.Username,
                        PenColor = pen.Color.Name,
                        PenWidth = pen.Width,
                        ShapeTag = shapeTag,
                        Position = pos
                    };
                    sendToServer(p);
                }
            }
            catch { }
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y)
            );
        }
        private void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color newColor)
        {
            if (targetColor.ToArgb() == newColor.ToArgb()) return;

            bool[,] visited = new bool[bmp.Width, bmp.Height];
            Queue<Point> pixels = new Queue<Point>();
            pixels.Enqueue(pt);

            while (pixels.Count > 0)
            {
                Point p = pixels.Dequeue();
                if (p.X < 0 || p.Y < 0 || p.X >= bmp.Width || p.Y >= bmp.Height) continue;
                if (visited[p.X, p.Y]) continue;

                Color currentColor = bmp.GetPixel(p.X, p.Y);
                if (currentColor.ToArgb() == targetColor.ToArgb())
                {
                    bmp.SetPixel(p.X, p.Y, newColor);
                    visited[p.X, p.Y] = true;

                    pixels.Enqueue(new Point(p.X + 1, p.Y));
                    pixels.Enqueue(new Point(p.X - 1, p.Y));
                    pixels.Enqueue(new Point(p.X, p.Y + 1));
                    pixels.Enqueue(new Point(p.X, p.Y - 1));
                }
            }
        }

        private readonly object writerLock = new object();
        private void sendToServer(Packet message)
        {
            if (writer == null) return;

            lock (writerLock)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(message);
                    writer.WriteLine(json);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    // log check
                }
            }
        }



        private void UpdateUndoRedoButtons()
        {
            try
            {
                if (Button_Undo != null)
                    Button_Undo.Enabled = undoStack.Count > 0;
                if (Button_Redo != null)
                    Button_Redo.Enabled = redoStack.Count > 0;
            }
            catch { }
        }

        private void PenOptimizer(Pen p)
        {
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;
            p.LineJoin = LineJoin.Round;
        }

        private void TrySendPolygonSegment()
        {
            if (polygonPoints.Count < 2) return;

            try
            {
                if (!isOffline && writer != null)
                {
                    var p = new Packet
                    {
                        Code = 4,
                        RoomID = this_client_info.RoomID,
                        Username = this_client_info.Username,
                        PenColor = pen.Color.Name,
                        PenWidth = pen.Width,
                        ShapeTag = 10,
                        Points_1 = new List<Point> { polygonPoints[polygonPoints.Count - 2] },
                        Points_2 = new List<Point> { polygonPoints[polygonPoints.Count - 1] }
                    };
                    sendToServer(p);
                }
            }
            catch { }
        }

        private void SyncBitmapToServer()
        {
            try
            {
                if (!isOffline && writer != null && bitmap != null)
                {
                    var p = new Packet
                    {
                        Code = 3,
                        RoomID = this_client_info.RoomID,
                        BitmapString = Manager.BitmapToString(bitmap)
                    };
                    sendToServer(p);
                }
            }
            catch { }
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

        public void SetUsername(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                currentUsername = username;
            }
        }

        public void SetUserId(int userId)
        {
            currentUserId = userId;
            if (this_client_info != null)
                this_client_info.UserId = userId;
        }

        public void LoadBitmap(Bitmap loadedBitmap, int projectId, string projectName)
        {
            if (loadedBitmap == null) return;

            try
            {
                if (bitmap != null && Canvas.Image == bitmap)
                {
                    Canvas.Image = null;
                }
                bitmap?.Dispose();
                graphics?.Dispose();

                bitmap = new Bitmap(loadedBitmap.Width, loadedBitmap.Height);
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                graphics.DrawImage(loadedBitmap, 0, 0);

                Canvas.Image = bitmap;
                Canvas.Size = new Size(bitmap.Width, bitmap.Height);
                Canvas.Refresh();

                currentProjectId = projectId;
                currentProjectName = projectName;

                this.Text = $"KayArt - {projectName}";

                // log check
            }
            catch (Exception ex)
            {
                // log check
                MessageBox.Show($"Error loading project: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isClosing = true;

            try
            {
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    if (!receiveThread.Join(1000))
                    {
                        try { receiveThread.Abort(); } catch { }
                    }
                }

                try { reader?.Close(); } catch { }
                try { writer?.Close(); } catch { }
                try { client?.Close(); } catch { }
                graphics?.Dispose();
                bitmap?.Dispose();
                tempBmp?.Dispose();
                loadedImage?.Dispose();
                pen?.Dispose();
                fillBrush?.Dispose();
            }
            catch (Exception ex)
            {
                // log check
            }

            base.OnFormClosing(e);
        }



        private void Button_Home_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Return to home?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) this.Close();
        }

        private void Button_New_Click(object sender, EventArgs e)
        {
            PushState();
            graphics.Clear(Color.White);
            Canvas.Refresh();
            SyncBitmapToServer();
        }

        private void Button_Open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    PushState();
                    loadedImage = Image.FromFile(ofd.FileName);
                    imageRect = new Rectangle(50, 50, loadedImage.Width, loadedImage.Height);
                    isImageEditMode = true;
                    Canvas.Invalidate();
                }
            }
        }

        private async void Button_Save_Click(object sender, EventArgs e)
        {
            using (Form saveDialog = new Form())
            {
                saveDialog.Text = "Save Project";
                saveDialog.Size = new Size(400, 200);
                saveDialog.StartPosition = FormStartPosition.CenterParent;
                saveDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                saveDialog.MaximizeBox = false;
                saveDialog.MinimizeBox = false;
                saveDialog.BackColor = Color.FromArgb(45, 45, 48);
                Label lblPrompt = new Label()
                {
                    Text = "Choose where to save your project:",
                    Location = new Point(30, 30),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                };
                Button btnDatabase = new Button()
                {
                    Text = "💾 Save to Database",
                    Location = new Point(30, 70),
                    Size = new Size(150, 50),
                    BackColor = Color.FromArgb(100, 149, 237),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Tag = "database"
                };
                btnDatabase.FlatAppearance.BorderSize = 0;
                btnDatabase.DialogResult = DialogResult.Yes;
                Button btnLocal = new Button()
                {
                    Text = "📁 Save to File",
                    Location = new Point(210, 70),
                    Size = new Size(150, 50),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Tag = "local"
                };
                btnLocal.FlatAppearance.BorderSize = 0;
                btnLocal.DialogResult = DialogResult.No;
                Button btnCancel = new Button()
                {
                    Text = "❌ Cancel",
                    Location = new Point(120, 130),
                    Size = new Size(150, 35),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10)
                };
                btnCancel.FlatAppearance.BorderSize = 0;
                btnCancel.DialogResult = DialogResult.Cancel;

                saveDialog.Controls.AddRange(new Control[] {
                    lblPrompt, btnDatabase, btnLocal, btnCancel
                });

                DialogResult result = saveDialog.ShowDialog(this);

                if (result == DialogResult.Yes)
                {
                    await SaveToDatabase();
                }
                else if (result == DialogResult.No)
                {
                    SaveToLocalFile();
                }
            }
        }

        private async Task SaveToDatabase()
        {
            if (currentUsername == "Guest")
            {
                MessageBox.Show("Cannot save to database in Guest mode!\nPlease login first.",
                    "Guest Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string projectName = ShowProjectNameDialog();
                if (string.IsNullOrEmpty(projectName))
                    return;

                string imageData = DatabaseHelper.BitmapToBase64(bitmap);
                string thumbnailData = DatabaseHelper.GenerateThumbnailBase64(bitmap, 180, 140);
                if (Manager == null)
                {
                    Manager = new Manager(null, null);
                }

                await Manager.ConnectDataChannelAsync(ConfigHelper.GetServerIP(), 9999);
                int userId = await Manager.GetUserIdByUsernameAsync(currentUsername);

                if (userId <= 0)
                {
                    MessageBox.Show("Could not find user in database!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (currentProjectId > 0)
                {
                    bool success = await Manager.UpdateProjectImageAsync(
                        currentProjectId, imageData, thumbnailData);

                    if (success)
                    {
                        MessageBox.Show($"Project '{currentProjectName}' updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // log check
                    }
                    else
                    {
                        MessageBox.Show("Failed to update project.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    int newProjectId = await Manager.CreateProjectAsync(
                        userId, projectName, imageData, thumbnailData,
                        bitmap.Width, bitmap.Height);

                    if (newProjectId > 0)
                    {
                        currentProjectId = newProjectId;
                        currentProjectName = projectName;
                        this.Text = $"KayArt - {projectName}";

                        MessageBox.Show($"Project '{projectName}' saved successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // log check
                    }
                    else
                    {
                        MessageBox.Show("Failed to create project.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // log check
                MessageBox.Show($"Error saving to database:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveToLocalFile()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                sfd.FileName = !string.IsNullOrEmpty(currentProjectName) && currentProjectName != "Untitled"
                    ? currentProjectName
                    : "MyDrawing";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        bitmap.Save(sfd.FileName,
                            sfd.FilterIndex == 1 ? ImageFormat.Png : ImageFormat.Jpeg);

                        MessageBox.Show($"Saved to:\n{sfd.FileName}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // log check
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file:\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string ShowProjectNameDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Project Name";
                dialog.Size = new Size(400, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(45, 45, 48);

                Label label = new Label()
                {
                    Text = "Enter project name:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11)
                };

                TextBox textBox = new TextBox()
                {
                    Location = new Point(20, 50),
                    Width = 340,
                    Font = new Font("Segoe UI", 11),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = !string.IsNullOrEmpty(currentProjectName) && currentProjectName != "Untitled"
                        ? currentProjectName
                        : ""
                };

                Button btnSave = new Button()
                {
                    Text = "Save",
                    Location = new Point(150, 95),
                    Width = 90,
                    Height = 35,
                    BackColor = Color.Green,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                btnSave.FlatAppearance.BorderSize = 0;

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

                dialog.Controls.AddRange(new Control[] { label, textBox, btnSave, btnCancel });
                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                textBox.Focus();
                textBox.SelectAll();

                DialogResult result = dialog.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    string name = textBox.Text.Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("Project name cannot be empty!", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                    return name;
                }

                return null;
            }
        }

        private void Button_Undo_Click(object sender, EventArgs e)
        {
            if (undoStack.Count == 0) return;
            redoStack.Push(SnapshotBitmap(bitmap));
            bitmap = RestoreBitmap(undoStack.Pop());
            graphics?.Dispose();
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Canvas.Image = bitmap;
            Canvas.Refresh();
            UpdateUndoRedoButtons();
            SyncBitmapToServer();
        }

        private void Button_Redo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count == 0) return;
            undoStack.Push(SnapshotBitmap(bitmap));
            bitmap = RestoreBitmap(redoStack.Pop());
            graphics?.Dispose();
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Canvas.Image = bitmap;
            Canvas.Refresh();
            UpdateUndoRedoButtons();
            SyncBitmapToServer();
        }

        private void Button_Select_Click(object sender, EventArgs e)
        {
            if (isImageEditMode && loadedImage != null)
            {
                graphics.DrawImage(loadedImage, imageRect);
                Canvas.Image = bitmap;
                Canvas.Refresh();
                loadedImage?.Dispose();
                loadedImage = null;
                isImageEditMode = false;
                SyncBitmapToServer();
            }
        }

        private void Button_Pen_Click(object sender, EventArgs e)
        {
            currentTool = "Pen";
        }

        private void Button_Fill_Click(object sender, EventArgs e)
        {
            currentTool = "Fill";
        }

        private void Button_Eraser_Click(object sender, EventArgs e)
        {
            currentTool = "Eraser";
        }

        private void Button_Clear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Clear canvas?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                PushState();
                graphics.Clear(Color.White);
                Canvas.Refresh();
                if (!isOffline && writer != null)
                {
                    sendToServer(new Packet { Code = 4, RoomID = this_client_info.RoomID, ShapeTag = 99 });
                }
            }
        }

        private void Button_Line_Click(object sender, EventArgs e)
        {
            currentTool = "Line";
        }

        private void Button_Bezier_Click(object sender, EventArgs e)
        {
            currentTool = "Bezier";
        }

        private void Button_Rectangle_Click(object sender, EventArgs e)
        {
            currentTool = "Rectangle";
        }

        private void Button_Ellipse_Click(object sender, EventArgs e)
        {
            currentTool = "Ellipse";
        }

        private void Button_Polygon_Click(object sender, EventArgs e)
        {
            currentTool = "Polygon";
        }

        private void Button_ChangeColor_Click(object sender, EventArgs e)
        {
            PictureBox clickedBox = sender as PictureBox;
            if (clickedBox != null)
            {
                pen.Color = clickedBox.BackColor;
                fillBrush.Color = clickedBox.BackColor;
                if (ptbColor != null) ptbColor.BackColor = clickedBox.BackColor;
                if (ptbColor1 != null) ptbColor1.BackColor = clickedBox.BackColor;
            }
        }

        private void Button_EditColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = pen.Color;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    pen.Color = cd.Color;
                    fillBrush.Color = cd.Color;
                    if (ptbColor != null) ptbColor.BackColor = cd.Color;
                    if (ptbColor1 != null) ptbColor1.BackColor = cd.Color;
                }
            }
        }

        private void Button_LineSize_Scroll(object sender, EventArgs e)
        {
            if (Button_LineSize != null) pen.Width = Button_LineSize.Value;
        }

        private void Form_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void InitializeChatPanel()
        {
            try
            {
                // log check
                
                chatPanelContainer = new Panel
                {
                    Dock = DockStyle.Right,
                    Width = 350,
                    BackColor = Color.FromArgb(30, 30, 35),
                    Visible = true,
                    Name = "chatPanelContainer"
                };
                // log check

                chatPanel = new ChatPanel
                {
                    Dock = DockStyle.Fill,
                    Name = "chatPanel"
                };
                // log check

                chatPanelContainer.Controls.Add(chatPanel);
                // log check

                this.Controls.Add(chatPanelContainer);
                chatPanelContainer.BringToFront();
                
                chatPanelContainer.Visible = true;
                chatPanel.Visible = true;
                
                // log check
                
                this.PerformLayout();
                this.Refresh();
                
                // log check
            }
            catch (Exception ex)
            {
                // log check
                MessageBox.Show($"Failed to initialize chat panel: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Helper Methods

        private string BitmapToBase64(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private string GenerateThumbnailBase64(Bitmap bitmap, int maxWidth = 200, int maxHeight = 150)
        {
            if (bitmap == null) return null;
            float ratio = Math.Min((float)maxWidth / bitmap.Width, (float)maxHeight / bitmap.Height);
            int newWidth = (int)(bitmap.Width * ratio);
            int newHeight = (int)(bitmap.Height * ratio);
            using (Bitmap thumbnail = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                }
                return BitmapToBase64(thumbnail);
            }
        }

        #endregion

        #region AI Enhancement

        private void InitializeAIEnhanceButton()
        {
            btnAIEnhance = new Button
            {
                Text = "✨ AI Enhance",
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(138, 43, 226), // Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAIEnhance.FlatAppearance.BorderSize = 0;
            btnAIEnhance.Click += BtnAIEnhance_Click;

            btnAIEnhance.Location = new Point(this.ClientSize.Width - 150, 80);

            this.Controls.Add(btnAIEnhance);
            btnAIEnhance.BringToFront();

            // log check
        }

        private async void BtnAIEnhance_Click(object sender, EventArgs e)
        {
            if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
            {
                MessageBox.Show("Canvas is empty! Draw something first.", "AI Enhance",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var result = MessageBox.Show(
                "Transform your drawing with AI?\n\nThis will use Google Cloud Vertex AI.\nNote: May take 5-10 seconds.",
                "AI Enhancement",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;
            btnAIEnhance.Enabled = false;
            btnAIEnhance.Text = "⏳ Processing...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string apiKey = ConfigHelper.GetStabilityApiKey();

                if (string.IsNullOrEmpty(apiKey))
                {
                    MessageBox.Show("Stability AI API Key not configured!\n\nAdd your key to appsettings.json",
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var aiService = new GeminiVertexService(apiKey);
                await aiService.InitializeAsync();

                Bitmap enhanced = await aiService.EnhanceImageAsync(bitmap);

                if (enhanced != null)
                {
                    ShowEnhancedImageDialog(enhanced);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AI Enhancement failed:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAIEnhance.Enabled = true;
                btnAIEnhance.Text = "✨ AI Enhance";
                this.Cursor = Cursors.Default;
            }
        }

        private void ShowEnhancedImageDialog(Bitmap enhancedImage)
        {
            Form dialog = new Form
            {
                Text = "AI Enhanced Result",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(30, 30, 35)
            };

            PictureBox pictureBox = new PictureBox
            {
                Image = enhancedImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(10)
            };

            Button btnApply = new Button
            {
                Text = "✅ Apply",
                Size = new Size(100, 40),
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += (s, e) =>
            {
                bitmap = new Bitmap(enhancedImage);
                this.Invalidate();
                dialog.Close();
            };

            Button btnClose = new Button
            {
                Text = "❌ Close",
                Size = new Size(100, 40),
                Location = new Point(120, 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => dialog.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnApply, btnClose });
            dialog.Controls.AddRange(new Control[] { pictureBox, buttonPanel });

            dialog.ShowDialog(this);
        }

        #endregion

    }
}