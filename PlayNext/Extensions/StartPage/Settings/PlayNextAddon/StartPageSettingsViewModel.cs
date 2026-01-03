using System;
using System.Collections.Generic;
using PlayNext.Settings;

namespace PlayNext.Extensions.StartPage.Settings.PlayNextAddon
{
	public class StartPageSettingsViewModel : ObservableObject
	{
		private readonly PlayNextSettings _settings;
		private readonly Action<PlayNextSettings> _saveSettingsAction;

		public StartPageSettingsViewModel(PlayNextSettings settings, Action<PlayNextSettings> saveSettingsAction)
		{
			_settings = settings;
			_saveSettingsAction = saveSettingsAction;
		}

		public bool StartPageShowLabel
		{
			get => _settings.StartPageShowLabel;
			set
			{
				if (value == _settings.StartPageShowLabel)
				{
					return;
				}

				_settings.StartPageShowLabel = value;
				OnPropertyChanged();
				_saveSettingsAction(_settings);
			}
		}

		public bool StartPageLabelIsHorizontal
		{
			get => _settings.StartPageLabelIsHorizontal;
			set
			{
				if (value == _settings.StartPageLabelIsHorizontal)
				{
					return;
				}

				_settings.StartPageLabelIsHorizontal = value;
				OnPropertyChanged();
				_saveSettingsAction(_settings);
			}
		}

		public int StartPageMinCoverCount
		{
			get => _settings.StartPageMinCoverCount;
			set
			{
				if (value == _settings.StartPageMinCoverCount)
				{
					return;
				}

				_settings.StartPageMinCoverCount = value;
				OnPropertyChanged();
				_saveSettingsAction(_settings);
			}
		}
	}
}