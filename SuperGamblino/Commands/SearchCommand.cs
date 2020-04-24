using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class SearchCommand
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public SearchCommand(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        [Command("search")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            throw new NotImplementedException();
            var moneyFound = await _usersConnector.CommandSearch(command.Member.Id);
            await _messages.CoinsGain(command, moneyFound);
        }
    }
}