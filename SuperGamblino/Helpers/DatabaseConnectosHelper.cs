using DSharpPlus.CommandsNext;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Helpers
{
    public static class DatabaseConnectorsHelper
    {
        /// <summary>
        ///     Should be called AFTER ConnectionString service and Logger service
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static DependencyCollectionBuilder AddDatabaseConnectors(this DependencyCollectionBuilder builder)
        {
            return builder
                .Add<BlackjackConnector>()
                .Add<GameHistoryConnector>()
                .Add<CoindropConnector>()
                .Add<UsersConnector>();
        }
    }
}