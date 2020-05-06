using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class GlobalTopCommandLogic
    {
        private readonly ILogger _logger;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public GlobalTopCommandLogic(UsersConnector usersConnector, ILogger logger, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _logger = logger;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> GetGlobalTop(Func<ulong, Task<string>> getUserUsername)
        {
            var listUsers = await _usersConnector.CommandGetGlobalTop();

            var desc = "";
            foreach (var user in listUsers)
                try
                {
                    desc += await getUserUsername(user.Id) + ": " + user.Credits + "\n";
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't find {user.Id} username via getUserUsername method!", ex);
                }

            return _messagesHelper.Information(desc, "GlobalTop");
        }
    }
}