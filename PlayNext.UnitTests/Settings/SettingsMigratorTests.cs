using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Settings;
using Xunit;

namespace PlayNext.UnitTests.Settings
{
    public class SettingsMigratorTests
    {
        [Theory, AutoMoqData]
        public void LoadAndMigrateToNewest_ThrowsException_WhenCalledWithNonConfiguredVersion(
            SettingsMigrator sut)
        {
            // Act
            var act = new Action(() => sut.LoadAndMigrateToNewest(int.MaxValue));

            // Assert
            Assert.ThrowsAny<ArgumentException>(act);
        }

        [Theory, MemberAutoMoqData(nameof(GetAllOldSettingsVersions))]
        public void LoadAndMigrateToNewest_MigratesAllExistingNonCurrentSettingsVersionsToNewest(
            int version,
            SettingsMigrator sut)
        {
            // Act
            var result = sut.LoadAndMigrateToNewest(version);

            // Assert
            Assert.Equal(PlayNextSettings.CurrentVersion, result.Version);
        }

        public static IEnumerable<object[]> GetAllOldSettingsVersions()
        {
            var type = typeof(IVersionedSettings);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.StartsWith("PlayNext"))
                .SelectMany(s => s.GetTypes())
                .Where(x => x.IsClass)
                .Where(p => type.IsAssignableFrom(p));

            return types.Select(x =>
            {
                var ctor = x.GetConstructor(new Type[] { });
                object instance = ctor.Invoke(new object[] { });
                return new object[] { (instance as IVersionedSettings).Version };
            }).Where(x => (int)x[0] != PlayNextSettings.CurrentVersion);
        }
    }
}