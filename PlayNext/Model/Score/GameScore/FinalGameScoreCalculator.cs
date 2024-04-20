using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Extensions.HowLongToBeat;
using PlayNext.Model.Data;
using PlayNext.Model.Score.Attribute;
using Playnite.SDK.Models;

namespace PlayNext.Model.Score.GameScore
{
    public class FinalGameScoreCalculator
    {
        private readonly IHowLongToBeatExtension _howLongToBeatExtension;
        private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator;
        private readonly CriticScoreCalculator _criticScoreCalculator;
        private readonly CommunityScoreCalculator _communityScoreCalculator;
        private readonly ReleaseYearCalculator _releaseYearCalculator;
        private readonly GameLengthCalculator _gameLengthCalculator;
        private readonly ScoreNormalizer _scoreNormalizer;
        private readonly Summator _summator;

        public FinalGameScoreCalculator(IHowLongToBeatExtension howLongToBeatExtension,
            GameScoreByAttributeCalculator gameScoreByAttributeCalculator,
            CriticScoreCalculator criticScoreCalculator,
            CommunityScoreCalculator communityScoreCalculator,
            ReleaseYearCalculator releaseYearCalculator,
            GameLengthCalculator gameLengthCalculator,
            ScoreNormalizer scoreNormalizer,
            Summator summator)
        {
            _howLongToBeatExtension = howLongToBeatExtension;
            _gameScoreByAttributeCalculator = gameScoreByAttributeCalculator;
            _criticScoreCalculator = criticScoreCalculator;
            _communityScoreCalculator = communityScoreCalculator;
            _releaseYearCalculator = releaseYearCalculator;
            _gameLengthCalculator = gameLengthCalculator;
            _scoreNormalizer = scoreNormalizer;
            _summator = summator;
        }

        public IDictionary<Guid, float> Calculate(
            IEnumerable<Game> games,
            Dictionary<Guid, float> attributeScore,
            GameScoreWeights gameScoreCalculationWeights,
            int desiredReleaseYear,
            TimeSpan desiredGameLength)
        {
            var gameLengths = _howLongToBeatExtension.GetTimeToPlay();

            var weightedScoreByGenre = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.GenreIds, gameScoreCalculationWeights.Genre);
            var weightedScoreByFeature = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.FeatureIds, gameScoreCalculationWeights.Feature);
            var weightedScoreByDeveloper = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.DeveloperIds, gameScoreCalculationWeights.Developer);
            var weightedScoreByPublisher = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.PublisherIds, gameScoreCalculationWeights.Publisher);
            var weightedScoreByTag = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.TagIds, gameScoreCalculationWeights.Tag);
            var weightedScoreByCriticsScore = _criticScoreCalculator.Calculate(games).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.CriticScore);
            var weightedScoreByCommunityScore = _communityScoreCalculator.Calculate(games).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.CommunityScore);
            var weightedScoreByReleaseYear = _releaseYearCalculator.Calculate(games, desiredReleaseYear).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.ReleaseYear);
            var weightedScoreByGameLength = _gameLengthCalculator.Calculate(gameLengths, desiredGameLength).ToDictionary(x => x.Key, x => x.Value * gameScoreCalculationWeights.GameLength);

            var sum = _summator.AddUp(
                weightedScoreByGenre,
                weightedScoreByFeature,
                weightedScoreByDeveloper,
                weightedScoreByPublisher,
                weightedScoreByTag,
                weightedScoreByCriticsScore,
                weightedScoreByCommunityScore,
                weightedScoreByReleaseYear,
                weightedScoreByGameLength);

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