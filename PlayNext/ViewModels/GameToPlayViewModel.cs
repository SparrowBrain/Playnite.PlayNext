using System;
using System.Windows.Input;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.ViewModels
{
    public class GameToPlayViewModel
    {
        private readonly PlayNext _plugin;

        public GameToPlayViewModel(PlayNext plugin, Game game, float score)
        {
            _plugin = plugin;

            Id = game.Id;
            Name = game.Name;
            Score = score;
            Icon = game.Icon;
            CoverImage = game.CoverImage;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Score { get; }

        public string CoverImage { get; set; }

        public string Icon { get; set; }

        public ICommand OpenDetails
        {
            get => new RelayCommand(() =>
            {
                _plugin.PlayniteApi.MainView.SelectGame(Id);
                _plugin.PlayniteApi.MainView.SwitchToLibraryView();
            });
        }
    }
}