using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using DSharpPlus;

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
                "last_daily_reward DateTime," +
                "current_exp INT, " +
                "current_level INT)",
                c);
            await c.OpenAsync();
            await createUser.ExecuteNonQueryAsync();
            await c.CloseAsync();
        }

        public async Task SetupProcedures()
        {
            await using MySqlConnection c = GetConnection();
            MySqlCommand createTop = new MySqlCommand(
                "DROP procedure IF EXISTS `get_top_users`; " +
                "CREATE PROCEDURE `get_top_users`() " +
                "BEGIN " +
                "SELECT * " +
                "FROM user " +
                "ORDER BY currency DESC " +
                "LIMIT 10; " +
                "END",
                c);
            MySqlCommand createIncreaseExp = new MySqlCommand(
                "DROP procedure IF EXISTS `give_user_exp`;" +
                "CREATE PROCEDURE `give_user_exp`(	" +
                "   IN given_exp INT," +
                "   IN cur_user_id BIGINT," +
                "   OUT did_level_increase BOOL," +
                "   OUT cur_exp_needed INT," +
                "   OUT cur_exp INT" +
                ")" +
                "BEGIN" +
                "   DECLARE current_exp_user int DEFAULT 0;" +
                "   DECLARE current_level_user INT DEFAULT 0;" +
                "	DECLARE needed_exp INT DEFAULT 0;" +
                "   SELECT current_level" +
                "   INTO current_level_user" +
                "   FROM user" +
                "   WHERE user_id = cur_user_id;" +
                "   SELECT current_level_user * LOG10(current_level_user) * 100 + 100" +
                "   INTO needed_exp;" +
                "   SET cur_exp_needed = needed_exp;" +
                "   UPDATE user" +
                "   SET current_exp = current_exp + given_exp" +
                "   WHERE user_id = cur_user_id;" +
                "   SELECT current_exp" +
                "   INTO cur_exp" +
                "   FROM user" +
                "   WHERE user_id = cur_user_id;" +
                "   IF cur_exp > needed_exp THEN" +
                "		SET did_level_increase = true;" +
                "       SET cur_exp = 0;" +
                "       UPDATE user" +
                "       SET current_exp = 0, current_level = current_level + 1" +
                "       WHERE user_id = cur_user_id;" +
                "   ELSE" +
                "   	SET did_level_increase = false;" +
                "	END IF;" +
                "END",
                c);
            await c.OpenAsync();
            await createTop.ExecuteNonQueryAsync();
            await createIncreaseExp.ExecuteNonQueryAsync();
            await c.CloseAsync();
        }

        public async Task<int> CommandGetUserCredits(ulong userId)
        {
            await EnsureUserCreated(userId);
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

        public async Task<AddExpResult> CommandGiveUserExp(CommandContext command, int exp)
        {
            await EnsureUserCreated(command.User.Id);
            await using (MySqlConnection c = GetConnection())
            {
                await c.OpenAsync();

                MySqlCommand mySqlCommand = new MySqlCommand("give_user_exp;", c);
                mySqlCommand.CommandType = CommandType.StoredProcedure;

                //Add the input parameters
                mySqlCommand.Parameters.AddWithValue("?given_exp", exp);
                mySqlCommand.Parameters["?given_exp"].Direction = ParameterDirection.Input;
                mySqlCommand.Parameters.AddWithValue("?cur_user_id", command.User.Id);
                mySqlCommand.Parameters["?cur_user_id"].Direction = ParameterDirection.Input;
                //Add the output parameters
                mySqlCommand.Parameters.Add(new MySqlParameter("?did_level_increase", MySqlDbType.Bit));
                mySqlCommand.Parameters["?did_level_increase"].Direction = ParameterDirection.Output; //cur_exp_needed
                mySqlCommand.Parameters.Add(new MySqlParameter("?cur_exp_needed", MySqlDbType.Int32));
                mySqlCommand.Parameters["?cur_exp_needed"].Direction = ParameterDirection.Output;
                mySqlCommand.Parameters.Add(new MySqlParameter("?cur_exp", MySqlDbType.Int32));
                mySqlCommand.Parameters["?cur_exp"].Direction = ParameterDirection.Output;

                await mySqlCommand.ExecuteNonQueryAsync();

                return new AddExpResult(Convert.ToBoolean(mySqlCommand.Parameters["?did_level_increase"].Value), Convert.ToInt32(mySqlCommand.Parameters["?cur_exp_needed"].Value), Convert.ToInt32(mySqlCommand.Parameters["?cur_exp"].Value));
            }

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
                    discordUsers.Add(new User { DiscordUser = await command.Client.GetUserAsync(uid), Credits = cur });
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
                await EnsureUserCreated(userId);
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
                    @"INSERT INTO user (user_id, currency, current_exp, current_level) VALUES(@userId, @moneyFound, 0, 1) ON DUPLICATE KEY UPDATE currency = currency + @moneyFound",
                    c);
                await c.OpenAsync();
                searchCoins.Parameters.AddWithValue("@userId", userId);
                searchCoins.Parameters.AddWithValue("@moneyFound", foundMoney);
                _logger.LogInformation(searchCoins.CommandText);
                _logger.LogInformation($"User {userId} found {foundMoney}");
                await searchCoins.ExecuteNonQueryAsync();
                return foundMoney;
            }
            catch (Exception ex)
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
                var checkCmd = new MySqlCommand($@"SELECT EXISTS(SELECT * FROM user WHERE user_id = {userId})", connection);
                var checkResult = await checkCmd.ExecuteReaderAsync();
                await checkResult.ReadAsync();
                var doExists = checkResult.GetBoolean(0);
                await checkResult.CloseAsync();
                if (!doExists)
                {
                    var command = new MySqlCommand($@"INSERT INTO user (user_id, currency, last_daily_reward, last_hourly_reward, current_exp, current_level) VALUES ({userId}, 0, null, null, 0, 1)", connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while executing EnsureUserCreated method in Database class!");
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
                _logger.LogError(ex, $"Exception occured while executing GetDateTime with fieldName = {fieldName} method in Database class!");
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

        public async Task<User> GetUser(CommandContext command)
        {
            ulong userId = command.User.Id;
            await EnsureUserCreated(userId);
            await using MySqlConnection c = GetConnection();
            MySqlCommand selection = new MySqlCommand(@"SELECT * FROM user WHERE user_id = @user_id", c);
            selection.Parameters.AddWithValue("@user_id", userId);
            await c.OpenAsync();
            await selection.PrepareAsync();
            var results = await selection.ExecuteReaderAsync();
            await results.ReadAsync();

            User user = new User {
                Id = await results.GetFieldValueAsync<UInt64>(0),
                Credits = await results.GetFieldValueAsync<int>(1),
                LastHourlyReward = await results.GetFieldValueAsync<DateTime>(2),
                LastDailyReward = await results.GetFieldValueAsync<DateTime>(3),
                Experience = await results.GetFieldValueAsync<int>(4),
                Level = await results.GetFieldValueAsync<int>(5),
                DiscordUser = await command.Client.GetUserAsync(userId)

            };
            await results.CloseAsync();
            return user;
        }
    }
}