using System.Windows.Controls;

namespace PlayNext.Extensions.StartPage.Settings.PlayNextAddon
{
	/// <summary>
	/// Interaction logic for StartPageSettingsView.xaml
	/// </summary>
	public partial class StartPageSettingsView : UserControl
	{
		public StartPageSettingsView(StartPageSettingsViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}