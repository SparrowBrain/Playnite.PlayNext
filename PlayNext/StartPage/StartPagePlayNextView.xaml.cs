using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PlayNext.ViewModels;
using Playnite.SDK;

namespace PlayNext.StartPage
{
    /// <summary>
    /// Interaction logic for StartPagePlayNextView.xaml
    /// </summary>
    public partial class StartPagePlayNextView : UserControl
    {
        private ILogger _logger = LogManager.GetLogger(nameof(StartPagePlayNextView));

        public StartPagePlayNextView(StartPagePlayNextViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.DataContext is GameToPlayViewModel model)
                {
                    model.OpenDetails.Execute(null);
                }
            }
        }

        private void Dock_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dock = sender as FrameworkElement;
            var column = FindName("CoverListWidth") as ColumnDefinition;

            if (dock == null || column == null)
            {
                _logger.Warn("Dock or column is invalid");
                return;
            }

            var coverWidth = LandingPageExtension.Instance.Settings.MaxCoverWidth;

            var textHeight = 2 * 25;
            var coverMargin = 2 * 8;
            var newWidth = (Math.Floor((dock.ActualWidth - textHeight) / (coverWidth + coverMargin)) * (coverWidth + coverMargin));

            column.Width = new GridLength(newWidth, GridUnitType.Pixel);
        }

        private void OnCoverListMouseWheel(object sender, MouseWheelEventArgs e)
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