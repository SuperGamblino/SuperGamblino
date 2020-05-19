using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class CreditsTests
    {
        private CreditsCommand GetCreditsCommandLogic(UsersConnector usersConnector)
        {
            return new CreditsCommand(usersConnector, Helpers.GetMessages());
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0)]
        [InlineData(10000)]
        public async void IsCorrectAmountDisplayed(int amount)
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetCredits(0)).ReturnsAsync(amount);
            var logic = GetCreditsCommandLogic(usersConnector.Object);

            var result = await logic.GetCurrentCreditStatus(0);

            Assert.Contains(result.Fields, x => x.Name == "Credits balance");
            Assert.Equal("Credits balance", result.Fields[0].Name);
            Assert.Equal(amount.ToString(), result.Fields[0].Value);
        }
    }
}