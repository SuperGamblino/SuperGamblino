using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino
{
    public class Messages
    {
        private const string DefaultTitle = "SuperGamblino";
        
        private readonly Config _config;
        private readonly UsersConnector _usersConnector;

        public Messages(Config config, UsersConnector usersConnector)
        {
            _config = config;
            _usersConnector = usersConnector;
        }

        public DiscordEmbedBuilder Success(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder Information(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder Warning(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder InvalidArguments(IEnumerable<string> arguments, string command, string title = DefaultTitle)
        {
            var desc = $"Invalid arguments!\nUse the following {command} ";
            desc = arguments.Aggregate(desc, (current, arg) => current + (arg + ",")).Remove(desc.Length - 1, 1);
            return Warning(desc, title);
        }

        public DiscordEmbedBuilder CommandCalledTooEarly(TimeSpan timeLeft, string command, string title = DefaultTitle)
        {
            return Warning(
                $"You've tried to execute command '{command}' before it was ready! Command will be ready in {timeLeft:hh\\:mm\\:ss}", title);
        }
        
        public DiscordEmbedBuilder WinInformation(int earnedCred, string title = DefaultTitle)
        {
            return Success($"You've won {earnedCred} credits!", title);
        }

        public DiscordEmbedBuilder LoseInformation(int bet, string title = DefaultTitle)
        {
            return Warning($"You've lost {bet} credits!",
                title);
        }
        
        
        public DiscordEmbedBuilder NotEnoughCredits(string title = DefaultTitle)
        {
            return Warning("This is a casino, not a bank!\nYou do not have enough credits!", title);
        }

        public DiscordEmbedBuilder AddExpInformation(DiscordEmbedBuilder message, AddExpResult expResult)
        {
            return message.AddField("EXP",$"{expResult.CurrentExp} / {expResult.RequiredExp}");
        }

        public DiscordEmbedBuilder AddCoinsBalanceInformation(DiscordEmbedBuilder message, int currentCred)
        {
            return message.AddField("Credits balance", currentCred.ToString());
        }

        public DiscordEmbedBuilder ListCurrentCooldowns(IEnumerable<CooldownObject> cooldownObjects, string title = DefaultTitle)
        {
            return Information(cooldownObjects.Aggregate("", (current, cooldown) => current + $"{cooldown.Command} : {cooldown.TimeLeft:hh\\:mm\\:ss}\n"), title);
        }

        public DiscordEmbedBuilder Error(string description)
        {
            return Warning(description, "Error Occured!!!");
        }

        public DiscordEmbedBuilder AddLevelUpMessage(DiscordEmbedBuilder message)
        {
            return message.WithFooter("You've gained a level!");
        }

        public DiscordEmbedBuilder NotVotedYet()
        {
            return Information(
                "To gain a vote reward, you have to use this link\n[Vote](https://top.gg/bot/688160933574475800/vote)",
                "You haven't voted yet!");
        }

        public DiscordEmbedBuilder CoinDropSuccessful(int reward, string collectorUsername)
        {
            return Success($"Congratulations, the CoinDrop has been collected!\n**Reward** {reward}", "CoinDropCollected")
                .WithFooter($"Collected by {collectorUsername}");
        }

        public DiscordEmbedBuilder CoinDropTooLate()
        {
            return Information("Sadly the CoinDrop has already been collected!", "CoinDropAlreadyCollected");
        }
        
        //Obsolete below!!!
        
        public async Task Won(CommandContext command, int currentCred, AddExpResult result)
        {
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
            var currentCred = await _usersConnector.CommandGetUserCredits(command.User.Id);
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

        public async Task NotVotedYet(CommandContext command)
        {
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "You haven't voted yet!",
                Description = "To gain a vote reward, you have to use this link\n" +
                "[Vote](https://top.gg/bot/688160933574475800/vote)"
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
                $"**Job cooldown: ** {job.Cooldown}\n" +
                $"**Minimum bet: ** {user.Level * 15}"
            };

            await command.RespondAsync("", false, message);
        }

        public async Task About(CommandContext command)
        {
           int guildCount = command.Client.Guilds.Count;
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "About",
                Description = $"Servers: {guildCount}"
            };
            await command.RespondAsync("", false, message);
        }

        public async Task CoinDropAlert(MessageCreateEventArgs e, int claimId)
        {
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Coin Drop!",
                Description = $"THERE HAS BEEN A COINDROP\n" +
                $"To collect this drop use the collect command with the following id\n" +
                $"**Claim ID:** {claimId}"
            };
            await e.Message.RespondAsync("", false, message);
        }

        public async Task CoindDropSuccess(CommandContext command, int reward)
        {
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Title = "Coin Drop Reward",
                Description = $"Congratulations, you won the coindrop!\n" +
                $"**Reward:** {reward}"
            };
            await command.RespondAsync("", false, 
                message.WithFooter($"{command.Message.Author.Username}"));
        }
        public async Task CoindDropLate(CommandContext command)
        {
            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Coin Drop Reward",
                Description = $"Sadly there was no coindrop for you!"
            };
            await command.RespondAsync("", false,
                message.WithFooter($"{command.Message.Author.Username}"));
        }

    }
}