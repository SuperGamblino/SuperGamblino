using DSharpPlus.Entities;

namespace SuperGamblino.GameObjects
{
    public class User
    {
        public int currency;
        //public ulong userID;
        //public int currency;
        //public User(MySqlDataReader reader)
        //{
        //    this.userID = reader.GetUInt64("user_id");
        //    this.currency = reader.GetInt32("currency");
        //}

        public DiscordUser discordUser;
    }
}