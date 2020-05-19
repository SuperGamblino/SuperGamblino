using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Entities;

namespace SuperGamblino.Infrastructure.Connectors
{
    public class UsersConnector : DatabaseConnector
    {
        private const string CheckIfUserExistsKey = "UsersConnector-CheckIfUserExists-";
        private const string GetCreditsKey = "UsersConnector-GetCredits-";
        private const string GetUserKey = "UsersConnector-GetUser-";
        private const string GlobalTopKey = "UsersConnector-GlobalTop";
        public UsersConnector(ILogger<UsersConnector> logger, ConnectionString connectionString, IMemoryCache cache) : base(logger, connectionString, cache)
        {
            logger.LogInformation("Created UsersConnector!");
        }
        
        private async Task<bool> CheckIfUserExists(ulong userId)
        {
            if (MemoryCache.TryGetValue(CheckIfUserExistsKey + userId, out bool exist))
            {
                return exist;
            }
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                var doExists = (await connection.QueryAsync<bool>(
                    "SELECT EXISTS(SELECT * FROM Users WHERE Id = @UserId)",
                    new {UserId = userId})).SingleOrDefault();
                MemoryCache.Set(CheckIfUserExistsKey + userId, doExists, TimeSpan.FromSeconds(10));
                return doExists;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,"Exception occured while executing CheckIfUserExist method in UsersConnector class!");
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        private async Task EnsureUserCreated(ulong userId)
        {
            if (!await CheckIfUserExists(userId))
            {
                await using var connection = new MySqlConnection(ConnectionString);
                try
                {
                    await connection.OpenAsync();
                    await connection.InsertAsync(new User(userId));
                    MemoryCache.Remove(CheckIfUserExistsKey+userId);
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
        }

        public virtual async Task GiveCredits(ulong userId, int credits)
        {
            MemoryCache.Remove(GetCreditsKey+userId);
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await EnsureUserCreated(userId);
                await connection.OpenAsync();
                await connection.ExecuteAsync("UPDATE Users SET Credits = Credits + (@credits) WHERE Id = @userId", new
                {
                    credits, userId
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing CommandGiveCredits method in Database class!");
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        public virtual async Task<bool> TakeCredits(ulong userId, int credits)
        {
            MemoryCache.Remove(GetCreditsKey+userId);
            if (await GetCredits(userId) < credits) return false;
            await GiveCredits(userId, credits * -1);
            return true;

        }
        
        /// <summary>
        /// Gets user credits
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Number of credits or -1 if exception occured</returns>
        public virtual async Task<int> GetCredits(ulong userId)
        {
            if (MemoryCache.TryGetValue(GetCreditsKey + userId, out int credits))
            {
                return credits;
            }
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await EnsureUserCreated(userId);
                await connection.OpenAsync();
                var balance = (await connection.QueryAsync<int>("SELECT Credits FROM Users WHERE Id = @userId", new {userId}))
                    .Single();
                MemoryCache.Set(GetCreditsKey + userId, balance, TimeSpan.FromSeconds(5));
                return balance;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing GetCredits method in Database class!");
                return -1;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public virtual async Task<User> GetUser(ulong userId)
        {
            if (MemoryCache.TryGetValue(GetUserKey + userId, out User user))
            {
                return user;
            }
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await EnsureUserCreated(userId);
                await connection.OpenAsync();
                var result = await connection.GetAsync<User>(userId);
                MemoryCache.Set(GetUserKey + user, result, TimeSpan.FromSeconds(5));
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing GetUser method in Database class!");
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public virtual async Task UpdateUser(User user)
        {
            MemoryCache.Remove(GetUserKey+user);
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await EnsureUserCreated(user.Id);
                await connection.OpenAsync();
                await connection.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing UpdateUser method in Database class!");
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        
        public virtual async Task<AddExpResult> CommandGiveUserExp(ulong userId, int exp)
        {
            const string procedure = "give_user_exp";
            
            await EnsureUserCreated(userId);
            
            var parameters = new DynamicParameters();
            
            parameters.Add("given_exp", exp);
            parameters.Add("cur_user_id", userId);
            parameters.Add("did_level_increase", direction: ParameterDirection.Output, dbType: DbType.Byte);
            parameters.Add("cur_exp_needed", direction: ParameterDirection.Output);
            parameters.Add("cur_exp", direction: ParameterDirection.Output);
            
            await using var connection = new MySqlConnection(ConnectionString);
            
            await connection.OpenAsync();
            await connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            
            var didLevelIncrease = Convert.ToBoolean(parameters.Get<byte>("did_level_increase"));
            var curExpNeeded = parameters.Get<int>("cur_exp_needed");
            var curExp = parameters.Get<int>("cur_exp");
            return new AddExpResult(didLevelIncrease,curExpNeeded, curExp, exp);
        }

        public virtual async Task<IEnumerable<User>> CommandGetGlobalTop()
        {
            const string procedure = "get_top_users";

            if (MemoryCache.TryGetValue(GlobalTopKey, out IEnumerable<User> users))
            {
                return users;
            }
            
            await using var connection = new MySqlConnection(ConnectionString);

            await connection.OpenAsync();
            var result = (await connection.QueryAsync<User>(procedure, commandType: CommandType.StoredProcedure)).AsList();
            MemoryCache.Set(GlobalTopKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}