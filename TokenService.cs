using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class TokenService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> GetBearerToken(string username, string password)
    {
        string apiUrl = "https://sanayi.org.tr/api/authenticate"; // API URL

        // JSON payload oluştur
        var payload = new { username = username, password = password };

        // JSON'u serialize et
        string jsonPayload = JsonConvert.SerializeObject(payload);

        // HTTP POST isteği oluştur
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        try
        {
            // POST isteğini gönder
            HttpResponseMessage response = await client.SendAsync(request);

            // Başarılı bir cevap aldıysak
            if (response.IsSuccessStatusCode)
            {
                // Cevabı JSON olarak al
                string responseContent = await response.Content.ReadAsStringAsync();

                // JSON'dan token bilgisini çıkar
                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Bearer token'ı al
                string token = responseJson.id_token;

                Console.WriteLine("Token: " + token);

                return token; // Token'ı döndür
            }
            else
            {
                Console.WriteLine("Hata: " + response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return null;
        }
    }
}
