using System.Data;
using System.Diagnostics;

namespace Typeracer
{
    public class PlayGame
    {
        DatabaseConnection dbConnection = new DatabaseConnection();

        private int? loggedInUserId;

        public PlayGame(int? userId)
        {

            loggedInUserId = userId ?? throw new ArgumentNullException("Det finns ingen inloggad användare");
        }

        public async Task GetSentences(string playMenu)
        {
            try
            {
                Console.WriteLine("Tryck Enter under spelet för att avbryta.\nFörbered dig...");

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
                Console.Clear();
                Console.WriteLine(quote);

                //beräkna tid
                Stopwatch stopWatchWrite = new Stopwatch();
                stopWatchWrite.Start();

                //inmatningen
                string? registeredUserInput = string.Empty;

                //lagrar misstagen
                int mistakes = 0;

                //position i meningen
                int currentPosition = 0;

                //pågår så länge inte hela meningen har skrivits
                while (currentPosition < quote.Length)
                {
                    //kollar tangentryckningar
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    
                    //avbryter om enter
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                    //lägger till återstående tecken till misstag
                    mistakes += (quote.Length - currentPosition);
                        break;
                    }

                    //jämför tangenttryckning med förväntat
                    if (keyInfo.KeyChar != quote[currentPosition])
                    {
                        //plussar misstag
                        mistakes++;
                    }
                    else
                    {
                        //lägger till korrektinmatning och flyttar fram positionen
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

                //felhantering om användaren inte anger något
                if (String.IsNullOrWhiteSpace(registeredUserInput))
                {
                    Console.WriteLine("Ogiltigt resultat. Du måste skriva något.");
                    Console.WriteLine("Tryck på valfri tangent...");
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
                Console.WriteLine($"\nDet här skulle du skriva: {quote}");
                Console.WriteLine($"Det här skrev du: {registeredUserInput}");
                Console.WriteLine($"Tid: {seconds:F2} sekunder");
                Console.WriteLine($"Hastighet: {charactersPerMinute:F2} tecken/minut");
                Console.WriteLine($"Antal felskrivningar: {mistakes}");

            if (loggedInUserId.HasValue)
            {
                User? currentUser = dbConnection.GetUserStatistics(loggedInUserId.Value);

                if (currentUser != null)
                {
                    double bestTime = currentUser.BestTime ?? double.MaxValue;
                    double bestSpeed = currentUser.BestSpeed ?? 0;
                    int bestMistakes = currentUser.BestMistakes ?? int.MaxValue;

                    bool updated = false;

                    if (seconds < bestTime)
                    {
                        currentUser.BestTime = seconds;
                        updated = true;
                        Console.WriteLine("\nGrattis, du förbättrade din tid!");
                    }

                    if (charactersPerMinute > bestSpeed)
                    {
                        currentUser.BestSpeed = charactersPerMinute;
                        updated = true;
                        Console.WriteLine("\nGrattis, du förbättrade din hastighet!");
                    }

                    if (mistakes < bestMistakes)
                    {
                        currentUser.BestMistakes = mistakes;
                        updated = true;
                        Console.WriteLine("\nGrattis, du förbättrade din precision!");
                    }

                    if (updated)
                    {
                        dbConnection.UpdateUserStatistics(
                            currentUser.BestTime.GetValueOrDefault(double.MaxValue),
                            currentUser.BestSpeed.GetValueOrDefault(0),
                            currentUser.BestMistakes.GetValueOrDefault(int.MaxValue),
                            currentUser.Id);
                        Console.WriteLine("Statistik uppdaterad!");
                    }
                    else
                    {
                        Console.WriteLine("\nIngen förbättring registrerad, försök igen.");
                    }
                }
                else
                {
                    Console.WriteLine("Fel vid inhämtning av användarstatistik.");
                }
            }
            else
            {
                Console.WriteLine("Det går inte att läsa in Användar-ID.");
            }

            Console.WriteLine("\n\nTryck på en tangent för att återgå till spelmenyn.");
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
                Console.WriteLine("Vänligen ange 'j' eller 'n'.");
                await PlayAgain(playMenu);
            }
        }
           
    }
}
