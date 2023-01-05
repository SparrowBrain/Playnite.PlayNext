using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Moq;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests
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

    public interface IDateTimeProvider
    {
        DateTime GetNow();
    }

    public class RecentlyPlayedFilter
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public RecentlyPlayedFilter(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<Game> Filter(IEnumerable<Game> games, int recentDayCount)
        {
            return games.Where(x => x.LastActivity >= _dateTimeProvider.GetNow() - TimeSpan.FromDays(recentDayCount));
        }
    }
}