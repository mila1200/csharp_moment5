using System.Data;
using System.Diagnostics;

namespace Typeracer
{
    public class PlayGame
    {
        //interagera med databasen
        DatabaseConnection dbConnection = new DatabaseConnection();

        //lagrar inloggad användares id
        private int? loggedInUserId;

        public PlayGame(int? userId)
        {
            //kollar så användarid finns.
            loggedInUserId = userId ?? throw new ArgumentNullException("Det finns ingen inloggad användare");
        }

        //asynkron metod som returnerar en "Task"
        public async Task GetSentences(string playMenu)
        {
            try
            {
                Console.WriteLine("Tryck Enter under spelets gång för att avbryta.\nFörbered dig...");

                //hämtar meningar från API
                ApiSentences apiSentences = new ApiSentences();
                string url = "https://api.kanye.rest";

                //anropar metod asynkront för att hämta mening från API
                string quote = await apiSentences.sentencesFromApi(url);

                //för att fördröja visning av mening
                await Task.Delay(3000);

                //kontrollerar om meningen är giltig, i så fall anropas metod som påbörjar spelet.
                if (!String.IsNullOrWhiteSpace(quote))
                    {
                    await StartTypingTest(quote, playMenu);
                }
                else
                {
                    Console.WriteLine("Ett oväntat fel inträffade. Meningen kunde inte hämtas.");
                }

            }
            //felhantering
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");
            }
        }

        //asynkron metod som hanterar själva spelet.
        public async Task StartTypingTest(string quote, string playMenu)
        {
                //rensa konsol och visa meningen
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
                //skriver ut meningen
                    Console.WriteLine(quote);
                //skriver ut vad användaren skriver
                    Console.WriteLine(registeredUserInput);
                //skriver ut antal misstag
                    Console.WriteLine($"\nAntal felskrivningar: {mistakes}");

                }
                    //stoppar tidtagning
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

                //skriver ut resultatet
                Console.Clear();
                Console.WriteLine("D I T T R E S U L T A T");
                Console.WriteLine($"\nDet här skulle du skriva: {quote}");
                Console.WriteLine($"Det här skrev du: {registeredUserInput}");
                Console.WriteLine($"\nTid: {seconds:F2} sekunder");
                Console.WriteLine($"Hastighet: {charactersPerMinute:F2} tecken/minut");
                Console.WriteLine($"Antal felskrivningar: {mistakes}");

            //Om användar id - hämta inloggad användares data om spelet
            if (loggedInUserId.HasValue)
            {
                //hämtar från databasen
                User? currentUser = dbConnection.GetUserStatistics(loggedInUserId.Value);

                if (currentUser != null)
                {
                    //tilldelar värden men om det inte finns tilldelas standardvärden.
                    double bestTime = currentUser.BestTime ?? double.MaxValue;
                    double bestSpeed = currentUser.BestSpeed ?? 0;
                    int bestMistakes = currentUser.BestMistakes ?? int.MaxValue;

                    //boolean för att kunna hantera eventuella updateringar
                    bool updated = false;

                    //följande 4 if-satser hanterar och uppdaterar användarens prestation
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

                    //uppdaterar användarens statistik om något har förbättrats.
                    if (updated)
                    {
                        dbConnection.UpdateUserStatistics(
                            currentUser.BestTime.GetValueOrDefault(double.MaxValue),
                            currentUser.BestSpeed.GetValueOrDefault(0),
                            currentUser.BestMistakes.GetValueOrDefault(int.MaxValue),
                            currentUser.Id);
                        Console.WriteLine("\nStatistik uppdaterad!");
                    }
                    //visar meddelande om ingen förbättring registreras.
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
            //återgår till spelmenyn när användaren trycker på tangent
            Console.WriteLine("\n\nTryck på en tangent för att återgå till spelmenyn.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine(playMenu);
        }

        //ger användaren möjlighet att spela igen efter avslutat spel om användaren inte har skrivit något.
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
