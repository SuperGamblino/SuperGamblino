using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SuperGamblino
{
    class Messages
    {
        private readonly Database _database;
        private readonly Config _config;

        public Messages(Database database, Config config)
        {
            _database = database;
            _config = config;
        }

        public async Task Won (CommandContext command, int bet)
        {
            int currentCred = await _database.CommandGiveCredits(command.User.Id, bet * 2);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = "You've won!\n\nCurrent credits: " + currentCred
            };
            await command.RespondAsync("", false, message);
        }
        public async Task Lost (CommandContext command)
        {
            int currentCred = await _database.CommandGetUserCredits(command.User.Id);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = "You've lost...\n\nCurrent credits: " + currentCred

            };
            await command.RespondAsync("", false, message);
        }
        public async Task NotEnoughCredits (CommandContext command)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = "This is a casino, not a bank!\n\nYou do not have enough credits."

            };
            await command.RespondAsync("", false, message);
        }
        public async Task InvalidArgument (CommandContext command, string[] arguments)
        {
            
            string desc = "Invalid arugment.\nUse the following " +  command.Command.ToString() + " ";
            foreach (string arg in arguments)
            {
                desc += arg + ",";
            }
            desc = desc.Remove(desc.Length - 1, 1);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = desc
            };
            await command.RespondAsync("", false, message);
        }

        public async Task CoinsGain(CommandContext command, int ammount)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = "You've gained: " + ammount + " coins!"
            };
            await command.RespondAsync("",false, message);
        }

        public async Task TooEarly(CommandContext command, TimeSpan timeLeft)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "Sorry but it looks like you've used this command recently! To use it again please wait: "
                              + timeLeft.ToString(@"hh\:mm\:ss") + "."
            };
            await command.RespondAsync("",false, message);
        }

        public async Task Error(CommandContext command, string description)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Title = "Something went wrong!!!",
                Description = description
            };
            await command.RespondAsync("",false, message);
        }
    }
}
