using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Subpalette
	{
		private Document m_doc;
		private Palette m_mgr;
		private int m_nSubpaletteID;

		private PaletteColorData m_data;

		/// <summary>
		/// A snapshot of the palette data from the last undo checkpoint.
		/// </summary>
		private PaletteColorData m_snapshot;

		/// <summary>
		/// Array of brushes for each color in the palette.
		/// </summary>
		private SolidBrush[] m_Brush;

		private const int k_nColors = 16;

		public enum DefaultColorSet {
			None,
			BlackAndWhite,
			Color1,
			Color2,
			GrayScale,
		};
		
		public Subpalette(Document doc, Palette mgr, int nSubpaletteID)
		{
			Init(doc, mgr, nSubpaletteID, DefaultColorSet.None);
		}

		public Subpalette(Document doc, Palette mgr, int nSubpaletteID, DefaultColorSet eDefaultColorSet)
		{
			Init(doc, mgr, nSubpaletteID, eDefaultColorSet);
		}

		private void Init(Document doc, Palette mgr, int nSubpaletteID, DefaultColorSet eDefaultColorSet)
		{
			m_doc = doc;
			m_mgr = mgr;
			m_nSubpaletteID = nSubpaletteID;

			m_data = new PaletteColorData(k_nColors);
			m_Brush = new SolidBrush[k_nColors];

			// Default color = black (index 1)
			m_data.currentColor = 1;

			m_snapshot = new PaletteColorData(k_nColors);
			SetDefaultSubpaletteColors(eDefaultColorSet);
		}

		/// <summary>
		/// Set the colors for this subpalette to a set of default colors.
		/// </summary>
		public void SetDefaultSubpaletteColors(DefaultColorSet eDefaultColorSet)
		{
			switch (eDefaultColorSet)
			{
				case DefaultColorSet.None:
					break;
				case DefaultColorSet.BlackAndWhite:
					DefaultPalette_BlackAndWhite();
					break;
				case DefaultColorSet.Color1:
					DefaultPalette_Color1();
					break;
				case DefaultColorSet.Color2:
					DefaultPalette_Color2();
					break;
				case DefaultColorSet.GrayScale:
					DefaultPalette_GrayScale();
					break;
				default:
					break;
			}

			RecordSnapshot();
		}

		private void DefaultPalette_BlackAndWhite()
		{
			UpdateColor(0, 0x7fff);		// transparent (white)
			UpdateColor(1, 0x0000);		// black
			for (int i = 2; i < k_nColors; i++)
				UpdateColor(i, 0x7fff);	// white
		}

		private void DefaultPalette_Color1()
		{
			int i = 0;
			UpdateColor(i++, 0x1f, 0x1f, 0x1f);		// transparent (white)
			UpdateColor(i++, 0x00, 0x00, 0x00);		// black
			UpdateColor(i++, 0x10, 0x00, 0x00);		// dk red
			UpdateColor(i++, 0x00, 0x10, 0x00);		// green

			UpdateColor(i++, 0x1f, 0x1f, 0x1f);		// white
			UpdateColor(i++, 0x10, 0x10, 0x10);		// gray
			UpdateColor(i++, 0x1f, 0x00, 0x00);		// red
			UpdateColor(i++, 0x00, 0x1f, 0x00);		// lt green

			UpdateColor(i++, 0x10, 0x10, 0x00);		// greenish yellow
			UpdateColor(i++, 0x00, 0x00, 0x10);		// blue
			UpdateColor(i++, 0x10, 0x00, 0x10);		// purple
			UpdateColor(i++, 0x00, 0x10, 0x10);		// aqua

			UpdateColor(i++, 0x1f, 0x1f, 0x00);		// yellow
			UpdateColor(i++, 0x00, 0x00, 0x1f);		// blue
			UpdateColor(i++, 0x1f, 0x00, 0x1f);		// magenta
			UpdateColor(i++, 0x00, 0x1f, 0x1f);		// cyan
			if (i != k_nColors)
				//System.Windows.Forms.MessageBox.Show("Wrong number of colors in palette");
				System.Windows.Forms.MessageBox.Show(ResourceMgr.GetString("ErrorNumColorsInPalette0"));
		}

		private void DefaultPalette_Color2()
		{
			int i = 0;
			UpdateColor(i++, 0x7fff);		// transparent (white)
			UpdateColor(i++, 0x0000);		// black
			UpdateColor(i++, 0x7fff);		// white
			UpdateColor(i++, 0x0c59);		// red
			UpdateColor(i++, 0x0e04);		// green
			UpdateColor(i++, 0x6d08);		// blue
			UpdateColor(i++, 0x0fdf);		// yellow
			UpdateColor(i++, 0x6b64);		// cyan
			UpdateColor(i++, 0x56b5);		// lt gray
			UpdateColor(i++, 0x318c);		// dk gray
			UpdateColor(i++, 0x0d11);		// brown
			UpdateColor(i++, 0x5a9f);		// lt red (pink)
			UpdateColor(i++, 0x4351);		// lt green
			UpdateColor(i++, 0x7e93);		// lt blue
			UpdateColor(i++, 0x1e7f);		// orange
			UpdateColor(i++, 0x5cd1);		// purple
			if (i != k_nColors)
				//System.Windows.Forms.MessageBox.Show("Wrong number of colors in palette");
				System.Windows.Forms.MessageBox.Show(ResourceMgr.GetString("ErrorNumColorsInPalette0"));
		}

		private void DefaultPalette_GrayScale()
		{
			UpdateColor(0, 0x7fff);		// transparent (white)
			for (int i = 1, cGray=0x00; i < 8; i++, cGray += 2)
				UpdateColor(i, cGray, cGray, cGray);
			UpdateColor(8, 0x10, 0x10, 0x10);
			for (int i = 9, cGray = 0x13; i < 16; i++, cGray += 2)
				UpdateColor(i, cGray, cGray, cGray);
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public void Copy(Subpalette pal)
		{
			for (int i = 0; i < k_nColors; i++)
				UpdateColor(i, pal.Red(i), pal.Green(i), pal.Blue(i));
			RecordSnapshot();
		}

		public Palette Palette
		{
			get { return m_mgr; }
		}

		public int SubpaletteID
		{
			get { return m_nSubpaletteID; }
		}

		public int Red() { return m_data.cRed[m_data.currentColor]; }
		public int Green() { return m_data.cGreen[m_data.currentColor]; }
		public int Blue() { return m_data.cBlue[m_data.currentColor]; }

		public int Red(int nIndex) { return m_data.cRed[nIndex]; }
		public int Green(int nIndex) { return m_data.cGreen[nIndex]; }
		public int Blue(int nIndex) { return m_data.cBlue[nIndex]; }

		/// <summary>
		/// Return the 16-bit encoding for the specified palette index.
		/// </summary>
		/// <param name="nIndex">The index of the palette entry</param>
		/// <returns>The encoding for the specified palette entry</returns>
		public int Encoding(int nIndex)
		{
			return m_data.Encoding(nIndex);
		}

		/// <summary>
		/// Return the brush corresponding to this index in the palette.
		/// </summary>
		/// <param name="nIndex">Index into palette</param>
		/// <returns>Brush for this palette index</returns>
		public SolidBrush Brush(int nIndex)
		{
			return m_Brush[nIndex];
		}

		public SolidBrush Brush()
		{
			return m_Brush[m_data.currentColor];
		}

		public string Label(int nIndex)
		{
			return String.Format("{0:X1}", nIndex);
		}

		/// <summary>
		/// Return a brush that can be used to draw a label over this index in the palette.
		/// </summary>
		/// <param name="nIndex">Index into palette</param>
		/// <returns>Brush for drawing text over this palette index color</returns>
		public Brush LabelBrush(int nIndex)
		{
			if (m_data.cRed[nIndex] + m_data.cGreen[nIndex] + m_data.cBlue[nIndex] > 24)
				return Brushes.Black;
			return Brushes.White;
		}

		/// <summary>
		/// The index of the currently selected color in the sub-palette.
		/// </summary>
		public int CurrentColor
		{
			get { return m_data.currentColor; }
			set { m_data.currentColor = value; }
		}

		public void UpdateColor(int cRed, int cGreen, int cBlue)
		{
			UpdateColor(m_data.currentColor, cRed, cGreen, cBlue);
		}

		public void UpdateColor(int nIndex, int cRed, int cGreen, int cBlue)
		{
			m_data.cRed[nIndex] = cRed;
			m_data.cGreen[nIndex] = cGreen;
			m_data.cBlue[nIndex] = cBlue;

			if (m_Brush[nIndex] != null)
				m_Brush[nIndex].Dispose();
			m_Brush[nIndex] = new SolidBrush(Color555.CalcColor(cRed, cGreen, cBlue));
		}

		public void UpdateColor(int nIndex, int color)
		{
			int cRed, cGreen, cBlue;
			Color555.ExtractColors(color, out cRed, out cGreen, out cBlue);
			UpdateColor(nIndex, cRed, cGreen, cBlue);
		}

		/// <summary>
		/// Import an entire subpalette from an array of uints.
		/// </summary>
		/// <param name="iPalette"></param>
		/// <returns></returns>
		public bool Import(int[] anPalette)
		{
			for (int i = 0; i < 16; i++)
			{
				ImportColor(i, anPalette[i]);
			}
			return true;
		}

		/// <summary>
		/// Import a single color into the palette.
		/// </summary>
		/// <param name="nIndex"></param>
		/// <param name="color"></param>
		public void ImportColor(int nIndex, int color)
		{
			int cRed, cGreen, cBlue;
			Color555.ExtractColors(color, out cRed, out cGreen, out cBlue);
			UpdateColor(nIndex, cRed, cGreen, cBlue);
		}

		public void RecordUndoAction(string strDesc, UndoMgr undo)
		{
			if (undo == null)
				return;

			PaletteColorData data = GetUndoData();
			UndoAction_Subpalette16Edit action = new UndoAction_Subpalette16Edit(undo, this, m_snapshot, data, strDesc);
			bool fHandled = false;

			if (action.IsSelectionChange())
			{
				// Ignore selection changes
				fHandled = true;
			}
			else
			{
				// If the last undo action is also a PaletteEdit, try to merge them together
				UndoAction_Subpalette16Edit last_action = undo.GetCurrent() as UndoAction_Subpalette16Edit;
				if (last_action != null)
				{
					// Merge 2 color changes (as long as they edit the same color)
					int last_color, last_color_val1, last_color_val2;
					int this_color, this_color_val1, this_color_val2;
					if (last_action.IsColorChange(out last_color, out last_color_val1, out last_color_val2)
						&& action.IsColorChange(out this_color, out this_color_val1, out this_color_val2)
						)
					{
						if (last_color == this_color)
						{
							// If this color change takes it back to the original value, then
							// delete the UndoAction.
							if (last_color_val1 == this_color_val2)
								undo.DeleteCurrent();
							else
								last_action.UpdateRedoData(data);
							fHandled = true;
						}
					}
				}
			}
			if (!fHandled)
				undo.Push(action);

			// Update the snapshot for the next UndoAction
			RecordSnapshot();
		}

		private PaletteColorData GetUndoData()
		{
			PaletteColorData undo = new PaletteColorData(k_nColors);
			RecordUndoData(ref undo);
			return undo;
		}

		public void RecordSnapshot()
		{
			RecordUndoData(ref m_snapshot);
		}

		private void RecordUndoData(ref PaletteColorData undo)
		{
			undo.currentColor = m_data.currentColor;

			for (int i = 0; i < k_nColors; i++)
			{
				undo.cRed[i] = m_data.cRed[i];
				undo.cGreen[i] = m_data.cGreen[i];
				undo.cBlue[i] = m_data.cBlue[i];
			}
		}

		public void ApplyUndoData(PaletteColorData undo)
		{
			m_data.currentColor = undo.currentColor;

			for (int i = 0; i < k_nColors; i++)
				UpdateColor(i, undo.cRed[i], undo.cGreen[i], undo.cBlue[i]);

			RecordSnapshot();
		}

		public void Save(System.IO.TextWriter tw)
		{
			StringBuilder sb = null;
			int nPerLine = 8;
			string strIndent = "\t\t\t\t";

			for (int i = 0; i < 16; i++)
			{
				if ((i % nPerLine) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder(strIndent);
				}
				sb.Append(String.Format("<color rgb=\"{0:x2}{1:x2}{2:x2}\"/>", Red(i), Green(i), Blue(i)));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
		}

		public bool Export_Subpalette(System.IO.TextWriter tw, int nSubpaletteId)
		{
			StringBuilder sb = null;
			int nPerLine = 8;

			tw.WriteLine("\t// Subpalette #" + nSubpaletteId);

			for (int i = 0; i < 16; i++)
			{
				if ((i % nPerLine) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder("\t");
				}
				sb.Append(String.Format("0x{0:x4},", Encoding(i)));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
			return true;
		}

	}
}
