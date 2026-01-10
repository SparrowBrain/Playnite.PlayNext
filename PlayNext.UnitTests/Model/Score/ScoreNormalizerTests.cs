using AutoFixture.Xunit2;
using PlayNext.Model.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PlayNext.UnitTests.Model.Score
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

		[Theory, AutoData]
		public void Normalize_ReturnsGameWith0_When_TheScoreWasNegative(
			Dictionary<Guid, float> scores,
			ScoreNormalizer sut)
		{
			// Arrange
			var negativeScoreGame = scores.OrderByDescending(x => x.Value).First();
			scores[negativeScoreGame.Key] = -negativeScoreGame.Value;

			// Act
			var result = sut.Normalize(scores);

			// Assert
			var negativeScoreNormalized = result.OrderBy(x => x.Value).First();
			Assert.Equal(negativeScoreGame.Key, negativeScoreNormalized.Key);
			Assert.Equal(0, negativeScoreNormalized.Value);
		}

		[Theory, AutoData]
		public void Normalize_EmptyList_When_GivenScoreListIsEmpty(
			ScoreNormalizer sut)
		{
			// Arrange
			var scores = new Dictionary<Guid, float>();

			// Act
			var result = sut.Normalize(scores);

			// Assert
			Assert.Empty(result);
		}

		[Theory, AutoData]
		public void Normalize_EmptyList_When_GivenScoreListIsNull(
			ScoreNormalizer sut)
		{
			// Act
			var result = sut.Normalize(null);

			// Assert
			Assert.Empty(result);
		}
	}
}