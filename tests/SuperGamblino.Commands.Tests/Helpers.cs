using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Infrastructure;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Tests
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
                ColorSettings = new ColorSettings {Info = InfoColor, Success = SuccessColor, Warning = WarningColor},
                BotSettings = new BotSettings
                {
                    Prefix = "!",
                    Token = "<bot token>",
                    TopGgToken = "topggbottoken"
                }
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

        public static Mock<HttpMessageHandler> GetHttpMessageHandler()
        {
            return new Mock<HttpMessageHandler>();
        }

        public static MessagesHelper GetMessages()
        {
            return new MessagesHelper(GetConfig());
        }
    }
}