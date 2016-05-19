using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public abstract class Toolbox
	{
		/// <summary>
		/// Number of columns (for tools) in the toolbox.
		/// </summary>
		protected const int ToolboxColumns = 2;

		/// <summary>
		/// Number of rows (for tools) in the toolbox.
		/// This needs to be set based on the tools added.
		/// </summary>
		protected int ToolboxRows = 3;

		/// <summary>
		/// Width (in pixels) of the toolbox.
		/// </summary>
		protected const int k_pxToolboxWidth = 54;
		protected const int k_pxToolboxHeight = 230;

		/// <summary>
		/// Tool indent from edge of toolbox.
		/// </summary>
		protected const int k_pxToolboxIndent = 2;
		/// <summary>
		/// Size of each tool (including border).
		/// </summary>
		protected const int k_pxToolboxToolSize = 26;
		/// <summary>
		/// Indent of tool image with the tool "button".
		/// </summary>
		protected const int k_pxToolImageOffset = 2;

		/// <summary>
		/// Valid tool types.
		/// </summary>
		public enum ToolType
		{
			Blank=0,

			// Sprite editing tools
			Select,
			Pencil,
			Eyedropper,
			FloodFill,
			Eraser,
			Line,
			Rect,
			RectFilled,
			Ellipse,
			EllipseFilled,
			
			// Sprite option tools
			ShowTileGrid,
			ShowPixelGrid,
			ShowPaletteIndex,
			
			// Map tools
			RubberStamp,
			//FloodFill,	// Already defined above
			
			// Map option tools
			ShowScreen,
			//ShowTileGrid,	// Already defined above
		};

		public class Tool
		{
			public string Name;
			public ToolType Type;
			public int X;
			public int Y;
			public bool Show;
			public bool Enabled;
			public Bitmap ButtonBitmap;

			/// <summary>
			/// Used for option tools to show current state.
			/// Normal tool hilighting is handled automatically based on CurrentTool.
			/// </summary>
			public bool Hilight;

			/// <summary>
			/// Used for option tools to know which option to update when the tool
			/// button changes.
			/// </summary>
			public Options.BoolOptionName OptionName;

			public Tool(string strName, ToolType type, int x, int y, bool fShow, bool fEnable, Bitmap bm)
			{
				Name = strName;
				Type = type;
				X = x;
				Y = y;
				Show = fShow;
				Enabled = fEnable;
				ButtonBitmap = bm;
				Hilight = false;
			}

			public Tool(string strName, ToolType type, int x, int y, Bitmap bm)
			{
				Name = strName;
				Type = type;
				X = x;
				Y = y;
				Show = true;
				Enabled = true;
				ButtonBitmap = bm;
				Hilight = false;
			}

			public Tool(string strName, ToolType type, int x, int y, Bitmap bm, Options.BoolOptionName option)
			{
				Name = strName;
				Type = type;
				X = x;
				Y = y;
				Show = true;
				Enabled = true;
				ButtonBitmap = bm;
				Hilight = Options.Get(option);
				OptionName = option;
			}

			public void ChangeToolBitmap(Bitmap bmNew)
			{
				ButtonBitmap = bmNew;
			}
		};
		protected List<Tool> Tools;

		public Toolbox()
		{
			Tools = new List<Tool>();
		}

		protected Tool m_toolCurrent = null;

		/// <summary>
		/// The currently selected tool.
		/// </summary>
		public Tool CurrentTool
		{
			get { return m_toolCurrent; }
			set { m_toolCurrent = value; m_eSelectedToolType = m_toolCurrent.Type; }
		}

		protected ToolType m_eSelectedToolType = ToolType.Blank;

		/// <summary>
		/// The type of the currently selected tool.
		/// </summary>
		public ToolType CurrentToolType
		{
			get { return m_eSelectedToolType; }
			set
			{
				m_eSelectedToolType = value;
				// Make sure the current tool matches the current tool type.
				if (m_toolCurrent == null || m_toolCurrent.Type != m_eSelectedToolType)
				{
					foreach (Tool t in Tools)
					{
						if (t.Type == m_eSelectedToolType)
							m_toolCurrent = t;
					}
				}
			}
		}

		public virtual void HandleMouseDown(int pxX, int pxY, PictureBox pb)
		{
		}

		public virtual bool HandleMouseDown(int pxX, int pxY)
		{
			return HandleMouse(pxX, pxY);
		}

		public virtual void HandleMouseMove(int pxX, int pxY, PictureBox pb)
		{
		}

		/// <summary>
		/// Handle the mouse move events when the mouse button is not pressed.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if we need to redraw the screen</returns>
		public virtual bool HandleMouseMove(int pxX, int pxY)
		{
			return false;
		}

		public virtual void HandleMouseUp(int pxX, int pxY, PictureBox pb)
		{
		}

		public virtual bool HandleMouseUp()
		{
			return false;
		}

		public virtual void HandleMouseLeave(PictureBox pb)
		{
		}

		public virtual void HandleMouse(int pxX, int pxY, PictureBox pb)
		{
		}

		/// <summary>
		/// Handle the mouse movements when the button is held down.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if a new command was selected</returns>
		public virtual bool HandleMouse(int pxX, int pxY)
		{
			if (pxX < k_pxToolboxIndent || pxY < k_pxToolboxIndent)
				return false;

			// Convert pixel (x,y) to tool (x,y).
			int nX = pxX / k_pxToolboxToolSize;
			int nY = pxY / k_pxToolboxToolSize;

			// Ignore if outside the Toolbox bounds.
			if (nX >= ToolboxColumns || nY >= ToolboxRows)
				return false;

			foreach (Tool t in Tools)
			{
				if (nX != t.X || nY != t.Y)
					continue;

				if (!t.Show || !t.Enabled)
					return false;

				// Same as the currently selected tool - nothing to do.
				if (m_eSelectedToolType == t.Type)
					return false;

				CurrentTool = t;
				return true;
			}

			return false;
		}

		protected Bitmap s_bmHilite = ResourceMgr.GetBitmap("tool_hilite");
		protected Bitmap s_bmHilite2 = ResourceMgr.GetBitmap("tool_hilite2");

		public virtual void Draw(Graphics g, Size size)
		{
			g.DrawRectangle(Pens.LightGray, 0, 0, size.Width-1, size.Height-1);
			int pxX0, pxY0;

			foreach (Tool t in Tools)
			{
				if (!t.Show)
					continue;

				int iColumn = t.X;
				int iRow = t.Y;

				pxX0 = k_pxToolboxIndent + iColumn * k_pxToolboxToolSize;
				pxY0 = k_pxToolboxIndent + iRow * k_pxToolboxToolSize;

				if (t.Type == m_eSelectedToolType)
					g.DrawImage(s_bmHilite, pxX0, pxY0);

				g.DrawImage(t.ButtonBitmap, pxX0 + k_pxToolImageOffset, pxY0 + k_pxToolImageOffset);
			}
		}

	}
}
