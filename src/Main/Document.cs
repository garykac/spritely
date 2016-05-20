using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Document
	{
		private ProjectMainForm m_form;

		private struct DocumentData
		{
			public Palettes Palettes;
			public Spritesets Spritesets;
			public Palettes BackgroundPalettes;
			public Spritesets BackgroundSpritesets;
			public Maps BackgroundMaps;
			public BgImages BackgroundImages;
			public FileHandler Filer;
		}
		DocumentData m_data;

		/// <summary>
		/// Initialize a Spritely document.
		/// </summary>
		/// <param name="form">The display form that owns this document</param>
		public Document(ProjectMainForm form)
		{
			m_form = form;

			m_data.Palettes = new Palettes(this, Palettes.Type.Sprite);
			m_data.Spritesets = new Spritesets(this, false);
			m_data.BackgroundPalettes = new Palettes(this, Palettes.Type.Background);
			m_data.BackgroundSpritesets = new Spritesets(this, true);
			m_data.BackgroundMaps = new Maps(this);
			m_data.BackgroundImages = new BgImages(this);

			m_data.Filer = new FileHandler(this);
		}

		/// <summary>
		/// Add a default sprite to the new document when Spritely is first launched.
		/// This is so that first-time users don't see an empty window - they can 
		/// start editing their first sprite immediately.
		/// </summary>
		public void InitializeEmptyDocument()
		{
			// Palettes
			Palette16 pal = m_data.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			pal.SetDefaultPalette();

			// Spritesets
			Spriteset ss = m_data.Spritesets.AddSpriteset(Options.DefaultSpritesetName, Options.DefaultPaletteId, "", pal);

			// Add a single 2x2 sprite.
			ss.AddSprite(2, 2, "", -1, "", 0, null);
			ss.SelectFirstSprite();

			// Background palettes
			Palette16 bgpal = m_data.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			bgpal.SetDefaultPalette();

			// Background tiles (bgsprites and bgtilegroups)
			Spriteset bss = m_data.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, Options.DefaultBgPaletteId, "", bgpal);

			// Add a single blank background tile.
			bss.AddSprite(1, 1, "", -1, "", 0, null);
			bss.SelectFirstSprite();

			// Background tile map
			m_data.BackgroundMaps.AddMap("map", 0, "", m_data.BackgroundSpritesets.Current);

			// The sprites we just added above don't count as document changes.
			HasUnsavedChanges = false;
		}

		public void RecordSnapshot()
		{
			m_data.Palettes.RecordSnapshot();
			m_data.Spritesets.RecordSnapshot();
			m_data.BackgroundPalettes.RecordSnapshot();
			m_data.BackgroundSpritesets.RecordSnapshot();
			m_data.BackgroundMaps.RecordSnapshot();
			m_data.BackgroundImages.RecordSnapshot();
		}

		/// <summary>
		/// This bolean is used to verify that the Error/Warning/Ask dialogs are displayed
		/// through the Document class instead of having the ProjectMainform class methods
		/// called directly.
		/// </summary>
		public bool ShowMessageCheck = false;

		/// <summary>
		/// Display an error message using a hard-coded string.
		/// All calls to this routine should be converted to use ErrorId()
		/// so that they can be localized.
		/// </summary>
		/// <param name="strFormat">Hard-coded error message string</param>
		/// <param name="args">Arguments to be folded into the error string</param>
		public void ErrorString(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("ERROR") + strMessage);
			else
			{
				ShowMessageCheck = true;
				m_form.Error(strMessage);
				ShowMessageCheck = false;
			}
		}

		/// <summary>
		/// Display an error string using the given message-id.
		/// </summary>
		/// <param name="strMessageId">The name (message-id) of the string so display</param>
		/// <param name="args">Arguments to be folded into the error string</param>
		public void ErrorId(string strMessageId, params object[] args)
		{
			string strFormat = ResourceMgr.GetString(strMessageId);
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("ERROR") + strMessage);
			else
			{
				ShowMessageCheck = true;
				m_form.Error(strMessage);
				ShowMessageCheck = false;
			}
		}

		/// <summary>
		/// Display a warning message using a hard-coded string.
		/// All calls to this routine should be converted to use WarningId()
		/// so that they can be localized.
		/// </summary>
		/// <param name="strFormat">Hard-coded warning message string</param>
		/// <param name="args">Arguments to be folded into the warning string</param>
		public void WarningString(string strFormat, params object[] args)
		{
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("WARNING") + strMessage);
			else
			{
				ShowMessageCheck = true;
				m_form.Warning(strMessage);
				ShowMessageCheck = false;
			}
		}

		/// <summary>
		/// Display a warning string using the given message-id.
		/// </summary>
		/// <param name="strMessageId">The name (message-id) of the string so display</param>
		/// <param name="args">Arguments to be folded into the warning string</param>
		public void WarningId(string strMessageId, params object[] args)
		{
			string strFormat = ResourceMgr.GetString(strMessageId);
			string strMessage = String.Format(strFormat, args);
			if (m_form == null)
				System.Console.WriteLine(ResourceMgr.GetString("WARNING") + strMessage);
			else
			{
				ShowMessageCheck = true;
				m_form.Warning(strMessage);
				ShowMessageCheck = false;
			}
		}

		/// <summary>
		/// Pose a Yes/No question to the user.
		/// </summary>
		/// <param name="strMessage">Question to ask</param>
		/// <returns>True = yes; False = no</returns>
		public bool AskYesNo(string strMessage)
		{
			if (m_form == null)
			{
				System.Console.WriteLine("AskYesNo: " + strMessage);
				return false;
			}
			else
			{
				ShowMessageCheck = true;
				bool result = m_form.AskYesNo(strMessage);
				ShowMessageCheck = false;
				return result;
			}
		}

		/// <summary>
		/// Pose a Yes/No question to the user, allowing them to cancel.
		/// </summary>
		/// <param name="strMessage">Question to ask</param>
		/// <param name="fCancel">True is the user cancelled</param>
		/// <returns>True = yes; False = no</returns>
		public bool AskYesNoCancel(string strMessage, out bool fCancel)
		{
			if (m_form == null)
			{
				System.Console.WriteLine("AskYesNoCancel: " + strMessage);
				fCancel = false;
				return false;
			}
			else
			{
				ShowMessageCheck = true;
				bool result = m_form.AskYesNoCancel(strMessage, out fCancel);
				ShowMessageCheck = false;
				return result;
			}
		}

		/// <summary>
		/// The form that owns this document.
		/// </summary>
		public ProjectMainForm Owner
		{
			get { return m_form; }
		}

		/// <summary>
		/// The name of the file used to store this document.
		/// </summary>
		public string Name
		{
			get { return m_data.Filer.Filename; }
		}

		/// <summary>
		/// The filer used to manage loads/saves for this document.
		/// </summary>
		public FileHandler Filer
		{
			get { return m_data.Filer; }
		}

		/// <summary>
		/// Foreground spritesets.
		/// </summary>
		public Spritesets Spritesets
		{
			get { return m_data.Spritesets; }
		}

		/// <summary>
		/// Background spritesets (tilesets).
		/// </summary>
		public Spritesets BackgroundSpritesets
		{
			get { return m_data.BackgroundSpritesets; }
		}

		/// <summary>
		/// Foreground (sprite) palettes.
		/// </summary>
		public Palettes Palettes
		{
			get { return m_data.Palettes; }
		}

		/// <summary>
		/// Background palettes.
		/// </summary>
		public Palettes BackgroundPalettes
		{
			get { return m_data.BackgroundPalettes; }
		}

		public Palette GetSpritePalette(int id)
		{
			return m_data.Palettes.GetPalette(id);
		}

		public Palette GetBackgroundPalette(int id)
		{
			return m_data.BackgroundPalettes.GetPalette(id);
		}

		public Maps BackgroundMaps
		{
			get { return m_data.BackgroundMaps; }
		}

		public BgImages BackgroundImages
		{
			get { return m_data.BackgroundImages; }
		}

		/// <summary>
		/// Are there unsaved changes for this documnt?
		/// </summary>
		public bool HasUnsavedChanges
		{
			get { return m_data.Filer.HasUnsavedChanges; }
			set { m_data.Filer.HasUnsavedChanges = value; }
		}

		/// <summary>
		/// Flush all the bitmaps for all of the spritesets so that they can be
		/// regenerated.
		/// </summary>
		public void FlushBitmaps()
		{
			m_data.Spritesets.FlushBitmaps();
			m_data.BackgroundSpritesets.FlushBitmaps();
		}

		#region File

		/// <summary>
		/// Open a new file using the File open dialog.
		/// </summary>
		/// <returns>True if a new file was successfully opened</returns>
		public bool Open()
		{
			// Open into a separate doc so that we can keep the current doc untouched in case
			// there is an error.
			Document doc = new Document(m_form);
			if (!doc.m_data.Filer.OpenFile())
				return false;

			Open_(doc);
			return true;
		}

		/// <summary>
		/// Open the named file.
		/// </summary>
		/// <param name="strFilename">Name of file to open</param>
		/// <returns>True if the file was successfully opened</returns>
		public bool Open(string strFilename)
		{
			// Open into a separate doc so that we can keep the current doc untouched in case
			// there is an error.
			Document doc = new Document(m_form);
			if (!doc.m_data.Filer.OpenFile(strFilename))
				return false;

			Open_(doc);
			return true;
		}

		/// <summary>
		/// Copy the newly opened Document into the current document.
		/// </summary>
		/// <param name="doc">The newly opened document</param>
		private void Open_(Document doc)
		{
			// Copy data from newly loaded doc into this doc
			m_data = doc.m_data;
			// Update the document references to point to this document
			m_data.Palettes.UpdateDocument(this);
			m_data.Spritesets.UpdateDocument(this);
			m_data.BackgroundPalettes.UpdateDocument(this);
			m_data.BackgroundSpritesets.UpdateDocument(this);
			m_data.BackgroundMaps.UpdateDocument(this);
			m_data.Filer.UpdateDocument(this);

			Spriteset ss = m_data.Spritesets.Current;
			if (ss != null)
				ss.SelectFirstSprite();
			Spriteset bss = m_data.BackgroundSpritesets.Current;
			if (bss != null)
				bss.SelectFirstSprite();
			BgImages bgis = m_data.BackgroundImages;
			if (bgis != null)
				bgis.SelectFirstImage();
			Owner.ClearUndo();
		}

		public bool Close()
		{
			if (!m_data.Filer.Close())
				return false;

			m_data.Palettes.Clear();
			m_data.Spritesets.Clear();
			m_data.BackgroundPalettes.Clear();
			m_data.BackgroundSpritesets.Clear();
			m_data.BackgroundMaps.Clear();

			return true;
		}

		public void Save()
		{
			m_data.Filer.SaveFile();
		}

		public void SaveAs()
		{
			m_data.Filer.SaveFileAs();
		}

		#endregion

		#region Export

		/// <summary>
		/// Export this document/project as source code.
		/// </summary>
		public void Export()
		{
			string strProjectDir;
			bool fProject;
			FileHandler.ExportResult result = m_data.Filer.ExportDialog(Name, out strProjectDir, out fProject);

			if (result == FileHandler.ExportResult.Cancel)
				return;

			if (result == FileHandler.ExportResult.OK)
			{
				string strTarget = (Options.Platform == Options.PlatformType.NDS ? "NDS" : "GBA");
				StringBuilder sb = new StringBuilder("");
				if (fProject)
				{
					// "Successfully exported {0} project to "{1}"."
					sb.Append(String.Format(ResourceMgr.GetString("ExportProjectSuccess"), strTarget, strProjectDir));
					sb.Append("\r\n");
					// "Go to this directory and run "make" to build your project."
					sb.Append(ResourceMgr.GetString("InstrHowToMake"));
				}
				else
				{
					// "Successfully exported {0} sprite data to "{1}"."
					sb.Append(String.Format(ResourceMgr.GetString("ExportSpritesSuccess"), strTarget, strProjectDir));
				}
				m_form.Info(sb.ToString());
			}
			else
			{
				// "Unable to export!"
				ErrorId("ExportFail");
			}
		}

		#endregion

	}
}
