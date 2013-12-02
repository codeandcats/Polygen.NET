using Polygen.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WinFormsCursors = System.Windows.Forms.Cursors;

namespace Polygen
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private bool hasUnsavedChanges = false;
		private string imageFileName = "";

		public ImageDef ImageDef
		{
			get
			{
				return canvas.Image;
			}
			set
			{
				canvas.Image = value;
			}
		}

		private void mnuFileExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Settings.Load();

			DisableRefreshingControlStates();
			try
			{
				SnapToHandles = Settings.Snapping;
				ShowPolygonOutlines = Settings.ShowPolygonOutlines;
				ImageOpacity = Settings.ImageOpacity;

				btnPolygonTool.Tag = typeof(Polygen.Tools.PolygonTool);
				btnEraseTool.Tag = typeof(Polygen.Tools.EraserTool);

				// Open the first recent file that exists
				while (!string.IsNullOrEmpty(Settings.RecentFileNames.MostRecent))
				{
					string fileName = Settings.RecentFileNames.MostRecent;

					if (!File.Exists(fileName))
						Settings.RecentFileNames.Remove(fileName);
					else
					{
						OpenFile(fileName);
						break;
					}
				}

				lblStatus.Text = "";

				canvas.ActiveToolType = typeof(Tools.PolygonTool);
			}
			finally
			{
				EnableRefreshingControlStates();
			}
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
		}

		private void mnuFileNew_Click(object sender, EventArgs e)
		{
			if (CheckSaveChanges() == DialogResult.Cancel)
				return;

			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = "Select image to base Polygen Image on";
				dlg.Filter = "Image Files (*.bmp; *.png; *.jpg; *.jpeg; *.gif)|*.bmp; *.png; *.jpg; *.jpeg; *.gif|All Files (*.*)|*.*";
				if (dlg.ShowDialog() != DialogResult.OK)
					return;

				var imageDef = new ImageDef();
				imageDef.BaseFileName = dlg.FileName;

				ImageDef = imageDef;
				imageFileName = "";
			}

			canvas.ResetZoomAndCenter();

			RefreshControlStates();
		}

		private int disableRefreshControlStatesCount = 0;

		private void DisableRefreshingControlStates()
		{
			disableRefreshControlStatesCount++;
		}

		private void EnableRefreshingControlStates()
		{
			if (disableRefreshControlStatesCount > 0)
				disableRefreshControlStatesCount--;
			if (disableRefreshControlStatesCount == 0)
				RefreshControlStates();
		}

		private void RefreshControlStates()
		{
			if (disableRefreshControlStatesCount > 0)
				return;

			string windowTitle = "Polygen";
			if (imageFileName != "")
				windowTitle = Path.GetFileName(imageFileName) + (hasUnsavedChanges ? "*" : "") + " - " + windowTitle;
			if (windowTitle != Text)
				Text = windowTitle;

			mnuFileSave.Enabled = (ImageDef != null) && hasUnsavedChanges;
			mnuFileSaveAs.Enabled = (ImageDef != null);
			mnuFileClose.Enabled = (ImageDef != null);

			RefreshRecentFiles();

			mnuEditUndo.Enabled = canvas.CanUndo();
			mnuEditUndo.Text = "&Undo " + canvas.GetUndoDescription();
			mnuEditRedo.Enabled = canvas.CanRedo();
			mnuEditRedo.Text = "&Redo " + canvas.GetRedoDescription();

			mnuEditShadePolygons.Enabled = (ImageDef != null);
			mnuEditShadePolygons.Checked = (ImageDef != null) && ImageDef.Shading;

			mnuViewShowOutlines.Checked = canvas.ShowPolygonOutlines;
			RefreshZoom();
			txtImageOpacity.Value = canvas.ImageOpacity;

			mnuViewFps.Checked = canvas.ShowFps;

			EnableChildControls(pnlTop, ImageDef != null);

			if (System.Diagnostics.Debugger.IsAttached)
			{
				var canvasMousePos = canvas.PointToClient(MousePosition);
				if (canvas.ClientRectangle.Contains(canvasMousePos) || canvas.IsMouseDown)
				{
					var imageMousePos = canvas.ClientToImage(canvasMousePos).Round();

					imageMousePos = canvas.GetImagePointSnapped(imageMousePos);

					lblMouseImageCoordinates.Text =
						string.Format("{0}, {1}", canvasMousePos.X, canvasMousePos.Y) + " - " +
						string.Format("{0}, {1}", imageMousePos.X, imageMousePos.Y);
					this.Refresh();
				}
				else
				{
					lblMouseImageCoordinates.Text = "";
				}
			}
			else
			{
				lblMouseImageCoordinates.Text = "";
			}

			if (ImageDef == null)
			{
				lblPolygonCount.Text = "";
			}
			else
			{
				lblPolygonCount.Text = string.Format("{0} Polygons. {1}% Coverage",
					canvas.Image.Polygons.Count,
					Math.Truncate(canvas.CalcPolygonCoverage() * 100));
			}

			RefreshToolButton(btnPolygonTool);
			RefreshToolButton(btnEraseTool);
		}

		private void RefreshToolButton(ToolStripButton button)
		{
			button.Checked = (canvas.ActiveToolType == (Type)button.Tag);
		}

		private void EnableChildControls(Control parent, bool enabled)
		{
			foreach (Control control in parent.Controls)
			{
				control.Enabled = enabled;
			}
		}

		private void RefreshZoom()
		{
			string zoomStr = string.Format("{0:0.#}%", canvas.Zoom);

			if (!zoomStr.Equals(cboZoom.Text, StringComparison.OrdinalIgnoreCase))
				cboZoom.Text = zoomStr;
		}

		private void RefreshRecentFiles()
		{
			for (var index = 0; index < Settings.RecentFileNames.Count; index++)
			{
				ToolStripMenuItem menuItem;
				if (index < mnuFileOpenRecent.DropDownItems.Count)
					menuItem = (ToolStripMenuItem)mnuFileOpenRecent.DropDownItems[index];
				else
				{
					menuItem = new ToolStripMenuItem("", null, mnuFileOpenRecentFile_Click);
					mnuFileOpenRecent.DropDownItems.Add(menuItem);
				}
				menuItem.Text = Settings.RecentFileNames[index];
			}

			while (mnuFileOpenRecent.DropDownItems.Count > Settings.RecentFileNames.Count)
			{
				mnuFileOpenRecent.DropDownItems.RemoveAt(Settings.RecentFileNames.Count);
			}

			mnuFileOpenRecent.Visible = mnuFileOpenRecent.DropDownItems.Count > 0;
		}

		private void mnuFileOpenRecentFile_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			string fileName = menuItem.Text;

			OpenFile(fileName);
		}

		private void OpenFile(string fileName)
		{
			ImageDef = ImageDef.LoadFromFile(fileName);
			imageFileName = fileName;
			Settings.RecentFileNames.MostRecent = imageFileName;
			hasUnsavedChanges = false;

			canvas.ResetZoomAndCenter();

			RefreshControlStates();
		}

		private DialogResult CheckSaveChanges()
		{
			if (hasUnsavedChanges)
				switch (MessageBox.Show("Would you like to save your changes first?", "Save changes?", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.No:
						// Don't save changes
						return DialogResult.No;

					case DialogResult.OK:
					case DialogResult.Yes:
						mnuFileSave.PerformClick();
						return DialogResult.OK;

					default:
						return DialogResult.Cancel;
				}

			return DialogResult.OK;
		}

		private void mnuFileOpen_Click(object sender, EventArgs e)
		{
			if (CheckSaveChanges() == DialogResult.Cancel)
				return;

			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = "Open Polygen Image";
				dlg.Filter = "Polygen Images (*.pim)|*.pim|All Files (*.*)|*.*";
				dlg.CheckFileExists = true;

				if (dlg.ShowDialog() == DialogResult.OK)
				{
					OpenFile(dlg.FileName);
				}
			}
		}

		private void mnuFileSave_Click(object sender, EventArgs e)
		{
			if (ImageDef == null)
				return;

			if (imageFileName == "")
			{
				mnuFileSaveAs.PerformClick();
				return;
			}

			ImageDef.SaveToFile(imageFileName);
			hasUnsavedChanges = false;
			RefreshControlStates();
		}

		private void mnuFileSaveAs_Click(object sender, EventArgs e)
		{
			using (var dlg = new SaveFileDialog())
			{
				dlg.Title = "Save Polygen Image";
				dlg.Filter = "Polygen Images (*.pim)|*.pim|All Files (*.*)|*.*";
				dlg.OverwritePrompt = true;

				if (dlg.ShowDialog() == DialogResult.OK)
				{
					ImageDef.SaveToFile(dlg.FileName);
					imageFileName = dlg.FileName;
					Settings.RecentFileNames.MostRecent = imageFileName;
					hasUnsavedChanges = false;
				}
			}
			RefreshControlStates();
		}

		private void mnuFileClose_Click(object sender, EventArgs e)
		{
			if (CheckSaveChanges() == DialogResult.Cancel)
				return;

			ImageDef = null;
			imageFileName = "";

			RefreshControlStates();
		}

		private void mnuEditSnapToHandles_Click(object sender, EventArgs e)
		{
			SnapToHandles = !SnapToHandles;
		}

		public bool SnapToHandles
		{
			get { return mnuEditSnapToHandles.Checked; }
			set
			{
				mnuEditSnapToHandles.Checked = value;
				canvas.Snapping = value;
				Settings.Snapping = value;
			}
		}

		private void mnuEditUndo_Click(object sender, EventArgs e)
		{
			if ((ImageDef == null) || (!canvas.CanUndo()))
				return;

			canvas.Undo();

			hasUnsavedChanges = true;
			RefreshControlStates();
		}

		private void mnuEditRedo_Click(object sender, EventArgs e)
		{
			if ((ImageDef == null) || (!canvas.CanRedo()))
				return;

			canvas.Redo();

			hasUnsavedChanges = true;
			RefreshControlStates();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.Save();
		}

		private void canvas_ZoomChanged(object sender, EventArgs e)
		{
			cboZoom.Text = string.Format("{0:0.##}%", canvas.Zoom);
		}

		private void mnuUpdatePolygonColors_Click(object sender, EventArgs e)
		{
			if (ImageDef != null)
			{
				ImageDef.UpdatePolygonColors();

				canvas.MadeChanges("Update colours");
			}
		}

		private void canvas_ChangesMade(object sender, EventArgs e)
		{
			if (ImageDef != null)
				hasUnsavedChanges = true;
			RefreshControlStates();
			canvas.Invalidate();
		}

		private void cboZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			var zoomStr = cboZoom.Text.Replace("%", "");
			double zoom;

			if (double.TryParse(zoomStr, out zoom))
				canvas.Zoom = zoom;
		}

		private void mnuEditClearAllPolygons_Click(object sender, EventArgs e)
		{
			ImageDef.Polygons.Clear();
			canvas.MadeChanges("Clear all polygons");
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (CheckSaveChanges() == DialogResult.Cancel)
				e.Cancel = true;
		}

		private void canvas_MouseMove(object sender, MouseEventArgs e)
		{
			RefreshControlStates();
		}

		private void mnuViewShowOutlines_Click(object sender, EventArgs e)
		{
			ShowPolygonOutlines = !ShowPolygonOutlines;
		}

		public bool ShowPolygonOutlines
		{
			get
			{
				return Settings.ShowPolygonOutlines;
			}
			set
			{
				Settings.ShowPolygonOutlines = value;
				canvas.ShowPolygonOutlines = value;
				RefreshControlStates();
				canvas.Invalidate();
			}
		}

		private void txtImageOpacity_ValueChanged(object sender, EventArgs e)
		{
			ImageOpacity = (byte)Math.Truncate(txtImageOpacity.Value);
		}

		public byte ImageOpacity
		{
			get
			{
				return Settings.ImageOpacity;
			}
			set
			{
				Settings.ImageOpacity = value;
				canvas.ImageOpacity = value;

				if (value != txtImageOpacity.Value)
					txtImageOpacity.Value = value;
			}
		}

		private void btnTool_Click(object sender, EventArgs e)
		{
			var button = (ToolStripButton)sender;
			canvas.ActiveToolType = (Type)button.Tag;
			RefreshControlStates();
			canvas.Invalidate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var largestPolygon = ImageDef.Polygons.
				OrderByDescending(polygon => GeometeryUtils.GetPolygonArea(polygon.Points.Select(point => point.Point).ToArray()))
				.FirstOrDefault();


			var timer = new System.Diagnostics.Stopwatch();
			const int loopCount = 10;
			TimeSpan test1;
			TimeSpan test2;

			if (largestPolygon != null)
			{
				var bmp = ImageDef.GetBaseImage().GetBitmap();

				timer.Start();
				for (var loopIndex = 0; loopIndex < loopCount; loopIndex++)
				{
					GeometeryUtils.GetAllPointsInPolygonUsingGraphicsPath(
						largestPolygon.Points.Select(p => p.Point).ToArray(), bmp).ToArray();
				}
				timer.Stop();
				test1 = timer.Elapsed;


				timer.Restart();
				for (var loopIndex = 0; loopIndex < loopCount; loopIndex++)
				{
					GeometeryUtils.GetAllPointsInPolygonUsingRayTrace(
						largestPolygon.Points.Select(p => p.Point).ToArray()).ToArray();
				}
				timer.Stop();
				test2 = timer.Elapsed;

				MessageBox.Show(string.Format("Test 1 = {0}ms\nTest 2 = {1}ms", test1.TotalMilliseconds, test2.TotalMilliseconds));
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ImageDef.UpdatePolygonPointColors();
			canvas.Invalidate();
		}

		private void mnuClampPointsToImage_Click(object sender, EventArgs e)
		{
			var imageRect = new Rectangle(0, 0, ImageDef.Width, ImageDef.Height);

			foreach (var polygon in ImageDef.Polygons)
			{
				for (var pointIndex = 0; pointIndex < polygon.Points.Count; pointIndex++)
				{
					if (!imageRect.Contains(polygon.Points[pointIndex]))
						polygon.Points[pointIndex] = new ColoredPoint(
							Math.Min(Math.Max(0, polygon.Points[pointIndex].X), imageRect.Right),
							Math.Min(Math.Max(0, polygon.Points[pointIndex].Y), imageRect.Bottom),
							polygon.Points[pointIndex].Color);
				}
			}

			canvas.MadeChanges("Constrained Points Within Image");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			// Some cursors seem to be lying about their hotspot locations,
			// at least in my environment for some reason...
			var cursors = new Cursor[] {
				WinFormsCursors.AppStarting,
				WinFormsCursors.Arrow,
				WinFormsCursors.Cross,
				WinFormsCursors.Default,
				WinFormsCursors.Hand,
				WinFormsCursors.Help,
				WinFormsCursors.HSplit,
				WinFormsCursors.IBeam,
				WinFormsCursors.No,
				WinFormsCursors.NoMove2D,
				WinFormsCursors.NoMoveHoriz,
				WinFormsCursors.NoMoveVert,
				WinFormsCursors.PanEast,
				WinFormsCursors.PanNE,
				WinFormsCursors.PanNorth,
				WinFormsCursors.PanNW,
				WinFormsCursors.PanSE,
				WinFormsCursors.PanSouth,
				WinFormsCursors.PanSW,
				WinFormsCursors.PanWest,
				WinFormsCursors.SizeAll,
				WinFormsCursors.SizeNESW,
				WinFormsCursors.SizeNS,
				WinFormsCursors.SizeNWSE,
				WinFormsCursors.SizeWE,
				WinFormsCursors.UpArrow,
				WinFormsCursors.VSplit,
				WinFormsCursors.WaitCursor
			};

			var size = new Size();

			size.Width = cursors.Max(c => c.Size.Width);
			size.Height = cursors.Sum(c => c.Size.Height);

			using (var bmp = new Bitmap(size.Width, size.Height))
			{
				using (var graphics = Graphics.FromImage(bmp))
				{
					graphics.FillRectangle(Brushes.White, new Rectangle(new Point(0, 0), size));

					int top = 0;

					foreach (var cursor in cursors)
					{
						var rect = new Rectangle(new Point(0, top), cursor.Size);
						cursor.Draw(graphics, rect);

						graphics.DrawLine(
							Pens.Red,
							new Point(cursor.HotSpot.X - 1, top + cursor.HotSpot.Y),
							new Point(cursor.HotSpot.X + 1, top + cursor.HotSpot.Y));

						graphics.DrawLine(
							Pens.Red,
							new Point(cursor.HotSpot.X, top + cursor.HotSpot.Y - 1),
							new Point(cursor.HotSpot.X, top + cursor.HotSpot.Y + 1));

						//graphics.FillCircle(Brushes.Red, new Point(cursor.HotSpot.X, top + cursor.HotSpot.Y), 1);

						top += cursor.Size.Height;
					}
				}

				

				const string fileName = @"C:\Temp\Cursors.png";

				bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

				System.Diagnostics.Process.Start("mspaint.exe", fileName);
			}
		}

		private void mnuHelpAbout_Click(object sender, EventArgs e)
		{
			// Who can be bothered updating copyright information each year, am I right?
			MessageBox.Show(string.Format(
				"Copyright codeandcats.com {0}.\nContact: codeandcats@gmail.com",
				DateTime.Now.Year));
		}

		private void mnuViewFps_Click(object sender, EventArgs e)
		{
			canvas.ShowFps = !canvas.ShowFps;

			RefreshControlStates();
		}

		private void mnuFileExportImage_Click(object sender, EventArgs e)
		{
			if (ImageDef == null)
			{
				throw new Exception("No image open");
			}

			using (var dlg = new SaveFileDialog())
			{
				dlg.Title = "Export Polygened Image";
				dlg.Filter = "Jpeg (*.jpg; *.jpeg)|*.jpg; *.jpeg|Png (*.png)|*.png";
				dlg.FilterIndex = 0;
				dlg.OverwritePrompt = true;
				dlg.InitialDirectory = Path.GetDirectoryName(ImageDef.BaseFileName);
				dlg.FileName = Path.GetFileNameWithoutExtension(ImageDef.BaseFileName) + " - Polygened";
				dlg.AddExtension = true;
				dlg.SupportMultiDottedExtensions = true;

				if (dlg.ShowDialog() == DialogResult.OK)
				{
					var format =
						Path.GetExtension(dlg.FileName).Equals(".png", StringComparison.CurrentCultureIgnoreCase) ?
						System.Drawing.Imaging.ImageFormat.Png :
						System.Drawing.Imaging.ImageFormat.Jpeg;

					canvas.ExportImage(dlg.FileName, format);
					
					imageFileName = dlg.FileName;
					hasUnsavedChanges = false;
				}
			}
			RefreshControlStates();
		}

		private void mnuEditShadePolygons_Click(object sender, EventArgs e)
		{
			if (ImageDef == null)
				return;

			ImageDef.Shading = !ImageDef.Shading;

			canvas.MadeChanges(ImageDef.Shading ? "Enabled Shading" : "Disabled Shading");
		}
	}
}