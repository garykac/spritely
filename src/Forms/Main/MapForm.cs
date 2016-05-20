using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class MapForm : Form
	{
		private ProjectMainForm m_parent;

		private Map m_map;
		private Spriteset m_ss;

		private Toolbox_Map m_toolbox;
		private Optionbox_Map m_optionbox;

		private const int k_nMaxMapTilesX = 32;
		private const int k_nMaxMapTilesY = 32;
		private const int k_nGBAScreenTilesX = 30;
		private const int k_nGBAScreenTilesY = 20;
		private const int k_nNDSScreenTilesX = 32;
		private const int k_nNDSScreenTilesY = 24;

		// Tiles to highlight under the cursor in the Background Map.
		private int m_tileSpriteX;
		private int m_tileSpriteY;

		public static int m_pxBigBitmapSize = 4;
		public static int BigBitmapPixelSize
		{
			get { return m_pxBigBitmapSize; }
			set { m_pxBigBitmapSize = value; }
		}

		public static int BigBitmapScreenSize
		{
			get { return BigBitmapPixelSize * Tile.TileSize; }
		}

		/// <summary>
		/// The width of a sprite window that is best suited for allowing
		/// the editing of a single tile. This is used on the Background Map
		/// editing tab to size the sprite window and to calculate the x-offset
		/// for the map window.
		/// </summary>
		public const int SingleTileWindowWidth = 206;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);
		private static Pen m_penHilight2 = new Pen(Color.FromArgb(128, Color.Black), 1);

		public enum ZoomLevel
		{
			Zoom_1x = 0,
			Zoom_2x,
			Zoom_4x,
			Zoom_8x,
			Zoom_16x,
			Zoom_32x,
		};

		public enum ScreenBoundary
		{
			None = 0,
			GBA,
			NDS,
		};

		public MapForm(ProjectMainForm parent, Map m, Spriteset ss, Sprite s)
		{
			m_parent = parent;

			m_ss = ss;

			InitializeComponent();

			SetMap(m);
			m_toolbox = new Toolbox_Map();
			m_optionbox = new Optionbox_Map();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			// Set to 16x.
			cbZoom.SelectedIndex = (int)ZoomLevel.Zoom_16x;
			cbZoom.Enabled = false;

			m_tileSpriteX = -1;
			m_tileSpriteY = -1;
		}

		public void SetMap(Map m)
		{
			m_map = m;
			pbMap.Invalidate();

			// Update title with map name.
			if (m != null)
				Text = "Map '" + m.Name + "'";
		}

		#region Window events
		
		private void MapForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Subwindow updates

		public void PlatformChanged()
		{
			m_optionbox.UpdateScreenButton();
			pbOptions.Invalidate();
			pbMap.Invalidate();
		}

		/// <summary>
		/// The selected sprite has changed.
		/// </summary>
		public void SpriteSelectionChanged()
		{
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			pbMap.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbMap.Invalidate();
		}

		/// <summary>
		/// One of the colors in the palette has changed.
		/// </summary>
		public void ColorDataChanged()
		{
			pbMap.Invalidate();
		}

		/// <summary>
		/// The data for the map has changed.
		/// </summary>
		public void MapDataChanged()
		{
			pbMap.Invalidate();
		}

		#endregion

		#region Toolbox

		private bool m_fToolbox_Selecting = false;

		private void pbTools_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_toolbox.HandleMouseDown(e.X, e.Y))
			{
				pbTools.Invalidate();
			}
			m_fToolbox_Selecting = true;
		}

		private void pbTools_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fToolbox_Selecting)
			{
				if (m_toolbox.HandleMouse(e.X, e.Y))
					pbTools.Invalidate();
			}
			else
			{
				if (m_toolbox.HandleMouseMove(e.X, e.Y))
					pbTools.Invalidate();
			}
		}

		private void pbTools_MouseUp(object sender, MouseEventArgs e)
		{
			m_toolbox.HandleMouseUp();
			m_fToolbox_Selecting = false;
			pbTools.Invalidate();
			// Some tools may affect the map display.
			pbMap.Invalidate();
		}

		private void pbTools_MouseLeave(object sender, EventArgs e)
		{
			if (m_toolbox.HandleMouseMove(-10, -10))
				pbTools.Invalidate();
		}

		private void pbTools_Paint(object sender, PaintEventArgs e)
		{
			m_toolbox.Draw(e.Graphics, pbTools.Size);
		}

		#endregion

		#region Optionbox

		private void pbOptions_MouseDown(object sender, MouseEventArgs e)
		{
			m_optionbox.HandleMouseDown(e.X, e.Y, pbOptions);
		}

		private void pbOptions_MouseMove(object sender, MouseEventArgs e)
		{
			m_optionbox.HandleMouseMove(e.X, e.Y, pbOptions);
		}

		private void pbOptions_MouseUp(object sender, MouseEventArgs e)
		{
			m_optionbox.HandleMouseUp(e.X, e.Y, pbOptions);
			pbMap.Invalidate();
		}

		private void pbOptions_MouseLeave(object sender, EventArgs e)
		{
			m_optionbox.HandleMouseLeave(pbOptions);
		}

		private void pbOptions_Paint(object sender, PaintEventArgs e)
		{
			m_optionbox.Draw(e.Graphics, pbOptions.Size);
		}

		#endregion

		#region Map

		private bool m_fEditBackgroundMap_Selecting = false;

		private void pbMap_MouseDown(object sender, MouseEventArgs e)
		{
			m_fEditBackgroundMap_Selecting = true;
			pbMap_MouseMove(sender, e);
		}

		private void pbMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fEditBackgroundMap_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentToolType;
				if (HandleMouse(e.X, e.Y, tool))
				{
					m_parent.HandleMapDataChange(m_map);
				}
			}

			// Update boundary rect for currently selected sprite.
			if (HandleMouseMove(e.X, e.Y))
			{
				pbMap.Invalidate();
			}
		}

		private void pbMap_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_fEditBackgroundMap_Selecting)
			{
				Toolbox.Tool t = m_toolbox.CurrentTool;
				m_map.RecordUndoAction(t.Name, m_parent.ActiveUndo());
			}

			if (HandleMouseMove(-100, -100))
			{
				pbMap.Invalidate();
			}
			m_fEditBackgroundMap_Selecting = false;
		}

		private void pbMap_MouseLeave(object sender, EventArgs e)
		{
			if (HandleMouseMove(-100, -100))
			{
				pbMap.Invalidate();
			}
		}

		/// <summary>
		/// Handle a mouse move while the mouse button is held down.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <param name="tool"></param>
		/// <returns>True if we need to update the map display.</returns>
		public bool HandleMouse(int pxX, int pxY, Toolbox.ToolType tool)
		{
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (spriteSelected == null)
				return false;

			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (x >= k_nMaxMapTilesX || y >= k_nMaxMapTilesY)
				return false;

			if (tool == Toolbox.ToolType.FloodFill)
				return FloodFillClick(x, y);

			if (tool == Toolbox.ToolType.RubberStamp)
			{
				bool fUpdate = false;
				int nIndex = 0;
				for (int iy = 0; iy < spriteSelected.TileHeight; iy++)
				{
					if (y + iy >= k_nMaxMapTilesY)
					{
						nIndex++;
						continue;
					}
					for (int ix = 0; ix < spriteSelected.TileWidth; ix++)
					{
						if (x + ix >= k_nMaxMapTilesX)
						{
							nIndex++;
							continue;
						}
						m_map.SetTile(x + ix, y + iy,
								spriteSelected.FirstTileId + nIndex,
								spriteSelected.SubpaletteID);
						nIndex++;
						fUpdate = true;
					}
				}
				return fUpdate;
			}

			return false;
		}

		/// <summary>
		/// Highlight a mouse move over the map when the mouse button is not being held.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if we need to update the map display</returns>
		public bool HandleMouseMove(int pxX, int pxY)
		{
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (spriteSelected == null)
				return false;

			// Convert screen pixel (x,y) to map coordinate (x,y).
			int x = pxX / Tile.SmallBitmapScreenSize;
			int y = pxY / Tile.SmallBitmapScreenSize;

			if (pxX < 0 || pxY < 0 || x >= k_nMaxMapTilesX || y >= k_nMaxMapTilesY)
			{
				// Turn off any hilighting.
				if (m_tileSpriteX != -1 || m_tileSpriteY != -1)
				{
					m_tileSpriteX = -1;
					m_tileSpriteY = -1;
					return true;
				}
				return false;
			}

			// Has the current mouse position changed?
			if (x != m_tileSpriteX || y != m_tileSpriteY)
			{
				m_tileSpriteX = x;
				m_tileSpriteY = y;
				return true;
			}

			// No change.
			return false;
		}

		private void pbMap_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int pxX = 0;
			int pxY = 0;
			for (int ix = 0; ix < k_nMaxMapTilesX; ix++)
			{
				pxY = 0;
				for (int iy = 0; iy < k_nMaxMapTilesY; iy++)
				{
					bool fDrawn = false;
					int nTileId, nSubpalette;
					m_map.GetTile(ix, iy, out nTileId, out nSubpalette);
					Sprite s = m_map.Spriteset.FindSprite(nTileId);
					if (s != null)
					{
						Tile t = s.GetTile(nTileId - s.FirstTileId);
						if (t != null)
						{
							t.DrawSmallTile(g, pxX, pxY);
							fDrawn = true;
						}
					}

					if (!fDrawn)
					{
						int pxInset = Tile.SmallBitmapScreenSize / 4;
						int pxX0i = pxX + pxInset;
						int pxY0i = pxY + pxInset;
						int pxX1i = pxX + Tile.SmallBitmapScreenSize - pxInset;
						int pxY1i = pxY + Tile.SmallBitmapScreenSize - pxInset;
						g.DrawLine(Pens.Firebrick, pxX0i, pxY0i, pxX1i, pxY1i);
						g.DrawLine(Pens.Firebrick, pxX0i, pxY1i, pxX1i, pxY0i);
					}
					pxY += Tile.SmallBitmapScreenSize;
				}
				pxX += Tile.SmallBitmapScreenSize;
			}

			// Draw the grid and border.
			int pxTileSize = Tile.SmallBitmapScreenSize;
			pxX = 0;
			pxY = 0;
			int pxWidth = pxTileSize * k_nMaxMapTilesX;
			int pxHeight = pxTileSize * k_nMaxMapTilesY;

			if (Options.BackgroundMap_ShowGrid)
			{
				Pen penTileBorder = Pens.LightGray;

				// Draw a border around each tile.
				for (int i = pxX + pxTileSize; i < pxWidth; i += pxTileSize)
					g.DrawLine(penTileBorder, i, pxY, i, pxHeight);
				for (int i = pxY + pxTileSize; i < pxHeight; i += pxTileSize)
					g.DrawLine(penTileBorder, pxX, i, pxWidth, i);
			}

			// Draw the outer border.
			g.DrawRectangle(Pens.Black, pxX, pxY, pxWidth, pxHeight);

			if (Options.BackgroundMap_ShowScreen)
			{
				if (Options.Platform == Options.PlatformType.GBA)
				{
					pxWidth = pxTileSize * k_nGBAScreenTilesX;
					pxHeight = pxTileSize * k_nGBAScreenTilesY;
				}
				else
				{
					pxWidth = pxTileSize * k_nNDSScreenTilesX;
					pxHeight = pxTileSize * k_nNDSScreenTilesY;
				}
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}

			// Draw a border around the current background "sprite".
			Sprite spriteSelected = m_ss.CurrentSprite;
			if (m_tileSpriteX != -1 && m_tileSpriteY != -1 && spriteSelected != null)
			{
				pxX = m_tileSpriteX * pxTileSize;
				pxY = m_tileSpriteY * pxTileSize;
				pxWidth = spriteSelected.TileWidth * pxTileSize;
				pxHeight = spriteSelected.TileHeight * pxTileSize;
				g.DrawRectangle(m_penHilight, pxX, pxY, pxWidth, pxHeight);
				g.DrawRectangle(m_penHilight2, pxX, pxY, pxWidth, pxHeight);
			}
		}

		#endregion

		#region FloodFill tool

		public struct TileCoord
		{
			public int X, Y;

			public TileCoord(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		/// <summary>
		/// Perform a floodfill click at the specified (x,y) tile coords
		/// </summary>
		/// <param name="nMapX">Map click x-position (in tiles)</param>
		/// <param name="nMapY">Map click y-position (in tiles)</param>
		/// <returns>True if the map changes as a result of this click, false otherwise.</returns>
		public bool FloodFillClick(int nMapX, int nMapY)
		{
			int tileOld, subpaletteOld;
			m_map.GetTile(nMapX, nMapY, out tileOld, out subpaletteOld);
			Sprite spriteOld = m_ss.FindSprite(tileOld);

			Sprite spriteNew = m_ss.CurrentSprite;
			int tileNew = spriteNew.FirstTileId;
			int subpaletteNew = spriteNew.SubpaletteID;

			// Sprite & tile & subpalette match - nothing to do.
			if (tileOld == tileNew && subpaletteOld == subpaletteNew)
				return false;

			// If the old/new sprite matches, then we're filling over the same sprite,
			// but with the tiles offset. We need to first floodfill with another tile
			// so that we can detect the sprite boundaries correctly.
			if (spriteNew == spriteOld)
			{
				FloodFill_Sprite(nMapX, nMapY, spriteOld, subpaletteOld, null, -1, subpaletteOld);
				FloodFill_Sprite(nMapX, nMapY, null, subpaletteOld, spriteNew, tileNew, subpaletteOld);
			}
			else
			{
				FloodFill_Sprite(nMapX, nMapY, spriteOld, subpaletteOld, spriteNew, tileNew, subpaletteNew);
			}

			return true;
		}

		private Stack<TileCoord> m_stackTiles;
		private Dictionary<TileCoord, bool> m_tilesDone;

		private bool FloodFill_Sprite(int nMapX, int nMapY,
				Sprite spriteOld, int subpaletteOld,
				Sprite spriteNew, int tileNew, int subpaletteNew)
		{
			int spriteWidth = spriteNew == null ? 1 : spriteNew.TileWidth;
			int spriteHeight = spriteNew == null ? 1 : spriteNew.TileHeight;

			// Stack of tiles to process.
			m_stackTiles = new Stack<TileCoord>();
			m_tilesDone = new Dictionary<TileCoord, bool>();

			TileCoord t = new TileCoord(nMapX, nMapY);
			m_stackTiles.Push(t);
			m_tilesDone.Add(t, true);

			// Adjust the mapclick origin off the map (negative) so that all of our
			// offset calculations are guaranteed to be positive.
			while (nMapX > 0)
				nMapX -= spriteWidth;
			while (nMapY > 0)
				nMapY -= spriteHeight;

			while (m_stackTiles.Count != 0)
			{
				t = m_stackTiles.Pop();

				// Calc tile within sprite that we're painting:
				//
				//  Current sprite:   abcd
				//                    efgh
				//
				//  Original:    Click:       After:
				//  ___...___    ___...___    ___efg___
				//  __.....__    __.x...__    __dabcd__
				//  __...____    __...____    __hef____
				//  ____.____    ____.____    ____b____
				//
				//  '.' marks the map coords that belong to the same sprite
				//    (regardless of the particular tile within the sprite).
				int dx = (t.X - nMapX) % spriteWidth;
				int dy = (t.Y - nMapY) % spriteHeight;
				int tile = tileNew + dx + dy * spriteWidth;

				m_map.SetTile(t.X, t.Y, tile, subpaletteNew);

				FloodFill_CheckTile(t.X - 1, t.Y, spriteOld, subpaletteOld);
				FloodFill_CheckTile(t.X + 1, t.Y, spriteOld, subpaletteOld);
				FloodFill_CheckTile(t.X, t.Y - 1, spriteOld, subpaletteOld);
				FloodFill_CheckTile(t.X, t.Y + 1, spriteOld, subpaletteOld);
			}

			m_stackTiles = null;
			m_tilesDone = null;
			return true;
		}

		private void FloodFill_CheckTile(int x, int y, Sprite spriteOld, int subpaletteOld)
		{
			if (x >= 0 && x < m_map.Width
				&& y >= 0 && y < m_map.Height)
			{
				int tile, subpalette;
				m_map.GetTile(x, y, out tile, out subpalette);
				Sprite sprite = m_ss.FindSprite(tile);

				if (sprite == spriteOld && subpalette == subpaletteOld)
				{
					TileCoord t = new TileCoord(x, y);
					if (!m_tilesDone.ContainsKey(t))
					{
						m_stackTiles.Push(t);
						m_tilesDone.Add(t, true);
					}
				}
			}
		}

		#endregion

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			//BigBitmapPixelSize = 1 << cbZoom.SelectedIndex;
			//pbMap.Invalidate();
		}

	}
}
