using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Model.Data;
using PlayNext.Model.Score.GameScore;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
    public class GameScoreBySeriesCalculatorTests
    {
        [Theory]
        [AutoData]
        public void Calculate_GamesListIsEmpty_ScoreListIsEmpty(
            OrderSeriesBy orderSeriesBy,
            Dictionary<Guid, float> attributeScore,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var games = Array.Empty<Game>();

            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            Assert.Empty(result);
        }

        [Theory]
        [AutoData]
        public void Calculate_GamesHaveNoSeries_ScoreListIsEmpty(
            OrderSeriesBy orderSeriesBy,
            Game[] games,
            Dictionary<Guid, float> attributeScore,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            foreach (var game in games)
            {
                game.SeriesIds = null;
            }

            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            Assert.Empty(result);
        }

        [Theory]
        [AutoData]
        public void Calculate_AttributeScoreListIsEmpty_ScoreListIsEmpty(
            OrderSeriesBy orderSeriesBy,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var attributeScore = new Dictionary<Guid, float>();

            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            Assert.Empty(result);
        }

        [Theory]
        [AutoData]
        public void Calculate_NoGamesMatchSeriesWithScore_ScoreListIsEmpty(
            OrderSeriesBy orderSeriesBy,
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            Assert.Empty(result);
        }

        [Theory]
        [AutoData]
        public void Calculate_OneGameMatchesSeriesWithScore_ThatGameScoreIs100(
            OrderSeriesBy orderSeriesBy,
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var seriesAttributeScore = attributeScore.Last();
            games.Last().SeriesIds.Add(seriesAttributeScore.Key);

            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            var actualGame = Assert.Single(result, x => x.Value != 0);
            Assert.Equal(100, actualGame.Value);
        }

        [Theory]
        [AutoData]
        public void Calculate_TwoGamesMatchesSeriesWithScoreAndCalculatingByReleaseDate_OlderReleaseDateGameIs100NewerOneIs50Score(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var seriesAttributeScore = attributeScore.Last();
            var olderGame = games.Last();
            var newerGame = games.First();
            olderGame.SeriesIds.Add(seriesAttributeScore.Key);
            newerGame.SeriesIds.Add(seriesAttributeScore.Key);
            olderGame.ReleaseDate = new ReleaseDate(1988);
            newerGame.ReleaseDate = new ReleaseDate(2002, 05, 12);

            // Act
            var result = sut.Calculate(OrderSeriesBy.ReleaseDate, games, attributeScore);

            // Arrange
            var olderGameScore = result[olderGame.Id];
            var newerGameScore = result[newerGame.Id];
            Assert.Equal(100, olderGameScore);
            Assert.Equal(50, newerGameScore);
        }

        [Theory]
        [AutoData]
        public void Calculate_ThreeGamesMatchesSeriesWithScoreAndCalculatingByReleaseDate_OlderReleaseDateGameIs100NewerOneIs67NewestIs33Score(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var seriesAttributeScore = attributeScore.Last();
            var olderGame = games.Last();
            var newerGame = games.First();
            var newestGame = games[1];
            olderGame.SeriesIds.Add(seriesAttributeScore.Key);
            newerGame.SeriesIds.Add(seriesAttributeScore.Key);
            newestGame.SeriesIds.Add(seriesAttributeScore.Key);
            olderGame.ReleaseDate = new ReleaseDate(1988);
            newerGame.ReleaseDate = new ReleaseDate(2002, 05, 12);
            newestGame.ReleaseDate = new ReleaseDate(2002, 05, 13);

            // Act
            var result = sut.Calculate(OrderSeriesBy.ReleaseDate, games, attributeScore);

            // Arrange
            var olderGameScore = result[olderGame.Id];
            var newerGameScore = result[newerGame.Id];
            var newestGameScore = result[newestGame.Id];
            Assert.Equal(100, olderGameScore);
            Assert.Equal(200f / 3, newerGameScore, 0.5);
            Assert.Equal(100f / 3, newestGameScore, 0.5);
        }

        [Theory]
        [AutoData]
        public void Calculate_TwoGamesMatchesSeriesWithScoreAndCalculatingBySortingName_FirstGameIs100SecondIs50Score(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var seriesAttributeScore = attributeScore.Last();
            var olderGame = games.Last();
            var newerGame = games.First();
            olderGame.SeriesIds.Add(seriesAttributeScore.Key);
            newerGame.SeriesIds.Add(seriesAttributeScore.Key);
            olderGame.SortingName = "ABC1";
            newerGame.SortingName = "ABC2";

            // Act
            var result = sut.Calculate(OrderSeriesBy.SortingName, games, attributeScore);

            // Arrange
            var olderGameScore = result[olderGame.Id];
            var newerGameScore = result[newerGame.Id];
            Assert.Equal(100, olderGameScore);
            Assert.Equal(50, newerGameScore);
        }

        [Theory]
        [AutoData]
        public void Calculate_ThreeGamesMatchesSeriesWithScoreAndCalculatingBySortingName_FirstIs100SecondIs67ThirdIs33Score(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            var seriesAttributeScore = attributeScore.Last();
            var olderGame = games.Last();
            var newerGame = games.First();
            var newestGame = games[1];
            olderGame.SeriesIds.Add(seriesAttributeScore.Key);
            newerGame.SeriesIds.Add(seriesAttributeScore.Key);
            newestGame.SeriesIds.Add(seriesAttributeScore.Key);
            olderGame.SortingName = "ABC1";
            newerGame.SortingName = "ABC2";
            newestGame.SortingName = "ABC3";

            // Act
            var result = sut.Calculate(OrderSeriesBy.SortingName, games, attributeScore);

            // Arrange
            var olderGameScore = result[olderGame.Id];
            var newerGameScore = result[newerGame.Id];
            var newestGameScore = result[newestGame.Id];
            Assert.Equal(100, olderGameScore);
            Assert.Equal(200f / 3, newerGameScore, 0.5);
            Assert.Equal(100f / 3, newestGameScore, 0.5);
        }

        [Theory]
        [AutoData]
        public void Calculate_OneGameWithOneSeriesOneGameWithTwoSeriesWithScoreAndScoreIsAllEqual_TwoSeriesGameIs100(
            OrderSeriesBy orderSeriesBy,
            Dictionary<Guid, float> attributeScore,
            float score,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            attributeScore = attributeScore.ToDictionary(x => x.Key, x => score);
            var firstSeries = attributeScore.Last();
            var secondSeries = attributeScore.First();
            var gameWithOneSeries = games.Last();
            var gameWithTwoSeries = games.First();
            gameWithOneSeries.SeriesIds.Add(firstSeries.Key);
            gameWithTwoSeries.SeriesIds.Add(firstSeries.Key);
            gameWithTwoSeries.SeriesIds.Add(secondSeries.Key);

            // Act
            var result = sut.Calculate(orderSeriesBy, games, attributeScore);

            // Arrange
            var gameWithOneSeriesScore = result[gameWithOneSeries.Id];
            var gameWithTwoSeriesScore = result[gameWithTwoSeries.Id];
            Assert.Equal(100, gameWithTwoSeriesScore);
            Assert.NotEqual(100, gameWithOneSeriesScore);
        }
    }
}