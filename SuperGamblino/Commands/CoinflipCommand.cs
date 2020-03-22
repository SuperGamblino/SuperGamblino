using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System.Reflection;

namespace SuperGamblino.Commands
{
    class CoinflipCommand
    {
        [Command("coinflip")]
        [Aliases("cf")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            try
            {
                string[] argument = command.RawArgumentString.ToUpper().TrimStart().Split(' ');
                string option = argument[0] == "HEAD" || argument[0] == "TAIL" ? argument[0] : "";
                if (!String.IsNullOrWhiteSpace(option) && argument.Length == 2)
                {

                    bool isNumeric = int.TryParse(argument[1], out int bet);

                    if (await Database.CommandSubsctractCredits(command.User.Id, bet))
                    {

                        Random rnd = new Random();
                        string result = Convert.ToBoolean(rnd.Next(0, 2)) ? "HEAD" : "TAIL";

                        DiscordEmbed resultMsg = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(Config.colorInfo),
                            Description = result
                        };
                        await command.RespondAsync("", false, resultMsg);


                        if (result == option)
                        {
                            await Messages.Won(command, bet);
                        }
                        else
                        {
                            await Messages.Lost(command);
                        }
                    }
                    else
                    {
                        await Messages.NotEnoughCredits(command);
                    }

                }
                else
                {
                    await Messages.InvalidArgument(command, new string[] {"<Head|Tail>", "<Bet>"});
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    await Messages.InvalidArgument(command, new string[] { "<Head|Tail>", "<Bet>" });
                }
                else if (ex.Message == "Input string was not in a correct format.")
                {
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(Config.colorWarning),
                        Description = "Bets can only be whole numbers."
                    };
                    await command.RespondAsync("", false, message);
                }
                else
                {
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(Config.colorWarning),
                        Description = "This is unexpected..."
                    };
                    await command.RespondAsync("", false, message);
                }
            }
        }
    }
}
