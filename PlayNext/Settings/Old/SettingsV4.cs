using PlayNext.Model.Data;
using System;
using System.Collections.Generic;

namespace PlayNext.Settings.Old
{
	public class SettingsV4 : ObservableObject, IMigratableSettings
	{
		public const int MaxWeightValue = 100;

		public SettingsV4()
		{
			Version = 4;
		}

		private SettingsV4(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights) : this()
		{
			SetAttributeWeights(attributeCalculationWeights);
			SetGameWeights(gameScoreWeights);

			DesiredReleaseYear = 2000;
			ReleaseYearChoice = ReleaseYearChoice.Current;

			GameLengthHours = 0;

			ShowSidebarItem = true;
			NumberOfTopGames = 30;
			RecentDays = 14;
			AverageUserScore = 70;
			UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;
			UnplayedCompletionStatuses = Array.Empty<Guid>();
			RefreshOnGameUpdates = false;

			StartPageShowLabel = true;
			StartPageLabelIsHorizontal = false;
			StartPageMinCoverCount = 1;
		}

		public static SettingsV4 Default => new SettingsV4(AttributeCalculationWeights.Default, GameScoreWeights.Default);

		public Guid? SelectedPresetId { get; set; }

		public bool ShowSidebarItem { get; set; }

		public float TotalPlaytimeWeight { get; set; }

		public float RecentPlaytimeWeight { get; set; }

		public float RecentOrderWeight { get; set; }

		public float UserFavouritesWeight { get; set; }

		public float UserScoreWeight { get; set; }

		public float GenreWeight { get; set; }

		public float FeatureWeight { get; set; }

		public float DeveloperWeight { get; set; }

		public float PublisherWeight { get; set; }

		public float TagWeight { get; set; }

		public float SeriesWeight { get; set; }

		public OrderSeriesBy OrderSeriesBy { get; set; }

		public float CriticScoreWeight { get; set; }

		public float CommunityScoreWeight { get; set; }

		public float ReleaseYearWeight { get; set; }

		public int DesiredReleaseYear { get; set; }

		public ReleaseYearChoice ReleaseYearChoice { get; set; }

		public float GameLengthWeight { get; set; }

		public int GameLengthHours { get; set; }

		public float RandomWeight { get; set; }

		public int NumberOfTopGames { get; set; }

		public int RecentDays { get; set; }

		public int AverageUserScore { get; set; }

		public UnplayedGameDefinition UnplayedGameDefinition { get; set; }

		public Guid[] UnplayedCompletionStatuses { get; set; }

		public bool RefreshOnGameUpdates { get; set; }

		public HashSet<Guid> ExcludedSourceIds { get; set; } = new HashSet<Guid>();

		public HashSet<Guid> ExcludedPlatformIds { get; set; } = new HashSet<Guid>();

		public HashSet<Guid> ExcludedCategoryIds { get; set; } = new HashSet<Guid>();

		public HashSet<Guid> ExcludedTagIds { get; set; } = new HashSet<Guid>();

		public bool StartPageShowLabel { get; set; }

		public bool StartPageLabelIsHorizontal { get; set; }

		public int StartPageMinCoverCount { get; set; }

		public int Version { get; set; }

		public IVersionedSettings Migrate()
		{
			var settings = PlayNextSettings.Default;

			settings.SelectedPresetId = SelectedPresetId;
			settings.ShowSidebarItem = ShowSidebarItem;

			settings.TotalPlaytimeWeight = TotalPlaytimeWeight;
			settings.RecentPlaytimeWeight = RecentPlaytimeWeight;
			settings.RecentOrderWeight = RecentOrderWeight;
			settings.UserFavouritesWeight = UserFavouritesWeight;
			settings.UserScoreWeight = UserScoreWeight;

			settings.GenreWeight = GenreWeight;
			settings.FeatureWeight = FeatureWeight;
			settings.DeveloperWeight = DeveloperWeight;
			settings.PublisherWeight = PublisherWeight;
			settings.TagWeight = TagWeight;
			settings.CriticScoreWeight = CriticScoreWeight;
			settings.CommunityScoreWeight = CommunityScoreWeight;
			settings.ReleaseYearWeight = ReleaseYearWeight;
			settings.ReleaseYearChoice = ReleaseYearChoice;
			settings.DesiredReleaseYear = DesiredReleaseYear;
			settings.GameLengthWeight = GameLengthWeight;
			settings.GameLengthHours = GameLengthHours;
			settings.SeriesWeight = SeriesWeight;
			settings.OrderSeriesBy = OrderSeriesBy;
			settings.RandomWeight = RandomWeight;

			settings.RecentDays = RecentDays;
			settings.NumberOfTopGames = NumberOfTopGames;
			settings.AverageUserScore = AverageUserScore;
			settings.RefreshOnGameUpdates = RefreshOnGameUpdates;

			settings.UnplayedGameDefinition = UnplayedGameDefinition;
			settings.UnplayedCompletionStatuses = UnplayedCompletionStatuses;

			settings.ExcludedSourceIds = ExcludedSourceIds;
			settings.ExcludedPlatformIds = ExcludedPlatformIds;
			settings.ExcludedCategoryIds = ExcludedCategoryIds;
			settings.ExcludedTagIds = ExcludedTagIds;

			settings.StartPageShowLabel = StartPageShowLabel;
			settings.StartPageLabelIsHorizontal = StartPageLabelIsHorizontal;
			settings.StartPageMinCoverCount = StartPageMinCoverCount;

			return settings;
		}

		public void SetAttributeWeights(AttributeCalculationWeights attributeCalculationWeights)
		{
			TotalPlaytimeWeight = attributeCalculationWeights.TotalPlaytime * MaxWeightValue;
			RecentPlaytimeWeight = attributeCalculationWeights.RecentPlaytime * MaxWeightValue;
			RecentOrderWeight = attributeCalculationWeights.RecentOrder * MaxWeightValue;
			UserFavouritesWeight = attributeCalculationWeights.UserFavourites * MaxWeightValue;
			UserScoreWeight = attributeCalculationWeights.UserScore * MaxWeightValue;
		}

		public void SetGameWeights(GameScoreWeights gameScoreWeights)
		{
			GenreWeight = gameScoreWeights.Genre * MaxWeightValue;
			FeatureWeight = gameScoreWeights.Feature * MaxWeightValue;
			DeveloperWeight = gameScoreWeights.Developer * MaxWeightValue;
			PublisherWeight = gameScoreWeights.Publisher * MaxWeightValue;
			TagWeight = gameScoreWeights.Tag * MaxWeightValue;
			SeriesWeight = gameScoreWeights.Series * MaxWeightValue;
			CriticScoreWeight = gameScoreWeights.CriticScore * MaxWeightValue;
			CommunityScoreWeight = gameScoreWeights.CommunityScore * MaxWeightValue;
			ReleaseYearWeight = gameScoreWeights.ReleaseYear * MaxWeightValue;
			GameLengthWeight = gameScoreWeights.GameLength * MaxWeightValue;
			RandomWeight = gameScoreWeights.Random * MaxWeightValue;
		}
	}
}
