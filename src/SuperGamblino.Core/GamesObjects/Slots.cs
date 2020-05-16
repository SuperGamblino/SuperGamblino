﻿using System;

 namespace SuperGamblino.Core.GamesObjects
{
    public static class Slots
    {
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

        private const int BET_DIVIDER = 8;
        private const int JACKPOT_MULTIPLIER = 10;
        private const int DOUBLE_MULTIPLIER = 3;

        public static SlotsResult GetResult()
        {
            var resultOne = GetRandomEmote();
            var resultTwo = GetRandomEmote();
            var resultThree = GetRandomEmote();

            return new SlotsResult
            {
                ResultOne = resultOne,
                ResultTwo = resultTwo,
                ResultThree = resultThree,

                EmojiOne = SlotsEmoteToDiscordEmoji(resultOne),
                EmojiTwo = SlotsEmoteToDiscordEmoji(resultTwo),
                EmojiThree = SlotsEmoteToDiscordEmoji(resultThree)
            };
        }

        public static SlotsEmote GetRandomEmote()
        {
            var values = Enum.GetValues(typeof(SlotsEmote));
            var rand = new Random();
            return (SlotsEmote) values.GetValue(rand.Next(values.Length));
        }

        public static string SlotsEmoteToDiscordEmoji(SlotsEmote emote)
        {
            string emoji = null;

            switch (emote)
            {
                case SlotsEmote.GEM:
                    emoji = ":gem:";
                    break;
                case SlotsEmote.SEVEN:
                    emoji = ":seven:";
                    break;
                case SlotsEmote.HEART:
                    emoji = ":heart:";
                    break;
                case SlotsEmote.BELL:
                    emoji = ":bell:";
                    break;
                case SlotsEmote.PILL:
                    emoji = ":pill:";
                    break;
                case SlotsEmote.MELON:
                    emoji = ":melon:";
                    break;
                case SlotsEmote.CHERRY:
                    emoji = ":cherries:";
                    break;
                case SlotsEmote.LEMON:
                    emoji = ":lemon:";
                    break;
                case SlotsEmote.THINKING:
                    emoji = ":thinking:";
                    break;
            }

            return emoji;
        }

        public static bool IsJackpot(SlotsResult result)
        {
            if (result.ResultOne == result.ResultTwo && result.ResultTwo == result.ResultThree) return true;

            return false;
        }

        public static bool IsDouble(SlotsResult result)
        {
            if (result.ResultOne == result.ResultTwo) return true;

            if (result.ResultTwo == result.ResultThree) return true;

            return false;
        }

        public static int GetPointsFromResult(SlotsResult result, int betAmount)
        {
            var points = 0;

            if (IsJackpot(result))
                points = ((int) result.ResultOne + (int) result.ResultTwo + (int) result.ResultThree) *
                         JACKPOT_MULTIPLIER;
            else if (result.ResultOne == result.ResultTwo)
                points = ((int) result.ResultOne + (int) result.ResultTwo) * DOUBLE_MULTIPLIER;
            else if (result.ResultTwo == result.ResultThree)
                points = ((int) result.ResultTwo + (int) result.ResultThree) * DOUBLE_MULTIPLIER;

            if (points > 0)
                return PointMultiplier(points, betAmount);
            return 0;
        }

        public static int PointMultiplier(int currentPoints, int betAmount)
        {
            return currentPoints + currentPoints * (int) Math.Floor((decimal) (betAmount / BET_DIVIDER));
        }

        public class SlotsResult
        {
            public SlotsEmote ResultOne { get; set; }
            public SlotsEmote ResultTwo { get; set; }
            public SlotsEmote ResultThree { get; set; }

            public string EmojiOne { get; set; }
            public string EmojiTwo { get; set; }
            public string EmojiThree { get; set; }
        }
    }
}