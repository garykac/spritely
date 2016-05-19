namespace Spritely
{
	partial class BgImageListForm
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
			this.sbBgImages = new System.Windows.Forms.VScrollBar();
			this.pbBgImages = new System.Windows.Forms.PictureBox();
			this.bImport = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbBgImages)).BeginInit();
			this.SuspendLayout();
			// 
			// sbSprites
			// 
			this.sbBgImages.LargeChange = 2;
			this.sbBgImages.Location = new System.Drawing.Point(138, 5);
			this.sbBgImages.Maximum = 10;
			this.sbBgImages.Name = "sbSprites";
			this.sbBgImages.Size = new System.Drawing.Size(17, 393);
			this.sbBgImages.TabIndex = 0;
			this.sbBgImages.TabStop = true;
			this.sbBgImages.ValueChanged += new System.EventHandler(this.sbBgImages_ValueChanged);
			// 
			// pbBgImages
			// 
			this.pbBgImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbBgImages.Location = new System.Drawing.Point(6, 5);
			this.pbBgImages.Name = "pbBgImages";
			this.pbBgImages.Size = new System.Drawing.Size(133, 393);
			this.pbBgImages.TabIndex = 4;
			this.pbBgImages.TabStop = false;
			this.pbBgImages.DoubleClick += new System.EventHandler(this.pbBgImages_DoubleClick);
			this.pbBgImages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbBgImages_MouseMove);
			this.pbBgImages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbBgImages_MouseDown);
			this.pbBgImages.Paint += new System.Windows.Forms.PaintEventHandler(this.pbBgImages_Paint);
			this.pbBgImages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbBgImages_MouseUp);
			// 
			// bImport
			// 
			this.bImport.Location = new System.Drawing.Point(34, 404);
			this.bImport.Name = "bImport";
			this.bImport.Size = new System.Drawing.Size(89, 23);
			this.bImport.TabIndex = 5;
			this.bImport.Text = "Import...";
			this.bImport.UseVisualStyleBackColor = true;
			this.bImport.Click += new System.EventHandler(this.bImport_Click);
			// 
			// BgImageListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(161, 436);
			this.Controls.Add(this.bImport);
			this.Controls.Add(this.pbBgImages);
			this.Controls.Add(this.sbBgImages);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "BgImageListForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "BgImageListForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BgImageListForm_FormClosing);
			this.Resize += new System.EventHandler(this.BgImageListForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbBgImages)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.VScrollBar sbBgImages;
		private System.Windows.Forms.PictureBox pbBgImages;
		private System.Windows.Forms.Button bImport;
	}
}