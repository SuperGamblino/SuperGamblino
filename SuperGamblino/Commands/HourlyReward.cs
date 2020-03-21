using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    public class HourlyReward
    {
        [Command("hourly")]
        [Aliases("get-hourly")]
        [Cooldown(1, 3600, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 20;
            Database.CommandGiveCredits(command.User.Id, reward);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorSuccess),
                Description = "You've gained: " + reward + " coins!"
            };
            await command.RespondAsync("",false, message);
        }
    }
}