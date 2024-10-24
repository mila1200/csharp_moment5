using System.ComponentModel;

namespace Typeracer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Login login = new Login();
                    login.LoginMenu();

                    
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
                            {
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
                                        PlayGame playgame = new PlayGame();
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
                            }
                            break;
                        case "2":
                            {
                                Console.WriteLine("Försöker lägga till användare...");
                                Console.ReadKey();

                                DatabaseConnection db = new DatabaseConnection();
                                Console.WriteLine("Ange användarnamn: ");
                                string? inputUsername = Console.ReadLine();

                                Console.WriteLine("Ange lösenord: ");
                                string? inputPassword = Console.ReadLine();

                                if (!String.IsNullOrEmpty(inputUsername) && !String.IsNullOrEmpty(inputPassword))
                                {
                                    try
                                    {
                                        db.addUser(inputUsername, inputPassword);
                                        Console.WriteLine($"Användare {inputUsername} har lagts till.");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Fel vid tillägg av användare: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Användarnamn och lösenord får inte vara tomma.");

                                }

                                Console.WriteLine("Tryck på en tangent för att fortsätta...");
                                Console.ReadKey();

                            }
                            break;
                        case "3":
                            {
                                while (true)
                                {
                                    Console.WriteLine("hello");
                                }
                            }
                            break;
                            //beskriver programmet
                        case "4":
                            {
                                About about = new About();
                                Console.WriteLine(about);
                            }
                            break;
                            //avsluta programmet
                        case "5":
                            {
                                Environment.Exit(0);
                                break;
                            }
                        //default om fel inmatning
                        default:
                            {
                                Console.WriteLine("Ogiltigt val, vänligen ange '1', '2', '3', '4' eller '5'");
                                Console.WriteLine("Tryck på en tangent för att återgå till menyn");
                                Console.ReadKey();
                                break;
                            }
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