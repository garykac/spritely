namespace Spritely
{
	partial class SpritesetForm
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
			this.sbSprites = new System.Windows.Forms.VScrollBar();
			this.cbPalette = new System.Windows.Forms.ComboBox();
			this.lPalette = new System.Windows.Forms.Label();
			this.pbSprites = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbSprites)).BeginInit();
			this.SuspendLayout();
			// 
			// sbSprites
			// 
			this.sbSprites.LargeChange = 2;
			this.sbSprites.Location = new System.Drawing.Point(137, 5);
			this.sbSprites.Maximum = 10;
			this.sbSprites.Name = "sbSprites";
			this.sbSprites.Size = new System.Drawing.Size(17, 228);
			this.sbSprites.TabIndex = 0;
			this.sbSprites.TabStop = true;
			this.sbSprites.ValueChanged += new System.EventHandler(this.sbSprites_ValueChanged);
			// 
			// cbPalette
			// 
			this.cbPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPalette.FormattingEnabled = true;
			this.cbPalette.Location = new System.Drawing.Point(7, 252);
			this.cbPalette.Name = "cbPalette";
			this.cbPalette.Size = new System.Drawing.Size(147, 21);
			this.cbPalette.TabIndex = 2;
			this.cbPalette.TabStop = false;
			// 
			// lPalette
			// 
			this.lPalette.AutoSize = true;
			this.lPalette.Location = new System.Drawing.Point(3, 236);
			this.lPalette.Name = "lPalette";
			this.lPalette.Size = new System.Drawing.Size(43, 13);
			this.lPalette.TabIndex = 3;
			this.lPalette.Text = "Palette:";
			// 
			// pbSprites
			// 
			this.pbSprites.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbSprites.Location = new System.Drawing.Point(7, 5);
			this.pbSprites.Name = "pbSprites";
			this.pbSprites.Size = new System.Drawing.Size(131, 228);
			this.pbSprites.TabIndex = 4;
			this.pbSprites.TabStop = false;
			this.pbSprites.DoubleClick += new System.EventHandler(this.pbSprites_DoubleClick);
			this.pbSprites.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbSprites_MouseMove);
			this.pbSprites.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbSprites_MouseDown);
			this.pbSprites.Paint += new System.Windows.Forms.PaintEventHandler(this.pbSprites_Paint);
			this.pbSprites.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbSprites_MouseUp);
			// 
			// SpritesetForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(161, 282);
			this.Controls.Add(this.pbSprites);
			this.Controls.Add(this.lPalette);
			this.Controls.Add(this.cbPalette);
			this.Controls.Add(this.sbSprites);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "SpritesetForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "SpritesetForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpritesetForm_FormClosing);
			this.Resize += new System.EventHandler(this.SpritesetForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbSprites)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.VScrollBar sbSprites;
		private System.Windows.Forms.ComboBox cbPalette;
		private System.Windows.Forms.Label lPalette;
		private System.Windows.Forms.PictureBox pbSprites;
	}
}