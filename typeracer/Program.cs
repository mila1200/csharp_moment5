using System.ComponentModel;
using System.Data.Common;

namespace Typeracer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int? loggedInUserId = null;
            DatabaseConnection dbConnection = new DatabaseConnection();
            bool isLoggedIn = false;

            while (true)
            {
                try
                {
                    Console.Clear();

                    if (!isLoggedIn)
                    {
                        Login login = new Login();
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

                    MainMenu menu = new MainMenu();
                    Console.WriteLine(menu.GetMenu());
                    string? userInput = Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(userInput))
                    {
                        throw new ArgumentNullException("Felaktig inmatning, ange '1', '2', '3', '4' eller '5'.");
                    }

                    switch (userInput)
                    {
                        case "1":
                            
                                while (true)
                                {
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

                                    if (userPlayInput == "1")
                                    {
                                        Console.Clear();
                                        PlayGame playgame = new PlayGame(loggedInUserId);
                                        await playgame.GetSentences(otherMenu.GetPlayMenu());
                                    }
                                    else if (userPlayInput == "2")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Ogiltigt alternativ, ange '1' eller '2'.");
                                        Console.WriteLine("Tryck på en tangent för att fortsätta...");
                                        Console.ReadKey();
                                        continue;
                                    }
                                }
                            
                            break;
                        case "2":
                            Console.Clear();
                            Console.WriteLine("S T A T I S T I K\n");

                            ShowUserStatistics stats = new ShowUserStatistics();

                            if (loggedInUserId.HasValue)
                            {
                                User? currentUser = dbConnection.GetUserStatistics(loggedInUserId.Value);
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
                            
                                Console.WriteLine("Ogiltigt val, vänligen ange '1', '2', '3', '4' eller '5'");
                                Console.WriteLine("Tryck på en tangent för att återgå till menyn");
                                Console.ReadKey();
                                break;
                            
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                }
            }
        }
    }
}