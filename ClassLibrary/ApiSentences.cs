using System.Text.Json;

namespace Typeracer
{
    public class ApiSentences
    {

        //endast tillgänglig inom klassen, variabeln kan inte ändras i efterhand
        private readonly HttpClient client;

        public ApiSentences()
        {
            client = new HttpClient();
        }

        public async Task<string> sentencesFromApi(string url)
        {
            try
            {
                //skicka GET-förfrågan
                HttpResponseMessage responseFromApi = await client.GetAsync(url);

                //felhantering
                responseFromApi.EnsureSuccessStatusCode();

                string responseData = await responseFromApi.Content.ReadAsStringAsync();

                //tar ut meningen från objektet
                using (JsonDocument document = JsonDocument.Parse(responseData))
                {
                    string? quote = document.RootElement.GetProperty("quote").GetString();
                    return quote ?? string.Empty;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Det uppstod ett fel när API:n anropades: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
