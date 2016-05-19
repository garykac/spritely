using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_SpriteEdit : UndoAction
	{
		UndoMgr m_mgr;
		Spriteset m_ss;
		Sprite m_sprite;
		Sprite.UndoData m_before;
		Sprite.UndoData m_after;

		public UndoAction_SpriteEdit(UndoMgr mgr, Spriteset ss, Sprite sprite, Sprite.UndoData before, Sprite.UndoData after, string strDesc)
		{
			m_mgr = mgr;
			m_ss = ss;
			m_sprite = sprite;
			m_before = new Sprite.UndoData(before);
			m_after = new Sprite.UndoData(after);

			Description = "SpriteEdit " + sprite.Name + " " + strDesc;
			if (IsPaletteChange())
				Description += " " + before.subpalette + " to " + after.subpalette;
		}

		public Sprite GetSprite
		{
			get { return m_sprite; }
		}

		public Sprite.UndoData Before
		{
			get { return m_before; }
		}

		public Sprite.UndoData After
		{
			get { return m_after; }
		}

		public bool IsPaletteChange()
		{
			return m_before.subpalette != m_after.subpalette;
		}

		public bool IsPixelChange()
		{
			if (m_before.width != m_after.width || m_before.height != m_after.height)
				return true;

			int nTiles = m_before.width * m_before.height;
			for (int i = 0; i < nTiles; i++)
			{
				if (!m_before.tiles[i].Equals(m_after.tiles[i]))
					return true;
			}
			return false;
		}

		public override void ApplyUndo()
		{
			m_sprite.ApplyUndoData(m_before);
			m_ss.CurrentSprite = m_sprite;
			m_ss.MoveToCorrectSpriteType(m_sprite);
		}

		public override void ApplyRedo()
		{
			m_sprite.ApplyUndoData(m_after);
			m_ss.CurrentSprite = m_sprite;
			m_ss.MoveToCorrectSpriteType(m_sprite);
		}
	}
}
	