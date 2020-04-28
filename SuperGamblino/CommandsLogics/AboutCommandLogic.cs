using DSharpPlus.Entities;

namespace SuperGamblino.CommandsLogics
{
    public class AboutCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;

        public AboutCommandLogic(MessagesHelper messagesHelper)
        {
            _messagesHelper = messagesHelper;
        }

        public DiscordEmbed GetAboutInfo(int guildsCount)
        {
            return _messagesHelper.Information($"Bot is currently available on {guildsCount} servers!", "About");
        }
    }
}