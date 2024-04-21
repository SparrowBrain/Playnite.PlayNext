using System;
using System.Collections.Generic;
using System.Linq;
using PlayNext.Infrastructure.Services;
using PlayNext.Model.Data;
using PlayNext.Model.Filters;
using PlayNext.Model.Score.Attribute;
using PlayNext.Model.Score.GameScore;
using PlayNext.Settings;
using PlayNext.ViewModels;
using Playnite.SDK;

namespace PlayNext.Model.Score
{
    internal class TotalScoreCalculator
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;

        private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator = new GameScoreByAttributeCalculator();
        private readonly ScoreNormalizer _scoreNormalizer = new ScoreNormalizer();
        private readonly Summator _summator = new Summator();
        private readonly FinalGameScoreCalculator _finalGameScoreCalculator;
        private readonly AttributeScoreCalculator _attributeScoreCalculator = new AttributeScoreCalculator();
        private readonly GameScoreBySeriesCalculator _gameScoreBySeriesCalculator = new GameScoreBySeriesCalculator();
        private readonly DateTimeProvider _dateTimeProvider = new DateTimeProvider();
        private readonly FinalAttributeScoreCalculator _finalAttributeScoreCalculator;
        private readonly CriticScoreCalculator _criticScoreCalculator = new CriticScoreCalculator();
        private readonly CommunityScoreCalculator _communityScoreCalculator = new CommunityScoreCalculator();
        private readonly ReleaseYearCalculator _releaseYearCalculator = new ReleaseYearCalculator();
        private readonly GameLengthCalculator _gameLengthCalculator = new GameLengthCalculator();

        public TotalScoreCalculator(PlayNext plugin)
        {
            _plugin = plugin;

            _finalAttributeScoreCalculator = new FinalAttributeScoreCalculator(_attributeScoreCalculator, _summator);
            _finalGameScoreCalculator = new FinalGameScoreCalculator(plugin.HowLongToBeatExtension, _gameScoreByAttributeCalculator, _gameScoreBySeriesCalculator, _criticScoreCalculator, _communityScoreCalculator, _releaseYearCalculator, _gameLengthCalculator, _scoreNormalizer, _summator);
        }

        public ICollection<GameToPlayViewModel> Calculate(PlayNextSettings savedSettings)
        {
            var recentDayCount = savedSettings.RecentDays;

            var attributeCalculationWeights = new AttributeCalculationWeights()
            {
                TotalPlaytime = savedSettings.TotalPlaytimeWeight / PlayNextSettings.MaxWeightValue,
                RecentPlaytime = savedSettings.RecentPlaytimeWeight / PlayNextSettings.MaxWeightValue,
                RecentOrder = savedSettings.RecentOrderWeight / PlayNextSettings.MaxWeightValue,
            };

            var gameScoreCalculationWeights = new GameScoreWeights()
            {
                Genre = savedSettings.GenreWeight / PlayNextSettings.MaxWeightValue,
                Feature = savedSettings.FeatureWeight / PlayNextSettings.MaxWeightValue,
                Developer = savedSettings.DeveloperWeight / PlayNextSettings.MaxWeightValue,
                Publisher = savedSettings.PublisherWeight / PlayNextSettings.MaxWeightValue,
                Tag = savedSettings.TagWeight / PlayNextSettings.MaxWeightValue,
                Series = savedSettings.SeriesWeight / PlayNextSettings.MaxWeightValue,
                CriticScore = savedSettings.CriticScoreWeight / PlayNextSettings.MaxWeightValue,
                CommunityScore = savedSettings.CommunityScoreWeight / PlayNextSettings.MaxWeightValue,
                ReleaseYear = savedSettings.ReleaseYearWeight / PlayNextSettings.MaxWeightValue,
                GameLength = savedSettings.GameLengthWeight / PlayNextSettings.MaxWeightValue,
            };

            var desiredReleaseYear = GetDesiredReleaseYear(savedSettings);
            var desiredGameLength = GetDesiredGameLength(savedSettings);

            var allGames = _plugin.PlayniteApi.Database.Games.ToArray();
            var playedGames = new WithPlaytimeFilter().Filter(allGames);
            var recentGames = new RecentlyPlayedFilter(_dateTimeProvider).Filter(playedGames, recentDayCount);
            var unPlayedGames = new UnplayedFilter().Filter(allGames, savedSettings).ToArray();

            var gamesWithRecentPlaytime = _plugin.GameActivityExtension.GetRecentPlaytime(recentGames, savedSettings);
            var attributeScore = _finalAttributeScoreCalculator.Calculate(playedGames, gamesWithRecentPlaytime, recentGames, attributeCalculationWeights);
            var gameScore = _finalGameScoreCalculator.Calculate(unPlayedGames, attributeScore, gameScoreCalculationWeights, savedSettings.OrderSeriesBy, desiredReleaseYear, desiredGameLength);

            return gameScore.Select(score =>
            {
                var game = unPlayedGames.First(x => x.Id == score.Key);
                return new GameToPlayViewModel(_plugin, game, score.Value);
            }).ToArray();
        }

        private int GetDesiredReleaseYear(PlayNextSettings savedSettings)
        {
            switch (savedSettings.ReleaseYearChoice)
            {
                case ReleaseYearChoice.Current:
                    return DateTime.Now.Year;

                case ReleaseYearChoice.Specific:
                    return savedSettings.DesiredReleaseYear;

                default:
                    _logger.Error("Unknown release year choice");
                    return DateTime.Now.Year;
            }
        }

        private static TimeSpan GetDesiredGameLength(PlayNextSettings savedSettings)
        {
            return TimeSpan.FromHours(savedSettings.GameLengthHours);
        }
    }
}