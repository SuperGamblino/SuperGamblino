using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SuperGamblino.Discord.Commands
{
    internal class CooldownCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.CooldownCommand _cooldownCommand;

        public CooldownCommand(SuperGamblino.Commands.Commands.CooldownCommand cooldownCommand)
        {
            _cooldownCommand = cooldownCommand;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows the current command cooldowns. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false,
                await _cooldownCommand.GetCooldowns(command.User.Id).ToDiscordEmbed());
        }
    }
}