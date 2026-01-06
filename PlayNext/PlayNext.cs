using PlayNext.Extensions.GameActivity;
using PlayNext.Extensions.HowLongToBeat;
using PlayNext.Extensions.StartPage;
using PlayNext.Extensions.StartPage.Settings.PlayNextAddon;
using PlayNext.Infrastructure.Services;
using PlayNext.Model.Filters;
using PlayNext.Model.Score;
using PlayNext.Settings;
using PlayNext.Settings.Presets;
using PlayNext.ViewModels;
using PlayNext.Views;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using StartPage.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlayNext
{
	public class PlayNext : GenericPlugin, IStartPageExtension
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly GameActivityExtension _gameActivities;
		private readonly HowLongToBeatExtension _howLongToBeatExtension;
		private readonly StartupSettingsValidator _startupSettingsValidator;
		private readonly SettingsPresetManager _settingsPresetManager;
		private readonly TotalScoreCalculator _totalScoreCalculator;
		private readonly DateTimeProvider _dateTimeProvider = new DateTimeProvider();
		private readonly Timer _gameUpdatedTimer = new Timer(5000);
		private readonly Dictionary<string, StartPagePlayNextViewModel> _startPagePlayNextViewModels = new Dictionary<string, StartPagePlayNextViewModel>();
		private readonly Dictionary<string, StartPagePlayNextView> _startPageViews = new Dictionary<string, StartPagePlayNextView>();

		private PlayNextSettingsViewModel _settings;
		private PlayNextMainViewModel _playNextMainViewModel;
		private PlayNextMainView _playNextMainView;
		private SidebarItem _sidebarItem;

		public override Guid Id { get; } = Guid.Parse("05234f92-39d3-4432-98c1-6f37a3e4b870");

		public GameActivityExtension GameActivityExtension => _gameActivities;
		public HowLongToBeatExtension HowLongToBeatExtension => _howLongToBeatExtension;

		public PlayNext(IPlayniteAPI api) : base(api)
		{
			Api = api;
			Properties = new GenericPluginProperties
			{
				HasSettings = true,
			};

			_gameActivities = GameActivityExtension.Create(_dateTimeProvider, api.Paths.ExtensionsDataPath);
			_howLongToBeatExtension = HowLongToBeatExtension.Create(api.Paths.ExtensionsDataPath);

			_totalScoreCalculator = new TotalScoreCalculator(this);

			var pluginSettingsPersistence = new PluginSettingsPersistence(this);
			var settingsMigrator = new SettingsMigrator(pluginSettingsPersistence);
			_startupSettingsValidator = new StartupSettingsValidator(pluginSettingsPersistence, settingsMigrator);
			_settingsPresetManager = new SettingsPresetManager(Path.Combine(api.Paths.ExtensionsDataPath, Id.ToString()), settingsMigrator);
			_gameUpdatedTimer.Elapsed += (o, e) => RefreshPlayNextData();
			_gameUpdatedTimer.AutoReset = false;
			_gameUpdatedTimer.Enabled = false;
		}

		public static IPlayniteAPI Api { get; private set; }

		public override IEnumerable<SidebarItem> GetSidebarItems()
		{
			if (_sidebarItem == null)
			{
				_sidebarItem = new SidebarItem
				{
					Title = ResourceProvider.GetString("LOC_PlayNext_PluginName"),
					Icon = new TextBlock()
					{
						Text = "\u23ED",
						FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
					},
					Type = SiderbarItemType.View,
					Opened = () =>
					{
						if (_playNextMainView == null || _playNextMainViewModel == null)
						{
							_playNextMainViewModel = new PlayNextMainViewModel(this);
							_playNextMainView = new PlayNextMainView(_playNextMainViewModel);
							RefreshPlayNextData();
						}

						return _playNextMainView;
					}
				};
			}

			yield return _sidebarItem;
		}

		public override void OnGameInstalled(OnGameInstalledEventArgs args)
		{
			_startPagePlayNextViewModels.ForEach(x => x.Value.UpdateGame(args.Game));
		}

		public override void OnGameStarted(OnGameStartedEventArgs args)
		{
			// Add code to be executed when game is started running.
		}

		public override void OnGameStarting(OnGameStartingEventArgs args)
		{
			// Add code to be executed when game is preparing to be started.
		}

		public override void OnGameStopped(OnGameStoppedEventArgs args)
		{
			RefreshPlayNextData();
		}

		public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
		{
			_startPagePlayNextViewModels.ForEach(x => x.Value.UpdateGame(args.Game));
		}

		public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
		{
			_startupSettingsValidator.EnsureCorrectVersionSettingsExist();
			_settingsPresetManager.Initialize();
			LandingPageExtension.InstanceCreated += LandingPageExtension_InstanceCreated;
			LandingPageExtension.CreateInstanceInBackground(PlayniteApi);

			PlayniteApi.Database.Games.ItemUpdated += Games_ItemUpdated;

			UpdateSidebarItemVisibility();
		}

		public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
		{
			// Add code to be executed when Playnite is shutting down.
		}

		public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
		{
			// Add code to be executed when library is updated.
		}

		public override ISettings GetSettings(bool firstRunSettings)
		{
			return _settings ?? (_settings = new PlayNextSettingsViewModel(this, _settingsPresetManager));
		}

		public override UserControl GetSettingsView(bool firstRunSettings)
		{
			return new PlayNextSettingsView();
		}

		public StartPageExtensionArgs GetAvailableStartPageViews()
		{
			var presets = _settingsPresetManager.GetPersistedPresets();
			var views = presets.Select(x => new StartPageViewArgsBase()
			{
				ViewId = GetPresetName(x),
				Name = x.Name,
				Description = ResourceProvider.GetString("LOC_PlayNext_StartPageTopRecommendationsDescription"),
				HasSettings = true,
			}).ToList();
			views.Insert(0, new StartPageViewArgsBase
			{
				ViewId = "PlayNext_TopRecommendations",
				Name = ResourceProvider.GetString("LOC_PlayNext_StartPageActiveSettingsViewName"),
				Description = ResourceProvider.GetString("LOC_PlayNext_StartPageTopRecommendationsDescription"),
				HasSettings = true,
			});

			return new StartPageExtensionArgs()
			{
				ExtensionName = ResourceProvider.GetString("LOC_PlayNext_PluginName"),
				Views = views,
			};
		}

		public object GetStartPageView(string viewId, Guid instanceId)
		{
			if (LandingPageExtension.Instance == null)
			{
				LandingPageExtension.CreateInstance(PlayniteApi);
			}

			switch (viewId)
			{
				case "PlayNext_TopRecommendations":
					if (!_startPagePlayNextViewModels.TryGetValue("PlayNext_TopRecommendations", out var viewModel)
					   || !_startPageViews.TryGetValue("PlayNext_TopRecommendations", out var view))
					{
						var settings = LoadPluginSettings<PlayNextSettings>();
						viewModel = new StartPagePlayNextViewModel(this, ResourceProvider.GetString("LOC_PlayNext_PluginName"));

						view = new StartPagePlayNextView(viewModel, settings);
						_startPagePlayNextViewModels["PlayNext_TopRecommendations"] = viewModel;
						_startPageViews["PlayNext_TopRecommendations"] = view;

						RefreshPlayNextData();
					}

					return _startPageViews["PlayNext_TopRecommendations"];
			}

			var presets = _settingsPresetManager.GetPersistedPresets();
			var preset = presets.FirstOrDefault(x => GetPresetName(x) == viewId);
			if (preset != null)
			{
				var presetViewModel = new StartPagePlayNextViewModel(this, preset.Name);
				var presetView = new StartPagePlayNextView(presetViewModel, preset.Settings);
				_startPagePlayNextViewModels[GetPresetName(preset)] = presetViewModel;
				_startPageViews[GetPresetName(preset)] = presetView;
				RefreshPlayNextData();
				return presetView;
			}

			return null;
		}

		public Control GetStartPageViewSettings(string viewId, Guid instanceId)
		{
			switch (viewId)
			{
				case "PlayNext_TopRecommendations":
					if (_settings == null)
					{
						GetSettings(false);
					}

					return new StartPageSettingsView(new StartPageSettingsViewModel(_settings.Settings, settings =>
					{
						_settings.EndEdit();
					}));
			}

			var presets = _settingsPresetManager.GetPersistedPresets();
			var preset = presets.FirstOrDefault(x => GetPresetName(x) == viewId);
			if (preset != null)
			{
				return new StartPageSettingsView(new StartPageSettingsViewModel(preset.Settings, settings =>
				{
					_settingsPresetManager.WritePreset(preset);
					OnPlayNextSettingsSaved();
				}));
			}

			return null;
		}

		public void OnViewRemoved(string viewId, Guid instanceId)
		{
		}

		public void OnPlayNextSettingsSaved()
		{
			var activeSettings = LoadPluginSettings<PlayNextSettings>();
			var presets = _settingsPresetManager.GetPersistedPresets();
			_startPagePlayNextViewModels.ForEach(x =>
			{
				var settings = presets.FirstOrDefault(p => GetPresetName(p) == x.Key)?.Settings ?? activeSettings;
				x.Value.UpdateLabelDisplay(settings);
			});
			_startPageViews.ForEach(x =>
			{
				var settings = presets.FirstOrDefault(p => GetPresetName(p) == x.Key)?.Settings ?? activeSettings;
				x.Value.UpdateMinCoverCount(settings);
			});

			RefreshPlayNextData();
			UpdateSidebarItemVisibility();
			_playNextMainViewModel?.SetIsRefreshAvailable(activeSettings);
		}

		public void RefreshPlayNextData()
		{
			if (_playNextMainViewModel == null && _startPagePlayNextViewModels.Count == 0)
			{
				return;
			}

			new Task<Task>(async () =>
			{
				try
				{
					var presets = _settingsPresetManager.GetPersistedPresets();
					var activeSettings = LoadPluginSettings<PlayNextSettings>();
					var games = await CalculatePlayNextData(activeSettings);

					_playNextMainViewModel?.LoadData(games, activeSettings);
					foreach (var startPageViewModel in _startPagePlayNextViewModels)
					{
						var presetSettings = presets.FirstOrDefault(p => GetPresetName(p) == startPageViewModel.Key)?.Settings;
						if (presetSettings != null)
						{
							var presetGames = await CalculatePlayNextData(presetSettings);
							startPageViewModel.Value.LoadData(presetGames, presetSettings);
						}
						else
						{
							startPageViewModel.Value.LoadData(games, activeSettings);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex, "Failure while refreshing data.");
				}
			}).Start();
		}

		private void LandingPageExtension_InstanceCreated()
		{
			LandingPageExtension.InstanceCreated -= LandingPageExtension_InstanceCreated;
			PlayniteApi.MainView.UIDispatcher.Invoke(() =>
			{
				_startPageViews.ForEach(x => x.Value.UpdateCoversColumnWidth());
			});
		}

		private void Games_ItemUpdated(object sender, ItemUpdatedEventArgs<Playnite.SDK.Models.Game> e)
		{
			if (_settings == null)
			{
				GetSettings(false);
			}

			if (_settings?.Settings.RefreshOnGameUpdates == true)
			{
				_gameUpdatedTimer.Start();
			}
		}

		private void UpdateSidebarItemVisibility()
		{
			var settings = GetSettings(false) as PlayNextSettingsViewModel;
			_sidebarItem.Visible = settings?.Settings.ShowSidebarItem ?? true;
		}

		private static string GetPresetName(SettingsPreset<PlayNextSettings> preset)
		{
			return $"PlayNext_Preset_{preset.Id}".Replace('-', '_');
		}

		private async Task<ICollection<GameToPlayViewModel>> CalculatePlayNextData(PlayNextSettings settings)
		{
			var recentDayCount = settings.RecentDays;
			var gameLengthWeight = settings.GameLengthWeight;

			var allGames = PlayniteApi.Database.Games.ToArray();
			var playedGames = new WithPlaytimeFilter().Filter(allGames);
			var recentGames = new RecentlyPlayedFilter(_dateTimeProvider).Filter(playedGames, recentDayCount);
			var unPlayedGames = new UnplayedFilter().Filter(allGames, settings).ToArray();
			var filteredGames = new ExclusionListFilter().Filter(unPlayedGames, settings);

			var activitiesTask = recentGames.Any()
				? _gameActivities.ParseGameActivity(recentGames)
				: Task.CompletedTask;
			var howLongToBeatTask = gameLengthWeight > 0
				? _howLongToBeatExtension.ParseFiles(filteredGames)
				: Task.CompletedTask;

			await Task.WhenAll(activitiesTask, howLongToBeatTask);

			var games = _totalScoreCalculator.Calculate(settings);
			return games;
		}
	}
}