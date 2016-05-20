using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class FileHandler
	{
		public class ExportFile
		{
			/// <summary>
			/// Filename to create.
			/// </summary>
			public string Filename;
			/// <summary>
			/// Template file used to create the file.
			/// </summary>
			public string Template;
			/// <summary>
			/// Is this a project file (as opposed to a sprite/background data file)?
			/// </summary>
			public bool ProjectFile;
			/// <summary>
			/// Is this a game_state file?
			/// </summary>
			public bool GameStateFile;
			/// <summary>
			/// Subdirectory where this file should be placed when writing out projects.
			/// </summary>
			public string SubDirectory;
			public ExportFile(string strFile, string strTemplate, bool fProject, bool fGameState, string strDir)
			{
				Filename = strFile;
				Template = strTemplate;
				ProjectFile = fProject;
				GameStateFile = fGameState;
				SubDirectory = strDir;
			}
		};

		private ExportFile[] ExportFiles = new ExportFile[]
		{
			//             Filename             Template                Proj?   Game?   Dir
			// Makefile
			new ExportFile("Makefile",			"makefile.txt",			true,	false,	""),
			new ExportFile("%%_PNPROJ_%%",		"<unused>",				true,	false,	""),
			// Project files
			new ExportFile("animation.cpp",		"animation_cpp.txt",	true,	false,	"source"),
			new ExportFile("animation.h",		"animation_h.txt",		true,	false,	"source"),
			new ExportFile("game_state.h",		"game_state_h.txt",		true,	true,	"source"),
			new ExportFile("game_state.cpp",	"game_state_cpp.txt",	true,	true,	"source"),
			new ExportFile("game_utils.h",		"game_utils_h.txt",		true,	false,	"source"),
			new ExportFile("game_utils.cpp",	"game_utils_cpp.txt",	true,	false,	"source"),
			new ExportFile("collision.cpp",		"collision_cpp.txt",	true,	false,	"source"),
			new ExportFile("collision.h",		"collision_h.txt",		true,	false,	"source"),
			new ExportFile("main.cpp",			"main_cpp.txt",			true,	false,	"source"),
			//new ExportFile("oam_info.h",		"oam_info_h.txt",		true,	false,	"source"),
			new ExportFile("object_utils.cpp",	"object_utils_cpp.txt",	true,	false,	"source"),
			new ExportFile("object_utils.h",	"object_utils_h.txt",	true,	false,	"source"),
			new ExportFile("palettes.h",		"palettes_h.txt",		true,	false,	"source"),
			// Sprite/Background files
			new ExportFile("background_images.cpp",	"background_images_cpp.txt",false,	false,	"source"),
			new ExportFile("background_images.h",	"background_images_h.txt",	false,	false,	"source"),
			new ExportFile("background_maps.cpp",	"background_maps_cpp.txt",	false,	false,	"source"),
			new ExportFile("background_maps.h",		"background_maps_h.txt",	false,	false,	"source"),
			new ExportFile("sprites.cpp",			"sprites_cpp.txt",			false,	false,	"source"),
			new ExportFile("sprites.h",				"sprites_h.txt",			false,	false,	"source"),
		};

		public enum ExportResult
		{
			OK,
			Failed,
			Cancel,
		}

		public ExportResult ExportDialog(string strDocName, out string strProjectDir, out bool fExportProject)
		{
			strProjectDir = "";
			fExportProject = false;

			Export ex = new Export(strDocName);
			DialogResult result = ex.ShowDialog();
			if (result == DialogResult.Cancel)
				return ExportResult.Cancel;
			if (result != DialogResult.OK)
				return ExportResult.Failed;

			strProjectDir = ex.ExportLocation;
			fExportProject = ex.Project || ex.UpdateProject;
			bool fSkipGameState = ex.UpdateProject;

			bool fNDS = Options.Platform == Options.PlatformType.NDS;
			if (Export(strProjectDir, fExportProject, fNDS, fSkipGameState))
				return ExportResult.OK;

			return ExportResult.Failed;
		}

		public bool Export(string strProjectDir, bool fExportProject, bool fNDS, bool fSkipGameState)
		{
			// Since the strProjectDir doesn't end with a directory separator, this
			// returns the directory name.
			string strProjName = Path.GetFileName(strProjectDir);

			// Figure out which directory we should use.
			// In general, we use the directory selected by the user.
			// But, if:
			//   (1) we're exporting just sprites/backgrounds (not a complete project), and
			//   (2) the sprite/background files already exist in the "source" subdirectory
			// then we export the files into the "source" subdirectory instead of the selected directory.
			//
			// We need to consider the following use cases:
			//   (1) a. User exports a complete project into a "project" directory
			//       b. User re-exports just the sprites/backgrounds in to the "project" directory
			//       This is the common case. We expect the files in the "project/source" dir to be updated
			//       even though the user selected the "project" directory.
			//   (2) a. User starts a new project by accidentally exporting just the sprites/backgrounds
			//          into the "project" directory
			//       b. User notices mistake (when "make" doesn't work), and re-exports entire project into
			//          the "project" directory. This creates the "project/source" directory.
			//       c. There are now 2 copies of the sprite/background files: one in "project" and
			//          another in "project/source".
			//       d. User re-exports just the sprites/backgrounds into the "project" directory
			//       We expect the "project/source" files to be updated since they are the only ones
			//       recognized by the project makefile. The sprite/background files in the "project"
			//       directory are ignored (and should be deleted by the user).
			//   (3) a. User creates their own project from scratch and wants to use exported sprites.
			//       b. User exports sprites/backgrounds into their project directory.
			//       c. There is no "source" subdir (or it doesn't have sprites/backgrounds files), so
			//          files are written into the "project" directory.
			//       A similar situation is where the user exports sprites/backgrounds directly into the
			//       "project/source" directory.
			int nInSubdir = 0;

			if (!fExportProject)
			{
				// Lookup locations of any already-existing non-project files.
				foreach (ExportFile f in ExportFiles)
				{
					// This is a project-only file - ignore.
					if (f.ProjectFile)
						continue;

					// Does it already exist in the subdirectory?
					if (f.SubDirectory != "")
					{
						StringBuilder sbSubdirPath = new StringBuilder(strProjectDir);
						sbSubdirPath.Append(Path.DirectorySeparatorChar);
						sbSubdirPath.Append(f.SubDirectory);
						sbSubdirPath.Append(Path.DirectorySeparatorChar);
						sbSubdirPath.Append(f.Filename);
						if (System.IO.File.Exists(sbSubdirPath.ToString()))
							nInSubdir++;
					}
				}
			}

			// Should we ignore the subdir and write the files directly into the specified directory?
			bool fIgnoreSubdir = false;
			// Ignore the subdir if we're exporting just the sprite files and the sprite files don't
			// already exist in the "source" subdir
			if (!fExportProject && nInSubdir == 0)
				fIgnoreSubdir = true;

			foreach (ExportFile f in ExportFiles)
			{
				// Skip project files unless we're exporting a project.
				if (f.ProjectFile && !fExportProject)
					continue;

				// Don't overwrite the game_state files if we're updating a project.
				if (f.GameStateFile && fSkipGameState)
					continue;

				StringBuilder sb = new StringBuilder(strProjectDir);

				// Don't create the subdirectory unless we're
				if (!fIgnoreSubdir && f.SubDirectory != "")
				{
					sb.Append(Path.DirectorySeparatorChar);
					sb.Append(f.SubDirectory);

					if (!System.IO.Directory.Exists(sb.ToString()))
						System.IO.Directory.CreateDirectory(sb.ToString());
				}
				sb.Append(Path.DirectorySeparatorChar);

				// Special handling for the .pnproj file.
				if (f.Filename.Equals("%%_PNPROJ_%%"))
				{
					sb.Append(String.Format("{0}.pnproj", strProjName));
					if (!ExportPNProj(strProjName, sb.ToString()))
						return false;
					continue;
				}
				sb.Append(f.Filename);

				// TODO: if this is a project file, and it already exists and it is different from the
				// default (eg: changes made to main.cpp), then warn the user before overwriting.

				if (!ExportFromTemplate(strProjName, f.Template, sb.ToString(), fNDS))
					return false;
			}

			return true;
		}

		private bool ExportPNProj(string strProjName, string strOutputFilename)
		{
			TextWriter tw = null;

			try
			{
				try
				{
					tw = new StreamWriter(strOutputFilename);
				}
				catch (Exception ex)
				{
					// "An exception was thrown while opening the project file for writing: {0}"
					m_doc.ErrorId("ExceptionOpenProjectWrite", ex.Message);
					return false;
				}

				tw.Write(String.Format("<Project name=\"{0}\">", strProjName));
				foreach (ExportFile f in ExportFiles)
				{
					if (f.SubDirectory != "")
						tw.Write("<File path=\"{0}\\{1}\"></File>", f.SubDirectory, f.Filename);
				}
				tw.Write("</Project>");
			}
			finally
			{
				if (tw != null)
					tw.Dispose();
			}
			return true;
		}

		private bool ExportFromTemplate(string strProjName, string strTemplateFilename, string strOutputFilename, bool fNDS)
		{
			try 
			{
				string strExeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

				string strTemplateDir = String.Format("{0}{1}{2}{3}", strExeDir,
														Path.DirectorySeparatorChar,
														"templates",
														Path.DirectorySeparatorChar);

				using (TextReader tr = new StreamReader(strTemplateDir + strTemplateFilename, Encoding.UTF8))
				{
					using (TextWriter tw = new StreamWriter(strOutputFilename))
					{
						Spritesets sprites = m_doc.Spritesets;
						Spritesets bgsprites = m_doc.BackgroundSpritesets;
						Palettes palettes = m_doc.Palettes;
						Palettes bgpalettes = m_doc.BackgroundPalettes;
						Maps maps = m_doc.BackgroundMaps;
						BgImages bgimages = m_doc.BackgroundImages;

						sprites.Export_AssignIDs();
						bgsprites.Export_AssignIDs();
						palettes.Export_AssignIDs();
						bgpalettes.Export_AssignIDs();
						maps.Export_AssignIDs();
						bgimages.Export_AssignIDs();

						string strLine;
						while ((strLine = tr.ReadLine()) != null)
						{
							if (strLine.StartsWith("%%NDS:%%"))
							{
								if (fNDS)
									tw.WriteLine(strLine.Substring(8));
								continue;
							}
							if (strLine.StartsWith("%%GBA:%%"))
							{
								if (!fNDS)
									tw.WriteLine(strLine.Substring(8));
								continue;
							}

							if (strLine == "%%_SPRITE_PALETTE_INFO_%%")
							{
								palettes.Export_PaletteInfo(tw);
								continue;
							}
							if (strLine == "%%_SPRITE_PALETTES_%%")
							{
								palettes.Export_Palettes(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_PALETTE_INFO_%%")
							{
								bgpalettes.Export_PaletteInfo(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_PALETTES_%%")
							{
								bgpalettes.Export_Palettes(tw);
								continue;
							}

							if (strLine == "%%_SPRITESET_INFO_%%")
							{
								sprites.Export_SpritesetInfo(tw);
								continue;
							}
							if (strLine == "%%_SPRITESET_IDS_%%")
							{
								sprites.Export_SpritesetIDs(tw);
								continue;
							}

							if (strLine == "%%_SPRITE_INFO_%%")
							{
								sprites.Export_SpriteInfo(tw);
								continue;
							}
							if (strLine == "%%_SPRITE_TILES_%%")
							{
								sprites.Export_TileData(tw);
								continue;
							}
							if (strLine == "%%_SPRITE_MASKS_%%")
							{
								sprites.Export_SpriteMaskData(tw);
								continue;
							}
							if (strLine == "%%_SPRITE_IDS_%%")
							{
								sprites.Export_SpriteIDs(tw);
								continue;
							}

							if (strLine == "%%_BACKGROUND_TILESET_INFO_%%")
							{
								bgsprites.Export_BgTilesetInfo(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_TILESET_IDS_%%")
							{
								bgsprites.Export_BgTilesetIDs(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_TILES_%%")
							{
								bgsprites.Export_TileData(tw);
								continue;
							}

							if (strLine == "%%_BACKGROUND_TILE_IDS_%%")
							{
								bgsprites.Export_TileIDs(tw);
								continue;
							}

							if (strLine == "%%_BACKGROUND_MAP_INFO_%%")
							{
								maps.Export_MapInfo(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_MAP_IDS_%%")
							{
								maps.Export_MapIDs(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_MAP_DATA_%%")
							{
								maps.Export_MapData(tw);
								continue;
							}

							if (strLine == "%%_BACKGROUND_IMAGE_INFO_%%")
							{
								bgimages.Export_BgImageInfo(tw, fNDS);
								continue;
							}
							if (strLine == "%%_BACKGROUND_IMAGE_IDS_%%")
							{
								bgimages.Export_BgImageIDs(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_IMAGE_HEADERS_%%")
							{
								bgimages.Export_BgImageHeaders(tw, fNDS);
								continue;
							}
							if (strLine == "%%_BACKGROUND_IMAGE_PALETTEDATA_%%")
							{
								bgimages.Export_BgImagePaletteData(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_IMAGE_DATA_PALETTED_%%")
							{
								bgimages.Export_BgImageData_Paletted(tw);
								continue;
							}
							if (strLine == "%%_BACKGROUND_IMAGE_DATA_DIRECT_%%")
							{
								bgimages.Export_BgImageData_Direct(tw);
								continue;
							}

							strLine = strLine.Replace("%%_NAME_%%", strProjName);
							strLine = strLine.Replace("%%_VERSION_%%", ResourceMgr.GetString("Version"));
							strLine = strLine.Replace("%%_PLATFORM_%%", fNDS ? "NDS" : "GBA");

							strLine = strLine.Replace("%%_NUM_PALETTES_%%", palettes.NumPalettes.ToString());
							strLine = strLine.Replace("%%_NUM_SPRITESETS_%%", sprites.NumSpritesets.ToString());
							strLine = strLine.Replace("%%_NUM_SPRITES_%%", sprites.NumSprites.ToString());
							strLine = strLine.Replace("%%_NUM_TILES_%%", sprites.NumTiles.ToString());

							strLine = strLine.Replace("%%_NUM_BACKGROUND_PALETTES_%%", bgpalettes.NumPalettes.ToString());
							strLine = strLine.Replace("%%_NUM_BACKGROUND_TILESETS_%%", bgsprites.NumSpritesets.ToString());
							strLine = strLine.Replace("%%_NUM_BACKGROUND_TILES_%%", bgsprites.NumTiles.ToString());

							strLine = strLine.Replace("%%_NUM_BACKGROUND_MAPS_%%", maps.NumMaps.ToString());

							strLine = strLine.Replace("%%_NUM_BACKGROUND_IMAGES_%%", bgimages.NumImages.ToString());

							tw.WriteLine(strLine);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// "An exception was thrown while load the project template files: {0}"
				m_doc.ErrorId("ExceptionLoadTemplate", ex.Message);
				// "An exception was thrown while opening the project file for writing: {0}"
				//m_doc.ErrorId("ExceptionOpenProjectWrite", ex.Message);
				return false;
			}

			return true;
		}

	}
}
