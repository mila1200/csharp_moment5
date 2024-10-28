//för att kunna hantera hashning och verifiering av lösenord
using BCrypt.Net;

namespace Typeracer
{
    //klass som hanterar inloggning och registrering av användare.
    public class Login
    {   
        //ansluter till databasen
        private DatabaseConnection dbConnection = new DatabaseConnection();
       
        //visar en meny för att kunna logga in (kan returnera null eftersom "?")
        public int? LoginMenu()
        {
            bool exitMenu = false;

            //while-loop för att menyn ska visas tills det att användaren loggar in eller avslutar
            while (!exitMenu)
            {

                try
                {
                    Console.Clear();
                    Console.WriteLine("T Y P E R A C E R");
                    Console.WriteLine("\n1. Logga in");
                    Console.WriteLine("2. Skapa ny användare\n");
                    Console.WriteLine("\n3. Avsluta");
                    string? loginInput = Console.ReadLine();

                    //felhantering om användaren inte anger något.
                    if (String.IsNullOrEmpty(loginInput))
                    {
                        Console.WriteLine("Felaktig inmatning, ange '1', '2' eller '3'.");
                        Console.WriteLine("Tryck på en tangent för att återgå till menyn");
                        Console.ReadKey();
                        continue;
                    }

                    //switch som agerar olika baserat på användarens val.
                    switch (loginInput)
                    {
                        case "1":
                            Console.Clear();
                            Console.WriteLine("L O G G A  I N\n");
                           
                            //anropar metod för att logga in användare
                            int? userId = LoginUser();
                            //om id inte är null avslutas loopen och id reterneras.
                           if(userId != null)
                            {
                                exitMenu = true;
                                return userId.Value;
                            }
                            break;

                        case "2":
                            Console.Clear();
                            Console.WriteLine("S K A P A  N Y  A N V Ä N D A R E\n");
                            //anropar metod för att skapa ny användare
                            CreateNewUser();
                            break;

                        case "3":
                            //avslutar
                            Environment.Exit(0);
                            break;

                        default:
                            //Om användaren skriver något annat än 1,2 eller 3. Pausar programmet för att användaren ska hinna se felmeddelandet.
                            Console.WriteLine("Ogiltigt val, vänligen ange '1', '2' eller '3'.");
                            Console.WriteLine("Tryck på en tangent för att återgå till menyn");
                            Console.ReadKey();
                            break;
                    }
                }
                //felhantering.
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                }
            }
            return null;
        }

        //kan endast anropas inom klassen
        private void CreateNewUser()
        {
            Console.WriteLine("Nytt användarnamn: ");
            string? newUsername = Console.ReadLine();

            Console.WriteLine("\nLösenord: ");
            string? newPassword = Console.ReadLine();

            //kontrollerar så inte värdet är null på inmatade värden
            if (newUsername == null || newPassword == null)
            {
                Console.WriteLine("Du måste ange ett användarnamn och lösenord");
                return;
            }

            //lösenordet måste vara längre än 5 tecken.
            if (newPassword!.Length < 5)
            {
                Console.WriteLine("Lösenordet måste vara längre än fem tecken...");
                return;
            }

            //hashar lösenordet med hjälp av BCrypt.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            //anropar addUser för att spara användarnamn och hashat lösenord
            dbConnection.addUser(newUsername!, passwordHash);

            Console.WriteLine("Användare skapad.");

            Console.WriteLine("Tryck på en tangent för att fortsätta...");
            Console.ReadKey();
            
        }

        //tar in användare och lösenord för att verifiera användare
        public bool VerifyUser(string? username, string? password)
        {
            //hämtar hashat lösenord med hjälp av användarnamn
             string? passwordFromDb = dbConnection.GetPasswordHash(username);

            if (passwordFromDb == null)
            {
                return false;
            }
            //jämför angivet lösenord med det hashade lösenordet med hjälp av BCrypt.
            return BCrypt.Net.BCrypt.Verify(password, passwordFromDb);
        }

        //privat metod för att logga in användare
        private int? LoginUser()
        {
            Console.WriteLine("Användarnamn: ");
            string? loginUsername = Console.ReadLine();

            Console.WriteLine("\nLösenord: ");
            string? loginPassword = Console.ReadLine();

            //kollar efter null-värden
            if (String.IsNullOrWhiteSpace(loginUsername) || String.IsNullOrWhiteSpace(loginPassword))
            {
                Console.WriteLine("Du måste ange användarnamn och lösenord.");
                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
                Console.ReadKey();
                return null; 
            }

            //anropar VerifyUser med användarnamn och lösenord för att verifiera dessa.
            if (VerifyUser(loginUsername, loginPassword))
            {
                Console.WriteLine("Inloggning lyckades!");
                int userId = dbConnection.GetUserId(loginUsername!);
                Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
                Console.Clear();
                return userId;
            }
            //felhantering
            else
            {
                Console.WriteLine("Fel användarnamn eller lösenord...");
                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn");
                Console.ReadKey();
                
            }
            //returnerar null om inloggning misslyckas.
            return null;
        }

        //metod för att hämta användarstatistik baserat på användarid.
        public void ShowUserStatistics(int userId)
        {
            User? userStatistics = dbConnection.GetUserStatistics(userId);
            ShowUserStatistics showStats = new ShowUserStatistics();
            showStats.Display(userStatistics);
        }
    }
}