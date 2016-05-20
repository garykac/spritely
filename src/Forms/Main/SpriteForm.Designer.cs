namespace Spritely
{
	partial class SpriteForm
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
			this.pbTools = new System.Windows.Forms.PictureBox();
			this.pbSprite = new System.Windows.Forms.PictureBox();
			this.cbZoom = new System.Windows.Forms.ComboBox();
			this.pbArrowbox = new System.Windows.Forms.PictureBox();
			this.pbOptions = new System.Windows.Forms.PictureBox();
			this.bInfo = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSprite)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbArrowbox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbOptions)).BeginInit();
			this.SuspendLayout();
			// 
			// pbTools
			// 
			this.pbTools.Location = new System.Drawing.Point(5, 5);
			this.pbTools.Name = "pbTools";
			this.pbTools.Size = new System.Drawing.Size(55, 81);
			this.pbTools.TabIndex = 0;
			this.pbTools.TabStop = false;
			this.pbTools.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseMove);
			this.pbTools.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseDown);
			this.pbTools.Paint += new System.Windows.Forms.PaintEventHandler(this.pbTools_Paint);
			this.pbTools.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseUp);
			// 
			// pbSprite
			// 
			this.pbSprite.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pbSprite.Location = new System.Drawing.Point(65, 5);
			this.pbSprite.Name = "pbSprite";
			this.pbSprite.Size = new System.Drawing.Size(326, 342);
			this.pbSprite.TabIndex = 1;
			this.pbSprite.TabStop = false;
			this.pbSprite.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseMove);
			this.pbSprite.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseDown);
			this.pbSprite.Paint += new System.Windows.Forms.PaintEventHandler(this.pbSprite_Paint);
			this.pbSprite.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbSprite_MouseUp);
			// 
			// cbZoom
			// 
			this.cbZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbZoom.FormattingEnabled = true;
			this.cbZoom.Items.AddRange(new object[] {
            "1x",
            "2x",
            "4x",
            "8x",
            "16x",
            "32x"});
			this.cbZoom.Location = new System.Drawing.Point(4, 214);
			this.cbZoom.Name = "cbZoom";
			this.cbZoom.Size = new System.Drawing.Size(55, 21);
			this.cbZoom.TabIndex = 2;
			this.cbZoom.TabStop = false;
			this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
			// 
			// pbArrowbox
			// 
			this.pbArrowbox.Location = new System.Drawing.Point(4, 153);
			this.pbArrowbox.Name = "pbArrowbox";
			this.pbArrowbox.Size = new System.Drawing.Size(55, 55);
			this.pbArrowbox.TabIndex = 3;
			this.pbArrowbox.TabStop = false;
			this.pbArrowbox.MouseLeave += new System.EventHandler(this.pbArrowbox_MouseLeave);
			this.pbArrowbox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbArrowbox_MouseMove);
			this.pbArrowbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbArrowbox_MouseDown);
			this.pbArrowbox.Paint += new System.Windows.Forms.PaintEventHandler(this.pbArrowbox_Paint);
			this.pbArrowbox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbArrowbox_MouseUp);
			// 
			// pbOptions
			// 
			this.pbOptions.Location = new System.Drawing.Point(5, 92);
			this.pbOptions.Name = "pbOptions";
			this.pbOptions.Size = new System.Drawing.Size(55, 55);
			this.pbOptions.TabIndex = 4;
			this.pbOptions.TabStop = false;
			this.pbOptions.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseMove);
			this.pbOptions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseDown);
			this.pbOptions.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOptions_Paint);
			this.pbOptions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseUp);
			// 
			// bInfo
			// 
			this.bInfo.Location = new System.Drawing.Point(4, 241);
			this.bInfo.Name = "bInfo";
			this.bInfo.Size = new System.Drawing.Size(55, 20);
			this.bInfo.TabIndex = 5;
			this.bInfo.Text = "Info";
			this.bInfo.UseVisualStyleBackColor = true;
			this.bInfo.Click += new System.EventHandler(this.bInfo_Click);
			// 
			// SpriteForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(395, 352);
			this.Controls.Add(this.bInfo);
			this.Controls.Add(this.pbOptions);
			this.Controls.Add(this.pbArrowbox);
			this.Controls.Add(this.cbZoom);
			this.Controls.Add(this.pbSprite);
			this.Controls.Add(this.pbTools);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(206, 297);
			this.Name = "SpriteForm";
			this.Text = "SpriteForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpriteForm_FormClosing);
			this.Resize += new System.EventHandler(this.SpriteForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbSprite)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbArrowbox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbOptions)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbTools;
		private System.Windows.Forms.PictureBox pbSprite;
		private System.Windows.Forms.ComboBox cbZoom;
		private System.Windows.Forms.PictureBox pbArrowbox;
		private System.Windows.Forms.PictureBox pbOptions;
		private System.Windows.Forms.Button bInfo;
	}
}