using System.Collections.Generic;
using PlayNext.ViewModels;

namespace PlayNext.Extensions.StartPage.Settings.PlayNextAddon
{
	public class StartPageSettingsViewModel : ObservableObject
	{
		private readonly PlayNextSettingsViewModel _mainSettingsViewModel;


		public StartPageSettingsViewModel(PlayNextSettingsViewModel mainSettingsViewModel)
		{
			_mainSettingsViewModel = mainSettingsViewModel;
		}

		public bool StartPageShowLabel
		{
			get => _mainSettingsViewModel.Settings.StartPageShowLabel;
			set
			{
				if (value == _mainSettingsViewModel.Settings.StartPageShowLabel)
				{
					return;
				}
				
				_mainSettingsViewModel.BeginEdit();
				_mainSettingsViewModel.Settings.StartPageShowLabel = value;
				OnPropertyChanged();
				_mainSettingsViewModel.EndEdit();
			}
		}

		public bool StartPageLabelIsHorizontal
		{
			get => _mainSettingsViewModel.Settings.StartPageLabelIsHorizontal;
			set
			{
				if (value == _mainSettingsViewModel.Settings.StartPageLabelIsHorizontal)
				{
					return;
				}

				_mainSettingsViewModel.BeginEdit();
				_mainSettingsViewModel.Settings.StartPageLabelIsHorizontal = value;
				OnPropertyChanged();
				_mainSettingsViewModel.EndEdit();
			}
		}

		public int StartPageMinCoverCount
		{
			get => _mainSettingsViewModel.Settings.StartPageMinCoverCount;
			set
			{
				if (value == _mainSettingsViewModel.Settings.StartPageMinCoverCount)
				{
					return;
				}

				_mainSettingsViewModel.BeginEdit();
				_mainSettingsViewModel.Settings.StartPageMinCoverCount = value;
				OnPropertyChanged();
				_mainSettingsViewModel.EndEdit();
			}
			}


	}
}