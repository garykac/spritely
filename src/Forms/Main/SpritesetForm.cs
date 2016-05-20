using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpritesetForm : Form
	{
		private ProjectMainForm m_parent;
		private Spriteset m_ss;

		private Font m_font = new Font("Arial", 8, FontStyle.Bold);

		private const int MaxTilesX = 8;
		private const int MaxTilesY = 14;
		private const int MarginX = 0;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);

		public SpritesetForm(ProjectMainForm parent, Spriteset ss)
		{
			m_parent = parent;
			m_ss = ss;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			if (m_ss.IsBackground)
				Text = "Tileset '" + ss.Name + "'";
			else
				Text = "Spriteset '" + ss.Name + "'";

			cbPalette.Items.Add(ss.Palette.Name);
			cbPalette.SelectedIndex = 0;
		}

		#region Window events

		private void SpritesetForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void SpritesetForm_FormClosing(object sender, FormClosingEventArgs e)
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
			pbSprites.Invalidate();
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			pbSprites.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			pbSprites.Invalidate();
		}

		/// <summary>
		/// One of the colors in the palette has changed.
		/// </summary>
		public void ColorDataChanged()
		{
			pbSprites.Invalidate();
		}
		#endregion

		#region Scrollbar

		private int m_nScrollPosition;
		private int m_nTotalScrollHeight = 0;

		public int VisibleScrollRows
		{
			get { return MaxTilesY; }
		}

		public int MaxScrollRows
		{
			get { return m_nTotalScrollHeight; }
		}

		public void RecalcScrollHeights()
		{
			m_nTotalScrollHeight = 0;
			foreach (SpriteType st in m_ss.SpriteTypes)
			{
				st.FirstLine = m_nTotalScrollHeight;
				if (st.Sprites.Count == 0)
					st.ScrollHeight = 0;
				else
				{
					// Number of rows required for this SpriteType
					int nRows = ((st.Width * st.Sprites.Count) + (MaxTilesX - 1)) / MaxTilesX;
					// 1 (for the title bar) + rows * height of each row (in tiles)
					st.ScrollHeight = 1 + (nRows * st.Height);
				}

				m_nTotalScrollHeight += st.ScrollHeight;
			}
		}

		public void ScrollTo(int nPosition)
		{
			m_nScrollPosition = nPosition;
		}

		public void AdjustScrollbar()
		{
			int nVisibleRows = VisibleScrollRows;
			int nMaxRows = MaxScrollRows;

			if (nVisibleRows >= nMaxRows)
			{
				sbSprites.Enabled = false;
				sbSprites.Value = 0;
			}
			else
			{	
				sbSprites.Enabled = true;
				sbSprites.Minimum = 0;
				sbSprites.Maximum = nMaxRows - 2;
				sbSprites.LargeChange = nVisibleRows - 1;
			}
			pbSprites.Invalidate();
		}

		private void sbSprites_ValueChanged(object sender, EventArgs e)
		{
			ScrollTo(sbSprites.Value);
			pbSprites.Invalidate();
		}

		#endregion

		private bool m_fSpriteList_Selecting = false;

		private void pbSprites_MouseDown(object sender, MouseEventArgs e)
		{
			m_fSpriteList_Selecting = true;
			pbSprites_MouseMove(sender, e);
		}

		private void pbSprites_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fSpriteList_Selecting)
			{
				if (HandleMouse(e.X, e.Y))
				{
					m_parent.HandleSpriteSelectionChanged(m_ss);
				}
			}
		}

		private void pbSprites_MouseUp(object sender, MouseEventArgs e)
		{
			m_fSpriteList_Selecting = false;
		}

		private void pbSprites_DoubleClick(object sender, EventArgs e)
		{
			//m_ss.SpriteWindow.Show();
		}

		/// <summary>
		/// Handle a mouse event in the sprite list.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if the spritelist needs to be redrawn</returns>
		public bool HandleMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to tile (x,y).
			int nTileX = pxX / Tile.SmallBitmapScreenSize;
			int nTileY = pxY / Tile.SmallBitmapScreenSize;

			// Ignore if outside the SpriteList bounds.
			if (nTileX >= MaxTilesX || nTileY >= MaxTilesY)
				return false;

			// Adjust for the current scroll position.
			nTileY += m_nScrollPosition;

			foreach (SpriteType st in m_ss.SpriteTypes)
			{
				if (st.Sprites.Count != 0
					// Ignore (st.FirstLine == nTileY) because this is a click on the label
					&& st.FirstLine < nTileY
					&& (st.FirstLine + st.ScrollHeight) > nTileY
					)
				{
					// Calculate which tile was clicked on within this SpriteType.
					nTileY -= (st.FirstLine + 1);

					int nSpriteX = nTileX / st.Width;
					int nSpriteY = nTileY / st.Height;
					int nSpritesPerRow = MaxTilesX / st.Width;
					int nSelectedSprite = (nSpriteY * nSpritesPerRow) + nSpriteX;

					// Don't allow selection beyond the end of the valid sprites.
					if (nSelectedSprite >= st.Sprites.Count)
						return false;

					// Update the selection if a new sprite has been selected.
					if (m_ss.CurrentSprite != st.Sprites[nSelectedSprite])
					{
						m_ss.CurrentSprite = st.Sprites[nSelectedSprite];
						return true;
					}
				}
			}
			return false;
		}

		private void pbSprites_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// size of each tile (in screen pixels)
			int pxTileSize = Tile.SmallBitmapScreenSize;

			int pxX = MarginX;
			int pxY = 0;
			int nRow = 0;
			int pxScrollOffset = m_nScrollPosition * pxTileSize + 1;

			// Information about currently selected sprite.
			Rectangle rSelectedSprite = new Rectangle(0, 0, 0, 0);
			bool fSelectedSprite = false;

			Brush brTitleBar;
			if (m_ss.IsBackground)
				brTitleBar = Brushes.DarkViolet;
			else
				brTitleBar = Brushes.Green;

			foreach (SpriteType st in m_ss.SpriteTypes)
			{
				if (st.Sprites.Count != 0)
				{
					// Draw label for sprite size.
					if (nRow >= m_nScrollPosition && nRow < (m_nScrollPosition + MaxTilesY))
					{
						const int pxFontAdjustX = 2;	// Font indent
						const int pxFontAdjustY = 1;	// Font baseline adjust
						g.FillRectangle(brTitleBar, pxX + 1, pxY + 2 - pxScrollOffset, (MaxTilesX * pxTileSize) - 1, pxTileSize - 3);
						g.DrawString(st.Name, m_font, Brushes.White, pxX + pxFontAdjustX, pxY + pxFontAdjustY - pxScrollOffset);
						//g.DrawString(st.ScrollHeight.ToString(), m_font, Brushes.White, (MaxTilesX * (pxTileSize - 2)) + pxFontAdjustX, nY + pxFontAdjustY - pxScrollOffset);
					}
					pxY += pxTileSize;
					nRow++;

					int pxSpriteWidth = st.Width * pxTileSize;
					int pxSpriteHeight = st.Height * pxTileSize;

					foreach (Sprite s in st.Sprites)
					{
						// Move to a new row if necessary.
						if (pxX >= MaxTilesX * pxTileSize)
						{
							pxY += pxSpriteHeight;
							nRow += st.Height;
							pxX = MarginX;
						}

						int pxX0 = pxX;
						int pxY0 = pxY - pxScrollOffset;

						// Don't draw unless some part of the sprite is visible.
						if (nRow >= m_nScrollPosition && nRow <= (m_nScrollPosition + MaxTilesY)
							|| ((nRow + st.Height) >= m_nScrollPosition && (nRow + st.Height) < (m_nScrollPosition + MaxTilesY))
							)
						{
							// Draw the sprite.
							s.DrawSmallSprite(g, pxX0, pxY0);

							// Draw a border around the sprite.
							g.DrawRectangle(Pens.Gray, pxX0, pxY0, pxSpriteWidth, pxSpriteHeight);

							if (m_ss.CurrentSprite == s)
							{
								// Record the bounds of the currently selected sprite so that we can draw
								// a rectangle border around it. We want this border to be drawn on top of
								// all of the sprites (since it may extend slightly on top of neighboring
								// sprites), so we need to draw it last. 
								fSelectedSprite = true;
								rSelectedSprite = new Rectangle(pxX0, pxY0, pxSpriteWidth, pxSpriteHeight);
							}
						}

						pxX += pxSpriteWidth;
					}

					// Position pen for the next label.
					pxX = MarginX;
					pxY += pxSpriteHeight;
					nRow += st.Height;
				}
			}

			// Draw a heavy border around the current sprite (if any).
			if (fSelectedSprite)
				g.DrawRectangle(m_penHilight, rSelectedSprite);
		}

	}
}
