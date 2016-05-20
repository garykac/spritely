using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class Color555
	{
		public int Encoded;

		/// <summary>
		/// Create a Color555 value from an encoded 16-bit value.
		/// </summary>
		/// <param name="encoded"></param>
		public Color555(int encoded)
		{
			Encoded = encoded;
		}

		/// <summary>
		/// Create a Color555 value from individual 5-bit rgb values.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		public Color555(int r, int g, int b)
		{
			Encoded = Encode(r, g, b);
		}

		public static System.Drawing.Color CalcColor(int encoded)
		{
			int r, g, b;
			ExtractColors(encoded, out r, out g, out b);
			return CalcColor(r,g,b);
		}

		public static System.Drawing.Color CalcColor(int r, int g, int b)
		{
			return System.Drawing.Color.FromArgb(r*8, g*8, b*8);
		}

		/// <summary>
		/// Construct the encoded 16-bit value directly from the individual 5-bit rgb values.
		/// </summary>
		/// <param name="r">Red component (0-31)</param>
		/// <param name="g">Green component (0-31)</param>
		/// <param name="b">Blue component (0-31)</param>
		/// <returns>16-bit encoded color value</returns>
		public static int Encode(int r, int g, int b)
		{
			return r | (g << 5) | (b << 10);
		}

		public static int Encode(System.Drawing.Color c)
		{
			int r, g, b;
			ExtractColors(c, out r, out g, out b);
			return Encode(r, g, b);
		}

		/// <summary>
		/// Extract the individual rgb colors from the encoded color.
		/// </summary>
		/// <param name="encoded">16-bit rgb555 encoded color value</param>
		/// <param name="r">Red component (0-31)</param>
		/// <param name="g">Green component (0-31)</param>
		/// <param name="b">Blue component (0-31)</param>
		public static void ExtractColors(int encoded, out int r, out int g, out int b)
		{
			r = (encoded & 0x001F);
			encoded >>= 5;
			g = (encoded & 0x001F);
			encoded >>= 5;
			b = (encoded & 0x001F);
		}

		/// <summary>
		/// Extract the individual rgb colors from the sysem color.
		/// </summary>
		/// <param name="c">System color</param>
		/// <param name="r">Red component (0-31)</param>
		/// <param name="g">Green component (0-31)</param>
		/// <param name="b">Blue component (0-31)</param>
		public static void ExtractColors(System.Drawing.Color c, out int r, out int g, out int b)
		{
			r = c.R / 8;
			g = c.G / 8;
			b = c.B / 8;
		}

	}
}
