using System.ComponentModel;
using System.Data.Common;

namespace Typeracer
{
    class Program
    {
        //asynkron för att tillåta användningar av await
        static async Task Main(string[] args)
        {
            //håller id för användare, null då tom sträng inte fungerar för int.
            int? loggedInUserId = null;
            //instans av databasanslutning
            DatabaseConnection dbConnection = new DatabaseConnection();
            //för att hålla reda på om användaren är inloggad
            bool isLoggedIn = false;

            //loopar programmet
            while (true)
            {
                try
                {
                    Console.Clear();
                    /*om användaren inte är inloggad visas inloggningsmenyn. Om inloggning inte lyckas fortsätter loopen och inloggnings-menyn visas.
                     Annars skickas användaren till huvudmenyn*/
                    if (!isLoggedIn)
                    {
                        Login login = new Login();
                        //returnerar användarens id
                        loggedInUserId = login.LoginMenu();

                        if (loggedInUserId == null)
                        {
                            Console.WriteLine("Inloggningen misslyckades");
                            Console.WriteLine("Tryck på en tangent för att fortsätta...");
                            Console.ReadKey();
                            continue;
                        }
                        else
                        {
                            isLoggedIn = true;
                        }
                    }

                    //visar huvudmenyn om användaren är inloggad.
                    MainMenu menu = new MainMenu();
                    Console.WriteLine(menu.GetMenu());
                    string? userInput = Console.ReadLine();

                    //switch som läser av vad användaren väljer.
                    switch (userInput)
                    {
                        case "1":
                                
                                //loopar spelmeny
                                while (true)
                                {
                                    //instans av spelmeny
                                    Console.Clear();
                                    PlayMenu otherMenu = new PlayMenu();
                                    Console.WriteLine(otherMenu.GetPlayMenu());

                                    string? userPlayInput = Console.ReadLine();

                                    if (String.IsNullOrWhiteSpace(userPlayInput))
                                    {
                                        Console.WriteLine("Felaktig inmatning, ange '1' eller '2'");
                                        Console.WriteLine("Tryck på en tangent för att fortsätta...");
                                        Console.ReadKey();
                                        continue;
                                    }

                                    //om användaren väljer 1 drar spelet igång
                                    if (userPlayInput == "1")
                                    {
                                        Console.Clear();
                                        PlayGame playgame = new PlayGame(loggedInUserId);
                                        await playgame.GetSentences(otherMenu.GetPlayMenu());
                                    }
                                    
                                    //bryter och återgår till huvudmenyn
                                    else if (userPlayInput == "2")
                                    {
                                        break;
                                    }
                                    //felhantering om 1 eller 2 inte anges
                                    else
                                    {
                                        Console.WriteLine("Ogiltigt alternativ, ange '1' eller '2'.");
                                        Console.WriteLine("Tryck på en tangent för att fortsätta...");
                                        Console.ReadKey();
                                        continue;
                                    }
                                }
                            
                            break;
                            
                            //alternativ 2 visar användarstatistik
                        case "2":
                            Console.Clear();
                            Console.WriteLine("S T A T I S T I K\n");

                            //instans för att kunna hantera användarstatistik
                            ShowUserStatistics stats = new ShowUserStatistics();

                            //om användaren har Id anropas metod GetUserStatistics
                            if (loggedInUserId.HasValue)
                            {
                                User? currentUser = dbConnection.GetUserStatistics(loggedInUserId.Value);
                                //anropar Display för att presentera statistiken på strukturerat sätt.
                                stats.Display(currentUser);
                            }
                            else
                            {
                                Console.WriteLine("Ingen användare inloggad");
                            }

                            Console.WriteLine("Tryck på en tangent för att fortsätta...");
                            Console.ReadKey();
                            break;

                            //beskriver programmet
                        case "3":
                            
                                About about = new About();
                                Console.WriteLine(about);
                            
                            break;
                           
                            //avslutar programmet;
                        case "4":
                            
                                Environment.Exit(0);
                                break;
                            
                        //default om fel inmatning
                        default:
                                Console.WriteLine("Ogiltigt val, vänligen ange '1', '2', '3' eller '4'");
                                Console.WriteLine("Tryck på en tangent för att återgå till menyn");
                                Console.ReadKey();
                                break;
                            
                    }
                }
                //felhantering
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                }
            }
        }
    }
}