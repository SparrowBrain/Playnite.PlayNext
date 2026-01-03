using PlayNext.ViewModels;
using Playnite.SDK.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayNext.Views
{
	/// <summary>
	/// Interaction logic for CreatePresetView.xaml
	/// </summary>
	public partial class CreatePresetView : PluginUserControl
	{
		public CreatePresetView(CreatePresetViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
