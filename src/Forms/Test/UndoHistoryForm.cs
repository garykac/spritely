using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class UndoHistoryForm : Form
	{
		ProjectMainForm m_owner;

		public UndoHistoryForm(ProjectMainForm owner)
		{
			m_owner = owner;

			InitializeComponent();
		}

		public void Clear()
		{
			lbSprites.Items.Clear();
			lbBackgroundMaps.Items.Clear();
			lbBackgroundImages.Items.Clear();
		}

		/// <summary>
		/// Get the listbox associated with the given tab id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private ListBox GetListbox(TabMgr.TabId id)
		{
			switch (id)
			{
				case TabMgr.TabId.Sprites:
					return lbSprites;
				case TabMgr.TabId.BackgroundMaps:
					return lbBackgroundMaps;
				case TabMgr.TabId.BackgroundImages:
					return lbBackgroundImages;
			}
			return null;
		}

		public void Add(TabMgr.TabId id, UndoAction action)
		{
			ListBox lb = GetListbox(id);
			lb.Items.Add(action.Description);
		}

		public void Remove(TabMgr.TabId id, int nIndex)
		{
			ListBox lb = GetListbox(id);
			lb.Items.RemoveAt(nIndex);
		}

		public void RemoveRange(TabMgr.TabId id, int nStart, int nCount)
		{
			ListBox lb = GetListbox(id);
			for (int i = 0; i < nCount; i++)
				lb.Items.RemoveAt(nStart);
		}

		public void SetCurrent(TabMgr.TabId id, int nIndex)
		{
			ListBox lb = GetListbox(id);

			if (nIndex >= lb.Items.Count)
				return;
			lb.SelectedIndex = nIndex;
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			m_owner.UndoHistoryVisible = false;
		}

		private void UndoHistory_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_owner.UndoHistoryVisible = false;
			e.Cancel = true;
		}
	}
}