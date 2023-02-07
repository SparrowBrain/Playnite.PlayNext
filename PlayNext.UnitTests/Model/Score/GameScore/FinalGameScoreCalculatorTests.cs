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
    public class FinalGameScoreCalculatorTests
    {
        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.FeatureIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void Calculate(
            string attributeIdsName,
            Game[] games,
            int releaseYear,
            Dictionary<Guid, float> attributeScore,
            FinalGameScoreCalculator sut)
        {
            // Arrange
            var gameScoreCalculationWeights = GetEmptyWeights();
            gameScoreCalculationWeights.Genre = 1;

            var topGame = games.Last();
            SetAttributes(attributeIdsName, topGame, attributeScore.Keys.ToArray());
            SetWeight(attributeIdsName, gameScoreCalculationWeights, 1);

            // Act
            var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, releaseYear);

            // Assert
            Assert.Equal(topGame.Id, result.First().Key);
            Assert.Equal(100, result.First().Value);
        }

        private GameScoreWeights GetEmptyWeights()
        {
            return new GameScoreWeights
            {
                Genre = 0,
                Feature = 0,
                Developer = 0,
                Publisher = 0,
                Tag = 0,
            };
        }

        private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
        {
            game.GenreIds = null;
            game.CategoryIds = null;
            game.DeveloperIds = null;
            game.PublisherIds = null;
            game.TagIds = null;

            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }

        private static void SetWeight(string attributeIdsName, GameScoreWeights weights, float value)
        {
            weights.GetType().GetProperty(attributeIdsName.Substring(0, attributeIdsName.Length - 3)).SetValue(weights, value);
        }
    }
}