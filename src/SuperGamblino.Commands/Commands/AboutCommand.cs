﻿using SuperGamblino.Messages;

 namespace SuperGamblino.Commands.Commands
{
    public class AboutCommand
    {
        private readonly MessagesHelper _messagesHelper;

        public AboutCommand(MessagesHelper messagesHelper)
        {
            _messagesHelper = messagesHelper;
        }

        public Message GetAboutInfo(int guildsCount)
        {
            return _messagesHelper.Information($"Bot is currently available on {guildsCount} servers!", "About");
        }
    }
}