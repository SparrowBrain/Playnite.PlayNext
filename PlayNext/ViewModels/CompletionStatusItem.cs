using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.ViewModels
{
    public class CompletionStatusItem : ObservableObject
    {
        private readonly PlayNextSettingsViewModel _viewModel;
        private bool _isChecked;

        public CompletionStatusItem(Guid id, string name, PlayNextSettingsViewModel viewModel, bool isChecked)
        {
            Id = id;
            Name = name;
            _viewModel = viewModel;
            _isChecked = isChecked;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetValue(ref _isChecked, value);
                _viewModel.Settings.UnplayedCompletionStatuses = _viewModel.UnplayedCompletionStatuses
                    .Where(x => x.IsChecked)
                    .Select(x => x.Id)
                    .ToArray();
            }
        }
    }
}