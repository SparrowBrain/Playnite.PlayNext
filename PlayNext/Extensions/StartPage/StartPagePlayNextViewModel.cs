using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlayNext.Settings;
using PlayNext.ViewModels;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.Extensions.StartPage
{
    public class StartPagePlayNextViewModel : ObservableObject
    {
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly PlayNext _plugin;

        private ObservableCollection<GameToPlayViewModel> _games = new ObservableCollection<GameToPlayViewModel>();
        private bool _showVerticalLabel;
        private bool _showHorizontalLabel;
        private string _labelText;

        public StartPagePlayNextViewModel(PlayNext plugin)
        {
            _plugin = plugin;
            UpdateLabelDisplay(_plugin.LoadPluginSettings<PlayNextSettings>());
        }

        public bool ShowVerticalLabel
        {
            get => _showVerticalLabel;
            set => SetValue(ref _showVerticalLabel, value);
        }

        public bool ShowHorizontalLabel
        {
            get => _showHorizontalLabel;
            set => SetValue(ref _showHorizontalLabel, value);
        }

        public string LabelText
        {
            get => _labelText;
            set => SetValue(ref _labelText, value);
        }

        public void LoadData(ICollection<GameToPlayViewModel> games, PlayNextSettings settings)
        {
            new Task(() =>
            {
                try
                {
                    var numberOfGames = settings.NumberOfTopGames;

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

        public void UpdateGame(Game game)
        {
            var gameToUpdate = Games.FirstOrDefault(x => x.Id == game.Id);
            if (gameToUpdate == null)
            {
                return;
            }

            var index = Games.IndexOf(gameToUpdate);
            var newGame = new GameToPlayViewModel(_plugin, game, gameToUpdate.Score);
            var newGames = new ObservableCollection<GameToPlayViewModel>(Games);
            newGames.RemoveAt(index);
            newGames.Insert(index, newGame);
            Games = newGames;
        }

        public void UpdateLabelDisplay(PlayNextSettings settings)
        {
            ShowVerticalLabel = !settings.StartPageLabelIsHorizontal;
            ShowHorizontalLabel = settings.StartPageLabelIsHorizontal;
            LabelText = settings.StartPageShowLabel ? ResourceProvider.GetString("LOC_PlayNext_PluginName") : string.Empty;
        }
    }
}