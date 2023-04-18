﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            UnplayedCompletionStatuses = _plugin.PlayniteApi.Database.CompletionStatuses
                .Select(item => new CompletionStatusItem(item.Id, item.Name, this, Settings.UnplayedCompletionStatuses?.Contains(item.Id) ?? false)).OrderBy(x => x.Name).ToObservable();
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

        public float TotalPlaytimeWeight
        {
            get => Settings.TotalPlaytimeWeight;
            set
            {
                var difference = (value - Settings.TotalPlaytimeWeight) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.TotalPlaytimeWeight = value;
                PushAttributeWeightsToTotal(nameof(Settings.TotalPlaytimeWeight));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentPlaytimeWeight
        {
            get => Settings.RecentPlaytimeWeight;
            set
            {
                var difference = (value - Settings.RecentPlaytimeWeight) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentPlaytimeWeight = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentPlaytimeWeight));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float RecentOrderWeight
        {
            get => Settings.RecentOrderWeight;
            set
            {
                var difference = (value - Settings.RecentOrderWeight) / (AttributeCalculationWeights.Number - 1);
                RebalanceAttributeScoreSourceWeights(difference);
                Settings.RecentOrderWeight = value;
                PushAttributeWeightsToTotal(nameof(Settings.RecentOrderWeight));
                NotifyAttributeScoreSourcePropertiesChanged();
            }
        }

        public float GenreWeight
        {
            get => Settings.GenreWeight;
            set
            {
                var difference = (value - Settings.GenreWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.GenreWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.GenreWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float FeatureWeight
        {
            get => Settings.FeatureWeight;
            set
            {
                var difference = (value - Settings.FeatureWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.FeatureWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.FeatureWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float DeveloperWeight
        {
            get => Settings.DeveloperWeight;
            set
            {
                var difference = (value - Settings.DeveloperWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.DeveloperWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.DeveloperWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float PublisherWeight
        {
            get => Settings.PublisherWeight;
            set
            {
                var difference = (value - Settings.PublisherWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.PublisherWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.PublisherWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float TagWeight
        {
            get => Settings.TagWeight;
            set
            {
                var difference = (value - Settings.TagWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.TagWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.TagWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CriticScoreWeight
        {
            get => Settings.CriticScoreWeight;
            set
            {
                var difference = (value - Settings.CriticScoreWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CriticScoreWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CriticScoreWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float CommunityScoreWeight
        {
            get => Settings.CommunityScoreWeight;
            set
            {
                var difference = (value - Settings.CommunityScoreWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.CommunityScoreWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.CommunityScoreWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public float ReleaseYearWeight
        {
            get => Settings.ReleaseYearWeight;
            set
            {
                var difference = (value - Settings.ReleaseYearWeight) / (GameScoreWeights.Number - 1);
                RebalanceGameScoreWeights(difference);
                Settings.ReleaseYearWeight = value;
                PushGameScoreWeightsToTotal(nameof(Settings.ReleaseYearWeight));
                NotifyGameScoreSourcePropertiesChanged();
            }
        }

        public ObservableCollection<CompletionStatusItem> UnplayedCompletionStatuses { get; }

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
            Settings.TotalPlaytimeWeight = ContainInMinMax(Settings.TotalPlaytimeWeight - difference);
            Settings.RecentPlaytimeWeight = ContainInMinMax(Settings.RecentPlaytimeWeight - difference);
            Settings.RecentOrderWeight = ContainInMinMax(Settings.RecentOrderWeight - difference);
        }

        private void RebalanceGameScoreWeights(float difference)
        {
            Settings.GenreWeight = ContainInMinMax(Settings.GenreWeight - difference);
            Settings.FeatureWeight = ContainInMinMax(Settings.FeatureWeight - difference);
            Settings.DeveloperWeight = ContainInMinMax(Settings.DeveloperWeight - difference);
            Settings.PublisherWeight = ContainInMinMax(Settings.PublisherWeight - difference);
            Settings.TagWeight = ContainInMinMax(Settings.TagWeight - difference);
            Settings.CriticScoreWeight = ContainInMinMax(Settings.CriticScoreWeight - difference);
            Settings.CommunityScoreWeight = ContainInMinMax(Settings.CommunityScoreWeight - difference);
            Settings.ReleaseYearWeight = ContainInMinMax(Settings.ReleaseYearWeight - difference);
        }

        private void PushGameScoreWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.GenreWeight))
            {
                Settings.GenreWeight = ContainInMinMax(Settings.GenreWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.FeatureWeight))
            {
                Settings.FeatureWeight = ContainInMinMax(Settings.FeatureWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.DeveloperWeight))
            {
                Settings.DeveloperWeight = ContainInMinMax(Settings.DeveloperWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.PublisherWeight))
            {
                Settings.PublisherWeight = ContainInMinMax(Settings.PublisherWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.TagWeight))
            {
                Settings.TagWeight = ContainInMinMax(Settings.TagWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CriticScoreWeight))
            {
                Settings.CriticScoreWeight = ContainInMinMax(Settings.CriticScoreWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.CommunityScoreWeight))
            {
                Settings.CommunityScoreWeight = ContainInMinMax(Settings.CommunityScoreWeight + GetMissingGameWeightToTotal());
            }

            if (ignore != nameof(Settings.ReleaseYearWeight))
            {
                Settings.ReleaseYearWeight = ContainInMinMax(Settings.ReleaseYearWeight + GetMissingGameWeightToTotal());
            }
        }

        private void PushAttributeWeightsToTotal(string ignore)
        {
            if (ignore != nameof(Settings.TotalPlaytimeWeight))
            {
                Settings.TotalPlaytimeWeight = ContainInMinMax(Settings.TotalPlaytimeWeight + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentOrderWeight))
            {
                Settings.RecentOrderWeight = ContainInMinMax(Settings.RecentOrderWeight + GetMissingAttributeWeightToTotal());
            }

            if (ignore != nameof(Settings.RecentPlaytimeWeight))
            {
                Settings.RecentPlaytimeWeight = ContainInMinMax(Settings.RecentPlaytimeWeight + GetMissingAttributeWeightToTotal());
            }
        }

        private void NotifyAttributeScoreSourcePropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalPlaytimeWeight));
            OnPropertyChanged(nameof(RecentPlaytimeWeight));
            OnPropertyChanged(nameof(RecentOrderWeight));
        }

        private void NotifyGameScoreSourcePropertiesChanged()
        {
            OnPropertyChanged(nameof(GenreWeight));
            OnPropertyChanged(nameof(FeatureWeight));
            OnPropertyChanged(nameof(DeveloperWeight));
            OnPropertyChanged(nameof(PublisherWeight));
            OnPropertyChanged(nameof(TagWeight));
            OnPropertyChanged(nameof(CriticScoreWeight));
            OnPropertyChanged(nameof(CommunityScoreWeight));
            OnPropertyChanged(nameof(ReleaseYearWeight));
        }

        private float ContainInMinMax(float newValue)
        {
            return Math.Max(PlayNextSettings.MinWeightValue, Math.Min(PlayNextSettings.MaxWeightValue, newValue));
        }

        private float GetMissingAttributeWeightToTotal()
        {
            return PlayNextSettings.MaxWeightValue - Settings.TotalPlaytimeWeight - Settings.RecentPlaytimeWeight - Settings.RecentOrderWeight;
        }

        private float GetMissingGameWeightToTotal()
        {
            return PlayNextSettings.MaxWeightValue - Settings.GenreWeight - Settings.FeatureWeight - Settings.DeveloperWeight - Settings.PublisherWeight - Settings.TagWeight - Settings.CriticScoreWeight - Settings.CommunityScoreWeight - Settings.ReleaseYearWeight;
        }
    }
}