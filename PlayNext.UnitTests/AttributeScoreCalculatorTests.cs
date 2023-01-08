using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests
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

        private static void ClearAttributes(Game game)
        {
            game.GenreIds = new List<Guid>();
            game.CategoryIds = new List<Guid>();
            game.DeveloperIds = new List<Guid>();
            game.PublisherIds = new List<Guid>();
            game.TagIds = new List<Guid>();
        }

        private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
        {
            game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
        }
    }

    public class AttributeScoreCalculator
    {
        public Dictionary<Guid, float> CalculateByPlaytime(IEnumerable<Game> games, float weight)
        {
            var maxTime = games.Max(x => x.Playtime);
            var scores = new Dictionary<Guid, float>();

            foreach (var game in games)
            {
                CalculateAttributeScore(game, game.GenreIds, weight, maxTime, scores);
                CalculateAttributeScore(game, game.CategoryIds, weight, maxTime, scores);
                CalculateAttributeScore(game, game.DeveloperIds, weight, maxTime, scores);
                CalculateAttributeScore(game, game.PublisherIds, weight, maxTime, scores);
                CalculateAttributeScore(game, game.TagIds, weight, maxTime, scores);
            }

            return scores;
        }

        private static void CalculateAttributeScore(Game game, List<Guid> attributeIds, float weight, ulong maxTime, Dictionary<Guid, float> scores)
        {
            var genreScore = game.Playtime * 100 * weight / attributeIds.Count / maxTime;
            foreach (var genreId in attributeIds)
            {
                if (scores.ContainsKey(genreId))
                {
                    scores[genreId] += genreScore;
                }
                else
                {
                    scores[genreId] = genreScore;
                }
            }
        }
    }
}