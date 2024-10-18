namespace Typeracer
{
  public class MainMenu
    {
        public string welcomeMessage;
        public string selectionOne;
        public string selectionTwo;
        public string selectionThree;
        public string selectionFour;
        public string selectionFive;
        
        public MainMenu()
        {
            welcomeMessage = "V�lkommen till Typeracer!\n";
            selectionOne = "1. Spela";
            selectionTwo = "2. Topplista";
            selectionThree = "3. Statistik";
            selectionFour = "4. Om Typeracer";
            selectionFive = "\n5. St�ng";
        }

        public string GetMenu()
        {
            return ($"{welcomeMessage}\n{selectionOne}\n{selectionTwo}\n{selectionThree}\n{selectionFour}\n{selectionFive}");
        }
    }
}