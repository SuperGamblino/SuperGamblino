using Dapper.Contrib.Extensions;

namespace SuperGamblino.Core.Entities
{
    [Table("CoinDrops")]
    public class CoinDrop
    {
        [Key]
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public int ClaimId { get; set; }
        public int CoinReward { get; set; }
        public bool Claimed { get; set; }
    }
}