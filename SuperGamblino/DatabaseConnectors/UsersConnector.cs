using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;

namespace SuperGamblino.DatabaseConnectors
{
    public class UsersConnector : DatabaseConnector
    {
        public UsersConnector(ILogger logger, ConnectionString connectionString) : base(logger, connectionString)
        {
        }

        private async Task<bool> CheckIfUserExist(ulong userId)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                var checkCmd = new MySqlCommand($@"SELECT EXISTS(SELECT * FROM user WHERE user_id = {userId})",
                    connection);
                var checkResult = await checkCmd.ExecuteReaderAsync();
                await checkResult.ReadAsync();
                var doExists = checkResult.GetBoolean(0);
                await checkResult.CloseAsync();
                return doExists;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    "Exception occured while executing CheckIfUserExist method in UsersConnector class!");
                throw new Exception("Unknown problem occurred!");
                //I have no idea what kind of problem can occur here so I have no idea how to handle it
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        private async Task EnsureUserCreated(ulong userId)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                if (!await CheckIfUserExist(userId))
                {
                    await connection.OpenAsync();
                    var command =
                        new MySqlCommand(
                            $"INSERT INTO user (user_id, currency, last_daily_reward, last_hourly_reward," +
                            $" current_exp, current_level) VALUES ({userId}, 0, null, null, 0, 1)",
                            connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing" +
                                     " EnsureUserCreated method in Database class!");
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        public virtual async Task<int> CommandGiveCredits(ulong userId, int credits)
        {
            try
            {
                await EnsureUserCreated(userId);
                await using var c = new MySqlConnection(ConnectionString);
                var searchCoins = new MySqlCommand(
                    @"UPDATE user SET currency = currency + (@credits) WHERE user_id = @userId",
                    c);
                searchCoins.Parameters.AddWithValue("@userId", userId);
                searchCoins.Parameters.AddWithValue("@credits", credits);
                Logger.LogInformation(searchCoins.CommandText);
                await c.OpenAsync();
                await searchCoins.ExecuteNonQueryAsync();
                return await CommandGetUserCredits(userId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing CommandGiveCredits method in Database class!");
                return -1;
            }
        }
        
        public virtual async Task<User> GetUser(ulong userId)
        {
            await EnsureUserCreated(userId);
            await using MySqlConnection c = new MySqlConnection(ConnectionString);
            MySqlCommand selection = new MySqlCommand(@"SELECT * FROM user WHERE user_id = @user_id", c);
            selection.Parameters.AddWithValue("@user_id", userId);
            await c.OpenAsync();
            await selection.PrepareAsync();
            var results = await selection.ExecuteReaderAsync();
            if (!await results.ReadAsync()) return new User { };

            if (await results.IsDBNullAsync(2) || await results.IsDBNullAsync(3))
            {
                User user = new User
                {
                    Id = await results.GetFieldValueAsync<UInt64>(0),
                    Credits = await results.GetFieldValueAsync<int>(1),
                    LastHourlyReward = null,
                    LastDailyReward = null,
                    Experience = await results.GetFieldValueAsync<int>(4),
                    Level = await results.GetFieldValueAsync<int>(5),
                };
                return user;
            }
            else
            {
                User user = new User
                {
                    Id = await results.GetFieldValueAsync<UInt64>(0),
                    Credits = await results.GetFieldValueAsync<int>(1),
                    LastHourlyReward = await results.GetFieldValueAsync<DateTime>(2),
                    LastDailyReward = await results.GetFieldValueAsync<DateTime>(3),
                    Experience = await results.GetFieldValueAsync<int>(4),
                    Level = await results.GetFieldValueAsync<int>(5),
                };
                return user;
            }
        }

        public async Task SetUser(User user)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                if (!await CheckIfUserExist(user.Id))
                {
                    var command = new MySqlCommand(
                        $"INSERT INTO user(user_id, currency, last_daily_reward," +
                        $" last_hourly_reward, current_exp, current_level) VALUES ({user.Id}," +
                        $" {user.Credits}, {user.LastDailyReward}, {user.LastHourlyReward}," +
                        $" {user.Experience}, {user.Level})", connection);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                else
                {
                    var command = new MySqlCommand($"UPDATE user SET currency = {user.Credits}," +
                                                   $" last_daily_reward = {user.LastDailyReward}," +
                                                   $" last_hourly_reward = {user.LastHourlyReward}," +
                                                   $" current_exp = {user.Experience}," +
                                                   $" current_level = {user.Level} WHERE user_id={user.Id};",
                        connection);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing" +
                                     " SetUser method in UsersConnector class!");
                await Task.FromException(ex);
            }
        }
        
        public virtual async Task<DateTimeResult> GetDateTime(ulong userId, string fieldName)
        {
            await EnsureUserCreated(userId);
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                var command = new MySqlCommand(@$"SELECT {fieldName} from user where user_id = {userId}", connection);
                Logger.LogInformation(command.CommandText);
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
                        return new DateTimeResult(true, dateTime);
                    else
                        return new DateTimeResult(true, null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing GetDateTime with fieldName = {fieldName} method in Database class!");
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
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                var command = new MySqlCommand(
                    $"UPDATE user set {fieldName} = \'{time:yyyy-MM-dd HH:mm:ss}\' where user_id = {userId}",
                    connection);
                Logger.LogInformation(command.CommandText);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing SetDateTime with fieldName = {fieldName} and time = {time} method in Database class!");
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        public async Task<int> CommandSearch(ulong userId)
        {
            var rnd = new Random();
            var foundMoney = rnd.Next(10, 50);
            try
            {
                await using var c = new MySqlConnection();
                var searchCoins = new MySqlCommand(
                    @"INSERT INTO user (user_id, currency, current_exp, current_level) VALUES(@userId, @moneyFound, 0, 1) ON DUPLICATE KEY UPDATE currency = currency + @moneyFound",
                    c);
                await c.OpenAsync();
                searchCoins.Parameters.AddWithValue("@userId", userId);
                searchCoins.Parameters.AddWithValue("@moneyFound", foundMoney);
                Logger.LogInformation(searchCoins.CommandText);
                Logger.LogInformation($"User {userId} found {foundMoney}");
                await searchCoins.ExecuteNonQueryAsync();
                return foundMoney;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing CommandSearch method in Database class!");
                return -1;
            }
        }
        
        public virtual async Task<int> CommandGetUserCredits(ulong userId)
        {
            await EnsureUserCreated(userId);
            await using var c = new MySqlConnection(ConnectionString);
            var selection = new MySqlCommand(@"SELECT currency FROM user WHERE user_id = @user_id", c);
            selection.Parameters.AddWithValue("@user_id", userId);
            await c.OpenAsync();
            await selection.PrepareAsync();
            var results = await selection.ExecuteReaderAsync();
            if (!await results.ReadAsync()) return 0;

            var currentCredits = await results.GetFieldValueAsync<int>(0);
            await results.CloseAsync();
            return currentCredits;
        }

        public virtual async Task<bool> CommandSubsctractCredits(ulong userId, int credits)
        {
            if (await CommandGetUserCredits(userId) >= credits)
            {
                await CommandGiveCredits(userId, credits * -1);
                return true;
            }

            return false;
        }

        public virtual async Task<AddExpResult> CommandGiveUserExp(ulong userId, int exp)
        {
            await EnsureUserCreated(userId);
            await using (var c = new MySqlConnection(ConnectionString))
            {
                await c.OpenAsync();

                MySqlCommand mySqlCommand = new MySqlCommand("give_user_exp;", c);

                mySqlCommand.CommandType = CommandType.StoredProcedure;

                //Add the input parameters
                mySqlCommand.Parameters.AddWithValue("?given_exp", exp);
                mySqlCommand.Parameters["?given_exp"].Direction = ParameterDirection.Input;
                mySqlCommand.Parameters.AddWithValue("?cur_user_id", userId);
                mySqlCommand.Parameters["?cur_user_id"].Direction = ParameterDirection.Input;
                //Add the output parameters
                mySqlCommand.Parameters.Add(new MySqlParameter("?did_level_increase", MySqlDbType.Bit));
                mySqlCommand.Parameters["?did_level_increase"].Direction = ParameterDirection.Output; //cur_exp_needed
                mySqlCommand.Parameters.Add(new MySqlParameter("?cur_exp_needed", MySqlDbType.Int32));
                mySqlCommand.Parameters["?cur_exp_needed"].Direction = ParameterDirection.Output;
                mySqlCommand.Parameters.Add(new MySqlParameter("?cur_exp", MySqlDbType.Int32));
                mySqlCommand.Parameters["?cur_exp"].Direction = ParameterDirection.Output;

                await mySqlCommand.ExecuteNonQueryAsync();

                return new AddExpResult(Convert.ToBoolean(mySqlCommand.Parameters["?did_level_increase"].Value),
                    Convert.ToInt32(mySqlCommand.Parameters["?cur_exp_needed"].Value),
                    Convert.ToInt32(mySqlCommand.Parameters["?cur_exp"].Value), 
                    exp);
            }
        }

        public async Task<List<User>> CommandGetGlobalTop()
        {
            var discordUsers = new List<User>();
            await using (var c = new MySqlConnection(ConnectionString))
            {
                await c.OpenAsync();
                var selection = new MySqlCommand(@"CALL `get_top_users`()", c);
                var results = await selection.ExecuteReaderAsync();
                while (await results.ReadAsync())
                {
                    ulong uid = await results.GetFieldValueAsync<ulong>(0);
                    int cur = await results.GetFieldValueAsync<int>(1);
                    discordUsers.Add(new User { Id = uid, Credits = cur });
                }
                await results.CloseAsync();
            }

            return discordUsers;
        }
    }
}