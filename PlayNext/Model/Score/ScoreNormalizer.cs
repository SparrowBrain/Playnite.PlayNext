using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score
{
    public class ScoreNormalizer
    {
        public IDictionary<Guid, float> Normalize(IDictionary<Guid, float> scores)
        {
            var max = scores.Max(x => x.Value);
            if (max == 0)
            {
                return scores;
            }

            return scores.ToDictionary(x => x.Key, x => x.Value * 100 / max);
        }
    }
}