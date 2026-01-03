using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PlayNext.ViewModels
{
	public class CreatePresetViewModel : ObservableObject
	{
		private readonly Action<string> _createAction;

		private Window _window;
		private string _name;


		public CreatePresetViewModel(Action<string> createAction)
		{
			_createAction = createAction;
		}

		public void AssociateWindow(Window window)
		{
			_window = window;
		}

		public string Name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}

		public ICommand CreateCommand => new RelayCommand(() =>
		{
			_createAction(Name);
			_window?.Close();
		});

		public ICommand CancelCommand => new RelayCommand(() =>
		{
			_window?.Close();
		});
	}
}