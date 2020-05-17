﻿using System.ComponentModel.DataAnnotations;
 using Dapper.Contrib.Extensions;

#nullable enable
namespace SuperGamblino.Core.GamesObjects
{
    [Table("BlackJack")]
    public class BlackjackHelper
    {
        [Dapper.Contrib.Extensions.Key]
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public bool IsGameDone { get; set; }
        [MaxLength(45)]
        public string? UserHand { get; set; }
        [MaxLength(45)]
        public string? DealerHand { get; set; }
        public int? Bet { get; set; }
    }
}