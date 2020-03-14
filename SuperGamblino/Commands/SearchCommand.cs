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
            Database.CommandSearch(command.Member.Id, 10);

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorSuccess),
                Description = "You've gained: " + 10 + " coins!"
            };
            await command.RespondAsync("", false, message);
        }
    }
}
