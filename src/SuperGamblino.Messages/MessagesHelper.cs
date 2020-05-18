using System;
using System.Collections.Generic;
using System.Linq;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Entities;
using SuperGamblino.Core.GamesObjects;

namespace SuperGamblino.Messages
{
    public class MessagesHelper
    {
        private const string DefaultTitle = "SuperGamblino";

        private readonly Config _config;

        public MessagesHelper(Config config)
        {
            _config = config;
        }

        public Message Success(string description, string title = DefaultTitle)
        {
            return new Message()
            {
                Color = _config.ColorSettings.Success,
                Description = description,
                Title = title
            };
        }

        public Message Information(string description, string title = DefaultTitle)
        {
            return new Message()
            {
                Color = _config.ColorSettings.Info,
                Description = description,
                Title = title
            };
        }

        public Message Warning(string description, string title = DefaultTitle)
        {
            return new Message()
            {
                Color = _config.ColorSettings.Warning,
                Description = description,
                Title = title
            };
        }

        public Message InvalidArguments(IEnumerable<string> arguments, string command, string title = DefaultTitle)
        {
            var desc = $"Invalid arguments!\nUse the following {command}: ";
            desc = new string(arguments.Aggregate(desc, (current, arg) => current + arg + " ").SkipLast(1).ToArray());
            return Warning(desc, title);
        }

        public Message CommandCalledTooEarly(TimeSpan? timeLeft, string command, string title = DefaultTitle)
        {
            return Warning(
                $"You've tried to execute command '{command}' before it was ready! Command will be ready in {timeLeft:hh\\:mm\\:ss}",
                title);
        }

        public Message WinInformation(int earnedCred, string info = null, string title = DefaultTitle)
        {
            return Success($"{info}\nYou've won {earnedCred} credits!", title);
        }

        public Message LoseInformation(int bet, string info = null, string title = DefaultTitle)
        {
            return Warning($"{info}\nYou've lost {bet} credits!",
                title);
        }


        public Message NotEnoughCredits(string title = DefaultTitle)
        {
            return Warning("This is a casino, not a bank!\nYou do not have enough credits!", title);
        }

        public Message AddExpInformation(Message message, AddExpResult expResult)
        {
            return expResult.DidUserLevelUp
                ? AddLevelUpMessage(message.AddField("EXP", $"{expResult.CurrentExp} / {expResult.RequiredExp}"))
                : message.AddField("EXP", $"{expResult.CurrentExp} / {expResult.RequiredExp}");
        }

        public Message AddCoinsBalanceInformation(Message message, int currentCred)
        {
            return message.AddField("Credits balance", currentCred.ToString());
        }

        public Message AddCoinsBalanceAndExpInformation(Message message, AddExpResult expResult,
            int currentCred)
        {
            return AddExpInformation(AddCoinsBalanceInformation(message, currentCred), expResult);
        }

        public Message ListCurrentCooldowns(IEnumerable<CooldownObject> cooldownObjects,
            string title = DefaultTitle)
        {
            return Information(
                cooldownObjects.Aggregate("",
                    (current, cooldown) => current + $"{cooldown.Command} : {cooldown.TimeLeft:hh\\:mm\\:ss}\n"),
                title);
        }

        public Message Error(string description)
        {
            return Warning(description, "Error Occured!!!");
        }

        public Message AddLevelUpMessage(Message message)
        {
            return message.WithFooter("You've gained a level!");
        }

        public Message NotVotedYet()
        {
            return Information(
                "To gain a vote reward, you have to use this link\n[Vote](https://top.gg/bot/688160933574475800/vote)",
                "You haven't voted yet!");
        }

        public Message CoinDropSuccessful(int reward, string collectorUsername)
        {
            return Success($"Congratulations, the CoinDrop has been collected!\n**Reward** {reward}",
                    "CoinDropCollected")
                .WithFooter($"Collected by {collectorUsername}");
        }

        public Message CoinDropTooLate()
        {
            return Information("Sadly the CoinDrop has already been collected!", "CoinDropAlreadyCollected");
        }

        public Message CreditsGain(int amount, string title)
        {
            return Success($"You've gained {amount} credits!", title);
        }

        public Message GetProfile(User user, Work.Job job, string username)
        {
            return Information($"**Credits: ** {user.Credits}\n" +
                               $"**Level: ** {user.Level}\n" +
                               $"**Current exp: ** {user.Experience}\n" +
                               $"**Job title: ** {job.Title}\n" +
                               $"**Job salary: ** {job.Reward}\n" +
                               $"**Job cooldown: ** {job.Cooldown}\n" +
                               $"**Minimum bet: ** {user.Level * 15}", $"User profile - {username}");
        }

        public Message CoinDropAlert(int claimId)
        {
            return Information("THERE HAS BEEN A COINDROP\n" +
                               "To collect this drop use the collect command with the following id\n" +
                               $"**Claim ID:** {claimId}", "CoinDrop!");
        }
    }
}