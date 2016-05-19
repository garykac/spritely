using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class CollisionTest : Form
	{
		Document m_doc;

		Spriteset ss;
		Sprite s1, s2;
		int xOffset, yOffset;

		const int MaskWordWidth = 32;
		int[] mask1, mask2;
		int mask1w, mask1h;
		int mask2w, mask2h;

		public CollisionTest(Document d)
		{
			m_doc = d;

			InitializeComponent();

			xOffset = 0;
			yOffset = 0;

			ss = d.Owner.ActiveSpriteset();

			s1 = ss.CurrentSprite;
			s2 = ss.NextSprite(s1);

			mask1 = CalcMask(s1, out mask1w, out mask1h);
			mask2 = CalcMask(s2, out mask2w, out mask2h);

			CollisionCheck();
		}

		private int[] CalcMask(Sprite s, out int width, out int height)
		{
			width = 0;
			height = 0;
			if (s == null)
				return null;
			// Calc # of int32s required.
			int xsize = ((s.PixelWidth + MaskWordWidth-1) / MaskWordWidth);
			int ysize = s.PixelHeight;
			int size = xsize * ysize;
			int[] mask = new int[size];

			for (int y = 0; y < ysize; y++)
			{
				for (int x = 0; x < xsize; x++)
				{
					int index = y * xsize + x;
					mask[index] = 0;
					for (int i = 0; i < MaskWordWidth; i++)
					{
						mask[index] <<= 1;
						if (x * MaskWordWidth + i < s.PixelWidth)
						{
							if (s.GetPixel(x * MaskWordWidth + i, y) != 0)
								mask[index] |= 1;
						}
					}
				}
			}
			width = xsize;
			height = ysize;
			return mask;
		}

		private int min(int a, int b)
		{
			return (a < b) ? a : b;
		}

		//    +----------+         -
		//    |A         |         | y offset
		//    |     +---------+    -
		//    |     |B        |
		//    |     |         |
		//    +-----|         |
		//          |         |
		//          +---------+
		//
		//    |-----|
		//       x offset

		private bool CollisionCheck()
		{
			if (s2 == null)
				return false;

			StringBuilder sb = new StringBuilder();

			if (xOffset >= s1.PixelWidth || yOffset >= s1.PixelHeight
				|| -xOffset >= s2.PixelWidth || -yOffset >= s2.PixelHeight)
			{
				tbInfo.Text = "BBoxes don't overlap";
				return false;
			}

			// Calculate the intersection rect in the local coord system
			// of each sprite.
			int s1x = xOffset >= 0 ? xOffset : 0;
			int s1y = yOffset >= 0 ? yOffset : 0;
			int s2x = xOffset < 0 ? -xOffset : 0;
			int s2y = yOffset < 0 ? -yOffset : 0;
			sb.Append(String.Format("offset: {0},{1}\r\n", xOffset, yOffset));

			int w, h;
			if (xOffset >= 0)
				w = min(s2.PixelWidth, s1.PixelWidth - s1x);
			else
				w = min(s1.PixelWidth, s2.PixelWidth - s2x);
			if (yOffset >= 0)
				h = min(s2.PixelHeight, s1.PixelHeight - s1y);
			else
				h = min(s1.PixelHeight, s2.PixelHeight - s2y);
			sb.Append(String.Format("s1 size: {0},{1}\r\n", s1.PixelWidth, s1.PixelHeight));
			sb.Append(String.Format("s2 size: {0},{1}\r\n", s2.PixelWidth, s2.PixelHeight));
			sb.Append(String.Format("overlap: {0},{1}\r\n", w,h));

			// Slow but sure method to use for verification.
			int verify_count = 0;
			bool fFirst = true;
			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					if (s1.GetPixel(s1x + x, s1y + y) != 0
							&& s2.GetPixel(s2x + x, s2y + y) != 0)
					{
						if (fFirst)
							sb.Append(String.Format("Hit at {0},{1}\r\n", x, y));
						fFirst = false;
						verify_count++;
						tbInfo.Text = sb.ToString();
					}
				}
			}
			sb.Append(String.Format("count: {0}\r\n", verify_count));

			// Run bitmask based collision check.
			// Assumes bitmasks are padded out with 0's to a MaskWidth boundary
			// General approach is to:
			//   calculate the bitmask overlap area
			//   for each line:
			//     for each bitmask:
			//       shift bits in A so that they align with B
			//       if shited-A & B != 0:
			//         we have a collision
			//
			// Shifting the masks to align is the only tricky part.
			int[] a_mask, b_mask;
			int a_maskw, a_maskh;
			int b_maskw, b_maskh;
			int x_offset, y_offset;

			if (xOffset >= 0)
			{
				a_mask = mask1;
				a_maskw = mask1w;
				a_maskh = mask1h;
				b_mask = mask2;
				b_maskw = mask2w;
				b_maskh = mask2h;
				x_offset = xOffset;
				y_offset = yOffset;
			}
			else
			{
				a_mask = mask2;
				a_maskw = mask2w;
				a_maskh = mask2h;
				b_mask = mask1;
				b_maskw = mask1w;
				b_maskh = mask1h;
				x_offset = -xOffset;
				y_offset = -yOffset;
			}

			int a_lshift = x_offset % MaskWordWidth;
			int a_maskword_offset = x_offset / MaskWordWidth;
			int maskword_count = (w + MaskWordWidth - 1) / MaskWordWidth;
			int a_x0, b_x0;
			int a_xN;
			if (y_offset >= 0)
			{
				a_x0 = (y_offset * a_maskw) + a_maskword_offset;
				a_xN = a_maskw - (y_offset * a_maskw);
				b_x0 = 0;
			}
			else
			{
				a_x0 = a_maskword_offset;
				a_xN = a_maskw;
				b_x0 = (-y_offset * b_maskw);
			}

			bool fFirstMessage = true;
			int count = 0;
			if (a_lshift == 0)
			{
				sb.Append("PP checking mask aligned\r\n");
				// offset is aligned to maskwidth boundary - no bit shifting needed 
				for (int x = 0; x < maskword_count; x++)
				{
					for (int y = 0; y < h; y++)
					{
						if ((a_mask[a_x0 + x] & b_mask[b_x0 + x]) != 0)
						{
							if (fFirstMessage)
								sb.Append("PP found collision\r\n");
							fFirstMessage = false;
							count = 1;
						}
						a_x0 += a_maskw;
						b_x0 += b_maskw;
					}
				}
			}
			else
			{
				sb.Append("PP checking mask shifted\r\n");
				int a_rshift = MaskWordWidth - a_lshift;
				for (int x = 0; x < maskword_count; x++)
				{
					for (int y = 0; y < h; y++)
					{
						int a = a_mask[a_x0 + x] << a_lshift;
						if (x + 1 < a_xN)
							a |= a_mask[a_x0 + x + 1] >> a_rshift;
						if ((a & b_mask[b_x0 + x]) != 0)
						{
							if (fFirstMessage)
								sb.Append("PP found collision\r\n");
							fFirstMessage = false;
							count = 1;
						}
						a_x0 += a_maskw;
						b_x0 += b_maskw;
					}
				}
			}

			bool fCollision = verify_count != 0;
			lResult.Text = fCollision ? "Collision" : "No collision";
			tbInfo.Text = sb.ToString();

			if (!((count == 0 && verify_count == 0) || (count != 0 && verify_count != 0)))
				lResult.Text = "ERROR! no match";

			return fCollision;
		}

		private void pbCollision_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			s1.DrawTransparentSprite(g, 100, 100);
			if (s2 != null)
				s2.DrawTransparentSprite(g, 100+(xOffset * Tile.SmallBitmapPixelSize),
					100+(yOffset * Tile.SmallBitmapPixelSize));
		}

		private void bDown_Click(object sender, EventArgs e)
		{
			yOffset++;
			CollisionCheck();
			pbCollision.Invalidate();
		}

		private void bUp_Click(object sender, EventArgs e)
		{
			yOffset--;
			CollisionCheck();
			pbCollision.Invalidate();
		}

		private void bRight_Click(object sender, EventArgs e)
		{
			xOffset++;
			CollisionCheck();
			pbCollision.Invalidate();
		}

		private void bLeft_Click(object sender, EventArgs e)
		{
			xOffset--;
			CollisionCheck();
			pbCollision.Invalidate();
		}
	}
}