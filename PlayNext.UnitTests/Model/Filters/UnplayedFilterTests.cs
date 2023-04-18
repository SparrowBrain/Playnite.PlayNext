using System;
using System.Linq;
using PlayNext.Model.Data;
using PlayNext.Model.Filters;
using PlayNext.Settings;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Filters
{
    public class UnplayedFilterTests
    {
        [Theory, AutoMoqData]
        public void Filter_ReturnsOneGame_When_FilteringByPlaytimeAndOneGameHasZeroPlaytime(
            Game[] games,
            PlayNextSettings settings,
            UnplayedFilter sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.Hidden = false;
            }

            var expectedGame = games.Last();
            expectedGame.Playtime = 0;
            settings.UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;

            // Act
            var result = sut.Filter(games, settings);

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(expectedGame.Id, single.Id);
        }

        [Theory, AutoMoqData]
        public void Filter_ReturnsNothing_When_FilteringByCompletionStatusAndNoneCompletionStatusIsSelected(
            Game[] games,
            PlayNextSettings settings,
            UnplayedFilter sut)
        {
            // Arrange
            settings.UnplayedGameDefinition = UnplayedGameDefinition.SelectedCompletionStatuses;
            settings.UnplayedCompletionStatuses = Array.Empty<Guid>();

            // Act
            var result = sut.Filter(games, settings);

            // Assert
            Assert.Empty(result);
        }

        [Theory, AutoMoqData]
        public void Filter_ReturnsOneGame_When_FilteringByCompletionStatusAndGameHasCompletionStatusThatIsSelected(
            Game[] games,
            PlayNextSettings settings,
            UnplayedFilter sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.Hidden = false;
            }

            var expectedGame = games.Last();
            expectedGame.CompletionStatusId = settings.UnplayedCompletionStatuses.Last();
            settings.UnplayedGameDefinition = UnplayedGameDefinition.SelectedCompletionStatuses;

            // Act
            var result = sut.Filter(games, settings);

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(expectedGame.Id, single.Id);
        }

        [Theory, AutoMoqData]
        public void Filter_ReturnsOneGame_When_OtherGamesAreHidden(
            Game[] games,
            PlayNextSettings settings,
            UnplayedFilter sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.Hidden = true;
                game.Playtime = 0;
            }

            var expectedGame = games.Last();
            expectedGame.Hidden = false;
            settings.UnplayedGameDefinition = UnplayedGameDefinition.ZeroPlaytime;

            // Act
            var result = sut.Filter(games, settings);

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(expectedGame.Id, single.Id);
        }
    }
}