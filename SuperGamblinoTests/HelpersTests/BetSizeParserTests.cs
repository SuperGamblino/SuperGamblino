using SuperGamblino.Helpers;
using Xunit;

namespace SuperGamblinoTests.HelpersTests
{
    public class BetSizeParserTests
    {
        [Theory]
        [InlineData("1k",1000)]
        [InlineData("5k",5000)]
        [InlineData("1K",1000)]
        [InlineData("5K", 5000)]
        [InlineData("5105k", 5105000)]
        [InlineData("999999999k", -1)] //Over int
        [InlineData("-5k", -1)] //Below 1
        [InlineData("0k", -1)] //Below 1
        public void CanParseKCorrectly(string input, int correct)
        {
            var parser = new BetSizeParser();
            
            Assert.Equal(correct, parser.Parse(input));
        }

        [Theory]
        [InlineData("1m", 1000000)]
        [InlineData("1M", 1000000)]
        [InlineData("5m", 5000000)]
        [InlineData("5M", 5000000)]
        [InlineData("143m", 143000000)]
        [InlineData("999999m", -1)] //Over int
        [InlineData("-5m", -1)] //Below 1
        [InlineData("0m", -1)] //Below 1
        public void CanParseMCorrectly(string input, int correct)
        {
            var parser = new BetSizeParser();
            
            Assert.Equal(correct, parser.Parse(input));
        }

        [Theory]
        [InlineData("1b", 1000000000)]
        [InlineData("1B", 1000000000)]
        [InlineData("2b", 2000000000)]
        [InlineData("2B", 2000000000)]
        [InlineData("187b", -1)] //Over int
        [InlineData("-5b", -1)] //Below 1
        [InlineData("0b", -1)] //Below 1
        public void CanParseBCorrectly(string input, int correct)
        {
            var parser = new BetSizeParser();
            
            Assert.Equal(correct, parser.Parse(input));
        }

        [Theory]
        [InlineData("1m5k10", 1005010)]
        [InlineData("1B5m4300", 1005004300)]
        public void CanParseComplexInput(string input, int correct)
        {
            var parser = new BetSizeParser();
            Assert.Equal(correct, parser.Parse(input));
        }
        [Theory]
        [InlineData("1210212", 1210212)]
        public void CanParseWithoutAnySymbols(string input, int correct)
        {
            var parser = new BetSizeParser();
            Assert.Equal(correct, parser.Parse(input));
        }
        [Fact]
        public void CannotParseIncorrectText()
        {
            var parser = new BetSizeParser();
            Assert.Equal(-1, parser.Parse("xdxd"));
        }
    }
}