using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class Palettes
	{
		private Document m_doc;
		private Dictionary<int, Palette> m_palettes;

		private Palette m_paletteCurrent;

		public enum Type
		{
			Sprite,
			Background
		}
		private Type m_eType;

		public Palettes(Document doc, Type eType)
		{
			m_doc = doc;
			m_palettes = new Dictionary<int, Palette>();
			m_eType = eType;
			m_paletteCurrent = null;
		}

		public bool IsBackground
		{
			get { return m_eType == Type.Background; }
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (Palette p in m_palettes.Values)
			{
				p.UpdateDocument(doc);
			}
		}

		public void RecordSnapshot()
		{
			foreach (Palette p in m_palettes.Values)
			{
				p.RecordSnapshot();
			}
		}

		public int NumPalettes
		{
			get { return m_palettes.Count; }
		}

		public void Clear()
		{
			m_palettes.Clear();
		}

		public Palette16 AddPalette16(string strName, int id, string strDesc)
		{
			// Don't allow a second palette with the same name.
			if (m_palettes.ContainsKey(id))
				return null;

			Palette16 pal16 = new Palette16(m_doc, this, strName, id, strDesc);
			m_palettes.Add(id, pal16);
			m_paletteCurrent = pal16;
			return pal16;
		}

		public Palette GetPalette(int id)
		{
			if (!m_palettes.ContainsKey(id))
				return null;
			return m_palettes[id];
		}

		public Palette CurrentPalette
		{
			get { return m_paletteCurrent; }
		}

		public bool LoadXML_palettes(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "palette16":
						string strName = XMLUtils.GetXMLAttribute(xn, "name");
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
						string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");

						Palette16 p = AddPalette16(strName, id, strDesc);
						if (!p.LoadXML_palette16(xn))
							return false;
						break;
					case "palette256":
						// NYI - ignore
						break;
				}
			}
			return true;
		}

		public void Save(System.IO.TextWriter tw)
		{
			Export_AssignIDs();

			switch (m_eType)
			{
				case Type.Sprite:
					tw.WriteLine("\t<palettes>");
					break;
				case Type.Background:
					tw.WriteLine("\t<bgpalettes>");
					break;
			}

			foreach (Palette p in m_palettes.Values)
			{
				p.Save(tw);
			}

			switch (m_eType)
			{
				case Type.Sprite:
					tw.WriteLine("\t</palettes>");
					break;
				case Type.Background:
					tw.WriteLine("\t</bgpalettes>");
					break;
			}
		}

		public void Export_AssignIDs()
		{
			int nPaletteExportId = 0;
			foreach (Palette p in m_palettes.Values)
				p.Export_AssignIDs(nPaletteExportId);
		}

		public void Export_PaletteInfo(System.IO.TextWriter tw)
		{
			foreach (Palette p in m_palettes.Values)
				p.Export_PaletteInfo(tw);
		}

		public void Export_Palettes(System.IO.TextWriter tw)
		{
			foreach (Palette p in m_palettes.Values)
				p.Export_Palette(tw);
		}

	}
}
