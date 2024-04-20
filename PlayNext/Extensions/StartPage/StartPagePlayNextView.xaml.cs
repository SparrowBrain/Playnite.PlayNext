using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PlayNext.Settings;
using PlayNext.ViewModels;
using Playnite.SDK;

namespace PlayNext.Extensions.StartPage
{
    /// <summary>
    /// Interaction logic for StartPagePlayNextView.xaml
    /// </summary>
    public partial class StartPagePlayNextView : UserControl
    {
        private const int TextHeight = 2 * 25;
        private const int CoverMargin = 2 * 8;
        private ILogger _logger = LogManager.GetLogger(nameof(StartPagePlayNextView));
        private int _minCoverCount;

        public StartPagePlayNextView(StartPagePlayNextViewModel viewModel, PlayNextSettings settings)
        {
            DataContext = viewModel;
            UpdateMinCoverCount(settings);
            InitializeComponent();
        }

        public void UpdateMinCoverCount(PlayNextSettings settings)
        {
            _minCoverCount = settings.StartPageMinCoverCount;
            UpdateCoversColumnWidth();
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
            UpdateCoversColumnWidth(dock);
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

        private void UpdateCoversColumnWidth()
        {
            var dock = FindName("CoversDock") as FrameworkElement;
            UpdateCoversColumnWidth(dock);
        }

        private void UpdateCoversColumnWidth(FrameworkElement dock)
        {
            var column = FindName("CoverListWidth") as ColumnDefinition;
            if (dock == null || column == null)
            {
                _logger.Warn("Dock or column is invalid");
                return;
            }

            var coverWidth = LandingPageExtension.Instance.Settings.MaxCoverWidth;

            var dynamicWidth = (Math.Floor((dock.ActualWidth - TextHeight) / (coverWidth + CoverMargin)) * (coverWidth + CoverMargin));
            var minWidth = _minCoverCount * (coverWidth + CoverMargin);

            var newWidth = Math.Max(minWidth, dynamicWidth);
            if (newWidth <= 0)
            {
                return;
            }

            column.Width = new GridLength(newWidth, GridUnitType.Pixel);
        }
    }
}