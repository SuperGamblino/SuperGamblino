﻿using System.Collections.Generic;

 namespace SuperGamblino.Core.CommandsObjects
{
    public class History
    {
        public IEnumerable<GameHistory> GameHistories { get; set; }
    }

    public class GameHistory
    {
        public string GameName { get; set; }
        public bool HasWon { get; set; }
        public int CoinsDifference { get; set; }
    }
}