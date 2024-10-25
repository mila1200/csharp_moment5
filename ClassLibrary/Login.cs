using BCrypt.Net;

namespace Typeracer
{
    public class Login
    {
        private DatabaseConnection dbConnection = new DatabaseConnection();
       
        //Logga in
        public int? LoginMenu()
        {
            bool exitMenu = false;

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

                    if (String.IsNullOrEmpty(loginInput))
                    {
                        Console.WriteLine("Felaktig inmatning, ange '1', '2' eller '3'.");
                        Console.WriteLine("Tryck p� en tangent f�r att �terg� till menyn");
                        Console.ReadKey();
                        continue;
                    }

                    switch (loginInput)
                    {
                        case "1":
                            Console.Clear();
                            Console.WriteLine("L O G G A  I N\n");
                            int? userId = LoginUser();
                           if(userId != null)
                            {
                                exitMenu = true;
                                return userId.Value;
                            }
                            break;

                        case "2":
                            Console.Clear();
                            Console.WriteLine("S K A P A  N Y  A N V � N D A R E\n");
                            CreateNewUser();
                            break;

                        case "3":
                            Environment.Exit(0);
                            break;

                        default:

                            Console.WriteLine("Ogiltigt val, v�nligen ange '1', '2' eller '3'.");
                            Console.WriteLine("Tryck p� en tangent f�r att �terg� till menyn");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel intr�ffade: {ex.Message}");
                }
            }
            return null;
        }
        private void CreateNewUser()
        {
            Console.WriteLine("Nytt anv�ndarnamn: ");
            string? newUsername = Console.ReadLine();

            Console.WriteLine("\nL�senord: ");
            string? newPassword = Console.ReadLine();

            if (newUsername == null | newPassword == null)
            {
                Console.WriteLine("Du m�ste ange ett anv�ndarnamn och l�senord");
                return;
            }

            if (newPassword.Length < 5)
            {
                Console.WriteLine("L�senordet m�ste vara l�ngre �n fem tecken...");
                return;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            dbConnection.addUser(newUsername, passwordHash);

            Console.WriteLine("Anv�ndare skapad.");

            Console.WriteLine("Tryck p� en tangent f�r att forts�tta...");
            Console.ReadKey();
            
        }

        public bool VerifyUser(string? username, string? password)
        {
             string? passwordFromDb = dbConnection.GetPasswordHash(username);

            return BCrypt.Net.BCrypt.Verify(password, passwordFromDb);
        }

        private int? LoginUser()
        {
            Console.WriteLine("Anv�ndarnamn: ");
            string? loginUsername = Console.ReadLine();

            Console.WriteLine("\nL�senord: ");
            string? loginPassword = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(loginUsername) || String.IsNullOrWhiteSpace(loginPassword))
            {
                Console.WriteLine("Du m�ste ange anv�ndarnamn och l�senord.");
                Console.WriteLine("Tryck p� valfri tangent f�r att �terg� till menyn");
                Console.ReadKey();
                return null; 
            }

            if (VerifyUser(loginUsername, loginPassword))
            {
                Console.WriteLine("Inloggning lyckades!");
                int userId = dbConnection.GetUserId(loginUsername);
                Console.WriteLine("Tryck p� valfri tangent f�r att forts�tta...");
                Console.ReadKey();
                Console.Clear();
                return userId;
            }
            else
            {
                Console.WriteLine("Fel anv�ndarnamn eller l�senord...");
                Console.WriteLine("Tryck p� valfri tangent f�r att �terg� till menyn");
                Console.ReadKey();
                
            }
            
            return null;
        }

        public void ShowUserStatistics(int userId)
        {
            User? userStatistics = dbConnection.GetUserStatistics(userId);
            ShowUserStatistics showStats = new ShowUserStatistics();
            showStats.Display(userStatistics);
        }
    }
}