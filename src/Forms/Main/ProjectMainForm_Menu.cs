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
		/// <summary>
		/// Enable/disable menu items as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuBar_MenuActivate(object sender, EventArgs e)
		{
			ActivateMenuItems();
		}

		/// <summary>
		/// Enable/disable menu items as appropriate
		/// </summary>
		private void ActivateMenuItems()
		{
			bool fHasDoc = m_doc != null;
			bool fSpriteEditing = m_tabCurrent.Id == TabMgr.TabId.Sprites || m_tabCurrent.Id == TabMgr.TabId.BackgroundMaps;

			// Enable/disable File menu items
			menuFile.Enabled = true;
			menuFile_New.Enabled = true;
			menuFile_Open.Enabled = true;
			menuFile_Close.Enabled = fHasDoc;
			menuFile_Save.Enabled = fHasDoc;
			menuFile_SaveAs.Enabled = fHasDoc;
			menuFile_Export.Enabled = fHasDoc;
			menuFile_RecentFiles.Enabled = (m_recent.Count != 0);
			menuFile_Exit.Enabled = true;

			menuEdit.Enabled = true;
			UndoMgr undo = ActiveUndo();
			bool fCanUndo = fHasDoc && undo != null && undo.CanUndo();
			bool fCanRedo = fHasDoc && undo != null && undo.CanRedo();
			menuEdit_Undo.Enabled = fCanUndo;
			menuEdit_Redo.Enabled = fCanRedo;
			menuEdit_Cut.Enabled = false;
			menuEdit_Copy.Enabled = false;
			menuEdit_Paste.Enabled = false;

			menuProject.Enabled = true;
			menuProject_Platform.Enabled = true;
			menuProject_Platform_GBA.Enabled = true;
			menuProject_Platform_GBA.Checked = Options.Platform == Options.PlatformType.GBA;
			menuProject_Platform_NDS.Enabled = true;
			menuProject_Platform_NDS.Checked = Options.Platform == Options.PlatformType.NDS;
			menuProject_Palettes.Enabled = true;
			menuProject_Palettes_New.Enabled = false;
			menuProject_Spritesets.Enabled = true;
			menuProject_Spritesets_New.Enabled = false;
			menuProject_BackgroundPalettes.Enabled = true;
			menuProject_BackgroundPalettes_New.Enabled = false;
			menuProject_BackgroundTilesets.Enabled = true;
			menuProject_BackgroundTilesets_New.Enabled = false;
			menuProject_BackgroundMaps.Enabled = true;
			menuProject_BackgroundMaps_New.Enabled = false;

			// Enable/disable Sprite menu items
			menuSprite.Enabled = true;
			Sprite s = ActiveSprite();
			if (s != null)
			{
				menuSprite_New.Enabled = true;
				menuSprite_Duplicate.Enabled = true;
				menuSprite_Clear.Enabled = !s.IsEmpty();
				menuSprite_Resize.Enabled = true;
				menuSprite_Delete.Enabled = true;

				menuSprite_Resize_1x1.Enabled = !s.IsSize(1, 1);
				menuSprite_Resize_1x2.Enabled = !s.IsSize(1, 2);
				menuSprite_Resize_1x4.Enabled = !s.IsSize(1, 4);
				menuSprite_Resize_2x1.Enabled = !s.IsSize(2, 1);
				menuSprite_Resize_2x2.Enabled = !s.IsSize(2, 2);
				menuSprite_Resize_2x4.Enabled = !s.IsSize(2, 4);
				menuSprite_Resize_4x1.Enabled = !s.IsSize(4, 1);
				menuSprite_Resize_4x2.Enabled = !s.IsSize(4, 2);
				menuSprite_Resize_4x4.Enabled = !s.IsSize(4, 4);

				//bool fFirst, fLast;
				//tab.SpriteList.IsFirstLastSpriteOfType(s, out fFirst, out fLast);
				menuSprite_Properties.Enabled = true;
				menuSprite_Rotate.Enabled = true;
				menuSprite_Rotate_Clockwise.Enabled = true;
				menuSprite_Rotate_Counterclockwise.Enabled = true;
				menuSprite_Flip.Enabled = true;
				menuSprite_Flip_Horizontal.Enabled = true;
				menuSprite_Flip_Vertical.Enabled = true;
				menuSprite_Arrange.Enabled = false;
				menuSprite_Arrange_MoveUp.Enabled = false;// !fFirst;
				menuSprite_Arrange_MoveDown.Enabled = false;// !fLast;
			}
			else
			{
				// The 'new sprite' option is always enabled when were in sprite editing mode.
				menuSprite_New.Enabled = fHasDoc && fSpriteEditing;

				// Disable all sprite editing options if there is no sprite selection or
				// if we're not editing sprites.
				menuSprite_Duplicate.Enabled = false;
				menuSprite_Clear.Enabled = false;
				menuSprite_Resize.Enabled = false;
				menuSprite_Delete.Enabled = false;
				menuSprite_Properties.Enabled = false;
				menuSprite_Rotate.Enabled = false;
				menuSprite_Rotate_Clockwise.Enabled = false;
				menuSprite_Rotate_Counterclockwise.Enabled = false;
				menuSprite_Flip.Enabled = false;
				menuSprite_Flip_Horizontal.Enabled = false;
				menuSprite_Flip_Vertical.Enabled = false;
				menuSprite_Arrange.Enabled = false;
				menuSprite_Arrange_MoveUp.Enabled = false;
				menuSprite_Arrange_MoveDown.Enabled = false;
			}

			// Enable/disable Palette menu items
			menuPalette.Enabled = true;
			Palette p = ActivePalette();
			bool fValidPalette = false;
			if (p.PaletteType == Palette.Type.Color16)
			{
				Palette16 p16 = p as Palette16;
				if (p16.GetCurrentSubpalette() != null)
					fValidPalette = true;
			} else {
				fValidPalette = true;
			}
			if (fValidPalette)
			{
				menuPalette_Copy.Enabled = false;
				menuPalette_Paste.Enabled = false;
				menuPalette_Clear.Enabled = false;
				menuPalette_ViewEncoding.Enabled = fSpriteEditing;
				menuPalette_Color.Enabled = false;
				menuPalette_Color_Copy.Enabled = false;
				menuPalette_Color_Paste.Enabled = false;
				menuPalette_Color_Clear.Enabled = false;
			}
			else
			{
				menuPalette_Copy.Enabled = false;
				menuPalette_Paste.Enabled = false;
				menuPalette_Clear.Enabled = false;
				menuPalette_ViewEncoding.Enabled = false;
				menuPalette_Color.Enabled = false;
				menuPalette_Color_Copy.Enabled = false;
				menuPalette_Color_Paste.Enabled = false;
				menuPalette_Color_Clear.Enabled = false;
			}

			menuWindow.Enabled = true;
			menuWindow_Arrange.Enabled = fHasDoc;

			menuHelp.Enabled = true;
			menuHelp_About.Enabled = true;

			// Used for debugging only - set to false for release builds.
			menuTest.Visible = Options.DEBUG;
			menuTest_RunUnittests.Visible = true;
			menuTest_ShowUndoHistory.Visible = true;
			menuTest_ShowUndoHistory.Checked = UndoHistoryVisible;
			menuTest_CollisionTest.Visible = true;
		}

		private void AddSubmenuItem(ToolStripMenuItem tsmiParent, string str)
		{
			ToolStripMenuItem tsmi = new ToolStripMenuItem(str);
			tsmi.Checked = true;
			
			// Remove any existing items (except the last 2: separator + "Add new")
			while (tsmiParent.DropDownItems.Count > 2)
				tsmiParent.DropDownItems.RemoveAt(0);

			tsmiParent.DropDownItems.Insert(0, tsmi);
		}

		private void AddProjectMenuItems()
		{
			AddSubmenuItem(menuProject_Palettes, m_doc.Palettes.GetPalette(0).Name);
			AddSubmenuItem(menuProject_Spritesets, m_doc.Spritesets.GetSpriteset(0).Name);
			AddSubmenuItem(menuProject_BackgroundPalettes, m_doc.BackgroundPalettes.GetPalette(0).Name);
			AddSubmenuItem(menuProject_BackgroundTilesets, m_doc.BackgroundSpritesets.GetSpriteset(0).Name);
			AddSubmenuItem(menuProject_BackgroundMaps, m_doc.BackgroundMaps.GetMap(0).Name);
		}

		#region File Menu

		private void menuFile_New_Click(object sender, EventArgs e)
		{
			if (m_doc != null)
			{
				if (!m_doc.Close())
					return;
				DeleteAllSubwindows();
			}

			Handle_NewDocument();
			AddProjectMenuItems();
			AddSubwindowsToTabs();
			m_tabCurrent.ShowWindows();
			HandleEverythingChanged();
			m_doc.HasUnsavedChanges = false;
		}

		private void menuFile_Open_Click(object sender, EventArgs e)
		{
			if (m_doc != null)
			{
				if (!m_doc.Close())
					return;
				DeleteAllSubwindows();
			}
			else
				m_doc = new Document(this);

			if (!m_doc.Open())
			{
				m_doc = null;
				return;
			}

			AddProjectMenuItems();
			AddSubwindowsToTabs();
			m_tabCurrent.ShowWindows();

			m_recent.AddFile(m_doc.Name);

			HandleEverythingChanged();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Close_Click(object sender, EventArgs e)
		{
			if (!m_doc.Close())
				return;
			
			DeleteAllSubwindows();
			m_doc = null;
		}

		private void menuFile_Save_Click(object sender, EventArgs e)
		{
			m_doc.Save();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_SaveAs_Click(object sender, EventArgs e)
		{
			m_doc.SaveAs();
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Export_Click(object sender, EventArgs e)
		{
			//pbS_SpriteList.Focus();
			m_doc.Export();
		}

		public void menuFile_RecentFiles_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			int nIndex = Int32.Parse(strTag);
			string strFilename = m_recent.GetNthRecentFile(nIndex);
			if (strFilename == "")
				return;

			if (!System.IO.File.Exists(strFilename))
			{
				// "The requested file '{0}' doesn't exist."
				m_doc.WarningId("FileDoesntExist", strFilename);
				m_recent.RemoveFile(strFilename);
				return;
			}

			if (m_doc != null)
			{
				if (!m_doc.Close())
					return;
				DeleteAllSubwindows();
			}
			else
				m_doc = new Document(this);

			if (!m_doc.Open(strFilename))
			{
				m_doc = null;
				return;
			}

			AddProjectMenuItems();
			AddSubwindowsToTabs();
			m_tabCurrent.ShowWindows();

			HandleEverythingChanged();
			m_doc.HasUnsavedChanges = false;
			SetTitleBar(m_doc.Name);
		}

		private void menuFile_Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region Edit Menu

		private void menuEdit_Undo_Click(object sender, EventArgs e)
		{
			UndoMgr undo = ActiveUndo();
			if (undo == null)
				return;
			undo.ApplyUndo();

			HandleEverythingChanged();
		}

		private void menuEdit_Redo_Click(object sender, EventArgs e)
		{
			UndoMgr undo = ActiveUndo();
			if (undo == null)
				return;
			undo.ApplyRedo();

			HandleEverythingChanged();
		}

		private void menuEdit_Cut_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Copy_Click(object sender, EventArgs e)
		{
		}

		private void menuEdit_Paste_Click(object sender, EventArgs e)
		{
		}

		#endregion

		#region Project menu

		private void menuProject_Platform_GBA_Click(object sender, EventArgs e)
		{
			Options.Platform = Options.PlatformType.GBA;
			HandlePlatformChanged();
		}

		private void menuProject_Platform_NDS_Click(object sender, EventArgs e)
		{
			Options.Platform = Options.PlatformType.NDS;
			HandlePlatformChanged();
		}

		#endregion

		#region Sprite menu

		private void menuSprite_New_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();

			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			string[] aSize = strTag.Split('x');
			int nWidth = Int32.Parse(aSize[0]);
			int nHeight = Int32.Parse(aSize[1]);

			ss.AddSprite(nWidth, nHeight, "", -1, "", 0, ActiveUndo());
			HandleSpriteDataChanged(ss);
		}

		private void menuSprite_Clear_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				//if (!AskYesNo("Are you sure you want to erase the data in the currently selected sprite?"))
				if (!AskYesNo(ResourceMgr.GetString("EraseCurrentSprite")))
					return;
				s.Clear();
				s.RecordUndoAction("clear", ActiveUndo());
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Duplicate_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			Sprite sToCopy = s;
			Sprite sNew = ss.DuplicateSprite(sToCopy, ActiveUndo());
			if (sNew != null)
			{
				sNew.RecordSnapshot();
				ss.CurrentSprite = sNew;
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Resize_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			string strTag = mi.Tag as string;
			if (strTag == null || strTag == "")
				return;
			string[] aSize = strTag.Split('x');
			int tileWidth = Int32.Parse(aSize[0]);
			int tileHeight = Int32.Parse(aSize[1]);

			if (ss.ResizeSelectedSprite(tileWidth, tileHeight))
			{
				s.RecordUndoAction("resize", ActiveUndo());
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Delete_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				// "Are you sure you want to delete the currently selected sprite?"
				if (!m_doc.AskYesNo(ResourceMgr.GetString("DeleteCurrentSprite")))
					return;
			}

			ss.RemoveSelectedSprite(ActiveUndo());
			HandleSpriteDataChanged(ss);
		}

		private void menuSprite_Rotate_Clockwise_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				if (ss.RotateSelectedSprite(Sprite.RotateDirection.Clockwise90))
				{
					s.RecordUndoAction("rotatecw", ActiveUndo());
					HandleSpriteDataChanged(ss);
				}
			}
		}

		private void menuSprite_Rotate_Counterclockwise_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				if (ss.RotateSelectedSprite(Sprite.RotateDirection.Counterclockwise90))
				{
					s.RecordUndoAction("rotateccw", ActiveUndo());
					HandleSpriteDataChanged(ss);
				}
			}
		}

		private void menuSprite_Rotate_180_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				if (ss.RotateSelectedSprite(Sprite.RotateDirection.Clockwise180))
				{
					s.RecordUndoAction("rotate180", ActiveUndo());
					HandleSpriteDataChanged(ss);
				}
			}
		}

		private void menuSprite_Flip_Horizontal_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(true, false);
				s.RecordUndoAction("fliph", ActiveUndo());
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Flip_Vertical_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(false, true);
				s.RecordUndoAction("flipv", ActiveUndo());
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Flip_Both_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			if (!s.IsEmpty())
			{
				s.Flip(true, true);
				s.RecordUndoAction("flipboth", ActiveUndo());
				HandleSpriteDataChanged(ss);
			}
		}

		private void menuSprite_Properties_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;

			SpriteProperties properties = new SpriteProperties(m_doc, ss);
			properties.ShowDialog();

			// The name of the sprite may have changed, so update the form title.
			ss.SpriteWindow.SetTitle();
		}

		private void menuSprite_Arrange_MoveUp_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;
		}

		private void menuSprite_Arrange_MoveDown_Click(object sender, EventArgs e)
		{
			Spriteset ss = ActiveSpriteset();
			Sprite s = ActiveSprite();
			if (s == null)
				return;
		}

		#endregion

		#region Palette Menu

		private void menuPalette_EditColors_Click(object sender, EventArgs e)
		{
			Subpalette p = ActivePalette().GetCurrentSubpalette();
			if (p == null)
				return;

			ColorEncodingView cedit = new ColorEncodingView(this, p);
			DialogResult result = cedit.ShowDialog();

			//UpdatePaletteColor(tab);
			if (result == DialogResult.Yes)
			{
				m_doc.HasUnsavedChanges = true;
				HandleColorDataChange(ActivePalette());
			}
		}

		#endregion

		#region Window menu

		private void menuWindow_Arrange_Click(object sender, EventArgs e)
		{
			m_tabCurrent.ArrangeWindows();
		}

		#endregion

		#region Help menu

		private void menuHelp_About_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
		}

		#endregion

		#region Test menu

		private void menuTest_RunUnittests_Click(object sender, EventArgs e)
		{
			UnitTestForm ut = new UnitTestForm();
			ut.ShowDialog();
		}

		private void menuTest_RunTutorialTests_Click(object sender, EventArgs e)
		{
			TutorialTestForm tt = new TutorialTestForm(this);
			tt.ShowDialog();
		}

		private void menuTest_ShowUndoHistory_Click(object sender, EventArgs e)
		{
			UndoHistoryVisible = !UndoHistoryVisible;
			menuTest_ShowUndoHistory.Checked = UndoHistoryVisible;
		}

		private void menuTest_ShowTrace_Click(object sender, EventArgs e)
		{
			Debug.TraceFormVisible = true;
			menuTest_ShowTrace.Checked = Debug.TraceFormVisible;
		}

		private void menuTest_CollisionTest_Click(object sender, EventArgs e)
		{
			CollisionTest ct = new CollisionTest(m_doc);
			ct.ShowDialog();
		}

		#endregion
	}
}