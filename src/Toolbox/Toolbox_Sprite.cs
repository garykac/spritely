using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Toolbox_Sprite : Toolbox
	{
		public Toolbox_Sprite() : base()
		{
			Tools.Add(new Tool("Select",		ToolType.Select,		0,0,	true,	false,
						ResourceMgr.GetBitmap("tool_select")));
			Tools.Add(new Tool("Pencil", ToolType.Pencil, 1, 0, true, true,
						ResourceMgr.GetBitmap("tool_pencil")));
			Tools.Add(new Tool("Eyedropper", ToolType.Eyedropper, 0, 1, true, true,
						ResourceMgr.GetBitmap("tool_eyedropper")));
			Tools.Add(new Tool("FloodFill",		ToolType.FloodFill,		1,1,	true,	true,
						ResourceMgr.GetBitmap("tool_floodfill")));
			Tools.Add(new Tool("Eraser",		ToolType.Eraser,		0,2,	true,	true,
						ResourceMgr.GetBitmap("tool_eraser")));
			Tools.Add(new Tool("Line",			ToolType.Line,			1,2,	false,	false,
						ResourceMgr.GetBitmap("tool_line")));
			Tools.Add(new Tool("Rect",			ToolType.Rect,			0,3,	false,	false,
						ResourceMgr.GetBitmap("tool_rect")));
			Tools.Add(new Tool("RectFilled",	ToolType.RectFilled,	1,3,	false,	false,
						ResourceMgr.GetBitmap("tool_rectfilled")));
			Tools.Add(new Tool("Ellipse",		ToolType.Ellipse,		0,4,	false,	false,
						ResourceMgr.GetBitmap("tool_ellipse")));
			Tools.Add(new Tool("EllipseFilled", ToolType.EllipseFilled, 1,4,	false,	false,
						ResourceMgr.GetBitmap("tool_ellipsefilled")));

			ToolboxRows = 3;	// 3 visible rows.
			CurrentToolType = ToolType.Pencil;
		}

	}
}
