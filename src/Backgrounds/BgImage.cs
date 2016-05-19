using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public class BgImage
	{
		private Document m_doc;

		public string Name;
		private int m_id;
		public string Description;

		public string HeaderFileName;

		private Bitmap m_bm;

		private int m_width, m_height;
		private byte[,] m_ImageData = null;
		private Dictionary<Color, short> m_mapColor2Id = null;
		private Dictionary<short, Color> m_mapId2Color = null;

		public BgImage(Document doc, string strName, int id, string strDesc)
		{
			m_doc = doc;

			Name = strName;
			m_id = id;
			Description = strDesc;

			HeaderFileName = strName + ".h";

			m_bm = null;
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public void RecordSnapshot()
		{
		}

		public Bitmap Bitmap
		{
			get { return m_bm; }
		}

		/// <summary>
		/// Generate the GBA/NDS formatted data from the bitmap image.
		/// </summary>
		/// <returns>True if the data was successfully converted.</returns>
		public bool LoadBitmap(Bitmap bm)
		{
			m_bm = bm;

			m_width = bm.Width;
			m_height = bm.Height;

			// Gather the colors from the bitap.
			m_mapColor2Id = new Dictionary<Color, short>();
			m_mapId2Color = new Dictionary<short, Color>();
			short nColorId = 0;

			// Add the transparent color to the Id map at index 0.
			m_mapId2Color.Add(nColorId++, Color.White);

			for (int ix = 0; ix < m_width; ix++)
			{
				for (int iy = 0; iy < m_height; iy++)
				{
					Color c = bm.GetPixel(ix, iy);
					if (c != Color.Transparent && !m_mapColor2Id.ContainsKey(c))
					{
						m_mapColor2Id.Add(c, nColorId);
						m_mapId2Color.Add(nColorId, c);
						nColorId++;
					}
				}
			}

			// At this point,
			//   nColorId = number of colors in image + 1 (for transparency color at index 0)

			if (nColorId > 257)
			{
				m_doc.ErrorString("Too many colors {0} in image. Imported images are restricted to 255 colors (+1 transparency color)", nColorId);
				return false;
			}
			if (nColorId == 257)
			{
				// TODO: select transparency color
				m_doc.ErrorString("Exactly 256 colors - please choose one of the colors to be the transparent 'color'");
				return false;
			}
			while (nColorId < 256)
			{
				// Pad out palette with enough colors.
				m_mapId2Color.Add(nColorId++, Color.White);
			}

			m_ImageData = new byte[m_width, m_height];

			for (int ix = 0; ix < m_width; ix++)
			{
				for (int iy = 0; iy < m_height; iy++)
				{
					Color c = m_bm.GetPixel(ix, iy);
					if (c == Color.Transparent)
						m_ImageData[ix,iy] = 0;
					else
						m_ImageData[ix, iy] = (byte)m_mapColor2Id[c];
				}
			}

			m_mapColor2Id = null;
			return true;
		}

		#region Load

		public bool LoadXML_bgimage_pal8(XmlNode xnode, int nWidth, int nHeight)
		{
			m_width = nWidth;
			m_height = nHeight;

			m_mapId2Color = new Dictionary<short, Color>();
			m_ImageData = new byte[m_width, m_height];

			if (!LoadXML_bgimage_pal8_(xnode, nWidth, nHeight))
			{
				m_mapId2Color = null;
				m_ImageData = null;
				return false;
			}

			// Create bitmap from loaded image data.
			m_bm = new Bitmap(nWidth, nHeight);
			for (int x = 0; x < nWidth; x++)
			{
				for (int y = 0; y < nHeight; y++)
				{
					m_bm.SetPixel(x, y, m_mapId2Color[m_ImageData[x,y]]);
				}
			}

			return true;
		}

		private bool LoadXML_bgimage_pal8_(XmlNode xnode, int nWidth, int nHeight)
		{
			bool fLoadPalette = false;
			bool fLoadData = false;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "palette256")
				{
					string strName = XMLUtils.GetXMLAttribute(xn, "name");
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");

					// TODO: move this into Palette class.
					if (!LoadXML_palette256(xn))
						return false;
					fLoadPalette = true;
				}
				if (xn.Name == "bgimagedata")
				{
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");

					if (!LoadXML_bgimagedata(xn, nWidth, nHeight))
						return false;
					fLoadData = true;
				}
			}

			if (fLoadPalette && fLoadData)
				return true;

			m_doc.ErrorString("Unable to load background image");
			return false;
		}

		public bool LoadXML_palette256(XmlNode xnode)
		{
			short nColors = 0;
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "color")
				{
					string strRGB = XMLUtils.GetXMLAttribute(xn, "rgb");
					int nRGB;
					if (!Palette.ParseRGBColorValue(strRGB, out nRGB))
					{
						m_doc.ErrorString("Unable to parse color value '{0}' in palette '{1}'.", strRGB, Name);
						return false;
					}

					Color c = Color555.CalcColor(nRGB);
					if (nColors < 256)
						m_mapId2Color.Add(nColors, c);
					nColors++;
				}
			}

			if (nColors != 256)
			{
				m_doc.ErrorString("Incorrect number of colors in background image palette");
				return false;
			}
			return true;
		}

		public bool LoadXML_bgimagedata(XmlNode xnode, int nWidth, int nHeight)
		{
			int nRow = 0;
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "bgimagerow")
				{
					if (nRow < nHeight)
					{
						MatchCollection matches = Regex.Matches(xn.InnerText, @"([0-9a-zA-Z]{2}),?");
						int nColumn = 0;
						foreach (Match m in matches)
						{
							if (nColumn < nWidth)
							{
								string strPaletteIndex = m.Groups[1].ToString();
								int nPaletteIndex = XMLUtils.ParseHexInteger(strPaletteIndex);
								if (nPaletteIndex < 256)
									m_ImageData[nColumn, nRow] = (byte)nPaletteIndex;
							}
							nColumn++;
						}
						if (nColumn != nWidth)
						{
							m_doc.ErrorString("Incorrect number of columns in row {0} background image", nRow);
							return false;
						}
					}
					nRow++;
				}
			}

			if (nRow != nHeight)
			{
				m_doc.ErrorString("Incorrect number of rows in background image");
				return false;
			}
			return true;
		}

		#endregion

		#region Save

		public void Save(System.IO.TextWriter tw)
		{
			tw.Write("\t\t<bgimage_pal8");
			tw.Write(String.Format(" name=\"{0}\"", Name));
			tw.Write(String.Format(" id=\"{0}\"", ExportId));
			tw.Write(String.Format(" desc=\"{0}\"", Description));
			tw.Write(" size=\"{0}x{1}\"", m_width, m_height);
			tw.WriteLine(">");

			tw.Write("\t\t\t<palette256");
			tw.Write(String.Format(" name=\"{0}\"", Name));
			tw.Write(String.Format(" id=\"{0}\"", ExportId));
			tw.WriteLine(">");

			int nPerLine = 8;
			string strIndent = "\t\t\t\t";
			StringBuilder sb = null;

			for (short i = 0; i < 256; i++)
			{
				if ((i % nPerLine) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder(strIndent);
				}
				Color c = m_mapId2Color[i];
				int r, g, b;
				Color555.ExtractColors(c, out r, out g, out b);
				sb.Append(String.Format("<color rgb=\"{0:x2}{1:x2}{2:x2}\"/>", r, g, b));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
			
			tw.WriteLine("\t\t\t</palette256>");

			tw.Write("\t\t\t<bgimagedata");
			tw.Write(String.Format(" id=\"{0}\"", m_id));
			tw.WriteLine(">");

			for (int iy = 0; iy < m_height; iy++)
			{
				sb = new StringBuilder(strIndent);
				sb.Append("<bgimagerow>");
				for (int ix = 0; ix < m_width - 1; ix++)
				{
					sb.Append(String.Format("{0:x2},", m_ImageData[ix, iy]));
				}
				sb.Append(String.Format("{0:x2}", m_ImageData[m_width-1, iy]));
				sb.Append("</bgimagerow>");
				tw.WriteLine(sb.ToString());
			}

			tw.WriteLine("\t\t\t</bgimagedata>");

			tw.WriteLine("\t\t</bgimage_pal8>");
		}

		#endregion

		#region Export

		private int m_nExportId;

		public int ExportId
		{
			get { return m_nExportId; }
		}

		public void Export_AssignIDs(int nExportId)
		{
			m_nExportId = nExportId;
		}

		/// <summary>
		/// Export the 256-color palette for this image.
		/// </summary>
		/// <param name="tw"></param>
		public void Export_BgImagePaletteData(System.IO.TextWriter tw)
		{
			StringBuilder sb = null;
			int nPerLine = 8;

			for (short i = 0; i < 256; i++)
			{
				if ((i % nPerLine) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder("\t");
				}
				sb.Append(String.Format("0x{0:x4},", Color555.Encode(m_mapId2Color[i])));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
		}

		/// <summary>
		/// Export the image as an array of 8-bit palette indices.
		/// </summary>
		/// <param name="tw"></param>
		public void Export_BgImageData_Paletted(System.IO.TextWriter tw)
		{
			StringBuilder sb = null;
			int nPerLine = 32;

			for (int iy = 0; iy < m_height; iy++)
			{
				tw.WriteLine("\t// Row {0}", iy);
				for (int ix = 0; ix < m_width; ix++)
				{
					if ((ix % nPerLine) == 0)
					{
						if (sb != null)
							tw.WriteLine(sb.ToString());
						sb = new StringBuilder("\t");
					}
					sb.Append(String.Format("0x{0:x2},", m_ImageData[ix, iy]));
				}
				if (sb != null)
					tw.WriteLine(sb.ToString());
				sb = null;
			}
		}

		/// <summary>
		/// Export the image as an array of direct 16-bit color values.
		/// </summary>
		/// <param name="tw"></param>
		public void Export_BgImageData_Direct(System.IO.TextWriter tw)
		{
			StringBuilder sb = null;
			int nPerLine = 32;

			for (int iy = 0; iy < m_height; iy++)
			{
				tw.WriteLine("\t// Row {0}", iy);
				for (int ix = 0; ix < m_width; ix++)
				{
					if ((ix % nPerLine) == 0)
					{
						if (sb != null)
							tw.WriteLine(sb.ToString());
						sb = new StringBuilder("\t");
					}
					short palette_index = m_ImageData[ix, iy];
					Color c = m_mapId2Color[palette_index];
					int encoded = Color555.Encode(c);
					sb.Append(String.Format("0x{0:x4},", encoded));
				}
				if (sb != null)
					tw.WriteLine(sb.ToString());
				sb = null;
			}
		}

		#endregion

	}
}
 