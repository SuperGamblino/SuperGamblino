#nullable enable
namespace SuperGamblino.GameObjects
{
    public class BlackjackHelper
    {
        public bool IsGameDone { get; set; }
        public string? UserHand { get; set; }
        public string? DealerHand { get; set; }
        public int? Bet { get; set; }
    }
}