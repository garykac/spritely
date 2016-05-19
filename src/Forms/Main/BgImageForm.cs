using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class BgImageForm : Form
	{
		private ProjectMainForm m_parent;

		private BgImage m_bgimage;

		private Toolbox_BgImage m_toolbox;

		private const int k_nMaxMapTilesX = 32;
		private const int k_nMaxMapTilesY = 32;
		private const int k_nGBAScreenTilesX = 30;
		private const int k_nGBAScreenTilesY = 20;
		private const int k_nNDSScreenTilesX = 32;
		private const int k_nNDSScreenTilesY = 24;

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

		public BgImageForm(ProjectMainForm parent, BgImage bgi)
		{
			m_parent = parent;
			m_bgimage = bgi;

			InitializeComponent();

			m_toolbox = new Toolbox_BgImage();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			StartPosition = FormStartPosition.Manual;
			Visible = false;
			ControlBox = false;

			// Set to 16x.
			cbZoom.SelectedIndex = (int)ZoomLevel.Zoom_16x;
			cbZoom.Enabled = false;

			lNoImage.Visible = false;
		}

		public void SetBgImage(BgImage bgi)
		{
			m_bgimage = bgi;
			pbBgImage.Invalidate();
			SetTitle();
		}

		/// <summary>
		/// Update the form title with the bgimage name.
		/// </summary>
		public void SetTitle()
		{
			if (m_bgimage != null)
				Text = "Background Image '" + m_bgimage.Name + "'";
		}

		#region Window events
		
		private void BgImageForm_Resize(object sender, EventArgs e)
		{
			m_parent.ResizeSubwindow(this);
		}

		private void BgImageForm_FormClosing(object sender, FormClosingEventArgs e)
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
			pbBgImage.Invalidate();
		}

		#endregion

		private void bInfo_Click(object sender, EventArgs e)
		{
			if (m_bgimage == null)
				return;

			BgImageProperties properties = new BgImageProperties(m_parent.Document, m_bgimage);
			properties.ShowDialog();

			// The name of the background image may have changed, so update the form title.
			SetTitle();
		}

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
		}

		private void pbTools_MouseLeave(object sender, EventArgs e)
		{
			if (m_toolbox.HandleMouseMove(-10, -10))
			{
				pbTools.Invalidate();
			}
		}

		private void pbTools_Paint(object sender, PaintEventArgs e)
		{
			m_toolbox.Draw(e.Graphics, pbTools.Size);
		}

		#endregion

		#region BgImage

		private bool m_fBgImage_Selecting = false;

		private void pbBgImage_MouseDown(object sender, MouseEventArgs e)
		{
			m_fBgImage_Selecting = true;
			pbBgImage_MouseMove(sender, e);
		}

		private void pbBgImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_fBgImage_Selecting)
			{
				if (HandleMouse(e.X, e.Y))
				{
					//m_parent.HandleMapDataChange(m_map);
				}
			}

			// Update boundary rect for currently selected sprite.
			//if (HandleMouseMove_EditMap(e.X, e.Y))
			//{
			//	pbBgImage.Invalidate();
			//}
		}

		private void pbBgImage_MouseUp(object sender, MouseEventArgs e)
		{
			//if (HandleMouseMove_EditMap(-10, -10))
			//{
			//	pbBgImage.Invalidate();
			//}
			m_fBgImage_Selecting = false;
		}

		public bool HandleMouse(int pxX, int pxY)
		{
			return false;
		}

		private void pbBgImage_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (m_bgimage != null)
			{
				lNoImage.Visible = false;
				Bitmap bm = m_bgimage.Bitmap;
				if (bm != null)
				{
					g.DrawImage(bm, 0, 0, bm.Width * 2, bm.Height * 2);
					g.DrawRectangle(Pens.Black, 0, 0, bm.Width * 2, bm.Height * 2);
				}
			}
			else
			{
				lNoImage.Visible = true;
			}
		}

		#endregion

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{
			//BigBitmapPixelSize = 1 << cbZoom.SelectedIndex;
			pbBgImage.Invalidate();
		}

		public bool ValidateBitmap(Bitmap b)
		{
			int nHeight = b.Height;
			int nWidth = b.Width;
			int flags = b.Flags;

			// Determine the number of unique colors in the bitmap.
			Dictionary<Color, int> pal = new Dictionary<Color, int>();
			int nTransparent = 0;
			for (int ix = 0; ix < nWidth; ix++)
			{
				for (int iy = 0; iy < nHeight; iy++)
				{
					Color c = b.GetPixel(ix, iy);
					if (c == Color.Transparent)
						nTransparent++;
					if (!pal.ContainsKey(c))
						pal.Add(c, 0);
					pal[c]++;
				}
			}
			return true;
		}

	}
}
