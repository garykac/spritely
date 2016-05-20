using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Palette256Form : Form, IPaletteForm
	{
		private ProjectMainForm m_parent;
		private Palette m_palette;

		static System.Drawing.Drawing2D.HatchBrush m_brushTransparent = null;

		public Palette256Form(ProjectMainForm parent, Palette256 p)
		{
			m_parent = parent;
			m_palette = p;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			if (m_palette.IsBackground)
				Text = "BgPalette '" + p.Name + "'";
			else
				Text = "Palette '" + p.Name + "'";

			if (m_brushTransparent == null)
			{
				m_brushTransparent = new System.Drawing.Drawing2D.HatchBrush(
						Options.TransparentPattern,
						Color.LightGray, Color.Transparent);
			}
		}

		#region Window events

		private void Palette256Form_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void Palette256Form_FormClosing(object sender, FormClosingEventArgs e)
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
			// Nothing to update.
		}

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		public void SpriteDataChanged()
		{
			// Nothing to update.
		}

		/// <summary>
		/// The display options for the sprite has changed.
		/// </summary>
		public void SpriteDisplayOptionChanged()
		{
			pbPalette.Invalidate();
			//pbFgSwatch.Invalidate();
			//pbBgSwatch.Invalidate();
		}

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		public void SubpaletteSelectChanged()
		{
			// Nothing to update.
		}

		/// <summary>
		/// A new color has been selected.
		/// </summary>
		public void ColorSelectChanged()
		{
			pbPalette.Invalidate();
			//pbFgSwatch.Invalidate();
			//pbBgSwatch.Invalidate();
		}

		/// <summary>
		/// The current color value has changed in the palette.
		/// </summary>
		public void ColorDataChanged()
		{
			pbPalette.Invalidate();
			//pbFgSwatch.Invalidate();
			//pbBgSwatch.Invalidate();
		}

		#endregion

		#region Palette

		/// <summary>
		/// Size of each color square in the palette (in pixels).
		/// </summary>
		private const int k_pxColorSize = 10;

		/// <summary>
		/// Number of color columns displayed in the palette. 
		/// </summary>
		private const int k_nPaletteColumns = 16;

		/// <summary>
		/// Number of color rows displayed in the palette.
		/// </summary>
		private const int k_nPaletteRows = 16;

		private bool m_fPalette_Selecting = false;
		private int m_fPalette_OriginalColor = 0;

		private void pbPalette_MouseDown(object sender, MouseEventArgs e)
		{
			//m_fPalette_OriginalColor = m_palette.GetCurrentSubpalette().CurrentColor;
			//m_fPalette_Selecting = true;

			//pbPalette_MouseMove(sender, e);
		}

		private void pbPalette_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPalette_Selecting)
			{
				if (HandleMouse_Palette(e.X, e.Y))
				{
					m_parent.HandleColorSelectChange(m_palette);
				}
			}
		}

		private void pbPalette_MouseUp(object sender, MouseEventArgs e)
		{
			m_fPalette_Selecting = false;

			// Record an undo action if the current color selection has changed
			if (m_fPalette_OriginalColor != m_palette.CurrentColor())
			{
				//Subpalette sp = m_palette.GetCurrentSubpalette();
				//sp.RecordUndoAction("select color", m_parent.ActiveUndo());
			}
		}

		public bool HandleMouse_Palette(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to palette (x,y).
			int nX = pxX / k_pxColorSize;
			int nY = pxY / k_pxColorSize;

			// Ignore if outside the SpriteList bounds.
			if (nX >= k_nPaletteColumns || nY >= k_nPaletteRows)
				return false;

			int nSelectedColor = nY * k_nPaletteColumns + nX;

			// Update the selection if a new color has been selected.
			if (m_palette.CurrentColor() != nSelectedColor)
			{
				m_palette.SetCurrentColor(nSelectedColor);
				return true;
			}

			return false;
		}

		private void pbPalette_DoubleClick(object sender, EventArgs e)
		{
			//ColorEditor256 ce = new ColorEditor256(m_parent, this, m_palette);
			//ce.ShowDialog();
			pbPalette.Invalidate();
			//pbFgSwatch.Invalidate();
			//pbBgSwatch.Invalidate();
		}

		private void pbPalette_Paint(object sender, PaintEventArgs e)
		{
			DrawPalette(e.Graphics, m_palette);
		}

		public static void DrawPalette(Graphics g, Palette p)
		{
			int nRows = k_nPaletteRows;
			int nColumns = k_nPaletteColumns;
			int pxSize = k_pxColorSize;

			for (int iRow = 0; iRow < nRows; iRow++)
			{
				for (int iColumn = 0; iColumn < nColumns; iColumn++)
				{
					int nIndex = iRow * nColumns + iColumn;

					int pxX0 = 1 + iColumn * pxSize;
					int pxY0 = 1 + iRow * pxSize;

					//g.FillRectangle(CurrentSubpalette.Brush(nIndex%16), pxX0, pxY0, pxSize, pxSize);

					// Draw the transparent color (index 0) using a pattern.
					if (nIndex == 0)
						g.FillRectangle(m_brushTransparent, pxX0, pxY0, pxSize, pxSize);

					// Draw a border around each color swatch.
					g.DrawRectangle(Pens.White, pxX0, pxY0, pxSize, pxSize);
				}
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + nColumns * pxSize, 2 + nRows * pxSize);

			// Hilight the currently selected color.
			//if (m_mgr.HilightSelectedColor)
			//{
			//	int x = (m_data.currentColor % nColumns) * pxSize;
			//	int y = (m_data.currentColor / nColumns) * pxSize;
			//	g.DrawRectangle(m_penHilight, x + 1, y + 1, pxSize, pxSize);
			//}
		}

		#endregion

	}
}