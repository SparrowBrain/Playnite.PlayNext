using System;
using PlayNext.Settings.Old;

namespace PlayNext.Settings
{
    public class SettingsMigrator : ISettingsMigrator
    {
        private readonly IPluginSettingsPersistence _pluginSettingsPersistence;

        public SettingsMigrator(IPluginSettingsPersistence pluginSettingsPersistence)
        {
            _pluginSettingsPersistence = pluginSettingsPersistence;
        }

        public PlayNextSettings LoadAndMigrateToNewest(int version)
        {
            switch (version)
            {
                case 0:
                    var settingsV0 = _pluginSettingsPersistence.LoadPluginSettings<SettingsV0>();
                    return PlayNextSettings.Migrate(settingsV0);

                default:
                    throw new ArgumentException($"Version v{version} not configured in the migrator");
            }
        }
    }
}