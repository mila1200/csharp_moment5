namespace Typeracer
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public double? BestTime { get; set; }
        public double? BestSpeed { get; set; }
        public int? BestMistakes { get; set; }
    }
}