using Dapper.Contrib.Extensions;

namespace SuperGamblino.Core.Entities
{
    [Table("GameHistories")]
    public class GameHistory
    {
        [Key]
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string GameName { get; set; }
        public bool HasWon { get; set; }
        public int CoinsDifference { get; set; }
    }
}