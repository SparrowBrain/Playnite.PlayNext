using PlayNext.Extensions.StartPage;
using PlayNext.Model.Data;
using PlayNext.Settings;
using PlayNext.Settings.Presets;
using PlayNext.Views;
using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PlayNext.ViewModels
{
	public class PlayNextSettingsViewModel : ObservableObject, ISettings
	{
		private readonly PlayNext _plugin;
		private readonly SettingsPresetManager _settingsPresetManager;

		private PlayNextSettings _editingClone;
		private PlayNextSettings _settings;
		private ObservableCollection<CompletionStatusItem> _unplayedCompletionStatuses;
		private ObservableCollection<SettingsPreset<PlayNextSettings>> _presets;
		private SettingsPreset<PlayNextSettings> _selectedPreset;

		public PlayNextSettingsViewModel(PlayNext plugin, SettingsPresetManager settingsPresetManager)
		{
			_plugin = plugin;
			_settingsPresetManager = settingsPresetManager;


			Sources = new ExclusionList<GameSource>(
				() => _plugin.PlayniteApi.Database.Sources.ToList(),
				s => s.ExcludedSourceIds);
			Platforms = new ExclusionList<Platform>(
				() => _plugin.PlayniteApi.Database.Platforms.ToList(),
				s => s.ExcludedPlatformIds);
			Categories = new ExclusionList<Category>(
				() => _plugin.PlayniteApi.Database.Categories.ToList(),
				s => s.ExcludedCategoryIds);
			Tags = new ExclusionList<Tag>(
				() => _plugin.PlayniteApi.Database.Tags.ToList(),
				s => s.ExcludedTagIds);


			var savedSettings = plugin.LoadPluginSettings<PlayNextSettings>();
			Settings = savedSettings ?? PlayNextSettings.Default;
			GameActivityExtensionFound = _plugin.GameActivityExtension.GameActivityPathExists();
			HowLongToBeatExtensionFound = _plugin.HowLongToBeatExtension.DoesDataExist();
			StartPageExtensionFound = LandingPageExtension.Instance != null;

			InitializePresets();
			InitializeUnplayedCompletionStatuses();
		}

		public PlayNextSettings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				Sources.Settings = value;
				Platforms.Settings = value;
				Categories.Settings = value;
				Tags.Settings = value;
				OnPropertyChanged(string.Empty);
			}
		}

		public bool GameActivityExtensionFound { get; }

		public bool HowLongToBeatExtensionFound { get; }

		public bool StartPageExtensionFound { get; }

		public ObservableCollection<SettingsPreset<PlayNextSettings>> Presets
		{
			get => _presets;
			set => SetValue(ref _presets, value);
		}

		public SettingsPreset<PlayNextSettings> SelectedPreset
		{
			get => _selectedPreset;
			set
			{
				SetValue(ref _selectedPreset, value);
				if (value != null && Settings.SelectedPresetId != value.Id)
				{
					Settings = value.Settings;
					InitializeUnplayedCompletionStatuses();
				}
			}
		}

		public ICommand AddPreset => new RelayCommand(ShowCreatePresetDialog);

		public ICommand RemovePreset => new RelayCommand(() =>
		{
		});

		public ICommand SavePreset => new RelayCommand(() =>
		{
			var preset = Presets.FirstOrDefault(x => x.Id == Settings.SelectedPresetId);
			if (preset == null)
			{
				ShowCreatePresetDialog();
				return;
			}

			preset.Settings = Settings;
			_settingsPresetManager.WritePreset(preset);
		});

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

		public ICommand SetAllToDefault => new RelayCommand(() =>
		{
			Settings = PlayNextSettings.Default;
			InitializeUnplayedCompletionStatuses();
			NotifyAttributeScoreSourcePropertiesChanged();
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

		public float SeriesWeight
		{
			get => Settings.SeriesWeight;
			set
			{
				var difference = (value - Settings.SeriesWeight) / (GameScoreWeights.Number - 1);
				RebalanceGameScoreWeights(difference);
				Settings.SeriesWeight = value;
				PushGameScoreWeightsToTotal(nameof(Settings.SeriesWeight));
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

		public float GameLengthWeight
		{
			get => Settings.GameLengthWeight;
			set
			{
				var difference = (value - Settings.GameLengthWeight) / (GameScoreWeights.Number - 1);
				RebalanceGameScoreWeights(difference);
				Settings.GameLengthWeight = value;
				PushGameScoreWeightsToTotal(nameof(Settings.GameLengthWeight));
				NotifyGameScoreSourcePropertiesChanged();
			}
		}

		public ObservableCollection<CompletionStatusItem> UnplayedCompletionStatuses
		{
			get => _unplayedCompletionStatuses;
			set
			{
				SetValue(ref _unplayedCompletionStatuses, value);
				OnPropertyChanged(nameof(SelectedCompletionStatusesText));
			}
		}

		public string SelectedCompletionStatusesText
		{
			get => string.Join(", ", UnplayedCompletionStatuses?.Where(x => x.IsChecked).Select(x => x.Name) ?? Enumerable.Empty<string>());
			set => OnPropertyChanged();
		}

		public Dictionary<OrderSeriesBy, string> OrderSeriesByOptions { get; } =
			new Dictionary<OrderSeriesBy, string>()
			{
				{OrderSeriesBy.ReleaseDate, ResourceProvider.GetString("LOC_PlayNext_SettingsSeriesOrderedByReleaseDate")},
				{OrderSeriesBy.SortingName, ResourceProvider.GetString("LOC_PlayNext_SettingsSeriesOrderedBySortingName")},
			};
		
		public ExclusionList<GameSource> Sources { get; }


		public ExclusionList<Platform> Platforms { get; }

		public ExclusionList<Category> Categories { get; }

		public ExclusionList<Tag> Tags { get; }

		public void BeginEdit()
		{
			_editingClone = Serialization.GetClone(Settings);
		}

		public void CancelEdit()
		{
			Settings = _editingClone;
			InitializeUnplayedCompletionStatuses();
		}

		public void EndEdit()
		{
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

		private void InitializeUnplayedCompletionStatuses()
		{
			UnplayedCompletionStatuses = _plugin.PlayniteApi.Database.CompletionStatuses
				.Select(item => new CompletionStatusItem(item.Id, item.Name, this,
					Settings.UnplayedCompletionStatuses?.Contains(item.Id) ?? false)).OrderBy(x => x.Name).ToObservable();
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
			Settings.SeriesWeight = ContainInMinMax(Settings.SeriesWeight - difference);
			Settings.CriticScoreWeight = ContainInMinMax(Settings.CriticScoreWeight - difference);
			Settings.CommunityScoreWeight = ContainInMinMax(Settings.CommunityScoreWeight - difference);
			Settings.ReleaseYearWeight = ContainInMinMax(Settings.ReleaseYearWeight - difference);
			Settings.GameLengthWeight = ContainInMinMax(Settings.GameLengthWeight - difference);
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

			if (ignore != nameof(Settings.SeriesWeight))
			{
				Settings.SeriesWeight = ContainInMinMax(Settings.SeriesWeight + GetMissingGameWeightToTotal());
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

			if (ignore != nameof(Settings.GameLengthWeight))
			{
				Settings.GameLengthWeight = ContainInMinMax(Settings.GameLengthWeight + GetMissingGameWeightToTotal());
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
			OnPropertyChanged(nameof(SeriesWeight));
			OnPropertyChanged(nameof(CriticScoreWeight));
			OnPropertyChanged(nameof(CommunityScoreWeight));
			OnPropertyChanged(nameof(ReleaseYearWeight));
			OnPropertyChanged(nameof(GameLengthWeight));
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
			return PlayNextSettings.MaxWeightValue
				   - Settings.GenreWeight
				   - Settings.FeatureWeight
				   - Settings.DeveloperWeight
				   - Settings.PublisherWeight
				   - Settings.TagWeight
				   - Settings.SeriesWeight
				   - Settings.CriticScoreWeight
				   - Settings.CommunityScoreWeight
				   - Settings.ReleaseYearWeight
				   - Settings.GameLengthWeight;
		}

		private void InitializePresets()
		{
			Presets = _settingsPresetManager.GetPersistedPresets().ToObservable();
			SelectedPreset = Presets.FirstOrDefault(x => x.Id == Settings.SelectedPresetId);
		}

		private void CreatePreset(string name)
		{
			var presetId = Guid.NewGuid();
			Settings.SelectedPresetId = presetId;

			var preset = new SettingsPreset<PlayNextSettings>()
			{
				Id = presetId,
				Name = name,
				Settings = Settings,
			};

			_settingsPresetManager.WritePreset(preset);
			Presets = _settingsPresetManager.GetPersistedPresets().ToObservable();
			SelectedPreset = Presets.FirstOrDefault(x => x.Id == presetId);
			EndEdit();
		}

		private void ShowCreatePresetDialog()
		{
			var viewModel = new CreatePresetViewModel(CreatePreset);
			var view = new CreatePresetView(viewModel);

			var window = _plugin.PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions()
			{
				ShowCloseButton = true,
				ShowMaximizeButton = false,
				ShowMinimizeButton = false,
			});

			window.Height = 200;
			window.Width = 500;

			window.Content = view;
			viewModel.AssociateWindow(window);

			window.Owner = _plugin.PlayniteApi.Dialogs.GetCurrentAppWindow();
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			window.ShowDialog();
		}
	}
}