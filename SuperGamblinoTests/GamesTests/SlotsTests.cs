using SuperGamblino.GameObjects;
using Xunit;

namespace SuperGamblinoTests.GamesTests
{
    public class SlotsTests
    {
        [Fact]
        public void GetPointsFromResultTest1()
        {
            var result = new Slots.SlotsResult
            {
                ResultOne = Slots.SlotsEmote.THINKING,
                ResultTwo = Slots.SlotsEmote.THINKING,
                ResultThree = Slots.SlotsEmote.GEM,

                EmojiOne = ":thinking:",
                EmojiTwo = ":thinking:",
                EmojiThree = ":gem:"
            };
            var correctReward = 78;

            var actualReward = Slots.GetPointsFromResult(result, 100);

            Assert.Equal(correctReward, actualReward);
        }

        [Fact]
        public void GetPointsFromResultTest2()
        {
            var result = new Slots.SlotsResult
            {
                ResultOne = Slots.SlotsEmote.GEM,
                ResultTwo = Slots.SlotsEmote.GEM,
                ResultThree = Slots.SlotsEmote.GEM,

                EmojiOne = ":gem:",
                EmojiTwo = ":gem:",
                EmojiThree = ":gem:"
            };
            var correctReward = 3510;

            var actualReward = Slots.GetPointsFromResult(result, 100);

            Assert.Equal(correctReward, actualReward);
        }

        [Fact]
        public void GetPointsMultiplierTest()
        {
            var correctMultiplier = 78;
            var currentPoints = 6;

            var actualReward = Slots.PointMultiplier(currentPoints, 100);

            Assert.Equal(correctMultiplier, actualReward);
        }
    }
}