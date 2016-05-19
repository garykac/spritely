using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Spritely
{
	static class Spritely
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string strFilename = "";
			if (args.Length > 0)
				strFilename = args[0];

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ProjectMainForm(strFilename));
		}
	}
}