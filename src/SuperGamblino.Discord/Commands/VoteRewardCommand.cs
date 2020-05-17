using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class VoteRewardCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.VoteRewardCommand _logic;

        public VoteRewardCommand(SuperGamblino.Commands.Commands.VoteRewardCommand logic)
        {
            _logic = logic;
        }

        [Command("vote")]
        [Description("This command gives you 400 credits. It's usable every 12th hour.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.Vote(command.User.Id).ToDiscordEmbed());
        }
    }
}