using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class SpriteType
	{
		public string Name;
		/// <summary>
		/// Width (in tiles) for this sprite type.
		/// </summary>
		public int Width;
		/// <summary>
		/// Height (in tiles) for this sprite type.
		/// </summary>
		public int Height;
		public Sprite.GBASize Size;
		public Sprite.GBAShape Shape;
		public List<Sprite> Sprites;
		public int ScrollHeight;
		public int FirstLine;

		public SpriteType(string strName, int nWidth, int nHeight, Sprite.GBASize size, Sprite.GBAShape shape)
		{
			Name = strName;
			Width = nWidth;
			Height = nHeight;
			Size = size;
			Shape = shape;
			Sprites = new List<Sprite>();
			ScrollHeight = 0;
			FirstLine = 0;
		}
	}
}
