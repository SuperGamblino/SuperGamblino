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
                string argument = "";
                argument = command.RawArgumentString.ToUpper().Trim();

                if (argument == "HEAD" || argument == "TAIL" && !String.IsNullOrWhiteSpace(argument))
                {
                    Random rnd = new Random();
                    string result = Convert.ToBoolean(rnd.Next(0, 2)) ? "HEAD" : "TAIL";

                    DiscordEmbed resultMsg = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(Config.colorInfo),
                        Description = result
                    };
                    await command.RespondAsync("", false, resultMsg);


                    if (result == argument)
                    {
                        DiscordEmbed message = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(Config.colorSuccess),
                            Description = "You've won!"
                        };
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
