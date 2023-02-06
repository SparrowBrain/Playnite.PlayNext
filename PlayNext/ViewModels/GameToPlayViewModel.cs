using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.ViewModels
{
    public class GameToPlayViewModel
    {
        private readonly PlayNext _plugin;
        private readonly string _defaultIconPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "icon.png");

        public GameToPlayViewModel(PlayNext plugin, Game game, float score)
        {
            _plugin = plugin;
            Id = game.Id;
            Name = game.Name;
            Score = score;
            Icon = game.Icon != null ? _plugin.PlayniteApi.Database.GetFullFilePath(game.Icon) : _defaultIconPath;
            CoverImage = game.CoverImage != null ? _plugin.PlayniteApi.Database.GetFullFilePath(game.CoverImage) : _defaultIconPath;
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