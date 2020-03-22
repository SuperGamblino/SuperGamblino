using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino.Commands
{
    class CreditsCommand
    {
        [Command("credits")]
        [Aliases("cred")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            int currentCredits = await Database.CommandGetUserCredits(command.User.Id);

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorInfo),
                Description = "You currently have:\n" + currentCredits + " credits"
            };
            await command.RespondAsync("", false, message);
        }
    }
}
