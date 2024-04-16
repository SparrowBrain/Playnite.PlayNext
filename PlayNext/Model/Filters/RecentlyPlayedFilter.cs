using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Services;
using Playnite.SDK.Models;

namespace PlayNext.Model.Filters
{
    public class RecentlyPlayedFilter
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public RecentlyPlayedFilter(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IReadOnlyCollection<Game> Filter(IEnumerable<Game> games, int recentDayCount)
        {
            return games.Where(x => x.LastActivity >= _dateTimeProvider.GetNow() - TimeSpan.FromDays(recentDayCount)).ToList();
        }
    }
}