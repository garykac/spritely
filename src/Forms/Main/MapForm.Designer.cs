namespace Spritely
{
	partial class MapForm
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
			this.pbMap = new System.Windows.Forms.PictureBox();
			this.cbZoom = new System.Windows.Forms.ComboBox();
			this.pbOptions = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbMap)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbOptions)).BeginInit();
			this.SuspendLayout();
			// 
			// pbTools
			// 
			this.pbTools.Location = new System.Drawing.Point(5, 5);
			this.pbTools.Name = "pbTools";
			this.pbTools.Size = new System.Drawing.Size(55, 29);
			this.pbTools.TabIndex = 0;
			this.pbTools.TabStop = false;
			this.pbTools.MouseLeave += new System.EventHandler(this.pbTools_MouseLeave);
			this.pbTools.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseMove);
			this.pbTools.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseDown);
			this.pbTools.Paint += new System.Windows.Forms.PaintEventHandler(this.pbTools_Paint);
			this.pbTools.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTools_MouseUp);
			// 
			// pbMap
			// 
			this.pbMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pbMap.Location = new System.Drawing.Point(65, 5);
			this.pbMap.Name = "pbMap";
			this.pbMap.Size = new System.Drawing.Size(326, 291);
			this.pbMap.TabIndex = 1;
			this.pbMap.TabStop = false;
			this.pbMap.MouseLeave += new System.EventHandler(this.pbMap_MouseLeave);
			this.pbMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbMap_MouseMove);
			this.pbMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbMap_MouseDown);
			this.pbMap.Paint += new System.Windows.Forms.PaintEventHandler(this.pbMap_Paint);
			this.pbMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbMap_MouseUp);
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
			this.cbZoom.Location = new System.Drawing.Point(4, 75);
			this.cbZoom.Name = "cbZoom";
			this.cbZoom.Size = new System.Drawing.Size(56, 21);
			this.cbZoom.TabIndex = 2;
			this.cbZoom.TabStop = false;
			this.cbZoom.SelectedIndexChanged += new System.EventHandler(this.cbZoom_SelectedIndexChanged);
			// 
			// pbOptions
			// 
			this.pbOptions.Location = new System.Drawing.Point(4, 40);
			this.pbOptions.Name = "pbOptions";
			this.pbOptions.Size = new System.Drawing.Size(55, 29);
			this.pbOptions.TabIndex = 3;
			this.pbOptions.TabStop = false;
			this.pbOptions.MouseLeave += new System.EventHandler(this.pbOptions_MouseLeave);
			this.pbOptions.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseMove);
			this.pbOptions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseDown);
			this.pbOptions.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOptions_Paint);
			this.pbOptions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbOptions_MouseUp);
			// 
			// MapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(395, 301);
			this.Controls.Add(this.pbOptions);
			this.Controls.Add(this.cbZoom);
			this.Controls.Add(this.pbMap);
			this.Controls.Add(this.pbTools);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(206, 297);
			this.Name = "MapForm";
			this.Text = "MapForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapForm_FormClosing);
			this.Resize += new System.EventHandler(this.MapForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pbTools)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbMap)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbOptions)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbTools;
		private System.Windows.Forms.PictureBox pbMap;
		private System.Windows.Forms.ComboBox cbZoom;
		private System.Windows.Forms.PictureBox pbOptions;
	}
}