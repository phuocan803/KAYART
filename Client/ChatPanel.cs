using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Database;

namespace Client
{
    public partial class ChatPanel : UserControl
    {

        private Manager dataManager;
        private int currentUserId = -1;
        private string currentUsername = "Guest";
        private int? currentRoomCode = null;

        // UI Controls
        private TabControl tabControl;
        
        // Tab Friends
        private FlowLayoutPanel friendsListPanel;
        private TextBox txtSearchFriends;
        private Button btnRefreshFriends;
        
        // Tab Private Chat
        private RichTextBox rtbChatMessages;
        private TextBox txtChatMessage;
        // Removed: private Button btnSendMessage; - Not used (Enter key sends message)
        private Label lblChatWith;
        
        // Tab Room Chat
        private RichTextBox rtbRoomChatMessages;
        private TextBox txtRoomMessage;
        // Removed: private Button btnSendRoomMessage; - Not used (Enter key sends message)
        private Label lblRoomInfo;
        private FlowLayoutPanel roomMembersPanel;
        private Button btnRefreshRoom; // Declared for future use

        // State
        private int selectedFriendId = -1;
        private string selectedFriendName = "";
        private List<FriendInfo> cachedFriends = new List<FriendInfo>();
        
        // Auto-refresh timers
        private System.Windows.Forms.Timer friendsRefreshTimer;
        private System.Windows.Forms.Timer chatRefreshTimer;
        private System.Windows.Forms.Timer roomRefreshTimer;
        
        // Thread safety
        private readonly object stateLock = new object();
        private bool isDisposing = false;
        private SynchronizationContext syncContext;


        #region Constructor & Initialization

        public ChatPanel()
        {
            components = new System.ComponentModel.Container();
            InitializeComponent();
            syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            
            InitializeUI();
            InitializeTimers();
        }

        private void InitializeUI()
        {
            // Create TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Alignment = TabAlignment.Top,
                Padding = new Point(2, 2)
            };
            
            // Create tabs
            TabPage friendsTab = CreateFriendsTab();
            TabPage chatTab = CreateChatTab();
            TabPage roomTab = CreateRoomChatTab();
            
            tabControl.TabPages.AddRange(new TabPage[] { friendsTab, chatTab, roomTab });
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            
            this.Controls.Add(tabControl);
        }

        private TabPage CreateFriendsTab()
        {
            TabPage tab = new TabPage("Friends")
            {
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0)
            };

