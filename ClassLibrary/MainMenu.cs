namespace Typeracer
{
  public class MainMenu
    {
        public string welcomeMessage;
        public string selectionOne;
        public string selectionTwo;
        public string selectionThree;
        public string selectionFour;
        
        public MainMenu()
        {
            welcomeMessage = "Välkommen till T Y P E R A C E R!\n";
            selectionOne = "1. Spela";
            selectionTwo = "2. Statistik";
            selectionThree = "3. Om Typeracer";
            selectionFour = "\n4. Stäng";
        }

        public string GetMenu()
        {
            return $"{welcomeMessage}\n{selectionOne}\n{selectionTwo}\n{selectionThree}\n{selectionFour}";
        }
    }
}