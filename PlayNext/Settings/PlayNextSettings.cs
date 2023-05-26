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
        public const int CurrentVersion = 2;

        private int _desiredReleaseYear;
        private bool[] _releaseYearChoices = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
        private int _numberOfTopGames;

        private int _recentDays;
        private bool _unplayedGameIsWithCompletionStatus;
        private bool _unplayedGameIsWithZeroTime;
        private bool _startPageShowLabel;
        private bool _startPageLabelIsHorizontal;

        public PlayNextSettings()
        {
            Version = CurrentVersion;
        }

        private PlayNextSettings(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights) : this()
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            NumberOfTopGames = 30;
            RecentDays = 14;
            UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;
            UnplayedCompletionStatuses = Array.Empty<Guid>();

            StartPageShowLabel = true;
            StartPageLabelIsHorizontal = false;
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
        public bool UnplayedGameIsWithZeroTime
        {
            get => _unplayedGameIsWithZeroTime;
            set => SetValue(ref _unplayedGameIsWithZeroTime, value);
        }

        [DontSerialize]
        public bool UnplayedGameIsWithCompletionStatus
        {
            get => _unplayedGameIsWithCompletionStatus;
            set => SetValue(ref _unplayedGameIsWithCompletionStatus, value);
        }

        public UnplayedGameDefinition UnplayedGameDefinition
        {
            get
            {
                if (UnplayedGameIsWithZeroTime)
                {
                    return UnplayedGameDefinition.ZeroPlaytime;
                }

                if (UnplayedGameIsWithCompletionStatus)
                {
                    return UnplayedGameDefinition.SelectedCompletionStatuses;
                }

                return UnplayedGameDefinition.ZeroPlaytime;
            }
            set
            {
                switch (value)
                {
                    case UnplayedGameDefinition.SelectedCompletionStatuses:
                        UnplayedGameIsWithZeroTime = false;
                        UnplayedGameIsWithCompletionStatus = true;
                        break;

                    case UnplayedGameDefinition.ZeroPlaytime:
                    default:
                        UnplayedGameIsWithZeroTime = true;
                        UnplayedGameIsWithCompletionStatus = false;
                        break;
                }
            }
        }

        public Guid[] UnplayedCompletionStatuses { get; set; }

        public bool StartPageShowLabel
        {
            get => _startPageShowLabel;
            set => SetValue(ref _startPageShowLabel, value);
        }

        public bool StartPageLabelIsHorizontal
        {
            get => _startPageLabelIsHorizontal;
            set => SetValue(ref _startPageLabelIsHorizontal, value);
        }

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