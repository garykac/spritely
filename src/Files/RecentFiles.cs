using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public class RecentFiles
	{
		private const int MaxRecentFiles = 10;

		private ProjectMainForm m_form;
		private ToolStripMenuItem m_menu;
		private StringCollection m_files;

		public RecentFiles(ProjectMainForm form, ToolStripMenuItem menu)
		{
			m_form = form;
			m_menu = menu;
			m_files = Properties.Settings.Default.RecentFiles;
			if (m_files == null)
				m_files = new StringCollection();
			BuildRecentMenu();
		}

		private void BuildRecentMenu()
		{
			m_menu.DropDownItems.Clear();

			int nIndex = 0;
			foreach (string s in m_files)
			{
				ToolStripItem tsi = m_menu.DropDownItems.Add(String.Format("&{0:d} {1:s}",nIndex, s));
				tsi.Tag = String.Format("{0:d}", nIndex);
				tsi.Click += new System.EventHandler(m_form.menuFile_RecentFiles_Click);
				nIndex++;
			}
		}

		public int Count
		{
			get { return m_files.Count; }
		}

		public string GetNthRecentFile(int n)
		{
			if (n < 0 || n >= MaxRecentFiles)
				return "";
			return m_files[n];
		}

		public void AddFile(string strFilename)
		{
			// Does the file already exist in the list?
			int nIndex = 0;
			int nFound = -1;
			foreach (string s in m_files)
			{
				if (s == strFilename)
					nFound = nIndex;
				nIndex++;
			}
			if (nFound != -1)
				m_files.RemoveAt(nFound);

			// Add file to top of list.
			m_files.Insert(0, strFilename);

			// Remove files from bottom if we have too many.
			if (m_files.Count > MaxRecentFiles)
				m_files.RemoveAt(MaxRecentFiles);

			BuildRecentMenu();

			Properties.Settings.Default.RecentFiles = m_files;
			Properties.Settings.Default.Save();
		}

		public void RemoveFile(string strFilename)
		{
			int nIndex = 0;
			foreach (string s in m_files)
			{
				if (s == strFilename)
				{
					m_files.RemoveAt(nIndex);
					BuildRecentMenu();
					return;
				}
				nIndex++;
			}
		}

	}
}
