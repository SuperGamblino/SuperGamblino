using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Messages;

namespace SuperGamblino.Discord.Commands
{
    public class CountDEBUGCommand : BaseCommandModule
    {
        private SuperGamblino.Commands.Commands.CountDEBUGCommand _logic;

        public CountDEBUGCommand(SuperGamblino.Commands.Commands.CountDEBUGCommand logic)
        {
            _logic = logic;
        }

        [Command("count")]
        public async Task OnExecute(CommandContext context)
        {
            var message = new Message()
            {
                Description = "preparing...",
                Color = "#439ff0"
            };
            var discordMessage = await context.RespondAsync(embed: message.ToDiscordEmbed());
            message.OnUpdate += (sender, args) =>
            {
                if (sender is Message obj)
                {
                    discordMessage.ModifyAsync(embed: obj.ToDiscordEmbed());
                }
            };
            message.OnDelete += (sender, args) =>
            {
                if (sender is Message obj)
                {
                    discordMessage.DeleteAsync();
                }
            };
            await _logic.Count(ref message);
        }
    }
}