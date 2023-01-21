using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace PlayNext
{
    public class PlayNextSettings : ObservableObject
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        private string _option1 = string.Empty;
        private bool _option2 = false;
        private bool _optionThatWontBeSaved = false;
        private float _totalPlaytime;
        private float _recentPlaytime;
        private float _recentOrder;

        public string Option1 { get => _option1; set => SetValue(ref _option1, value); }
        public bool Option2 { get => _option2; set => SetValue(ref _option2, value); }

        public float TotalPlaytime
        {
            get => _totalPlaytime;
            set
            {
                var difference = (value - _totalPlaytime) / 2;
                _totalPlaytime = value;
                _recentPlaytime -= difference;
                _recentOrder = 100 - _totalPlaytime - _recentPlaytime;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecentPlaytime));
                OnPropertyChanged(nameof(RecentOrder));
            }
        }

        public float RecentPlaytime
        {
            get => _recentPlaytime;
            set
            {
                var difference = (value - _recentPlaytime) / 2;
                _recentPlaytime = value;
                _totalPlaytime -= difference;
                _recentOrder = 100 - _totalPlaytime - _recentPlaytime;
                OnPropertyChanged(nameof(TotalPlaytime));
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecentOrder));
            }
        }

        public float RecentOrder
        {
            get => _recentOrder;
            set
            {
                var difference = (value - _recentOrder) / 2;
                _recentOrder = value;
                _totalPlaytime -= difference;
                _recentPlaytime = 100 - _totalPlaytime - _recentOrder;
                OnPropertyChanged(nameof(TotalPlaytime));
                OnPropertyChanged(nameof(RecentPlaytime));
                OnPropertyChanged();
            }
        }

        private void RebalanceWeights(ref float caller)
        {
            //new 
        }

        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonDontSerialize` ignore attribute.
        [DontSerialize]
        public bool OptionThatWontBeSaved { get => _optionThatWontBeSaved; set => SetValue(ref _optionThatWontBeSaved, value); }
    }
}