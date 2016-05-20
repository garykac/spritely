namespace Spritely
{
	partial class TutorialTestForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TutorialTestForm));
			this.lbResults = new System.Windows.Forms.ListBox();
			this.bDone = new System.Windows.Forms.Button();
			this.clbTests = new System.Windows.Forms.CheckedListBox();
			this.bSelectAll = new System.Windows.Forms.Button();
			this.bRun = new System.Windows.Forms.Button();
			this.cbGBA = new System.Windows.Forms.CheckBox();
			this.cbNDS = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// lbResults
			// 
			this.lbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbResults.FormattingEnabled = true;
			this.lbResults.HorizontalScrollbar = true;
			this.lbResults.Location = new System.Drawing.Point(148, 8);
			this.lbResults.Name = "lbResults";
			this.lbResults.ScrollAlwaysVisible = true;
			this.lbResults.Size = new System.Drawing.Size(342, 225);
			this.lbResults.TabIndex = 0;
			// 
			// bDone
			// 
			this.bDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bDone.Location = new System.Drawing.Point(415, 251);
			this.bDone.Name = "bDone";
			this.bDone.Size = new System.Drawing.Size(75, 23);
			this.bDone.TabIndex = 1;
			this.bDone.Text = "Done";
			this.bDone.UseVisualStyleBackColor = true;
			// 
			// clbTests
			// 
			this.clbTests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.clbTests.CheckOnClick = true;
			this.clbTests.FormattingEnabled = true;
			this.clbTests.Location = new System.Drawing.Point(12, 8);
			this.clbTests.Name = "clbTests";
			this.clbTests.Size = new System.Drawing.Size(129, 229);
			this.clbTests.TabIndex = 2;
			// 
			// bSelectAll
			// 
			this.bSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bSelectAll.Location = new System.Drawing.Point(12, 251);
			this.bSelectAll.Name = "bSelectAll";
			this.bSelectAll.Size = new System.Drawing.Size(75, 23);
			this.bSelectAll.TabIndex = 3;
			this.bSelectAll.Text = "Select All";
			this.bSelectAll.UseVisualStyleBackColor = true;
			this.bSelectAll.Click += new System.EventHandler(this.bSelectAll_Click);
			// 
			// bRun
			// 
			this.bRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bRun.Location = new System.Drawing.Point(93, 251);
			this.bRun.Name = "bRun";
			this.bRun.Size = new System.Drawing.Size(75, 23);
			this.bRun.TabIndex = 4;
			this.bRun.Text = "Run";
			this.bRun.UseVisualStyleBackColor = true;
			this.bRun.Click += new System.EventHandler(this.bRun_Click);
			// 
			// cbGBA
			// 
			this.cbGBA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbGBA.AutoSize = true;
			this.cbGBA.Location = new System.Drawing.Point(203, 255);
			this.cbGBA.Name = "cbGBA";
			this.cbGBA.Size = new System.Drawing.Size(48, 17);
			this.cbGBA.TabIndex = 5;
			this.cbGBA.Text = "GBA";
			this.cbGBA.UseVisualStyleBackColor = true;
			// 
			// cbNDS
			// 
			this.cbNDS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbNDS.AutoSize = true;
			this.cbNDS.Location = new System.Drawing.Point(257, 255);
			this.cbNDS.Name = "cbNDS";
			this.cbNDS.Size = new System.Drawing.Size(49, 17);
			this.cbNDS.TabIndex = 6;
			this.cbNDS.Text = "NDS";
			this.cbNDS.UseVisualStyleBackColor = true;
			// 
			// TutorialTestForm
			// 
			this.AcceptButton = this.bDone;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 286);
			this.Controls.Add(this.cbNDS);
			this.Controls.Add(this.cbGBA);
			this.Controls.Add(this.bRun);
			this.Controls.Add(this.bSelectAll);
			this.Controls.Add(this.clbTests);
			this.Controls.Add(this.bDone);
			this.Controls.Add(this.lbResults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TutorialTestForm";
			this.Text = "Tutorial Tests";
			this.Load += new System.EventHandler(this.UnitTestForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lbResults;
		private System.Windows.Forms.Button bDone;
		private System.Windows.Forms.CheckedListBox clbTests;
		private System.Windows.Forms.Button bSelectAll;
		private System.Windows.Forms.Button bRun;
		private System.Windows.Forms.CheckBox cbGBA;
		private System.Windows.Forms.CheckBox cbNDS;
	}
}