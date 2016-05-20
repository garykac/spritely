using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public interface IPaletteForm
	{
		/// <summary>
		/// The selected sprite has changed.
		/// </summary>
		void SpriteSelectionChanged();

		/// <summary>
		/// The data for one of the sprites has changed.
		/// </summary>
		void SpriteDataChanged();

		/// <summary>
		/// The display options for the sprite has changed.
		/// </summary>
		void SpriteDisplayOptionChanged();

		/// <summary>
		/// The currently subpalette selection has changed.
		/// </summary>
		void SubpaletteSelectChanged();

		/// <summary>
		/// A new color has been selected.
		/// </summary>
		void ColorSelectChanged();

		/// <summary>
		/// The current color value has changed in the palette.
		/// </summary>
		void ColorDataChanged();

	}
}