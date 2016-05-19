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
	public partial class BgImageProperties : Form
	{
		private Document m_doc;
		private BgImage m_bgimage;

		public BgImageProperties(Document doc, BgImage bgi)
		{
			InitializeComponent();

			m_doc = doc;
			m_bgimage = bgi;

			UpdateSpriteInfo();
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			m_bgimage.Name = tbName.Text;
			m_bgimage.Description = tbDescription.Text;

			this.Close();
		}

		private void bCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void UpdateSpriteInfo()
		{
			tbName.Text = m_bgimage.Name;
			tbDescription.Text = m_bgimage.Description;

			// "{2}x{3} pixels"
			string strFormat = ResourceMgr.GetString("BgImageSizeFormat");
			lSizeData.Text = String.Format(strFormat, m_bgimage.Bitmap.Width, m_bgimage.Bitmap.Height);
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
					strNew = m_doc.BackgroundImages.GenerateUniqueBgImageName();;
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
			if (m_bgimage != null)
			{
				m_bgimage.Name = tbName.Text;
				m_doc.HasUnsavedChanges = true;
			}
		}

		private void RecordDescription()
		{
			if (m_bgimage != null)
			{
				m_bgimage.Description = tbDescription.Text;
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

	}
}