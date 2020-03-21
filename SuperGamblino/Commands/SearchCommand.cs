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
        [Command("search")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            int moneyFound = Database.CommandSearch(command.Member.Id);
            await Messages.CoinsGain(command, moneyFound);
        }
    }
}
