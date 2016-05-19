using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class FileHandler
	{
		private Document m_doc;

		private string m_strFilename;
		private bool m_fHasUnsavedChanges;

		public FileHandler(Document doc)
		{
			m_doc = doc;

			m_strFilename = "";
			m_fHasUnsavedChanges = false;
		}

		public string Filename
		{
			get { return m_strFilename; }
		}

		public bool HasUnsavedChanges
		{
			get { return m_fHasUnsavedChanges; }
			set { m_fHasUnsavedChanges = value; }
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public bool OpenFile()
		{
			OpenFileDialog OpenFileDialog;
			OpenFileDialog = new OpenFileDialog();
			OpenFileDialog.InitialDirectory = @"";
			OpenFileDialog.Filter = "XML files (*.xml)|*.xml|All files|*.*";

			if (OpenFileDialog.ShowDialog() == DialogResult.OK)
				return OpenFile(OpenFileDialog.FileName);

			return false;
		}

		public bool OpenFile(string strFileName)
		{
			if (!LoadFile(strFileName))
				return false;

			m_strFilename = strFileName;
			m_fHasUnsavedChanges = false;
			return true;
		}

		public bool Close()
		{
			if (m_fHasUnsavedChanges)
			{
				bool fCancel;
				bool fSave = m_doc.AskYesNoCancel(ResourceMgr.GetString("SaveChanges"), out fCancel);
				if (fCancel)
					return false;
				if (fSave)
				{
					if (!SaveFile())
						// Implicitly cancel if we were unable to save.
						return false;
				}
			}

			m_strFilename = "";
			m_fHasUnsavedChanges = false;
			return true;
		}

		/// <summary>
		/// Show a file dialog box so that the user can choose an image file to load
		/// as a background.
		/// </summary>
		/// <returns></returns>
		public string ChooseBgImageFile()
		{
			OpenFileDialog OpenFileDialog;
			OpenFileDialog = new OpenFileDialog();
			OpenFileDialog.InitialDirectory = @"";
			OpenFileDialog.Filter = "Image files (bmp,gif,jpg,png)|*.bmp;*.gif;*.jpg;*.png|All files|*.*";

			if (OpenFileDialog.ShowDialog() == DialogResult.OK)
				return OpenFileDialog.FileName;

			return "";
		}

		public bool SaveFile()
		{
			if (m_strFilename == "")
				return SaveFileAs();
			else
				return SaveFile_(m_strFilename);
		}

		public bool SaveFileAs()
		{
			bool fResult = false;

			SaveFileDialog SaveFileDialog;
			SaveFileDialog = new SaveFileDialog();
			SaveFileDialog.InitialDirectory = @"";
			SaveFileDialog.Filter = "XML files (*.xml)|*.xml|All files|*.*";
			if (SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				fResult = SaveFile_(SaveFileDialog.FileName);
				if (fResult)
					m_strFilename = SaveFileDialog.FileName;
			}

			return fResult;
		}

		//TODO: save into temp file and overwrite original if successful
		// so that if there is an exception, the old file is still intact.
		private bool SaveFile_(string strPath)
		{
			TextWriter tw;

			try
			{
				tw = new StreamWriter(strPath);
			}
			catch (Exception ex)
			{
				// "An exception was thrown while opening the file for writing: {0}"
				m_doc.ErrorId("ExceptionOpenWrite", ex.Message);
				return false;
			}

			tw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

			tw.WriteLine("<spritely");
			tw.WriteLine("\txmlns=\"http://schemas.kacmarcik.com/spritely/2009\"");
			tw.WriteLine("\txmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
			tw.WriteLine("\txsi:schemaLocation=\"http://schemas.kacmarcik.com/spritely/2009 spritely.xsd\"");
			tw.WriteLine("\t");
			tw.WriteLine("\tversion=\"2\" name=\"project\"");
			tw.WriteLine("\t>");
			tw.WriteLine("");

			Options.Save(tw);

			m_doc.Palettes.Save(tw);
			m_doc.Spritesets.Save(tw);

			m_doc.BackgroundPalettes.Save(tw);
			m_doc.BackgroundSpritesets.Save(tw);

			m_doc.BackgroundMaps.Save(tw);
			m_doc.BackgroundImages.Save(tw);

			tw.WriteLine("</spritely>");

			tw.Close();

			m_fHasUnsavedChanges = false;
			return true;
		}

	}
}
