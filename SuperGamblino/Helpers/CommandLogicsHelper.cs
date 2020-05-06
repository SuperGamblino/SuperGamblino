using DSharpPlus.CommandsNext;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Helpers
{
    public static class CommandLogicsHelper
    {
        /// <summary>
        ///     Should be called AFTER AddDatabaseConnectors, Messages service and BetSizeParser service
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DependencyCollectionBuilder AddCommandLogics(this DependencyCollectionBuilder builder)
        {
            return builder
                .Add<AboutCommandLogic>()
                .Add<CoinflipCommandLogic>()
                .Add<CollectDropCommandLogic>()
                .Add<CooldownCommandLogic>()
                .Add<CreditsCommandLogic>()
                .Add<DailyRewardCommandLogic>()
                .Add<GameHistoryCommandLogic>()
                .Add<GlobalTopCommandLogic>()
                .Add<HourlyRewardCommandLogic>()
                .Add<RouletteCommandLogic>()
                .Add<ShowProfileCommandLogic>()
                .Add<SlotsCommandLogic>()
                .Add<VoteRewardCommandLogic>()
                .Add<WorkRewardCommandLogic>();
        }
    }
}