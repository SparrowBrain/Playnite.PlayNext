using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PlayNext.Model.Data;
using PlayNext.Settings;
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
            get => Settings.TotalPlaytime;
            set
            {
                var difference = (value - Settings.TotalPlaytime) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.TotalPlaytime = value;
                PushAttributeWeightsToTotal(nameof(Settings.TotalPlaytime));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentPlaytimeUi
        {
            get => Settings.RecentPlaytime;
            set
            {
                var difference = (value - Settings.RecentPlaytime) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentPlaytime = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentPlaytime));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentOrderUi
        {
            get => Settings.RecentOrder;
            set
            {
                var difference = (value - Settings.RecentOrder) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentOrder = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentOrder));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float GenreUi
        {
            get => Settings.Genre;
            set
            {
                var difference = (value - Settings.Genre) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.Genre = value;
                PushGameScoreWeightsToTotal(nameof(Settings.Genre));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float FeatureUi
        {
            get => Settings.Feature;
            set
            {
                var difference = (value - Settings.Feature) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.Feature = value;
                PushGameScoreWeightsToTotal(nameof(Settings.Feature));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float DeveloperUi
        {
            get => Settings.Developer;
            set
            {
                var difference = (value - Settings.Developer) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.Developer = value;
                PushGameScoreWeightsToTotal(nameof(Settings.Developer));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float PublisherUi
        {
            get => Settings.Publisher;
            set
            {
                var difference = (value - Settings.Publisher) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.Publisher = value;
                PushGameScoreWeightsToTotal(nameof(Settings.Publisher));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float TagUi
        {
            get => Settings.Tag;
            set
            {
                var difference = (value - Settings.Tag) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.Tag = value;
                PushGameScoreWeightsToTotal(nameof(Settings.Tag));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CriticScoreUi
        {
            get => Settings.CriticScore;
            set
            {
                var difference = (value - Settings.CriticScore) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CriticScore = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CriticScore));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CommunityScoreUi
        {
            get => Settings.CommunityScore;
            set
            {
                var difference = (value - Settings.CommunityScore) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CommunityScore = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CommunityScore));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float ReleaseYearUi
        {
            get => Settings.ReleaseYear;
            set
            {
                var difference = (value - Settings.ReleaseYear) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.ReleaseYear = value;
                PushGameScoreWeightsToTotal(nameof(Settings.ReleaseYear));
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
            Settings.TotalPlaytime = ContainInMinMax(Settings.TotalPlaytime - difference);
            Settings.RecentPlaytime = ContainInMinMax(Settings.RecentPlaytime - difference);
            Settings.RecentOrder = ContainInMinMax(Settings.RecentOrder - difference);
        }

        private void RebalanceGameScoreWeights(float difference)
        {
            Settings.Genre = ContainInMinMax(Settings.Genre - difference);
            Settings.Feature = ContainInMinMax(Settings.Feature - difference);
            Settings.Developer = ContainInMinMax(Settings.Developer - difference);
            Settings.Publisher = ContainInMinMax(Settings.Publisher - difference);
            Settings.Tag = ContainInMinMax(Settings.Tag - difference);
            Settings.CriticScore = ContainInMinMax(Settings.CriticScore - difference);
            Settings.CommunityScore = ContainInMinMax(Settings.CommunityScore - difference);
            Settings.ReleaseYear = ContainInMinMax(Settings.ReleaseYear - difference);
        }

        private void PushGameScoreWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.Genre))
            {
                Settings.Genre = ContainInMinMax(Settings.Genre + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.Feature))
            {
                Settings.Feature = ContainInMinMax(Settings.Feature + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.Developer))
            {
                Settings.Developer = ContainInMinMax(Settings.Developer + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.Publisher))
            {
                Settings.Publisher = ContainInMinMax(Settings.Publisher + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.Tag))
            {
                Settings.Tag = ContainInMinMax(Settings.Tag + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CriticScore))
            {
                Settings.CriticScore = ContainInMinMax(Settings.CriticScore + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CommunityScore))
            {
                Settings.CommunityScore = ContainInMinMax(Settings.CommunityScore + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.ReleaseYear))
            {
                Settings.ReleaseYear = ContainInMinMax(Settings.ReleaseYear + GetMissingGameWeightToTotal());
            }
        }

        private void PushAttributeWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.TotalPlaytime))
            {
                Settings.TotalPlaytime = ContainInMinMax(Settings.TotalPlaytime + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentOrder))
            {
                Settings.RecentOrder = ContainInMinMax(Settings.RecentOrder + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentPlaytime))
            {
                Settings.RecentPlaytime = ContainInMinMax(Settings.RecentPlaytime + GetMissingAttributeWeightToTotal());
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
            return PlayNextSettings.MaxWeightValue - Settings.TotalPlaytime - Settings.RecentPlaytime - Settings.RecentOrder;
        }

        private float GetMissingGameWeightToTotal()
        {
            return PlayNextSettings.MaxWeightValue - Settings.Genre - Settings.Feature - Settings.Developer - Settings.Publisher - Settings.Tag - Settings.CriticScore - Settings.CommunityScore - Settings.ReleaseYear;
        }
    }
}