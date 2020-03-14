using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino
{
	class Database
	{
		private static string connectionString = "";
		public static void SetConnectionString(string host, int port, string database, string username, string password)
		{
			connectionString = "server=" + host +
							   ";database=" + database +
							   ";port=" + port +
							   ";userid=" + username +
							   ";password=" + password;
		}
		public static MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public static void SetupTables()
		{
			using (MySqlConnection c = GetConnection())
			{
				MySqlCommand createUser = new MySqlCommand(
					"CREATE TABLE IF NOT EXISTS user(" +
					"id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT," +
					"created_time DATETIME NOT NULL," +
					"user_id BIGINT UNSIGNED NOT NULL)",
					c);
				c.Open();
				createUser.ExecuteNonQuery();
			}
		}

	}
}