using System.Collections.Generic;
using System.IO;
using System.Text;

//f�r att kunna anv�nda Sqlite-databas.
using Microsoft.Data.Sqlite;

namespace Typeracer

{   //klass f�r att hantera databasanslutningar.
	public class DatabaseConnection
	{
				//privat lista med anv�ndare f�r att lagra lokalt.
				private List<User> users = new List<User>();

				//s�kv�gen till SQLite-databasen.
				string connectionString = "Data Source=C:/Users/sLarsson/Desktop/moment5/my_db.db";

		//konstruktor som skapar en tabell (users) med hj�lp av SQL-fr�ga.
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
		//tar in anv�ndarnamn och l�senord och skapar en anv�ndare, l�gger in i databasen baserat p� angivna v�rden.
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
						//standardv�rde (MaxValue och 0) f�r att markera att inget v�rde satts �n + f�r att nya rekord ska kunna registreras vid f�rsta spelet.
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
					Console.WriteLine($"Kunde inte l�gga till anv�ndaren.");
				}
				return person;
			}
		}

		//h�mtar det hashade l�senordet fr�n databasen f�r en specifik anv�ndare.
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

						//h�mtar ett enda v�rd (det hashade l�senordet).
						object? result = command.ExecuteScalar();

						//har koll p� eventuella null-v�rden.
						if (result != null)
						{
							return result.ToString();
						}
						else
						{
							throw new Exception("Fel vid anv�ndarverifiering");
						}

					}
				}
				//felhantering
				catch (Exception ex)
				{
					Console.WriteLine($"Anv�ndaren kunde inte verifieras: {ex.Message}");
					return null;
				}

			}
		}

		//tar emot anv�ndarens Id (baserat p� Id:n som finns i databasen) och returnerar en anv�ndare (eller null).
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
							//returnerar anv�ndare med v�rden fr�n databasen
							return new User
							{
								//h�mta det f�rsta v�rdet som heltal
								Id = reader.GetInt32(0),
								//h�mta andra v�rdet som en str�ng
								UserName = reader.GetString(1),
								//kollar om v�rdet �r Null eller double.MaxValue, I s� fall s�tts v�rdet till null, annars h�mtas v�rdet som double. 
								BestTime = reader.IsDBNull(2) || reader.GetDouble(2) == double.MaxValue ? null : reader.GetDouble(2),
								//h�mtar v�rdet som double.
								BestSpeed = reader.GetDouble(3),
								//kollar om v�rdet �r Null eller int.MaxValue. I s� fall s�tts v�rdet till null annars h�mtas det som ett heltal.
								BestMistakes = reader.IsDBNull(4) || reader.GetInt32(4) == int.MaxValue ? null : reader.GetInt32(4)
							};
						}
						//felhantering
						else
						{
							throw new Exception("Ingen statistik hittades f�r anv�ndaren");
						}
					}
				}
			}

		}

		//h�mtar anv�ndarens id, konverterat till heltal, baserat p� anv�ndarnamn.
        public int GetUserId(string username)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id FROM users WHERE username = @username";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
					
					//h�mtar ett enda v�rde(id)
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Anv�ndare hittades inte");
                    }
                }
            }
        }

		//tar in v�rden och uppdaterar dessa i databasen f�r en anv�ndare baserat p� anv�ndar-id.
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
				Console.WriteLine($"Fel vid uppdatering av anv�ndarstatistik: {ex.Message}");
			}
		}	
    }
}

