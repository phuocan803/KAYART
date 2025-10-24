using System.Drawing;

namespace Server
{
    partial class Server
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
            label_room_count = new System.Windows.Forms.Label();
            textBox_room_count = new System.Windows.Forms.TextBox();
            button_start_server = new System.Windows.Forms.Button();
            button_stop_server = new System.Windows.Forms.Button();
            textBox_server_local_IP = new System.Windows.Forms.TextBox();
            listView_log = new System.Windows.Forms.ListView();
            button_get_server_IP = new System.Windows.Forms.Button();
            label_user_count = new System.Windows.Forms.Label();
            textBox_user_count = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // label_room_count
            // 
            label_room_count.AutoSize = true;
            label_room_count.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_room_count.ForeColor = Color.CornflowerBlue;
            label_room_count.Location = new Point(199, 421);
            label_room_count.Name = "label_room_count";
            label_room_count.Size = new Size(109, 19);
            label_room_count.TabIndex = 0;
            label_room_count.Text = "Available rooms";
            // 
            // textBox_room_count
            // 
            textBox_room_count.BackColor = SystemColors.Window;
            textBox_room_count.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_room_count.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox_room_count.ForeColor = SystemColors.HotTrack;
            textBox_room_count.Location = new Point(337, 419);
            textBox_room_count.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBox_room_count.Name = "textBox_room_count";
            textBox_room_count.ReadOnly = true;
            textBox_room_count.Size = new Size(71, 25);
            textBox_room_count.TabIndex = 1;
            // 
            // button_start_server
            // 
            button_start_server.BackColor = Color.CornflowerBlue;
            button_start_server.Cursor = System.Windows.Forms.Cursors.Hand;
            button_start_server.FlatAppearance.BorderSize = 0;
            button_start_server.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button_start_server.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_start_server.ForeColor = Color.White;
            button_start_server.Location = new Point(22, 60);
            button_start_server.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            button_start_server.Name = "button_start_server";
            button_start_server.Size = new Size(151, 53);
            button_start_server.TabIndex = 2;
            button_start_server.Text = "Start";
            button_start_server.UseVisualStyleBackColor = false;
            button_start_server.Click += button_start_server_Click;
            // 
            // button_stop_server
            // 
            button_stop_server.BackColor = Color.CornflowerBlue;
            button_stop_server.Cursor = System.Windows.Forms.Cursors.Hand;
            button_stop_server.FlatAppearance.BorderSize = 0;
            button_stop_server.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button_stop_server.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_stop_server.ForeColor = Color.White;
            button_stop_server.Location = new Point(22, 134);
            button_stop_server.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            button_stop_server.Name = "button_stop_server";
            button_stop_server.Size = new Size(151, 53);
            button_stop_server.TabIndex = 3;
            button_stop_server.Text = "Stop";
            button_stop_server.UseVisualStyleBackColor = false;
            button_stop_server.Click += button_stop_server_Click;
            // 
            // textBox_server_local_IP
            // 
            textBox_server_local_IP.BackColor = SystemColors.Window;
            textBox_server_local_IP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_server_local_IP.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox_server_local_IP.Location = new Point(22, 360);
            textBox_server_local_IP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBox_server_local_IP.Name = "textBox_server_local_IP";
            textBox_server_local_IP.ReadOnly = true;
            textBox_server_local_IP.Size = new Size(151, 25);
            textBox_server_local_IP.TabIndex = 4;
            // 
            // listView_log
            // 
            listView_log.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listView_log.Location = new Point(199, 13);
            listView_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            listView_log.Name = "listView_log";
            listView_log.Size = new Size(571, 398);
            listView_log.TabIndex = 4;
            listView_log.UseCompatibleStateImageBehavior = false;
            listView_log.View = System.Windows.Forms.View.List;
            // 
            // button_get_server_IP
            // 
            button_get_server_IP.BackColor = Color.CornflowerBlue;
            button_get_server_IP.Cursor = System.Windows.Forms.Cursors.Hand;
            button_get_server_IP.FlatAppearance.BorderSize = 0;
            button_get_server_IP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button_get_server_IP.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_get_server_IP.ForeColor = Color.White;
            button_get_server_IP.Location = new Point(22, 282);
            button_get_server_IP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            button_get_server_IP.Name = "button_get_server_IP";
            button_get_server_IP.Size = new Size(151, 61);
            button_get_server_IP.TabIndex = 5;
            button_get_server_IP.Text = "Get server's IP address";
            button_get_server_IP.UseVisualStyleBackColor = false;
            button_get_server_IP.Click += button_get_server_IP_Click;
            // 
            // label_user_count
            // 
            label_user_count.AutoSize = true;
            label_user_count.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_user_count.ForeColor = Color.CornflowerBlue;
            label_user_count.Location = new Point(491, 421);
            label_user_count.Name = "label_user_count";
            label_user_count.Size = new Size(94, 19);
            label_user_count.TabIndex = 6;
            label_user_count.Text = "Existing users";
            // 
            // textBox_user_count
            // 
            textBox_user_count.BackColor = SystemColors.Window;
            textBox_user_count.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_user_count.Font = new Font("Yu Gothic UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox_user_count.ForeColor = SystemColors.HotTrack;
            textBox_user_count.Location = new Point(610, 419);
            textBox_user_count.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBox_user_count.Name = "textBox_user_count";
            textBox_user_count.ReadOnly = true;
            textBox_user_count.Size = new Size(71, 25);
            textBox_user_count.TabIndex = 7;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = SystemColors.InactiveBorder;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new Size(782, 453);
            Controls.Add(textBox_user_count);
            Controls.Add(label_user_count);
            Controls.Add(button_get_server_IP);
            Controls.Add(textBox_server_local_IP);
            Controls.Add(listView_log);
            Controls.Add(button_stop_server);
            Controls.Add(button_start_server);
            Controls.Add(textBox_room_count);
            Controls.Add(label_room_count);
            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Server";
            Text = "KayArt";
            FormClosed += Server_FormClosed;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_room_count;
        private System.Windows.Forms.TextBox textBox_room_count;
        private System.Windows.Forms.Button button_start_server;
        private System.Windows.Forms.Button button_stop_server;
        private System.Windows.Forms.TextBox textBox_server_local_IP;
        private System.Windows.Forms.ListView listView_log;
        private System.Windows.Forms.Button button_get_server_IP;
        private System.Windows.Forms.Label label_user_count;
        private System.Windows.Forms.TextBox textBox_user_count;
    }
}

