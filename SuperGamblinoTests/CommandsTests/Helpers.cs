using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SuperGamblino;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public static class Helpers
    {
        public const string InfoColor = "#439ff0";
        public const string SuccessColor = "#4beb50";
        public const string WarningColor = "#bf1004";

        public static ILogger<T> GetLogger<T>()
        {
            return LoggerFactory.Create(x => x.AddConsole()).CreateLogger<T>();
        }

        public static Config GetConfig()
        {
            return new Config
            {
                DatabaseSettings = new DatabaseSettings
                {
                    Address = "<Address>",
                    Name = "<Name>",
                    Password = "<Password>",
                    Port = 1122,
                    Username = "<Username>"
                },
                ColorSettings = new ColorSettings {Info = InfoColor, Success = SuccessColor, Warning = WarningColor}
            };
            ;
        }

        public static ConnectionString GetConnectionString()
        {
            return new ConnectionString(GetConfig());
        }

        public static Mock<T> GetDatabaseConnector<T>() where T : DatabaseConnector
        {
            return new Mock<T>(GetLogger<T>(), GetConnectionString());
        }

        public static IProtectedMock<HttpMessageHandler> GetHttpMessageHandler()
        {
            return new Mock<HttpMessageHandler>().Protected();
        }

        public static Messages GetMessages()
        {
            return new Messages(GetConfig(), GetDatabaseConnector<UsersConnector>().Object);
        }
    }
}