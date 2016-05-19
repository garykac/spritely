namespace Spritely
{
	partial class UndoHistoryForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UndoHistoryForm));
			this.bOK = new System.Windows.Forms.Button();
			this.lbSprites = new System.Windows.Forms.ListBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabSprites = new System.Windows.Forms.TabPage();
			this.tabBackgroundMaps = new System.Windows.Forms.TabPage();
			this.tabBackgroundImages = new System.Windows.Forms.TabPage();
			this.lbBackgroundMaps = new System.Windows.Forms.ListBox();
			this.lbBackgroundImages = new System.Windows.Forms.ListBox();
			this.tabControl1.SuspendLayout();
			this.tabSprites.SuspendLayout();
			this.tabBackgroundMaps.SuspendLayout();
			this.tabBackgroundImages.SuspendLayout();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(220, 242);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 1;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// lbSprites
			// 
			this.lbSprites.Enabled = false;
			this.lbSprites.FormattingEnabled = true;
			this.lbSprites.Location = new System.Drawing.Point(5, 6);
			this.lbSprites.Name = "lbSprites";
			this.lbSprites.Size = new System.Drawing.Size(265, 186);
			this.lbSprites.TabIndex = 2;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabSprites);
			this.tabControl1.Controls.Add(this.tabBackgroundMaps);
			this.tabControl1.Controls.Add(this.tabBackgroundImages);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(283, 224);
			this.tabControl1.TabIndex = 3;
			// 
			// tabSprites
			// 
			this.tabSprites.Controls.Add(this.lbSprites);
			this.tabSprites.Location = new System.Drawing.Point(4, 22);
			this.tabSprites.Name = "tabSprites";
			this.tabSprites.Padding = new System.Windows.Forms.Padding(3);
			this.tabSprites.Size = new System.Drawing.Size(275, 198);
			this.tabSprites.TabIndex = 0;
			this.tabSprites.Text = "Sprites";
			this.tabSprites.UseVisualStyleBackColor = true;
			// 
			// tabBackgroundMaps
			// 
			this.tabBackgroundMaps.Controls.Add(this.lbBackgroundMaps);
			this.tabBackgroundMaps.Location = new System.Drawing.Point(4, 22);
			this.tabBackgroundMaps.Name = "tabBackgroundMaps";
			this.tabBackgroundMaps.Padding = new System.Windows.Forms.Padding(3);
			this.tabBackgroundMaps.Size = new System.Drawing.Size(275, 198);
			this.tabBackgroundMaps.TabIndex = 1;
			this.tabBackgroundMaps.Text = "Background Maps";
			this.tabBackgroundMaps.UseVisualStyleBackColor = true;
			// 
			// tabBackgroundImages
			// 
			this.tabBackgroundImages.Controls.Add(this.lbBackgroundImages);
			this.tabBackgroundImages.Location = new System.Drawing.Point(4, 22);
			this.tabBackgroundImages.Name = "tabBackgroundImages";
			this.tabBackgroundImages.Size = new System.Drawing.Size(275, 198);
			this.tabBackgroundImages.TabIndex = 2;
			this.tabBackgroundImages.Text = "Background Images";
			this.tabBackgroundImages.UseVisualStyleBackColor = true;
			// 
			// lbBackgroundMaps
			// 
			this.lbBackgroundMaps.Enabled = false;
			this.lbBackgroundMaps.FormattingEnabled = true;
			this.lbBackgroundMaps.Location = new System.Drawing.Point(5, 6);
			this.lbBackgroundMaps.Name = "lbBackgroundMaps";
			this.lbBackgroundMaps.Size = new System.Drawing.Size(265, 186);
			this.lbBackgroundMaps.TabIndex = 3;
			// 
			// lbBackgroundImages
			// 
			this.lbBackgroundImages.Enabled = false;
			this.lbBackgroundImages.FormattingEnabled = true;
			this.lbBackgroundImages.Location = new System.Drawing.Point(5, 6);
			this.lbBackgroundImages.Name = "lbBackgroundImages";
			this.lbBackgroundImages.Size = new System.Drawing.Size(265, 186);
			this.lbBackgroundImages.TabIndex = 3;
			// 
			// UndoHistory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(300, 272);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.bOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "UndoHistory";
			this.Text = "UndoHistory";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UndoHistory_FormClosing);
			this.tabControl1.ResumeLayout(false);
			this.tabSprites.ResumeLayout(false);
			this.tabBackgroundMaps.ResumeLayout(false);
			this.tabBackgroundImages.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.ListBox lbSprites;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabSprites;
		private System.Windows.Forms.TabPage tabBackgroundMaps;
		private System.Windows.Forms.TabPage tabBackgroundImages;
		private System.Windows.Forms.ListBox lbBackgroundMaps;
		private System.Windows.Forms.ListBox lbBackgroundImages;
	}
}