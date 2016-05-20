using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Optionbox_Sprite : Optionbox
	{
		public Optionbox_Sprite() : base()
		{
			Tools.Add(new Tool("ShowTileScreen", ToolType.ShowTileGrid, 0, 0,
					ResourceMgr.GetBitmap("tool_largegrid"),
					Options.BoolOptionName.Sprite_ShowTileGrid));

			Tools.Add(new Tool("ShowPixelGrid", ToolType.ShowPixelGrid, 1, 0,
					ResourceMgr.GetBitmap("tool_smallgrid"),
					Options.BoolOptionName.Sprite_ShowPixelGrid));

			Tools.Add(new Tool("ShowPaletteIndex", ToolType.ShowPaletteIndex, 0, 1,
					ResourceMgr.GetBitmap("tool_number"),
					Options.BoolOptionName.Sprite_ShowPaletteIndex));

			ToolboxRows = 2;
		}

	}
}
