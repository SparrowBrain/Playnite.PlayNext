using PlayNext.Model.Data;
using PlayNext.Model.Score;
using PlayNext.Settings;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlayNext.ViewModels
{
	public class PlayNextMainViewModel : ObservableObject
	{
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly PlayNext _plugin;

		private ObservableCollection<GameToPlayViewModel> _games = new ObservableCollection<GameToPlayViewModel>();
		private ShowcaseType _activeShowcaseType;
		private int _numberOfGames = 30;
		private bool _isRefreshAvailable;

		public PlayNextMainViewModel(PlayNext plugin)
		{
			_plugin = plugin;
			var settings = _plugin.LoadPluginSettings<PlayNextSettings>();
			SetIsRefreshAvailable(settings);
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

		public bool IsRefreshAvailable
		{
			get => _isRefreshAvailable;
			set => SetValue(ref _isRefreshAvailable, value);
		}

		public ICommand Refresh => new RelayCommand(() => { _plugin.RefreshPlayNextData(); });

		// ReSharper disable once UnusedMember.Global
		public ICommand SwitchToCovers => new RelayCommand(() => { ActiveShowcaseType = ShowcaseType.Covers; });

		// ReSharper disable once UnusedMember.Global
		public ICommand SwitchToList => new RelayCommand(() => { ActiveShowcaseType = ShowcaseType.List; });

		public void LoadData(ICollection<GameToPlayViewModel> games, PlayNextSettings settings)
		{
			new Task(() =>
			{
				try
				{
					_numberOfGames = settings.NumberOfTopGames;

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

		public void SetIsRefreshAvailable(PlayNextSettings settings)
		{
			IsRefreshAvailable = settings.RandomWeight > 0;
		}
	}
}