using System.Diagnostics;

namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences(string playMenu)
        {
            try
            {
                Console.WriteLine("Förbered dig...");

                ApiSentences apiSentences = new ApiSentences();
                string url = "https://api.kanye.rest";

                string quote = await apiSentences.sentencesFromApi(url);

                //för att fördröja visning av mening
                await Task.Delay(3000);

                if (!String.IsNullOrWhiteSpace(quote))
                    {
                    await StartTypingTest(quote, playMenu);
                }
                else
                {
                    Console.WriteLine("Ett oväntat fel inträffade. Meningen kunde inte hämtas.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");
            }
        }

        public async Task StartTypingTest(string quote, string playMenu)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(quote);

                //beräkna tid
                Stopwatch stopWatchWrite = new Stopwatch();
                stopWatchWrite.Start();

                string? registeredUserInput = Console.ReadLine();

                stopWatchWrite.Stop();

                TimeSpan elapsedTime = stopWatchWrite.Elapsed;

                //felhantering om användaren inte anger något
                if (String.IsNullOrWhiteSpace(registeredUserInput))
                {
                    Console.WriteLine("Ogiltigt resultat. Du måste skriva något.");
                    Console.WriteLine("Tryck på valfri tangent...");
                    Console.ReadKey();
                    await PlayAgain(playMenu);
                    return;
                }

                int typedLength = registeredUserInput.Length;

                //antal felskrivningar
                int mistakes = 0;

                for (int i = 0; i < quote.Length; i++)
                {
                    if (i >= typedLength || quote[i] != registeredUserInput[i])
                    {
                        mistakes++;
                    }
                }

                //total tid
                double seconds = elapsedTime.TotalSeconds;
                //totalt per minut
                double charactersPerMinute = (typedLength / seconds) * 60;

                Console.Clear();
                Console.WriteLine($"\nDet här skulle du skriva: {quote}");
                Console.WriteLine($"Det här skrev du: {registeredUserInput}");
                Console.WriteLine($"Tid: {seconds:F2} sekunder");
                Console.WriteLine($"Hastighet: {charactersPerMinute:F2} tecken/minut");
                Console.WriteLine($"Antal felskrivningar: {mistakes}");

                break;
            }
         
        }

        public async Task PlayAgain(string playMenu)
        {
            Console.WriteLine("Vill du spela igen? j/n");
            string? answer = Console.ReadLine();

            if (answer?.ToLower() == "j")
            {
                await GetSentences(playMenu);
            }
            else if (answer?.ToLower() == "n")
            {
                Console.Clear();
                Console.WriteLine(playMenu);
            }
            else
            {
                Console.WriteLine("Vänligen ange 'j' eller 'n'.");
                await PlayAgain(playMenu);
            }
        }
           
    }
}
