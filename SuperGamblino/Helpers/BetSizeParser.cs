using System.Collections.Generic;

namespace SuperGamblino.Helpers
{
    public class BetSizeParser
    {
        private readonly Dictionary<char, int> _conversations = new Dictionary<char, int>
        {
            {'k', 1000},
            {'m', 1000000},
            {'b', 1000000000}
        };

        public int Parse(string input)
        {
            var data = input.ToLower().Trim();
            if (data.Contains("-")) return -1;
            var completeValue = 0;
            var currentValue = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var c = data[i];
                if (char.IsLetter(c))
                {
                    if (_conversations.ContainsKey(c))
                    {
                        currentValue *= _conversations[c];
                        completeValue += currentValue;
                        currentValue = 0;
                    }
                    else
                    {
                        return -1; //Invalid character detected
                    }
                }
                else
                {
                    currentValue *= 10;
                    currentValue += c - 48;
                }
            }

            if (currentValue != 0) completeValue += currentValue;

            if (completeValue < 1) return -1;

            return completeValue;
        }
    }
}