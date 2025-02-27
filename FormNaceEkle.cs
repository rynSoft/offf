using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormNaceEkle : Form
    {
        public string Aciklama { get; private set; }
        private string naceKodu;

        public FormNaceEkle(string naceKodu)
        {
            InitializeComponent();
            this.naceKodu = naceKodu;
            textBoxNaceKodu.Text = naceKodu;
            textBoxNaceKodu.ReadOnly = true;
        }

        private void buttonKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxAciklama.Text))
            {
                MessageBox.Show("Açıklama giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Aciklama = textBoxAciklama.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
