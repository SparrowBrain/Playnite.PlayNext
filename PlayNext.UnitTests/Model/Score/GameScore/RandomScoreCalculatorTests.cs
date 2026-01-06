using AutoFixture.Xunit2;
using Playnite.SDK.Models;
using System.Collections.Generic;
using PlayNext.Model.Score.GameScore;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
	public class RandomScoreCalculatorTests
	{
		[Theory]
		[AutoData]
		public void Calculate_AssignsRandomScores(
			List<Game> games,
			RandomScoreCalculator sut)
		{
			// Act
			var result = sut.Calculate(games);

			// Assert
			Assert.Equal(games.Count, result.Count);
			Assert.All(result, x => Assert.True(x.Value >= 0f && x.Value <= 100f));
		}
	}
}