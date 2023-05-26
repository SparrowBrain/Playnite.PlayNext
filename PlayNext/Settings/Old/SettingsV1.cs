using System;
using PlayNext.Model.Data;

namespace PlayNext.Settings.Old
{
    public class SettingsV1 : IMigratableSettings
    {
        public const int MaxWeightValue = 100;

        public SettingsV1()
        {
            Version = 1;
        }

        private SettingsV1(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights) : this()
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            NumberOfTopGames = 30;
            RecentDays = 14;
            UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;
            UnplayedCompletionStatuses = Array.Empty<Guid>();
        }

        public static SettingsV1 Default => new SettingsV1(AttributeCalculationWeights.Default, GameScoreWeights.Default);

        public float TotalPlaytimeWeight { get; set; }

        public float RecentPlaytimeWeight { get; set; }

        public float RecentOrderWeight { get; set; }

        public float GenreWeight { get; set; }

        public float FeatureWeight { get; set; }

        public float DeveloperWeight { get; set; }

        public float PublisherWeight { get; set; }

        public float TagWeight { get; set; }

        public float CriticScoreWeight { get; set; }

        public float CommunityScoreWeight { get; set; }

        public float ReleaseYearWeight { get; set; }

        public int DesiredReleaseYear { get; set; }

        public ReleaseYearChoice ReleaseYearChoice { get; set; }

        public int NumberOfTopGames { get; set; }

        public int RecentDays { get; set; }

        public UnplayedGameDefinition UnplayedGameDefinition { get; set; }

        public Guid[] UnplayedCompletionStatuses { get; set; }

        public int Version { get; set; }

        public IVersionedSettings Migrate()
        {
            var settings = PlayNextSettings.Default;
            settings.TotalPlaytimeWeight = TotalPlaytimeWeight;
            settings.RecentPlaytimeWeight = RecentPlaytimeWeight;
            settings.RecentOrderWeight = RecentOrderWeight;

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

            settings.RecentDays = RecentDays;
            settings.NumberOfTopGames = NumberOfTopGames;

            settings.UnplayedGameDefinition = UnplayedGameDefinition;
            settings.UnplayedCompletionStatuses = UnplayedCompletionStatuses;

            return settings;
        }

        public void SetAttributeWeights(AttributeCalculationWeights attributeCalculationWeights)
        {
            TotalPlaytimeWeight = attributeCalculationWeights.TotalPlaytime * MaxWeightValue;
            RecentPlaytimeWeight = attributeCalculationWeights.RecentPlaytime * MaxWeightValue;
            RecentOrderWeight = attributeCalculationWeights.RecentOrder * MaxWeightValue;
        }

        public void SetGameWeights(GameScoreWeights gameScoreWeights)
        {
            GenreWeight = gameScoreWeights.Genre * MaxWeightValue;
            FeatureWeight = gameScoreWeights.Feature * MaxWeightValue;
            DeveloperWeight = gameScoreWeights.Developer * MaxWeightValue;
            PublisherWeight = gameScoreWeights.Publisher * MaxWeightValue;
            TagWeight = gameScoreWeights.Tag * MaxWeightValue;
            CriticScoreWeight = gameScoreWeights.CriticScore * MaxWeightValue;
            CommunityScoreWeight = gameScoreWeights.CommunityScore * MaxWeightValue;
            ReleaseYearWeight = gameScoreWeights.ReleaseYear * MaxWeightValue;
        }
    }
}