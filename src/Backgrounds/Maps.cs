using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class Maps
	{
		private Document m_doc;
		private Dictionary<int, Map> m_maps;

		private Map m_mapCurrent;

		public Maps(Document doc)
		{
			m_doc = doc;
			m_maps = new Dictionary<int, Map>();
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (Map m in m_maps.Values)
			{
				m.UpdateDocument(doc);
			}
		}

		public void RecordSnapshot()
		{
			foreach (Map m in m_maps.Values)
			{
				m.RecordSnapshot();
			}
		}

		public Dictionary<int, Map>.ValueCollection AllMaps
		{
			get { return m_maps.Values; }
		}

		public int NumMaps
		{
			get { return m_maps.Count; }
		}

		public Map CurrentMap
		{
			get { return m_mapCurrent; }
		}

		public void Clear()
		{
			m_maps.Clear();
			m_mapCurrent = null;
		}

		public Map GetMap(int id)
		{
			if (id < 0 || id >= NumMaps)
				return null;
			return m_maps[id];
		}

		public Map AddMap(string strName, int id, string strDesc, Spriteset bgtiles)
		{
			if (m_maps.ContainsKey(id))
				return null;

			Map m = new Map(m_doc, strName, id, strDesc, bgtiles);
			m_maps.Add(id, m);
			m_mapCurrent = m;
			return m;
		}

		public bool LoadXML_bgmaps(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "map16":
						string strName = XMLUtils.GetXMLAttribute(xn, "name");
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
						string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
						string strSize = XMLUtils.GetXMLAttribute(xn, "size");
						int nSpritesetId = XMLUtils.GetXMLIntegerAttribute(xn, "bgspriteset_id");
						int nDefaultSubpaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "default_subpalette_id");

						string[] aSize = strSize.Split('x');
						int nWidth = XMLUtils.ParseInteger(aSize[0]);
						int nHeight = XMLUtils.ParseInteger(aSize[1]);

						Spriteset ts = m_doc.BackgroundSpritesets.GetSpriteset(nSpritesetId);
						Map m = AddMap(strName, id, strDesc, ts);
						if (!m.LoadXML_map16(xn, nDefaultSubpaletteId))
							return false;
						break;
				}
			}
			return true;
		}

		public void Save(System.IO.TextWriter tw)
		{
			Export_AssignIDs();

			tw.WriteLine("\t<bgmaps>");

			foreach (Map m in m_maps.Values)
			{
				m.Save(tw);
			}

			tw.WriteLine("\t</bgmaps>");
		}

		public void Export_AssignIDs()
		{
			int nMapExportId = 0;
			foreach (Map m in m_maps.Values)
				m.Export_AssignIDs(nMapExportId++);
		}

		public void Export_MapInfo(System.IO.TextWriter tw)
		{
			int nMapOffset = 0;
			foreach (Map m in m_maps.Values)
			{
				tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4}}}, // Map_{3}",
					nMapOffset, 32, 32, m.Name));
				nMapOffset += (32 * 32 * 2);
			}
		}

		public void Export_MapIDs(System.IO.TextWriter tw)
		{
			foreach (Map m in m_maps.Values)
			{
				tw.WriteLine(String.Format("const int kBgMap_{0} = {1};", m.Name, m.ExportId));
			}
		}

		public void Export_MapData(System.IO.TextWriter tw)
		{
			foreach (Map m in m_maps.Values)
			{
				m.Export_BackgroundMap(tw);
			}
		}

	}
}
