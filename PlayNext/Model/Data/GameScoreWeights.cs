namespace PlayNext.Model.Data
{
    public class GameScoreWeights
    {
        public const float Number = 10;

        public float Genre { get; set; }
        public float Feature { get; set; }
        public float Developer { get; set; }
        public float Publisher { get; set; }
        public float Tag { get; set; }
        public float Series { get; set; }
        public float CriticScore { get; set; }
        public float CommunityScore { get; set; }
        public float ReleaseYear { get; set; }
        public float GameLength { get; set; }

        public static GameScoreWeights Flat { get; } = new GameScoreWeights()
        {
            Genre = 1 / Number,
            Feature = 1 / Number,
            Developer = 1 / Number,
            Publisher = 1 / Number,
            Tag = 1 / Number,
            Series = 1 / Number,
            CriticScore = 1 / Number,
            CommunityScore = 1 / Number,
            ReleaseYear = 1 / Number,
            GameLength = 1 / Number,
        };

        public static GameScoreWeights Default { get; } = new GameScoreWeights()
        {
            Genre = 0.1f,
            Feature = DistributeRemainderEqually(),
            Developer = DistributeRemainderEqually(),
            Publisher = DistributeRemainderEqually(),
            Tag = DistributeRemainderEqually(),
            Series = DistributeRemainderEqually(),
            CriticScore = 0.4f,
            CommunityScore = 0.2f,
            ReleaseYear = 0.1f,
            GameLength = 0f,
        };

        private static float DistributeRemainderEqually()
        {
            return 0.2f / 5;
        }
    }
}