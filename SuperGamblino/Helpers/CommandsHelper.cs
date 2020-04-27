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
            commands.RegisterCommands<HourlyReward>();
            commands.RegisterCommands<DailyReward>();
            commands.RegisterCommands<Cooldown>();
            commands.RegisterCommands<WorkReward>();
            commands.RegisterCommands<GamesHistory>();
            commands.RegisterCommands<ShowProfile>();
            commands.RegisterCommands<VoteReward>();
            commands.RegisterCommands<About>();
            commands.RegisterCommands<CollectDrop>();
        }
    }
}