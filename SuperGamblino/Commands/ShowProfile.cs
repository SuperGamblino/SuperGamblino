using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class ShowProfile
    {
        private readonly ShowProfileCommandLogic _logic;

        public ShowProfile(ShowProfileCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("profile")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a profile, descriping detailed information about the user. This command has no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.ShowProfile(command.User.Id, command.User.Username));
        }
    }
}