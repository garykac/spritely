using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	public abstract class UndoAction
	{
		private string m_strDescription;

		public string Description
		{
			get { return m_strDescription; }
			set { m_strDescription = value; }
		}

		public abstract void ApplyUndo();
		public abstract void ApplyRedo();
	}
}
