using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.Attribute
{
	public class AttributeScoreCalculator
	{
		public Dictionary<Guid, float> CalculateByPlaytime(IEnumerable<Game> games, float weight)
		{
			var scores = new Dictionary<Guid, float>();
			if (!games.Any() || weight == 0)
			{
				return scores;
			}

			var maxTime = games.Max(x => x.Playtime);

			foreach (var game in games)
			{
				CalculateAllAttributeScores(game, game.Playtime, maxTime, weight, scores);
			}

			return scores;
		}

		public Dictionary<Guid, float> CalculateByRecentOrder(IEnumerable<Game> games, float weight)
		{
			var scores = new Dictionary<Guid, float>();
			if (!games.Any() || weight == 0)
			{
				return scores;
			}

			var orderedGames = games.OrderByDescending(x => x.LastActivity).ToArray();
			var gameCount = (ulong)orderedGames.Count();

			for (ulong i = 0; i < gameCount; i++)
			{
				var recentPosition = gameCount - i;
				var game = orderedGames[i];

				CalculateAllAttributeScores(game, recentPosition, gameCount, weight, scores);
			}

			return scores;
		}

		public Dictionary<Guid, float> CalculateByFavourite(IReadOnlyCollection<Game> games, float weight)
		{
			var score = new Dictionary<Guid, float>();
			if (!games.Any() || weight == 0)
			{
				return score;
			}

			foreach (var game in games)
			{
				if (game.Favorite)
				{
					CalculateAllAttributeScores(game, 1, 1, weight, score);
				}
			}

			return score;
		}

		public Dictionary<Guid, float> CalculateByUserScore(IReadOnlyCollection<Game> games, int averageScore, float weight)
		{
			var score = new Dictionary<Guid, float>();
			if (!games.Any() || weight == 0)
			{
				return score;
			}

			var attributeCounts = new Dictionary<Guid, int>();
			foreach (var game in games)
			{
				CountAttributes(game.GenreIds, attributeCounts);
				CountAttributes(game.CategoryIds, attributeCounts);
				CountAttributes(game.DeveloperIds, attributeCounts);
				CountAttributes(game.PublisherIds, attributeCounts);
				CountAttributes(game.TagIds, attributeCounts);
				CountAttributes(game.SeriesIds, attributeCounts);
			}

			foreach (var game in games)
			{
				var userScore = game.UserScore ?? averageScore;
				var adjustedUserScore = userScore == 0
					? 0
					: userScore <= averageScore
						? (ulong)(userScore * 50.0 / averageScore)
						: (ulong)(50 + (userScore - averageScore) * 50.0 / (100 - averageScore));
				CalculateAllAttributeScores(game, adjustedUserScore, 100, weight, score);
			}

			score = score.ToDictionary(x => x.Key, x => x.Value / attributeCounts[x.Key]);
			return score;
		}

		private static void CalculateAllAttributeScores(Game game, ulong valueInGame, ulong maxValue, float weight, Dictionary<Guid, float> scores)
		{
			CalculateAttributeScore(game.GenreIds, valueInGame, maxValue, weight, scores);
			CalculateAttributeScore(game.CategoryIds, valueInGame, maxValue, weight, scores);
			CalculateAttributeScore(game.DeveloperIds, valueInGame, maxValue, weight, scores);
			CalculateAttributeScore(game.PublisherIds, valueInGame, maxValue, weight, scores);
			CalculateAttributeScore(game.TagIds, valueInGame, maxValue, weight, scores);
			CalculateAttributeScore(game.SeriesIds, valueInGame, maxValue, weight, scores);
		}

		private static void CalculateAttributeScore(List<Guid> attributeIds, ulong valueInGame, ulong maxValue, float weight, Dictionary<Guid, float> scores)
		{
			if (attributeIds == null)
			{
				return;
			}

			var attributeScore = maxValue == 0
				? 0
				: valueInGame * 100 * weight / attributeIds.Count / maxValue;

			foreach (var attributeId in attributeIds)
			{
				if (scores.ContainsKey(attributeId))
				{
					scores[attributeId] += attributeScore;
				}
				else
				{
					scores[attributeId] = attributeScore;
				}
			}
		}

		private static void CountAttributes(List<Guid> attributeIds, Dictionary<Guid, int> attributeCounts)
		{
			if (attributeIds == null)
			{
				return;
			}

			foreach (var attributeId in attributeIds)
			{
				if (attributeCounts.ContainsKey(attributeId))
				{
					attributeCounts[attributeId] += 1;
				}
				else
				{
					attributeCounts[attributeId] = 1;
				}
			}
		}
	}
}