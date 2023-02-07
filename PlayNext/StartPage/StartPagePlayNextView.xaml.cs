using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using PlayNext.ViewModels;

namespace PlayNext.StartPage
{
    /// <summary>
    /// Interaction logic for StartPagePlayNextView.xaml
    /// </summary>
    public partial class StartPagePlayNextView : UserControl
    {
        public StartPagePlayNextView(StartPagePlayNextViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            (DataContext as StartPagePlayNextViewModel)?.LoadData();
        }

        static readonly Random rng = new Random();

        private void Description_Closed(object sender, EventArgs e)
        {
            //if (rng.NextDouble() <= 0.25)
            //{
            //    GC.Collect();
            //}
        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.DataContext is GameToPlayViewModel model)
                {
                    //model.OpenCommand?.Execute(null);
                }
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.DataContext is GameToPlayViewModel model)
                {
                    //model.StartCommand?.Execute(null);
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bt && bt.DataContext is GameToPlayViewModel game)
            {
                //game.StartCommand?.Execute(null);
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bt && bt.DataContext is GameToPlayViewModel game)
            {
                //game.OpenCommand?.Execute(null);
            }
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (sender is FrameworkElement element && element.DataContext is GameToPlayViewModel model)
            //{
            //    if (DataContext is LandingPageViewModel viewModel)
            //    {
            //        viewModel.CurrentlyHoveredGame = model.Game;
            //    }
            //}
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (sender is FrameworkElement element && element.DataContext is GameToPlayViewModel model)
            //{
            //    if (DataContext is LandingPageViewModel viewModel)
            //    {
            //        viewModel.CurrentlyHoveredGame = null;
            //    }
            //}
        }
    }
}