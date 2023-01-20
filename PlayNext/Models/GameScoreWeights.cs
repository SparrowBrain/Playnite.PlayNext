namespace PlayNext.Models
{
    public class GameScoreWeights
    {
        private const float Number = 8;

        public float Genre { get; set; }
        public float Feature { get; set; }
        public float Developer { get; set; }
        public float Publisher { get; set; }
        public float Tag { get; set; }
        public float CriticScore { get; set; }
        public float CommunityScore { get; set; }
        public float ReleaseYear { get; set; }

        public static GameScoreWeights Flat { get; } = new GameScoreWeights()
        {
            Genre = 1 / Number,
            Feature = 1 / Number,
            Developer = 1 / Number,
            Publisher = 1 / Number,
            Tag = 1 / Number,
            CriticScore = 1 / Number,
            CommunityScore = 1 / Number,
            ReleaseYear = 1 / Number,
        };
    }
}