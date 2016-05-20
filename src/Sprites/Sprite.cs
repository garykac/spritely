using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class Sprite
	{
		private Document m_doc;
		private Spriteset m_ss;

		private Tile[] m_Tiles;
		private string m_strName;
		private string m_strDesc;

		/// <summary>
		/// Width of the sprite (in tiles)
		/// </summary>
		private int m_tileWidth = 1;

		/// <summary>
		/// Height of the sprite (in tiles)
		/// </summary>
		private int m_tileHeight = 1;

		/// <summary>
		/// This class should contain all of the user-editable data for the Sprite.
		/// It is used by the Undo class
		/// </summary>
		public class UndoData
		{
			public int subpalette;
			public Tile.UndoData[] tiles;
			public string name;
			public string desc;
			public int width, height;

			// Private to avoid accidental use.
			private UndoData() { }

			public UndoData(int nWidth, int nHeight)
			{
				subpalette = 0;
				name = "";
				desc = "";
				width = nWidth;
				height = nHeight;

				int nTiles = nWidth * nHeight;
				tiles = new Tile.UndoData[nTiles];
			}

			public UndoData(UndoData data)
			{
				subpalette = data.subpalette;
				name = data.name;
				desc = data.desc;
				width = data.width;
				height = data.height;

				int nTiles = width * height;
				tiles = new Tile.UndoData[nTiles];
				for (int i = 0; i < nTiles; i++)
					tiles[i] = new Tile.UndoData(data.tiles[i]);
			}

			public bool Equals(UndoData data)
			{
				if (subpalette != data.subpalette
					|| name != data.name
					|| desc != data.desc
					|| width != data.width
					|| height != data.height
					)
					return false;

				int nTiles = width * height;
				for (int i = 0; i < nTiles; i++)
					if (!tiles[i].Equals(data.tiles[i]))
						return false;

				return true;
			}
		}

		/// <summary>
		/// A snapshot of the sprite data from the last undo checkpoint.
		/// </summary>
		private UndoData m_snapshot;

		/// <summary>
		/// Exported ID of the sprite.
		/// This is only assigned when exporting the sprites.
		/// </summary>
		private int m_nExportSpriteID = 0;

		/// <summary>
		/// ID of the first tile for this sprite.
		/// This is only assigned when exporting the sprites.
		/// </summary>
		private int m_nExportFirstTileID = 0;

		/// <summary>
		/// Index into sprite mask array of first entry for this sprite.
		/// </summary>
		private int m_nMaskIndex = 0;

		//  SQUARE  00    SIZE_8   00    8 x 8              1
		//  SQUARE  00    SIZE_16  01    16 x 16            4
		//  SQUARE  00    SIZE_32  10    32 x 32            16
		//  SQUARE  00    SIZE_64  11    64 x 64            64
		//   WIDE   01    SIZE_8   00    16 x 8             2
		//   WIDE   01    SIZE_16  01    32 x 8             4
		//   WIDE   01    SIZE_32  10    32 x 16            8
		//   WIDE   01    SIZE_64  11    64 x 32            32
		//   TALL   10    SIZE_8   00    8 x 16             2
		//   TALL   10    SIZE_16  01    8 x 32             4
		//   TALL   10    SIZE_32  10    16 x 32            8
		//   TALL   10    SIZE_64  11    32 x 64            32
		public enum GBASize
		{
			Size8,
			Size16,
			Size32,
			Size64,
		}

		public enum GBAShape
		{
			Square,
			Wide,
			Tall,
		}

		public Sprite(Document doc, Spriteset ts, int nWidth, int nHeight, string strName, int id, string strDesc)
		{
			Init(doc, ts, nWidth, nHeight, strName, id, strDesc, 0);
		}

		// This assumes that width x height are valid.
		public Sprite(Document doc, Spriteset ts, int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette)
		{
			Init(doc, ts, nWidth, nHeight, strName, id, strDesc, nSubpalette);
		}

		public void Init(Document doc, Spriteset ss, int nWidth, int nHeight, string strName, int id, string strDesc, int nSubpalette)
		{
			m_doc = doc;
			m_ss = ss;

			if (strName == ""
				|| ss.HasNamedSprite(strName)
				)
				strName = ss.AutoGenerateSpriteName();
			m_strName = strName;

			m_strDesc = strDesc;
			SubpaletteID = nSubpalette;
			m_tileWidth = nWidth;
			m_tileHeight = nHeight;

			int nTiles = NumTiles;
			m_Tiles = new Tile[nTiles];
			for (int i=0; i < nTiles; i++)
				m_Tiles[i] = new Tile(this, ss.NextTileId++);

			// Make an initial snapshot of the (empty) sprite.
			m_snapshot = GetUndoData();
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;
		}

		public void Duplicate(Sprite sToCopy)
		{
			CopyData(sToCopy);
			SubpaletteID = sToCopy.SubpaletteID;
		}

		/// <summary>
		/// Copy the tile data from the specified sprite
		/// </summary>
		/// <param name="sToCopy">Sprite to copy</param>
		public void CopyData(Sprite sToCopy)
		{
			// Make sure that the sprites have the same dimensions
			if (TileWidth != sToCopy.TileWidth || TileHeight != sToCopy.TileHeight)
				return;

			// Copy over the tile data
			for (int i = 0; i < NumTiles; i++)
				m_Tiles[i].CopyData(sToCopy.m_Tiles[i]);
		}

		/// <summary>
		/// Width of sprite (in tiles)
		/// </summary>
		public int TileWidth
		{
			get { return m_tileWidth; }
		}

		/// <summary>
		/// Height of sprite (in tiles)
		/// </summary>
		public int TileHeight
		{
			get { return m_tileHeight; }
		}

		/// <summary>
		/// Total number of tiles for sprite
		/// </summary>
		public int NumTiles
		{
			get { return m_tileWidth * m_tileHeight; }
		}

		/// <summary>
		/// Width of sprite (in pixels)
		/// </summary>
		public int PixelWidth
		{
			get { return m_tileWidth * Tile.TileSize; }
		}

		/// <summary>
		/// Height of sprite (in pixels)
		/// </summary>
		public int PixelHeight
		{
			get { return m_tileHeight * Tile.TileSize; }
		}

		public bool IsSize(int tileWidth, int tileHeight)
		{
			return tileWidth == m_tileWidth && tileHeight == m_tileHeight;
		}

		/// <summary>
		/// The tile ID for the first tile in this sprite.
		/// This is the internal tile id and should not be exported. Instead, use 
		/// ExportFirstTileId for this purpose.
		/// </summary>
		public int FirstTileId
		{
			get { return m_Tiles[0].TileId; }
		}

		/// <summary>
		/// The exportable tile ID for the first tile in this sprite.
		/// This value is valid only after the sprites have been loaded, exported or saved.
		/// Editing operations (adding/removing sprites) will cause this to become invalid.
		/// </summary>
		public int ExportFirstTileId
		{
			get { return m_nExportFirstTileID; }
		}

		public string Name
		{
			get { return m_strName; }
			set { m_strName = value; }
		}

		public string Description
		{
			get { return m_strDesc; }
			set { m_strDesc = value; }
		}

		private int m_nSubpaletteID;
		public int SubpaletteID
		{
			get { return m_nSubpaletteID; }
			set {
				m_nSubpaletteID = value;
				if (m_ss.CurrentSprite == this && m_ss.Palette.PaletteType == Palette.Type.Color16)
				{
					Palette16 p16 = m_ss.Palette as Palette16;
					p16.CurrentSubpaletteId = m_nSubpaletteID;
				}
			}
		}

		//public Palette Palette
		//{
		//	get { return m_sl.Palette; }
		//}

		public Subpalette Subpalette
		{
			get {
				if (m_ss.Palette.PaletteType != Palette.Type.Color16)
					return null;
				return (m_ss.Palette as Palette16).GetSubpalette(SubpaletteID);
			}
		}

		public Tile GetTile(int nTileIndex)
		{
			if (nTileIndex < 0 || nTileIndex >= NumTiles)
				nTileIndex = 0;
			return m_Tiles[nTileIndex];
		}

		public Color GetPixelColor(int pxSpriteX, int pxSpriteY)
		{
			// Convert sprite pixel coords (x,y) into tile index (x,y) and tile pixel coords (x,y).
			int tileX = pxSpriteX / Tile.TileSize;
			int pxX = pxSpriteX % Tile.TileSize;
			int tileY = pxSpriteY / Tile.TileSize;
			int pxY = pxSpriteY % Tile.TileSize;

			int nTileIndex = (tileY * TileWidth) + tileX;

			return m_Tiles[nTileIndex].GetPixelColor(pxX, pxY);
		}

		public int GetPixel(int pxSpriteX, int pxSpriteY)
		{
			// Convert sprite pixel coords (x,y) into tile index (x,y) and tile pixel coords (x,y).
			int tileX = pxSpriteX / Tile.TileSize;
			int pxX = pxSpriteX % Tile.TileSize;
			int tileY = pxSpriteY / Tile.TileSize;
			int pxY = pxSpriteY % Tile.TileSize;

			int nTileIndex = (tileY * TileWidth) + tileX;

			return m_Tiles[nTileIndex].GetPixel(pxX, pxY);
		}

		public void SetPixel(int pxSpriteX, int pxSpriteY, int color)
		{
			// Convert sprite pixel coords (x,y) into tile index (x,y) and tile pixel coords (x,y).
			int tileX = pxSpriteX / Tile.TileSize;
			int pxX = pxSpriteX % Tile.TileSize;
			int tileY = pxSpriteY / Tile.TileSize;
			int pxY = pxSpriteY % Tile.TileSize;

			int nTileIndex = (tileY * TileWidth) + tileX;

			m_Tiles[nTileIndex].SetPixel(pxX, pxY, color);
		}

		/// <summary>
		/// Is the sprite empty?
		/// </summary>
		/// <returns>True if the sprite has no data</returns>
		public bool IsEmpty()
		{
			int nTiles = NumTiles;

			// Check each tile for emptiness
			for (int i = 0; i < nTiles; i++)
				if (!m_Tiles[i].IsEmpty())
					return false;

			// All the tiles are empty, therefore the sprite is empty
			return true;
		}

		public bool Resize(int tileWidth, int tileHeight)
		{
			// If the new size == the old size
			if (tileWidth == m_tileWidth && tileHeight == m_tileHeight)
				return false;

			int oldX = m_tileWidth;		// AKA: TileWidth
			int oldY = m_tileHeight;	// AKA: TileHeight
			int old_tiles = m_tileWidth * m_tileHeight;	// AKA: NumTiles
			int newX = tileWidth;
			int newY = tileHeight;
			int new_tiles = tileWidth * tileHeight;

			// If we are clipping any tiles with the new size
			if (oldX > newX || oldY > newY)
			{
				// Check to see if any of the clipped tiles contain data
				bool fHasData = false;

				// Handle case where the old sprite is wider than the new sprite:
				// e.g. old=4,2  xxOO     old=4,2  xxOO    old=4,4  xxOO
				//      new=2,4  xxOO     new=2,2  xxOO    new=2,2  xxOO
				//               nn                                 ooOO
				//               nn                                 ooOO
				// (O = tile only in old sprite - checked by this loop,
				//  o = tile only in old sprite - NOT checked by this loop,
				//  n = tile only in new sprite - not checked,
				//  x = tile shared in both sprites - not checked)
				for (int ix = newX; ix < oldX; ix++)
				{
					for (int iy = 0; iy < oldY; iy++)
					{
						if (!m_Tiles[(iy * oldX) + ix].IsEmpty())
							fHasData = true;
					}
				}

				// Handle the case where the old sprite is taller than the new sprite:
				// e.g. old=2,4  xxnn     old=2,4  xx      old=4,4  xxoo
				//      new=4,2  xxnn     new=2,2  xx      new=2,2  xxoo
				//               OO                OO               OOoo
				//               OO                OO               OOoo
				// Note that we can cut the x-loop short when oldX > newX since we've covered that case
				// in the above loop.
				int minX = (oldX < newX ? oldX : newX);
				for (int iy = newY; iy < oldY; iy++)
				{
					for (int ix = 0; ix < minX; ix++)
					{
						if (!m_Tiles[(iy * oldX) + ix].IsEmpty())
							fHasData = true;
					}
				}

				// Make sure it's OK with the user before discarding clipped data
				//if (fHasData && !m_doc.AskYesNo("Resizing the sprite will cause some tiles to be clipped and the associated data will be lost.\nAre you sure you want to resize the sprite?"))
				if (fHasData && !m_doc.AskYesNo(ResourceMgr.GetString("WarnSpriteResizeClip")))
					return false;
			}

			// Allocate the new tiles (copying over from the current tiles as needed)
			Tile[] newTiles = new Tile[new_tiles];
			for (int ix = 0; ix < newX; ix++)
			{
				for (int iy = 0; iy < newY; iy++)
				{
					newTiles[(iy * newX) + ix] = new Tile(this, m_ss.NextTileId++);
					if (ix < oldX && iy < oldY)
						newTiles[(iy * newX) + ix].CopyData(m_Tiles[(iy * oldX) + ix]);
				}
			}

			// Switch over to the newly resized tile data
			m_tileWidth = newX;
			m_tileHeight = newY;
			m_Tiles = newTiles;

			return true;
		}

		public enum RotateDirection
		{
			Clockwise90 = -1,
			Counterclockwise90 = 1,
			Clockwise180 = 2
		}

		/// <summary>
		/// Rotate the sprite by the offset.
		/// </summary>
		/// <param name="dir">Direction to rotate.</param>
		public bool Rotate(RotateDirection dir)
		{
			if (PixelWidth == PixelHeight)
				return RotateSquareInPlace(dir);

			if (dir == RotateDirection.Clockwise180)
				return RotateRect180InPlace();

			// Rotate rect sprite:
			int newX = TileHeight;
			int newY = TileWidth;
			int num_tiles = newX * newY;

			// Allocate the new tiles.
			Tile[] newTiles = new Tile[num_tiles];
			for (int i = 0; i < num_tiles; i++)
				newTiles[i] = new Tile(this, m_ss.NextTileId++);

			// Rotate/copy data into new tiles.
			int pxOldWidth = PixelWidth;
			int pxOldHeight = PixelHeight;
			for (int ix = 0; ix < pxOldWidth; ix++)
			{
				for (int iy = 0; iy < pxOldHeight; iy++)
				{
					int y = iy;
					if (dir == RotateDirection.Clockwise90)
						y = pxOldHeight-1 - iy;

					// Calc tile/pixel index for rotated sprite.
					// Note that x,y are reversed for the rotated sprite.
					int tileNewX = y / Tile.TileSize;
					int pxNewX = y % Tile.TileSize;
					int tileNewY = ix / Tile.TileSize;
					int pxNewY = ix % Tile.TileSize;
					int nTileIndex = (tileNewY * TileHeight) + tileNewX;

					newTiles[nTileIndex].SetPixel(pxNewX, pxNewY, GetPixel(ix,iy));
				}
			}

			// Switch over to the newly created tile data.
			m_tileWidth = newX;
			m_tileHeight = newY;
			m_Tiles = newTiles;

			return true;
		}

		// Rotate the square sprite in place (without reallocating new tiles).
		private bool RotateSquareInPlace(RotateDirection dir)
		{
			int offset = (int)dir;

			// Rotate the sprite from the outer ring of pixels to the inner ring.
			// E.g.:   aaaaaaaa  First the 'a' ring is processed
			//         abbbbbba  Then the 'b' ring
			//         abccccba  Then 'c'
			//         abcddcba  And finally 'd'
			//         abcddcba
			//         abccccba
			//         abbbbbba
			//         aaaaaaaa
			//
			// For each ring, we process as follows:
			//     012340  First, we record the 4 orig pixels at '0' 
			//     4    1  Then we rotate these pixels and write them to the
			//     3    2    new location. E.g., for clockwise rotation, the
			//     2    3    upper-left '0' is written to the upper-right '0',
			//     1    4    upper-right '0' -> lower-right '0', and so on.
			//     043210  This continues for the 4 '1's, the 4 '2's, ...
			//
			int[] vals = new int[4];
			int pxSize = PixelWidth;
			int rings = pxSize / 2;
			for (int ring = 0; ring < rings; ring++)
			{
				int pxFirst = ring;
				int pxLast = pxSize - ring - 1;
				int pxCount = pxLast - pxFirst;
				for (int px = 0; px < pxCount; px++)
				{
					// Record the original values of the pixels to be rotated.
					vals[0] = GetPixel(pxFirst + px, pxFirst);
					vals[1] = GetPixel(pxLast, pxFirst + px);
					vals[2] = GetPixel(pxLast - px, pxLast);
					vals[3] = GetPixel(pxFirst, pxLast - px);

					// Rotate the pixels
					SetPixel(pxFirst + px, pxFirst, vals[(4 + offset) % 4]);
					SetPixel(pxLast, pxFirst + px, vals[(5 + offset) % 4]);
					SetPixel(pxLast - px, pxLast, vals[(6 + offset) % 4]);
					SetPixel(pxFirst, pxLast - px, vals[(7 + offset) % 4]);
				}
			}

			return true;
		}

		// Rotate the non-square sprite in place by 180 degrees.
		private bool RotateRect180InPlace()
		{
			// Rotate a row at a time, top <-> bottom:
			//    aaaaaaaa  
			//    bbbbbbbb
			//    bbbbbbbb
			//    aaaaaaaa
			// For each row, we process as follows:
			//    01234567
			//    ...
			//    76543210
			//
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;
			int rows = PixelHeight / 2;
			for (int row = 0; row < rows; row++)
			{
				int pxFirst = row;
				int pxLast = pxHeight - row - 1;
				for (int px = 0; px < pxWidth; px++)
				{
					// Record the original values of the pixels to be rotated.
					int top = GetPixel(px, pxFirst);
					int bottom = GetPixel(pxWidth-1 - px, pxLast);

					// Rotate the pixels
					SetPixel(px, pxFirst, bottom);
					SetPixel(pxWidth-1 - px, pxLast, top);
				}
			}

			return true;
		}

		public void Flip(bool fHorizontal, bool fVertical)
		{
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;

			if (fHorizontal && fVertical)
			{
				// Flipping along both axes is the same as rotating 180 degrees,
				// so we use that routine since it loops through the pixels only once.
				RotateRect180InPlace();
				return;
			}

			if (fHorizontal)
			{
				for (int pxY = 0; pxY < pxHeight; pxY++)
					for (int pxX = 0; pxX < pxWidth / 2; pxX++)
					{
						int nColor = GetPixel(pxX, pxY);
						SetPixel(pxX, pxY, GetPixel(pxWidth - 1 - pxX, pxY));
						SetPixel(pxWidth - 1 - pxX, pxY, nColor);
					}
			}

			if (fVertical)
			{
				for (int pxX = 0; pxX < pxWidth; pxX++)
					for (int pxY = 0; pxY < pxHeight / 2; pxY++)
					{
						int nColor = GetPixel(pxX, pxY);
						SetPixel(pxX, pxY, GetPixel(pxX, pxHeight - 1 - pxY));
						SetPixel(pxX, pxHeight - 1 - pxY, nColor);
					}
			}
		}

		public void ShiftPixels(Arrowbox.ShiftArrow shift)
		{
			if (shift == Arrowbox.ShiftArrow.Left)
				ShiftPixels_Left();
			if (shift == Arrowbox.ShiftArrow.Right)
				ShiftPixels_Right();
			if (shift == Arrowbox.ShiftArrow.Up)
				ShiftPixels_Up();
			if (shift == Arrowbox.ShiftArrow.Down)
				ShiftPixels_Down();
			FlushBitmaps();
		}

		public void ShiftPixels_Left()
		{
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;

			for (int pxX = 1; pxX < pxWidth; pxX++)
				for (int pxY = 0; pxY < pxHeight; pxY++)
					SetPixel(pxX - 1, pxY, GetPixel(pxX, pxY));
			for (int pxY = 0; pxY < pxHeight; pxY++)
				SetPixel(pxWidth - 1, pxY, 0);
		}

		public void ShiftPixels_Right()
		{
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;

			for (int pxX = pxWidth-1; pxX > 0; pxX--)
				for (int pxY = 0; pxY < pxHeight; pxY++)
					SetPixel(pxX, pxY, GetPixel(pxX-1, pxY));
			for (int pxY = 0; pxY < pxHeight; pxY++)
				SetPixel(0, pxY, 0);
		}

		public void ShiftPixels_Up()
		{
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;

			for (int pxY = 1; pxY < pxHeight; pxY++)
				for (int pxX = 0; pxX < pxWidth; pxX++)
					SetPixel(pxX, pxY-1, GetPixel(pxX, pxY));
			for (int pxX = 0; pxX < pxWidth; pxX++)
				SetPixel(pxX, pxHeight-1, 0);
		}

		public void ShiftPixels_Down()
		{
			int pxWidth = PixelWidth;
			int pxHeight = PixelHeight;

			for (int pxY = pxHeight - 1; pxY > 0; pxY--)
				for (int pxX = 0; pxX < pxWidth; pxX++)
					SetPixel(pxX, pxY, GetPixel(pxX, pxY-1));
			for (int pxX = 0; pxX < pxWidth; pxX++)
				SetPixel(pxX, 0, 0);
		}

		#region FloodFill tool

		public class PixelCoord
		{
			public int X, Y;

			public PixelCoord(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		/// <summary>
		/// Perform a floodfill click at the specified (x,y) pixel coords
		/// </summary>
		/// <param name="pxSpriteX">Click x-position (in pixels)</param>
		/// <param name="pxSpriteY">Click y-position (in pixels)</param>
		/// <returns>True if the sprite changes as a result of this click, false otherwise.</returns>
		public bool FloodFillClick(int pxSpriteX, int pxSpriteY)
		{
			int colorOld = GetPixel(pxSpriteX, pxSpriteY);
			int colorNew = Subpalette.CurrentColor;
			if (colorOld == colorNew)
				return false;

			// Stack of pixels to process.
			Stack<PixelCoord> stackPixels = new Stack<PixelCoord>();

			PixelCoord p = new PixelCoord(pxSpriteX, pxSpriteY);
			stackPixels.Push(p);

			while (stackPixels.Count != 0)
			{
				p = stackPixels.Pop();
				SetPixel(p.X, p.Y, colorNew);

				FloodFill_CheckPixel(p.X - 1, p.Y, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X + 1, p.Y, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X, p.Y - 1, colorOld, ref stackPixels);
				FloodFill_CheckPixel(p.X, p.Y + 1, colorOld, ref stackPixels);
			}

			FlushBitmaps();
			return true;
		}

		private void FloodFill_CheckPixel(int x, int y, int color, ref Stack<PixelCoord> stack)
		{
			if (x >= 0 && x < PixelWidth
				&& y >= 0 && y < PixelHeight
				&& GetPixel(x, y) == color)
			{
				stack.Push(new PixelCoord(x, y));
			}
		}

		#endregion

		public void Clear()
		{
			int nTiles = NumTiles;
			for (int i = 0; i < nTiles; i++)
				m_Tiles[i].Clear();
		}

		public bool ImportTile(int nTileIndex, int[] b)
		{
			if (nTileIndex >= NumTiles)
			{
				// "Too many tiles specified for sprite '{0}'."
				m_doc.ErrorId("ErrorSpriteTooManyTiles", m_strName);

				// Return true because the error isn't fatal.
				return true;
			}
			m_Tiles[nTileIndex].Import(b);
			return true;
		}

		public bool ImportTile32(int nTileIndex, uint[] b)
		{
			if (nTileIndex >= NumTiles)
			{
				// "Too many tiles specified for sprite '{0}'."
				m_doc.ErrorId("ErrorSpriteTooManyTiles", m_strName);

				// Return true because the error isn't fatal.
				return true;
			}
			m_Tiles[nTileIndex].Import32(b);
			return true;
		}

		public void FlushBitmaps()
		{
			int nTiles = NumTiles;
			for (int i=0; i < nTiles; i++)
				m_Tiles[i].FlushBitmaps();
		}

		public void DrawTransparentSprite(Graphics g, int nX0, int nY0)
		{
			int nX, nY;

			Color cTrans = this.GetPixelColor(0, 0);
			for (int iRow = 0; iRow < TileHeight; iRow++)
			{
				for (int iColumn = 0; iColumn < TileWidth; iColumn++)
				{
					nX = (iColumn * Tile.SmallBitmapScreenSize);
					nY = (iRow * Tile.SmallBitmapScreenSize);
					m_Tiles[(iRow * TileWidth) + iColumn].DrawTransparentTile(g, cTrans, nX0 + nX, nY0 + nY);
				}
			}
		}

		public void DrawSmallSprite(Graphics g, int nX0, int nY0)
		{
			int nX, nY;

			for (int iRow = 0; iRow < TileHeight; iRow++)
			{
				for (int iColumn = 0; iColumn < TileWidth; iColumn++)
				{
					nX = (iColumn * Tile.SmallBitmapScreenSize);
					nY = (iRow * Tile.SmallBitmapScreenSize);
					m_Tiles[(iRow * TileWidth) + iColumn].DrawSmallTile(g, nX0 + nX, nY0 + nY);
				}
			}
		}

		#region Undo

		public void RecordUndoAction(string strDesc, UndoMgr undo)
		{
			if (undo == null)
				return;

			UndoData data = GetUndoData();

			// Don't record anything if there aren't any changes
			if (!data.Equals(m_snapshot))
			{
				UndoAction_SpriteEdit action = new UndoAction_SpriteEdit(undo, m_ss, this, m_snapshot, data, strDesc);
				undo.Push(action);

				// Update the snapshot for the next UndoAction
				RecordSnapshot();
			}
		}

		public UndoData GetUndoData()
		{
			UndoData undo = new UndoData(m_tileWidth, m_tileHeight);
			RecordUndoData(ref undo);
			return undo;
		}

		public void RecordSnapshot()
		{
			RecordUndoData(ref m_snapshot);
		}

		private void RecordUndoData(ref UndoData undo)
		{
			// If the given undo struct isn't the right size, resize it.
			if (undo.width * undo.height != m_tileWidth * m_tileHeight)
				undo = new UndoData(m_tileWidth, m_tileHeight);

			undo.name = m_strName;
			undo.desc = m_strDesc;
			undo.subpalette = SubpaletteID;
			undo.width = m_tileWidth;
			undo.height = m_tileHeight;

			int nTiles = NumTiles;
			for (int i = 0; i < nTiles; i++)
				undo.tiles[i] = m_Tiles[i].GetUndoData();
		}

		public void ApplyUndoData(UndoData undo)
		{
			// If the sprite isn't the same size (in tiles) as the undo data, resize it.
			if (undo.width * undo.height != m_tileWidth * m_tileHeight)
			{
				int nTiles = undo.width * undo.height;
				m_Tiles = new Tile[nTiles];
				for (int i = 0; i < nTiles; i++)
					m_Tiles[i] = new Tile(this, m_ss.NextTileId++);
			}

			m_strName = undo.name;
			m_strDesc = undo.desc;
			SubpaletteID = undo.subpalette;
			m_tileWidth = undo.width;
			m_tileHeight = undo.height;

			for (int i = 0; i < NumTiles; i++)
				m_Tiles[i].ApplyUndoData(undo.tiles[i]);

			RecordSnapshot();
		}

		#endregion

	}

}
