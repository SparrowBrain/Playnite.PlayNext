﻿using System.Collections.Generic;
using System.Linq;
using PlayNext.Model.Filters;
using Playnite.SDK.Models;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Model.Filters
{
    public class WithPlaytimeFilterTests
    {
        [Theory, AutoMoqData]
        public void Filter_ReturnsSingleGameWithPlaytime_When_OneGameHasPlaytime(
            Game[] games,
            ulong playtime,
            WithPlaytimeFilter sut)
        {
            // Arrange
            games.ForEach(game => { game.Playtime = 0; });
            var gameWithPlaytime = games.Last();
            gameWithPlaytime.Playtime = playtime;

            // Act
            var result = sut.Filter(games);

            // Assert
            var single = Assert.Single(result);
            Assert.Equal(gameWithPlaytime, single);
        }
    }
}