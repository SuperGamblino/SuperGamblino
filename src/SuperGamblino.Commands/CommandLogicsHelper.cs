using Microsoft.Extensions.DependencyInjection;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Commands
{
    public static class CommandsHelper
    {
        /// <summary>
        ///     Should be called AFTER AddDatabaseConnectors, Messages service and BetSizeParser service
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommands(this IServiceCollection builder)
        {
            return builder
                .AddTransient<AboutCommand>()
                .AddTransient<CoinflipCommand>()
                .AddTransient<CollectDropCommand>()
                .AddTransient<CooldownCommand>()
                .AddTransient<CreditsCommand>()
                .AddTransient<DailyRewardCommand>()
                .AddTransient<GameHistoryCommand>()
                .AddTransient<GlobalTopCommand>()
                .AddTransient<HourlyRewardCommand>()
                .AddTransient<RouletteCommand>()
                .AddTransient<ShowProfileCommand>()
                .AddTransient<SlotsCommand>()
                .AddTransient<VoteRewardCommand>()
                .AddTransient<WorkRewardCommand>()
                .AddTransient<CountDEBUGCommand>();
        }
    }
}