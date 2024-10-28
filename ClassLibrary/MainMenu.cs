namespace Typeracer
{
    //klass f�r att hantera hvudmenyn
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
            welcomeMessage = "V�lkommen till T Y P E R A C E R!\n";
            selectionOne = "1. Spela";
            selectionTwo = "2. Statistik";
            selectionThree = "3. Om Typeracer";
            selectionFour = "\n4. St�ng";
        }

        //returnerar en str�ng f�r att l�sa ut huvudmenyn
        public string GetMenu()
        {
            return $"{welcomeMessage}\n{selectionOne}\n{selectionTwo}\n{selectionThree}\n{selectionFour}";
        }
    }
}