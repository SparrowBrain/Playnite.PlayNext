using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlayNext.Filters;
using PlayNext.Models;
using PlayNext.Score;
using PlayNext.Settings;
using Playnite.SDK;

namespace PlayNext.ViewModels
{
    public class PlayNextMainViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;
        private ObservableCollection<GameToPlay> _games = new ObservableCollection<GameToPlay>();

        public PlayNextMainViewModel(PlayNext plugin)
        {
            _plugin = plugin;

            // Load saved settings.
            //var savedSettings = plugin.LoadPluginSettings<PlayNextSettings>();
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
                    var allGames = _plugin.PlayniteApi.Database.Games.ToArray();
                    var filteredGames = new WithPlaytimeFilter().Filter(allGames);
                    var unPlayedGames = allGames.Where(x => x.Playtime == 0 && !x.Hidden).ToArray();

                    var attributeScore = new AttributeScoreCalculator().CalculateByPlaytime(filteredGames, 1);
                    var gameScore = new GameScoreCalculator(new GameScoreByAttributeCalculator(), new ScoreNormalizer(), new Summator()).Calculate(unPlayedGames, attributeScore, GameScoreCalculationWeights.Flat);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Games = new ObservableCollection<GameToPlay>(gameScore.Select(score =>
                        {
                            var game = unPlayedGames.First(x => x.Id == score.Key);
                            return new GameToPlay(game, score.Value);
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