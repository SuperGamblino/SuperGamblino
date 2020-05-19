using System;

namespace SuperGamblino.Core.Helpers
{
    public static class ExpHelpers
    {
        public static int CalculateBet(int userLevel, int bet)
        {
            var exp = new Random().Next(50, 125);
            return userLevel * 15 > bet ? 0 : exp;
        }
    }
}