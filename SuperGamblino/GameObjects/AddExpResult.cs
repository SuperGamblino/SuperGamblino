using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    class AddExpResult
    {
        public AddExpResult(bool didUserLevelUp, int requiredExp, int currentExp)
        {
            DidUserLevelUp = didUserLevelUp;
            RequiredExp = requiredExp;
            CurrentExp = currentExp;
        }
        public bool DidUserLevelUp { get; set; }
        public int RequiredExp { get; set; }
        public int CurrentExp { get; set; }
    }
}
