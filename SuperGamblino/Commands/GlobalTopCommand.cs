using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino.Commands
{
    class GlobalTopCommand
    {
        [Command("globaltop")]
        [Aliases("gt")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
           List<User> listUsers = await Database.CommandGetGlobalTop(command);

            string desc = "";
            foreach (User user in listUsers)
            {
                desc += user.discordUser.Username + ": " + user.currency + "\n";
            }

            DiscordEmbedBuilder message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorInfo),
                Title = "Top 10 users",
                Description = desc

            };
            await command.RespondAsync("", false, message);
        }
    }
}
