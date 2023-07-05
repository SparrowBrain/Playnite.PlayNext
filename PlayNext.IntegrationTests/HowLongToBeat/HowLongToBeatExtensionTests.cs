using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlayNext.HowLongToBeat;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;

namespace PlayNext.IntegrationTests.HowLongToBeat
{
    public class HowLongToBeatExtensionTests
    {
        private const string TestDataPath = @"HowLongToBeat\TestData";
        private const string ExtensionsDataPath = @"HowLongToBeat\ExtensionsData";

        public HowLongToBeatExtensionTests()
        {
            Utils.CopyDirectory(TestDataPath, ExtensionsDataPath, true);
        }

        [Fact]
        public void DoesDataExist_ReturnsFalse_WhenNoExtensionDataExists()
        {
            // Arrange
            CleanUpExtensionsDataPath();
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);

            // Act
            var result = sut.DoesDataExist();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DoesDataExist_ReturnsTrue_WhenExtensionDataExists()
        {
            // Arrange
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);

            // Act
            var result = sut.DoesDataExist();

            // Assert
            Assert.True(result);
        }

        [Theory, AutoMoqData]
        public async Task GetTimeToPlay_ReturnsEmptyDictionary_WhenGamesHaveNoData(
            List<Game> games)
        {
            // Arrange
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);

            // Act
            await sut.ParseFiles(games);
            var result = sut.GetTimeToPlay();

            // Assert
            Assert.Empty(result);
        }

        [Theory, AutoMoqData]
        public async Task GetTimeToPlay_ReturnsAverageTime_WhenGamesHasData(
            List<Game> games)
        {
            // Arrange
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);
            var gameId = Guid.Parse("80e0983b-9855-4d1a-b801-53084aabd7a9");
            var gameWithData = games.First();
            gameWithData.Id = gameId;

            // Act
            await sut.ParseFiles(games);
            var result = sut.GetTimeToPlay();

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(gameId, single.Key);
            Assert.Equal(21777, single.Value);
        }

        [Theory, AutoMoqData]
        public async Task GetTimeToPlay_ReturnsMainStoryTime_WhenGamesHasOnlyMainStoryTime(
            List<Game> games)
        {
            // Arrange
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);
            var gameId = Guid.Parse("90a89c58-5205-421f-9853-3fe20b783b48");
            var gameWithData = games.First();
            gameWithData.Id = gameId;

            // Act
            await sut.ParseFiles(games);
            var result = sut.GetTimeToPlay();

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(gameId, single.Key);
            Assert.Equal(3626, single.Value);
        }

        [Theory, AutoMoqData]
        public async Task GetTimeToPlay_ReturnsOneGame_WhenOtherGamesHaveAllTimesAsZeroes(
            List<Game> games)
        {
            // Arrange
            var sut = HowLongToBeatExtension.Create(ExtensionsDataPath);
            var gameWithZeroesId = Guid.Parse("1a1f204c-3106-4bf9-9d78-311a83501b77");
            var gameWithZeroTime = games.First();
            gameWithZeroTime.Id = gameWithZeroesId;

            var gameWithDataId = Guid.Parse("90a89c58-5205-421f-9853-3fe20b783b48");
            var gameWithData = games.Last();
            gameWithData.Id = gameWithDataId;

            // Act
            await sut.ParseFiles(games);
            var result = sut.GetTimeToPlay();

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(gameWithDataId, single.Key);
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