using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	/// <summary>
	/// The UndoMgr manages the stack of UndoActions.
	/// Each UndoAction contains a before/after snapshot.
	/// The current entry into the stack points to the current (after) snapshot.
	/// 
	/// When Undo is selected:
	///   The before snapshot of the current UndoAction is applied
	///   The current UndoAction index is decremented
	/// 
	/// When Redo is selected:
	///   The current UndoAction index is incremented
	///   The after snapshot of the current UndoAction is applied
	/// 
	/// If all UndoActions on the stack are the same type and for the same sprite,
	/// then the nth item's after snapshot is the same as the (n+1)th item's before
	/// snapshot.
	/// However, when the UndoActions are of different types or are for different
	/// sprites, then the duplicate info makes is easier to apply the undo/redo actions.
	/// </summary>
	public class UndoMgr
	{
		private ProjectMainForm m_owner;
		private TabMgr.TabId m_id;

		/// <summary>
		/// A stack of the editing history.
		/// </summary>
		List<UndoAction> m_history;

		/// <summary>
		/// Maximum depth of undo events stack.
		/// </summary>
		const int k_nMaxUndoHistory = 100;

		/// <summary>
		/// The current entry in the undo stack.
		/// This points to the snapshot that corresponds to the current state being
		/// shown to the user.
		/// </summary>
		int m_nCurrent;

		public UndoMgr(ProjectMainForm owner, TabMgr.TabId id)
		{
			m_owner = owner;
			m_id = id;
			m_history = new List<UndoAction>();
			m_nCurrent = -1;
		}

		public void Reset()
		{
			m_history.Clear();
			m_nCurrent = -1;
		}

		public int Count
		{
			get { return m_history.Count; }
		}

		public int Current
		{
			get { return m_nCurrent; }
		}

		public bool CanUndo()
		{
			return m_nCurrent >= 0;
		}

		public bool CanRedo()
		{
			return m_history.Count > m_nCurrent + 1;
		}

		public UndoAction GetCurrent()
		{
			if (m_nCurrent < 0 || m_nCurrent >= m_history.Count)
				return null;
			return m_history[m_nCurrent];
		}

		public void Push(UndoAction action)
		{
			// Remove any "redo" actions from the history.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//                     ^current
			//   count = 5; current = 2;
			//   Remove the last 2 items.
			int nCount = m_history.Count;
			if (nCount > m_nCurrent + 1)
			{
				m_history.RemoveRange(m_nCurrent + 1, nCount - (m_nCurrent + 1));
				if (m_owner != null)
					m_owner.RemoveUndoRange(m_id, m_nCurrent + 1, nCount - (m_nCurrent + 1));
			}

			// Add the new item at the end.
			//   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | N |
			//   +---+   +---+   +---+   +---+
			//                     ^old    ^current
			m_history.Add(action);
			m_nCurrent++;
			if (m_owner != null)
			{
				m_owner.AddUndo(m_id, action);
				m_owner.SetCurrentUndo(m_id, m_nCurrent);
			}

			// Remove the oldest action if the stack is too large.
			//   +---+   +---+   +---+
			//   | 1 | > | 2 | > | N |
			//   +---+   +---+   +---+
			//                     ^current
			if (m_history.Count > k_nMaxUndoHistory)
			{
				m_history.RemoveAt(0);
				m_nCurrent--;
				if (m_owner != null)
				{
					m_owner.RemoveUndo(m_id, 0);
					m_owner.SetCurrentUndo(m_id, m_nCurrent);
				}
			}
		}

		/// <summary>
		/// Remove the current entry (and everything after it) from the UndoAction list.
		/// This is called instead of Push() when 2 consecutive UndoActions would cancel
		/// each other out.
		/// </summary>
		public void DeleteCurrent()
		{
			int nCount = m_history.Count;
			m_history.RemoveRange(m_nCurrent, nCount - m_nCurrent);
			if (m_owner != null)
				m_owner.RemoveUndoRange(m_id, m_nCurrent, nCount - m_nCurrent);
			m_nCurrent--;
			if (m_owner != null)
				m_owner.SetCurrentUndo(m_id, m_nCurrent);
		}

		public void ApplyUndo()
		{
			if (m_nCurrent < 0 || m_nCurrent >= m_history.Count)
				return;

			// Apply the current undo action
			m_history[m_nCurrent].ApplyUndo();

			// Decrement the current undo action index.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//             ^new    ^old
			//   Decrement current from old to new
			// Current can be -1 after this operation.
			if (m_nCurrent >= 0)
			{
				m_nCurrent--;
				if (m_owner != null)
					m_owner.SetCurrentUndo(m_id, m_nCurrent);
			}
		}

		public void ApplyRedo()
		{
			if (m_nCurrent < -1 || m_nCurrent >= m_history.Count-1)
				return;

			// Increment the current undo action index.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//                     ^old    ^new
			//   Increment current from old to new and return that UndoAction (for Redo)
			if (m_history.Count > m_nCurrent + 1)
			{
				m_nCurrent++;
				if (m_owner != null)
					m_owner.SetCurrentUndo(m_id, m_nCurrent);
			}

			// Apply the current redo action
			m_history[m_nCurrent].ApplyRedo();
		}

		/// <summary>
		/// Scan backwards in the undo stack for the most recently edited sprite.
		/// This is used to select a sprite when the current one is deleted.
		/// </summary>
		/// <returns>The most recent Sprite, or null if a suitable Sprite cannot be found.</returns>
		public Sprite FindMostRecentSprite()
		{
			for (int i = m_nCurrent-1; i >= 0; i--)
			{
				// Use sprite from most recent edit.
				UndoAction_SpriteEdit action_edit = m_history[i] as UndoAction_SpriteEdit;
				if (action_edit != null)
					return action_edit.GetSprite;

				// Use sprite from most recent Add (not Delete, since the sprite doesn't exist anymore).
				UndoAction_AddSprite action_add = m_history[i] as UndoAction_AddSprite;
				if (action_add != null && action_add.Add)
					return action_add.GetSprite;
			}
			return null;
		}
	}

	#region Tests

	[TestFixture]
	public class UndoAction_Test
	{
		Document m_doc;
		Palette m_palette;
		Spriteset m_ss;

		UndoMgr m_mgr;

		[SetUp]
		public void TestInit()
		{
			m_doc = new Document(null);
			Assert.IsNotNull(m_doc);

			m_palette = m_doc.Palettes.AddPalette16(Options.DefaultPaletteName, 0, "");
			Assert.IsNotNull(m_palette);

			m_ss = m_doc.Spritesets.AddSpriteset(Options.DefaultSpritesetName, 0, "", m_palette);
			Assert.IsNotNull(m_ss);

			m_mgr = new UndoMgr(null, TabMgr.TabId.Sprites);
			Assert.IsNotNull(m_mgr);
		}

		[Test]
		public void Test_AddSprite_noundo()
		{
			Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, null);
			Assert.IsNotNull(s);
			Assert.AreEqual(0, m_mgr.Count);
			Assert.IsFalse(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());
		}

		[Test]
		public void Test_AddSprite()
		{
			Assert.AreEqual(0, m_mgr.Count);
			Assert.AreEqual(-1, m_mgr.Current);

			// Add a sprite.
			Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, m_mgr);
			Assert.IsNotNull(s);
			Assert.AreEqual(1, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());
		}

		[Test]
		public void Test_AddSprite_draw()
		{
			Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, m_mgr);
			Assert.IsNotNull(s);
			Assert.AreEqual(1, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Draw a pixel.
			int x1 = 3, y1 = 4;
			int color1 = 1;
			s.SetPixel(x1, y1, color1);
			Assert.AreEqual(color1, s.GetPixel(x1,y1));
			s.RecordUndoAction("pencil1", m_mgr);
			Assert.AreEqual(2, m_mgr.Count);
			Assert.AreEqual(1, m_mgr.Current);
			UndoAction_SpriteEdit u1 = m_mgr.GetCurrent() as UndoAction_SpriteEdit;
			Assert.AreEqual(0, u1.Before.tiles[0].pixels[x1, y1]);
			Assert.AreEqual(color1, u1.After.tiles[0].pixels[x1, y1]);
		}

		[Test]
		public void Test_AddSprite_draw_undo()
		{
			Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, m_mgr);
			Assert.IsNotNull(s);
			Assert.AreEqual(1, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Draw a pixel.
			int x1 = 3, y1 = 4;
			int color1 = 1;
			s.SetPixel(x1, y1, color1);
			Assert.AreEqual(color1, s.GetPixel(x1, y1));
			s.RecordUndoAction("pencil1", m_mgr);
			Assert.AreEqual(2, m_mgr.Count);
			Assert.AreEqual(1, m_mgr.Current);
			UndoAction_SpriteEdit u1 = m_mgr.GetCurrent() as UndoAction_SpriteEdit;
			Assert.AreEqual(0, u1.Before.tiles[0].pixels[x1, y1]);
			Assert.AreEqual(color1, u1.After.tiles[0].pixels[x1, y1]);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Draw another pixel.
			int x2=4, y2=5;
			int color2 = 2;
			s.SetPixel(x2, y2, color2);
			Assert.AreEqual(color2, s.GetPixel(x2,y2));
			s.RecordUndoAction("pencil2", m_mgr);
			Assert.AreEqual(3, m_mgr.Count);
			Assert.AreEqual(2, m_mgr.Current);
			UndoAction_SpriteEdit u2 = m_mgr.GetCurrent() as UndoAction_SpriteEdit;
			Assert.AreEqual(0, u2.Before.tiles[0].pixels[x2, y2]);
			Assert.AreEqual(color2, u2.After.tiles[0].pixels[x2, y2]);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Undo the last pixel draw.
			m_mgr.ApplyUndo();
			Assert.AreEqual(3, m_mgr.Count);
			Assert.AreEqual(1, m_mgr.Current);
			// Last pencil reverted.
			Assert.AreEqual(0, s.GetPixel(x2,y2));
			// First pencil still present.
			Assert.AreEqual(color1, s.GetPixel(x1, y1));
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsTrue(m_mgr.CanRedo());

			// Undo the first pixel draw.
			m_mgr.ApplyUndo();
			Assert.AreEqual(3, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			// Both pencil marks reverted.
			Assert.AreEqual(0, s.GetPixel(x1, y1));
			Assert.AreEqual(0, s.GetPixel(x2, y2));
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsTrue(m_mgr.CanRedo());

			// Undo the sprite add.
			m_mgr.ApplyUndo();
			Assert.AreEqual(3, m_mgr.Count);
			Assert.AreEqual(-1, m_mgr.Current);
			Assert.IsFalse(m_mgr.CanUndo());
			Assert.IsTrue(m_mgr.CanRedo());
		}

		[Test]
		public void Test_AddSprite_draw_redo()
		{
			Sprite s = m_ss.AddSprite(1, 1, "sample", 0, "", 0, m_mgr);
			Assert.IsNotNull(s);
			Assert.AreEqual(1, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Draw a pixel.
			int x1 = 3, y1 = 4;
			int color1 = 1;
			s.SetPixel(x1, y1, color1);
			Assert.AreEqual(color1, s.GetPixel(x1, y1));
			s.RecordUndoAction("pencil1", m_mgr);
			Assert.AreEqual(2, m_mgr.Count);
			Assert.AreEqual(1, m_mgr.Current);
			UndoAction_SpriteEdit u1 = m_mgr.GetCurrent() as UndoAction_SpriteEdit;
			Assert.AreEqual(0, u1.Before.tiles[0].pixels[x1, y1]);
			Assert.AreEqual(color1, u1.After.tiles[0].pixels[x1, y1]);
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());

			// Undo the pixel draw.
			m_mgr.ApplyUndo();
			Assert.AreEqual(2, m_mgr.Count);
			Assert.AreEqual(0, m_mgr.Current);
			// Pencil mark reverted.
			Assert.AreEqual(0, s.GetPixel(x1, y1));
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsTrue(m_mgr.CanRedo());

			// Redo the pixel draw.
			m_mgr.ApplyRedo();
			Assert.AreEqual(2, m_mgr.Count);
			Assert.AreEqual(1, m_mgr.Current);
			Assert.AreEqual(color1, s.GetPixel(x1, y1));
			Assert.IsTrue(m_mgr.CanUndo());
			Assert.IsFalse(m_mgr.CanRedo());
		}

	}

	#endregion

}
