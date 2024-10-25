namespace Typeracer
{
    public class ShowUserStatistics
    {
        public void Display(User? user)
        {
            if (user == null)
            {
                Console.WriteLine("Ingen användare är inloggad.");
                return;
            }
            else
            {
                Console.WriteLine($"Statistik för {user.UserName}: \n");
                Console.WriteLine($"Bästa tid: {user.BestTime:F2} sekunder");
                Console.WriteLine($"Hastighet: {user.BestSpeed:F2} tecken/minut");
                Console.WriteLine($"Minst antal misstag: {user.BestMistakes}");
            }
        }
    }
}
