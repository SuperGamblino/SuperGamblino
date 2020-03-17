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
        [Command("roulette")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Description("<Red|Black|Odd|Even|Number> <Bet>\n\nEx. roulette Red 100")]
        public async Task OnExecute(CommandContext command)
        {
            string[] argument = command.RawArgumentString.ToUpper().TrimStart().Split(' ');
            bool isNumber = int.TryParse(argument[0], out int nmbGuess);
            bool betIsNmb = int.TryParse(argument[1], out int nmbBet);
            bool enoughCredits = true;

            if (betIsNmb)
            {
                int curCred = Database.CommandGetUserCredits(command.User.Id);
                if (curCred < nmbBet)
                {
                    enoughCredits = false;
                    await Messages.NotEnoughCredits(command);
                }
                else
                {
                    Database.CommandSubsctractCredits(command.User.Id, nmbBet);
                }
                if (enoughCredits)
                {
                    int rewardMulti = 0;
                    bool hasWon = false;
                    int credWon = 0;
                    bool invalid = false;
                    Random random = new Random();
                    int number = random.Next(0, 37);

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
                                await Messages.InvalidArgument(command, new string[] { "<Red|Black|Odd|Even|Number> <Bet>" });
                                invalid = true;
                                break;
                        }
                    }

                    string color = "";

                    switch (result.Color)
                    {
                        case "Red":
                            color = Config.colorWarning;
                            break;
                        case "Black":
                            color = "#000000";
                            break;
                        case "Green":
                            color = Config.colorSuccess;
                            break;
                        default:
                            break;
                    }
                    string title = "";
                    if (hasWon)
                    {
                        title = "Roulette - You've won!";
                        credWon = Convert.ToInt32(argument[1]) * rewardMulti;
                        Database.CommandGiveCredits(command.User.Id, credWon);
                    }
                    else
                        title = "Roulette - You've lost!";

                    DiscordEmbedBuilder message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(color),
                        Title = title,
                        Description = string.Format("You rolled {0} {1}.\nThe result is {2}\n\nProfit: {3}", result.Color, result.Number, result.OddOrEven, credWon)
                    };
                    if (!invalid)
                    {
                        await command.RespondAsync("", false, message.WithFooter("Current credits: " + Database.CommandGetUserCredits(command.User.Id).ToString()));
                    }
                }
            }
            else
                await Messages.InvalidArgument(command, new string[] { "<Red|Black|Odd|Even|Number> <Bet>" });
        }
    }
}
