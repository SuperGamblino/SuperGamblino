using DSharpPlus.Entities;

namespace SuperGamblino.CommandsLogics
{
    public class AboutCommandLogic
    {
        private readonly Messages _messages;

        public AboutCommandLogic(Messages messages)
        {
            _messages = messages;
        }

        public DiscordEmbed GetAboutInfo(int guildsCount)
        {
            return _messages.Information($"Bot is currently available on {guildsCount} servers!", "About");
        }
    }
}