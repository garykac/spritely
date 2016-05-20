using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public abstract class Optionbox : Toolbox
	{
		public Optionbox()
			: base()
		{
		}

		private bool m_fOptionbox_Selecting = false;

		private Tool m_toolOptionCurrent;
		private ToolType m_eCurrentOptionTool;
		private bool m_fOriginalOptionToolState;

		public override void HandleMouseDown(int pxX, int pxY, PictureBox pb)
		{
			Debug.ClearTrace();
			Debug.Trace("MouseDown");

			m_eCurrentOptionTool = ToolType.Blank;
			m_fOptionbox_Selecting = true;
			HandleMouse(pxX, pxY, pb);
		}

		public override void HandleMouseMove(int pxX, int pxY, PictureBox pb)
		{
			if (!m_fOptionbox_Selecting)
				return;
			HandleMouse(pxX, pxY, pb);
		}

		public override void HandleMouseUp(int pxX, int pxY, PictureBox pb)
		{
			if (!m_fOptionbox_Selecting)
				return;
			m_fOptionbox_Selecting = false;
			pb.Invalidate();
		}

		public override void HandleMouseLeave(PictureBox pb)
		{
			HandleMouseMove(-10, -10, pb);
		}

		public override void HandleMouse(int pxX, int pxY, PictureBox pb)
		{
			if (!m_fOptionbox_Selecting)
				return;

			// Convert pixel (x,y) to tool (x,y).
			int nX = pxX / k_pxToolboxToolSize;
			int nY = pxY / k_pxToolboxToolSize;

			// If outside the Toolbox bounds.
			if (pxX < k_pxToolboxIndent || pxY < k_pxToolboxIndent
					|| nX >= ToolboxColumns || nY >= ToolboxRows)
			{
				// If we have a current option tool, then we need to revert the button state.
				if (m_eCurrentOptionTool != ToolType.Blank)
				{
					// If button state is unchanged, nothing to revert.
					if (m_fOriginalOptionToolState == m_toolOptionCurrent.Hilight)
						return;

					// Revert tool to original state.
					SetCurrentToolHilight(pb, m_fOriginalOptionToolState);
				}
				return;
			}

			foreach (Tool t in Tools)
			{
				// Find the tool at the mouse position.
				if (nX != t.X || nY != t.Y)
					continue;

				// Ignore if not visible or not enabled.
				if (!t.Show || !t.Enabled)
					return;

				// Select the tool (if needed).
				// Only one option can be changed at a time, so record the first
				// option tool that is selected.
				if (m_eCurrentOptionTool == ToolType.Blank)
				{
					m_toolOptionCurrent = t;
					m_eCurrentOptionTool = t.Type;
					m_fOriginalOptionToolState = t.Hilight;
				}

				// Mousemoves are now processed in the context of the current option tool.
				// The current option tool was either just selected above, or is left over
				// from the previous mouse event.

				// If same as the currently selected tool and we've already toggled the
				// button state - nothing to do.
				if (t.Type == m_eCurrentOptionTool && m_fOriginalOptionToolState != m_toolOptionCurrent.Hilight)
					return;
				// Similarly if it's outside the current tool and we haven't toggled the tool state.
				if (t.Type != m_eCurrentOptionTool && m_fOriginalOptionToolState == m_toolOptionCurrent.Hilight)
					return;

				// Set the button hilighting based on whether or not we're still in the original tool.
				if (t.Type == m_eCurrentOptionTool)
					SetCurrentToolHilight(pb, !m_fOriginalOptionToolState);
				else
					SetCurrentToolHilight(pb, m_fOriginalOptionToolState);
				return;
			}

			// No matching tool found - revert tool to original state.
			SetCurrentToolHilight(pb, m_fOriginalOptionToolState);
		}

		private void SetCurrentToolHilight(PictureBox pb, bool fValue)
		{
			// Update toolbox button hilight value.
			m_toolOptionCurrent.Hilight = fValue;

			// Update the option based on the state of the toolbox button.
			Options.Set(m_toolOptionCurrent.OptionName, fValue);

			// Update toolbox display
			pb.Invalidate();
		}

		public override void Draw(Graphics g, Size size)
		{
			base.Draw(g, size);

			int pxX0, pxY0;
			foreach (Tool t in Tools)
			{
				if (!t.Show)
					continue;

				int iColumn = t.X;
				int iRow = t.Y;

				pxX0 = k_pxToolboxIndent + iColumn * k_pxToolboxToolSize;
				pxY0 = k_pxToolboxIndent + iRow * k_pxToolboxToolSize;

				if (t.Hilight)
					g.DrawImage(s_bmHilite2, pxX0, pxY0);

				g.DrawImage(t.ButtonBitmap, pxX0 + k_pxToolImageOffset, pxY0 + k_pxToolImageOffset);
			}
		}

	}
}
