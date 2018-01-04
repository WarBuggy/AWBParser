namespace AWBParser
{
    partial class FormMain
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
            this.ButtonChoose = new System.Windows.Forms.Button();
            this.ButtonLocation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtBoxFilename = new System.Windows.Forms.TextBox();
            this.LabelFullOutputDetail = new System.Windows.Forms.Label();
            this.ButtonGenerate = new System.Windows.Forms.Button();
            this.LabelOutputLocation = new System.Windows.Forms.Label();
            this.LabelTargetFiles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonChoose
            // 
            this.ButtonChoose.Location = new System.Drawing.Point(12, 12);
            this.ButtonChoose.Name = "ButtonChoose";
            this.ButtonChoose.Size = new System.Drawing.Size(117, 23);
            this.ButtonChoose.TabIndex = 0;
            this.ButtonChoose.Text = "Choose files";
            this.ButtonChoose.UseVisualStyleBackColor = true;
            this.ButtonChoose.Click += new System.EventHandler(this.ButtonChoose_Click);
            // 
            // ButtonLocation
            // 
            this.ButtonLocation.Location = new System.Drawing.Point(12, 41);
            this.ButtonLocation.Name = "ButtonLocation";
            this.ButtonLocation.Size = new System.Drawing.Size(117, 23);
            this.ButtonLocation.TabIndex = 1;
            this.ButtonLocation.Text = "Output location";
            this.ButtonLocation.UseVisualStyleBackColor = true;
            this.ButtonLocation.Click += new System.EventHandler(this.ButtonLocation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Output filename";
            // 
            // TxtBoxFilename
            // 
            this.TxtBoxFilename.Location = new System.Drawing.Point(99, 69);
            this.TxtBoxFilename.Name = "TxtBoxFilename";
            this.TxtBoxFilename.Size = new System.Drawing.Size(338, 20);
            this.TxtBoxFilename.TabIndex = 3;
            // 
            // LabelFullOutputDetail
            // 
            this.LabelFullOutputDetail.Location = new System.Drawing.Point(12, 96);
            this.LabelFullOutputDetail.Name = "LabelFullOutputDetail";
            this.LabelFullOutputDetail.Size = new System.Drawing.Size(425, 13);
            this.LabelFullOutputDetail.TabIndex = 4;
            // 
            // ButtonGenerate
            // 
            this.ButtonGenerate.Location = new System.Drawing.Point(12, 122);
            this.ButtonGenerate.Name = "ButtonGenerate";
            this.ButtonGenerate.Size = new System.Drawing.Size(117, 23);
            this.ButtonGenerate.TabIndex = 5;
            this.ButtonGenerate.Text = "Generate";
            this.ButtonGenerate.UseVisualStyleBackColor = true;
            // 
            // LabelOutputLocation
            // 
            this.LabelOutputLocation.Location = new System.Drawing.Point(135, 46);
            this.LabelOutputLocation.Name = "LabelOutputLocation";
            this.LabelOutputLocation.Size = new System.Drawing.Size(302, 13);
            this.LabelOutputLocation.TabIndex = 6;
            // 
            // LabelTargetFiles
            // 
            this.LabelTargetFiles.Location = new System.Drawing.Point(135, 17);
            this.LabelTargetFiles.Name = "LabelTargetFiles";
            this.LabelTargetFiles.Size = new System.Drawing.Size(302, 13);
            this.LabelTargetFiles.TabIndex = 7;
            this.LabelTargetFiles.Text = "Please choose target pdf files!";
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(445, 261);
            this.Controls.Add(this.LabelTargetFiles);
            this.Controls.Add(this.LabelOutputLocation);
            this.Controls.Add(this.ButtonGenerate);
            this.Controls.Add(this.LabelFullOutputDetail);
            this.Controls.Add(this.TxtBoxFilename);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonLocation);
            this.Controls.Add(this.ButtonChoose);
            this.Name = "FormMain";
            this.Text = "AWB Parser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

