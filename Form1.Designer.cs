namespace CharacterSync
{
    partial class Form1
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
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.copyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(12, 12);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(247, 249);
            this.propertyGrid1.TabIndex = 0;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(131, 276);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(128, 23);
            this.copyButton.TabIndex = 1;
            this.copyButton.Text = "Copy Settings";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(271, 311);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "Form1";
            this.Text = "Character Sync Settings";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button copyButton;

        private System.Windows.Forms.PropertyGrid propertyGrid1;

        private System.Windows.Forms.ColorDialog colorDialog1;

        #endregion
    }
}