namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences()
        {
            Console.WriteLine("F�rbered dig...");

            ApiSentences apiSentences = new ApiSentences();
            string url = "https://api.kanye.rest";

            string apiData = await apiSentences.sentencesFromApi(url);
            
            //f�r att f�rdr�ja visning av mening
            int milliseconds = 3000;
            Thread.Sleep(milliseconds);

            Console.WriteLine(apiData);
        }
    }
}
