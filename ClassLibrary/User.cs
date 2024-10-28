namespace Typeracer
{
    //klass som definierar en modell f�r en anv�ndare. Anv�nder standardv�rden f�r att identifiera om anv�ndaren inledningsvis har n�gra resultat.
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public double? BestTime { get; set; } = double.MaxValue;
        public double? BestSpeed { get; set; }
        public int? BestMistakes { get; set; } = int.MaxValue;
    }
}
