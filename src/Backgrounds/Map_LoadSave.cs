using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;

namespace Spritely
{
	public partial class Map
	{
		#region Load

		public bool LoadXML_map16(XmlNode xnode, int nDefaultSubpaletteId)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "map16block")
				{
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
					if (!LoadXML_map16block(xn, id, nDefaultSubpaletteId))
						return false;
				}
			}

			return true;
		}

		public bool LoadXML_map16block(XmlNode xnode, int id, int nDefaultSubpaletteId)
		{
			int nRow = 0;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "map16row")
				{
					if (nRow < 32)
					{
						if (!LoadXML_map16row(xn, nRow, nDefaultSubpaletteId))
							return false;
					}
					nRow++;
				}
			}

			if (nRow != 32)
			{
				m_doc.ErrorString("Incorrect number of rows in mapblock {0} of map '{1}'", id, Name);
				return false;
			}

			return true;
		}

		public bool LoadXML_map16row(XmlNode xnode, int nRow, int nDefaultSubpaletteId)
		{
			int nColumn = 0;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "map16tile")
				{
					if (nColumn < 32)
					{
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "tile_id");
						int nSubpalette = nDefaultSubpaletteId;
						if (XMLUtils.HasXMLAttribute(xn, "subpalette_id"))
							nSubpalette = XMLUtils.GetXMLIntegerAttribute(xn, "subpalette_id");
						string strFlip = XMLUtils.GetXMLAttribute(xn, "flip");

						m_BackgroundMap[nColumn, nRow].nTileIndex = id;
						m_BackgroundMap[nColumn, nRow].nSubpalette = nSubpalette;
						m_BackgroundMap[nColumn, nRow].fHFlip = strFlip.Contains("h");
						m_BackgroundMap[nColumn, nRow].fVFlip = strFlip.Contains("v");
					}
					nColumn++;
				}
			}

			if (nColumn != 32)
			{
				m_doc.ErrorString("Incorrect number of tiles in maprow {0} of map '{1}'", nRow, Name);
				return false;
			}

			return true;
		}

		#endregion

		#region Save

		public void Save(System.IO.TextWriter tw)
		{
			int nDefaultSubpalette = 0;

			tw.Write("\t\t<map16");
			tw.Write(String.Format(" name=\"{0}\"", m_strName));
			tw.Write(String.Format(" id=\"{0}\"", m_id));
			tw.Write(String.Format(" desc=\"{0}\"", m_strDesc));
			tw.Write(" size=\"32x32\"");
			tw.Write(String.Format(" bgspriteset_id=\"{0}\"", m_ss.Id));
			tw.Write(String.Format(" default_subpalette_id=\"{0}\"", nDefaultSubpalette));
			tw.WriteLine(">");
			tw.WriteLine("\t\t\t<map16block id=\"0\">");

			for (int iy = 0; iy < kMaxMapTilesY; iy++)
			{
				tw.WriteLine(String.Format("\t\t\t\t<map16row row=\"{0}\">", iy));

				StringBuilder sb = null;
				int nPerLine = 8;
				for (int ix = 0; ix < kMaxMapTilesX; ix++)
				{
					if (ix % nPerLine == 0)
					{
						if (sb != null)
							tw.WriteLine(sb.ToString());
						sb = new StringBuilder("\t\t\t\t\t");
					}

					int nTileExportId = m_ss.GetTileExportId(m_BackgroundMap[ix, iy].nTileIndex);

					sb.Append(String.Format("<map16tile tile_id=\"{0}\"", nTileExportId));
					if (m_BackgroundMap[ix, iy].nSubpalette != nDefaultSubpalette)
						sb.Append(String.Format(" subpalette_id=\"{1}\"", m_BackgroundMap[ix, iy].nSubpalette));
					// TODO: add flip h,v values
					sb.Append("/>");
				}
				if (sb != null)
					tw.WriteLine(sb.ToString());

				tw.WriteLine("\t\t\t\t</map16row>");
			}

			tw.WriteLine("\t\t\t</map16block>");
			tw.WriteLine("\t\t</map16>");
		}

		#endregion

		#region Export

		private int m_nExportId;

		public int ExportId
		{
			get { return m_nExportId; }
		}

		public void Export_AssignIDs(int nMapExportId)
		{
			m_nExportId = nMapExportId;
		}

		public void Export_BackgroundMap(System.IO.TextWriter tw)
		{
			for (int iy = 0; iy < kMaxMapTilesY; iy++)
			{
				StringBuilder sb = new StringBuilder();
				for (int ix = 0; ix < kMaxMapTilesX; ix++)
				{
					int nTileId = m_ss.GetTileExportId(m_BackgroundMap[ix, iy].nTileIndex);
					int nSubpaletteId = m_BackgroundMap[ix, iy].nSubpalette;
					//TODO: add support for h/v flipped tiles
					int nMap = (nSubpaletteId << 12) | nTileId;
					sb.Append(String.Format("0x{0:x4},", nMap));
				}
				tw.WriteLine("\t{0}", sb.ToString());
			}
		}

		#endregion

	}

	#region Tests

	[TestFixture]
	public class Map_LoadSave_Test
	{
		Document m_doc;
		XmlDocument m_xd;

		[SetUp]
		public void TestInit()
		{
			m_doc = new Document(null);
			m_xd = new XmlDocument();
		}

		[Test]
		public void Test_LoadXML_map16_32x32()
		{
			Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Map m = m_doc.BackgroundMaps.AddMap("map", 0, "", ss);
			Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
			Assert.IsNotNull(m);

			// Valid map.
			XmlElement xnMap = m_xd.CreateElement("map16");
			Test_LoadXML_map16_AddBlock(xnMap, 0, 32, 32);
			// <map16> attributes are not needed for test.
			Assert.IsTrue(m.LoadXML_map16(xnMap, 0));

			int nTileId, nSubpaletteId;
			bool fHorizontal, fVertical;

			m.GetTile(2, 4, out nTileId, out nSubpaletteId);
			m.GetFlip(2, 4, out fHorizontal, out fVertical);
			Assert.AreEqual(6, nTileId);
			Assert.AreEqual(0, nSubpaletteId);
			
			m.GetTile(7, 9, out nTileId, out nSubpaletteId);
			Assert.AreEqual(16, nTileId);
			Assert.AreEqual(0, nSubpaletteId);
			
			m.GetTile(31, 31, out nTileId, out nSubpaletteId);
			Assert.AreEqual(62, nTileId);
			Assert.AreEqual(1, nSubpaletteId);	// x==y, so subpalette=1

			m.GetFlip(0, 0, out fHorizontal, out fVertical);
			Assert.IsTrue(fHorizontal);
			Assert.IsTrue(fVertical);

			m.GetFlip(0, 5, out fHorizontal, out fVertical);
			Assert.IsFalse(fHorizontal);
			Assert.IsTrue(fVertical);

			m.GetFlip(5, 0, out fHorizontal, out fVertical);
			Assert.IsTrue(fHorizontal);
			Assert.IsFalse(fVertical);

			m.GetFlip(5, 5, out fHorizontal, out fVertical);
			Assert.IsFalse(fHorizontal);
			Assert.IsFalse(fVertical);
		}

		[Test]
		public void Test_LoadXML_map16_32x32_31rows()
		{
			Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			Assert.IsNotNull(ss);

			Map m = m_doc.BackgroundMaps.AddMap("map", 0, "", ss);
			Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
			Assert.IsNotNull(m);

			// Valid map.
			XmlElement xnMap = m_xd.CreateElement("map16");
			Test_LoadXML_map16_AddBlock(xnMap, 0, 32, 31);
			Assert.IsFalse(m.LoadXML_map16(xnMap, 0));
		}

		[Test]
		public void Test_LoadXML_map16_32x32_33rows()
		{
			Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			Assert.IsNotNull(ss);

			Map m = m_doc.BackgroundMaps.AddMap("map", 0, "", ss);
			Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
			Assert.IsNotNull(m);

			// Valid map.
			XmlElement xnMap = m_xd.CreateElement("map16");
			Test_LoadXML_map16_AddBlock(xnMap, 0, 32, 33);
			Assert.IsFalse(m.LoadXML_map16(xnMap, 0));
		}

		[Test]
		public void Test_LoadXML_map16_32x32_31columns()
		{
			Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			Assert.IsNotNull(ss);

			Map m = m_doc.BackgroundMaps.AddMap("map", 0, "", ss);
			Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
			Assert.IsNotNull(m);

			// Valid map.
			XmlElement xnMap = m_xd.CreateElement("map16");
			Test_LoadXML_map16_AddBlock(xnMap, 0, 31, 32);
			Assert.IsFalse(m.LoadXML_map16(xnMap, 0));
		}

		[Test]
		public void Test_LoadXML_map16_32x32_33columns()
		{
			Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			Assert.IsNotNull(ss);

			Map m = m_doc.BackgroundMaps.AddMap("map", 0, "", ss);
			Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
			Assert.IsNotNull(m);

			// Valid map.
			XmlElement xnMap = m_xd.CreateElement("map16");
			Test_LoadXML_map16_AddBlock(xnMap, 0, 33, 32);
			Assert.IsFalse(m.LoadXML_map16(xnMap, 0));
		}

		private void Test_LoadXML_map16_AddBlock(XmlElement xnMap, int id, int nColumns, int nRows)
		{
			XmlElement xnBlock = m_xd.CreateElement("map16block");
			xnBlock.SetAttribute("id", id.ToString());
			for (int y = 0; y < nRows; y++)
			{
				XmlElement xnRow = m_xd.CreateElement("map16row");
				xnRow.SetAttribute("row", y.ToString());
				for (int x = 0; x < nColumns; x++)
				{
					XmlElement xnTile = m_xd.CreateElement("map16tile");
					xnTile.SetAttribute("tile_id", (x+y).ToString());

					// Override the default subpalette id for some tiles.
					if (x == y)
						xnTile.SetAttribute("subpalette_id", "1");

					// Flip some of the tiles.
					if (x == 0 && y == 0)
						xnTile.SetAttribute("flip", "hv");
					else if (x == 0)
						xnTile.SetAttribute("flip", "v");
					else if (y == 0)
						xnTile.SetAttribute("flip", "h");

					xnRow.AppendChild(xnTile);
				}
				xnBlock.AppendChild(xnRow);
			}
			xnMap.AppendChild(xnBlock);
		}

	}

	#endregion
}
