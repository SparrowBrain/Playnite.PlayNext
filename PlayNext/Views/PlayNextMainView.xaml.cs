using System;
using PlayNext.ViewModels;
using Playnite.SDK.Controls;

namespace PlayNext.Views
{
    /// <summary>
    /// Interaction logic for PlayNextMainView.xaml
    /// </summary>
    public partial class PlayNextMainView : PluginUserControl
    {
        private readonly PlayNextMainViewModel _mainViewModel;

        public PlayNextMainView(PlayNextMainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            DataContext = mainViewModel;

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _mainViewModel.LoadData();
        }
    }
}