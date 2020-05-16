using Microsoft.Extensions.DependencyInjection;

namespace SuperGamblino.Commands
{
    public static class CommandLogicsHelper
    {
        /// <summary>
        ///     Should be called AFTER AddDatabaseConnectors, Messages service and BetSizeParser service
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandLogics(this IServiceCollection builder)
        {
            return builder
                .AddTransient<AboutCommandLogic>()
                .AddTransient<CoinflipCommandLogic>()
                .AddTransient<CollectDropCommandLogic>()
                .AddTransient<CooldownCommandLogic>()
                .AddTransient<CreditsCommandLogic>()
                .AddTransient<DailyRewardCommandLogic>()
                .AddTransient<GameHistoryCommandLogic>()
                .AddTransient<GlobalTopCommandLogic>()
                .AddTransient<HourlyRewardCommandLogic>()
                .AddTransient<RouletteCommandLogic>()
                .AddTransient<ShowProfileCommandLogic>()
                .AddTransient<SlotsCommandLogic>()
                .AddTransient<VoteRewardCommandLogic>()
                .AddTransient<WorkRewardCommandLogic>();
        }
    }
}