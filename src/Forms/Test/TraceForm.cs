using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class TraceForm : Form
	{
		public TraceForm()
		{
			InitializeComponent();
		}

		private void TraceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Debug.TraceFormVisible = false;
			e.Cancel = true;
		}

		public void Clear()
		{
			lbTrace.Items.Clear();
		}

		public void AddTrace(string strMessage)
		{
			lbTrace.Items.Insert(0, strMessage);
		}
	}
}
