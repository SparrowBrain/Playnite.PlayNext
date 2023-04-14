using System;
using System.Collections.Generic;
using PlayNext.Model.Data;
using Playnite.SDK.Data;

namespace PlayNext
{
    public class PlayNextSettings : ObservableObject
    {
        public const int MaxWeightValue = 100;
        public const int MinWeightValue = 0;

        private int _desiredReleaseYear;
        private bool[] _releaseYearChoices = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
        private int _numberOfTopGames;
        private int _recentDays;

        public PlayNextSettings()
        {
        }

        private PlayNextSettings(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights)
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            NumberOfTopGames = 30;
            RecentDays = 14;
        }

        public static PlayNextSettings Default => new PlayNextSettings(AttributeCalculationWeights.Default, GameScoreWeights.Default);

        public float TotalPlaytimeSerialized { get; set; }

        public float RecentPlaytimeSerialized { get; set; }

        public float RecentOrderSerialized { get; set; }

        public float GenreSerialized { get; set; }

        public float FeatureSerialized { get; set; }

        public float DeveloperSerialized { get; set; }

        public float PublisherSerialized { get; set; }

        public float TagSerialized { get; set; }

        public float CriticScoreSerialized { get; set; }

        public float CommunityScoreSerialized { get; set; }

        public float ReleaseYearSerialized { get; set; }

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

        public void SetAttributeWeights(AttributeCalculationWeights attributeCalculationWeights)
        {
            TotalPlaytimeSerialized = attributeCalculationWeights.TotalPlaytime * MaxWeightValue;
            RecentPlaytimeSerialized = attributeCalculationWeights.RecentPlaytime * MaxWeightValue;
            RecentOrderSerialized = attributeCalculationWeights.RecentOrder * MaxWeightValue;
        }

        public void SetGameWeights(GameScoreWeights gameScoreWeights)
        {
            GenreSerialized = gameScoreWeights.Genre * MaxWeightValue;
            FeatureSerialized = gameScoreWeights.Feature * MaxWeightValue;
            DeveloperSerialized = gameScoreWeights.Developer * MaxWeightValue;
            PublisherSerialized = gameScoreWeights.Publisher * MaxWeightValue;
            TagSerialized = gameScoreWeights.Tag * MaxWeightValue;
            CriticScoreSerialized = gameScoreWeights.CriticScore * MaxWeightValue;
            CommunityScoreSerialized = gameScoreWeights.CommunityScore * MaxWeightValue;
            ReleaseYearSerialized = gameScoreWeights.ReleaseYear * MaxWeightValue;
        }
    }
}