namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences()
        {
            Console.WriteLine("Förbered dig...");

            ApiSentences apiSentences = new ApiSentences();
            string url = "https://api.kanye.rest";

            string quote = await apiSentences.sentencesFromApi(url);

            //för att fördröja visning av mening
            int milliseconds = 3000;
            Thread.Sleep(milliseconds);

            if (quote != null)
            {
                Console.WriteLine(quote);
            }
            else
            {
                Console.WriteLine("Ett oväntat fel inträffade. Meningen kunde inte hämtas.");
            }


        }
    }
}
