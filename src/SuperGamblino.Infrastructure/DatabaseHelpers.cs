using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Infrastructure
{
    public static class DatabaseHelpers
    {
        public static async Task SetupTables(ConnectionString connectionString)
        {
            await using var connection = new MySqlConnection(connectionString.GetConnectionString());
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "CREATE TABLE IF NOT EXISTS Users(Id BIGINT UNSIGNED NOT NULL PRIMARY KEY,Credits INT,LastHourlyReward DateTime, LastDailyReward DateTime, LastVoteReward DateTime, LastWorkReward DateTime, Experience INT, Level INT)");
            await connection.ExecuteAsync(
                "CREATE TABLE IF NOT EXISTS GameHistories(Id BIGINT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,UserId BIGINT UNSIGNED NOT NULL,GameName TEXT NOT NULL,HasWon BOOLEAN NOT NULL,CoinsDifference INT NOT NULL)");
            await connection.ExecuteAsync(
                "CREATE TABLE IF NOT EXISTS CoinDrops (Id BIGINT NOT NULL PRIMARY KEY AUTO_INCREMENT,ChannelId BIGINT NOT NULL,ClaimId INT NOT NULL,CoinReward INT NOT NULL,Claimed BOOLEAN NOT NULL DEFAULT FALSE)");
            await connection.CloseAsync();
        }

        public static async Task SetupProcedures(ConnectionString connectionString)
        {
            await using var connection = new MySqlConnection(connectionString.GetConnectionString());
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                "DROP procedure IF EXISTS `get_top_users`;\nCREATE PROCEDURE `get_top_users`()\nBEGIN\n    SELECT * FROM Users ORDER BY Credits DESC LIMIT 10;\nEND;");
            await connection.ExecuteAsync(
                "DROP procedure IF EXISTS `give_user_exp`;\nCREATE PROCEDURE `give_user_exp`(IN given_exp INT, IN cur_user_id BIGINT, OUT did_level_increase BOOL, OUT cur_exp_needed INT, OUT cur_exp INT)\nBEGIN\n    DECLARE current_exp_user INT DEFAULT 0;\n    DECLARE current_level_user INT DEFAULT 0;\n    DECLARE needed_exp INT DEFAULT 0;\n    SELECT Level INTO current_level_user FROM Users WHERE Id = cur_user_id;\n    SELECT current_level_user * LOG10(current_level_user) * 100 + 100 INTO needed_exp;\n    SET cur_exp_needed = needed_exp;\n    UPDATE Users SET Experience = Experience + given_exp WHERE Id = cur_user_id;\n    SELECT Experience INTO cur_exp FROM Users WHERE Id = cur_user_id;\n    IF cur_exp > needed_exp THEN\n        SET did_level_increase = TRUE;\n        SET cur_exp = cur_exp - needed_exp;\n        UPDATE Users SET Experience = cur_exp, Level = Level + 1 WHERE Id = cur_user_id;\n    ELSE\n        SET did_level_increase = FALSE;\n    END IF;\nEND;");
            await connection.CloseAsync();
        }

        public static async Task<bool> CheckConnection(ConnectionString connectionString)
        {
            await using var c = new MySqlConnection(connectionString.GetConnectionString());
            try
            {
                c.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Should be called AFTER ConnectionString service and Logger service
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseConnectors(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<GameHistoryConnector>()
                .AddTransient<CoindropConnector>()
                .AddTransient<UsersConnector>();
        }
    }
}