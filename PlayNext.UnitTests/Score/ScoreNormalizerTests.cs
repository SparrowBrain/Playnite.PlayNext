using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Score;
using Xunit;

namespace PlayNext.UnitTests.Score
{
    public class ScoreNormalizerTests
    {
        [Theory, AutoData]
        public void Normalize_ReturnsSingleGameWithScore100_When_ThereIsOneGame(
            Guid id,
            float score,
            ScoreNormalizer sut)
        {
            // Arrange
            var scores = new Dictionary<Guid, float>() { { id, score } };

            // Act
            var result = sut.Normalize(scores);

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(id, single.Key);
            Assert.Equal(100, single.Value);
        }

        [Theory, AutoData]
        public void Normalize_ReturnsGameWithScore50_When_TheScoreWasHalfTheMaximum(
            Guid maxScoredGameId,
            Dictionary<Guid, float> scores,
            ScoreNormalizer sut)
        {
            // Arrange
            var halfScoredGame = scores.OrderByDescending(x => x.Value).First();
            var maxScoredGame = new KeyValuePair<Guid, float>(maxScoredGameId, halfScoredGame.Value * 2);
            scores.Add(maxScoredGame.Key, maxScoredGame.Value);

            // Act
            var result = sut.Normalize(scores);

            // Assert
            var halfScoredNormalized = result.OrderByDescending(x => x.Value).Skip(1).First();
            Assert.Equal(halfScoredGame.Key, halfScoredNormalized.Key);
            Assert.Equal(50, halfScoredNormalized.Value);
        }
    }
}