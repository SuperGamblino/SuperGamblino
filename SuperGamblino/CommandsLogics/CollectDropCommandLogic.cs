using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class CollectDropCommandLogic
    {
        private readonly Messages _messages;
        private readonly CoindropConnector _coindropConnector;
        private readonly UsersConnector _usersConnector;

        public CollectDropCommandLogic(Messages messages, CoindropConnector coindropConnector, UsersConnector usersConnector)
        {
            _messages = messages;
            _coindropConnector = coindropConnector;
            _usersConnector = usersConnector;
        }

        public async Task<DiscordEmbed> Collect(string argument, ulong channelId, ulong userId, string username)
        {
            var arguments = argument.ToUpper().TrimStart().Split(' ');

            if (arguments.Length != 1)
            {
                return _messages.InvalidArguments(new [] {"<Claim Id>"}, "Collect", "Collect");
            }
            
            try
            {
                var collectId = Convert.ToInt32(arguments[0]);
                var reward = await _coindropConnector.CollectCoinDrop(channelId, collectId);
                if (reward != 0)
                {
                    await _usersConnector.CommandGiveCredits(userId, reward);
                    return _messages.CoinDropSuccessful(reward, username);
                }
                else
                {
                    return _messages.CoinDropTooLate();
                }
            }
            catch (Exception)
            {
                return _messages.InvalidArguments(new [] {"<Claim Id>"}, "Collect", "Collect");
            }
        }
    }
}