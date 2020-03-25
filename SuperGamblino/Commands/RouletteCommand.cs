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
    class RouletteCommand
    {
        private readonly Database _database;
        private readonly Messages _messages;
        private readonly Config _config;

        public RouletteCommand(Database database, Messages messages, Config config)
        {
            _database = database;
            _messages = messages;
            _config = config;
        }

        [Command("roulette")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("<Red|Black|Odd|Even|Number> <Bet>\n\nEx. roulette Red 100")]
        public async Task OnExecute(CommandContext command)
        {
            string[] argument = command.RawArgumentString != null ? command.RawArgumentString.ToUpper().TrimStart().Split(' ') : new string[0];
            bool isNumber = false;
            bool betIsNmb = false;
            int nmbBet = 0;
            int nmbGuess = 0;
            if (argument.Length >= 2)
            {
                isNumber = int.TryParse(argument[0], out nmbGuess);
                betIsNmb = int.TryParse(argument[1], out nmbBet);
            }
            bool enoughCredits = true;

            if (betIsNmb)
            {
                int curCred = await _database.CommandGetUserCredits(command.User.Id);
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
                    int rewardMulti = 0;
                    bool hasWon = false;
                    int credWon = 0;
                    bool invalid = false;
                    Random random = new Random();
                    int number = random.Next(0, 37);
                    int exp = number * 3 + 25;
                    AddExpResult expResult = await _database.CommandGiveUserExp(command, exp);
                    if (expResult.DidUserLevelUp)
                    {
                        await _messages.LevelUp(command);
                    }
                    Roulette.Result result = Roulette.GetResult(number);

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
                                await _messages.InvalidArgument(command, new string[] { "<Red|Black|Odd|Even|Number> <Bet>" });
                                invalid = true;
                                break;
                        }
                    }

                    string color = "";

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
                        default:
                            break;
                    }
                    string title = "";
                    if (hasWon)
                    {
                        title = "Roulette - You've won!";
                        credWon = Convert.ToInt32(argument[1]) * rewardMulti;
                        await _database.CommandGiveCredits(command.User.Id, credWon);
                    }
                    else
                        title = "Roulette - You've lost!";

                    DiscordEmbedBuilder message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(color),
                        Title = title,
                        Description =
                            $"You rolled {result.Color} {result.Number}.\nThe result is {result.OddOrEven}\n\nProfit: {credWon}\nExp: {exp}"
                    };
                    if (!invalid)
                    {
                        await command.RespondAsync("", false, message.WithFooter("Current credits: " + await _database.CommandGetUserCredits(command.User.Id) + "\nCurrent exp: " + expResult.CurrentExp + "/" + expResult.RequiredExp));
                    }
                }
            }
            else
                await _messages.InvalidArgument(command, new string[] { "<Red|Black|Odd|Even|Number> <Bet>" });
        }
    }
}
