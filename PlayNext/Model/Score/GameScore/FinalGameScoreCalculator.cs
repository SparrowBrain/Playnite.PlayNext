using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Model.Data;
using PlayNext.Model.Score.Attribute;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.Model.Score.GameScore
{
    public class FinalGameScoreCalculator
    {
        private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator;
        private readonly CriticScoreCalculator _criticScoreCalculator;
        private readonly CommunityScoreCalculator _communityScoreCalculator;
        private readonly ReleaseYearCalculator _releaseYearCalculator;
        private readonly ScoreNormalizer _scoreNormalizer;
        private readonly Summator _summator;

        private ILogger _logger = LogManager.GetLogger();

        public FinalGameScoreCalculator(
            GameScoreByAttributeCalculator gameScoreByAttributeCalculator,
            CriticScoreCalculator criticScoreCalculator,
            CommunityScoreCalculator communityScoreCalculator,
            ReleaseYearCalculator releaseYearCalculator,
            ScoreNormalizer scoreNormalizer,
            Summator summator)
        {
            _gameScoreByAttributeCalculator = gameScoreByAttributeCalculator;
            _criticScoreCalculator = criticScoreCalculator;
            _communityScoreCalculator = communityScoreCalculator;
            _releaseYearCalculator = releaseYearCalculator;
            _scoreNormalizer = scoreNormalizer;
            _summator = summator;
        }

        public IDictionary<Guid, float> Calculate(IEnumerable<Game> games, Dictionary<Guid, float> attributeScore, GameScoreWeights gameScoreCalculationWeights, int desiredReleaseYear)
        {
            var weightedScoreByGenre = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.GenreIds, gameScoreCalculationWeights.Genre);
            var weightedScoreByFeature = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.FeatureIds, gameScoreCalculationWeights.Feature);
            var weightedScoreByDeveloper = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.DeveloperIds, gameScoreCalculationWeights.Developer);
            var weightedScoreByPublisher = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.PublisherIds, gameScoreCalculationWeights.Publisher);
            var weightedScoreByTag = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.TagIds, gameScoreCalculationWeights.Tag);
            var weightedScoreByCriticsScore = _criticScoreCalculator.Calculate(games).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.CriticScore);
            var weightedScoreByCommunityScore = _communityScoreCalculator.Calculate(games).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.CommunityScore);
            var weightedScoreByReleaseYear = _releaseYearCalculator.Calculate(games, desiredReleaseYear).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.ReleaseYear);

            var sum = _summator.AddUp(
                weightedScoreByGenre,
                weightedScoreByFeature,
                weightedScoreByDeveloper,
                weightedScoreByPublisher,
                weightedScoreByTag,
                weightedScoreByCriticsScore,
                weightedScoreByCommunityScore,
                weightedScoreByReleaseYear);

            var normalizedSum = _scoreNormalizer.Normalize(sum);
            var ordered = normalizedSum.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return ordered;
        }

        private Dictionary<Guid, float> CalculateWeightedGameScoreByAttribute(
            IEnumerable<Game> games,
            Dictionary<Guid, float> attributeScore,
            Func<Game, IEnumerable<Guid>> attributeSelector,
            float weight)
        {
            var scoreByAttribute = _gameScoreByAttributeCalculator.Calculate(games, attributeSelector, attributeScore);
            var normalizedScoreByAttribute = _scoreNormalizer.Normalize(scoreByAttribute);
            var weightedGameScoreByAttribute = normalizedScoreByAttribute.ToDictionary(x => x.Key, x => x.Value * weight);
            return weightedGameScoreByAttribute;
        }
    }
}