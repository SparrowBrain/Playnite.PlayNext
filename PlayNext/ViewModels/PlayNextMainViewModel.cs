using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlayNext.Filters;
using PlayNext.Models;
using PlayNext.Score;
using PlayNext.Score.Attribute;
using PlayNext.Score.GameScore;
using PlayNext.Services;
using Playnite.SDK;

namespace PlayNext.ViewModels
{
    public class PlayNextMainViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;
        private ObservableCollection<GameToPlay> _games = new ObservableCollection<GameToPlay>();

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

        public PlayNextMainViewModel(PlayNext plugin)
        {
            _plugin = plugin;

            // Load saved settings.

            _finalGameScoreCalculator = new FinalGameScoreCalculator(_gameScoreByAttributeCalculator, _criticScoreCalculator, _communityScoreCalculator, _releaseYearCalculator, _scoreNormalizer, _summator);
            _finalAttributeScoreCalculator = new FinalAttributeScoreCalculator(_attributeScoreCalculator, _summator);
        }

        public ObservableCollection<GameToPlay> Games
        {
            get => _games;
            set
            {
                if (Equals(value, _games)) return;
                _games = value;
                OnPropertyChanged();
            }
        }

        public void LoadData()
        {
            new Task(() =>
            {
                try
                {
                    var savedSettings = _plugin.LoadPluginSettings<PlayNextSettings>();

                    var attributeCalculationWeights = new AttributeCalculationWeights()
                    {
                        TotalPlaytime = savedSettings.TotalPlaytimeSerialized / 100,
                        RecentPlaytime = savedSettings.RecentPlaytimeSerialized / 100,
                        RecentOrder = savedSettings.RecentOrderSerialized / 100,
                    };

                    var gameScoreCalculationWeights = new GameScoreWeights()
                    {
                        Genre = savedSettings.GenreSerialized / 100,
                        Feature = savedSettings.FeatureSerialized / 100,
                        Developer = savedSettings.DeveloperSerialized / 100,
                        Publisher = savedSettings.PublisherSerialized / 100,
                        Tag = savedSettings.TagSerialized / 100,
                        CriticScore = savedSettings.CriticScoreSerialized / 100,
                        CommunityScore = savedSettings.CommunityScoreSerialized / 100,
                        ReleaseYear = savedSettings.ReleaseYearSerialized / 100,
                    };

                    var allGames = _plugin.PlayniteApi.Database.Games.ToArray();
                    var playedGames = new WithPlaytimeFilter().Filter(allGames);
                    var recentGames = new RecentlyPlayedFilter(_dateTimeProvider).Filter(playedGames, 14);
                    var unPlayedGames = allGames.Where(x => x.Playtime == 0 && !x.Hidden).ToArray();

                    var attributeScore = _finalAttributeScoreCalculator.Calculate(playedGames, recentGames, attributeCalculationWeights);

                    var gameScore = _finalGameScoreCalculator.Calculate(unPlayedGames, attributeScore, gameScoreCalculationWeights);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Games = new ObservableCollection<GameToPlay>(gameScore.Select(score =>
                        {
                            var game = unPlayedGames.First(x => x.Id == score.Key);

                            var gameToPlay = new GameToPlay(game, score.Value);
                            Task.Run(() =>
                            {
                                var coverImage = game.CoverImage == null
                                    ? null
                                    : _plugin.PlayniteApi.Database.GetFullFilePath(game.CoverImage);
                                gameToPlay.CoverImage = coverImage;
                            });
                            return gameToPlay;
                        }).ToArray());
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while trying to calculate game scores.");
                }
            }).Start();
        }
    }
}