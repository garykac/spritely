namespace Spritely
{
	partial class BgImageProperties
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
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.tbName = new System.Windows.Forms.TextBox();
			this.lSizeData = new System.Windows.Forms.Label();
			this.lName = new System.Windows.Forms.Label();
			this.lSize = new System.Windows.Forms.Label();
			this.lDescription = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(245, 115);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 23);
			this.bOK.TabIndex = 5;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(164, 115);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 23);
			this.bCancel.TabIndex = 4;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// tbName
			// 
			this.tbName.Location = new System.Drawing.Point(79, 12);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(136, 20);
			this.tbName.TabIndex = 0;
			this.tbName.Validated += new System.EventHandler(this.Name_Validated);
			this.tbName.Validating += new System.ComponentModel.CancelEventHandler(this.Name_Validating);
			// 
			// lSizeData
			// 
			this.lSizeData.AutoSize = true;
			this.lSizeData.Location = new System.Drawing.Point(76, 36);
			this.lSizeData.Name = "lSizeData";
			this.lSizeData.Size = new System.Drawing.Size(77, 13);
			this.lSizeData.TabIndex = 8;
			this.lSizeData.Text = "240x160 pixels";
			// 
			// lName
			// 
			this.lName.AutoSize = true;
			this.lName.Location = new System.Drawing.Point(38, 15);
			this.lName.Name = "lName";
			this.lName.Size = new System.Drawing.Size(35, 13);
			this.lName.TabIndex = 6;
			this.lName.Text = "Name";
			// 
			// lSize
			// 
			this.lSize.AutoSize = true;
			this.lSize.Location = new System.Drawing.Point(46, 36);
			this.lSize.Name = "lSize";
			this.lSize.Size = new System.Drawing.Size(27, 13);
			this.lSize.TabIndex = 7;
			this.lSize.Text = "Size";
			// 
			// lDescription
			// 
			this.lDescription.AutoSize = true;
			this.lDescription.Location = new System.Drawing.Point(13, 57);
			this.lDescription.Name = "lDescription";
			this.lDescription.Size = new System.Drawing.Size(60, 13);
			this.lDescription.TabIndex = 9;
			this.lDescription.Text = "Description";
			// 
			// tbDescription
			// 
			this.tbDescription.Location = new System.Drawing.Point(79, 54);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.Size = new System.Drawing.Size(241, 48);
			this.tbDescription.TabIndex = 1;
			this.tbDescription.Validated += new System.EventHandler(this.Description_Validated);
			// 
			// BgImageProperties
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(333, 151);
			this.Controls.Add(this.tbDescription);
			this.Controls.Add(this.lDescription);
			this.Controls.Add(this.lSize);
			this.Controls.Add(this.lName);
			this.Controls.Add(this.lSizeData);
			this.Controls.Add(this.tbName);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "BgImageProperties";
			this.Text = "Background Image Properties";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label lSizeData;
		private System.Windows.Forms.Label lName;
		private System.Windows.Forms.Label lSize;
		private System.Windows.Forms.Label lDescription;
		private System.Windows.Forms.TextBox tbDescription;
	}
}