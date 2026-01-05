using AutoFixture.Xunit2;
using PlayNext.Settings;
using PlayNext.Settings.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlayNext.UnitTests.Settings.Old
{
	public class SettingsV3Tests
	{
		[Theory, AutoData]
		public void Migrate_MigratesToV4(
			SettingsV3 oldSettings)
		{
			// Act
			var result = oldSettings.Migrate() as PlayNextSettings;

			// Assert
			Assert.NotNull(result);
			Assert.Equal(oldSettings.SelectedPresetId, result.SelectedPresetId);
			Assert.Equal(oldSettings.RecentDays, result.RecentDays);
			Assert.Equal(oldSettings.NumberOfTopGames, result.NumberOfTopGames);
			Assert.Equal(oldSettings.RefreshOnGameUpdates, result.RefreshOnGameUpdates);

			Assert.Equal(oldSettings.TotalPlaytimeWeight, result.TotalPlaytimeWeight);
			Assert.Equal(oldSettings.RecentPlaytimeWeight, result.RecentPlaytimeWeight);
			Assert.Equal(oldSettings.RecentOrderWeight, result.RecentOrderWeight);

			Assert.Equal(oldSettings.GenreWeight, result.GenreWeight);
			Assert.Equal(oldSettings.FeatureWeight, result.FeatureWeight);
			Assert.Equal(oldSettings.DeveloperWeight, result.DeveloperWeight);
			Assert.Equal(oldSettings.PublisherWeight, result.PublisherWeight);
			Assert.Equal(oldSettings.TagWeight, result.TagWeight);
			Assert.Equal(oldSettings.CriticScoreWeight, result.CriticScoreWeight);
			Assert.Equal(oldSettings.CommunityScoreWeight, result.CommunityScoreWeight);
			Assert.Equal(oldSettings.ReleaseYearWeight, result.ReleaseYearWeight);
			Assert.Equal(oldSettings.ReleaseYearChoice, result.ReleaseYearChoice);
			Assert.Equal(oldSettings.DesiredReleaseYear, result.DesiredReleaseYear);
			Assert.Equal(oldSettings.GameLengthWeight, result.GameLengthWeight);
			Assert.Equal(oldSettings.GameLengthHours, result.GameLengthHours);
			Assert.Equal(oldSettings.SeriesWeight, result.SeriesWeight);
			Assert.Equal(oldSettings.OrderSeriesBy, result.OrderSeriesBy);

			Assert.Equal(oldSettings.UnplayedGameDefinition, result.UnplayedGameDefinition);
			Assert.Equal(oldSettings.UnplayedCompletionStatuses, result.UnplayedCompletionStatuses);

			Assert.Equivalent(oldSettings.ExcludedSourceIds, result.ExcludedSourceIds);
			Assert.Equivalent(oldSettings.ExcludedPlatformIds, result.ExcludedPlatformIds);
			Assert.Equivalent(oldSettings.ExcludedCategoryIds, result.ExcludedCategoryIds);
			Assert.Equivalent(oldSettings.ExcludedTagIds, result.ExcludedTagIds);

			Assert.Equal(oldSettings.StartPageShowLabel, result.StartPageShowLabel);
			Assert.Equal(oldSettings.StartPageLabelIsHorizontal, result.StartPageLabelIsHorizontal);
			Assert.Equal(oldSettings.StartPageMinCoverCount, result.StartPageMinCoverCount);

			Assert.Equal(4, result.Version);
		}
	}
}