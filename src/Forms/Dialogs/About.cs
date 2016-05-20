using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms; 

namespace Spritely
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
			string strFormat = lVersion.Text;
			lVersion.Text = String.Format(strFormat, Options.VersionString, Options.VersionDate);

			lDebug.Visible = Options.DEBUG;
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
