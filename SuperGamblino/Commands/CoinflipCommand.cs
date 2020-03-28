using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.Commands
{
    internal class CoinflipCommand
    {
        private readonly Config _config;
        private readonly Database _database;
        private readonly Messages _messages;
        private readonly BetSizeParser _betSizeParser;

        public CoinflipCommand(Database database, Config config, Messages messages, BetSizeParser betSizeParser)
        {
            _database = database;
            _config = config;
            _messages = messages;
            _betSizeParser = betSizeParser;
        }

        [Command("coinflip")]
        [Aliases("cf")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Simple coinflip game. This command takes two argument Head or Tail and your bet. \nEx. coinflip head 100")]
        public async Task OnExecute(CommandContext command)
        {
            try
            {
                var argument = command.RawArgumentString.ToUpper().TrimStart().Split(' ');
                var option = argument[0] == "HEAD" || argument[0] == "TAIL" ? argument[0] : "";
                if (!string.IsNullOrWhiteSpace(option) && argument.Length == 2)
                {
                    int bet = -1;
                    if (argument[1].Trim().ToLower() == "all")
                    {
                        bet = await _database.CommandGetUserCredits(command.User.Id);
                    }else if (argument[1].Trim().ToLower() == "half")
                    {
                        bet = await _database.CommandGetUserCredits(command.User.Id) / 2;
                    }
                    else
                    {
                        bet = _betSizeParser.Parse(argument[1]);
                    }
                    if (bet == -1)
                    {
                        throw new NotImplementedException(); //Handle if bet is given in wrong format
                    }
                    if (await _database.CommandSubsctractCredits(command.User.Id, bet))
                    {
                        var rnd = new Random();
                        var result = Convert.ToBoolean(rnd.Next(0, 2)) ? "HEAD" : "TAIL";
                        var expResult = await _database.CommandGiveUserExp(command, 100);
                        if (expResult.DidUserLevelUp) await _messages.LevelUp(command);
                        DiscordEmbed resultMsg = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(_config.ColorSettings.Info),
                            Description = result
                        };
                        await command.RespondAsync("", false, resultMsg);

                        await _database.AddGameHistory(command.User.Id, new GameHistory()
                        {
                            GameName = "coinflip",
                            HasWon = result == option,
                            CoinsDifference = result == option ? bet : bet * -1
                        });
                        if (result == option)
                            await _messages.Won(command, bet, expResult);
                        else
                            await _messages.Lost(command);
                    }
                    else
                    {
                        await _messages.NotEnoughCredits(command);
                    }
                }
                else
                {
                    await _messages.InvalidArgument(command, new[] {"<Head|Tail>", "<Bet>"});
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    await _messages.InvalidArgument(command, new[] {"<Head|Tail>", "<Bet>"});
                }
                else if (ex.Message == "Input string was not in a correct format.")
                {
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(_config.ColorSettings.Warning),
                        Description = "Bets can only be whole numbers."
                    };
                    await command.RespondAsync("", false, message);
                }
                else
                {
                    Console.WriteLine(ex);
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(_config.ColorSettings.Warning),
                        Description = "This is unexpected..."
                    };
                    await command.RespondAsync("", false, message);
                }
            }
        }
    }
}