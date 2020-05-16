using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class CooldownCommand : BaseCommandModule
    {
        private readonly CooldownCommandLogic _cooldownCommandLogic;

        public CooldownCommand(CooldownCommandLogic cooldownCommandLogic)
        {
            _cooldownCommandLogic = cooldownCommandLogic;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows the current command cooldowns. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _cooldownCommandLogic.GetCooldowns(command.User.Id).ToDiscordEmbed());
        }
    }
}