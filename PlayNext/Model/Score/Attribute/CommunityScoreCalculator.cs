using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Model.Score.Attribute
{
    public class CommunityScoreCalculator
    {
        public Dictionary<Guid, float> Calculate(IEnumerable<Game> games)
        {
            return games.ToDictionary(x => x.Id, x => x.CommunityScore ?? 0f);
        }
    }
}