            // Search panel
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(40, 40, 45),
                Padding = new Padding(5)
            };

            txtSearchFriends = new TextBox
            {
                Location = new Point(5, 10),
                Width = 230,
                Height = 25,
                BackColor = Color.FromArgb(50, 50, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "Search users..."
            };
            txtSearchFriends.KeyDown += TxtSearchFriends_KeyDown;

            btnRefreshFriends = new Button
            {
                Location = new Point(240, 10),
                Width = 50,
                Height = 25,
                Text = "",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F),
                Cursor = Cursors.Hand
            };
            btnRefreshFriends.FlatAppearance.BorderSize = 0;
            btnRefreshFriends.Click += BtnRefreshFriends_Click;

            searchPanel.Controls.AddRange(new Control[] { txtSearchFriends, btnRefreshFriends });

            // Friends list - FIXED: Set minimum height to prevent empty space
            friendsListPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0, 0, 0, 0),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false // Prevent auto-sizing issues
            };
            tab.Controls.AddRange(new Control[] { friendsListPanel, searchPanel });
            // tab.Controls.AddRange(new Control[] { searchPanel, friendsListPanel });
            // Hoặc an toàn nhất là dùng lệnh này:
            searchPanel.SendToBack();       // Đẩy panel tìm kiếm xuống đáy Z-order (được tính toán layout trước)
            friendsListPanel.BringToFront(); // Đẩy list lên đầu Z-order (được tính toán layout sau cùng -> chỉ fill phần còn dư)
            return tab;
        }

        //private TabPage CreateChatTab()
        //{
        //    TabPage tab = new TabPage("Chat")
        //    {
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // Chat area (Full width - no left panel)
        //    Panel chatPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(5)
        //    };

        //    // Header with friend name
        //    Panel headerPanel = new Panel
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 30,
        //        BackColor = Color.FromArgb(50, 50, 55),
        //        Padding = new Padding(5, 3, 5, 3)
        //    };

        //    lblChatWith = new Label
        //    {
        //        Text = "Select a friend from Friends tab to chat",
        //        Dock = DockStyle.Fill,
        //        TextAlign = ContentAlignment.MiddleLeft,
        //        ForeColor = Color.White,
        //        Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        //    };

        //    headerPanel.Controls.Add(lblChatWith);

        //    // Messages display
        //    rtbChatMessages = new RichTextBox
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        ReadOnly = true,
        //        Font = new Font("Segoe UI", 8F),
        //        ScrollBars = RichTextBoxScrollBars.Vertical,
        //        Padding = new Padding(5)
        //    };

        //    // Input panel - UPDATED: Full width input, no Send button
        //    //Panel inputPanel = new Panel
        //    //{
        //    //    Dock = DockStyle.Bottom,
        //    //    Height = 38,
        //    //    BackColor = Color.FromArgb(45, 45, 50),
        //    //    Padding = new Padding(3)
        //    //};

        //    Panel inputPanel = new Panel
        //    {
        //        Dock = DockStyle.Bottom, 
        //        Height = 50,            
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    txtChatMessage = new TextBox
        //    {
        //        Location = new Point(3, 6),
        //        Width = inputPanel.Width - 9, // Full width
        //        Height = 25,
        //        BackColor = Color.FromArgb(50, 50, 55),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.FixedSingle,
        //        Font = new Font("Segoe UI", 8F),
        //        PlaceholderText = "Press Enter to send...",
        //        Enabled = false,
        //        Dock = DockStyle.Fill
        //    };
        //    txtChatMessage.KeyDown += TxtChatMessage_KeyDown;

        //    // Remove Send button - input takes full width
        //    btnSendMessage = null; // Not used anymore

        //    inputPanel.Controls.Add(txtChatMessage);
        //    //chatPanel.Controls.AddRange(new Control[] { headerPanel, rtbChatMessages, inputPanel });

        //    chatPanel.Controls.AddRange(new Control[] { rtbChatMessages, headerPanel, inputPanel });

        //    inputPanel.SendToBack();      // Tính toán Bottom trước
        //    headerPanel.SendToBack();     // Tính toán Top tiếp theo
        //    rtbChatMessages.BringToFront(); // Fill vào khoảng trống còn lại

        //   // tab.Controls.Add(chatPanel);
        //    return tab;
        //}

        private TabPage CreateChatTab()
        {
            TabPage tab = new TabPage("Chat")
            {
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0)
            };

            // 1. Panel chứa toàn bộ nội dung (Container) -> Cái này mới là Dock.Fill
            Panel chatMainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0)
            };

            // 2. Header (Tên người đang chat) -> Dock.Top
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,  // QUAN TRỌNG: Dính lên trên cùng
                Height = 40,           // Chiều cao cố định
                BackColor = Color.FromArgb(50, 50, 55),
                Padding = new Padding(10, 0, 0, 0) // Căn lề trái chữ cho đẹp
            };

            lblChatWith = new Label
            {
                Text = "Select a friend to chat",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            headerPanel.Controls.Add(lblChatWith);

            // 3. Input Panel (Ô nhập liệu) -> Dock.Bottom (KHÔNG ĐƯỢC LÀ FILL)
            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Bottom, // QUAN TRỌNG: Dính xuống đáy
                Height = 50,             // Chiều cao cố định
                BackColor = Color.FromArgb(45, 45, 50),
                Padding = new Padding(5)
            };

            txtChatMessage = new TextBox
            {
                Dock = DockStyle.Fill,   // TextBox fill đầy cái inputPanel
                Multiline = true,        // Cho phép xuống dòng nếu muốn
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Type a message... (Enter to send)"
            };
            txtChatMessage.KeyDown += TxtChatMessage_KeyDown;
            inputPanel.Controls.Add(txtChatMessage);

            // 4. Khung hiển thị tin nhắn -> Dock.Fill (Nằm giữa)
            rtbChatMessages = new RichTextBox
            {
                Dock = DockStyle.Fill, // QUAN TRỌNG: Fill vào khoảng trống GIỮA Top và Bottom
                BackColor = Color.FromArgb(40, 40, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(10)
            };

            // --- 5. LẮP RÁP VÀ XỬ LÝ Z-ORDER (QUAN TRỌNG NHẤT) ---

            // Add tất cả vào Container chính
            chatMainContainer.Controls.AddRange(new Control[] {
        headerPanel,      // Top
        inputPanel,       // Bottom
        rtbChatMessages   // Fill
    });

            // Ép thứ tự hiển thị để không bị đè:
            // Quy tắc: Những thằng chiếm chỗ (Top/Bottom) phải được SendToBack (ưu tiên xếp chỗ trước)
            // Thằng Fill phải được BringToFront (điền vào chỗ trống còn lại)

            headerPanel.SendToBack();
            inputPanel.SendToBack();
            rtbChatMessages.BringToFront();

            // Cuối cùng add Container vào Tab
            tab.Controls.Add(chatMainContainer);

            return tab;
        }

        //private TabPage CreateRoomChatTab()
        //{
        //    TabPage tab = new TabPage("Room")
        //    {
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // Top panel - Room info
        //    Panel topPanel = new Panel
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 40,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    lblRoomInfo = new Label
        //    {
        //        Location = new Point(5, 5),
        //        Width = 150,
        //        Height = 18,
        //        Text = "Not in a room",
        //        ForeColor = Color.White,
        //        Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        //    };

        //    btnCopyRoomCode = new Button
        //    {
        //        Location = new Point(160, 5),
        //        Width = 75,
        //        Height = 28,
        //        Text = "Copy",
        //        BackColor = Color.FromArgb(70, 130, 180),
        //        ForeColor = Color.White,
        //        FlatStyle = FlatStyle.Flat,
        //        Font = new Font("Segoe UI", 7F),
        //        Visible = false,
        //        Cursor = Cursors.Hand
        //    };
        //    btnCopyRoomCode.FlatAppearance.BorderSize = 0;
        //    btnCopyRoomCode.Click += BtnCopyRoomCode_Click;

        //    btnRefreshRoom = new Button
        //    {
        //        Location = new Point(240, 5),
        //        Width = 40,
        //        Height = 28,
        //        Text = "",
        //        BackColor = Color.FromArgb(100, 149, 237),
        //        ForeColor = Color.White,
        //        FlatStyle = FlatStyle.Flat,
        //        Font = new Font("Segoe UI", 7F),
        //        Visible = false,
        //        Cursor = Cursors.Hand
        //    };
        //    btnRefreshRoom.FlatAppearance.BorderSize = 0;
        //    btnRefreshRoom.Click += BtnRefreshRoom_Click;

        //    topPanel.Controls.AddRange(new Control[] { lblRoomInfo, btnCopyRoomCode, btnRefreshRoom });

        //    // Split view: Chat (left) + Members (right)
        //    Panel chatPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(3)
        //    };

        //    rtbRoomChatMessages = new RichTextBox
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        ReadOnly = true,
        //        Font = new Font("Segoe UI", 8F),
        //        ScrollBars = RichTextBoxScrollBars.Vertical
        //    };

        //    // Input panel - UPDATED: Full width input, no Send button
        //    Panel roomInputPanel = new Panel
        //    {
        //        Dock = DockStyle.Bottom,
        //        Height = 38,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(3)
        //    };

        //    txtRoomMessage = new TextBox
        //    {
        //        Location = new Point(3, 6),
        //        Height = 25,
        //        BackColor = Color.FromArgb(50, 50, 55),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.FixedSingle,
        //        Font = new Font("Segoe UI", 8F),
        //        PlaceholderText = "Press Enter to send...",
        //        Enabled = false,
        //        Dock = DockStyle.Fill
        //    };
        //    txtRoomMessage.KeyDown += TxtRoomMessage_KeyDown;

        //    // Remove Send button - input takes full width
        //    btnSendRoomMessage = null; // Not used anymore

        //    roomInputPanel.Controls.Add(txtRoomMessage);
        //    chatPanel.Controls.AddRange(new Control[] { rtbRoomChatMessages, roomInputPanel });

        //    // Members panel (right side)
        //    Panel membersPanel = new Panel
        //    {
        //        Dock = DockStyle.Right,
        //        Width = 95,
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        Padding = new Padding(2)
        //    };

        //    Label lblMembers = new Label
        //    {
        //        Dock = DockStyle.Top,
        //        Text = "Members",
        //        Height = 22,
        //        TextAlign = ContentAlignment.MiddleCenter,
        //        ForeColor = Color.White,
        //        Font = new Font("Segoe UI", 8F, FontStyle.Bold),
        //        BackColor = Color.FromArgb(50, 50, 55)
        //    };

        //    roomMembersPanel = new FlowLayoutPanel
        //    {
        //        Dock = DockStyle.Fill,
        //        AutoScroll = true,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        FlowDirection = FlowDirection.TopDown,
        //        WrapContents = false,
        //        Padding = new Padding(2)
        //    };

        //    membersPanel.Controls.AddRange(new Control[] { lblMembers, roomMembersPanel });

        //    tab.Controls.AddRange(new Control[] { topPanel, chatPanel, membersPanel });
        //    return tab;
        //}


        //private TabPage CreateRoomChatTab()
        //{
        //    // 1. Khởi tạo TabPage
        //    TabPage tab = new TabPage("Room")
        //    {
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // --- PHẦN 1: TẠO CÁC CONTROL CON (GIỮ NGUYÊN CODE CŨ) ---

        //    // A. Top Panel (Header hiển thị mã phòng) -> DOCK TOP
        //    Panel topPanel = new Panel
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 40,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    lblRoomInfo = new Label
        //    {
        //        Location = new Point(5, 10),
        //        AutoSize = true,
        //        Text = "Not in a room",
        //        ForeColor = Color.White,
        //        Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        //    };

        //    btnCopyRoomCode = new Button
        //    {
        //        Location = new Point(150, 5),
        //        Size = new Size(60, 30),
        //        Text = "Copy",
        //        BackColor = Color.FromArgb(70, 130, 180),
        //        ForeColor = Color.White,
        //        FlatStyle = FlatStyle.Flat,
        //        Visible = false
        //    };
        //    btnCopyRoomCode.Click += BtnCopyRoomCode_Click;

        //    btnRefreshRoom = new Button
        //    {
        //        Location = new Point(220, 5),
        //        Size = new Size(40, 30),
        //        Text = "↻",
        //        BackColor = Color.FromArgb(100, 149, 237),
        //        ForeColor = Color.White,
        //        FlatStyle = FlatStyle.Flat,
        //        Visible = false
        //    };
        //    btnRefreshRoom.Click += BtnRefreshRoom_Click;

        //    topPanel.Controls.AddRange(new Control[] { lblRoomInfo, btnCopyRoomCode, btnRefreshRoom });

        //    // B. Members Panel (Danh sách thành viên) -> DOCK RIGHT
        //    Panel membersPanel = new Panel
        //    {
        //        Dock = DockStyle.Right,
        //        Width = 120, // Độ rộng cố định
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        Padding = new Padding(1)
        //    };

        //    Label lblMembersHeader = new Label
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 25,
        //        Text = "Members",
        //        TextAlign = ContentAlignment.MiddleCenter,
        //        ForeColor = Color.DarkGray,
        //        Font = new Font("Segoe UI", 8F, FontStyle.Bold),
        //        BackColor = Color.FromArgb(30, 30, 35)
        //    };

        //    roomMembersPanel = new FlowLayoutPanel
        //    {
        //        Dock = DockStyle.Fill,
        //        AutoScroll = true,
        //        FlowDirection = FlowDirection.TopDown,
        //        WrapContents = false,
        //        BackColor = Color.FromArgb(40, 40, 45)
        //    };
        //    membersPanel.Controls.AddRange(new Control[] { roomMembersPanel, lblMembersHeader });
        //    // Lưu ý: Add header sau cùng trong mảng hoặc dùng SendToBack cho Header nếu muốn nó nằm trên cùng trong panel này

        //    // C. Chat Container (Chứa Chat + Input) -> DOCK FILL
        //    Panel chatPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // C1. Input Panel (Ô nhập liệu) -> DOCK BOTTOM
        //    Panel roomInputPanel = new Panel
        //    {
        //        Dock = DockStyle.Bottom,
        //        Height = 50,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    txtRoomMessage = new TextBox
        //    {
        //        Dock = DockStyle.Fill,
        //        Multiline = true,
        //        BackColor = Color.FromArgb(50, 50, 55),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        Font = new Font("Segoe UI", 10F),
        //        PlaceholderText = "Type into room...",
        //        Enabled = false
        //    };
        //    txtRoomMessage.KeyDown += TxtRoomMessage_KeyDown;
        //    roomInputPanel.Controls.Add(txtRoomMessage);

        //    // C2. RichTextBox (Tin nhắn) -> DOCK FILL
        //    rtbRoomChatMessages = new RichTextBox
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        ReadOnly = true,
        //        Font = new Font("Segoe UI", 9F),
        //        Padding = new Padding(10)
        //    };

        //    // --- PHẦN 2: LẮP RÁP LAYOUT & XỬ LÝ Z-ORDER (CODE BẠN YÊU CẦU) ---

        //    // 1. Xử lý bên trong Chat Panel trước
        //    chatPanel.Controls.AddRange(new Control[] { rtbRoomChatMessages, roomInputPanel });

        //    // Quy tắc: Bottom (Input) phải được tính toán chỗ trước -> SendToBack
        //    // Fill (RTB) điền vào chỗ còn lại -> BringToFront
        //    roomInputPanel.SendToBack();
        //    rtbRoomChatMessages.BringToFront();


        //    // 2. Xử lý Tab chính
        //    // Add tất cả vào Tab
        //    tab.Controls.AddRange(new Control[] { chatPanel, topPanel, membersPanel });

        //    // Quy tắc: 
        //    // - Top (Header) và Right (Members) phải được tính toán chỗ trước -> SendToBack
        //    // - Fill (ChatPanel) điền vào chỗ còn lại -> BringToFront
        //    topPanel.SendToBack();      // 1. Winforms tính toán vị trí Top Panel
        //    membersPanel.SendToBack();  // 2. Winforms tính toán vị trí Right Panel
        //    chatPanel.BringToFront();   // 3. ChatPanel fill vào khoảng trống ở giữa

        //    return tab;
        //}

        //private TabPage CreateRoomChatTab()
        //{
        //    // 1. Khởi tạo TabPage
        //    TabPage tab = new TabPage("Room")
        //    {
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // --- A. TOP PANEL (Header) ---
        //    // Chỉ giữ lại Label và nút Copy
        //    Panel topPanel = new Panel
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 40,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    lblRoomInfo = new Label
        //    {
        //        Location = new Point(5, 10),
        //        AutoSize = true,
        //        Text = "Not in a room", // Giá trị mặc định, sẽ thay đổi khi vào phòng
        //        ForeColor = Color.White,
        //        Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        //    };

        //    btnCopyRoomCode = new Button
        //    {
        //        Location = new Point(150, 5), // Nằm cạnh Label
        //        Size = new Size(60, 30),
        //        Text = "Copy",
        //        BackColor = Color.FromArgb(70, 130, 180),
        //        ForeColor = Color.White,
        //        FlatStyle = FlatStyle.Flat,
        //        Cursor = Cursors.Hand,
        //        Visible = false // Ẩn đi khi chưa vào phòng
        //    };
        //    btnCopyRoomCode.FlatAppearance.BorderSize = 0;
        //    btnCopyRoomCode.Click += BtnCopyRoomCode_Click;

        //    // Đã xóa btnRefreshRoom ở đây theo yêu cầu
        //    topPanel.Controls.AddRange(new Control[] { lblRoomInfo, btnCopyRoomCode });


        //    // --- B. MEMBERS PANEL (Bên Phải) ---
        //    // Giữ nguyên như cũ
        //    Panel membersPanel = new Panel
        //    {
        //        Dock = DockStyle.Right,
        //        Width = 140, // Mở rộng thêm xíu cho thoải mái tên
        //        BackColor = Color.FromArgb(40, 40, 45),
        //        Padding = new Padding(1)
        //    };

        //    Label lblMembersHeader = new Label
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 25,
        //        Text = "Members",
        //        TextAlign = ContentAlignment.MiddleCenter,
        //        ForeColor = Color.DarkGray,
        //        Font = new Font("Segoe UI", 8F, FontStyle.Bold),
        //        BackColor = Color.FromArgb(30, 30, 35)
        //    };

        //    roomMembersPanel = new FlowLayoutPanel
        //    {
        //        Dock = DockStyle.Fill,
        //        AutoScroll = true,
        //        FlowDirection = FlowDirection.TopDown,
        //        WrapContents = false,
        //        BackColor = Color.FromArgb(40, 40, 45)
        //    };
        //    // Header add vào sau (hoặc dùng SendToBack) để nằm trên cùng trong Panel con này
        //    membersPanel.Controls.AddRange(new Control[] { roomMembersPanel, lblMembersHeader });
        //    lblMembersHeader.SendToBack();


        //    // --- C. CHAT PANEL (Phần Giữa - Quan trọng nhất) ---
        //    // Panel này sẽ Fill vào khoảng trống còn lại
        //    Panel chatPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(35, 35, 40),
        //        Padding = new Padding(0)
        //    };

        //    // C1. Input Panel (Nằm dưới đáy của Chat Panel)
        //    Panel roomInputPanel = new Panel
        //    {
        //        Dock = DockStyle.Bottom,
        //        Height = 50,
        //        BackColor = Color.FromArgb(45, 45, 50),
        //        Padding = new Padding(5)
        //    };

        //    txtRoomMessage = new TextBox
        //    {
        //        Dock = DockStyle.Fill, // QUAN TRỌNG: Fill đầy chiều ngang của roomInputPanel
        //        Multiline = true,
        //        BackColor = Color.FromArgb(60, 60, 65),
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        Font = new Font("Segoe UI", 10F),
        //        PlaceholderText = "Type into room...",
        //        Enabled = false
        //    };
        //    txtRoomMessage.KeyDown += TxtRoomMessage_KeyDown;
        //    roomInputPanel.Controls.Add(txtRoomMessage);

        //    // C2. Nội dung chat (Fill phần còn lại của Chat Panel)
        //    rtbRoomChatMessages = new RichTextBox
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.FromArgb(35, 35, 40), // Cùng màu nền để đẹp hơn
        //        ForeColor = Color.White,
        //        BorderStyle = BorderStyle.None,
        //        ReadOnly = true,
        //        Font = new Font("Segoe UI", 9F),
        //        Padding = new Padding(10)
        //    };

        //    // Lắp ráp Chat Panel
        //    chatPanel.Controls.AddRange(new Control[] { rtbRoomChatMessages, roomInputPanel });
        //    roomInputPanel.SendToBack();        // Ưu tiên tính chỗ cho Input trước
        //    rtbRoomChatMessages.BringToFront(); // Chat fill chỗ còn lại


        //    // --- D. LẮP RÁP TỔNG THỂ VÀO TAB (Z-ORDER FIX) ---

        //    // Add 3 thành phần chính vào Tab
        //    tab.Controls.AddRange(new Control[] { chatPanel, topPanel, membersPanel });

        //    // Quy tắc hiển thị chuẩn:
        //    // 1. Top Panel (Header) tính trước -> SendToBack
        //    topPanel.SendToBack();

        //    // 2. Right Panel (Thành viên) tính tiếp theo -> SendToBack
        //    membersPanel.SendToBack();

        //    // 3. Chat Panel (Giữa) fill vào chỗ trống còn lại -> BringToFront
        //    chatPanel.BringToFront();

        //    return tab;
        //}

        private TabPage CreateRoomChatTab()
        {
            TabPage tab = new TabPage("Room")
            {
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0)
            };

            // --- 1. TOP PANEL (Header) ---
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(45, 45, 50),
                Padding = new Padding(5)
            };

            lblRoomInfo = new Label
            {
                Location = new Point(5, 10),
                AutoSize = true,
                Text = "Not in a room",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand // ✅ Make it look clickable
            };
            
            // ✅ NEW: Click to copy room code (silent)
            lblRoomInfo.Click += (s, e) =>
            {
                if (currentRoomCode.HasValue)
                {
                    Clipboard.SetText(currentRoomCode.Value.ToString());
                    // Silent copy - no message box
                }
            };

            topPanel.Controls.Add(lblRoomInfo);


            // --- 2. MEMBERS PANEL (Ở Dưới - Collapsible) ---
            Panel membersPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30, // ✅ Always show header (30px minimum)
                BackColor = Color.FromArgb(40, 40, 45),
                Padding = new Padding(10, 0, 10, 0) // ✅ Add horizontal padding
            };

            // ✅ Header IS the drag handle - always visible
            Label lblMembersHeader = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "▲ Members List (Drag to expand)",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(50, 50, 55),
                Cursor = Cursors.SizeNS,
                Padding = new Padding(5, 8, 5, 5)
            };

            // ✅ Add drag functionality to header with smooth animation
            bool isDragging = false;
            Point dragStartPoint = Point.Empty;
            int dragStartHeight = 0;

            lblMembersHeader.MouseDown += (s, e) =>
            {
                isDragging = true;
                dragStartPoint = e.Location;
                dragStartHeight = membersPanel.Height;
                membersPanel.SuspendLayout(); // ✅ Suspend layout for smoother dragging
            };

            lblMembersHeader.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    int deltaY = dragStartPoint.Y - e.Y;
                    int newHeight = dragStartHeight + deltaY;
                    
                    // Min: 30px (just header), Max: 400px
                    if (newHeight < 30) newHeight = 30;
                    if (newHeight > 400) newHeight = 400;
                    
                    // ✅ OPTIMIZED: Only update if height actually changed
                    if (membersPanel.Height != newHeight)
                    {
                        membersPanel.Height = newHeight;
                        
                        // ✅ NEW: Update text during drag to stay in sync
                        if (newHeight <= 35)
                        {
                            lblMembersHeader.Text = "▲ Members List (Drag to expand)";
                        }
                        else
                        {
                            lblMembersHeader.Text = "▼ Members List (Drag to collapse)";
                        }
                    }
                }
            };

            lblMembersHeader.MouseUp += (s, e) =>
            {
                if (isDragging)
                {
                    isDragging = false;
                    membersPanel.ResumeLayout(true); // ✅ Resume layout
                    
                    // ✅ NEW: Snap to predefined heights for better UX
                    int currentHeight = membersPanel.Height;
                    int targetHeight;
                    
                    if (currentHeight < 100)
                    {
                        targetHeight = 30; // Snap to collapsed
                    }
                    else if (currentHeight < 250)
                    {
                        targetHeight = 200; // Snap to medium
                    }
                    else
                    {
                        targetHeight = 400; // Snap to full
                    }
                    
                    // Smooth animation to snap point
                    AnimateToHeight(membersPanel, lblMembersHeader, targetHeight);
                }
            };

            // Members list panel
            roomMembersPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(40, 40, 45)
            };
            
            membersPanel.Controls.AddRange(new Control[] { roomMembersPanel, lblMembersHeader });
            lblMembersHeader.SendToBack(); // ✅ FIXED: Header at bottom so members list is visible


            // --- 3. CHAT PANEL (Ở Giữa - Fill) ---
            Panel chatPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 40),
                Padding = new Padding(0)
            };

            // Input nằm dưới đáy
            Panel roomInputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 50),
                Padding = new Padding(5)
            };

            txtRoomMessage = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Type into room...",
                Enabled = false
            };
            txtRoomMessage.KeyDown += TxtRoomMessage_KeyDown;
            roomInputPanel.Controls.Add(txtRoomMessage);

            // Chat Message nằm giữa
            rtbRoomChatMessages = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(10)
            };

            chatPanel.Controls.AddRange(new Control[] { rtbRoomChatMessages, roomInputPanel });
            roomInputPanel.SendToBack();
            rtbRoomChatMessages.BringToFront();


            // --- 4. LẮP RÁP (Z-ORDER) ---
            tab.Controls.AddRange(new Control[] { chatPanel, membersPanel, topPanel });

            topPanel.SendToBack();      // 1. Header ưu tiên chỗ trước
            membersPanel.SendToBack();  // 2. Members panel ở dưới
            chatPanel.BringToFront();   // 3. Chat panel điền vào chỗ còn lại

            return tab;
        }

        /// <summary>
        /// ✅ NEW: Smooth animation to target height with easing
        /// </summary>
        private void AnimateToHeight(Panel panel, Label header, int targetHeight)
        {
            int startHeight = panel.Height;
            if (startHeight == targetHeight) return;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 10 };
            int step = 0;
            const int totalSteps = 15; // 150ms total animation

            timer.Tick += (s, e) =>
            {
                step++;
                if (step >= totalSteps)
                {
                    panel.Height = targetHeight;
                    timer.Stop();
                    timer.Dispose();
                    
                    // Update header text
                    if (targetHeight <= 35)
                    {
                        header.Text = "▲ Members List (Drag to expand)";
                    }
                    else
                    {
                        header.Text = "▼ Members List (Drag to collapse)";
                    }
                }
                else
                {
                    // Easing function (ease-out cubic)
                    double progress = (double)step / totalSteps;
                    double eased = 1 - Math.Pow(1 - progress, 3);
                    
                    int newHeight = (int)(startHeight + (targetHeight - startHeight) * eased);
                    panel.Height = newHeight;
                }
            };
            timer.Start();
        }


        private void InitializeTimers()
        {
            friendsRefreshTimer = new System.Windows.Forms.Timer { Interval = 30000 };
            friendsRefreshTimer.Tick += async (s, e) => await LoadFriendsListAsync();

            chatRefreshTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            chatRefreshTimer.Tick += async (s, e) => await RefreshCurrentChatAsync();

            // ? FIXED: Reduce room refresh interval to 2 seconds for better real-time updates
            roomRefreshTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            roomRefreshTimer.Tick += async (s, e) => await RefreshRoomChatAsync();
        }

        #endregion

        #region Public Methods

        public void Initialize(Manager manager, int userId, string username, int? roomCode = null)
        {
            lock (stateLock)
            {
                this.dataManager = manager;
                this.currentUserId = userId;
                this.currentUsername = username;
                this.currentRoomCode = roomCode;
            }

            // ✅ FIXED: Only load data if we have valid user ID OR room code
            // If both are invalid, we're in offline mode - disable chat features
            if (userId > 0 || roomCode.HasValue)
            {
                _ = InitialLoadAsync();
                
                // ✅ NEW: If roomCode is provided, update UI to enable room chat
                if (roomCode.HasValue)
                {
                    UpdateRoomUI();
                }
            }
            else
            {
                // Offline mode - disable all chat features
                DisableChatFeatures();
                System.Diagnostics.Debug.WriteLine("[CHAT] Offline mode detected - chat features disabled");
            }
        }

        public void SetRoomCode(int roomCode)
        {
            lock (stateLock)
            {
                currentRoomCode = roomCode;
            }

            UpdateRoomUI();
            _ = LoadRoomChatAsync();
            roomRefreshTimer.Start();
        }

        public void ClearRoomCode()
        {
            lock (stateLock)
            {
                currentRoomCode = null;
            }

            roomRefreshTimer.Stop();
            UpdateRoomUI();
        }

        /// <summary>
        /// ✅ NEW: Update room members list
        /// </summary>
        public void UpdateRoomMembers(List<string> memberNames)
        {
            SafeInvoke(() =>
            {
                roomMembersPanel.SuspendLayout();
                roomMembersPanel.Controls.Clear();

                foreach (string memberName in memberNames)
                {
                    Label memberLabel = new Label
                    {
                        Text = $"👤 {memberName}",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9F),
                        Padding = new Padding(10, 5, 10, 5),
                        Margin = new Padding(0, 2, 0, 2)
                    };
                    roomMembersPanel.Controls.Add(memberLabel);
                }

                roomMembersPanel.ResumeLayout(true);
                System.Diagnostics.Debug.WriteLine($"[CHAT] Updated room members: {memberNames.Count} members");
            });
        }

        #endregion

        #region Data Loading

        private async Task InitialLoadAsync()
        {
            try
            {
                await LoadFriendsListAsync();
                if (currentRoomCode.HasValue)
                    await LoadRoomChatAsync();
                friendsRefreshTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Initial load error: {ex.Message}");
            }
        }

        private async Task LoadFriendsListAsync()
        {
            if (isDisposing || currentUserId <= 0) return;

            try
            {
                var friends = await dataManager.GetFriendListAsync(currentUserId);
                lock (stateLock)
                {
                    cachedFriends = friends;
                }
                UpdateFriendsUI(friends);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Load friends error: {ex.Message}");
            }
        }

        private async Task LoadRoomChatAsync()
        {
            if (isDisposing || !currentRoomCode.HasValue) return;

            try
            {
                var messages = await dataManager.GetRoomChatMessagesAsync(currentRoomCode.Value, 100);
                var members = await dataManager.GetRoomMembersAsync(currentRoomCode.Value);

                SafeInvoke(() =>
                {
                    DisplayRoomChatMessages(messages);
                    DisplayRoomMembers(members);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Load room chat error: {ex.Message}");
            }
        }

        #endregion

        #region UI Update

        private void UpdateFriendsUI(List<FriendInfo> friends)
        {
            SafeInvoke(() =>
            {
                friendsListPanel.SuspendLayout();
                friendsListPanel.Controls.Clear();

                foreach (var friend in friends.OrderByDescending(f => f.IsOnline).ThenBy(f => f.Username))
                {
                    friendsListPanel.Controls.Add(CreateFriendCard(friend));
                }

                if (friends.Count == 0)
                {
                    Label lbl = new Label { Text = "No friends yet", AutoSize = true, ForeColor = Color.Gray, Padding = new Padding(20) };
                    friendsListPanel.Controls.Add(lbl);
                }

                friendsListPanel.ResumeLayout(true);
            });
        }

        private Panel CreateFriendCard(FriendInfo friend)
        {
            // Get parent panel width directly
            int panelWidth = friendsListPanel.Width;
            if (panelWidth <= 0) panelWidth = 300;
            
            // Account for scrollbar (15px) + buffer (5px)
            int cardWidth = panelWidth - 20;
            if (cardWidth < 200) cardWidth = 200;
            
            Panel card = new Panel 
            { 
                Width = cardWidth,
                Height = 56, // Slightly increased for better spacing
                BackColor = Color.FromArgb(45, 45, 50), 
                Margin = new Padding(0, 0, 0, 0), // NO margin to prevent gaps
                Tag = friend.UserId // Store user ID for reference
            };

            PictureBox avatar = new PictureBox { Size = new Size(35, 35), Location = new Point(8, 10), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Gray };
            if (!string.IsNullOrEmpty(friend.AvatarBase64))
            {
                try
                {
                    var bmp = DatabaseHelper.Base64ToBitmap(friend.AvatarBase64);
                    if (bmp != null) avatar.Image = bmp;
                }
                catch { }
            }

            Label nameLabel = new Label { Text = friend.Username, Location = new Point(50, 8), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            Label statusLabel = new Label { Text = friend.IsOnline ? "Online" : "Offline", Location = new Point(50, 28), AutoSize = true, ForeColor = friend.IsOnline ? Color.LightGreen : Color.Gray, Font = new Font("Segoe UI", 7F) };

            Button chatBtn = new Button { Text = "", Location = new Point(card.Width - 38, 14), Size = new Size(30, 28), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand };
            chatBtn.FlatAppearance.BorderSize = 0;
            chatBtn.Click += (s, e) => OpenChatWithFriend(friend);

            card.Controls.AddRange(new Control[] { avatar, nameLabel, statusLabel, chatBtn });
            return card;
        }

        private void DisplayChatMessages(List<ChatMessage> messages)
        {
            rtbChatMessages.Clear();
            foreach (var msg in messages)
            {
                bool isMe = msg.FromUserId == currentUserId;
                string time = msg.SentAt.ToString("HH:mm");

                rtbChatMessages.SelectionColor = isMe ? Color.LightBlue : Color.LightGreen;
                rtbChatMessages.SelectionFont = new Font("Segoe UI", 9F, FontStyle.Bold);
                rtbChatMessages.AppendText($"[{time}] {msg.FromUsername}:\n");

                rtbChatMessages.SelectionColor = Color.White;
                rtbChatMessages.SelectionFont = new Font("Segoe UI", 9F);
                rtbChatMessages.AppendText($"{msg.Message}\n\n");
            }

            rtbChatMessages.SelectionStart = rtbChatMessages.Text.Length;
            rtbChatMessages.ScrollToCaret();
        }

        private void DisplayRoomChatMessages(List<RoomChatMessage> messages)
        {
            int scrollPos = rtbRoomChatMessages.SelectionStart;
            bool wasAtBottom = rtbRoomChatMessages.SelectionStart >= rtbRoomChatMessages.Text.Length - 10;

            rtbRoomChatMessages.Clear();
            foreach (var msg in messages)
            {
                bool isMe = msg.Username == currentUsername;
                string time = msg.SentAt.ToString("HH:mm");

                rtbRoomChatMessages.SelectionColor = isMe ? Color.LightBlue : Color.LightGreen;
                rtbRoomChatMessages.SelectionFont = new Font("Segoe UI", 9F, FontStyle.Bold);
                rtbRoomChatMessages.AppendText($"[{time}] {msg.Username}:\n");

                rtbRoomChatMessages.SelectionColor = Color.White;
                rtbRoomChatMessages.SelectionFont = new Font("Segoe UI", 9F);
                rtbRoomChatMessages.AppendText($"{msg.Message}\n\n");
            }

            if (wasAtBottom)
            {
                rtbRoomChatMessages.SelectionStart = rtbRoomChatMessages.Text.Length;
                rtbRoomChatMessages.ScrollToCaret();
            }
        }

        private void DisplayRoomMembers(List<RoomMemberInfo> members)
        {
            roomMembersPanel.SuspendLayout();
            roomMembersPanel.Controls.Clear();

            foreach (var member in members.OrderBy(m => m.Username))
            {
                Panel memberCard = new Panel { Width = roomMembersPanel.Width - 10, Height = 35, BackColor = Color.FromArgb(50, 50, 55), Margin = new Padding(1) };

                PictureBox avatar = new PictureBox { Size = new Size(28, 28), Location = new Point(2, 3), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Gray };
                if (!string.IsNullOrEmpty(member.AvatarBase64))
                {
                    try { avatar.Image = DatabaseHelper.Base64ToBitmap(member.AvatarBase64); }
                    catch { }
                }

                Label nameLabel = new Label { Text = member.Username.Length > 10 ? member.Username.Substring(0, 10) + ".." : member.Username, Location = new Point(33, 3), Width = memberCard.Width - 60, Height = 15, ForeColor = Color.White, Font = new Font("Segoe UI", 7F, FontStyle.Bold) };
                
                // ? FIXED: Show online/offline status with visual indicator
                Label statusLabel = new Label 
                { 
                    Text = member.IsOnline ? "? Online" : "? Offline", 
                    Location = new Point(33, 19), 
                    AutoSize = true, 
                    Font = new Font("Segoe UI", 7F),
                    ForeColor = member.IsOnline ? Color.LightGreen : Color.Gray
                };

                memberCard.Controls.AddRange(new Control[] { avatar, nameLabel, statusLabel });
                roomMembersPanel.Controls.Add(memberCard);
            }

            roomMembersPanel.ResumeLayout(true);
        }

        //private void UpdateRoomUI()
        //{
        //    SafeInvoke(() =>
        //    {
        //        if (currentRoomCode.HasValue)
        //        {
        //            lblRoomInfo.Text = $"Room: {currentRoomCode.Value}";
        //            btnCopyRoomCode.Visible = true;
        //            btnRefreshRoom.Visible = true;
        //            txtRoomMessage.Enabled = true;
        //        }
        //        else
        //        {
        //            lblRoomInfo.Text = "Not in a room";
        //            btnCopyRoomCode.Visible = false;
        //            btnRefreshRoom.Visible = false;
        //            txtRoomMessage.Enabled = false;
        //            rtbRoomChatMessages.Clear();
        //            roomMembersPanel.Controls.Clear();
        //        }
        //    });
        //}
        private void UpdateRoomUI()
{
    SafeInvoke(() =>
    {
        if (currentRoomCode.HasValue)
        {
            // --- KHI VÀO PHÒNG ---
            lblRoomInfo.Text = $"Room: {currentRoomCode.Value} (Click to copy)";
            
            // ✅ FIXED: Check null before accessing btnRefreshRoom
            if (btnRefreshRoom != null) btnRefreshRoom.Visible = false;
            

            
            txtRoomMessage.Enabled = true;
            System.Diagnostics.Debug.WriteLine($"[CHAT] Room chat ENABLED: Room={currentRoomCode.Value}, Enabled={txtRoomMessage.Enabled}");
        }
        else
        {
            // --- KHI THOÁT PHÒNG ---
            lblRoomInfo.Text = "Not in a room";
            

            
            // ✅ CHANGED: Collapse members panel to minimum height instead of hiding
            if (roomMembersPanel?.Parent != null)
            {
                ((Panel)roomMembersPanel.Parent).Height = 30; // Just show header
            }

            txtRoomMessage.Enabled = false;
            rtbRoomChatMessages.Clear();
            roomMembersPanel.Controls.Clear();
        }
    });
}


        #endregion

        #region Event Handlers

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            chatRefreshTimer.Stop();
            roomRefreshTimer.Stop();

            switch (tabControl.SelectedIndex)
            {
                case 1:
                    if (selectedFriendId > 0) chatRefreshTimer.Start();
                    break;
                case 2:
                    if (currentRoomCode.HasValue) { roomRefreshTimer.Start(); _ = LoadRoomChatAsync(); }
                    break;
            }
        }

        private async void BtnRefreshFriends_Click(object sender, EventArgs e) => await LoadFriendsListAsync();
        private async void TxtSearchFriends_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await SearchAndShowUsersAsync(); } }
        private async void TxtChatMessage_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await SendChatMessageAsync(); } }
        private async void TxtRoomMessage_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await SendRoomMessageAsync(); } }
        private async void BtnSendMessage_Click(object sender, EventArgs e) => await SendChatMessageAsync(); // DEPRECATED - use KeyDown instead
        private async void BtnSendRoomMessage_Click(object sender, EventArgs e) => await SendRoomMessageAsync(); // DEPRECATED - use KeyDown instead
        private void BtnCopyRoomCode_Click(object sender, EventArgs e) { if (currentRoomCode.HasValue) { Clipboard.SetText(currentRoomCode.Value.ToString()); MessageBox.Show($"Room code copied!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); } }
        private async void BtnRefreshRoom_Click(object sender, EventArgs e) => await LoadRoomChatAsync();

        #endregion

        #region Action Methods

        private async Task SearchAndShowUsersAsync()
        {
            string searchTerm = txtSearchFriends.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm)) { await LoadFriendsListAsync(); return; }

            try
            {
                var users = await dataManager.SearchUsersAsync(searchTerm, currentUserId, 10);

                SafeInvoke(() =>
                {
                    friendsListPanel.SuspendLayout();
                    friendsListPanel.Controls.Clear();

                    // Get panel width once
                    int panelWidth = friendsListPanel.Width;
                    if (panelWidth <= 0) panelWidth = 300;
                    int cardWidth = panelWidth - 20;
                    if (cardWidth < 200) cardWidth = 200;

                    foreach (var user in users)
                    {
                        Panel card = new Panel 
                        { 
                            Width = cardWidth,
                            Height = 56, // Consistent height
                            BackColor = Color.FromArgb(45, 45, 50), 
                            Margin = new Padding(0, 0, 0, 0), // NO margin
                            Tag = user.Id
                        };

                        PictureBox avatar = new PictureBox { Size = new Size(35, 35), Location = new Point(8, 10), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Gray };
                        if (!string.IsNullOrEmpty(user.AvatarBase64))
                        {
                            try { avatar.Image = DatabaseHelper.Base64ToBitmap(user.AvatarBase64); }
                            catch { }
                        }

                        Label nameLabel = new Label { Text = user.Username, Location = new Point(50, 8), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
                        Label emailLabel = new Label { Text = user.Email, Location = new Point(50, 28), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 7F) };

                        bool isFriend = cachedFriends.Any(f => f.UserId == user.Id);
                        Button actionBtn = new Button { Text = isFriend ? " Remove" : " Add", Location = new Point(card.Width - 73, 14), Size = new Size(65, 28), BackColor = isFriend ? Color.FromArgb(180, 50, 50) : Color.FromArgb(0, 150, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8F), Cursor = Cursors.Hand };
                        actionBtn.FlatAppearance.BorderSize = 0;

                        if (isFriend)
                        {
                            actionBtn.Click += async (s, e) => await RemoveFriendByIdAsync(user.Id, user.Username, actionBtn);
                        }
                        else
                        {
                            actionBtn.Click += async (s, e) => await AddFriendAsync(user, actionBtn);
                        }

                        card.Controls.AddRange(new Control[] { avatar, nameLabel, emailLabel, actionBtn });
                        friendsListPanel.Controls.Add(card);
                    }

                    if (users.Count == 0)
                    {
                        Label lbl = new Label { Text = "No users found", AutoSize = true, ForeColor = Color.Gray, Padding = new Padding(20) };
                        friendsListPanel.Controls.Add(lbl);
                    }

                    friendsListPanel.ResumeLayout(true);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Search error: {ex.Message}");
            }
        }

        private async Task AddFriendAsync(UserInfo user, Button button)
        {
            try
            {
                bool success = await dataManager.SendFriendRequestAsync(currentUserId, user.Id);
                if (success)
                {
                    button.Text = "Remove";
                    button.BackColor = Color.FromArgb(180, 50, 50);
                    MessageBox.Show($"Added {user.Username}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadFriendsListAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Add friend error: {ex.Message}");
            }
        }

        private async Task RemoveFriendByIdAsync(int userId, string username, Button button)
        {
            if (MessageBox.Show($"Remove {username}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                button.Text = "Add";
                button.BackColor = Color.FromArgb(0, 150, 0);
                var friendToRemove = cachedFriends.FirstOrDefault(f => f.UserId == userId);
                if (friendToRemove != null) cachedFriends.Remove(friendToRemove);
                MessageBox.Show($"Removed {username}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenChatWithFriend(FriendInfo friend)
        {
            selectedFriendId = friend.UserId;
            selectedFriendName = friend.Username;
            tabControl.SelectedIndex = 1;
            lblChatWith.Text = $"Chat with {selectedFriendName}";
            txtChatMessage.Enabled = true; // Enable input only
            _ = LoadChatMessagesAsync();
        }

        private async Task LoadChatMessagesAsync()
        {
            if (selectedFriendId <= 0) return;
            try
            {
                var messages = await dataManager.GetChatMessagesAsync(currentUserId, selectedFriendId, 50);
                SafeInvoke(() => DisplayChatMessages(messages));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Load messages error: {ex.Message}");
            }
        }

        private async Task SendChatMessageAsync()
        {
            string message = txtChatMessage.Text.Trim();
            if (string.IsNullOrEmpty(message) || selectedFriendId <= 0) return;

            try
            {
                bool success = await dataManager.SendChatMessageAsync(currentUserId, selectedFriendId, message);
                if (success) { txtChatMessage.Clear(); await LoadChatMessagesAsync(); }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Send message error: {ex.Message}");
            }
        }

        private async Task SendRoomMessageAsync()
        {
            if (!currentRoomCode.HasValue) return;
            string message = txtRoomMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            try
            {
                bool success = await dataManager.SaveRoomChatMessageAsync(currentRoomCode.Value, currentUserId, currentUsername, message);
                if (success) { txtRoomMessage.Clear(); await LoadRoomChatAsync(); }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT] Send room message error: {ex.Message}");
            }
        }

        private async Task RefreshCurrentChatAsync()
        {
            if (selectedFriendId > 0 && tabControl.SelectedIndex == 1) await LoadChatMessagesAsync();
        }

        private async Task RefreshRoomChatAsync()
        {
            if (currentRoomCode.HasValue && tabControl.SelectedIndex == 2) await LoadRoomChatAsync();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// ✅ UPDATED: Hide entire chat panel for offline mode
        /// </summary>
        private void DisableChatFeatures()
        {
            SafeInvoke(() =>
            {
                // ✅ CHANGED: Hide the entire ChatPanel instead of disabling individual controls
                this.Visible = false;

                // Stop all timers
                friendsRefreshTimer?.Stop();
                chatRefreshTimer?.Stop();
                roomRefreshTimer?.Stop();

                System.Diagnostics.Debug.WriteLine("[CHAT] Chat panel hidden (offline mode)");
            });
        }

        private void SafeInvoke(Action action)
        {
            if (isDisposing) return;
            if (InvokeRequired)
            {
                try { Invoke(action); }
                catch (ObjectDisposedException) { }
            }
            else
            {
                action();
            }
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            lock (stateLock)
            {
                isDisposing = true;
            }

            if (disposing)
            {
                friendsRefreshTimer?.Stop();
                chatRefreshTimer?.Stop();
                roomRefreshTimer?.Stop();

                friendsRefreshTimer?.Dispose();
                chatRefreshTimer?.Dispose();
                roomRefreshTimer?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
