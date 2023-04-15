using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PlayNext.Model.Data;
using PlayNext.Model.Score;
using PlayNext.Settings;
using Playnite.SDK;

namespace PlayNext.ViewModels
{
    public class PlayNextMainViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;
        private readonly TotalScoreCalculator _totalScoreCalculator;

        private ObservableCollection<GameToPlayViewModel> _games = new ObservableCollection<GameToPlayViewModel>();
        private ShowcaseType _activeShowcaseType;
        private int _numberOfGames = 30;

        public PlayNextMainViewModel(PlayNext plugin)
        {
            _plugin = plugin;
            _totalScoreCalculator = new TotalScoreCalculator(plugin);
        }

        public ObservableCollection<GameToPlayViewModel> Games
        {
            get => _games;
            set
            {
                SetValue(ref _games, value);
                OnPropertyChanged(nameof(TopGames));
            }
        }

        public GameToPlayViewModel[] TopGames => Games.Take(_numberOfGames).ToArray();

        public ShowcaseType ActiveShowcaseType
        {
            get => _activeShowcaseType;
            set => SetValue(ref _activeShowcaseType, value);
        }

        // ReSharper disable once UnusedMember.Global
        public ICommand SwitchToCovers => new RelayCommand(() => { ActiveShowcaseType = ShowcaseType.Covers; });

        // ReSharper disable once UnusedMember.Global
        public ICommand SwitchToList => new RelayCommand(() => { ActiveShowcaseType = ShowcaseType.List; });

        public void LoadData()
        {
            new Task(() =>
            {
                try
                {
                    var savedSettings = _plugin.LoadPluginSettings<PlayNextSettings>();
                    _numberOfGames = savedSettings.NumberOfTopGames;

                    var games = _totalScoreCalculator.Calculate(savedSettings);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Games = new ObservableCollection<GameToPlayViewModel>(games);
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