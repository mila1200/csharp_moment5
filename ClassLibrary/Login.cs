//f�r att kunna hantera hashning och verifiering av l�senord
using BCrypt.Net;

namespace Typeracer
{
    //klass som hanterar inloggning och registrering av anv�ndare.
    public class Login
    {   
        //ansluter till databasen
        private DatabaseConnection dbConnection = new DatabaseConnection();
       
        //visar en meny f�r att kunna logga in (kan returnera null eftersom "?")
        public int? LoginMenu()
        {
            bool exitMenu = false;

            //while-loop f�r att menyn ska visas tills det att anv�ndaren loggar in eller avslutar
            while (!exitMenu)
            {

                try
                {
                    Console.Clear();
                    Console.WriteLine("T Y P E R A C E R");
                    Console.WriteLine("\n1. Logga in");
                    Console.WriteLine("2. Skapa ny anv�ndare\n");
                    Console.WriteLine("\n3. Avsluta");
                    string? loginInput = Console.ReadLine();

                    //felhantering om anv�ndaren inte anger n�got.
                    if (String.IsNullOrEmpty(loginInput))
                    {
                        Console.WriteLine("Felaktig inmatning, ange '1', '2' eller '3'.");
                        Console.WriteLine("Tryck p� en tangent f�r att �terg� till menyn");
                        Console.ReadKey();
                        continue;
                    }

                    //switch som agerar olika baserat p� anv�ndarens val.
                    switch (loginInput)
                    {
                        case "1":
                            Console.Clear();
                            Console.WriteLine("L O G G A  I N\n");
                           
                            //anropar metod f�r att logga in anv�ndare
                            int? userId = LoginUser();
                            //om id inte �r null avslutas loopen och id reterneras.
                           if(userId != null)
                            {
                                exitMenu = true;
                                return userId.Value;
                            }
                            break;

                        case "2":
                            Console.Clear();
                            Console.WriteLine("S K A P A  N Y  A N V � N D A R E\n");
                            //anropar metod f�r att skapa ny anv�ndare
                            CreateNewUser();
                            break;

                        case "3":
                            //avslutar
                            Environment.Exit(0);
                            break;

                        default:
                            //Om anv�ndaren skriver n�got annat �n 1,2 eller 3. Pausar programmet f�r att anv�ndaren ska hinna se felmeddelandet.
                            Console.WriteLine("Ogiltigt val, v�nligen ange '1', '2' eller '3'.");
                            Console.WriteLine("Tryck p� en tangent f�r att �terg� till menyn");
                            Console.ReadKey();
                            break;
                    }
                }
                //felhantering.
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel intr�ffade: {ex.Message}");
                }
            }
            return null;
        }

        //kan endast anropas inom klassen
        private void CreateNewUser()
        {
            Console.WriteLine("Nytt anv�ndarnamn: ");
            string? newUsername = Console.ReadLine();

            Console.WriteLine("\nL�senord: ");
            string? newPassword = Console.ReadLine();

            //kontrollerar s� inte v�rdet �r null p� inmatade v�rden
            if (newUsername == null || newPassword == null)
            {
                Console.WriteLine("Du m�ste ange ett anv�ndarnamn och l�senord");
                return;
            }

            //l�senordet m�ste vara l�ngre �n 5 tecken.
            if (newPassword!.Length < 5)
            {
                Console.WriteLine("L�senordet m�ste vara l�ngre �n fem tecken...");
                return;
            }

            //hashar l�senordet med hj�lp av BCrypt.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            //anropar addUser f�r att spara anv�ndarnamn och hashat l�senord
            dbConnection.addUser(newUsername!, passwordHash);

            Console.WriteLine("Anv�ndare skapad.");

            Console.WriteLine("Tryck p� en tangent f�r att forts�tta...");
            Console.ReadKey();
            
        }

        //tar in anv�ndare och l�senord f�r att verifiera anv�ndare
        public bool VerifyUser(string? username, string? password)
        {
            //h�mtar hashat l�senord med hj�lp av anv�ndarnamn
             string? passwordFromDb = dbConnection.GetPasswordHash(username);

            if (passwordFromDb == null)
            {
                return false;
            }
            //j�mf�r angivet l�senord med det hashade l�senordet med hj�lp av BCrypt.
            return BCrypt.Net.BCrypt.Verify(password, passwordFromDb);
        }

        //privat metod f�r att logga in anv�ndare
        private int? LoginUser()
        {
            Console.WriteLine("Anv�ndarnamn: ");
            string? loginUsername = Console.ReadLine();

            Console.WriteLine("\nL�senord: ");
            string? loginPassword = Console.ReadLine();

            //kollar efter null-v�rden
            if (String.IsNullOrWhiteSpace(loginUsername) || String.IsNullOrWhiteSpace(loginPassword))
            {
                Console.WriteLine("Du m�ste ange anv�ndarnamn och l�senord.");
                Console.WriteLine("Tryck p� valfri tangent f�r att �terg� till menyn");
                Console.ReadKey();
                return null; 
            }

            //anropar VerifyUser med anv�ndarnamn och l�senord f�r att verifiera dessa.
            if (VerifyUser(loginUsername, loginPassword))
            {
                Console.WriteLine("Inloggning lyckades!");
                int userId = dbConnection.GetUserId(loginUsername!);
                Console.WriteLine("Tryck p� valfri tangent f�r att forts�tta...");
                Console.ReadKey();
                Console.Clear();
                return userId;
            }
            //felhantering
            else
            {
                Console.WriteLine("Fel anv�ndarnamn eller l�senord...");
                Console.WriteLine("Tryck p� valfri tangent f�r att �terg� till menyn");
                Console.ReadKey();
                
            }
            //returnerar null om inloggning misslyckas.
            return null;
        }

        //metod f�r att h�mta anv�ndarstatistik baserat p� anv�ndarid.
        public void ShowUserStatistics(int userId)
        {
            User? userStatistics = dbConnection.GetUserStatistics(userId);
            ShowUserStatistics showStats = new ShowUserStatistics();
            showStats.Display(userStatistics);
        }
    }
}