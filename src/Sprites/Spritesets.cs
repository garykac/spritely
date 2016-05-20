using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class Spritesets
	{
		private Document m_doc;
		private Dictionary<int, Spriteset> m_spritesets;

		private bool m_fBackground;

		private Spriteset m_ssCurrent;

		public Spritesets(Document doc, bool fBackground)
		{
			m_doc = doc;
			m_fBackground = fBackground;
			m_spritesets = new Dictionary<int, Spriteset>();
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (Spriteset ss in m_spritesets.Values)
			{
				ss.UpdateDocument(doc);
			}
		}

		public void RecordSnapshot()
		{
			foreach (Spriteset ss in m_spritesets.Values)
			{
				ss.RecordSnapshot();
			}
		}

		public int NumSpritesets
		{
			get { return m_spritesets.Count; }
		}

		public int NumSprites
		{
			get
			{
				int nSprites = 0;
				foreach (Spriteset ss in m_spritesets.Values)
					nSprites += ss.NumSprites;
				return nSprites;
			}
		}

		public int NumTiles
		{
			get
			{
				int nTiles = 0;
				foreach (Spriteset ss in m_spritesets.Values)
					nTiles += ss.NumTiles;
				return nTiles;
			}
		}

		public void Clear()
		{
			m_spritesets.Clear();
			m_ssCurrent = null;
		}

		public Spriteset AddSpriteset(string strName, int id, string strDesc, Palette pal)
		{
			// Don't allow a second spriteset with the same id.
			if (m_spritesets.ContainsKey(id))
				return null;

			Spriteset ss = new Spriteset(m_doc, strName, id, strDesc, pal);
			m_spritesets.Add(id, ss);

			// Make this spriteset the current one.
			m_ssCurrent = ss;
			return ss;
		}

		public Spriteset Current
		{
			get {return m_ssCurrent;}
		}

		public Spriteset GetSpriteset(int id)
		{
			return m_spritesets[id];
		}

		public void FlushBitmaps()
		{
			foreach (Spriteset ss in m_spritesets.Values)
			{
				ss.FlushBitmaps();
			}
		}

		public bool LoadXML_spritesets(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "spriteset16":
						string strName = XMLUtils.GetXMLAttribute(xn, "name");
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
						string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
						int nPaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "palette_id");

						Palette pal;
						if (m_fBackground)
							pal = m_doc.GetBackgroundPalette(id);
						else
							pal = m_doc.GetSpritePalette(id);
						Spriteset s = AddSpriteset(strName, id, strDesc, pal);
						if (!s.LoadXML_spriteset16(xn))
							return false;
						break;
					case "spriteset256":
						// NYI - ignore
						break;
				}
			}
			return true;
		}

		public void Save(System.IO.TextWriter tw)
		{
			Export_AssignIDs();

			if (m_fBackground)
				tw.WriteLine("\t<bgspritesets>");
			else
				tw.WriteLine("\t<spritesets>");

			foreach (Spriteset ss in m_spritesets.Values)
			{
				ss.Save(tw);
			}

			if (m_fBackground)
				tw.WriteLine("\t</bgspritesets>");
			else
				tw.WriteLine("\t</spritesets>");
		}

		#region Export

		public void Export_AssignIDs()
		{
			int nSpritesetExportID = 0;
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_AssignIDs(nSpritesetExportID++);
		}

		public void Export_SpritesetInfo(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_SpritesetInfo(tw);
		}

		public void Export_SpritesetIDs(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_SpritesetIDs(tw);
		}

		public void Export_BgTilesetInfo(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_BgTilesetInfo(tw);
		}

		public void Export_BgTilesetIDs(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_BgTilesetIDs(tw);
		}

		public void Export_SpriteInfo(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_SpriteInfo(tw);
		}

		public void Export_SpriteIDs(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_SpriteIDs(tw);
		}

		public void Export_TileIDs(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_TileIDs(tw);
		}

		public void Export_TileData(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_TileData(tw);
		}

		public void Export_SpriteMaskData(System.IO.TextWriter tw)
		{
			foreach (Spriteset ss in m_spritesets.Values)
				ss.Export_SpriteMaskData(tw);
		}

		#endregion

	}
}
