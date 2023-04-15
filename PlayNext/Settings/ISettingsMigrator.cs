namespace PlayNext.Settings
{
    public interface ISettingsMigrator
    {
        PlayNextSettings LoadAndMigrateToNewest(int version);
    }
}