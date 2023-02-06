using System;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void OnCoversListBoxMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is VirtualizingStackPanel virtualizingStackPanel))
            {
                return;
            }

            for (var i = 0; i < Math.Abs(e.Delta); i++)
            {
                if (e.Delta < 0)
                {
                    virtualizingStackPanel.LineRight();
                }
                else
                {
                    virtualizingStackPanel.LineLeft();
                }
            }
        }
    }
}