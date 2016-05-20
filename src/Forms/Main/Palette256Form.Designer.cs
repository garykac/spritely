namespace Spritely
{
	partial class Palette256Form
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
			this.pbPalette = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPalette
			// 
			this.pbPalette.Location = new System.Drawing.Point(-1, -1);
			this.pbPalette.Name = "pbPalette";
			this.pbPalette.Size = new System.Drawing.Size(163, 163);
			this.pbPalette.TabIndex = 35;
			this.pbPalette.TabStop = false;
			this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
			// 
			// Palette256Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(161, 161);
			this.Controls.Add(this.pbPalette);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "Palette256Form";
			this.Text = "Palette256Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Palette256Form_FormClosing);
			this.Resize += new System.EventHandler(this.Palette256Form_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPalette;

	}
}