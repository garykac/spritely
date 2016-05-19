using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Spritely
{
	public abstract class Palette
	{
		protected Document m_doc;
		protected Palettes m_palettes;

		protected string m_strName;
		protected int m_id;
		protected string m_strDesc;

		protected int m_nMaxSubpalettes;

		public enum Type
		{
			Unknown,
			Color16,
			Color256,
		};

		protected Type m_type;

		public Palette(Document doc, Palettes pals, string strName, int id, string strDesc)
		{
			m_doc = doc;
			m_palettes = pals;
			m_strName = strName;
			m_id = id;
			m_strDesc = strDesc;

			m_type = Type.Unknown;
			m_nMaxSubpalettes = 0;
		}

		public virtual void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public virtual void RecordSnapshot()
		{
		}

		public abstract IPaletteForm PaletteWindow();
		public abstract System.Windows.Forms.Form PaletteWindowForm();

		public string Name
		{
			get { return m_strName; }
		}

		public string NameDesc
		{
			get { return m_strName + " [16]"; }
		}

		public Type PaletteType
		{
			get { return m_type; }
		}

		public bool IsBackground
		{
			get { return m_palettes.IsBackground; }
		}

		public int NumSubpalettes
		{
			get { return m_nMaxSubpalettes; }
		}

		public virtual Subpalette GetCurrentSubpalette()
		{
			return null;
		}

		public virtual Subpalette GetSubpalette(int nIndex)
		{
			return null;
		}

		/// <summary>
		/// The index of the currently selected color in the palette.
		/// </summary>
		public abstract int CurrentColor();

		public abstract void SetCurrentColor(int nColor);

		public static bool ParseRGBColorValue(string strRGB, out int nRGB)
		{
			Regex rxRGB = new Regex(@"([0-1][0-9A-Fa-f])([0-1][0-9A-Fa-f])([0-1][0-9A-Fa-f])");
			Match mxRGB = rxRGB.Match(strRGB);
			nRGB = 0;
			if (!mxRGB.Success)
				return false;

			GroupCollection matchGroups = mxRGB.Groups;
			int r = Convert.ToInt32(matchGroups[1].Value, 16);
			int g = Convert.ToInt32(matchGroups[2].Value, 16);
			int b = Convert.ToInt32(matchGroups[3].Value, 16);
			nRGB = Color555.Encode(r, g, b);
			return true;
		}

		#region Load/Save/Export

		public virtual void Save(System.IO.TextWriter tw)
		{
		}

		protected int m_nExportId;

		public int ExportId
		{
			get { return m_nExportId; }
		}

		public void Export_AssignIDs(int nPaletteExportId)
		{
			m_nExportId = nPaletteExportId;
		}

		public virtual void Export_PaletteInfo(System.IO.TextWriter tw)
		{
		}

		public virtual void Export_Palette(System.IO.TextWriter tw)
		{
		}

		#endregion

	}
}
