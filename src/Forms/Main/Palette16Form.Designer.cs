namespace Spritely
{
	partial class Palette16Form
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
			this.pbPaletteSelect = new System.Windows.Forms.PictureBox();
			this.pbPalette = new System.Windows.Forms.PictureBox();
			this.pbBgSwatch = new System.Windows.Forms.PictureBox();
			this.pbFgSwatch = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSelect)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbBgSwatch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbFgSwatch)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPaletteSelect
			// 
			this.pbPaletteSelect.Location = new System.Drawing.Point(4, 5);
			this.pbPaletteSelect.Name = "pbPaletteSelect";
			this.pbPaletteSelect.Size = new System.Drawing.Size(153, 31);
			this.pbPaletteSelect.TabIndex = 32;
			this.pbPaletteSelect.TabStop = false;
			this.pbPaletteSelect.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseMove);
			this.pbPaletteSelect.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseDown);
			this.pbPaletteSelect.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPaletteSelect_Paint);
			this.pbPaletteSelect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPaletteSelect_MouseUp);
			// 
			// pbPalette
			// 
			this.pbPalette.Location = new System.Drawing.Point(8, 49);
			this.pbPalette.Name = "pbPalette";
			this.pbPalette.Size = new System.Drawing.Size(99, 99);
			this.pbPalette.TabIndex = 25;
			this.pbPalette.TabStop = false;
			this.pbPalette.DoubleClick += new System.EventHandler(this.pbPalette_DoubleClick);
			this.pbPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseMove);
			this.pbPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseDown);
			this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
			this.pbPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseUp);
			// 
			// pbBgSwatch
			// 
			this.pbBgSwatch.Location = new System.Drawing.Point(127, 92);
			this.pbBgSwatch.Name = "pbBgSwatch";
			this.pbBgSwatch.Size = new System.Drawing.Size(27, 27);
			this.pbBgSwatch.TabIndex = 33;
			this.pbBgSwatch.TabStop = false;
			this.pbBgSwatch.Paint += new System.Windows.Forms.PaintEventHandler(this.pbBgSwatch_Paint);
			// 
			// pbFgSwatch
			// 
			this.pbFgSwatch.Location = new System.Drawing.Point(114, 78);
			this.pbFgSwatch.Name = "pbFgSwatch";
			this.pbFgSwatch.Size = new System.Drawing.Size(27, 27);
			this.pbFgSwatch.TabIndex = 34;
			this.pbFgSwatch.TabStop = false;
			this.pbFgSwatch.Paint += new System.Windows.Forms.PaintEventHandler(this.pbFgSwatch_Paint);
			// 
			// Palette16Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(161, 161);
			this.Controls.Add(this.pbFgSwatch);
			this.Controls.Add(this.pbBgSwatch);
			this.Controls.Add(this.pbPaletteSelect);
			this.Controls.Add(this.pbPalette);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "Palette16Form";
			this.Text = "Palette16Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Palette16Form_FormClosing);
			this.Resize += new System.EventHandler(this.Palette16Form_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteSelect)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbBgSwatch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbFgSwatch)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPaletteSelect;
		private System.Windows.Forms.PictureBox pbPalette;
		private System.Windows.Forms.PictureBox pbBgSwatch;
		private System.Windows.Forms.PictureBox pbFgSwatch;

	}
}