using SuperGamblino.GameObjects;
using Xunit;

namespace SuperGamblinoTests.GamesTests
{
    public class SlotsTests
    {
        [Fact]
        public void SampleTest()
        {
            var result = new Slots.SlotsResult()
            {
                EmojiOne = ":thinking:",
                EmojiTwo = ":thinking:",
                EmojiThree = ":gem:"
            };
            var correctReward = 500; //idk, it's simple ammount
            
            var actualReward = Slots.GetPointsFromResult(result, 100);
            
            Assert.Equal(correctReward, actualReward);
        }
    }
}