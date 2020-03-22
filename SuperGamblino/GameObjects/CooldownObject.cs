using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    class CooldownObject
    {
        public CooldownObject(string command, TimeSpan timeLeft = new TimeSpan())
        {
            Command = command;
            TimeLeft = timeLeft;
        }
        public string Command { get; set; }
        public TimeSpan TimeLeft { get; set; }
    }
}
