using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    partial class Form_Client
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
            components = new System.ComponentModel.Container();
            panelTopMenu = new Panel();
            Button_Home = new Button();
            Button_Save = new Button();
            Button_Open = new Button();
            Button_New = new Button();
            panelMainToolbar = new Panel();
            labelActions = new Label();
            Button_Undo = new Button();
            Button_Redo = new Button();
            labelSelection = new Label();
            Button_Select = new Button();
            labelTools = new Label();
            Button_Pen = new Button();
            Button_Fill = new Button();
            Button_Eraser = new Button();
            Button_Clear = new Button();
            labelShapes = new Label();
            Button_Line = new Button();
            Button_Bezier = new Button();
            Button_Rectangle = new Button();
            Button_Ellipse = new Button();
            Button_Polygon = new Button();
            labelBrushSize = new Label();
            labelColors = new Label();
            panelColors = new Panel();
            ptbColor1 = new PictureBox();
            ptbColor2 = new PictureBox();
            panelColorPalette = new FlowLayoutPanel();
            colorBox_0 = new PictureBox();
            colorBox_1 = new PictureBox();
            colorBox_2 = new PictureBox();
            colorBox_3 = new PictureBox();
            colorBox_4 = new PictureBox();
            colorBox_5 = new PictureBox();
            colorBox_6 = new PictureBox();
            colorBox_7 = new PictureBox();
            colorBox_8 = new PictureBox();
            colorBox_9 = new PictureBox();
            ptbEditColor = new PictureBox();
            Button_LineSize = new TrackBar();
            toolTip1 = new ToolTip(components);
            Canvas = new PictureBox();
            btnMakeOnline = new Button();
            panelTopMenu.SuspendLayout();
            panelMainToolbar.SuspendLayout();
            panelColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbColor1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbColor2).BeginInit();
            panelColorPalette.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)colorBox_0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbEditColor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Button_LineSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            SuspendLayout();
           
            // 
            // panelTopMenu
            // 
            panelTopMenu.BackColor = Color.FromArgb(25, 25, 28);
            //panelTopMenu.Controls.Add(Button_Home);
            //panelTopMenu.Controls.Add(btnMakeOnline);

            panelTopMenu.Controls.Add(btnMakeOnline);
            panelTopMenu.Controls.Add(Button_Home);

            panelTopMenu.Controls.Add(Button_Save);
            panelTopMenu.Controls.Add(Button_Open);
            panelTopMenu.Controls.Add(Button_New);
            panelTopMenu.Dock = DockStyle.Top;
            panelTopMenu.Location = new Point(0, 0);
            panelTopMenu.Name = "panelTopMenu";
            panelTopMenu.Padding = new Padding(3);
            panelTopMenu.Size = new Size(1600, 38);
            panelTopMenu.TabIndex = 0;
            // 
            // Button_Home
            // 
            Button_Home.BackColor = Color.FromArgb(40, 40, 43);
            Button_Home.BackgroundImage = Properties.Resources.home;
            Button_Home.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Home.Cursor = Cursors.Hand;
            Button_Home.Dock = DockStyle.Right;
            Button_Home.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 75);
            Button_Home.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Home.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 63, 70);
            Button_Home.FlatStyle = FlatStyle.Flat;
            Button_Home.Location = new Point(1503, 3);
            Button_Home.Margin = new Padding(2);
            Button_Home.Name = "Button_Home";
            Button_Home.Padding = new Padding(6);
            Button_Home.Size = new Size(44, 32);
            Button_Home.TabIndex = 3;
            toolTip1.SetToolTip(Button_Home, "🏠 Back to Home (Esc)");
            Button_Home.UseVisualStyleBackColor = false;
            Button_Home.Click += Button_Home_Click;
            // 
            // Button_Save
            // 
            Button_Save.BackColor = Color.FromArgb(40, 40, 43);
            Button_Save.BackgroundImage = Properties.Resources.save;
            Button_Save.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Save.Cursor = Cursors.Hand;
            Button_Save.Dock = DockStyle.Left;
            Button_Save.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 75);
            Button_Save.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Save.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 63, 70);
            Button_Save.FlatStyle = FlatStyle.Flat;
            Button_Save.Location = new Point(91, 3);
            Button_Save.Margin = new Padding(2);
            Button_Save.Name = "Button_Save";
            Button_Save.Padding = new Padding(6);
            Button_Save.Size = new Size(44, 32);
            Button_Save.TabIndex = 2;
            toolTip1.SetToolTip(Button_Save, "💾 Save (Ctrl+S)");
            Button_Save.UseVisualStyleBackColor = false;
            Button_Save.Click += Button_Save_Click;
            // 
            // Button_Open
            // 
            Button_Open.BackColor = Color.FromArgb(40, 40, 43);
            Button_Open.BackgroundImage = Properties.Resources.open;
            Button_Open.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Open.Cursor = Cursors.Hand;
            Button_Open.Dock = DockStyle.Left;
            Button_Open.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 75);
            Button_Open.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Open.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 63, 70);
            Button_Open.FlatStyle = FlatStyle.Flat;
            Button_Open.Location = new Point(47, 3);
            Button_Open.Margin = new Padding(2);
            Button_Open.Name = "Button_Open";
            Button_Open.Padding = new Padding(6);
            Button_Open.Size = new Size(44, 32);
            Button_Open.TabIndex = 1;
            toolTip1.SetToolTip(Button_Open, "📂 Open (Ctrl+O)");
            Button_Open.UseVisualStyleBackColor = false;
            Button_Open.Click += Button_Open_Click;
            // 
            // Button_New
            // 
            Button_New.BackColor = Color.FromArgb(40, 40, 43);
            Button_New.BackgroundImage = Properties.Resources._new;
            Button_New.BackgroundImageLayout = ImageLayout.Zoom;
            Button_New.Cursor = Cursors.Hand;
            Button_New.Dock = DockStyle.Left;
            Button_New.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 75);
            Button_New.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_New.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 63, 70);
            Button_New.FlatStyle = FlatStyle.Flat;
            Button_New.Location = new Point(3, 3);
            Button_New.Margin = new Padding(2);
            Button_New.Name = "Button_New";
            Button_New.Padding = new Padding(6);
            Button_New.Size = new Size(44, 32);
            Button_New.TabIndex = 0;
            toolTip1.SetToolTip(Button_New, "📄 New (Ctrl+N)");
            Button_New.UseVisualStyleBackColor = false;
            Button_New.Click += Button_New_Click;
            // 
            // panelMainToolbar
            // 
            panelMainToolbar.BackColor = Color.FromArgb(37, 37, 40);
            panelMainToolbar.Controls.Add(labelActions);
            panelMainToolbar.Controls.Add(Button_Undo);
            panelMainToolbar.Controls.Add(Button_Redo);
            panelMainToolbar.Controls.Add(labelSelection);
            panelMainToolbar.Controls.Add(Button_Select);
            panelMainToolbar.Controls.Add(labelTools);
            panelMainToolbar.Controls.Add(Button_Pen);
            panelMainToolbar.Controls.Add(Button_Fill);
            panelMainToolbar.Controls.Add(Button_Eraser);
            panelMainToolbar.Controls.Add(Button_Clear);
            panelMainToolbar.Controls.Add(labelShapes);
            panelMainToolbar.Controls.Add(Button_Line);
            panelMainToolbar.Controls.Add(Button_Bezier);
            panelMainToolbar.Controls.Add(Button_Rectangle);
            panelMainToolbar.Controls.Add(Button_Ellipse);
            panelMainToolbar.Controls.Add(Button_Polygon);
            panelMainToolbar.Controls.Add(labelBrushSize);
            panelMainToolbar.Controls.Add(labelColors);
            panelMainToolbar.Controls.Add(panelColors);
            panelMainToolbar.Controls.Add(Button_LineSize);
            panelMainToolbar.Dock = DockStyle.Top;
            panelMainToolbar.Location = new Point(0, 38);
            panelMainToolbar.Name = "panelMainToolbar";
            panelMainToolbar.Padding = new Padding(3);
            panelMainToolbar.Size = new Size(1600, 82);
            panelMainToolbar.TabIndex = 1;
            // 
            // labelActions
            // 
            labelActions.AutoSize = true;
            labelActions.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelActions.ForeColor = Color.FromArgb(200, 200, 205);
            labelActions.Location = new Point(10, 5);
            labelActions.Name = "labelActions";
            labelActions.Size = new Size(47, 15);
            labelActions.TabIndex = 0;
            labelActions.Text = "Actions";
            // 
            // Button_Undo
            // 
            Button_Undo.BackColor = Color.FromArgb(55, 55, 60);
            Button_Undo.BackgroundImage = Properties.Resources.undo;
            Button_Undo.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Undo.Cursor = Cursors.Hand;
            Button_Undo.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Undo.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Undo.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Undo.FlatStyle = FlatStyle.Flat;
            Button_Undo.Location = new Point(10, 23);
            Button_Undo.Name = "Button_Undo";
            Button_Undo.Size = new Size(46, 46);
            Button_Undo.TabIndex = 1;
            toolTip1.SetToolTip(Button_Undo, "↶ Undo (Ctrl+Z)");
            Button_Undo.UseVisualStyleBackColor = false;
            Button_Undo.Click += Button_Undo_Click;
            // 
            // Button_Redo
            // 
            Button_Redo.BackColor = Color.FromArgb(55, 55, 60);
            Button_Redo.BackgroundImage = Properties.Resources.redo;
            Button_Redo.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Redo.Cursor = Cursors.Hand;
            Button_Redo.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Redo.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Redo.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Redo.FlatStyle = FlatStyle.Flat;
            Button_Redo.Location = new Point(58, 23);
            Button_Redo.Name = "Button_Redo";
            Button_Redo.Size = new Size(46, 46);
            Button_Redo.TabIndex = 2;
            toolTip1.SetToolTip(Button_Redo, "↷ Redo (Ctrl+Y)");
            Button_Redo.UseVisualStyleBackColor = false;
            Button_Redo.Click += Button_Redo_Click;
            // 
            // labelSelection
            // 
            labelSelection.AutoSize = true;
            labelSelection.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelSelection.ForeColor = Color.FromArgb(200, 200, 205);
            labelSelection.Location = new Point(120, 5);
            labelSelection.Name = "labelSelection";
            labelSelection.Size = new Size(56, 15);
            labelSelection.TabIndex = 3;
            labelSelection.Text = "Selection";
            // 
            // Button_Select
            // 
            Button_Select.BackColor = Color.FromArgb(55, 55, 60);
            Button_Select.BackgroundImage = Properties.Resources.ic_select;
            Button_Select.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Select.Cursor = Cursors.Hand;
            Button_Select.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Select.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Select.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Select.FlatStyle = FlatStyle.Flat;
            Button_Select.Location = new Point(120, 23);
            Button_Select.Name = "Button_Select";
            Button_Select.Size = new Size(46, 46);
            Button_Select.TabIndex = 4;
            toolTip1.SetToolTip(Button_Select, "⬚ Select");
            Button_Select.UseVisualStyleBackColor = false;
            Button_Select.Click += Button_Select_Click;
            // 
            // labelTools
            // 
            labelTools.AutoSize = true;
            labelTools.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelTools.ForeColor = Color.FromArgb(200, 200, 205);
            labelTools.Location = new Point(182, 5);
            labelTools.Name = "labelTools";
            labelTools.Size = new Size(35, 15);
            labelTools.TabIndex = 5;
            labelTools.Text = "Tools";
            // 
            // Button_Pen
            // 
            Button_Pen.BackColor = Color.FromArgb(55, 55, 60);
            Button_Pen.BackgroundImage = Properties.Resources.ic_pen;
            Button_Pen.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Pen.Cursor = Cursors.Hand;
            Button_Pen.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Pen.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Pen.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Pen.FlatStyle = FlatStyle.Flat;
            Button_Pen.Location = new Point(182, 23);
            Button_Pen.Name = "Button_Pen";
            Button_Pen.Size = new Size(46, 46);
            Button_Pen.TabIndex = 6;
            toolTip1.SetToolTip(Button_Pen, "✏️ Pencil");
            Button_Pen.UseVisualStyleBackColor = false;
            Button_Pen.Click += Button_Pen_Click;
            // 
            // Button_Fill
            // 
            Button_Fill.BackColor = Color.FromArgb(55, 55, 60);
            Button_Fill.BackgroundImage = Properties.Resources.ic_fill;
            Button_Fill.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Fill.Cursor = Cursors.Hand;
            Button_Fill.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Fill.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Fill.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Fill.FlatStyle = FlatStyle.Flat;
            Button_Fill.Location = new Point(230, 23);
            Button_Fill.Name = "Button_Fill";
            Button_Fill.Size = new Size(46, 46);
            Button_Fill.TabIndex = 7;
            toolTip1.SetToolTip(Button_Fill, "🎨 Fill");
            Button_Fill.UseVisualStyleBackColor = false;
            Button_Fill.Click += Button_Fill_Click;
            // 
            // Button_Eraser
            // 
            Button_Eraser.BackColor = Color.FromArgb(55, 55, 60);
            Button_Eraser.BackgroundImage = Properties.Resources.ic_eraser;
            Button_Eraser.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Eraser.Cursor = Cursors.Hand;
            Button_Eraser.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Eraser.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Eraser.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Eraser.FlatStyle = FlatStyle.Flat;
            Button_Eraser.Location = new Point(278, 23);
            Button_Eraser.Name = "Button_Eraser";
            Button_Eraser.Size = new Size(46, 46);
            Button_Eraser.TabIndex = 8;
            toolTip1.SetToolTip(Button_Eraser, "\U0001f9f9 Eraser");
            Button_Eraser.UseVisualStyleBackColor = false;
            Button_Eraser.Click += Button_Eraser_Click;
            // 
            // Button_Clear
            // 
            Button_Clear.BackColor = Color.FromArgb(70, 35, 35);
            Button_Clear.BackgroundImage = Properties.Resources.ic_clear;
            Button_Clear.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Clear.Cursor = Cursors.Hand;
            Button_Clear.FlatAppearance.BorderColor = Color.FromArgb(120, 50, 50);
            Button_Clear.FlatAppearance.MouseDownBackColor = Color.FromArgb(139, 0, 0);
            Button_Clear.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 45, 45);
            Button_Clear.FlatStyle = FlatStyle.Flat;
            Button_Clear.Location = new Point(326, 23);
            Button_Clear.Name = "Button_Clear";
            Button_Clear.Size = new Size(46, 46);
            Button_Clear.TabIndex = 9;
            toolTip1.SetToolTip(Button_Clear, "🗑️ Clear Canvas");
            Button_Clear.UseVisualStyleBackColor = false;
            Button_Clear.Click += Button_Clear_Click;
            // 
            // labelShapes
            // 
            labelShapes.AutoSize = true;
            labelShapes.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelShapes.ForeColor = Color.FromArgb(200, 200, 205);
            labelShapes.Location = new Point(388, 5);
            labelShapes.Name = "labelShapes";
            labelShapes.Size = new Size(45, 15);
            labelShapes.TabIndex = 10;
            labelShapes.Text = "Shapes";
            // 
            // Button_Line
            // 
            Button_Line.BackColor = Color.FromArgb(55, 55, 60);
            Button_Line.BackgroundImage = Properties.Resources.ic_line;
            Button_Line.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Line.Cursor = Cursors.Hand;
            Button_Line.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Line.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Line.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Line.FlatStyle = FlatStyle.Flat;
            Button_Line.Location = new Point(388, 23);
            Button_Line.Name = "Button_Line";
            Button_Line.Size = new Size(46, 46);
            Button_Line.TabIndex = 11;
            toolTip1.SetToolTip(Button_Line, "📏 Line");
            Button_Line.UseVisualStyleBackColor = false;
            Button_Line.Click += Button_Line_Click;
            // 
            // Button_Bezier
            // 
            Button_Bezier.BackColor = Color.FromArgb(55, 55, 60);
            Button_Bezier.BackgroundImage = Properties.Resources.ic_bezier;
            Button_Bezier.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Bezier.Cursor = Cursors.Hand;
            Button_Bezier.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Bezier.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Bezier.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Bezier.FlatStyle = FlatStyle.Flat;
            Button_Bezier.Location = new Point(436, 23);
            Button_Bezier.Name = "Button_Bezier";
            Button_Bezier.Size = new Size(46, 46);
            Button_Bezier.TabIndex = 12;
            toolTip1.SetToolTip(Button_Bezier, "〰️ Curve");
            Button_Bezier.UseVisualStyleBackColor = false;
            Button_Bezier.Click += Button_Bezier_Click;
            // 
            // Button_Rectangle
            // 
            Button_Rectangle.BackColor = Color.FromArgb(55, 55, 60);
            Button_Rectangle.BackgroundImage = Properties.Resources.ic_rectangle;
            Button_Rectangle.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Rectangle.Cursor = Cursors.Hand;
            Button_Rectangle.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Rectangle.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Rectangle.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Rectangle.FlatStyle = FlatStyle.Flat;
            Button_Rectangle.Location = new Point(484, 23);
            Button_Rectangle.Name = "Button_Rectangle";
            Button_Rectangle.Size = new Size(46, 46);
            Button_Rectangle.TabIndex = 13;
            toolTip1.SetToolTip(Button_Rectangle, "▭ Rectangle");
            Button_Rectangle.UseVisualStyleBackColor = false;
            Button_Rectangle.Click += Button_Rectangle_Click;
            // 
            // Button_Ellipse
            // 
            Button_Ellipse.BackColor = Color.FromArgb(55, 55, 60);
            Button_Ellipse.BackgroundImage = Properties.Resources.ic_ellipse;
            Button_Ellipse.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Ellipse.Cursor = Cursors.Hand;
            Button_Ellipse.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Ellipse.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Ellipse.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Ellipse.FlatStyle = FlatStyle.Flat;
            Button_Ellipse.Location = new Point(532, 23);
            Button_Ellipse.Name = "Button_Ellipse";
            Button_Ellipse.Size = new Size(46, 46);
            Button_Ellipse.TabIndex = 14;
            toolTip1.SetToolTip(Button_Ellipse, "⭕ Ellipse");
            Button_Ellipse.UseVisualStyleBackColor = false;
            Button_Ellipse.Click += Button_Ellipse_Click;
            // 
            // Button_Polygon
            // 
            Button_Polygon.BackColor = Color.FromArgb(55, 55, 60);
            Button_Polygon.BackgroundImage = Properties.Resources.ic_polygon;
            Button_Polygon.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Polygon.Cursor = Cursors.Hand;
            Button_Polygon.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 90);
            Button_Polygon.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 122, 204);
            Button_Polygon.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 82);
            Button_Polygon.FlatStyle = FlatStyle.Flat;
            Button_Polygon.Location = new Point(580, 23);
            Button_Polygon.Name = "Button_Polygon";
            Button_Polygon.Size = new Size(46, 46);
            Button_Polygon.TabIndex = 15;
            toolTip1.SetToolTip(Button_Polygon, "⬡ Polygon");
            Button_Polygon.UseVisualStyleBackColor = false;
            Button_Polygon.Click += Button_Polygon_Click;
            // 
            // labelBrushSize
            // 
            labelBrushSize.AutoSize = true;
            labelBrushSize.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelBrushSize.ForeColor = Color.FromArgb(200, 200, 205);
            labelBrushSize.Location = new Point(946, 3);
            labelBrushSize.Name = "labelBrushSize";
            labelBrushSize.Size = new Size(62, 15);
            labelBrushSize.TabIndex = 16;
            labelBrushSize.Text = "Brush Size";
            // 
            // labelColors
            // 
            labelColors.AutoSize = true;
            labelColors.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            labelColors.ForeColor = Color.FromArgb(200, 200, 205);
            labelColors.Location = new Point(642, 5);
            labelColors.Name = "labelColors";
            labelColors.Size = new Size(40, 15);
            labelColors.TabIndex = 18;
            labelColors.Text = "Colors";
            // 
            // panelColors
            // 
            panelColors.BackColor = Color.FromArgb(45, 45, 48);
            panelColors.Controls.Add(ptbColor1);
            panelColors.Controls.Add(ptbColor2);
            panelColors.Controls.Add(panelColorPalette);
            panelColors.Controls.Add(ptbEditColor);
            panelColors.Location = new Point(642, 23);
            panelColors.Name = "panelColors";
            panelColors.Padding = new Padding(4);
            panelColors.Size = new Size(290, 62);
            panelColors.TabIndex = 19;
            // 
            // ptbColor1
            // 
            ptbColor1.BackColor = Color.Black;
            ptbColor1.BorderStyle = BorderStyle.FixedSingle;
            ptbColor1.Cursor = Cursors.Hand;
            ptbColor1.Location = new Point(6, 6);
            ptbColor1.Name = "ptbColor1";
            ptbColor1.Size = new Size(28, 50);
            ptbColor1.TabIndex = 0;
            ptbColor1.TabStop = false;
            toolTip1.SetToolTip(ptbColor1, "🎨 Primary Color");
            ptbColor1.Click += Button_ChangeColor_Click;
            // 
            // ptbColor2
            // 
            ptbColor2.BackColor = Color.White;
            ptbColor2.BorderStyle = BorderStyle.FixedSingle;
            ptbColor2.Cursor = Cursors.Hand;
            ptbColor2.Location = new Point(40, 6);
            ptbColor2.Name = "ptbColor2";
            ptbColor2.Size = new Size(28, 50);
            ptbColor2.TabIndex = 1;
            ptbColor2.TabStop = false;
            toolTip1.SetToolTip(ptbColor2, "🎨 Secondary Color");
            ptbColor2.Click += Button_ChangeColor_Click;
            // 
            // panelColorPalette
            // 
            panelColorPalette.BackColor = Color.FromArgb(37, 37, 40);
            panelColorPalette.Controls.Add(colorBox_0);
            panelColorPalette.Controls.Add(colorBox_1);
            panelColorPalette.Controls.Add(colorBox_2);
            panelColorPalette.Controls.Add(colorBox_3);
            panelColorPalette.Controls.Add(colorBox_4);
            panelColorPalette.Controls.Add(colorBox_5);
            panelColorPalette.Controls.Add(colorBox_6);
            panelColorPalette.Controls.Add(colorBox_7);
            panelColorPalette.Controls.Add(colorBox_8);
            panelColorPalette.Controls.Add(colorBox_9);
            panelColorPalette.Location = new Point(74, 6);
            panelColorPalette.Name = "panelColorPalette";
            panelColorPalette.Padding = new Padding(2);
            panelColorPalette.Size = new Size(166, 54);
            panelColorPalette.TabIndex = 2;
            // 
            // colorBox_0
            // 
            colorBox_0.BackColor = Color.Black;
            colorBox_0.BorderStyle = BorderStyle.FixedSingle;
            colorBox_0.Cursor = Cursors.Hand;
            colorBox_0.Location = new Point(4, 4);
            colorBox_0.Margin = new Padding(2);
            colorBox_0.Name = "colorBox_0";
            colorBox_0.Size = new Size(28, 22);
            colorBox_0.TabIndex = 0;
            colorBox_0.TabStop = false;
            colorBox_0.Click += Button_ChangeColor_Click;
            colorBox_0.MouseEnter += ColorBox_MouseEnter;
            colorBox_0.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_1
            // 
            colorBox_1.BackColor = Color.White;
            colorBox_1.BorderStyle = BorderStyle.FixedSingle;
            colorBox_1.Cursor = Cursors.Hand;
            colorBox_1.Location = new Point(36, 4);
            colorBox_1.Margin = new Padding(2);
            colorBox_1.Name = "colorBox_1";
            colorBox_1.Size = new Size(28, 22);
            colorBox_1.TabIndex = 1;
            colorBox_1.TabStop = false;
            colorBox_1.Click += Button_ChangeColor_Click;
            colorBox_1.MouseEnter += ColorBox_MouseEnter;
            colorBox_1.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_2
            // 
            colorBox_2.BackColor = Color.Red;
            colorBox_2.BorderStyle = BorderStyle.FixedSingle;
            colorBox_2.Cursor = Cursors.Hand;
            colorBox_2.Location = new Point(68, 4);
            colorBox_2.Margin = new Padding(2);
            colorBox_2.Name = "colorBox_2";
            colorBox_2.Size = new Size(28, 22);
            colorBox_2.TabIndex = 2;
            colorBox_2.TabStop = false;
            colorBox_2.Click += Button_ChangeColor_Click;
            colorBox_2.MouseEnter += ColorBox_MouseEnter;
            colorBox_2.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_3
            // 
            colorBox_3.BackColor = Color.FromArgb(0, 128, 0);
            colorBox_3.BorderStyle = BorderStyle.FixedSingle;
            colorBox_3.Cursor = Cursors.Hand;
            colorBox_3.Location = new Point(100, 4);
            colorBox_3.Margin = new Padding(2);
            colorBox_3.Name = "colorBox_3";
            colorBox_3.Size = new Size(28, 22);
            colorBox_3.TabIndex = 3;
            colorBox_3.TabStop = false;
            colorBox_3.Click += Button_ChangeColor_Click;
            colorBox_3.MouseEnter += ColorBox_MouseEnter;
            colorBox_3.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_4
            // 
            colorBox_4.BackColor = Color.Blue;
            colorBox_4.BorderStyle = BorderStyle.FixedSingle;
            colorBox_4.Cursor = Cursors.Hand;
            colorBox_4.Location = new Point(132, 4);
            colorBox_4.Margin = new Padding(2);
            colorBox_4.Name = "colorBox_4";
            colorBox_4.Size = new Size(28, 22);
            colorBox_4.TabIndex = 4;
            colorBox_4.TabStop = false;
            colorBox_4.Click += Button_ChangeColor_Click;
            colorBox_4.MouseEnter += ColorBox_MouseEnter;
            colorBox_4.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_5
            // 
            colorBox_5.BackColor = Color.Yellow;
            colorBox_5.BorderStyle = BorderStyle.FixedSingle;
            colorBox_5.Cursor = Cursors.Hand;
            colorBox_5.Location = new Point(4, 30);
            colorBox_5.Margin = new Padding(2);
            colorBox_5.Name = "colorBox_5";
            colorBox_5.Size = new Size(28, 22);
            colorBox_5.TabIndex = 5;
            colorBox_5.TabStop = false;
            colorBox_5.Click += Button_ChangeColor_Click;
            colorBox_5.MouseEnter += ColorBox_MouseEnter;
            colorBox_5.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_6
            // 
            colorBox_6.BackColor = Color.Orange;
            colorBox_6.BorderStyle = BorderStyle.FixedSingle;
            colorBox_6.Cursor = Cursors.Hand;
            colorBox_6.Location = new Point(36, 30);
            colorBox_6.Margin = new Padding(2);
            colorBox_6.Name = "colorBox_6";
            colorBox_6.Size = new Size(28, 22);
            colorBox_6.TabIndex = 6;
            colorBox_6.TabStop = false;
            colorBox_6.Click += Button_ChangeColor_Click;
            colorBox_6.MouseEnter += ColorBox_MouseEnter;
            colorBox_6.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_7
            // 
            colorBox_7.BackColor = Color.Purple;
            colorBox_7.BorderStyle = BorderStyle.FixedSingle;
            colorBox_7.Cursor = Cursors.Hand;
            colorBox_7.Location = new Point(68, 30);
            colorBox_7.Margin = new Padding(2);
            colorBox_7.Name = "colorBox_7";
            colorBox_7.Size = new Size(28, 22);
            colorBox_7.TabIndex = 7;
            colorBox_7.TabStop = false;
            colorBox_7.Click += Button_ChangeColor_Click;
            colorBox_7.MouseEnter += ColorBox_MouseEnter;
            colorBox_7.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_8
            // 
            colorBox_8.BackColor = Color.Pink;
            colorBox_8.BorderStyle = BorderStyle.FixedSingle;
            colorBox_8.Cursor = Cursors.Hand;
            colorBox_8.Location = new Point(100, 30);
            colorBox_8.Margin = new Padding(2);
            colorBox_8.Name = "colorBox_8";
            colorBox_8.Size = new Size(28, 22);
            colorBox_8.TabIndex = 8;
            colorBox_8.TabStop = false;
            colorBox_8.Click += Button_ChangeColor_Click;
            colorBox_8.MouseEnter += ColorBox_MouseEnter;
            colorBox_8.MouseLeave += ColorBox_MouseLeave;
            // 
            // colorBox_9
            // 
            colorBox_9.BackColor = Color.Brown;
            colorBox_9.BorderStyle = BorderStyle.FixedSingle;
            colorBox_9.Cursor = Cursors.Hand;
            colorBox_9.Location = new Point(132, 30);
            colorBox_9.Margin = new Padding(2);
            colorBox_9.Name = "colorBox_9";
            colorBox_9.Size = new Size(28, 22);
            colorBox_9.TabIndex = 9;
            colorBox_9.TabStop = false;
            colorBox_9.Click += Button_ChangeColor_Click;
            colorBox_9.MouseEnter += ColorBox_MouseEnter;
            colorBox_9.MouseLeave += ColorBox_MouseLeave;
            // 
            // ptbEditColor
            // 
            ptbEditColor.BackColor = Color.FromArgb(55, 55, 60);
            ptbEditColor.BackgroundImage = Properties.Resources.ic_edit_color;
            ptbEditColor.BackgroundImageLayout = ImageLayout.Zoom;
            ptbEditColor.BorderStyle = BorderStyle.FixedSingle;
            ptbEditColor.Cursor = Cursors.Hand;
            ptbEditColor.Location = new Point(246, 7);
            ptbEditColor.Name = "ptbEditColor";
            ptbEditColor.Padding = new Padding(5);
            ptbEditColor.Size = new Size(38, 50);
            ptbEditColor.TabIndex = 3;
            ptbEditColor.TabStop = false;
            toolTip1.SetToolTip(ptbEditColor, "🎨 Custom Color Picker");
            ptbEditColor.Click += Button_EditColor_Click;
            // 
            // Button_LineSize
            // 
            Button_LineSize.BackColor = Color.FromArgb(55, 55, 60);
            Button_LineSize.Cursor = Cursors.Hand;
            Button_LineSize.Location = new Point(948, 24);
            Button_LineSize.Maximum = 20;
            Button_LineSize.Minimum = 1;
            Button_LineSize.Name = "Button_LineSize";
            Button_LineSize.Size = new Size(120, 45);
            Button_LineSize.TabIndex = 17;
            Button_LineSize.TickStyle = TickStyle.None;
            toolTip1.SetToolTip(Button_LineSize, "Brush size (1-20)");
            Button_LineSize.Value = 2;
            Button_LineSize.Scroll += Button_LineSize_Scroll;
            // 
            // toolTip1
            // 
            toolTip1.AutoPopDelay = 5000;
            toolTip1.BackColor = Color.FromArgb(45, 45, 48);
            toolTip1.ForeColor = Color.White;
            toolTip1.InitialDelay = 500;
            toolTip1.IsBalloon = true;
            toolTip1.ReshowDelay = 100;
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            toolTip1.ToolTipTitle = "KayArt";
            // 
            // Canvas
            // 
            Canvas.BackColor = Color.White;
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(0, 120);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(1600, 580);
            Canvas.TabIndex = 2;
            Canvas.TabStop = false;
            Canvas.Paint += Canvas_Paint;
            // 
            // btnMakeOnline
            // 
            btnMakeOnline.BackColor = Color.FromArgb(0, 150, 0);
            btnMakeOnline.Cursor = Cursors.Hand;
            btnMakeOnline.Dock = DockStyle.Right;
            btnMakeOnline.FlatAppearance.BorderSize = 0;
            btnMakeOnline.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 180, 0);
            btnMakeOnline.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 130, 0);
            btnMakeOnline.FlatStyle = FlatStyle.Flat;
            btnMakeOnline.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnMakeOnline.ForeColor = Color.White;
            btnMakeOnline.Location = new Point(1547, 3);
            btnMakeOnline.Margin = new Padding(2);
            btnMakeOnline.Name = "btnMakeOnline";
            btnMakeOnline.Padding = new Padding(6);
            btnMakeOnline.Size = new Size(50, 32);
            btnMakeOnline.TabIndex = 4;
            btnMakeOnline.Text = "🌐";
            toolTip1.SetToolTip(btnMakeOnline, "🌐 Make Project Online");
            btnMakeOnline.UseVisualStyleBackColor = false;
            btnMakeOnline.Visible = false;
            btnMakeOnline.Click += btnMakeOnline_Click;
            // 
            // Form_Client
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(30, 30, 32);
            ClientSize = new Size(1600, 700);
            Controls.Add(Canvas);
            Controls.Add(panelMainToolbar);
            Controls.Add(panelTopMenu);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(240, 240, 240);
            KeyPreview = true;
            MinimumSize = new Size(1400, 600);
            Name = "Form_Client";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KayArt - Collaborative Drawing (F11 for Fullscreen)";
            WindowState = FormWindowState.Maximized;
            FormClosed += Form_Client_FormClosed;
            Load += Form_Client_Load;
            panelTopMenu.ResumeLayout(false);
            panelMainToolbar.ResumeLayout(false);
            panelMainToolbar.PerformLayout();
            panelColors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ptbColor1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbColor2).EndInit();
            panelColorPalette.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)colorBox_0).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_1).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_2).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_3).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_4).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_5).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_6).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_7).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_8).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorBox_9).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbEditColor).EndInit();
            ((System.ComponentModel.ISupportInitialize)Button_LineSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            ResumeLayout(false);
        }
        
        private Panel CreateSeparator(int x)
        {
            return new Panel
            {
                BackColor = Color.FromArgb(100, 100, 105),
                Location = new Point(x, 10),
                Size = new Size(1, 65),
                Name = $"separator_{x}"
            };
        }
        
        // hộp màu khi di chuột
        private void ColorBox_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox ptb) 
            {
                ptb.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        // hộp màu khi rời chuột
        private void ColorBox_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox ptb) 
            {
                ptb.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        
        #endregion

        private Panel panelTopMenu;
        private Panel panelMainToolbar;
        private Panel panelColors;
        
        private Label labelActions;
        private Label labelSelection;
        private Label labelTools;
        private Label labelShapes;
        private Label labelColors;
        private Label labelBrushSize;
        
        private Button Button_Home;

        private Button Button_New;
        private Button Button_Open;
        private Button Button_Save;
        private Button Button_Undo;
        private Button Button_Redo;
        private Button Button_Select;
        private Button Button_Pen;
        private Button Button_Fill;
        private Button Button_Eraser;
        private Button Button_Clear;
        private Button Button_Line;
        private Button Button_Bezier;
        private Button Button_Rectangle;
        private Button Button_Ellipse;
        private Button Button_Polygon;
        
        private TrackBar Button_LineSize;
        private PictureBox ptbColor1;
        private PictureBox ptbColor2;
        private PictureBox ptbEditColor;
        private FlowLayoutPanel panelColorPalette;
        
        private ToolTip toolTip1;
        
        // 10 màu
        private PictureBox colorBox_0;
        private PictureBox colorBox_1;
        private PictureBox colorBox_2;
        private PictureBox colorBox_3;
        private PictureBox colorBox_4;
        private PictureBox colorBox_5;
        private PictureBox colorBox_6;
        private PictureBox colorBox_7;
        private PictureBox colorBox_8;
        private PictureBox colorBox_9;
        private PictureBox Canvas;

        // màu mặc định hiện tại
        private PictureBox ptbColor => ptbColor1;
        
        private void InitializeColorBoxes()
        {
            if (panelColorPalette == null) return;
            panelColorPalette.Visible = true;
        }
        private Button btnMakeOnline;
    }
}