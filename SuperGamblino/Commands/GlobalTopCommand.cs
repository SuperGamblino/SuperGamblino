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
        private readonly Database _database;
        private readonly Config _config;

        public GlobalTopCommand(Database database, Config config)
        {
            _database = database;
            _config = config;
        }

        [Command("globaltop")]
        [Aliases("gt")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
           List<User> listUsers = await _database.CommandGetGlobalTop(command);

            string desc = "";
            foreach (User user in listUsers)
            {
                desc += user.DiscordUser.Username + ": " + user.Credits + "\n";
            }

            DiscordEmbedBuilder message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Top 10 users",
                Description = desc

            };
            await command.RespondAsync("", false, message);
        }
    }
}
