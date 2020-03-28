using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.Commands
{
    internal class RouletteCommand
    {
        private readonly Config _config;
        private readonly Database _database;
        private readonly Messages _messages;
        private readonly BetSizeParser _betSizeParser;

        public RouletteCommand(Database database, Messages messages, Config config, BetSizeParser betSizeParser)
        {
            _database = database;
            _messages = messages;
            _config = config;
            _betSizeParser = betSizeParser;
        }

        [Command("roulette")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("<Red|Black|Odd|Even|Number> <Bet>\n\nEx. roulette Red 100")]
        public async Task OnExecute(CommandContext command)
        {
            var argument = command.RawArgumentString != null
                ? command.RawArgumentString.ToUpper().TrimStart().Split(' ')
                : new string[0];
            var isNumber = false;
            var nmbBet = -1;
            var nmbGuess = 0;
            if (argument.Length >= 2)
            {
                isNumber = int.TryParse(argument[0], out nmbGuess);
                if (argument[1].Trim() == "ALL")
                {
                    nmbBet = await _database.CommandGetUserCredits(command.User.Id);
                } else if (argument[1].Trim() == "HALF")
                {
                    nmbBet = await _database.CommandGetUserCredits(command.User.Id) / 2;
                }
                else
                {
                    nmbBet = _betSizeParser.Parse(argument[1]);
                }
            }

            var enoughCredits = true;

            if (nmbBet != -1)
            {
                var curCred = await _database.CommandGetUserCredits(command.User.Id);
                if (curCred < nmbBet)
                {
                    enoughCredits = false;
                    await _messages.NotEnoughCredits(command);
                }
                else
                {
                    await _database.CommandSubsctractCredits(command.User.Id, nmbBet);
                }

                if (enoughCredits)
                {
                    var rewardMulti = 0;
                    var hasWon = false;
                    var credWon = 0;
                    var invalid = false;
                    var random = new Random();
                    var number = random.Next(0, 37);
                    var exp = number * 3 + 25;
                    var expResult = await _database.CommandGiveUserExp(command, exp);
                    if (expResult.DidUserLevelUp) await _messages.LevelUp(command);
                    var result = Roulette.GetResult(number);

                    if (isNumber)
                    {
                        if (nmbGuess == result.Number)
                        {
                            hasWon = true;
                            rewardMulti = 36;
                        }
                    }
                    else
                    {
                        switch (argument[0].ToUpper())
                        {
                            case "RED":
                                if (result.Color.ToUpper() == "RED")
                                {
                                    hasWon = true;
                                    rewardMulti = 2;
                                }

                                break;
                            case "BLACK":
                                if (result.Color.ToUpper() == "BLACK")
                                {
                                    hasWon = true;
                                    rewardMulti = 2;
                                }

                                break;
                            case "ODD":
                                if (result.OddOrEven.ToUpper() == "ODD")
                                {
                                    hasWon = true;
                                    rewardMulti = 2;
                                }

                                break;
                            case "EVEN":
                                if (result.OddOrEven.ToUpper() == "EVEN")
                                {
                                    hasWon = true;
                                    rewardMulti = 2;
                                }

                                break;
                            default:
                                await _messages.InvalidArgument(command, new[] {"<Red|Black|Odd|Even|Number> <Bet>"});
                                invalid = true;
                                break;
                        }
                    }

                    var color = "";

                    switch (result.Color)
                    {
                        case "Red":
                            color = _config.ColorSettings.Warning;
                            break;
                        case "Black":
                            color = "#000000";
                            break;
                        case "Green":
                            color = _config.ColorSettings.Success;
                            break;
                    }

                    var title = "";
                    if (hasWon)
                    {
                        title = "Roulette - You've won!";
                        credWon = nmbBet * rewardMulti;
                        await _database.CommandGiveCredits(command.User.Id, credWon);
                    }
                    else
                    {
                        title = "Roulette - You've lost!";
                    }

                    await _database.AddGameHistory(command.User.Id, new GameHistory()
                    {
                        GameName = "roulette",
                        HasWon = hasWon,
                        CoinsDifference = credWon
                    });
                    
                    var message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(color),
                        Title = title,
                        Description =
                            $"You rolled {result.Color} {result.Number}.\nThe result is {result.OddOrEven}\n\nProfit: {credWon}\nExp: {exp}"
                    };
                    if (!invalid)
                        await command.RespondAsync("", false,
                            message.WithFooter("Current credits: " +
                                               await _database.CommandGetUserCredits(command.User.Id) +
                                               "\nCurrent exp: " + expResult.CurrentExp + "/" + expResult.RequiredExp));
                }
            }
            else
            {
                await _messages.InvalidArgument(command, new[] {"<Red|Black|Odd|Even|Number> <Bet>"});
            }
        }
    }
}