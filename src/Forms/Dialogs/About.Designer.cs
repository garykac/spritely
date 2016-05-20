namespace Spritely
{
	partial class About
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lVersion = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.bOK = new System.Windows.Forms.Button();
			this.lWebsite = new System.Windows.Forms.Label();
			this.lDebug = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial Black", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(11, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(117, 33);
			this.label1.TabIndex = 0;
			this.label1.Text = "Spritely";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(14, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(329, 18);
			this.label2.TabIndex = 1;
			this.label2.Text = "Sprite && background editor for GBA and NDS";
			// 
			// lVersion
			// 
			this.lVersion.AutoSize = true;
			this.lVersion.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lVersion.Location = new System.Drawing.Point(14, 86);
			this.lVersion.Name = "lVersion";
			this.lVersion.Size = new System.Drawing.Size(84, 16);
			this.lVersion.TabIndex = 2;
			this.lVersion.Text = "Version {0} : {1}";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(14, 102);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(169, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "Copyright 2007-9 Gary Kacmarcik";
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(269, 112);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 4;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// lWebsite
			// 
			this.lWebsite.AutoSize = true;
			this.lWebsite.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lWebsite.Location = new System.Drawing.Point(14, 63);
			this.lWebsite.Name = "lWebsite";
			this.lWebsite.Size = new System.Drawing.Size(161, 16);
			this.lWebsite.TabIndex = 5;
			this.lWebsite.Text = "http://code.google.com/p/spritely/";
			// 
			// lDebug
			// 
			this.lDebug.AutoSize = true;
			this.lDebug.Location = new System.Drawing.Point(273, 63);
			this.lDebug.Name = "lDebug";
			this.lDebug.Size = new System.Drawing.Size(71, 13);
			this.lDebug.TabIndex = 6;
			this.lDebug.Text = "DEBUG Build";
			this.lDebug.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lDebug.Visible = false;
			// 
			// About
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(356, 147);
			this.Controls.Add(this.lDebug);
			this.Controls.Add(this.lWebsite);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "About";
			this.Text = "About Spritely...";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lVersion;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Label lWebsite;
		private System.Windows.Forms.Label lDebug;
	}
}