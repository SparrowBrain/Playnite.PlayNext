using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PlayNext.ViewModels;
using Playnite.SDK;
using Playnite.SDK.Controls;

namespace PlayNext.Views
{
    /// <summary>
    /// Interaction logic for PlayNextMainView.xaml
    /// </summary>
    public partial class PlayNextMainView : PluginUserControl
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(PlayNextMainView));
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
            if (!(sender is IScrollInfo scrollInfo))
            {
                _logger.Warn("Mouse wheel scroll triggered from non-scrolling control.");
                return;
            }

            for (var i = 0; i < Math.Abs(e.Delta); i++)
            {
                if (e.Delta < 0)
                {
                    scrollInfo.LineRight();
                }
                else
                {
                    scrollInfo.LineLeft();
                }
            }
        }
    }
}