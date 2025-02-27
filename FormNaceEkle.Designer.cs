namespace WindowsFormsApp1
{
    partial class FormNaceEkle
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
            this.textBoxNaceKodu = new System.Windows.Forms.TextBox();
            this.textBoxAciklama = new System.Windows.Forms.TextBox();
            this.buttonKaydet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxNaceKodu
            // 
            this.textBoxNaceKodu.Location = new System.Drawing.Point(75, 22);
            this.textBoxNaceKodu.Name = "textBoxNaceKodu";
            this.textBoxNaceKodu.Size = new System.Drawing.Size(316, 20);
            this.textBoxNaceKodu.TabIndex = 0;
            this.textBoxNaceKodu.Text = "Nace Kodu";
            // 
            // textBoxAciklama
            // 
            this.textBoxAciklama.Location = new System.Drawing.Point(75, 48);
            this.textBoxAciklama.Name = "textBoxAciklama";
            this.textBoxAciklama.Size = new System.Drawing.Size(316, 20);
            this.textBoxAciklama.TabIndex = 1;
            this.textBoxAciklama.Text = "Açıklama";
            // 
            // buttonKaydet
            // 
            this.buttonKaydet.Location = new System.Drawing.Point(203, 81);
            this.buttonKaydet.Name = "buttonKaydet";
            this.buttonKaydet.Size = new System.Drawing.Size(75, 23);
            this.buttonKaydet.TabIndex = 2;
            this.buttonKaydet.Text = "Kaydet";
            this.buttonKaydet.UseVisualStyleBackColor = true;
            this.buttonKaydet.Click += new System.EventHandler(this.buttonKaydet_Click);
            // 
            // FormNaceEkle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 116);
            this.Controls.Add(this.buttonKaydet);
            this.Controls.Add(this.textBoxAciklama);
            this.Controls.Add(this.textBoxNaceKodu);
            this.Name = "FormNaceEkle";
            this.Text = "FormNaceEkle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxNaceKodu;
        private System.Windows.Forms.TextBox textBoxAciklama;
        private System.Windows.Forms.Button buttonKaydet;
    }
}