using System;
using System.Collections.Generic;
using PlayNext.Model.Data;
using PlayNext.Settings.Old;
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

            Version = CurrentVersion;
        }

        public static PlayNextSettings Default => new PlayNextSettings(AttributeCalculationWeights.Default, GameScoreWeights.Default);

        public static PlayNextSettings Migrate(SettingsV0 oldSettings)
        {
            if (oldSettings.Version != CurrentVersion - 1)
            {
                throw new ArgumentException($"Cannot migrate v{oldSettings.Version} settings to v{CurrentVersion}");
            }

            var settings = Default;
            settings.TotalPlaytime = oldSettings.TotalPlaytimeSerialized;
            settings.RecentPlaytime = oldSettings.RecentPlaytimeSerialized;
            settings.RecentOrder = oldSettings.RecentOrderSerialized;

            settings.Genre = oldSettings.GenreSerialized;
            settings.Feature = oldSettings.FeatureSerialized;
            settings.Developer = oldSettings.DeveloperSerialized;
            settings.Publisher = oldSettings.PublisherSerialized;
            settings.Tag = oldSettings.TagSerialized;
            settings.CriticScore = oldSettings.CriticScoreSerialized;
            settings.CommunityScore = oldSettings.CommunityScoreSerialized;
            settings.ReleaseYear = oldSettings.ReleaseYearSerialized;
            settings.ReleaseYearChoice = oldSettings.ReleaseYearChoice;
            settings.DesiredReleaseYear = oldSettings.DesiredReleaseYear;

            settings.RecentDays = oldSettings.RecentDays;
            settings.NumberOfTopGames = oldSettings.NumberOfTopGames;

            return settings;
        }

        public float TotalPlaytime { get; set; }

        public float RecentPlaytime { get; set; }

        public float RecentOrder { get; set; }

        public float Genre { get; set; }

        public float Feature { get; set; }

        public float Developer { get; set; }

        public float Publisher { get; set; }

        public float Tag { get; set; }

        public float CriticScore { get; set; }

        public float CommunityScore { get; set; }

        public float ReleaseYear { get; set; }

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
                OnPropertyChanged();
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

        public int Version { get; set; }

        public void SetAttributeWeights(AttributeCalculationWeights attributeCalculationWeights)
        {
            TotalPlaytime = attributeCalculationWeights.TotalPlaytime * MaxWeightValue;
            RecentPlaytime = attributeCalculationWeights.RecentPlaytime * MaxWeightValue;
            RecentOrder = attributeCalculationWeights.RecentOrder * MaxWeightValue;
        }

        public void SetGameWeights(GameScoreWeights gameScoreWeights)
        {
            Genre = gameScoreWeights.Genre * MaxWeightValue;
            Feature = gameScoreWeights.Feature * MaxWeightValue;
            Developer = gameScoreWeights.Developer * MaxWeightValue;
            Publisher = gameScoreWeights.Publisher * MaxWeightValue;
            Tag = gameScoreWeights.Tag * MaxWeightValue;
            CriticScore = gameScoreWeights.CriticScore * MaxWeightValue;
            CommunityScore = gameScoreWeights.CommunityScore * MaxWeightValue;
            ReleaseYear = gameScoreWeights.ReleaseYear * MaxWeightValue;
        }
    }
}