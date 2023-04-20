using AutoFixture.Xunit2;
using Moq;
using PlayNext.Settings;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Settings
{
    public class StartupSettingsValidatorTests
    {
        [Theory, AutoMoqData]
        public void EnsureCorrectVersionSettingsExist_CreatesAndSavesNewestVersion_WhenSettingsDontExist(
            [Frozen] Mock<IPluginSettingsPersistence> pluginSettingsPersistenceMock,
            StartupSettingsValidator sut)
        {
            // Arrange
            pluginSettingsPersistenceMock.Setup(x => x.LoadPluginSettings<PlayNextSettings>())
                .Returns((PlayNextSettings)null);

            // Act
            sut.EnsureCorrectVersionSettingsExist();

            // Assert
            pluginSettingsPersistenceMock.Verify(x => x.SavePluginSettings(It.IsAny<PlayNextSettings>()),
                Times.Once());
        }

        [Theory, AutoMoqData]
        public void EnsureCorrectVersionSettingsExist_DoesNotSaveNewVersion_WhenSettingsInNewestVersionAlreadyExist(
            [Frozen] Mock<IPluginSettingsPersistence> pluginSettingsPersistenceMock,
            VersionedSettings settings,
            StartupSettingsValidator sut)
        {
            // Arrange
            settings.Version = PlayNextSettings.CurrentVersion;
            pluginSettingsPersistenceMock.Setup(x => x.LoadPluginSettings<VersionedSettings>()).Returns(settings);

            // Act
            sut.EnsureCorrectVersionSettingsExist();

            // Assert
            pluginSettingsPersistenceMock.Verify(x => x.SavePluginSettings(It.IsAny<PlayNextSettings>()),
                Times.Never());
        }

        [Theory, AutoMoqData]
        public void
            EnsureCorrectVersionSettingsExist_DoesNotMigrateSettings_WhenSettingsInNewestVersionAlreadyExist(
                [Frozen] Mock<IPluginSettingsPersistence> pluginSettingsPersistenceMock,
                [Frozen] Mock<ISettingsMigrator> settingsMigratorMock,
                VersionedSettings settings,
                StartupSettingsValidator sut)
        {
            // Arrange
            settings.Version = PlayNextSettings.CurrentVersion;
            pluginSettingsPersistenceMock.Setup(x => x.LoadPluginSettings<VersionedSettings>()).Returns(settings);

            // Act
            sut.EnsureCorrectVersionSettingsExist();

            // Assert
            settingsMigratorMock.Verify(x => x.LoadAndMigrateToNewest(It.IsAny<int>()), Times.Never());
        }

        [Theory, AutoMoqData]
        public void EnsureCorrectVersionSettingsExist_MigratesAndSavesSettings_WhenSettingsInOldVersionExist(
            [Frozen] Mock<IPluginSettingsPersistence> pluginSettingsPersistenceMock,
            [Frozen] Mock<ISettingsMigrator> settingsMigratorMock,
            VersionedSettings oldSettings,
            PlayNextSettings newSettings,
            StartupSettingsValidator sut)
        {
            // Arrange
            oldSettings.Version = PlayNextSettings.CurrentVersion - 1;
            pluginSettingsPersistenceMock.Setup(x => x.LoadPluginSettings<VersionedSettings>())
                .Returns(oldSettings);
            settingsMigratorMock.Setup(x => x.LoadAndMigrateToNewest(It.IsAny<int>())).Returns(newSettings);

            // Act
            sut.EnsureCorrectVersionSettingsExist();

            // Assert
            pluginSettingsPersistenceMock.Verify(x => x.SavePluginSettings(newSettings), Times.Once());
        }
    }
}