using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino.Commands
{
    class SearchCommand
    {
        private Database _database;
        private Messages _messages;

        public SearchCommand(Database database, Messages messages)
        {
            _database = database;
            _messages = messages;
        }

        [Command("search")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            int moneyFound = await _database.CommandSearch(command.Member.Id);
            await _messages.CoinsGain(command, moneyFound);
        }
    }
}
