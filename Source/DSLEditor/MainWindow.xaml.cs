using System.Windows;

namespace GridDominance.DSLEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var vm = new MainWindowViewModel();
			DataContext = vm;
			InitializeComponent();
			vm.TabCtrl = ContentTabCtrl;

			vm.TimerCountDown = 2;
		}
	}
}
