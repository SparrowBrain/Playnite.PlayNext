using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Model.Data;
using PlayNext.Settings;
using Playnite.SDK.Models;

namespace PlayNext.Model.Filters
{
    public class UnplayedFilter
    {
        public IEnumerable<Game> Filter(IEnumerable<Game> games, PlayNextSettings settings)
        {
            switch (settings.UnplayedGameDefinition)
            {
                case UnplayedGameDefinition.ZeroPlaytime:
                    return games.Where(x => !x.Hidden && x.Playtime == 0);

                case UnplayedGameDefinition.SelectedCompletionStatuses:
                    return games.Where(game => !game.Hidden && settings.UnplayedCompletionStatuses.Any(x => x == game.CompletionStatusId));

                default:
                    throw new Exception("Unknown unplayed game definition when filtering for unplayed games");
            }
        }
    }
}