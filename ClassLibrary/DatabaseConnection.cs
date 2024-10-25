using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Data.Sqlite;

namespace Typeracer
{
	public class DatabaseConnection
	{
		private List<User> users = new List<User>();

		string connectionString = "Data Source=C:/Users/sLarsson/Desktop/moment5/my_db.db";

		public DatabaseConnection()
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
						command.Parameters.AddWithValue("@besttime", 0);
						command.Parameters.AddWithValue("@bestspeed", 0);
						command.Parameters.AddWithValue("@bestmistakes", 0);
						command.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-ddThh:mm.ss"));

						command.ExecuteNonQuery();
					}
				}
				catch
				{
					Console.WriteLine($"Kunde inte lägga till användaren.");
				}
				return person;
			}
		}

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

						//hämtar ett enskilt värde med en SQL-fråga
						object? result = command.ExecuteScalar();

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
				catch (Exception ex)
				{
					Console.WriteLine($"Användaren kunde inte verifieras: {ex.Message}");
					return null;
				}

			}
		}

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
							return new User
							{
								Id = reader.GetInt32(0),
								UserName = reader.GetString(1),
								BestTime = reader.GetDouble(2),
								BestSpeed = reader.GetDouble(3),
								BestMistakes = reader.GetInt32(4)
							};
						}
						else
						{
							throw new Exception("Ingen statistik hittades för användaren");
						}
					}
				}
			}

		}

        public int GetUserId(string username)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id FROM users WHERE username = @username";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
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
			catch (Exception ex)
			{
				Console.WriteLine($"Fel vid uppdatering av användarstatistik: {ex.Message}");
			}
		}	
    }
}

