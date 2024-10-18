namespace Typeracer
{
    class Program
    {
        static async Task Main(string[] args)
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
                        Console.Clear();
                        PlayMenu otherMenu = new PlayMenu();
                        Console.WriteLine(otherMenu.GetPlayMenu());

                        string? userPlayInput = Console.ReadLine();

                        if (String.IsNullOrWhiteSpace(userPlayInput))
                        {
                            throw new ArgumentNullException("Felaktig inmatning, ange '1' eller '2'");
                        }

                        if (userPlayInput == "1")
                        {
                            Console.Clear();
                            PlayGame playgame = new PlayGame();
                            await playgame.GetSentences();
                        }
                        else if (userPlayInput == "2")
                        {
                            Console.WriteLine("Hello");
                        }
                        break;

                }
            }
            catch
            {

            }
                
        }
    }
}