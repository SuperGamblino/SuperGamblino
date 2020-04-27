using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class VoteReward
    {
        private readonly VoteRewardCommandLogic _logic;

        public VoteReward(VoteRewardCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("vote")]
        [Description("This command gives you 400 credits. It's usable every 12th hour.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.Vote(command.User.Id));
        }
    }
}