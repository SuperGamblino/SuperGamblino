using System.Collections.Generic;

namespace SuperGamblino.GameObjects
{
    public class History
    {
        public ulong UserId { get; set; }
        public IEnumerable<GameHistory> GameHistories { get; set; }
        
    }

    public class GameHistory
    {
        public string GameName { get; set; }
        public bool HasWon { get; set; }
        public int CoinsDifference { get; set; }
    }
}