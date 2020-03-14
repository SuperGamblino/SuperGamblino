using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;
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
		
		public static bool CommandSubsctractCredits(ulong userId, int credits)
		{
			using (MySqlConnection c = GetConnection())
			{
				c.Open();
				MySqlCommand selection = new MySqlCommand(@"SELECT currency FROM user WHERE user_id = @user_id", c);
				selection.Parameters.AddWithValue("@user_id", userId);
				selection.Prepare();
				MySqlDataReader results = selection.ExecuteReader();

				// Check if staff exists in the database
				if (!results.Read())
				{
					return false;
				}

				Object currentCredits = results.GetValue(0);
				results.Close();
				if (Convert.ToInt32(currentCredits) >= credits)
				{
					CommandGiveCredits(userId, credits * -1);
					return true;
				}

				return false;
			}
		}
		public static void CommandGiveCredits(ulong userId, int credits)
		{
			try
			{
				using (MySqlConnection c = GetConnection())
				{
					MySqlCommand searchCoins = new MySqlCommand(
						@"UPDATE user SET currency = currency + (@credits) WHERE user_id = @userId",
						c);
					c.Open();
					searchCoins.Parameters.AddWithValue("@userId", userId);
					searchCoins.Parameters.AddWithValue("@credits", credits);
					Console.WriteLine(searchCoins.CommandText);
					searchCoins.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public static int CommandSearch(ulong userId)
		{
			Random rnd = new Random();
			int foundMoney = rnd.Next(10, 50);
			try
			{
				using (MySqlConnection c = GetConnection())
				{
					MySqlCommand searchCoins = new MySqlCommand(
						@"INSERT INTO user (user_id, currency) VALUES(@userId, @moneyFound) ON DUPLICATE KEY UPDATE currency = currency + @moneyFound",
						c);
					c.Open();
					searchCoins.Parameters.AddWithValue("@userId", userId);
					searchCoins.Parameters.AddWithValue("@moneyFound", foundMoney);
					Console.WriteLine(searchCoins.CommandText);
					Console.WriteLine(foundMoney);
					searchCoins.ExecuteNonQuery();
					return foundMoney;
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return -1;
			}

		}

	}
}