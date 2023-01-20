using System;
using System.Collections.Generic;

namespace PlayNext.Score.GameScore
{
    public class GameScoreByAttributeCalculator
    {
        public IDictionary<Guid, float> Calculate(IEnumerable<Playnite.SDK.Models.Game> games, Func<Playnite.SDK.Models.Game, IEnumerable<Guid>> attributeSelector, Dictionary<Guid, float> attributeScore)
        {
            var gameScoreForAttribute = new Dictionary<Guid, float>();
            foreach (var game in games)
            {
                var score = 0f;
                foreach (var attributeId in attributeSelector.Invoke(game) ?? Array.Empty<Guid>())
                {
                    if (attributeScore.ContainsKey(attributeId))
                    {
                        score += attributeScore[attributeId];
                    }
                }

                gameScoreForAttribute.Add(game.Id, score);
            }

            return gameScoreForAttribute;
        }
    }
}