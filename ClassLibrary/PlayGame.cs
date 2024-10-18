namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences()
        {
            Console.WriteLine("Förbered dig...");

            ApiSentences apiSentences = new ApiSentences();
            string url = "https://api.kanye.rest";

            string apiData = await apiSentences.sentencesFromApi(url);
            
            //för att fördröja visning av mening
            int milliseconds = 3000;
            Thread.Sleep(milliseconds);

            Console.WriteLine(apiData);
        }
    }
}
