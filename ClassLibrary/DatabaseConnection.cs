using System.Collections.Generic;
using System.IO;
using System.Text;

//för att kunna använda Sqlite-databas.
using Microsoft.Data.Sqlite;

namespace Typeracer

{   //klass för att hantera databasanslutningar.
	public class DatabaseConnection
	{
				//privat lista med användare för att lagra lokalt.
				private List<User> users = new List<User>();

				//sökvägen till SQLite-databasen.
				string connectionString = "Data Source=C:/Users/sLarsson/Desktop/moment5/my_db.db";

		//konstruktor som skapar en tabell (users) med hjälp av SQL-fråga.
		public DatabaseConnection()
		{
			try
			{

				using (var connection = new SqliteConnection(connectionString))
				{
					connection.Open();

					string createTableQuery = @"CREATE TABLE IF NOT EXISTS users (
											id	INTEGER PRIMARY KEY AUTOINCREMENT,
											username VARCHAR(40) NOT NULL UNIQUE,
											password VARCHAR(1000) NOT NULL,
											besttime DOUBLE,
											bestspeed DOUBLE,
											bestmistakes INT,
											created DATETIME DEFAULT CURRENT_TIMESTAMP
											);";

					using var createCommand = new SqliteCommand(createTableQuery, connection);
					{
						createCommand.ExecuteNonQuery();
					}

				}
			}
			//felhantering
			catch (Exception ex)
			{
				Console.WriteLine($"Ett fel uppstod: {ex.Message} ");
			}
		}
		//tar in användarnamn och lösenord och skapar en användare, lägger in i databasen baserat på angivna värden.
		public User addUser(string username, string password)
		{
			User person = new User();
			person.UserName = username;
			users.Add(person);

			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();

				try
				{
					string insertQuery = "INSERT INTO users (username, password, besttime, bestspeed, bestmistakes, created) VALUES (@username, @password, @besttime, @bestspeed, @bestmistakes, @created);";
					using (var command = new SqliteCommand(insertQuery, connection))
					{
						command.Parameters.AddWithValue("@username", username);
						command.Parameters.AddWithValue("@password", password);
						//standardvärde (MaxValue och 0) för att markera att inget värde satts än + för att nya rekord ska kunna registreras vid första spelet.
						command.Parameters.AddWithValue("@besttime", double.MaxValue);
						command.Parameters.AddWithValue("@bestspeed", 0);
						command.Parameters.AddWithValue("@bestmistakes", int.MaxValue);
						command.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-ddThh:mm.ss"));

						command.ExecuteNonQuery();
					}
				}

				//felhantering
				catch
				{
					Console.WriteLine($"Kunde inte lägga till användaren.");
				}
				return person;
			}
		}

		//hämtar det hashade lösenordet från databasen för en specifik användare.
		public string? GetPasswordHash(string? username)
		{
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();

				try
				{
					string selectQuery = "SELECT password FROM users WHERE username = @username;";
					using (var command = new SqliteCommand(selectQuery, connection))
					{
						command.Parameters.AddWithValue("@username", username);

						//hämtar ett enda värd (det hashade lösenordet).
						object? result = command.ExecuteScalar();

						//har koll på eventuella null-värden.
						if (result != null)
						{
							return result.ToString();
						}
						else
						{
							throw new Exception("Fel vid användarverifiering");
						}

					}
				}
				//felhantering
				catch (Exception ex)
				{
					Console.WriteLine($"Användaren kunde inte verifieras: {ex.Message}");
					return null;
				}

			}
		}

		//tar emot användarens Id (baserat på Id:n som finns i databasen) och returnerar en användare (eller null).
		public User? GetUserStatistics(int userId)
		{
			using var connection = new SqliteConnection(connectionString);
			{
				connection.Open();
				string selectQuery = "SELECT id, username, besttime, bestspeed, bestmistakes FROM users WHERE Id = @userId";
				
				using (var command = new SqliteCommand(selectQuery, connection))
				{
					command.Parameters.AddWithValue("@userId", userId);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//returnerar användare med värden från databasen
							return new User
							{
								//hämta det första värdet som heltal
								Id = reader.GetInt32(0),
								//hämta andra värdet som en sträng
								UserName = reader.GetString(1),
								//kollar om värdet är Null eller double.MaxValue, I så fall sätts värdet till null, annars hämtas värdet som double. 
								BestTime = reader.IsDBNull(2) || reader.GetDouble(2) == double.MaxValue ? null : reader.GetDouble(2),
								//hämtar värdet som double.
								BestSpeed = reader.GetDouble(3),
								//kollar om värdet är Null eller int.MaxValue. I så fall sätts värdet till null annars hämtas det som ett heltal.
								BestMistakes = reader.IsDBNull(4) || reader.GetInt32(4) == int.MaxValue ? null : reader.GetInt32(4)
							};
						}
						//felhantering
						else
						{
							throw new Exception("Ingen statistik hittades för användaren");
						}
					}
				}
			}

		}

		//hämtar användarens id, konverterat till heltal, baserat på användarnamn.
        public int GetUserId(string username)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id FROM users WHERE username = @username";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
					
					//hämtar ett enda värde(id)
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Användare hittades inte");
                    }
                }
            }
        }

		//tar in värden och uppdaterar dessa i databasen för en användare baserat på användar-id.
		public void UpdateUserStatistics (double bestTime, double bestSpeed, int bestMistakes, int userId)
		{
			try
			{
				using (var connection = new SqliteConnection(connectionString))
				{
					connection.Open();
					string query = "UPDATE users SET besttime = @BestTime, bestspeed = @BestSpeed, bestmistakes = @BestMistakes WHERE id = @UserId";

					using (var command = new SqliteCommand(query, connection))
					{
						command.Parameters.AddWithValue("@BestTime", bestTime);
						command.Parameters.AddWithValue("@BestSpeed", bestSpeed);
						command.Parameters.AddWithValue("@BestMistakes", bestMistakes);
						command.Parameters.AddWithValue("@UserId", userId);
						command.ExecuteNonQuery();

					}
				}
			}
			//felhantering
			catch (Exception ex)
			{
				Console.WriteLine($"Fel vid uppdatering av användarstatistik: {ex.Message}");
			}
		}	
    }
}

