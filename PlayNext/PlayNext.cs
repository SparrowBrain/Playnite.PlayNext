using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using PlayNext.GameActivity;
using PlayNext.Model.Filters;
using PlayNext.Services;
using PlayNext.Settings;
using PlayNext.StartPage;
using PlayNext.ViewModels;
using PlayNext.Views;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using StartPage.SDK;

namespace PlayNext
{
    public class PlayNext : GenericPlugin, IStartPageExtension
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly GameActivityExtension _gameActivities;
        private readonly StartupSettingsValidator _startupSettingsValidator;

        private PlayNextSettingsViewModel _settings;
        private StartPagePlayNextViewModel _startPagePlayNextViewModel;
        private PlayNextMainViewModel _playNextMainViewModel;
        private PlayNextMainView _playNextMainView;

        public override Guid Id { get; } = Guid.Parse("05234f92-39d3-4432-98c1-6f37a3e4b870");

        public GameActivityExtension GameActivityExtension => _gameActivities;

        public PlayNext(IPlayniteAPI api) : base(api)
        {
            Api = api;
            Properties = new GenericPluginProperties
            {
                HasSettings = true,
            };

            _gameActivities = GameActivityExtension.Create(api);
            _gameActivities.ActivityRefreshed += OnActivitiesRefreshed;

            var pluginSettingsPersistence = new PluginSettingsPersistence(this);
            _startupSettingsValidator = new StartupSettingsValidator(pluginSettingsPersistence, new SettingsMigrator(pluginSettingsPersistence));
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
            LandingPageExtension.CreateInstance(PlayniteApi);
            RefreshPlayNextData();
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
                    _startPagePlayNextViewModel = new StartPagePlayNextViewModel(this);
                    return new StartPagePlayNextView(_startPagePlayNextViewModel);
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
            _startPagePlayNextViewModel?.UpdateLabelDisplay();
            RefreshPlayNextData();
        }

        private void OnActivitiesRefreshed()
        {
            _playNextMainViewModel?.LoadData();
            _startPagePlayNextViewModel?.LoadData();
        }

        private void RefreshPlayNextData()
        {
            ParseRecentActivities();
        }

        private void ParseRecentActivities()
        {
            new Task(() =>
            {
                try
                {
                    var savedSettings = LoadPluginSettings<PlayNextSettings>();
                    var recentDayCount = savedSettings.RecentDays;
                    var allGames = PlayniteApi.Database.Games.ToArray();
                    var playedGames = new WithPlaytimeFilter().Filter(allGames);
                    var recentGames = new RecentlyPlayedFilter(new DateTimeProvider()).Filter(playedGames, recentDayCount);

                    _gameActivities.ParseGameActivity(recentGames);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failure while parsing activities");
                }
            }).Start();
        }
    }
}