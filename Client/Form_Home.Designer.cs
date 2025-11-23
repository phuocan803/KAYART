using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    partial class Form_Home
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            button_create_room = new Button();
            panel_left = new Panel();
            btnMyProjects = new Button();
            btnSharedWithMe = new Button();
            panel2 = new Panel();
            Button_Logout = new Button();
            panel1 = new Panel();
            lblUserEmail = new Label();
            lblUsername = new Label();
            Pic_Avt = new PictureBox();
            button_go_create_canvas = new Button();
            panel4 = new Panel();
            panel5 = new Panel();
            flowLayoutPanel_ActiveRooms = new FlowLayoutPanel();
            flowLayoutPanel_Projects = new FlowLayoutPanel();
            label3 = new Label();
            label2 = new Label();
            panel7 = new Panel();
            Button_join_room1 = new Button();
            panel6 = new Panel();
            label1 = new Label();
            chatPanelContainer = new Panel();
            panel_left.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Pic_Avt).BeginInit();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // button_create_room
            // 
            button_create_room.BackColor = Color.CornflowerBlue;
            button_create_room.Cursor = Cursors.Hand;
            button_create_room.FlatAppearance.BorderSize = 0;
            button_create_room.FlatStyle = FlatStyle.Flat;
            button_create_room.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button_create_room.ForeColor = Color.White;
            button_create_room.Location = new Point(206, 14);
            button_create_room.Margin = new Padding(3, 4, 3, 4);
            button_create_room.Name = "button_create_room";
            button_create_room.Size = new Size(150, 80);
            button_create_room.TabIndex = 2;
            button_create_room.Text = "Create Room";
            button_create_room.UseVisualStyleBackColor = false;
            button_create_room.Visible = false;
            button_create_room.Click += button_create_room_Click;
            // 
            // panel_left
            // 
            panel_left.BackColor = Color.FromArgb(37, 37, 38);
            panel_left.Controls.Add(btnMyProjects);
            panel_left.Controls.Add(btnSharedWithMe);
            panel_left.Controls.Add(panel2);
            panel_left.Controls.Add(panel1);
            panel_left.Dock = DockStyle.Left;
            panel_left.Location = new Point(0, 0);
            panel_left.Margin = new Padding(3, 4, 3, 4);
            panel_left.Name = "panel_left";
            panel_left.Size = new Size(276, 894);
            panel_left.TabIndex = 14;
            // 
            // btnMyProjects
            // 
            btnMyProjects.Cursor = Cursors.Hand;
            btnMyProjects.FlatAppearance.BorderSize = 0;
            btnMyProjects.FlatStyle = FlatStyle.Flat;
            btnMyProjects.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnMyProjects.ForeColor = Color.FromArgb(200, 200, 200);
            btnMyProjects.Location = new Point(13, 186);
            btnMyProjects.Margin = new Padding(4);
            btnMyProjects.Name = "btnMyProjects";
            btnMyProjects.Size = new Size(284, 61);
            btnMyProjects.TabIndex = 1;
            btnMyProjects.Text = "   My Projects";
            btnMyProjects.TextAlign = ContentAlignment.MiddleLeft;
            btnMyProjects.UseVisualStyleBackColor = true;
            btnMyProjects.Click += Button_MyProjects_Click;
            // 
            // btnSharedWithMe
            // 
            btnSharedWithMe.Cursor = Cursors.Hand;
            btnSharedWithMe.FlatAppearance.BorderSize = 0;
            btnSharedWithMe.FlatStyle = FlatStyle.Flat;
            btnSharedWithMe.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnSharedWithMe.ForeColor = Color.FromArgb(200, 200, 200);
            btnSharedWithMe.Location = new Point(13, 265);
            btnSharedWithMe.Margin = new Padding(4);
            btnSharedWithMe.Name = "btnSharedWithMe";
            btnSharedWithMe.Size = new Size(284, 61);
            btnSharedWithMe.TabIndex = 2;
            btnSharedWithMe.Text = "   Shared With Me";
            btnSharedWithMe.TextAlign = ContentAlignment.MiddleLeft;
            btnSharedWithMe.UseVisualStyleBackColor = true;
            btnSharedWithMe.Click += Button_SharedWithMe_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(30, 30, 30);
            panel2.Controls.Add(Button_Logout);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 781);
            panel2.Name = "panel2";
            panel2.Size = new Size(276, 113);
            panel2.TabIndex = 1;
            // 
            // Button_Logout
            // 
            Button_Logout.Anchor = AnchorStyles.None;
            Button_Logout.BackColor = Color.Firebrick;
            Button_Logout.Cursor = Cursors.Hand;
            Button_Logout.FlatAppearance.BorderSize = 0;
            Button_Logout.FlatStyle = FlatStyle.Flat;
            Button_Logout.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Button_Logout.ForeColor = Color.White;
            Button_Logout.Location = new Point(49, 28);
            Button_Logout.Margin = new Padding(3, 4, 3, 4);
            Button_Logout.Name = "Button_Logout";
            Button_Logout.Size = new Size(175, 51);
            Button_Logout.TabIndex = 16;
            Button_Logout.Text = "Logout";
            Button_Logout.UseVisualStyleBackColor = false;
            Button_Logout.Click += Button_Logout_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(30, 30, 30);
            panel1.Controls.Add(lblUserEmail);
            panel1.Controls.Add(lblUsername);
            panel1.Controls.Add(Pic_Avt);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(276, 150);
            panel1.TabIndex = 0;
            // 
            // lblUserEmail
            // 
            lblUserEmail.Font = new Font("Segoe UI", 9F);
            lblUserEmail.ForeColor = Color.FromArgb(160, 160, 160);
            lblUserEmail.Location = new Point(122, 77);
            lblUserEmail.Margin = new Padding(4, 0, 4, 0);
            lblUserEmail.Name = "lblUserEmail";
            lblUserEmail.Size = new Size(176, 31);
            lblUserEmail.TabIndex = 2;
            lblUserEmail.Text = "user@email.com";
            // 
            // lblUsername
            // 
            lblUsername.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;
            lblUsername.Location = new Point(122, 38);
            lblUsername.Margin = new Padding(4, 0, 4, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(176, 38);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username";
            // 
            // Pic_Avt
            // 
            Pic_Avt.Location = new Point(25, 27);
            Pic_Avt.Name = "Pic_Avt";
            Pic_Avt.Size = new Size(88, 88);
            Pic_Avt.TabIndex = 0;
            Pic_Avt.TabStop = false;
            Pic_Avt.Click += Button_AVT_click;
            // 
            // button_go_create_canvas
            // 
            button_go_create_canvas.BackColor = Color.CornflowerBlue;
            button_go_create_canvas.Cursor = Cursors.Hand;
            button_go_create_canvas.FlatAppearance.BorderSize = 0;
            button_go_create_canvas.FlatStyle = FlatStyle.Flat;
            button_go_create_canvas.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button_go_create_canvas.ForeColor = Color.White;
            button_go_create_canvas.Location = new Point(18, 14);
            button_go_create_canvas.Margin = new Padding(3, 4, 3, 4);
            button_go_create_canvas.Name = "button_go_create_canvas";
            button_go_create_canvas.Size = new Size(150, 80);
            button_go_create_canvas.TabIndex = 15;
            button_go_create_canvas.Text = "New Canvas";
            button_go_create_canvas.UseVisualStyleBackColor = false;
            button_go_create_canvas.Click += button_go_create_canvas_Click;
            // 
            // panel4
            // 
            panel4.AutoScroll = true;
            panel4.BackgroundImage = Properties.Resources.BG;
            panel4.BackgroundImageLayout = ImageLayout.Stretch;
            panel4.Controls.Add(panel5);
            panel4.Controls.Add(chatPanelContainer);
            panel4.Controls.Add(panel6);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(276, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(1350, 894);
            panel4.TabIndex = 17;
            // 
            // chatPanelContainer
            // 
            chatPanelContainer.BackColor = Color.FromArgb(30, 30, 35);
            chatPanelContainer.Dock = DockStyle.Right;
            chatPanelContainer.Location = new Point(1050, 0);
            chatPanelContainer.Name = "chatPanelContainer";
            chatPanelContainer.Size = new Size(300, 894);
            chatPanelContainer.TabIndex = 23;
            // 
            // panel5
            // 
            panel5.AutoScroll = true;
            panel5.BackColor = Color.FromArgb(29, 41, 99);
            panel5.BackgroundImageLayout = ImageLayout.Stretch;
            panel5.Controls.Add(flowLayoutPanel_ActiveRooms);
            panel5.Controls.Add(flowLayoutPanel_Projects);
            panel5.Controls.Add(label3);
            panel5.Controls.Add(label2);
            panel5.Controls.Add(panel7);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 150);
            panel5.Name = "panel5";
            panel5.Size = new Size(1050, 744);
            panel5.TabIndex = 17;
            // 
            // flowLayoutPanel_ActiveRooms
            // 
            flowLayoutPanel_ActiveRooms.AutoScroll = true;
            flowLayoutPanel_ActiveRooms.BackColor = Color.FromArgb(19, 30, 74);
            flowLayoutPanel_ActiveRooms.Location = new Point(26, 631);
            flowLayoutPanel_ActiveRooms.Name = "flowLayoutPanel_ActiveRooms";
            flowLayoutPanel_ActiveRooms.Size = new Size(1043, 165);
            flowLayoutPanel_ActiveRooms.TabIndex = 22;
            // 
            // flowLayoutPanel_Projects
            // 
            flowLayoutPanel_Projects.AutoScroll = true;
            flowLayoutPanel_Projects.BackColor = Color.FromArgb(19, 30, 74);
            flowLayoutPanel_Projects.Location = new Point(26, 231);
            flowLayoutPanel_Projects.Name = "flowLayoutPanel_Projects";
            flowLayoutPanel_Projects.Size = new Size(1043, 322);
            flowLayoutPanel_Projects.TabIndex = 20;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(22, 590);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(517, 38);
            label3.TabIndex = 19;
            label3.Text = "🚪 Active Rooms";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(22, 183);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(517, 38);
            label2.TabIndex = 18;
            label2.Text = "📁 My Projects";
            // 
            // panel7
            // 
            panel7.BackColor = Color.FromArgb(19, 30, 74);
            panel7.Controls.Add(Button_join_room1);
            panel7.Controls.Add(button_go_create_canvas);
            panel7.Controls.Add(button_create_room);
            panel7.Location = new Point(26, 23);
            panel7.Name = "panel7";
            panel7.Size = new Size(1043, 111);
            panel7.TabIndex = 16;
            // 
            // Button_join_room1
            // 
            Button_join_room1.BackColor = Color.CornflowerBlue;
            Button_join_room1.Cursor = Cursors.Hand;
            Button_join_room1.FlatAppearance.BorderSize = 0;
            Button_join_room1.FlatStyle = FlatStyle.Flat;
            Button_join_room1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            Button_join_room1.ForeColor = Color.White;
            Button_join_room1.Location = new Point(394, 14);
            Button_join_room1.Margin = new Padding(3, 4, 3, 4);
            Button_join_room1.Name = "Button_join_room1";
            Button_join_room1.Size = new Size(150, 80);
            Button_join_room1.TabIndex = 16;
            Button_join_room1.Text = "Join Room";
            Button_join_room1.UseVisualStyleBackColor = false;
            Button_join_room1.Visible = false;
            Button_join_room1.Click += button_join_click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(19, 30, 74);
            panel6.Controls.Add(label1);
            panel6.Dock = DockStyle.Top;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(1350, 150);
            panel6.TabIndex = 18;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(22, 18);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(517, 38);
            label1.TabIndex = 17;
            label1.Text = "Welcome back, User!";
            // 
            // Form_Home
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            BackgroundImage = Properties.Resources.BG;
            ClientSize = new Size(1626, 894);
            Controls.Add(panel4);
            Controls.Add(panel_left);
            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form_Home";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KayArt";
            WindowState = FormWindowState.Maximized;
            panel_left.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Pic_Avt).EndInit();
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button button_create_room;
        private System.Windows.Forms.Panel panel_left;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox Pic_Avt;
        private System.Windows.Forms.Label lblUserEmail;
        private System.Windows.Forms.Label lblUsername;
        private Panel panel2;
        private Button Button_Logout;
        private Button button_go_create_canvas;
        private Panel panel4;
        private Panel panel6;
        private Panel panel5;
        private Panel panel7;
        private Label label1;
        private Label label3;
        private Label label2;
        private FlowLayoutPanel flowLayoutPanel_ActiveRooms;
        private FlowLayoutPanel flowLayoutPanel_Projects;
        private Button btnSharedWithMe;
        private Button btnMyProjects;
        private Button Button_join_room1;
        private Panel chatPanelContainer;
    }
}
