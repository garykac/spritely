namespace Spritely
{
	partial class UnitTestForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnitTestForm));
			this.lbResults = new System.Windows.Forms.ListBox();
			this.bOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbResults
			// 
			this.lbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbResults.FormattingEnabled = true;
			this.lbResults.Location = new System.Drawing.Point(12, 17);
			this.lbResults.Name = "lbResults";
			this.lbResults.ScrollAlwaysVisible = true;
			this.lbResults.Size = new System.Drawing.Size(415, 225);
			this.lbResults.TabIndex = 0;
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(352, 251);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 1;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			// 
			// UnitTestForm
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(439, 286);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.lbResults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "UnitTestForm";
			this.Text = "UnitTests";
			this.Load += new System.EventHandler(this.UnitTestForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lbResults;
		private System.Windows.Forms.Button bOK;
	}
}