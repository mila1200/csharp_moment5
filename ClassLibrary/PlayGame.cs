using System.Data;
using System.Diagnostics;

namespace Typeracer
{
    public class PlayGame
    {
        //interagera med databasen
        DatabaseConnection dbConnection = new DatabaseConnection();

        //lagrar inloggad anv�ndares id
        private int? loggedInUserId;

        public PlayGame(int? userId)
        {
            //kollar s� anv�ndarid finns.
            loggedInUserId = userId ?? throw new ArgumentNullException("Det finns ingen inloggad anv�ndare");
        }

        //asynkron metod som returnerar en "Task"
        public async Task GetSentences(string playMenu)
        {
            try
            {
                Console.WriteLine("Tryck Enter under spelets g�ng f�r att avbryta.\nF�rbered dig...");

                //h�mtar meningar fr�n API
                ApiSentences apiSentences = new ApiSentences();
                string url = "https://api.kanye.rest";

                //anropar metod asynkront f�r att h�mta mening fr�n API
                string quote = await apiSentences.sentencesFromApi(url);

                //f�r att f�rdr�ja visning av mening
                await Task.Delay(3000);

                //kontrollerar om meningen �r giltig, i s� fall anropas metod som p�b�rjar spelet.
                if (!String.IsNullOrWhiteSpace(quote))
                    {
                    await StartTypingTest(quote, playMenu);
                }
                else
                {
                    Console.WriteLine("Ett ov�ntat fel intr�ffade. Meningen kunde inte h�mtas.");
                }

            }
            //felhantering
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel intr�ffade: {ex.Message}");
            }
        }

        //asynkron metod som hanterar sj�lva spelet.
        public async Task StartTypingTest(string quote, string playMenu)
        {
                //rensa konsol och visa meningen
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
                //skriver ut meningen
                    Console.WriteLine(quote);
                //skriver ut vad anv�ndaren skriver
                    Console.WriteLine(registeredUserInput);
                //skriver ut antal misstag
                    Console.WriteLine($"\nAntal felskrivningar: {mistakes}");

                }
                    //stoppar tidtagning
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

                //skriver ut resultatet
                Console.Clear();
                Console.WriteLine("D I T T R E S U L T A T");
                Console.WriteLine($"\nDet h�r skulle du skriva: {quote}");
                Console.WriteLine($"Det h�r skrev du: {registeredUserInput}");
                Console.WriteLine($"\nTid: {seconds:F2} sekunder");
                Console.WriteLine($"Hastighet: {charactersPerMinute:F2} tecken/minut");
                Console.WriteLine($"Antal felskrivningar: {mistakes}");

            //Om anv�ndar id - h�mta inloggad anv�ndares data om spelet
            if (loggedInUserId.HasValue)
            {
                //h�mtar fr�n databasen
                User? currentUser = dbConnection.GetUserStatistics(loggedInUserId.Value);

                if (currentUser != null)
                {
                    //tilldelar v�rden men om det inte finns tilldelas standardv�rden.
                    double bestTime = currentUser.BestTime ?? double.MaxValue;
                    double bestSpeed = currentUser.BestSpeed ?? 0;
                    int bestMistakes = currentUser.BestMistakes ?? int.MaxValue;

                    //boolean f�r att kunna hantera eventuella updateringar
                    bool updated = false;

                    //f�ljande 4 if-satser hanterar och uppdaterar anv�ndarens prestation
                    if (seconds < bestTime)
                    {
                        currentUser.BestTime = seconds;
                        updated = true;
                        Console.WriteLine("\nGrattis, du f�rb�ttrade din tid!");
                    }

                    if (charactersPerMinute > bestSpeed)
                    {
                        currentUser.BestSpeed = charactersPerMinute;
                        updated = true;
                        Console.WriteLine("\nGrattis, du f�rb�ttrade din hastighet!");
                    }

                    if (mistakes < bestMistakes)
                    {
                        currentUser.BestMistakes = mistakes;
                        updated = true;
                        Console.WriteLine("\nGrattis, du f�rb�ttrade din precision!");
                    }

                    //uppdaterar anv�ndarens statistik om n�got har f�rb�ttrats.
                    if (updated)
                    {
                        dbConnection.UpdateUserStatistics(
                            currentUser.BestTime.GetValueOrDefault(double.MaxValue),
                            currentUser.BestSpeed.GetValueOrDefault(0),
                            currentUser.BestMistakes.GetValueOrDefault(int.MaxValue),
                            currentUser.Id);
                        Console.WriteLine("\nStatistik uppdaterad!");
                    }
                    //visar meddelande om ingen f�rb�ttring registreras.
                    else
                    {
                        Console.WriteLine("\nIngen f�rb�ttring registrerad, f�rs�k igen.");
                    }
                }
                else
                {
                    Console.WriteLine("Fel vid inh�mtning av anv�ndarstatistik.");
                }
            }
            else
            {
                Console.WriteLine("Det g�r inte att l�sa in Anv�ndar-ID.");
            }
            //�terg�r till spelmenyn n�r anv�ndaren trycker p� tangent
            Console.WriteLine("\n\nTryck p� en tangent f�r att �terg� till spelmenyn.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine(playMenu);
        }

        //ger anv�ndaren m�jlighet att spela igen efter avslutat spel om anv�ndaren inte har skrivit n�got.
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
