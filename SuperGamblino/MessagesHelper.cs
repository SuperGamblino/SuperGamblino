using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino
{
    public class MessagesHelper
    {
        private const string DefaultTitle = "SuperGamblino";

        private readonly Config _config;

        public MessagesHelper(Config config)
        {
            _config = config;
        }

        public DiscordEmbedBuilder Success(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Success),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder Information(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder Warning(string description, string title = DefaultTitle)
        {
            return new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Warning),
                Description = description,
                Title = title
            };
        }

        public DiscordEmbedBuilder InvalidArguments(IEnumerable<string> arguments, string command,
            string title = DefaultTitle)
        {
            var desc = $"Invalid arguments!\nUse the following {command} ";
            desc = arguments.Aggregate(desc, (current, arg) => current + arg + ",").Remove(desc.Length - 1, 1);
            return Warning(desc, title);
        }

        public DiscordEmbedBuilder CommandCalledTooEarly(TimeSpan timeLeft, string command, string title = DefaultTitle)
        {
            return Warning(
                $"You've tried to execute command '{command}' before it was ready! Command will be ready in {timeLeft:hh\\:mm\\:ss}",
                title);
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
            return expResult.DidUserLevelUp
                ? AddLevelUpMessage(message.AddField("EXP", $"{expResult.CurrentExp} / {expResult.RequiredExp}"))
                : message.AddField("EXP", $"{expResult.CurrentExp} / {expResult.RequiredExp}");
        }

        public DiscordEmbedBuilder AddCoinsBalanceInformation(DiscordEmbedBuilder message, int currentCred)
        {
            return message.AddField("Credits balance", currentCred.ToString());
        }

        public DiscordEmbedBuilder AddCoinsBalanceAndExpInformation(DiscordEmbedBuilder message, AddExpResult expResult,
            int currentCred)
        {
            return AddExpInformation(AddCoinsBalanceInformation(message, currentCred), expResult);
        }

        public DiscordEmbedBuilder ListCurrentCooldowns(IEnumerable<CooldownObject> cooldownObjects,
            string title = DefaultTitle)
        {
            return Information(
                cooldownObjects.Aggregate("",
                    (current, cooldown) => current + $"{cooldown.Command} : {cooldown.TimeLeft:hh\\:mm\\:ss}\n"),
                title);
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
            return Success($"Congratulations, the CoinDrop has been collected!\n**Reward** {reward}",
                    "CoinDropCollected")
                .WithFooter($"Collected by {collectorUsername}");
        }

        public DiscordEmbedBuilder CoinDropTooLate()
        {
            return Information("Sadly the CoinDrop has already been collected!", "CoinDropAlreadyCollected");
        }

        public DiscordEmbedBuilder CreditsGain(int amount, string title)
        {
            return Success($"You've gained {amount} credits!", title);
        }

        public DiscordEmbedBuilder GetProfile(User user, Job job, string username)
        {
            return Information($"**Credits: **{user.Credits}\n" +
                               $"**Level: **{user.Level}\n" +
                               $"**Current exp: **{user.Experience}\n" +
                               $"**Job title: **{job.Title}\n" +
                               $"**Job salery: ** {job.Reward}\n" +
                               $"**Job cooldown: ** {job.Cooldown}\n" +
                               $"**Minimum bet: ** {user.Level * 15}", $"User profile - {username}");
        }

        public DiscordEmbed CoinDropAlert(int claimId)
        {
            return Information("THERE HAS BEEN A COINDROP\n" +
                               "To collect this drop use the collect command with the following id\n" +
                               $"**Claim ID:** {claimId}", "CoinDrop!");
        }
    }
}