using System;
using System.Collections.Generic;
using PlayNext.Model.Data;

namespace PlayNext.Settings.Old
{
    public class SettingsV2 : ObservableObject, IMigratableSettings
    {
        public const int MaxWeightValue = 100;

        public SettingsV2()
        {
            Version = 2;
        }

        private SettingsV2(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights) : this()
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            GameLengthHours = 0;

            NumberOfTopGames = 30;
            RecentDays = 14;
            UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;
            UnplayedCompletionStatuses = Array.Empty<Guid>();

            StartPageShowLabel = true;
            StartPageLabelIsHorizontal = false;
        }

        public static SettingsV2 Default => new SettingsV2(AttributeCalculationWeights.Default, GameScoreWeights.Default);

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

        public float GameLengthWeight { get; set; }

        public int GameLengthHours { get; set; }

        public int NumberOfTopGames { get; set; }

        public int RecentDays { get; set; }

        public UnplayedGameDefinition UnplayedGameDefinition { get; set; }

        public Guid[] UnplayedCompletionStatuses { get; set; }

        public bool StartPageShowLabel { get; set; }

        public bool StartPageLabelIsHorizontal { get; set; }

        public int Version { get; set; }

        public IVersionedSettings Migrate()
        {
            var settings = SettingsV3.Default;
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
            settings.GameLengthWeight = GameLengthWeight;
            settings.GameLengthHours = GameLengthHours;

            settings.RecentDays = RecentDays;
            settings.NumberOfTopGames = NumberOfTopGames;

            settings.UnplayedGameDefinition = UnplayedGameDefinition;
            settings.UnplayedCompletionStatuses = UnplayedCompletionStatuses;

            settings.StartPageShowLabel = StartPageShowLabel;
            settings.StartPageLabelIsHorizontal = StartPageLabelIsHorizontal;

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
            GameLengthWeight = gameScoreWeights.GameLength * MaxWeightValue;
        }
    }
}