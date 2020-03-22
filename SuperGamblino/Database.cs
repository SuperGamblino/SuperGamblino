using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SuperGamblino
{
	class Database
	{
		private string _connectionString = "";
		private readonly ILogger _logger;

		public Database(ILogger logger)
		{
			_logger = logger;
		}

		public void SetConnectionString(string host, int port, string database, string username, string password)
		{
			_connectionString = "server=" + host +
							   ";database=" + database +
							   ";port=" + port +
							   ";userid=" + username +
							   ";password=" + password;
		}

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(_connectionString);
		}

		public async Task SetupTables()
		{
			await using MySqlConnection c = GetConnection();
			MySqlCommand createUser = new MySqlCommand(
				"CREATE TABLE IF NOT EXISTS user(" +
				"user_id BIGINT UNSIGNED NOT NULL PRIMARY KEY," +
				"currency INT," +
				"last_hourly_reward DateTime," +
				"last_daily_reward DateTime)",
				c);
			await c.OpenAsync();
			await createUser.ExecuteNonQueryAsync();
			await c.CloseAsync();
		}
	
		public async Task SetupProcedures()
		{
			await using MySqlConnection c = GetConnection();
			MySqlCommand createUser = new MySqlCommand(
				"DROP procedure IF EXISTS `get_top_users`; " +
				"CREATE PROCEDURE `get_top_users`() " +
				"BEGIN " +
				"SELECT * " +
				"FROM user " +
				"ORDER BY currency DESC " + 
				"LIMIT 10; " + 
				"END",
				c);
			await c.OpenAsync();
			await createUser.ExecuteNonQueryAsync();
			await c.CloseAsync();
		}

		public async Task<int> CommandGetUserCredits(ulong userId)
		{
			await using MySqlConnection c = GetConnection();
			MySqlCommand selection = new MySqlCommand(@"SELECT currency FROM user WHERE user_id = @user_id", c);
			selection.Parameters.AddWithValue("@user_id", userId);
			await c.OpenAsync();
			await selection.PrepareAsync();
			var results = await selection.ExecuteReaderAsync();
			if (!await results.ReadAsync())
			{
				return 0;
			}

			var currentCredits = await results.GetFieldValueAsync<int>(0);
			await results.CloseAsync();
			return currentCredits;
		}
		
		public async Task<bool> CommandSubsctractCredits(ulong userId, int credits)
		{
				if (await CommandGetUserCredits(userId) >= credits)
				{
					await CommandGiveCredits(userId, credits * -1);
					return true;
				}
				return false;
		}

		public async Task<List<User>> CommandGetGlobalTop(CommandContext command)
		{
			List<User> discordUsers = new List<User>();
			//CALL `supergamblino`.`get_top_users`();
			await using (MySqlConnection c = GetConnection())
			{
				await c.OpenAsync();
				MySqlCommand selection = new MySqlCommand(@"CALL `get_top_users`()", c);
				var results = await selection.ExecuteReaderAsync();
				while (results.Read())
				{
					ulong uid = await results.GetFieldValueAsync<ulong>(0);
					int cur = await results.GetFieldValueAsync<int>(1);
					discordUsers.Add(new User { discordUser = await command.Client.GetUserAsync(uid), currency = cur});
				}
				//Object currentCredits = results.GetValue(0);
				await results.CloseAsync();
			}
			return discordUsers;
		}

		public async Task<int> CommandGiveCredits(ulong userId, int credits)
		{
			try
			{
				await using MySqlConnection c = GetConnection();
				MySqlCommand searchCoins = new MySqlCommand(
					@"UPDATE user SET currency = currency + (@credits) WHERE user_id = @userId",
					c);
				searchCoins.Parameters.AddWithValue("@userId", userId);
				searchCoins.Parameters.AddWithValue("@credits", credits);
				_logger.LogInformation(searchCoins.CommandText);
				await c.OpenAsync();
				await searchCoins.ExecuteNonQueryAsync();
				return await CommandGetUserCredits(userId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occured while executing CommandGiveCredits method in Database class!");
				return -1;
			}
		}

		public async Task<int> CommandSearch(ulong userId)
		{
			Random rnd = new Random();
			int foundMoney = rnd.Next(10, 50);
			try
			{
				await using MySqlConnection c = GetConnection();
				MySqlCommand searchCoins = new MySqlCommand(
					@"INSERT INTO user (user_id, currency) VALUES(@userId, @moneyFound) ON DUPLICATE KEY UPDATE currency = currency + @moneyFound",
					c);
				await c.OpenAsync();
				searchCoins.Parameters.AddWithValue("@userId", userId);
				searchCoins.Parameters.AddWithValue("@moneyFound", foundMoney);
				_logger.LogInformation(searchCoins.CommandText);
				_logger.LogInformation($"User {userId} found {foundMoney}");
				await searchCoins.ExecuteNonQueryAsync();
				return foundMoney;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Exception occured while executing CommandSearch method in Database class!");
				return -1;
			}
		}

		private async Task EnsureUserCreated(ulong userId)
		{
			await using var connection = GetConnection();
			try
			{
				await connection.OpenAsync();
				var checkCmd = new MySqlCommand($@"SELECT EXISTS(SELECT * FROM user WHERE user_id = {userId})",connection);
				var checkResult = await checkCmd.ExecuteReaderAsync();
				await checkResult.ReadAsync();
				var doExists = checkResult.GetBoolean(0);
				await checkResult.CloseAsync();
				if (!doExists)
				{
					var command = new MySqlCommand($@"INSERT INTO user (user_id, currency, last_daily_reward, last_hourly_reward) VALUES ({userId}, 0, null, null)",connection);
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,"Exception occured while executing EnsureUserCreated method in Database class!");
			}
			finally
			{
				await connection.CloseAsync();
			}
		}
		

		public async Task<DateTimeResult> GetDateTime(ulong userId, string fieldName)
		{
			await EnsureUserCreated(userId);
			await using var connection = GetConnection();
			try
			{
				var command = new MySqlCommand(@$"SELECT {fieldName} from user where user_id = {userId}", connection);
				_logger.LogInformation(command.CommandText);
				await connection.OpenAsync();
				var reader = await command.ExecuteReaderAsync();
				if (!await reader.ReadAsync())
				{
					return new DateTimeResult(false, null);
				}
				else
				{
					var value = reader.GetValue(0);
					if (value is DateTime dateTime)
					{
						return new DateTimeResult(true, dateTime);
					}
					else
					{
						return new DateTimeResult(true, null);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,$"Exception occured while executing GetDateTime with fieldName = {fieldName} method in Database class!");
				return new DateTimeResult(false, null);
			}
			finally
			{
				await connection.CloseAsync();
			}
		}

		public async Task<bool> SetDateTime(ulong userId, string fieldName, DateTime time)
		{
			//Here is no need to use EnsureUserCreated because this method is always called after GetDateTime
			await using var connection = GetConnection();
			try
			{
				var command = new MySqlCommand($"UPDATE user set {fieldName} = \'{time:yyyy-MM-dd HH:mm:ss}\' where user_id = {userId}",
					connection);
				_logger.LogInformation(command.CommandText);
				await connection.OpenAsync();
				await command.ExecuteNonQueryAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Exception occured while executing SetDateTime with fieldName = {fieldName} and time = {time} method in Database class!");
				return false;
			}
			finally
			{
				await connection.CloseAsync();
			}
		}

	}
}