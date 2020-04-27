using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class Cooldown
    {
        private readonly CooldownCommandLogic _cooldownCommandLogic;

        public Cooldown(CooldownCommandLogic cooldownCommandLogic)
        {
            _cooldownCommandLogic = cooldownCommandLogic;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows the current command cooldowns. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _cooldownCommandLogic.GetCooldowns(command.User.Id));
        }
    }
}