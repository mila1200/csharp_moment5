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