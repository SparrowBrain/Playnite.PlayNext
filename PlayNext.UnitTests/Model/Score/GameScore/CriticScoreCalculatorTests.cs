using AutoFixture.Xunit2;
using PlayNext.Model.Score.GameScore;
using Playnite.SDK.Models;
using System.Collections.Generic;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
	public class CriticScoreCalculatorTests
	{
		[Theory]
		[AutoData]
		public void Calculate_ReturnsTheScores_WhenScoresExist(
			List<Game> games,
			CriticScoreCalculator sut)
		{
			// Act
			var results = sut.Calculate(games);

			// Assert
			Assert.All(results, x => Assert.Contains(games, g => g.Id == x.Key && g.CriticScore == x.Value));
		}

		[Theory]
		[AutoData]
		public void Calculate_ReturnsEmpty_WhenScoresDoNotExist(
			List<Game> games,
			CriticScoreCalculator sut)
		{
			// Arrange
			foreach (var game in games)
			{
				game.CriticScore = null;
			}

			// Act
			var results = sut.Calculate(games);

			// Assert
			Assert.Empty(results);
		}
	}
}