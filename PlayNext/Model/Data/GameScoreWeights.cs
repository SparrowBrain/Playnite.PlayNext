namespace PlayNext.Model.Data
{
    public class GameScoreWeights
    {
        public const float Number = 9;

        public float Genre { get; set; }
        public float Feature { get; set; }
        public float Developer { get; set; }
        public float Publisher { get; set; }
        public float Tag { get; set; }
        public float CriticScore { get; set; }
        public float CommunityScore { get; set; }
        public float ReleaseYear { get; set; }
        public float Length { get; set; }

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
            Length = 1 / Number,
        };

        public static GameScoreWeights Default { get; } = new GameScoreWeights()
        {
            Genre = 0.1f,
            Feature = 0.2f / (Number - 4),
            Developer = 0.2f / (Number - 4),
            Publisher = 0.2f / (Number - 4),
            Tag = 0.2f / (Number - 4),
            CriticScore = 0.4f,
            CommunityScore = 0.2f,
            ReleaseYear = 0.1f,
            Length = 0f,
        };
    }
}