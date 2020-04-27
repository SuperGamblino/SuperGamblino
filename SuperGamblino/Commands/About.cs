using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class About
    {
        private readonly AboutCommandLogic _aboutCommandLogic;

        public About(AboutCommandLogic aboutCommandLogic)
        {
            _aboutCommandLogic = aboutCommandLogic;
        }

        [Command("about")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows how many server the bot has been added to.")]
        public async Task OnExecute(CommandContext command)
        {
            var guildCount = command.Client.Guilds.Count;
            await command.RespondAsync("", false, _aboutCommandLogic.GetAboutInfo(guildCount));
        }
    }
}