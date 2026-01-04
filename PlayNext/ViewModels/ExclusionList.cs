using PlayNext.Settings;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PlayNext.ViewModels
{
	public class ExclusionList<T> : ObservableObject where T : DatabaseObject
	{
		private readonly Func<List<T>> _getAll;
		private readonly Func<PlayNextSettings, HashSet<Guid>> _settingsSelector;
		private PlayNextSettings _settings;
		private ObservableCollection<T> _allowedItems = new ObservableCollection<T>();
		private ObservableCollection<T> _excludedItems = new ObservableCollection<T>();
		private T _selectedAllowedItem;
		private T _selectedExcludedItem;
		private string _allowedItemsFilter;
		private string _excludedItemsFilter;

		public ExclusionList(Func<List<T>> getAll, Func<PlayNextSettings, HashSet<Guid>> settingsSelector)
		{
			_getAll = getAll;
			_settingsSelector = settingsSelector;
		}

		public PlayNextSettings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				Initialize();
			}
		}

		private void Initialize()
		{
			var allItems = _getAll.Invoke();

			ExcludedItems = allItems
				.Where(x => _settingsSelector(Settings).Contains(x.Id)
							&& (string.IsNullOrEmpty(ExcludedItemsFilter) || x.Name.ToLower().Contains(ExcludedItemsFilter.ToLower())))
				.OrderBy(x => x.Name)
				.ToObservable();

			AllowedItems = allItems.Where(x => !_settingsSelector(Settings).Contains(x.Id)
											 && (string.IsNullOrEmpty(AllowedItemsFilter) || x.Name.ToLower().Contains(AllowedItemsFilter.ToLower())))
				.OrderBy(x => x.Name)
				.ToObservable();
		}

		public ObservableCollection<T> AllowedItems
		{
			get => _allowedItems;
			set => SetValue(ref _allowedItems, value);
		}

		public T SelectedAllowedItem
		{
			get => _selectedAllowedItem;
			set => SetValue(ref _selectedAllowedItem, value);
		}

		public string AllowedItemsFilter
		{
			get => _allowedItemsFilter;
			set
			{
				SetValue(ref _allowedItemsFilter, value);
				Initialize();
			}
		}

		public ICommand ClearAllowedItemsFilter => new RelayCommand(() => AllowedItemsFilter = string.Empty);

		public ObservableCollection<T> ExcludedItems
		{
			get => _excludedItems;
			set => SetValue(ref _excludedItems, value);
		}

		public T SelectedExcludedItem
		{
			get => _selectedExcludedItem;
			set => SetValue(ref _selectedExcludedItem, value);
		}

		public string ExcludedItemsFilter
		{
			get => _excludedItemsFilter;
			set
			{
				SetValue(ref _excludedItemsFilter, value);
				Initialize();
			}
		}

		public ICommand ClearExcludedItemsFilter => new RelayCommand(() => ExcludedItemsFilter = string.Empty);

		public ICommand ExcludeItemCommand => new RelayCommand(() =>
		{
			if (SelectedAllowedItem != null)
			{
				_settingsSelector(Settings).Add(SelectedAllowedItem.Id);
				Initialize();
				SelectedAllowedItem = null;
			}
		});

		public ICommand AllowItemCommand => new RelayCommand(() =>
		{
			if (SelectedExcludedItem != null)
			{
				_settingsSelector(Settings).Remove(SelectedExcludedItem.Id);
				Initialize();
				SelectedExcludedItem = null;
			}
		});
	}
}