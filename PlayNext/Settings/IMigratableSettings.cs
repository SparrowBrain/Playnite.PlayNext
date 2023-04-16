namespace PlayNext.Settings
{
    public interface IMigratableSettings : IVersionedSettings
    {
        IVersionedSettings Migrate();
    }
}