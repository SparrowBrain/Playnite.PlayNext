namespace PlayNext.Settings
{
    internal class PluginSettingsPersistence : IPluginSettingsPersistence
    {
        private readonly PlayNext _plugin;

        public PluginSettingsPersistence(PlayNext plugin)
        {
            _plugin = plugin;
        }

        public T LoadPluginSettings<T>() where T : class
        {
            return _plugin.LoadPluginSettings<T>();
        }

        public void SavePluginSettings<T>(T settings) where T : class
        {
            _plugin.SavePluginSettings(settings);
        }
    }
}