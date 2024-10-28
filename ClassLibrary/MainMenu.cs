namespace Typeracer
{
    //klass för att hantera hvudmenyn
  public class MainMenu
    {
        //lagrar meddelande och menyval
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

        //returnerar en sträng för att läsa ut huvudmenyn
        public string GetMenu()
        {
            return $"{welcomeMessage}\n{selectionOne}\n{selectionTwo}\n{selectionThree}\n{selectionFour}";
        }
    }
}