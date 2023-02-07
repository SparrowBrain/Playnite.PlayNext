using System;
using System.Linq;
using System.Threading.Tasks;
using PlayNext.StartPage.Settings;
using Playnite.SDK;

namespace PlayNext.StartPage
{
    public class LandingPageExtension
    {
        private ILogger _logger = LogManager.GetLogger(nameof(LandingPageExtension));

        public LandingPageExtension(IPlayniteAPI api)
        {
            new Task(async () =>
            {
                try
                {
                    while (Instance == null)
                    {
                        var plugin = api.Addons.Plugins.FirstOrDefault(x =>
                            x.Id == Guid.Parse("a6a3dcf6-9bfe-426c-afb0-9f49409ae0c5"));

                        if (plugin == null)
                        {
                            await Task.Delay(1000);
                            _logger.Debug("Did not find start page plugin");
                            continue;
                        }

                        var settings = plugin.LoadPluginSettings<LandingPageSettings>();

                        Settings = settings;
                        Instance = this;
                        _logger.Debug("Settings loaded: " + settings);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error loading start page settings");
                }
            }).Start();
        }

        public LandingPageSettings Settings { get; set; }

        public static LandingPageExtension Instance { get; set; }
    }
}