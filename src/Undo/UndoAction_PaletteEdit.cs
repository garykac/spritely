using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_Subpalette16Edit : UndoAction
	{
		UndoMgr m_mgr;
		Subpalette m_subpalette;
		PaletteColorData m_before;	// Undo
		PaletteColorData m_after;	// Redo

		public UndoAction_Subpalette16Edit(UndoMgr mgr, Subpalette subpalette, PaletteColorData before, PaletteColorData after, string strDesc)
		{
			m_mgr = mgr;
			m_subpalette = subpalette;
			m_before = new PaletteColorData(before);
			m_after = new PaletteColorData(after);

			int b = before.currentColor;
			int a = after.currentColor;
			Description = "Subpalette16Edit " + subpalette.SubpaletteID + "," + before.currentColor + " ("
				+ before.cRed[b] + "," + before.cGreen[b] + "," + before.cBlue[b]
				+ ") -> ("
				+ after.cRed[a] + "," + after.cGreen[a] + "," + after.cBlue[a]
				+ ")";
		}

		/// <summary>
		/// Is the UndoAction a simple selection change (as opposed to a palette color change)?
		/// </summary>
		/// <returns></returns>
		public bool IsSelectionChange()
		{
			if (IsColorChange())
				return false;
			return m_before.currentColor != m_after.currentColor;
		}

		public bool IsColorChange()
		{
			int nColorIndex, nColorValue1, nColorValue2;
			return IsColorChange(out nColorIndex, out nColorValue1, out nColorValue2);
		}

		/// <summary>
		/// Does this UndoAction change the color of one of the palette entries?
		/// </summary>
		/// <param name="nColorIndex"></param>
		/// <returns></returns>
		public bool IsColorChange(out int nColorIndex, out int nColorValue1, out int nColorValue2)
		{
			nColorIndex = -1;
			nColorValue1 = 0;
			nColorValue2 = 0;
			int nColors = m_before.numColors;
			for (int i = 0; i < nColors; i++)
			{
				// Note: this assumes that there is only 1 color change in an UndoAction
				if (m_before.cRed[i] != m_after.cRed[i]
					|| m_before.cGreen[i] != m_after.cGreen[i]
					|| m_before.cBlue[i] != m_after.cBlue[i]
					)
				{
					nColorIndex = i;
					nColorValue1 = Color555.Encode(m_before.cRed[i], m_before.cGreen[i], m_before.cBlue[i]);
					nColorValue2 = Color555.Encode(m_after.cRed[i], m_after.cGreen[i], m_after.cBlue[i]);
				}
			}
			return nColorIndex != -1;
		}

		public PaletteColorData GetUndoData()
		{
			return m_before;
		}

		/// <summary>
		/// Update the "after" UndoData. This is used when merging two UndoActions together.
		/// </summary>
		/// <param name="after"></param>
		public void UpdateRedoData(PaletteColorData after)
		{
			m_after = new PaletteColorData(after);
		}

		public override void ApplyUndo()
		{
			m_subpalette.ApplyUndoData(m_before);
		}

		public override void ApplyRedo()
		{
			m_subpalette.ApplyUndoData(m_after);
		}
	}
}
