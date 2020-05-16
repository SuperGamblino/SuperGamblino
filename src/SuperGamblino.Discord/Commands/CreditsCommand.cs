using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class CreditsCommand : BaseCommandModule
    {
        private readonly CreditsCommandLogic _logic;

        public CreditsCommand(CreditsCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("credits")]
        [Aliases("cred")]
        [Description("Shows your current credits. This command has no arguments.")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetCurrentCreditStatus(command.User.Id).ToDiscordEmbed());
        }
    }
}