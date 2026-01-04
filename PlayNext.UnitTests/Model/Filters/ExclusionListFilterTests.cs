using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Model.Filters;
using PlayNext.Settings;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Filters
{
	public class ExclusionListFilterTests
	{
		[Theory]
		[AutoData]
		public void Filter_ExcludesGame_ThatHasSourceIdFiltered(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var filteredOutGame = games.Last();
			filteredOutGame.SourceId = settings.ExcludedSourceIds.First();

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count - 1, result.Count);
			Assert.DoesNotContain(result, x => x.Id == filteredOutGame.Id);
		}

		[Theory]
		[AutoData]
		public void Filter_ExcludesGame_ThatHasPlatformIdFiltered(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var filteredOutGame = games.Last();
			filteredOutGame.PlatformIds.Add(settings.ExcludedPlatformIds.First());

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count - 1, result.Count);
			Assert.DoesNotContain(result, x => x.Id == filteredOutGame.Id);
		}

		[Theory]
		[AutoData]
		public void Filter_ReturnsGames_WhenPlatformIdsIsNull(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var specialGame = games.Last();
			specialGame.PlatformIds = null;

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count, result.Count);
		}

		[Theory]
		[AutoData]
		public void Filter_ExcludesGame_ThatHasCategoryIdFiltered(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var filteredOutGame = games.Last();
			filteredOutGame.CategoryIds.Add(settings.ExcludedCategoryIds.First());

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count - 1, result.Count);
			Assert.DoesNotContain(result, x => x.Id == filteredOutGame.Id);
		}

		[Theory]
		[AutoData]
		public void Filter_ReturnsGames_WhenCategoryIdsIsNull(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var specialGame = games.Last();
			specialGame.CategoryIds = null;

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count, result.Count);
		}

		[Theory]
		[AutoData]
		public void Filter_ExcludesGame_ThatHasTagIdFiltered(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var filteredOutGame = games.Last();
			filteredOutGame.TagIds.Add(settings.ExcludedTagIds.First());

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count - 1, result.Count);
			Assert.DoesNotContain(result, x => x.Id == filteredOutGame.Id);
		}

		[Theory]
		[AutoData]
		public void Filter_ReturnsGames_WhenTagIdsIsNull(
			List<Game> games,
			PlayNextSettings settings,
			ExclusionListFilter sut)
		{
			// Arrange
			var specialGame = games.Last();
			specialGame.TagIds = null;

			// Act
			var result = sut.Filter(games, settings);

			// Assert
			Assert.Equal(games.Count, result.Count);
		}
	}
}