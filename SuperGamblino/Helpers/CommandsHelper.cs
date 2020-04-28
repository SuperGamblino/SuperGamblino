using DSharpPlus.CommandsNext;
using SuperGamblino.Commands;

namespace SuperGamblino.Helpers
{
    public static class CommandsHelper
    {
        public static void AddCommands(this CommandsNextModule commands)
        {
            commands.RegisterCommands<RouletteCommand>();
            commands.RegisterCommands<CoinflipCommand>();
            commands.RegisterCommands<SlotsCommand>();
            commands.RegisterCommands<CreditsCommand>();
            commands.RegisterCommands<GlobalTopCommand>();
            commands.RegisterCommands<HourlyRewardCommand>();
            commands.RegisterCommands<DailyRewardCommand>();
            commands.RegisterCommands<CooldownCommand>();
            commands.RegisterCommands<WorkRewardCommand>();
            commands.RegisterCommands<GamesHistoryCommand>();
            commands.RegisterCommands<ShowProfileCommand>();
            commands.RegisterCommands<VoteRewardCommand>();
            commands.RegisterCommands<AboutCommand>();
            commands.RegisterCommands<CollectDropCommand>();
        }
    }
}