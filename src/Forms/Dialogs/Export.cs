using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Export : Form
	{
		static string m_strLastDocument = "";
		static string m_strLastExportDirectory = "";

		public Export(string strDocName)
		{
			InitializeComponent();

			this.DialogResult = DialogResult.Cancel;

			// Add Tooltips.
			ToolTip ttSprites = new ToolTip();
			ttSprites.SetToolTip(rbSprites, ResourceMgr.GetString("ExportTooltipSprites"));
			ToolTip ttUpdateProject = new ToolTip();
			ttUpdateProject.SetToolTip(rbUpdateProject, ResourceMgr.GetString("ExportTooltipUpdateProject"));
			ToolTip ttCompleteProject = new ToolTip();
			ttCompleteProject.SetToolTip(rbProject, ResourceMgr.GetString("ExportTooltipCompleteProject"));
			
			// Reset the export directory if we've opened a new file.
			if (m_strLastDocument != strDocName)
				m_strLastExportDirectory = "";
			m_strLastDocument = strDocName;

			// Reset any invalid directories.
			if (m_strLastExportDirectory != "" && !System.IO.Directory.Exists(m_strLastExportDirectory))
				m_strLastExportDirectory = "";

			// Default to save in document's directory.
			if (m_strLastExportDirectory == "" && strDocName != "")
			{
				m_strLastExportDirectory = System.IO.Path.GetDirectoryName(strDocName);
				if (!System.IO.Directory.Exists(m_strLastExportDirectory))
					m_strLastExportDirectory = "";
			}

			// Set the default directory to be the same as the application's directory.
			if (m_strLastExportDirectory == "")
				m_strLastExportDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

			tbLocation.Text = m_strLastExportDirectory;
			rbSprites.Checked = true;

			this.Text += " - " + (Options.Platform == Options.PlatformType.NDS ? "NDS" : "GBA");
		}

		private void bBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog SaveFolderDialog = new FolderBrowserDialog();
			//SaveFolderDialog.Description = "Select the directory where you want to store the exported files:";
			SaveFolderDialog.Description = ResourceMgr.GetString("SelectExportDir");
			SaveFolderDialog.SelectedPath = m_strLastExportDirectory;
			if (SaveFolderDialog.ShowDialog() != DialogResult.OK)
				return;

			tbLocation.Text = SaveFolderDialog.SelectedPath;

			// TODO: verify that the path doesn't contain spaces or punctuation
			if (tbLocation.Text.IndexOfAny(new char[] { ' ' }) != -1)
			{
				//MessageBox.Show("The directory path that you've chosen contains at least one space (' ').\r\nWhile this is a valid Windows path, the spaces will cause problems for the Gameboy/Nintendo DS development tools.\r\nPlease select a path that does not contain these characters.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
				MessageBox.Show(ResourceMgr.GetString("InvalidPath"), ResourceMgr.GetString("InvalidPathTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void bExport_Click(object sender, EventArgs e)
		{
			m_strLastExportDirectory = tbLocation.Text;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		public string ExportLocation
		{
			get { return tbLocation.Text; }
		}

		public bool Project
		{
			get { return rbProject.Checked; }
		}

		public bool UpdateProject
		{
			get { return rbUpdateProject.Checked; }
		}

	}
}
