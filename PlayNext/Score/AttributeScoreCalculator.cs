using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Score
{
    public class AttributeScoreCalculator
    {
        public Dictionary<Guid, float> CalculateByPlaytime(IEnumerable<Game> games, float weight)
        {
            var maxTime = games.Max(x => x.Playtime);
            var scores = new Dictionary<Guid, float>();

            foreach (var game in games)
            {
                CalculateAttributeScore(game.GenreIds, game.Playtime, maxTime, weight, scores);
                CalculateAttributeScore(game.CategoryIds, game.Playtime, maxTime, weight, scores);
                CalculateAttributeScore(game.DeveloperIds, game.Playtime, maxTime, weight, scores);
                CalculateAttributeScore(game.PublisherIds, game.Playtime, maxTime, weight, scores);
                CalculateAttributeScore(game.TagIds, game.Playtime, maxTime, weight, scores);
            }

            return scores;
        }

        public Dictionary<Guid, float> CalculateByRecent(IEnumerable<Game> games, float weight)
        {
            var orderedGames = games.OrderByDescending(x => x.LastActivity).ToArray();
            var gameCount = (ulong)orderedGames.Count();
            var scores = new Dictionary<Guid, float>();

            for (ulong i = 0; i < gameCount; i++)
            {
                var recentPosition = gameCount - i;
                var game = orderedGames[i];
                CalculateAttributeScore(game.GenreIds, recentPosition, gameCount, weight, scores);
                CalculateAttributeScore(game.CategoryIds, recentPosition, gameCount, weight, scores);
                CalculateAttributeScore(game.DeveloperIds, recentPosition, gameCount, weight, scores);
                CalculateAttributeScore(game.PublisherIds, recentPosition, gameCount, weight, scores);
                CalculateAttributeScore(game.TagIds, recentPosition, gameCount, weight, scores);
            }

            return scores;
        }

        private static void CalculateAttributeScore(List<Guid> attributeIds, ulong valueInGame, ulong maxValue, float weight, Dictionary<Guid, float> scores)
        {
            var genreScore = valueInGame * 100 * weight / attributeIds.Count / maxValue;
            foreach (var genreId in attributeIds)
            {
                if (scores.ContainsKey(genreId))
                {
                    scores[genreId] += genreScore;
                }
                else
                {
                    scores[genreId] = genreScore;
                }
            }
        }
    }
}