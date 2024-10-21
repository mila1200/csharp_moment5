namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences()
        {
            Console.WriteLine("F�rbered dig...");

            ApiSentences apiSentences = new ApiSentences();
            string url = "https://api.kanye.rest";

            string quote = await apiSentences.sentencesFromApi(url);

            //f�r att f�rdr�ja visning av mening
            int milliseconds = 3000;
            Thread.Sleep(milliseconds);

            if (quote != null)
            {
                Console.WriteLine(quote);
            }
            else
            {
                Console.WriteLine("Ett ov�ntat fel intr�ffade. Meningen kunde inte h�mtas.");
            }


        }
    }
}
