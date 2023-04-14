using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PlayNext.Model.Data;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace PlayNext.ViewModels
{
    public class PlayNextSettingsViewModel : ObservableObject, ISettings
    {
        private readonly PlayNext _plugin;
        private PlayNextSettings EditingClone { get; set; }

        private PlayNextSettings _settings;

        public PlayNextSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        public PlayNextSettingsViewModel(PlayNext plugin)
        {
            _plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<PlayNextSettings>();
            Settings = savedSettings ?? PlayNextSettings.Default;
        }

        public ICommand SetAttributeWeightsToFlat => new RelayCommand(() =>
        {
            Settings.SetAttributeWeights(AttributeCalculationWeights.Flat);
            NotifyAttributeScoreSourcePropertiesChanged();
        });

        public ICommand SetGameWeightsToFlat => new RelayCommand(() =>
        {
            Settings.SetGameWeights(GameScoreWeights.Flat);
            NotifyGameScoreSourcePropertiesChanged();
        });

        public ICommand SetAttributeWeightsToDefault => new RelayCommand(() =>
        {
            Settings.SetAttributeWeights(AttributeCalculationWeights.Default);
            NotifyAttributeScoreSourcePropertiesChanged();
        });

        public ICommand SetGameWeightsToDefault => new RelayCommand(() =>
        {
            Settings.SetGameWeights(GameScoreWeights.Default);
            NotifyGameScoreSourcePropertiesChanged();
        });

        public float TotalPlaytimeUi
        {
            get => Settings.TotalPlaytimeSerialized;
            set
            {
                var difference = (value - Settings.TotalPlaytimeSerialized) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.TotalPlaytimeSerialized = value;
                PushAttributeWeightsToTotal(nameof(Settings.TotalPlaytimeSerialized));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentPlaytimeUi
        {
            get => Settings.RecentPlaytimeSerialized;
            set
            {
                var difference = (value - Settings.RecentPlaytimeSerialized) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentPlaytimeSerialized = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentPlaytimeSerialized));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentOrderUi
        {
            get => Settings.RecentOrderSerialized;
            set
            {
                var difference = (value - Settings.RecentOrderSerialized) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentOrderSerialized = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentOrderSerialized));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float GenreUi
        {
            get => Settings.GenreSerialized;
            set
            {
                var difference = (value - Settings.GenreSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.GenreSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.GenreSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float FeatureUi
        {
            get => Settings.FeatureSerialized;
            set
            {
                var difference = (value - Settings.FeatureSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.FeatureSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.FeatureSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float DeveloperUi
        {
            get => Settings.DeveloperSerialized;
            set
            {
                var difference = (value - Settings.DeveloperSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.DeveloperSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.DeveloperSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float PublisherUi
        {
            get => Settings.PublisherSerialized;
            set
            {
                var difference = (value - Settings.PublisherSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.PublisherSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.PublisherSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float TagUi
        {
            get => Settings.TagSerialized;
            set
            {
                var difference = (value - Settings.TagSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.TagSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.TagSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CriticScoreUi
        {
            get => Settings.CriticScoreSerialized;
            set
            {
                var difference = (value - Settings.CriticScoreSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CriticScoreSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CriticScoreSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CommunityScoreUi
        {
            get => Settings.CommunityScoreSerialized;
            set
            {
                var difference = (value - Settings.CommunityScoreSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CommunityScoreSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CommunityScoreSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float ReleaseYearUi
        {
            get => Settings.ReleaseYearSerialized;
            set
            {
                var difference = (value - Settings.ReleaseYearSerialized) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.ReleaseYearSerialized = value;
                PushGameScoreWeightsToTotal(nameof(Settings.ReleaseYearSerialized));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            EditingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = EditingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            _plugin.SavePluginSettings(Settings);
            _plugin.OnPlayNextSettingsSaved();
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();

            if (!DateTime.TryParse($"{Settings.DesiredReleaseYear}-01-01", out var year))
            {
                errors.Add(ResourceProvider.GetString("LOC_PlayNext_SettingsValidationFailureToParseYear"));
            }

            return !errors.Any();
        }

        private void RebalanceAttributeScoreSourceWeights(float difference)
        {
            Settings.TotalPlaytimeSerialized = ContainInMinMax(Settings.TotalPlaytimeSerialized - difference);
            Settings.RecentPlaytimeSerialized = ContainInMinMax(Settings.RecentPlaytimeSerialized - difference);
            Settings.RecentOrderSerialized = ContainInMinMax(Settings.RecentOrderSerialized - difference);
        }

        private void RebalanceGameScoreWeights(float difference)
        {
            Settings.GenreSerialized = ContainInMinMax(Settings.GenreSerialized - difference);
            Settings.FeatureSerialized = ContainInMinMax(Settings.FeatureSerialized - difference);
            Settings.DeveloperSerialized = ContainInMinMax(Settings.DeveloperSerialized - difference);
            Settings.PublisherSerialized = ContainInMinMax(Settings.PublisherSerialized - difference);
            Settings.TagSerialized = ContainInMinMax(Settings.TagSerialized - difference);
            Settings.CriticScoreSerialized = ContainInMinMax(Settings.CriticScoreSerialized - difference);
            Settings.CommunityScoreSerialized = ContainInMinMax(Settings.CommunityScoreSerialized - difference);
            Settings.ReleaseYearSerialized = ContainInMinMax(Settings.ReleaseYearSerialized - difference);
        }

        private void PushGameScoreWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.GenreSerialized))
            {
                Settings.GenreSerialized = ContainInMinMax(Settings.GenreSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.FeatureSerialized))
            {
                Settings.FeatureSerialized = ContainInMinMax(Settings.FeatureSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.DeveloperSerialized))
            {
                Settings.DeveloperSerialized = ContainInMinMax(Settings.DeveloperSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.PublisherSerialized))
            {
                Settings.PublisherSerialized = ContainInMinMax(Settings.PublisherSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.TagSerialized))
            {
                Settings.TagSerialized = ContainInMinMax(Settings.TagSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CriticScoreSerialized))
            {
                Settings.CriticScoreSerialized = ContainInMinMax(Settings.CriticScoreSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CommunityScoreSerialized))
            {
                Settings.CommunityScoreSerialized = ContainInMinMax(Settings.CommunityScoreSerialized + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.ReleaseYearSerialized))
            {
                Settings.ReleaseYearSerialized = ContainInMinMax(Settings.ReleaseYearSerialized + GetMissingGameWeightToTotal());
            }
        }

        private void PushAttributeWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.TotalPlaytimeSerialized))
            {
                Settings.TotalPlaytimeSerialized = ContainInMinMax(Settings.TotalPlaytimeSerialized + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentOrderSerialized))
            {
                Settings.RecentOrderSerialized = ContainInMinMax(Settings.RecentOrderSerialized + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentPlaytimeSerialized))
            {
                Settings.RecentPlaytimeSerialized = ContainInMinMax(Settings.RecentPlaytimeSerialized + GetMissingAttributeWeightToTotal());
            }
        }

        private void NotifyAttributeScoreSourcePropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalPlaytimeUi));
            OnPropertyChanged(nameof(RecentPlaytimeUi));
            OnPropertyChanged(nameof(RecentOrderUi));
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
            return Math.Max(PlayNextSettings.MinWeightValue, Math.Min(PlayNextSettings.MaxWeightValue, newValue));
        }

        private float GetMissingAttributeWeightToTotal()
        {
            return PlayNextSettings.MaxWeightValue - Settings.TotalPlaytimeSerialized - Settings.RecentPlaytimeSerialized - Settings.RecentOrderSerialized;
        }

        private float GetMissingGameWeightToTotal()
        {
            return PlayNextSettings.MaxWeightValue - Settings.GenreSerialized - Settings.FeatureSerialized - Settings.DeveloperSerialized - Settings.PublisherSerialized - Settings.TagSerialized - Settings.CriticScoreSerialized - Settings.CommunityScoreSerialized - Settings.ReleaseYearSerialized;
        }
    }
}