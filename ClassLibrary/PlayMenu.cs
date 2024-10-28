namespace Typeracer
{
	public class PlayMenu
	{
		public string header;
		public string instructions;
		public string message;
		public string playSelectionOne;
		public string playSelectionTwo;

		public PlayMenu()
		{
			header = "S P E L A\n";
			instructions = "F�rs�k att skriva s� snabbt och noggrant du kan.";
			message = "Lycka till!\n";
			playSelectionOne = "1. B�rja";
			playSelectionTwo = "2. Avsluta";
		}

		public string GetPlayMenu()
		{
			return ($"{header}\n{instructions}\n{message}\n{playSelectionOne}\n{playSelectionTwo}");
		}
	}
}