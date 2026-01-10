using PlayNext.Model.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;

namespace PlayNext.Model.Score.Attribute
{
	public class FinalAttributeScoreCalculator
	{
		private readonly AttributeScoreCalculator _attributeScoreCalculator;
		private readonly Summator _summator;

		public FinalAttributeScoreCalculator(AttributeScoreCalculator attributeScoreCalculator, Summator summator)
		{
			_attributeScoreCalculator = attributeScoreCalculator;
			_summator = summator;
		}

		public Dictionary<Guid, float> Calculate(IReadOnlyCollection<Game> allGames, IEnumerable<Game> gamesWithRecentPlaytime, IEnumerable<Game> recentGames, AttributeCalculationWeights attributeCalculationWeights)
		{
			var weightedTotalPlaytimeScore = _attributeScoreCalculator.CalculateByPlaytime(allGames, attributeCalculationWeights.TotalPlaytime);
			var weightedRecentPlaytimeScore = _attributeScoreCalculator.CalculateByPlaytime(gamesWithRecentPlaytime, attributeCalculationWeights.RecentPlaytime);
			var weightedRecentOrderScore = _attributeScoreCalculator.CalculateByRecentOrder(recentGames, attributeCalculationWeights.RecentOrder);
			var weightedUserFavouritesScore = _attributeScoreCalculator.CalculateByFavourite(allGames, attributeCalculationWeights.UserFavourites);

			var sum = _summator.AddUp(
				weightedTotalPlaytimeScore,
				weightedRecentPlaytimeScore,
				weightedRecentOrderScore,
				weightedUserFavouritesScore);

			return sum;
		}
	}
}