namespace Typeracer
{
    public class ShowUserStatistics
    {
        //metod för att läsa ut användarens statistik
        public void Display(User? user)
        {
            //om användaren inte är null skrivs statistiken ut.
            if (user == null)
            {
                Console.WriteLine("Ingen användare är inloggad.");
                return;
            }

            Console.WriteLine($"Statistik för {user.UserName}: \n");
            Console.WriteLine($"Bästa tid: {user.BestTime:F2} sekunder");
            Console.WriteLine($"Hastighet: {user.BestSpeed:F2} tecken/minut");
            Console.WriteLine($"Minst antal misstag: {user.BestMistakes}");

        }
    }
}
