using AutoFixture.Xunit2;
using PlayNext.Model.Data;
using PlayNext.Model.Score.Attribute;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Model.Score.Attribute
{
    public class FinalAttributeScoreCalculatorTests
    {
        [Theory, AutoData]
        public void Calculate(
            Game[] allGames,
            Game[] recentGames,
            Game[] gamesWithRecentPlaytime,
            FinalAttributeScoreCalculator sut)
        {
            var attributeCalculationWeights = AttributeCalculationWeights.Flat;

            var result = sut.Calculate(allGames, recentGames, gamesWithRecentPlaytime, attributeCalculationWeights);
        }
    }
}