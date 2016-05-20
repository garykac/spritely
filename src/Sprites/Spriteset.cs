using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public class Spriteset
	{
		private Document m_doc;
		private string m_strName;
		private int m_id;
		private string m_strDesc;
		private bool m_fIsBackground;
		private Palette m_palette;

		private SpritesetForm m_winSpriteset;
		private SpriteForm m_winSprite;

		// List of maps that are based on this spritelist.
		private List<Map> m_Maps;

		private int m_nSprites;
		private int m_nTiles;

		public SpriteType[] SpriteTypes = new SpriteType[]
		{
			new SpriteType("1x1",		1,1,	Sprite.GBASize.Size8,		Sprite.GBAShape.Square),
			new SpriteType("1x2",		1,2,	Sprite.GBASize.Size8,		Sprite.GBAShape.Tall),
			new SpriteType("1x4",		1,4,	Sprite.GBASize.Size16,		Sprite.GBAShape.Tall),
			new SpriteType("2x1",		2,1,	Sprite.GBASize.Size8,		Sprite.GBAShape.Wide),
			new SpriteType("2x2",		2,2,	Sprite.GBASize.Size16,		Sprite.GBAShape.Square),
			new SpriteType("2x4",		2,4,	Sprite.GBASize.Size32,		Sprite.GBAShape.Tall),
			new SpriteType("4x1",		4,1,	Sprite.GBASize.Size16,		Sprite.GBAShape.Wide),
			new SpriteType("4x2",		4,2,	Sprite.GBASize.Size32,		Sprite.GBAShape.Wide),
			new SpriteType("4x4",		4,4,	Sprite.GBASize.Size32,		Sprite.GBAShape.Square),
			new SpriteType("4x8",		4,8,	Sprite.GBASize.Size64,		Sprite.GBAShape.Tall),
			new SpriteType("8x4",		8,4,	Sprite.GBASize.Size64,		Sprite.GBAShape.Wide),
			new SpriteType("8x8",		8,8,	Sprite.GBASize.Size64,		Sprite.GBAShape.Square),
		};

		public Spriteset(Document doc, string strName, int id, string strDesc, Palette pal)
		{
			m_doc = doc;
			m_strName = strName;
			m_id = id;
			m_strDesc = strDesc;
			m_palette = pal;
			m_fIsBackground = pal.IsBackground;

			m_Maps = new List<Map>();

			if (m_doc.Owner != null)
			{
				m_winSpriteset = new SpritesetForm(m_doc.Owner, this); ;
				m_winSprite = new SpriteForm(m_doc.Owner, this, CurrentSprite);
			}
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.UpdateDocument(doc);
				}
			}
		}

		public void RecordSnapshot()
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.RecordSnapshot();
				}
			}
		}

		public SpritesetForm SpritesetWindow
		{
			get { return m_winSpriteset; }
		}

		public SpriteForm SpriteWindow
		{
			get { return m_winSprite; }
		}

		public Palette Palette
		{
			get { return m_palette; }
		}

		/// <summary>
		/// Flush the bitmaps for all sprites so that they get regenerated.
		/// </summary>
		public void FlushBitmaps()
		{
			foreach (SpriteType st in SpriteTypes)
				foreach (Sprite s in st.Sprites)
					s.FlushBitmaps();
		}

		public int Id
		{
			get { return m_id; }
		}

		public bool IsBackground
		{
			get { return m_fIsBackground; }
		}

		/// <summary>
		/// The total number of sprites in this spriteset.
		/// </summary>
		public int NumSprites
		{
			get { return m_nSprites; }
		}

		/// <summary>
		/// The total number of tiles in this spriteset.
		/// </summary>
		public int NumTiles
		{
			get { return m_nTiles; }
		}

		private int m_nNextTileId = 0;

		public int NextTileId
		{
			get { return m_nNextTileId; }
			set { m_nNextTileId = value; }
		}

		private int m_nNextSpriteId = 0;

		public int NextSpriteId
		{
			get { return m_nNextSpriteId; }
			set { m_nNextSpriteId = value; }
		}

		#region Sprite Info

		/// <summary>
		/// The name of this spriteset.
		/// </summary>
		public string Name
		{
			get { return m_strName; }
		}

		public string GenerateUniqueSpriteName()
		{
			// When we're auto-generating sprite names, make sure the new name doesn't collide with
			// the names of any already-existing sprites.
			string strName = AutoGenerateSpriteName();
			while (HasNamedSprite(strName))
				strName = AutoGenerateSpriteName();
			return strName;
		}

		public string AutoGenerateSpriteName()
		{
			return String.Format(m_fIsBackground ? "BgS{0}" : "S{0}", NextSpriteId++);
		}

		/// <summary>
		/// Check if a sprite with the specified name already exists in this SpriteSet.
		/// </summary>
		/// <param name="strName">The name of the sprite to look for.</param>
		/// <returns>True if a sprite with that name exists.</returns>
		public bool HasNamedSprite(string strName)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (s.Name == strName)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Is this sprite the first/last one in the group of SpriteTypes
		/// </summary>
		/// <param name="sprite">The sprite to check</param>
		/// <param name="fIsFirst">Returns true if this is the first sprite</param>
		/// <param name="fIsLast">Returns true if this is the last sprite</param>
		public void IsFirstLastSpriteOfType(Sprite sprite, out bool fIsFirst, out bool fIsLast)
		{
			fIsFirst = false;
			fIsLast = false;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Contains(sprite))
				{
					bool fFirst = true;
					bool fLast = false;
					foreach (Sprite s in st.Sprites)
					{
						if (s == sprite)
						{
							if (fFirst)
								fIsFirst = true;
							fLast = true;
						}
						else
							fLast = false;
						fFirst = false;
					}
					if (fLast)
						fIsLast = true;
					return;
				}
			}
		}

		// Is this the first sprite in the SpriteList?
		public bool IsFirstSprite(Sprite sBase)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					return s == sBase;
				}
			}
			return false;
		}

		// Is this the last sprite in the SpriteList?
		public bool IsLastSprite(Sprite sBase)
		{
			bool fIsLast = false;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					fIsLast = (s == sBase);
				}
			}
			return fIsLast;
		}

		// Return the next sprite after the given sprite in the SpriteList.
		// Returns null if the given sprite is the last one, or if the given sprite doesn't exist.
		public Sprite NextSprite(Sprite sBase)
		{
			bool fReturnNextSprite = false;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (fReturnNextSprite)
						return s;
					if (s == sBase)
						fReturnNextSprite = true;
				}
			}
			return null;
		}

		// Return the previous sprite before the given sprite in the SpriteList.
		// Returns null if the given sprite is the first one, or if the given sprite doesn't exist.
		public Sprite PrevSprite(Sprite sBase)
		{
			Sprite sPrev = null;
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (s == sBase)
						return sPrev;
					sPrev = s;
				}
			}
			return null;
		}

		#endregion

		#region Add/Remove Sprite

		public void RemoveSelectedSprite(UndoMgr undo)
		{
			RemoveSprite(CurrentSprite, undo);
		}

		public void RemoveSprite(Sprite sToRemove, UndoMgr undo)
		{
			SpriteType stToRemove = null;
			Sprite sPrev = null, sCurr = null, sNext = null;
			Sprite sNewSelection = null;

			if (sToRemove == null)
				return;

			// Determine which sprite should be selected when this one is removed.
			foreach (SpriteType st in SpriteTypes)
			{
				if (sNewSelection != null)
					break;

				foreach (Sprite s in st.Sprites)
				{
					sPrev = sCurr;
					sCurr = sNext;
					sNext = s;
					if (s == sToRemove)
						stToRemove = st;
					if (sCurr == sToRemove)
					{
						sNewSelection = sNext;
						break;
					}
				}
			}
			// If the last sprite is deleted, select the one before it.
			if (sNext == sToRemove)
				sNewSelection = sCurr;

			int nTiles = sToRemove.NumTiles;
			if (stToRemove == null)
				return;

			if (stToRemove.Sprites.Remove(sToRemove))
			{
				CurrentSprite = null;
				if (undo != null)
					CurrentSprite = undo.FindMostRecentSprite();
				if (CurrentSprite == null)
					CurrentSprite = sNewSelection;

				m_nSprites--;
				m_nTiles -= nTiles;

				if (undo != null)
					undo.Push(new UndoAction_AddSprite(undo, this, sToRemove, false));

				if (m_doc.Owner != null)
					m_doc.Owner.HandleSpriteTypeChanged(this);
			}

			foreach (Map m in m_Maps)
			{
				m.RemoveSpriteTilesFromMap(sToRemove);
			}
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, UndoMgr undo)
		{
			return AddSprite(nWidth, nHeight, strName, id, strDesc, 0, undo);
		}

		public Sprite AddSprite(int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette, UndoMgr undo)
		{
			if (id == -1)
				id = NextTileId++;
			Sprite s = AddSprite_(nWidth, nHeight, strName, id, strDesc, nSubpalette, undo);

			// Make this the currently selected sprite.
			CurrentSprite = s;
			return s;
		}

		private Sprite AddSprite_(int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette, UndoMgr undo)
		{
			List<Sprite> slist = null;

			// Make sure that the requested size is valid.
			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Width == nWidth && st.Height == nHeight)
					slist = st.Sprites;
			}

			// Invalid sprite size - return.
			if (slist == null)
				return null;

			Sprite s = new Sprite(m_doc, this, nWidth, nHeight, strName, id, strDesc, nSubpalette);
			return AddSprite_2(s, slist, undo);
		}

		public Sprite AddSprite(Sprite s, UndoMgr undo)
		{
			List<Sprite> slist = null;

			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Width == s.TileWidth && st.Height == s.TileHeight)
				{
					slist = st.Sprites;
					break;
				}
			}

			if (slist == null)
				return null;

			return AddSprite_2(s, slist, undo);
		}

		private Sprite AddSprite_2(Sprite s, List<Sprite> slist, UndoMgr undo)
		{
			slist.Add(s);

			m_nSprites++;
			m_nTiles += (s.TileWidth * s.TileHeight);

			if (undo != null)
				undo.Push(new UndoAction_AddSprite(undo, this, s, true));

			if (m_doc.Owner != null)
				m_doc.Owner.HandleSpriteTypeChanged(this);
			return s;
		}

		public Sprite DuplicateSprite(Sprite sToCopy, UndoMgr undo)
		{
			if (sToCopy == null)
				return null;

			// Calulate an appropriate name for the copy
			string strNewBaseName;
			int nCopy = 1;
			string strNewName;
			string strCopySuffix = ResourceMgr.GetString("CopySuffix");
			Match m = Regex.Match(sToCopy.Name, String.Format("^(.*){0}([0-9]*)$", strCopySuffix));
			if (m.Success)
			{
				strNewBaseName = String.Format("{0}{1}", m.Groups[1].Value, strCopySuffix);
				if (m.Groups[2].Value != "")
					nCopy = Int32.Parse(m.Groups[2].Value);
				strNewName = String.Format("{0}{1}", strNewBaseName, ++nCopy);
			}
			else
			{
				strNewBaseName = String.Format("{0}{1}", sToCopy.Name, strCopySuffix);
				strNewName = strNewBaseName;
			}

			while (HasNamedSprite(strNewName))
				strNewName = String.Format("{0}{1}", strNewBaseName, ++nCopy);

			Sprite sNew = AddSprite(sToCopy.TileWidth, sToCopy.TileHeight, strNewName, NextTileId++, sToCopy.Description, undo);
			sNew.Duplicate(sToCopy);
			return sNew;
		}

		#endregion

		/// <summary>
		/// Find the sprite that owns this tile.
		/// </summary>
		/// <param name="nTileID">Tile id</param>
		/// <returns>The sprite that owns this tile</returns>
		public Sprite FindSprite(int nTileID)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					if (nTileID >= s.FirstTileId && nTileID < s.FirstTileId + s.NumTiles)
						return s;
				}
			}
			return null;
		}

		/// <summary>
		/// Select the first sprite.
		/// This is useful after loading a file of sprites since otherwise the selected
		/// sprite will be the last one loaded from the file.
		/// </summary>
		public void SelectFirstSprite()
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					CurrentSprite = s;
					return;
				}
			}
		}

		/// <summary>
		/// The currently selected sprite in the spriteset.
		/// </summary>
		private Sprite m_spriteCurrent;

		/// <summary>
		/// The currently selected sprite in the spriteset.
		/// </summary>
		public Sprite CurrentSprite
		{
			get { return m_spriteCurrent; }
			set
			{
				m_spriteCurrent = value;
				if (m_doc.Owner != null)
					m_doc.Owner.HandleSpriteSelectionChanged(this);
			}
		}

		#region Sprite Arrangement

		// Remove from the old SpriteType and add it to the new one
		public void MoveToCorrectSpriteType(Sprite sprite)
		{
			SpriteType stRemove = null;
			SpriteType stAdd = null;
			foreach (SpriteType st in SpriteTypes)
			{
				if (st.Sprites.Contains(sprite))
					stRemove = st;
				if (st.Width == sprite.TileWidth && st.Height == sprite.TileHeight)
					stAdd = st;
			}
			if (stRemove != stAdd)
			{
				if (stRemove != null)
					stRemove.Sprites.Remove(sprite);
				if (stAdd != null)
					stAdd.Sprites.Add(sprite);
				m_doc.Owner.HandleSpriteTypeChanged(this);
			}
		}

		public bool ResizeSelectedSprite(int tileNewWidth, int tileNewHeight)
		{
			Sprite sToResize = CurrentSprite;
			if (sToResize == null)
				return false;

			int nOldTiles = sToResize.NumTiles;

			if (!sToResize.Resize(tileNewWidth, tileNewHeight))
				return false;

			m_nTiles += sToResize.NumTiles - nOldTiles;

			//TODO: adjust the background map

			MoveToCorrectSpriteType(sToResize);
			return true;
		}

		// Return true if successfully rotated.
		public bool RotateSelectedSprite(Sprite.RotateDirection dir)
		{
			Sprite sToRotate = CurrentSprite;
			if (sToRotate == null)
				return false;

			int tileNewWidth = sToRotate.TileHeight;
			int tileNewHeight = sToRotate.TileWidth;

			if (!sToRotate.Rotate(dir))
				return false;

			MoveToCorrectSpriteType(sToRotate);
			return true;
		}

		public void ShiftPixels(Arrowbox.ShiftArrow shift)
		{
			if (CurrentSprite == null)
				return;

			CurrentSprite.ShiftPixels(shift);
		}

		#endregion

		/// <summary>
		/// Add a map to the list of maps that are based on this SpriteList.
		/// Only used for background SpriteLists.
		/// </summary>
		/// <param name="m">The map</param>
		public void AddMap(Map m)
		{
			m_Maps.Add(m);
		}

		#region Load

		public bool LoadXML_spriteset16(XmlNode xnode)
		{
			int nTileId = 0;

			foreach (XmlNode xn in xnode.ChildNodes)
			{
				if (xn.Name == "sprite16")
				{
					string strName = XMLUtils.GetXMLAttribute(xn, "name");
					int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
					string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
					string strSize = XMLUtils.GetXMLAttribute(xn, "size");
					int nSubpaletteId = XMLUtils.GetXMLIntegerAttribute(xn, "subpalette_id");

					string[] aSize = strSize.Split('x');
					int nWidth = XMLUtils.ParseInteger(aSize[0]);
					int nHeight = XMLUtils.ParseInteger(aSize[1]);

					Sprite s = AddSprite(nWidth, nHeight, strName, NextSpriteId++, strDesc, nSubpaletteId, null);
					if (!s.LoadXML_sprite16(xn, nTileId))
						return false;

					nTileId += s.NumTiles;
				}
			}
			return true;
		}

		#endregion

		#region Save

		public void Save(System.IO.TextWriter tw)
		{
			int nExportSpriteID = 0;
			int nExportFirstTileID = 0;

			tw.WriteLine("\t\t<spriteset16 name=\"sprites\" id=\"0\" desc=\"\" palette_id=\"0\">");

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Save(tw, nExportSpriteID, nExportFirstTileID);
					nExportSpriteID++;
					nExportFirstTileID += st.Width * st.Height;
				}
			}

			tw.WriteLine("\t\t</spriteset16>");
		}

		#endregion

		#region Export

		private int m_nExportId;

		public void Export_AssignIDs(int nSpritesetExportId)
		{
			m_nExportId = nSpritesetExportId;

			int nSpriteExportID = 0;
			int nFirstTileID = 0;
			int nMaskIndex = 0;

			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_AssignIDs(nSpriteExportID, nFirstTileID, nMaskIndex);
					nSpriteExportID++;
					nFirstTileID += st.Width * st.Height;
					nMaskIndex += s.CalcMaskSize();
				}
			}
		}

		public void Export_SpritesetInfo(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4},{3,4} }}, // Spriteset #{4} : {5}",
				0, NumSprites, NumTiles, m_palette.ExportId, m_nExportId, m_strName));
		}

		public void Export_SpritesetIDs(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("const int kSpriteset_{0} = {1};", m_strName, m_nExportId));
		}

		public void Export_BgTilesetInfo(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("\t{{{0,4},{1,4},{2,4} }}, // BgTileset #{3} : {4}",
				0, NumTiles, m_palette.ExportId, m_nExportId, m_strName));
		}

		public void Export_BgTilesetIDs(System.IO.TextWriter tw)
		{
			tw.WriteLine(String.Format("const int kBgTileset_{0} = {1};", m_strName, m_nExportId));
		}

		public void Export_SpriteInfo(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_SpriteInfo(tw, st.Size, st.Shape);
				}
			}
		}

		public void Export_SpriteIDs(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_SpriteIDs(tw, m_strName);
				}
			}
		}

		public void Export_TileIDs(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
				{
					s.Export_TileIDs(tw, m_strName);
				}
			}
		}

		public void Export_TileData(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
					s.Export_TileData(tw);
			}
		}

		public void Export_SpriteMaskData(System.IO.TextWriter tw)
		{
			foreach (SpriteType st in SpriteTypes)
			{
				foreach (Sprite s in st.Sprites)
					s.Export_SpriteMaskData(tw);
			}
		}

		/// <summary>
		/// Convert internal tile id into an export tile id.
		/// </summary>
		/// <param name="nInternalTileId">Internal tile id.</param>
		/// <returns>Exportable tile id.</returns>
		public int GetTileExportId(int nInternalTileId)
		{
			// Find sprite that owns this tile.
			Sprite s = FindSprite(nInternalTileId);
			// Tile index into sprite.
			int nTileIndex = nInternalTileId - s.FirstTileId;
			// Export Id for this tile.
			int nTileExportId = s.ExportFirstTileId + nTileIndex;

			return nTileExportId;
		}

		#endregion

	}
}
