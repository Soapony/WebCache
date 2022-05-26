namespace CacheServer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.clearbtn = new System.Windows.Forms.Button();
            this.Log = new System.Windows.Forms.TextBox();
            this.cacheLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // clearbtn
            // 
            this.clearbtn.Location = new System.Drawing.Point(669, 393);
            this.clearbtn.Name = "clearbtn";
            this.clearbtn.Size = new System.Drawing.Size(119, 45);
            this.clearbtn.TabIndex = 1;
            this.clearbtn.Text = "Clear";
            this.clearbtn.UseVisualStyleBackColor = true;
            this.clearbtn.Click += new System.EventHandler(this.clearbtn_Click);
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(12, 9);
            this.Log.Multiline = true;
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Log.Size = new System.Drawing.Size(368, 429);
            this.Log.TabIndex = 3;
            // 
            // cacheLog
            // 
            this.cacheLog.Location = new System.Drawing.Point(386, 9);
            this.cacheLog.Multiline = true;
            this.cacheLog.Name = "cacheLog";
            this.cacheLog.ReadOnly = true;
            this.cacheLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cacheLog.Size = new System.Drawing.Size(402, 378);
            this.cacheLog.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cacheLog);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.clearbtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button clearbtn;
        private TextBox Log;
        private TextBox cacheLog;
    }
}