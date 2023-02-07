using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using PlayNext.StartPage;
using PlayNext.StartPage.Markup;
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
        private static readonly ILogger logger = LogManager.GetLogger();

        private PlayNextSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("05234f92-39d3-4432-98c1-6f37a3e4b870");

        public PlayNext(IPlayniteAPI api) : base(api)
        {
            settings = new PlayNextSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true,
            };

            var startPage = new LandingPageExtension(api);
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            //new PlayNextSidebarItem()
            yield return new SidebarItem
            {
                Title = "Play Next",
                Icon = new TextBlock()
                {
                    Text = "\u23ED",
                    FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
                },
                Type = SiderbarItemType.View,
                Opened = () => new PlayNextMainView(new PlayNextMainViewModel(this))
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
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
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
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new PlayNextSettingsView();
        }

        public StartPageExtensionArgs GetAvailableStartPageViews()
        {
            return new StartPageExtensionArgs()
            {
                ExtensionName = "Play Next",
                Views = new[]
                {
                    new StartPageViewArgsBase
                    {
                        ViewId = "TopRecommendations",
                        Name = ResourceProvider.GetString("LOCPlayNextStartPageTopRecommendationsViewName"),
                        Description = ResourceProvider.GetString("LOCPlayNextStartPageTopRecommendationsDescription")
                    }
                }
            };
        }

        public object GetStartPageView(string viewId, Guid instanceId)
        {
            switch (viewId)
            {
                case "TopRecommendations":
                    return new StartPagePlayNextView(new StartPagePlayNextViewModel(this));
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
    }
}