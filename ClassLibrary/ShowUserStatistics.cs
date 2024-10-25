namespace Typeracer
{
    public class ShowUserStatistics
    {
        public void Display(User? user)
        {
            if (user == null)
            {
                Console.WriteLine("Ingen anv�ndare �r inloggad.");
                return;
            }
            else
            {
                Console.WriteLine($"Statistik f�r {user.UserName}: \n");
                Console.WriteLine($"B�sta tid: {user.BestTime:F2} sekunder");
                Console.WriteLine($"Hastighet: {user.BestSpeed:F2} tecken/minut");
                Console.WriteLine($"Minst antal misstag: {user.BestMistakes}");
            }
        }
    }
}
