using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SuperGamblino.GameObjects;

namespace SuperGamblino.DatabaseConnectors
{
    public class BlackjackConnector : DatabaseConnector
    {
        public BlackjackConnector(ILogger logger, ConnectionString connectionString) : base(logger, connectionString)
        {
        }

        public async Task<BlackjackHelper> GetBlackjackGame(ulong userId)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                var checkCmd = new MySqlCommand(
                    $@"SELECT EXISTS(SELECT * FROM blackjack WHERE user_id = {userId} AND is_game_done = 0)",
                    connection);
                var checkResult = await checkCmd.ExecuteReaderAsync();
                await checkResult.ReadAsync();
                var doExists = checkResult.GetBoolean(0);
                await checkResult.CloseAsync();
                if (doExists)
                {
                    var getCmd = new MySqlCommand(
                        $@"SELECT user_hand, dealer_hand, is_game_done FROM blackjack WHERE user_id = {userId} AND is_game_done = 0",
                        connection);
                    var result = await getCmd.ExecuteReaderAsync();
                    await result.ReadAsync();
                    var blackjackHelper = new BlackjackHelper
                    {
                        UserHand = result.GetString(0),
                        DealerHand = result.GetString(1),
                        IsGameDone = Convert.ToBoolean(result.GetByte(2))
                    };
                    await result.CloseAsync();
                    return blackjackHelper;
                }
                else
                {
                    return new BlackjackHelper {IsGameDone = true};
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing GetBlackjackGame method in Database class!");
                return new BlackjackHelper();
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task StartNewBlackjackGame(BlackjackHelper blackjackHelper, ulong userId)
        {
            await using var connection = new MySqlConnection(ConnectionString);

            try
            {
                await connection.OpenAsync();
                var insertCmd = new MySqlCommand(
                    $@"INSERT INTO blackjack(user_hand, dealer_hand, is_game_done, user_id, bet) VALUES('{blackjackHelper.UserHand}', '{blackjackHelper.DealerHand}',{Convert.ToInt32(blackjackHelper.IsGameDone)},{userId},{blackjackHelper.Bet})",
                    connection);
                await insertCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    "Exception occured while executing StartBewBlackhackGame method in Database class!");
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task UpdateBlackjackGame(BlackjackHelper blackjackHelper, ulong userId)
        {
            await using var connection = new MySqlConnection(ConnectionString);

            try
            {
                await connection.OpenAsync();
                var updateCmd = new MySqlCommand(
                    $@"UPDATE blackjack SET user_hand = '{blackjackHelper.UserHand}', dealer_hand = '{blackjackHelper.DealerHand}', is_game_done = {Convert.ToInt32(blackjackHelper.IsGameDone)}, bet = {blackjackHelper.Bet} WHERE user_id = {userId} AND is_game_done = 0",
                    connection);

                await updateCmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occured while executing UpdateBlackhackGame method in Database class!");
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}