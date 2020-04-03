using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    public class Slots
    {
        public static SlotsResult GetResult(DiscordClient client)
        {
            SlotsEmote resultOne = GetRandomEmote();
            SlotsEmote resultTwo = GetRandomEmote();
            SlotsEmote resultThree = GetRandomEmote();

            return new SlotsResult
            {
                ResultOne = resultOne,
                ResultTwo = resultTwo,
                ResultThree = resultThree,

                EmojiOne = SlotsEmoteToDiscordEmoji(client, resultOne),
                EmojiTwo = SlotsEmoteToDiscordEmoji(client, resultTwo),
                EmojiThree = SlotsEmoteToDiscordEmoji(client, resultThree)
            };
        }

        public enum SlotsEmote
        {
            GEM = 9,
            SEVEN = 8,
            HEART = 7,
            BELL = 6,
            PILL = 5,
            MELON = 4,
            CHERRY = 3,
            LEMON = 2,
            THINKING = 1
        }

        public static SlotsEmote GetRandomEmote() 
        {
            Array values = Enum.GetValues(typeof(SlotsEmote));
            Random rand = new Random();
            return (SlotsEmote)values.GetValue(rand.Next(values.Length));
        }

        public static DiscordEmoji SlotsEmoteToDiscordEmoji(DiscordClient client, SlotsEmote emote) 
        {
            DiscordEmoji emoji = null;

            switch (emote) 
            {
                case SlotsEmote.GEM:
                    emoji = DiscordEmoji.FromName(client, ":gem:");
                    break;
                case SlotsEmote.SEVEN:
                    emoji = DiscordEmoji.FromName(client, ":seven:");
                    break;
                case SlotsEmote.HEART:
                    emoji = DiscordEmoji.FromName(client, ":heart:");
                    break;
                case SlotsEmote.BELL:
                    emoji = DiscordEmoji.FromName(client, ":bell:");
                    break;
                case SlotsEmote.PILL:
                    emoji = DiscordEmoji.FromName(client, ":pill:");
                    break;
                case SlotsEmote.MELON:
                    emoji = DiscordEmoji.FromName(client, ":melon:");
                    break;
                case SlotsEmote.CHERRY:
                    emoji = DiscordEmoji.FromName(client, ":cherries:");
                    break;
                case SlotsEmote.LEMON:
                    emoji = DiscordEmoji.FromName(client, ":lemon:");
                    break;
                case SlotsEmote.THINKING:
                    emoji = DiscordEmoji.FromName(client, ":thinking:");
                    break;
            }

            return emoji;
        }

        public static bool IsJackpot(SlotsResult result) 
        {
            if (result.ResultOne == result.ResultTwo && result.ResultTwo == result.ResultThree)
            {
                return true;
            }

            return false;
        }

        public static bool IsDouble(SlotsResult result) 
        {
            if (result.ResultOne == result.ResultTwo)
            {
                return true;
            }

            if (result.ResultTwo == result.ResultThree) 
            {
                return true;
            }

            return false;
        }

        public static int GetPointsFromResult(SlotsResult result, int betAmount) 
        {
            int points = 0;

            if (IsJackpot(result)) 
            {
                points = ((int)result.ResultOne + (int)result.ResultTwo + (int)result.ResultThree) * 10;
                return PointMultiplier(points, betAmount);
            }
            
            if (result.ResultOne == result.ResultTwo)
            {
                points = ((int)result.ResultOne + (int)result.ResultTwo) * 4;
                return PointMultiplier(points, betAmount);

            }
            
            if (result.ResultTwo == result.ResultThree) 
            {
                points = ((int)result.ResultTwo + (int)result.ResultThree) * 4;
                return PointMultiplier(points, betAmount);
            }

            return PointMultiplier(points, betAmount);
        }

        public static int PointMultiplier(int currentPoints, int betAmount) 
        {
            return currentPoints + (currentPoints * (betAmount / 10));
        }

        public class SlotsResult
        {
            public SlotsEmote ResultOne { get;  set; }
            public SlotsEmote ResultTwo { get; set; }
            public SlotsEmote ResultThree { get; set; }

            public DiscordEmoji EmojiOne { get; set; }
            public DiscordEmoji EmojiTwo { get; set; }
            public DiscordEmoji EmojiThree { get; set; }
        }
    }
}
