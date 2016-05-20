using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Optionbox_Map : Optionbox
	{
		Tool m_toolScreen;

		public Optionbox_Map() : base()
		{
			m_toolScreen = new Tool("ShowScreen", ToolType.ShowScreen, 0, 0,
					ResourceMgr.GetBitmap("tool_gba"),
					Options.BoolOptionName.BackgroundMap_ShowScreen);
			Tools.Add(m_toolScreen);

			Tools.Add(new Tool("ShowTileGrid", ToolType.ShowTileGrid, 1, 0,
					ResourceMgr.GetBitmap("tool_smallgrid"),
					Options.BoolOptionName.BackgroundMap_ShowGrid));

			ToolboxRows = 1;
		}

		/// <summary>
		/// Update the screen bounds button to match the current platform.
		/// </summary>
		public void UpdateScreenButton()
		{
			if (Options.Platform == Options.PlatformType.GBA)
				m_toolScreen.ChangeToolBitmap(ResourceMgr.GetBitmap("tool_gba"));
			else
				m_toolScreen.ChangeToolBitmap(ResourceMgr.GetBitmap("tool_nds"));
		}

	}
}
