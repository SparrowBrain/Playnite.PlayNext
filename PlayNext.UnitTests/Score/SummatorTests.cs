using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using PlayNext.Score;
using Xunit;

namespace PlayNext.UnitTests.Score
{
    public class SummatorTests
    {
        [Theory, AutoData]
        public void AddUp_ReturnsSameScores_When_SingleAttributeScoresIsPassedIn(
            Dictionary<Guid, float> singleAttributeScores,
            Summator sut)
        {
            // Arrange
            var expectedItem = singleAttributeScores.First();

            // Act
            var result = sut.AddUp(singleAttributeScores);

            // Assert
            Assert.Equal(singleAttributeScores.Count, result.Count);
            Assert.True(result.ContainsKey(expectedItem.Key));
            var value = result[expectedItem.Key];
            Assert.Equal(expectedItem.Value, value);
        }

        [Theory, AutoData]
        public void AddUp_ReturnsAddedUpScores_WhenTwoAttributesScoresPassedIn(
            Guid gameId,
            Summator sut)
        {
            // Arrange
            var attributeScores1 = new Dictionary<Guid, float>() { { gameId, 1f } };
            var attributeScores2 = new Dictionary<Guid, float>() { { gameId, 10f } };

            // Act
            var result = sut.AddUp(attributeScores1, attributeScores2);

            // Assert
            Assert.Single(result);
            Assert.Equal(11f, result[gameId]);
        }
    }
}