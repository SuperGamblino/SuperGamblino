using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SuperGamblino.Commands
{
    internal class SearchCommand
    {
        private readonly Database _database;
        private readonly Messages _messages;

        public SearchCommand(Database database, Messages messages)
        {
            _database = database;
            _messages = messages;
        }

        [Command("search")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            var moneyFound = await _database.CommandSearch(command.Member.Id);
            await _messages.CoinsGain(command, moneyFound);
        }
    }
}