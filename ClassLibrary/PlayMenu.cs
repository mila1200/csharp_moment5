namespace Typeracer
{
	//klass för att hantera spelmenyn
	public class PlayMenu
	{
		//lagrar meddelande och menyval
		public string header;
		public string instructions;
		public string message;
		public string playSelectionOne;
		public string playSelectionTwo;

		public PlayMenu()
		{
			header = "S P E L A\n";
			instructions = "Försök att skriva så snabbt och noggrant du kan.";
			message = "Lycka till!\n";
			playSelectionOne = "1. Börja";
			playSelectionTwo = "2. Avsluta";
		}

		//returnerar en sträng för att läsa ut menyn
		public string GetPlayMenu()
		{
			return ($"{header}\n{instructions}\n{message}\n{playSelectionOne}\n{playSelectionTwo}");
		}
	}
}