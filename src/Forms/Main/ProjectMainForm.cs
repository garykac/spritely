using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ProjectMainForm : Form
	{
		public string AppName = "Spritely";

		private Document m_doc;
		private RecentFiles m_recent;

		private TabMgr[] m_tabs;
		private TabMgr m_tabCurrent;

		/// <summary>
		/// Debug window to display the current contents of the Undo stack.
		/// </summary>
		private UndoHistoryForm m_undoHistory;

		/// <summary>
		/// Internal flag for UndoHistory visible state.
		/// Use UndoHistoryVisible instead.
		/// </summary>
		private bool m_fUndoHistoryVisible = false;

		/// <summary>
		/// Is the UndoHistory window visible? (debug only)
		/// </summary>
		public bool UndoHistoryVisible
		{
			get { return m_fUndoHistoryVisible; }
			set
			{
				m_fUndoHistoryVisible = value;
				if (!m_fUndoHistoryVisible)
					m_undoHistory.Hide();
				else
					m_undoHistory.Show();
			}
		}

		public ProjectMainForm(string strFilename)
		{
			bool fNewDocument = true;

			InitializeComponent();

			// If we were given a filename, than open the specified file.
			if (strFilename != "")
			{
				m_doc = new Document(this);
				if (m_doc.Open(strFilename))
				{
					fNewDocument = false;
					SetTitleBar(m_doc.Name);
				}
			}

			// otherwise, create a brand new (empty) document.
			if (fNewDocument)
				Handle_NewDocument();

			// Init the list of recent files.
			m_recent = new RecentFiles(this, menuFile_RecentFiles);
		}

		public int ContentWidth
		{
			get { return ClientSize.Width - 6; }
		}

		public int ContentHeight
		{
			get { return ClientSize.Height - menuBar.Height - tabSet.Height - 6; }
		}

		private void SetTitleBar(string strFilename)
		{
			this.Text = String.Format("{0} - {1}", AppName, strFilename);
		}

		#region Window events

		private void ProjectMainForm_Load(object sender, EventArgs e)
		{
			TabMgr.ResizeMainForm(this);

			m_tabs = new TabMgr[(int)TabMgr.TabId.MAX];

			TabMgr tabSprites = new TabMgr(this, TabMgr.TabId.Sprites);
			m_tabs[(int)TabMgr.TabId.Sprites] = tabSprites;

			TabMgr tabBackgroundMaps = new TabMgr(this, TabMgr.TabId.BackgroundMaps);
			m_tabs[(int)TabMgr.TabId.BackgroundMaps] = tabBackgroundMaps;

			TabMgr tabBackgroundImages = new TabMgr(this, TabMgr.TabId.BackgroundImages);
			m_tabs[(int)TabMgr.TabId.BackgroundImages] = tabBackgroundImages;

			m_tabCurrent = tabSprites;
			tabSet.SelectedIndex = (int)m_tabCurrent.Id;

			AddProjectMenuItems();
			AddSubwindowsToTabs();
			HandleEverythingChanged();

			m_undoHistory = new UndoHistoryForm(this);
			UndoHistoryVisible = false;
			ClearUndo();

			m_doc.HasUnsavedChanges = false;

			// Add an 
			Application.Idle += new EventHandler(OnIdle);
		}

		private void ProjectMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// The close event has already been sent to the frontmost child window,
			// which hides itself and cancels the Close.
			// This causes 2 problems:
			//   If we show a dialog (e.g., to ask if we need to save changes), then the
			//     front child window will disappear when the dialog is shown.
			//   The main window Close is cancelled.
			// To fix this, show all current windows in the tab...
			m_tabCurrent.ShowWindows();
			// ... and un-cancel the Close so that the main form can close.
			e.Cancel = false;

			// Make sure the document is closed properly.
			if (m_doc != null && !m_doc.Close())
			{
				e.Cancel = true;
				return;
			}
		}

		private void ProjectMainForm_Shown(object sender, EventArgs e)
		{
			m_tabCurrent.ShowWindows();
		}

		private void OnIdle(object sender, EventArgs e)
		{
			ActivateMenuItems();
		}

		#endregion

		#region Document

		public Document Document
		{
			get { return m_doc; }
		}

		public void Handle_NewDocument()
		{
			m_doc = new Document(this);
			m_doc.InitializeEmptyDocument();

			SetTitleBar("Untitled");
		}

		/// <summary>
		/// The undo manager for the currently active tab.
		/// </summary>
		/// <returns></returns>
		public UndoMgr ActiveUndo()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			return m_tabCurrent.Undo;
		}

		public Palette ActivePalette()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return m_doc.Palettes.CurrentPalette;
			else if (m_tabCurrent.Id == TabMgr.TabId.BackgroundMaps)
				return m_doc.BackgroundPalettes.CurrentPalette;
			return null;
		}

		public Spriteset ActiveSpriteset()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return m_doc.Spritesets.Current;
			else if (m_tabCurrent.Id == TabMgr.TabId.BackgroundMaps)
				return m_doc.BackgroundSpritesets.Current;
			return null;
		}

		public Sprite ActiveSprite()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			Spritesets ss;
			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				ss = m_doc.Spritesets;
			else if (m_tabCurrent.Id == TabMgr.TabId.BackgroundMaps)
				ss = m_doc.BackgroundSpritesets;
			else
				return null;

			if (ss.Current != null)
				return ss.Current.CurrentSprite;
			return null;
		}

		public Map ActiveMap()
		{
			if (m_doc == null || m_tabCurrent == null)
				return null;

			if (m_tabCurrent.Id == TabMgr.TabId.Sprites)
				return null;
			else if (m_tabCurrent.Id == TabMgr.TabId.BackgroundMaps)
				return m_doc.BackgroundMaps.CurrentMap;
			return null;
		}

		#endregion

		#region Tabs

		public TabMgr CurrentTab
		{
			get { return m_tabCurrent; }
		}

		public TabMgr GetTab(TabMgr.TabId id)
		{
			return m_tabs[(int)id];
		}

		public TabMgr GetTab(int id)
		{
			return m_tabs[id];
		}

		public void ShowTabs()
		{
			tabSet.Show();
		}

		public void HideTabs()
		{
			tabSet.Hide();
		}

		private void tabSet_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			TabPage tp = tabSet.TabPages[e.Index];
			string strTabName = tp.Text;

			Rectangle r = e.Bounds;
			SizeF size = g.MeasureString(strTabName, e.Font);
			float x = r.X + (r.Width - size.Width) / 2;
			float y = r.Y + 4;

			if (tabSet.SelectedIndex == e.Index)
			{
				g.FillRectangle(Brushes.White, r);
				g.DrawString(strTabName, e.Font, Brushes.Black, x, y + 1);
			}
			else
			{
				Color c = ControlPaint.Dark(this.BackColor, 0.01f);
				System.Drawing.Drawing2D.LinearGradientBrush b = new System.Drawing.Drawing2D.LinearGradientBrush(
					new Point(0, 0), new Point(0, 30), Color.White, c);
				g.FillRectangle(b, r);
				g.DrawString(strTabName, e.Font, Brushes.Black, x, y);
				//g.DrawLine(Pens.Black, r.Left, r.Bottom, r.Right, r.Bottom);
			}

			// Draw a line out past the last tab.
			//if (e.Index == tabSet.TabPages.Count - 1)
			//{
			//	g.DrawLine(Pens.Black, r.Right, r.Bottom, r.Right+tabSet.Width, r.Bottom);
			//}
		}

		private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Hide windows from old tab.
			m_tabCurrent.HideWindows();

			// Show windws for new tab.
			m_tabCurrent = m_tabs[tabSet.SelectedIndex];
			m_tabCurrent.ShowWindows();
		}

		#endregion

		#region Undo

		/// <summary>
		/// Clear all the undo history.
		/// </summary>
		public void ClearUndo()
		{
			m_undoHistory.Clear();
			for (int i = 0; i < m_tabs.Length; i++)
			{
				m_tabs[i].Undo.Reset();
			}
		}

		public void AddUndo(TabMgr.TabId id, UndoAction action)
		{
			m_undoHistory.Add(id, action);
		}

		public void RemoveUndo(TabMgr.TabId id, int nIndex)
		{
			m_undoHistory.Remove(id, nIndex);
		}

		/// <summary>
		/// Remove a range of undo actions.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="nStart">Index of the first undo action to remove</param>
		/// <param name="nCount">Number of undo actions to remove</param>
		public void RemoveUndoRange(TabMgr.TabId id, int nStart, int nCount)
		{
			m_undoHistory.RemoveRange(id, nStart, nCount);
		}

		public void SetCurrentUndo(TabMgr.TabId id, int nCurrent)
		{
			m_undoHistory.SetCurrent(id, nCurrent);
		}

		#endregion
		
		#region Subwindow Management

		public void ResizeSubwindow(Form f)
		{
			if (f.WindowState == FormWindowState.Maximized)
			{
				// When maximized: hide the tabs and enable the control box.
				// The control box is needed to allow the user to un-minimize the window.
				HideTabs();
				if (!f.ControlBox)
					f.ControlBox = true;
			}
			else
			{
				// Normal display mode: show the tabs and disable the control bar.
				// The control bar shows the min/max/close boxes in the window and
				// we don't want them for our subwindows.
				ShowTabs();
				// Turn off the control box only if it is already turned on.
				// Otherwise, the subwindow will start to shrink mysteriously when
				// you switch tabs.
				if (f.ControlBox)
					f.ControlBox = false;
			}
		}

		/// <summary>
		/// Close (actually, just hide) the specified subwindow.
		/// </summary>
		/// <param name="f"></param>
		public void CloseSubwindow(Form f)
		{
			// Un-maximize the window before closing.
			if (f.WindowState == FormWindowState.Maximized)
			{
				f.WindowState = FormWindowState.Normal;
			}

			// Hide the subwindow instead of closing.
			//f.Hide();
		}

		public void AddSubwindowsToTabs()
		{
			TabMgr tabSprites = GetTab(TabMgr.TabId.Sprites);
			tabSprites.AddSpritesetWindow(m_doc.Spritesets.Current.SpritesetWindow);
			tabSprites.AddSpriteWindow(m_doc.Spritesets.Current.SpriteWindow);
			tabSprites.AddPaletteWindow(m_doc.Palettes.CurrentPalette.PaletteWindowForm());
			tabSprites.ArrangeWindows();

			TabMgr tabBackgroundMaps = GetTab(TabMgr.TabId.BackgroundMaps);
			tabBackgroundMaps.AddSpritesetWindow(m_doc.BackgroundSpritesets.Current.SpritesetWindow);
			tabBackgroundMaps.AddSpriteWindow(m_doc.BackgroundSpritesets.Current.SpriteWindow);
			tabBackgroundMaps.AddPaletteWindow(m_doc.BackgroundPalettes.CurrentPalette.PaletteWindowForm());
			tabBackgroundMaps.AddMapWindow(m_doc.BackgroundMaps.CurrentMap.MapWindow);
			tabBackgroundMaps.ArrangeWindows();

			TabMgr tabBackgroundImages = GetTab(TabMgr.TabId.BackgroundImages);
			tabBackgroundImages.AddBgImageListWindow(m_doc.BackgroundImages.BgImageListWindow);
			tabBackgroundImages.AddBgImageWindow(m_doc.BackgroundImages.BgImageWindow);
			tabBackgroundImages.ArrangeWindows();
		}

		/// <summary>
		/// When a document is closed, we delete all subwindows.
		/// </summary>
		public void DeleteAllSubwindows()
		{
			for (int i=0; i<(int)TabMgr.TabId.MAX; i++)
				GetTab(i).RemoveAllWindows();
			GC.Collect();
		}

		/// <summary>
		/// We've just created a new document or loaded one from a file.
		/// Everything has changed, so make sure everything is updated.
		/// </summary>
		public void HandleEverythingChanged()
		{
			m_doc.Spritesets.Current.FlushBitmaps();
			HandleSpriteDataChanged(m_doc.Spritesets.Current);
			HandleSpriteTypeChanged(m_doc.Spritesets.Current);
			HandleSubpaletteSelectChange(m_doc.Palettes.CurrentPalette);
			HandleColorDataChange(m_doc.Palettes.CurrentPalette);

			m_doc.BackgroundSpritesets.Current.FlushBitmaps();
			HandleSpriteDataChanged(m_doc.BackgroundSpritesets.Current);
			HandleSpriteTypeChanged(m_doc.BackgroundSpritesets.Current);
			HandleSubpaletteSelectChange(m_doc.BackgroundPalettes.CurrentPalette);
			HandleColorDataChange(m_doc.BackgroundPalettes.CurrentPalette);
			HandleMapDataChange(m_doc.BackgroundMaps.CurrentMap);

			HandleBackgroundImageListChanged(m_doc.BackgroundImages);
		}

		/// <summary>
		/// The target platform (GBA/NDS) has changed.
		/// </summary>
		public void HandlePlatformChanged()
		{
			m_doc.BackgroundMaps.CurrentMap.MapWindow.PlatformChanged();
		}

		/// <summary>
		/// The current sprite seletion has changed.
		/// </summary>
		public void HandleSpriteSelectionChanged(Spriteset ss)
		{
			//m_doc.HasUnsavedChanges = true;
			ss.Palette.PaletteWindow().SpriteSelectionChanged();
			ss.SpritesetWindow.SpriteSelectionChanged();
			ss.SpriteWindow.SpriteSelectionChanged();
		}

		/// <summary>
		/// The data for one (or more) of the sprites has changed.
		/// </summary>
		public void HandleSpriteDataChanged(Spriteset ss)
		{
			m_doc.HasUnsavedChanges = true;
			ss.SpritesetWindow.SpriteDataChanged();
			ss.SpriteWindow.SpriteDataChanged();

			if (ss.IsBackground)
			{
				Map m = ActiveMap();
				if (m != null)
					m.MapWindow.SpriteDataChanged();
			}
		}

		/// <summary>
		/// The sprite display options have changed..
		/// </summary>
		public void HandleSpriteDisplayOptionsChanged(Spriteset ss)
		{
			ss.Palette.PaletteWindow().SpriteDisplayOptionChanged();
			ss.SpriteWindow.SpriteDisplayOptionChanged();
		}

		/// <summary>
		/// One of the spritetypes has changed.
		/// E.g., by having a sprite added, deleted or having its tile geometry changed.
		/// </summary>
		public void HandleSpriteTypeChanged(Spriteset ss)
		{
			m_doc.HasUnsavedChanges = true;

			SpritesetForm win = ss.SpritesetWindow;
			if (win != null)
			{
				win.RecalcScrollHeights();
				win.AdjustScrollbar();
			}
		}

		/// <summary>
		/// A new subpalette has been selected, notify all other windows that are potentially
		/// impacted by this change
		/// </summary>
		public void HandleSubpaletteSelectChange(Palette p)
		{
			p.PaletteWindow().SubpaletteSelectChanged();
			Spriteset ss;
			if (p.IsBackground)
			{
				ss = m_doc.BackgroundSpritesets.Current;
				Map m = ActiveMap();
				if (m != null)
					m.MapWindow.SubpaletteSelectChanged();
			}
			else
			{
				ss = m_doc.Spritesets.Current;
			}

			ss.SpritesetWindow.SubpaletteSelectChanged();
			ss.SpriteWindow.SubpaletteSelectChanged();
		}

		/// <summary>
		/// A new color has been selected in the subpalette. Update everyone
		/// who needs to be notified.
		/// </summary>
		public void HandleColorSelectChange(Palette p)
		{
			p.PaletteWindow().ColorSelectChanged();
		}

		/// <summary>
		/// A color value has changed in the current palette.
		/// </summary>
		public void HandleColorDataChange(Palette p)
		{
			m_doc.FlushBitmaps();

			p.PaletteWindow().ColorDataChanged();
			Spriteset ss;
			if (p.IsBackground)
			{
				ss = m_doc.BackgroundSpritesets.Current;
				Map m = ActiveMap();
				if (m != null)
					m.MapWindow.ColorDataChanged();
			}
			else
			{
				ss = m_doc.Spritesets.Current;
			}

			ss.SpritesetWindow.ColorDataChanged();
			ss.SpriteWindow.ColorDataChanged();
		}

		/// <summary>
		/// The map data has changed.
		/// </summary>
		public void HandleMapDataChange(Map m)
		{
			m_doc.HasUnsavedChanges = true;
			m.MapWindow.MapDataChanged();
		}

		/// <summary>
		/// The background image list has changed.
		/// E.g., by having an image added or deleted.
		/// </summary>
		public void HandleBackgroundImageListChanged(BgImages bgis)
		{
			m_doc.HasUnsavedChanges = true;
			BgImageListForm win = bgis.BgImageListWindow;
			if (win != null)
			{
				win.RecalcScrollHeights();
				win.AdjustScrollbar();
			}
		}

		/// <summary>
		/// The current background image seletion has changed.
		/// </summary>
		public void HandleBackgroundImageSelectionChanged(BgImages bgis)
		{
			bgis.BgImageListWindow.BgImageSelectionChanged();
			bgis.BgImageWindow.BgImageSelectionChanged();
		}

		#endregion
			
		#region User Messaging

		/// <summary>
		/// A dummy function on which we can set a debug breakpoint.
		/// This routine is called in some circumstances where the code is not
		/// functioning correctly/efficiently, but we don't want to show an
		/// error/warning to the user.
		/// By setting a breakpoint here, we can catch these conditions in
		/// the debugger.
		/// </summary>
		private void BreakpointCheck()
		{
		}

		public void Info(string str)
		{
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void NYI()
		{
			// "Sorry - Not Yet Implemented"
			MessageBox.Show(ResourceMgr.GetString("NYI"), AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void Warning(string str)
		{
			if (!m_doc.ShowMessageCheck)
			{
				// Oops! ProjectMainForm.Warning was called directly.
				// Call Document.WarningString/WarningId instead.
				BreakpointCheck();
			}
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// Display an error message to the user.
		/// This is called by Document.ErrorString/ErrorId and should not be called directly.
		/// </summary>
		/// <param name="str"></param>
		public void Error(string str)
		{
			if (!m_doc.ShowMessageCheck)
			{
				// Oops! ProjectMainForm.Error was called directly.
				// Call Document.ErrorString/ErrorId instead.
				// Set a breakpoint on the following line to detect this situation.
				BreakpointCheck();
			}
			MessageBox.Show(str, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Pose a yes/no question to the user
		/// </summary>
		/// <param name="str">The yes/no question to pose to the user</param>
		/// <returns>True if yes, false if no</returns>
		public bool AskYesNo(string str)
		{
			if (!m_doc.ShowMessageCheck)
			{
				// Oops! ProjectMainForm.AskYesNo was called directly.
				// Call Document.AskYesNo instead.
				BreakpointCheck();
			}
			DialogResult result = MessageBox.Show(str, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
				return true;
			return false;
		}

		/// <summary>
		/// Pose a yes/no question to the user, allowing the user to cancel
		/// </summary>
		/// <param name="str">The yes/no question to pose to the user</param>
		/// <param name="fCancel">Flag indicating if the user has cancelled</param>
		/// <returns>True if yes, false if no</returns>
		public bool AskYesNoCancel(string str, out bool fCancel)
		{
			if (!m_doc.ShowMessageCheck)
			{
				// Oops! ProjectMainForm.AskYesNoCancel was called directly.
				// Call Document.AskYesNoCancel instead.
				BreakpointCheck();
			}
			fCancel = false;
			DialogResult result = MessageBox.Show(this, str, AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (result == DialogResult.Cancel)
				fCancel = true;
			if (result == DialogResult.Yes)
				return true;
			return false;
		}

		#endregion

	}
}
