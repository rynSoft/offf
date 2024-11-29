using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Oracle.ManagedDataAccess.Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient client = new HttpClient();
        // Oracle Database Connection String
        private const string connectionString =
            "User Id=SBS;Password=San2934BS+Prod;" +
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=soradb-scan.tobb.org.tr)(PORT=1521))" +
            "(CONNECT_DATA=(SERVICE_NAME=TOBB)))";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            // Optional: Add any initialization code here
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
                    
                    OracleCommand command = new OracleCommand("SELECT ID,KAPASITE_RAPORU_ID,BASVURU_NO FROM SBS.FIR_BASV WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")", connection);
                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                      
                        Logger.Info($"FIR_BASV Data - Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}" +
                                     $" Basvuru No : {rdr.GetString(2)}");

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
                                    Logger.Info($"COMMIT DONE! FIR_BASV Data - Id : {rdr.GetString(0)}");
                                    MessageBox.Show("Transaction committed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch (Exception ex)
                                {
                                    // Rollback the transaction in case of an error
                                    transaction.Rollback();
                                    Logger.Error(ex, $"Transaction rolled back due to error: {ex.Message}");
                                }
                            }
                        }

                        rdr.Dispose();
                        command.Dispose();
                    }
        
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
            var data =(JObject)JsonConvert.DeserializeObject(result);
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
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand(" SELECT ID,KAPASITE_RAPORU_ID,BASVURU_NO "+
                                                              " FROM SBS.FIR_BASV WHERE BASVURU_NO IN (" + paremeterBasvuruNo.Text + ")", connection);
                    OracleDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        Logger.Info($" Case_2 Bas - Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}" +
                                    $" Basvuru No : {rdr.GetString(2)}");

                        bool isSuccess1 = long.TryParse(rdr.GetString(0), out long firmaBasvuruId);
                        bool isSuccess2 = long.TryParse(rdr.GetString(1), out long kapasiteNo);

                        var result = PostAsync(firmaBasvuruId, kapasiteNo).GetAwaiter().GetResult();

                        var data = (JObject)JsonConvert.DeserializeObject(result);
                        textBox3.Text = data["sonuc"].Value<string>();

                        Logger.Info($" Case_2 Son - Id : {rdr.GetString(0)} KapasiteRaporuId : {rdr.GetString(1)}" +
                         $" Basvuru No : {rdr.GetString(2)}");
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

        private void button4_Click(object sender, EventArgs e)
        {
            Case_3();
        }
    }

    public class Datas
    {
        public int rapor { get; set; }
        public int raporFormat { get; set; }
        public long firmaBasvuruId { get; set; }
        public string kapasiteNo { get; set; }
    }
}
