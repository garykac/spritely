using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ColorEncodingView : Form
	{
		ProjectMainForm m_parent;
		Subpalette m_subpalette;
		bool m_fHasChanges = false;

		/// <summary>
		/// Pen used to hilight the current color in the palette.
		/// </summary>
		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);
		private static System.Drawing.Drawing2D.HatchBrush m_brushTransparent = null;

		const int k_pxSwatchSize = 24;
		const int k_nSwatches = 16;

		const int k_pxRGBBitSize = 20;
		const int k_nRGBBits = 16;

		const int k_nRGBNybbles = 4;
		const int k_nBitsPerNybble = 4;

		int[] m_nRGBWordBits = new int[k_nRGBBits];

		public ColorEncodingView(ProjectMainForm parent, Subpalette p)
		{
			m_parent = parent;

			InitializeComponent();
			this.DialogResult = DialogResult.No;

			m_subpalette = p;

			AdjustColorScrollbars();

			if (m_brushTransparent == null)
			{
				m_brushTransparent = new System.Drawing.Drawing2D.HatchBrush(
						Options.TransparentPattern,
						Color.LightGray, Color.Transparent);
			}
		}

		private bool m_fUpdatePalette = true;

		private void sbColor_ValueChanged(object sender, EventArgs e)
		{
			if (m_fUpdatePalette)
			{
				m_subpalette.UpdateColor(sbRed.Value, sbGreen.Value, sbBlue.Value);
				AdjustColorScrollbars();
				m_fHasChanges = true;
			}
		}

		private bool m_fPalette_Selecting = false;

		private void pbPalette_MouseDown(object sender, MouseEventArgs e)
		{
			m_fPalette_Selecting = true;
			if (HandlePaletteMouse(e.X, e.Y))
			{
				AdjustColorScrollbars();
			}
		}

		private void pbPalette_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fPalette_Selecting)
			{
				if (HandlePaletteMouse(e.X, e.Y))
				{
					AdjustColorScrollbars();
				}
			}
		}

		private void pbPalette_MouseUp(object sender, MouseEventArgs e)
		{
			m_fPalette_Selecting = false;
		}

		public bool HandlePaletteMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert pixel (x,y) to palette (x,y).
			int nX = pxX / k_pxSwatchSize;
			int nY = pxY / k_pxSwatchSize;

			// Ignore if outside the palette list bounds.
			if (nX >= k_nSwatches || nY >= 1)
				return false;

			int nSelectedColor = nX;

			// Update the selection if a new color has been selected.
			if (m_subpalette.CurrentColor != nSelectedColor)
			{
				m_subpalette.CurrentColor = nSelectedColor;
				return true;
			}

			return false;
		}

		private void CalcRGBWord()
		{
			int nIndex = 0;
			m_nRGBWordBits[nIndex++] = 0;

			int cBlue = m_subpalette.Blue();
			m_nRGBWordBits[nIndex++] = (cBlue & 0x10) == 0x10 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cBlue & 0x08) == 0x08 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cBlue & 0x04) == 0x04 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cBlue & 0x02) == 0x02 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cBlue & 0x01) == 0x01 ? 1 : 0;

			int cGreen = m_subpalette.Green();
			m_nRGBWordBits[nIndex++] = (cGreen & 0x10) == 0x10 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cGreen & 0x08) == 0x08 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cGreen & 0x04) == 0x04 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cGreen & 0x02) == 0x02 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cGreen & 0x01) == 0x01 ? 1 : 0;

			int cRed = m_subpalette.Red();
			m_nRGBWordBits[nIndex++] = (cRed & 0x10) == 0x10 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cRed & 0x08) == 0x08 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cRed & 0x04) == 0x04 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cRed & 0x02) == 0x02 ? 1 : 0;
			m_nRGBWordBits[nIndex++] = (cRed & 0x01) == 0x01 ? 1 : 0;
		}

		private void AdjustColorScrollbars()
		{
			CalcRGBWord();

			m_fUpdatePalette = false;
			sbRed.Value = m_subpalette.Red();
			sbGreen.Value = m_subpalette.Green();
			sbBlue.Value = m_subpalette.Blue();
			m_fUpdatePalette = true;

			lRed.Text = String.Format("{0:X2}", sbRed.Value);
			lGreen.Text = String.Format("{0:X2}", sbGreen.Value);
			lBlue.Text = String.Format("{0:X2}", sbBlue.Value);

			m_parent.HandleColorDataChange(m_subpalette.Palette);

			pbPalette.Invalidate();
			pbPaletteLine.Invalidate();
			pbCurrent.Invalidate();
			
			pbRed.Invalidate();
			pbGreen.Invalidate();
			pbBlue.Invalidate();
			
			sbRed.Invalidate();
			sbGreen.Invalidate();
			sbBlue.Invalidate();

			pbRGBWord.Invalidate();
			pbHex.Invalidate();
			pbPaletteCode.Invalidate();
		}

		private void pbPalette_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.Black, 0, 0, 1000, 1000);

			Font f = new Font("Arial Black", 10);
			int[] nXOffset = new int[16] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6 };

			for (int iColumn = 0; iColumn < k_nSwatches; iColumn++)
			{
				int nIndex = iColumn;

				int pxX0 = 1 + iColumn * k_pxSwatchSize;
				int pxY0 = 1;

				g.FillRectangle(m_subpalette.Brush(nIndex), pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

				// Draw the transparent color (index 0) using a pattern.
				if (nIndex == 0)
					g.FillRectangle(m_brushTransparent, pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);

				// Draw the palette index in each swatch.
				if (Options.Sprite_ShowPaletteIndex)
				{
					int pxLabelOffsetX = nXOffset[nIndex];
					int pxLabelOffsetY = 2;
					g.DrawString(m_subpalette.Label(nIndex), f, m_subpalette.LabelBrush(nIndex), pxX0 + pxLabelOffsetX, pxY0 + pxLabelOffsetY);
				}

				// Draw a border around each color swatch.
				g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxSwatchSize, k_pxSwatchSize);
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_nSwatches * k_pxSwatchSize, 2 + k_pxSwatchSize);

			// Hilight the currently selected color.
			g.DrawRectangle(m_penHilight, (m_subpalette.CurrentColor * k_pxSwatchSize) + 1, 1, k_pxSwatchSize, k_pxSwatchSize);
		}

		private void pbPaletteLine_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Pen p = new Pen(Color.LightGray, 3.0f);
			g.DrawLine(p, (m_subpalette.CurrentColor * k_pxSwatchSize) + k_pxSwatchSize/2, 0, 194, 32);
		}

		private void pbCurrent_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.Black, 0, 0, 1000, 1000);

			g.FillRectangle(m_subpalette.Brush(), 1, 1, k_pxSwatchSize, k_pxSwatchSize);

			// Draw a border around each color swatch.
			g.DrawRectangle(Pens.White, 1, 1, k_pxSwatchSize, k_pxSwatchSize);

			//g.DrawRectangle(Pens.Black, 0, 0, 2 + k_nSwatches * k_pxSwatchSize, 2 + k_pxSwatchSize);
		}

		private void pbRGBLine_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Pen p = new Pen(Color.LightGray, 3.0f);
			g.DrawLine(p, 110, 0, 10, 38);
			g.DrawLine(p, 110, 0, 110, 38);
			g.DrawLine(p, 110, 0, 210, 38);
		}

		private void pbRed_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int cRed = m_subpalette.Red();
			int cOther = (31 - cRed) * 8;
			Brush bRed = new SolidBrush(Color.FromArgb(0xff, cOther, cOther));
			g.FillRectangle(bRed, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.White, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_pxRGBBitSize, 2 + k_pxRGBBitSize);
		}

		private void pbGreen_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int cGreen = m_subpalette.Green();
			int cOther = (31 - cGreen) * 8;
			Brush bGreen = new SolidBrush(Color.FromArgb(cOther, 0xff, cOther));
			g.FillRectangle(bGreen, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.White, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_pxRGBBitSize, 2 + k_pxRGBBitSize);
		}

		private void pbBlue_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int cBlue = m_subpalette.Blue();
			int cOther = (31 - cBlue) * 8;
			Brush bBlue = new SolidBrush(Color.FromArgb(cOther, cOther, 0xff));
			g.FillRectangle(bBlue, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.White, 1, 1, k_pxRGBBitSize, k_pxRGBBitSize);
			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_pxRGBBitSize, 2 + k_pxRGBBitSize);
		}

		private void pbRGBWord_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.White, 0,0,1000,1000);
			
			Font f = new Font("Arial", 9);
			int pxX0 = 1;
			int pxY0 = 1;

			for (int nIndex = 0; nIndex < k_nRGBBits; nIndex++)
			{
				SolidBrush b;
				Pen p;
				if (nIndex >= 11)
				{
					b = new SolidBrush(Color.FromArgb(0xff, 0x80, 0x80));
					p = Pens.Red;
				}
				else if (nIndex >= 6)
				{
					b = new SolidBrush(Color.FromArgb(0x80, 0xff, 0x80));
					p = Pens.Green;
				}
				else if (nIndex >= 1)
				{
					b = new SolidBrush(Color.FromArgb(0x80, 0x80, 0xff));
					p = Pens.Blue;
				}
				else
				{
					b = new SolidBrush(Color.White);
					p = Pens.Black;
				}

				//g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxRGBBitSize, k_pxRGBBitSize);
				g.FillRectangle(b, pxX0, pxY0, k_pxRGBBitSize, k_pxRGBBitSize);
				g.DrawRectangle(p, pxX0+1, pxY0+1, k_pxRGBBitSize-2, k_pxRGBBitSize-2);

				// Draw the bit value.
				g.DrawString(m_nRGBWordBits[nIndex].ToString(), f, Brushes.Black, pxX0 + 5, pxY0 + 3);

				// Draw a border around each color swatch.
				g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxRGBBitSize, k_pxRGBBitSize);

				pxX0 += k_pxRGBBitSize;
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_nRGBBits * k_pxRGBBitSize, 2 + k_pxRGBBitSize);
		}

		private void pbHexAlign_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int nTop = 3;
			int nBottom = 13;
			g.DrawLine(Pens.Gray, 1, nTop, 1, nBottom);
			g.DrawLine(Pens.Gray, 81, nTop, 81, nBottom);
			g.DrawLine(Pens.Gray, 161, nTop, 161, nBottom);
			g.DrawLine(Pens.Gray, 241, nTop, 241, nBottom);
			g.DrawLine(Pens.Gray, 321, nTop, 321, nBottom);
		}

		private void pbHex_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.White, 0, 0, 1000, 1000);

			Font f = new Font("Arial", 9);

			int pxNybbleWidth = k_pxRGBBitSize * k_nBitsPerNybble;

			int pxX0 = 1;
			int pxY0 = 1;

			// Most-significant-bit index for current nybble.
			int nMSB = 0;

			for (int nIndex = 0; nIndex < k_nRGBNybbles; nIndex++)
			{
				//g.DrawRectangle(Pens.White, pxX0, pxY0, k_pxRGBBitSize, k_pxRGBBitSize);
				//g.FillRectangle(b, pxX0, pxY0, k_pxRGBBitSize, k_pxRGBBitSize);
				g.DrawRectangle(Pens.Black, pxX0 + 1, pxY0 + 1, pxNybbleWidth - 2, k_pxRGBBitSize - 2);

				int nVal = m_nRGBWordBits[nMSB];
				nVal <<= 1;
				nVal |= m_nRGBWordBits[nMSB + 1];
				nVal <<= 1;
				nVal |= m_nRGBWordBits[nMSB + 2];
				nVal <<= 1;
				nVal |= m_nRGBWordBits[nMSB + 3];

				g.DrawString(String.Format("{0:X1}", nVal), f, Brushes.Black, pxX0 + 35, pxY0 + 3);

				// Draw a border around each color swatch.
				g.DrawRectangle(Pens.White, pxX0, pxY0, pxNybbleWidth, k_pxRGBBitSize);

				pxX0 += pxNybbleWidth;
				nMSB += 4;
			}

			g.DrawRectangle(Pens.Black, 0, 0, 2 + k_nRGBNybbles * pxNybbleWidth, 2 + k_pxRGBBitSize);
		}

		private void pbPaletteCode_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.FillRectangle(Brushes.White, 0, 0, 1000, 1000);

			Font f = new Font("Courier New", 8);
			int nLineHeight = (int)f.GetHeight();
			int nTabIndent = 15;
			int nY = 0;
			int nX = 0;
			g.DrawString(String.Format("const unsigned short int Palette[] = {{"), f, Brushes.Black, 0, nY);

			// Aaaargh! All we want to do here is measure the width of the 3 string segments:
			//      0x0000;
			//      aabbbbc
			// so a = width of "0x", b = width of "0000" and c = width = of ";".
			//
			// MeasureString seems perfect for this:
			//    PointF ptOrigin = new PointF(0.0f, 0.0f);
			//    int nWidth0 = (int)g.MeasureString("0x", f, ptOrigin, StringFormat.GenericTypographic).Width;
			//    int nWidth1 = (int)g.MeasureString("0000", f, ptOrigin, StringFormat.GenericTypographic).Width;
			//    int nWidth2 = (int)g.MeasureString(",", f, ptOrigin, StringFormat.GenericTypographic).Width;
			// except that it doesn't return the correct size.
			//
			// Fortunately, MeasureCharacterRanges does return the correct size, but it require significantly more
			// work to set up.
			string strTemplate = "0x0000;";		// We assume that all numbers in the font are the same width
			CharacterRange[] charanges =
			{
				new CharacterRange(0,2),
				new CharacterRange(2,4),
				new CharacterRange(6,1),
			};
			RectangleF rLayout = new RectangleF(0.0f, 0.0f, 100.0f, 100.0f);
			StringFormat sformat = new StringFormat();
			sformat.SetMeasurableCharacterRanges(charanges);
			Region[] sregions = new Region[3];
			sregions = g.MeasureCharacterRanges(strTemplate, f, rLayout, sformat);
			int nWidth0 = (int)sregions[0].GetBounds(g).Width;
			int nWidth1 = (int)sregions[1].GetBounds(g).Width;
			int nWidth2 = (int)sregions[2].GetBounds(g).Width;

			int nWidthTotal = nWidth0 + nWidth1 + nWidth2;

			nX = nTabIndent;
			nY += nLineHeight;
			for (int i = 0; i < 8; i++)
			{
				if (m_subpalette.CurrentColor == i)
					g.DrawRectangle(Pens.Red, nX + 1 + nWidth0, nY + 1, nWidth1, nLineHeight);
				g.DrawString(String.Format("0x{0:x4},", m_subpalette.Encoding(i)), f, Brushes.Black, nX, nY);
				nX += nWidthTotal;
			}

			nY += nLineHeight;
			nX = nTabIndent;
			for (int i = 0; i < 8; i++)
			{
				if (m_subpalette.CurrentColor == i+8)
					g.DrawRectangle(Pens.Red, nX + 1 + nWidth0, nY + 1, nWidth1, nLineHeight);
				g.DrawString(String.Format("0x{0:x4},", m_subpalette.Encoding(8+i)), f, Brushes.Black, nX, nY);
				nX += nWidthTotal;
			}
			
			nY += nLineHeight;
			g.DrawString(String.Format("}};"), f, Brushes.Black, 0, nY);
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			if (m_fHasChanges)
				this.DialogResult = DialogResult.Yes;
			this.Close();
		}

		private void ColorEncodingView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_fHasChanges)
				this.DialogResult = DialogResult.Yes;
		}

	}
}
