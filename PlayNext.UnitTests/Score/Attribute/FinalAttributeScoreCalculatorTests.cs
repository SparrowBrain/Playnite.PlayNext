using AutoFixture.Xunit2;
using PlayNext.Models;
using PlayNext.Score.Attribute;
using Xunit;

namespace PlayNext.UnitTests.Score.Attribute
{
    public class FinalAttributeScoreCalculatorTests
    {
        [Theory, AutoData]
        public void Calculate(
            Playnite.SDK.Models.Game[] allGames,
            Playnite.SDK.Models.Game[] recentGames,
            FinalAttributeScoreCalculator sut)
        {
            var attributeCalculationWeights = AttributeCalculationWeights.Flat;

            var result = sut.Calculate(allGames, recentGames, attributeCalculationWeights);
        }
    }
}