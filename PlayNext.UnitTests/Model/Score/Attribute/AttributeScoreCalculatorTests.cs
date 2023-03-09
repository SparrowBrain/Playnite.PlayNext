using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using PlayNext.Model.Score.Attribute;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.Attribute
{
    public class AttributeScoreCalculatorTests
    {
        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByPlaytime_Returns1AttributeWithScore100_When_1Game1AttributeWith1Weight(
            string attributeIdsName,
            Game game,
            Guid attributeId,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new[] { game };
            ClearAttributes(game);
            SetAttributes(attributeIdsName, game, attributeId);

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.NotNull(result);
            Assert.Equal(1, result.Keys.Count);
            Assert.Equal(100, result[attributeId]);
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByPlaytime_Returns2AttributeWithScore50_When_1Game2AttributeWith1Weight(
            string attributeIdsName,
            Game game,
            Guid attribute1Id,
            Guid attribute2Id,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new[] { game };
            ClearAttributes(game);
            SetAttributes(attributeIdsName, game, attribute1Id, attribute2Id);

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.NotNull(result);
            Assert.Equal(2, result.Keys.Count);
            Assert.Equal(50, result[attribute1Id]);
            Assert.Equal(50, result[attribute2Id]);
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByPlaytime_Returns1AttributeWithScore25_When_1Game1AttributeWith025Weight(
            string attributeIdsName,
            Game game,
            Guid attributeId,
            AttributeScoreCalculator sut)
        {
            var weight = 0.25f;
            var games = new[] { game };
            ClearAttributes(game);
            SetAttributes(attributeIdsName, game, attributeId);

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.NotNull(result);
            Assert.Equal(1, result.Keys.Count);
            Assert.Equal(25, result[attributeId]);
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByPlaytime_ReturnsAttributeWithScore50_When_2GamesAndAttributeInGameWithHalfThePlaytimeWith1Weight(
            string attributeIdsName,
            Game maxPlaytimeGame,
            Game ourGame,
            Guid attributeId,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new[] { maxPlaytimeGame, ourGame };
            maxPlaytimeGame.Playtime = ourGame.Playtime * 2;
            ClearAttributes(ourGame);
            SetAttributes(attributeIdsName, ourGame, attributeId);

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.Equal(50, result[attributeId]);
        }

        [Theory]
        [AutoData]
        public void CalculateByPlaytime_ReturnsEmptyEnumerable_When_NoGames(AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new Game[] { };

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.Empty(result);
        }

        [Theory]
        [AutoData]
        public void CalculateByPlaytime_ReturnsGamesWithScoreZero_When_AllGamesWithZeroPlaytime(
            Game[] games,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            foreach (var game in games)
            {
                game.Playtime = 0;
            }

            var result = sut.CalculateByPlaytime(games, weight);

            Assert.All(result, (gameScore, index) => Assert.Equal(0, gameScore.Value));
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByRecentOrder_Returns1AttributeWithScore100_When_1Game1AttributeWith1Weight(
            string attributeIdsName,
            Game game,
            Guid attributeId,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new[] { game };
            ClearAttributes(game);
            SetAttributes(attributeIdsName, game, attributeId);

            var result = sut.CalculateByRecentOrder(games, weight);

            Assert.NotNull(result);
            Assert.Equal(1, result.Keys.Count);
            Assert.Equal(100, result[attributeId]);
        }

        [Theory]
        [InlineAutoData(nameof(Game.GenreIds))]
        [InlineAutoData(nameof(Game.CategoryIds))]
        [InlineAutoData(nameof(Game.DeveloperIds))]
        [InlineAutoData(nameof(Game.PublisherIds))]
        [InlineAutoData(nameof(Game.TagIds))]
        public void CalculateByRecentOrder_ReturnsAttributeWithScore50_When_2GamesAndAttributeInLessRecentGameWith1Weight(
            string attributeIdsName,
            Game mostRecentGame,
            Game ourGame,
            Guid attributeId,
            AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new[] { mostRecentGame, ourGame };
            mostRecentGame.LastActivity = ourGame.LastActivity + TimeSpan.FromHours(1);
            ClearAttributes(ourGame);
            SetAttributes(attributeIdsName, ourGame, attributeId);

            var result = sut.CalculateByRecentOrder(games, weight);

            Assert.Equal(50, result[attributeId]);
        }

        [Theory]
        [AutoData]
        public void CalculateByRecentOrder_ReturnsEmptyEnumerable_When_NoGames(AttributeScoreCalculator sut)
        {
            var weight = 1f;
            var games = new Game[] { };

            var result = sut.CalculateByRecentOrder(games, weight);

            Assert.Empty(result);
        }

        private static void ClearAttributes(Game game)
        {
            game.GenreIds = null;
            game.CategoryIds = null;
            game.DeveloperIds = null;
            game.PublisherIds = null;
            game.TagIds = null;
        }

        private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
        {
            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }
    }
}