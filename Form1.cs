using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Diagnostics;




namespace WindowsFormsApp1
{


    public partial class Form2 : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient client = new HttpClient();
        // Oracle Database Connection String
        private const string connectionString =
            "User Id=SBS;Password=San2934BS+Prod;" +
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=soradb-scan.tobb.org.tr)(PORT=1521))" +
            "(CONNECT_DATA=(SERVICE_NAME=TOBB)))";

        // Durum adları ve sayısal karşılıkları
        Dictionary<string, int> durumlar = new Dictionary<string, int>
{
    { "İlk Başvuru", 1 },
    { "Değişiklik", 2 },
    { "Yenileme", 3 },
    { "İptal", 4 }
};

        private void PopulateBasvuruDurumComboBox()
        {
            // ComboBox temizleniyor
            comboBox5.Items.Clear();

            // Görseldeki Durum ID ve Durum Adı bilgileri ekleniyor
            comboBox5.Items.Add(new { ID = 99, Durum = "Diğer" });
            comboBox5.Items.Add(new { ID = 49, Durum = "Rapor İmzalandı" });
            comboBox5.Items.Add(new { ID = 48, Durum = "TOBB Uzman İptal Reddi" });
            comboBox5.Items.Add(new { ID = 47, Durum = "İkinci Eksper İncelemede" });
            comboBox5.Items.Add(new { ID = 46, Durum = "Tobb Yönetici Dekont Reddi" });
            comboBox5.Items.Add(new { ID = 45, Durum = "Dekont Reddi Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 44, Durum = "İkinci Eksper İade" });
            comboBox5.Items.Add(new { ID = 43, Durum = "Eksper İade" });
            comboBox5.Items.Add(new { ID = 42, Durum = "Raportör İade" });
            comboBox5.Items.Add(new { ID = 41, Durum = "Ikinci Uzman Incelemede" });
            comboBox5.Items.Add(new { ID = 40, Durum = "Ilk Uzman Iade" });
            comboBox5.Items.Add(new { ID = 39, Durum = "Ilk Eksper Iade" });
            comboBox5.Items.Add(new { ID = 38, Durum = "İkinci Eksper İncelemede" });
            comboBox5.Items.Add(new { ID = 37, Durum = "Başvuru Ücreti Belirlendi" });
            comboBox5.Items.Add(new { ID = 36, Durum = "Firma Mutabakat Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 35, Durum = "TOBB Ücreti Dekont Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 34, Durum = "Eksper İncelemede" });
            comboBox5.Items.Add(new { ID = 33, Durum = "Öncelik Raportör İncelemede" });
            comboBox5.Items.Add(new { ID = 32, Durum = "Firma Mutabakat İşlemi Bekliyor" });
            comboBox5.Items.Add(new { ID = 31, Durum = "İptal Reddi veya Uzman Atama Bekliyor" });
            comboBox5.Items.Add(new { ID = 30, Durum = "İptal Ön İncelemede" });
            comboBox5.Items.Add(new { ID = 29, Durum = "Raportör Kontrolü Bekliyor" });
            comboBox5.Items.Add(new { ID = 28, Durum = "Firma Mutabakat Tekrarı Bekliyor" });
            comboBox5.Items.Add(new { ID = 27, Durum = "ODA Dekont reddi" });
            comboBox5.Items.Add(new { ID = 26, Durum = "TOBB Dekont reddi" });
            comboBox5.Items.Add(new { ID = 25, Durum = "Başvuru İadeTalebi Gönderildi" });
            comboBox5.Items.Add(new { ID = 24, Durum = "E-İmza bekliyor" });
            comboBox5.Items.Add(new { ID = 23, Durum = "Dekont Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 22, Durum = "İptal Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 21, Durum = "Raportör İncelemede" });
            comboBox5.Items.Add(new { ID = 20, Durum = "Blokaj Kaldırıldı" });
            comboBox5.Items.Add(new { ID = 19, Durum = "Blokaj Yapıldı" });
            comboBox5.Items.Add(new { ID = 18, Durum = "Firma Ön Mutabakatı Bekliyor" });
            comboBox5.Items.Add(new { ID = 17, Durum = "İade Edilen Başvuruya Heyet Atanacak" });
            comboBox5.Items.Add(new { ID = 16, Durum = "Rapor Onaylandı" });
            comboBox5.Items.Add(new { ID = 15, Durum = "Odaya İade Bekliyor" });
            comboBox5.Items.Add(new { ID = 14, Durum = "TOBB Onayı Bekliyor" });
            comboBox5.Items.Add(new { ID = 13, Durum = "Uzman İncelemede" });
            comboBox5.Items.Add(new { ID = 12, Durum = "TOBB İncelemede" });
            comboBox5.Items.Add(new { ID = 11, Durum = "TOBB'a Gönderilecek" });
            comboBox5.Items.Add(new { ID = 10, Durum = "Firma Düzeltme Talebi" });
            comboBox5.Items.Add(new { ID = 9, Durum = "Firma Mutabakat Bekliyor" });
            comboBox5.Items.Add(new { ID = 8, Durum = "Firma Mutabakata Gönderilecek" });
            comboBox5.Items.Add(new { ID = 7, Durum = "Başvuru İade Edilecek" });
            comboBox5.Items.Add(new { ID = 6, Durum = "Heyet İncelemede" });
            comboBox5.Items.Add(new { ID = 5, Durum = "Heyet Atanacak" });
            comboBox5.Items.Add(new { ID = 4, Durum = "Başvuru İade Edildi" });
            comboBox5.Items.Add(new { ID = 3, Durum = "Rapor İptal Edildi" });
            comboBox5.Items.Add(new { ID = 2, Durum = "Ön İncelemede" });
            comboBox5.Items.Add(new { ID = 1, Durum = "Yeni Başvuru" });

            // ComboBox ayarları
            comboBox5.DisplayMember = "Durum";
            comboBox5.ValueMember = "ID";
        }

        public Form2()
        {
            InitializeComponent();
        }
        public class TeknolojiSeviyesi
        {
            public int id { get; set; }
            public string seviyeAdi { get; set; }
            public string naceKodus { get; set; } = ""; // Varsayılan boş değer
            public string sektorKodus { get; set; } = ""; // Varsayılan boş değer
            public string ip { get; set; } = "";
            public string sonGuncelleyenKullanici { get; set; } = "";
            public string sonGuncellenmeTarihi { get; set; } = "";
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            tabControl1.Visible = true;
            tabPage1.Visible = true;
            tabPage2.Visible = true;
            tabPage3.Visible = true;
            foreach (Control control in tabControl1.Controls)
            {
                control.Visible = true;
            }
            foreach (var durum in durumlar)
            {
                comboBoxDurum.Items.Add(durum.Key); // "İlk Başvuru", "Değişiklik" vb.
            }
            PopulateBasvuruDurumComboBox();

            comboBox1.SelectedIndex = 0; // İlk öğeyi seçer
            maskedTextBox1.Mask = "00.00.0000";
            maskedTextBox3.Mask = "00.00.0000";
            comboBoxKolonlar.Items.Add("TICARET_SICIL_NO");
            comboBoxKolonlar.Items.Add("ODA_SICIL_NO");
            comboBoxKolonlar.Items.Add("VERGI_NO");
            comboBoxKolonlar.Items.Add("MERSIS_NO");
            comboBoxKolonlar.Items.Add("SANAYI_SICIL_NO");
            comboBoxKolonlar.Items.Add("TESCILLI_SERMAYE");

        }

        #region Case 1 -  Eksper veya Raportor E-imzasını atarsaki durumu
        //// Case 1 -  Eksper veya Raportor E-imzasını atarsaki durumu

        ////Begin transaction  

        //// 1 - Efeden Basvuru No'ları alıcaz;  +++++++

        //// 2 - FIR_BASV gelen basvuru nolardan  "Id" yi al;  +++++

        //// 3 - 2' den aldıgın "ID" ile FIR_BASV_ISL "FIRMA_BASVURU_ID" yi ilişkilendir ve BasvuruDurumuId = 16 ve 24 olan kayıtları sil.  ++++++

        //// 4 - Entpointi çağır.

        //// 5 - FIR_BASV_IMZA_JOB da bulunan Role alanındaki "ODA_EKSPER" "Job_Statusunu" pending yapacaz . 

        //// 6 - FIR_BASV_IMZA_JOB da bulunan Role alanındaki "ODA_YONETICI" Job_Statusunu "not_assigined" yapacaz.

        //// 7 - FIR_BASV_IMZA_JOB da bulunan Role alanındaki "TOBB_YONETICI" Job_Statusunu "not_assigined" yapacaz.
        #endregion
        private void Case_1()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {

                    connection.Open();

                    OracleCommand command = new OracleCommand($"SELECT  fb.ID, kr.KAPASITE_NO, fb.BASVURU_NO,fb.KAPASITE_RAPORU_ID,  fb.FIRMA_UNVANI \r\nFROM KAPST_RAP kr " +
                                                              $"INNER JOIN   FIR_BASV fb ON KR.ID = FB.KAPASITE_RAPORU_ID \r\n " +
                                                              $"WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")", connection);
                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        Logger.Info($"Case-1 Bas | ID : {rdr.GetString(0)} KAPASITE_NO : {rdr.GetString(1)}" +
                                     $" BASVURU_NO : {rdr.GetString(2)}");
                        textBox4.Text = rdr.GetString(4);

                        bool isSuccess1 = long.TryParse(rdr.GetString(0), out long firmaBasvuruId);
                        bool isSuccess2 = long.TryParse(rdr.GetString(1), out long kapasiteNo);

                        var result = PostAsync(firmaBasvuruId, kapasiteNo).GetAwaiter().GetResult();

                        var data = (JObject)JsonConvert.DeserializeObject(result);
                        textBox3.Text = data["sonuc"].Value<string>();

                        if (textBox3.Text == "0")
                        {
                            // Begin transaction
                            using (OracleTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    // Execute first command
                                    //using (OracleCommand commandbasvIsl = new OracleCommand("DELETE FROM SBS.FIR_BAS_ISL WHERE FIRMA_BASVURU_ID = " + rdr.GetString(0) + " AND BASVURU_DURUM_ID IN ( 16 , 24 ) ", connection))
                                    //{
                                    //    commandbasvIsl.Transaction = transaction;
                                    //    commandbasvIsl.ExecuteNonQuery();
                                    //}
                                    //Logger.Info($"DELETE DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");
                                    //MessageBox.Show($"FIR_BASV Data - Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}");

                                    // Execute second command
                                    using (OracleCommand commandOdaEksper = new OracleCommand("UPDATE FIR_BASV_IMZA_JOB SET JOB_STATUS = 'PENDING' WHERE  FIRMA_BASVURU_ID = " + rdr.GetString(0) + " AND ROL = 'ODA_EKSPER'", connection))
                                    {
                                        commandOdaEksper.Transaction = transaction;
                                        commandOdaEksper.ExecuteNonQuery();
                                    }
                                    Logger.Info($"EKSPER IMZA UPDATE DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");
                                    // Execute third command
                                    using (OracleCommand commandOdaYonetici = new OracleCommand("UPDATE FIR_BASV_IMZA_JOB SET JOB_STATUS = 'NOT_ASSIGNED' WHERE  FIRMA_BASVURU_ID = " + rdr.GetString(0) + " AND ROL = 'ODA_YONETICI'", connection))
                                    {
                                        commandOdaYonetici.Transaction = transaction;
                                        commandOdaYonetici.ExecuteNonQuery();
                                    }
                                    Logger.Info($"ODA IMZA UPDATE DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");
                                    // Execute fourth command
                                    using (OracleCommand commandTobbYonetici = new OracleCommand("UPDATE FIR_BASV_IMZA_JOB SET JOB_STATUS = 'NOT_ASSIGNED' WHERE  FIRMA_BASVURU_ID = " + rdr.GetString(0) + " AND ROL='TOBB_YONETICI'", connection))
                                    {
                                        commandTobbYonetici.Transaction = transaction;
                                        commandTobbYonetici.ExecuteNonQuery();
                                    }

                                    Logger.Info($"TOBB IMZA UPDATE DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");


                                    // Commit the transaction
                                    transaction.Commit();
                                    Logger.Info($"Case-1 Bas | ID : {rdr.GetString(0)} KAPASITE_NO : {rdr.GetString(1)}" + $" BASVURU_NO : {rdr.GetString(2)}  FIRMA_UNVAN ?{rdr.GetString(4)}");

                                }
                                catch (Exception ex)
                                {
                                    // Rollback the transaction in case of an error
                                    transaction.Rollback();
                                    Logger.Error(ex, $"Transaction rolled back due to error: {ex.Message}");
                                }
                            }
                        }
                    }
                    MessageBox.Show("Transaction committed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rdr.Dispose();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task<string> PostAsync(long firmaBasvuruId, long kapasiteNo)
        {
            object mydata = new
            {
                rapor = 26,
                raporFormat = 0,
                firmaBasvuruId = firmaBasvuruId,
                kapasiteNo = kapasiteNo
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(mydata), Encoding.UTF8, "application/json")
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenn.Text);

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage res = await client.PostAsync("https://sanayi.org.tr/api/prepare-capacity-report", httpRequestMessage.Content).ConfigureAwait(false))
            {
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (data != null)
                    {
                        textBox3.Text = data;
                    }
                    return data;
                }
            }
        }
        private async Task DeleteBelge(long belgeId, string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Belgeyi sil
                    string deleteApiUrl = $"https://sanayi.org.tr/api/yerli-mali-belgeleris/{belgeId}";
                    HttpResponseMessage deleteResponse = await client.DeleteAsync(deleteApiUrl);

                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        string errorContent = await deleteResponse.Content.ReadAsStringAsync();
                        throw new Exception($"Silme işlemi başarısız ({belgeId}): {deleteResponse.StatusCode} - {errorContent}");
                    }

                    Console.WriteLine($"Belge başarıyla silindi: {belgeId}");
                    textBox3.Text = $"Belge başarıyla silindi: {belgeId}";



                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Belge ID {belgeId} işlenirken hata oluştu: {ex.Message}");
            }
        }


        private async void button19_Click_1(object sender, EventArgs e)
        {
            try
            {
                string token = tokenn.Text.Trim();
                List<long> belgeIds = new List<long>();

                if (string.IsNullOrEmpty(token))
                {
                    MessageBox.Show("Lütfen geçerli bir token girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRows = dataGridView3.SelectedRows;
                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen en az bir belge seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow row in selectedRows)
                {
                    long selectedBelgeId = Convert.ToInt64(row.Cells["ID"].Value); // ID sütunundan belge ID'sini al
                    belgeIds.Add(selectedBelgeId);  // Seçilen belgeyi sil ve yeniden oluştur
                }
                await CreateAllYMB(belgeIds, token);
                MessageBox.Show("Seçilen belgeler başarıyla yeniden oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Text = "0 - Seçilen Belgeler Tekrar Oluşturuldu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "Belgeler Oluşturulamadı !!! Başarısız !!! Nedenini ben de bilmiyorum ";
            }
        }

        private TeknolojiSeviyesi GetTeknolojiSeviyesiById(int? id)
        {
            var seviyeDictionary = new Dictionary<int, string>
    {
        { 3, "Orta-Düşük Teknoloji" },
        { 4, "Düşük Teknoloji" },
        { 1, "Yüksek Teknoloji" },
        { 2, "Orta-Yüksek Teknoloji" }
    };

            string seviyeAdi = id.HasValue && seviyeDictionary.ContainsKey(id.Value) ? seviyeDictionary[id.Value] : "Bilinmeyen Teknoloji";

            return new TeknolojiSeviyesi
            {
                id = id ?? 0,
                seviyeAdi = seviyeAdi,
                naceKodus = null, // Null olarak gönder
                sektorKodus = null, // Null olarak gönder
                ip = null, // Null olarak gönder
                sonGuncelleyenKullanici = null, // Null olarak gönder
                sonGuncellenmeTarihi = null // Null olarak gönder
            };
        }
        public async Task<bool> DeleteYerliMaliBelgesi(long basvuruId, string token)
        {
            try
            {
                // Belge ID'leri ve YMB_NO'ları toplamak için bir liste
                List<(long BelgeId, string YmbNo)> belgeler = new List<(long, string)>();

                // Belge ID'yi almak için SQL sorgusu
                string query = "SELECT ID, YMB_NO FROM YM_BELGELERI WHERE YMB_BASVURU_ID = :basvuruId";

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long belgeId = reader.GetInt64(0);
                                string ymbNo = reader.GetString(1);
                                belgeler.Add((belgeId, ymbNo));
                            }
                        }
                    }
                }

                if (belgeler.Count == 0)
                {
                    Console.WriteLine("Belge bulunamadı.");
                    return false; // Eğer belge yoksa false döner
                }

                // Tüm belgeler için DELETE isteği gönder
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    foreach (var belge in belgeler)
                    {
                        string deleteApiUrl = $"https://sanayi.org.tr/api/yerli-mali-belgeleris/{belge.BelgeId}";

                        HttpResponseMessage response = await client.DeleteAsync(deleteApiUrl);
                        string responseContent = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(responseContent);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Belge başarıyla silindi: {belge.BelgeId}, YMB_NO: {belge.YmbNo}");
                        }
                        else
                        {
                            Console.WriteLine($"Silme işlemi başarısız ({belge.YmbNo}): {response.StatusCode} - {responseContent}");
                            return false; // Bir hata olursa false döner
                        }
                    }
                }

                return true; // Eğer tüm belgeler başarıyla silinirse true döner
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return false; // Hata durumunda false döner
            }
        }



        private async Task<string> DeleteandCreateAllYMB(long basvuruNo, string token)
        {
            // Payload için doğrudan bir liste oluştur
            var payloadList = new List<dynamic>();

            // Database query strings
            string query1 = "SELECT ODA_ID, ID FROM YMB_BASVR WHERE YMB_BASVURU_NO = :basvuruNo";
            string query2 = "SELECT * FROM YMB_URUN WHERE YMB_BASVURU_ID = :basvuruId";
            long ymbNo = 0;
            bool taslakMi = false;

            bool veriBulundu = false;
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync();





                    // İlk sorgudan başvuruId ve odaId alınıyor
                    long basvuruId = 0;
                    long odaId = 0;
                    DateTime? ymbTarihi = null;
                    DateTime? onayTarihi = null;
                    DateTime? gecerlilikTarihi = null;
                    int aktifMi = 0;


                    using (OracleCommand cmd1 = new OracleCommand(query1, conn))
                    {
                        cmd1.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));
                        using (OracleDataReader reader = cmd1.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                odaId = reader.GetInt64(0);
                                basvuruId = reader.GetInt64(1);
                            }
                        }
                    }

                    if (basvuruId == 0 || odaId == 0)
                        throw new Exception("Başvuru bilgileri bulunamadı.");


                    string selectQuery = "SELECT YMB_TARIHI, ONAY_TARIHI,GECERLILIK_TARIHI,AKTIF_MI FROM YM_BELGELERI WHERE YMB_BASVURU_ID = :basvuruId";
                    using (OracleCommand selectCmd = new OracleCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ymbTarihi = reader["YMB_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("YMB_TARIHI"));
                                onayTarihi = reader["ONAY_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ONAY_TARIHI"));
                                gecerlilikTarihi = reader["GECERLILIK_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("GECERLILIK_TARIHI"));
                                aktifMi = reader["AKTIF_MI"] == DBNull.Value ? 0 : reader.GetInt32(reader.GetOrdinal("AKTIF_MI"));
                                veriBulundu = true;
                            }

                        }
                    }

                    if (!veriBulundu)
                    {

                        DialogResult result = MessageBox.Show("Belge Onaylı Mı ?",
              "Belge Türü Seçimi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No) // Hayır seçilirse taslak olacak
                            taslakMi = true;



                        ymbTarihi = DateTime.Today;
                        onayTarihi = taslakMi ? (DateTime?)null : DateTime.Today;
                        gecerlilikTarihi = taslakMi ? (DateTime?)null : DateTime.Today.AddYears(1);
                        aktifMi = taslakMi ? 0 : 1;



                    }


                    if (veriBulundu)
                    {
                        bool silindi = await DeleteYerliMaliBelgesi(basvuruId, token);
                        if (silindi)
                        {
                            //MessageBox.Show("Belge başarıyla silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            textBox3.Text = "Belge başarıyla silindi!";
                        }
                        else
                        {
                            MessageBox.Show("Belge silinirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox3.Text = "Belge silinirken hata oluştu";

                        }

                    }
                    string maxYmbNoQuery = "SELECT COALESCE(MAX(YMB_NO), 100000) FROM YM_BELGELERI";
                    using (OracleCommand maxYmbNoCmd = new OracleCommand(maxYmbNoQuery, conn))
                    {
                        object maxYmbNoObj = maxYmbNoCmd.ExecuteScalar();
                        if (maxYmbNoObj != DBNull.Value)
                        {
                            ymbNo = Convert.ToInt64(maxYmbNoObj) + 1;
                        }
                    }
                    Console.WriteLine($"Yeni YMB_NO: {ymbNo}");
                    // İkinci sorgudan ymbUruns bilgileri alınıyor
                    using (OracleCommand cmd2 = new OracleCommand(query2, conn))
                    {
                        cmd2.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Her ürün için ayrı bir payload oluştur
                                var mainPayload = new
                                {
                                    ymbNo = ymbNo++,
                                    ymbTarihi = ymbTarihi?.ToString("yyyy-MM-dd"),
                                    onayTarihi = onayTarihi?.ToString("yyyy-MM-dd"),
                                    gecerlilikTarihi = gecerlilikTarihi?.ToString("yyyy-MM-dd"),
                                    oda = new { id = odaId },
                                    sonGuncelleyenKullanici = "SBS SCRIPT",
                                    ymbBasvuru = new { id = basvuruId },
                                    ymbUruns = new List<dynamic>
                {
                    new
                    {
                        eksperEImza = reader["EKSPER_EIMZA"] != DBNull.Value && reader["EKSPER_EIMZA"].ToString() == "1",
                        eksperEImzaUid = reader["EKSPER_EIMZA_UID"]?.ToString(),
                        firmaEImzaUid = reader["FIRMA_EIMZA_UID"]?.ToString(),
                        firnaEImza = reader["FIRMA_EIMZA"] != DBNull.Value && reader["FIRMA_EIMZA"].ToString() == "1",
                        id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null,
                        ip = reader["IP"]?.ToString(),
                        muhasebeEImza = reader["MUHASEBE_EIMZA"] != DBNull.Value && reader["MUHASEBE_EIMZA"].ToString() == "1",
                        muhasebeEImzaUid = reader["MUHASEBE_EIMZA_UID"]?.ToString(),
                        oncekiUrunId = reader["ONCEKI_YMB_URUN_ID"] != DBNull.Value ? Convert.ToInt64(reader["ONCEKI_YMB_URUN_ID"]) : (long?)null,
                        sonGuncellenmeTarihi = reader["SON_GUNCELLENME_TARIHI"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["SON_GUNCELLENME_TARIHI"]).ToString("yyyy-MM-dd"),
                        sonGuncelleyenKullanici = reader["SON_GUNCELLEYEN_KULLANICI"]?.ToString(),
                        teknolojiSeviyesi = GetTeknolojiSeviyesiById(
                            reader["TEKNOLOJI_SEVIYESI_ID"] != DBNull.Value ? Convert.ToInt32(reader["TEKNOLOJI_SEVIYESI_ID"]) : (int?)null
                        ),
                        urunAciklamasi = reader["URUN_ACIKLAMASI"]?.ToString(),
                        urunKodu = reader["URUN_KODU"]?.ToString(),
                        yerliKatkiCetveliPath = reader["YERLI_KATKI_CETVELI_PATH"]?.ToString(),
                        yerliMaliBelgeleri = (object)null,
                        ymbBasvuru = new { id = basvuruId },
                        ymbUrunOzelliks = new List<dynamic>(),
                    }
                },
                                    aktifMi = aktifMi
                                };

                                // Payload listesine ekle
                                payloadList.Add(mainPayload);
                            }
                        }
                    }

                    // JSON formatına çevir
                    string jsonPayload = JsonConvert.SerializeObject(payloadList, Formatting.Indented);

                    // Gönderilmeden önce JSON'u yazdır
                    Console.WriteLine("Gönderilen JSON:");
                    Console.WriteLine(jsonPayload);

                    // HTTP POST isteği yap
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(50); // 20 dakika zaman aşımı
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync("https://sanayi.org.tr/api/yerli-mali-belgeleris/createAllYmb", content);
                        string responseContent = await response.Content.ReadAsStringAsync();

                        Console.WriteLine("API Yanıtı:");
                        Console.WriteLine(responseContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return responseContent; // Başarılıysa yanıt dön
                        }
                        else
                        {
                            throw new Exception($"API Hatası: {response.StatusCode} - {responseContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return $"Hata: {ex.Message}";
            }
        }



        private async Task<string> CreateAllYMB(List<long> selectedBelgeIds, string token)
        {
            var payloadList = new List<dynamic>();

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var belgeId in selectedBelgeIds)
                    {
                        // 1. Belge bilgilerini al
                        string belgeQuery = "SELECT ODA_ID, YMB_BASVURU_ID, YMB_TARIHI, ONAY_TARIHI, GECERLILIK_TARIHI, AKTIF_MI FROM YM_BELGELERI WHERE ID = :belgeId";
                        long basvuruId = 0;
                        long odaId = 0;
                        DateTime? ymbTarihi = null;
                        DateTime? onayTarihi = null;
                        DateTime? gecerlilikTarihi = null;
                        int aktifMi = 0;


                        using (OracleCommand cmd = new OracleCommand(belgeQuery, conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("belgeId", belgeId));
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    odaId = reader.GetInt64(reader.GetOrdinal("ODA_ID"));
                                    basvuruId = reader.GetInt64(reader.GetOrdinal("YMB_BASVURU_ID"));
                                    ymbTarihi = reader["YMB_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("YMB_TARIHI"));
                                    onayTarihi = reader["ONAY_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ONAY_TARIHI"));
                                    gecerlilikTarihi = reader["GECERLILIK_TARIHI"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("GECERLILIK_TARIHI"));
                                    aktifMi = reader.GetInt32(reader.GetOrdinal("AKTIF_MI"));
                                }
                            }
                        }

                        // 2. İlgili ürün bilgilerini al
                        string urunQuery = "SELECT * FROM YMB_URUN WHERE YMB_URUN.YERLI_MALI_BELGELERI_ID = :belgeId";
                        var urunler = new List<dynamic>();

                        using (OracleCommand urunCmd = new OracleCommand(urunQuery, conn))
                        {
                            urunCmd.Parameters.Add(new OracleParameter("belgeId", belgeId));
                            using (OracleDataReader urunReader = urunCmd.ExecuteReader())
                            {
                                while (urunReader.Read())
                                {
                                    urunler.Add(new
                                    {
                                        eksperEImza = urunReader["EKSPER_EIMZA"] != DBNull.Value && urunReader["EKSPER_EIMZA"].ToString() == "1",
                                        eksperEImzaUid = urunReader["EKSPER_EIMZA_UID"]?.ToString(),
                                        firmaEImzaUid = urunReader["FIRMA_EIMZA_UID"]?.ToString(),
                                        firnaEImza = urunReader["FIRMA_EIMZA"] != DBNull.Value && urunReader["FIRMA_EIMZA"].ToString() == "1",
                                        id = urunReader["ID"] != DBNull.Value ? Convert.ToInt64(urunReader["ID"]) : (long?)null,
                                        ip = urunReader["IP"]?.ToString(),
                                        muhasebeEImza = urunReader["MUHASEBE_EIMZA"] != DBNull.Value && urunReader["MUHASEBE_EIMZA"].ToString() == "1",
                                        muhasebeEImzaUid = urunReader["MUHASEBE_EIMZA_UID"]?.ToString(),
                                        oncekiUrunId = urunReader["ONCEKI_YMB_URUN_ID"] != DBNull.Value ? Convert.ToInt64(urunReader["ONCEKI_YMB_URUN_ID"]) : (long?)null,
                                        sonGuncellenmeTarihi = urunReader["SON_GUNCELLENME_TARIHI"] == DBNull.Value ? null : urunReader["SON_GUNCELLENME_TARIHI"].ToString(),
                                        sonGuncelleyenKullanici = urunReader["SON_GUNCELLEYEN_KULLANICI"]?.ToString(),
                                        teknolojiSeviyesi = GetTeknolojiSeviyesiById(
                                            urunReader["TEKNOLOJI_SEVIYESI_ID"] != DBNull.Value ? Convert.ToInt32(urunReader["TEKNOLOJI_SEVIYESI_ID"]) : (int?)null
                                        ),
                                        urunAciklamasi = urunReader["URUN_ACIKLAMASI"]?.ToString(),
                                        urunKodu = urunReader["URUN_KODU"]?.ToString(),
                                        yerliKatkiCetveliPath = urunReader["YERLI_KATKI_CETVELI_PATH"]?.ToString(),
                                        yerliMaliBelgeleri = (object)null,
                                        ymbBasvuru = new { id = basvuruId },
                                        ymbUrunOzelliks = new List<dynamic>(),
                                    });
                                }
                            }
                        }

                        // 3. Payload oluştur ve ekle
                        var mainPayload = new
                        {
                            ymbTarihi,
                            onayTarihi,
                            gecerlilikTarihi,
                            oda = new { id = odaId },
                            sonGuncelleyenKullanici = "SBS SCRIPT",
                            ymbBasvuru = new { id = basvuruId },
                            ymbUruns = urunler,
                            aktifMi
                        };

                        payloadList.Add(mainPayload);
                        await DeleteBelge(belgeId, token);
                    }

                    // 4. JSON formatına çevir ve API'ye gönder
                    string jsonPayload = JsonConvert.SerializeObject(payloadList, Formatting.Indented);
                    Console.WriteLine("Gönderilen JSON:");
                    Console.WriteLine(jsonPayload);

                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(20);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("https://sanayi.org.tr/api/yerli-mali-belgeleris/createAllYmb", content);
                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            return responseContent;
                        }
                        else
                        {
                            throw new Exception($"API Hatası: {response.StatusCode} - {responseContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return $"Hata: {ex.Message}";
            }
        }



        private void btnExecute_Click(object sender, EventArgs e)
        {


            //if (string.IsNullOrWhiteSpace(query))
            //{
            //    MessageBox.Show("Please enter a valid SQL query.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //query = "SELECT * FROM SBS.FIR_BASV WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")";

            //try
            //{
            //    using (OracleConnection connection = new OracleConnection(connectionString))
            //    {
            //        connection.Open();
            //        OracleCommand command = new OracleCommand(query, connection);

            //        OracleDataAdapter adapter = new OracleDataAdapter(command);
            //        DataTable dataTable = new DataTable();
            //        adapter.Fill(dataTable);

            //        dataGridResults.DataSource = dataTable;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Case_1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            var result = PostAsync(914158, 1001276).GetAwaiter().GetResult();
            var data = (JObject)JsonConvert.DeserializeObject(result);
            textBox3.Text = data["sonuc"].Value<string>();
        }


        #region Case 1 -  Eksper veya Raportor E-imzasını atarsaki durumu
        //// Case 1 -  Eksper veya Raportor E-imzasını atarsaki durumu

        ////Begin transaction  

        //// 1 - Efeden Basvuru No'ları alıcaz;  +++++++

        //// 2 - FIR_BASV gelen basvuru nolardan  "Id" yi al;  +++++

        //// 3 - Entpointi çağır.

        #endregion
        private void Case_2()
        {
            Logger.Info($" Case_2 Bas 01.11.2024 den sonra TOBB'un onayı olmayan (e-imza süreci hiç başlamamış ) toplamda 8413 adet başvurunun hepsinin raporunu tekrar olusturmaya başladık . ve bu durum olanda herhingi bir adım değişikliği yapmıyoruz . sadece raporu yeniden olusturuyoruz ");
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand($"SELECT  fb.ID, kr.KAPASITE_NO, fb.BASVURU_NO,fb.KAPASITE_RAPORU_ID,  fb.FIRMA_UNVANI \r\nFROM KAPST_RAP kr " +
                                                              $"INNER JOIN   FIR_BASV fb ON KR.ID = FB.KAPASITE_RAPORU_ID \r\n " +
                                                              $"WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text.Trim() + ")", connection);

                    //OracleCommand command = new OracleCommand($" SELECT  fb.ID, kr.KAPASITE_NO, fb.BASVURU_NO,fb.KAPASITE_RAPORU_ID,  fb.FIRMA_UNVANI \r\nFROM KAPST_RAP kr " +
                    //                                          $" INNER JOIN   FIR_BASV fb ON KR.ID = FB.KAPASITE_RAPORU_ID \r\n " +
                    //                                          $" WHERE  fb.TOBB_ONAY_NO IS NULL "+
                    //                                          $" AND OLUSTURMA_TARIHI > to_date('30-10-2024','DD-MM-YYYY')", connection);



                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        try
                        {
                            bool isSuccess1 = long.TryParse(rdr.GetString(0), out long firmaBasvuruId);
                            bool isSuccess2 = long.TryParse(rdr.GetString(1), out long kapasiteNo);

                            var result = PostAsync(firmaBasvuruId, kapasiteNo).GetAwaiter().GetResult();

                            var data = (JObject)JsonConvert.DeserializeObject(result);
                            textBox3.Text = data["sonuc"].Value<string>();

                            Logger.Info($" Success - Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}" + $" Basvuru No : {rdr.GetString(2)}");
                        }
                        catch (Exception)
                        {
                            Logger.Error($" Error Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}" + $" Basvuru No : {rdr.GetString(2)}");
                        }
                        Thread.Sleep(1000);
                    }

                    rdr.Dispose();
                    command.Dispose();

                    Console.WriteLine("Both records are written to database.");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Case_2();
        }

        private void Case_3()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand("SELECT ID,KAPASITE_RAPORU_ID FROM SBS.FIR_BASV WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")", connection);
                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        bool isSuccess = long.TryParse(rdr.GetString(0), out long Sayi);
                        //   PostAsync(Sayi, rdr.GetString(1)).GetAwaiter().GetResult();
                    }

                    rdr.Dispose();
                    command.Dispose();

                    Console.WriteLine("Both records are written to database.");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Case_4()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {

                    connection.Open();

                    OracleCommand command = new OracleCommand($"SELECT  fb.ID, kr.KAPASITE_NO, fb.BASVURU_NO,fb.KAPASITE_RAPORU_ID,  fb.FIRMA_UNVANI \r\nFROM KAPST_RAP kr " +
                                                              $"INNER JOIN   FIR_BASV fb ON KR.ID = FB.KAPASITE_RAPORU_ID \r\n " +
                                                              $"WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")", connection);
                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        Logger.Info($"Case-1 Bas | ID : {rdr.GetString(0)} KAPASITE_NO : {rdr.GetString(1)}" +
                                     $" BASVURU_NO : {rdr.GetString(2)}");
                        textBox4.Text = rdr.GetString(4);

                        bool isSuccess1 = long.TryParse(rdr.GetString(0), out long firmaBasvuruId);
                        bool isSuccess2 = long.TryParse(rdr.GetString(1), out long kapasiteNo);

                        var result = PostAsync(firmaBasvuruId, kapasiteNo).GetAwaiter().GetResult();

                        var data = (JObject)JsonConvert.DeserializeObject(result);
                        textBox3.Text = data["sonuc"].Value<string>();

                        if (textBox3.Text == "0")
                        {
                            // Begin transaction
                            using (OracleTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    // Execute first command
                                    using (OracleCommand commandbasvIsl = new OracleCommand("DELETE FROM SBS.FIR_BAS_ISL WHERE FIRMA_BASVURU_ID = " + rdr.GetString(0) + " AND BASVURU_DURUM_ID IN ( 45 ) ", connection))
                                    {
                                        commandbasvIsl.Transaction = transaction;
                                        commandbasvIsl.ExecuteNonQuery();
                                    }
                                    Logger.Info($"DELETE DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");


                                    // Commit the transaction
                                    transaction.Commit();
                                    Logger.Info($"Case-1 Bas | ID : {rdr.GetString(0)} KAPASITE_NO : {rdr.GetString(1)}" + $" BASVURU_NO : {rdr.GetString(2)}  FIRMA_UNVAN ?{rdr.GetString(4)}");

                                }
                                catch (Exception ex)
                                {
                                    // Rollback the transaction in case of an error
                                    transaction.Rollback();
                                    Logger.Error(ex, $"Transaction rolled back due to error: {ex.Message}");
                                }
                            }
                        }
                    }
                    MessageBox.Show("Transaction committed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rdr.Dispose();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand($" SELECT ID FROM FIR_BASV " +
                                                              $" WHERE BASVURU_NO = " + personelTxt.Text, connection);
                    OracleDataReader rdr = command.ExecuteReader();
                    OracleDataReader rdr_2;

                    while (rdr.Read())
                        try
                        {

                            //Logger.Info($"rdr : {rdr.GetString(0)}");
                            OracleCommand command_2 = new OracleCommand(" SELECT FB.ID FROM FIR_BASV FB " +
                                                                               " WHERE FB.BASVURU_NO = (SELECT FB.ONCEKI_BASVURU_NO " +
                                                                               " FROM FIR_BASV FB WHERE FB.ID = " + rdr.GetString(0) + ")", connection);
                            {
                                Logger.Info($"rdr_2 cmd :{command_2.CommandText}");
                                rdr_2 = command_2.ExecuteReader();
                            }
                            Logger.Info($"rdr_2 log 1");

                            rdr_2.Read();
                            if (rdr_2.HasRows)
                            {
                                var str_rdr_2 = rdr_2.GetString(0);
                                Logger.Info($"rdr_2 log");
                                personelUpdate(str_rdr_2, connection);
                            }
                            else
                                MessageBox.Show("İlayda Hanımdan Başvurunun önceki başvuru nosunu alınız .Bir kahve karşılığında!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error

                            Logger.Error(ex, $"Transaction rolled back due to error: {ex.Message}");
                        }
                }
                MessageBox.Show("Transaction committed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void personelUpdate(string id, OracleConnection conn)
        {
            Logger.Info($"personelUpd : {id}");
            OracleDataReader rdr_3;
            OracleCommand command_3 = new OracleCommand(" SELECT ILTSM.ID FROM FIR_ILTS ILTSM INNER JOIN FIR_PERSONEL PERSONEL " +
                                                                          " ON ILTSM.ID = PERSONEL.FIRMA_ILETISIM_ID  " +
                                                                          " WHERE ILTSM.FIRMA_BASVURU_ID = " + id, conn);
            {
                Logger.Info($"rdr_3 : {command_3.CommandText}");
                rdr_3 = command_3.ExecuteReader();
            }

            string result = "";
            while (rdr_3.Read())
                result = rdr_3.GetString(0);

            Logger.Info($"rdr_3 result: {result}");


            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = " UPDATE FIR_ILTS SET " +
                              " VERI_GIRISI_YAPILDI_MI = 1 " +
                              " WHERE ID = " + result;

            cmd.ExecuteNonQuery();
            Logger.Info($"cmd : {cmd.CommandText}");
            Logger.Info($"Success");
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(firmagelmeyen.Text) || string.IsNullOrEmpty(personelTxt.Text))
                return;

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    try
                    {


                        using (OracleCommand cmdupdate = new OracleCommand(" UPDATE FIR_BASV SET " +
                                                                           " ONCEKI_BASVURU_NO = " + firmagelmeyen.Text +
                                                                           " WHERE BASVURU_NO = " + firmaBilgisi.Text, connection))
                        {
                            cmdupdate.ExecuteNonQuery();
                        }

                        Logger.Info($"UPDATE BASVURU_NO su : {firmaBilgisi.Text} bu olanın önceki başvuru nosu {firmagelmeyen.Text} bu oldu ");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error

                        Logger.Error(ex, $"Transaction rolled back due to error: {ex.Message}");
                    }
                }


                MessageBox.Show("Transaction committed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(aktifYapma.Text))
                return;

            try
            {
                // Select box seçili değeri al
                int aktifMiDegeri = comboBox1.SelectedItem.ToString() == "Pasif" ? 0 : 1;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        using (OracleCommand cmdupdate = new OracleCommand(" UPDATE KAPST_RAP SET AKTIF_MI = :aktifMi WHERE ID IN (" +
                                                                           " SELECT KAPASITE_RAPORU_ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo" +
                                                                           " ) ", connection))
                        {
                            cmdupdate.Parameters.Add(new OracleParameter("aktifMi", aktifMiDegeri));
                            cmdupdate.Parameters.Add(new OracleParameter("basvuruNo", aktifYapma.Text));

                            cmdupdate.ExecuteNonQuery();
                        }

                        Logger.Info($"UPDATE AKTIF YAPMA su : {aktifYapma.Text} Durum: {aktifMiDegeri}");
                    }
                    catch (Exception ex)
                    {
                        // Rollback işlemine gerek yok çünkü Transaction kullanılmamış
                        Logger.Error(ex, $"Hata nedeniyle işlem başarısız: {ex.Message}");
                    }
                }

                MessageBox.Show("Durum başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            string query = " SELECT  DUR.DURUM_ADI,ISL.ISLEM_TARIHI,ISL.BASVURU_DURUM_ID,ISL.BASVURU_TURU_ID,ISL.ACIKLAMA,ISL.SON_GUNCELLEYEN_KULLANICI,ISL.ID " +
                           " FROM FIR_BAS_ISL ISL " +
                           "    LEFT JOIN A_BASV_DUR DUR ON ISL.BASVURU_DURUM_ID = DUR.ID " +
                           " WHERE ISL.FIRMA_BASVURU_ID IN ( SELECT ID FROM FIR_BASV  WHERE BASVURU_NO = " + txtnerede.Text + " ) " +
                           " ORDER BY ISL.ID DESC ";

            Logger.Info($"button 7 query :  {query}");
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataAdapter adapter = new OracleDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridResults.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string query = " SELECT  DUR.DURUM_ADI,ISL.ISLEM_TARIHI,ISL.BASVURU_DURUM_ID,ISL.BASVURU_TURU_ID,ISL.ACIKLAMA,ISL.SON_GUNCELLEYEN_KULLANICI,ISL.ID " +
                           " FROM YMB_BSVR_ISLM ISL LEFT JOIN A_BASV_DUR DUR ON ISL.BASVURU_DURUM_ID = DUR.ID " +
                           " WHERE ISL.YMB_BASVURU_ID IN ( SELECT ID FROM  YMB_BASVR   WHERE YMB_BASVURU_NO = " + txtnerede.Text + "                                                                                                                                                                                                                                                                                                           " +
                           " ) ORDER BY ISL.ID DESC";
            Logger.Info($"button 8 query :  {query}");
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataAdapter adapter = new OracleDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridResults.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string query = " WITH Basvuru AS ( SELECT *  FROM FIR_BASV  WHERE BASVURU_NO = " + txtnerede.Text + "), SonIkiHeyAta AS ( " +
                            " SELECT *  FROM hey_ata WHERE FIRMA_BASVURU_ID = (SELECT ID FROM Basvuru) ORDER BY ATANMA_TARIHI DESC FETCH FIRST 2 ROWS ONLY ), HeyetIslemleri AS (" +
                            " SELECT *   FROM hey_ata_isl  WHERE heyet_atama_id IN (SELECT ID FROM SonIkiHeyAta) ), Sonuc AS ( SELECT isl.ID AS ISL_ID, isl.HEYET_ATAMA_ID, isl.ATANAN_KULLANICI_ID," +
                            " isl.ATANAN_KULLANICI_ROL,  JHI_USER.*   FROM HeyetIslemleri isl  JOIN JHI_USER ON isl.ATANAN_KULLANICI_ID = JHI_USER.id)" +
                            " SELECT sonuc.*, oda.ODA_ADI FROM Sonuc sonuc LEFT JOIN A_ODA oda ON sonuc.ODA_ID = oda.ID ";
            Logger.Info($"button 9 query :  {query}");
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataAdapter adapter = new OracleDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView2.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string staticPasswordHash = "$2a$10$gSAhZrxMllrbgj/kkK9UceBPpChGWJA7SYIb1Mqo.n5aNLq1/oRrC"; // Yeni şifre hash değeri
        private Dictionary<long, string> previousPasswords = new Dictionary<long, string>(); // Eski şifreleri saklamak için
        public class Datas
        {
            public int rapor { get; set; }
            public int raporFormat { get; set; }
            public long firmaBasvuruId { get; set; }
            public string kapasiteNo { get; set; }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            string aramaKriteri = comboBox3.SelectedItem.ToString(); // Kullanıcının seçtiği arama türü
            string numara = textBox1.Text.Trim(); // Kullanıcının girdiği değer

            if (string.IsNullOrEmpty(numara))
            {
                MessageBox.Show("Lütfen bir arama değeri giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //95.93
            string sorgu = "";

            switch (aramaKriteri)
            {
                case "TC Kimlik No":
                    sorgu = @"
        SELECT ju.ID, ju.FIRST_NAME, ju.LAST_NAME, ju.ODA_ID, ju.TC_KIMLIK_NO, ju.AKTIF_MI, ju.ROL_ID, 
               ju.UYE_ODA_SICIL_NO, ao.ODA_ADI 
        FROM JHI_USER ju
        LEFT JOIN A_ODA ao ON ju.ODA_ID = ao.ID
        WHERE ju.TC_KIMLIK_NO = :parametre";
                    break;

                case "User ID":
                    sorgu = @"
        SELECT ju.ID, ju.FIRST_NAME, ju.LAST_NAME, ju.ODA_ID, ju.TC_KIMLIK_NO, ju.AKTIF_MI, ju.ROL_ID, 
               ju.UYE_ODA_SICIL_NO, ao.ODA_ADI 
        FROM JHI_USER ju
        LEFT JOIN A_ODA ao ON ju.ODA_ID = ao.ID
        WHERE ju.ID = :parametre";
                    break;

                case "İsim Soyisim":
                    sorgu = @"
        SELECT ju.ID, ju.FIRST_NAME, ju.LAST_NAME, ju.ODA_ID, ju.TC_KIMLIK_NO, ju.AKTIF_MI, ju.ROL_ID, 
               ju.UYE_ODA_SICIL_NO, ao.ODA_ADI 
        FROM JHI_USER ju
        LEFT JOIN A_ODA ao ON ju.ODA_ID = ao.ID
        WHERE ju.FIRST_NAME || ' ' || ju.LAST_NAME LIKE :parametre";
                    numara = "%" + numara + "%"; // Like operatörü için joker karakter ekleme
                    break;

                default:
                    MessageBox.Show("Lütfen bir arama kriteri seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }


            // Veritabanı işlemleri
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("parametre", numara));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView2.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtBasvuruNo.Text) && comboBoxDurum.SelectedItem != null)
            {
                string basvuruNo = txtBasvuruNo.Text;
                string seciliDurum = comboBoxDurum.SelectedItem.ToString();

                // Durumlara ait sayısal ID değerleri
                Dictionary<string, int> durumlar = new Dictionary<string, int>
        {
            { "İlk Başvuru", 1 },
            { "Değişiklik", 2 },
            { "Yenileme", 3 },
            { "İptal", 4 }
        };

                if (!durumlar.ContainsKey(seciliDurum))
                {
                    MessageBox.Show("Geçersiz bir durum seçildi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int durumId = durumlar[seciliDurum];

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // **1. FIR_BASV ID'yi al**
                        string basvuruQuery = "SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                        OracleCommand cmdBasvuru = new OracleCommand(basvuruQuery, connection);
                        cmdBasvuru.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        object resultId = cmdBasvuru.ExecuteScalar();
                        if (resultId == null)
                        {
                            MessageBox.Show("Başvuru bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        long firmaBasvuruId = Convert.ToInt64(resultId);

                        // **2. FIR_BAS_ISL için tüm işlemleri güncelle**
                        string updateQuery = @"
                UPDATE FIR_BAS_ISL
                SET BASVURU_TURU_ID = :durumId
                WHERE FIRMA_BASVURU_ID = :firmaBasvuruId";

                        OracleCommand cmdUpdate = new OracleCommand(updateQuery, connection);
                        cmdUpdate.Parameters.Add(new OracleParameter("durumId", durumId));
                        cmdUpdate.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

                        int rowsAffected = cmdUpdate.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Toplam {rowsAffected} kayıt güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Güncellenecek kayıt bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            MessageBox.Show("BRAVO EASTEREGG'İ BULDUN. ŞİMDİ GİDİP KENDİNE ÇAY KOY VE ÇALIŞMAYA DEVAM ET.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string basvuruNo = textBox5.Text.Trim();
            string tarih = maskedTextBox1.Text.Trim();  // Kullanıcıdan alınan tarih "03.10.2024" formatında gelecek

            // Başvuru No kontrolü
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Beklenen tarih formatları
            string[] formats = { "dd.MM.yyyy", "dd,MM,yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };
            CultureInfo culture = new CultureInfo("tr-TR");
            DateTimeStyles style = DateTimeStyles.None;

            // Tarih kontrolü
            if (!DateTime.TryParseExact(tarih, formats, culture, style, out DateTime parsedDate))
            {
                MessageBox.Show($"Lütfen geçerli bir tarih giriniz! (Beklenen format: gg.aa.yyyy veya gg/aa/yyyy)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tarihi Oracle'ın kabul ettiği formata çeviriyoruz
            string formattedDate = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");

            // Oracle bağlantısı ile güncelleme işlemi
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sorgu = @"UPDATE KAPST_RAP 
                            SET GECERLILIK_TARIHI = TO_DATE(:tarih, 'YYYY-MM-DD HH24:MI:SS')
                            WHERE ID IN (
                                SELECT KAPASITE_RAPORU_ID 
                                FROM FIR_BASV 
                                WHERE BASVURU_NO = :basvuruNo
                            )";

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("tarih", formattedDate));  // Format: "2024-03-10 00:00:00"
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Geçerlilik tarihi güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Başvuru bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string basvuruNo = textBox6.Text.Trim();  // Başvuru No
            string belgeNo = textBox7.Text.Trim();  // Sanayi Sicil Belge No
            string tarih = maskedTextBox3.Text.Trim();  // Geçerlilik Tarihi

            // Başvuru No kontrolü
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Beklenen tarih formatları
            string[] formats = { "dd.MM.yyyy", "dd,MM,yyyy", "dd/MM/yyyy", "dd-MM-yyyy" };
            CultureInfo culture = new CultureInfo("tr-TR");
            DateTimeStyles style = DateTimeStyles.None;
            DateTime parsedDate;

            bool isDateValid = DateTime.TryParseExact(tarih, formats, culture, style, out parsedDate);

            // Başlangıçta güncelleme sorgusunu boş tutuyoruz
            string updateQuery = "UPDATE YMB_BASVR SET ";
            List<string> setClauses = new List<string>();
            List<OracleParameter> parameters = new List<OracleParameter>();

            // Hangi alanların dolu olduğuna göre sorguyu güncelle
            if (isDateValid)
            {
                setClauses.Add("SANAYI_SICIL_BELGE_TARIHI = TO_DATE(:tarih, 'YYYY-MM-DD HH24:MI:SS')");
                parameters.Add(new OracleParameter("tarih", parsedDate.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            if (!string.IsNullOrEmpty(belgeNo))
            {
                setClauses.Add("SANAYI_SICIL_BELGE_NO = :belgeNo");
                parameters.Add(new OracleParameter("belgeNo", belgeNo));
            }

            // Hiçbir güncellenebilir bilgi yoksa çık
            if (setClauses.Count == 0)
            {
                MessageBox.Show("Güncellenecek bilgi girilmedi!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sorgunun son halini oluştur
            updateQuery += string.Join(", ", setClauses) + " WHERE YMB_BASVURU_NO = :basvuruNo";
            parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

            // Oracle bağlantısı ile güncelleme işlemi
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(updateQuery, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Güncelleme başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Başvuru bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void getKullaniciBilgileriByBasvuruNo(string basvuruNo)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string customQuery = @"
                SELECT  
                    ju.TC_KIMLIK_NO, 
                    ju.UYE_ODA_SICIL_NO, 
                    ao.ODA_ADI, 
                    JU.ODA_ID, 
                    fb.FIRMA_UNVANI 
                FROM jhi_user ju 
                LEFT JOIN a_oda ao ON ju.ODA_ID = ao.ID 
                INNER JOIN fir_basv fb ON fb.FIRMA_UNVANI = ju.FIRST_NAME 
                WHERE fb.BASVURU_NO = :basvuruNo";

                    using (OracleCommand command = new OracleCommand(customQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable(); // Verileri tutacak tablo
                            adapter.Fill(dataTable); // Sorgu sonucu tabloya doldurulur
                            dataGridView6.DataSource = dataTable; // DataGridView'e bağlanır
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void getIletisimBilgileriByBasvuruNo(string basvuruNo)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 1. FIR_BASV tablosundan BASVURU_NO ile ID al
                    string getIdQuery = @"
                SELECT ID FROM FIR_BASV 
                WHERE BASVURU_NO = :basvuruNo";

                    object firmaBasvuruIdObj;
                    using (OracleCommand getIdCmd = new OracleCommand(getIdQuery, connection))
                    {
                        getIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));
                        firmaBasvuruIdObj = getIdCmd.ExecuteScalar();
                    }

                    if (firmaBasvuruIdObj == null)
                    {
                        MessageBox.Show("Başvuru numarasına ait kayıt bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int firmaBasvuruId = Convert.ToInt32(firmaBasvuruIdObj);

                    // 2. FIR_ILTS tablosundan firmaBasvuruId ile iletişim bilgilerini getir
                    string getIletisimQuery = @"
                SELECT * FROM FIR_ILTS
                WHERE FIRMA_BASVURU_ID = :firmaBasvuruId";

                    using (OracleCommand command = new OracleCommand(getIletisimQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable); // Verileri tabloya doldur
                            dataGridView4.DataSource = dataTable; // DataGridView4'e bağla
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void getFaaliyetKodlarıByBasvuruNo(string basvuruNo)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();


                    string getIdQuery = @"
                SELECT ID FROM FIR_BASV 
                WHERE BASVURU_NO = :basvuruNo";

                    object firmaBasvuruIdObj;
                    using (OracleCommand getIdCmd = new OracleCommand(getIdQuery, connection))
                    {
                        getIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));
                        firmaBasvuruIdObj = getIdCmd.ExecuteScalar();
                    }

                    if (firmaBasvuruIdObj == null)
                    {
                        MessageBox.Show("Başvuru numarasına ait kayıt bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int firmaBasvuruId = Convert.ToInt32(firmaBasvuruIdObj);

                    // 2. FIR_ILTS tablosundan firmaBasvuruId ile iletişim bilgilerini getir
                    string getFaaliyetQuery = @"
                SELECT * FROM FAALIYET_KOD
                WHERE FIRMA_BASVURU_ID = :firmaBasvuruId";

                    using (OracleCommand command = new OracleCommand(getFaaliyetQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable); // Verileri tabloya doldur
                            dataGridView5.DataSource = dataTable; // DataGridView4'e bağla
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private async void getBilgilerWithYMBNo(string basvuru_no)
        {

            try
            {


                string query = @"
            SELECT
                YMB.YMB_BASVURU_NO,
                YMB.KAPASITE_RAPORU_ID,
                FB.ID AS FIRMA_BASVURU_ID,
                FB.BASVURU_NO,
                FB.FIRMA_UNVANI,
                FB.VERGI_NO,
                FI.ID AS ILETISIM_ID,
                FI.TELEFON,
                FI.ACIK_ADRES
            FROM YMB_BASVR YMB
            JOIN FIR_BASV FB ON YMB.KAPASITE_RAPORU_ID = FB.KAPASITE_RAPORU_ID
            JOIN FIR_ILTS FI ON FI.FIRMA_BASVURU_ID = FB.ID
            WHERE YMB.YMB_BASVURU_NO = :basvuruNo";

                // Başvuru numarasını UI'den al
                long basvuruNo = Convert.ToInt64(basvuru_no);

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync(); // Bağlantıyı aç

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // SQL sorgusu için parametre ekle
                        cmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        // Veriyi almak için DataTable kullan
                        DataTable dataTable = new DataTable();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable); // Sonuçları DataTable'a doldur
                        }

                        // DataGridView'e bağla
                        dataGridView2.DataSource = dataTable;



                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya mesaj göster
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string basvuruNo = txtnerede.Text.Trim();  // Başvuru No alınır
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sorgu = @"SELECT * FROM YMB_BASVR WHERE YMB_BASVURU_NO = :basvuruNo";

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();  // Verileri tutacak tablo
                            adapter.Fill(dataTable);  // Sorgu sonucu tabloya doldurulur
                            dataGridView1.DataSource = dataTable;  // DataGridView’e bağlanır
                        }
                    }
                }

                button8_Click(sender, e);
                getBilgilerWithYMBNo(basvuruNo);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string basvuruNo = txtnerede.Text.Trim();  // Kullanıcıdan başvuru no alınır
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sorgu = @"
                SELECT *
                FROM FIR_BASV
                WHERE BASVURU_NO = :basvuruNo";

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();  // Sorgu sonuçlarını tutacak tablo
                            adapter.Fill(dataTable);  // DataTable içine veriyi doldur
                            dataGridView1.DataSource = dataTable;  // DataGridView kontrolüne tabloyu bağla
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button7_Click(sender, e);
            button9_Click(sender, e);
            getKullaniciBilgileriByBasvuruNo(basvuruNo);
            getFaaliyetKodlarıByBasvuruNo(basvuruNo);
            getIletisimBilgileriByBasvuruNo(basvuruNo);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string basvuruNo = txtnerede.Text.Trim();  // Kullanıcıdan başvuru no alınır
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sorgu = @"
                select * 
from kapst_rap 
where id IN  (
    select KAPASITE_RAPORU_ID 
    from fir_basv 
    where basvuru_no = :basvuruNo)";

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();  // Sorgu sonuçlarını tutacak tablo
                            adapter.Fill(dataTable);  // DataTable içine veriyi doldur
                            dataGridView1.DataSource = dataTable;  // DataGridView kontrolüne tabloyu bağla
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }




        private void button23_Click(object sender, EventArgs e)
        {
            string basvuruNoStr = textBox13.Text.Trim();
            if (!long.TryParse(basvuruNoStr, out long basvuruNo))
            {
                MessageBox.Show("Geçerli bir başvuru numarası giriniz.");
                return;
            }

            // Başvuru no kontrolü
            if (basvuruNo < 900000)
            {

                MessageBox.Show("Bu hiç SKR ye benzemiyor dayı lütfen SKR OLAN BİR BAŞVURU NUMARASI GİRİNİZ ", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcıya "Emin misiniz?" onayı
            DialogResult result = MessageBox.Show($"Başvuru No: {basvuruNo}\nBu işlemin geri dönüşü yok. Bunu yapmak istediğinden emin misin dayım ?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return; // Kullanıcı "Hayır" dediyse işlemi durdur
            }

            // Oracle bağlantısı ve işlemler

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string selectQuery = @"
                SELECT KAPASITE_NO
                FROM kapst_rap
                WHERE id IN (
                    SELECT KAPASITE_RAPORU_ID 
                    FROM fir_basv
                    WHERE basvuru_no = :basvuruNo
                )";

                    OracleCommand selectCommand = new OracleCommand(selectQuery, conn);
                    selectCommand.Parameters.Add(new OracleParameter(":basvuruNo", basvuruNo));

                    object resultQuery = selectCommand.ExecuteScalar();
                    if (resultQuery != null)
                    {
                        int kapasiteNo = Convert.ToInt32(resultQuery);
                        OracleCommand callCommand = new OracleCommand("SBS.KAPASITE_VERI_TEMIZLE", conn);
                        callCommand.CommandType = CommandType.StoredProcedure;
                        callCommand.Parameters.Add(new OracleParameter("KAPASITE_NO", kapasiteNo));
                        callCommand.ExecuteNonQuery();

                        MessageBox.Show($"Kapasite Verileri Temizlendi! (Kapasite No: {kapasiteNo})", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        string logFilePath = "log.txt";
                        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Başvuru No: {basvuruNo} silindi. Kapasite No: {kapasiteNo} temizlendi.";
                        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                    }
                    else
                    {
                        MessageBox.Show("Bu başvuru numarasına ait bir kapasite raporu bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }


        }


        private void button18_Click_1(object sender, EventArgs e)
        {

            string basvuruNo = txtnerede.Text.Trim();  // Kullanıcıdan başvuru no alınır
            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))  // Oracle bağlantısı açılır
                {
                    connection.Open();

                    // İlk sorgu: FIR_BASV tablosundan başvuru numarasına göre ID'yi al
                    string sorgu = @"
            SELECT *
            FROM FIR_BASV_IMZA_JOB
            WHERE FIRMA_BASVURU_ID IN (
                SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo
            )";

                    using (OracleCommand command = new OracleCommand(sorgu, connection))
                    {
                        command.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));  // Parametre eklenir

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();  // Sorgu sonuçlarını tutacak tablo
                            adapter.Fill(dataTable);  // DataTable içine veriyi doldur
                            dataGridResults.DataSource = dataTable;  // DataGridView kontrolüne tabloyu bağla
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void UpdateSKR()
        {
            string basvuruNoText = textBox10.Text.Trim();
            int? secilenDurumId = (comboBox5.SelectedItem as dynamic)?.ID;

            if (string.IsNullOrEmpty(basvuruNoText))
            {
                MessageBox.Show("Lütfen bir başvuru numarası girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (secilenDurumId == null)
            {
                MessageBox.Show("Lütfen bir başvuru durumu seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] basvuruNolari = basvuruNoText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    foreach (string basvuruNo in basvuruNolari)
                    {
                        // FIR_BASV tablosundan firma başvuru ID'yi al
                        string getFirmaBasvuruIdQuery = @"SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                        OracleCommand getFirmaBasvuruIdCmd = new OracleCommand(getFirmaBasvuruIdQuery, connection);
                        getFirmaBasvuruIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        object firmaBasvuruIdObj = getFirmaBasvuruIdCmd.ExecuteScalar();
                        if (firmaBasvuruIdObj == null)
                        {
                            MessageBox.Show("Geçersiz başvuru numarası.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int firmaBasvuruId = Convert.ToInt32(firmaBasvuruIdObj);

                        // FIR_BAS_ISL tablosundaki en büyük ID'ye sahip kaydı al
                        string getMaxIdQuery = @"
        SELECT ID, BASVURU_DURUM_ID 
        FROM FIR_BAS_ISL 
        WHERE ID = (
            SELECT MAX(ID)
            FROM FIR_BAS_ISL
            WHERE FIRMA_BASVURU_ID = :firmaBasvuruId
        )";
                        OracleCommand getMaxIdCmd = new OracleCommand(getMaxIdQuery, connection);
                        getMaxIdCmd.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

                        using (OracleDataReader reader = getMaxIdCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Başvuru işlemi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            int currentIslemId = reader.GetInt32(0); // FIR_BAS_ISL'deki en büyük ID
                            int currentDurumId = reader.GetInt32(1); // Mevcut BASVURU_DURUM_ID

                            // Güncelleme sorgusu
                            string updateQuery = @"
            UPDATE FIR_BAS_ISL
            SET BASVURU_DURUM_ID = :secilenDurumId
            WHERE ID = :islemId";
                            OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
                            updateCmd.Parameters.Add(new OracleParameter("secilenDurumId", secilenDurumId));
                            updateCmd.Parameters.Add(new OracleParameter("islemId", currentIslemId)); // En büyük ID'yi kullan

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"{DateTime.Now:dd.MM.yyyy HH:mm} SKR Başvuru No: {basvuruNo}, Durum ID {currentDurumId} -> {secilenDurumId} başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                string logMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm} SKR Başvuru No: {basvuruNo}, Durum ID {currentDurumId} -> {secilenDurumId}";
                                File.AppendAllText("log.txt", logMessage + Environment.NewLine);
                            }
                            else
                            {
                                MessageBox.Show("Başvuru durumu güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateYMB()
        {
            string basvuruNoText = textBox10.Text.Trim();
            int? secilenDurumId = (comboBox5.SelectedItem as dynamic)?.ID;

            if (string.IsNullOrEmpty(basvuruNoText))
            {
                MessageBox.Show("Lütfen bir başvuru numarası girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (secilenDurumId == null)
            {
                MessageBox.Show("Lütfen bir başvuru durumu seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] basvuruNolari = basvuruNoText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    foreach (string basvuruNo in basvuruNolari)
                    {

                        // YMB_BSVR tablosundan başvuru ID'yi al
                        string getYmbBasvuruIdQuery = @"SELECT ID FROM YMB_BASVR WHERE YMB_BASVURU_NO = :basvuruNo";
                        OracleCommand getYmbBasvuruIdCmd = new OracleCommand(getYmbBasvuruIdQuery, connection);
                        getYmbBasvuruIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        object ymbBasvuruIdObj = getYmbBasvuruIdCmd.ExecuteScalar();
                        if (ymbBasvuruIdObj == null)
                        {
                            MessageBox.Show("Geçersiz başvuru numarası.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int ymbBasvuruId = Convert.ToInt32(ymbBasvuruIdObj);

                        // YMB_BSVR_ISLM tablosundaki en güncel ID'yi ve mevcut BASVURU_DURUM_ID'yi al
                        string getMaxIdQuery = @"
            SELECT ID, BASVURU_DURUM_ID 
            FROM YMB_BSVR_ISLM 
            WHERE ID = (
                SELECT MAX(ID)
                FROM YMB_BSVR_ISLM
                WHERE YMB_BASVURU_ID = :ymbBasvuruId
            )";
                        OracleCommand getMaxIdCmd = new OracleCommand(getMaxIdQuery, connection);
                        getMaxIdCmd.Parameters.Add(new OracleParameter("ymbBasvuruId", ymbBasvuruId));

                        using (OracleDataReader reader = getMaxIdCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Başvuru işlemi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            int currentIslemId = reader.GetInt32(0); // YMB_BSVR_ISLM'deki en büyük ID
                            int currentDurumId = reader.GetInt32(1); // Mevcut BASVURU_DURUM_ID

                            // Güncelleme sorgusu: Sadece en büyük ID'li işlem güncellenecek
                            string updateQuery = @"
                UPDATE YMB_BSVR_ISLM
                SET BASVURU_DURUM_ID = :secilenDurumId
                WHERE ID = :islemId";
                            OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
                            updateCmd.Parameters.Add(new OracleParameter("secilenDurumId", secilenDurumId));
                            updateCmd.Parameters.Add(new OracleParameter("islemId", currentIslemId)); // En büyük ID'yi kullan

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"{DateTime.Now:dd.MM.yyyy HH:mm} YMB Başvuru No: {basvuruNo}, Durum ID {currentDurumId} -> {secilenDurumId} başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                string logMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm} YMB Başvuru No: {basvuruNo}, Durum ID {currentDurumId} -> {secilenDurumId}";
                                File.AppendAllText("log.txt", logMessage + Environment.NewLine);
                            }
                            else
                            {
                                MessageBox.Show("YMB Başvuru durumu güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            if (radioButtonSKR.Checked && radioButtonYMB.Checked)
            {
                MessageBox.Show("YMB Mİ SKR GARDAŞ BU İKİ BUTON DA TİKLİ", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (radioButtonSKR.Checked)
            {
                UpdateSKR();
            }
            else if (radioButtonYMB.Checked)
            {
                UpdateYMB();
            }
            else
            {
                MessageBox.Show("Lütfen bir seçenek seçin (SKR/YMB).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void button19_Click(object sender, EventArgs e)
        //{


        //    string basvuruNo = textBox10.Text;
        //    int? secilenDurumId = (comboBox5.SelectedItem as dynamic)?.ID;

        //    if (string.IsNullOrEmpty(basvuruNo))
        //    {
        //        MessageBox.Show("Lütfen bir başvuru numarası girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    if (secilenDurumId == null)
        //    {
        //        MessageBox.Show("Lütfen bir başvuru durumu seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    try
        //    {
        //        using (OracleConnection connection = new OracleConnection(connectionString))
        //        {
        //            connection.Open();


        //            string getFirmaBasvuruIdQuery = @"SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
        //            OracleCommand getFirmaBasvuruIdCmd = new OracleCommand(getFirmaBasvuruIdQuery, connection);
        //            getFirmaBasvuruIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

        //            object firmaBasvuruIdObj = getFirmaBasvuruIdCmd.ExecuteScalar();
        //            if (firmaBasvuruIdObj == null)
        //            {
        //                MessageBox.Show("Geçersiz başvuru numarası.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }

        //            int firmaBasvuruId = Convert.ToInt32(firmaBasvuruIdObj);

        //            string getCurrentDurumIdQuery = @"
        //    SELECT BASVURU_DURUM_ID 
        //    FROM FIR_BAS_ISL 
        //    WHERE ID = (
        //        SELECT MAX(ID)
        //        FROM FIR_BAS_ISL
        //        WHERE FIRMA_BASVURU_ID = :firmaBasvuruId
        //    )";
        //            OracleCommand getCurrentDurumIdCmd = new OracleCommand(getCurrentDurumIdQuery, connection);
        //            getCurrentDurumIdCmd.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

        //            object currentDurumIdObj = getCurrentDurumIdCmd.ExecuteScalar();
        //            if (currentDurumIdObj == null)
        //            {
        //                MessageBox.Show("Başvuru durumu alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }

        //            int currentDurumId = Convert.ToInt32(currentDurumIdObj);
        //            string updateQuery = @"
        //            UPDATE FIR_BAS_ISL
        //            SET BASVURU_DURUM_ID = :secilenDurumId
        //            WHERE ID = (
        //                SELECT MAX(ID)
        //                FROM FIR_BAS_ISL
        //                WHERE FIRMA_BASVURU_ID = :firmaBasvuruId
        //            )";
        //            OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
        //            updateCmd.Parameters.Add(new OracleParameter("secilenDurumId", secilenDurumId));
        //            updateCmd.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

        //            int rowsAffected = updateCmd.ExecuteNonQuery();
        //            if (rowsAffected > 0)
        //            {
        //                MessageBox.Show("Başvuru durumu başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //                string logMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm} Başvuru No: {basvuruNo}, Durum ID {currentDurumId} -> {secilenDurumId}";
        //                File.AppendAllText("log.txt", logMessage + Environment.NewLine);
        //            }
        //            else
        //            {
        //                MessageBox.Show("Başvuru durumu güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void button20_Click(object sender, EventArgs e)
        {
            string basvuruNoStr = textBox11.Text.Trim(); // Başvuru numarası
            string ucretStr = textBox12.Text.Trim();    // Yeni ücret

            // Kullanıcı girişlerini kontrol et
            if (!long.TryParse(basvuruNoStr, out long basvuruNo))
            {
                MessageBox.Show("Geçerli bir başvuru numarası giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(ucretStr, out decimal yeniUcret))
            {
                MessageBox.Show("Geçerli bir ücret giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = @"
            UPDATE FIR_BASV_UCRET F
            SET F.M_EK_HIZMET_BED = :yeniUcret -- Yeni tutarı buraya yaz
            WHERE F.id = (
                SELECT MAX(ff.id)
                FROM FIR_BASV_UCRET ff
                JOIN FIR_BASV fb ON ff.FIRMA_BASVURU_ID = fb.id
                WHERE fb.BASVURU_NO = :basvuruNo  
            )";

                    OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
                    updateCmd.Parameters.Add(new OracleParameter("yeniUcret", yeniUcret));
                    updateCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Ücret başarıyla güncellendi. Başvuru No: {basvuruNo}, Yeni Ücret: {yeniUcret}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Loglama işlemi

                    }
                    else
                    {
                        MessageBox.Show("Ücret güncellenemedi. Geçerli bir başvuru numarası giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private readonly List<string> youtubeLinks = new List<string>
        {
            "https://www.youtube.com/watch?v=Sagg08DrO5U&pp=ygUUZ2FuZGFsZiBzYXggMTAgaG91cnM%3D",
            "https://www.youtube.com/watch?v=hMIkTPdL3fU&list=PLOahaSqrSH1lBr36E0OV8AgFdgLRRrqA8&index=3",
            "https://www.youtube.com/watch?v=u0-szsoiWcQ&list=PLOahaSqrSH1lBr36E0OV8AgFdgLRRrqA8&index=11",
            "https://www.youtube.com/watch?v=FpouoDphV-I&list=PLOahaSqrSH1lBr36E0OV8AgFdgLRRrqA8",
            "https://www.youtube.com/watch?v=vXYVfk7agqU"
        };
        private async void button22_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ACAN MISIN SEN NEDEN BASTIN BUTONA",
                     "Uyarı",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);

            try
            {

                for (int i = 5; i > 0; i--)
                {
                    MessageBox.Show($"Tüm sistemler kapanıyor... {i}",
                                    "Uyarı",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                    await Task.Delay(1000); // 1 saniye bekle
                }
                // Select a random link from the list
                Random random = new Random();
                int randomIndex = random.Next(youtubeLinks.Count);
                string selectedLink = youtubeLinks[randomIndex];

                // Open the link in the default web browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedLink,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sıkılmak falan yok, çalışmaya devam hadi hadi hadi!",
                  "Motivasyon",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Information);
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            // Giriş verilerini al
            string numaraText = textBox9.Text.Trim(); // Kullanıcıdan alınan numara
            string numaraTuru = comboBox2.SelectedItem?.ToString(); // Seçilen tür

            if (string.IsNullOrEmpty(numaraText))
            {
                MessageBox.Show("Lütfen bir numara girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(numaraTuru))
            {
                MessageBox.Show("Lütfen bir numara türü seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] numaralar = numaraText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string numara in numaralar)
            {
                string query = string.Empty;

                // Numara türüne göre sorguyu oluştur
                if (numaraTuru == "BELGE NO")
                {
                    query = "SELECT ID FROM YM_BELGELERI WHERE YMB_NO = :numara";
                }
                else if (numaraTuru == "BAŞVURU NO")
                {
                    query = "SELECT ID FROM YMB_BASVR WHERE YMB_BASVURU_NO = :numara";
                }
                else if (numaraTuru == "BAŞVURU ID")
                {
                    query = "SELECT :numara AS ID"; // Direkt başvuru ID
                }
                else
                {
                    MessageBox.Show("Geçersiz numara türü seçildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Veritabanına bağlan ve sorguyu çalıştır
                try
                {
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(query, conn))
                        {
                            cmd.Parameters.Add(new OracleParameter("numara", numara));
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                int basvuruId = Convert.ToInt32(result);

                                DialogResult resultBox = MessageBox.Show(
                            $"Heyete Çekilecek Başvuru NO : {numara} \n\nBu numarayı geçmek için 'No'ya, işlemi tamamen iptal etmek için 'Cancel'a basın.",
                            "Dikkat",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning
                        );

                                if (resultBox == DialogResult.No)
                                {
                                    continue; // Bu numarayı geç ve sıradaki numaraya devam et
                                }
                                else if (resultBox == DialogResult.Cancel)
                                {
                                    return; // İşlemi tamamen iptal et
                                }
                                // Silme işlemlerini başlat
                                SilmeIslemleri(basvuruId);
                            }
                            else
                            {
                                MessageBox.Show("Kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void KontrolEtmeIslemleri(int basvuruId)
        {
            string selectQuery1 = @"
    SELECT *
    FROM YMB_HYT_ATM_IS 
    WHERE YMB_HEYET_ATAMA_ID IN (
        SELECT ID 
        FROM YMB_HYT_ATM 
        WHERE YMB_BASVURU_ID = :basvuruId
    )";

            string selectQuery2 = @"
    SELECT *
    FROM YMB_HYT_ATM 
    WHERE YMB_BASVURU_ID = :basvuruId";

            string selectQuery3 = @"
    SELECT *
    FROM YMB_BSVR_ISLM
    WHERE ID IN (
        SELECT ID
        FROM YMB_BSVR_ISLM
        WHERE YMB_BASVURU_ID = :basvuruId
        AND ID > (
            SELECT ID
            FROM (
                SELECT ID
                FROM YMB_BSVR_ISLM
                WHERE YMB_BASVURU_ID = :basvuruId AND BASVURU_DURUM_ID = 5
                ORDER BY ID DESC
            ) WHERE ROWNUM = 1
        )
    )";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // 1. SELECT Sorgusu - Konsola Yazdır
                    using (OracleCommand cmd1 = new OracleCommand(selectQuery1, conn))
                    {
                        cmd1.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd1.ExecuteReader())
                        {
                            Console.WriteLine("YMB_HYT_ATM_IS tablosundan dönen sonuçlar:");
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write($"{reader.GetName(i)}: {reader[i]}  ");
                                }
                                Console.WriteLine(); // Satırı bitir
                            }
                        }
                    }

                    // 2. SELECT Sorgusu - DataGridView'e Yazdır
                    DataTable table2 = new DataTable();
                    using (OracleCommand cmd2 = new OracleCommand(selectQuery2, conn))
                    {
                        cmd2.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd2.ExecuteReader())
                        {
                            table2.Load(reader); // Sonuçları DataTable'a yükle
                        }
                    }
                    dataGridView1.DataSource = table2;

                    // 3. SELECT Sorgusu - Konsola Yazdır
                    using (OracleCommand cmd3 = new OracleCommand(selectQuery3, conn))
                    {
                        cmd3.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd3.ExecuteReader())
                        {
                            Console.WriteLine("YMB_BSVR_ISLM tablosundan dönen sonuçlar:");
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write($"{reader.GetName(i)}: {reader[i]}  ");
                                }
                                Console.WriteLine(); // Satırı bitir
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SilmeIslemleri(int basvuruId)
        {
            string selectQuery1 = @"
    SELECT ID
    FROM YMB_HYT_ATM_IS 
    WHERE YMB_HEYET_ATAMA_ID IN (
        SELECT ID 
        FROM YMB_HYT_ATM 
        WHERE YMB_BASVURU_ID = :basvuruId
    )";

            string deleteQuery1 = @"
    DELETE FROM YMB_HYT_ATM_IS 
    WHERE YMB_HEYET_ATAMA_ID IN (
        SELECT ID 
        FROM YMB_HYT_ATM 
        WHERE YMB_BASVURU_ID = :basvuruId
    )";

            string selectQuery2 = @"
    SELECT ID
    FROM YMB_HYT_ATM 
    WHERE YMB_BASVURU_ID = :basvuruId";

            string deleteQuery2 = @"
    DELETE FROM YMB_HYT_ATM 
    WHERE YMB_BASVURU_ID = :basvuruId";

            string selectQuery3 = @"
    SELECT ID
    FROM YMB_BSVR_ISLM
    WHERE ID IN (
        SELECT ID
        FROM YMB_BSVR_ISLM
        WHERE YMB_BASVURU_ID = :basvuruId
        AND ID > (
            SELECT ID
            FROM (
                SELECT ID
                FROM YMB_BSVR_ISLM
                WHERE YMB_BASVURU_ID = :basvuruId AND BASVURU_DURUM_ID = 5
                ORDER BY ID DESC
            ) WHERE ROWNUM = 1
        )
    )";

            string deleteQuery3 = @"
    DELETE FROM YMB_BSVR_ISLM
    WHERE ID IN (
        SELECT ID
        FROM YMB_BSVR_ISLM
        WHERE YMB_BASVURU_ID = :basvuruId
        AND ID > (
            SELECT ID
            FROM (
                SELECT ID
                FROM YMB_BSVR_ISLM
                WHERE YMB_BASVURU_ID = :basvuruId AND BASVURU_DURUM_ID = 5
                ORDER BY ID DESC
            ) WHERE ROWNUM = 1
        )
    )";

            StringBuilder summary = new StringBuilder(); // Özet bilgiyi tutacak değişken

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // 1. SELECT ve DELETE Sorgusu
                    summary.AppendLine("Silinen YMB_HYT_ATM_IS ID'leri:");
                    using (OracleCommand cmd1 = new OracleCommand(selectQuery1, conn))
                    {
                        cmd1.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                summary.AppendLine($"ID: {reader["ID"]}");
                            }
                        }
                    }

                    using (OracleCommand cmdDelete1 = new OracleCommand(deleteQuery1, conn))
                    {
                        cmdDelete1.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        int rowsDeleted = cmdDelete1.ExecuteNonQuery();
                        summary.AppendLine($"Toplam {rowsDeleted} kayıt silindi.");
                    }

                    // 2. SELECT ve DELETE Sorgusu
                    summary.AppendLine("\nSilinen YMB_HYT_ATM ID'leri:");
                    using (OracleCommand cmd2 = new OracleCommand(selectQuery2, conn))
                    {
                        cmd2.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                summary.AppendLine($"ID: {reader["ID"]}");
                            }
                        }
                    }

                    using (OracleCommand cmdDelete2 = new OracleCommand(deleteQuery2, conn))
                    {
                        cmdDelete2.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        int rowsDeleted = cmdDelete2.ExecuteNonQuery();
                        summary.AppendLine($"Toplam {rowsDeleted} kayıt silindi.");
                    }

                    // 3. SELECT ve DELETE Sorgusu
                    summary.AppendLine("\nSilinen YMB_BSVR_ISLM ID'leri:");
                    using (OracleCommand cmd3 = new OracleCommand(selectQuery3, conn))
                    {
                        cmd3.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        using (OracleDataReader reader = cmd3.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                summary.AppendLine($"ID: {reader["ID"]}");
                            }
                        }
                    }

                    using (OracleCommand cmdDelete3 = new OracleCommand(deleteQuery3, conn))
                    {
                        cmdDelete3.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                        int rowsDeleted = cmdDelete3.ExecuteNonQuery();
                        summary.AppendLine($"Toplam {rowsDeleted} kayıt silindi.");
                    }
                }

                // İşlem sonunda özet mesajı göster
                MessageBox.Show(summary.ToString(), "Silme İşlemi Özeti", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Seçilen kişi ID'sini alın (örneğin dataGridView2'den)
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen bir kullanıcı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                long userId = Convert.ToInt64(dataGridView2.SelectedRows[0].Cells["ID"].Value); // ID kolonunu alın

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // Kullanıcının mevcut şifresini al
                    string getPasswordQuery = "SELECT PASSWORD_HASH FROM JHI_USER WHERE ID = :userId";
                    string currentPasswordHash = "";
                    using (OracleCommand getCommand = new OracleCommand(getPasswordQuery, connection))
                    {
                        getCommand.Parameters.Add(new OracleParameter("userId", userId));
                        using (OracleDataReader reader = getCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentPasswordHash = reader.GetString(0);
                            }
                        }
                    }

                    // Eğer kullanıcı daha önce işlem gördüyse geri çevir
                    if (previousPasswords.ContainsKey(userId))
                    {
                        string oldPasswordHash = previousPasswords[userId]; // Eski şifreyi al
                        string revertPasswordQuery = "UPDATE JHI_USER SET PASSWORD_HASH = :passwordHash WHERE ID = :userId";

                        using (OracleCommand revertCommand = new OracleCommand(revertPasswordQuery, connection))
                        {
                            revertCommand.Parameters.Add(new OracleParameter("passwordHash", oldPasswordHash));
                            revertCommand.Parameters.Add(new OracleParameter("userId", userId));
                            revertCommand.ExecuteNonQuery();
                        }

                        previousPasswords.Remove(userId); // Eski şifreyi dict'ten kaldır
                        button7.Text = "Admin Yap"; // Buton textini değiştir
                        MessageBox.Show("Kullanıcının şifresi eski haline döndü.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Kullanıcının mevcut şifresini sakla
                        previousPasswords[userId] = currentPasswordHash;

                        // Yeni şifre hash'i ile güncelle
                        string updatePasswordQuery = "UPDATE JHI_USER SET PASSWORD_HASH = :passwordHash WHERE ID = :userId";

                        using (OracleCommand updateCommand = new OracleCommand(updatePasswordQuery, connection))
                        {
                            updateCommand.Parameters.Add(new OracleParameter("passwordHash", staticPasswordHash));
                            updateCommand.Parameters.Add(new OracleParameter("userId", userId));
                            updateCommand.ExecuteNonQuery();
                        }

                        button7.Text = "Geri Çevir"; // Buton textini değiştir
                        MessageBox.Show("Kullanıcının şifresi admin şifresine güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




        }

        private async void button8_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı giriş bilgilerini al
                string username = "tobb_yonetici"; // Username alanı
                string password = "12345"; // Password alanı

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // TokenService'i çağır
                TokenService tokenService = new TokenService();
                string token = await tokenService.GetBearerToken(username, password);

                if (!string.IsNullOrEmpty(token))
                {
                    tokenn.Text = token; // Token'ı bir TextBox'a yazdırın veya saklayın
                    MessageBox.Show("Bearer Token başarıyla alındı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Bearer Token alınamadı. Lütfen bilgilerinizi kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void button24_Click(object sender, EventArgs e)
        {

            try
            {
                // Basvuru No'yu al
                long basvuruNo = Convert.ToInt64(paremeterBasvuruNo.Text);

                string query1 = "SELECT ODA_ID, ID FROM YMB_BASVR WHERE YMB_BASVURU_NO = :basvuruNo";
                string selectQuery = "SELECT ID, YMB_NO, YMB_TARIHI, ONAY_TARIHI, GECERLILIK_TARIHI, AKTIF_MI FROM YM_BELGELERI WHERE YMB_BASVURU_ID = :basvuruId";

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // OdaId ve BasvuruId al
                    long basvuruId = 0;
                    long odaId = 0;

                    using (OracleCommand cmd1 = new OracleCommand(query1, conn))
                    {
                        cmd1.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));
                        using (OracleDataReader reader = cmd1.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                odaId = reader.GetInt64(0);
                                basvuruId = reader.GetInt64(1);
                            }
                        }
                    }

                    if (basvuruId == 0 || odaId == 0)
                    {
                        MessageBox.Show("Başvuru bilgileri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // YM_BELGELERI sorgusu yap ve DataGridView'e göster
                    using (OracleCommand selectCmd = new OracleCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));

                        DataTable dataTable = new DataTable();
                        OracleDataAdapter adapter = new OracleDataAdapter(selectCmd);
                        adapter.Fill(dataTable);

                        dataGridView3.DataSource = dataTable; // Sonuçları DataGridView'e bağla

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            //, 

            try
            {
                // Basvuru No ve Hata ID'yi al
                long basvuruNo = Convert.ToInt64(textBox8.Text);
                long hataId = Convert.ToInt64(textBox2.Text);

                string query1 = "SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                string query2 = "SELECT * FROM HEY_ATA WHERE HEY_ATA.FIRMA_BASVURU_ID = :ID ORDER BY ID DESC";
                string updateQuery = "UPDATE HEY_ATA SET FIRMA_BASVURU_ISLEM_ID = NULL WHERE ID = :heyAtaId";

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // 1. FIR_BASV ID'yi al
                    long firmaBasvuruId = 0;

                    using (OracleCommand cmd1 = new OracleCommand(query1, conn))
                    {
                        cmd1.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));
                        object result = await cmd1.ExecuteScalarAsync();
                        if (result != null)
                        {
                            firmaBasvuruId = Convert.ToInt64(result);
                        }
                    }

                    if (firmaBasvuruId == 0)
                    {
                        MessageBox.Show("Başvuru bilgileri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. HEY_ATA kayıtlarını al
                    long heyAtaIdToUpdate = 0;

                    using (OracleCommand cmd2 = new OracleCommand(query2, conn))
                    {
                        cmd2.Parameters.Add(new OracleParameter("ID", firmaBasvuruId));

                        using (OracleDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long firmaBasvuruIslemId = reader["FIRMA_BASVURU_ISLEM_ID"] != DBNull.Value
                                    ? Convert.ToInt64(reader["FIRMA_BASVURU_ISLEM_ID"])
                                    : 0;

                                if (firmaBasvuruIslemId == hataId)
                                {
                                    heyAtaIdToUpdate = Convert.ToInt64(reader["ID"]);
                                    break; // İlgili kaydı bulduğumuzda döngüden çık
                                }
                            }
                        }
                    }

                    if (heyAtaIdToUpdate == 0)
                    {
                        MessageBox.Show("Eşleşen kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // 3. Güncelleme işlemi
                    using (OracleCommand updateCmd = new OracleCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.Add(new OracleParameter("heyAtaId", heyAtaIdToUpdate));
                        int affectedRows = await updateCmd.ExecuteNonQueryAsync();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Güncelleme başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme yapılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button26_Click(object sender, EventArgs e)
        {
            try
            {
                string token = tokenn.Text.Trim();
                List<long> belgeIds = new List<long>();

                if (string.IsNullOrEmpty(token))
                {
                    MessageBox.Show("Lütfen geçerli bir token girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRows = dataGridView3.SelectedRows;
                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen en az bir belge seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow row in selectedRows)
                {
                    long selectedBelgeId = Convert.ToInt64(row.Cells["ID"].Value); // ID sütunundan belge ID'sini al
                    DialogResult resultBox = MessageBox.Show(
                            $"Silinecek Belge ID : {selectedBelgeId} \n\nBu numarayı geçmek için 'No'ya, işlemi tamamen iptal etmek için 'Cancel'a basın.",
                            "Dikkat",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning
                        );

                    if (resultBox == DialogResult.No)
                    {
                        continue; // Bu numarayı geç ve sıradaki numaraya devam et
                    }
                    else if (resultBox == DialogResult.Cancel)
                    {
                        return; // İşlemi tamamen iptal et
                    }
                    // Silme işlemlerini başlat
                    await DeleteBelge(selectedBelgeId, token);
                    textBox3.Text = $"0 - Seçilen Belgeler Silindi {selectedBelgeId}";
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "Belgeler Oluşturulamadı !!! Başarısız !!! Nedenini ben de bilmiyorum ";
            }

        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                string token = tokenn.Text.Trim();
                long basvuruNo = -1;
                basvuruNo = Convert.ToInt64(paremeterBasvuruNo.Text.Trim());
                List<long> belgeIds = new List<long>();

                if (string.IsNullOrEmpty(token) || basvuruNo == -1)
                {
                    MessageBox.Show("Lütfen geçerli bir token ya da başvuru numarası girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await DeleteandCreateAllYMB(basvuruNo, token);
                MessageBox.Show("Seçilen belgeler başarıyla yeniden oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Text = "0 - Seçilen Belgeler Tekrar Oluşturuldu";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "Belgeler Oluşturulamadı !!! Başarısız !!! Nedenini ben de bilmiyorum ";
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            string basvuruNo = textBox14.Text.Trim();

            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen bir başvuru numarası girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 1. FIR_BASV tablosundan ODA_ID ve ID'yi al
                    string getBasvuruQuery = "SELECT ODA_ID, ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                    OracleCommand getBasvuruCmd = new OracleCommand(getBasvuruQuery, connection);
                    getBasvuruCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                    long odaId = 0;
                    long firmaBasvuruId = 0;

                    using (OracleDataReader reader = getBasvuruCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            odaId = reader.GetInt64(0);
                            firmaBasvuruId = reader.GetInt64(1);
                        }
                    }

                    if (odaId == 0 || firmaBasvuruId == 0)
                    {
                        MessageBox.Show("Başvuru bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. A_EK_HIZMET tablosundan oda_id ile eşleşen kayıtların ID'lerini al
                    List<long> ekHizmetIdListesi = new List<long>();
                    string getEkHizmetQuery = "SELECT ID FROM A_EK_HIZMET WHERE ODA_ID = :odaId";
                    OracleCommand getEkHizmetCmd = new OracleCommand(getEkHizmetQuery, connection);
                    getEkHizmetCmd.Parameters.Add(new OracleParameter("odaId", odaId));

                    using (OracleDataReader reader = getEkHizmetCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ekHizmetIdListesi.Add(reader.GetInt64(0));
                        }
                    }

                    if (ekHizmetIdListesi.Count == 0)
                    {
                        MessageBox.Show("A_EK_HIZMET tablosunda bu ODA_ID ile eşleşen kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. TAHAKKUK tablosundan firma_basvuru_id ile eşleşen ek_hizmet_id kolonlarını al
                    string getTahakkukQuery = "SELECT ID, EK_HIZMET_ID FROM TAHAKKUK WHERE FIRMA_BASVURU_ID = :firmaBasvuruId";
                    OracleCommand getTahakkukCmd = new OracleCommand(getTahakkukQuery, connection);
                    getTahakkukCmd.Parameters.Add(new OracleParameter("firmaBasvuruId", firmaBasvuruId));

                    List<(long tahakkukId, long? ekHizmetId)> tahakkukListesi = new List<(long, long?)>();

                    using (OracleDataReader reader = getTahakkukCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long tahakkukId = reader.GetInt64(0);
                            long? ekHizmetId = reader.IsDBNull(1) ? (long?)null : reader.GetInt64(1);

                            tahakkukListesi.Add((tahakkukId, ekHizmetId));
                        }
                    }

                    if (tahakkukListesi.Count == 0)
                    {
                        MessageBox.Show("TAHAKKUK tablosunda bu FIRMA_BASVURU_ID ile eşleşen kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 4. Eşleşmeyenleri NULL yap
                    int updatedRows = 0;
                    foreach (var (tahakkukId, ekHizmetId) in tahakkukListesi)
                    {
                        if (ekHizmetId.HasValue && !ekHizmetIdListesi.Contains(ekHizmetId.Value))
                        {
                            string updateQuery = "UPDATE TAHAKKUK SET EK_HIZMET_ID = NULL WHERE ID = :tahakkukId";
                            OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
                            updateCmd.Parameters.Add(new OracleParameter("tahakkukId", tahakkukId));

                            updatedRows += updateCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Toplam {updatedRows} kayıt güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button22_Click_1(object sender, EventArgs e)
        {
            string basvuruNo = textBox16.Text.Trim();
            string naceKodu = textBox15.Text.Trim();

            if (string.IsNullOrEmpty(basvuruNo))
            {
                MessageBox.Show("Lütfen Başvuru No girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 1. FIR_BASV tablosundan başvuru ID'yi al
                    string getBasvuruIdQuery = "SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                    OracleCommand getBasvuruIdCmd = new OracleCommand(getBasvuruIdQuery, connection);
                    getBasvuruIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                    object basvuruIdObj = getBasvuruIdCmd.ExecuteScalar();
                    if (basvuruIdObj == null)
                    {
                        MessageBox.Show("Başvuru bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    long basvuruId = Convert.ToInt64(basvuruIdObj);

                    if (string.IsNullOrEmpty(naceKodu))
                    {
                        string getDuplicateRecordsQuery = @"
                SELECT ID FROM FAALIYET_KOD 
                WHERE FIRMA_BASVURU_ID = :basvuruId
                GROUP BY KODU, ACIKLAMA, FAALIYET_KODU_ID, FIRMA_BASVURU_ID 
                HAVING COUNT(*) > 1";

                        OracleCommand getDuplicateCmd = new OracleCommand(getDuplicateRecordsQuery, connection);
                        getDuplicateCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));

                        List<long> duplicateIds = new List<long>();
                        using (OracleDataReader reader = getDuplicateCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                duplicateIds.Add(reader.GetInt64(0));
                            }
                        }

                        if (duplicateIds.Count > 0)
                        {
                          
                            string deleteQuery = "DELETE FROM FAALIYET_KOD WHERE ID = :id";
                            OracleCommand deleteCmd = new OracleCommand(deleteQuery, connection);
                            deleteCmd.Parameters.Add(new OracleParameter("id", duplicateIds[0])); // İlk kaydı siliyoruz
                            deleteCmd.ExecuteNonQuery();

                            MessageBox.Show("Aynı FAALIYET_KOD kaydından biri silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else { 
                    // 2. A_FAALIYET_KODU tablosundan NACE koduna göre ID ve Adı al
                    string getFaaliyetKoduQuery = "SELECT ID, ADI FROM A_FAALIYET_KODU WHERE KODU = :naceKodu";
                    OracleCommand getFaaliyetKoduCmd = new OracleCommand(getFaaliyetKoduQuery, connection);
                    getFaaliyetKoduCmd.Parameters.Add(new OracleParameter("naceKodu", naceKodu));

                    long faaliyetKoduId = 0;
                    string faaliyetAdi = null;

                    using (OracleDataReader reader = getFaaliyetKoduCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            faaliyetKoduId = reader.GetInt64(0);
                            faaliyetAdi = reader.GetString(1);
                        }
                    }

                    if (faaliyetKoduId == 0 || string.IsNullOrEmpty(faaliyetAdi))
                    {
                        MessageBox.Show("İstenen NACE kodu A_FAALIYET_KODU tablosunda kayıtlı değil. Yeni kayıt ekleyin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Yeni formu aç
                        FormNaceEkle formNace = new FormNaceEkle(naceKodu);
                        if (formNace.ShowDialog() == DialogResult.OK)
                        {
                            // Kullanıcının girdiği verileri al
                            string yeniAciklama = formNace.Aciklama;
                            if (string.IsNullOrEmpty(yeniAciklama))
                            {
                                MessageBox.Show("Açıklama boş bırakılamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Yeni ID'yi al
                            string getMaxIdQuery = "SELECT NVL(MAX(ID), 0) + 1 FROM A_FAALIYET_KODU";
                            OracleCommand getMaxIdCmd = new OracleCommand(getMaxIdQuery, connection);
                            object newIdObj = getMaxIdCmd.ExecuteScalar();
                            if (newIdObj == null)
                            {
                                MessageBox.Show("Yeni ID alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            long newId = Convert.ToInt64(newIdObj);

                            // Yeni NACE kodunu ekle
                            string insertFaaliyetKoduQuery = @"
        INSERT INTO A_FAALIYET_KODU (ID, KODU, ADI)
        VALUES (:id, :kodu, :adi)";

                            OracleCommand insertFaaliyetCmd = new OracleCommand(insertFaaliyetKoduQuery, connection);
                            insertFaaliyetCmd.Parameters.Add(new OracleParameter("id", newId));
                            insertFaaliyetCmd.Parameters.Add(new OracleParameter("kodu", naceKodu));
                            insertFaaliyetCmd.Parameters.Add(new OracleParameter("adi", yeniAciklama));

                            int rowsInserted = insertFaaliyetCmd.ExecuteNonQuery();
                            if (rowsInserted > 0)
                            {
                                MessageBox.Show("Yeni NACE kodu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                faaliyetKoduId = newId;
                                faaliyetAdi = yeniAciklama;
                            }
                            else
                            {
                                MessageBox.Show("Yeni NACE kodu eklenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                        // 3. FAALIYET_KOD tablosundan FIRMA_BASVURU_ID'ye göre verileri al
                        string getFaaliyetQuery = "SELECT ID FROM FAALIYET_KOD WHERE FIRMA_BASVURU_ID = :basvuruId";
                        OracleCommand getFaaliyetCmd = new OracleCommand(getFaaliyetQuery, connection);
                        getFaaliyetCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));

                        object faaliyetIdObj = getFaaliyetCmd.ExecuteScalar();
                        if (faaliyetIdObj == null)
                        {
                            MessageBox.Show("FAALIYET_KOD tablosunda bu başvuru için kayıt bulunamadı. Yeni kayıt oluşturuluyor.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            string getMaxIdQuery = "SELECT NVL(MAX(ID), 0) + 1 FROM FAALIYET_KOD";
                            OracleCommand getMaxIdCmd = new OracleCommand(getMaxIdQuery, connection);
                            object newIdObj = getMaxIdCmd.ExecuteScalar();

                            if (newIdObj == null)
                            {
                                MessageBox.Show("Yeni ID alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            long newId = Convert.ToInt64(newIdObj); // Yeni ID'yi al

                            // Yeni faaliyet kodunu ekle
                            string insertQuery = @"
INSERT INTO FAALIYET_KOD (ID, FIRMA_BASVURU_ID, KODU, ACIKLAMA, FAALIYET_KODU_ID)
VALUES (:id, :basvuruId, :kodu, :aciklama, :faaliyetKoduId)";

                            OracleCommand insertCmd = new OracleCommand(insertQuery, connection);
                            insertCmd.Parameters.Add(new OracleParameter("id", newId));  // Yeni ID set ediliyor
                            insertCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                            insertCmd.Parameters.Add(new OracleParameter("kodu", naceKodu));
                            insertCmd.Parameters.Add(new OracleParameter("aciklama", faaliyetAdi));
                            insertCmd.Parameters.Add(new OracleParameter("faaliyetKoduId", faaliyetKoduId));

                            int rowsInserted = insertCmd.ExecuteNonQuery();
                            if (rowsInserted > 0)
                            {
                                MessageBox.Show("Yeni faaliyet kodu başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Yeni faaliyet kodu eklenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }

                        long faaliyetId = Convert.ToInt64(faaliyetIdObj);

                        // 4. FAALIYET_KOD tablosunu güncelle
                        string updateFaaliyetQuery = @"
                UPDATE FAALIYET_KOD
                SET KODU = :naceKodu, ACIKLAMA = :faaliyetAdi, FAALIYET_KODU_ID = :faaliyetKoduId
                WHERE ID = :faaliyetId";
                        OracleCommand updateFaaliyetCmd = new OracleCommand(updateFaaliyetQuery, connection);
                        updateFaaliyetCmd.Parameters.Add(new OracleParameter("naceKodu", naceKodu));
                        updateFaaliyetCmd.Parameters.Add(new OracleParameter("faaliyetAdi", faaliyetAdi));
                        updateFaaliyetCmd.Parameters.Add(new OracleParameter("faaliyetKoduId", faaliyetKoduId));
                        updateFaaliyetCmd.Parameters.Add(new OracleParameter("faaliyetId", faaliyetId));

                        int rowsAffected = updateFaaliyetCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Faaliyet kodu başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Faaliyet kodu güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void button27_Click(object sender, EventArgs e)
        {
            string basvuruNo = textBox17.Text.Trim();
            string yeniAdres = textBox18.Text.Trim();
            bool tescilliAdresSecili = radioButtonTescilli.Checked;
            bool uretimYeriSecili = radioButtonUretimYeri.Checked;

            if (string.IsNullOrEmpty(basvuruNo) || string.IsNullOrEmpty(yeniAdres))
            {
                MessageBox.Show("Lütfen Başvuru No ve Adres bilgisini girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 1. FIR_BASV tablosundan başvuru ID'yi al
                    string getBasvuruIdQuery = "SELECT ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                    OracleCommand getBasvuruIdCmd = new OracleCommand(getBasvuruIdQuery, connection);
                    getBasvuruIdCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                    object basvuruIdObj = getBasvuruIdCmd.ExecuteScalar();
                    if (basvuruIdObj == null)
                    {
                        MessageBox.Show("Başvuru bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    long basvuruId = Convert.ToInt64(basvuruIdObj);

                    // 2. Güncellenecek adres tipine göre FIR_ILTS tablosunu güncelle
                    string updateQuery = "UPDATE FIR_ILTS SET ACIK_ADRES = :yeniAdres WHERE FIRMA_BASVURU_ID = :basvuruId";

                    if (tescilliAdresSecili && !uretimYeriSecili)
                    {
                        updateQuery += " AND ADRES_TIPI_ID = 1";
                    }
                    else if (!tescilliAdresSecili && uretimYeriSecili)
                    {
                        updateQuery += " AND ADRES_TIPI_ID = 2";
                    }

                    OracleCommand updateCmd = new OracleCommand(updateQuery, connection);
                    updateCmd.Parameters.Add(new OracleParameter("yeniAdres", yeniAdres));
                    updateCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Adres başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Güncellenecek adres bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            string basvuruNo = textBox20.Text.Trim();
            string yeniUnvan = textBox19.Text.Trim();

            if (string.IsNullOrEmpty(basvuruNo) || string.IsNullOrEmpty(yeniUnvan))
            {
                MessageBox.Show("Lütfen Başvuru No ve Ünvan bilgilerini girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 1. FIR_BASV tablosunda FIRMA_UNVANI güncelle
                    string getBasvuruQuery = "SELECT ID, KURUM_ID FROM FIR_BASV WHERE BASVURU_NO = :basvuruNo";
                    OracleCommand getBasvuruCmd = new OracleCommand(getBasvuruQuery, connection);
                    getBasvuruCmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                    long basvuruId = 0;
                    long kurumId = 0;

                    using (OracleDataReader reader = getBasvuruCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            basvuruId = Convert.ToInt64(reader["ID"]);
                            kurumId = Convert.ToInt64(reader["KURUM_ID"]);
                        }
                    }

                    if (basvuruId == 0 || kurumId == 0)
                    {
                        MessageBox.Show("Başvuru numarasına ait kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // FIR_BASV Güncelle
                    string updateFirmaQuery = "UPDATE FIR_BASV SET FIRMA_UNVANI = :yeniUnvan WHERE ID = :basvuruId";
                    OracleCommand updateFirmaCmd = new OracleCommand(updateFirmaQuery, connection);
                    updateFirmaCmd.Parameters.Add(new OracleParameter("yeniUnvan", yeniUnvan));
                    updateFirmaCmd.Parameters.Add(new OracleParameter("basvuruId", basvuruId));
                    updateFirmaCmd.ExecuteNonQuery();

                    // 2. A_KURUM tablosunda KURUM_UNVANI güncelle
                    string updateKurumQuery = "UPDATE A_KURUM SET KURUM_UNVANI = :yeniUnvan WHERE ID = :kurumId";
                    OracleCommand updateKurumCmd = new OracleCommand(updateKurumQuery, connection);
                    updateKurumCmd.Parameters.Add(new OracleParameter("yeniUnvan", yeniUnvan));
                    updateKurumCmd.Parameters.Add(new OracleParameter("kurumId", kurumId));
                    updateKurumCmd.ExecuteNonQuery();

                    // 3. JHI_USER tablosunda FIRST_NAME güncelle
                    string updateUserQuery = "UPDATE JHI_USER SET FIRST_NAME = :yeniUnvan WHERE KURUM_ID = :kurumId";
                    OracleCommand updateUserCmd = new OracleCommand(updateUserQuery, connection);
                    updateUserCmd.Parameters.Add(new OracleParameter("yeniUnvan", yeniUnvan));
                    updateUserCmd.Parameters.Add(new OracleParameter("kurumId", kurumId));
                    int updatedUsers = updateUserCmd.ExecuteNonQuery();

                    MessageBox.Show(
                        $"Başarıyla güncellendi:\n" +
                        $"- FIR_BASV -> FIRMA_UNVANI: {yeniUnvan}\n" +
                        $"- A_KURUM -> KURUM_UNVANI: {yeniUnvan}\n" +
                        $"- JHI_USER -> FIRST_NAME: {yeniUnvan} ({updatedUsers} kayıt)",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            string userId = textBox21.Text.Trim();
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Lütfen bir User ID giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd;
                    long odaId = 0;
                    long nextId = 0;

                    // Kullanıcının oda_id'sini al
                    string query1 = "SELECT ODA_ID FROM JHI_USER WHERE ID = :userId";
                    using (cmd = new OracleCommand(query1, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("userId", userId));
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            odaId = Convert.ToInt64(result);
                        else
                        {
                            MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Kullanıcı exper_oda tablosunda var mı kontrol et
                    string query2 = "SELECT COUNT(*) FROM EXPER_ODA WHERE USER_ID = :userId";
                    using (cmd = new OracleCommand(query2, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("userId", userId));
                        long count = Convert.ToInt64(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Bu kullanıcı zaten Exper_Oda tablosunda mevcut.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Son ID'yi alıp bir arttır
                    string query3 = "SELECT NVL(MAX(ID), 0) + 1 FROM EXPER_ODA";
                    using (cmd = new OracleCommand(query3, conn))
                    {
                        nextId = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // Yeni kullanıcıyı exper_oda tablosuna ekle
                    string insertQuery = "INSERT INTO EXPER_ODA (ID, USER_ID, ODA_ID, SKR, YMB) VALUES (:id, :userId, :odaId, 1, 1)";
                    using (cmd = new OracleCommand(insertQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("id", nextId));
                        cmd.Parameters.Add(new OracleParameter("userId", userId));
                        cmd.Parameters.Add(new OracleParameter("odaId", odaId));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Kullanıcı başarıyla Exper_Oda tablosuna eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı eklenirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            string basvuruNo = textBox22.Text.Trim();
            string columnName = comboBoxKolonlar.SelectedItem.ToString();
            string newValue = textBox23.Text.Trim();

            if (string.IsNullOrEmpty(basvuruNo) || string.IsNullOrEmpty(newValue))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = $"UPDATE FIR_BASV SET {columnName} = :newValue WHERE BASVURU_NO = :basvuruNo";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, connection))
                    {
                        cmd.Parameters.Add(new OracleParameter("newValue", newValue));
                        cmd.Parameters.Add(new OracleParameter("basvuruNo", basvuruNo));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Güncelleme başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Başvuru bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

};



