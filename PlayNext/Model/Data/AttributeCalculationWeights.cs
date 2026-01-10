namespace PlayNext.Model.Data
{
	public class AttributeCalculationWeights
	{
		public const float Number = 5;

		public static AttributeCalculationWeights Flat { get; } = new AttributeCalculationWeights
		{
			TotalPlaytime = 1 / Number,
			RecentPlaytime = 1 / Number,
			RecentOrder = 1 / Number,
			UserFavourites = 1 / Number,
			UserScore = 1 / Number,
		};

		public static AttributeCalculationWeights Default { get; } = new AttributeCalculationWeights
		{
			TotalPlaytime = 0.6f,
			RecentPlaytime = 0.1f,
			RecentOrder = 0.1f,
			UserFavourites = 0.1f,
			UserScore = 0.1f,
		};

		public float TotalPlaytime { get; set; }

		public float RecentPlaytime { get; set; }

		public float RecentOrder { get; set; }

		public float UserFavourites { get; set; }

		public float UserScore { get; set; }
	}
}