using AutoFixture.Xunit2;
using PlayNext.Settings;
using PlayNext.Settings.Old;
using Xunit;

namespace PlayNext.UnitTests.Settings.Old
{
    public class SettingsV0Tests
    {
        [Theory, AutoData]
        public void Migrate_MigratesToV1(
            SettingsV0 settingsV0)
        {
            // Act
            var result = settingsV0.Migrate() as PlayNextSettings;

            // Assert
            Assert.NotNull(result);
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

            Assert.Equal(1, result.Version);
        }
    }
}