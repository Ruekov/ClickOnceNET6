using System.Windows.Forms;

namespace TestClickOnceNET6
{
    partial class MainForm
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
            this.bttnCheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bttnCheck
            // 
            this.bttnCheck.Location = new System.Drawing.Point(12, 12);
            this.bttnCheck.Name = "bttnCheck";
            this.bttnCheck.Size = new System.Drawing.Size(307, 126);
            this.bttnCheck.TabIndex = 0;
            this.bttnCheck.Text = "Click to check updates...";
            this.bttnCheck.UseVisualStyleBackColor = true;
            this.bttnCheck.Click += new System.EventHandler(this.bttnCheck_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 150);
            this.Controls.Add(this.bttnCheck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "ClickOnceUpdateCheck";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button bttnCheck;
    }
}