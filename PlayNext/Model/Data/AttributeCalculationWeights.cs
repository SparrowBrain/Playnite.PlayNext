namespace PlayNext.Model.Data
{
    public class AttributeCalculationWeights
    {
        public const float Number = 3;

        public static AttributeCalculationWeights Flat { get; } = new AttributeCalculationWeights
        {
            TotalPlaytime = 1 / Number,
            RecentPlaytime = 1 / Number,
            RecentOrder = 1 / Number
        };

        public static AttributeCalculationWeights Default { get; } = new AttributeCalculationWeights
        {
            TotalPlaytime = 0.7f,
            RecentPlaytime = 0.2f,
            RecentOrder = 0.1f,
        };

        public float TotalPlaytime { get; set; }

        public float RecentPlaytime { get; set; }

        public float RecentOrder { get; set; }
    }
}