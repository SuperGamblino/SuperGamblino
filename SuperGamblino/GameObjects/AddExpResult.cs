namespace SuperGamblino.GameObjects
{
    public class AddExpResult
    {
        public AddExpResult(bool didUserLevelUp, int requiredExp, int currentExp, int givenExp)
        {
            DidUserLevelUp = didUserLevelUp;
            RequiredExp = requiredExp;
            CurrentExp = currentExp;
            GivenExp = givenExp;
        }

        public bool DidUserLevelUp { get; set; }
        public int RequiredExp { get; set; }
        public int CurrentExp { get; set; }
        public int GivenExp { get; set; }
    }
}