using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Toolbox_Map : Toolbox
	{
		public Toolbox_Map() : base()
		{
			Tools.Add(new Tool("RubberStamp",	ToolType.RubberStamp,	0, 0,
					ResourceMgr.GetBitmap("tool_rubberstamp")));
			Tools.Add(new Tool("FloodFill",		ToolType.FloodFill,		1, 0,
					ResourceMgr.GetBitmap("tool_floodfill")));

			ToolboxRows = 1;
			CurrentToolType = ToolType.RubberStamp;
		}

	}
}
