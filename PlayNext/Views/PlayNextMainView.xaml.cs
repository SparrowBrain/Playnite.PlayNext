using PlayNext.ViewModels;
using Playnite.SDK.Controls;

namespace PlayNext.Views
{
    /// <summary>
    /// Interaction logic for PlayNextMainView.xaml
    /// </summary>
    public partial class PlayNextMainView : PluginUserControl
    {
        public PlayNextMainView(PlayNextMainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}