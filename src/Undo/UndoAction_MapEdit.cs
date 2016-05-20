using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public class UndoAction_MapEdit : UndoAction
	{
		UndoMgr m_mgr;
		Map m_map;
		Map.UndoData m_before;
		Map.UndoData m_after;

		public UndoAction_MapEdit(UndoMgr mgr, Map m, Map.UndoData before, Map.UndoData after, string strDesc)
		{
			m_mgr = mgr;
			m_map = m;
			m_before = new Map.UndoData(before);
			m_after = new Map.UndoData(after);

			Description = "MapEdit " + m.Name + " " + strDesc;
		}

		public Map GetMap
		{
			get { return m_map; }
		}

		public Map.UndoData Before
		{
			get { return m_before; }
		}

		public Map.UndoData After
		{
			get { return m_after; }
		}

		public override void ApplyUndo()
		{
			m_map.ApplyUndoData(m_before);
		}

		public override void ApplyRedo()
		{
			m_map.ApplyUndoData(m_after);
		}
	}
}
