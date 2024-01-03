using System;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Model.Score.GameScore;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
    public class ReleaseYearCalculatorTests
    {
        [Theory, AutoData]
        public void Calculate_ReturnsScore100_WhenGameHasDesiredReleaseYear(
            Game[] games,
            ReleaseYearCalculator sut)
        {
            // Arrange
            var topGame = games.First();
            var desiredReleaseYear = topGame.ReleaseYear.Value;

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.Contains(result.Keys, x => x == topGame.Id);
            Assert.Equal(100, result[topGame.Id]);
        }

        [Theory, AutoData]
        public void Calculate_ReturnsScore100_WhenAllGamesHaveDesiredReleaseYear(
            Game[] games,
            int desiredReleaseYear,
            ReleaseYearCalculator sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.ReleaseDate = new ReleaseDate(desiredReleaseYear);
            }

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.All(result, x => Assert.Equal(100, x.Value));
        }

        [Theory, AutoData]
        public void Calculate_ReturnsScore0_WhenNoGameHasReleaseYear(
            Game[] games,
            int desiredReleaseYear,
            ReleaseYearCalculator sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.ReleaseDate = null;
            }

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.All(result, x => Assert.Equal(0, x.Value));
        }

        [Theory, AutoData]
        public void Calculate_ReturnsScore0_WhenGameIsAtMaxDifference(
            Game topGame,
            Game bottomGame,
            int desiredReleaseYear,
            int yearDifference,
            ReleaseYearCalculator sut)
        {
            // Arrange
            topGame.ReleaseDate = new ReleaseDate(desiredReleaseYear);
            bottomGame.ReleaseDate = new ReleaseDate(desiredReleaseYear + yearDifference);
            var games = new[] { topGame, bottomGame };

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.Contains(result.Keys, x => x == bottomGame.Id);
            Assert.Equal(0, result[bottomGame.Id]);
        }

        [Theory, AutoData]
        public void Calculate_ReturnsScore50_WhenGameIsInTheMiddleOfMaxDifference(
            Game topGame,
            Game centerGame,
            Game bottomGame,
            int desiredReleaseYear,
            int halfYearDifference,
            ReleaseYearCalculator sut)
        {
            // Arrange
            topGame.ReleaseDate = new ReleaseDate(desiredReleaseYear);
            centerGame.ReleaseDate = new ReleaseDate(desiredReleaseYear + halfYearDifference);
            bottomGame.ReleaseDate = new ReleaseDate(desiredReleaseYear + halfYearDifference * 2);
            var games = new[] { topGame, centerGame, bottomGame };

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.Contains(result.Keys, x => x == centerGame.Id);
            Assert.Equal(50, result[centerGame.Id]);
        }

        [Theory, AutoData]
        public void Calculate_ReturnsScore0_WhenGameIsAtMaxDifferenceAndThereIsNullYearGame(
            Game topGame,
            Game bottomGame,
            Game nullGame,
            int desiredReleaseYear,
            int yearDifference,
            ReleaseYearCalculator sut)
        {
            // Arrange
            topGame.ReleaseDate = new ReleaseDate(desiredReleaseYear);
            bottomGame.ReleaseDate = new ReleaseDate(desiredReleaseYear + yearDifference);
            nullGame.ReleaseDate = null;
            var games = new[] { topGame, bottomGame, nullGame };

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.Contains(result.Keys, x => x == bottomGame.Id);
            Assert.Equal(0, result[bottomGame.Id]);
        }

        [Theory, AutoData]
        public void Calculate_ReturnsEmptyScoreList_WhenInputListIsEmpty(
            int desiredReleaseYear,
            ReleaseYearCalculator sut)
        {
            // Arrange
            var games = Array.Empty<Game>();

            // Act
            var result = sut.Calculate(games, desiredReleaseYear);

            // Assert
            Assert.Empty(result);
        }
    }
}