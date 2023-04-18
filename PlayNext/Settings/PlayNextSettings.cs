using System;
using System.Collections.Generic;
using PlayNext.Model.Data;
using Playnite.SDK.Data;

namespace PlayNext.Settings
{
    public class PlayNextSettings : ObservableObject, IVersionedSettings
    {
        public const int MaxWeightValue = 100;
        public const int MinWeightValue = 0;
        public const int CurrentVersion = 1;

        private int _desiredReleaseYear;
        private bool[] _releaseYearChoices = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
        private bool[] _unplayedGameDefinitions = new bool[Enum.GetValues(typeof(UnplayedGameDefinition)).Length];
        private int _numberOfTopGames;
        private int _recentDays;

        public PlayNextSettings()
        {
            Version = CurrentVersion;
        }

        private PlayNextSettings(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights)
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            NumberOfTopGames = 30;
            RecentDays = 14;
            UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;
            UnplayedCompletionStatuses = Array.Empty<Guid>();

            Version = CurrentVersion;
        }

        public static PlayNextSettings Default => new PlayNextSettings(AttributeCalculationWeights.Default, GameScoreWeights.Default);

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

        public int DesiredReleaseYear
        {
            get => _desiredReleaseYear;
            set => SetValue(ref _desiredReleaseYear, value);
        }

        [DontSerialize]
        public bool[] ReleaseYearChoices
        {
            get => _releaseYearChoices;
        }

        public ReleaseYearChoice ReleaseYearChoice
        {
            get
            {
                var choice = Array.IndexOf(_releaseYearChoices, true);
                if (choice == -1)
                {
                    choice = 0;
                }

                return (ReleaseYearChoice)choice;
            }
            set
            {
                var newValue = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
                newValue[(int)value] = true;
                _releaseYearChoices = newValue;
                OnPropertyChanged(nameof(ReleaseYearChoices));
            }
        }

        public int NumberOfTopGames
        {
            get => _numberOfTopGames;
            set => SetValue(ref _numberOfTopGames, value);
        }

        public int RecentDays
        {
            get => _recentDays;
            set => SetValue(ref _recentDays, value);
        }

        [DontSerialize]
        public bool[] UnplayedGameDefinitions
        {
            get => _unplayedGameDefinitions;
        }

        public UnplayedGameDefinition UnplayedGameDefinition
        {
            get
            {
                var choice = Array.IndexOf(_unplayedGameDefinitions, true);
                if (choice == -1)
                {
                    choice = 0;
                }

                return (UnplayedGameDefinition)choice;
            }
            set
            {
                var newValue = new bool[Enum.GetValues(typeof(UnplayedGameDefinition)).Length];
                newValue[(int)value] = true;
                _unplayedGameDefinitions = newValue;
                OnPropertyChanged(nameof(UnplayedGameDefinitions));
            }
        }

        public Guid[] UnplayedCompletionStatuses { get; set; }

        public int Version { get; set; }

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