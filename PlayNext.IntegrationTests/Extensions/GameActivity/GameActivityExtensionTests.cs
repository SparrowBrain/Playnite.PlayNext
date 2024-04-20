using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PlayNext.Extensions.GameActivity;
using PlayNext.Infrastructure.Services;
using PlayNext.Settings;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;

namespace PlayNext.IntegrationTests.Extensions.GameActivity
{
    public class GameActivityExtensionTests
    {
        private const string TestDataPath = @"GameActivity\TestData";
        private const string ExtensionsDataPath = @"GameActivity\ExtensionsData";

        public GameActivityExtensionTests()
        {
            Utils.CopyDirectory(TestDataPath, ExtensionsDataPath, true);
        }

        [Theory, AutoMoqData]
        public void GameActivityPathExists_ReturnsFalse_WhenPathDoesNotExist(
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            CleanUpExtensionsDataPath();
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);

            // Act
            var pathExists = sut.GameActivityPathExists();

            // Assert
            Assert.False(pathExists);
        }

        [Theory, AutoMoqData]
        public void GameActivityPathExists_ReturnsTrue_WhenPathExists(
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);

            // Act
            var pathExists = sut.GameActivityPathExists();

            // Assert
            Assert.True(pathExists);
        }

        [Theory, AutoMoqData]
        public async Task GetRecentPlaytime_ReturnsTrue_WhenPathExists(
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);

            // Act
            var pathExists = sut.GameActivityPathExists();

            // Assert
            Assert.True(pathExists);
        }

        [Theory, AutoMoqData]
        public async Task GetRecentPlaytime_ReturnsAllGamesWithZeroTime_WhenNoActivityFilesExistsForGames(
            IEnumerable<Game> games,
            PlayNextSettings settings,
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);

            // Act
            await sut.ParseGameActivity(games);
            var gamesWithRecentPlaytime = sut.GetRecentPlaytime(games, settings);

            // Assert
            Assert.Equal(games.Count(), gamesWithRecentPlaytime.Count());
            Assert.All(gamesWithRecentPlaytime, x => Assert.Equal(0d, x.Playtime));
        }

        [Theory, AutoMoqData]
        public async Task GetRecentPlaytime_ReturnsGameWithPlaytime_WhenActivityWasWithinRecentTime(
            int recentDays,
            IEnumerable<Game> games,
            PlayNextSettings settings,
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            var gameId = Guid.Parse("f1044699-4b97-4968-868b-e871e37ae1f3");
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);
            var gameWithActivity = games.First();
            gameWithActivity.Id = gameId;
            settings.RecentDays = recentDays;
            dateTimeProviderMock
                .Setup(x => x.GetNow())
                .Returns(DateTime.Parse("2023-02-12T13:40:18.2429471Z") + TimeSpan.FromDays(recentDays / 2));

            // Act
            await sut.ParseGameActivity(games);
            var gamesWithRecentPlaytime = sut.GetRecentPlaytime(games, settings);

            // Assert
            var game = gamesWithRecentPlaytime.First(x => x.Id == gameId);
            Assert.Equal(67, (int)game.Playtime);
        }

        [Theory, AutoMoqData]
        public async Task GetRecentPlaytime_ReturnsGameWithZeroTime_WhenActivityWasNotWithinRecentTime(
            int recentDays,
            IEnumerable<Game> games,
            PlayNextSettings settings,
            Mock<IDateTimeProvider> dateTimeProviderMock)
        {
            // Arrange
            var gameId = Guid.Parse("f1044699-4b97-4968-868b-e871e37ae1f3");
            var sut = GameActivityExtension.Create(dateTimeProviderMock.Object, ExtensionsDataPath);
            var gameWithActivity = games.First();
            gameWithActivity.Id = gameId;
            settings.RecentDays = recentDays;
            dateTimeProviderMock
                .Setup(x => x.GetNow())
                .Returns(DateTime.Parse("2023-02-12T13:40:18.2429471Z") + TimeSpan.FromDays(recentDays * 2));

            // Act
            await sut.ParseGameActivity(games);
            var gamesWithRecentPlaytime = sut.GetRecentPlaytime(games, settings);

            // Assert
            var game = gamesWithRecentPlaytime.First(x => x.Id == gameId);
            Assert.Equal(0d, game.Playtime);
        }

        private static void CleanUpExtensionsDataPath()
        {
            foreach (var dir in Directory.GetDirectories(ExtensionsDataPath))
            {
                Directory.Delete(dir, true);
            }
        }
    }
}