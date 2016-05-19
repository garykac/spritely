using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml;

namespace Spritely
{
	public static class Options
	{
		/// <summary>
		/// Is this the debug/testing build?
		/// </summary>
		public static bool DEBUG = false;

		/// <summary>
		/// Path to the raw tutorial files so that they can be processed by the
		/// tutorial test.
		/// </summary>
		/// <remarks>
		/// These are used only in the DEBUG=true version.
		/// Yes, they're hard-coded paths. If you want to build/update the tutorials,
		/// you'll need to change these to match your directory hierarchy.
		/// </remarks>
		public static string Debug_TutorialPath = @"C:\dev\spritely\tutorials";
		public static string Debug_TutorialRawPath = @"C:\dev\spritely\tutorials\raw";
		public static string Debug_TutorialRawDataPath = @"C:\dev\spritely\tutorials\raw_data";

		/// <summary>
		/// Version of devkitARM that we have tested with.
		/// </summary>
		public static int Debug_DevkitArmVersion = 26;

		/// <summary>
		/// Current Spritely version string.
		/// </summary>
		public static string VersionString = "0.19.26 beta";
		public static string VersionDate = "26 July 2009";

		public static string DefaultSpritesetName = "Sprites";
		public static string DefaultPaletteName = "Pal16";
		public static int DefaultPaletteId = 0;
		public static string DefaultBgTilesetName = "BgTiles";
		public static string DefaultBgPaletteName = "BgPal16";
		public static int DefaultBgPaletteId = 0;
		public static string DefaultMapName = "Map";

		/// <summary>
		/// The pattern to use when drawing transparent pixels.
		/// </summary>
		//public static HatchStyle TransparentPattern = HatchStyle.DiagonalCross;
		public static HatchStyle TransparentPattern = HatchStyle.DarkUpwardDiagonal;

		public enum PlatformType
		{
			GBA,
			NDS,
		};

		public static PlatformType Platform = PlatformType.GBA;

		class BoolOptionInfo
		{
			public string Name;
			public bool Value;

			public BoolOptionInfo(string strName, bool fDefault)
			{
				Name = strName;
				Value = fDefault;
			}
		}

		public enum BoolOptionName {
			Sprite_ShowPixelGrid,
			Sprite_ShowTileGrid,
			Sprite_ShowPaletteIndex,

			BackgroundMap_ShowGrid,
			BackgroundMap_ShowScreen,
		};

		static BoolOptionInfo[] BoolOptions = new BoolOptionInfo[]
		{
			new BoolOptionInfo("sprite_show_pixelgrid", true),
			new BoolOptionInfo("sprite_show_tilegrid", true),
			new BoolOptionInfo("sprite_show_palette_index", false),

			new BoolOptionInfo("bgmap_show_tilegrid", true),
			new BoolOptionInfo("bgmap_show_screen_boundary", true),
		};

		static string[] ObsoleteOptions = new string[]
		{
			"sprite_show_transparent",
			"palette_show_transparent",
			"palette_show_palette_index",
		};

		public static bool Get(BoolOptionName name)
		{
			return BoolOptions[(int)name].Value;
		}

		public static void Set(BoolOptionName name, bool value)
		{
			BoolOptions[(int)name].Value = value;
		}

		public static bool Sprite_ShowPixelGrid
		{
			get { return BoolOptions[(int)BoolOptionName.Sprite_ShowPixelGrid].Value; }
			set { BoolOptions[(int)BoolOptionName.Sprite_ShowPixelGrid].Value = value; }
		}
		public static bool Sprite_ShowTileGrid
		{
			get { return BoolOptions[(int)BoolOptionName.Sprite_ShowTileGrid].Value; }
			set { BoolOptions[(int)BoolOptionName.Sprite_ShowTileGrid].Value = value; }
		}
		public static bool Sprite_ShowPaletteIndex
		{
			get { return BoolOptions[(int)BoolOptionName.Sprite_ShowPaletteIndex].Value; }
			set { BoolOptions[(int)BoolOptionName.Sprite_ShowPaletteIndex].Value = value; }
		}

		public static bool BackgroundMap_ShowGrid
		{
			get { return BoolOptions[(int)BoolOptionName.BackgroundMap_ShowGrid].Value; }
			set { BoolOptions[(int)BoolOptionName.BackgroundMap_ShowGrid].Value = value; }
		}
		public static bool BackgroundMap_ShowScreen
		{
			get { return BoolOptions[(int)BoolOptionName.BackgroundMap_ShowScreen].Value; }
			set { BoolOptions[(int)BoolOptionName.BackgroundMap_ShowScreen].Value = value; }
		}

		public static bool LoadXML_options(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "option":
						if (!LoadXML_option(xn))
							return false;
						break;
				}
			}
			return true;
		}

		private static bool LoadXML_option(XmlNode xnode)
		{
			string strName = XMLUtils.GetXMLAttribute(xnode, "name");
			string strValue = XMLUtils.GetXMLAttribute(xnode, "value");

			if (strName == "platform")
			{
				Platform = strValue == "nds" ? PlatformType.NDS : PlatformType.GBA;
				return true;
			}

			// Ignore obsolete options.
			foreach (string badoption in ObsoleteOptions)
			{
				if (strName == badoption)
					return true;
			}

			// Handle any boolean options.
			foreach (BoolOptionInfo option in BoolOptions)
			{
				if (strName == option.Name)
				{
					option.Value = (strValue == "true" ? true : false);
					return true;
				}
			}

			return false;
		}

		public static void Save(System.IO.TextWriter tw)
		{
			tw.WriteLine("\t<options>");

			tw.WriteLine("\t\t<option name=\"platform\" value=\"{0}\"/>",
				Platform == PlatformType.GBA ? "gba" : "nds");

			foreach (BoolOptionInfo option in BoolOptions)
			{
				tw.WriteLine("\t\t<option name=\"{0}\" value=\"{1}\"/>",
					option.Name, option.Value ? "true" : "false");
			}

			tw.WriteLine("\t</options>");
		}

	}
}
