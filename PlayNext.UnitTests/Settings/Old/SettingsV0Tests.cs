using AutoFixture.Xunit2;
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
            var result = settingsV0.Migrate() as SettingsV1;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(settingsV0.RecentDays, result.RecentDays);
            Assert.Equal(settingsV0.NumberOfTopGames, result.NumberOfTopGames);

            Assert.Equal(settingsV0.TotalPlaytimeSerialized, result.TotalPlaytimeWeight);
            Assert.Equal(settingsV0.RecentPlaytimeSerialized, result.RecentPlaytimeWeight);
            Assert.Equal(settingsV0.RecentOrderSerialized, result.RecentOrderWeight);

            Assert.Equal(settingsV0.GenreSerialized, result.GenreWeight);
            Assert.Equal(settingsV0.FeatureSerialized, result.FeatureWeight);
            Assert.Equal(settingsV0.DeveloperSerialized, result.DeveloperWeight);
            Assert.Equal(settingsV0.PublisherSerialized, result.PublisherWeight);
            Assert.Equal(settingsV0.TagSerialized, result.TagWeight);
            Assert.Equal(settingsV0.CriticScoreSerialized, result.CriticScoreWeight);
            Assert.Equal(settingsV0.CommunityScoreSerialized, result.CommunityScoreWeight);
            Assert.Equal(settingsV0.ReleaseYearSerialized, result.ReleaseYearWeight);
            Assert.Equal(settingsV0.ReleaseYearChoice, result.ReleaseYearChoice);
            Assert.Equal(settingsV0.DesiredReleaseYear, result.DesiredReleaseYear);

            Assert.Equal(1, result.Version);
        }
    }
}