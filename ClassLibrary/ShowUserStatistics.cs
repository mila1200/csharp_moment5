namespace Typeracer
{
    public class ShowUserStatistics
    {
        //metod f�r att l�sa ut anv�ndarens statistik
        public void Display(User? user)
        {
            //om anv�ndaren inte �r null skrivs statistiken ut.
            if (user == null)
            {
                Console.WriteLine("Ingen anv�ndare �r inloggad.");
                return;
            }

            Console.WriteLine($"Statistik f�r {user.UserName}: \n");
            Console.WriteLine($"B�sta tid: {user.BestTime:F2} sekunder");
            Console.WriteLine($"Hastighet: {user.BestSpeed:F2} tecken/minut");
            Console.WriteLine($"Minst antal misstag: {user.BestMistakes}");

        }
    }
}
