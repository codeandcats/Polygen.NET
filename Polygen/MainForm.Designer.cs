namespace Polygen
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFileOpenRecent = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuFileClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuEditSnapToHandles = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuUpdatePolygonColors = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuEditClearAllPolygons = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuClampPointsToImage = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewShowOutlines = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewCheckerboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuViewFps = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblSpacer = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblMouseImageCoordinates = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblPolygonCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.txtImageOpacity = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.cboZoom = new System.Windows.Forms.ComboBox();
			this.lblZoom = new System.Windows.Forms.Label();
			this.pnlTools = new System.Windows.Forms.ToolStrip();
			this.lblTools = new System.Windows.Forms.ToolStripLabel();
			this.btnPolygonTool = new System.Windows.Forms.ToolStripButton();
			this.btnEraseTool = new System.Windows.Forms.ToolStripButton();
			this.canvas = new Polygen.UI.ImageCanvas();
			this.mnuFileExportImage = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuMainMenu.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.pnlTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtImageOpacity)).BeginInit();
			this.pnlTools.SuspendLayout();
			this.SuspendLayout();
			// 
			// mnuMainMenu
			// 
			this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuHelp});
			this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
			this.mnuMainMenu.Name = "mnuMainMenu";
			this.mnuMainMenu.Size = new System.Drawing.Size(994, 33);
			this.mnuMainMenu.TabIndex = 1;
			this.mnuMainMenu.Text = "menuStrip1";
			// 
			// mnuFile
			// 
			this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileOpenRecent,
            this.toolStripMenuItem1,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.toolStripMenuItem2,
            this.mnuFileExportImage,
            this.toolStripMenuItem6,
            this.mnuFileClose,
            this.mnuFileExit});
			this.mnuFile.Name = "mnuFile";
			this.mnuFile.Size = new System.Drawing.Size(50, 29);
			this.mnuFile.Text = "&File";
			// 
			// mnuFileNew
			// 
			this.mnuFileNew.Name = "mnuFileNew";
			this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.mnuFileNew.Size = new System.Drawing.Size(202, 30);
			this.mnuFileNew.Text = "&New";
			this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
			// 
			// mnuFileOpen
			// 
			this.mnuFileOpen.Name = "mnuFileOpen";
			this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.mnuFileOpen.Size = new System.Drawing.Size(205, 30);
			this.mnuFileOpen.Text = "&Open...";
			this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
			// 
			// mnuFileOpenRecent
			// 
			this.mnuFileOpenRecent.Name = "mnuFileOpenRecent";
			this.mnuFileOpenRecent.Size = new System.Drawing.Size(202, 30);
			this.mnuFileOpenRecent.Text = "Open &Recent";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
			// 
			// mnuFileSave
			// 
			this.mnuFileSave.Name = "mnuFileSave";
			this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mnuFileSave.Size = new System.Drawing.Size(202, 30);
			this.mnuFileSave.Text = "&Save";
			this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
			// 
			// mnuFileSaveAs
			// 
			this.mnuFileSaveAs.Name = "mnuFileSaveAs";
			this.mnuFileSaveAs.Size = new System.Drawing.Size(205, 30);
			this.mnuFileSaveAs.Text = "Save &As...";
			this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(199, 6);
			// 
			// mnuFileClose
			// 
			this.mnuFileClose.Name = "mnuFileClose";
			this.mnuFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mnuFileClose.Size = new System.Drawing.Size(202, 30);
			this.mnuFileClose.Text = "&Close";
			this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
			// 
			// mnuFileExit
			// 
			this.mnuFileExit.Name = "mnuFileExit";
			this.mnuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.mnuFileExit.Size = new System.Drawing.Size(202, 30);
			this.mnuFileExit.Text = "E&xit";
			this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
			// 
			// mnuEdit
			// 
			this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditUndo,
            this.mnuEditRedo,
            this.toolStripMenuItem3,
            this.mnuEditSnapToHandles,
            this.toolStripMenuItem4,
            this.mnuUpdatePolygonColors,
            this.mnuEditClearAllPolygons,
            this.toolStripMenuItem5,
            this.mnuClampPointsToImage});
			this.mnuEdit.Name = "mnuEdit";
			this.mnuEdit.Size = new System.Drawing.Size(54, 29);
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuEditUndo
			// 
			this.mnuEditUndo.Image = global::Polygen.Properties.Resources.Undo;
			this.mnuEditUndo.Name = "mnuEditUndo";
			this.mnuEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.mnuEditUndo.Size = new System.Drawing.Size(324, 30);
			this.mnuEditUndo.Text = "&Undo";
			this.mnuEditUndo.Click += new System.EventHandler(this.mnuEditUndo_Click);
			// 
			// mnuEditRedo
			// 
			this.mnuEditRedo.Image = global::Polygen.Properties.Resources.Redo;
			this.mnuEditRedo.Name = "mnuEditRedo";
			this.mnuEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.mnuEditRedo.Size = new System.Drawing.Size(324, 30);
			this.mnuEditRedo.Text = "&Redo";
			this.mnuEditRedo.Click += new System.EventHandler(this.mnuEditRedo_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(321, 6);
			// 
			// mnuEditSnapToHandles
			// 
			this.mnuEditSnapToHandles.Name = "mnuEditSnapToHandles";
			this.mnuEditSnapToHandles.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.mnuEditSnapToHandles.Size = new System.Drawing.Size(324, 30);
			this.mnuEditSnapToHandles.Text = "&Snap Handles";
			this.mnuEditSnapToHandles.Click += new System.EventHandler(this.mnuEditSnapToHandles_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(321, 6);
			// 
			// mnuUpdatePolygonColors
			// 
			this.mnuUpdatePolygonColors.Name = "mnuUpdatePolygonColors";
			this.mnuUpdatePolygonColors.Size = new System.Drawing.Size(324, 30);
			this.mnuUpdatePolygonColors.Text = "&Update Polygon Colors";
			this.mnuUpdatePolygonColors.Click += new System.EventHandler(this.mnuUpdatePolygonColors_Click);
			// 
			// mnuEditClearAllPolygons
			// 
			this.mnuEditClearAllPolygons.Name = "mnuEditClearAllPolygons";
			this.mnuEditClearAllPolygons.Size = new System.Drawing.Size(324, 30);
			this.mnuEditClearAllPolygons.Text = "&Clear all Polygons";
			this.mnuEditClearAllPolygons.Click += new System.EventHandler(this.mnuEditClearAllPolygons_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(321, 6);
			// 
			// mnuClampPointsToImage
			// 
			this.mnuClampPointsToImage.Name = "mnuClampPointsToImage";
			this.mnuClampPointsToImage.Size = new System.Drawing.Size(324, 30);
			this.mnuClampPointsToImage.Text = "Constrain Points Within Image";
			this.mnuClampPointsToImage.Click += new System.EventHandler(this.mnuClampPointsToImage_Click);
			// 
			// mnuView
			// 
			this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewShowOutlines,
            this.mnuViewCheckerboard,
            this.mnuViewFps});
			this.mnuView.Name = "mnuView";
			this.mnuView.Size = new System.Drawing.Size(61, 29);
			this.mnuView.Text = "&View";
			// 
			// mnuViewShowOutlines
			// 
			this.mnuViewShowOutlines.Name = "mnuViewShowOutlines";
			this.mnuViewShowOutlines.Size = new System.Drawing.Size(235, 30);
			this.mnuViewShowOutlines.Text = "&Wireframe";
			this.mnuViewShowOutlines.Click += new System.EventHandler(this.mnuViewShowOutlines_Click);
			// 
			// mnuViewCheckerboard
			// 
			this.mnuViewCheckerboard.Name = "mnuViewCheckerboard";
			this.mnuViewCheckerboard.Size = new System.Drawing.Size(235, 30);
			this.mnuViewCheckerboard.Text = "&Checkerboard";
			this.mnuViewCheckerboard.Visible = false;
			this.mnuViewCheckerboard.Click += new System.EventHandler(this.mnuViewCheckerboard_Click);
			// 
			// mnuViewFps
			// 
			this.mnuViewFps.Name = "mnuViewFps";
			this.mnuViewFps.Size = new System.Drawing.Size(235, 30);
			this.mnuViewFps.Text = "&Frames Per Second";
			this.mnuViewFps.Click += new System.EventHandler(this.mnuViewFps_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
			this.mnuHelp.Name = "mnuHelp";
			this.mnuHelp.Size = new System.Drawing.Size(61, 29);
			this.mnuHelp.Text = "&Help";
			// 
			// mnuHelpAbout
			// 
			this.mnuHelpAbout.Name = "mnuHelpAbout";
			this.mnuHelpAbout.Size = new System.Drawing.Size(134, 30);
			this.mnuHelpAbout.Text = "&About";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblSpacer,
            this.lblMouseImageCoordinates,
            this.lblPolygonCount});
			this.statusStrip1.Location = new System.Drawing.Point(0, 540);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(994, 30);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(80, 25);
			this.lblStatus.Text = "lblStatus";
			// 
			// lblSpacer
			// 
			this.lblSpacer.Name = "lblSpacer";
			this.lblSpacer.Size = new System.Drawing.Size(523, 25);
			this.lblSpacer.Spring = true;
			// 
			// lblMouseImageCoordinates
			// 
			this.lblMouseImageCoordinates.Name = "lblMouseImageCoordinates";
			this.lblMouseImageCoordinates.Size = new System.Drawing.Size(231, 25);
			this.lblMouseImageCoordinates.Text = "lblMouseImageCoordinates";
			// 
			// lblPolygonCount
			// 
			this.lblPolygonCount.Name = "lblPolygonCount";
			this.lblPolygonCount.Size = new System.Drawing.Size(145, 25);
			this.lblPolygonCount.Text = "lblPolygonCount";
			// 
			// pnlTop
			// 
			this.pnlTop.Controls.Add(this.button2);
			this.pnlTop.Controls.Add(this.button3);
			this.pnlTop.Controls.Add(this.button1);
			this.pnlTop.Controls.Add(this.txtImageOpacity);
			this.pnlTop.Controls.Add(this.label1);
			this.pnlTop.Controls.Add(this.cboZoom);
			this.pnlTop.Controls.Add(this.lblZoom);
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 33);
			this.pnlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(994, 38);
			this.pnlTop.TabIndex = 6;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(448, 9);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Visible = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(528, 9);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 6;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Visible = false;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(366, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtImageOpacity
			// 
			this.txtImageOpacity.Location = new System.Drawing.Point(289, 5);
			this.txtImageOpacity.Name = "txtImageOpacity";
			this.txtImageOpacity.Size = new System.Drawing.Size(59, 31);
			this.txtImageOpacity.TabIndex = 3;
			this.txtImageOpacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtImageOpacity.ThousandsSeparator = true;
			this.txtImageOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.txtImageOpacity.ValueChanged += new System.EventHandler(this.txtImageOpacity_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(177, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 25);
			this.label1.TabIndex = 2;
			this.label1.Text = "Image Opacity";
			// 
			// cboZoom
			// 
			this.cboZoom.FormattingEnabled = true;
			this.cboZoom.Items.AddRange(new object[] {
            "5%",
            "10%",
            "25%",
            "33%",
            "50%",
            "66%",
            "75%",
            "90%",
            "100%",
            "150%",
            "200%",
            "250%",
            "300%",
            "400%",
            "500%"});
			this.cboZoom.Location = new System.Drawing.Point(67, 5);
			this.cboZoom.Name = "cboZoom";
			this.cboZoom.Size = new System.Drawing.Size(90, 33);
			this.cboZoom.TabIndex = 1;
			this.cboZoom.SelectedIndexChanged += new System.EventHandler(this.cboZoom_SelectedIndexChanged);
			// 
			// lblZoom
			// 
			this.lblZoom.AutoSize = true;
			this.lblZoom.Location = new System.Drawing.Point(12, 8);
			this.lblZoom.Name = "lblZoom";
			this.lblZoom.Size = new System.Drawing.Size(60, 25);
			this.lblZoom.TabIndex = 0;
			this.lblZoom.Text = "&Zoom";
			// 
			// pnlTools
			// 
			this.pnlTools.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.pnlTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTools,
            this.btnPolygonTool,
            this.btnEraseTool});
			this.pnlTools.Location = new System.Drawing.Point(0, 71);
			this.pnlTools.Name = "pnlTools";
			this.pnlTools.Size = new System.Drawing.Size(994, 28);
			this.pnlTools.TabIndex = 9;
			this.pnlTools.Text = "toolStrip1";
			// 
			// lblTools
			// 
			this.lblTools.Name = "lblTools";
			this.lblTools.Size = new System.Drawing.Size(55, 25);
			this.lblTools.Text = "Tools";
			// 
			// btnPolygonTool
			// 
			this.btnPolygonTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPolygonTool.Image = ((System.Drawing.Image)(resources.GetObject("btnPolygonTool.Image")));
			this.btnPolygonTool.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPolygonTool.Name = "btnPolygonTool";
			this.btnPolygonTool.Size = new System.Drawing.Size(23, 25);
			this.btnPolygonTool.Text = "Polygon Tool";
			this.btnPolygonTool.ToolTipText = "Polygon Tool";
			this.btnPolygonTool.Click += new System.EventHandler(this.btnTool_Click);
			// 
			// btnEraseTool
			// 
			this.btnEraseTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnEraseTool.Image = ((System.Drawing.Image)(resources.GetObject("btnEraseTool.Image")));
			this.btnEraseTool.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnEraseTool.Name = "btnEraseTool";
			this.btnEraseTool.Size = new System.Drawing.Size(23, 25);
			this.btnEraseTool.Text = "Eraser";
			this.btnEraseTool.ToolTipText = "Eraser";
			this.btnEraseTool.Click += new System.EventHandler(this.btnTool_Click);
			// 
			// canvas
			// 
			this.canvas.ActiveToolType = null;
			this.canvas.BackgroundColor = System.Drawing.Color.White;
			this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.canvas.Image = null;
			this.canvas.ImageOpacity = ((byte)(0));
			this.canvas.Location = new System.Drawing.Point(0, 99);
			this.canvas.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.canvas.Name = "canvas";
			this.canvas.ShadePolygons = false;
			this.canvas.ShowFps = false;
			this.canvas.ShowPolygonOutlines = true;
			this.canvas.Size = new System.Drawing.Size(994, 441);
			this.canvas.Snapping = true;
			this.canvas.TabIndex = 10;
			this.canvas.Zoom = 100D;
			this.canvas.ZoomChanged += new Polygen.UI.ZoomChangedEvent(this.canvas_ZoomChanged);
			this.canvas.ChangesMade += new Polygen.UI.ChangesMadeEvent(this.canvas_ChangesMade);
			this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
			// 
			// mnuFileExportImage
			// 
			this.mnuFileExportImage.Name = "mnuFileExportImage";
			this.mnuFileExportImage.Size = new System.Drawing.Size(202, 30);
			this.mnuFileExportImage.Text = "&Export Image...";
			this.mnuFileExportImage.Click += new System.EventHandler(this.mnuFileExportImage_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(199, 6);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(994, 570);
			this.Controls.Add(this.canvas);
			this.Controls.Add(this.pnlTools);
			this.Controls.Add(this.pnlTop);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.mnuMainMenu);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.MainMenuStrip = this.mnuMainMenu;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "Polygen";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.mnuMainMenu.ResumeLayout(false);
			this.mnuMainMenu.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.pnlTop.ResumeLayout(false);
			this.pnlTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtImageOpacity)).EndInit();
			this.pnlTools.ResumeLayout(false);
			this.pnlTools.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mnuMainMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuFile;
		private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
		private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
		private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
		private System.Windows.Forms.ToolStripMenuItem mnuFileClose;
		private System.Windows.Forms.ToolStripMenuItem mnuEdit;
		private System.Windows.Forms.ToolStripMenuItem mnuEditUndo;
		private System.Windows.Forms.ToolStripMenuItem mnuEditRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem mnuEditSnapToHandles;
		private System.Windows.Forms.ToolStripMenuItem mnuFileOpenRecent;
		private System.Windows.Forms.ToolStripMenuItem mnuUpdatePolygonColors;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem mnuEditClearAllPolygons;
		private System.Windows.Forms.ToolStripStatusLabel lblSpacer;
		private System.Windows.Forms.ToolStripStatusLabel lblMouseImageCoordinates;
		private System.Windows.Forms.ToolStripMenuItem mnuView;
		private System.Windows.Forms.ToolStripMenuItem mnuViewShowOutlines;
		private System.Windows.Forms.Label lblZoom;
		private System.Windows.Forms.ComboBox cboZoom;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown txtImageOpacity;
		private System.Windows.Forms.ToolStrip pnlTools;
		private System.Windows.Forms.ToolStripLabel lblTools;
		private System.Windows.Forms.ToolStripButton btnPolygonTool;
		private System.Windows.Forms.ToolStripButton btnEraseTool;
		private System.Windows.Forms.Button button1;
		private UI.ImageCanvas canvas;
		private System.Windows.Forms.ToolStripMenuItem mnuViewCheckerboard;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem mnuClampPointsToImage;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ToolStripMenuItem mnuHelp;
		private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
		private System.Windows.Forms.ToolStripMenuItem mnuViewFps;
		private System.Windows.Forms.ToolStripStatusLabel lblPolygonCount;
		private System.Windows.Forms.ToolStripMenuItem mnuFileExportImage;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
	}
}

