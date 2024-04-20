using System;
using System.Linq;
using System.Threading.Tasks;
using PlayNext.Extensions.StartPage.Settings;
using Playnite.SDK;

namespace PlayNext.Extensions.StartPage
{
    public class LandingPageExtension
    {
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(LandingPageExtension));

        private LandingPageExtension(LandingPageSettings settings)
        {
            Settings = settings;
        }

        public LandingPageSettings Settings { get; set; }

        public static LandingPageExtension Instance { get; set; }

        public static void CreateInstance(IPlayniteAPI api)
        {
            new Task(() =>
            {
                try
                {
                    var plugin = api.Addons.Plugins.FirstOrDefault(x =>
                        x.Id == Guid.Parse("a6a3dcf6-9bfe-426c-afb0-9f49409ae0c5"));

                    if (plugin == null)
                    {
                        Logger.Debug("Did not find start page plugin");
                        return;
                    }

                    var settings = plugin.LoadPluginSettings<LandingPageSettings>();

                    var landingPageExtension = new LandingPageExtension(settings);
                    Instance = landingPageExtension;
                    Logger.Debug("Settings loaded: " + settings);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error loading start page settings");
                }
            }).Start();
        }
    }
}