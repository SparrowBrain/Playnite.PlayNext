using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Score.Attribute
{
    public class ReleaseYearCalculator
    {
        public Dictionary<Guid, float> Calculate(IEnumerable<Game> games, int desiredReleaseYear)
        {
            var maxYearDifference = games.Max(x => Math.Abs((int)(desiredReleaseYear - x.ReleaseYear ?? desiredReleaseYear)));
            var score = new Dictionary<Guid, float>();
            foreach (var game in games)
            {
                if (game.ReleaseYear.HasValue)
                {
                    score[game.Id] = (maxYearDifference - Math.Abs(desiredReleaseYear - game.ReleaseYear.Value)) * 100 / (float)maxYearDifference;
                }
            }
            return score;
        }
    }
}