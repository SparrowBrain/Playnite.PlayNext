﻿using PlayNext.Extensions.GameActivity;
using PlayNext.Extensions.HowLongToBeat;
using PlayNext.Extensions.StartPage;
using PlayNext.Infrastructure.Services;
using PlayNext.Model.Filters;
using PlayNext.Model.Score;
using PlayNext.Settings;
using PlayNext.ViewModels;
using PlayNext.Views;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using StartPage.SDK;
using System;
using System.Collections.Generic;
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
		private readonly TotalScoreCalculator _totalScoreCalculator;
		private readonly DateTimeProvider _dateTimeProvider = new DateTimeProvider();
		private readonly Timer _gameUpdatedTimer = new Timer(5000);

		private PlayNextSettingsViewModel _settings;
		private StartPagePlayNextViewModel _startPagePlayNextViewModel;
		private PlayNextMainViewModel _playNextMainViewModel;
		private PlayNextMainView _playNextMainView;
		private StartPagePlayNextView _startPageView;

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
			_startupSettingsValidator = new StartupSettingsValidator(pluginSettingsPersistence, new SettingsMigrator(pluginSettingsPersistence));
			_gameUpdatedTimer.Elapsed += (o, e) => RefreshPlayNextData();
			_gameUpdatedTimer.AutoReset = false;
			_gameUpdatedTimer.Enabled = false;
		}

		public static IPlayniteAPI Api { get; private set; }

		public override IEnumerable<SidebarItem> GetSidebarItems()
		{
			yield return new SidebarItem
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

		public override void OnGameInstalled(OnGameInstalledEventArgs args)
		{
			_startPagePlayNextViewModel?.UpdateGame(args.Game);
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
			_startPagePlayNextViewModel?.UpdateGame(args.Game);
		}

		public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
		{
			_startupSettingsValidator.EnsureCorrectVersionSettingsExist();
			LandingPageExtension.InstanceCreated += LandingPageExtension_InstanceCreated;
			LandingPageExtension.CreateInstanceInBackground(PlayniteApi);

			PlayniteApi.Database.Games.ItemUpdated += Games_ItemUpdated;
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
			return _settings ?? (_settings = new PlayNextSettingsViewModel(this));
		}

		public override UserControl GetSettingsView(bool firstRunSettings)
		{
			return new PlayNextSettingsView();
		}

		public StartPageExtensionArgs GetAvailableStartPageViews()
		{
			return new StartPageExtensionArgs()
			{
				ExtensionName = ResourceProvider.GetString("LOC_PlayNext_PluginName"),
				Views = new[]
				{
					new StartPageViewArgsBase
					{
						ViewId = "PlayNext_TopRecommendations",
						Name = ResourceProvider.GetString("LOC_PlayNext_StartPageTopRecommendationsViewName"),
						Description = ResourceProvider.GetString("LOC_PlayNext_StartPageTopRecommendationsDescription")
					}
				}
			};
		}

		public object GetStartPageView(string viewId, Guid instanceId)
		{
			switch (viewId)
			{
				case "PlayNext_TopRecommendations":
					if (_startPagePlayNextViewModel == null || _startPageView == null)
					{
						var settings = LoadPluginSettings<PlayNextSettings>();
						_startPagePlayNextViewModel = new StartPagePlayNextViewModel(this);
						if (LandingPageExtension.Instance == null)
						{
							LandingPageExtension.CreateInstance(PlayniteApi);
						}

						_startPageView = new StartPagePlayNextView(_startPagePlayNextViewModel, settings);
						RefreshPlayNextData();
					}

					return _startPageView;
			}

			return null;
		}

		public Control GetStartPageViewSettings(string viewId, Guid instanceId)
		{
			return null;
		}

		public void OnViewRemoved(string viewId, Guid instanceId)
		{
		}

		public void OnPlayNextSettingsSaved()
		{
			var settings = LoadPluginSettings<PlayNextSettings>();
			_startPagePlayNextViewModel?.UpdateLabelDisplay(settings);
			_startPageView?.UpdateMinCoverCount(settings);
			RefreshPlayNextData();
		}

		private void LandingPageExtension_InstanceCreated()
		{
			LandingPageExtension.InstanceCreated -= LandingPageExtension_InstanceCreated;
			PlayniteApi.MainView.UIDispatcher.Invoke(() =>
			{
				_startPageView?.UpdateCoversColumnWidth();
			});
		}

		private void Games_ItemUpdated(object sender, ItemUpdatedEventArgs<Playnite.SDK.Models.Game> e)
		{
			_gameUpdatedTimer.Start();
		}

		private void RefreshPlayNextData()
		{
			if (_playNextMainViewModel == null && _startPagePlayNextViewModel == null)
			{
				return;
			}

			new Task<Task>(async () =>
			{
				try
				{
					var savedSettings = LoadPluginSettings<PlayNextSettings>();
					var recentDayCount = savedSettings.RecentDays;
					var gameLengthWeight = savedSettings.GameLengthWeight;

					var allGames = PlayniteApi.Database.Games.ToArray();
					var playedGames = new WithPlaytimeFilter().Filter(allGames);
					var recentGames = new RecentlyPlayedFilter(_dateTimeProvider).Filter(playedGames, recentDayCount);
					var unPlayedGames = new UnplayedFilter().Filter(allGames, savedSettings).ToArray();

					var activitiesTask = recentGames.Any()
						? _gameActivities.ParseGameActivity(recentGames)
						: Task.CompletedTask;
					var howLongToBeatTask = gameLengthWeight > 0
						? _howLongToBeatExtension.ParseFiles(unPlayedGames)
						: Task.CompletedTask;

					await Task.WhenAll(activitiesTask, howLongToBeatTask);

					var games = _totalScoreCalculator.Calculate(savedSettings);

					_playNextMainViewModel?.LoadData(games);
					_startPagePlayNextViewModel?.LoadData(games);
				}
				catch (Exception ex)
				{
					_logger.Error(ex, "Failure while refreshing data.");
				}
			}).Start();
		}
	}
}