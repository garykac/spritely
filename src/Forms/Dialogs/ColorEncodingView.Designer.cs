namespace Spritely
{
	partial class ColorEncodingView
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
			this.pbRGBWord = new System.Windows.Forms.PictureBox();
			this.pbBlue = new System.Windows.Forms.PictureBox();
			this.pbGreen = new System.Windows.Forms.PictureBox();
			this.pbRed = new System.Windows.Forms.PictureBox();
			this.sbRed = new System.Windows.Forms.HScrollBar();
			this.sbGreen = new System.Windows.Forms.HScrollBar();
			this.sbBlue = new System.Windows.Forms.HScrollBar();
			this.pbCurrent = new System.Windows.Forms.PictureBox();
			this.pbHex = new System.Windows.Forms.PictureBox();
			this.pbPaletteLine = new System.Windows.Forms.PictureBox();
			this.pbRGBLine = new System.Windows.Forms.PictureBox();
			this.lR = new System.Windows.Forms.Label();
			this.lG = new System.Windows.Forms.Label();
			this.lB = new System.Windows.Forms.Label();
			this.bOK = new System.Windows.Forms.Button();
			this.pbHexAlign = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lBlue = new System.Windows.Forms.Label();
			this.lGreen = new System.Windows.Forms.Label();
			this.lRed = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.pbPaletteCode = new System.Windows.Forms.PictureBox();
			this.label8 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRGBWord)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbHex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteLine)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRGBLine)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbHexAlign)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteCode)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPalette
			// 
			this.pbPalette.Location = new System.Drawing.Point(96, 74);
			this.pbPalette.Name = "pbPalette";
			this.pbPalette.Size = new System.Drawing.Size(387, 27);
			this.pbPalette.TabIndex = 0;
			this.pbPalette.TabStop = false;
			this.pbPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseDown);
			this.pbPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseMove);
			this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
			this.pbPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseUp);
			// 
			// pbRGBWord
			// 
			this.pbRGBWord.Location = new System.Drawing.Point(122, 269);
			this.pbRGBWord.Name = "pbRGBWord";
			this.pbRGBWord.Size = new System.Drawing.Size(323, 23);
			this.pbRGBWord.TabIndex = 1;
			this.pbRGBWord.TabStop = false;
			this.pbRGBWord.Paint += new System.Windows.Forms.PaintEventHandler(this.pbRGBWord_Paint);
			// 
			// pbBlue
			// 
			this.pbBlue.Location = new System.Drawing.Point(181, 197);
			this.pbBlue.Name = "pbBlue";
			this.pbBlue.Size = new System.Drawing.Size(23, 23);
			this.pbBlue.TabIndex = 2;
			this.pbBlue.TabStop = false;
			this.pbBlue.Paint += new System.Windows.Forms.PaintEventHandler(this.pbBlue_Paint);
			// 
			// pbGreen
			// 
			this.pbGreen.Location = new System.Drawing.Point(281, 198);
			this.pbGreen.Name = "pbGreen";
			this.pbGreen.Size = new System.Drawing.Size(23, 23);
			this.pbGreen.TabIndex = 3;
			this.pbGreen.TabStop = false;
			this.pbGreen.Paint += new System.Windows.Forms.PaintEventHandler(this.pbGreen_Paint);
			// 
			// pbRed
			// 
			this.pbRed.Location = new System.Drawing.Point(381, 198);
			this.pbRed.Name = "pbRed";
			this.pbRed.Size = new System.Drawing.Size(23, 23);
			this.pbRed.TabIndex = 4;
			this.pbRed.TabStop = false;
			this.pbRed.Paint += new System.Windows.Forms.PaintEventHandler(this.pbRed_Paint);
			// 
			// sbRed
			// 
			this.sbRed.LargeChange = 1;
			this.sbRed.Location = new System.Drawing.Point(356, 224);
			this.sbRed.Maximum = 31;
			this.sbRed.Name = "sbRed";
			this.sbRed.Size = new System.Drawing.Size(80, 17);
			this.sbRed.TabIndex = 11;
			this.sbRed.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// sbGreen
			// 
			this.sbGreen.LargeChange = 1;
			this.sbGreen.Location = new System.Drawing.Point(255, 224);
			this.sbGreen.Maximum = 31;
			this.sbGreen.Name = "sbGreen";
			this.sbGreen.Size = new System.Drawing.Size(80, 17);
			this.sbGreen.TabIndex = 12;
			this.sbGreen.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// sbBlue
			// 
			this.sbBlue.LargeChange = 1;
			this.sbBlue.Location = new System.Drawing.Point(155, 223);
			this.sbBlue.Maximum = 31;
			this.sbBlue.Name = "sbBlue";
			this.sbBlue.Size = new System.Drawing.Size(80, 17);
			this.sbBlue.TabIndex = 13;
			this.sbBlue.ValueChanged += new System.EventHandler(this.sbColor_ValueChanged);
			// 
			// pbCurrent
			// 
			this.pbCurrent.Location = new System.Drawing.Point(277, 133);
			this.pbCurrent.Name = "pbCurrent";
			this.pbCurrent.Size = new System.Drawing.Size(27, 27);
			this.pbCurrent.TabIndex = 14;
			this.pbCurrent.TabStop = false;
			this.pbCurrent.Paint += new System.Windows.Forms.PaintEventHandler(this.pbCurrent_Paint);
			// 
			// pbHex
			// 
			this.pbHex.Location = new System.Drawing.Point(122, 308);
			this.pbHex.Name = "pbHex";
			this.pbHex.Size = new System.Drawing.Size(323, 23);
			this.pbHex.TabIndex = 15;
			this.pbHex.TabStop = false;
			this.pbHex.Paint += new System.Windows.Forms.PaintEventHandler(this.pbHex_Paint);
			// 
			// pbPaletteLine
			// 
			this.pbPaletteLine.Location = new System.Drawing.Point(96, 101);
			this.pbPaletteLine.Name = "pbPaletteLine";
			this.pbPaletteLine.Size = new System.Drawing.Size(387, 32);
			this.pbPaletteLine.TabIndex = 16;
			this.pbPaletteLine.TabStop = false;
			this.pbPaletteLine.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPaletteLine_Paint);
			// 
			// pbRGBLine
			// 
			this.pbRGBLine.Location = new System.Drawing.Point(181, 160);
			this.pbRGBLine.Name = "pbRGBLine";
			this.pbRGBLine.Size = new System.Drawing.Size(219, 38);
			this.pbRGBLine.TabIndex = 17;
			this.pbRGBLine.TabStop = false;
			this.pbRGBLine.Paint += new System.Windows.Forms.PaintEventHandler(this.pbRGBLine_Paint);
			// 
			// lR
			// 
			this.lR.AutoSize = true;
			this.lR.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lR.ForeColor = System.Drawing.Color.Red;
			this.lR.Location = new System.Drawing.Point(364, 201);
			this.lR.Name = "lR";
			this.lR.Size = new System.Drawing.Size(15, 15);
			this.lR.TabIndex = 18;
			this.lR.Text = "R";
			this.lR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lG
			// 
			this.lG.AutoSize = true;
			this.lG.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lG.ForeColor = System.Drawing.Color.Green;
			this.lG.Location = new System.Drawing.Point(264, 201);
			this.lG.Name = "lG";
			this.lG.Size = new System.Drawing.Size(15, 15);
			this.lG.TabIndex = 19;
			this.lG.Text = "G";
			this.lG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lB
			// 
			this.lB.AutoSize = true;
			this.lB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lB.ForeColor = System.Drawing.Color.Blue;
			this.lB.Location = new System.Drawing.Point(164, 201);
			this.lB.Name = "lB";
			this.lB.Size = new System.Drawing.Size(15, 15);
			this.lB.TabIndex = 20;
			this.lB.Text = "B";
			this.lB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(255, 403);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(80, 23);
			this.bOK.TabIndex = 21;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// pbHexAlign
			// 
			this.pbHexAlign.Location = new System.Drawing.Point(122, 292);
			this.pbHexAlign.Name = "pbHexAlign";
			this.pbHexAlign.Size = new System.Drawing.Size(323, 16);
			this.pbHexAlign.TabIndex = 22;
			this.pbHexAlign.TabStop = false;
			this.pbHexAlign.Paint += new System.Windows.Forms.PaintEventHandler(this.pbHexAlign_Paint);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Current Palette:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 141);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(166, 13);
			this.label2.TabIndex = 24;
			this.label2.Text = "Currently selected color in palette:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 204);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(119, 26);
			this.label3.TabIndex = 25;
			this.label3.Text = "RGB components for\r\ncurrently selected color:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 273);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(86, 13);
			this.label4.TabIndex = 26;
			this.label4.Text = "Binary encoding:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 312);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "Hexadecimal:";
			// 
			// lBlue
			// 
			this.lBlue.AutoSize = true;
			this.lBlue.Location = new System.Drawing.Point(184, 245);
			this.lBlue.Name = "lBlue";
			this.lBlue.Size = new System.Drawing.Size(19, 13);
			this.lBlue.TabIndex = 28;
			this.lBlue.Text = "00";
			this.lBlue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lGreen
			// 
			this.lGreen.AutoSize = true;
			this.lGreen.Location = new System.Drawing.Point(284, 245);
			this.lGreen.Name = "lGreen";
			this.lGreen.Size = new System.Drawing.Size(19, 13);
			this.lGreen.TabIndex = 29;
			this.lGreen.Text = "00";
			this.lGreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lRed
			// 
			this.lRed.AutoSize = true;
			this.lRed.Location = new System.Drawing.Point(384, 245);
			this.lRed.Name = "lRed";
			this.lRed.Size = new System.Drawing.Size(19, 13);
			this.lRed.TabIndex = 30;
			this.lRed.Text = "00";
			this.lRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(361, 26);
			this.label6.TabIndex = 31;
			this.label6.Text = "Each entry in the palette is stored as a 16-bit RGB (red, green, blue) value.\r\nTh" +
				"ese 3 color components require 5 bits each. The remaining bit is unused.";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(13, 9);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(338, 20);
			this.label7.TabIndex = 32;
			this.label7.Text = "How color values are encoded in palettes";
			// 
			// pbPaletteCode
			// 
			this.pbPaletteCode.Location = new System.Drawing.Point(96, 340);
			this.pbPaletteCode.Name = "pbPaletteCode";
			this.pbPaletteCode.Size = new System.Drawing.Size(387, 54);
			this.pbPaletteCode.TabIndex = 33;
			this.pbPaletteCode.TabStop = false;
			this.pbPaletteCode.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPaletteCode_Paint);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(13, 349);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(76, 26);
			this.label8.TabIndex = 34;
			this.label8.Text = "C source code\r\nfor palette:";
			// 
			// ColorEncodingView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 437);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.pbPaletteCode);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.lRed);
			this.Controls.Add(this.lGreen);
			this.Controls.Add(this.lBlue);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pbHexAlign);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.lB);
			this.Controls.Add(this.lG);
			this.Controls.Add(this.lR);
			this.Controls.Add(this.pbPaletteLine);
			this.Controls.Add(this.pbHex);
			this.Controls.Add(this.pbCurrent);
			this.Controls.Add(this.sbBlue);
			this.Controls.Add(this.sbGreen);
			this.Controls.Add(this.sbRed);
			this.Controls.Add(this.pbRed);
			this.Controls.Add(this.pbGreen);
			this.Controls.Add(this.pbBlue);
			this.Controls.Add(this.pbRGBWord);
			this.Controls.Add(this.pbPalette);
			this.Controls.Add(this.pbRGBLine);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ColorEncodingView";
			this.Text = "View Color Encoding";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorEncodingView_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRGBWord)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbCurrent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbHex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteLine)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRGBLine)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbHexAlign)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbPaletteCode)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPalette;
		private System.Windows.Forms.PictureBox pbRGBWord;
		private System.Windows.Forms.PictureBox pbBlue;
		private System.Windows.Forms.PictureBox pbGreen;
		private System.Windows.Forms.PictureBox pbRed;
		private System.Windows.Forms.HScrollBar sbRed;
		private System.Windows.Forms.HScrollBar sbGreen;
		private System.Windows.Forms.HScrollBar sbBlue;
		private System.Windows.Forms.PictureBox pbCurrent;
		private System.Windows.Forms.PictureBox pbHex;
		private System.Windows.Forms.PictureBox pbPaletteLine;
		private System.Windows.Forms.PictureBox pbRGBLine;
		private System.Windows.Forms.Label lR;
		private System.Windows.Forms.Label lG;
		private System.Windows.Forms.Label lB;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.PictureBox pbHexAlign;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lBlue;
		private System.Windows.Forms.Label lGreen;
		private System.Windows.Forms.Label lRed;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.PictureBox pbPaletteCode;
		private System.Windows.Forms.Label label8;
	}
}