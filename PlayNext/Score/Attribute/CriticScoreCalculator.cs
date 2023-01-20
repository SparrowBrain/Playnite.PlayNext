using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Score.Attribute
{
    public class CriticScoreCalculator
    {
        public Dictionary<Guid, float> Calculate(IEnumerable<Game> games)
        {
            return games.ToDictionary(x => x.Id, x => x.CriticScore ?? 0f);
        }
    }
}