using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino
{
    internal class Messages
    {
        private readonly Config _config;
        private readonly Database _database;

        public Messages(Database database, Config config)
        {
            _database = database;
            _config = config;
        }

        public async Task Won(CommandContext command, int bet, AddExpResult result)
        {
            var currentCred = await _database.CommandGiveCredits(command.User.Id, bet * 2);
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = "You've won!\n\nCurrent credits: " + currentCred
            };
            await command.RespondAsync("", false,
                message.WithFooter(string.Format("EXP: {0}/{1}", result.CurrentExp, result.RequiredExp)));
        }

        public async Task Lost(CommandContext command)
        {
            var currentCred = await _database.CommandGetUserCredits(command.User.Id);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = "You've lost...\n\nCurrent credits: " + currentCred
            };
            await command.RespondAsync("", false, message);
        }

        public async Task NotEnoughCredits(CommandContext command)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = "This is a casino, not a bank!\n\nYou do not have enough credits."
            };
            await command.RespondAsync("", false, message);
        }

        public async Task InvalidArgument(CommandContext command, string[] arguments)
        {
            var desc = "Invalid arugment.\nUse the following " + command.Command + " ";
            foreach (var arg in arguments) desc += arg + ",";
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
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = "You've gained: " + ammount + " coins!"
            };
            await command.RespondAsync("", false, message);
        }


        public async Task TooEarly(CommandContext command, TimeSpan timeLeft)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "Sorry but it looks like you've used this command recently! To use it again please wait: "
                              + timeLeft.ToString(@"hh\:mm\:ss") + "."
            };
            await command.RespondAsync("", false, message);
        }

        public async Task ListCooldown(CommandContext command, List<CooldownObject> cooldowns)
        {
            var desc = "";
            foreach (var cooldown in cooldowns)
                desc += string.Format("{0} : {1}\n", cooldown.Command, cooldown.TimeLeft.ToString(@"hh\:mm\:ss"));

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "Current commands are on cooldown:\n\n"
                              + desc
            };
            await command.RespondAsync("", false, message);
        }

        public async Task Error(CommandContext command, string description)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Title = "Something went wrong!!!",
                Description = description
            };
            await command.RespondAsync("", false, message);
        }

        public async Task LevelUp(CommandContext command)
        {
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Title = "Level Up!",
                Description = "You've gained a level!"
            };
            await command.RespondAsync("", false, message);
        }

        public async Task Profile(CommandContext command, User user, Job job)
        {

            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = $"User profile - {command.User.Username}",
                Description = $"**Credits: **{user.Credits}\n" +
                $"**Level: **{user.Level}\n" +
                $"**Current exp: **{user.Experience}\n" +
                $"**Job title: **{job.Title}\n" +
                $"**Job salery: ** {job.Reward}\n" +
                $"**Job cooldown: ** {job.Cooldown}"
            };

            await command.RespondAsync("", false, message);
        }
    }
}