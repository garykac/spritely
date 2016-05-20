using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_AddSprite : UndoAction
	{
		UndoMgr m_mgr;
		Spriteset m_ss;
		Sprite m_sprite;
		bool m_fAdd;

		public UndoAction_AddSprite(UndoMgr mgr, Spriteset ss, Sprite sprite, bool fAdd)
		{
			m_mgr = mgr;
			m_ss = ss;
			m_sprite = sprite;
			m_fAdd = fAdd;

			Description = (fAdd ? "AddSprite " : "RemoveSprite ") + sprite.Name;
		}

		public Sprite GetSprite
		{
			get { return m_sprite; }
		}

		public bool Add
		{
			get { return m_fAdd; }
		}

		public override void ApplyUndo()
		{
			if (m_fAdd)
			{
				m_ss.RemoveSprite(m_sprite, null);
				m_ss.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_ss.CurrentSprite == null)
					m_ss.SelectFirstSprite();
			}
			else
			{
				m_ss.AddSprite(m_sprite, null);
				m_ss.CurrentSprite = m_sprite;
			}
		}

		public override void ApplyRedo()
		{
			if (m_fAdd)
			{
				m_ss.AddSprite(m_sprite, null);
				m_ss.CurrentSprite = m_sprite;
			}
			else
			{
				m_ss.RemoveSprite(m_sprite, null);
				m_ss.CurrentSprite = m_mgr.FindMostRecentSprite();
				if (m_ss.CurrentSprite == null)
					m_ss.SelectFirstSprite();
			}
		}

	}
}
