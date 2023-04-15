using System;
using AutoFixture.Xunit2;
using PlayNext.Settings;
using PlayNext.Settings.Old;
using Xunit;

namespace PlayNext.UnitTests.Settings
{
    public class PlayNextSettingsTests
    {
        [Theory, AutoData]
        public void Migrate_MigratesToV1_WhenV0IsSupplied(
            SettingsV0 settingsV0)
        {
            // Arrange
            settingsV0.Version = 0;

            // Act
            var result = PlayNextSettings.Migrate(settingsV0);

            // Assert
            Assert.Equal(settingsV0.RecentDays, result.RecentDays);
            Assert.Equal(settingsV0.NumberOfTopGames, result.NumberOfTopGames);

            Assert.Equal(settingsV0.TotalPlaytimeSerialized, result.TotalPlaytime);
            Assert.Equal(settingsV0.RecentPlaytimeSerialized, result.RecentPlaytime);
            Assert.Equal(settingsV0.RecentOrderSerialized, result.RecentOrder);

            Assert.Equal(settingsV0.GenreSerialized, result.Genre);
            Assert.Equal(settingsV0.FeatureSerialized, result.Feature);
            Assert.Equal(settingsV0.DeveloperSerialized, result.Developer);
            Assert.Equal(settingsV0.PublisherSerialized, result.Publisher);
            Assert.Equal(settingsV0.TagSerialized, result.Tag);
            Assert.Equal(settingsV0.CriticScoreSerialized, result.CriticScore);
            Assert.Equal(settingsV0.CommunityScoreSerialized, result.CommunityScore);
            Assert.Equal(settingsV0.ReleaseYearSerialized, result.ReleaseYear);
            Assert.Equal(settingsV0.ReleaseYearChoice, result.ReleaseYearChoice);
            Assert.Equal(settingsV0.DesiredReleaseYear, result.DesiredReleaseYear);
        }

        [Theory]
        [InlineAutoData(-1)]
        [InlineAutoData(1)]
        [InlineAutoData(2)]
        public void Migrate_ThrowsException_WhenNonV0IsSupplied(
            int oldVersion,
            SettingsV0 settingsV0)
        {
            // Arrange
            settingsV0.Version = oldVersion;

            // Act
            var act = new Action(() => PlayNextSettings.Migrate(settingsV0));

            // Assert
            Assert.ThrowsAny<ArgumentException>(act);
        }
    }
}