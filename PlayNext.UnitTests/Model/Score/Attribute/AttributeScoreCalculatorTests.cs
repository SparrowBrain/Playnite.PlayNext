using AutoFixture.Xunit2;
using PlayNext.Model.Score.Attribute;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
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
		[InlineAutoData(nameof(Game.SeriesIds))]
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
			Assert.Single(result.Keys);
			Assert.Equal(100, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
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
		[InlineAutoData(nameof(Game.SeriesIds))]
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
			Assert.Single(result);
			Assert.Equal(25, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
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
		[AutoData]
		public void CalculateByPlaytime_ReturnsEmptyEnumerable_When_WeightIsZero(
			Game[] games,
			AttributeScoreCalculator sut)
		{
			var weight = 0f;

			var result = sut.CalculateByPlaytime(games, weight);

			Assert.Empty(result);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
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
			Assert.Single(result);
			Assert.Equal(100, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
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

		[Theory]
		[AutoData]
		public void CalculateByRecentOrder_ReturnsEmptyEnumerable_When_WeightIsZero(
			Game[] games,
			AttributeScoreCalculator sut)
		{
			var weight = 0f;

			var result = sut.CalculateByRecentOrder(games, weight);

			Assert.Empty(result);
		}

		[Theory]
		[AutoData]
		public void CalculateByFavourite_ReturnsEmpty_When_WeightIs0(
			List<Game> games,
			AttributeScoreCalculator sut)
		{
			var weight = 0f;

			var result = sut.CalculateByFavourite(games, weight);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Theory]
		[AutoData]
		public void CalculateByFavourite_ReturnsEmpty_When_NoGamesAreFavourites(
			float weight,
			List<Game> games,
			AttributeScoreCalculator sut)
		{
			foreach (var game in games)
			{
				game.Favorite = false;
			}

			var result = sut.CalculateByFavourite(games, weight);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByFavourite_Returns1AttributeWithScore100_When_1Game1AttributeWith1Weight(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			game.Favorite = true;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByFavourite(games, weight);

			Assert.NotNull(result);
			Assert.Single(result.Keys);
			Assert.Equal(100, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByFavourite_Returns2AttributeWithScore50_When_1Game2AttributeWith1Weight(
			string attributeIdsName,
			Game game,
			Guid attribute1Id,
			Guid attribute2Id,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			game.Favorite = true;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attribute1Id, attribute2Id);

			var result = sut.CalculateByFavourite(games, weight);

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
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByFavourite_Returns1AttributeWithScore25_When_1Game1AttributeWith025Weight(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 0.25f;
			game.Favorite = true;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByFavourite(games, weight);

			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(25, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByFavourite_ReturnsAttributeWithScore100_When_2GamesAndBothFavouritesWith1Weight(
			string attributeIdsName,
			Game otherGame,
			Game ourGame,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			ourGame.Favorite = true;
			otherGame.Favorite = true;
			var games = new[] { otherGame, ourGame };
			ClearAttributes(ourGame);
			SetAttributes(attributeIdsName, ourGame, attributeId);

			var result = sut.CalculateByFavourite(games, weight);

			Assert.Equal(100, result[attributeId]);
		}

		[Theory]
		[AutoData]
		public void CalculateByUserScore_ReturnsEmpty_When_WeightIs0(
			List<Game> games,
			AttributeScoreCalculator sut)
		{
			var weight = 0f;
			var averageScore = 50;

			var result = sut.CalculateByUserScore(games, averageScore, weight);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByUserScore_Returns1AttributeWithScore50_When_1Game1AttributeWith1WeightAndNoUserScore(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			var averageScore = 61;
			game.UserScore = null;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByUserScore(games, averageScore, weight);

			Assert.NotNull(result);
			Assert.Single(result.Keys);
			Assert.Equal(50, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByUserScore_Returns1AttributeWithScore100_When_1Game1AttributeWith1WeightAndUserScore100(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			var averageScore = 61;
			game.UserScore = 100;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByUserScore(games, averageScore, weight);

			Assert.NotNull(result);
			Assert.Single(result.Keys);
			Assert.Equal(100, result[attributeId]);
		}


		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByUserScore_Returns1AttributeWithScore50_When_1Game1AttributeWith1WeightAndUserAverageScore(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			var averageScore = 72;
			game.UserScore = averageScore;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByUserScore(games, averageScore, weight);

			Assert.NotNull(result);
			Assert.Single(result.Keys);
			Assert.Equal(50, result[attributeId]);
		}

		[Theory]
		[InlineAutoData(nameof(Game.GenreIds))]
		[InlineAutoData(nameof(Game.CategoryIds))]
		[InlineAutoData(nameof(Game.DeveloperIds))]
		[InlineAutoData(nameof(Game.PublisherIds))]
		[InlineAutoData(nameof(Game.TagIds))]
		[InlineAutoData(nameof(Game.SeriesIds))]
		public void CalculateByUserScore_Returns1AttributeWithScore16_When_1Game1AttributeWith1WeightAndScore25AndAverage75(
			string attributeIdsName,
			Game game,
			Guid attributeId,
			AttributeScoreCalculator sut)
		{
			var weight = 1f;
			var averageScore = 75;
			game.UserScore = 25;
			var games = new[] { game };
			ClearAttributes(game);
			SetAttributes(attributeIdsName, game, attributeId);

			var result = sut.CalculateByUserScore(games, averageScore, weight);

			Assert.NotNull(result);
			Assert.Single(result.Keys);
			Assert.Equal(16, result[attributeId]);
		}

		private static void ClearAttributes(Game game)
		{
			game.GenreIds = null;
			game.CategoryIds = null;
			game.DeveloperIds = null;
			game.PublisherIds = null;
			game.TagIds = null;
			game.SeriesIds = null;
		}

		private static void SetAttributes(string attributeIdsName, Game game, params Guid[] attributeIds)
		{
			game.GetType().GetProperty(attributeIdsName).SetValue(game, new List<Guid>(attributeIds));
		}
	}
}