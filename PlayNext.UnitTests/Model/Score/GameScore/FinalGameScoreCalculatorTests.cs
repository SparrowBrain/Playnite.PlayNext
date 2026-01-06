using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using Moq;
using PlayNext.Extensions.HowLongToBeat;
using PlayNext.Model.Data;
using PlayNext.Model.Score.GameScore;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.GameScore
{
	public class FinalGameScoreCalculatorTests
	{
		[Theory]
		[InlineAutoMoqData(nameof(Game.GenreIds), nameof(Game.TagIds), 0.50, 0.25, 100, 50)]
		[InlineAutoMoqData(nameof(Game.FeatureIds), nameof(Game.GenreIds), 0.50, 0.49, 100, 98)]
		[InlineAutoMoqData(nameof(Game.DeveloperIds), nameof(Game.FeatureIds), 0.2, 0.1, 100, 50)]
		[InlineAutoMoqData(nameof(Game.PublisherIds), nameof(Game.DeveloperIds), 0.50, 0.49, 100, 98)]
		[InlineAutoMoqData(nameof(Game.TagIds), nameof(Game.PublisherIds), 0.50, 0.49, 100, 98)]
		public void TwoAttributesBetweenTwoGames(
			string topAttribute,
			string secondTopAttribute,
			float topAttributeWeight,
			float secondTopAttributeWeight,
			float topGameScore,
			float secondTopGameScore,
			Dictionary<Guid, float> topAttributeScores,
			Dictionary<Guid, float> secondTopAttributeScores,
			Game[] games,
			OrderSeriesBy orderSeriesBy,
			int releaseYear,
			TimeSpan gameLength,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForAttribute(topAttribute, gameScoreCalculationWeights, topAttributeWeight);
			SetWeightForAttribute(secondTopAttribute, gameScoreCalculationWeights, secondTopAttributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			SetAttributes(topAttribute, topGame, topAttributeScores.Keys.ToArray());
			SetAttributes(secondTopAttribute, secondTopGame, secondTopAttributeScores.Keys.ToArray());
			var attributeScore = topAttributeScores.Concat(secondTopAttributeScores).ToDictionary(x => x.Key, x => x.Value);

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, gameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
		}

		[Theory]
		[InlineAutoMoqData(nameof(Game.GenreIds), 0.50, 100, 50, 100, 50)]
		[InlineAutoMoqData(nameof(Game.FeatureIds), 0.1, 5, 1, 100, 20)]
		[InlineAutoMoqData(nameof(Game.DeveloperIds), 1, 50, 25, 100, 50)]
		[InlineAutoMoqData(nameof(Game.PublisherIds), 0.33, 100, 1, 100, 1)]
		[InlineAutoMoqData(nameof(Game.TagIds), 0.2, 50, 49, 100, 98)]
		public void SameAttributeBetweenTwoGames(
			string attribute,
			float attributeWeight,
			float topGameAttributeSum,
			float secondGameTopAttributeSum,
			float topGameScore,
			float secondTopGameScore,
			Game[] games,
			int releaseYear,
			OrderSeriesBy orderSeriesBy,
			TimeSpan gameLength,
			Guid topAttributeId,
			Guid secondTopAttributeId,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var topAttributeScores = new Dictionary<Guid, float>() { { topAttributeId, topGameAttributeSum } };
			var secondTopAttributeScores = new Dictionary<Guid, float>() { { secondTopAttributeId, secondGameTopAttributeSum } };
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForAttribute(attribute, gameScoreCalculationWeights, attributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			SetAttributes(attribute, topGame, topAttributeScores.Keys.ToArray());
			SetAttributes(attribute, secondTopGame, secondTopAttributeScores.Keys.ToArray());
			var attributeScore = topAttributeScores.Concat(secondTopAttributeScores).ToDictionary(x => x.Key, x => x.Value);

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, gameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
		}

		[Theory]
		[InlineAutoMoqData(nameof(Game.CriticScore), 100, nameof(Game.TagIds), 0.50, 0.25, 100, 50)]
		[InlineAutoMoqData(nameof(Game.CommunityScore), 100, nameof(Game.GenreIds), 0.50, 0.49, 100, 98)]
		public void GamePropertyAndAttributeBetweenTwoGames(
			string topProperty,
			int propertyValue,
			string secondTopAttribute,
			float topWeight,
			float secondTopAttributeWeight,
			float topGameScore,
			float secondTopGameScore,
			Dictionary<Guid, float> attributeScore,
			Game[] games,
			OrderSeriesBy orderSeriesBy,
			int releaseYear,
			TimeSpan gameLength,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForProperty(topProperty, gameScoreCalculationWeights, topWeight);
			SetWeightForAttribute(secondTopAttribute, gameScoreCalculationWeights, secondTopAttributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			games = new[] { secondTopGame, topGame };
			SetProperty(topProperty, topGame, propertyValue);
			SetAttributes(secondTopAttribute, secondTopGame, attributeScore.Keys.ToArray());

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, gameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
		}

		[Theory]
		[InlineAutoMoqData(nameof(GameScoreWeights.ReleaseYear), nameof(Game.TagIds), 0.50, 0.25, 100, 50)]
		public void ReleaseYearAndAttributeBetweenTwoGames(
			string topProperty,
			string secondTopAttribute,
			float topWeight,
			float secondTopAttributeWeight,
			float topGameScore,
			float secondTopGameScore,
			Dictionary<Guid, float> attributeScore,
			Game[] games,
			OrderSeriesBy orderSeriesBy,
			int releaseYear,
			TimeSpan gameLength,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForProperty(topProperty, gameScoreCalculationWeights, topWeight);
			SetWeightForAttribute(secondTopAttribute, gameScoreCalculationWeights, secondTopAttributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			games = new[] { secondTopGame, topGame };
			var topGameReleaseDate = new ReleaseDate(releaseYear);
			SetProperty(nameof(Game.ReleaseDate), topGame, topGameReleaseDate);
			SetAttributes(secondTopAttribute, secondTopGame, attributeScore.Keys.ToArray());

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, gameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
		}

		[Theory]
		[InlineAutoMoqData(nameof(GameScoreWeights.GameLength), nameof(Game.DeveloperIds), 0.50, 0.25, 100, 50)]
		public void GameLengthAndAttributeBetweenTwoGames(
			string topProperty,
			string secondTopAttribute,
			float topWeight,
			float secondTopAttributeWeight,
			float topGameScore,
			float secondTopGameScore,
			[Frozen] Mock<IHowLongToBeatExtension> howLongToBeatExtensionMock,
			Dictionary<Guid, float> attributeScore,
			Game[] games,
			OrderSeriesBy orderSeriesBy,
			int releaseYear,
			TimeSpan preferredGameLength,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForProperty(topProperty, gameScoreCalculationWeights, topWeight);
			SetWeightForAttribute(secondTopAttribute, gameScoreCalculationWeights, secondTopAttributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			games = new[] { secondTopGame, topGame };
			SetAttributes(secondTopAttribute, secondTopGame, attributeScore.Keys.ToArray());
			howLongToBeatExtensionMock.Setup(x => x.GetTimeToPlay()).Returns(new Dictionary<Guid, int>()
				{ { topGame.Id, (int)preferredGameLength.TotalSeconds } });

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, preferredGameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
		}

		[Theory]
		[InlineAutoMoqData(nameof(GameScoreWeights.Random), nameof(Game.PublisherIds), 0.50, 0.25, 100, 50)]
		public void RandomAndAttributeBetweenTwoGames(
			string topProperty,
			string secondTopAttribute,
			float topWeight,
			float secondTopAttributeWeight,
			float topGameScore,
			float secondTopGameScore,
			[Frozen] Mock<IRandomScoreCalculator> randomScoreCalculatorMock,
			Dictionary<Guid, float> attributeScore,
			Game[] games,
			OrderSeriesBy orderSeriesBy,
			int releaseYear,
			TimeSpan preferredGameLength,
			FinalGameScoreCalculator sut)
		{
			// Arrange
			var gameScoreCalculationWeights = GetEmptyWeights();
			SetWeightForProperty(topProperty, gameScoreCalculationWeights, topWeight);
			SetWeightForAttribute(secondTopAttribute, gameScoreCalculationWeights, secondTopAttributeWeight);

			var topGame = games.Last();
			var secondTopGame = games.First();
			games = new[] { secondTopGame, topGame };
			SetAttributes(secondTopAttribute, secondTopGame, attributeScore.Keys.ToArray());
			randomScoreCalculatorMock.Setup(x => x.Calculate(It.IsAny<IEnumerable<Game>>())).Returns(new Dictionary<Guid, float>()
				{ { topGame.Id, 100 } });

			// Act
			var result = sut.Calculate(games, attributeScore, gameScoreCalculationWeights, orderSeriesBy, releaseYear, preferredGameLength);

			// Assert
			Assert.Equal(topGame.Id, result.First().Key);
			Assert.Equal(topGameScore, result.First().Value);
			Assert.Equal(secondTopGame.Id, result.Skip(1).First().Key);
			Assert.Equal(secondTopGameScore, result.Skip(1).First().Value);
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

			game.CriticScore = null;
			game.CommunityScore = null;

			game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
		}

		private static void SetProperty<T>(string propertyName, Game game, T value)
		{
			game.GenreIds = null;
			game.CategoryIds = null;
			game.DeveloperIds = null;
			game.PublisherIds = null;
			game.TagIds = null;

			game.CriticScore = null;
			game.CommunityScore = null;

			game.GetType().GetProperty(propertyName).SetValue(game, value);
		}

		private static void SetWeightForAttribute(string attributeIdsName, GameScoreWeights weights, float value)
		{
			SetWeightForProperty(attributeIdsName.Substring(0, attributeIdsName.Length - 3), weights, value);
		}

		private static void SetWeightForProperty(string propertyName, GameScoreWeights weights, float value)
		{
			weights.GetType().GetProperty(propertyName).SetValue(weights, value);
		}
	}
}