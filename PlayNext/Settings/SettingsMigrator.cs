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
            IVersionedSettings versionedSettings;
            switch (version)
            {
                case 0:
                    versionedSettings = _pluginSettingsPersistence.LoadPluginSettings<SettingsV0>();
                    break;

                default:
                    throw new ArgumentException($"Version v{version} not configured in the migrator");
            }

            while (true)
            {
                if (versionedSettings is PlayNextSettings newestSettings)
                {
                    return newestSettings;
                }

                var oldSettings = versionedSettings as IMigratableSettings;
                if (oldSettings == null)
                {
                    throw new Exception($"Somehow v{oldSettings.Version} settings are not migratable. This should have never happened. What have you done?");
                }

                var newSettings = oldSettings.Migrate();
                if (newSettings.Version != oldSettings.Version + 1)
                {
                    throw new Exception($"Invalid migration in v{oldSettings.Version} - version changed to v{newSettings.Version}, but only allowed to increment by one.");
                }

                versionedSettings = newSettings;
            }
        }
    }
}