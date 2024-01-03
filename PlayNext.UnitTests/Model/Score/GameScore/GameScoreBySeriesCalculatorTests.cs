using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
    public class GameScoreBySeriesCalculatorTests
    {
        [Theory]
        [AutoData]
        public void Calculate_NoGamesMatchSeriesWithScore_AllScoreIs0(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Act
            var result = sut.Calculate(games, attributeScore);

            // Arrange
            Assert.All(result, x => Assert.Equal(0, x.Value));
        }

        [Theory]
        [AutoData]
        public void Calculate_OneGameMatchesSeriesWithScore_ThatGamesScoreIs100(
            Dictionary<Guid, float> attributeScore,
            Game[] games,
            GameScoreBySeriesCalculator sut)
        {
            // Arrange
            games.Last().SeriesIds.Add(attributeScore.Last().Key);

            // Act
            var result = sut.Calculate(games, attributeScore);

            // Arrange
            var actualGame = Assert.Single(result, x => x.Value != 0);
            Assert.Equal(100, actualGame.Value);
        }
    }

    public class GameScoreBySeriesCalculator
    {
        public Dictionary<Guid, float> Calculate(IEnumerable<Game> games, Dictionary<Guid, float> attributeScore)
        {
            var maxScore = 0f;
            var gamesWithSeriesScores = games.ToDictionary(x => x.Id, x => x.SeriesIds.ToDictionary(s => s, s =>
            {
                if (!attributeScore.TryGetValue(s, out var value))
                {
                    return 0;
                }

                if (value > maxScore)
                {
                    maxScore = value;
                }

                return value;
            }));

            if (maxScore == 0)
            {
                return new Dictionary<Guid, float>();
            }

            return gamesWithSeriesScores.ToDictionary(x => x.Key, x => x.Value.Values.Sum() * 100 / maxScore);
        }
    }
}