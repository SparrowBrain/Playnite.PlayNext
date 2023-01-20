using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Score.GameScore;
using Xunit;

namespace PlayNext.UnitTests.Score.GameScore
{
    public class GameScoreByAttributeCalculatorTests
    {
        [Theory]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.GenreIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.FeatureIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.DeveloperIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.PublisherIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.TagIds))]
        public void Calculate_ReturnsGameWithAttributeScore_When_1GameAnd1Attribute(
            string attributeIdsName,
            Dictionary<Guid, float> attributeScore,
            Playnite.SDK.Models.Game game,
            GameScoreByAttributeCalculator sut)
        {
            // Arrange
            var games = new[] { game };
            var attributeInGame = attributeScore.Keys.First();
            SetAttributes(attributeIdsName, game, attributeInGame);
            var attributeSelector = new Func<Playnite.SDK.Models.Game, IEnumerable<Guid>>(x => (IEnumerable<Guid>)x.GetType().GetProperty(attributeIdsName).GetValue(x));

            // Act
            var result = sut.Calculate(games, attributeSelector, attributeScore);

            // Assert
            var gameWithScore = Assert.Single(result);
            Assert.Equal(attributeScore[attributeInGame], gameWithScore.Value);
            Assert.Equal(game.Id, gameWithScore.Key);
        }

        [Theory]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.GenreIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.FeatureIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.DeveloperIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.PublisherIds))]
        [InlineAutoData(nameof(Playnite.SDK.Models.Game.TagIds))]
        public void Calculate_ReturnsSumOfAttributesScore_When_1GameAndManyAttributes(
            string attributeIdsName,
            Dictionary<Guid, float> attributeScore,
            Playnite.SDK.Models.Game game,
            GameScoreByAttributeCalculator sut)
        {
            // Arrange
            var games = new[] { game };
            SetAttributes(attributeIdsName, game, attributeScore.Keys.ToArray());
            var attributeSelector = new Func<Playnite.SDK.Models.Game, IEnumerable<Guid>>(x => (IEnumerable<Guid>)x.GetType().GetProperty(attributeIdsName).GetValue(x));

            // Act
            var result = sut.Calculate(games, attributeSelector, attributeScore);

            // Assert
            var gameWithScore = Assert.Single(result);
            Assert.Equal(attributeScore.Values.Sum(), gameWithScore.Value);
            Assert.Equal(game.Id, gameWithScore.Key);
        }

        [Theory, AutoData]
        public void Calculate_Zero_When_AttributeIdsIsNull(
            Dictionary<Guid, float> attributeScore,
            Playnite.SDK.Models.Game game,
            GameScoreByAttributeCalculator sut)
        {
            // Arrange
            var games = new[] { game };
            var attributeSelector = new Func<Playnite.SDK.Models.Game, IEnumerable<Guid>>(x => null);

            // Act
            var result = sut.Calculate(games, attributeSelector, attributeScore);

            // Assert
            var gameWithScore = Assert.Single(result);
            Assert.Equal(game.Id, gameWithScore.Key);
            Assert.Equal(0, gameWithScore.Value);
        }

        private static void SetAttributes(string attributeIdsName, Playnite.SDK.Models.Game game, params Guid[] attributeIds)
        {
            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }
    }
}