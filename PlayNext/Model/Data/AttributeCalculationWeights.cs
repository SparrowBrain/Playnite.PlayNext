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
            TotalPlaytime = 0.8f,
            RecentPlaytime = 0,
            RecentOrder = 0.2f,
        };

        public float TotalPlaytime { get; set; }

        public float RecentPlaytime { get; set; }

        public float RecentOrder { get; set; }
    }
}