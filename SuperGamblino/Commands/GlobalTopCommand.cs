using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class GlobalTopCommand
    {
        private readonly Config _config;
        private readonly UsersConnector _usersConnector;

        public GlobalTopCommand(Config config, UsersConnector usersConnector)
        {
            _config = config;
            _usersConnector = usersConnector;
        }

        [Command("globaltop")]
        [Aliases("gt")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a global leaderboard based on credits. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            var listUsers = await _usersConnector.CommandGetGlobalTop();

            var desc = "";
            foreach (var user in listUsers)
            {
                try
                {
                    DiscordMember member = await command.Guild.GetMemberAsync(user.Id);
                    desc += member.Username + ": " + user.Credits + "\n";
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message.ToString());
                }
            }

            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Top 10 users",
                Description = desc
            };
            await command.RespondAsync("", false, message);
        }
    }
}