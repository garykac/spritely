namespace Spritely
{
	partial class CollisionTest
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
			this.pbCollision = new System.Windows.Forms.PictureBox();
			this.bUp = new System.Windows.Forms.Button();
			this.bDown = new System.Windows.Forms.Button();
			this.bLeft = new System.Windows.Forms.Button();
			this.bRight = new System.Windows.Forms.Button();
			this.lResult = new System.Windows.Forms.Label();
			this.tbInfo = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pbCollision)).BeginInit();
			this.SuspendLayout();
			// 
			// pbCollision
			// 
			this.pbCollision.Location = new System.Drawing.Point(12, 12);
			this.pbCollision.Name = "pbCollision";
			this.pbCollision.Size = new System.Drawing.Size(257, 248);
			this.pbCollision.TabIndex = 0;
			this.pbCollision.TabStop = false;
			this.pbCollision.Paint += new System.Windows.Forms.PaintEventHandler(this.pbCollision_Paint);
			// 
			// bUp
			// 
			this.bUp.Location = new System.Drawing.Point(305, 14);
			this.bUp.Name = "bUp";
			this.bUp.Size = new System.Drawing.Size(23, 23);
			this.bUp.TabIndex = 1;
			this.bUp.Text = "^";
			this.bUp.UseVisualStyleBackColor = true;
			this.bUp.Click += new System.EventHandler(this.bUp_Click);
			// 
			// bDown
			// 
			this.bDown.Location = new System.Drawing.Point(305, 57);
			this.bDown.Name = "bDown";
			this.bDown.Size = new System.Drawing.Size(23, 23);
			this.bDown.TabIndex = 2;
			this.bDown.Text = "v";
			this.bDown.UseVisualStyleBackColor = true;
			this.bDown.Click += new System.EventHandler(this.bDown_Click);
			// 
			// bLeft
			// 
			this.bLeft.Location = new System.Drawing.Point(284, 35);
			this.bLeft.Name = "bLeft";
			this.bLeft.Size = new System.Drawing.Size(23, 23);
			this.bLeft.TabIndex = 3;
			this.bLeft.Text = "<";
			this.bLeft.UseVisualStyleBackColor = true;
			this.bLeft.Click += new System.EventHandler(this.bLeft_Click);
			// 
			// bRight
			// 
			this.bRight.Location = new System.Drawing.Point(325, 35);
			this.bRight.Name = "bRight";
			this.bRight.Size = new System.Drawing.Size(23, 23);
			this.bRight.TabIndex = 4;
			this.bRight.Text = ">";
			this.bRight.UseVisualStyleBackColor = true;
			this.bRight.Click += new System.EventHandler(this.bRight_Click);
			// 
			// lResult
			// 
			this.lResult.AutoSize = true;
			this.lResult.Location = new System.Drawing.Point(276, 96);
			this.lResult.Name = "lResult";
			this.lResult.Size = new System.Drawing.Size(39, 13);
			this.lResult.TabIndex = 5;
			this.lResult.Text = "lResult";
			// 
			// tbInfo
			// 
			this.tbInfo.Location = new System.Drawing.Point(279, 112);
			this.tbInfo.Multiline = true;
			this.tbInfo.Name = "tbInfo";
			this.tbInfo.ReadOnly = true;
			this.tbInfo.Size = new System.Drawing.Size(77, 147);
			this.tbInfo.TabIndex = 6;
			// 
			// CollisionTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 272);
			this.Controls.Add(this.tbInfo);
			this.Controls.Add(this.lResult);
			this.Controls.Add(this.bRight);
			this.Controls.Add(this.bLeft);
			this.Controls.Add(this.bDown);
			this.Controls.Add(this.bUp);
			this.Controls.Add(this.pbCollision);
			this.Name = "CollisionTest";
			this.Text = "CollisionTest";
			((System.ComponentModel.ISupportInitialize)(this.pbCollision)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pbCollision;
		private System.Windows.Forms.Button bUp;
		private System.Windows.Forms.Button bDown;
		private System.Windows.Forms.Button bLeft;
		private System.Windows.Forms.Button bRight;
		private System.Windows.Forms.Label lResult;
		private System.Windows.Forms.TextBox tbInfo;
	}
}