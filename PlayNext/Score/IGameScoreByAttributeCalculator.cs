using System;
using System.Collections.Generic;
using Playnite.SDK.Models;

namespace PlayNext.Score
{
    public interface IGameScoreByAttributeCalculator
    {
        IDictionary<Guid, float> Calculate(IEnumerable<Game> games, Func<Game, IEnumerable<Guid>> attributeSelector, Dictionary<Guid, float> attributeScore);
    }
}