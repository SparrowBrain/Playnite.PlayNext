using System;
using System.Linq;
using AutoFixture.Xunit2;
using Moq;
using PlayNext.Infrastructure.Services;
using PlayNext.Model.Filters;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Model.Filters
{
    public class RecentlyPlayedFilterTests
    {
        [Theory, AutoMoqData]
        public void Filter_ReturnsOneGame_When_OnlyOneGameIsWithinRecentDayCount(
            [Frozen] Mock<IDateTimeProvider> dateTimeProviderMock,
            DateTime now,
            int recentDayCount,
            Game[] games,
            RecentlyPlayedFilter sut)
        {
            // Arrange
            dateTimeProviderMock.Setup(x => x.GetNow()).Returns(now);
            games.First().LastActivity = now - TimeSpan.FromDays(recentDayCount - 1);
            foreach (var game in games.Skip(1))
            {
                game.LastActivity = now - TimeSpan.FromDays(recentDayCount + 1);
            }

            // Act
            var recentGames = sut.Filter(games, recentDayCount);

            // Assert
            var recentGame = Assert.Single(recentGames);
            Assert.Equal(games[0], recentGame);
        }
    }
}