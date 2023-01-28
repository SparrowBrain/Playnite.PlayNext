using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.Score.Attribute
{
    public class ReleaseYearCalculator
    {
        private ILogger _logger = LogManager.GetLogger();

        public Dictionary<Guid, float> Calculate(IEnumerable<Game> games, int desiredReleaseYear)
        {
            var maxYearDifference = games.Max(x => Math.Abs(desiredReleaseYear - (x.ReleaseYear ?? desiredReleaseYear)));
            if (maxYearDifference == 0)
            {
                return games.Where(x => x.ReleaseYear.HasValue).ToDictionary(x => x.Id, x => 100f);
            }

            var score = new Dictionary<Guid, float>();
            foreach (var game in games.Where(x => x.ReleaseYear.HasValue))
            {
                score[game.Id] = (maxYearDifference - Math.Abs(desiredReleaseYear - game.ReleaseYear.Value)) * 100 / (float)maxYearDifference;
            }

            return score;
        }
    }
}