using System;
using System.Collections.Generic;
using PlayNext.Model.Data;
using Playnite.SDK.Models;

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

        public Dictionary<Guid, float> Calculate(IEnumerable<Game> allGames, IEnumerable<Game> recentGames, AttributeCalculationWeights attributeCalculationWeights)
        {
            var weightedTotalPlaytimeScore = _attributeScoreCalculator.CalculateByPlaytime(allGames, attributeCalculationWeights.TotalPlaytime);
            var weightedRecentPlaytimeScore = _attributeScoreCalculator.CalculateByPlaytime(recentGames, attributeCalculationWeights.RecentPlaytime);
            var weightedRecentOrderScore = _attributeScoreCalculator.CalculateByRecentOrder(recentGames, attributeCalculationWeights.RecentOrder);

            var sum = _summator.AddUp(
                weightedTotalPlaytimeScore,
                weightedRecentPlaytimeScore,
                weightedRecentOrderScore);

            return sum;
        }
    }
}