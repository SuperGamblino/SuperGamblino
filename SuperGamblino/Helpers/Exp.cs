using DSharpPlus.CommandsNext;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino.Helpers
{
    class Exp
    {
        private readonly Database _database;

        public Exp (Database database)
        {
            _database = database;
        }
        public async Task<AddExpResult> Give(CommandContext command, int bet)
        {
            User user = await _database.GetUser(command);
            Random rnd = new Random();
            int exp = rnd.Next(50, 125);
            if (user.Level * 15 >= bet)
                return await _database.CommandGiveUserExp(command, 0);
            else
                return await _database.CommandGiveUserExp(command, exp);
        }
    }
}
