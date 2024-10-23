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
											password VARCHAR(25) NOT NULL,
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
				catch(Exception ex)
				{
					throw new Exception($"Kunde inte lägga till användaren: {ex.Message}.");
				}
				return person;
			}
		}
	}
}
