using PlayNext.Extensions.HowLongToBeat;
using PlayNext.Model.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.GameScore
{
	public class FinalGameScoreCalculator
	{
		private readonly IHowLongToBeatExtension _howLongToBeatExtension;
		private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator;
		private readonly GameScoreBySeriesCalculator _gameScoreBySeriesCalculator;
		private readonly CriticScoreCalculator _criticScoreCalculator;
		private readonly CommunityScoreCalculator _communityScoreCalculator;
		private readonly ReleaseYearCalculator _releaseYearCalculator;
		private readonly GameLengthCalculator _gameLengthCalculator;
		private readonly ScoreNormalizer _scoreNormalizer;
		private readonly Summator _summator;

		public FinalGameScoreCalculator(IHowLongToBeatExtension howLongToBeatExtension,
			GameScoreByAttributeCalculator gameScoreByAttributeCalculator,
			GameScoreBySeriesCalculator gameScoreBySeriesCalculator,
			CriticScoreCalculator criticScoreCalculator,
			CommunityScoreCalculator communityScoreCalculator,
			ReleaseYearCalculator releaseYearCalculator,
			GameLengthCalculator gameLengthCalculator,
			ScoreNormalizer scoreNormalizer,
			Summator summator)
		{
			_howLongToBeatExtension = howLongToBeatExtension;
			_gameScoreByAttributeCalculator = gameScoreByAttributeCalculator;
			_gameScoreBySeriesCalculator = gameScoreBySeriesCalculator;
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
			OrderSeriesBy orderSeriesBy,
			int desiredReleaseYear,
			TimeSpan desiredGameLength)
		{
			var gameLengths = _howLongToBeatExtension.GetTimeToPlay()
				.Where(x => games.Any(g => g.Id == x.Key))
				.ToDictionary(x => x.Key, x => x.Value);

			var weightedScoreByGenre = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.GenreIds, gameScoreCalculationWeights.Genre);
			var weightedScoreByFeature = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.FeatureIds, gameScoreCalculationWeights.Feature);
			var weightedScoreByDeveloper = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.DeveloperIds, gameScoreCalculationWeights.Developer);
			var weightedScoreByPublisher = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.PublisherIds, gameScoreCalculationWeights.Publisher);
			var weightedScoreByTag = CalculateWeightedGameScoreByAttribute(games, attributeScore, x => x.TagIds, gameScoreCalculationWeights.Tag);
			var weightedScoreBySeries = CalculateWeightedScores(() => _gameScoreBySeriesCalculator.Calculate(orderSeriesBy, games, attributeScore), gameScoreCalculationWeights.Series);
			var weightedScoreByCriticsScore = CalculateWeightedScores(() => _criticScoreCalculator.Calculate(games), gameScoreCalculationWeights.CriticScore);
			var weightedScoreByCommunityScore = CalculateWeightedScores(() => _communityScoreCalculator.Calculate(games), gameScoreCalculationWeights.CommunityScore);
			var weightedScoreByReleaseYear = CalculateWeightedScores(() => _releaseYearCalculator.Calculate(games, desiredReleaseYear), gameScoreCalculationWeights.ReleaseYear);
			var weightedScoreByGameLength = CalculateWeightedScores(() => _gameLengthCalculator.Calculate(gameLengths, desiredGameLength), gameScoreCalculationWeights.GameLength);

			var sum = _summator.AddUp(
				weightedScoreByGenre,
				weightedScoreByFeature,
				weightedScoreByDeveloper,
				weightedScoreByPublisher,
				weightedScoreByTag,
				weightedScoreBySeries,
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
			if (weight == 0)
			{
				return new Dictionary<Guid, float>();
			}

			var scoreByAttribute = _gameScoreByAttributeCalculator.Calculate(games, attributeSelector, attributeScore);
			var normalizedScoreByAttribute = _scoreNormalizer.Normalize(scoreByAttribute);
			var weightedGameScoreByAttribute = normalizedScoreByAttribute.ToDictionary(x => x.Key, x => x.Value * weight);
			return weightedGameScoreByAttribute;
		}

		private Dictionary<Guid, float> CalculateWeightedScores(
			Func<Dictionary<Guid, float>> scoreCalculationFunction,
			float weight)
		{
			if (weight == 0)
			{
				return new Dictionary<Guid, float>();
			}

			return scoreCalculationFunction.Invoke().ToDictionary(x => x.Key, x => x.Value * weight);
		}
	}
}