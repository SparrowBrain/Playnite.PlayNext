using System;
using System.Linq;
using System.Threading.Tasks;
using PlayNext.StartPage.Settings;
using Playnite.SDK;

namespace PlayNext.StartPage
{
    public class LandingPageExtension
    {
        private const int SecondsToTryLoadPlugin = 30;
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(LandingPageExtension));

        private LandingPageExtension(LandingPageSettings settings)
        {
            Settings = settings;
        }

        public LandingPageSettings Settings { get; set; }

        public static LandingPageExtension Instance { get; set; }

        public static void CreateInstance(IPlayniteAPI api)
        {
            var startTime = DateTime.Now;
            new Task(async () =>
            {
                try
                {
                    while (Instance == null && DateTime.Now < startTime.AddSeconds(SecondsToTryLoadPlugin))
                    {
                        var plugin = api.Addons.Plugins.FirstOrDefault(x =>
                            x.Id == Guid.Parse("a6a3dcf6-9bfe-426c-afb0-9f49409ae0c5"));

                        if (plugin == null)
                        {
                            await Task.Delay(1000);
                            Logger.Debug("Did not find start page plugin");
                            continue;
                        }

                        var settings = plugin.LoadPluginSettings<LandingPageSettings>();

                        var landingPageExtension = new LandingPageExtension(settings);
                        Instance = landingPageExtension;
                        Logger.Debug("Settings loaded: " + settings);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error loading start page settings");
                }
            }).Start();
        }
    }
}