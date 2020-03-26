using System.Collections.Generic;

namespace SuperGamblino.GameObjects
{
    public class Roulette
    {
        private Result result = new Result();

        public static Result GetResult(int nmb)
        {
            var oddOrEven = nmb % 2 != 0 ? "Odd" : "Even";
            return new Result
            {
                Color = GetColor(nmb),
                Number = nmb,
                OddOrEven = oddOrEven,
                FirstEighteen = nmb < 19 && nmb != 0
            };
        }

        private static string GetColor(int number)
        {
            var redNmbs = new List<int>
            {
                1, 3, 5, 7, 9, 12,
                14, 16, 18, 19, 21, 23,
                25, 27, 30, 32, 34, 36
            };
            var blackNmbs = new List<int>
            {
                2, 4, 6, 8, 10, 11,
                13, 15, 17, 20, 22, 24,
                26, 28, 29, 31, 33, 35
            };
            var greenNmbs = new List<int>
            {
                0
            };

            if (redNmbs.Contains(number))
                return "Red";
            if (blackNmbs.Contains(number))
                return "Black";
            if (greenNmbs.Contains(number))
                return "Green";
            return "ERROR";
        }

        public class Result
        {
            public string Color { get; set; }
            public int Number { get; set; }
            public string OddOrEven { get; set; }
            public bool FirstEighteen { get; set; }
        }
    }
}