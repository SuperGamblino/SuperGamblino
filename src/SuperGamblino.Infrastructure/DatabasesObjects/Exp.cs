using System;
using System.Threading.Tasks;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Infrastructure.DatabasesObjects
{
    public class Exp
    {
       private readonly UsersConnector _usersConnector;

        public Exp(UsersConnector usersConnector)
        {
            _usersConnector = usersConnector;
        }

        public async Task<AddExpResult> Give(ulong userId, int bet)
        {
            var user = await _usersConnector.GetUser(userId);
            var exp = new Random().Next(50, 125);
            return await _usersConnector.CommandGiveUserExp(userId, user.Level * 15 > bet ? 0 : exp);
        }
    }
}