using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlayNext.Model.Score;
using PlayNext.ViewModels;
using Playnite.SDK;

namespace PlayNext.StartPage
{
    public class StartPagePlayNextViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;
        private readonly TotalScoreCalculator _totalScoreCalculator;

        private ObservableCollection<GameToPlayViewModel> _games = new ObservableCollection<GameToPlayViewModel>();

        public StartPagePlayNextViewModel(PlayNext plugin)
        {
            _plugin = plugin;
            _totalScoreCalculator = new TotalScoreCalculator(plugin);
        }

        public void LoadData()
        {
            new Task(() =>
            {
                try
                {
                    var savedSettings = _plugin.LoadPluginSettings<PlayNextSettings>();
                    var numberOfGames = savedSettings.NumberOfTopGames;
                    var games = _totalScoreCalculator.Calculate(savedSettings);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Games = new ObservableCollection<GameToPlayViewModel>(games.Take(numberOfGames));
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
    }
}