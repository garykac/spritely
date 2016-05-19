using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	// This class should contain all of the user-editable data for the Subpalette.
	// It is used by the Undo class
	public class PaletteColorData
	{
		/// <summary>
		/// Array of RGB values for each color in the palette.
		/// </summary>
		public int[] cRed, cGreen, cBlue;

		public int numColors;

		/// <summary>
		/// Index of the currently selected color in the palette.
		/// </summary>
		public int currentColor;

		private PaletteColorData() { }

		public PaletteColorData(int nColors)
		{
			numColors = nColors;
			cRed = new int[numColors];
			cGreen = new int[numColors];
			cBlue = new int[numColors];
			currentColor = 0;
		}

		public PaletteColorData(PaletteColorData data)
		{
			numColors = data.numColors;
			currentColor = data.currentColor;
			cRed = new int[numColors];
			cGreen = new int[numColors];
			cBlue = new int[numColors];
			for (int i = 0; i < numColors; i++)
			{
				cRed[i] = data.cRed[i];
				cGreen[i] = data.cGreen[i];
				cBlue[i] = data.cBlue[i];
			}
		}

		public bool Equals(PaletteColorData data)
		{
			if (currentColor != data.currentColor
				|| numColors != data.numColors
				)
				return false;

			for (int i = 0; i < numColors; i++)
			{
				if (cRed[i] != data.cRed[i]
					|| cGreen[i] != data.cGreen[i]
					|| cBlue[i] != data.cBlue[i]
					)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Return the 16-bit encoding for the specified palette index.
		/// </summary>
		/// <param name="nIndex">The index of the palette entry</param>
		/// <returns>The encoding for the specified palette entry</returns>
		public int Encoding(int nIndex)
		{
			return Color555.Encode(cRed[nIndex], cGreen[nIndex], cBlue[nIndex]);
		}

	}
}
