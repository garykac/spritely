using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class BgImageListForm : Form
	{
		private ProjectMainForm m_parent;
		private BgImages m_bgimages;

		private Font m_font = new Font("Arial", 8, FontStyle.Bold);

		/// <summary>
		/// The maximum size images that we support.
		/// Images smaller than this size are displayed at 50% resolution in list.
		/// Images larger than this size (either dimension) are scaled to fit.
		/// </summary>
		private const int k_nImageMaxSize = 256;

		/// <summary>
		/// The size of the small images in the image list.
		/// This should be 2x ImageMaxSize
		/// </summary>
		private const int k_nImageDisplaySize = 128;

		private const int k_nMaxImagesX = 1;
		private const int k_nMaxImagesY = 3;

		private static Pen m_penHilight = new Pen(Color.FromArgb(128, Color.Red), 3);

		public BgImageListForm(ProjectMainForm parent, BgImages bmis)
		{
			m_parent = parent;
			m_bgimages = bmis;

			InitializeComponent();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;
			Text = "Background Images";
		}

		#region Window events

		private void BgImageListForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void BgImageListForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_parent.CloseSubwindow(this);
			e.Cancel = true;
		}

		#endregion

		#region Subwindow updates

		/// <summary>
		/// The bgimage selecton has changed.
		/// </summary>
		public void BgImageSelectionChanged()
		{
			pbBgImages.Invalidate();
		}

		#endregion

		#region Scrollbar

		private int m_nScrollPosition = 0;

		private int m_nTotalScrollHeight = 0;

		public int VisibleScrollRows
		{
			get { return k_nMaxImagesY; }
		}

		public int MaxScrollRows
		{
			get { return m_nTotalScrollHeight; }
		}

		public void RecalcScrollHeights()
		{
			m_nTotalScrollHeight = m_bgimages.NumImages;
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
				sbBgImages.Enabled = false;
				sbBgImages.Value = 0;
			}
			else
			{
				sbBgImages.Enabled = true;
				sbBgImages.Minimum = 0;
				sbBgImages.Maximum = nMaxRows - 2;
				sbBgImages.LargeChange = nVisibleRows - 1;
			}
			pbBgImages.Invalidate();
		}

		private void sbBgImages_ValueChanged(object sender, EventArgs e)
		{
			ScrollTo(sbBgImages.Value);
			pbBgImages.Invalidate();
		}

		#endregion

		private bool m_fBgImageList_Selecting = false;

		private void pbBgImages_MouseDown(object sender, MouseEventArgs e)
		{
			m_fBgImageList_Selecting = true;
			pbBgImages_MouseMove(sender, e);
		}

		private void pbBgImages_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fBgImageList_Selecting)
			{
				if (HandleMouse(e.X, e.Y))
				{
					m_parent.HandleBackgroundImageSelectionChanged(m_bgimages);
				}
			}
		}

		private void pbBgImages_MouseUp(object sender, MouseEventArgs e)
		{
			m_fBgImageList_Selecting = false;
		}

		private void pbBgImages_DoubleClick(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Handle a mouse event in the bgimage list.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <returns>True if the bgimage list needs to be redrawn</returns>
		public bool HandleMouse(int pxX, int pxY)
		{
			if (pxX < 0 || pxY < 0)
				return false;

			// Convert screen pixel (x,y) to image (x,y).
			int nImageX = pxX / k_nImageDisplaySize;
			int nImageY = pxY / k_nImageDisplaySize;

			// Ignore if outside the BgImage list display bounds.
			if (nImageX >= k_nMaxImagesX || nImageY >= k_nMaxImagesY)
				return false;

			// Adjust for the current scroll position.
			nImageY += m_nScrollPosition;

			// Disallow selections beyond end of bgimage list.
			if (nImageY >= m_bgimages.NumImages)
				return false;

			// Find selected image.
			int nIndex = 0;
			foreach (BgImage bgi in m_bgimages.AllBgImages)
				if (nImageY == nIndex++)
				{
					if (m_bgimages.CurrentImage != bgi)
					{
						m_bgimages.CurrentImage = bgi;
						return true;
					}
					break;
				}

			return false;
		}

		private void pbBgImages_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			int pxMargin = 1;
			int x0 = pxMargin;
			int y0 = pxMargin;
			int nImageIndex = -1;
			foreach (BgImage bgi in m_bgimages.AllBgImages)
			{
				nImageIndex++;
				if (nImageIndex < m_nScrollPosition)
					continue;
				if (nImageIndex >= m_nScrollPosition + k_nMaxImagesY)
					break;

				if (bgi.Bitmap != null)
				{
					g.DrawRectangle(Pens.Gray, x0, y0, k_nImageDisplaySize, k_nImageDisplaySize);

					int w = bgi.Bitmap.Width;
					int h = bgi.Bitmap.Height;
					int pxScaleW, pxScaleH;

					// Scale and center image in 128x128 rect
					if (w <= k_nImageMaxSize && h <= k_nImageMaxSize)
					{
						// Show image at half-size in list.
						pxScaleW = w * k_nImageDisplaySize / k_nImageMaxSize;
						pxScaleH = h * k_nImageDisplaySize / k_nImageMaxSize;
					}
					else
					{
						// Image is larger than 256 pixels in either dimension.
						// Choose largest dimension and scale appropriately.
						if (w > h)
						{
							pxScaleW = k_nImageDisplaySize;
							pxScaleH = h * k_nImageDisplaySize / w;
						}
						else
						{
							pxScaleW = w * k_nImageDisplaySize / h;
							pxScaleH = k_nImageDisplaySize;
						}
					}

					int pxOriginX = (k_nImageDisplaySize - pxScaleW) / 2;
					int pxOriginY = (k_nImageDisplaySize - pxScaleH) / 2;
					g.DrawImage(bgi.Bitmap, x0+pxOriginX, y0+pxOriginY, pxScaleW, pxScaleH);
					g.DrawRectangle(Pens.Black, x0+pxOriginX, y0+pxOriginY, pxScaleW, pxScaleH);

					if (bgi == m_bgimages.CurrentImage)
						g.DrawRectangle(m_penHilight, x0, y0, k_nImageDisplaySize, k_nImageDisplaySize);

					y0 += k_nImageDisplaySize + 2*pxMargin;
				}
			}
		}

		private void bImport_Click(object sender, EventArgs e)
		{
			string strFilename = m_parent.Document.Filer.ChooseBgImageFile();
			if (strFilename == "")
				return;

			Bitmap b = new Bitmap(strFilename);

			// Cleanup filename to create image name.
			string[] s = strFilename.Split("\\/".ToCharArray());
			string strImageName = Regex.Replace(s[s.Length - 1], @"[-\.,;:+= <>]", "_");
			m_bgimages.AddBgImage(strImageName, -1, "", b);
			pbBgImages.Invalidate();
		}

	}
}
