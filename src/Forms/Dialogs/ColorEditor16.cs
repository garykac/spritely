using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ColorEditor16 : Form
	{
		ProjectMainForm m_parent;
		Palette16Form m_winPalette;
		Subpalette m_subpalette;

		const int k_pxSwatchSize = 24;

		public ColorEditor16(ProjectMainForm parent, Palette16Form p16, Subpalette sp)
		{
			m_parent = parent;
			m_winPalette = p16;
			m_subpalette = sp;

			InitializeComponent();

			UpdateColorScrollbars();
		}

		private void pbPalette_Paint(object sender, PaintEventArgs e)
		{
			Palette16Form.DrawPalette(e.Graphics, m_subpalette);
		}

		private void pbCurrent_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.Black, 0, 0, 1000, 1000);

			g.FillRectangle(m_subpalette.Brush(), 1, 1, k_pxSwatchSize, k_pxSwatchSize);

			// Draw a border around each color swatch.
			g.DrawRectangle(Pens.White, 1, 1, k_pxSwatchSize, k_pxSwatchSize);
		}

		private void UpdateColorScrollbars()
		{
			// Record the RGB values because once we start updating the scrollbars,
			// sbColor_ValueChanged will be called which will write the scrollbar
			// values (all 3) into the palette.
			int r = m_subpalette.Red();
			int g = m_subpalette.Green();
			int b = m_subpalette.Blue();
			sbRed.Value = r;
			sbGreen.Value = g;
			sbBlue.Value = b;
		}

		private void sbColor_ValueChanged(object sender, EventArgs e)
		{
			m_subpalette.UpdateColor(sbRed.Value, sbGreen.Value, sbBlue.Value);
			lRed.Text = String.Format("{0:X2}", sbRed.Value);
			lGreen.Text = String.Format("{0:X2}", sbGreen.Value);
			lBlue.Text = String.Format("{0:X2}", sbBlue.Value);

			m_parent.HandleColorDataChange(m_subpalette.Palette);
			pbPalette.Invalidate();
			pbCurrent.Invalidate();
		}

		private bool m_fPalette_Selecting = false;
		private int m_fPalette_OriginalColor = 0;

		private void pbPalette_MouseDown(object sender, MouseEventArgs e)
		{
			m_fPalette_OriginalColor = m_subpalette.CurrentColor;
			m_fPalette_Selecting = true;

			pbPalette_MouseMove(sender, e);
		}

		private void pbPalette_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPalette_Selecting)
			{
				if (m_winPalette.HandleMouse_Palette(e.X, e.Y))
				{
					m_parent.HandleColorSelectChange(m_subpalette.Palette);
					pbPalette.Invalidate();
					pbCurrent.Invalidate();
					UpdateColorScrollbars();
				}
			}
		}

		private void pbPalette_MouseUp(object sender, MouseEventArgs e)
		{
			m_fPalette_Selecting = false;

			// Record an undo action if the current color selection has changed
			if (m_fPalette_OriginalColor != m_subpalette.CurrentColor)
			{
				m_subpalette.RecordUndoAction("select color", m_parent.ActiveUndo());
			}
		}
	}
}