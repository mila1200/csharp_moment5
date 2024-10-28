using System.Text.Json;

namespace Typeracer
{
    //klass för att hämta meningar från ett API
    public class ApiSentences
    {

        //endast tillgänglig inom klassen, variabeln kan inte ändras i efterhand
        private readonly HttpClient client;

        //skapar ny instans av HttpClient
        public ApiSentences()
        {
            client = new HttpClient();
        }

        //asynkron metod som tar en url och returnerar en sträng när den är klar.
        public async Task<string> sentencesFromApi(string url)
        {
            try
            {
                //skicka GET-förfrågan
                HttpResponseMessage responseFromApi = await client.GetAsync(url);

                //felhantering
                responseFromApi.EnsureSuccessStatusCode();

                //om svar ok, lagra svaret i en variabel
                string responseData = await responseFromApi.Content.ReadAsStringAsync();

                //tar ut meningen från objektet. Det vill säga, använder JsonDocument för att leta efter quote och returnera dess värde.
                using (JsonDocument document = JsonDocument.Parse(responseData))
                {
                    string? quote = document.RootElement.GetProperty("quote").GetString();
                    return quote ?? "Ett fel inträffade";
                }
                
            }
            //felhantering
            catch (Exception ex)
            {
                Console.WriteLine($"Det uppstod ett fel när API:n anropades: {ex.Message}");
                return "Ett fel inträffade";
            }
        }
    }
}
