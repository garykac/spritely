using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public partial class FileHandler
	{
		private bool LoadFile(string strFile)
		{
			XmlTextReader xr = new XmlTextReader(strFile);
			XmlDocument xd = new XmlDocument();
			try
			{
				xd.Load(xr);
			}
			catch (Exception ex)
			{
				// "An exception was thrown while parsing the XML file: {0}"
				m_doc.ErrorId("ExceptionParseXML", ex.Message);
				return false;
			}
			xr.Close();

			return LoadXML(xd.ChildNodes);
		}

		bool m_fFoundSpritePalettes;
		bool m_fFoundSprites;
		bool m_fFoundBackgroundPalettes;
		bool m_fFoundBackgroundSprites;
		bool m_fFoundBackgroundMap;

		private bool LoadXML(XmlNodeList xnl)
		{
			m_fFoundSpritePalettes = false;
			m_fFoundSprites = false;
			m_fFoundBackgroundPalettes = false;
			m_fFoundBackgroundSprites = false;
			m_fFoundBackgroundMap = false;

			foreach (XmlNode xn in xnl)
			{
				// Old version 1 file format.
				if (xn.Name == "gba_tileset"
					// Obsolete name for gba_tileset.
					// No longer used - included for backwards compatibility.
					|| xn.Name == "gba_sprite_collection"
					)
				{
					if (!LoadXML_OLD_gba_tileset(xn.ChildNodes))
						return false;
				}

				// Version 2 files.
				if (xn.Name == "spritely")
				{
					if (!LoadXML_spritely(xn))
						return false;
				}
			}

			if (!m_fFoundSpritePalettes)
			{
				Palette16 pal = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				pal.SetDefaultPalette();
			}
			if (!m_fFoundSprites)
			{
				Palette pal = m_doc.Palettes.GetPalette(0);
				m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, Options.DefaultPaletteId, "", pal);
			}
			if (!m_fFoundBackgroundPalettes)
			{
				Palette16 bgpal = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				bgpal.SetDefaultPalette();
			}
			if (!m_fFoundBackgroundSprites)
			{
				Palette bgpal = m_doc.BackgroundPalettes.GetPalette(0);
				m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgPaletteName, Options.DefaultBgPaletteId, "", bgpal);
				m_doc.BackgroundSpritesets.Current.AddSprite(1, 1, "", -1, "", 0, null);
			}
			if (!m_fFoundBackgroundMap)
			{
				m_doc.BackgroundMaps.AddMap("map", 0, "", m_doc.BackgroundSpritesets.Current);
			}

			// Remove any UndoActions since we just loaded from a file.
			m_doc.Owner.ClearUndo();
			m_doc.RecordSnapshot();
			return true;
		}

		private bool LoadXML_spritely(XmlNode xnode)
		{
			int nVersion = XMLUtils.GetXMLIntegerAttribute(xnode, "version");
			string strName = XMLUtils.GetXMLAttribute(xnode, "name");

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "options":
						if (!Options.LoadXML_options(xn))
							return false;
						break;
					case "palettes":
						if (!m_doc.Palettes.LoadXML_palettes(xn))
							return false;
						m_fFoundSpritePalettes = true;
						break;
					case "spritesets":
						if (!m_doc.Spritesets.LoadXML_spritesets(xn))
							return false;
						m_fFoundSprites = true;
						break;
					case "bgpalettes":
						if (!m_doc.BackgroundPalettes.LoadXML_palettes(xn))
							return false;
						m_fFoundBackgroundPalettes = true;
						break;
					case "bgspritesets":
						if (!m_doc.BackgroundSpritesets.LoadXML_spritesets(xn))
							return false;
						m_fFoundBackgroundSprites = true;
						break;
					case "bgmaps":
						if (!m_doc.BackgroundMaps.LoadXML_bgmaps(xn))
							return false;
						m_fFoundBackgroundMap = true;
						break;
					case "bgimages":
						if (!m_doc.BackgroundImages.LoadXML_bgimages(xn))
							return false;
						break;
				}
			}
			return true;
		}

		#region Load Old XML

		private bool LoadXML_OLD_gba_tileset(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				switch (xn.Name)
				{
					case "palettes":
						if (!LoadXML_OLD_palettes(xn.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName))
							return false;
						m_fFoundSpritePalettes = true;
						break;
					case "sprites":
						if (!LoadXML_OLD_sprites(xn.ChildNodes, false))
							return false;
						m_fFoundSprites = true;
						break;
					case "bgpalettes":
						if (!LoadXML_OLD_palettes(xn.ChildNodes, m_doc.BackgroundPalettes, Options.DefaultBgPaletteName))
							return false;
						m_fFoundBackgroundPalettes = true;
						break;
					case "bgsprites":
						if (!LoadXML_OLD_sprites(xn.ChildNodes, true))
							return false;
						m_fFoundBackgroundSprites = true;
						break;
					case "map":
						if (!LoadXML_OLD_map(xn.ChildNodes))
							return false;
						m_fFoundBackgroundMap = true;
						break;
				}
			}
			return true;
		}

		private bool LoadXML_OLD_palettes(XmlNodeList xnl, Palettes palettes, string strName)
		{
			Palette16 palette = palettes.AddPalette16(strName, 0, "");
			int nSubpalette = 0;

			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "palette")
				{
					int nSubpaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "id");
					if (nSubpalette > 16)
					{
						// Incorrect number of subpalettes - too many.
						m_doc.ErrorString("Too many subpalettes specified for palette (expected 16)");
						return false;
					}
					if (nSubpaletteId != nSubpalette)
					{
						// Subpalettes out of order.
						m_doc.ErrorString("Subpalette ids are out of order");
						return false;
					}

					int[] anPalette = new int[16];
					int nCurrEntry = 0;

					Regex rxPalette = new Regex(@"\s*0x([0-9A-Fa-f]{4})\s*,");
					Match mxPalette = rxPalette.Match(xn.InnerText);
					if (mxPalette.Success)
					{
						while (mxPalette.Success)
						{
							GroupCollection matchGroups = mxPalette.Groups;

							if (nCurrEntry >= 16)
							{
								// Wrong number of colors in palette - too many.
								m_doc.ErrorString("Too many colors specified for subpalette {0} (expected 16)", nSubpaletteId);
								return false;
							}

							anPalette[nCurrEntry] = Convert.ToInt32(matchGroups[1].Value, 16);
							nCurrEntry++;
							if (nCurrEntry == 16)
							{
								// After we've read 16 colors, update the subpalette.
								if (!palette.ImportSubpalette(nSubpaletteId, anPalette))
								{
									// Warning/Error message already displayed.
									return false;
								}

								// Since we just loaded from a file, update the snapshot without creating an UndoAction
								palette.GetSubpalette(nSubpaletteId).RecordSnapshot();
							}

							// Get the next match.
							mxPalette = mxPalette.NextMatch();
						}

						if (nCurrEntry != 16)
						{
							// "Wrong number of colors in palette with ID='{0}'. Found {1}, expected 16."
							m_doc.ErrorId("ErrorNumColorsInPalette", nSubpaletteId, nCurrEntry);
							return false;
						}
					}
					nSubpalette++;
				}
			}

			if (nSubpalette != 16)
			{
				// Incorrect number of subpalettes - too few.
				m_doc.ErrorString("Too few subpalettes (expected 16)");
				return false;
			}

			return true;
		}

		private bool LoadXML_OLD_sprites(XmlNodeList xnl, bool fBackground)
		{
			Palette p;
			Spriteset ts;
			if (fBackground)
			{
				p = m_doc.GetBackgroundPalette(0);
				ts = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
			}
			else
			{
				p = m_doc.GetSpritePalette(0);
				ts = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
			}
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "sprite")
				{
					string strName = XMLUtils.GetXMLAttribute(xn, "name");
					string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
					int nSubpaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "palette");
					string strSize = XMLUtils.GetXMLAttribute(xn, "size");
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
					int nFirstTileID = XMLUtils.GetXMLIntegerAttribute(xn, "firsttileid");
					string[] aSize = strSize.Split('x');
					int nWidth = XMLUtils.ParseInteger(aSize[0]);
					int nHeight = XMLUtils.ParseInteger(aSize[1]);

					Sprite s = ts.AddSprite(nWidth, nHeight, strName, id, strDesc, nSubpaletteId, null);
					if (s == null)
					{
						// "Invalid sprite size ({0}) for sprite named '{1}'."
						m_doc.ErrorId("ErrorInvalidSpriteSize", strSize, strName);
						return false;
					}

					if (!LoadXML_OLD_sprite(s, xn.ChildNodes))
					{
						// Error message already displayed
						return false;
					}

					// Since we just loaded from a file, update the snapshot without creating an UndoAction
					s.RecordSnapshot();
				}
			}
			return true;
		}

		private bool LoadXML_OLD_sprite(Sprite s, XmlNodeList xnl)
		{
			int nNumTiles = s.NumTiles;
			int nTileIndex = 0;

			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "tile")
				{
					if (nTileIndex >= nNumTiles)
					{
						// too many tiles in sprite
						m_doc.ErrorString("Too many tiles specified for sprite '{0}' (expected {1})", s.Name, nNumTiles);
						return false;
					}
					uint[] bTile = new uint[32];
					int nCurrByte = 0;

					Regex rxTile = new Regex(@"\s*0x([0-9A-Fa-f]{2})\s*,");
					Match mxTile = rxTile.Match(xn.InnerText);
					if (mxTile.Success)
					{
						while (mxTile.Success)
						{
							GroupCollection matchGroups = mxTile.Groups;

							if (nCurrByte >= 32)
							{
								// too many data bytes in tile
								m_doc.ErrorString("Too many data bytes in tile #{0} of sprite '{0}'", nTileIndex, s.Name);
								return false;
							}
							bTile[nCurrByte] = Convert.ToUInt32(matchGroups[1].Value, 16);
							nCurrByte++;
							if (nCurrByte == 32)
							{
								// After we've read 32 bytes, import the tile into the sprite.
								if (!s.ImportTile32(nTileIndex, bTile))
								{
									// Warning/Error message already displayed.
									return false;
								}
								nTileIndex++;
							}

							// Get the next match.
							mxTile = mxTile.NextMatch();
						}
					}

					if (nCurrByte != 32)
					{
						// too few data bytes for tile
						m_doc.ErrorString("Too few data bytes in tile #{0} of sprite '{0}'", nTileIndex, s.Name);
						return false;
					}
				}
			}

			if (nTileIndex != s.NumTiles)
			{
				// incorrect number of tiles for this sprite - too few
				m_doc.ErrorString("Too few tiles specified for sprite '{0}' (expected {1})", s.Name, nNumTiles);
				return false;
			}
			return true;
		}

		private bool LoadXML_OLD_map(XmlNodeList xnl)
		{
			int x = 0;
			int y = 0;

			Map m = m_doc.BackgroundMaps.AddMap(Options.DefaultMapName, 0, "",
												m_doc.BackgroundSpritesets.GetSpriteset(0));
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "row")
				{
					if (y >= 32)
					{
						m_doc.ErrorString("Too many rows specified for map");
						return false;
					}

					x = 0;

					foreach (XmlNode xn2 in xn.ChildNodes)
					{
						int nTileID = XMLUtils.GetXMLIntegerAttribute(xn2, "tileid");
						int nPaletteID = 0; // = Int32.Parse(GetXMLAttribute(xn2, "palette"));
						if (x < 32 && y < 32)
							m.SetTile(x, y, nTileID, nPaletteID);
						x++;
					}

					if (x != 32)
					{
						m_doc.ErrorString("Incorrect number of tiles ({0}) in row {1} of map (expected 32 tiles)", x, y);
						return false;
					}

					y++;
				}
			}

			if (y != 32)
			{
				m_doc.ErrorString("Not enough rows specified for map");
				return false;
			}

			return true;
		}

		#endregion

		#region Tests

		[TestFixture]
		public class FileHandler_Load_Test
		{
			Document m_doc;
			FileHandler m_filer;
			XmlDocument m_xd;

			[SetUp]
			public void TestInit()
			{
				m_doc = new Document(null);
				m_filer = m_doc.Filer;
				m_xd = new XmlDocument();
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_valid()
			{
				// Valid old palette.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 16);
				Assert.IsTrue(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));

				Assert.AreEqual(1, m_doc.Palettes.NumPalettes);
				Palette16 p = m_doc.Palettes.GetPalette(0) as Palette16;
				Assert.IsNotNull(p);

				// Verify the colors are the same ones from the XML:
				// Color 0 is the same as the subpalette id
				Assert.AreEqual(0, p.GetSubpalette(0).Encoding(0));
				Assert.AreEqual(4, p.GetSubpalette(4).Encoding(0));
				// Other colors are the same as the color index
				Assert.AreEqual(3, p.GetSubpalette(0).Encoding(3));
				Assert.AreEqual(7, p.GetSubpalette(6).Encoding(7));
				Assert.AreEqual(12, p.GetSubpalette(8).Encoding(12));
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_out_of_order()
			{
				// Valid old palette.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 15; id >= 0; id--)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 16);
				Assert.IsFalse(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_15_subpalettes()
			{
				// Invalid - too few subpalettes.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 0; id < 15; id++)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 16);
				Assert.IsFalse(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_17_subpalettes()
			{
				// Invalid - too many subpalettes.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 0; id < 17; id++)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 16);
				Assert.IsFalse(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_15_colors()
			{
				// Invalid - too few subpalettes.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 15);
				Assert.IsFalse(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));
			}

			[Test]
			public void Test_LoadXML_OLD_palettes_17_colors()
			{
				// Invalid - too few subpalettes.
				XmlElement xnPalettes = m_xd.CreateElement("palettes");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_OLD_palettes_AddSubpalette(xnPalettes, id, 17);
				Assert.IsFalse(m_filer.LoadXML_OLD_palettes(xnPalettes.ChildNodes, m_doc.Palettes, Options.DefaultPaletteName));
			}

			private void Test_LoadXML_OLD_palettes_AddSubpalette(XmlElement xnPalettes, int id, int nColors)
			{
				XmlElement xnPalette = m_xd.CreateElement("palette");
				xnPalette.SetAttribute("id", id.ToString());
				xnPalette.InnerText += String.Format("0x{0:x4},", id);
				for (int i = 1; i < nColors; i++)
					xnPalette.InnerText += String.Format("0x{0:x4},", i);
				xnPalettes.AppendChild(xnPalette);
			}

			[Test]
			public void Test_LoadXML_OLD_sprite_1x1()
			{
				Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
				Assert.IsNotNull(ss);

				Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
				Assert.IsNotNull(s);

				XmlElement xnSprite = m_xd.CreateElement("sprite");
				Test_LoadXML_OLD_sprite_AddTile(xnSprite, 0);
				Assert.IsTrue(m_filer.LoadXML_OLD_sprite(s, xnSprite.ChildNodes));

				// Verify the data is the same as the XML
				// Pixel 0 is the same as the tileid
				Assert.AreEqual(0, s.GetTile(0).GetPixel(0, 0));
				// Other pixels = x+y
				Assert.AreEqual(3, s.GetTile(0).GetPixel(1, 2));
				Assert.AreEqual(14, s.GetTile(0).GetPixel(7, 7));
			}

			[Test]
			public void Test_LoadXML_OLD_sprite_2x4()
			{
				Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
				Assert.IsNotNull(ss);

				Sprite s = ss.AddSprite(2, 4, "sample", 0, "", 0, null);
				Assert.IsNotNull(s);

				XmlElement xnSprite = m_xd.CreateElement("sprite");
				for (int i = 0; i < 8; i++)
					Test_LoadXML_OLD_sprite_AddTile(xnSprite, i);
				Assert.IsTrue(m_filer.LoadXML_OLD_sprite(s, xnSprite.ChildNodes));

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
			public void Test_LoadXML_OLD_sprite_1x1_2tiles()
			{
				Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
				Assert.IsNotNull(ss);

				Sprite s = ss.AddSprite(1, 1, "sample", 0, "", 0, null);
				Assert.IsNotNull(s);

				XmlElement xnSprite = m_xd.CreateElement("sprite");
				for (int i = 0; i < 2; i++)
					Test_LoadXML_OLD_sprite_AddTile(xnSprite, i);
				Assert.IsFalse(m_filer.LoadXML_OLD_sprite(s, xnSprite.ChildNodes));
			}

			[Test]
			public void Test_LoadXML_OLD_sprite_2x2_3tiles()
			{
				Palette p = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", p);
				Assert.IsNotNull(ss);

				Sprite s = ss.AddSprite(2, 2, "sample", 0, "", 0, null);
				Assert.IsNotNull(s);

				XmlElement xnSprite = m_xd.CreateElement("sprite");
				for (int i = 0; i < 3; i++)
					Test_LoadXML_OLD_sprite_AddTile(xnSprite, i);
				Assert.IsFalse(m_filer.LoadXML_OLD_sprite(s, xnSprite.ChildNodes));
			}

			private void Test_LoadXML_OLD_sprite_AddTile(XmlElement xnSprite, int nTileId)
			{
				XmlElement xnTile = m_xd.CreateElement("tile");
				xnTile.SetAttribute("id", nTileId.ToString());

				for (int y = 0; y < 8; y++)
					for (int x = 0; x < 8; x++)
					{
						int b1 = x + y;
						if (x == 0 && y == 0)
							b1 = nTileId;
						x++;
						int b2 = x + y;
						xnTile.InnerText += String.Format("0x{0:x2},", b2 * 16 + b1);
					}
				xnSprite.AppendChild(xnTile);
			}

			[Test]
			public void Test_LoadXML_OLD_map_valid()
			{
				Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
				Assert.IsNotNull(ss);

				// Valid old map.
				XmlElement xnMap = m_xd.CreateElement("map");
				for (int id = 0; id < 32; id++)
					Test_LoadXML_OLD_map_AddRow(xnMap, id, 32);
				Assert.IsTrue(m_filer.LoadXML_OLD_map(xnMap.ChildNodes));

				Assert.AreEqual(1, m_doc.BackgroundMaps.NumMaps);
				Map m = m_doc.BackgroundMaps.GetMap(0);
				Assert.IsNotNull(m);

				int nTileId, nSubpaletteId;
				m.GetTile(2, 4, out nTileId, out nSubpaletteId);
				Assert.AreEqual(6, nTileId);
				m.GetTile(7, 9, out nTileId, out nSubpaletteId);
				Assert.AreEqual(16, nTileId);
				m.GetTile(31, 31, out nTileId, out nSubpaletteId);
				Assert.AreEqual(62, nTileId);
			}

			[Test]
			public void Test_LoadXML_OLD_map_20tiles()
			{
				Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
				Assert.IsNotNull(ss);

				// Map with only 20 tiles in each row
				XmlElement xnMap = m_xd.CreateElement("map");
				for (int id = 0; id < 32; id++)
					Test_LoadXML_OLD_map_AddRow(xnMap, id, 20);
				Assert.IsFalse(m_filer.LoadXML_OLD_map(xnMap.ChildNodes));
			}

			[Test]
			public void Test_LoadXML_OLD_map_35tiles()
			{
				Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
				Assert.IsNotNull(ss);

				// Map with 35 tiles in each row
				XmlElement xnMap = m_xd.CreateElement("map");
				for (int id = 0; id < 32; id++)
					Test_LoadXML_OLD_map_AddRow(xnMap, id, 35);
				Assert.IsFalse(m_filer.LoadXML_OLD_map(xnMap.ChildNodes));
			}

			[Test]
			public void Test_LoadXML_OLD_map_20rows()
			{
				Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
				Assert.IsNotNull(ss);

				// Map with only 20 rows
				XmlElement xnMap = m_xd.CreateElement("map");
				for (int id = 0; id < 20; id++)
					Test_LoadXML_OLD_map_AddRow(xnMap, id, 32);
				Assert.IsFalse(m_filer.LoadXML_OLD_map(xnMap.ChildNodes));
			}

			[Test]
			public void Test_LoadXML_OLD_map_35rows()
			{
				Palette p = m_doc.BackgroundPalettes.AddPalette16(Options.DefaultBgPaletteName, 0, "");
				Assert.IsNotNull(p);

				Spriteset ss = m_doc.BackgroundSpritesets.AddSpriteset(Options.DefaultBgTilesetName, 0, "", p);
				Assert.IsNotNull(ss);

				// Map with 35 rows
				XmlElement xnMap = m_xd.CreateElement("map");
				for (int id = 0; id < 35; id++)
					Test_LoadXML_OLD_map_AddRow(xnMap, id, 32);
				Assert.IsFalse(m_filer.LoadXML_OLD_map(xnMap.ChildNodes));
			}

			private void Test_LoadXML_OLD_map_AddRow(XmlElement xnMap, int id, int nTiles)
			{
				XmlElement xnRow = m_xd.CreateElement("row");
				xnRow.SetAttribute("y", id.ToString());
				for (int i = 0; i < nTiles; i++)
				{
					XmlElement xnTile = m_xd.CreateElement("bgtile");
					xnTile.SetAttribute("x", i.ToString());
					xnTile.SetAttribute("tileid", (id + i).ToString());
					xnRow.AppendChild(xnTile);
				}
				xnMap.AppendChild(xnRow);
			}

		}

		#endregion

	}
}
