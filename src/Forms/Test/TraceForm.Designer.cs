namespace Spritely
{
	partial class TraceForm
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
			this.lbTrace = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lbTrace
			// 
			this.lbTrace.FormattingEnabled = true;
			this.lbTrace.Location = new System.Drawing.Point(12, 12);
			this.lbTrace.Name = "lbTrace";
			this.lbTrace.Size = new System.Drawing.Size(434, 238);
			this.lbTrace.TabIndex = 0;
			// 
			// TraceForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(458, 264);
			this.Controls.Add(this.lbTrace);
			this.Name = "TraceForm";
			this.Text = "Debug Trace";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TraceForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox lbTrace;
	}
}