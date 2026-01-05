using AutoFixture.Xunit2;
using PlayNext.Settings;
using PlayNext.Settings.Old;
using Xunit;

namespace PlayNext.UnitTests.Settings.Old
{
    public class SettingsV2Tests
    {
        [Theory, AutoData]
        public void Migrate_MigratesToV3(
            SettingsV2 oldSettings)
        {
            // Act
            var result = oldSettings.Migrate() as SettingsV3;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(oldSettings.RecentDays, result.RecentDays);
            Assert.Equal(oldSettings.NumberOfTopGames, result.NumberOfTopGames);

            Assert.Equal(oldSettings.TotalPlaytimeWeight, result.TotalPlaytimeWeight);
            Assert.Equal(oldSettings.RecentPlaytimeWeight, result.RecentPlaytimeWeight);
            Assert.Equal(oldSettings.RecentOrderWeight, result.RecentOrderWeight);

            Assert.Equal(oldSettings.GenreWeight, result.GenreWeight);
            Assert.Equal(oldSettings.FeatureWeight, result.FeatureWeight);
            Assert.Equal(oldSettings.DeveloperWeight, result.DeveloperWeight);
            Assert.Equal(oldSettings.PublisherWeight, result.PublisherWeight);
            Assert.Equal(oldSettings.TagWeight, result.TagWeight);
            Assert.Equal(oldSettings.CriticScoreWeight, result.CriticScoreWeight);
            Assert.Equal(oldSettings.CommunityScoreWeight, result.CommunityScoreWeight);
            Assert.Equal(oldSettings.ReleaseYearWeight, result.ReleaseYearWeight);
            Assert.Equal(oldSettings.ReleaseYearChoice, result.ReleaseYearChoice);
            Assert.Equal(oldSettings.DesiredReleaseYear, result.DesiredReleaseYear);
            Assert.Equal(oldSettings.GameLengthWeight, result.GameLengthWeight);
            Assert.Equal(oldSettings.GameLengthHours, result.GameLengthHours);

            Assert.Equal(oldSettings.UnplayedGameDefinition, result.UnplayedGameDefinition);
            Assert.Equal(oldSettings.UnplayedCompletionStatuses, result.UnplayedCompletionStatuses);

            Assert.Equal(oldSettings.StartPageShowLabel, result.StartPageShowLabel);
            Assert.Equal(oldSettings.StartPageLabelIsHorizontal, result.StartPageLabelIsHorizontal);

            Assert.Equal(3, result.Version);
        }
    }
}