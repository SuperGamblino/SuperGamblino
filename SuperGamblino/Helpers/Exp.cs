using DSharpPlus.CommandsNext;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Helpers
{
    class Exp
    {
        private readonly UsersConnector _usersConnector;

        public Exp (UsersConnector usersConnector)
        {
            _usersConnector = usersConnector;
        }
        public async Task<AddExpResult> Give(CommandContext command, int bet)
        {
            User user = await _usersConnector.GetUser(command.User.Id);
            Random rnd = new Random();
            int exp = rnd.Next(50, 125);
            if (user.Level * 15 > bet)
                return await _usersConnector.CommandGiveUserExp(command, 0);
            else
                return await _usersConnector.CommandGiveUserExp(command, exp);
        }
    }
}
