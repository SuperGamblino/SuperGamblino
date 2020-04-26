using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;

namespace SuperGamblino.DatabaseConnectors
{
    public class CoindropConnector : DatabaseConnector
    {
        public CoindropConnector(ILogger logger, ConnectionString connectionString) : base(logger, connectionString)
        {
        }

        public async Task<int> AddCoindrop(ulong channelId, int coinReward)
        {
            await using var c = new MySqlConnection(ConnectionString);
            Random rand = new Random();
            int claimId = rand.Next(1000, 9999);
            try
            {
                var command = new MySqlCommand("INSERT INTO coin_drop (channel_id, claim_id, coin_reward)" +
                                               $" VALUES ({channelId.ToString()}, \"{claimId}\", {coinReward})",
                    c);
                await c.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return claimId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing AddCoindrop method in Database class!");
                return 0;
            }
            finally
            {
                await c.CloseAsync();
            }
        }
        public async Task SetClaimed(ulong channelId)
        {
            await using var c = new MySqlConnection(ConnectionString);
            try
            {
                //UPDATE `supergamblino`.`coin_drop` SET `claimed` = '1' WHERE (`id` = '16');
                var command = new MySqlCommand($"UPDATE coin_drop SET claimed = '1' WHERE channel_id = '{channelId}'",
                    c);
                await c.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    $"Exception occured while executing SetClaimed method in Database class!");
            }
            finally
            {
                await c.CloseAsync();
            }

        }
        public virtual async Task<int> CollectCoinDrop(ulong channelId, int claimId)
        {
            await using var c = new MySqlConnection(ConnectionString);
            try
            {
                var sql = new MySqlCommand($"SELECT claimed, coin_reward, claim_id FROM coin_drop WHERE channel_id = '{channelId}' ORDER BY id DESC LIMIT 0, 1 ", c);
                await c.OpenAsync();
                var reader = await sql.ExecuteReaderAsync();

                sbyte claimed = 1;
                int coinReward = 0;
                int claim_id = 0;
                while (await reader.ReadAsync())
                {
                    claimed = await reader.GetFieldValueAsync<sbyte>(0);
                    coinReward = await reader.GetFieldValueAsync<int>(1);
                    claim_id = await reader.GetFieldValueAsync<int>(2);
                }
                if (claimed == 0 && claim_id == claimId)
                {
                    await SetClaimed(channelId);
                    return coinReward;
                }
                else
                    return 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.LogError(ex,
                    $"Exception occured while executing CollectCoinDrop method in Database class!");
                return 0;
            }
            finally
            {
                await c.CloseAsync();
            }
        }
    }
}