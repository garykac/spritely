namespace Spritely
{
	partial class ColorEditor16
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorEditor16));
			this.pbPalette = new System.Windows.Forms.PictureBox();
			this.bOK = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabRGB = new System.Windows.Forms.TabPage();
			this.pbCurrent = new System.Windows.Forms.PictureBox();
			this.lRed = new System.Windows.Forms.Label();
			this.lGreen = new System.Windows.Forms.Label();
			this.lBlue = new System.Windows.Forms.Label();
			this.lB = new System.Windows.Forms.Label();
			this.lG = new System.Windows.Forms.Label();
			this.lR = new System.Windows.Forms.Label();
			this.sbBlue = new System.Windows.Forms.HScrollBar();
			this.sbGreen = new System.Windows.Forms.HScrollBar();
			this.sbRed = new System.Windows.Forms.HScrollBar();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabRGB.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPalette
			// 
			this.pbPalette.Location = new System.Drawing.Point(22, 40);
			this.pbPalette.Name = "pbPalette";
			this.pbPalette.Size = new System.Drawing.Size(99, 99);
			this.pbPalette.TabIndex = 26;
			this.pbPalette.TabStop = false;
			this.pbPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseMove);
			this.pbPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseDown);
			this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
			this.pbPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseUp);
			// 
			// bOK
			// 
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(271, 175);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 27;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabRGB);
			this.tabControl1.Location = new System.Drawing.Point(143, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(203, 157);
			this.tabControl1.TabIndex = 28;
			// 
			// tabRGB
			// 
			this.tabRGB.BackColor = System.Drawing.Color.Transparent;
			this.tabRGB.Controls.Add(this.pbCurrent);
			this.tabRGB.Controls.Add(this.lRed);
			this.tabRGB.Controls.Add(this.lGreen);
			this.tabRGB.Controls.Add(this.lBlue);
			this.tabRGB.Controls.Add(this.lB);
			this.tabRGB.Controls.Add(this.lG);
			this.tabRGB.Controls.Add(this.lR);
			this.tabRGB.Controls.Add(this.sbBlue);
			this.tabRGB.Controls.Add(this.sbGreen);
			this.tabRGB.Controls.Add(this.sbRed);
			this.tabRGB.Location = new System.Drawing.Point(4, 22);
			this.tabRGB.Name = "tabRGB";
			this.tabRGB.Padding = new System.Windows.Forms.Padding(3);
			this.tabRGB.Size = new System.Drawing.Size(195, 131);
			this.tabRGB.TabIndex = 0;
			this.tabRGB.Text = "RGB";
			this.tabRGB.UseVisualStyleBackColor = true;
			// 
			// pbCurrent
			// 
			this.pbCurrent.Location = new System.Drawing.Point(13, 47);
			this.pbCurrent.Name = "pbCurrent";
			this.pbCurrent.Size = new System.Drawing.Size(27, 27);
			this.pbCurrent.TabIndex = 43;
			this.pbCurrent.TabStop = false;
			this.pbCurrent.Paint += new System.Windows.Forms.PaintEventHandler(this.pbCurrent_Paint);
			// 
			// lRed
			// 
			this.lRed.AutoSize = true;
			this.lRed.Location = new System.Drawing.Point(156, 26);
			this.lRed.Name = "lRed";
			this.lRed.Size = new System.Drawing.Size(19, 13);
			this.lRed.TabIndex = 42;
			this.lRed.Text = "00";
			this.lRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lGreen
			// 
			this.lGreen.AutoSize = true;
			this.lGreen.Location = new System.Drawing.Point(156, 56);
			this.lGreen.Name = "lGreen";
			this.lGreen.Size = new System.Drawing.Size(19, 13);
			this.lGreen.TabIndex = 41;
			this.lGreen.Text = "00";
			this.lGreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lBlue
			// 
			this.lBlue.AutoSize = true;
			this.lBlue.Location = new System.Drawing.Point(156, 86);
			this.lBlue.Name = "lBlue";
			this.lBlue.Size = new System.Drawing.Size(19, 13);
			this.lBlue.TabIndex = 40;
			this.lBlue.Text = "00";
			this.lBlue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lB
			// 
			this.lB.AutoSize = true;
			this.lB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lB.ForeColor = System.Drawing.Color.Blue;
			this.lB.Location = new System.Drawing.Point(55, 84);
			this.lB.Name = "lB";
			this.lB.Size = new System.Drawing.Size(15, 15);
			this.lB.TabIndex = 39;
			this.lB.Text = "B";
			this.lB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lG
			// 
			this.lG.AutoSize = true;
			this.lG.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lG.ForeColor = System.Drawing.Color.Green;
			this.lG.Location = new System.Drawing.Point(55, 54);
			this.lG.Name = "lG";
			this.lG.Size = new System.Drawing.Size(15, 15);
			this.lG.TabIndex = 38;
			this.lG.Text = "G";
			this.lG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lR
			// 
			this.lR.AutoSize = true;
			this.lR.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lR.ForeColor = System.Drawing.Color.Red;
			this.lR.Location = new System.Drawing.Point(55, 25);
			this.lR.Name = "lR";
			this.lR.Size = new System.Drawing.Size(15, 15);
			this.lR.TabIndex = 37;
			this.lR.Text = "R";
			this.lR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sbBlue
			// 
			this.sbBlue.LargeChange = 1;
			this.sbBlue.Location = new System.Drawing.Point(73, 84);
			this.sbBlue.Maximum = 31;
			this.sbBlue.Name = "sbBlue";
			this.sbBlue.Size = new System.Drawing.Size(80, 17);
			this.sbBlue.TabIndex = 36;
			this.sbBlue.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// sbGreen
			// 
			this.sbGreen.LargeChange = 1;
			this.sbGreen.Location = new System.Drawing.Point(73, 54);
			this.sbGreen.Maximum = 31;
			this.sbGreen.Name = "sbGreen";
			this.sbGreen.Size = new System.Drawing.Size(80, 17);
			this.sbGreen.TabIndex = 35;
			this.sbGreen.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// sbRed
			// 
			this.sbRed.LargeChange = 1;
			this.sbRed.Location = new System.Drawing.Point(73, 25);
			this.sbRed.Maximum = 31;
			this.sbRed.Name = "sbRed";
			this.sbRed.Size = new System.Drawing.Size(80, 17);
			this.sbRed.TabIndex = 34;
			this.sbRed.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// ColorEditor
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(361, 210);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.pbPalette);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ColorEditor";
			this.Text = "ColorEditor";
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabRGB.ResumeLayout(false);
			this.tabRGB.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPalette;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabRGB;
		private System.Windows.Forms.Label lRed;
		private System.Windows.Forms.Label lGreen;
		private System.Windows.Forms.Label lBlue;
		private System.Windows.Forms.Label lB;
		private System.Windows.Forms.Label lG;
		private System.Windows.Forms.Label lR;
		private System.Windows.Forms.HScrollBar sbBlue;
		private System.Windows.Forms.HScrollBar sbGreen;
		private System.Windows.Forms.HScrollBar sbRed;
		private System.Windows.Forms.PictureBox pbCurrent;
	}
}