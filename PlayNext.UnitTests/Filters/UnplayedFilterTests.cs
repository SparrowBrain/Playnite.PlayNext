using AutoFixture.Xunit2;
using Playnite.SDK.Models;
using Xunit;

namespace PlayNext.UnitTests.Filters
{
    public class UnplayedFilterTests
    {
        [Theory, AutoData]
        public void Filter_OneGame_When_OneGameHasUnplayedStatus(
            Game[] games,
            UnplayedFilter sut)
        {
            // Arrange
            foreach (var game in games)
            {
                //(game.CompletionStatus.Name.Equals("Unplayed", StringComparison.CurrentCultureIgnoreCase))
                {
                }
            }
        }
    }

    public class UnplayedFilter
    {
    }
}