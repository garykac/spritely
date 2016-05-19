using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Spritely
{
	public class Tile
	{
		private Sprite m_sprite;
		private int m_id;

		// This class should contain all of the user-editable data for the Tile.
		// It is used by the Undo class
		public class UndoData
		{
			public int tilesize;
			public int[,] pixels;

			private UndoData() { }

			public UndoData(int nTileSize)
			{
				tilesize = nTileSize;
				pixels = new int[nTileSize, nTileSize];
			}

			public UndoData(UndoData data)
			{
				tilesize = data.tilesize;
				pixels = new int[tilesize, tilesize];
				for (int iRow = 0; iRow < tilesize; iRow++)
					for (int iColumn = 0; iColumn < tilesize; iColumn++)
						pixels[iColumn, iRow] = data.pixels[iColumn, iRow];
			}

			public bool Equals(UndoData data)
			{
				if (tilesize != data.tilesize)
					return false;
				for (int iRow = 0; iRow < tilesize; iRow++)
					for (int iColumn = 0; iColumn < tilesize; iColumn++)
						if (pixels[iColumn, iRow] != data.pixels[iColumn, iRow])
							return false;

				return true;
			}
		}
		private UndoData m_data;

		private bool m_fHasSmallBitmap = false;
		private Bitmap m_bmSmall = null;

		/// <summary>
		/// Number of pixels in a tile (x or y).
		/// </summary>
		public const int TileSize = 8;

		private int m_nExportId;

		public Tile(Sprite s, int nTileId)
		{
			m_sprite = s;
			m_id = nTileId;
			m_data = new UndoData(TileSize);

			// This will be assigned when the spriteset is saved or exported.
			m_nExportId = 0;
		}

		/// <summary>
		/// The internal tile id for use when editing.
		/// </summary>
		public int TileId
		{
			get { return m_id; }
		}

		/// <summary>
		/// The tile id used when exporting tiles.
		/// </summary>
		public int ExportId
		{
			get { return m_nExportId; }
		}

		public const int SmallBitmapPixelSize = 2;

		// size of small bitmap
		public const int SmallBitmapScreenSize = SmallBitmapPixelSize * TileSize;
		
		//public static int BigBitmapScreenSize
		//{
		//	get { return BigBitmapPixelSize * TileSize; }
		//}

		/// <summary>
		/// Get the subpalette for this tile
		/// </summary>
		/// <returns></returns>
		public Subpalette GetSubpalette()
		{
			return m_sprite.Subpalette;
		}

		/// <summary>
		/// Get the pixel at the given (x,y) coord.
		/// Assumes that x,y are valid pixel coords.
		/// </summary>
		/// <returns>The rgb color value of the specified pixel</returns>
		public Color GetPixelColor(int pxX, int pxY)
		{
			Subpalette p = m_sprite.Subpalette;
			int nIndex = m_data.pixels[pxX, pxY];
			int cRed = p.Red(nIndex);
			int cGreen = p.Green(nIndex);
			int cBlue = p.Blue(nIndex);
			return Color.FromArgb(cRed * 8, cGreen * 8, cBlue * 8);
		}

		/// <summary>
		/// Get the pixel at the given (x,y) coord.
		/// Assumes that x,y are valid pixel coords.
		/// </summary>
		/// <returns>The value (index into palette) of the specified pixel</returns>
		public int GetPixel(int pxX, int pxY)
		{
			return m_data.pixels[pxX, pxY];
		}

		/// <summary>
		/// Set the pixel to the given color (index into palette).
		/// Assumes that (x,y) are valid pixel coordinates.
		/// Does *not* flush the tile bitmaps. You will need to do that manually.
		/// </summary>
		/// <param name="pxX"></param>
		/// <param name="pxY"></param>
		/// <param name="color"></param>
		public void SetPixel(int pxX, int pxY, int color)
		{
			m_data.pixels[pxX, pxY] = color;
		}

		public void Duplicate(Tile tileToCopy)
		{
			CopyData(tileToCopy);

			m_sprite = tileToCopy.m_sprite;
		}

		/// <summary>
		/// Is the tile empty?
		/// </summary>
		/// <returns>True if the tile has no data</returns>
		public bool IsEmpty()
		{
			// Check each pixel in the tile for emptiness
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					if (GetPixel(iColumn, iRow) != 0)
						return false;
				}
			}

			// All of the pixels are transparent, therefore the tile is empty
			return true;
		}

		public Bitmap SmallBitmap()
		{
			if (m_fHasSmallBitmap)
				return m_bmSmall;

			Bitmap bm = CreateBitmap(SmallBitmapPixelSize);
			m_bmSmall = bm;
			m_fHasSmallBitmap = true;
			return bm;
		}

		public void DrawTransparentTile(Graphics g, Color cTrans, int pxOriginX, int pxOriginY)
		{
			Rectangle dst = new Rectangle(pxOriginX, pxOriginY, Tile.SmallBitmapScreenSize, Tile.SmallBitmapScreenSize);
			g.DrawImage(CreateTransparentBitmap(SmallBitmapPixelSize, cTrans), dst);
		}

		public void DrawSmallTile(Graphics g, int pxOriginX, int pxOriginY)
		{
			Rectangle dst = new Rectangle(pxOriginX, pxOriginY, Tile.SmallBitmapScreenSize, Tile.SmallBitmapScreenSize);
			g.DrawImage(SmallBitmap(), dst);
		}

		// create a bitmap from this tile data
		private Bitmap CreateBitmap(int pxSize)
		{
			// create a new bitmap
			Bitmap bm = new Bitmap(TileSize * pxSize, TileSize * pxSize);//, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bm);
			Subpalette sp = m_sprite.Subpalette;

			g.FillRectangle(Brushes.White, 0, 0, TileSize * pxSize, TileSize * pxSize);
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					g.FillRectangle(sp.Brush(GetPixel(iColumn, iRow)), iColumn * pxSize, iRow * pxSize, pxSize, pxSize);
				}
			}

			return bm;
		}

		// create a bitmap from this tile data
		private Bitmap CreateTransparentBitmap(int pxSize, Color cTrans)
		{
			// create a new bitmap
			Bitmap bm = new Bitmap(TileSize * pxSize, TileSize * pxSize);//, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bm);
			Subpalette sp = m_sprite.Subpalette;

			g.FillRectangle(Brushes.White, 0, 0, TileSize * pxSize, TileSize * pxSize);
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					g.FillRectangle(sp.Brush(GetPixel(iColumn, iRow)), iColumn * pxSize, iRow * pxSize, pxSize, pxSize);
				}
			}

			bm.MakeTransparent(cTrans);
			return bm;
		}

		// mark the bitmaps as invalid
		public void FlushBitmaps()
		{
			if (m_fHasSmallBitmap)
			{
				m_bmSmall.Dispose();
				m_bmSmall = null;
				m_fHasSmallBitmap = false;
			}
		}

		/// <summary>
		/// Erase all of the pixel data in the tile.
		/// </summary>
		public void Clear()
		{
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
					SetPixel(iColumn, iRow, 0);
			}
			FlushBitmaps();
		}

		/// <summary>
		/// Copy the data from the specified tile into this one
		/// </summary>
		/// <param name="t"></param>
		public void CopyData(Tile t)
		{
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
					SetPixel(iColumn, iRow, t.GetPixel(iColumn, iRow));
			}
			FlushBitmaps();
		}

		public UndoData GetUndoData()
		{
			UndoData undo = new UndoData(TileSize);
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
					undo.pixels[iColumn, iRow] = GetPixel(iColumn, iRow);
			}
			return undo;
		}

		public void ApplyUndoData(UndoData undo)
		{
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
					SetPixel(iColumn, iRow, undo.pixels[iColumn, iRow]);
			}
		}

		#region Save/Import/Export

		public void Save(System.IO.TextWriter tw, int nTileID)
		{
			tw.Write("\t\t\t\t<tile");
			tw.Write(String.Format(" id=\"{0}\"", nTileID));
			tw.WriteLine(">");

			StringBuilder sb = null;
			int nPerLine = 1;
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				if (iRow % nPerLine == 0)
				{
					if (sb != null)
					{
						sb.Append("</tilerow>");
						tw.WriteLine(sb.ToString());
					}
					sb = new StringBuilder("\t\t\t\t\t<tilerow>");
				}
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					byte b1, b2;
					int n = GetPixel(iColumn, iRow);
					b1 = (byte)n;
					iColumn++;
					n = GetPixel(iColumn, iRow);
					b2 = (byte)n;
					byte b = (byte)((b2 << 4) + b1);
					sb.Append(String.Format("{0},", b1));
					sb.Append(String.Format(iColumn == TileSize - 1 ? "{0}" : "{0},", b2));
				}
			}
			if (sb != null)
			{
				sb.Append("</tilerow>");
				tw.WriteLine(sb.ToString());
			}

			tw.WriteLine("\t\t\t\t</tile>");
		}

		/// <summary>
		/// Import tile from an array of 64 integers.
		/// </summary>
		/// <param name="b"></param>
		public void Import(int[] b)
		{
			int iByte = 0;
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					int nData = b[iByte++];
					SetPixel(iColumn, iRow, (nData & 0x0f));
				}
			}
			FlushBitmaps();
		}

		/// <summary>
		/// Import tile from an array of 32 integers.
		/// Each integer contains data for 2 pixels (4-bits per pixel).
		/// </summary>
		/// <param name="b"></param>
		public void Import32(uint[] b)
		{
			int iByte = 0;
			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; )
				{
					uint nData = b[iByte++];
					SetPixel(iColumn, iRow, (int)(nData & 0x0f));
					nData >>= 4;
					SetPixel(iColumn + 1, iRow, (int)(nData & 0x0f));
					iColumn += 2;
				}
			}
			FlushBitmaps();
		}

		public void Export_AssignIDs(int nTileId)
		{
			m_nExportId = nTileId;
		}

		public void Export_TileIDs(System.IO.TextWriter tw, string strSpriteset, string strSprite,
			int nTileIndex, int nTotalTiles)
		{
			if (nTotalTiles == 1)
				tw.WriteLine(String.Format("const int k{0}_{1} = {2};", strSpriteset, strSprite, m_nExportId));
			else
				tw.WriteLine(String.Format("const int k{0}_{1}_{2} = {3};", strSpriteset, strSprite, nTileIndex, m_nExportId));
		}

		// Export_TileData
		// Export the tile as a string so that we can write it out to a C-source file.
		// For each pixel, we write out the corresponding palette index.
		// For a 16-color palette, each index is 4 bits (1 nybble), so we can fit 2 pixels in a single byte.
		public bool Export_TileData(System.IO.TextWriter tw, int nIndex)
		{
			// The number of tile rows that should be grouped together on a single line.
			const int kExportByteGroup = 1;// 4;

			tw.WriteLine("\t// Tile #" + nIndex.ToString());

			StringBuilder sb = new StringBuilder("\t");

			for (int iRow = 0; iRow < TileSize; iRow++)
			{
				for (int iColumn = 0; iColumn < TileSize; iColumn++)
				{
					byte b1, b2;

					// Read 2 pixel values (the palette index for each pixel) so that we can
					// combine them into a single byte.
					int n = GetPixel(iColumn, iRow);
					b1 = (byte)n;
					iColumn++;
					n = GetPixel(iColumn, iRow);
					b2 = (byte)n;

					// Because the GBA is little-endian, we need to swap the 2 nybbles:
					//   the right pixel value is stored in the upper nybble
					//   the left pixel value is stored in the lower nybble 
					byte b = (byte)((b2 << 4) + b1);
					sb.Append(String.Format("0x{0:x2},", b));
				}
				if ((iRow != TileSize-1)
					&& ((iRow+1) % kExportByteGroup == 0))
				{
					tw.WriteLine(sb.ToString());
					sb.Length = 0;
					sb.Append("\t");
				}
			}
			tw.WriteLine(sb.ToString());
			return true;
		}

		#endregion

	}
}
