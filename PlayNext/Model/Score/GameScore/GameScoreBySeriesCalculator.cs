using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Model.Data;
using Playnite.SDK.Models;

namespace PlayNext.Model.Score.GameScore
{
    public class GameScoreBySeriesCalculator
    {
        public Dictionary<Guid, float> Calculate(
            OrderSeriesBy orderSeriesBy,
            IEnumerable<Game> games,
            Dictionary<Guid, float> attributeScore)
        {
            var gamesWithSeries = games.Where(x => x.SeriesIds != null && x.SeriesIds.Any()).ToList();
            if (!gamesWithSeries.Any())
            {
                return new Dictionary<Guid, float>();
            }

            var gamesWithSeriesScores = GetGamesWithSeriesScores(gamesWithSeries, attributeScore);
            var seriesWithGames = GetSeriesWithGames(gamesWithSeries, gamesWithSeriesScores);
            var gamesWithScores = GetGamesWithSummedUpScores(orderSeriesBy, gamesWithSeriesScores, seriesWithGames);

            var maxScore = gamesWithScores.Max(x => x.Value);
            if (maxScore == 0)
            {
                return new Dictionary<Guid, float>();
            }

            return CalculateGamesWithNormalizedScores(gamesWithScores, maxScore);
        }

        private static Dictionary<Guid, Dictionary<Guid, float>> GetGamesWithSeriesScores(IEnumerable<Game> games, Dictionary<Guid, float> attributeScore)
        {
            return games.ToDictionary(x => x.Id, x => x.SeriesIds.ToDictionary(s => s, s =>
            {
                if (!attributeScore.TryGetValue(s, out var value))
                {
                    return 0;
                }

                return value;
            }));
        }

        private static Dictionary<Guid, IEnumerable<Game>> GetSeriesWithGames(IEnumerable<Game> games, Dictionary<Guid, Dictionary<Guid, float>> gamesWithSeriesScores)
        {
            return gamesWithSeriesScores
                .SelectMany(x => x.Value.Keys.Select(s => new { SeriesId = s, Game = games.First(g => g.Id == x.Key) }))
                .GroupBy(x => x.SeriesId)
                .ToDictionary(x => x.Key, x => x.Select(g => g.Game));
        }

        private static Dictionary<Guid, float> GetGamesWithSummedUpScores(OrderSeriesBy orderSeriesBy, Dictionary<Guid, Dictionary<Guid, float>> gamesWithSeriesScores, Dictionary<Guid, IEnumerable<Game>> seriesWithGames)
        {
            return gamesWithSeriesScores.ToDictionary(x => x.Key, x =>
            {
                var score = 0f;
                foreach (var seriesScore in x.Value)
                {
                    var scoreMultiplier = GetScoreMultiplierByPositionInSeries(x.Key, orderSeriesBy, seriesWithGames, seriesScore);
                    score += seriesScore.Value * scoreMultiplier;
                }

                return score;
            });
        }

        private static Dictionary<Guid, float> CalculateGamesWithNormalizedScores(Dictionary<Guid, float> gamesWithScores, float maxScore)
        {
            return gamesWithScores.ToDictionary(x => x.Key, x => x.Value * 100 / maxScore);
        }

        private static float GetScoreMultiplierByPositionInSeries(
            Guid gameId,
            OrderSeriesBy orderSeriesBy,
            Dictionary<Guid, IEnumerable<Game>> seriesWithGames,
            KeyValuePair<Guid, float> seriesScore)
        {
            var seriesGames = seriesWithGames[seriesScore.Key].ToList();
            seriesGames.Sort((a, b) =>
            {
                if (orderSeriesBy == OrderSeriesBy.ReleaseDate)
                {
                    if (a.ReleaseDate != null && b.ReleaseDate != null)
                    {
                        return a.ReleaseDate.Value.Date.CompareTo(b.ReleaseDate.Value.Date);
                    }

                    if (a.ReleaseDate != null)
                    {
                        return -1;
                    }

                    if (b.ReleaseDate != null)
                    {
                        return 1;
                    }
                }

                if (orderSeriesBy == OrderSeriesBy.SortingName)
                {
                    return string.Compare(a.SortingName, b.SortingName, StringComparison.CurrentCulture);
                }

                throw new NotSupportedException("Not supported series calculation method");
            });

            var seriesGameIds = seriesGames.Select(g => g.Id).ToList();
            var gameCountInSeries = seriesGameIds.Count;
            var scoreMultiplier = ((float)gameCountInSeries - seriesGameIds.IndexOf(gameId)) / gameCountInSeries;
            return scoreMultiplier;
        }
    }
}