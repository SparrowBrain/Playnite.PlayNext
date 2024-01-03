using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Model.Score.GameScore;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
    public class GameLengthCalculatorTests
    {
        [Theory, AutoMoqData]
        public void Calculate_ReturnsEmptyScores_WhenNoInputGames(
            int gameLength,

            GameLengthCalculator sut)
        {
            // Arrange
            var games = new Dictionary<Guid, int>();
            var length = TimeSpan.FromSeconds(gameLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            Assert.Empty(result);
        }

        [Theory, AutoMoqData]
        public void Calculate_ReturnsEmptyScores_WhenInputGamesAreNull(
            int gameLength,

            GameLengthCalculator sut)
        {
            // Arrange
            Dictionary<Guid, int> games = null;
            var length = TimeSpan.FromSeconds(gameLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            Assert.Empty(result);
        }

        [Theory, AutoMoqData]
        public void Calculate_ReturnsScore100_WhenGameLengthMatchesPreferredLength(
            int gameLength,
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            games = games.ToDictionary(x => x.Key, x => gameLength);
            var length = TimeSpan.FromSeconds(gameLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            Assert.Equal(games.Count, result.Count);
            Assert.All(result, x => Assert.Equal(100, x.Value));
        }

        [Theory, AutoMoqData]
        public void Calculate_ReturnsScore0_WhenGameLengthIsTheMostFarAwayFromPreference(
            int gameLength,
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            var game = games.OrderByDescending(x => Math.Abs(x.Value - gameLength)).First();
            var length = TimeSpan.FromSeconds(gameLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            var actualGameScore = result.FirstOrDefault(x => x.Key == game.Key);
            Assert.NotNull(actualGameScore);
            Assert.Equal(0, actualGameScore.Value);
        }

        [Theory, AutoMoqData]
        public void Calculate_ReturnsScore50_WhenGameLengthHalfOfMaxFarAwayFromPreference(
            int gameLength,
            int halfLength,
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            games = games.ToDictionary(x => x.Key, x => gameLength + halfLength * 2);
            var game = games.First();
            games[game.Key] = gameLength + halfLength;
            var length = TimeSpan.FromSeconds(gameLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            var actualGameScore = result.FirstOrDefault(x => x.Key == game.Key);
            Assert.NotNull(actualGameScore);
            Assert.Equal(50, actualGameScore.Value);
        }
    }
}