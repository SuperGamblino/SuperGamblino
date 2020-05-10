using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class CollectDropCommandLogic
    {
        private readonly CoindropConnector _coindropConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CollectDropCommandLogic(MessagesHelper messagesHelper, CoindropConnector coindropConnector,
            UsersConnector usersConnector)
        {
            _messagesHelper = messagesHelper;
            _coindropConnector = coindropConnector;
            _usersConnector = usersConnector;
        }

        public async Task<DiscordEmbed> Collect(string argument, ulong channelId, ulong userId, string username)
        {
            var arguments = argument.ToUpper().TrimStart().Split(' ');

            if (arguments.Length != 1)
                return _messagesHelper.InvalidArguments(new[] {"<Claim Id>"}, "Collect", "Collect");

            try
            {
                var collectId = Convert.ToInt32(arguments[0]);
                var reward = await _coindropConnector.CollectCoinDrop(channelId, collectId);
                if (reward != 0)
                {
                    await _usersConnector.CommandGiveCredits(userId, reward);
                    return _messagesHelper.CoinDropSuccessful(reward, username);
                }

                return _messagesHelper.CoinDropTooLate();
            }
            catch (Exception)
            {
                return _messagesHelper.InvalidArguments(new[] {"<Claim Id>"}, "Collect", "Collect");
            }
        }
    }
}