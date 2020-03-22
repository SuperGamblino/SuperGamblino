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
        private readonly Config _config;
        private readonly Database _database;

        public CreditsCommand(Config config, Database database)
        {
            _config = config;
            _database = database;
        }

        [Command("credits")]
        [Aliases("cred")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            int currentCredits = await _database.CommandGetUserCredits(command.User.Id);

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "You currently have:\n" + currentCredits + " credits"
            };
            await command.RespondAsync("", false, message);
        }
    }
}
