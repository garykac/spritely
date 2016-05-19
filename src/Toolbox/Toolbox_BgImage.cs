using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Toolbox_BgImage : Toolbox
	{
		public Toolbox_BgImage() : base()
		{
			Tools.Add(new Tool("Select",		ToolType.Select,		0,0,	true,	false,
						ResourceMgr.GetBitmap("tool_select")));

			ToolboxRows = 1;
			CurrentToolType = ToolType.Blank;
		}

	}
}
