using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Entities;

namespace SuperGamblino.Infrastructure.Connectors
{
    public class CoindropConnector : DatabaseConnector
    {
        public CoindropConnector(ILogger<CoindropConnector> logger, ConnectionString connectionString,
            IMemoryCache memoryCache) : base(logger, connectionString, memoryCache)
        {
        }

        public async Task<int> AddCoindrop(ulong channelId, int claimId, int coinReward)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await connection.InsertAsync(new CoinDrop
                {
                    ChannelId = channelId,
                    ClaimId = claimId
                });
                return claimId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing AddCoindrop method in Database class!");
                return 0;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public virtual async Task<int> CollectCoinDrop(ulong channelId, int claimId)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                var result = (await connection.QueryAsync<CoinDrop>(
                    "SELECT * FROM CoinDrops WHERE ChannelId = @channelId ORDER BY Id DESC LIMIT 0,1", new
                    {
                        channelId
                    })).Single();
                if (result.Claimed == false && result.ClaimId == claimId)
                {
                    await connection.ExecuteAsync("UPDATE CoinDrops SET Claimed = TRUE WHERE ChannelId = @channelId",
                        new {channelId});
                    return result.CoinReward;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.LogError(ex,
                    "Exception occured while executing CollectCoinDrop method in Database class!");
                return 0;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}