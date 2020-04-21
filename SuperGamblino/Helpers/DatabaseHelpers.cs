using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace SuperGamblino.Helpers
{
    public static class DatabaseHelpers
    {
        public static async Task SetupTables(string connectionString)
        {
            await using var c = new MySqlConnection(connectionString);
            var createUser = new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS user(" +
                "user_id BIGINT UNSIGNED NOT NULL PRIMARY KEY," +
                "currency INT," +
                "last_hourly_reward DateTime," +
                "last_daily_reward DateTime," +
                "current_exp INT, " +
                "current_level INT, " +
                "last_work_reward DateTime," +
                "last_vote_reward DateTime)",
                c);
            var createHistory = new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS history(" +
                "user_id BIGINT UNSIGNED NOT NULL," +
                "game TEXT NOT NULL," +
                "did_win BOOL NOT NULL," +
                "credits_difference INT NOT NULL)",
                c);
            var createBlackjack = new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS blackjack(" +
                "id INT NOT NULL AUTO_INCREMENT," +
                "user_hand VARCHAR(45) NULL," +
                "dealer_hand VARCHAR(45) NULL," +
                "is_game_done TINYINT NULL," +
                "user_id BIGINT NULL," +
                "bet INT NULL," +
                "PRIMARY KEY (id))",
                c);
            var createCoindrop = new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS coin_drop (" +
                "id INT NOT NULL AUTO_INCREMENT," +
                "channel_id BIGINT NOT NULL," +
                "claim_id INT NOT NULL," +
                "coin_reward INT NOT NULL," +
                "claimed TINYINT NOT NULL DEFAULT '0'," +
                "PRIMARY KEY (id)," +
                "UNIQUE INDEX id_UNIQUE (id ASC) VISIBLE);"
                , c);
            await c.OpenAsync();
            await createUser.ExecuteNonQueryAsync();
            await createHistory.ExecuteNonQueryAsync();
            await createBlackjack.ExecuteNonQueryAsync();
            await createCoindrop.ExecuteNonQueryAsync();
            await c.CloseAsync();
        }

        public static async Task SetupProcedures(string connectionString)
        {
            await using var c = new MySqlConnection(connectionString);
            var createTop = new MySqlCommand(
                "DROP procedure IF EXISTS `get_top_users`; " +
                "CREATE PROCEDURE `get_top_users`() " +
                "BEGIN " +
                "SELECT * " +
                "FROM user " +
                "ORDER BY currency DESC " +
                "LIMIT 10; " +
                "END",
                c);
            var createIncreaseExp = new MySqlCommand(
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
                "       SET cur_exp = cur_exp - needed_exp;" +
                "       UPDATE user" +
                "       SET current_exp = cur_exp, current_level = current_level + 1" +
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
    }
}