using System;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Entities;
using SuperGamblino.Core.GamesObjects;
using Xunit;

namespace SuperGamblino.Messages.Tests
{
    public class MessagesHelperTests
    {
        private MessagesHelper GetMessagesHelper()
        {
            return new MessagesHelper(new Config()
            {
                ColorSettings = new ColorSettings()
                {
                    Info = "INFO",
                    Success = "SUCCESS",
                    Warning = "WARNING"
                }
            });    
        }
        
        [Fact]
        public void IsSuccessFormattedCorrectly()
        {
            var result = GetMessagesHelper().Success("Example success information!", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example success information!", result.Description);
            Assert.Equal("SUCCESS", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsInformationFormattedCorrectly()
        {
            var result = GetMessagesHelper().Information("Example informative information!", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example informative information!", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsWarningFormattedCorrectly()
        {
            var result = GetMessagesHelper().Warning("Example warning information!", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example warning information!", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsInvalidArgumentsFormattedCorrectly()
        {
            var result = GetMessagesHelper().InvalidArguments(new []{"<HEAD|TAIL>", "<BET>"},"example-command", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Invalid arguments!\nUse the following example-command: <HEAD|TAIL> <BET>", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsCommandCalledTooEarlyFormattedCorrectly()
        {
            var result = GetMessagesHelper()
                .CommandCalledTooEarly(TimeSpan.FromHours(5), "time-based-command", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("You've tried to execute command 'time-based-command' before it was ready! Command will be ready in 05:00:00", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsWinInformationFormattedCorrectly()
        {
            var result = GetMessagesHelper()
                .WinInformation(100, "Example info about the winned game.", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example info about the winned game.\nYou've won 100 credits!", result.Description);
            Assert.Equal("SUCCESS", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsLoseInformationFormattedCorrectly()
        {
            var result = GetMessagesHelper().LoseInformation(100, "Example info about the losed game.", "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example info about the losed game.\nYou've lost 100 credits!", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsNotEnoughCreditsFormattedCorrectly()
        {
            var result = GetMessagesHelper().NotEnoughCredits("Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("This is a casino, not a bank!\nYou do not have enough credits!", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
        
        [Fact]
        public void IsAddExpInformationWorkingCorrectlyWhenUserLeveledUp()
        {
            var messagesHelper = GetMessagesHelper();
            var result = messagesHelper.AddExpInformation(messagesHelper.Information("Example Information", "Example Title"), new AddExpResult(true, 100, 20, 300));
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example Information", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.Equal("You've gained a level!", result.Footer);
            Assert.Single(result.Fields);
            Assert.Contains(result.Fields, field => field.Name == "EXP" && field.Value == "20 / 100");
        }
        
        [Fact]
        public void IsAddExpInformationWorkingCorrectlyWhenUserDidNotLevelUp()
        {
            var messagesHelper = GetMessagesHelper();
            var result = messagesHelper.AddExpInformation(messagesHelper.Information("Example Information", "Example Title"), new AddExpResult(false, 100, 20, 300));
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example Information", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Single(result.Fields);
            Assert.Contains(result.Fields, field => field.Name == "EXP" && field.Value == "20 / 100");
        }

        [Fact]
        public void IsAddCoinsBalanceInformationFormattedCorrectly()
        {
            var messageHelper = GetMessagesHelper();
            var result = messageHelper.AddCoinsBalanceInformation(messageHelper.Information("Example Information", "Example Title"),100);
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example Information", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Single(result.Fields);
            Assert.Contains(result.Fields, field => field.Name == "Credits balance" && field.Value == "100");
        }

        [Fact]
        public void IsAddCoinsBalanceAndExpInformationBeingFormattedCorrectlyWhenUserLeveledUp()
        {
            var messagesHelper = GetMessagesHelper();
            var result = messagesHelper.AddCoinsBalanceAndExpInformation(messagesHelper.Information("Example Information", "Example Title"), new AddExpResult(true, 100, 20, 300), 100);
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example Information", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.Equal("You've gained a level!", result.Footer);
            Assert.Equal(2,result.Fields.Count);
            Assert.Contains(result.Fields, field => field.Name == "EXP" && field.Value == "20 / 100");
            Assert.Contains(result.Fields, field => field.Name == "Credits balance" && field.Value == "100");
        }
        
        [Fact]
        public void IsAddCoinsBalanceAndExpInformationBeingFormattedCorrectlyWhenUserDidNotLevelUp()
        {
            var messagesHelper = GetMessagesHelper();
            var result = messagesHelper.AddCoinsBalanceAndExpInformation(messagesHelper.Information("Example Information", "Example Title"), new AddExpResult(false, 100, 20, 300), 100);
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example Information", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Equal(2,result.Fields.Count);
            Assert.Contains(result.Fields, field => field.Name == "EXP" && field.Value == "20 / 100");
            Assert.Contains(result.Fields, field => field.Name == "Credits balance" && field.Value == "100");
        }

        [Fact]
        public void IsListCurrentCooldownsFormattedCorrectly()
        {
            var result = GetMessagesHelper().ListCurrentCooldowns(
                new[]
                {
                    new CooldownObject("test1", TimeSpan.FromHours(3)),
                    new CooldownObject("test2", TimeSpan.FromHours(1))
                }, "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("test1 : 03:00:00\ntest2 : 01:00:00\n", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsErrorFormattedCorrectly()
        {
            var result = GetMessagesHelper().Error("Example explanation.");
            
            Assert.Equal("Error Occured!!!", result.Title);
            Assert.Equal("Example explanation.", result.Description);
            Assert.Equal("WARNING", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsAddLevelUpMessageFormattedCorrectly()
        {
            var messageHelper = GetMessagesHelper();
            var result = messageHelper.AddLevelUpMessage(messageHelper.Information("Example informative information!", "Example Title"));
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("Example informative information!", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.Equal("You've gained a level!", result.Footer);
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsNotVotedYetFormattedCorrectly()
        {
            var result = GetMessagesHelper().NotVotedYet();
            
            Assert.Equal("You haven't voted yet!", result.Title);
            Assert.Equal("To gain a vote reward, you have to use this link\n[Vote](https://top.gg/bot/688160933574475800/vote)", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsCoinDropSuccessfulFormattedCorrectly()
        {
            var result = GetMessagesHelper().CoinDropSuccessful(100, "user");
            
            Assert.Equal("CoinDropCollected", result.Title);
            Assert.Equal("Congratulations, the CoinDrop has been collected!\n**Reward** 100", result.Description);
            Assert.Equal("SUCCESS", result.Color);
            Assert.Equal("Collected by user", result.Footer);
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsCoinDropTooLateFormattedCorrectly()
        {
            var result = GetMessagesHelper().CoinDropTooLate();
            
            Assert.Equal("CoinDropAlreadyCollected", result.Title);
            Assert.Equal("Sadly the CoinDrop has already been collected!", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsCreditsGainFormattedCorrectly()
        {
            var result = GetMessagesHelper().CreditsGain(100, "Example Title");
            
            Assert.Equal("Example Title", result.Title);
            Assert.Equal("You've gained 100 credits!", result.Description);
            Assert.Equal("SUCCESS", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsGetProfileFormattedCorrectly()
        {
            var result = GetMessagesHelper().GetProfile(new User()
            {
                Credits = 100,
                Experience = 110,
                Level = 10
            }, new Work.Job()
            {
                Level = 3,
                Reward = 100,
                Cooldown = 10,
                Title = "Example Job"
            }, "user");
            
            Assert.Equal("User profile - user", result.Title);
            Assert.Equal($"**Credits: ** 100\n" +
                         $"**Level: ** 10\n" +
                         $"**Current exp: ** 110\n" +
                         $"**Job title: ** Example Job\n" +
                         $"**Job salary: ** 100\n" +
                         $"**Job cooldown: ** 10\n" +
                         $"**Minimum bet: ** 150", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }

        [Fact]
        public void IsCoinDropAlertFormattedCorrectly()
        {
            var result = GetMessagesHelper().CoinDropAlert(4230);
            
            Assert.Equal("CoinDrop!", result.Title);
            Assert.Equal("THERE HAS BEEN A COINDROP\n" +
                         "To collect this drop use the collect command with the following id\n" +
                         $"**Claim ID:** 4230", result.Description);
            Assert.Equal("INFO", result.Color);
            Assert.True(string.IsNullOrWhiteSpace(result.Footer));
            Assert.Empty(result.Fields);
        }
    }
}