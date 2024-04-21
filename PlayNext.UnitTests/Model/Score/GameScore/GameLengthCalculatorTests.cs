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
        public void Calculate_ReturnsScore0_WhenGameLengthIsFullDeviationAwayFromPreference(
            int gameLength,
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            var game = games.Last();
            var length = TimeSpan.FromSeconds(gameLength);
            var halfPreferredLength = gameLength / 2;
            games[game.Key] = gameLength - halfPreferredLength;

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            var actualGameScore = result.FirstOrDefault(x => x.Key == game.Key);
            Assert.Equal(0, actualGameScore.Value);
        }

        [Theory, AutoMoqData]
        public void Calculate_ReturnsScore50_WhenGameLengthHalfDeviationFromPreference(
            int halfDeviation,
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            var deviation = halfDeviation * 2;
            var preferredLength = deviation * 2;
            games = games.ToDictionary(x => x.Key, x => preferredLength);
            var game = games.First();
            games[game.Key] = preferredLength - halfDeviation;
            var length = TimeSpan.FromSeconds(preferredLength);

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            var actualGameScore = result.FirstOrDefault(x => x.Key == game.Key);
            Assert.Equal(50, actualGameScore.Value);
        }

        [Theory, AutoMoqData]
        public void Calculate_DeviationIsAnHour_WhenPreferredLengthIsZero(
            Dictionary<Guid, int> games,
            GameLengthCalculator sut)
        {
            // Arrange
            var deviation = 3600;
            var halfDeviation = deviation / 2;
            var length = TimeSpan.FromSeconds(0);

            var halfGame = games.First();
            var zeroGame = games.Last();
            games[halfGame.Key] = halfDeviation;
            games[zeroGame.Key] = deviation;

            // Act
            var result = sut.Calculate(games, length);

            // Assert
            var actualHalfGameScore = result.FirstOrDefault(x => x.Key == halfGame.Key);
            Assert.Equal(50, actualHalfGameScore.Value);
            var actualZeroGameScore = result.FirstOrDefault(x => x.Key == zeroGame.Key);
            Assert.Equal(0, actualZeroGameScore.Value);
        }
    }
}