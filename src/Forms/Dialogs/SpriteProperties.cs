using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpriteProperties : Form
	{
		private Document m_doc;
		private Spriteset m_ss;
		private Sprite m_sprite;

		public SpriteProperties(Document doc, Spriteset ss)
		{
			InitializeComponent();

			m_doc = doc;
			m_ss = ss;
			m_sprite = m_ss.CurrentSprite;

			UpdateSpriteInfo();
		}

		private void SpriteProperties_Load(object sender, EventArgs e)
		{
			// This allows the form to preview the key before it is passed to the control.
			this.KeyPreview = true;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Alt | Keys.Left:
				case Keys.Alt | Keys.Up:
					if (ValidateAllTextFields() && !m_ss.IsFirstSprite(m_sprite))
						bPrev_Click(null, null);
					return true;
				case Keys.Alt | Keys.Right:
				case Keys.Alt | Keys.Down:
					if (ValidateAllTextFields() && !m_ss.IsLastSprite(m_sprite))
						bNext_Click(null, null);
					return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			m_sprite.Name = tbName.Text;
			m_sprite.Description = tbDescription.Text;

			//OldTab tab = m_doc.OldOwner.GetTab(OldTab.Type.Sprites);
			//m_doc.OldOwner.UpdateSpriteInfo(tab);
			this.Close();
		}

		private void bCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void UpdateSpriteInfo()
		{
			tbName.Text = m_sprite.Name;
			tbDescription.Text = m_sprite.Description;

			// "{0}x{1} tiles ({2}x{3} pixels)"
			string strFormat = ResourceMgr.GetString("SpriteSizeFormat");
			lSizeData.Text = String.Format(strFormat, m_sprite.TileWidth, m_sprite.TileHeight,
				m_sprite.PixelWidth, m_sprite.PixelHeight);

			pbSprite.Invalidate();

			// Enable/disable the next/prev buttons.
			bNext.Enabled = !m_ss.IsLastSprite(m_sprite);
			bPrev.Enabled = !m_ss.IsFirstSprite(m_sprite);
		}

		private void bNext_Click(object sender, EventArgs e)
		{
			Sprite s = m_ss.NextSprite(m_sprite);
			if (s != null)
			{
				m_sprite = s;
				UpdateSpriteInfo();
			}
		}

		private void bPrev_Click(object sender, EventArgs e)
		{
			Sprite s = m_ss.PrevSprite(m_sprite);
			if (s != null)
			{
				m_sprite = s;
				UpdateSpriteInfo();
			}
		}

		private bool ValidateAllTextFields()
		{
			if (tbName.Focused)
			{
				if (!ValidateName())
					return false;
				RecordName();
			}

			if (tbDescription.Focused)
				RecordDescription();

			return true;
		}

		// Returns false if the original name was invalid.
		private bool ValidateName()
		{
			if (!Regex.IsMatch(tbName.Text, "^[A-Za-z_][A-Za-z0-9_]*$"))
			{
				string strOriginal = tbName.Text;
				string strNew = tbName.Text;

				// Fixup the name so that if the user tries to validate again, it will work.
				// First, auto-generate a name if the field is blank
				if (strNew == "")
					strNew = m_ss.GenerateUniqueSpriteName();;
				// Replace invalid characters with an underscore
				strNew = Regex.Replace(strNew, "[^A-Za-z0-9_]", "_");
				// Make sure string begins with a letter
				if (!Regex.IsMatch(strNew, "^[A-Za-z_]"))
					strNew = "S" + strNew;

				// "Invalid sprite name.
				//	Valid names must begin with a letter and contain only 'A'-'Z', 'a'-'z', '0'-'9' and '_' characters.
				//  Fixing up sprite name - changing from '{0}' to '{1}'."
				m_doc.ErrorId("ErrorInvalidSpriteName", strOriginal, strNew);

				// Update the sprite name.
				tbName.Text = strNew;

				// The name is now valid, but we need to cancel the current operation so that the
				// user has a chance to read the error message displayed above before continuing.
				return false;
			}
			return true;
		}

		private void RecordName()
		{
			if (m_sprite != null)
			{
				m_sprite.Name = tbName.Text;
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void RecordDescription()
		{
			if (m_sprite != null)
			{
				m_sprite.Description = tbDescription.Text;
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void Name_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!ValidateName())
				e.Cancel = true;
		}

		private void Name_Validated(object sender, EventArgs e)
		{
			RecordName();
		}

		private void Description_Validated(object sender, EventArgs e)
		{
			RecordDescription();
		}

		private void pbSprite_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.FillRectangle(Brushes.LightGray, 0, 0, 65, 65);
			g.DrawRectangle(Pens.Black, 0, 0, 65, 65);
			m_sprite.DrawSmallSprite(g, 1, 1);
		}

	}
}