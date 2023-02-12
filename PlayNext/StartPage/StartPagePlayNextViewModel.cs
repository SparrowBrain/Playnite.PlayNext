using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlayNext.Model.Data;
using PlayNext.Model.Filters;
using PlayNext.Model.Score;
using PlayNext.Model.Score.Attribute;
using PlayNext.Model.Score.GameScore;
using PlayNext.Services;
using PlayNext.ViewModels;
using Playnite.SDK;

namespace PlayNext.StartPage
{
    public class StartPagePlayNextViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;

        private ObservableCollection<GameToPlayViewModel> _games = new ObservableCollection<GameToPlayViewModel>();

        private readonly GameScoreByAttributeCalculator _gameScoreByAttributeCalculator = new GameScoreByAttributeCalculator();
        private readonly ScoreNormalizer _scoreNormalizer = new ScoreNormalizer();
        private readonly Summator _summator = new Summator();
        private readonly FinalGameScoreCalculator _finalGameScoreCalculator;
        private readonly AttributeScoreCalculator _attributeScoreCalculator = new AttributeScoreCalculator();
        private readonly DateTimeProvider _dateTimeProvider = new DateTimeProvider();
        private readonly FinalAttributeScoreCalculator _finalAttributeScoreCalculator;
        private readonly CriticScoreCalculator _criticScoreCalculator = new CriticScoreCalculator();
        private readonly CommunityScoreCalculator _communityScoreCalculator = new CommunityScoreCalculator();
        private readonly ReleaseYearCalculator _releaseYearCalculator = new ReleaseYearCalculator();

        public StartPagePlayNextViewModel(PlayNext plugin)
        {
            _plugin = plugin;
            _finalGameScoreCalculator = new FinalGameScoreCalculator(_gameScoreByAttributeCalculator, _criticScoreCalculator, _communityScoreCalculator, _releaseYearCalculator, _scoreNormalizer, _summator);
            _finalAttributeScoreCalculator = new FinalAttributeScoreCalculator(_attributeScoreCalculator, _summator);
        }

        public void LoadData()
        {
            new Task(() =>
            {
                try
                {
                    var savedSettings = _plugin.LoadPluginSettings<PlayNextSettings>();
                    var numberOfGames = savedSettings.NumberOfTopGames;
                    var recentDayCount = savedSettings.RecentDays;

                    var attributeCalculationWeights = new AttributeCalculationWeights()
                    {
                        TotalPlaytime = savedSettings.TotalPlaytimeSerialized / PlayNextSettings.MaxWeightValue,
                        RecentPlaytime = savedSettings.RecentPlaytimeSerialized / PlayNextSettings.MaxWeightValue,
                        RecentOrder = savedSettings.RecentOrderSerialized / PlayNextSettings.MaxWeightValue,
                    };

                    var gameScoreCalculationWeights = new GameScoreWeights()
                    {
                        Genre = savedSettings.GenreSerialized / PlayNextSettings.MaxWeightValue,
                        Feature = savedSettings.FeatureSerialized / PlayNextSettings.MaxWeightValue,
                        Developer = savedSettings.DeveloperSerialized / PlayNextSettings.MaxWeightValue,
                        Publisher = savedSettings.PublisherSerialized / PlayNextSettings.MaxWeightValue,
                        Tag = savedSettings.TagSerialized / PlayNextSettings.MaxWeightValue,
                        CriticScore = savedSettings.CriticScoreSerialized / PlayNextSettings.MaxWeightValue,
                        CommunityScore = savedSettings.CommunityScoreSerialized / PlayNextSettings.MaxWeightValue,
                        ReleaseYear = savedSettings.ReleaseYearSerialized / PlayNextSettings.MaxWeightValue,
                    };

                    var desiredReleaseYear = GetDesiredReleaseYear(savedSettings);

                    var allGames = _plugin.PlayniteApi.Database.Games.ToArray();
                    var playedGames = new WithPlaytimeFilter().Filter(allGames);

                    var recentGames = new RecentlyPlayedFilter(_dateTimeProvider).Filter(playedGames, recentDayCount);
                    var unPlayedGames = allGames.Where(x => x.Playtime == 0 && !x.Hidden).ToArray();

                    var attributeScore = _finalAttributeScoreCalculator.Calculate(playedGames, recentGames, attributeCalculationWeights);
                    var gameScore = _finalGameScoreCalculator.Calculate(unPlayedGames, attributeScore, gameScoreCalculationWeights, desiredReleaseYear);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Games = new ObservableCollection<GameToPlayViewModel>(gameScore.Select(score =>
                        {
                            var game = unPlayedGames.First(x => x.Id == score.Key);
                            return new GameToPlayViewModel(_plugin, game, score.Value);
                        }).Take(numberOfGames).ToArray());
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to calculate game scores.");
                }
            }).Start();
        }

        public ObservableCollection<GameToPlayViewModel> Games
        {
            get => _games;
            set => SetValue(ref _games, value);
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
    }
}