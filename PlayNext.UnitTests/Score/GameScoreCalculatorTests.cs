using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Score;
using PlayNext.Settings;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Score
{
    public class GameScoreCalculatorTests
    {
        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void Calculate(
            string attributeIdsName,
            Game[] games,
            Dictionary<Guid, float> attributeScore,
            GameScoreCalculator sut)
        {
            // Arrange
            var gameScoreCalculationWeights = GetEmptyWeights();
            gameScoreCalculationWeights.Genre = 1;

            var topGame = games.Last();
            SetAttributes(attributeIdsName, topGame, attributeScore.Keys.ToArray());
            SetWeight(attributeIdsName, gameScoreCalculationWeights, 1);

            // Act
            var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights);

            // Assert
            Assert.Equal(topGame.Id, result.First().Key);
            Assert.Equal(100, result.First().Value);
        }

        private GameScoreCalculationWeights GetEmptyWeights()
        {
            return new GameScoreCalculationWeights
            {
                Genre = 0,
                Category = 0,
                Developer = 0,
                Publisher = 0,
                Tag = 0,
            };
        }

        private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
        {
            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }

        private static void SetWeight(string attributeIdsName, GameScoreCalculationWeights weights, float value)
        {
            weights.GetType().GetProperty(attributeIdsName.Substring(0, attributeIdsName.Length - 3)).SetValue(weights, value);
        }
    }
}