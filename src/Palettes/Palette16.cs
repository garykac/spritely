using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public class Palette16 : Palette
	{
		/// <summary>
		/// Index of the current subpalette (16-color palettes only).
		/// </summary>
		public int CurrentSubpaletteId;

		private Palette16Form m_winPalette;

		private Subpalette[] m_subpalettes;

		public Palette16(Document doc, Palettes pals, string strName, int id, string strDesc) :
			base(doc, pals, strName, id, strDesc)
		{
			m_type = Palette.Type.Color16;
			m_nMaxSubpalettes = 16;

			m_subpalettes = new Subpalette[m_nMaxSubpalettes];
			for (int i = 0; i < m_nMaxSubpalettes; i++)
				m_subpalettes[i] = new Subpalette(doc, this, i, Subpalette.DefaultColorSet.BlackAndWhite);

			CurrentSubpaletteId = 0;

			if (m_doc.Owner != null)
				m_winPalette = new Palette16Form(m_doc.Owner, this);
		}

		public override void UpdateDocument(Document doc)
		{
			base.UpdateDocument(doc);

			for (int i = 0; i < m_nMaxSubpalettes; i++)
			{
				m_subpalettes[i].UpdateDocument(doc);
			}
		}

		public override void RecordSnapshot()
		{
			base.RecordSnapshot();

			for (int i = 0; i < m_nMaxSubpalettes; i++)
			{
				m_subpalettes[i].RecordSnapshot();
			}
		}

		public override IPaletteForm PaletteWindow()
		{
			return m_winPalette;
		}

		public override System.Windows.Forms.Form PaletteWindowForm()
		{
			return m_winPalette;
		}

		public override Subpalette GetCurrentSubpalette()
		{
			return m_subpalettes[CurrentSubpaletteId];
		}

		public override Subpalette GetSubpalette(int nIndex)
		{
			return m_subpalettes[nIndex];
		}

		/// <summary>
		/// Set the default subpalettes.
		/// </summary>
		public void SetDefaultPalette()
		{
			SetDefaultSubpaletteColors(0, Subpalette.DefaultColorSet.Color1);
			SetDefaultSubpaletteColors(1, Subpalette.DefaultColorSet.Color2);
			SetDefaultSubpaletteColors(2, Subpalette.DefaultColorSet.GrayScale);
			SetDefaultSubpaletteColors(3, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(4, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(5, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(6, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(7, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(8, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(9, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(10, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(11, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(12, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(13, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(14, Subpalette.DefaultColorSet.BlackAndWhite);
			SetDefaultSubpaletteColors(15, Subpalette.DefaultColorSet.BlackAndWhite);
		}

		public void SetDefaultSubpaletteColors(int nId, Subpalette.DefaultColorSet colorset)
		{
			if (nId < 0 || nId >= m_nMaxSubpalettes)
				return;
			m_subpalettes[nId].SetDefaultSubpaletteColors(colorset);
		}

		public bool ImportSubpalette(int nId, int[] anPalette)
		{
			if (nId < 0 || nId >= m_nMaxSubpalettes)
				return false;
			return m_subpalettes[nId].Import(anPalette);
		}

		/// <summary>
		/// The index of the currently selected color in the sub-palette.
		/// </summary>
		public override int CurrentColor()
		{
			return m_subpalettes[CurrentSubpaletteId].CurrentColor;
		}

		public override void SetCurrentColor(int nColor)
		{
			m_subpalettes[CurrentSubpaletteId].CurrentColor = nColor;
		}

		#region Load/Save/Export

		public bool LoadXML_palette16(XmlNode xnode)
		{
			int nCount = 0;
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "subpalette16")
				{
					if (!LoadXML_subpalette16(xn, nCount++))
						return false;
				}
			}
			if (nCount != 16)
			{
				m_doc.ErrorString("Incorrect number of subpalettes in palette '{0}'.", Name);
				return false;
			}

			return true;
		}

		private bool LoadXML_subpalette16(XmlNode xnode, int nSubpaletteId)
		{
			int nId = XMLUtils.GetXMLIntegerAttribute(xnode, "id");
			if (nId != nSubpaletteId)
			{
				m_doc.WarningString("Expected subpalette id = {0} in palette '{1}'.", nSubpaletteId, Name);
				return false;
			}
			if (nSubpaletteId >= 16)
			{
				m_doc.ErrorString("Invalid subpalette id ({0}) in palette '{1}'.", nSubpaletteId, Name);
				return false;
			}

			int[] anPalette = new int[16];
			int nCount = 0;
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "color")
				{
					string strRGB = XMLUtils.GetXMLAttribute(xn, "rgb");
					int nRGB;
					if (!ParseRGBColorValue(strRGB, out nRGB))
					{
						m_doc.ErrorString("Unable to parse color value '{0}' in palette '{1}'.", strRGB, Name);
						return false;
					}

					if (nCount < 16)
						anPalette[nCount] = nRGB;
					nCount++;
				}
			}

			if (nCount != 16)
			{
				// "Wrong number of colors in palette with ID='{0}'. Found {1}, expected 16."
				m_doc.ErrorId("ErrorNumColorsInPalette", nId.ToString(), nCount);
				return false;
			}

			// Load the colors into the subpalette.
			if (!ImportSubpalette(nSubpaletteId, anPalette))
			{
				// Warning/Error message already displayed.
				return false;
			}

			// Since we just loaded from a file, update the snapshot without creating an UndoAction
			GetSubpalette(nSubpaletteId).RecordSnapshot();

			return true;
		}

		public override void Save(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t\t<palette16 name=\"{0}\" id=\"{1}\" desc=\"{2}\">",
					m_strName, m_id, m_strDesc));

			for (int i = 0; i < m_nMaxSubpalettes; i++)
			{
				tw.WriteLine(String.Format("\t\t\t<subpalette16 id=\"{0}\">", i));

				m_subpalettes[i].Save(tw);

				tw.WriteLine("\t\t\t</subpalette16>");
			}

			tw.WriteLine("\t\t</palette16>");
		}

		public override void Export_PaletteInfo(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t{{{0,4},{1,6} }}, // Palette #{2} : {3}",
				0, "true", m_nExportId, m_strName));
		}

		public override void Export_Palette(System.IO.TextWriter tw)
		{
			if (m_nExportId != 0)
				tw.WriteLine("");

			tw.WriteLine("\t// Palette : " + m_strName + " [16-color]");
			if (m_strDesc != "")
				tw.WriteLine("\t// Description : " + m_strDesc);
			for (int i = 0; i < m_nMaxSubpalettes; i++)
				m_subpalettes[i].Export_Subpalette(tw, i);
		}

		#endregion

		#region Tests

		[TestFixture]
		public class Palette_Test
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
			public void Test_LoadXML_palette16_valid()
			{
				Palette16 p = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p);

				// Valid palette16.
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				// Attributes on palette are not needed for test:
				//xnPalette16.SetAttribute("name", "pal16");
				//xnPalette16.SetAttribute("id", "0");
				//xnPalette16.SetAttribute("desc", "");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_palette16_AddSubpalette(xnPalette16, id, 16);

				Assert.IsTrue(p.LoadXML_palette16(xnPalette16));

				// Verify the colors are the same ones from the XML:
				// Each component of Color 0 is the same as the subpalette id
				Assert.AreEqual(0, p.GetSubpalette(0).Red(0));
				Assert.AreEqual(4, p.GetSubpalette(4).Green(0));
				// Components of other colors are the same as the color index
				Assert.AreEqual(3, p.GetSubpalette(0).Red(3));
				Assert.AreEqual(7, p.GetSubpalette(6).Green(7));
				Assert.AreEqual(12, p.GetSubpalette(8).Blue(12));
			}

			[Test]
			public void Test_LoadXML_palette16_invalidid()
			{
				Palette16 p16 = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p16);

				// Invalid palette id (145)
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				Test_LoadXML_palette16_AddSubpalette(xnPalette16, 145, 16);

				Assert.IsFalse(p16.LoadXML_palette16(xnPalette16));
			}

			[Test]
			public void Test_LoadXML_palette16_15subpalettes()
			{
				Palette16 p = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p);

				// 15 subpalettes
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				for (int id = 0; id < 15; id++)
					Test_LoadXML_palette16_AddSubpalette(xnPalette16, id, 16);

				Assert.IsFalse(p.LoadXML_palette16(xnPalette16));
			}

			[Test]
			public void Test_LoadXML_palette16_17subpalettes()
			{
				Palette16 p = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p);

				// 17 subpalettes
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_palette16_AddSubpalette(xnPalette16, id, 16);
				Test_LoadXML_palette16_AddSubpalette(xnPalette16, 0, 16);

				Assert.IsFalse(p.LoadXML_palette16(xnPalette16));
			}

			[Test]
			public void Test_LoadXML_palette16_15colors()
			{
				Palette16 p = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p);

				// 15 colors in each subpalette
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_palette16_AddSubpalette(xnPalette16, id, 15);

				Assert.IsFalse(p.LoadXML_palette16(xnPalette16));
			}

			[Test]
			public void Test_LoadXML_palette16_17colors()
			{
				Palette16 p = m_doc.Palettes.AddPalette16("pal16", 0, "");
				Assert.IsNotNull(p);

				// 17 colors in each subpalette
				XmlElement xnPalette16 = m_xd.CreateElement("palette16");
				for (int id = 0; id < 16; id++)
					Test_LoadXML_palette16_AddSubpalette(xnPalette16, id, 17);

				Assert.IsFalse(p.LoadXML_palette16(xnPalette16));
			}

			private void Test_LoadXML_palette16_AddSubpalette(XmlElement xnPalette16, int id, int nColors)
			{
				XmlElement xnSubpalette = m_xd.CreateElement("subpalette16");
				xnSubpalette.SetAttribute("id", id.ToString());

				XmlElement xnColor = m_xd.CreateElement("color");
				xnColor.SetAttribute("rgb", String.Format("{0:x2}{1:x2}{2:x2}", id, id, id));
				xnSubpalette.AppendChild(xnColor);

				for (int i = 1; i < nColors; i++)
				{
					xnColor = m_xd.CreateElement("color");
					xnColor.SetAttribute("rgb", String.Format("{0:x2}{1:x2}{2:x2}", i, i, i));
					xnSubpalette.AppendChild(xnColor);
				}
				xnPalette16.AppendChild(xnSubpalette);
			}

		}

		#endregion

	}
}
