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
					"user_id BIGINT UNSIGNED NOT NULL PRIMARY KEY," +
					"currency INT)",
					c);
				c.Open();
				createUser.ExecuteNonQuery();
			}
		}
		
		public static void CommandSearch(ulong userId, int currency)
		{
			try
			{
				using (MySqlConnection c = GetConnection())
				{
					MySqlCommand searchCoins = new MySqlCommand(
						@"INSERT INTO user (user_id, currency) VALUES(@userId, @currency) ON DUPLICATE KEY UPDATE currency = currency + 10",
						c);
					c.Open();
					searchCoins.Parameters.AddWithValue("@userId", userId);
					searchCoins.Parameters.AddWithValue("@currency", currency);

					Console.WriteLine(searchCoins.CommandText);
					searchCoins.ExecuteNonQuery();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

	}
}