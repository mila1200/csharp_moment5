using System.Diagnostics;

namespace Typeracer
{
    public class PlayGame
    {
        public async Task GetSentences(string playMenu)
        {
            try
            {
                Console.WriteLine("Tryck Enter under spelet f�r att avbryta.\nF�rbered dig...");

                ApiSentences apiSentences = new ApiSentences();
                string url = "https://api.kanye.rest";

                string quote = await apiSentences.sentencesFromApi(url);

                //f�r att f�rdr�ja visning av mening
                await Task.Delay(3000);

                if (!String.IsNullOrWhiteSpace(quote))
                    {
                    await StartTypingTest(quote, playMenu);
                }
                else
                {
                    Console.WriteLine("Ett ov�ntat fel intr�ffade. Meningen kunde inte h�mtas.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel intr�ffade: {ex.Message}");
            }
        }

        public async Task StartTypingTest(string quote, string playMenu)
        {
            
            
                Console.Clear();
                Console.WriteLine(quote);

                //ber�kna tid
                Stopwatch stopWatchWrite = new Stopwatch();
                stopWatchWrite.Start();

                //inmatningen
                string? registeredUserInput = string.Empty;

                //lagrar misstagen
                int mistakes = 0;

                //position i meningen
                int currentPosition = 0;

                //p�g�r s� l�nge inte hela meningen har skrivits
                while (currentPosition < quote.Length)
                {
                    //kollar tangentryckningar
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    
                    //avbryter om enter
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                    //l�gger till �terst�ende tecken till misstag
                    mistakes += (quote.Length - currentPosition);
                        break;
                    }

                    //j�mf�r tangenttryckning med f�rv�ntat
                    if (keyInfo.KeyChar != quote[currentPosition])
                    {
                        //plussar misstag
                        mistakes++;
                    }
                    else
                    {
                        //l�gger till korrektinmatning och flyttar fram positionen
                        registeredUserInput += keyInfo.KeyChar;
                        currentPosition++;
                    }

                    Console.Clear();
                    Console.WriteLine(quote);
                    Console.WriteLine(registeredUserInput);
                    Console.WriteLine($"\nAntal felskrivningar: {mistakes}");

                }

                    stopWatchWrite.Stop();

                TimeSpan elapsedTime = stopWatchWrite.Elapsed;

                //felhantering om anv�ndaren inte anger n�got
                if (String.IsNullOrWhiteSpace(registeredUserInput))
                {
                    Console.WriteLine("Ogiltigt resultat. Du m�ste skriva n�got.");
                    Console.WriteLine("Tryck p� valfri tangent...");
                    Console.ReadKey();
                    await PlayAgain(playMenu);
                    return;
                }

                //total tid
                double seconds = elapsedTime.TotalSeconds;
                //totalt per minut
                double charactersPerMinute = (registeredUserInput.Length / seconds) * 60;

                Console.Clear();
                Console.WriteLine("D I T T R E S U L T A T");
                Console.WriteLine($"\nDet h�r skulle du skriva: {quote}");
                Console.WriteLine($"Det h�r skrev du: {registeredUserInput}");
                Console.WriteLine($"Tid: {seconds:F2} sekunder");
                Console.WriteLine($"Hastighet: {charactersPerMinute:F2} tecken/minut");
                Console.WriteLine($"Antal felskrivningar: {mistakes}");

            Console.WriteLine("\n\nTryck p� en tangent f�r att �terg� till spelmenyn.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine(playMenu);
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
                Console.WriteLine("V�nligen ange 'j' eller 'n'.");
                await PlayAgain(playMenu);
            }
        }
           
    }
}
