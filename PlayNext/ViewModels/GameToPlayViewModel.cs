using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.ViewModels
{
    public class GameToPlayViewModel
    {
        private readonly PlayNext _plugin;
        private static readonly string DefaultIconPath;
        private static readonly string DefaultCoverImagePath;

        static GameToPlayViewModel()
        {
            DefaultCoverImagePath = Application.Current.Resources["DefaultGameCover"] is BitmapImage image
                ? image.UriSource.AbsolutePath
                : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "icon.png");

            DefaultIconPath = Application.Current.Resources["DefaultGameIcon"] is BitmapImage icon
                ? icon.UriSource.AbsolutePath
                : Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "icon.png");
        }

        public GameToPlayViewModel(PlayNext plugin, Game game, float score)
        {
            _plugin = plugin;
            Id = game.Id;
            Name = game.Name;
            Score = score;
            Icon = game.Icon != null ? _plugin.PlayniteApi.Database.GetFullFilePath(game.Icon) : DefaultIconPath;

            CoverImage = game.CoverImage != null ? _plugin.PlayniteApi.Database.GetFullFilePath(game.CoverImage) : DefaultCoverImagePath;
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