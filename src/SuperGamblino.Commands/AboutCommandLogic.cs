﻿using SuperGamblino.Messages;

 namespace SuperGamblino.Commands
{
    public class AboutCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;

        public AboutCommandLogic(MessagesHelper messagesHelper)
        {
            _messagesHelper = messagesHelper;
        }

        public Message GetAboutInfo(int guildsCount)
        {
            return _messagesHelper.Information($"Bot is currently available on {guildsCount} servers!", "About");
        }
    }
}