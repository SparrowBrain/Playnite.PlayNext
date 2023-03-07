﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using PlayNext.GameActivity;
using PlayNext.Model.Filters;
using PlayNext.Services;
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
        private readonly PlayNextSettingsViewModel _settings;
        private readonly GameActivityExtension _gameActivities;

        private StartPagePlayNextViewModel _startPagePlayNextViewModel;
        private PlayNextMainViewModel _playNextMainViewModel;
        private PlayNextMainView _playNextMainView;

        public override Guid Id { get; } = Guid.Parse("05234f92-39d3-4432-98c1-6f37a3e4b870");

        public GameActivityExtension GameActivityExtension => _gameActivities;

        public PlayNext(IPlayniteAPI api) : base(api)
        {
            _settings = new PlayNextSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true,
            };

            LandingPageExtension.CreateInstance(api);
            _gameActivities = GameActivityExtension.Create(api);
            _gameActivities.ActivityRefreshed += OnActivitiesRefreshed;
        }

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
            // Add code to be executed when game is finished installing.
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
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
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
            return _settings;
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
                    _logger.Debug($"Recent days: {recentDayCount}");
                    var allGames = PlayniteApi.Database.Games.ToArray();
                    _logger.Debug($"All games: {allGames.Length}");
                    var playedGames = new WithPlaytimeFilter().Filter(allGames);
                    var recentGames = new RecentlyPlayedFilter(new DateTimeProvider()).Filter(playedGames, recentDayCount);

                    _logger.Debug($"Recent games: {recentGames.Count()}");
                    _gameActivities.StartParsingActivity(recentGames);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failure while parsing activities");
                }
            }).Start();
        }
    }
}