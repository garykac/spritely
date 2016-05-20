using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpriteForm : Form
	{
		private ProjectMainForm m_parent;

		private Spriteset m_ss;
		private Sprite m_sprite;

		private Toolbox_Sprite m_toolbox;
		private Optionbox_Sprite m_optionbox;
		private Arrowbox m_arrowbox;

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

		public enum ZoomLevel
		{
			Zoom_1x = 0,
			Zoom_2x,
			Zoom_4x,
			Zoom_8x,
			Zoom_16x,
			Zoom_32x,
		};

		static System.Drawing.Drawing2D.HatchBrush m_brushTransparent = null;

		public SpriteForm(ProjectMainForm parent, Spriteset ss, Sprite s)
		{
			m_parent = parent;

			m_ss = ss;

			InitializeComponent();

			SetSprite(s);
			m_toolbox = new Toolbox_Sprite();
			m_optionbox = new Optionbox_Sprite();
			m_arrowbox = new Arrowbox_Sprite();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			// Set to 16x.
			cbZoom.SelectedIndex = (int)ZoomLevel.Zoom_16x;

			if (m_brushTransparent == null)
			{
				m_brushTransparent = new System.Drawing.Drawing2D.HatchBrush(
						Options.TransparentPattern,
						Color.LightGray, Color.Transparent);
			}
		}

		public Sprite Sprite
		{
			get { return m_sprite; }
		}

		public void SetSprite(Sprite s)
		{
			m_sprite = s;
			SetTitle();
		}

		/// <summary>
		/// Update the form title with the sprite name.
		/// </summary>
		public void SetTitle()
		{
			if (m_sprite != null)
				Text = "Sprite '" + m_sprite.Name + "'";
		}

		#region Window events
		
		private void SpriteForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void SpriteForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Subwindow updates

		/// <summary>
		/// The selected sprite has changed.
		/// </summary>
		public void SpriteSelectionChanged()
		{
			SetSprite(m_ss.CurrentSprite);
			pbSprite.Invalidate();
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			pbSprite.Invalidate();
		}

		/// <summary>
		/// The display options for the sprite has changed.
		/// </summary>
		public void SpriteDisplayOptionChanged()
		{
			pbSprite.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbSprite.Invalidate();
		}

		/// <summary>
		/// One of the colors in the palette has changed.
		/// </summary>
		public void ColorDataChanged()
		{
			pbSprite.Invalidate();
		}

		#endregion

		private void bInfo_Click(object sender, EventArgs e)
		{
			SpriteProperties properties = new SpriteProperties(m_parent.Document, m_ss);
			properties.ShowDialog();

			// The name of the sprite may have changed, so update the form title.
			SetTitle();
		}

		#region Toolbox

		private bool m_fToolbox_Selecting = false;

		private void pbTools_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_toolbox.HandleMouseDown(e.X, e.Y))
				pbTools.Invalidate();

			m_fToolbox_Selecting = true;
		}

		private void pbTools_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fToolbox_Selecting)
			{
				if (m_toolbox.HandleMouse(e.X, e.Y))
					pbTools.Invalidate();
			}
		}

		private void pbTools_MouseUp(object sender, MouseEventArgs e)
		{
			m_toolbox.HandleMouseUp();

			m_fToolbox_Selecting = false;
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
			m_parent.HandleSpriteDisplayOptionsChanged(m_ss);
			pbSprite.Invalidate();
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

		#region Arrowbox

		private bool m_fArrowbox_Selecting = false;

		private void pbArrowbox_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_arrowbox.HilightedShiftArrow() != Arrowbox.ShiftArrow.None)
			{
				m_arrowbox.SetMouseDownShiftArrow(true);
				pbArrowbox.Invalidate();

				m_sprite.ShiftPixels(m_arrowbox.HilightedShiftArrow());
				m_sprite.RecordUndoAction("shift", m_parent.ActiveUndo());
				m_parent.HandleSpriteDataChanged(m_ss);
				return;
			}

			if (m_arrowbox.HandleMouseDown(e.X, e.Y))
				pbArrowbox.Invalidate();

			m_fArrowbox_Selecting = true;
		}

		private void pbArrowbox_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fArrowbox_Selecting)
			{
				if (m_arrowbox.HandleMouse(e.X, e.Y))
					pbArrowbox.Invalidate();
			}
			else
			{
				if (m_arrowbox.HandleMouseMove(e.X, e.Y))
					pbArrowbox.Invalidate();
			}
		}

		private void pbArrowbox_MouseUp(object sender, MouseEventArgs e)
		{
			m_arrowbox.HandleMouseUp();
			m_arrowbox.SetMouseDownShiftArrow(false);

			m_fArrowbox_Selecting = false;
			pbArrowbox.Invalidate();
		}

		private void pbArrowbox_MouseLeave(object sender, EventArgs e)
		{
			m_arrowbox.SetMouseDownShiftArrow(false);

			if (m_arrowbox.HandleMouseMove(-100, -100))
				pbArrowbox.Invalidate();
		}

		private void pbArrowbox_Paint(object sender, PaintEventArgs e)
		{
			m_arrowbox.Draw(e.Graphics, pbArrowbox.Size);
		}

		#endregion

		#region Sprite

		private bool m_fEditSprite_Selecting = false;
		private bool m_fEditSprite_Erase = false;

		private void pbSprite_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_sprite == null)
				return;

			m_fEditSprite_Selecting = true;

			// Right click is handled the same as the Eraser tool
			m_fEditSprite_Erase = (e.Button == MouseButtons.Right);

			pbSprite_MouseMove(sender, e);
		}

		private void pbSprite_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentToolType;
				if (m_fEditSprite_Erase)
					tool = Toolbox.ToolType.Eraser;

				if (HandleMouseMove(e.X, e.Y, tool))
				{
					if (tool == Toolbox.ToolType.Eyedropper)
					{
						m_parent.HandleColorSelectChange(m_ss.Palette);
					}
					else
					{
						m_sprite.FlushBitmaps();
						m_parent.HandleSpriteDataChanged(m_ss);
					}
				}
			}
		}

		private void pbSprite_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_fEditSprite_Selecting)
			{
				Toolbox.ToolType tool = m_toolbox.CurrentToolType;

				// Close out the edit action.
				FinishEdit(tool);

				// Record the undo action.
				string strTool = "";
				bool fRecordUndo = false;
				switch (tool)
				{
					//case Toolbox.ToolType.Select - no  undo
					case Toolbox.ToolType.Pencil:
						strTool = "pencil";
						fRecordUndo = true;
						break;
					//case Toolbox.ToolType.Eyedropper - no undo
					case Toolbox.ToolType.FloodFill:
						strTool = "bucket";
						fRecordUndo = true;
						break;
					case Toolbox.ToolType.Eraser:
						strTool = "eraser";
						break;
				}

				if (fRecordUndo)
				{
					Sprite s = m_ss.CurrentSprite;
					if (s != null)
						s.RecordUndoAction(strTool, m_parent.ActiveUndo());
				}
			}

			m_fEditSprite_Selecting = false;
		}

		private bool HandleMouseMove(int pxX, int pxY, Toolbox.ToolType tool)
		{
			if (m_sprite == null || pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to sprite pixel index (x,y).
			int pxSpriteX = pxX / BigBitmapPixelSize;
			int pxSpriteY = pxY / BigBitmapPixelSize;

			// Ignore if pixel is outside bounds of current sprite.
			if (pxSpriteX >= m_sprite.PixelWidth || pxSpriteY >= m_sprite.PixelHeight)
				return false;

			if (tool == Toolbox.ToolType.FloodFill)
				return m_sprite.FloodFillClick(pxSpriteX, pxSpriteY);

			// Convert sprite pixel coords (x,y) into tile index (x,y) and tile pixel coords (x,y).
			int tileX = pxSpriteX / Tile.TileSize;
			int pxTileX = pxSpriteX % Tile.TileSize;
			int tileY = pxSpriteY / Tile.TileSize;
			int pxTileY = pxSpriteY % Tile.TileSize;

			int nTileIndex = (tileY * m_sprite.TileWidth) + tileX;
			Palette p = m_parent.ActivePalette();

			if (tool == Toolbox.ToolType.Eyedropper)
			{
				int nCurrColor = m_sprite.GetPixel(pxSpriteX, pxSpriteY);
				if (nCurrColor == p.CurrentColor())
					return false;
				p.SetCurrentColor(nCurrColor);
				return true;
			}

			Tile t = m_sprite.GetTile(nTileIndex);
			int nColor = (tool == Toolbox.ToolType.Eraser ? 0 : p.CurrentColor());

			// Same color - no need to update.
			if (t.GetPixel(pxTileX, pxTileY) == nColor)
				return false;

			// Set the new color.
			t.SetPixel(pxTileX, pxTileY, nColor);

			return true;
		}

		/// <summary>
		/// Finish an editing operation.
		/// </summary>
		private void FinishEdit(Toolbox.ToolType tool)
		{
			switch (tool)
			{
				case Toolbox.ToolType.Eraser:
				case Toolbox.ToolType.Eyedropper:
				case Toolbox.ToolType.FloodFill:
				case Toolbox.ToolType.Pencil:
					// These are immediate tools - nothing to finish up.
					return;
			}

			//return m_Tiles[nTileIndex].Click(pxX, pxY, tool == Toolbox.ToolType.Eraser);
		}

		private void pbSprite_Paint(object sender, PaintEventArgs e)
		{
			if (m_sprite == null)
				return;

			Graphics g = e.Graphics;

			for (int iRow = 0; iRow < m_sprite.TileHeight; iRow++)
			{
				for (int iColumn = 0; iColumn < m_sprite.TileWidth; iColumn++)
				{
					int pxX = (iColumn * BigBitmapScreenSize);
					int pxY = (iRow * BigBitmapScreenSize);
					DrawTile(g, m_sprite.GetTile((iRow * m_sprite.TileWidth) + iColumn), pxX, pxY);
				}
			}

			// Draw the grid and border.
			int pxPixelSize = BigBitmapPixelSize;
			int pxTileSize = BigBitmapScreenSize;
			int pxX0 = 0;
			int pxY0 = 0;
			int pxX1 = m_sprite.TileWidth * pxTileSize;
			int pxY1 = m_sprite.TileHeight * pxTileSize;

			Pen penPixelBorder = Pens.LightGray;
			Pen penSpriteBorder = Pens.Gray;

			// Draw border around each pixel.
			if (BigBitmapPixelSize > 2 && Options.Sprite_ShowPixelGrid)
			{
				for (int i = pxX0 + pxPixelSize; i < pxX1; i += pxPixelSize)
					g.DrawLine(penPixelBorder, i, pxY0, i, pxY1);
				for (int i = pxY0 + pxPixelSize; i < pxY1; i += pxPixelSize)
					g.DrawLine(penPixelBorder, pxX0, i, pxX1, i);
			}
			else
			{
				penSpriteBorder = Pens.LightGray;
			}

			// Draw a border around each sprite.
			if (BigBitmapPixelSize > 1 && Options.Sprite_ShowTileGrid)
			{
				for (int i = pxX0 + pxTileSize; i < pxX1; i += pxTileSize)
					g.DrawLine(penSpriteBorder, i, pxY0, i, pxY1);
				for (int i = pxY0 + pxTileSize; i < pxY1; i += pxTileSize)
					g.DrawLine(penSpriteBorder, pxX0, i, pxX1, i);
			}

			// Draw the outer border.
			g.DrawLine(Pens.Black, pxX0, pxY0, pxX1, pxY0);	// Top
			g.DrawLine(Pens.Black, pxX0, pxY0, pxX0, pxY1);	// Left
			g.DrawLine(Pens.Black, pxX0, pxY1, pxX1, pxY1);	// Bottom
			g.DrawLine(Pens.Black, pxX1, pxY0, pxX1, pxY1);	// Right
		}

		public void DrawTile(Graphics g, Tile t, int pxOriginX, int pxOriginY)
		{
			// create a new bitmap
			int pxSize = BigBitmapPixelSize;
			//int pxInset = pxSize / 4;
			Subpalette sp = m_sprite.Subpalette;

			Font f; 
			int[] nXOffset;
			int nYOffset;
			if (pxSize == 16)
			{
				f = new Font("Arial", 9);
				nXOffset = new int[16] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 3 };
				nYOffset = 1;
			}
			else
			{
				f = new Font("Arial Black", 10);
				nXOffset = new int[16] { 10, 9, 10, 10, 10, 10, 10, 10, 10, 10, 9, 9, 9, 9, 9, 10 };
				nYOffset = 6;
			}

			g.FillRectangle(Brushes.White, pxOriginX, pxOriginY, Tile.TileSize * pxSize, Tile.TileSize * pxSize);
			for (int iRow = 0; iRow < Tile.TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < Tile.TileSize; iColumn++)
				{
					int nPaletteIndex = t.GetPixel(iColumn, iRow);

					int pxX0 = pxOriginX + (iColumn * pxSize);
					int pxY0 = pxOriginY + (iRow * pxSize);
					g.FillRectangle(sp.Brush(nPaletteIndex), pxX0, pxY0, pxSize, pxSize);

					// Draw the transparent color (index 0) using a pattern.
					if (nPaletteIndex == 0 && BigBitmapPixelSize >= 8)
						g.FillRectangle(m_brushTransparent, pxX0, pxY0, pxSize, pxSize);

					if (Options.Sprite_ShowPaletteIndex && pxSize >= 16)
					{
						// Draw the palette index in each pixel.
						int pxLabelOffsetX = nXOffset[nPaletteIndex];
						int pxLabelOffsetY = nYOffset;
						g.DrawString(sp.Label(nPaletteIndex), f, sp.LabelBrush(nPaletteIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
					}
				}
			}
		}

		#endregion

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			BigBitmapPixelSize = 1 << cbZoom.SelectedIndex;
			pbSprite.Invalidate();
		}

	}
}
