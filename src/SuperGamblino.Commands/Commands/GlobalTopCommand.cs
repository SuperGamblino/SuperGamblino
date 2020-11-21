using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class GlobalTopCommand
    {
        private readonly ILogger<GlobalTopCommand> _logger;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public GlobalTopCommand(UsersConnector usersConnector, ILogger<GlobalTopCommand> logger,
            MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _logger = logger;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetGlobalTop(Func<ulong, Task<string>> getUserUsername)
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