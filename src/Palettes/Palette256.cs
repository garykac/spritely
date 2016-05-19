using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public class Palette256 : Palette
	{
		private Palette256Form m_winPalette;

		private PaletteColorData m_data;

		private const int k_nColors = 256;

		public Palette256(Document doc, Palettes pals, string strName, int id, string strDesc) :
			base(doc, pals, strName, id, strDesc)
		{
			m_type = Palette.Type.Color256;

			m_data = new PaletteColorData(k_nColors);

			if (m_doc.Owner != null)
				m_winPalette = new Palette256Form(m_doc.Owner, this);
		}

		public override void UpdateDocument(Document doc)
		{
			base.UpdateDocument(doc);
		}

		public override void RecordSnapshot()
		{
			base.RecordSnapshot();
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
			return null;
		}

		public override Subpalette GetSubpalette(int nIndex)
		{
			return null;
		}

		/// <summary>
		/// Set the default subpalettes.
		/// </summary>
		public void SetDefaultPalette()
		{
		}

		/// <summary>
		/// The index of the currently selected color in the sub-palette.
		/// </summary>
		public override int CurrentColor()
		{
			return m_data.currentColor;
		}

		public override void SetCurrentColor(int nColor)
		{
			m_data.currentColor = nColor;
		}

		#region Load/Save/Export

		public bool LoadXML_palette256(XmlNode xnode)
		{
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

			if (nCount != 256)
			{
				// "Wrong number of colors in palette with ID='{0}'. Found {1}, expected 256."
				m_doc.ErrorId("ErrorNumColorsInPalette", m_id.ToString(), nCount);
				return false;
			}

			// Load the colors into the subpalette.
			//if (!ImportPalette(anPalette))
			//{
			//	// Warning/Error message already displayed.
			//	return false;
			//}

			// Since we just loaded from a file, update the snapshot without creating an UndoAction
			RecordSnapshot();

			return true;
		}

		public override void Save(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t\t<palette256 name=\"{0}\" id=\"{1}\" desc=\"{2}\">",
					m_strName, m_id, m_strDesc));

			// TODO: save 256 palette colors
			tw.WriteLine("\t\t</palette256>");
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

			tw.WriteLine("\t// Palette : " + m_strName + " [256-color]");
			if (m_strDesc != "")
				tw.WriteLine("\t// Description : " + m_strDesc);
			StringBuilder sb = null;
			int nPerLine = 8;

			for (int i = 0; i < 256; i++)
			{
				if ((i % nPerLine) == 0)
				{
					if (sb != null)
						tw.WriteLine(sb.ToString());
					sb = new StringBuilder("\t");
				}
				sb.Append(String.Format("0x{0:x4},", m_data.Encoding(i)));
			}
			if (sb != null)
				tw.WriteLine(sb.ToString());
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
