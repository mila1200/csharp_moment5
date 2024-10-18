﻿namespace Typeracer
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
                return responseData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Det uppstod ett fel när API:n anropades: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
