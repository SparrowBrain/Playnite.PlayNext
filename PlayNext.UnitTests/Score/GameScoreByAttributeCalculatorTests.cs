using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Score;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Score
{
    public class GameScoreByAttributeCalculatorTests
    {
        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void Calculate_ReturnsGameWithAttributeScore_When_1GameAnd1Attribute(
            string attributeIdsName,
            Dictionary<Guid, float> attributeScore,
            Game game,
            GameScoreByAttributeCalculator sut)
        {
            // Arrange
            var games = new[] { game };
            var attributeInGame = attributeScore.Keys.First();
            SetAttributes(attributeIdsName, game, attributeInGame);
            var attributeSelector = new Func<Game, IEnumerable<Guid>>(x => (IEnumerable<Guid>)x.GetType().GetProperty(attributeIdsName).GetValue(x));

            // Act
            var result = sut.Calculate(games, attributeSelector, attributeScore);

            // Assert
            var gameWithScore = Assert.Single(result);
            Assert.Equal(attributeScore[attributeInGame], gameWithScore.Value);
            Assert.Equal(game.Id, gameWithScore.Key);
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void Calculate_ReturnsSumOfAttributesScore_When_1GameAndManyAttributes(
            string attributeIdsName,
            Dictionary<Guid, float> attributeScore,
            Game game,
            GameScoreByAttributeCalculator sut)
        {
            // Arrange
            var games = new[] { game };
            SetAttributes(attributeIdsName, game, attributeScore.Keys.ToArray());
            var attributeSelector = new Func<Game, IEnumerable<Guid>>(x => (IEnumerable<Guid>)x.GetType().GetProperty(attributeIdsName).GetValue(x));

            // Act
            var result = sut.Calculate(games, attributeSelector, attributeScore);

            // Assert
            var gameWithScore = Assert.Single(result);
            Assert.Equal(attributeScore.Values.Sum(), gameWithScore.Value);
            Assert.Equal(game.Id, gameWithScore.Key);
        }

        private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
        {
            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }
    }
}