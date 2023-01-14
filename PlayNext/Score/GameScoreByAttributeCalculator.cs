using System;
using System.Collections.Generic;
using Playnite.SDK.Models;

namespace PlayNext.Score
{
    public class GameScoreByAttributeCalculator
    {
        public IDictionary<Guid, float> Calculate(IEnumerable<Game> games, Func<Game, IEnumerable<Guid>> attributeSelector, Dictionary<Guid, float> attributeScore)
        {
            var gameScoreForAttribute = new Dictionary<Guid, float>();
            foreach (var game in games)
            {
                var score = 0f;
                foreach (var attributeId in attributeSelector.Invoke(game))
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