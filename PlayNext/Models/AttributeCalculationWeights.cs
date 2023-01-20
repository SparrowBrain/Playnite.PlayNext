namespace PlayNext.Models
{
    public class AttributeCalculationWeights
    {
        private const float Number = 3;

        public static AttributeCalculationWeights Flat { get; } = new AttributeCalculationWeights
        {
            TotalPlaytime = 1 / Number,
            RecentPlaytime = 1 / Number,
            RecentOrder = 1 / Number
        };

        public float TotalPlaytime { get; set; }

        public float RecentPlaytime { get; set; }

        public float RecentOrder { get; set; }
    }
}