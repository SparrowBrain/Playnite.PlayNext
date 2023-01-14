using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Settings;
using Playnite.SDK.Models;

namespace PlayNext.Score
{
    public class GameScoreCalculator
    {
        private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator;
        private readonly ScoreNormalizer _scoreNormalizer;
        private readonly Summator _summator;

        public GameScoreCalculator(
            GameScoreByAttributeCalculator gameScoreByAttributeCalculator,
            ScoreNormalizer scoreNormalizer,
            Summator summator)
        {
            _gameScoreByAttributeCalculator = gameScoreByAttributeCalculator;
            _scoreNormalizer = scoreNormalizer;
            _summator = summator;
        }

        public IDictionary<Guid, float> Calculate(IEnumerable<Game> games, Dictionary<Guid, float> attributeScore, GameScoreCalculationWeights gameScoreCalculationWeights)
        {
            var weightedScoreByGenre = CalculateWeightedGameScoreByAttribute(games, attributeScore, gameScoreCalculationWeights, x => x.GenreIds);
            var weightedScoreByCategory = CalculateWeightedGameScoreByAttribute(games, attributeScore, gameScoreCalculationWeights, x => x.CategoryIds);
            var weightedScoreByDeveloper = CalculateWeightedGameScoreByAttribute(games, attributeScore, gameScoreCalculationWeights, x => x.DeveloperIds);
            var weightedScoreByPublisher = CalculateWeightedGameScoreByAttribute(games, attributeScore, gameScoreCalculationWeights, x => x.PublisherIds);
            var weightedScoreByTag = CalculateWeightedGameScoreByAttribute(games, attributeScore, gameScoreCalculationWeights, x => x.TagIds);

            var sum = _summator.AddUp(
                weightedScoreByGenre,
                weightedScoreByCategory,
                weightedScoreByDeveloper,
                weightedScoreByPublisher,
                weightedScoreByTag);

            var normalizedSum = _scoreNormalizer.Normalize(sum);
            var ordered = normalizedSum.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return ordered;
        }

        private Dictionary<Guid, float> CalculateWeightedGameScoreByAttribute(IEnumerable<Game> games, Dictionary<Guid, float> attributeScore,
            GameScoreCalculationWeights gameScoreCalculationWeights, Func<Game, IEnumerable<Guid>> attributeSelector)
        {
            var gameScoreByGenre = _gameScoreByAttributeCalculator.Calculate(games, attributeSelector, attributeScore);
            var normalizedGameScoresByGenre = _scoreNormalizer.Normalize(gameScoreByGenre);
            var weightedGameScoreByGenre =
                normalizedGameScoresByGenre.ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.Genre);
            return weightedGameScoreByGenre;
        }
    }
}