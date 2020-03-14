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

                    int bet = Convert.ToInt32(argument[1]);
                    if (Database.CommandSubsctractCredits(command.User.Id, bet))
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
                            DiscordEmbed message = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(Config.colorSuccess),
                                Description = "You've won!"
                            };
                            Database.CommandGiveCredits(command.User.Id, bet * 2);
                            await command.RespondAsync("", false, message);
                        }
                        else
                        {
                            DiscordEmbed message = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(Config.colorWarning),
                                Description = "You've lost..."

                            };
                            await command.RespondAsync("", false, message);
                        }
                    }
                    else
                    {
                        DiscordEmbed message = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(Config.colorWarning),
                            Description = "This is a casino, not a bank!"

                        };
                        await command.RespondAsync("", false, message);
                    }

                }
                else
                {
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(Config.colorWarning),
                        Description = "Invalid arugment.\nValid arguments are: 'Head' & 'Tail'"
                    };
                    await command.RespondAsync("", false, message);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    DiscordEmbed message = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(Config.colorWarning),
                        Description = "Invalid arugment.\nValid arguments are: 'Head' & 'Tail'"
                    };
                    await command.RespondAsync("", false, message);
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
