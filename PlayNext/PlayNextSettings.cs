using System;
using System.Collections.Generic;
using PlayNext.Model.Data;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace PlayNext
{
    public class PlayNextSettings : ObservableObject
    {
        public const int MaxWeightValue = 100;
        private const int MinWeightValue = 0;
        private static readonly ILogger Logger = LogManager.GetLogger();

        private float _totalPlaytime;
        private float _recentPlaytime;
        private float _recentOrder;
        private float _genre;
        private float _feature;
        private float _developer;
        private float _publisher;
        private float _tag;
        private float _criticScore;
        private float _communityScore;
        private float _releaseYear;
        private int _desiredReleaseYear;
        private bool[] _releaseYearChoices = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
        private int _numberOfTopGames;
        private int _recentDays;

        public PlayNextSettings()
        {
        }

        public PlayNextSettings(AttributeCalculationWeights attributeCalculationWeights, GameScoreWeights gameScoreWeights)
        {
            SetAttributeWeights(attributeCalculationWeights);
            SetGameWeights(gameScoreWeights);

            DesiredReleaseYear = 2000;
            ReleaseYearChoice = ReleaseYearChoice.Current;

            NumberOfTopGames = 30;
            RecentDays = 14;
        }

        public float TotalPlaytimeSerialized
        {
            get => _totalPlaytime;
            set => _totalPlaytime = value;
        }

        public float RecentPlaytimeSerialized
        {
            get => _recentPlaytime;
            set => _recentPlaytime = value;
        }

        public float RecentOrderSerialized
        {
            get => _recentOrder;
            set => _recentOrder = value;
        }

        public float GenreSerialized
        {
            get => _genre;
            set => _genre = value;
        }

        public float FeatureSerialized
        {
            get => _feature;
            set => _feature = value;
        }

        public float DeveloperSerialized
        {
            get => _developer;
            set => _developer = value;
        }

        public float PublisherSerialized
        {
            get => _publisher;
            set => _publisher = value;
        }

        public float TagSerialized
        {
            get => _tag;
            set => _tag = value;
        }

        public float CriticScoreSerialized
        {
            get => _criticScore;
            set => _criticScore = value;
        }

        public float CommunityScoreSerialized
        {
            get => _communityScore;
            set => _communityScore = value;
        }

        public float ReleaseYearSerialized
        {
            get => _releaseYear;
            set => _releaseYear = value;
        }

        [DontSerialize]
        public float TotalPlaytimeUi
        {
            get => _totalPlaytime;
            set
            {
                var difference = (value - _totalPlaytime) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                _totalPlaytime = value;
                _recentOrder = ContainInMinMax(_recentOrder + GetMissingAttributeWeightToTotal());
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float RecentPlaytimeUi
        {
            get => _recentPlaytime;
            set
            {
                var difference = (value - _recentPlaytime) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                _recentPlaytime = value;
                _recentOrder = ContainInMinMax(_recentOrder + GetMissingAttributeWeightToTotal());
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float RecentOrderUi
        {
            get => _recentOrder;
            set
            {
                var difference = (value - _recentOrder) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                _recentOrder = value;
                _recentPlaytime = ContainInMinMax(_recentPlaytime + GetMissingAttributeWeightToTotal());
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float GenreUi
        {
            get => _genre;
            set
            {
                var difference = (value - _genre) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _genre = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float FeatureUi
        {
            get => _feature;
            set
            {
                var difference = (value - _feature) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _feature = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float DeveloperUi
        {
            get => _developer;
            set
            {
                var difference = (value - _developer) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _developer = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float PublisherUi
        {
            get => _publisher;
            set
            {
                var difference = (value - _publisher) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _publisher = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float TagUi
        {
            get => _tag;
            set
            {
                var difference = (value - _tag) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _tag = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float CriticScoreUi
        {
            get => _criticScore;
            set
            {
                var difference = (value - _criticScore) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _criticScore = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float CommunityScoreUi
        {
            get => _communityScore;
            set
            {
                var difference = (value - _communityScore) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _communityScore = value;
                _releaseYear = ContainInMinMax(_releaseYear + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        [DontSerialize]
        public float ReleaseYearUi
        {
            get => _releaseYear;
            set
            {
                var difference = (value - _releaseYear) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                _releaseYear = value;
                _genre = ContainInMinMax(_genre + GetMissingGameWeightToTotal());
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

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
            get => (ReleaseYearChoice)Array.IndexOf(_releaseYearChoices, true);
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
            _totalPlaytime = attributeCalculationWeights.TotalPlaytime * MaxWeightValue;
            _recentPlaytime = attributeCalculationWeights.RecentPlaytime * MaxWeightValue;
            _recentOrder = attributeCalculationWeights.RecentOrder * MaxWeightValue;
            NotifyAttributeScoreSourcePropertiesChanged();
        }

        public void SetGameWeights(GameScoreWeights gameScoreWeights)
        {
            _genre = gameScoreWeights.Genre * MaxWeightValue;
            _feature = gameScoreWeights.Feature * MaxWeightValue;
            _developer = gameScoreWeights.Developer * MaxWeightValue;
            _publisher = gameScoreWeights.Publisher * MaxWeightValue;
            _tag = gameScoreWeights.Tag * MaxWeightValue;
            _criticScore = gameScoreWeights.CriticScore * MaxWeightValue;
            _communityScore = gameScoreWeights.CommunityScore * MaxWeightValue;
            _releaseYear = gameScoreWeights.ReleaseYear * MaxWeightValue;
            NotifyGameScoreSourcePropertiesChanged();
        }

        private void RebalanceAttributeScoreSourceWeights(float difference)
        {
            _totalPlaytime = ContainInMinMax(_totalPlaytime - difference);
            _recentPlaytime = ContainInMinMax(_recentPlaytime - difference);
            _recentOrder = ContainInMinMax(_recentOrder - difference);
        }

        private float GetMissingAttributeWeightToTotal()
        {
            return MaxWeightValue - _totalPlaytime - _recentPlaytime - _recentOrder;
        }

        private void NotifyAttributeScoreSourcePropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalPlaytimeUi));
            OnPropertyChanged(nameof(RecentPlaytimeUi));
            OnPropertyChanged(nameof(RecentOrderUi));
        }

        private void RebalanceGameScoreWeights(float difference)
        {
            _genre = ContainInMinMax(_genre - difference);
            _feature = ContainInMinMax(_feature - difference);
            _developer = ContainInMinMax(_developer - difference);
            _publisher = ContainInMinMax(_publisher - difference);
            _tag = ContainInMinMax(_tag - difference);
            _criticScore = ContainInMinMax(_criticScore - difference);
            _communityScore = ContainInMinMax(_communityScore - difference);
            _releaseYear = ContainInMinMax(_releaseYear - difference);
        }

        private float GetMissingGameWeightToTotal()
        {
            return MaxWeightValue - _genre - _feature - _developer - _publisher - _tag - _criticScore - _communityScore - _releaseYear;
        }

        private void NotifyGameScoreSourcePropertiesChanged()
        {
            OnPropertyChanged(nameof(GenreUi));
            OnPropertyChanged(nameof(FeatureUi));
            OnPropertyChanged(nameof(DeveloperUi));
            OnPropertyChanged(nameof(PublisherUi));
            OnPropertyChanged(nameof(TagUi));
            OnPropertyChanged(nameof(CriticScoreUi));
            OnPropertyChanged(nameof(CommunityScoreUi));
            OnPropertyChanged(nameof(ReleaseYearUi));
        }

        private float ContainInMinMax(float newValue)
        {
            return Math.Max(MinWeightValue, Math.Min(MaxWeightValue, newValue));
        }
    }
}