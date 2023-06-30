using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.Attribute
{
    public class GameLengthCalculator
    {
        public Dictionary<Guid, float> Calculate(Dictionary<Guid, int> games, TimeSpan length)
        {
            if (games == null || !games.Any())
            {
                return new Dictionary<Guid, float>();
            }

            var preferredLengthInSeconds = (int)length.TotalSeconds;
            var maxDifference = games.Max(x => Math.Abs(x.Value - preferredLengthInSeconds));
            if (maxDifference == 0)
            {
                return games.ToDictionary(x => x.Key, x => 100f);
            }

            return games.ToDictionary(x => x.Key, x => (maxDifference - Math.Abs(x.Value - preferredLengthInSeconds)) * 100f / maxDifference);
        }
    }
}