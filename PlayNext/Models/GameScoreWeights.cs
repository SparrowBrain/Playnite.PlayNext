namespace PlayNext.Models
{
    public class GameScoreWeights
    {
        private const float Number = 5;

        public float Genre { get; set; }
        public float Category { get; set; }
        public float Developer { get; set; }
        public float Publisher { get; set; }
        public float Tag { get; set; }

        public static GameScoreWeights Flat { get; } = new GameScoreWeights()
        {
            Genre = 1 / Number,
            Category = 1 / Number,
            Developer = 1 / Number,
            Publisher = 1 / Number,
            Tag = 1 / Number,
        };
    }
}