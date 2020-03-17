using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    public class Roulette
    {
        Result result = new Result();

        public class Result
        {
            public string Color { get; set; }
            public int Number { get; set; }
            public string OddOrEven { get; set; }
            public bool FirstEighteen { get; set; }
        }

        public static Result GetResult(int nmb)
        {
            string oddOrEven = nmb % 2 != 0 ? "Odd" : "Even";
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
            List<int> redNmbs = new List<int>
            {
                1, 3, 5, 7, 9, 12,
                14, 16, 18, 19, 21, 23,
                25, 27, 30, 32, 34, 36
            };
            List<int> blackNmbs = new List<int>
            {
                2, 4, 6, 8, 10, 11,
                13, 15, 17, 20, 22, 24,
                26, 28, 29, 31, 33, 35
            };
            List<int> greenNmbs = new List<int>
            {
                0
            };

            if (redNmbs.Contains(number))
                return "Red";
            else if (blackNmbs.Contains(number))
                return "Black";
            else if (greenNmbs.Contains(number))
                return "Green";
            else
                return "ERROR";
        }
    }
}
