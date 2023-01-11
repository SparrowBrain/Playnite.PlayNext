using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Score
{
    public class ScoreNormalizer
    {
        public IDictionary<Guid, float> Normalize(Dictionary<Guid, float> scores)
        {
            var max = scores.Max(x => x.Value);
            return scores.ToDictionary(x => x.Key, x => x.Value * 100 / max);
        }
    }
}