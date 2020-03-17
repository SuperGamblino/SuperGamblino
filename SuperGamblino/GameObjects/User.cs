using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    public class User
    {
        //public ulong userID;
        //public int currency;
        //public User(MySqlDataReader reader)
        //{
        //    this.userID = reader.GetUInt64("user_id");
        //    this.currency = reader.GetInt32("currency");
        //}

        public DiscordUser discordUser;
        public int currency;
    }
}
