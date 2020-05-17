using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class AboutCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.AboutCommand _aboutCommand;

        public AboutCommand(SuperGamblino.Commands.Commands.AboutCommand aboutCommand)
        {
            _aboutCommand = aboutCommand;
        }

        [Command("about")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows how many server the bot has been added to.")]
        public async Task OnExecute(CommandContext command)
        {
            var guildCount = command.Client.Guilds.Count;
            await command.RespondAsync("", false, _aboutCommand.GetAboutInfo(guildCount).ToDiscordEmbed());
        }
    }
}