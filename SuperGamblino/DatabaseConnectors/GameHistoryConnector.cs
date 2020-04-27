using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;

namespace SuperGamblino.DatabaseConnectors
{
    public class GameHistoryConnector : DatabaseConnector
    {
        public GameHistoryConnector(ILogger logger, ConnectionString connectionString) : base(logger, connectionString)
        {
        }

        public async Task<History> GetGameHistories(ulong userId)
        {
            await using var c = new MySqlConnection(ConnectionString);
            try
            {
                var command = new MySqlCommand($"SELECT * FROM history WHERE user_id = {userId}", c);
                await c.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                var history = new History();
                history.UserId = userId;
                var list = new List<GameHistory>();
                while (await reader.ReadAsync())
                    list.Add(new GameHistory
                    {
                        GameName = await reader.GetFieldValueAsync<string>(1),
                        HasWon = await reader.GetFieldValueAsync<bool>(2),
                        CoinsDifference = await reader.GetFieldValueAsync<int>(3)
                    });

                history.GameHistories = list;
                return history;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing GetGameHistories with userId = {userId} method in Database class!");
                return null;
            }
            finally
            {
                await c.CloseAsync();
            }
        }

        public async Task<bool> AddGameHistory(ulong userId, GameHistory history)
        {
            await using var c = new MySqlConnection(ConnectionString);
            try
            {
                var command = new MySqlCommand("INSERT INTO history (user_id, game, did_win, credits_difference)" +
                                               $" VALUES ({userId}, \"{history.GameName}\", {history.HasWon}, {history.CoinsDifference})",
                    c);
                await c.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing AddGameHistory with userId = {userId} method in Database class!");
                return false;
            }
            finally
            {
                await c.CloseAsync();
            }
        }
    }
}