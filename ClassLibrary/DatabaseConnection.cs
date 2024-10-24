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
					Console.WriteLine($"Kunde inte l�gga till anv�ndaren.");
				}
				return person;
			}
		}

		public string GetPasswordHash(string username)
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

						//h�mtar ett enskilt v�rde med en SQL-fr�ga
						object? result = command.ExecuteScalar();

						if (result!= null)
						{
							return result.ToString();
						}
						else
						{
							throw new Exception("Fel vid anv�ndarverifiering");
						}
						
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Anv�ndaren kunde inte verifieras: {ex.Message}");
					return string.Empty; 
				}
				
			}
		}
	}
}
