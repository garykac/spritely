using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public partial class Sprite
	{
		#region Load

		public bool LoadXML_sprite16(XmlNode xnode, int nFirstTileId)
		{
			int nTileIndex = 0;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "tile")
				{
					if (nTileIndex < NumTiles)
					{
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
						if (id != (nFirstTileId + nTileIndex))
						{
							m_doc.ErrorString("Out of order tile id in sprite '{0}'. Expected {1}, found {2}",
									Name, nFirstTileId + nTileIndex, id);
							return false;
						}

						if (!LoadXML_tile(xn, nTileIndex))
						{
							// Error message already displayed
							return false;
						}
					}

					nTileIndex++;
				}
			}

			if (nTileIndex != NumTiles)
			{
				// TODO: error message
				m_doc.ErrorString("Incorrect number of tiles in sprite '{0}'", Name);
				return false;
			}
			return true;
		}

		public bool LoadXML_tile(XmlNode xnode, int nTileIndex)
		{
			int[] bTile = new int[64];
			int nCurrRow = 0;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "tilerow")
				{
					if (nCurrRow < 8)
					{
						Regex rxTile = new Regex(@"^(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),(\d+)$");
						Match mxTile = rxTile.Match(xn.InnerText);
						if (mxTile.Success)
						{
							GroupCollection matchGroups = mxTile.Groups;
							int nBase = nCurrRow * 8;
							for (int i = 0; i < 8; i++)
								bTile[nBase + i] = XMLUtils.ParseInteger(matchGroups[i + 1].Value);
						}
						else
						{
							m_doc.ErrorString("Unable to parse tiledata for row {0} of tile {1} in sprite '{2}'", nCurrRow, nTileIndex, Name);
							return false;
						}
					}
					nCurrRow++;
				}
			}

			if (nCurrRow != 8)
			{
				m_doc.ErrorString("Incorrect number of rows in tile {0} of sprite '{1}'", nTileIndex, Name);
				return false;
			}

			// After we've read 8 rows (64 pixels), import the tile into the sprite.
			if (!ImportTile(nTileIndex, bTile))
			{
				// Warning/Error message already displayed.
				return false;
			}

			return true;
		}

		#endregion

		#region Save

		public void Save(System.IO.TextWriter tw, int nExportSpriteID, int nExportFirstTileID)
		{
			// Record the export ID and the first tile ID so that the other export routines can use it.
			m_nExportSpriteID = nExportSpriteID;
			m_nExportFirstTileID = nExportFirstTileID;

			tw.Write("\t\t\t<sprite16");
			tw.Write(String.Format(" name=\"{0}\"", m_strName));
			tw.Write(String.Format(" id=\"{0}\"", nExportSpriteID));
			tw.Write(String.Format(" desc=\"{0}\"", m_strDesc));
			tw.Write(String.Format(" size=\"{0}x{1}\"", m_tileWidth, m_tileHeight));
			tw.Write(String.Format(" subpalette_id=\"{0}\"", SubpaletteID));
			tw.WriteLine(">");

			int nTileID = ExportFirstTileId;
			foreach (Tile t in m_Tiles)
				t.Save(tw, nTileID++);

			tw.WriteLine("\t\t\t</sprite16>");
		}

		#endregion

		#region Export

		public void Export_AssignIDs(int nSpriteExportID, int nFirstTileID, int nMaskIndex)
		{
			// Record the export ID and the first tile ID so that the other export routines can use it.
			m_nExportSpriteID = nSpriteExportID;
			m_nExportFirstTileID = nFirstTileID;
			m_nMaskIndex = nMaskIndex;

			int nTileId = nFirstTileID;
			for (int i = 0; i < m_Tiles.Length; i++)
			{
				m_Tiles[i].Export_AssignIDs(nTileId++);
			}
		}

		public void Export_SpriteIDs(System.IO.TextWriter tw, string strSpriteset)
		{
			tw.WriteLine(String.Format("const int k{0}_{1} = {2};", strSpriteset, m_strName, m_nExportSpriteID));
		}

		public void Export_TileIDs(System.IO.TextWriter tw, string strSpritesetName)
		{
			int nExportId = m_nExportFirstTileID;
			for (int i = 0; i < m_Tiles.Length; i++)
			{
				m_Tiles[i].Export_TileIDs(tw, strSpritesetName, m_strName, i, m_Tiles.Length);
			}
		}

		public void Export_SpriteInfo(System.IO.TextWriter tw, GBASize size, GBAShape shape)
		{
			if (tw == null)
				return;

			string strShape = "INVALID";
			switch (shape)
			{
				case GBAShape.Square: strShape = "ATTR0_SQUARE"; break;
				case GBAShape.Wide: strShape = "ATTR0_WIDE"; break;
				case GBAShape.Tall: strShape = "ATTR0_TALL"; break;
			}

			string strSize = "INVALID";
			switch (size)
			{
				case GBASize.Size8: strSize = "ATTR1_SIZE_8"; break;
				case GBASize.Size16: strSize = "ATTR1_SIZE_16"; break;
				case GBASize.Size32: strSize = "ATTR1_SIZE_32"; break;
				case GBASize.Size64: strSize = "ATTR1_SIZE_64"; break;
			}

			tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4},{3,4},{4,4},{5,4},{6,16},{7,16} }}, // Sprite_{8}",
				m_nExportFirstTileID, NumTiles,
				PixelWidth, PixelHeight, SubpaletteID,
				m_nMaskIndex,
				strShape, strSize, m_strName));
		}

		public void Export_TileData(System.IO.TextWriter tw)
		{
			if (m_nExportSpriteID != 0)
				tw.WriteLine("");

			tw.WriteLine("\t// Sprite : " + m_strName);
			if (m_strDesc != "")
				tw.WriteLine("\t// Description : " + m_strDesc);
			tw.WriteLine(String.Format("\t// Size : {0}x{1} = {2} tiles", TileWidth, TileHeight, NumTiles));

			int nIndex = m_nExportFirstTileID;
			foreach (Tile t in m_Tiles)
				t.Export_TileData(tw, nIndex++);
		}

		const int MaskWordWidth = 32;

		public int CalcMaskSize()
		{
			int xsize = ((PixelWidth + MaskWordWidth - 1) / MaskWordWidth);
			int ysize = PixelHeight;
			return xsize * ysize;
		}

		public void Export_SpriteMaskData(System.IO.TextWriter tw)
		{
			if (m_nExportSpriteID != 0)
				tw.WriteLine("");

			tw.WriteLine("\t// Sprite : " + m_strName);

			// Calc # of int32s required.
			int xsize = ((PixelWidth + MaskWordWidth - 1) / MaskWordWidth);
			int ysize = PixelHeight;
			int size = xsize * ysize;
			int mask;

			for (int y = 0; y < ysize; y++)
			{
				for (int x = 0; x < xsize; x++)
				{
					mask = 0;
					for (int i = 0; i < MaskWordWidth; i++)
					{
						mask <<= 1;
						if (x * MaskWordWidth + i < PixelWidth)
						{
							if (GetPixel(x * MaskWordWidth + i, y) != 0)
								mask |= 1;
						}
					}
					tw.WriteLine(String.Format("\t0x{0:x8},", mask));
				}
			}
		}

		#endregion
	}

	#region Tests

	[TestFixture]
	public class Sprite_LoadSave_Test
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
		public void Test_LoadXML_sprite16_1x1()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			// Note: <sprite16> attributes not needed for test.
			Test_LoadXML_sprite16_AddTile(xnSprite, 0, 8, 8);
			Assert.IsTrue(s.LoadXML_sprite16(xnSprite, 0));

			// Verify the data is the same as the XML
			// Pixel 0 is the same as the tileid
			Assert.AreEqual(0, s.GetTile(0).GetPixel(0, 0));
			// Other pixels = x+y
			Assert.AreEqual(9, s.GetTile(0).GetPixel(4, 5));
			Assert.AreEqual(14, s.GetTile(0).GetPixel(7, 7));
		}

		[Test]
		public void Test_LoadXML_sprite16_2x4()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);

			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(2, 4, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			for (int i = 0; i < 8; i++)
				Test_LoadXML_sprite16_AddTile(xnSprite, i, 8, 8);
			Assert.IsTrue(s.LoadXML_sprite16(xnSprite, 0));

			// Verify the data is the same as the XML
			// Pixel 0 is the same as the tileid
			Assert.AreEqual(0, s.GetTile(0).GetPixel(0, 0));
			Assert.AreEqual(3, s.GetTile(3).GetPixel(0, 0));
			Assert.AreEqual(7, s.GetTile(7).GetPixel(0, 0));
			// Other pixels = x+y
			Assert.AreEqual(9, s.GetTile(0).GetPixel(4, 5));
			Assert.AreEqual(3, s.GetTile(4).GetPixel(0, 3));
			Assert.AreEqual(14, s.GetTile(7).GetPixel(7, 7));
		}

		[Test]
		public void Test_LoadXML_sprite16_1x1_2tiles()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			for (int i = 0; i < 2; i++)
				Test_LoadXML_sprite16_AddTile(xnSprite, i, 8, 8);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		[Test]
		public void Test_LoadXML_sprite16_2x2_3tiles()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(2, 2, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			for (int i = 0; i < 3; i++)
				Test_LoadXML_sprite16_AddTile(xnSprite, i, 8, 8);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		[Test]
		public void Test_LoadXML_sprite16_1x1_7tilerows()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			Test_LoadXML_sprite16_AddTile(xnSprite, 0, 8, 7);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		[Test]
		public void Test_LoadXML_sprite16_1x1_9tilerows()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			Test_LoadXML_sprite16_AddTile(xnSprite, 0, 8, 9);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		[Test]
		public void Test_LoadXML_sprite16_1x1_7columns()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			Test_LoadXML_sprite16_AddTile(xnSprite, 0, 7, 8);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		[Test]
		public void Test_LoadXML_sprite16_1x1_9columns()
		{
			Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(p);
			
			Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			Assert.IsNotNull(ss);
			
			Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);

			XmlElement xnSprite = m_xd.CreateElement("sprite16");
			Test_LoadXML_sprite16_AddTile(xnSprite, 0, 9, 8);
			Assert.IsFalse(s.LoadXML_sprite16(xnSprite, 0));
		}

		private void Test_LoadXML_sprite16_AddTile(XmlElement xnSprite, int nTileId, int nColumns, int nRows)
		{
			XmlElement xnTile = m_xd.CreateElement("tile");
			xnTile.SetAttribute("id", nTileId.ToString());

			for (int y = 0; y < nRows; y++)
			{
				XmlElement xnTilerow = m_xd.CreateElement("tilerow");
				for (int x = 0; x < nColumns; x++)
				{
					int t = x + y;
					if (x == 0 && y == 0)
						t = nTileId;

					xnTilerow.InnerText += String.Format("{0}{1}", t, (x == nColumns - 1 ? "" : ","));
				}
				xnTile.AppendChild(xnTilerow);
			}
			xnSprite.AppendChild(xnTile);
		}

	}

	#endregion
}
