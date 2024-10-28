namespace Typeracer
{
    //klass som definierar en modell för en användare. Använder standardvärden för att identifiera om användaren inledningsvis har några resultat.
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
